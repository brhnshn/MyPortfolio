using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

#nullable enable

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SiteSettingsController : Controller
    {
        private readonly IGenericRepository<SiteSettings> _siteSettingsRepo;

        public SiteSettingsController(IGenericRepository<SiteSettings> siteSettingsRepo)
        {
            _siteSettingsRepo = siteSettingsRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var settings = _siteSettingsRepo.GetList().FirstOrDefault();

            // If no settings exist, create default
            if (settings == null)
            {
                settings = new SiteSettings
                {
                    SiteTitle = "My Portfolio",
                    FaviconUrl = "/favicon.ico",
                    FooterTitle = "MY PORTFOLIO",
                    FooterDescription = "Kendinizi, işinizi veya hobilerinizi dünyaya tanıtın.",
                    CopyrightText = "MyPortfolio",
                    DeveloperName = "Burhan Şahin",
                    DeveloperUrl = "#",
                    CreatedDate = DateTime.Now
                };
                _siteSettingsRepo.Insert(settings);
            }

            return View(settings);
        }

        [HttpPost]
        public IActionResult Update(SiteSettings model)
        {
            // URL validation - reject base64 data URLs
            if (!string.IsNullOrEmpty(model.FaviconUrl) && model.FaviconUrl.StartsWith("data:"))
            {
                TempData["Error"] = "Base64 veri URL'leri desteklenmiyor. Lütfen normal bir URL girin (https://...)";
                return RedirectToAction("Index");
            }

            // Trim all URL fields to max 500 chars
            model.FaviconUrl = TrimUrl(model.FaviconUrl);
            model.GithubUrl = TrimUrl(model.GithubUrl);
            model.LinkedinUrl = TrimUrl(model.LinkedinUrl);
            model.InstagramUrl = TrimUrl(model.InstagramUrl);
            model.XUrl = TrimUrl(model.XUrl);
            model.DeveloperUrl = TrimUrl(model.DeveloperUrl);

            var existing = _siteSettingsRepo.GetList().FirstOrDefault();

            if (existing == null)
            {
                model.CreatedDate = DateTime.Now;
                _siteSettingsRepo.Insert(model);
            }
            else
            {
                existing.SiteTitle = model.SiteTitle;
                existing.MetaDescription = model.MetaDescription;
                existing.FaviconUrl = model.FaviconUrl;
                existing.Email = model.Email;
                existing.Phone = model.Phone;
                existing.GithubUrl = model.GithubUrl;
                existing.LinkedinUrl = model.LinkedinUrl;
                existing.XUrl = model.XUrl;
                existing.InstagramUrl = model.InstagramUrl;

                // Footer Settings
                existing.FooterTitle = model.FooterTitle;
                existing.FooterDescription = model.FooterDescription;
                existing.CopyrightText = model.CopyrightText;
                existing.DeveloperName = model.DeveloperName;
                existing.DeveloperUrl = model.DeveloperUrl;

                existing.UpdatedDate = DateTime.Now;
                _siteSettingsRepo.Update(existing);
            }

            TempData["Success"] = "Site ayarları başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        private string? TrimUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            return url.Length > 500 ? url.Substring(0, 500) : url;
        }
    }
}
