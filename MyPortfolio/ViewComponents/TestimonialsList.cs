using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

public class TestimonialsList : ViewComponent
{
    private readonly IGenericRepository<Testimonial> _testimonialRepository;

    public TestimonialsList(IGenericRepository<Testimonial> testimonialRepository)
    {
        _testimonialRepository = testimonialRepository;
    }

    public IViewComponentResult Invoke()
    {
        var values = _testimonialRepository.GetList();
        return View(values);
    }
}