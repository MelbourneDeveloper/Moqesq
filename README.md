# Moqesq

Yet another [Moq](https://github.com/moq/moq) extensions library. Nuget [here](https://www.nuget.org/packages/Moqesq)

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
Notice how much code is necessary to mock many dependencies - even when you are not interested them.

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

