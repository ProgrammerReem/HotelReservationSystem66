using HotelReservationSystem.Models;
using HotelReservationSystem.Models.Data;
using HotelReservationSystem.ViewModel.Hotel;
using HotelReservationSystem.ViewModel.Room;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers
{
    public class HotelController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;
        public HotelController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        public IActionResult Index()
        {
            var hotels = _context.hotels.Include(x => x.rooms).ToList();
            return View(hotels);//@model IEnumrable<hotels>
        }
        [HttpGet]
        public IActionResult Create()
        {
            //
            #region Admin Check ?
            //check user ? ("Admin")
            var user = GetUser();
            if (user == null)
            {
                return RedirectToAction("Register", controllerName: "Account");
            }
            if (user.Role == "user")
            {
                return RedirectToAction("index", controllerName: "Home");

            }
            #endregion


            return View();
        }
        [HttpPost]
        public IActionResult Create(HotelsVM model)
        {


            if (ModelState.IsValid)
            {

                //check Hotel Name?
                var ExistedHotel = _context.hotels.Where(x => x.Name == model.Name).FirstOrDefault();
                if (ExistedHotel != null)
                {
                    ViewBag.HotelName = "Thier is Hotel With This Name Existed !";
                    return View(model);
                }

                //user => hotel view model
                //Steps

                //new object from hotel
                //mapping HotelVM To Hotel
                var hotel = new Hotel()
                {
                    Loc = model.Loc,
                    Name = model.Name,
                    Desc = model.Desc,
                    Imagepath = SaveImage(model.ImageFile)
                };
                //Save image !!

                //Save Hotel in DB
                _context.hotels.Add(hotel);
                _context.SaveChanges();
                //database<= hotel
                return RedirectToAction("index");
            }
            //view.erro=
            return View(model);
        }
        public IActionResult Delete(int Id)
        {
            #region Admin Check ?
            //check user ? ("Admin")
            var user = GetUser();
            if (user == null)
            {
                return RedirectToAction("Register", controllerName: "Account");
            }
            if (user.Role == "user")
            {
                return RedirectToAction("index", controllerName: "Home");

            }
            #endregion


            var hotel = _context.hotels.FirstOrDefault(x => x.Id == Id);
            _context.Remove(hotel);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            #region Admin Check ?
            //check user ? ("Admin")
            var user = GetUser();
            if (user == null)
            {
                return RedirectToAction("Register", controllerName: "Account");
            }
            if (user.Role == "user")
            {
                return RedirectToAction("index", controllerName: "Home");

            }
            #endregion

           

            var hotel = _context.hotels.FirstOrDefault(x => x.Id == id);
            //View Model
            var model = new HotelsVM()
            {
                imagePath = hotel.Imagepath,
                Desc = hotel.Desc,
                Loc = hotel.Loc,
                Name = hotel.Name,
                id = id,
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(HotelsVM hotel)
        {
            var DBHotel = _context.hotels.FirstOrDefault(x => x.Id == hotel.id);
            //check name
            var checkingName = _context.hotels.Where(x => x.Id != hotel.id && x.Name == hotel.Name).FirstOrDefault();
            if (checkingName != null)
            {
                // فعلا فيه اوتيل بالاسم الجديد موجود في الداتا بيز
                ViewBag.Name = "This Name is Already for other hotel";
                return View(hotel);
            }
            var xx = _context.hotels.Where(x => x.Name == hotel.Name).ToList();
            DBHotel.Name = hotel.Name;
            var hotels = _context.hotels.ToList();
            DBHotel.Loc = hotel.Loc;
            DBHotel.Desc = hotel.Desc;
            //file image?
            if (hotel.ImageFile != null)
            {
                DBHotel.Imagepath = SaveImage(hotel.ImageFile);
            }
            _context.Update(DBHotel);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            //data about hotel
            //Rating
            //drop list - int rate - int hotel id
            //---------------------------------------------
            var rate = new HotelRating()
            {
                HotelId = id
            };
            var model = new HotelDetailsVM()
            {
                Hotel = _context.hotels.Include(x => x.rooms).FirstOrDefault(x => x.Id == id),
                HotelRating = rate

            };

            return View(model);
        }

        public IActionResult AddRating(HotelRating HotelRating)
        {
            //hotel 
            var hotel = _context.hotels.FirstOrDefault(x => x.Id == HotelRating.HotelId);
            //count++
            hotel.Count++;
            //rating(average rating)
            int newRating = (hotel.Rating * (hotel.Count - 1) + HotelRating.Rating) / hotel.Count;
            hotel.Rating = newRating;
            // save
            _context.Update(hotel);
            _context.SaveChanges();

            return RedirectToAction("Details", controllerName: "Hotel", routeValues: new { id = hotel.Id });
        }

        public IActionResult GetAll(GetAllHotelsVM getAll)
        {


            var hotels = _context.hotels.ToList();

            if (!string.IsNullOrWhiteSpace(getAll.Search))
            {
                hotels = hotels.Where(x => x.Name.Contains(getAll.Search)).ToList();
            }

            var x = hotels.ToList();
            var model = new GetAllHotelsVM()
            {
                Hotels = x
            };

            return View(model);
        }


        private string SaveImage(IFormFile file)
        {
            if (file == null)
            {
                return string.Empty;
            }
            string RootPath = _environment.WebRootPath;//== ~
            if (file != null)
            {
                string filename = Guid.NewGuid().ToString();
                var Upload = Path.Combine(RootPath, @"Images");
                var ext = Path.GetExtension(file.FileName);

                using (var filestream = new FileStream(Path.Combine(Upload, filename + ext), FileMode.Create))
                {
                    file.CopyTo(filestream);
                }
                return @"Images\" + filename + ext;
            }
            return "";
        }

        private User GetUser()
        {
            var userId = HttpContext.Session.GetString("userId");
            if (userId == null)
            {
                return null;
            }
            var user = _context.users.FirstOrDefault(x => x.Id == int.Parse(userId));
            if (user == null)
            {
                return null;
            }
            return user;
        }

    }
}
