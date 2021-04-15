# Moqesq

Yet another [Moq](https://github.com/moq/moq) extensions library

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

Nuget here: https://www.nuget.org/packages/Moqesq
