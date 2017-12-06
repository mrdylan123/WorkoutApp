using System.Data.Entity;
using WorkoutApp.Models;

namespace WorkoutApp.Config
{
    public class EFDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Workout> Workouts { get; set; }

        public EFDbContext() : base("EFDbContext")
        {

        }
    }
}