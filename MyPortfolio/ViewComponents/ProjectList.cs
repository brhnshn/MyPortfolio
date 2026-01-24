using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;
using System.Linq;

namespace MyPortfolio.ViewComponents
{
    public class ProjectList : ViewComponent
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public ProjectList(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public IViewComponentResult Invoke()
        {
            // Eğer DisplayOrder kullanıyorsan ona göre sıralayabilirsin, yoksa ID'ye göre
            var values = _projectRepository.GetList().OrderBy(x => x.DisplayOrder).ToList();
            return View(values);
        }
    }
}