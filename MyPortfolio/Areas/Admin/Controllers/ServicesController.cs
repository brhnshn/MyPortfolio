using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServicesController : Controller
    {
        private readonly IGenericRepository<Service> _serviceRepository;

        public ServicesController(IGenericRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
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
            }
            // İşlem bitti, listeye geri dön
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var value = _serviceRepository.GetById(id);
            if (value != null)
            {
                _serviceRepository.Delete(value);
            }
            // Sildi ve listeye geri döndü
            return RedirectToAction("Index");
        }
    }
}