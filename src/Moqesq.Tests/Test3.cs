using System.Threading.Tasks;

namespace Moqesq.Tests
{
    public class Test3 : ITest3
    {
        ITest2 test2;

        public Test3(ITest2 test2) => this.test2 = test2;

        public Task<int> GetInt() => test2.GetInt();
    }
}
