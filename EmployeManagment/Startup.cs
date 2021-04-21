using EmployeManagment.Models;
using EmployeManagment.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//sd
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

            //services.AddDbContextPool<AppDbContext>(
            //    options => options.UseSqlServer(_config.GetConnectionString("EmployeeDbConnection")));

            services.AddDbContextPool<AppDbContext>(
             options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            // identity service  związane z uzytkownikiem 
            // options to modyfikacja 
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>();


            // Mozna tak zmienic wlasciwosci identity usr w moim przypadku to jest w rejestracji uzytkownika


            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequiredLength = 10;
            //    options.Password.RequiredUniqueChars=3;
            //    options.Password.RequireNonAlphanumeric = false;
            //})



            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // zeby dodac autenttkacje google nalezy dodac paczke authentication.google
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "1063636099489-cln1coujusdb1vd55biq9koqba9iod07.apps.googleusercontent.com";
                    options.ClientSecret = "-Q7msf37CW71dahy5zb2BqJf";
                });


            // Zmiana sciezki metody i widoku AccessDenied na Administration/AccessDenied(metode i widok przeniesc)
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            // Deklaracja dzialania Claimsow nalezy zaimplementowac rownuez przed metoda ktora chcemy chronic
            // w tym wypadku administration/DeleteRole
            // aby zadzialalo po zmianie claims dla danego uzytkownika nalezy go przelogowac
            // aby usunac role oprocz roli admina uzytkownik musid posiadac rowniez claims delete Role
            services.AddAuthorization(options =>
            {
            // mozesz usuwac jesli jestes adminem i masz claim delete role
            // lub jesli jestes super adminem
            options.AddPolicy("DeleteRolePolicy", policy => policy.RequireAssertion(context =>
           context.User.IsInRole("Admin") &&
           context.User.HasClaim(claim => claim.Type == "Delete Role" && claim.Value == "true") ||
           context.User.IsInRole("Super Admin")
           ));

                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context=>
                //context.User.IsInRole("Admin") &&
                //context.User.HasClaim(claim=>claim.Type=="Edit Role" &&claim.Value=="true" )||
                //context.User.IsInRole("Super Admin")
                //)); 

                // Z nasza wlasna modyfikacja
                options.AddPolicy("EditRolePolicy", policy =>
                        policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

                //options.InvokeHandlersAfterFailure = false;

                // litery claim.value true musza byc takie same jak w bazie
                // claim type nie jest wrazliwy 

                options.AddPolicy("AdminRolePolicy", policy => policy
                .RequireRole("Admin", "true"));
            });

            //services.AddMvcCore(options=>options.EnableEndpointRouting=false,
            //    config => {
            //        var policy = new AuthorizationPolicyBuilder()
            //                            .RequireAuthenticatedUser()
            //                            .Build();
            //        config.Filters.Add(new AuthorizeFilter(policy));
            //    }

            //        ).AddXmlSerializerFormatters();

            //Singleton Instancja obiektu tworzona tylko raz a potem pracujemy na tym samym obiekcie
            // Ilosc uzytkownikow dowolna
            //services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();

            // Instancja tworzona raz na zadanie w tym zakresie  jak singleton w jednym zakresie.
            // Tworzy jeden obiekt dla  żądania http i uzywa go do w innych wywolaniach, nastepne wyslanie
            //zadania stworzy jednak nowy obiekt
            //obiekt (max 4, potem sie zeruje) przyklad z filmu
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            // deklaracja do policy, rejestracja Security

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>(); 
            

            //nowy obiekt jest tworzony zawsze kiedy zostaje wysylane zapytanie http(zawsze 3)
            //services.AddTransient<IEmployeeRepository, SQLEmployeeRepository>();

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
            else
            {
                app.UseExceptionHandler("/Error"); 
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

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

            //Autthentication middleware, koniecznie przed mvc, nalezy pierw w metodzie on modelCreating w 
            //AppDbContext dodac, ze wykorzystujemy base.onmodelCreating(modelBuilder) a kolejno mozemy dodac
            // migracje
            app.UseAuthentication();
            app.UseAuthorization();
            // Domyslna sciezka taka jak dla komendy DefauultRoute przypisanie home i index wlacza domyslnie sciezke
            // Home/index po wlaczeniu programu



            //app.UseEndpoints(endpoints =>
            //{
            //    // Require Authorization for all your Razor Pages
            //    endpoints.MapRazorPages().RequireAuthorization();

            //    // Default page
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseEndpoints(endpoints =>
            {
                // Require Authorization for all your Razor Pages
                endpoints.MapRazorPages().RequireAuthorization();

                // Default page
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //});

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
