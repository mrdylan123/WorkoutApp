using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using WorkoutApp.Models;

namespace WorkoutApp.Logic
{
    public class LoginLogic
    {
        public Boolean CheckLogin(IEnumerable<User> allUsers, string username, string password)
        {
            // Get all accounts with entered username
            IEnumerable<User> findUsers = allUsers.Where(u => u.Username == username);
            List<User> foundUsers = findUsers.ToList();

            // Check if there have been any found results
            if (foundUsers.Count < 1) // No results found
            {
                return false;
            }

            // There is an user with the entered username
            User foundUser = foundUsers[0];

            // Get the hashed password and compare it to the user input
            /*Fetch the stored value */
            string savedPasswordHash = foundUser.Password;
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 2000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
                    
            // Both username and password have been entered correctly
            // All checks have been passed, the user is allowed to log in
            return true;
        }

        public User GetUser(IEnumerable<User> allUsers, string username)
        {
            User foundUser = allUsers.Where(u => u.Username == username).ToList()[0];
            return foundUser;
        }

        public Boolean UsernameAlreadyExists(IEnumerable<User> allUsers, string username)
        {
            // Try to find an user in the list with the same username
            IEnumerable<User> foundUser = allUsers.Where(u => u.Username == username);

            return foundUser.Any();
        }

        public Boolean EmailAlreadyExists(IEnumerable<User> allUsers, string email)
        {
            // Try to find an user in the list with the same email
            IEnumerable<User> foundUser = allUsers.Where(u => u.Email == email);

            return foundUser.Any();
        }
    }
}