using System;
using System.Collections.Generic;
using System.Text;
using MuratOnNet.Email;

namespace EmailSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // uncomment following line to send e-mail sync.
            // SendEmail();

            // uncomment following line to send e-mail async.
            // SendAsyncEmail();
        }


        static void SendEmail()
        {
            // set network settings of your SMTP server
            string host = "SMTP_SERVER_HOST_ADDRESS";
            int port = 0; // "SMTP_SERVER_IP";
            string userName = "YOUR_USER_NAME";
            string password = "YOUR_PASSWORD";
            bool enableSsl = false; // true
            EmailSettings.NetworkSettings networkSettings = new EmailSettings.NetworkSettings(
                                                                host, port, userName, password, enableSsl);

            // create and e-mail
            Email email = new Email();
            // set properties
            email.NetworkSettings = networkSettings;
            email.From = "YOUR_EMAIL_ADDRESS";
            email.To = new string[] { "EMAIL_ADDRESS_1", "EMAIL_ADDRESS_2", "EMAIL_ADDRESS_2", "..." };
            email.Subject = "EMAIL_SUBJECT";
            email.Body = "EMAIL_BODY";
            // send e-mail
            email.Send();

            // show message
            // we can see this message when e-mail is transmitted
            ShowMessage("Email has been sent!");

        }


        // async. token for e-mail
        private static int _asyncToken = 12345;

        static void SendAsyncEmail()
        {
            // set network settings of your SMTP server
            string host = "SMTP_SERVER_HOST_ADDRESS";
            int port = 0; // "SMTP_SERVER_IP";
            string userName = "YOUR_USER_NAME";
            string password = "YOUR_PASSWORD";
            bool enableSsl = false; // true
            EmailSettings.NetworkSettings networkSettings = new EmailSettings.NetworkSettings(
                                                                host, port, userName, password, enableSsl);

            // create and e-mail
            Email email = new Email();
            // set properties
            email.NetworkSettings = networkSettings;
            email.From = "YOUR_EMAIL_ADDRESS";
            email.To = new string[] { "EMAIL_ADDRESS_1", "EMAIL_ADDRESS_2", "EMAIL_ADDRESS_2", "..." };
            email.Subject = "EMAIL_SUBJECT";
            email.Body = "EMAIL_BODY";
            // set async properties and events
            email.IsAsync = true;
            email.Token = _asyncToken;
            email.SendCompleted += new Email.SendCompletedEventHandler(email_SendCompleted);
            // send e-mail
            email.Send();

            // show message
            // we can see this message immediately
            ShowMessage("Sending email...");
        }

        // when message is transmitted
        static void email_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

            if ((int)e.UserState == _asyncToken)
            {
                // create message
                string message = string.Format("Email {0} has been sent!", e.UserState);
                // show message
                // we can see this message when e-mail is transmitted
                ShowMessage(message);
            }

        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
    }
}
