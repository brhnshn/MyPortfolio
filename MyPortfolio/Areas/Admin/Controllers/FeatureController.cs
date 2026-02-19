using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FeatureController : Controller
    {
        private readonly IGenericRepository<Feature> _featureRepository;
        private readonly IMemoryCache _cache;
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public FeatureController(IGenericRepository<Feature> featureRepository, IMemoryCache cache)
        {
            _featureRepository = featureRepository;
            _cache = cache;
        }

        public IActionResult Index()
        {
            var values = _featureRepository.GetList();
            return View(values);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var value = _featureRepository.GetById(id);
            if (value != null)
            {
                // Varsa eski resmi de klasörden silelim (Temizlik imandan gelir)
                if (!string.IsNullOrEmpty(value.ImageUrl) && !value.ImageUrl.Contains("default"))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", value.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                _featureRepository.Delete(value);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Feature feature, IFormFile imageFile)
        {
            // Limit kontrolü (1'den fazla ekleme yapılmasın)
            if (_featureRepository.GetList().Count >= 1) return RedirectToAction("Index");

            // Resim Yükleme İşlemi
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AllowedImageExtensions.Contains(extension) || imageFile.Length > MaxFileSize)
                {
                    TempData["Error"] = "Geçersiz resim dosyası! Sadece JPG, PNG, GIF, WebP, SVG (max 5MB) yükleyebilirsiniz.";
                    return RedirectToAction("Index");
                }
                var newImageName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/featureimages/", newImageName);

                // Klasör yoksa oluştur
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/featureimages/")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/featureimages/"));
                }

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                feature.ImageUrl = "/featureimages/" + newImageName;
            }
            else
            {
                feature.ImageUrl = "/img/default-bg.jpg"; // Resim seçilmezse varsayılan
            }

            _featureRepository.Insert(feature);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] Feature feature, IFormFile imageFile)
        {
            var existing = _featureRepository.GetById(feature.Id);
            if (existing != null)
            {
                existing.Header = feature.Header;
                existing.Name = feature.Name;
                existing.Title = feature.Title;

                // Resim Değiştiyse
                if (imageFile != null)
                {
                    // 1. Eski resmi sil
                    if (!string.IsNullOrEmpty(existing.ImageUrl) && !existing.ImageUrl.Contains("default"))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    // 2. Yeni resmi yükle
                    var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                    if (!AllowedImageExtensions.Contains(extension) || imageFile.Length > MaxFileSize)
                    {
                        TempData["Error"] = "Geçersiz resim dosyası! Sadece JPG, PNG, GIF, WebP, SVG (max 5MB) yükleyebilirsiniz.";
                        return RedirectToAction("Index");
                    }
                    var newImageName = Guid.NewGuid() + extension;
                    var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/featureimages/", newImageName);

                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/featureimages/")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/featureimages/"));
                    }

                    using (var stream = new FileStream(location, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    existing.ImageUrl = "/featureimages/" + newImageName;
                }

                _featureRepository.Update(existing);
            }
            return RedirectToAction("Index");
        }
    }
}