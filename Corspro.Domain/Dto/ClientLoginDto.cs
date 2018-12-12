using System;
using System.Runtime.Serialization;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class ClientLoginDto
    {
        /// <summary>
        /// Gets or sets the client login id.
        /// </summary>
        /// <value>
        /// The client login id.
        /// </value>
        [DataMember]
        public string ClientLoginId { get; set; }

        /// <summary>
        /// Gets or sets the client login PWD.
        /// </summary>
        /// <value>
        /// The client login PWD.
        /// </value>
        [DataMember]
        public string ClientLoginPwd { get; set; }

        [DataMember]
        public int NextCRMOppID { get; set; }

        [DataMember]
        public bool ManageOppysInCRM { get; set; }

        [DataMember]
        public string CRMData { get; set; }

        [DataMember]
        public string SyncServerLocation { get; set; }
        [DataMember]
        public string SyncServerLogin { get; set; }
        [DataMember]
        public string SyncServerPwd { get; set; }
        [DataMember]
        public string OpportunityMgmt { get; set; }

        [DataMember]
        public string LastSDAVer { get; set; }

        [DataMember]
        public string LastSDAVerWithDBUpdates { get; set; }

        [DataMember]
        public string CorsProSyncInd { get; set; }
    }
}
