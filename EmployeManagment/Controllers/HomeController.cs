﻿using EmployeManagment.Models;
using EmployeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous] // pozwol uzytkownikom niezalogowanym
        public ViewResult Index(string searchBy, string search)
        {
            if (searchBy == "Name")
            {
                var modelByName = _employeeRepository.GetEmployeeByName(search);
                return View(modelByName);
            }
            else if(searchBy== "Department")
            {
                var modelByDepartment = _employeeRepository.GetEmployeeByDepartment(search);
                return View(modelByDepartment);
            }
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }
        public ViewResult Details(int? id )
        {

            Employee employee = _employeeRepository.GetEmployee(id.Value);

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel homeDetailsViewMode = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle="Employee Details"

            };

            //ViewBag.PageTitle = "Employee Details";

            return View(homeDetailsViewMode); // podstawowe rozwiazanie - nazwa taka jak akcja
            //return View("MyViews/Test.cshtml"); // Rozwiazanie z konkretna sciezka
        }

        [HttpGet]

        //[Authorize] -- wymagaj zalogowania
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    // zapisz nazwe pliku  obiektu pracownika
                    // ktory zostanie zapisany w bazie danych pracownikow
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if(model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                                    "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }


                _employeeRepository.Update(employee);
                return RedirectToAction("index", new { id = employee.Id });
            }

            return View();
        }

        [HttpGet]
        public  ViewResult Delete(int id)
        {
            _employeeRepository.Delete(id);
            return View("deleted");
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;



            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }



            }

            return uniqueFileName;
            
        }



    }
}
