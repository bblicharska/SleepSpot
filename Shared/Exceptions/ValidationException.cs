using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class ValidationException : Exception
    {
        // Expose invalid items (emails) if available
        public IReadOnlyList<string> InvalidEmails { get; }

        // Default ctor with only message
        public ValidationException(string message) : base(message)
        {
            InvalidEmails = Array.Empty<string>();
        }

        // New ctor: message + invalid emails
        public ValidationException(string message, IEnumerable<string> invalidEmails)
            : base(message)
        {
            InvalidEmails = (invalidEmails ?? Enumerable.Empty<string>()).ToArray();
        }

        // New ctor: message + invalid emails + inner exception
        public ValidationException(string message, IEnumerable<string> invalidEmails, Exception innerException)
            : base(message, innerException)
        {
            InvalidEmails = (invalidEmails ?? Enumerable.Empty<string>()).ToArray();
        }

        // Keep the existing constructor that accepts an inner Exception (if you want)
        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            InvalidEmails = Array.Empty<string>();
        }
    }
}
