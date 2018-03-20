using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutApp.Models
{
    [Table("User")]
    public class User
    {
        public User () {}

        [Key]
        public int UserID { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Accounttype { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime BirthDate { get; set; }
    }
}