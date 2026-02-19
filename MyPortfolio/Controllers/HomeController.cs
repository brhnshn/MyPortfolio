using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGenericRepository<Message> _messageRepository;
        private static readonly Dictionary<string, DateTime> _lastSubmission = new();
        private const int RateLimitSeconds = 30;

        public HomeController(IGenericRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult SendMessage(Message p, string website)
        {
            // Honeypot - botlar bu gizli alani doldurur
            if (!string.IsNullOrEmpty(website))
            {
                return RedirectToAction("Index");
            }

            // Rate Limiting - ayni IP'den 30 saniyede bir mesaj
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (_lastSubmission.TryGetValue(ip, out var lastTime) && (DateTime.Now - lastTime).TotalSeconds < RateLimitSeconds)
            {
                TempData["Error"] = "Cok sik mesaj gonderiyorsunuz. Lutfen biraz bekleyin.";
                return RedirectToAction("Index");
            }

            // Model dogrulama
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            p.Date = DateTime.Now;
            p.IsRead = false;

            if (string.IsNullOrEmpty(p.Subject))
            {
                p.Subject = "Iletisim Formu Mesaji";
            }

            _messageRepository.Insert(p);

            // Rate limit kaydini guncelle
            _lastSubmission[ip] = DateTime.Now;

            TempData["Success"] = "Mesajiniz basariyla gonderildi!";
            return RedirectToAction("Index");
        }
    }
}
