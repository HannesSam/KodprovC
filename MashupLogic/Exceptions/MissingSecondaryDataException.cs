﻿using System;

namespace MashupLogic.Exceptions
{
    public class MissingSecondaryDataException : Exception
    {
        public MissingSecondaryDataException()
        {
        }

        public MissingSecondaryDataException(string message)
            : base(message)
        {
        }

        public MissingSecondaryDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
