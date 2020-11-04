

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelClubProto.Data
{
    public abstract class Account
    {
        private DateTime LoginDate { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private string Type { get; set; }
        private DateTime CreationDate { get; set; }
        public int ID { get; set; }
        public Boolean LoggedIn { get; set; }

        public Account(string username, string password)
        {
            CreationDate = DateTime.Now;
            Username = username;
            Password = password;
        }
    }
}
