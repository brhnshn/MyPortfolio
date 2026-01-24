using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(30, ErrorMessage = "Başlık en fazla 30 karakter olabilir.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(100, ErrorMessage = "Açıklama en fazla 100 karakter olabilir.")] // Kısıtlama burada
        public string Description { get; set; }

        public string IconUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}