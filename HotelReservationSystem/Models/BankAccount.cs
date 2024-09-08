using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationSystem.Models
{
    public class BankAccount
    {
        //userid-Card-Cvv-Balance
        public int Id { get; set; }
        //fk
        [ForeignKey(nameof(user))]
        public int UserId { get; set; }
        public int CardId { get; set; } // 212 2233 
        public int Cvv { get; set; }
        public int Balance { get; set; } // 10 k$




        public User user { get; set; }



    }
}
