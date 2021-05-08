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

            // identity service  związane z uzytkownikiem, w tym przypadku 
            // za pomoca options.password edytujemy wymagania dotyczace hasla
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>();




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


            // Zmiana sciezki metody i widoku AccessDenied na 
            //Administration/AccessDenied(metode i widok przeniesc)
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            // Deklaracja dzialania zadan nalezy zaimplementowac rowniez przed metoda ktora chcemy chronic
            // w tym wypadku administration/DeleteRole
            // aby zadzialalo po zmianie zadania dla danego uzytkownika nalezy go przelogowac
         
            services.AddAuthorization(options =>
            {
            // mozesz usuwac jesli jestes adminem i masz dostep do zadania: delete role
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

                // litery claim.value true musza byc takie same jak w bazie, jest to bardzo wrazliwe
                // claim type nie jest wrazliwy

                options.AddPolicy("AdminRolePolicy", policy => policy
                .RequireRole("Admin", "true"));
            });


            //Singleton Instancja obiektu tworzona tylko raz, a potem wykorzystywana wiele razy przy kazdym
            //zapytaniu.


            // Scoped - mamy ta sama instancje dla danego zapytania, ale dla innego zapytania mamy nowa instancje

            // Transient - nowa instancja tworzona jest za kazdym razem podczas wywolania zapytania 
            // nawet dla tego samego zapytania lub oczywiscie innego zapytania


            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            // deklaracja do policy, rejestracja Security

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>(); 
            



            services.AddControllersWithViews(); // for views 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error"); 
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

            }
                


            //zmien domyslna nazwe strony
            //DefaultFilesOptions defaultFileOptions = new DefaultFilesOptions();
            //defaultFileOptions.DefaultFileNames.Clear();
            //defaultFileOptions.DefaultFileNames.Add("foo.html");

            ////ustaw default.html jako domyslna strone
            //app.UseDefaultFiles(defaultFileOptions);
            //app.UseStaticFiles();

            //lepsze rozwiazanie do ustawienia foo.html jako glowna strone 
            //FileServerOptions fileServerOptions = new FileServerOptions();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("foo.html");
            //app.UseFileServer(fileServerOptions);

            // app.UseFileServer();

            app.UseStaticFiles();
            app.UseRouting();



            //Autthentication middleware, koniecznie przed mvc, nalezy najpierw w metodzie on modelCreating w 
            //AppDbContext dodac, ze wykorzystujemy base.onmodelCreating(modelBuilder) a kolejno mozemy dodac
            // migracje
            app.UseAuthentication();
            app.UseAuthorization();






            app.UseEndpoints(endpoints =>
            {
                // wymagaj autoryzacji dla wszystkich stron
                endpoints.MapRazorPages().RequireAuthorization();

                // domyslna strona
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });



        }
    }
}
