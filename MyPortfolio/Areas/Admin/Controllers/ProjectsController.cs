using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfoliod.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IGenericRepository<Project> _projectRepository;
        private readonly IMemoryCache _cache;

        public ProjectsController(IGenericRepository<Project> projectRepository, IMemoryCache cache)
        {
            _projectRepository = projectRepository;
            _cache = cache;
        }

        public IActionResult Index()
        {
            var values = _projectRepository.GetList();
            return View(values);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var value = _projectRepository.GetById(id);
            if (value != null)
            {
                _projectRepository.Delete(value);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Create([FromForm] Project p)
        {
            if (p == null) p = new Project();

            p.CreatedDate = DateTime.Now;
            p.UpdatedDate = DateTime.Now;

            // URL boşsa placeholder kullan
            if (string.IsNullOrEmpty(p.ImageUrl))
            {
                p.ImageUrl = "https://placehold.co/600x400?text=Resim+Yok";
            }

            _projectRepository.Insert(p);
            TempData["Success"] = "Proje başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(Project p)
        {
            var existing = _projectRepository.GetById(p.Id);
            if (existing == null) return RedirectToAction("Index");

            existing.Title = p.Title;
            existing.Description = p.Description;
            existing.GithubLink = p.GithubLink;
            existing.ProjectUrl = p.ProjectUrl;
            existing.Platform = p.Platform;
            existing.ImageUrl = p.ImageUrl;
            existing.UpdatedDate = DateTime.Now;

            _projectRepository.Update(existing);
            TempData["Success"] = "Proje başarıyla güncellendi!";
            return RedirectToAction("Index");
        }
    }
}