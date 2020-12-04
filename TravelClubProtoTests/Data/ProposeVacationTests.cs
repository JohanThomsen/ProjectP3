using System;
using System.Collections.Generic;
using System.Text;
using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelClubProto.Pages;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using BlazorStrap;

namespace TravelClubProto.Data.Tests
{
    [TestClass()]
    public class ProposeVacationTests
    {
        [TestMethod()]
        public void DestinationDropdownOpenTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<ProposeVacation>();

            cut.Find(".btn.btn-primary.dropdown-toggle").Click();
            cut.Find(".dropdown.show");
        }

        [TestMethod()]
        public void CreateNewDestinationDropdownOpenTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<ProposeVacation>();

            cut.Find(".btn.btn-secondary.new-destination-btn").Click();
            cut.Find("div.modal-content");
        }
        [TestMethod()]
        public void AddPriceTextShown()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<ProposeVacation>();

            cut.Find("div.AddPriceButton button.btn.btn-secondary"); //Would check click, but its a button without a click? idk
            cut.Find(".textAddprice").InnerHtml = "";
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }

        public IConfiguration config = InitConfiguration();

        public void InitialiseTestContext(Bunit.TestContext ctx)
        {

            ctx.Services.AddBlazoredLocalStorage();
            ctx.Services.AddSingleton<DataAccessService>();
            ctx.Services.AddSingleton<IConfiguration>(config);
            ctx.Services.AddBootstrapCss();
        }
    }
}