using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class NavbarController : Controller
    {
        private readonly IGenericRepository<NavbarItem> _navbarRepo;
        private readonly IMemoryCache _cache;

        public NavbarController(IGenericRepository<NavbarItem> navbarRepo, IMemoryCache cache)
        {
            _navbarRepo = navbarRepo;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var items = _navbarRepo.GetList().OrderBy(x => x.DisplayOrder).ToList();

            // If empty, seed default menu items
            if (!items.Any())
            {
                SeedDefaultItems();
                items = _navbarRepo.GetList().OrderBy(x => x.DisplayOrder).ToList();
            }

            // AUTO-FIX: Ensure "hero" is always at DisplayOrder 0
            var heroItem = items.FirstOrDefault(x => x.SectionId == "hero");
            if (heroItem != null && heroItem.DisplayOrder != 0)
            {
                // Set hero to 0
                heroItem.DisplayOrder = 0;
                heroItem.UpdatedDate = DateTime.Now;
                _navbarRepo.Update(heroItem);
                _cache.Remove("navbar_items");

                // Fix others: If any other item has 0, move it to 1, etc.
                // Simple reorder: Get all non-hero items, sort by current order, re-assign 1..N
                var otherItems = items.Where(x => x.SectionId != "hero").OrderBy(x => x.DisplayOrder).ToList();
                for (int i = 0; i < otherItems.Count; i++)
                {
                    var item = otherItems[i];
                    if (item.DisplayOrder != i + 1)
                    {
                        item.DisplayOrder = i + 1;
                        item.UpdatedDate = DateTime.Now;
                        _navbarRepo.Update(item);
                _cache.Remove("navbar_items");
                    }
                }

                // Refresh list
                items = _navbarRepo.GetList().OrderBy(x => x.DisplayOrder).ToList();
            }

            return View(items);
        }

        [HttpPost]
        public IActionResult ToggleActive(int id)
        {
            var item = _navbarRepo.GetById(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                item.UpdatedDate = DateTime.Now;
                _navbarRepo.Update(item);
                _cache.Remove("navbar_items");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateOrder(int id, int newOrder)
        {
            var item = _navbarRepo.GetById(id);
            if (item != null)
            {
                // Prevent moving the hero section (Ana Sayfa) or moving anything to position 0
                if (item.SectionId == "hero" || newOrder < 0)
                {
                    return RedirectToAction("Index");
                }

                // If target is 0, prevent because 0 is reserved for hero
                if (newOrder == 0)
                {
                    TempData["Error"] = "Ana Sayfa her zaman ilk sırada olmalıdır.";
                    return RedirectToAction("Index");
                }

                // Find the item currently at the new order to swap with
                var targetItem = _navbarRepo.GetList().FirstOrDefault(x => x.DisplayOrder == newOrder);

                if (targetItem != null)
                {
                    // Swap logic
                    targetItem.DisplayOrder = item.DisplayOrder;
                    targetItem.UpdatedDate = DateTime.Now;
                    _navbarRepo.Update(targetItem);
                _cache.Remove("navbar_items");
                }

                item.DisplayOrder = newOrder;
                item.UpdatedDate = DateTime.Now;
                _navbarRepo.Update(item);
                _cache.Remove("navbar_items");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateTitle(int id, string title)
        {
            var item = _navbarRepo.GetById(id);
            if (item != null && !string.IsNullOrEmpty(title))
            {
                item.Title = title;
                item.UpdatedDate = DateTime.Now;
                _navbarRepo.Update(item);
                _cache.Remove("navbar_items");
            }
            return RedirectToAction("Index");
        }

        private void SeedDefaultItems()
        {
            var defaults = new List<NavbarItem>
            {
                new() { Title = "Ana Sayfa", SectionId = "hero", DisplayOrder = 0, IsActive = true, CreatedDate = DateTime.Now },
                new() { Title = "Hakkımda", SectionId = "about", DisplayOrder = 1, IsActive = true, CreatedDate = DateTime.Now },
                new() { Title = "Hizmetler", SectionId = "services", DisplayOrder = 2, IsActive = true, CreatedDate = DateTime.Now },
                new() { Title = "Yetenekler", SectionId = "skills", DisplayOrder = 3, IsActive = true, CreatedDate = DateTime.Now },
                new() { Title = "Projeler", SectionId = "projects", DisplayOrder = 4, IsActive = true, CreatedDate = DateTime.Now },
                new() { Title = "Referanslar", SectionId = "testimonials", DisplayOrder = 5, IsActive = true, CreatedDate = DateTime.Now },
                new() { Title = "İletişim", SectionId = "contact", DisplayOrder = 6, IsActive = true, CreatedDate = DateTime.Now }
            };

            foreach (var item in defaults)
            {
                _navbarRepo.Insert(item);
                _cache.Remove("navbar_items");
            }
        }
    }
}
