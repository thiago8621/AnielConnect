using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Dal.Service
{
    public static class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }

        public static IConfiguration Build { get; }

        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            Build = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("build.json")
                    .Build();
        }
    }
}
