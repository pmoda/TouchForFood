using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchForFood.Exceptions
{
    public class ItemActiveException : Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.
        public ItemActiveException()
        {
        }

        public ItemActiveException(string message)
            : base(message)
        {
        }
    }
}