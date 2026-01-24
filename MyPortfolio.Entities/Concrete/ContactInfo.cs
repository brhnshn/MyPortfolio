using MyPortfolio.Entities.Concrete.Base;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class ContactInfo : BaseEntity
    {
        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public string MapUrl { get; set; } // Google Maps 'embed' linki için
    }
}