using HotelReservationSystem.Models;
using HotelReservationSystem.Models.Data;
using HotelReservationSystem.ViewModel.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;

        public AccountController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                //check if thier user used this email !
                var ExistedUser = _context.users.FirstOrDefault(x => x.Email == userVM.Email);
                if (ExistedUser != null)
                {
                    //view bag error will display
                    ViewBag.Email = "This Email Already Registerd";
                    return View(userVM);
                }
                //create user
                var user = new User()
                {
                    CreatedAT = DateTime.Now,
                    Email = userVM.Email,
                    Name = userVM.Name,
                    Password = userVM.Password,
                    phone = userVM.phone,
                    Balance = 10000,
                    CardID = userVM.CardID,
                    CardCVV = userVM.CardCvv
                };
                //role - image path
                if (userVM.ImageFile != null)
                {
                    user.ImagePath = SaveImage(userVM.ImageFile);
                }
                //first Account => Admin
                if (_context.users.Count() == 0)
                {
                    user.Role = "Admin";
                }
                else
                {
                    user.Role = "user";
                }
                _context.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(userVM);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(UserVM userVM)
        {
            //email => vm و pass=>vm


            var user = _context.users.FirstOrDefault(x => x.Email == userVM.Email && x.Password == userVM.Password);
            if (user != null)
            {
                //login
                HttpContext.Session.SetString("userId", user.Id.ToString());

                return RedirectToAction("index", controllerName: "Home");
            }
            var users = _context.users.ToList();
            ViewBag.error = " invalid Email Or Password";
            return View(userVM);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var user = GetUser();
            if (user == null)
            {
                return RedirectToAction("Register");
            }
            //
            var model = new UserVM()
            {
                id = user.Id,
                CardID = user.CardID,
                CardCvv = user.CardCVV,
                Email = user.Email,
                Name = user.Name,
                phone = user.phone,
                ImagePath = user.ImagePath,
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(UserVM userVM)
        {
            var user = _context.users.FirstOrDefault(x => x.Id == userVM.id);

            user.phone = userVM.phone;
            user.Name = userVM.Name;
            user.CardCVV = userVM.CardCvv;
            user.CardID = userVM.CardID;
            user.Email = userVM.Email;

            //password - image
            if (userVM.Password != null)
            {
                user.Password = userVM.Password;
            }
            if (userVM.ImageFile != null)
            {
                user.ImagePath = SaveImage(userVM.ImageFile);
            }
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("index", controllerName: "Home");
        }


        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("index", "Home");
        }


        [HttpGet]
        public IActionResult UserTransactions()
        {
            var user = GetUser();
            if (user == null)
            {
                return RedirectToAction("Register");
            }
            //join
            var userTransaction = _context.userTransactions.Include(x => x.user)
                .Include(x => x.room)
                .ThenInclude(x => x.hotel)
                .Where(x => x.UserID == user.Id).ToList();
            return View(userTransaction);
        }

        public IActionResult Users()
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


            var users = _context.users.ToList();

            return View(users);
        }



        [HttpGet]
        public IActionResult UserEmails()
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

            var users = _context.reservations.Include(x => x.user).ToList();

            return View(users);
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
