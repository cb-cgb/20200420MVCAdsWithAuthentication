using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AdsAuth.Data;
using _20200420MVCAdsWithAuthentication.Models;




namespace _20200420MVCAdsWithAuthentication.Controllers
{
    public class AccountController : Controller
    {
        private string _conn = @"Data Source=.\sqlExpress;Initial Catalog=AdsAuthentication;Integrated Security=True;";


        public IActionResult SignUpForm()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Signup(User u)
        {
            var db = new DBManager(_conn);
            var user = db.GetUserByEmail(u.Email);
            if (user != null) // user already exists
            {
                TempData["Message"] = "A user is already registered with this email. Please log in using this email or register with a new email";
                return Redirect("/Account/LoginForm");
            }
           
            db.AddUser(u);

            TempData["Message"] = $"Thank you for signing up! You are registered as {u.Email}. Please log in to proceed.";

            return Redirect("/Account/LoginForm");//find the userId of user currently logged in
           
        
       }
            

        public IActionResult LoginForm()
        { 
           if (TempData["Message"] != null)
           {
                ViewBag.Message = TempData["Message"];
           }

            return View();

        }

        [HttpPost]
        public IActionResult Login(User u)
        {

            var db = new AdsAuth.Data.DBManager(_conn);

            User user = db.Login(u);

            if (user is null)
            {
                TempData["Message"] = "Invalid email or password credentials. Please try again!";
                return Redirect("/Account/LoginForm");
            }

            //credentials are correct, log the user in  . this creates the "ASPNetCore" cookie. 
            var claims = new List<Claim>
            {
                new Claim("user", u.Email)
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/Home/Index");

        }

        public IActionResult Logout(User u)
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync().Wait(); //logs out the user
            }

            return Redirect("/Account/LoginForm");
        }
    }
}
