using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moqesq
{
    public static class Ext
    {


        public static MockContainer<T> FromCtors<T>(Action<T>? act = null, Action<IServiceCollection>? configureServices = null) where T : notnull
        {
            var serviceCollection = new ServiceCollection();
            var mocksByType = new Dictionary<Type, Mock>();

            typeof(T)
                .GetConstructors()
                .SelectMany(c => c.GetParameters())
                .Select(p => p.ParameterType)
                .ToList()
                .ForEach((t) =>
                {
                    object? mockInstance = Activator.CreateInstance(typeof(Mock<>).MakeGenericType(new Type[] { t }));
                    if (mockInstance == null) throw new InvalidOperationException($"Type {t} cannot be mocked");
                    var mock = (Mock)mockInstance;
                    mocksByType.Add(t, mock);
                    serviceCollection.AddSingleton(t, mock.Object);
                });

            serviceCollection.AddSingleton(typeof(T));

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

        internal static IServiceCollection Clone(this IServiceCollection serviceCollection)
        {
            var serviceCollection2 = new ServiceCollection();

            foreach (var sd in serviceCollection)
            {
                ((IList<ServiceDescriptor>)serviceCollection2).Add(sd);
            }

            return serviceCollection2;
        }

    }

}
