using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.ViewComponents
{
    public class SkillList : ViewComponent
    {
        private readonly IGenericRepository<Skill> _skillRepository;

        public SkillList(IGenericRepository<Skill> skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public IViewComponentResult Invoke()
        {
            var values = _skillRepository.GetList();
            return View(values);    
        }
    }
}