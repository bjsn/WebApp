namespace Corspro.Reporting.App.Models
{
    public class QuoteModel
    {
        public string QuoteID { get; set; }

        public int OppID { get; set; }

        public string QuoteSiteDescription { get; set; }

        public string Rollup { get; set; }

        public decimal? QuotedAmount { get; set; }

        public decimal? QuotedCost { get; set; }

        public decimal? QuotedMargin { get; set; }

        public int SDALastUpdBy { get; set; }

        public string SDALastUpdByName { get; set; }

        public string LastFileSavedLocation { get; set; }
        
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