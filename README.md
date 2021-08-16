# Moqesq

Yet another [Moq](https://github.com/moq/moq) extensions library. Nuget [here](https://www.nuget.org/packages/Moqesq). Samples [here](https://github.com/MelbourneDeveloper/Moqesq/blob/b65585d05df6cdead69009baf37b36975c7628df/src/Moqesq.Tests/UnitTest1.cs#L10).

### Fluent Style

Do this:
```cs
[TestMethod]
public Task TestMethod()
=> new Func<SomeClass, Task<string>>(sc => sc.GetTheString()).FromCtors()
        .Arrange((container) => container.GetRequiredMock<ITest1>().Setup(t => t.GetAString()).Returns("123"))
        .Assert((result, someClass) => Assert.AreEqual("123", result))
        .Go();
```

Instead of:

```cs
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
```
Notice how much code is necessary to mock many dependencies - even when you are not interested them. Whenever the dependency list changes, the unit tests break. This solves that problem. 

Or...

### Minimal Style

```cs
[TestMethod]
public void TestMethod1()
=> Ext.Go<SomeClass, string>(
//Act
sc => sc.Bla2(),
//Assert
(container) => container.Verify<ITest1>(t => t.DoTestTask(), Times.Once()));
```

Instead of

```cs
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
```

Perform an integration test on two classes (`SomeClass` and `Test3`). `SomeClass` depends on `ITest3` and `Test3` depends on `ITest2` which we mock. The extensions automatically create the mocks.

```cs
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
```

### Test Mapping

This tests that `y` has the same properties as `d` and that those properties are equal regardless of the type.

```csharp
    [TestMethod]
    public void TestShouldHave()
    {
        var d = new D { First = "1", Second = 2 };
        var y = new Y { First = "1", Second = 2 };
        y.ShouldHave(d);
    }
```

And, you can recurse deeper in to the object graph like this:

```csharp
    [TestMethod]
    public void TestShouldHave2()
    {
        bool RecurseOrCompare(string propertyName, object a, object b)
         => new List<string> { "C", "D" }.Contains(propertyName) ? a.Has(b, RecurseOrCompare) : a.Equals(b);

        new B { StringProperty = "1", IntProperty = 2, C = new C() { AnotherStringProperty = "2", D = new() { First = "123", Second = 2 } } }
        .Has(
        new A { StringProperty = "1", IntProperty = 2, C = new C() { AnotherStringProperty = "2", D = new() { First = "123", Second = 2 } } },
        RecurseOrCompare);
    }
```
