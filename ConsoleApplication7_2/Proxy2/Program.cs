using System;
using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;

namespace Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            //Storage.countNodes = args[0];
            Storage.countNodes = "2";
            Storage.defaultPort = 9000;

            string baseAddress = "http://localhost:" + Storage.defaultPort.ToString() + "/";
            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                for (; ; ) ;
            }
        }
    }
}
