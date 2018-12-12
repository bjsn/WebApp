using System;

namespace Corspro.Domain
{
    public class OutdatedQuoteException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutdatedQuoteException"/> class.
        /// </summary>
        public OutdatedQuoteException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutdatedQuoteException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OutdatedQuoteException(string message):base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutdatedQuoteException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public OutdatedQuoteException(string message,Exception inner)
            : base(message,inner)
        {
        }
    }
}
