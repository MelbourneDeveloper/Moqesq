using Microsoft.Extensions.DependencyInjection;
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

        [TestMethod]
        public void TestMethod2()
        {
            var serviceCollection = new ServiceCollection();
            var someClass = serviceCollection.FromCtors<SomeClass>();
            var test = someClass.GetRequiredService<Mock<ITest>>();
            someClass.Bla();
            test.Verify(t => t.DoTestThing(), Times.Once);
        }


    }

}
