using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.Models
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int Id);
        IEnumerable<Employee> GetAllEmployee();
        IEnumerable<Employee> GetEmployeeByName(string name);
        IEnumerable<Employee> GetEmployeeByDepartment(string department);

        Employee Add  (Employee employee);
        Employee Update(Employee employeeChanges);
        Employee Delete(int id); 

    }
}
