using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class ValidationException : Exception
    {
        public IReadOnlyList<string> InvalidEmails { get; }

        public ValidationException(string message) : base(message)
        {
            InvalidEmails = Array.Empty<string>();
        }

        public ValidationException(string message, IEnumerable<string> invalidEmails)
            : base(message)
        {
            InvalidEmails = (invalidEmails ?? Enumerable.Empty<string>()).ToArray();
        }

        public ValidationException(string message, IEnumerable<string> invalidEmails, Exception innerException)
            : base(message, innerException)
        {
            InvalidEmails = (invalidEmails ?? Enumerable.Empty<string>()).ToArray();
        }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            InvalidEmails = Array.Empty<string>();
        }
    }
}
