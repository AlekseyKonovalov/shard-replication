using System;
using Microsoft.Owin.Hosting;
using System;
using System.Threading;
using System.Net.Http;

namespace Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            Storage.countNodes = args[0];
            //Storage.countNodes = "4";
            Storage.defaultPort = 9000;

            //adress proxy
            string baseAddress = "http://localhost:" + (Storage.defaultPort+100).ToString() + "/";
            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                for (; ; ) ;
            }
        }
    }
}
