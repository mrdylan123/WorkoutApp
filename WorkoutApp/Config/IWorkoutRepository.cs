using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;

namespace WorkoutApp.Config
{
    public interface IWorkoutRepository
    {
        IEnumerable<Workout> GetWorkouts();
    }
}
