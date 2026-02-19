using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SkillsController : Controller
    {
        private readonly IGenericRepository<Skill> _skillRepository;
        private readonly IMemoryCache _cache;

        public SkillsController(IGenericRepository<Skill> skillRepository, IMemoryCache cache)
        {
            _skillRepository = skillRepository;
            _cache = cache;
        }

        public IActionResult Index()
        {
            var values = _skillRepository.GetList();
            return View(values);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var value = _skillRepository.GetById(id);
            if (value != null) _skillRepository.Delete(value);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                // Değer 100'den büyükse 100'e eşitle (Hata önleyici)
                if (skill.Percentage > 100) skill.Percentage = 100;
                if (skill.Percentage < 0) skill.Percentage = 0;

                _skillRepository.Insert(skill);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(Skill skill)
        {
            var existingSkill = _skillRepository.GetById(skill.Id);

            if (existingSkill != null)
            {
                existingSkill.Title = skill.Title;
                existingSkill.Category = skill.Category; // Kategori güncellemesi eklendi

                // Oran kontrolü
                if (skill.Percentage > 100) skill.Percentage = 100;
                else if (skill.Percentage < 0) skill.Percentage = 0;
                else existingSkill.Percentage = skill.Percentage;

                _skillRepository.Update(existingSkill);
            }

            return RedirectToAction("Index");
        }
    }
}