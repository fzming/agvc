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

        // [Fact]
        // public void TestCommandParser()
        // {
        //     var mqMessage =
        //         "TX501I                      001BL$WMS202                                        BL        N    A               LKXLJBT01 01        DJSLJBT01 01                            10105114601764                  ";
        //
        //     var command = MessageParser.Parse(mqMessage);
        //     Assert.NotNull(command);
        // }

        [Fact]
        public void TestWS()
        {
            //VirtualRobotManager.TryRefreshMRStatus("MR01");
            //VirtualRobotManager.GetMRStatusSync("MR01");
        }
    }
}