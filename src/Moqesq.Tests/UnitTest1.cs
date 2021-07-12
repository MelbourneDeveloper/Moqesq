using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moqesq.Tests
{
    [TestClass]
    public class UnitTest1
    {
        Func<SomeClass, Task<string>> Act = new Func<SomeClass, Task<string>>(sc => sc.GetTheString());

        [TestMethod]
        public void TestMethod1()
        => Ext.Go<SomeClass, string>(
        //Act
        sc => sc.Bla2(),
        //Assert
        (container) => container.Verify<ITest1>(t => t.DoTestTask(), Times.Once()));

        [TestMethod]
        public async Task TestMethod1Verbose()
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
            testMock1.Verify(t => t.DoTestTask(), Times.Once());
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
        => new Func<SomeClass, Task<string>>((someClass) => someClass.GetTheString())
                .Go(
                (container) => container.GetMock<ITest1>().Setup(t => t.GetAString()).Returns("123"),
                (result, container) => Assert.AreEqual("123", result));

        [TestMethod]
        public void TestMethod4()
        => new Func<SomeClass, Task<string>>((someClass)
                => someClass.GetTheString())
                .Go(
                (result, container) => Assert.AreEqual(null, result));

        [TestMethod]
        public Task TestMethod5()
        => Ext.FromCtors<SomeClass, string>()
                .Arrange((container) => container.GetMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
                .Assert((result, someClass) => Assert.AreEqual("123", result))
                .Act(sc => sc.GetTheString())
                .Go();

        [TestMethod]
        public Task TestMethod6()
        => Act.FromCtors()
                .Arrange((container) => container.GetMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
                .Assert((result, someClass) => Assert.AreEqual("123", result))
                .Go();


        [TestMethod]
        public Task TestMethod8()
        {
            Task<string> LocaFunction(SomeClass someClass) => someClass.GetTheString();

            return ((Func<SomeClass, Task<string>>)LocaFunction).FromCtors()
                .Arrange((container) => container.GetMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
                .Assert((result, someClass) => Assert.AreEqual("123", result))
                .Go();
        }

        [TestMethod]
        public Task TestMethod()
        => Act.FromCtors()
        .Arrange((container) => container.GetMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
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
            var result = await someClass.GetTheString();

            //Assert
            Assert.AreEqual("123", result);
        }

        [TestMethod]
        public async Task TestIntegration()
        {
            //Arrange

            const int expectedResult = 345;

            //Create the mocks and services and put them in the container
            var serviceCollection = new ServiceCollection()
                .AddMocksFor<SomeClass>()
                .AddMocksFor<ITest3, Test3>();

            //Build the service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //Get the mock and do setup
            serviceProvider.GetRequiredService<Mock<ITest2>>()
                .Setup(t => t.GetInt())
                .Returns(Task.FromResult(expectedResult));

            //Act
            var result = await serviceProvider
                .GetRequiredService<SomeClass>()
                .GetTheInt();

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public Task TestShouldEqual()
        => Act.FromCtors()
                .SetupResult<SomeClass, string, ITest1>(t => t.GetAString(), "123")
                .Assert((result, someClass) => result.ShouldEqual("123"))
                .Go();


        [TestMethod]
        public Task TestShouldEqual2()
        => Act.FromCtors()
        .SetupResult((c) => c.GetMock<ITest1>(), t => t.GetAString(), "123")
        .Assert((result) => result.ShouldEqual("123"))
        .Go();

        [TestMethod]
        public async Task TestMethod9()
        {
            Task<string> LocaFunction(SomeClass someClass) => someClass.GetTheString();

            await ((Func<SomeClass, Task<string>>)LocaFunction).FromCtors()
                .Arrange((container) => container.SetupResult<SomeClass, string, ITest1>(t => t.GetAString(), "123"))
                .Assert((result, someClass) => Assert.AreEqual("123", result))
                .Go();
        }

        [TestMethod]
        public async Task TestMethod10()
        {
            Task<object> LocaFunction(ConstructorValidationClass constructorValidationClass) => constructorValidationClass.GetValue();

            await ((Func<ConstructorValidationClass, Task<object>>)LocaFunction)
                .FromCtors(configureServices: (l) => l.GetMock<IValueHolder>().Setup(v => v.Value).Returns(new object()))
                .Go();
        }


        [TestMethod]
        public async Task TestShouldHave()
        {
            var d = new D { First = "1", Second = 2 };
            var y = new Y { First = "1", Second = 2 };
            y.ShouldHave(d);
        }

        [TestMethod]
        public async Task TestShouldHave2()
        =>
        new B { StringProperty = "1", IntProperty = 2, C = new C() { AnotherStringProperty = "2", D = new() { First = "123" } } }
        .ShouldHave(
        new A { StringProperty = "1", IntProperty = 2, C = new C() { AnotherStringProperty = "2", D = new() { First = "123" } } },
        new List<string> { "C", "D" });


        [TestMethod]
        public async Task TestShouldHave3()
        {
            bool RecurseOrCompare(string propertyName, object a, object b)
            => new List<string> { "C", "D" }.Contains(propertyName) ? a.ShouldHave(b, RecurseOrCompare) : a.Equals(b);

            new B { StringProperty = "1", IntProperty = 2, C = new C() { AnotherStringProperty = "2", D = new() { First = "123", Second = 2 } } }
            .ShouldHave(
            new A { StringProperty = "1", IntProperty = 2, C = new C() { AnotherStringProperty = "2", D = new() { First = "123", Second = 2 } } },
            RecurseOrCompare);
        }

        [TestMethod]
        public async Task TestShouldHave4()
        {
            bool RecurseOrCompare(string propertyName, object a, object b)
            => new List<string> { "C", "D" }.Contains(propertyName) ? a.ShouldHave(b, RecurseOrCompare) : a.Equals(b);

            Assert.ThrowsException<AssertionFailureException>(() =>
            {
                new B { StringProperty = "1", IntProperty = 2, C = new C() { AnotherStringProperty = "2", D = new() { First = "123", Second = 2 } } }
                .ShouldHave(
                new A { StringProperty = "1", IntProperty = 2, C = new C() { AnotherStringProperty = "2", D = new() { First = "124", Second = 2 } } },
                RecurseOrCompare);
            });

        }
    }

}
