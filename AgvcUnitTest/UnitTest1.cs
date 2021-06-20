using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using AgvUtility;
using Messages.Parser;
using RobotDefine;
using RobotFactory;
using Xunit;
using Xunit.Abstractions;

namespace AgvcUnitTest
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TestCommandParser()
        {
            var mqMessage =
                "TX501I                      001BL$WMS202                                        BL        N    A               LKXLJBT01 01        DJSLJBT01 01                            10105114601764                  ";

            var command = MessageParser.Parse(mqMessage);
            Assert.NotNull(command);
        }

        [Fact]
        public void TestWS()
        {

            //VirtualRobotManager.TryRefreshMRStatus("MR01");
            //VirtualRobotManager.GetMRStatusSync("MR01");
           var mrList= VirtualRobotManager.ReadMrListFromIm();
           if (mrList.Any())
           {
               mrList.ForEach(mrid =>
               {
                   VirtualRobotManager.AddVirtualRobot(new VirtualRobot
                   {
                       MRStatus = new MRStatus
                       {
                           MRID = mrid
                       }
                   });
               });
               VirtualRobotManager.TryRefreshMRStatus();
               Thread.Sleep(2000);
               foreach (var robot in VirtualRobotManager.VirtualRobots)
               {
                   _testOutputHelper.WriteLine(robot.MRStatus.ToJson());
               }
                //úy‘á»ŒÑ’
                var mqMessage =
                    "TX501I                      001BL$WMS202                                        BL        N    A               LKXLJBT01 01        DJSLJBT01 01                            10105114601764                  ";

                var message = MessageParser.Parse(mqMessage);
                var  taskEngine =  new RobotTaskEngine();
                taskEngine.TransferMessage(message);

           }
        }
    }
}
