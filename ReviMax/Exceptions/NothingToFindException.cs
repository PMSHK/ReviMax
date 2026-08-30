using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Exceptions
{
    internal class NothingToFindException : Exception
    {
        public NothingToFindException() : base("Nothing to find.")
        {
        }
        public NothingToFindException(string message) : base(message)
        {
        }
        public NothingToFindException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
