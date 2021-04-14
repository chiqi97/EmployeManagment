using EmployeManagment.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse",controller:"Account")]
        //[ValidEmailDomain (allowedDomain: "o2.pl", 
        //    ErrorMessage ="email domain must be o2.pl")] // dozwolone maile tylko 02 utitlities
        // allow domain wysylany do validemailDomain
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name ="Confirm password")]
        [Compare("Password",
                ErrorMessage = "Password and confirmation password dont match.")]
        public string ConfirmPassword { get; set; }

        [Required]

        public string City { get; set; }

    }
}
