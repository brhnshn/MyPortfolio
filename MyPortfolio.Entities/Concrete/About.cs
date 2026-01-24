using MyPortfolio.Entities.Abstract;

namespace MyPortfolio.Entities.Concrete
{
    public class About : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? SubDescription { get; set; }
        public string? Details { get; set; }
        public string? ImageUrl { get; set; }
        public string? CvUrl { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? FullName { get; set; }
        public string? Age { get; set; }

        // YENİ EKLENEN İSTATİSTİK ALANLARI (String yaptık ki "20+" yazabilsin)
        public string? ProjectCount { get; set; }   // Örn: 20+
        public string? ExperienceYear { get; set; } // Örn: 3+ Yıl
        public string? CustomerCount { get; set; }  // Örn: 10+
    }
}