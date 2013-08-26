using System;
using System.Net.Mail;

namespace MuratOnNet.Email
{
    public class EmailSettings
    {
        public class NetworkSettings
        {
            /// <summary>
            /// The name or IP address of the host computer used for SMTP transactions.
            /// </summary>
            public string Host { get; set; }
            public Int32 Port { get; set; }
            /// <summary>
            /// The user name associated with the credentials. 
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// The password associated with the credentials. 
            /// </summary>
            public string Password { get; set; }
            /// <summary>
            /// Use Secure Sockets Layer (SSL) to encrypt the connection.
            /// </summary>
            public bool EnableSsl { get; set; }

            public NetworkSettings(string host, int port, string userName, string password, bool enableSsl)
            {
                this.Host = host;
                this.Port = port;
                this.UserName = userName;
                this.Password = password;
                this.EnableSsl = enableSsl;
            }
        }
    }

    public class Email
    {
        #region Properties
        /// <summary>
        /// A user-defined object that is passed to the method invoked when the asynchronous operation completes.(use when IsAsync = true)
        /// </summary>
        public object Token { get; set; }
        /// <summary>
        /// Network settings of SMTP
        /// </summary>
        public EmailSettings.NetworkSettings NetworkSettings { get; set; }
        /// <summary>
        /// From address for e-mail message
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Recipients of e-mail message
        /// </summary>
        public string[] To { get; set; }
        /// <summary>
        /// CC recipients of e-mail message
        /// </summary>
        public string[] CC { get; set; }
        /// <summary>
        /// Bcc recipients of e-mail message
        /// </summary>
        public string[] Bcc { get; set; }
        /// <summary>
        /// Subject of e-mail message
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// E-mail message body
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// If true : Mail message body is in Html.
        /// </summary>
        public bool IsBodyHtml { get; set; }
        /// <summary>
        /// If true : Do not block the calling thread when sending e-mail message.
        /// </summary>
        public bool IsAsync { get; set; }
        #endregion

        #region Events
        public delegate void SendCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
        /// <summary>
        /// Occurs when an asynchronous e-mail send operation completes.
        /// </summary>
        public event SendCompletedEventHandler SendCompleted;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes an instance of the Email class
        /// </summary>
        public Email() { }

        /// <summary>
        /// Initializes an instance of the Email class
        /// </summary>
        /// <param name="networkSettings">Network settings of SMTP</param>
        /// <param name="from">From address for e-mail message</param>
        /// <param name="to">Recipients of e-mail message</param>
        /// <param name="subject">Subject of e-mail message</param>
        /// <param name="body">E-mail message body</param>
        public Email(EmailSettings.NetworkSettings networkSettings, string from, string[] to, string subject, string body)
            : this(Guid.NewGuid(), networkSettings, from, to, subject, body, false, false) { }

        /// <summary>
        /// Initializes an instance of the Email class
        /// </summary>
        /// <param name="token">A user-defined object that is passed to the method invoked when the asynchronous operation completes.(use when IsAsync = true)</param>
        /// <param name="networkSettings">Network settings of SMTP</param>
        /// <param name="from">From address for e-mail message</param>
        /// <param name="to">Recipients of e-mail message</param>
        /// <param name="subject">Subject of e-mail message</param>
        /// <param name="body">E-mail message body</param>
        /// <param name="isBodyHtml">If true : Mail message body is in Html.</param>
        /// <param name="isAsync">If true : Do not block the calling thread when sending e-mail message.</param>
        public Email(object token, EmailSettings.NetworkSettings networkSettings, string from, string[] to, string subject, string body, bool isBodyHtml, bool isAsync)
        {
            this.Token = token;
            this.NetworkSettings = networkSettings;
            this.From = from;
            this.To = to;
            this.Subject = subject;
            this.Body = body;
            this.IsBodyHtml = isBodyHtml;
            this.IsAsync = IsAsync;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sends the e-mail message to an SMTP server for delivery. 
        /// </summary>
        public void Send()
        {
            if (this.IsValid())
            {
                // create mail message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(this.From);
                for (int i = 0; i < this.To.Length; i++)
                {
                    mail.To.Add(new MailAddress(this.To[i]));
                }
                if (this.CC != null && this.CC.Length > 0)
                {
                    for (int i = 0; i < this.CC.Length; i++)
                    {
                        mail.CC.Add(new MailAddress(this.CC[i]));
                    }
                }
                if (this.Bcc != null && this.Bcc.Length > 0)
                {
                    for (int i = 0; i < this.Bcc.Length; i++)
                    {
                        mail.Bcc.Add(new MailAddress(this.Bcc[i]));
                    }
                }
                mail.Subject = this.Subject;
                mail.Body = this.Body;
                mail.IsBodyHtml = this.IsBodyHtml;

                // create smtp client
                SmtpClient smtp;
                if (this.NetworkSettings.Port != 0)
                {
                    smtp = new SmtpClient(this.NetworkSettings.Host, this.NetworkSettings.Port);
                }
                else
                {
                    smtp = new SmtpClient(this.NetworkSettings.Host);
                }
                smtp.Credentials = new System.Net.NetworkCredential(this.NetworkSettings.UserName, this.NetworkSettings.Password);
                smtp.EnableSsl = this.NetworkSettings.EnableSsl;
                // set completed event
                smtp.SendCompleted += new System.Net.Mail.SendCompletedEventHandler(smtp_SendCompleted);
                // send mail 
                if (IsAsync)
                {
                    smtp.SendAsync(mail, this.Token);
                }
                else
                {
                    smtp.Send(mail);
                }
            }
        }

        void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (SendCompleted != null)
                SendCompleted(sender, e);
        }
        #endregion

        #region Private functions
        private bool IsValid()
        {
            // check properties
            if (string.IsNullOrEmpty(this.NetworkSettings.Host))
            {
                throw new Exception("NetworkSettings.Host cannot be null or empty");
            }
            if (string.IsNullOrEmpty(this.NetworkSettings.UserName))
            {
                throw new Exception("NetworkSettings.UserName cannot be null or empty");
            }
            if (string.IsNullOrEmpty(this.NetworkSettings.Password))
            {
                throw new Exception("NetworkSettings.Password cannot be null or empty");
            }
            if (string.IsNullOrEmpty(this.From))
            {
                throw new Exception("From cannot be null or empty");
            }
            if (this.To == null || this.To.Length == 0)
            {
                throw new Exception("To cannot be null or empty");
            }
            if (string.IsNullOrEmpty(this.Subject))
            {
                throw new Exception("Subject cannot be null or empty");
            }
            if (string.IsNullOrEmpty(this.Body))
            {
                throw new Exception("Body cannot be null or empty");
            }

            return true;
        }
        #endregion
    }
}
