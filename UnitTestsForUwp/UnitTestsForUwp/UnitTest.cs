using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsForUwp
{
    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {
            Assert.Equal(4, 4);
        }

        [Theory]
        [InlineData(6)]
        public void TestMethod2(int value)
        {
            Assert.True(IsOdd(value));
        }

        bool IsOdd(int value)
        {
            return value % 2 == 1;
        }
    }
}
