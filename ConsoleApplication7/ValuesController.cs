using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
 
using System.Net.Http; 
using System.Net.Http.Headers;
 


namespace ConsoleApplication7
{
    public class ValuesController : ApiController
    {
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();
       
        private void readAllNotes()
        {
            foreach (var item in File.ReadLines(Storage.filePath).ToList())
            {
                string key = item.Split(' ')[0];
                string value = item.Split(' ')[1];
                dictionary.Add(key, value);
            }
        }
        private void writeAllNotes()
        {
            using (StreamWriter writer = new StreamWriter(Storage.filePath, false))
            {
                foreach (var item in dictionary)
                {
                    writer.Write(item.Key + " ");
                    writer.WriteLine(item.Value);
                }
            }
        }
        

        // GET api/values/5 
        public string Get(string id)
        {
            readAllNotes();
            return dictionary[id];
        }


        // PUT api/values/5 
        public void Put(string id, [FromBody]string value)
        {

            readAllNotes();
            if (!dictionary.ContainsKey(id))
            {
                dictionary.Add(id, value); 
            }
            else
            {
                dictionary[id] = value;
            }
            writeAllNotes();

            //рассылка в слейвы
            if (Storage.slavesPorts.Count() > 0)
            {
                foreach(var port in Storage.slavesPorts)
                {               
                    HttpClient client = new HttpClient();
                    var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(value));
                    jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json"); ;
                    var response = client.PutAsync("http://localhost:" + port + "/api/values/" + id,
                      jsonContent
                       ).Result;
                }
            }
   


        }

        public void Delete(string id)
        {
            readAllNotes();
            if (dictionary.ContainsKey(id))
            {
                dictionary.Remove(id);
            }
            writeAllNotes();

            //рассылка в слейвы
            if (Storage.slavesPorts.Count() > 0)
            {
                foreach (var port in Storage.slavesPorts)
                {
                    var result = new HttpClient().DeleteAsync("http://localhost:" + port + "/api/nodes/" + id).Result;
                }
            }
        }

       
    }
}
