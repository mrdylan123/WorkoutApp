using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkoutApp.Models
{
    [Table("Workout")]
    public class Workout
    {

        public Workout() {}

        [Key]
        public int WorkoutID { get; set; }

        public string WorkoutTitle { get; set; }
        public string DescriptionShort { get; set; }

        [AllowHtml]
        public string DescriptionLong { get; set; }

        public string Goal { get; set; }
        public decimal Price {get; set; }
        public DateTime CreationDateTime { get; set; }
        public int Writer { get; set; }
    }
}