using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.ViewModels
{
    public class EditRoleViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Role name is required")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }

    }
}
