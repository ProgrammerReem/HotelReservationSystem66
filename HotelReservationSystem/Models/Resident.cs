using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationSystem.Models
{
    public class Resident
    {
        public int Id { get; set; }
        [ForeignKey(nameof(user))]
        public int UserID { get; set; }
        [ForeignKey(nameof(room))]

        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }


        public User user { get; set; }
        public Room room { get; set; }
    }
}
