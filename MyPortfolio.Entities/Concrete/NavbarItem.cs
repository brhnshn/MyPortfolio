using MyPortfolio.Entities.Concrete.Base;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class NavbarItem : BaseEntity
    {
        [Required, StringLength(50)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string SectionId { get; set; } = string.Empty;

        [StringLength(50)]
        public string? IconClass { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
