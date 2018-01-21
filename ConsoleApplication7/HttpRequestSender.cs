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
    public static class HttpRequestSender
    {
        public static string Get(string key, string port, string controller)
        {
            string content;
            HttpClient client = new HttpClient();
            var response = client.GetAsync("http://localhost:" + port + controller + key).Result;
            var tmp = response.StatusCode;
            if (response.StatusCode.ToString() != "OK")
            {
                content = System.Net.HttpStatusCode.BadRequest.ToString();
            }
            else
                content = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();

            return content;
        }

        public static string Put(string value, string key, string port, string controller)
        {
            HttpClient client = new HttpClient();
            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(value));
            jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json"); ;
            var response = client.PutAsync("http://localhost:" + port + controller + key,
              jsonContent
               ).Result;
            return response.ToString();
        }

        public static string Delete(string key, string port, string controller)
        {
            var result = new HttpClient().DeleteAsync("http://localhost:" + port + controller + key).Result;
            return result.ToString();
        }
    }
}
