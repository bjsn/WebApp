using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System.Reflection;

namespace Corspro.Data.External
{
    public class ClientDefinedFieldDL
    {
        /// <summary>
        /// Gets the client defined fields.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public List<ClientDefinedFieldDto> GetClientDefinedFields(int clientID, string table)
        {
            List<ClientDefinedFieldDto> cDFList = GetClientDefinedFields(clientID);

            if (table.Equals("Quote"))
            {
                cDFList = cDFList.Where(i => i.Table == table).ToList();
            }
            else
            {
                cDFList = cDFList.Where(i => i.Table == table || (i.Table == "Quote" && i.Field.Contains("ClientDefinedTotal"))).ToList();
            }

            return cDFList;
        }

        public List<ClientDefinedFieldDto> GetClientDefinedFields(int clientID, string table, string field)
        {
            List<ClientDefinedFieldDto> cDFList = GetClientDefinedFields(clientID, table);

            cDFList = cDFList.Where(i => i.Field == field).ToList();

            return cDFList;
        }

        /// <summary>
        /// Gets the client defined fields.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public List<ClientDefinedFieldDto> GetClientDefinedFields(int clientID)
        {
            List<ClientDefinedFieldDto> cDFList = new List<ClientDefinedFieldDto>();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var clientFields = sdaCloudEntities.ClientDefinedFields.Where(i => i.ClientID == clientID).ToList();
                    Mapper.CreateMap<ClientDefinedField, ClientDefinedFieldDto>();

                    if (clientFields.Count > 0)
                    {
                        cDFList = Mapper.Map<List<ClientDefinedField>, List<ClientDefinedFieldDto>>(clientFields);
                        ////Mapper.AssertConfigurationIsValid();

                    }
                }
            }
            return cDFList;
        }

        public bool ValidateFieldToUpdate(int clientID, int cdfId, string field, string newValue)
        {
            bool result = true;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingCDF = sdaCloudEntities.ClientDefinedFields.FirstOrDefault(i => i.ClientDefinedFieldID == cdfId);

                    if (existingCDF != null)
                    {
                        var existingOpp = sdaCloudEntities.ClientDefinedFields.Where(i => i.ClientID == clientID).ToList();

                        if (field.Equals("Table"))
                        {
                            existingOpp = existingOpp.Where(opp => opp.Table.Equals(newValue) && opp.Field.Equals(existingCDF.Field)).ToList();
                        }

                        if (field.Equals("Field"))
                        {
                            existingOpp = existingOpp.Where(opp => opp.Table.Equals(existingCDF.Table) && opp.Field.Equals(newValue)).ToList();
                        }

                        if (existingOpp.Count > 0) return false;
                    }
                }
            }
            return result;
        }

        public int AddClientDefinedField(ClientDefinedFieldDto clientDefinedField)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingCDF = new ClientDefinedField
                    {
                        ClientID = clientDefinedField.ClientID,
                        Table = clientDefinedField.Table,
                        Field = clientDefinedField.Field,
                        ColumnHeader = clientDefinedField.ColumnHeader,
                        Format = clientDefinedField.Format,
                        SDARangeName = clientDefinedField.SDARangeName
                    };

                    sdaCloudEntities.ClientDefinedFields.AddObject(existingCDF);

                    result = sdaCloudEntities.SaveChanges();

                    if (result > 0)
                    {
                        InterfaceXRefDL interfaceXrefDL = new InterfaceXRefDL();
                        var interfaceXrefId = interfaceXrefDL.AddInterfaceXRef(clientDefinedField);
                        if (interfaceXrefId > 0)
                        {
                            existingCDF.InterfaceXRefID = interfaceXrefId;
                            sdaCloudEntities.SaveChanges();
                        }
                    }

                    result = existingCDF.ClientDefinedFieldID;

                    transactionScope.Complete();
                }
            }
            return result;
        }

        public int UpdateCDFField(int cdfId, string field, string newValue)
        {
            int result = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingCDF = sdaCloudEntities.ClientDefinedFields.FirstOrDefault(i => i.ClientDefinedFieldID == cdfId);

                        if (existingCDF != null)
                        {
                            Type t = existingCDF.GetType();
                            PropertyInfo info = t.GetProperty(field);

                            if (info == null)
                                return -1;
                            if (!info.CanWrite)
                                return -1;
                            info.SetValue(existingCDF, newValue, null);

                            result = sdaCloudEntities.SaveChanges();

                            Mapper.CreateMap<ClientDefinedField, ClientDefinedFieldDto>();

                            var cDF = Mapper.Map<ClientDefinedField, ClientDefinedFieldDto>(existingCDF);
                            ////Mapper.AssertConfigurationIsValid();

                            InterfaceXRefDL interfaceXrefDL = new InterfaceXRefDL();
                            var interfaceXrefId = interfaceXrefDL.UpdateInterfaceXRef(cDF);
                        }

                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }


        public string DeleteCDFs(string uList)
        {
            string result = string.Empty;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<string> tagIds = uList.Split(',').ToList();
                var clientDefinedFields = new List<ClientDefinedField>();
                foreach (var uv in tagIds)
                {
                    int uvId;
                    Int32.TryParse(uv, out uvId);
                    var clientDefinedField = sdaCloudEntities.ClientDefinedFields.Where(q => q.ClientDefinedFieldID == uvId).SingleOrDefault();
                    if (clientDefinedField != null)
                    {
                        clientDefinedFields.Add(clientDefinedField);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(result)) result += ",";
                        result += uv;
                    }
                }
                foreach (var cdf in clientDefinedFields)
                {
                    Mapper.CreateMap<ClientDefinedField, ClientDefinedFieldDto>();

                    var existingCDF = Mapper.Map<ClientDefinedField, ClientDefinedFieldDto>(cdf);
                    ////Mapper.AssertConfigurationIsValid();

                    InterfaceXRefDL interfaceXrefDL = new InterfaceXRefDL();
                    var deleted = interfaceXrefDL.DeleteInterfaceXRef(existingCDF);
                    if (deleted >= 0)
                    {
                        //check this
                        //sdaCloudEntities.DeleteObject(cdf);
                    }
                }
                sdaCloudEntities.SaveChanges();
            }
            return result;
        }

        public ClientDto GetClientIdByName(string clientName)
        {
            ClientDto cLientBase = new ClientDto();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var client = sdaCloudEntities.Clients.Where(i => i.ClientName == clientName).ToList().FirstOrDefault();
                    Mapper.CreateMap<Client, ClientDto>();

                    if (client != null)
                    {
                        cLientBase = Mapper.Map<Client, ClientDto>(client);
                        Mapper.AssertConfigurationIsValid();
                    }
                }
            }
            return cLientBase;
        }


        public ClientDto GetClientById(int clientId)
        {
            ClientDto cLientBase = new ClientDto();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var client = sdaCloudEntities.Clients.Where(i => i.ClientID == clientId).ToList().FirstOrDefault();
                    Mapper.CreateMap<Client, ClientDto>();

                    if (client != null)
                    {
                        cLientBase = Mapper.Map<Client, ClientDto>(client);
                        Mapper.AssertConfigurationIsValid();
                    }
                }
            }
            return cLientBase;
        }
    }
}
