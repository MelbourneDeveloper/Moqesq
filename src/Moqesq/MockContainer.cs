﻿using Microsoft.Extensions.DependencyInjection;
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

        public TService Instance { get; }

        internal MockContainer(
            IServiceCollection serviceCollection,
            IServiceProvider serviceProvider,
            IDictionary<Type, Mock> mocksByType,
            TService instance,
            Func<TService, Task<TResult>> act,
            Action<MockContainer<TService, TResult>> arrange,
            Action<TResult, MockContainer<TService, TResult>> assert
            )
        {
            ServiceCollection = serviceCollection;
            ServiceProvider = serviceProvider;
            MocksByType = mocksByType.ToImmutableDictionary();
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
            var  serviceCollection = ServiceCollection.Clone();

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
                AssertFunc);
        }

        public Mock<TMock> GetRequiredMock<TMock>() where TMock : class
            => (Mock<TMock>)MocksByType[typeof(TMock)];

        public Task<TResult> Go() => ActFunc(Instance);
    }

}
