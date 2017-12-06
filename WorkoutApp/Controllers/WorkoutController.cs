using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WorkoutApp.Config;
using WorkoutApp.Logic;
using WorkoutApp.Models;

namespace WorkoutApp.Controllers
{
    public class WorkoutController : Controller
    {
        private IWorkoutRepository repo;

        public WorkoutController(IWorkoutRepository repo)
        {
            this.repo = repo;
        }

        // GET: Workout
        public ActionResult Workouts()
        {
            IEnumerable<Workout> workouts = repo.GetWorkouts();
            List<Workout> workoutslist = workouts.ToList();

            ViewBag.WorkoutsListCount = workoutslist.Count;
            ViewBag.WorkoutsList = workoutslist;
            return View("Workouts");
        }

        public async Task<ActionResult> Filter()
        {
            // Retrieve both inputs
            string price = Request["Price"];
            string goal = Request["Goal"];

            // Check if there has been an input to filter on
            if (price == "" && goal == "" || price == null && goal == null)
            {
                // Nothing has been selected, return the normal list
                TempData["alertMessage"] = "Kies eerst een rubriek om op te filteren";
                List<Workout> workoutslist = repo.GetWorkouts().ToList();
                ViewBag.WorkoutsList = workoutslist;
                ViewBag.WorkoutsListCount = workoutslist.Count;
                return PartialView("WorkoutsPartial");
                //return RedirectToAction("Workouts", "Workout");
            }

            WorkoutLogic logic = new WorkoutLogic();
            IEnumerable<Workout> workouts = await Task.Run(() => repo.GetWorkouts());

            if (Request.IsAjaxRequest())
            {
                // Price is empty so there has been filtered by goal
                if (string.IsNullOrEmpty(price))
                {
                    List<Workout> workoutslist = logic.FilterWorkoutsByGoal(workouts, goal);
                    ViewBag.WorkoutsListCount = workoutslist.Count;
                    ViewBag.WorkoutsList = workoutslist;
                    return PartialView("WorkoutsPartial");
                }

                // Goal is empty so there has been filtered by price
                if (string.IsNullOrEmpty(goal))
                {
                    List<Workout> workoutslist = logic.OrderWorkoutsByPrice(workouts, price);
                    ViewBag.WorkoutsListCount = workoutslist.Count;
                    ViewBag.WorkoutsList = workoutslist;
                    return PartialView("WorkoutsPartial");
                }

                // Both filters have been entered so filter by both
                if (!string.IsNullOrEmpty(goal) && !string.IsNullOrEmpty(price))
                {
                    List<Workout> workoutslist = logic.FilterWorkoutsByBoth(workouts, price, goal);
                    ViewBag.WorkoutsListCount = workoutslist.Count;
                    ViewBag.WorkoutsList = workoutslist;
                    return PartialView("WorkoutsPartial");
                }
            }


            TempData["alertMessage"] = "Er is iets fout gegaan, probeer het later opnieuw of neem contact met ons op.";
            return RedirectToAction("Workouts", "Workout");
        }

        [HttpPost]
        public ActionResult AddWorkout()
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
                        // The user has the rights to add a workout

                        // Retrieve all inputs and check them
                        string workouttitle = Request["Workouttitle"];
                        string price = Request["Workoutprice"];
                        string goal = Request["Goal"];
                        string descShort = Request["DescriptionShort"];

                        string descLong = Request.Unvalidated["DescriptionLong"];

                        if (goal != "Afvallen" && goal != "Spieropbouw" && goal != "Spierkracht" &&
                            goal != "Core")
                        {
                            TempData["alertMessage"] = "Onjuiste invoer bij doel, gelieve opnieuw te proberen";
                            return View("../Admin/WriteWorkout");
                        }

                        // Change decimal separator to a period ( . ) for converting to a decimal to work
                        price = price.Replace(",", ".");

                        decimal pricedecimal = decimal.Parse(price, NumberStyles.Any, CultureInfo.InvariantCulture);




                        // All inputs should be correct, add them into the database
                        DateTime creationdate;
                        creationdate = DateTime.Now;

                        int writer = founduser.UserID;

                        Workout workout = new Workout()
                        {
                            CreationDateTime = creationdate,
                            DescriptionLong = descLong,
                            DescriptionShort = descShort,
                            Goal = goal,
                            Price = pricedecimal,
                            WorkoutTitle = workouttitle,
                            Writer = writer
                        };

                        var context = new EFDbContext();

                        context.Workouts.Add(workout);
                        context.SaveChanges();

                        return RedirectToAction("Workouts");

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