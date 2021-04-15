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
            Ext.FromCtors<SomeClass, string>(sc => sc.Bla())
                .GetRequiredMock<ITest1>()
                .Verify(t => t.DoTestThing(), Times.Once);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var serviceProvider = Ext.BuildServiceProviderFor<SomeClass>();
            var someClass = serviceProvider.GetRequiredService<SomeClass>();
            var test = serviceProvider.GetRequiredService<Mock<ITest1>>();
            someClass.Bla();
            test.Verify(t => t.DoTestThing(), Times.Once);
        }

        [TestMethod]
        public void TestMethod3()
        => new Func<SomeClass, Task<string>>((someClass) => someClass.Bla2())
                .PerformTest(
                (container) => container.GetRequiredMock<ITest1>().Setup(t => t.GetAString()).Returns("123"),
                (result, container) => Assert.AreEqual("123", result));

        [TestMethod]
        public void TestMethod4()
        => new Func<SomeClass, Task<string>>((someClass)
                => someClass.Bla2())
                .PerformTest(
                (result, container) => Assert.AreEqual(null, result));

        [TestMethod]
        public Task TestMethod5()
        => Ext.FromCtors<SomeClass, string>()
                .Arrange((container) => container.GetRequiredMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
                .Assert((result, someClass) => Assert.AreEqual("123", result))
                .Act(sc => sc.Bla2())
                .Go();

        [TestMethod]
        public Task TestMethod6()
        => new Func<SomeClass, Task<string>>(sc => sc.Bla2()).FromCtors()
                .Arrange((container) => container.GetRequiredMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
                .Assert((result, someClass) => Assert.AreEqual("123", result))
                .Go();


        [TestMethod]
        public Task TestMethod8()
        {
            Task<string> LocaFunction(SomeClass someClass) => someClass.Bla2();

            return ((Func<SomeClass, Task<string>>)LocaFunction).FromCtors()
                .Arrange((container) => container.GetRequiredMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
                .Assert((result, someClass) => Assert.AreEqual("123", result))
                .Go();
        }

        [TestMethod]
        public Task TestMethod()
        => new Func<SomeClass, Task<string>>(sc => sc.Bla2()).FromCtors()
                .Arrange((container) => container.GetRequiredMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
                .Assert((result, someClass) => Assert.AreEqual("123", result))
                .Go();

        [TestMethod]
        public async Task TestMethodVerbose()
        {
            //Arrange
            var testMock1 = new Mock<ITest1>();
            var testMock2 = new Mock<ITest2>();
            var testMock3 = new Mock<ITest3>();
            var testMock4 = new Mock<ITest4>();
            var testMock5 = new Mock<ITest5>();

            _ = testMock1.Setup(t => t.GetAString()).Returns("123");

            var someClass = new SomeClass(
                testMock1.Object,
                testMock2.Object,
                testMock3.Object,
                testMock4.Object,
                testMock5.Object);

            //Act
            var result = await someClass.Bla2();

            //Assert
            Assert.AreEqual("123", result);
        }

    }

}
