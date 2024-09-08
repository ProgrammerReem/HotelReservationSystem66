using HotelReservationSystem.Models;

namespace HotelReservationSystem.ViewModel.PageContent
{
    public class MainPageVM
    {
        public Models.PageContent PageContent { get; set; }
        public List<Models.Hotel> hotels { get; set; }
        public List<Testimonial> testimonials { get; set; }
        public ContactUsVM contactUs { get; set; }
    }
}
