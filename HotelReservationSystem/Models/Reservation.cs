using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationSystem.Models
{
    public class Reservation // request طلب حجز
    {
        // id - RoomId - UserID - Time -    Stuts - CheckIn - CheckOut
        //      int   int    int     DateTime   string  (DateTime)

        public int Id { get; set; }
        [ForeignKey(nameof(user))]
        public int UserID { get; set; }
        [ForeignKey(nameof(room))]
        public int RoomId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool Status { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public User user { get; set; }
        public Room room { get; set; }


    }
}
