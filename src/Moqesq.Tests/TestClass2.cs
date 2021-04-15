using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moqesq.Tests
{
    public class Test2 : ITest2
    {
        public Task<int> GetInt() => Task.FromResult(123);
    }
}
