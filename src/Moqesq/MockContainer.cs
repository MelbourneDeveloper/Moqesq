using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Moqesq
{
    public class MockContainer<T>
    {
        internal IServiceCollection ServiceCollection { get; }
        internal IServiceProvider ServiceProvider { get; }
        internal IDictionary<Type, Mock> MocksByType { get; }

        public T Instance { get; }

        internal MockContainer(
            IServiceCollection serviceCollection,
            IServiceProvider serviceProvider,
            IDictionary<Type, Mock> mocksByType,
            T instance)
        {
            ServiceCollection = serviceCollection;
            ServiceProvider = serviceProvider;
            MocksByType = mocksByType;
            Instance = instance;
        }

        public MockContainer<T> ReplaceMock<TFrom>(Mock<TFrom> mock) where TFrom : class
        {
            if (mock == null) throw new ArgumentNullException(nameof(mock));

            var serviceCollection = ServiceCollection.Clone().AddSingleton(typeof(TFrom), mock.Object);

            return new MockContainer<T>(serviceCollection, serviceCollection.BuildServiceProvider(), ImmutableDictionary.CreateRange(MocksByType).Add(typeof(T), mock), Instance);
        }

        public Mock<TMock> GetRequiredMock<TMock>() where TMock : class
            => (Mock<TMock>)MocksByType[typeof(TMock)];
    }

}
