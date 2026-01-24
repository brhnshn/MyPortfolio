using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IGenericRepository<SiteSettings> _settingsRepo;

        public FooterViewComponent(IGenericRepository<SiteSettings> settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        public IViewComponentResult Invoke()
        {
            var settings = _settingsRepo.GetList().FirstOrDefault();

            // Return default values if settings not found
            if (settings == null)
            {
                settings = new SiteSettings
                {
                    FooterTitle = "MY PORTFOLIO",
                    FooterDescription = "Kendinizi, işinizi veya hobilerinizi dünyaya tanıtın.",
                    CopyrightText = "MyPortfolio",
                    DeveloperName = "Burhan Şahin",
                    DeveloperUrl = "#",
                    GithubUrl = "#",
                    InstagramUrl = "#",
                    LinkedinUrl = "#",
                    Email = "example@email.com"
                };
            }

            return View(settings);
        }
    }
}
