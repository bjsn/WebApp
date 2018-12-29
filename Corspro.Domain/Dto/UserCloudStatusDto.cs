using Corspro.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class UserCloudStatusDto
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string UserStatus { get; set; }

        [DataMember]
        public string AppVersion { get; set; }

        [DataMember]
        public string ValidUserNextCheckHours { get; set; }

        [DataMember]
        public string ValidUserNextCheckReqDays { get; set; }
    }
}
