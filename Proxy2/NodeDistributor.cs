using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Proxy2;
using System.Web.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using ConsoleApplication7;
using Proxy;

namespace Proxy2
{
    public class NodeDistributor
    {
        private List<TableNote> tableNoteList;
        private List<TableNote> lastTableNoteList;

        public void Resharding()
        {
            if (!(Storage.countNodes.Equals(Storage.lastCountNodes)))
            {
                tableNoteList = readTable(Storage.countNodes);
                lastTableNoteList = readTable(Storage.lastCountNodes);

                for (int i = 0; i < tableNoteList.Count(); i++)
                {
                    if (tableNoteList[i].Port != lastTableNoteList[i].Port)
                    {
                        for (int j = lastTableNoteList[i].BottomLine;
                            j < lastTableNoteList[i].UpperLine; j++)
                        {
                            string content;
                            // гет к ласту
                            content = Get(lastTableNoteList[i].Port, j);
                            if (!(content.Equals("BadRequest")))
                            {
                                // пут к карренту
                                Put(content, tableNoteList[i].Port.ToString(), j.ToString());
                                // delete к ласту
                                Delete(lastTableNoteList[i].Port.ToString(), j.ToString());
                            }
                        }

                    }
                }
            }
        }

        private List<TableNote> readTable(string countNodes)
        {
            List<TableNote> tblNoteList = new List<TableNote>();
            string fileTablePath = @"table\" + countNodes + ".txt";

            if (File.Exists(fileTablePath))
            {
                foreach (var item in File.ReadLines(fileTablePath).ToList())
                {
                    string bottomLine = item.Split(' ')[0];
                    string upperLine = item.Split(' ')[1];
                    string port = item.Split(' ')[2];

                    tblNoteList.Add(new TableNote(Convert.ToInt32(bottomLine),
                        Convert.ToInt32(upperLine), Convert.ToInt32(port)));

                }
            }
            return tblNoteList;
        }

        private string Get(int port, int  key)
        {
            string content = HttpRequestSender.Get(key.ToString(), port.ToString(), "/api/values/");

            return content;
        }

        private void Put(string value, string port, string key)
        {
            var response = HttpRequestSender.Put(value, key, port, "/api/values/");
        }

        private void Delete(string port, string key)
        {
            var result = HttpRequestSender.Delete(key, port, "/api/values/");
        }
    }

}
