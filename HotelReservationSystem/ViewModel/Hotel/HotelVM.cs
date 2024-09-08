using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.ViewModel.Hotel
{

    public class HotelsVM
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? imagePath { get; set; }
        public string? Desc { get; set; }
        [Required]

        public string Loc { get; set; }


    }
}
