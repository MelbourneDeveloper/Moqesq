using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moqesq
{
    public class MockContainer
    {
        internal IDictionary<Type, Mock> MocksByType { get; }

        internal MockContainer(
            IDictionary<Type, Mock> mocksByType
            )
        {
            MocksByType = mocksByType.ToImmutableDictionary();
        }

        public Mock<TMock> GetMock<TMock>() where TMock : class
            => (Mock<TMock>)MocksByType[typeof(TMock)];
    }

    public class MockContainer<TService, TResult> : MockContainer
    {
        internal IServiceCollection ServiceCollection { get; }
        internal IServiceProvider ServiceProvider { get; }

        internal Func<TService, Task<TResult>> ActFunc;
        internal Action<MockContainer<TService, TResult>> ArrangeFunc;
        internal Action<TResult, MockContainer<TService, TResult>> AssertFunc;

        public TService Instance { get; }

        internal MockContainer(
            IServiceCollection serviceCollection,
            IServiceProvider serviceProvider,
            IDictionary<Type, Mock> mocksByType,
            TService instance,
            Func<TService, Task<TResult>> act,
            Action<MockContainer<TService, TResult>> arrange,
            Action<TResult, MockContainer<TService, TResult>> assert
            ) : base(mocksByType.ToImmutableDictionary())
        {
            ServiceCollection = serviceCollection;
            ServiceProvider = serviceProvider;
            Instance = instance;
            ActFunc = act;
            ArrangeFunc = arrange;
            AssertFunc = assert;
        }

        public MockContainer<TService, TResult> ReplaceMock<TFrom>(Mock<TFrom> mock) where TFrom : class
        {
            if (mock == null) throw new ArgumentNullException(nameof(mock));

            var serviceCollection = ServiceCollection.Clone().AddSingleton(typeof(TFrom), mock.Object);

            return new MockContainer<TService, TResult>(
                serviceCollection,
                serviceCollection.BuildServiceProvider(),
                ImmutableDictionary.CreateRange(MocksByType).Add(typeof(TService), mock),
                Instance,
                ActFunc,
                ArrangeFunc,
                AssertFunc
                );
        }

        public MockContainer<TService, TResult> Act(Func<TService, Task<TResult>> act)
        {
            var serviceCollection = ServiceCollection.Clone();

            return new MockContainer<TService, TResult>(
                serviceCollection,
                serviceCollection.BuildServiceProvider(),
                //Ok to pass reference?
                MocksByType,
                Instance,
                act,
                ArrangeFunc,
                AssertFunc);
        }

        public MockContainer<TService, TResult> Assert(Action<TResult, MockContainer<TService, TResult>> assert)
        {
            var serviceCollection = ServiceCollection.Clone();

            return new MockContainer<TService, TResult>(
                serviceCollection,
                serviceCollection.BuildServiceProvider(),
                //Ok to pass reference?
                MocksByType,
                Instance,
                ActFunc,
                ArrangeFunc,
                assert);
        }

        public MockContainer<TService, TResult> Assert(Action<TResult> assert)
        {
            var serviceCollection = ServiceCollection.Clone();

            return new MockContainer<TService, TResult>(
                serviceCollection,
                serviceCollection.BuildServiceProvider(),
                //Ok to pass reference?
                MocksByType,
                Instance,
                ActFunc,
                ArrangeFunc,
                (result, b) => assert(result));
        }

        public MockContainer<TService, TResult> Arrange(Action<MockContainer<TService, TResult>> arrange)
        {
            var serviceCollection = ServiceCollection.Clone();

            return new MockContainer<TService, TResult>(
                serviceCollection,
                serviceCollection.BuildServiceProvider(),
                //Ok to pass reference?
                MocksByType,
                Instance,
                ActFunc,
                arrange,
                AssertFunc
                );
        }

        public MockContainer<TService, TResult> Arrange<T>(Expression<Action<T>> arrange) where T : class
        {
            var serviceCollection = ServiceCollection.Clone();

            return new MockContainer<TService, TResult>(
                serviceCollection,
                serviceCollection.BuildServiceProvider(),
                //Ok to pass reference?
                MocksByType,
                Instance,
                ActFunc,
                (a) => a.GetMock<T>().Setup(arrange),
                AssertFunc
                );
        }

        public async Task<TResult> Go()
        {
            ArrangeFunc(this);
            var result = await ActFunc(Instance).ConfigureAwait(false);
            AssertFunc(result, this);
            return result;
        }
    }

}
