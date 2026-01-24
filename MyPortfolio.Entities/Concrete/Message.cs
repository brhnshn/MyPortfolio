using MyPortfolio.Entities.Concrete.Base;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class Message : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; } // Bunu ekledim (Hata almamak için)

        [Required]
        public string Content { get; set; } // Mesaj içeriği

        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
    }
}