using Nancy.Hosting.Self;
using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;
using System;

namespace Newbe.Mahua.Plugins.Parrot
{
    class Program
    {
        static void Main(string[] args)
        {
            //var data = new InitializationData();
            //data.InitializationQQXXLEVEL();
            using (var host = new NancyHost(new Uri("http://localhost:65321")))
            {
                host.Start();
                Console.WriteLine("Started! press Enter to exit.");
                Console.ReadLine();
            }
        }
    }

}