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
                using (SDACloudEntities SDACloudEntities = new SDACloudEntities())
                {
                    using (SDACloudEntities)
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            var existingEntity = SDACloudEntities.ClientImportOptions.FirstOrDefault(i => i.ClientID == ClientId && i.ImportOption.Equals(ImportOption));
                            if (existingEntity == null)
                            {
                                ClientImportOption newEntity = new ClientImportOption()
                                {
                                    ClientID = ClientId,
                                    ImportOption = ImportOption,
                                    Status = Status
                                };
                                SDACloudEntities.ClientImportOptions.AddObject(newEntity);
                                result = SDACloudEntities.SaveChanges();
                                transactionScope.Complete();     
                            }
                            else
                            {
                                if (!existingEntity.Status.Equals(Status))
                                {
                                    existingEntity.Status = Status;
                                    existingEntity.UpdateDT = DateTime.UtcNow;
                                    result = SDACloudEntities.SaveChanges();
                                    transactionScope.Complete();     
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + " " + e.InnerException.Message );
            }
            return result;
        }
    }
}
