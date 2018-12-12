using System;
using System.Linq;
using System.Transactions;
using Corspro.Domain.External;

namespace Corspro.Data.External
{
    public class ApplicationLogDL
    {
        /// <summary>
        /// Adds the error message.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="record">The record.</param>
        public bool AddErrorMessage(int clientId, string errorMessage, string record)
        {
            bool result = false;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingError = sdaCloudEntities.ErrorLogs.FirstOrDefault(
                           i =>
                           i.ClientID == clientId &&
                           i.ErrorMessage.Trim().ToUpper().Equals(errorMessage.Trim().ToUpper()) &&
                           i.Record.Trim().ToUpper().Equals(record.Trim().ToUpper()));

                        if (existingError != null)
                        {
                            existingError.ProcessDT = DateTime.UtcNow;
                            if (!string.IsNullOrEmpty(existingError.NoLongerSend) && existingError.NoLongerSend.Equals("Y"))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            var errorLog = new ErrorLog
                            {
                                ClientID = clientId,
                                ErrorMessage = errorMessage,
                                Record = record,
                                ProcessDT = DateTime.UtcNow
                            };

                            sdaCloudEntities.ErrorLogs.AddObject(errorLog);
                        }

                        sdaCloudEntities.SaveChanges();

                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Adds the transaction message.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="record">The record.</param>
        public void AddTransactionMessage(int clientId, string message, string record)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingLog = sdaCloudEntities.TransactionLogs.FirstOrDefault(
                           i =>
                           i.ClientID == clientId &&
                           i.Message.Trim().ToUpper().Equals(message.Trim().ToUpper()) &&
                           i.Record.Trim().ToUpper().Equals(record.Trim().ToUpper()));

                        if (existingLog != null)
                        {
                            existingLog.ProcessDT = DateTime.UtcNow;
                        }
                        else
                        {
                            var transLog = new TransactionLog
                            {
                                ClientID = clientId,
                                Message = message,
                                Record = record,
                                ProcessDT = DateTime.UtcNow
                            };

                            sdaCloudEntities.TransactionLogs.AddObject(transLog);
                        }

                        sdaCloudEntities.SaveChanges();

                        transactionScope.Complete();
                    }
                }
            }
        }
    }
}
