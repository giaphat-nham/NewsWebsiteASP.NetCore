﻿namespace NewsWebsite.Models
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
    }
}