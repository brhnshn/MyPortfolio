using MyPortfolio.Entities.Concrete.Base;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class Skill : BaseEntity
    {
        [Required, StringLength(50)]
        public string Title { get; set; }

        [Range(0, 100)]
        public byte Percentage { get; set; }

        [Required, StringLength(50)]
        public string Category { get; set; }
    }
}