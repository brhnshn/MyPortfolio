using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class TickerViewComponent : ViewComponent
    {
        private readonly IGenericRepository<TickerItem> _tickerRepo;
        private readonly IMemoryCache _cache;

        public TickerViewComponent(IGenericRepository<TickerItem> tickerRepo, IMemoryCache cache)
        {
            _tickerRepo = tickerRepo;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var items = _cache.GetOrCreate("ticker_list", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _tickerRepo.GetList().OrderBy(x => x.DisplayOrder).ToList();
            });
            return View(items);
        }
    }
}