using MyPortfolio.Entities.Concrete.Base;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class Project : BaseEntity
    {
        [Required, StringLength(150)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [StringLength(250)]
        public string ImageUrl { get; set; }

        [StringLength(250)]
        public string? GithubLink { get; set; }

        [StringLength(250)]
        public string? ProjectUrl { get; set; } 

        public int DisplayOrder { get; set; }

        [StringLength(100)]
        public string Platform { get; set; } 
    }
}