using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class ClientImportOptionDto
    {
        [DataMember]
        public int ClientID { get; set; }

        [DataMember]
        public string ImportOption { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public DateTime UpdateDT { get; set; }
    }

}
