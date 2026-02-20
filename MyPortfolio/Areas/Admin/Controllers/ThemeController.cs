using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ThemeController : Controller
    {
        private readonly IGenericRepository<SiteSettings> _siteSettingsRepo;
        private readonly IMemoryCache _cache;

        public ThemeController(IGenericRepository<SiteSettings> siteSettingsRepo, IMemoryCache cache)
        {
            _siteSettingsRepo = siteSettingsRepo;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var settings = _siteSettingsRepo.GetList().FirstOrDefault();
            ViewBag.CurrentTheme = settings?.ActiveTemplate ?? "DefaultTheme";
            ViewBag.LayoutMode = settings?.LayoutMode ?? "SinglePage";
            return View();
        }

        [HttpPost]
        public IActionResult SelectTheme(string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                return BadRequest();
            }

            var settings = _siteSettingsRepo.GetList().FirstOrDefault();

            if (settings == null)
            {
                // Create new settings if not exists
                settings = new SiteSettings
                {
                    SiteTitle = "My Portfolio",
                    ActiveTemplate = themeName,
                    PrimaryColor = GetDefaultPrimaryColor(themeName),
                    SecondaryColor = GetDefaultSecondaryColor(themeName)
                };
                _siteSettingsRepo.Insert(settings);
            }
            else
            {
                settings.ActiveTemplate = themeName;
                settings.PrimaryColor = GetDefaultPrimaryColor(themeName);
                settings.SecondaryColor = GetDefaultSecondaryColor(themeName);
                _siteSettingsRepo.Update(settings);
            }

            TempData["Success"] = $"{GetThemeDisplayName(themeName)} teması başarıyla aktif edildi!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult SetLayoutMode(string mode)
        {
            if (mode != "SinglePage" && mode != "MultiPage")
                return BadRequest();

            var settings = _siteSettingsRepo.GetList().FirstOrDefault();
            if (settings == null)
            {
                settings = new SiteSettings
                {
                    SiteTitle = "My Portfolio",
                    ActiveTemplate = "DefaultTheme",
                    LayoutMode = mode
                };
                _siteSettingsRepo.Insert(settings);
            }
            else
            {
                settings.LayoutMode = mode;
                _siteSettingsRepo.Update(settings);
            }

            _cache.Remove("footer_settings");
            _cache.Remove("navbar_settings");
            _cache.Remove("navbar_items");

            var modeName = mode == "SinglePage" ? "Tek Sayfa" : "Cok Sayfa";
            TempData["Success"] = modeName + " modu basariyla aktif edildi!";
            return RedirectToAction("Index");
        }

        private string GetThemeDisplayName(string theme)
        {
            return theme switch
            {
                "ModernTheme" => "Modern Glass",
                "DarkTheme" => "Varsayılan (Dark)",
                _ => "Varsayılan (Light)"
            };
        }

        private string GetDefaultPrimaryColor(string theme)
        {
            return theme switch
            {
                "ModernTheme" => "#667eea",
                "DarkTheme" => "#bb86fc",
                _ => "#0d6efd"
            };
        }

        private string GetDefaultSecondaryColor(string theme)
        {
            return theme switch
            {
                "ModernTheme" => "#764ba2",
                "DarkTheme" => "#03dac6",
                _ => "#6c757d"
            };
        }
    }
}
