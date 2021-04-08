using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Moqesq.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Ext.FromCtors<SomeClass>(sc => sc.Bla())
                .GetRequiredMock<ITest>()
                .Verify(t => t.DoTestThing(), Times.Once);
        }
    }

}
