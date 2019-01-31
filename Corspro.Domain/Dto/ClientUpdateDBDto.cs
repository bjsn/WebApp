using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class ClientUpdateDBDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string DBFileName { get; set; }

        [DataMember]
        public string DBDescription { get; set; }

        [DataMember]
        public string AWSFlePath { get; set; }

        [DataMember]
        public string AWSFileName { get; set; }

        [DataMember]
        public string AWSFileID { get; set; }

        [DataMember]
        public DateTime DBFileUpdDT { get; set; }

        [DataMember]
        public DateTime DBUploadedDT { get; set; }

        [DataMember]
        public string DeleteInd { get; set; }

        [DataMember]
        public string DBFileType { get; set; }

        [DataMember]
        public string BetaAwsFilePath { get; set; }

        [DataMember]
        public string BetaAWSFileName { get; set; }

        [DataMember]
        public string BetaAWSFileID { get; set; }

        [DataMember]
        public DateTime BetaDBFileUpdDT { get; set; }
        
        [DataMember]
        public int BetaUploaderClientID { get; set; }

        [DataMember]
        public int BetaUploaderUserID { get; set; }

        [DataMember]
        public string BetaUploaderUserName { get; set; }

        [DataMember]
        public int UploaderClientID { get; set; }

        [DataMember]
        public int UploaderUserID { get; set; }

        [DataMember]
        public string UploaderUserName { get; set; }
    }
}
