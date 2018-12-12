using System;
using System.Runtime.Serialization;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class RetrieveQuoteDto
    {
        [DataMember]
        public string QuoteID { get; set; }

        [DataMember]
        public int ClientID { get; set; }

        [DataMember]
        public string CRMOppID { get; set; }

        [DataMember]
        public int OppID { get; set; }

        [DataMember]
        public string Rollup { get; set; }

        [DataMember]
        public decimal? QuotedAmount { get; set; }

        [DataMember]
        public decimal? QuotedCost { get; set; }

        [DataMember]
        public decimal? QuoteMargin { get; set; }

        [DataMember]
        public string QuoteSiteDesc { get; set; }

        [DataMember]
        public int SDALastUpdBy { get; set; }

        [DataMember]
        public string CloudLastUpdBy { get; set; }

        [DataMember]
        public string SDAUpdatedByName { get; set; }

        [DataMember]
        public string SDALastUpdDT { get; set; }

        [DataMember]
        public int CloudLastUpdById { get; set; }

        [DataMember]
        public string CloudLastUpdDT { get; set; }

        [DataMember]
        public string DeleteInd { get; set; }

        [DataMember]
        public string QuoteIDMainSite { get; set; }

        [DataMember]
        public string LastFileSavedLocation { get; set; }

        [DataMember]
        public string FilePlatformFileID { get; set; }

        [DataMember]
        public string CreateDT { get; set; }

        [DataMember]
        public int CreateBy { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal1 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal2 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal3 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal4 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal5 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal6 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal7 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal8 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal9 { get; set; }

        [DataMember]
        public decimal? ClientDefinedTotal10 { get; set; }

        [DataMember]
        public string ClientDefinedText1 { get; set; }

        [DataMember]
        public string ClientDefinedText2 { get; set; }

        [DataMember]
        public string ClientDefinedText3 { get; set; }

        [DataMember]
        public string ClientDefinedText4 { get; set; }

        [DataMember]
        public string ClientDefinedText5 { get; set; }
    }
}
