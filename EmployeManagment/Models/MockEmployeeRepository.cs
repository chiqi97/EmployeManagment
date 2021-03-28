using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() {Id=1,Name="Mary",Department=Dept.HR,Email="mary@mail.pl"},
                new Employee() {Id=2,Name="Tom",Department=Dept.IT,Email="Tom@mail.pl"},
                new Employee() {Id=3,Name="John",Department=Dept.IT,Email="John@mail.pl"}
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id=_employeeList.Max(x => x.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList; 
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == Id);
        }
    }
}
