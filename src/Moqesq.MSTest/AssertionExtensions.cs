using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
    }
}
