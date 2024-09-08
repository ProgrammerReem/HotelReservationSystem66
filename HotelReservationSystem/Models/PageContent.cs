using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationSystem.Models
{
    public class PageContent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string AboutUs { get; set; }
        public string AboutUsTitle { get; set; }

        public string AboutUsImagePath { get; set; }

        [NotMapped]

        public IFormFile? AboutUsImageFile { get; set; }

    }
}
