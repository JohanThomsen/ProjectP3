using System;
using System.Collections.Generic;
using System.Text;
using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelClubProto.Shared;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using BlazorStrap;

namespace TravelClubProto.Data.Tests
{
    [TestClass()]
    public class NavMenuTests
    {
        [TestMethod()]
        public void HamburgerMenuOpenTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<NavMenu>();


            cut.Find(".btn.dropdown-toggle.oi.oi-menu").Click();
            cut.Find(".dropdown-menu.dropdown-menu-right.show");
        }

        [TestMethod()]
        public void HamburgerMenuCloseTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<NavMenu>();


            cut.Find(".btn.dropdown-toggle.oi.oi-menu").Click();
            cut.Find(".btn.dropdown-toggle.oi.oi-menu").Click();

            ElementNotFoundException ex = Assert.ThrowsException<ElementNotFoundException>(() => cut.Find(".dropdown-menu.dropdown-menu-right.show"));
            Assert.IsNotNull(ex);
        }

        [TestMethod()]
        public void LoginModalOpenTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<NavMenu>();

            cut.Find(".btn.dropdown-toggle.oi.oi-menu").Click();
            cut.Find("a.dropdown-item").Click();
            cut.Find(".modal-dialog.modal-dialog-centered");
        }

        [TestMethod()]
        public void LoginModalCloseTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<NavMenu>();

            cut.Find(".btn.dropdown-toggle.oi.oi-menu").Click();
            cut.Find("a.dropdown-item").Click();
            cut.Find("button.close").Click();

            ElementNotFoundException ex = Assert.ThrowsException<ElementNotFoundException>(() => cut.Find("div.modal.fade.show"));
            Assert.IsNotNull(ex);
        }

        [TestMethod()]
        public void CreateUserModalOpenTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<NavMenu>();

            cut.Find(".btn.dropdown-toggle.oi.oi-menu").Click();
            cut.FindAll("a.dropdown-item")[1].Click();
            cut.Find(".modal-dialog.modal-dialog-centered");
        }

        [TestMethod()]
        public void CreateUserModalCloseTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<NavMenu>();

            cut.Find(".btn.dropdown-toggle.oi.oi-menu").Click();
            cut.FindAll("a.dropdown-item")[1].Click();
            cut.Find("button.close").Click();

            ElementNotFoundException ex = Assert.ThrowsException<ElementNotFoundException>(() => cut.Find("div.modal.fade.show"));
            Assert.IsNotNull(ex);
        }

        [TestMethod()]
        public void NavBarLinkActivationTest()
        {
            using var ctx = new Bunit.TestContext();
            InitialiseTestContext(ctx);
            var cut = ctx.RenderComponent<NavMenu>();

            cut.FindAll("a.nav-link.text-dark.Nav.Bar")[1].Click();
            Assert.IsTrue(cut.FindAll("a.nav-link.text-dark.Nav.Bar")[1].ClassList.Contains("active"));
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
