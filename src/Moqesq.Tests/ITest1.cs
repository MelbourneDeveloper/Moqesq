using System.Threading.Tasks;

namespace Moqesq.Tests
{
    public interface ITest1
    {
        void DoTestThing();
        string GetAString();
    }

    public interface ITest2 { Task<int> GetInt(); }
    public interface ITest3 { }
    public interface ITest4 { }
    public interface ITest5 { }
}
