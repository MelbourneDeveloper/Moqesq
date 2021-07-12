using System;
using System.Collections.Generic;
using System.Text;

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
