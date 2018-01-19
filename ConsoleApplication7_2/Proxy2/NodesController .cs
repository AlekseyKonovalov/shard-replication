using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Proxy
{
    class NodesController
    {
        private Dictionary<string, string> dictionaryDB = new Dictionary<string, string>();

        private int shard(int key)
        {
            return key/Convert.ToInt32(Storage.countNodes);
        }       

        private void readAll(string currentPort)
        {
            string filePath= @"nodes\" + currentPort + ".txt";
            foreach (var item in File.ReadLines(filePath).ToList())
            {
                string key = item.Split(' ')[0];
                string value = item.Split(' ')[1];
                dictionaryDB.Add(key, value);
            }
        }

        private void writeAll(string currentPort)
        {
            string filePath = @"nodes\" + currentPort + ".txt";
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                foreach (var item in dictionaryDB)
                {
                    writer.Write(item.Key + " ");
                    writer.WriteLine(item.Value);
                }
            }
        }


        // GET api/values/5 
        public string Get(string id)
        {
            string currentPort = (Storage.defaultPort + shard(Convert.ToInt32(id))).ToString(); 
            readAll(currentPort) ;
            return dictionaryDB[id];
        }


        // PUT api/values/5 
        public void Put(string id, [FromBody]string value)
        {
            string currentPort = (Storage.defaultPort + shard(Convert.ToInt32(id))).ToString();
            readAll(currentPort);
            if (!dictionaryDB.ContainsKey(id))
            {
                dictionaryDB.Add(id, value);
            }
            else
            {
                dictionaryDB[id] = value;
            }
            writeAll(currentPort);

        }

        public void Delete(string id)
        {
            string currentPort = (Storage.defaultPort + shard(Convert.ToInt32(id))).ToString();
            readAll(currentPort);
            if (dictionaryDB.ContainsKey(id))
            {
                dictionaryDB.Remove(id);
            }
            writeAll(currentPort);
        }

    }
}
