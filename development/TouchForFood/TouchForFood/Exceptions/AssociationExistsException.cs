using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchForFood.Exceptions
{
    public class AssociationExistsException : Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.
        public AssociationExistsException()
        {
        }

        public AssociationExistsException(string message)
            : base(message)
        {
        }
    }
}