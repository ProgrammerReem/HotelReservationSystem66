namespace HotelReservationSystem.ViewModel.Room
{
    public class ReservationVM //Enroll
    {
        //roomid - checkin - check out 
        public int roomid { get; set; }
        public DateTime checkIn { get; set; }
        public DateTime checkOut { get; set; }
        public int CardId { get; set; }
        public int CardCvv { get; set; }

    }
}
