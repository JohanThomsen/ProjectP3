using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Notification
    {
        public Notification(string content, List<string> emails, string type)
        {
            Content = content;
            Emails = emails;
            Type = type;
        }

        public string Content { get; set; }
        public List<string> Emails { get; set; }
        public string Type { get; set; }

        public void Execute()
        {
            foreach (string mail in Emails)
            {
                Console.WriteLine(mail);
            }
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("from_address@example.com"));
            email.To.Add(MailboxAddress.Parse("to_address@example.com"));
            email.Subject = "Test Email Subject";
            email.Body = new TextPart(TextFormat.Html) { Text = Content };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("giovanny99@ethereal.email", "tse1cnG9M2JcmVPZ6k");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
