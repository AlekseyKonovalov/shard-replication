using System;
using Microsoft.Owin.Hosting;
using System;
using System.Threading;
using System.Net.Http;
using Proxy2;

namespace Proxy
{
    class Program
    {
        //первый аргумент количество нод, второй аргумент прошлое количество нод(опционален)
        static void Main(string[] args)
        {
            //Storage.countNodes = args[0];
            Storage.countNodes = "4";
            Storage.defaultPort = 9000;
            //if (args[1] != null)
            //{
                Storage.lastCountNodes = "2";
                NodeDistributor nodeDistributor = new NodeDistributor();
                nodeDistributor.Resharding();
                
            //}


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
