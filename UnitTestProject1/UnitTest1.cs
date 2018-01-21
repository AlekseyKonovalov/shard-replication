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
            int count = 1000;

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
        private string path = @"nodes\" + port + ".txt";
        static private string pathToNode = "D:\\Github\\shard-replication" +
            "\\ConsoleApplication7\\bin\\Debug\\Node.exe";

        private string baseAddress = "http://localhost:" + port + "/";
        public Dictionary<string, string> testData;

        public UnitTest1()
        {           
            testData = new TestDataGenerator().GenerateTestData();
        }

        [TestMethod]
        public void OpenConnection()
        {
            Process.Start(pathToNode, port);

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
                var response=HttpRequestSender.Put(item.Value, item.Key, port, "/api/values/");
            }
            int newSize = File.ReadAllLines(path).Length;

            Assert.AreEqual(newSize, oldSize + testData.Count);

        }

        [TestMethod]
        public void GetValues()
        {

            foreach (var item in testData)
            {
                string content = HttpRequestSender.Get(item.Key, port, "/api/values/");

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
                var response = HttpRequestSender.Put(item.Value, item.Key, port, "/api/values/");
            }

            GetValues();

        }
        [TestMethod]
        public void RemoveValues()
        {
            foreach (var item in testData)
            {
                var result = HttpRequestSender.Delete(item.Key, port, "/api/values/");
            }
            Assert.AreEqual(File.ReadAllLines(path).Length, 0);
        }

        [TestMethod]
        public void CheckRemoved()
        {
            foreach (var item in testData)
            {
                string content = HttpRequestSender.Get(item.Key, port, "/api/values/");
                Assert.AreEqual(content, "BadRequest");
            }
        }
    }


    [TestClass]
    public class UnitTest2 
    {
        static private string port = "9100";
        static private string countNodes = "2";
        static private string pathToProxy = "D:\\Github\\shard-replication\\Proxy2\\bin\\Debug\\Proxy2.exe";        
        static private string pathToNode = "D:\\Github\\shard-replication\\ConsoleApplication7\\bin\\Debug\\Node.exe";


        public Dictionary<string, string> testData;

        public UnitTest2()
        {           
            testData = new TestDataGenerator().GenerateTestData();
        }

        [TestMethod]
        public void StartProxy()
        {
            Process.Start(pathToProxy, countNodes);
        }

        [TestMethod]
        public void StartTwoNodes()
        {
            Process.Start(pathToNode, "9000");
            Process.Start(pathToNode, "9001");
        }

        [TestMethod]
        public void PutValues()
        {           
            foreach (var item in testData)
            {
                var response = HttpRequestSender.Put(item.Value, item.Key, port, "/api/nodes/");
            }
        }

        [TestMethod]
        public void GetValues()
        {
            foreach (var item in testData)
            {
                string content = HttpRequestSender.Get(item.Key, port, "/api/nodes/");

                Assert.AreNotEqual(content, "BadRequest");
                Assert.AreEqual(content, item.Value);
            }
        }

        [TestMethod]
        public void StartFourNodes()
        {
            Process.Start(pathToNode, "9000");
            Process.Start(pathToNode, "9001");
            Process.Start(pathToNode, "9002");
            Process.Start(pathToNode, "9003");
        }

        // старт прокси с решардингом
        [TestMethod]
        public void StartProxyWithReshard()
        {
            //аргументы: первый - сколько стало  нод, второй - сколько было нод
            Process.Start(pathToProxy, "4" + " " + "2");
        }        
    }

    [TestClass]
    public class UnitTest3
    {
        static private string MasterPort = "9000";
        static private string pathToNode = "D:\\Github\\shard-replication\\ConsoleApplication7\\bin\\Debug\\Node.exe";
        private string path = @"nodes\" + MasterPort + ".txt";

        public Dictionary<string, string> testData;
        List<string> slavesPorts = new List<string>();

        public UnitTest3()
        {
            testData = new TestDataGenerator().GenerateTestData();
            slavesPorts.Add("9001");
            slavesPorts.Add("9002");
        }

        [TestMethod]
        public void StartThreeNodes()
        {
            Process.Start(pathToNode, MasterPort);
            Process.Start(pathToNode, slavesPorts[0]);
            Process.Start(pathToNode, slavesPorts[1]);
        }

        [TestMethod]
        public void PutSlavesPortsInMaster()
        {

            foreach (var port in slavesPorts)
            {
                var response = HttpRequestSender.Put("", port, MasterPort, "/api/slaves/");
            }
        }

        [TestMethod]
        public void PutValuesInMaster()
        {
            int oldSize = File.ReadAllLines(path).Length;

            foreach (var item in testData)
            {
                var response = HttpRequestSender.Put(item.Value, item.Key, MasterPort, "/api/values/");
            }
            int newSize = File.ReadAllLines(path).Length;

            Assert.AreEqual(newSize, oldSize + testData.Count);
        }


        [TestMethod]
        public void GetValuesInMasterAndSlaves()
        {
            foreach (var item in testData)
            {
                string contentMaster = HttpRequestSender.Get(item.Key, MasterPort, "/api/values/");

                string contentSlave1 = HttpRequestSender.Get(item.Key, slavesPorts[0], "/api/values/");

                string contentSlave2 = HttpRequestSender.Get(item.Key, slavesPorts[1], "/api/values/");

                Assert.AreNotEqual(contentMaster, "BadRequest");
                Assert.AreNotEqual(contentSlave1, "BadRequest");
                Assert.AreNotEqual(contentSlave2, "BadRequest");

                Assert.AreEqual(contentMaster, contentSlave1);
                Assert.AreEqual(contentMaster, contentSlave2);
            }
        }
    }


}
