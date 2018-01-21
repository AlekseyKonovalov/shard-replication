using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using Proxy2;
using ConsoleApplication7;

namespace Proxy
{
    public class NodesController : ApiController
    {
        private string fileTablePath= @"table\" + Storage.countNodes + ".txt";
        private List<TableNote> TableNoteList = new List<TableNote>();

        private void readTable()
        {
            if (File.Exists(fileTablePath)){
                foreach (var item in File.ReadLines(fileTablePath).ToList())
                {
                    string bottomLine = item.Split(' ')[0];
                    string upperLine = item.Split(' ')[1];
                    string port = item.Split(' ')[2];

                    TableNoteList.Add(new TableNote(Convert.ToInt32(bottomLine),
                        Convert.ToInt32(upperLine), Convert.ToInt32(port)));

                }
            }
        }

        private int shard(int key)
        {
            readTable();
            foreach(var item in TableNoteList)
            {
                if (key >= item.BottomLine && key<=item.UpperLine)
                {
                    return item.Port;
                }
            }
            return 9000;
        }

        // GET api/nodes/5 
        public string Get(string id)
        {
            string currentPort = shard(Convert.ToInt32(id)).ToString();

            string content = HttpRequestSender.Get(id, currentPort, "/api/values/");
            return content;
        }


        // PUT api/nodes/5 
        public void Put(string id, [FromBody]string value)
        {
            string currentPort = shard(Convert.ToInt32(id)).ToString();
            var response = HttpRequestSender.Put(value, id, currentPort, "/api/values/");
        }

        public void Delete(string id)
        {
            string currentPort = shard(Convert.ToInt32(id)).ToString();
            var result = HttpRequestSender.Delete(id, currentPort, "/api/values/");
        }

    }
}
