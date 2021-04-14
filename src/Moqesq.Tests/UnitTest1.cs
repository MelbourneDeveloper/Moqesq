using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

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
            var serviceProvider = Ext.BuildServiceProviderFor<SomeClass>();
            var someClass = serviceProvider.GetRequiredService<SomeClass>();
            var test = serviceProvider.GetRequiredService<Mock<ITest>>();
            someClass.Bla();
            test.Verify(t => t.DoTestThing(), Times.Once);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var func = new Func<SomeClass, Task<string>>((someClass) => Task.FromResult(someClass.Bla2()))
                .PerformTest(
                (container) => container.GetRequiredMock<ITest>().Setup(t => t.GetAString()).Returns("123"),
                (result, container) => Assert.AreEqual("123", result));

        }

        [TestMethod]
        public void TestMethod4()
        {
            var func = new Func<SomeClass, Task<string>>((someClass) 
                => Task.FromResult(someClass.Bla2()))
                .PerformTest(
                (result, container) => Assert.AreEqual(null, result));

        }

    }

}
