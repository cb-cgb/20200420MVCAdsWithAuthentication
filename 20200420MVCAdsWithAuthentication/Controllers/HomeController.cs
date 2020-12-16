using AdsAuth.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using _20200420MVCAdsWithAuthentication.Models;

namespace _20200420MVCAdsWithAuthentication.Controllers
{
    public class HomeController : Controller
    {
        

        private string _conn = @"Data Source=.\sqlExpress;Initial Catalog=AdsAuthentication;Integrated Security=True;";

        //find the userId of user currently logged in
        private int GetCurrentUser() 
        {
            var db = new DBManager(_conn);
            var user = User.Identity.Name;

            if (user == null)
            {
                return 0;
            }

            return db.GetUserByEmail(user).UserId;
        }

        public IActionResult Index()
        {
            if (TempData["success-message"] != null)
            {
                ViewBag.Message = TempData["success-message"];
            }


            DBManager db = new DBManager(_conn);
            HomeViewModel vm = new HomeViewModel();

            vm.Ads = db.GetAds();
            vm.CurrentUser = GetCurrentUser();

            if (vm.Ads is null)
            {
                TempData["Message"] = $"No Ads found for  user {User.Identity.Name}";
                return Redirect("/Home/Index");
            }

            return View(vm);
        }

          public IActionResult MyAds()
        {

            if (!User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "You  must log in to view Ads.";
               // return Redirect("/Account/LoginForm");
            }
            DBManager db = new DBManager(_conn);
            var Ads =  db.GetAdsById(GetCurrentUser());

            //if (Ads is null)
            //{ 
          
            //    TempData["Message"] = $"No Ads found for  user {User.Identity.Name}";
            //}

            return View(Ads);

        }

        [Authorize]
        public IActionResult NewAdForm ()
        {
            return View();
        }


        [Authorize] 
        [HttpPost]
        public IActionResult AddAd (Ad Ad)
        {
            DBManager db = new DBManager(_conn);
            Ad.UserId = GetCurrentUser();
            db.AddAd(Ad);

            TempData["success-message"] = $"New Ad {Ad.Title} Posted!";

            return Redirect("/");

        }


        [Authorize] 
        [HttpPost]
        public IActionResult DeleteAd(int Id)
        {
            DBManager db = new DBManager(_conn);
            db.DeleteAd(Id);

            TempData["success-message"] = "Post Deleted! ";

            return Redirect("/");

        }


    }
}
