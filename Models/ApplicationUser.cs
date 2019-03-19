using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Soc.Models
{
        public class ApplicationUser : userValidate
        {
            public string Email { get; set; }
            public string ConfirmationToken { get; set; }
            public bool IsConfirmed { get; set; }
        }
}