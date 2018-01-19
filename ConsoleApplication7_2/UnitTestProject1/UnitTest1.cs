using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using ConsoleApplication7;
using System.Web.Http.SelfHost;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.Owin.Hosting;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTestProject1
{
    public class TestDataGenerator
    {
        public Dictionary<string, string> GenerateTestData()
        {
            Dictionary<string, string> testData=new Dictionary<string, string>();
            int count = 100;
            for (int i = 0; i < count; i++)
            {
                testData.Add(i.ToString(), (i*10).ToString());
            }
            return testData;
        }        
    }

    [TestClass]
    public class UnitTest1 
    {
        static private string port = "9000";
        static private string countNodes = "2";
        static private string baseAddress = "http://localhost:" + port + "/";


        static private string pathToProxy = "D:\\Github\\shard-replication\\ConsoleApplication7_2\\Proxy2\\bin\\Debug\\Proxy2.exe";
        static private string pathToNode = "D:\\Github\\shard-replication\\ConsoleApplication7_2\\ConsoleApplication7\\bin\\Debug\\Node.exe";

        private string path = @"nodes\"+port+".txt";
        public Dictionary<string, string> testData;

        public UnitTest1()
        {           
            testData = new TestDataGenerator().GenerateTestData();
        }

        [TestMethod]
        public void OpenConnection()
        {
            Process.Start(pathToProxy, countNodes);

        }
        [TestMethod]
        public void ClearDB()
        {
            File.WriteAllText(path, string.Empty);
            Assert.AreEqual(new FileInfo(path).Length, 0);
        }

        

        [TestMethod]
        public void PutValues()
        {
            int oldSize = File.ReadAllLines(path).Length;
           
            foreach (var item in testData)
            {
                HttpClient client = new HttpClient();
                var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(item.Value));
                jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json"); ;
                var response = client.PutAsync("http://localhost:" + port + "/api/values/" + item.Key,
                  jsonContent
                   ).Result;

            }
            int newSize = File.ReadAllLines(path).Length;

            Assert.AreEqual(newSize, oldSize + testData.Count);

        }

        [TestMethod]
        public void GetValues()
        {

            foreach (var item in testData)
            {
                string content;
                               
                HttpClient client = new HttpClient();
                var response = client.GetAsync("http://localhost:" + port + "/api/values/" + item.Key).Result;
                var tmp = response.StatusCode;
                if (response.StatusCode.ToString() != "OK")
                {
                    content= System.Net.HttpStatusCode.BadRequest.ToString();
                }
                else
                    content= JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                                
                Assert.AreNotEqual(content, "BadRequest");
                Assert.AreEqual(content, item.Value);
            }

        }

        [TestMethod]
        public void UpdateValues()
        {
            int oldSize = File.ReadAllLines(path).Length;
            
            foreach (var item in testData)
            {
                HttpClient client = new HttpClient();
                var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(item.Value));
                jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json"); ;
                var response = client.PutAsync("http://localhost:" + port + "/api/values/" + item.Key,
                  jsonContent
                   ).Result;
            }

            GetValues();

        }
        /*
        [TestMethod]
        public void RemoveValues()
        {
            foreach (var item in testData)
            {
                Delete(item.Key);
            }
            Assert.AreEqual(File.ReadAllLines(path).Length, 0);
        }
        

        [TestMethod]
        public void CheckRemoved()
        {
            foreach (var item in testData)
            {
                var value = Get(item.Key);
                Assert.AreEqual(Get(item.Key), "BadRequest");
            }
        }
        */
    }

    
    [TestClass]
    public class UnitTest2 
    {
        static private string pathToNode = "D:\\Github\\shard-replication\\ConsoleApplication7_2\\ConsoleApplication7\\bin\\Debug\\Node.exe";
        [TestMethod]
        public void StartTwoNodes()
        {
            Process.Start(pathToNode, "9001");
            Process.Start(pathToNode, "9002");
        }

    }
    
}
