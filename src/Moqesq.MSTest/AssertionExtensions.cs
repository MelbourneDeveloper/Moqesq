using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moqesq
{
    public static class AssertionExtensions
    {
        public static T ShouldEqual<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
            return actual;
        }

        public static T ShouldNotBeNull<T>(this T actual)
        {
            Assert.IsNotNull(actual);
            return actual;
        }

        public static T And<T>(this T actual, Action and)
        {
            and();
            return actual;
        }

        public static T Should<T>(this T actual, Func<T, bool> check)
        {
            Assert.IsTrue(check(actual));
            return actual;
        }

        public static IEnumerable<T> AllShould<T>(this IEnumerable<T> actualItems, Func<T, bool> check)
        {
            foreach (var actual in actualItems)
            {
                Assert.IsTrue(check(actual));
            }

            return actualItems;
        }

        public static T ShouldBeNull<T>(this T actual)
        {
            Assert.IsNull(actual);
            return actual;
        }

        public static T And<T>(this T actual, Func<T, bool> check)
        {
            Assert.IsTrue(check(actual));
            return actual;
        }

        public static bool ShouldHave<T>(
        this T item,
        object has,
        IList<string> recurseProperties)
        {
            bool RecurseOrCompare(string propertyName, object a, object b)
            => recurseProperties.Contains(propertyName) ? a.ShouldHave(b, RecurseOrCompare) : a.Equals(b);

            return ShouldHave(item, has, RecurseOrCompare);
        }

        public static bool ShouldHave<T>(
        this T item,
        object has,
        CheckValue? comp = null)
        {
            comp ??= (propertyName, a, b) => a.Equals(b);

            has
            .GetType()
            .GetProperties()
            .ToList<System.Reflection.PropertyInfo>()
            .ForEach(p =>
            {
                object expected = p.GetValue(has);

                object actual = item.GetType().GetProperty(p.Name).GetValue(item);

                bool condition = comp(
                                p.Name,
                                expected,
                                actual);

                Assert.IsTrue(
                    condition
                    );
            }
            );

            return true;
        }
    }

    public delegate bool CheckValue(string propertyName, object expected, object actual);

}
