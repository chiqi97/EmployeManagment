using EmployeManagment.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
                
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddMvcCore(options=>options.EnableEndpointRouting=false).AddXmlSerializerFormatters();

            //Singleton Instancja obiektu tworzona tylko raz a potem pracujemy na tym samym obiekcie
            // Ilosc uzytkownikow dowolna
            services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();

            // Instancja tworzona raz na zadanie w tym zakresie  jak singleton w jednym zakresie.
            // Tworzy jeden obiekt dla  żądania http i uzywa go do w innych wywolaniach, nastepne wyslanie
            //zadania stworzy jednak nowy obiekt
            //obiekt (max 4, potem sie zeruje)
            //services.AddScoped<IEmployeeRepository, MockEmployeeRepository>();

            //nowy obiekt jest tworzony zawsze kiedy zostaje wysylane zapytanie http(zawsze 3)
            //services.AddTransient<IEmployeeRepository, MockEmployeeRepository>();

            services.AddControllersWithViews(); // for views 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //// option of exception page 5 lines up 
                //DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions
                //{
                //    SourceCodeLineCount = 10
                //};
                app.UseDeveloperExceptionPage();
            }

            //change name of default page
            //DefaultFilesOptions defaultFileOptions = new DefaultFilesOptions();
            //defaultFileOptions.DefaultFileNames.Clear();
            //defaultFileOptions.DefaultFileNames.Add("foo.html");

            ////set a default.html as a main page
            //app.UseDefaultFiles(defaultFileOptions);
            //app.UseStaticFiles();

            //better way to set the foo.html as main page   
            //FileServerOptions fileServerOptions = new FileServerOptions();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("foo.html");
            //app.UseFileServer(fileServerOptions);

            // app.UseFileServer();
            app.UseStaticFiles();
            app.UseRouting();

            // konfiguruje domyslna sciezke czyli HomeController i metoda details > /home/details
            //  app.UseMvcWithDefaultRoute();

            // Domyslna sciezka taka jak dla komendy DefauultRoute przypisanie home i index wlacza domyslnie sciezke
            // Home/index po wlaczeniu programu
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            // Z tym ustawieniem mozemy ustawiac sciezke za pomoca [Route] w kontrolerze
            //app.UseMvc();


            //app.Run(async (context) =>

            //{
            //    await context.Response.WriteAsync("hello world");
            //});

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {

            //        await context.Response.WriteAsync("hello world");

            //    });

            //});


        }
    }
}
