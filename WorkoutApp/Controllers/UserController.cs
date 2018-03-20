using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WorkoutApp.Config;
using WorkoutApp.Logic;
using WorkoutApp.Models;

namespace WorkoutApp.Controllers
{
    public class UserController : Controller
    {
        private IUserRepository repo;

        public UserController(IUserRepository repo)
        {
            this.repo = repo;
        }

        // GET: Register
        // Reached when an user wants to register, so check if he isn't logged in first (USE HASH)
        public ViewResult Register(string error)
        {
            // Check if user is not already logged in first
            if (Session["User"] != null)
            {
                return View("../Home/Index");
            }

            LoginLogic loginLogic = new LoginLogic();

            // Retrieve the user input
            string email = Request["email"];
            string username = Request["Registerusername"];
            string birthdatestring = Request["dateofbirth"];
            string gender = Request["gender"];
            string password = Request["Registerpassword"];
            string name = Request["Name"];

            // Check if account with same username/email already exists
            IEnumerable<User> allUsers = repo.GetUsers();

            Boolean usernameexists = loginLogic.UsernameAlreadyExists(allUsers, username);
            if (usernameexists == true)
            {
                ViewBag.errorRegister = "Er bestaat al een account met deze gebruikersnaam";
                return View("Login");
            }

            Boolean emailexists = loginLogic.EmailAlreadyExists(allUsers, email);
            if (emailexists == true)
            {
                ViewBag.errorRegister = "Er bestaat al een account met dit emailadres";
                return View("Login");
            }

            // Start building the User class and insert it into the database

            // First, hash the password to prevent storing the password in plain text
            // 1: Create the salt value with a cryptographic PRNG
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // 2: Create the Rfc2898DeriveBytes and get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 2000);
            byte[] hash = pbkdf2.GetBytes(20);

            // 3: Combine the salt and password bytes for later use
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // 4: Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);


            // Convert date string to datetime object
            DateTime birthdate;
            birthdate = DateTime.ParseExact(birthdatestring, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            DateTime creationdate;
            creationdate = DateTime.Now;

            // Assign all new users the accounttype visitor, untill an admin hands it extra rights if needed
            const string accounttype = "bezoeker";

            // Create the user object
            User newUser = new User()
            {
                Email = email,
                Username = username,
                BirthDate = birthdate,
                Name = name,
                Gender = gender,
                Password = savedPasswordHash,
                CreationDate = creationdate,
                Accounttype = accounttype
            };

            var context = new EFDbContext();

            context.Users.Add(newUser);
            context.SaveChanges();

            // Log the user in with the new account
            Session["User"] = newUser;
            return View("Dashboard");
        }

        // GET: Login Page
        // Requested when an user wants to register / login
        public ViewResult LoginView(string error)
        {
            // Check if user is not already logged in before showing the page
            if (Session["username"] != null && (string)Session["username"] != "")
            {
                return View("Dashboard");
            }

            return View("Login");
        }

        // GET: Dashboard
        // Requested when an user tries to log in, so check user credentials
        public ActionResult Dashboard(string error)
        {
            LoginLogic loginLogic = new LoginLogic();

            // Retrieve user input
            string username = Request["Loginusername"];
            string password = Request["Loginpassword"];

            // Check if user was already logged in
            if (Session["User"] != null)
            {
                User founduser = (User)Session["User"];
                if (founduser.Username != "")
                {
                    return View("Dashboard");
                }
                TempData["alertMessage"] = "Log in om deze pagina te bezoeken";
                return View("../Home/Index");
            }

            // Check if there was any input, else send back
            if (username == null || password == null)
            {
                //ViewBag.errorlogin = "Log in om deze pagina te bezoeken";
                TempData["alertMessage"] = "Log in om deze pagina te bezoeken";
                return View("Login");
            }

            // Get all users
            IEnumerable<User> allUsers = repo.GetUsers();

            // Check if the input matches an account
            Boolean b = false;
            b = loginLogic.CheckLogin(allUsers, username, password);

            // No right username - password combination
            if (b != true)
            {
                //ViewBag.errorlogin = "Incorrect username or password";
                TempData["alertMessage"] = "Onjuiste gebruikersnaam of wachtwoord";
                return View("Login");
            }

            // User input is correct, get the user and log it in
            User loggedinuser = loginLogic.GetUser(allUsers, username);
            Session["User"] = loggedinuser;
            
            return View("Dashboard");
        }

        // GET: Logout
        // Requested when an user tries to log out, so abandon the session and redirect to index page.
        public ActionResult LogOut(string error)
        {
            LoginLogic loginlogic = new LoginLogic();

            // Check if the user somehow wasnt logged in in the first place
            if (Session["User"] == null)
            {
                return View("../Home/Index");
            }

            // Now log the user out
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}