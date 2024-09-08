using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.ViewModel.Room
{
    public class RoomVM
    {
        public int id { get; set; }
       // [Range(0,10)] 
        public int roomNumber { get; set; }
        [Required]
       
        public string RoomType { get; set; }
        public new List<string>? RoomTypes { get; set; } = new List<string>() { "Single",  "Suite" , "Meeting" };
        public int PriceByNight { get; set; }
        [Required]
        public string Desc { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImagePath { get; set; }
        public IEnumerable<SelectListItem>? Hotels { get; set; }
        public int hotelId { get; set; }
        public string HotelName { get; set; }


    }
}
