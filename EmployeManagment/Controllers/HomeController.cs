using EmployeManagment.Models;
using EmployeManagment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.Controllers
{
    
    // ustawienia sciezki, najlepsze poniewaz gdy zmienimy nazwe nie musimy nic zmieniac
    //[Route("[controller]/[action]")]
    public class HomeController:Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        //public string Index()
        //{



        //    return _employeeRepository.GetEmployee(2).Name;
        //}
        // ustawienia sciezki 
        //[Route("")]
        //[Route("~/")]
        //[Route("/pragim")]

        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        //Znaki zaptania = jesli null to podaj do id 1
        //[Route("{id?}")]
        public ViewResult Details(int? id )
        {
            //Employee model = _employeeRepository.GetEmployee(1);
            //ViewData["Employee"] = model;
            //ViewData["PageTitle"] = "Employee Details";

            HomeDetailsViewModel homeDetailsViewMode = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id??1),
                PageTitle="Employee Details"

            };

            //ViewBag.PageTitle = "Employee Details";

            return View(homeDetailsViewMode); // Regular solution - the same name as the action method
            //return View("MyViews/Test.cshtml"); // Solution with source 
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee newEmployee = _employeeRepository.Add(employee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
    }
}
