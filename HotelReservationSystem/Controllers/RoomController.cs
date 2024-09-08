using HotelReservationSystem.Models;
using HotelReservationSystem.Models.Data;
using HotelReservationSystem.ViewModel.Hotel;
using HotelReservationSystem.ViewModel.Room;
using HotelReservationSystem.ViewModel.testimonials;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;


namespace HotelReservationSystem.Controllers
{
    public class RoomController : Controller

    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public RoomController(ModelContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            var room = _context.room.Include(x => x.hotel).Include(x => x.Residents).Include(x => x.reservations).Include(x => x.testimonials).ToList();

            return View(room);
        }
        [HttpGet]
        public IActionResult Add()
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

            var model = new RoomVM()
            {
                Hotels = _context.hotels.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(model);

        }
        [HttpPost]
        public IActionResult Add(RoomVM roomVM)
        {


            //check room id in same hotel?
            var existRoom = _context.room.Where(x => x.roomNumber == roomVM.roomNumber && x.HotelId == roomVM.hotelId).FirstOrDefault();
            if (existRoom != null)
            {
                ViewBag.RoomNumber = "Thier Already Room with this number in this hotel..";
                return View(roomVM);
            }

            var room = new Room()
            {
                Desc = roomVM.Desc,
                HotelId = roomVM.hotelId,
                ImagePath = SaveImage(roomVM.ImageFile),
                PriceByNight = roomVM.PriceByNight,
                roomNumber = roomVM.roomNumber,
                RoomType = roomVM.RoomType,
                CreatedAt = DateTime.Now
            };

            _context.room.Add(room);
            _context.SaveChanges();
            return RedirectToAction("index");

        }
        public IActionResult Delete(int id)
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


            //room
            var room = _context.room.FirstOrDefault(x => x.id == id);

            //remove
            _context.Remove(room);
            _context.SaveChanges();
            return RedirectToAction("index");

        }

        [HttpGet]
        public IActionResult Edit(int id)
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


            var room = _context.room.Include(x => x.hotel).FirstOrDefault(x => x.id == id);
            //Room(DB)==>RoomVm(View)
            var model = new RoomVM()
            {
                Desc = room.Desc,
                hotelId = room.HotelId,
                ImagePath = room.ImagePath,
                PriceByNight = room.PriceByNight,
                roomNumber = room.roomNumber,
                RoomType = room.RoomType,
                id = room.id,
                HotelName = room.hotel.Name,
                Hotels = _context.hotels.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(RoomVM room)
        {
            //Room => Db
            var DbRoom = _context.room.Find(room.id);
            //check Room Number
            var RoomNumberCheck = _context.room
                .Where(x => x.id != room.id && x.roomNumber == room.roomNumber && x.HotelId == room.hotelId)
                .FirstOrDefault();
            if (RoomNumberCheck != null)
            {
                room.Hotels = _context.hotels.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                ViewBag.RoomNumber = "This Room Number is already in hotel ! ";
                return View(room);
            }
            //vm=>room
            DbRoom.roomNumber = room.roomNumber;
            DbRoom.Desc = room.Desc;
            DbRoom.HotelId = room.hotelId;
            DbRoom.PriceByNight = room.PriceByNight;
            DbRoom.RoomType = room.RoomType;
            //Vm=> file
            if (room.ImageFile != null)
            {
                DbRoom.ImagePath = SaveImage(room.ImageFile);
            }
            //update
            _context.Update(DbRoom);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Details(int id, string? viewBagValue = null)
        {
            var room = _context.room.Include(x => x.testimonials).ThenInclude(x => x.user).FirstOrDefault(x => x.id == id);
            List<Testimonial> tt = new List<Testimonial>();
            foreach (var item in room.testimonials)
            {
                if (item.Status == true)
                {
                    tt.Add(item);
                }
            }
            room.testimonials = tt;
            //room - test - Addtest -Enroll
            var TestVM = new AddTestimonialVM()
            {
                RoomId = id
            };
            var ReservVM = new ReservationVM()
            {
                roomid = id
            };
            var model = new RoomDetailsVM()
            {
                room = room,
                resveration = ReservVM,
                testimonialsVM = TestVM

            };
            if (viewBagValue == "C")
            {
                ViewBag.CardError = "CardId or Card Cvv is invalid";

            }
            if (viewBagValue == "D")
            {
                ViewBag.DateError = "Please Enter Valid Date !!";

            }
            if (viewBagValue == "B")
            {
                ViewBag.BalanceError = "You Don't Have money ..";

            }
            return View(model);
        }
        [HttpPost]
        //enroll - addTest - add rating for hotel
        public IActionResult Enroll(ReservationVM resveration)
        {
            //user id
            var user = GetUser();
            if (user == null)
            {
                return RedirectToAction("Register", controllerName: "Account");
            }
            //card handle
            if (user.CardID != resveration.CardId || user.CardCVV != resveration.CardCvv)
            {


                return RedirectToAction("Details", new { id = resveration.roomid, viewBagValue = "C" });
            }
            //valid hisory
            //min => 1-1-1
            if (!resveration.checkIn.Equals(DateTime.MinValue) && !resveration.checkOut.Equals(DateTime.MinValue))
            {
                var room = _context.room.FirstOrDefault(x => x.id == resveration.roomid);
                //Balance?
                var totalCount = ((resveration.checkOut - resveration.checkIn).TotalDays) * (double)room.PriceByNight;
                //check balance for user according to total price
                var userBalance = user.Balance - (int)totalCount;
                //check if userBalance is bigger than zero or not?
                if (userBalance < 0)
                {
                    //return Erro Message
                    return RedirectToAction("Details", new { id = resveration.roomid, viewBagValue = "B" });

                }
                //change user balance
                user.Balance -= (int)totalCount;
                _context.Update(user);
                //obj reserv
                var reservation = new Reservation()
                {
                    CheckOut = resveration.checkOut,
                    CheckIn = resveration.checkIn,
                    CreatedAt = DateTime.Now,
                    RoomId = resveration.roomid,
                    UserID = user.Id
                };
                _context.Add(reservation);

                //userT
                var userTransaction = new UserTransactions()
                {
                    CreatedAt = DateTime.Now,
                    Price = (int)totalCount,
                    RoomId = resveration.roomid,
                    UserID = user.Id,
                };
                _context.Add(userTransaction);
                _context.SaveChanges();
                //send email
                SendEmail(user.Email, room.roomNumber, (decimal)totalCount);
                return RedirectToAction("Index", "Home");
                //new onject from reser
                //save 
            }

            return RedirectToAction("Details", new { id = resveration.roomid, viewBagValue = "D" });


        }


        public IActionResult GetAll(GetAllRoomsVM getAll)
        {
            //querable vs Ienumtable
            var rooms = _context.room.Include(x => x.hotel).ToList();
            //rooms
            //search
            if (!string.IsNullOrWhiteSpace(getAll.Search))
            {
                //r==
                rooms = rooms.Where(x => x.hotel.Name.Contains(getAll.Search)).ToList();
            }
            //to
            if (getAll.To != null && getAll.From == null)
            {
                rooms = rooms.Where(x => x.CreatedAt <= getAll.To).ToList();

            }
            //from
            if (getAll.From != null && getAll.To == null)
            {
                rooms = rooms.Where(x => x.CreatedAt >= getAll.From).ToList();

            }
            //from - to
            if (getAll.From != null && getAll.To != null)
            {
                rooms = rooms.Where(x => x.CreatedAt >= getAll.From && x.CreatedAt <= getAll.To).ToList();
            }
            var x = rooms.ToList();
            var model = new GetAllRoomsVM()
            {
                Rooms = x
            };

            return View(model);
        }

        public void SendEmail(string EmailSentTo, int roomNumber, decimal price)
        {
            //google developer 
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_configuration["SendEmail:FromName"], _configuration["SendEmail:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(EmailSentTo));
            email.Subject = "Room Request";
            var htmlPage = $"<h1>Room Request in website</h1><p>Good morning .. you sucessfully send request for room number:{roomNumber} the total price is : {price}$ . </p>";
            email.Body = new TextPart(TextFormat.Html) { Text = htmlPage };
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, true);
            smtp.Authenticate(userName: _configuration["SendEmail:FromEmail"], password: _configuration["SendEmail:Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);
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




    }
}
