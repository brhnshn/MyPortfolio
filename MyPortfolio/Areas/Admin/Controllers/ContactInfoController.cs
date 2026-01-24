using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ContactInfoController : Controller
    {
        private readonly IGenericRepository<ContactInfo> _contactInfoRepository;

        public ContactInfoController(IGenericRepository<ContactInfo> contactInfoRepository)
        {
            _contactInfoRepository = contactInfoRepository;
        }

        public IActionResult Index()
        {
            var values = _contactInfoRepository.GetList();
            return View(values);
        }

        [HttpPost]
        public IActionResult Create(ContactInfo p)
        {
            // EĞER P NULL GELİRSE İŞLEM YAPMA (HATAYI ÖNLER)
            if (p == null)
            {
                return RedirectToAction("Index");
            }

            _contactInfoRepository.Insert(p);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(ContactInfo p)
        {
            if (p == null) return RedirectToAction("Index"); // Koruma

            var existing = _contactInfoRepository.GetById(p.Id);
            if (existing != null)
            {
                existing.Address = p.Address;
                existing.Phone = p.Phone;
                existing.Email = p.Email;
                existing.MapUrl = p.MapUrl; // Artık burada Şehir ismi var
                _contactInfoRepository.Update(existing);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var value = _contactInfoRepository.GetById(id);
            if (value != null) _contactInfoRepository.Delete(value);
            return RedirectToAction("Index");
        }
    }
}