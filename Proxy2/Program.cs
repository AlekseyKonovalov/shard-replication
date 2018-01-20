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
        //первый аргумент количество нод, 
        //второй аргумент прошлое количество нод(опционален), 
        //нужен если хотим сделать решардинг

        static void Main(string[] args)
        {
            Storage.countNodes = args[0];
            //Storage.countNodes = "4";
            //Storage.defaultPort = 9000;
            
            if (args[1] != null)
            {
                Storage.lastCountNodes = args[1];
                Thread ReshardThread = new Thread(resharding);
                ReshardThread.Start(); 

            }


            //adress proxy
            string baseAddress = "http://localhost:9100/";
            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                for (; ; ) ;
            }
        }

        private static void resharding()
        {
            NodeDistributor nodeDistributor = new NodeDistributor();
            nodeDistributor.Resharding();
        }
    }
}
