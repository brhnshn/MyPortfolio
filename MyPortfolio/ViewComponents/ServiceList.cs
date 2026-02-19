using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class ServiceList : ViewComponent
    {
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IMemoryCache _cache;

        public ServiceList(IGenericRepository<Service> serviceRepository, IMemoryCache cache)
        {
            _serviceRepository = serviceRepository;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var values = _cache.GetOrCreate("service_list", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _serviceRepository.GetList();
            });
            return View(values);
        }
    }
}