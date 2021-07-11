using Moq;
using System;
using System.Collections.Generic;

namespace Moqesq
{
    public class MockLookup
    {
        IReadOnlyDictionary<Type, Mock> mocks;

        public MockLookup(IReadOnlyDictionary<Type, Mock> mocks)
        {
            this.mocks = mocks;
        }

        public Mock<T> GetMock<T>() where T : class
            => (Mock<T>)mocks[typeof(T)];
    }

}
