using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract; // Repository interface'leri için
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        // 7 Ana Modül için Repository Tanımları
        private readonly IGenericRepository<Feature> _featureRepo;
        private readonly IGenericRepository<About> _aboutRepo;
        private readonly IGenericRepository<Project> _projectRepo;
        private readonly IGenericRepository<Skill> _skillRepo;
        private readonly IGenericRepository<Service> _serviceRepo;
        private readonly IGenericRepository<Testimonial> _testimonialRepo;
        private readonly IGenericRepository<Message> _messageRepo;

        // Constructor Injection: Hepsini tek seferde alıyoruz
        public DashboardController(
            IGenericRepository<Feature> featureRepo,
            IGenericRepository<About> aboutRepo,
            IGenericRepository<Project> projectRepo,
            IGenericRepository<Skill> skillRepo,
            IGenericRepository<Service> serviceRepo,
            IGenericRepository<Testimonial> testimonialRepo,
            IGenericRepository<Message> messageRepo)
        {
            _featureRepo = featureRepo;
            _aboutRepo = aboutRepo;
            _projectRepo = projectRepo;
            _skillRepo = skillRepo;
            _serviceRepo = serviceRepo;
            _testimonialRepo = testimonialRepo;
            _messageRepo = messageRepo;
        }

        public IActionResult Index()
        {
            // Sadece sayıları alıp ViewBag ile View'a taşıyoruz.
            // Bu yöntem veritabanını yormaz, sadece liste boyutunu (Count) alır.

            ViewBag.FeatureCount = _featureRepo.GetList().Count;
            ViewBag.AboutCount = _aboutRepo.GetList().Count;
            ViewBag.ProjectCount = _projectRepo.GetList().Count;
            ViewBag.SkillCount = _skillRepo.GetList().Count;
            ViewBag.ServiceCount = _serviceRepo.GetList().Count;
            ViewBag.TestimonialCount = _testimonialRepo.GetList().Count;
            ViewBag.MessageCount = _messageRepo.GetList().Count;

            // Son 5 mesaj
            ViewBag.RecentMessages = _messageRepo.GetList()
                .OrderByDescending(m => m.Id)
                .Take(5)
                .ToList();

            return View();
        }
    }
}