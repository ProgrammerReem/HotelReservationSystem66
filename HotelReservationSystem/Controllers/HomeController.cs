using HotelReservationSystem.Models.Data;
using HotelReservationSystem.ViewModel.PageContent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;

        public HomeController(ModelContext context)
        {

            _context = context;
        }

        public IActionResult Index()
        {
            var conatcUs = new ContactUsVM();
            var test = _context.testimonials.Include(x => x.user).Include(x => x.room).ThenInclude(x => x.hotel).Where(x => x.Status == true).ToList();
            var pageContent = _context.pageContent.FirstOrDefault();
            if (pageContent.AboutUsImagePath == null)
            {
                pageContent.AboutUsImagePath = "ee";
            }

            var model = new MainPageVM()
            {
                hotels = _context.hotels.ToList(),
                contactUs = conatcUs,
                testimonials = test,
                PageContent = _context.pageContent.FirstOrDefault()
            };

            return View(model);
        }


    }
}
