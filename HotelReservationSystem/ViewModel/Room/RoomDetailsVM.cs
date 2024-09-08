using HotelReservationSystem.ViewModel.testimonials;

namespace HotelReservationSystem.ViewModel.Room
{
    public class RoomDetailsVM
    {
        public Models.Room room { get; set; }
        public ReservationVM resveration { get; set; }
        //Testimonial 
        public AddTestimonialVM testimonialsVM { get; set; }
    }
}
