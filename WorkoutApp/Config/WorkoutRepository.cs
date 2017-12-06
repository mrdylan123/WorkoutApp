using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkoutApp.Models;

namespace WorkoutApp.Config
{
    public class WorkoutRepository : IWorkoutRepository
    {
        EFDbContext context = new EFDbContext();

        public IEnumerable<Workout> GetWorkouts()
        {
            return context.Workouts;
        }
    }
}