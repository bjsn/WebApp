using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class ClientDto
    {
        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string ClientLoginID { get; set; }

        [DataMember]
        public string ClientLoginPwd { get; set; }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string ManageOppysInCRM { get; set; }

        [DataMember]
        public int NextCRMOppID { get; set; }

        [DataMember]
        public string SendCRMOppValueUpdates { get; set; }

        [DataMember]
        public DateTime CRMXrefUpdDT { get; set; }

        [DataMember]
        public string CRMSystem { get; set; }

        [DataMember]
        public string CRMData { get; set; }

        [DataMember]
        public string SDAOppMgmt { get; set; }
    }
}
