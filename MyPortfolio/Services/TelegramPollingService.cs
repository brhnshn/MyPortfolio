using System.Text.Json;

namespace MyPortfolio.Services
{
    /// <summary>
    /// Telegram Bot API'den long-polling ile dinler.
    /// Hem buton tıklamalarını (callback_query) hem de komutları (/link) algılar.
    /// </summary>
    public class TelegramPollingService : BackgroundService
    {
        private readonly TelegramService _telegramService;
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TelegramPollingService> _logger;
        private int _lastUpdateId = 0;

        public TelegramPollingService(TelegramService telegramService, IConfiguration configuration, ILogger<TelegramPollingService> logger)
        {
            _telegramService = telegramService;
            _logger = logger;
            _botToken = configuration["TelegramSettings:BotToken"] ?? "";
            _chatId = configuration["TelegramSettings:ChatId"] ?? "";
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(35) };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(_botToken) || _botToken.Contains("__TELEGRAM_BOT_TOKEN__"))
            {
                _logger.LogError("TelegramBotToken is missing or has a placeholder value. TelegramPollingService will NOT run.");
                return;
            }

            // Bot komutlarını Telegram'a kaydet
            await _telegramService.RegisterBotCommandsAsync();

            _logger.LogInformation("TelegramPollingService started successfully.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // callback_query ve message güncellemelerini iste
                    var url = $"https://api.telegram.org/bot{_botToken}/getUpdates?offset={_lastUpdateId + 1}&timeout=30&allowed_updates=[\"message\",\"callback_query\"]";
                    var response = await _httpClient.GetStringAsync(url, stoppingToken);

                    var doc = JsonSerializer.Deserialize<JsonElement>(response);
                    if (doc.GetProperty("ok").GetBoolean() && doc.TryGetProperty("result", out var results))
                    {
                        foreach (var update in results.EnumerateArray())
                        {
                            _lastUpdateId = update.GetProperty("update_id").GetInt32();

                            // 1. Buton tıklamaları işlenir
                            if (update.TryGetProperty("callback_query", out var callbackQuery))
                            {
                                var callbackQueryId = callbackQuery.GetProperty("id").GetString() ?? "";
                                var data = callbackQuery.GetProperty("data").GetString() ?? "";

                                await _telegramService.ProcessCallbackAsync(callbackQueryId, data);
                            }

                            // 2. Mesajlar / komutlar işlenir
                            else if (update.TryGetProperty("message", out var message))
                            {
                                if (message.TryGetProperty("text", out var textElem))
                                {
                                    var text = textElem.GetString()?.Trim() ?? "";

                                    // Sadece yetkili kişinin (AdminSettings:ChatId) mesajlarını dinle
                                    var fromChatId = message.GetProperty("chat").GetProperty("id").GetInt64().ToString();

                                    if (fromChatId == _chatId.Trim())
                                    {
                                        if (text.Equals("/link", StringComparison.OrdinalIgnoreCase))
                                        {
                                            await _telegramService.GenerateDynamicLoginLinkAsync();
                                        }
                                        else if (text.Equals("/kickall", StringComparison.OrdinalIgnoreCase))
                                        {
                                            await _telegramService.KickAllAdminsAsync();
                                        }
                                    }
                                    else
                                    {
                                        // Yetkisiz kullanıcı botu keşfetti ve komut gönderdi!
                                        // İsteğe bağlı olarak asıl admine "Botunuzu biri buldu!" diye uyarı atılabilir
                                        await _telegramService.SendMessageAsync($"⚠️ *Yetkisiz Bot Erişimi Denemesi*\n\nKimliği belirsiz bir Telegram hesabı bota mesaj gönderdi.\nGönderilen mesaj: `{text}`\nChat ID: `{fromChatId}`");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("TelegramPollingService execution is being cancelled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the Telegram polling loop.");
                    await Task.Delay(3000, stoppingToken);
                }
            }
        }
    }
}

