using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 4,
                    Name = "Johny",
                    Department = Dept.HR,
                    Email = "Johny@mail.pl"

                },
                new Employee
                {
                    Id = 5,
                    Name = "Iza",
                    Department = Dept.HR,
                    Email = "Iza@mail.pl"

                }

                );
        }
    }
}
