using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.ViewModel.User
{
    public class UserVM
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int phone { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public IFormFile? ImageFile { get; set; }
        public int CardID { get; set; }
        public int CardCvv { get; set; }

        public string? ImagePath { get; set; }
    }
}
