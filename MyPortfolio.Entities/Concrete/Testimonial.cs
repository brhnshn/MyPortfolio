using MyPortfolio.Entities.Abstract;

namespace MyPortfolio.Entities.Concrete
{
    public class Testimonial : BaseEntity
    {
        public string ClientName { get; set; }   // Müşteri Adı
        public string Company { get; set; }      // Şirketi / Ünvanı
        public string Comment { get; set; }      // Yorumu
        public string ImageUrl { get; set; }     // Müşteri Fotosu
        public string? Title { get; set; }       // Başlık (Örn: Harika İş!)
    }
}