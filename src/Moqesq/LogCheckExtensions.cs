﻿using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moqesq
{
    public static class LogCheckExtensions
    {
        /// <summary>
        /// Verify that the log was called and get access to check the log arguments 
        /// </summary>
        public static void VerifyLog<T, TException>(
                   this Mock<ILogger<T>> loggerMock,
                   Expression<Func<object, Type, bool>> match,
                   LogLevel logLevel,
                   int times,
                   EventId? eventId = default) where TException : Exception
        {
            if (loggerMock == null) throw new ArgumentNullException(nameof(loggerMock));

            loggerMock.Verify
            (
                l => l.Log
                (
                    //Check the severity level
                    logLevel,
                    //This may or may not be relevant to your scenario
                    //If you need to check this, add a parameter for it
                    eventId ?? It.IsAny<EventId>(),
                    //This is the magical Moq code that exposes internal log processing from the extension methods
                    It.Is<It.IsAnyType>(match),
                    //Confirm the exception type
                    It.IsAny<TException>(),
                    //Accept any valid Func here. The Func is specified by the extension methods
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                //Make sure the message was logged the correct number of times
                Times.Exactly(times)
            );
        }

        /// <summary>
        /// Verify that the log was called and get access to check the log arguments 
        /// </summary>
        public static void VerifyLog<T>(
           this Mock<ILogger<T>> loggerMock,
           Expression<Func<object, Type, bool>> match,
           LogLevel logLevel,
           int times)
        => VerifyLog<T, Exception>(loggerMock, match, logLevel, times);

        /// <summary>
        /// Check whether or not the log arguments match the expected result
        /// </summary>
        public static bool CheckValue<T>(this object state, string key, T expectedValue)
        => CheckValue<T>(state, key, (actualValue)
               => (actualValue == null && expectedValue == null) || (actualValue != null && actualValue.Equals(expectedValue)));

        /// <summary>
        /// Check whether or not the log arguments match the expected result
        /// </summary>
        public static bool CheckValue<T>(this object state, string key, Func<T, bool> compare)
        {
            if (compare == null) throw new ArgumentNullException(nameof(compare));

            var exists = state.GetValue<T>(key, out var actualValue);

            return exists && compare(actualValue);
        }

#pragma warning disable CA1021 // Avoid out parameters
        public static bool GetValue<T>(this object state, string key, out T value)
#pragma warning restore CA1021 // Avoid out parameters
        {
            var keyValuePairList = (IReadOnlyList<KeyValuePair<string, object>>)state;

            var keyValuePair = keyValuePairList.FirstOrDefault(kvp => string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0);

            value = (T)keyValuePair.Value;

            return keyValuePair.Key != null;
        }
    }
}