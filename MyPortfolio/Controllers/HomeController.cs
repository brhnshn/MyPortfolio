using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;       // Repository arayüzü için
using MyPortfolio.Entities.Concrete;   // Message entity'si için

namespace MyPortfolio.Controllers
{
    public class HomeController : Controller
    {
        // 1. Repository'i burada tanýmlýyoruz
        private readonly IGenericRepository<Message> _messageRepository;

        // 2. Constructor'da inject ediyoruz (Bu olmazsa _messageRepository null gelir)
        public HomeController(IGenericRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(Message p)
        {
            // Tarih ve Okundu bilgisini arka planda biz atýyoruz
            p.Date = DateTime.Now;
            p.IsRead = false;

            // Konu boþ gelirse varsayýlan bir þey yazalým ki veritabaný kýzmasýn
            if (string.IsNullOrEmpty(p.Subject))
            {
                p.Subject = "Ýletiþim Formu Mesajý";
            }

            // Kaydet
            _messageRepository.Insert(p);

            // Ýþlem bitince ana sayfaya dön
            return RedirectToAction("Index");
        }
    }
}