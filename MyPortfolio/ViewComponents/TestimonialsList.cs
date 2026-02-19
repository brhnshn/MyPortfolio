using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

public class TestimonialsList : ViewComponent
{
    private readonly IGenericRepository<Testimonial> _testimonialRepository;
    private readonly IMemoryCache _cache;

    public TestimonialsList(IGenericRepository<Testimonial> testimonialRepository, IMemoryCache cache)
    {
        _testimonialRepository = testimonialRepository;
        _cache = cache;
    }

    public IViewComponentResult Invoke()
    {
        var values = _cache.GetOrCreate("testimonials_list", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return _testimonialRepository.GetList();
        });
        return View(values);
    }
}