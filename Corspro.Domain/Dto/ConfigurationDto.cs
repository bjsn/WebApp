using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public  class ConfigurationDto
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

    }
}
