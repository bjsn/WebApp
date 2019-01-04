using Corspro.Data.External;
using Corspro.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corspro.Business.External
{
    public class ClientImportOptionBL
    {
        public List<ClientImportOptionDto> GetListByClientId(int ClientId)
        {
            try
            {
                return new ClientImportOptionDL().GetListByClientId(ClientId);
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
        }

        public int InsertClientImportOption(int ClientId, string ImportOption, string Status)
        {
            try
            {
                return new ClientImportOptionDL().InsertClientImportOption(ClientId, ImportOption, Status);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
