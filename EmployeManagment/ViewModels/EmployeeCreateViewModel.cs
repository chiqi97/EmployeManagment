using EmployeManagment.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.ViewModels
{
    public class EmployeeCreateViewModel
    {
      
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot have more than 50 characters")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Office Email")]
        public string Email { get; set; }
        [Required]
        //znak zapytania mowi, ze enum dept jest opcjonalny
        public Dept? Department { get; set; }
        //Typ pozwalajacy na odczytywanie zdjec
        public List<IFormFile> Photos { get; set; }
    }
}
