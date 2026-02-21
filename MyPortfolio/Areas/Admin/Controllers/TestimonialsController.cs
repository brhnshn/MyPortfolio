using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyPortfolio.Hubs;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TestimonialsController : Controller
    {
        private readonly IGenericRepository<Testimonial> _testimonialRepository;
        private readonly IMemoryCache _cache;
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        private readonly IHubContext<PortfolioHub> _hubContext;

        public TestimonialsController(IGenericRepository<Testimonial> testimonialRepository, IMemoryCache cache, IHubContext<PortfolioHub> hubContext)
        {
            _testimonialRepository = testimonialRepository;
            _cache = cache;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            var values = _testimonialRepository.GetList();
            return View(values);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var value = _testimonialRepository.GetById(id);
            if (value != null)
            {
                try
                {
                    // Varsa resim dosyasını sil
                    if (!string.IsNullOrEmpty(value.ImageUrl) && !value.ImageUrl.Contains("placeholder"))
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", value.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }
                }
                catch { }

                _testimonialRepository.Delete(value);
                _cache.Remove("testimonials_list");
                _hubContext.Clients.All.SendAsync("UpdateComponent", "TestimonialsList");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Testimonial t, IFormFile imageFile)
        {
            // Null kontrolü
            if (t == null) t = new Testimonial();

            t.CreatedDate = DateTime.Now;

            // Resim Yükleme
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AllowedImageExtensions.Contains(extension) || imageFile.Length > MaxFileSize)
                {
                    TempData["Error"] = "Geçersiz resim dosyası! Sadece JPG, PNG, GIF, WebP, SVG (max 5MB) yükleyebilirsiniz.";
                    return RedirectToAction("Index");
                }
                var newImageName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/testimonialimages/", newImageName);

                // Klasör yoksa oluştur
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/testimonialimages/")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/testimonialimages/"));
                }

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                t.ImageUrl = "/testimonialimages/" + newImageName;
            }
            else
            {
                // Resim yoksa rastgele avatar ata (Erkek/Kadın ayrımı yapamıyoruz ama nötr avatar koyabiliriz)
                t.ImageUrl = "https://ui-avatars.com/api/?name=" + t.ClientName + "&background=random";
            }

            _testimonialRepository.Insert(t);
            _cache.Remove("testimonials_list");
                _hubContext.Clients.All.SendAsync("UpdateComponent", "TestimonialsList");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] Testimonial t, IFormFile imageFile)
        {
            var existing = _testimonialRepository.GetById(t.Id);
            if (existing == null) return RedirectToAction("Index");

            // Bilgileri güncelle
            existing.ClientName = t.ClientName;
            existing.Company = t.Company;
            existing.Title = t.Title;
            existing.Comment = t.Comment;
            existing.UpdatedDate = DateTime.Now;

            // Resim değiştiyse
            if (imageFile != null)
            {
                // Eski resmi sil
                try
                {
                    if (!string.IsNullOrEmpty(existing.ImageUrl) && !existing.ImageUrl.Contains("ui-avatars"))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                }
                catch { }

                // Yeniyi yükle
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AllowedImageExtensions.Contains(extension) || imageFile.Length > MaxFileSize)
                {
                    TempData["Error"] = "Geçersiz resim dosyası! Sadece JPG, PNG, GIF, WebP, SVG (max 5MB) yükleyebilirsiniz.";
                    return RedirectToAction("Index");
                }
                var newImageName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/testimonialimages/", newImageName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/testimonialimages/")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/testimonialimages/"));
                }

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                existing.ImageUrl = "/testimonialimages/" + newImageName;
            }

            _testimonialRepository.Update(existing);
            _cache.Remove("testimonials_list");
                _hubContext.Clients.All.SendAsync("UpdateComponent", "TestimonialsList");
            return RedirectToAction("Index");
        }
    }
}
