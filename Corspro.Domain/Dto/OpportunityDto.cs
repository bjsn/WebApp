using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class OpportunityDto:RetrieveOpportunityDto
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
        /// The opportunity table.
        /// </value>
        [DataMember]
        public DataTable OpportunityTable { get; set; }

        /// <summary>
        /// Gets or sets the CRM xref definition.
        /// </summary>
        /// <value>
        /// The CRM xref definition.
        /// </value>
        [DataMember]
        public DataTable CRMXrefDefinition { get; set; }
        
        [DataMember]
        public List<QuoteDto> Quotes { get; set; }
    }
}
