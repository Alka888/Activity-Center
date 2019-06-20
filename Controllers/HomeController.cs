using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//added these line
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using beltExam.Models;



namespace beltExam.Controllers
{
	public class HomeController : Controller
	{
		private MyContext dbContext;

		public HomeController(MyContext context)
		{
			dbContext = context;
		}
		////////////////home page////////////////
		[Route("")]
		[HttpGet]
		public IActionResult Index()
		{

			return View();
		}

		/////////////method for registration, when you hit the register button, this method runs////

		[HttpPost("register")]
		public IActionResult register(User user)
		{
			// Check initial ModelState
			if (ModelState.IsValid)
			{
				// If a User exists with provided email
				if (dbContext.Users.Any(u => u.Email == user.Email))
				{
					// Manually add a ModelState error to the Email field, with provided
					// error message
					ModelState.AddModelError("Email", "Email already in use!");
					// You may consider returning to the View at this point
					return View("Index");
				}
				PasswordHasher<User> Hasher = new PasswordHasher<User>();
				user.Password = Hasher.HashPassword(user, user.Password);
				dbContext.Add(user);
				dbContext.SaveChanges();
				HttpContext.Session.SetInt32("userInSession", user.UserId);
				return RedirectToAction("Index");
			}
			return View("Index");
		}
		// other code


		////////////this method is for login method, when you hit the login button this code runs/////////

		[HttpPost("login")]
		public IActionResult LoginUser(LoginUser userSubmission)
		{
			if (ModelState.IsValid)
			{
				// If inital ModelState is valid, query for a user with provided email
				var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
				// If no user exists with provided email
				if (userInDb == null)
				{
					// Add an error to ModelState and return to View!
					ModelState.AddModelError("Email", "Invalid Email/Password");
					return View("Index");
				}

				// Initialize hasher object
				var hasher = new PasswordHasher<LoginUser>();

				// varify provided password against hash stored in db
				var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);

				// result can be compared to 0 for failure
				if (result == 0)
				{
					ModelState.AddModelError("Password", "Invalid Password");
					return View("Index");
					// handle failure (this should be similar to how "existing email" is handled)
				}
				HttpContext.Session.SetInt32("userInSession", userInDb.UserId);

				HttpContext.Session.SetString("User", userInDb.FirstName);


				return RedirectToAction("Dashboard");
			}
			else
			{
				return View("Index");
			}

		}

		[Route("Logout")]
		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}


		[HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("userInSession") == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User newu = dbContext.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserID"));
                ViewBag.User = newu;

                ViewBag.Userid = HttpContext.Session.GetInt32("UserID");

                List<Activity> allAct = dbContext.Activities
                .Include(c => c.Users)
                .ToList();
                ViewBag.AllAct = allAct;

                // User theUser = (dbContext.Users.Where(u => u.UserId == allAct.UserId)).SingleOrDefault();

                return View();
            }
        }
		
		[Route("New")]
		[HttpGet]
		public IActionResult New()
		{
			if (HttpContext.Session.GetInt32("userInSession") == null)
				return RedirectToAction("Index");
			ViewBag.UserId = HttpContext.Session.GetInt32("userInSession");
			return View();
		}

		[Route("AddActivity")]
		[HttpPost]
		public IActionResult AddActivity(Activity Actform)
		{
			if (Actform.Date < DateTime.Now)
			{
				ModelState.AddModelError("Activity", "Activity must be in the future");
			}
			if (ModelState.IsValid)
			{
				Console.WriteLine("//////////////////////////here//////////////////");
				dbContext.Add(Actform);
				dbContext.SaveChanges();
				return RedirectToAction("Dashboard");
			}
			else
			{
				Console.WriteLine("///////////////// MODEL INVALID /////////////");
				ViewBag.errors = ModelState.Values;
				return View("New");
			}
		}


		[HttpGet]
		[Route("detail/{ActivityId}")]
		public IActionResult Activity(int ActivityId)
		{
			 Activity CurrentActivity = dbContext.Activities
			.Include(activity => activity.Users)
			.ThenInclude(User => User.User)
			.FirstOrDefault(a => a.ActivityId == ActivityId);
			ViewBag.CurrentActivity = CurrentActivity;
			ViewBag.Userid = HttpContext.Session.GetInt32("UserID");

			return View("Activity");
			
		}

		[HttpGet]
		[Route("joinact/{ActivityId}")]
		public IActionResult AddRsvp(int ActivityId)
		{
			
			Reservation add = new Reservation();
			add.UserId = (int)HttpContext.Session.GetInt32("userInSession");
			add.ActivityId = ActivityId;
			dbContext.Add(add);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
		}

		[HttpGet("cancel/{ActivityId}")]
		public IActionResult DeleteRsvp(int ActivityId)
		{
			List<Reservation> canceled = dbContext.Reservations.Where(a => a.ActivityId == ActivityId).ToList();
			Reservation first = canceled.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("userInSession"));
			dbContext.Remove(first);
			dbContext.SaveChanges();
			return RedirectToAction("Dashboard");
		}

		[HttpGet("delete/{ActivityId}")]
        public IActionResult Delete(int ActivityId)
        {
            Activity deleted = dbContext.Activities.FirstOrDefault(a => a.ActivityId == ActivityId);
            dbContext.Remove(deleted);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }



	}
}
