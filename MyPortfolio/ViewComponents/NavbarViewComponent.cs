using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IGenericRepository<NavbarItem> _navbarRepo;
        private readonly IGenericRepository<SiteSettings> _settingsRepo;

        public NavbarViewComponent(
            IGenericRepository<NavbarItem> navbarRepo,
            IGenericRepository<SiteSettings> settingsRepo)
        {
            _navbarRepo = navbarRepo;
            _settingsRepo = settingsRepo;
        }

        public IViewComponentResult Invoke()
        {
            var items = _navbarRepo.GetList()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            var settings = _settingsRepo.GetList().FirstOrDefault();
            ViewBag.BrandTitle = settings?.FooterTitle ?? "MY PORTFOLIO";

            return View(items);
        }
    }
}
