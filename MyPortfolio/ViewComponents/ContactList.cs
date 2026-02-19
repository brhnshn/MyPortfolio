using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class ContactList : ViewComponent
    {
        private readonly IGenericRepository<ContactInfo> _contactInfoRepository;
        private readonly IMemoryCache _cache;

        public ContactList(IGenericRepository<ContactInfo> contactInfoRepository, IMemoryCache cache)
        {
            _contactInfoRepository = contactInfoRepository;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var values = _cache.GetOrCreate("contact_list", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _contactInfoRepository.GetList();
            });
            return View(values);
        }
    }
}