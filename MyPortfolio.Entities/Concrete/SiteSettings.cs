using MyPortfolio.Entities.Concrete.Base;
using System.ComponentModel.DataAnnotations;

#nullable enable

namespace MyPortfolio.Entities.Concrete
{
    public class SiteSettings : BaseEntity
    {
        [Required, StringLength(100)]
        public string SiteTitle { get; set; } = "My Portfolio";

        [StringLength(500)]
        public string? MetaDescription { get; set; }

        [StringLength(200)]
        public string? FooterText { get; set; }

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [StringLength(500)]
        public string? FaviconUrl { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(30)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? FormspreeId { get; set; }

        [StringLength(500)]
        public string? GithubUrl { get; set; }

        [StringLength(500)]
        public string? LinkedinUrl { get; set; }

        [StringLength(500)]
        public string? XUrl { get; set; }

        [StringLength(500)]
        public string? InstagramUrl { get; set; }

        [Required]
        [StringLength(50)]
        public string ActiveTemplate { get; set; } = "DefaultTheme";

        [Required]
        [StringLength(7)]
        public string PrimaryColor { get; set; } = "#0d6efd";

        [Required]
        [StringLength(7)]
        public string SecondaryColor { get; set; } = "#6c757d";

        // Footer Settings
        [StringLength(100)]
        public string? FooterTitle { get; set; } = "MY PORTFOLIO";

        [StringLength(500)]
        public string? FooterDescription { get; set; } = "Kendinizi, işinizi veya hobilerinizi dünyaya tanıtın.";

        [StringLength(200)]
        public string? CopyrightText { get; set; } = "MyPortfolio";

        [StringLength(100)]
        public string? DeveloperName { get; set; } = "Burhan Şahin";

        [StringLength(500)]
        public string? DeveloperUrl { get; set; } = "#";
    }
}