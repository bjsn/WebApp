using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corspro.Domain.Dto
{
    public class OppGridDto
    {
        public int OppID { get; set; }

        public string OppName { get; set; }

        public int OppStatusId { get; set; }

        public string OppStatusName { get; set; }

        public string StageType { get; set; }

        public int? OppProbability { get; set; }

        public string OppCloseDate { get; set; }

        public int ClientID { get; set; }

        public string CRMOppID { get; set; }

        public string CompanyName { get; set; }

        public int NumofQuotes { get; set; }

        public int OppOwner { get; set; }

        public string OppOwnerName { get; set; }

        public string Manager { get; set; }

        public string SDALastUpdDT { get; set; }

        public string CloudLastUpdDT { get; set; }

        public decimal? QuotedAmount { get; set; }

        public decimal? QuotedCost { get; set; }

        public decimal? QuotedMargin { get; set; }

        public string CreateDT { get; set; }

        public decimal? ClientDefinedTotal1 { get; set; }

        public decimal? ClientDefinedTotal2 { get; set; }

        public decimal? ClientDefinedTotal3 { get; set; }

        public decimal? ClientDefinedTotal4 { get; set; }

        public decimal? ClientDefinedTotal5 { get; set; }

        public decimal? ClientDefinedTotal6 { get; set; }

        public decimal? ClientDefinedTotal7 { get; set; }

        public decimal? ClientDefinedTotal8 { get; set; }

        public decimal? ClientDefinedTotal9 { get; set; }

        public decimal? ClientDefinedTotal10 { get; set; }

        public string ClientDefinedText1 { get; set; }

        public string ClientDefinedText2 { get; set; }

        public string ClientDefinedText3 { get; set; }

        public string ClientDefinedText4 { get; set; }

        public string ClientDefinedText5 { get; set; }
    }
}
