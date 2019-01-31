using System;
using System.Runtime.Serialization;

namespace Corspro.Domain.Dto
{
    [DataContract]
    [Serializable]
    public class UserDto
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string LoginID { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public int? ManagerUserID { get; set; }

        //[DataMember]
        public string ManagerUserName { get; set; }

        [DataMember]
        public bool Administrator { get; set; }

        [DataMember]
        public bool DeleteInd { get; set; }

        [DataMember]
        public int? CloudLastUpdBy { get; set; }

        [DataMember]
        public string CloudLastUpdDT { get; set; }

        [DataMember]
        public string EmailSent { get; set; }

        [DataMember]
        public string SDAOppMgmt { get; set; }

    }
}
