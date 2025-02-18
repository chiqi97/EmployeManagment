﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;
        public SQLEmployeeRepository(AppDbContext context)
        {
            this.context = context;

        }
        public Employee Add(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = context.Employees.Find(id);
            if (employee != null)
            {
                context.Employees.Remove(employee);
                context.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return context.Employees;
        }

        public Employee GetEmployee(int Id)
        {
            return context.Employees.Find(Id);
        }

        public IEnumerable<Employee> GetEmployeeByName(string name)
        {

                return context.Employees.Where(x => x.Name.StartsWith(name) || name == null);

        }
        public IEnumerable<Employee> GetEmployeeByDepartment(string department)
        {
            if (department != null)
            {
                return context.Employees.Where(x => x.Department == GetEnum<Dept>(department.ToUpper()) || department == null);
            }
            return context.Employees;
           
        }

  

        public Employee Update(Employee employeeChanges)
        {
            var employee =context.Employees.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return employeeChanges;
           
        }

        private static TEnum? GetEnum<TEnum>(string value) where TEnum : struct
        {
            TEnum result;

            return Enum.TryParse<TEnum>(value, out result) ? (TEnum?)result : null;
        }
    }
}
