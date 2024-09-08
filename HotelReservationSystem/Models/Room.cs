using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationSystem.Models
{
    public class Room
    {

        public int id { get; set; }
        public int roomNumber { get; set; }

        public string RoomType { get; set; }
        public int PriceByNight { get; set; }
        public string Desc { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? ImagePath { get; set; }

        [ForeignKey("hotel")]
        public int HotelId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Hotel hotel { get; set; }
        public List<Reservation> reservations { get; set; }
        public List<Resident> Residents { get; set; }
        public List<Testimonial> testimonials { get; set; }
        public List<UserTransactions> userTransactions { get; set; }


    }
}
