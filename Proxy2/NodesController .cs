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

namespace Proxy
{
    public class NodesController : ApiController
    {
        private string fileTablePath= @"table\" + Storage.countNodes + ".txt";
        private List<TableNote> TableNoteList = new List<TableNote>();

        /*
        private int shard2(int key)
        {
            return key % Convert.ToInt32(Storage.countNodes);
        }  
        */      

        private void readAll()
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
            readAll();
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
            //string currentPort = (Storage.defaultPort + shard(Convert.ToInt32(id))).ToString();
            string currentPort = shard(Convert.ToInt32(id)).ToString();

            HttpClient client = new HttpClient();
            var response = client.GetAsync("http://localhost:" + currentPort + "/api/values/" + id).Result;
            var tmp = response.StatusCode;
            if (response.StatusCode.ToString() != "OK")
            {
                return System.Net.HttpStatusCode.BadRequest.ToString();
            }
            else
                return JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
        }


        // PUT api/nodes/5 
        public void Put(string id, [FromBody]string value)
        {
            //string currentPort = (Storage.defaultPort + shard2(Convert.ToInt32(id))).ToString();
            string currentPort = shard(Convert.ToInt32(id)).ToString();


            HttpClient client = new HttpClient();
            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(value));
            jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json"); ;
            var response = client.PutAsync("http://localhost:" + currentPort + "/api/values/" + id,
              jsonContent
               ).Result;

            //dictionary.Add(id, currentPort);
            //writeNote(id, currentPort);

        }

        public void Delete(string id)
        {
            string currentPort = shard(Convert.ToInt32(id)).ToString();
            //string currentPort = (Storage.defaultPort + shard(Convert.ToInt32(id))).ToString();
            var result = new HttpClient().DeleteAsync("http://localhost:" + currentPort + "/api/values/" + id).Result;
        }



    }
}
