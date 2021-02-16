using System;

namespace CygniKodprovApp.Exceptions
{
    public class MissingCriticalDataException : Exception
    {
        public MissingCriticalDataException()
        {
        }

        public MissingCriticalDataException(string message)
            : base(message)
        {
        }

        public MissingCriticalDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
