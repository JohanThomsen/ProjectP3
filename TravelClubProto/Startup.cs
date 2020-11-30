using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TravelClubProto.Data;
using BlazorStrap;
using Blazored.LocalStorage;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace TravelClubProto

{
    public class Startup
    {
        private static Timer aTimer;
        private static Timer HourTimer;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            aTimer = new Timer();
            UpdateDatabaseForDeadline(aTimer);
            HourTimer = new Timer();
            PriceAgentCheck(HourTimer);
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
            aTimer.Interval = 60000;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckForDeadlines();
            Console.WriteLine($"Looping {e.SignalTime}");
        }

        private async void CheckForDeadlines()
        {
            DataAccessService DaService = new DataAccessService(Configuration);
            List<Vacation> vacs = await DaService.GetAllVacations(DaService);

            foreach (Vacation vacation in vacs)
            {
                if ((vacation.Dates["Deadline"] < DateTime.Now) && (vacation.State == "Published"))
                {
                    vacation.State = "GracePeriod";
                    EmailCreator("GracePeriod");
                }
                else if ((vacation.Dates["GracePeriodLength"] < DateTime.Now) && (vacation.State == "GracePeriod"))
                {
                    vacation.State = "Deadline";
                    EmailCreator("Deadline");
                }
            }
        }
        private void PriceAgentCheck(Timer hourTimer)
        {
            aTimer.Interval = 3600000;
            aTimer.Elapsed += OnTimedEventHour;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private void OnTimedEventHour(object source, ElapsedEventArgs e)
        {
            UpdatePriceAgents();
        }

        private void UpdatePriceAgents()
        {
            DataAccessService DaService = new DataAccessService(Configuration);
            List<Customer> customers = DaService.GetAllCustomers().GetAwaiter().GetResult();

            foreach (Customer customer in customers)
            {
                customer.priceAgentManager.GatherVacations();
            }
        }

        private void EmailCreator(string statechange)
        {
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("from_address@example.com"));
            email.To.Add(MailboxAddress.Parse("to_address@example.com"));
            email.Subject = "Test Email Subject";
            email.Body = new TextPart(TextFormat.Html) { Text = $"<h1>The Vacation you have favourited has changed to {statechange}</h1>" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("mortimer55@ethereal.email", "bTsRwKbZBsxzt588RB");
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}
