using System;
using System.Collections.Generic;
using Corspro.Data.External;
using Corspro.Domain.Dto;

namespace Corspro.Business.External
{
    public class InterfaceXRefBL
    {
        /// <summary>
        /// Gets the records with mapped fields by client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="txn">The TXN.</param>
        /// <param name="crmXrefUpdDT">The CRM xref upd dt.</param>
        /// <returns></returns>
        public List<InterfaceXRefDto> GetRecordsWithMappedFieldsByClient(long clientId, string txn, DateTime crmXrefUpdDT)
        {
            var interfaceXRefDl = new InterfaceXRefDL();
            return interfaceXRefDl.GetRecordsWithMappedFieldsByClient(clientId, txn, crmXrefUpdDT);
        }
    }
}
