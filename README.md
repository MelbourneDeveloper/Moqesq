# Moqesq

Yet another [Moq](https://github.com/moq/moq) extensions library

Do this:
```cs
[TestMethod]
public async Task TestMethod6()
{
    await new Func<SomeClass, Task<string>>(sc => Task.FromResult(sc.Bla2())).FromCtors()
        .Arrange((container) => container.GetRequiredMock<ITest>().Setup(t => t.GetAString()).Returns("123"))
        .Assert((result, someClass) => Assert.AreEqual("123", result))
        .Go();
}
```

Instead of:

```cs
[TestMethod]
public async Task TestMethod1()
{
    //Arrange
    var testMock = new Mock<ITest>();
    _ = testMock.Setup(t => t.GetAString()).Returns("123");
    var someClass = new SomeClass(testMock.Object);

    //Act
    var result = someClass.Bla2();

    //Assert
    Assert.AreEqual("123", result);
}
```

Nuget here: https://www.nuget.org/packages/Moqesq
