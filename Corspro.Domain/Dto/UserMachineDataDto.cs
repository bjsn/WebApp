using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class UserMachineDataDto
    {
        [DataMember]
        public int ClientID { get; set; }
        
        [DataMember]
        public int UserID { get; set; }

        [DataMember]
        public string WindowsUserName { get; set; }

        [DataMember]
        public string MACAddress { get; set; }

        [DataMember]
        public string VersionDotNet { get; set; }

        [DataMember]
        public string VersionExcel { get; set; }

        [DataMember]
        public string VersionWord { get; set; }

        [DataMember]
        public string VersionSDA { get; set; }

        [DataMember]
        public string VersionSalesManager { get; set; }

        [DataMember]
        public string VersionWindows { get; set; }

        [DataMember]
        public string InstallType { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string UserTimeZone { get; set; }

        [DataMember]
        public DateTime CreateDT { get; set; }

        [DataMember]
        public DateTime LastUpdDT { get; set; }
    }
}
