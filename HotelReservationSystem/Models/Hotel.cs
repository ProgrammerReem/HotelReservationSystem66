using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationSystem.Models
{
    public class Hotel
    {
        //id -  name -   image -            desc - list<Room> - loc - rating 
        //int   string  (IFormFile-string) string    int       string  int
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public IFormFile? ImagFile { get; set; }
        public string? Imagepath { get; set; }
        public string? Desc { get; set; }

        public string Loc { get; set; }
        public int Rating { get; set; }
        public int Count { get; set; }

        public List<Room> rooms { get; set; }
        public List<Testimonial> testimonials { get; set; }


    }

}
