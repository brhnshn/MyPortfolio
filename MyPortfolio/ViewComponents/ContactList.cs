using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;
using System.Linq;

namespace MyPortfolio.ViewComponents
{
    public class ContactList : ViewComponent
    {
        private readonly IGenericRepository<ContactInfo> _contactInfoRepository;

        public ContactList(IGenericRepository<ContactInfo> contactInfoRepository)
        {
            _contactInfoRepository = contactInfoRepository;
        }

        public IViewComponentResult Invoke()
        {
            // Veritabanındaki ilk kaydı çekiyoruz (Genelde 1 tane olur)
            var values = _contactInfoRepository.GetList();
            return View(values);
        }
    }
}