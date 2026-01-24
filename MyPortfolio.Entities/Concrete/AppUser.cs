using Microsoft.AspNetCore.Identity;

namespace MyPortfolio.Entities.Concrete
{
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        // Ekstra istediklerini buraya yazabilirsin.
    }
}