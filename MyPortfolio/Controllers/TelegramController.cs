using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers
{
    /// <summary>
    /// Telegram 2FA onay durumu sorgulama endpoint'i.
    /// Sadece AdminGate çerezi olan kullanıcılar erişebilir.
    /// </summary>
    [Route("api/telegram")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly TelegramService _telegramService;

        public TelegramController(TelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        /// <summary>
        /// AJAX polling endpoint — Login sayfasının onay durumunu sorgulaması için.
        /// AdminGate çerezi olmadan erişilemez.
        /// </summary>
        [HttpGet("check/{requestId}")]
        [IgnoreAntiforgeryToken]
        public IActionResult CheckApproval(string requestId)
        {
            // AdminGate çerezi kontrolü — sadece login sayfasından erişilebilir
            if (!Request.Cookies.ContainsKey("AdminGate"))
            {
                return NotFound();
            }

            var result = _telegramService.CheckApproval(requestId);

            return Ok(new
            {
                status = result == null ? "pending" : (result == true ? "approved" : "denied")
            });
        }
    }
}
