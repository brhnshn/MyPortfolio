using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class SkillList : ViewComponent
    {
        private readonly IGenericRepository<Skill> _skillRepository;
        private readonly IMemoryCache _cache;

        public SkillList(IGenericRepository<Skill> skillRepository, IMemoryCache cache)
        {
            _skillRepository = skillRepository;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var values = _cache.GetOrCreate("skill_list", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _skillRepository.GetList();
            });
            return View(values);
        }
    }
}