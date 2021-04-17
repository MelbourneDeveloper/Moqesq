using System.Threading.Tasks;

namespace Moqesq.Tests
{
    public interface ITest1
    {
        Task DoTestTask();
        void DoTestThing();
        string GetAString();
    }
}
