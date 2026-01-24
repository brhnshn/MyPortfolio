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

        public ThemeController(IGenericRepository<SiteSettings> siteSettingsRepo)
        {
            _siteSettingsRepo = siteSettingsRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var settings = _siteSettingsRepo.GetList().FirstOrDefault();
            ViewBag.CurrentTheme = settings?.ActiveTemplate ?? "DefaultTheme";
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
