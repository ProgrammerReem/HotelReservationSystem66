using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationSystem.Models
{
    public class Testimonial
    {//rating
        //user - hotel - room- text
        public int Id { get; set; }
        public string text { get; set; }
        [ForeignKey(nameof(user))]
        public int userId { get; set; }
        //  [ForeignKey(nameof(hotel))]

        //public int hotelId { get; set; }
        [ForeignKey(nameof(room))]
        public int roomId { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //     public Hotel hotel { get; set; }
        public User user { get; set; }
        public Room room { get; set; }
    }
}
