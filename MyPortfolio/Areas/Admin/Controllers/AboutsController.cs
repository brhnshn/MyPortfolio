using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AboutsController : Controller
    {
        private readonly IGenericRepository<About> _aboutRepository;
        private readonly IMemoryCache _cache;
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        private static readonly string[] AllowedCvExtensions = { ".pdf" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public AboutsController(IGenericRepository<About> aboutRepository, IMemoryCache cache)
        {
            _aboutRepository = aboutRepository;
            _cache = cache;
        }

        public IActionResult Index()
        {
            var values = _aboutRepository.GetList();
            return View(values);
        }

        [HttpPost]
        public async Task<IActionResult> Create(About about, IFormFile imageFile, IFormFile cvFile)
        {
            // 1 KAYIT KURALI: Sadece 1 tane Hakkımda yazısı olabilir.
            if (_aboutRepository.GetList().Count >= 1) return RedirectToAction("Index");

            about.CreatedDate = DateTime.Now;

            // 1. RESİM YÜKLEME
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AllowedImageExtensions.Contains(extension) || imageFile.Length > MaxFileSize)
                {
                    TempData["Error"] = "Geçersiz resim dosyası! Sadece JPG, PNG, GIF, WebP, SVG (max 5MB) yükleyebilirsiniz.";
                    return RedirectToAction("Index");
                }
                var newImageName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/aboutimages/", newImageName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/aboutimages/")))
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/aboutimages/"));

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                about.ImageUrl = "/aboutimages/" + newImageName;
            }

            // 2. CV YÜKLEME
            if (cvFile != null)
            {
                var extension = Path.GetExtension(cvFile.FileName).ToLowerInvariant();
                if (!AllowedCvExtensions.Contains(extension) || cvFile.Length > MaxFileSize)
                {
                    TempData["Error"] = "Geçersiz CV dosyası! Sadece PDF (max 5MB) yükleyebilirsiniz.";
                    return RedirectToAction("Index");
                }
                var newCvName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/cvfiles/", newCvName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/cvfiles/")))
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/cvfiles/"));

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await cvFile.CopyToAsync(stream);
                }
                about.CvUrl = "/cvfiles/" + newCvName;
            }

            _aboutRepository.Insert(about);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var about = _aboutRepository.GetById(id);
            if (about != null)
            {
                // Dosyaları klasörden de sil
                try
                {
                    if (!string.IsNullOrEmpty(about.ImageUrl) && !about.ImageUrl.Contains("default"))
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", about.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }
                    if (!string.IsNullOrEmpty(about.CvUrl))
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", about.CvUrl.TrimStart('/'));
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }
                }
                catch { }

                _aboutRepository.Delete(about);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(About about, IFormFile imageFile, IFormFile cvFile)
        {
            var existing = _aboutRepository.GetById(about.Id);
            if (existing == null) return RedirectToAction("Index");

            // --- METİNSEL ALANLARI GÜNCELLE ---
            existing.Title = about.Title;
            existing.Description = about.Description;
            existing.SubDescription = about.SubDescription;
            existing.Details = about.Details;
            existing.FullName = about.FullName;
            existing.Phone = about.Phone;
            existing.Address = about.Address;

            // Mevcut Alanlar
            existing.Email = about.Email;
            existing.Age = about.Age;

            // --- YENİ EKLENEN İSTATİSTİKLER BURADA ---
            existing.ProjectCount = about.ProjectCount;     // Proje Sayısı
            existing.ExperienceYear = about.ExperienceYear; // Deneyim Yılı
            existing.CustomerCount = about.CustomerCount;   // Müşteri Sayısı

            existing.UpdatedDate = DateTime.Now;

            // --- RESİM GÜNCELLEME ---
            if (imageFile != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(existing.ImageUrl))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                }
                catch { }

                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AllowedImageExtensions.Contains(extension) || imageFile.Length > MaxFileSize)
                {
                    TempData["Error"] = "Geçersiz resim dosyası! Sadece JPG, PNG, GIF, WebP, SVG (max 5MB) yükleyebilirsiniz.";
                    return RedirectToAction("Index");
                }
                var newImageName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/aboutimages/", newImageName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/aboutimages/")))
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/aboutimages/"));

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                existing.ImageUrl = "/aboutimages/" + newImageName;
            }

            // --- CV GÜNCELLEME ---
            if (cvFile != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(existing.CvUrl))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.CvUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                }
                catch { }

                var extension = Path.GetExtension(cvFile.FileName).ToLowerInvariant();
                if (!AllowedCvExtensions.Contains(extension) || cvFile.Length > MaxFileSize)
                {
                    TempData["Error"] = "Geçersiz CV dosyası! Sadece PDF (max 5MB) yükleyebilirsiniz.";
                    return RedirectToAction("Index");
                }
                var newCvName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/cvfiles/", newCvName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/cvfiles/")))
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/cvfiles/"));

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await cvFile.CopyToAsync(stream);
                }
                existing.CvUrl = "/cvfiles/" + newCvName;
            }

            _aboutRepository.Update(existing);
            return RedirectToAction("Index");
        }
    }
}