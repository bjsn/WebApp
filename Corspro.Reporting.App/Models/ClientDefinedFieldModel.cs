using System;
namespace Corspro.Reporting.App.Models
{
    [Serializable]
    public class ClientDefinedFieldModel
    {
        public int ClientDefinedFieldID { get; set; }

        public int ClientID { get; set; }

        public int InterfaceXRefID { get; set; }

        public string Table { get; set; }

        public string Field { get; set; }

        public string ColumnHeader { get; set; }

        public string Format { get; set; }

        public string SDARangeName { get; set; }
    }
}