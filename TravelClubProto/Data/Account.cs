

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelClubProto.Data
{
    abstract class Account
    {
        private DateTime       LoginDate    { get; set; }
        private string         Username     { get; set; }
        private string         Password     { get; set; }
        private string         Type         { get; set; }
        private DateTime       CreationDate { get; set; }
        public  int            ID           { get; set; }
        public  Boolean        LoggedIn     { get; set; }

        public Account(string username, string password)
        {
            CreationDate = DateTime.Now;
            Username     = username;
            Password     = password;
            Login(username, password);
        }

        public void Login(string username, string password, List customerList)
        {
            foreach(Customer customer in customerList)
            {
                if (customer.username == username && customer.password == password)
                {
                    LoginDate = DateTime.Now;
                    LoggedIn  = true;
                    Console.WriteLine("Login successful - you are now logged in!");
                }
                else
                {
                    Console.WriteLine("Login went wrong - please check your login credentials and try again!");        
                }
            }
                
        }

        public void Logout()
        {
            LoggedIn = false;
        }
    }
