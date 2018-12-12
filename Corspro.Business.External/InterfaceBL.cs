namespace Corspro.Business.External
{
    using Corspro.Data.External;
    using Corspro.Domain.Dto;
    using System;

    public class InterfaceBL
    {
        public InterfaceDto GetInterfaceByClientId(int clientId, string InterfaceSystem)
        {
            return new InterfaceDL().GetInterfaceByClientId(clientId, InterfaceSystem);
        }
    }
}

