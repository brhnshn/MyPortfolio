using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyPortfolio.Hubs;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ServicesController : Controller
    {
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IMemoryCache _cache;

        private readonly IHubContext<PortfolioHub> _hubContext;

        public ServicesController(IGenericRepository<Service> serviceRepository, IMemoryCache cache, IHubContext<PortfolioHub> hubContext)
        {
            _serviceRepository = serviceRepository;
            _cache = cache;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            var values = _serviceRepository.GetList();
            return View(values);
        }

        [HttpPost]
        public IActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                service.CreatedDate = DateTime.Now;
                _serviceRepository.Insert(service);
                _cache.Remove("service_list");
                _hubContext.Clients.All.SendAsync("UpdateComponent", "ServiceList");
            }
            // HATA BURADAYDI: return View() dersen Create.cshtml arar. 
            // Index'e dönmesi için RedirectToAction kullanmalısın.
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(Service service)
        {
            var existing = _serviceRepository.GetById(service.Id);
            if (existing != null)
            {
                existing.Title = service.Title;
                existing.Description = service.Description;
                existing.IconUrl = service.IconUrl;
                existing.UpdatedDate = DateTime.Now;
                _serviceRepository.Update(existing);
                _cache.Remove("service_list");
                _hubContext.Clients.All.SendAsync("UpdateComponent", "ServiceList");
            }
            // İşlem bitti, listeye geri dön
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var value = _serviceRepository.GetById(id);
            if (value != null)
            {
                _serviceRepository.Delete(value);
                _cache.Remove("service_list");
                _hubContext.Clients.All.SendAsync("UpdateComponent", "ServiceList");
            }
            // Sildi ve listeye geri döndü
            return RedirectToAction("Index");
        }
    }
}
