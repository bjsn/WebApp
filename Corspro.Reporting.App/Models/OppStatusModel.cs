using System;
namespace Corspro.Reporting.App.Models
{
    [Serializable]
    public class OppStatusModel
    {
        public int ID { get; set; }

        public int ClientID { get; set; }

        public string OppStatus { get; set; }

        public int Order { get; set; }

        public int OppProbability { get; set; }

        public bool DeleteInd { get; set; }

        public string StageType { get; set; }

        public bool DefaultB { get; set; }
    }
}