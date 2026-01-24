using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Models
{
    public class DashboardViewModel
    {
        public List<Project> Projects { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Service> Services { get; set; }
        public List<Testimonial> Testimonials { get; set; }
    }
}