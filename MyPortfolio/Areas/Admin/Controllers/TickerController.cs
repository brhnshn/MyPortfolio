using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TickerController : Controller
    {
        private readonly IGenericRepository<TickerItem> _tickerRepo;
        private const int MaxTickerItems = 10;

        public TickerController(IGenericRepository<TickerItem> tickerRepo)
        {
            _tickerRepo = tickerRepo;
        }

        public IActionResult Index()
        {
            var items = _tickerRepo.GetList().OrderBy(x => x.DisplayOrder).ToList();
            return View(items);
        }

        [HttpPost]
        public IActionResult Create(TickerItem item)
        {
            var currentCount = _tickerRepo.GetList().Count;
            if (currentCount >= MaxTickerItems)
            {
                TempData["Error"] = $"Maksimum {MaxTickerItems} kayan yazı ekleyebilirsiniz!";
                return RedirectToAction("Index");
            }

            item.CreatedDate = DateTime.Now;
            item.IsActive = true;
            _tickerRepo.Insert(item);

            TempData["Success"] = "Kayan yazı başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(TickerItem item)
        {
            var existing = _tickerRepo.GetById(item.Id);
            if (existing == null) return RedirectToAction("Index");

            existing.Text = item.Text;
            existing.IconClass = item.IconClass;
            existing.DisplayOrder = item.DisplayOrder;
            existing.UpdatedDate = DateTime.Now;

            _tickerRepo.Update(existing);

            TempData["Success"] = "Kayan yazı başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var item = _tickerRepo.GetById(id);
            if (item != null)
            {
                _tickerRepo.Delete(item);
                TempData["Success"] = "Kayan yazı başarıyla silindi!";
            }
            return RedirectToAction("Index");
        }
    }
}
