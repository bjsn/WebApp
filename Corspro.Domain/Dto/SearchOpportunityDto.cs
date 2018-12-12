using System;
using System.Runtime.Serialization;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class SearchOpportunityDto
    {
        [DataMember]
        public int OppID { get; set; }

        [DataMember]
        public String CRMOppID { get; set; }

        [DataMember]
        public String OppName { get; set; }

        [DataMember]
        public String CompanyName { get; set; }

        [DataMember]
        public string OppStatus { get; set; }

        [DataMember]
        public string OppStatusName { get; set; }

        [DataMember]
        public string OppProbability { get; set; }

        [DataMember]
        public string OppCloseDate { get; set; }

        [DataMember]
        public int OppOwner { get; set; }

        [DataMember]
        public string OppOwnerName { get; set; }
    }
}
