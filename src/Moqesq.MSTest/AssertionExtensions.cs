using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
    }
}
