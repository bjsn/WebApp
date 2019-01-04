using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Corspro.Data.External
{
    public class ClientImportOptionDL
    {
        /// <summary>
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        public List<ClientImportOptionDto> GetListByClientId(int ClientId) 
        {
            List<ClientImportOptionDto> dtoList = new List<ClientImportOptionDto>();
            try
            {
                using (SDACloudEntities SDACloudEntities = new SDACloudEntities())
                {
                    using (SDACloudEntities)
                    {
                        var listClientImportOption = SDACloudEntities.ClientImportOptions.Where(i => i.ClientID == ClientId).ToList();
                        Mapper.CreateMap<ClientImportOption, ClientImportOptionDto>();
                        dtoList = Mapper.Map<List<ClientImportOption>, List<ClientImportOptionDto>>(listClientImportOption);
                    }
                }
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
            return dtoList;
        }


        public int InsertClientImportOption(int ClientId, string ImportOption, string Status)
        {
            int result = 0;
            try
            {
                using (var sdaCloudEntities = new SDACloudEntities())
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingEntity = sdaCloudEntities
                                            .ClientImportOptions
                                            .Where(i => i.ClientID == ClientId && i.ImportOption.Equals(ImportOption) /*&& i.Status.Equals(Status)*/)
                                            .FirstOrDefault();

                        if (existingEntity == null)
                        {
                            existingEntity = new ClientImportOption() 
                            {
                                ClientID = ClientId,
                                ImportOption = ImportOption
                            };
                        }
                        else 
                        {
                            existingEntity.Status = Status;
                            existingEntity.UpdateDT = DateTime.UtcNow;
                        }
                        result = sdaCloudEntities.SaveChanges();
                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return result;
        }
    }
}
