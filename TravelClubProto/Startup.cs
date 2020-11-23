using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TravelClubProto.Data;
using BlazorStrap;
using Blazored.LocalStorage;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace TravelClubProto

{
    public class Startup
    {
        private static Timer aTimer;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            aTimer = new Timer();
            UpdateDatabaseForDeadline(aTimer);
            //CheckForDeadlines();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<DataAccessService>();
            services.AddBootstrapCss();
            services.AddBlazoredLocalStorage();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void UpdateDatabaseForDeadline(Timer aTimer)
        {
            aTimer.Interval = 10000;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private async void CheckForDeadlines()
        {
            DataAccessService DaService = new DataAccessService(Configuration);
            List<Vacation> vacs = await DaService.GetAllVacations(DaService);

            foreach (Vacation vacation in vacs)
            {
                Console.WriteLine(vacation);
                if ((vacation.Dates["Deadline"] < DateTime.Now) && (vacation.State == "Published"))
                {
                    vacation.State = "GracePeriod";
                }
                else if ((vacation.Dates["GracePeriodLength"] < DateTime.Now) && (vacation.State == "GracePeriod"))
                {
                    vacation.State = "Completed";
                }
                /*else if ((vacation.Dates["Deadline"] < DateTime.Now) && (vacation.State == "Proposed"))
                {
                    vacation.State = "Rejected";
                }*/
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckForDeadlines();
            Console.WriteLine($"Looping {e.SignalTime}");
        }


    }
}
