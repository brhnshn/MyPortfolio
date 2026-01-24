using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class FeatureList : ViewComponent
    {
        private readonly IGenericRepository<Feature> _featureRepository;

        public FeatureList(IGenericRepository<Feature> featureRepository)
        {
            _featureRepository = featureRepository;
        }

        public IViewComponentResult Invoke()
        {
            var values = _featureRepository.GetList();
            return View(values);
        }
    }
}