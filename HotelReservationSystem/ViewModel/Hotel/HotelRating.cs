namespace HotelReservationSystem.ViewModel.Hotel
{
    public class HotelRating
    {
        //int hotel id - int rating number  - int user id 
        public int HotelId { get; set; }
        public int Rating { get; set; }
        public List<int> RatingList { get; set; } = new List<int> { 1, 2, 3, 4, 5 };
    }
}
