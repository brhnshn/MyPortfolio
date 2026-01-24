using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class TickerViewComponent : ViewComponent
    {
        private readonly IGenericRepository<TickerItem> _tickerRepo;

        public TickerViewComponent(IGenericRepository<TickerItem> tickerRepo)
        {
            _tickerRepo = tickerRepo;
        }

        public IViewComponentResult Invoke()
        {
            var items = _tickerRepo.GetList()
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            return View(items);
        }
    }
}
