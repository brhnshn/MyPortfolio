using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class AboutList : ViewComponent
    {
        private readonly IGenericRepository<About> _aboutRepository;
        private readonly IMemoryCache _cache;

        public AboutList(IGenericRepository<About> aboutRepository, IMemoryCache cache)
        {
            _aboutRepository = aboutRepository;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var values = _cache.GetOrCreate("about_list", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _aboutRepository.GetList();
            });
            return View(values);
        }
    }
}