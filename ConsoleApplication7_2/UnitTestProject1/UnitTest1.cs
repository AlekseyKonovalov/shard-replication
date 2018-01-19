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
    public class UnitTest1 : Sender
    {
        static private string port = "9000";
        static private string countNodes = "2";
        
        static private string pathToProxy = "D:\\Github\\sharding replication\\ConsoleApplication7_2\\Proxy2\\bin\\Debug\\Proxy2.exe";
        static private string pathToNode = "";

        private string path = @"nodes\"+port+".txt";
        public Dictionary<string, string> testData;

        public UnitTest1()
        {
            baseAddress = "http://localhost:"+port+"/";
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
                Put(item.Key, item.Value);
            }
            int newSize = File.ReadAllLines(path).Length;

            Assert.AreEqual(newSize, oldSize + testData.Count);

        }

        [TestMethod]
        public void GetValues()
        {

            foreach (var item in testData)
            {
                string content = Get(item.Key);
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
                Put(item.Key, item.Value);
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

    /*
    [TestClass]
    public class UnitTest2 : Sender
    {
        
        [TestMethod]
        public void StartTwoNodes()
        {
            Process.Start("Node.exe", "9001");
            Process.Start("Node.exe", "9002");
        }

    }
    */
}
