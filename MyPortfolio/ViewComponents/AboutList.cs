using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class AboutList : ViewComponent
    {
        private readonly IGenericRepository<About> _aboutRepository;

        public AboutList(IGenericRepository<About> aboutRepository)
        {
            _aboutRepository = aboutRepository;
        }

        public IViewComponentResult Invoke()
        {
            var values = _aboutRepository.GetList();
            return View(values);
        }
    }
}