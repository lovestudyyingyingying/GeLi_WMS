using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GeLiService_WMS;
using GeLi_Utils.Utils.AGVUtils;
using System.Collections.Generic;
using System.Linq;
using GeLiService_WMS.Threads;

namespace 单元测试
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        //public void TestGetMapInfo()
        //{
        //    AGVOrderHelper aGVOrderHelper = new AGVOrderHelper("121.5.2.81:7001");
        //   var a =  aGVOrderHelper.GetMapInfo();
        //    var aa = a.data.Where(u=>u.type>=40).Select(u=>u.name).ToArray();
        //    Assert.AreEqual("", string.Join(",", aa));
        //}

        //[TestMethod]
        //public void getOnlineAgv()
        //{
        //    AGVOrderHelper aGVOrderHelper = new AGVOrderHelper("121.5.2.81:7001");
        //    var a = aGVOrderHelper.GetOnlineAgv();
        //    Assert.AreEqual(200, a.code);
        //}
        //[TestMethod]
        //public void taskQuery()
        //{
        //    AGVOrderHelper aGVOrderHelper = new AGVOrderHelper("121.5.2.81:7001");
        //    var a = aGVOrderHelper.TaskQuery("12345678");
        //    Assert.AreEqual(200, a.code);
        //}
        //[TestMethod]
        //public void ChangeFormat()
        //{
        //    int D1000 = 12312;//从数组中取得D1000的值；
        //    string stateToBinary = Convert.ToString(D1000, 2).PadLeft(3, '0');
        //    string state = "123";
        //    Assert.AreEqual('3', state[0]);
        //}
        //[TestMethod]
        //public void TestCreateTask()
        //{
        //    AGVOrderHelper aGVOrderHelper = new AGVOrderHelper("121.5.2.81:7001");
        //    var a = aGVOrderHelper.CreateTask("101",new List<string> { "500001", "500002" },null);
        //    Assert.AreEqual(200, a.code);
        //}
        [TestMethod]
        public void GroupMissionThread1()
        {
            GroupMissionThread groupMissionThread = new GroupMissionThread();
            groupMissionThread.Control();
        }
        
    }
}
