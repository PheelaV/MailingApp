using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailingApp
{
    public class App
    {
        public static IConfiguration Configuration;

        public static IConfigurationSection AppSettings => Configuration?.GetSection("AppSettings");

        public App()
        {

        }
    }
}
