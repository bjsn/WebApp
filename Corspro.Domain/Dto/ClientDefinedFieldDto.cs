using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class ClientDefinedFieldDto
    {
        [DataMember]
        public int ClientDefinedFieldID { get; set; }

        [DataMember]
        public int InterfaceXRefID { get; set; }

        [DataMember]
        public int ClientID { get; set; }

        [DataMember]
        public string Table { get; set; }

        [DataMember]
        public string Field { get; set; }

        [DataMember]
        public string ColumnHeader { get; set; }

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public string SDARangeName { get; set; }
    }
}
