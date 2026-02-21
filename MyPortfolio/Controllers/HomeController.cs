using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGenericRepository<Message> _messageRepository;
        private readonly IGenericRepository<SiteSettings> _siteSettingsRepo;
        private static readonly Dictionary<string, DateTime> _lastSubmission = new();
        private const int RateLimitSeconds = 30;

        public HomeController(IGenericRepository<Message> messageRepository, IGenericRepository<SiteSettings> siteSettingsRepo)
        {
            _messageRepository = messageRepository;
            _siteSettingsRepo = siteSettingsRepo;
        }

        public IActionResult Index()
        {
            var settings = _siteSettingsRepo.GetList().FirstOrDefault();
            ViewBag.LayoutMode = settings?.LayoutMode ?? "SinglePage";
            return View();
        }

        // Multi-Page bolum action'lari
        public IActionResult About() => View();
        public IActionResult Projects() => View();
        public IActionResult Skills() => View();
        public IActionResult Services() => View();
        public IActionResult Testimonials() => View();
        public IActionResult Contact() => View();


        public IActionResult RenderComponent(string componentName)
        {
            if (string.IsNullOrEmpty(componentName)) return BadRequest();
            return ViewComponent(componentName);
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
                TempData["Error"] = "Cok s�k mesaj gonderiyorsunuz. Lutfen biraz bekleyin.";
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

            // Eski kayitlari temizle (bellek sizintisi onleme)
            var staleKeys = _lastSubmission.Where(x => (DateTime.Now - x.Value).TotalMinutes > 5).Select(x => x.Key).ToList();
            foreach (var key in staleKeys) _lastSubmission.Remove(key);

            TempData["Success"] = "Mesajiniz basariyla gonderildi!";
            return RedirectToAction("Index");
        }
    }
}

