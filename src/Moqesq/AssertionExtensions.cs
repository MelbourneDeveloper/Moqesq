using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moqesq
{
    public static class AssertionExtensions
    {
        public static T ShouldEqual<T>(this T actual, T expected, EqualityComparer<T>? comparer = null)
        => (comparer ?? EqualityComparer<T>.Default)
            .Equals(expected, actual)
                ? actual
                : throw new AssertionFailureException();


        public static T ShouldNotBeNull<T>(this T actual)
        => actual != null ? actual : throw new AssertionFailureException();

        public static T ShouldBeNull<T>(this T actual)
        => actual == null ? actual : throw new AssertionFailureException();

        public static T And<T>(this T actual, Action and)
        {
            if (and == null) throw new ArgumentNullException(nameof(and));
            and();
            return actual;
        }

  
        public static T Should<T>(this T actual, Func<T, bool> check)
        => check == null ? throw new ArgumentNullException(nameof(check))
            : check(actual) ? actual : throw new AssertionFailureException();


        public static IEnumerable<T> AllShould<T>(this IEnumerable<T> actualItems, Func<T, bool> check)
        {
            if (check == null) throw new ArgumentNullException(nameof(check));
            if (actualItems == null) throw new ArgumentNullException(nameof(actualItems));

            foreach (var actual in actualItems)
            {
                if (!check(actual)) throw new AssertionFailureException();
            }

            return actualItems;
        }

        /// <summary>
        /// Tests that the item has all the properties in the expected, and that the value of those match.
        /// </summary>
        public static bool Has<T>(
        this T item,
        object has,
        IList<string> recurseProperties)
        {
            bool RecurseOrCompare(string propertyName, object a, object b)
            => recurseProperties.Contains(propertyName) ? a.Has(b, RecurseOrCompare) : a.Equals(b);

            return Has(item, has, RecurseOrCompare);
        }

        /// <summary>
        /// Tests that the item has all the properties in the expected, and that the value of those match, but allows for comparison and deep recursion with checkValue
        /// </summary>
        public static bool Has<T>(
        this T item,
        object has,
        CheckValue? checkValue = null)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (has == null) throw new ArgumentNullException(nameof(has));

            checkValue ??= (propertyName, a, b) => a.Equals(b);

            has
            .GetType()
            .GetProperties()
            .ToList()
            .ForEach(p =>
            ThrowOnFailure(
                    checkValue(
                        p.Name,
                        p.GetValue(has),
                        item.GetType().GetProperty(p.Name).GetValue(item))
                    )
            );

            return true;
        }

        /// <summary>
        /// Tests that the item has all the properties in the expected, and that the value of those match, but allows for comparison and deep recursion with checkValue
        /// </summary>
        public static T ShouldHave<T>(
        this T item,
        object has,
        CheckValue? checkValue = null)
        {
            Has(item, has, checkValue);
            return item;
        }

        private static void ThrowOnFailure(bool condition)
        {
            if (!condition) throw new AssertionFailureException();
        }
    }


    /// <summary>
    /// Specify logic for comparison of two objects based on the name of their property
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <param name="expected">Expected property value</param>
    /// <param name="actual">Actual property value</param>
    /// <returns></returns>
    public delegate bool CheckValue(string propertyName, object expected, object actual);

}
