using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    internal class TestConfiguration
    {
        public static IConfiguration GetConfiguration()
        {
            var configuration = new ConfigurationBuilder();
            System.Reflection.Assembly assem =
                    System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream fs =
                         assem.GetManifestResourceStream("TestProject.binary.appsettings.json"))
            {
                configuration.AddJsonStream(fs);
                return configuration.Build();
            }
        }
    }
}
