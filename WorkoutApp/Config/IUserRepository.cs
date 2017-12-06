using System.Collections.Generic;
using WorkoutApp.Models;

namespace WorkoutApp.Config
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
    }
}
