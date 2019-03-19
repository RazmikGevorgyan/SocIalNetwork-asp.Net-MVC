using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Soc.Models
{
    public class userValidate
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string surname { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "login length will be more than 8")]
        public string login { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "password will be consist of more that 5 and less than 30 symbols")]
        [Compare("PasswordConfirm", ErrorMessage = "Password Are not the same")]
        public string password { get; set; }
        public string PasswordConfirm { get; set; }
        public string image { get; set; }
        [Required]
        public int age { get; set; }
        public string country { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string phone { get; set; }
        [Required]
        public string gender { get; set; }
        [Required]
        public DateTime birthdate { get; set; }
    }
}