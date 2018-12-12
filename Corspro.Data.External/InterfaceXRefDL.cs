using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System.Transactions;

namespace Corspro.Data.External
{
    public class InterfaceXRefDL
    {
        /// <summary>
        /// Gets the records with mapped fields.
        /// </summary>
        /// <param name="interfaceId">The interface identifier.</param>
        /// <param name="txn">The TXN.</param>
        /// <returns></returns>
        public List<InterfaceXRefDto> GetRecordsWithMappedFields(int clientId, string txn)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingClient = sdaCloudEntities.InterfaceXRefs.Where(o => o.ClientID == clientId && o.Txn == txn).ToList();

                    Mapper.CreateMap<InterfaceXRef, InterfaceXRefDto>()
                        .ForMember(dest => dest.CRMXrefUpdDT, opt => opt.Ignore());

                    var peopleVm = Mapper.Map<List<InterfaceXRef>, List<InterfaceXRefDto>>(existingClient);

                    return peopleVm;
                }
            }
        }

        /// <summary>
        /// Gets the records with mapped fields by client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="txn">The TXN.</param>
        /// <param name="crmXrefUpdDT">The CRM xref upd dt.</param>
        /// <returns></returns>
        public List<InterfaceXRefDto> GetRecordsWithMappedFieldsByClient(long clientId, string txn, DateTime crmXrefUpdDT)
        {
            int defaultClientId = 999999;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingClient = sdaCloudEntities.InterfaceXRefs.Where(o => (o.ClientID == clientId || o.ClientID == defaultClientId) && o.Txn.ToUpper() == txn).ToList();

                    Mapper.CreateMap<InterfaceXRef, InterfaceXRefDto>()
                        .ForMember(dest => dest.CRMXrefUpdDT, opt => opt.Ignore());

                    var peopleVm = Mapper.Map<List<InterfaceXRef>, List<InterfaceXRefDto>>(existingClient);

                    foreach (var p in peopleVm)
                    {
                        p.CRMXrefUpdDT = crmXrefUpdDT.ToString(CultureInfo.InvariantCulture);
                    }

                    return peopleVm;
                }
            }
        }

        public int AddInterfaceXRef(ClientDefinedFieldDto clientDefinedField)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingCDF = new InterfaceXRef
                    {
                        Txn = "CRMXref",
                        ClientID = clientDefinedField.ClientID,
                        InterfaceTable = clientDefinedField.Table,
                        InterfaceField = clientDefinedField.Field,
                        InterfaceFieldType = (clientDefinedField.Format.Equals("Text")) ? "Varchar" : "Decimal",
                        SDASMTable = clientDefinedField.Table,
                        SDASMField = clientDefinedField.Field,
                        SDASMFieldType = clientDefinedField.Format,
                        SDARangeName = clientDefinedField.SDARangeName
                    };

                    sdaCloudEntities.InterfaceXRefs.AddObject(existingCDF);

                    sdaCloudEntities.SaveChanges();

                    result = existingCDF.InterfaceXRefID;

                    if (result > 0)
                    {
                        var client = sdaCloudEntities.Clients.FirstOrDefault(o => o.ClientID == clientDefinedField.ClientID);
                        if (client != null)
                        {
                            client.CRMXrefUpdDT = DateTime.Now;
                            sdaCloudEntities.SaveChanges();
                        }
                    }

                    transactionScope.Complete();
                }
            }
            return result;
        }

        public int UpdateInterfaceXRef(ClientDefinedFieldDto clientDefinedField)
        {
            int result = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingCDF = sdaCloudEntities.InterfaceXRefs.FirstOrDefault(i => i.InterfaceXRefID == clientDefinedField.InterfaceXRefID);

                    if (existingCDF != null)
                    {
                        existingCDF.InterfaceTable = clientDefinedField.Table;
                        existingCDF.InterfaceField = clientDefinedField.Field;
                        existingCDF.InterfaceFieldType = (clientDefinedField.Format.Equals("Text")) ? "Varchar" : "Decimal";
                        existingCDF.SDASMTable = clientDefinedField.Table;
                        existingCDF.SDASMField = clientDefinedField.Field;
                        existingCDF.SDASMFieldType = clientDefinedField.Format;
                        existingCDF.SDARangeName = clientDefinedField.SDARangeName;
                    }

                    result = sdaCloudEntities.SaveChanges();

                    if (result > 0)
                    {
                        var client = sdaCloudEntities.Clients.FirstOrDefault(o => o.ClientID == clientDefinedField.ClientID);
                        if (client != null)
                        {
                            client.CRMXrefUpdDT = DateTime.Now;
                            sdaCloudEntities.SaveChanges();
                        }
                    }

                    transactionScope.Complete();
                }
            }
            return result;
        }

        public int DeleteInterfaceXRef(ClientDefinedFieldDto clientDefinedField)
        {
            int result = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingCDF = sdaCloudEntities.InterfaceXRefs.FirstOrDefault(i => i.InterfaceXRefID == clientDefinedField.InterfaceXRefID);

                    if (existingCDF != null)
                    {
                        //sdaCloudEntities.DeleteObject(existingCDF);
                    }

                    result = sdaCloudEntities.SaveChanges();

                    if (result >= 0)
                    {
                        var client = sdaCloudEntities.Clients.FirstOrDefault(o => o.ClientID == clientDefinedField.ClientID);
                        if (client != null)
                        {
                            client.CRMXrefUpdDT = DateTime.Now;
                            sdaCloudEntities.SaveChanges();
                        }
                    }

                    transactionScope.Complete();
                }
            }
            return result;
        }
    }
}