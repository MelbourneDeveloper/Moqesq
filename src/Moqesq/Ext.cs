using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moqesq
{
    public static class Ext
    {

        #region Public Methods

        public static IServiceCollection AddMocksFor<T>(this IServiceCollection serviceCollection, Action<Type>? foreachType = null)
        {
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

            serviceCollection.AddSingleton(typeof(T));

            return serviceCollection;
        }

        public static ServiceProvider BuildServiceProviderFor<T>(this IServiceCollection serviceCollection)
        => serviceCollection.AddMocksFor<T>().BuildServiceProvider();

        public static ServiceProvider BuildServiceProviderFor<T>()
        => new ServiceCollection().BuildServiceProviderFor<T>();

        public static MockContainer<T> FromCtors<T>(Action<T>? act = null, Action<IServiceCollection>? configureServices = null) where T : notnull
        {
            var serviceCollection = new ServiceCollection();
            var mocksByType = new Dictionary<Type, Mock>();

            serviceCollection.AddMocksFor<T>((t) =>
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

            return new(serviceCollection, serviceProvider, mocksByType, service);
        }

        public static Task PerformTest<TResult, TManager>(
            this Func<TManager, Task<TResult>> act,
            Action<MockContainer<TManager>> arrange,
            Action<TResult, MockContainer<TManager>> assert) where TManager : notnull
            =>
            act == null ? throw new ArgumentNullException(nameof(act)) :
            assert == null ? throw new ArgumentNullException(nameof(assert)) :
            PerformTest(arrange, act, assert);

        public static Task PerformTest<TResult, TManager>(
            this Func<TManager, Task<TResult>> act,
            Action<TResult, MockContainer<TManager>> assert) 
             where TManager : notnull
            =>
            act == null ? throw new ArgumentNullException(nameof(act)) :
            assert == null ? throw new ArgumentNullException(nameof(assert)) :
            PerformTest(null, act, assert);

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
            Action<MockContainer<TManager>>? arrange,
            Func<TManager, Task<TResult>> act,
            Action<TResult, MockContainer<TManager>> assert
            ) where TManager : notnull
        {
            var mockContainer = FromCtors<TManager>();
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
