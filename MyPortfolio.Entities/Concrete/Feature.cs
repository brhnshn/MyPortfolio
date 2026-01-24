using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Entities.Concrete
{
    public class Feature
    {
        [Key]
        public int Id { get; set; }
        public string Header { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
    }
}