using System.Threading.Tasks;

namespace Moqesq.Tests
{
    public class SomeClass
    {
        ITest1 test1;
        ITest2 test2;
        ITest3 test3;
        ITest4 test4;
        ITest5 test5;

        public SomeClass(
            ITest1 test1,
            ITest2 test2,
            ITest3 test3,
            ITest4 test4,
            ITest5 test5
            )
        {
            this.test1 = test1;
            this.test2 = test2;
            this.test3 = test3;
            this.test4 = test4;
            this.test5 = test5;
        }

        public void Bla()
        {
            test1.DoTestThing();
        }

        public Task<string> Bla2()
        {
            test1.DoTestThing();
            return Task.FromResult("SomeString");
        }

        public Task<int> GetTheInt() => test3.GetInt();

        public Task<string> GetTheString() => Task.FromResult(test1.GetAString());
    }
}
