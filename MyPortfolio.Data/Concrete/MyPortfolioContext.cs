using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // BU EKLENDİ
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Data.Concrete
{
    // DİKKAT: Artık DbContext değil, IdentityDbContext<AppUser, AppRole, int> kullanıyoruz.
    public class MyPortfolioContext : IdentityDbContext<AppUser, AppRole, int>
    {
        // Constructor yapını koruyoruz, gayet doğru.
        public MyPortfolioContext(DbContextOptions<MyPortfolioContext> options) : base(options)
        {
        }

        // Veritabanı Tablolarımız
        public DbSet<About> Abouts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<SiteSettings> SiteSettings { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<TickerItem> TickerItems { get; set; } = null!;
        public DbSet<NavbarItem> NavbarItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity tablolarının düzgün oluşması için bu satır ŞARTTIR.
            base.OnModelCreating(modelBuilder);
        }
    }
}