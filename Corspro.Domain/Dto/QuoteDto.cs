using System;
using System.Runtime.Serialization;
using System.Data;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class QuoteDto : RetrieveQuoteDto
    {
        /// <summary>
        /// Gets or sets the login info.
        /// </summary>
        /// <value>
        /// The login info.
        /// </value>
        [DataMember]
        public ClientLoginDto LoginInfo { get; set; }

        /// <summary>
        /// Gets or sets the opportunity table.
        /// </summary>
        /// <value>
        /// The quote table.
        /// </value>
        [DataMember]
        public DataTable QuoteTable { get; set; }

        /// <summary>
        /// Gets or sets the CRM xref definition.
        /// </summary>
        /// <value>
        /// The CRM xref definition.
        /// </value>
        [DataMember]
        public DataTable CRMXrefDefinition { get; set; }
    }
}
