using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Moqesq
{
    public class MockContainer<T, TResult>
    {
        internal IServiceCollection ServiceCollection { get; }
        internal IServiceProvider ServiceProvider { get; }
        internal IDictionary<Type, Mock> MocksByType { get; }

        internal Func<T, Task<TResult>> Act;
        internal Action<MockContainer<T, TResult>> Arrange;
        internal Action<TResult, MockContainer<T, TResult>> Assert;

        public T Instance { get; }

        internal MockContainer(
            IServiceCollection serviceCollection,
            IServiceProvider serviceProvider,
            IDictionary<Type, Mock> mocksByType,
            T instance,
            Func<T, Task<TResult>> act,
            Action<MockContainer<T, TResult>> arrange,
            Action<TResult, MockContainer<T, TResult>> assert
            )
        {
            ServiceCollection = serviceCollection;
            ServiceProvider = serviceProvider;
            MocksByType = mocksByType;
            Instance = instance;
            Act = act;
            Arrange = arrange;
            Assert = assert;
        }

        public MockContainer<T, TResult> ReplaceMock<TFrom>(Mock<TFrom> mock) where TFrom : class
        {
            if (mock == null) throw new ArgumentNullException(nameof(mock));

            var serviceCollection = ServiceCollection.Clone().AddSingleton(typeof(TFrom), mock.Object);

            return new MockContainer<T, TResult>(
                serviceCollection,
                serviceCollection.BuildServiceProvider(),
                ImmutableDictionary.CreateRange(MocksByType).Add(typeof(T), mock),
                Instance,
                Act,
                Arrange,
                Assert
                );
        }

        public Mock<TMock> GetRequiredMock<TMock>() where TMock : class
            => (Mock<TMock>)MocksByType[typeof(TMock)];
    }

}
