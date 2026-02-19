using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class ProjectList : ViewComponent
    {
        private readonly IGenericRepository<Project> _projectRepository;
        private readonly IMemoryCache _cache;

        public ProjectList(IGenericRepository<Project> projectRepository, IMemoryCache cache)
        {
            _projectRepository = projectRepository;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var values = _cache.GetOrCreate("project_list", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _projectRepository.GetList().OrderBy(x => x.DisplayOrder).ToList();
            });
            return View(values);
        }
    }
}