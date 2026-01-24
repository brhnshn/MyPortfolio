using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class ServiceList : ViewComponent
    {
        private readonly IGenericRepository<Service> _serviceRepository;

        public ServiceList(IGenericRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public IViewComponentResult Invoke()
        {
            var values = _serviceRepository.GetList();
            return View(values);
        }
    }
}