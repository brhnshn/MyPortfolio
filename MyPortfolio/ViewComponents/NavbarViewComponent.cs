using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IGenericRepository<NavbarItem> _navbarRepo;
        private readonly IGenericRepository<SiteSettings> _settingsRepo;
        private readonly IMemoryCache _cache;

        public NavbarViewComponent(
            IGenericRepository<NavbarItem> navbarRepo,
            IGenericRepository<SiteSettings> settingsRepo,
            IMemoryCache cache)
        {
            _navbarRepo = navbarRepo;
            _settingsRepo = settingsRepo;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var items = _cache.GetOrCreate("navbar_items", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _navbarRepo.GetList()
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();
            });

            var settings = _cache.GetOrCreate("navbar_settings", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _settingsRepo.GetList().FirstOrDefault();
            });

            ViewBag.BrandTitle = settings?.FooterTitle ?? "MY PORTFOLIO";
            return View(items);
        }
    }
}