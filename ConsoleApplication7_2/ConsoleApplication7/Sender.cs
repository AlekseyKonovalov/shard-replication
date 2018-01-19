using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    public class Sender
    {
        
        public string baseAddress;
        public void Put(string key, string value)
        {
            HttpClient client = new HttpClient();
            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(value));
            jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json"); ;
            var response = client.PutAsync(baseAddress+"api/values/" + key,
              jsonContent
               ).Result;

        }
        public string Get(string key)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync(baseAddress + "api/values/" + key).Result;
            var tmp = response.StatusCode;
            if (response.StatusCode.ToString() != "OK")
            {
                return System.Net.HttpStatusCode.BadRequest.ToString();
            }
            else
                return JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();

        }
        public void Delete(string key)
        {
            var result = new HttpClient().DeleteAsync(baseAddress + "api/values/" + key).Result;

        }
    }
}
