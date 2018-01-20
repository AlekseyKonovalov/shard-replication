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
using Proxy;

namespace Proxy2
{
    public class NodeDistributor
    {
        private List<TableNote> tableNoteList;
        private List<TableNote> lastTableNoteList;
        

        public NodeDistributor()
        {
            if (!(Storage.countNodes.Equals(Storage.lastCountNodes)))
            {
                tableNoteList = readTable(Storage.countNodes);
                lastTableNoteList = readTable(Storage.lastCountNodes);

                for (int i = 0; i < tableNoteList.Count(); i++){
                    if(tableNoteList[i].Port != lastTableNoteList[i].Port)
                    {
                        for (int j = lastTableNoteList[i].BottomLine; 
                            j< lastTableNoteList[i].UpperLine; j++)
                        {
                            // гет к ласту
                            // пут к карренту
                            // delete к ласту
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
    }
}
