using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System;
using System.Linq;

namespace Corspro.Data.External
{
    public class InterfaceDL
    {
        public InterfaceDto GetInterfaceByClientId(int clientId, string InterfaceSystem)
        {
            InterfaceDto result;
            using (SDACloudEntities sDACloudEntities = new SDACloudEntities())
            {
                Interface source = (from i in sDACloudEntities.Interfaces
                                    where i.ClientID == clientId && i.InterfaceSystem.Equals(InterfaceSystem)
                                    select i).SingleOrDefault<Interface>();
                Mapper.CreateMap<Interface, InterfaceDto>();
                InterfaceDto interfaceDto = Mapper.Map<Interface, InterfaceDto>(source);
                result = interfaceDto;
            }
            return result;
        }
    }
}
