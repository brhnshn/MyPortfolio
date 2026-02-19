using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class FeatureList : ViewComponent
    {
        private readonly IGenericRepository<Feature> _featureRepository;
        private readonly IMemoryCache _cache;

        public FeatureList(IGenericRepository<Feature> featureRepository, IMemoryCache cache)
        {
            _featureRepository = featureRepository;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var values = _cache.GetOrCreate("feature_list", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _featureRepository.GetList();
            });
            return View(values);
        }
    }
}