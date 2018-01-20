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

namespace Proxy
{
    public class NodesController : ApiController
    {
  
        private int shard(int key)
        {
            //return key / 2;
            return key % Convert.ToInt32(Storage.countNodes);
        }

        // GET api/values/5 
        public string Get(string id)
        {
            string currentPort = (Storage.defaultPort + shard(Convert.ToInt32(id))).ToString();

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


        // PUT api/values/5 
        public void Put(string id, [FromBody]string value)
        {
            string currentPort = (Storage.defaultPort + shard(Convert.ToInt32(id))).ToString();

            HttpClient client = new HttpClient();
            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(value));
            jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json"); ;
            var response = client.PutAsync("http://localhost:" + currentPort + "/api/values/" + id,
              jsonContent
               ).Result;

        }

        public void Delete(string id)
        {
            string currentPort = (Storage.defaultPort + shard(Convert.ToInt32(id))).ToString();
            var result = new HttpClient().DeleteAsync("http://localhost:" + currentPort + "/api/values/" + id).Result;
        }

    }
}
