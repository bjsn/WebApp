using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class AppVersion
    {
        [DataMember]
        public string AppVersionDetail { get; set; }
    }
}
