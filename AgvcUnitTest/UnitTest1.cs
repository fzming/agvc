using System;
using MesCommunication.Protocol;
using Xunit;

namespace AgvcUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void TestCommandParser()
        {
            var mqMessage =
                "TX501I                      001BL$WMS202                                        BL        N    A               LKXLJBT01 01        DJSLJBT01 01                            10105114601764                  ";

            var command = CommandParser.Parse(mqMessage);
            Assert.NotNull(command);
        }
    }
}
