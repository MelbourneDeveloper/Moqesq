using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moqesq
{
    public class AssertionFailureException : Exception
    {
        public AssertionFailureException(string message) : base(message)
        {
        }

        public AssertionFailureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AssertionFailureException()
        {
        }
    }
}
