using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Soc.Models
{
    public class ChangePas
    {
        [Required]
        public string prevPas { get; set;}
        [Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "password will be consist of more that 5 and less than 30 symbols")]
        [Compare("PasswordConfirm", ErrorMessage = "Password Are not the same")]
        public string password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}