namespace MyPortfolio.Middleware
{
    using MyPortfolio.Services;

    /// <summary>
    /// /Admin rotasına gelen istekleri korur:
    /// - Honeypot Trap: Yetkisiz /admin isteklerini yakalar ve loglar/Telegram'a bildirir.
    /// - Dinamik Login: Sadece Telegram'dan alınan geçerli geçici token'larla girişe izin verir.
    /// - Session Hijacking Koruması: Her istekte IP + User-Agent değişimi kontrol eder.
    /// - Brute-force: IP bazlı başarısız giriş sınırlaması.
    /// </summary>
    public class AdminShieldMiddleware
    {
        private readonly RequestDelegate _next;

        // --- Brute-force ve Honeypot takibi ---
        private static readonly Dictionary<string, (int FailCount, DateTime BlockedUntil, int HoneypotHits)> _ipStats = new();
        private static readonly object _lock = new();
        private const int MaxAttempts = 5;
        private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(15);

        // --- Global Logout ---
        public static DateTime LastKickAllTime { get; set; } = DateTime.MinValue;

        public AdminShieldMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TelegramService telegramService)
        {
            var path = context.Request.Path.Value ?? "";

            // X-Forwarded-For desteği
            var clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                           ?? context.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                           ?? "unknown";

            // Sadece /Admin rotalarını kontrol et
            if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
            {
                var user = context.User;

                // 1. GİRİŞ YAPMIŞ KULLANICI İÇİN SESSION HIJACKING KONTROLÜ
                if (user?.Identity?.IsAuthenticated == true && user.IsInRole("Admin"))
                {
                    // Çikiş işlemiyse (Logout) session kontrolüne takılma
                    if (path.Equals("/Admin/Login/LogOut", StringComparison.OrdinalIgnoreCase))
                    {
                        await _next(context);
                        return;
                    }

                    var sessionIp = context.Session.GetString("AdminIP");
                    var sessionUa = context.Session.GetString("AdminUA");
                    var currentUa = context.Request.Headers["User-Agent"].ToString();

                    // IP veya User-Agent değiştiyse oturumu sonlandır
                    if (sessionIp != null && (sessionIp != clientIp || sessionUa != currentUa))
                    {
                        context.Response.Redirect("/Admin/Login/LogOut");
                        return;
                    }

                    // Kickall tetiklenmiş mi kontrol et
                    var loginTimeStr = context.Session.GetString("LoginTime");
                    if (string.IsNullOrEmpty(loginTimeStr) ||
                        (long.TryParse(loginTimeStr, out var ticks) && new DateTime(ticks, DateTimeKind.Utc) < LastKickAllTime))
                    {
                        context.Response.Redirect("/Admin/Login/LogOut");
                        return;
                    }

                    await _next(context);
                    return;
                }

                // BLOKE KONTROLÜ (Brute-force)
                if (IsBlocked(clientIp))
                {
                    context.Response.Redirect("/Error/404");
                    return;
                }

                // 2. GİRİŞ SAYFASINA ERİŞİM KONTROLÜ
                if (path.StartsWith("/Admin/Login", StringComparison.OrdinalIgnoreCase))
                {
                    // Şifremi Unuttum vb. sayfalara /Admin/Login/* üzerinden erişiliyor olabilir
                    // Login.Index POST/GET dışındakileri serbest bırakabiliriz ama biz ana Gate'i kapattık

                    // A) Anahtarla geliyorsa (Dinamik Link veya Statik)
                    var key = context.Request.Query["key"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(key) && telegramService.ValidateDynamicToken(key))
                    {
                        // Geçerli bir dinamik token! 
                        // Token'ı geçersiz kıl ki tek kullanımlık olsun (opsiyonel, şu an süre bazlı)
                        // telegramService.InvalidateDynamicToken(key);

                        SetGateCookie(context);
                        await _next(context);
                        return;
                    }

                    // B) Gate Çerezi varsa form POST/GET işlemine devam etsin
                    if (context.Request.Cookies.ContainsKey("AdminGate"))
                    {
                        // Başarısız giriş takibi
                        context.Response.OnCompleted(() =>
                        {
                            if (context.Request.Method == "POST" && context.Response.StatusCode != 302)
                                RecordFailedAttempt(clientIp);
                            else if (context.Request.Method == "POST" && context.Response.StatusCode == 302)
                                ResetAttempts(clientIp);

                            return Task.CompletedTask;
                        });

                        await _next(context);
                        return;
                    }

                    // C) Ne anahtar ne çerez var → yetkisiz /Admin/Login isteği
                    // Honeypot olarak say (yetkisizler login'i bulmaya çalışıyor)
                    HandleHoneypotHit(clientIp, telegramService);
                    context.Response.Redirect("/Error/404");
                    return;
                }

                // 3. DİĞER TÜM /ADMIN ROTALARI (Yetkisiz)
                // Honeypot Hit (örneğin botlar /admin/dashboard vs arıyor)
                HandleHoneypotHit(clientIp, telegramService);
                context.Response.Redirect("/Error/404");
                return;
            }

            await _next(context);
        }

        private void SetGateCookie(HttpContext context)
        {
            context.Response.Cookies.Append("AdminGate", "1", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(15)
            });
        }

        // --- Korumalar ---

        private void HandleHoneypotHit(string ip, TelegramService telegramService)
        {
            int hits = 0;
            lock (_lock)
            {
                if (_ipStats.TryGetValue(ip, out var data))
                {
                    hits = data.HoneypotHits + 1;
                    _ipStats[ip] = (data.FailCount, data.BlockedUntil, hits);
                }
                else
                {
                    hits = 1;
                    _ipStats[ip] = (0, DateTime.MinValue, hits);
                }
            }

            // Her 10 honeypot vuruşunda bir Telegram'a uyar ki botlar spam yapıp Telegram limitlerini doldurmasın
            if (hits == 1 || hits % 10 == 0)
            {
                // Task.Run ile asenkron arka planda gönder, middleware'i bloklama
                _ = Task.Run(() => telegramService.SendHoneypotAlertAsync(ip, hits));
            }
        }

        private bool IsBlocked(string ip)
        {
            lock (_lock)
            {
                if (_ipStats.TryGetValue(ip, out var data))
                {
                    if (DateTime.UtcNow < data.BlockedUntil) return true;
                    if (DateTime.UtcNow >= data.BlockedUntil && data.FailCount >= MaxAttempts)
                        _ipStats.Remove(ip);
                }
                return false;
            }
        }

        private void RecordFailedAttempt(string ip)
        {
            lock (_lock)
            {
                var expired = _ipStats.Where(x => DateTime.UtcNow >= x.Value.BlockedUntil && x.Value.FailCount >= MaxAttempts).Select(x => x.Key).ToList();
                foreach (var key in expired) _ipStats.Remove(key);

                if (_ipStats.TryGetValue(ip, out var data))
                {
                    var count = data.FailCount + 1;
                    _ipStats[ip] = count >= MaxAttempts ? (count, DateTime.UtcNow.Add(BlockDuration), data.HoneypotHits) : (count, DateTime.MinValue, data.HoneypotHits);
                }
                else
                {
                    _ipStats[ip] = (1, DateTime.MinValue, 0);
                }
            }
        }

        private void ResetAttempts(string ip)
        {
            lock (_lock)
            {
                if (_ipStats.TryGetValue(ip, out var data))
                {
                    _ipStats[ip] = (0, DateTime.MinValue, data.HoneypotHits);
                }
            }
        }
    }
}
