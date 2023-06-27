using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Exceptions
{
    public class UsernameAlreadyExistsException : ArgumentException
    {
        public UsernameAlreadyExistsException()
        {
        }

        public UsernameAlreadyExistsException(string? username) : base($"Username ``{username}`` already exists.")
        {
        }

        public UsernameAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UsernameAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
