using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Moqesq
{
    public class MockContainer<TService, TResult>
    {
        internal IServiceCollection ServiceCollection { get; }
        internal IServiceProvider ServiceProvider { get; }
        internal IDictionary<Type, Mock> MocksByType { get; }

        internal Func<TService, Task<TResult>> ActFunc;
        internal Action<MockContainer<TService, TResult>> ArrangeFunc;
        internal Action<TResult, MockContainer<TService, TResult>> AssertFunc;
        internal Action<IServiceCollection> ConfigureServicesFunc;

        public TService Instance { get; }

        internal MockContainer(
            IServiceCollection serviceCollection,
            IServiceProvider serviceProvider,
            IDictionary<Type, Mock> mocksByType,
            TService instance,
            Func<TService, Task<TResult>> act,
            Action<MockContainer<TService, TResult>> arrange,
            Action<TResult, MockContainer<TService, TResult>> assert,
            Action<IServiceCollection> configureServices
            )
        {
            ServiceCollection = serviceCollection;
            ServiceProvider = serviceProvider;
            MocksByType = mocksByType.ToImmutableDictionary();
            Instance = instance;
            ActFunc = act;
            ArrangeFunc = arrange;
            AssertFunc = assert;
            ConfigureServicesFunc = configureServices;
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
                AssertFunc,
                ConfigureServicesFunc
                );
        }

        public MockContainer<TService, TResult> Act(Func<TService, Task<TResult>> act)
        {
            var  serviceCollection = ServiceCollection.Clone();

            return new MockContainer<TService, TResult>(
                serviceCollection,
                serviceCollection.BuildServiceProvider(),
                //Ok to pass reference?
                MocksByType,
                Instance,
                act,
                ArrangeFunc,
                AssertFunc,
                ConfigureServicesFunc);
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
                assert,
                ConfigureServicesFunc);
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
                AssertFunc, 
                ConfigureServicesFunc
                );
        }

        public MockContainer<TService, TResult> ConfigureServices(Action<IServiceCollection> configureServices)
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
                AssertFunc,
                configureServices
                );
        }

        public Mock<TMock> GetRequiredMock<TMock>() where TMock : class
            => (Mock<TMock>)MocksByType[typeof(TMock)];

        public async Task<TResult> Go()
        {
            ConfigureServicesFunc(ServiceCollection);
            ArrangeFunc(this);
            var result = await ActFunc(Instance).ConfigureAwait(false);
            AssertFunc(result, this);
            return result;
        }
    }

}
