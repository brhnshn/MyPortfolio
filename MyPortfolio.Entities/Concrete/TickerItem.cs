using MyPortfolio.Entities.Concrete.Base;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class TickerItem : BaseEntity
    {
        [Required, StringLength(150)]
        public string Text { get; set; }

        [StringLength(50)]
        public string IconClass { get; set; } = "fas fa-star";

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
