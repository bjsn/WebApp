using System;
using System.Runtime.Serialization;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class DisplayOppDto
    {
            [DataMember]
            public int OppID { get; set; }

            [DataMember]
            public string OppName { get; set; }

            [DataMember]
            public int OppStatusId { get; set; }

            [DataMember]
            public string OppStatusName { get; set; }

            [DataMember]
            public int? OppProbability { get; set; }

            [DataMember]
            public string OppCloseDate { get; set; }

            [DataMember]
            public string DeleteInd { get; set; }

            [DataMember]
            public int ClientID { get; set; }

            [DataMember]
            public string CRMOppID { get; set; }

            [DataMember]
            public string CompanyName { get; set; }

            [DataMember]
            public int NumofQuotes { get; set; }

            [DataMember]
            public int OppOwner { get; set; }

            [DataMember]
            public string OppOwnerName { get; set; }

            [DataMember]
            public string Manager { get; set; }

            [DataMember]
            public string SDALastUpdDT { get; set; }

            [DataMember]
            public string CloudLastUpdDT { get; set; }

            [DataMember]
            public string CloudLastUpdBy { get; set; }

            [DataMember]
            public decimal? QuotedAmount { get; set; }

            [DataMember]
            public decimal? QuotedCost { get; set; }

            [DataMember]
            public decimal? QuotedMargin { get; set; }

            [DataMember]
            public int QuoteId { get; set; }

            [DataMember]
            public string QuoteSiteDesc { get; set; }

            [DataMember]
            public string QuoteType { get; set; }

            [DataMember]
            public decimal QuoteTotal { get; set; }
    }
}
