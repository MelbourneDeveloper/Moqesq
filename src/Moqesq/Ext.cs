using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Moqesq
{
    public static class Ext
    {

        #region Public Methods

        public static IServiceCollection AddMocksFor<T>(this IServiceCollection serviceCollection, Action<Type>? foreachType = null)
        => AddMocksFor<T, T>(serviceCollection, foreachType);

        public static IServiceCollection AddMocksFor<TInterface, T>(this IServiceCollection serviceCollection, Action<Type>? foreachType = null)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            typeof(T)
                .GetConstructors()
                .SelectMany(c => c.GetParameters())
                .Select(p => p.ParameterType)
                .ToList()
                .ForEach(foreachType ?? ((t) =>
                {
                    var mock = GetMock(t);
                    serviceCollection.AddSingleton(mock.GetType(), mock);
                    serviceCollection.AddSingleton(t, mock.Object);
                }));

            var serviceDescriptor = serviceCollection.FirstOrDefault(sc => sc.ServiceType == typeof(TInterface));
            if (serviceDescriptor != null) serviceCollection.Remove(serviceDescriptor);

            serviceCollection.AddSingleton(typeof(TInterface), typeof(T));

            return serviceCollection;
        }

        public static ServiceProvider BuildServiceProviderFor<T>(this IServiceCollection serviceCollection)
        => serviceCollection.AddMocksFor<T, T>().BuildServiceProvider();

        public static ServiceProvider BuildServiceProviderFor<T>()
        => new ServiceCollection().BuildServiceProviderFor<T>();

        public static MockContainer<T, TResult> FromCtors<T, TResult>(Action<T>? act = null, Action<IServiceCollection>? configureServices = null) where T : notnull
        {
            var serviceCollection = new ServiceCollection();
            var mocksByType = new Dictionary<Type, Mock>();

            serviceCollection.AddMocksFor<T, T>((t) =>
            {
                RegisterMock(serviceCollection, t, mocksByType);
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            T service = serviceProvider.GetRequiredService<T>();

            if (configureServices != null)
            {
                configureServices(serviceCollection);
            }

            if (act != null)
            {
                act(service);
            }

            return new(
                serviceCollection,
                serviceProvider,
                mocksByType,
                service,
                //TODO
                (a) => throw new NotImplementedException(),
                (a) => { },
                (a, b) => { });
        }

        public static MockContainer<TService, TResult> FromCtors<TService, TResult>(this Func<TService, Task<TResult>> act, Action<IServiceCollection>? configureServices = null) where TService : notnull
        {
            var serviceCollection = new ServiceCollection();
            var mocksByType = new Dictionary<Type, Mock>();

            serviceCollection.AddMocksFor<TService, TService>((t) =>
            {
                RegisterMock(serviceCollection, t, mocksByType);
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            TService service = serviceProvider.GetRequiredService<TService>();

            if (configureServices != null)
            {
                configureServices(serviceCollection);
            }

            return new(
                serviceCollection,
                serviceProvider,
                mocksByType,
                service,
                act,
                (a) => { },
                (a, b) => { });
        }

        public static Task PerformTest<TResult, TManager>(
            this Func<TManager, Task<TResult>> act,
            Action<MockContainer<TManager, TResult>> arrange,
            Action<TResult, MockContainer<TManager, TResult>> assert) where TManager : notnull
            =>
            act == null ? throw new ArgumentNullException(nameof(act)) :
            assert == null ? throw new ArgumentNullException(nameof(assert)) :
            PerformTest(arrange, act, assert);

        public static Task PerformTest<TResult, TManager>(
            this Func<TManager, Task<TResult>> act,
            Action<TResult, MockContainer<TManager, TResult>> assert)
             where TManager : notnull
            =>
            act == null ? throw new ArgumentNullException(nameof(act)) :
            assert == null ? throw new ArgumentNullException(nameof(assert)) :
            PerformTest(null, act, assert);

        public static MockContainer<T, TResult> SetupResult<T, TResult, TMock>(this MockContainer<T, TResult> container, Expression<Func<TMock, TResult>> expression, TResult result) where TMock : class
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.GetMock<TMock>().Setup(expression).Returns(result);

            return container;
        }

        public static MockContainer<T, TResult> SetupResult<T, TResult, TMock>(this MockContainer<T, TResult> container, Func<MockContainer<T, TResult>,Mock<TMock>> mock, Expression<Func<TMock, TResult>> expression, TResult result) where TMock : class
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (mock == null) throw new ArgumentNullException(nameof(mock));

            mock(container).Setup(expression).Returns(result);

            return container;
        }
        #endregion Public Methods

        #region Internal Methods

        internal static IServiceCollection Clone(this IServiceCollection serviceCollection)
        {
            var serviceCollection2 = new ServiceCollection();

            foreach (var sd in serviceCollection)
            {
                ((IList<ServiceDescriptor>)serviceCollection2).Add(sd);
            }

            return serviceCollection2;
        }

        #endregion Internal Methods

        #region Private Methods

        private static Mock GetMock(Type t)
        {
            object? mockInstance = Activator.CreateInstance(typeof(Mock<>).MakeGenericType(new Type[] { t }));
            if (mockInstance == null) throw new InvalidOperationException($"Type {t} cannot be mocked");
            var mock = (Mock)mockInstance;
            return mock;
        }

        private static async Task PerformTest<TResult, TManager>(
            Action<MockContainer<TManager, TResult>>? arrange,
            Func<TManager, Task<TResult>> act,
            Action<TResult, MockContainer<TManager, TResult>> assert
            ) where TManager : notnull
        {
            var mockContainer = FromCtors<TManager, TResult>();
            arrange?.Invoke(mockContainer);

            var response = await act(mockContainer.Instance).ConfigureAwait(false);

            assert(response, mockContainer);
        }

        private static void RegisterMock(IServiceCollection serviceCollection, Type t, Dictionary<Type, Mock> mocksByType)
        {
            Mock mock = GetMock(t);
            mocksByType.Add(t, mock);
            serviceCollection.AddSingleton(t, mock.Object);
        }

        #endregion Private Methods
    }

}
