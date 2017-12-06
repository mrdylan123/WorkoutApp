using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkoutApp.Config;
using WorkoutApp.Models;

namespace WorkoutApp.Controllers
{
    public class AdminController : Controller
    {
        private IUserRepository repo;

        public AdminController(IUserRepository repo)
        {
            this.repo = repo;
        }

        // GET: AdminPanel
        public ActionResult AdminPanel()
        {
            // Check if this page is requested by a logged in user and if the user is an moderator/owner
            if (Session["User"] != null)
            {
                User founduser = (User) Session["User"];
                if (founduser.Username != "")
                {
                    // User is logged in
                    if (founduser.Accounttype == "moderator" || founduser.Accounttype == "owner")
                    {
                        // Get the total number of users to show on the admin panel
                        int totalUsers = repo.GetUsers().ToList().Count;
                        ViewBag.TotalUsers = totalUsers;
                        return View("AdminPanel");
                    }
                    TempData["alertMessage"] =
                        "Je hebt niet de rechten om deze pagina te bezoeken, als je denkt dat dit een fout is neem dan contact met ons op.";
                    return View("../Home/Index");
                }
                TempData["alertMessage"] = "Log in om deze pagina te bezoeken";
                return View("../Home/Index");
            }
            TempData["alertMessage"] = "Log in om deze pagina te bezoeken";
            return View("../Home/Index");
        }


        public ViewResult CreateWorkout()
        {
            // Check if this page is requested by a logged in user and if the user is an moderator/owner
            if (Session["User"] != null)
            {
                User founduser = (User) Session["User"];
                if (founduser.Username != "")
                {
                    // User is logged in
                    if (founduser.Accounttype == "moderator" || founduser.Accounttype == "owner")
                    {
                        // The user has the rights to continue
                        return View("WriteWorkout");
                    }
                    TempData["alertMessage"] =
                        "Je hebt niet de rechten om deze pagina te bezoeken, als je denkt dat dit een fout is neem dan contact met ons op.";
                    return View("../Home/Index");
                }
                TempData["alertMessage"] = "Log in om deze pagina te bezoeken";
                return View("../Home/Index");
            }
            TempData["alertMessage"] = "Log in om deze pagina te bezoeken";
            return View("../Home/Index");
        }
    }
}