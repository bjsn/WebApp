using System;
using System.Runtime.Serialization;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class InterfaceXRefDto
    {
        [DataMember(Order = 1)]
        public string Txn { get; set; }

        [DataMember(Order = 1)]
        public int ClientID { get; set; }

        [DataMember(Order = 1)]
        public string InterfaceTable { get; set; }

        [DataMember(Order = 1)]
        public string InterfaceField { get; set; }

        [DataMember(Order = 1)]
        public string InterfaceFieldType { get; set; }

        [DataMember(Order = 1)]
        public string SDASMTable { get; set; }

        [DataMember(Order = 1)]
        public string SDASMField { get; set; }

        [DataMember(Order = 1)]
        public string SDASMFieldType { get; set; }

        [DataMember(Order = 1)]
        public string CRMXrefUpdDT { get; set; }

        [DataMember(Order = 2)]
        public string SDARangeName { get; set; }
    }
}
