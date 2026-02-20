using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IGenericRepository<SiteSettings> _settingsRepo;
        private readonly IMemoryCache _cache;

        public FooterViewComponent(IGenericRepository<SiteSettings> settingsRepo, IMemoryCache cache)
        {
            _settingsRepo = settingsRepo;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var settings = _cache.GetOrCreate("footer_settings", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var s = _settingsRepo.GetList().FirstOrDefault();
                return s ?? new SiteSettings
                {
                    FooterTitle = "MY PORTFOLIO",
                    FooterDescription = "Portfolio",
                    CopyrightText = "MyPortfolio",
                    DeveloperName = "Burhan Sahin",
                    DeveloperUrl = "#",
                    GithubUrl = "#",
                    InstagramUrl = "#",
                    LinkedinUrl = "#",
                    Email = "example@email.com"
                };
            });
            ViewBag.LayoutMode = settings?.LayoutMode ?? "SinglePage";
            return View(settings);
        }
    }
}