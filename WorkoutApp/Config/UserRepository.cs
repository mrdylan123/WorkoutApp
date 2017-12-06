using System.Collections.Generic;
using WorkoutApp.Models;

namespace WorkoutApp.Config
{
    public class UserRepository : IUserRepository
    {
        private EFDbContext context = new EFDbContext();


        public IEnumerable<User> GetUsers()
        {
            return context.Users;
        }
    }
}