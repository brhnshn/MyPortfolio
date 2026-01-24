using MyPortfolio.Entities.Concrete.Base;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class AdminUser : BaseEntity
    {
        [Required, StringLength(50)]
        public string UserName { get; set; }

        [Required, StringLength(250)]
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;
    }
}