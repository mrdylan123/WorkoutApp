using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkoutApp.Config;
using WorkoutApp.Models;

namespace WorkoutApp.Logic
{
    public class WorkoutLogic
    {
        //public List<Workout> GetWorkouts()
        //{
        //    EFDbContext context = new EFDbContext();
        //    IEnumerable<Workout> workouts = context.Workouts;


        //}

        public List<Workout> FilterWorkoutsByGoal(IEnumerable<Workout> workouts, string goal)
        {
            IEnumerable<Workout> filteredworkouts = workouts.Where(w => w.Goal == goal);

            return filteredworkouts.ToList();
        }

        public List<Workout> OrderWorkoutsByPrice(IEnumerable<Workout> workouts, string price)
        {
            if (price == "up")
            {
                IEnumerable<Workout> workoutsbyprice_asc = workouts.OrderBy(w => w.Price);
                return workoutsbyprice_asc.ToList();
            }
            if (price == "down")
            {
                IEnumerable<Workout> workoutsbyprice_desc = workouts.OrderByDescending(w => w.Price);
                return workoutsbyprice_desc.ToList();
            }

            return workouts.ToList();
        }

        public List<Workout> FilterWorkoutsByBoth(IEnumerable<Workout> workouts, string price, string goal)
        {
            // First trim the list to only the objects with the defined goal
            IEnumerable<Workout> workoutsbygoal = FilterWorkoutsByGoal(workouts, goal);

            // Order them by price
            IEnumerable<Workout> workoutsresult = OrderWorkoutsByPrice(workoutsbygoal, price);

            return workoutsresult.ToList();
        }
    }
}