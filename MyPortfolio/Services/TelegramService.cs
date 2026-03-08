using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace MyPortfolio.Services
{
    /// <summary>
    /// Telegram Bot API üzerinden 2FA, bildirim ve dinamik link yönetimi.
    /// </summary>
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly string _siteUrl;
        private readonly HttpClient _httpClient;

        // Bekleyen onay istekleri: requestId → (approved?, expiresAt)
        private static readonly ConcurrentDictionary<string, (bool? Approved, DateTime ExpiresAt)> _pendingApprovals = new();

        // Dinamik login linkleri: token → expiresAt
        private static readonly ConcurrentDictionary<string, DateTime> _dynamicTokens = new();

        public TelegramService(IConfiguration configuration)
        {
            _botToken = configuration["TelegramSettings:BotToken"] ?? "";
            _chatId = configuration["TelegramSettings:ChatId"] ?? "";
            _siteUrl = configuration["TelegramSettings:SiteUrl"] ?? "http://localhost:5096";
            _httpClient = new HttpClient();
        }

        // ===== DİNAMİK LİNK YÖNETİMİ =====

        public async Task RegisterBotCommandsAsync()
        {
            var commands = new[]
            {
                new { command = "link", description = "Gecici login linki olusturur (5 dk)" },
                new { command = "kickall", description = "Tum admin oturumlarini sonlandirir" }
            };

            var payload = new { commands };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                await _httpClient.PostAsync($"https://api.telegram.org/bot{_botToken}/setMyCommands", content);
            }
            catch
            {
                // Sessizce yut
            }
        }

        /// <summary>
        /// /link komutu ile çağrılır. 5 dakikalık geçici login linki oluşturur.
        /// </summary>
        public async Task GenerateDynamicLoginLinkAsync()
        {
            // Eski token'ları temizle
            CleanupDynamicTokens();

            var token = Guid.NewGuid().ToString("N");
            var expiresAt = DateTime.UtcNow.AddMinutes(5);
            _dynamicTokens[token] = expiresAt;

            var link = $"{_siteUrl}/Admin/Login?key={token}";
            var message = $"🔗 *Geçici Giriş Linki*\n\n" +
                          $"[Giriş Sayfasını Aç]({link})\n\n" +
                          $"👉 *Bağlantı:* `{link}`\n\n" +
                          $"⏳ _Bu link 5 dakika geçerlidir._\n" +
                          $"🕐 Son kullanma: `{DateTime.Now.AddMinutes(5):HH:mm:ss}`";

            await SendMessageAsync(message);
        }

        public async Task KickAllAdminsAsync()
        {
            MyPortfolio.Middleware.AdminShieldMiddleware.LastKickAllTime = DateTime.UtcNow;

            var message = $"🚨 *PANİK BUTONU TETİKLENDİ* 🚨\n\n" +
                          $"Tüm aktif yönetici oturumları sonlandırıldı. Paneli kullanan herkes şu an sistemden atıldı.\n\n" +
                          $"`Oturum sıfırlama saati: {DateTime.Now:HH:mm:ss}`";

            await SendMessageAsync(message);
        }

        /// <summary>
        /// Dinamik token geçerli mi kontrol eder.
        /// </summary>
        public bool ValidateDynamicToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;

            if (_dynamicTokens.TryGetValue(token, out var expiresAt))
            {
                if (DateTime.UtcNow <= expiresAt)
                    return true;

                // Süresi dolmuş → kaldır
                _dynamicTokens.TryRemove(token, out _);
            }
            return false;
        }

        /// <summary>
        /// Kullanılmış token'ı siler (tek kullanımlık yapma opsiyoneli).
        /// </summary>
        public void InvalidateDynamicToken(string token)
        {
            _dynamicTokens.TryRemove(token, out _);
        }

        private void CleanupDynamicTokens()
        {
            var expired = _dynamicTokens.Where(x => DateTime.UtcNow > x.Value).Select(x => x.Key).ToList();
            foreach (var key in expired) _dynamicTokens.TryRemove(key, out _);
        }

        // ===== GİRİŞ ONAY SİSTEMİ (2FA) =====

        /// <summary>
        /// Telegram'a onay mesajı gönderir. GeoIP bilgisiyle birlikte.
        /// </summary>
        public async Task<string> SendApprovalRequestAsync(string username, string ipAddress)
        {
            var requestId = Guid.NewGuid().ToString("N")[..12];
            var expiresAt = DateTime.UtcNow.AddMinutes(2);
            _pendingApprovals[requestId] = (null, expiresAt);

            var message = $"🔐 *Admin Giriş Onayı*\n\n" +
                          $"👤 Kullanıcı: `{username}`\n" +
                          $"🌐 IP: `{ipAddress}`\n" +
                          $"🕐 Zaman: `{DateTime.Now:dd.MM.yyyy HH:mm:ss}`\n\n" +
                          $"⏳ _2 dakika içinde onaylamanız gerekmektedir._";

            var inlineKeyboard = new
            {
                inline_keyboard = new[]
                {
                    new[]
                    {
                        new { text = "✅ Onayla", callback_data = $"approve_{requestId}" },
                        new { text = "❌ Reddet", callback_data = $"deny_{requestId}" }
                    }
                }
            };

            var payload = new
            {
                chat_id = _chatId,
                text = message,
                parse_mode = "Markdown",
                reply_markup = inlineKeyboard
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"https://api.telegram.org/bot{_botToken}/sendMessage", content);

            CleanupExpired();
            return requestId;
        }

        public async Task ProcessCallbackAsync(string callbackQueryId, string data)
        {
            string responseText;

            if (data.StartsWith("approve_"))
            {
                var requestId = data["approve_".Length..];
                if (_pendingApprovals.TryGetValue(requestId, out var entry) && entry.Approved == null)
                {
                    _pendingApprovals[requestId] = (true, entry.ExpiresAt);
                    responseText = "✅ Giriş onaylandı!";
                }
                else
                    responseText = "⚠️ Bu istek artık geçerli değil.";
            }
            else if (data.StartsWith("deny_"))
            {
                var requestId = data["deny_".Length..];
                if (_pendingApprovals.TryGetValue(requestId, out var entry) && entry.Approved == null)
                {
                    _pendingApprovals[requestId] = (false, entry.ExpiresAt);
                    responseText = "❌ Giriş reddedildi!";
                }
                else
                    responseText = "⚠️ Bu istek artık geçerli değil.";
            }
            else
            {
                responseText = "Bilinmeyen işlem.";
            }

            var payload = new { callback_query_id = callbackQueryId, text = responseText, show_alert = true };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"https://api.telegram.org/bot{_botToken}/answerCallbackQuery", content);
        }

        public bool? CheckApproval(string requestId)
        {
            if (_pendingApprovals.TryGetValue(requestId, out var entry))
            {
                if (DateTime.UtcNow > entry.ExpiresAt)
                {
                    _pendingApprovals.TryRemove(requestId, out _);
                    return false;
                }
                return entry.Approved;
            }
            return false;
        }

        public void RemoveApproval(string requestId)
        {
            _pendingApprovals.TryRemove(requestId, out _);
        }

        // ===== BİLDİRİMLER =====

        /// <summary>
        /// Çıkış bildirimi gönderir.
        /// </summary>
        public async Task SendLogoutNotificationAsync(string username, string ipAddress)
        {
            var message = $"🚪 *Admin Çıkış Bildirimi*\n\n" +
                          $"👤 Kullanıcı: `{username}`\n" +
                          $"🌐 IP: `{ipAddress}`\n" +
                          $"🕐 Zaman: `{DateTime.Now:dd.MM.yyyy HH:mm:ss}`";
            await SendMessageAsync(message);
        }

        /// <summary>
        /// Honeypot: Şüpheli tarama uyarısı gönderir.
        /// </summary>
        public async Task SendHoneypotAlertAsync(string ipAddress, int attemptCount)
        {
            var message = $"⚠️ *Şüpheli Tarama Tespit Edildi*\n\n" +
                          $"🌐 IP: `{ipAddress}`\n" +
                          $"🔢 Deneme sayısı: `{attemptCount}`\n" +
                          $"🕐 Zaman: `{DateTime.Now:dd.MM.yyyy HH:mm:ss}`\n\n" +
                          $"_Bu IP, /admin rotasına yetkisiz erişim denemesi yapıyor._";
            await SendMessageAsync(message);
        }

        // ===== YARDIMCI =====

        public async Task SendMessageAsync(string message)
        {
            var payload = new { chat_id = _chatId, text = message, parse_mode = "Markdown" };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"https://api.telegram.org/bot{_botToken}/sendMessage", content);
        }

        private void CleanupExpired()
        {
            var expiredKeys = _pendingApprovals.Where(x => DateTime.UtcNow > x.Value.ExpiresAt).Select(x => x.Key).ToList();
            foreach (var key in expiredKeys) _pendingApprovals.TryRemove(key, out _);
        }
    }
}
