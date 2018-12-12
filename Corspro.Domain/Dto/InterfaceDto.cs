namespace Corspro.Domain.Dto
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [Serializable, DataContract]
    public class InterfaceDto
    {
        public int ClientId { get; set; }

        public string InterfaceSystem { get; set; }

        public string InterfaceConnectionStr { get; set; }

        public string InterfaceLogin { get; set; }

        public string InterfacePwd { get; set; }

        public string InterfaceToken { get; set; }

        public string InterfaceSessionId { get; set; }

        public string InterfaceServerURL { get; set; }
    }
}

