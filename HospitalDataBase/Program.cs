using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using HospitalDataBase.Objects;
using DAL;
using BLL.Services;
using BLL.Interfaces;
using HospitalDataBase.MapProfiles;

namespace HospitalDataBase
{
    internal static class Program
    {


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var host = Host.CreateDefaultBuilder()
           .ConfigureAppConfiguration(config =>
           {
               config.SetBasePath(AppContext.BaseDirectory);
               config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
           })
           .ConfigureServices((context, services) =>
           {
               // Database
               var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

               services.AddDbContext<HospitalDbContext>(options =>
                   options.UseSqlServer(connectionString));

               services.AddAutoMapper(cfg =>
                   {
                       cfg.LicenseKey = "NoKey";
                   },
                   typeof(AppointmentProfile).Assembly,
                   typeof(DoctorProfile).Assembly,
                   typeof(PatientProfile).Assembly,
                   typeof(RecipeProfile).Assembly
               );

               // DI
               services.AddScoped<IUnitOfWork, UnitOfWork>();
               services.AddScoped<IAppointmentService, AppointmentService>();
               services.AddScoped<IDoctorService, DoctorService>();
               services.AddScoped<IPatientService, PatientService>();
               services.AddScoped<IRecipeService, RecipeService>();

               services.AddScoped<MainMenu>();
           })
           .Build();

            // Start the application
            ApplicationConfiguration.Initialize();

            var mainForm = host.Services.GetRequiredService<MainMenu>();
            Application.Run(mainForm);
        }
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
    }
}