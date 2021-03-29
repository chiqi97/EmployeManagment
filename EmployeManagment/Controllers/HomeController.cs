using EmployeManagment.Models;
using EmployeManagment.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.Controllers
{
    
    // ustawienia sciezki, najlepsze poniewaz gdy zmienimy nazwe nie musimy nic zmieniac
    //[Route("[controller]/[action]")]
    public class HomeController:Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository,
                              IWebHostEnvironment hostingEnviroment)
        {
            _hostingEnvironment = hostingEnviroment;
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
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // If the Photos property on the incoming model object is not null and if count > 0,
                // then the user has selected at least one file to upload

                if (model.Photos != null && model.Photos.Count > 0)
                {
                    // Loop thru each selected file
                    foreach (IFormFile photo in model.Photos)
                    {
                        // The file must be uploaded to the images folder in wwwroot
                        // To get the path of the wwwroot folder we are using the injected
                        // IHostingEnvironment service provided by ASP.NET Core
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                        // To make sure the file name is unique we are appending a new
                        // GUID value and and an underscore to the file name
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        // Use CopyTo() method provided by IFormFile interface to
                        // copy the file to wwwroot/images folder
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
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
