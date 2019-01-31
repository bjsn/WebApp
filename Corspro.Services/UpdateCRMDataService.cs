using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Corspro.Business.External;
using Corspro.Domain;
using Corspro.Domain.Dto;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Corspro.Services
{
    /// <summary>
    /// UpdateCRMDataService is a class that implements all the functionalities of the application
    /// </summary>
    public class UpdateCRMDataService : IUpdateCRMDataService
    {
        private static readonly List<CrmSdaMapper> DatabaseToCRMMap = new List<CrmSdaMapper>();

        public Response Authenticate(ClientLoginDto pClientLoginDto)
        {
            var response = new Response();
            var clientLoginBL = new ClientLoginBL();
            if (clientLoginBL.ValidateClientLogin(pClientLoginDto))
            {
                response.Results.Add("Authenticated");
            }
            else
            {
                response.Errors.Add("Invalid authentication info");
            }
            return response;
        }

        public Response GetCRMXRefUpdDT(int clientId, string cloudCRMXrefUpdDT)
        {
            Response response = new Response();
            ClientLoginBL _clientBl = new ClientLoginBL();
            DateTime extDT;

            if (DateTime.TryParse(cloudCRMXrefUpdDT, out extDT))
            {
                var newDate = _clientBl.GetCRMXrefUpdDT(clientId);
                if (extDT < newDate)
                {
                    response.Results.Add(newDate.ToString());
                }
                else
                {
                    response.Results.Add(string.Empty);
                }
                var crmSystem = _clientBl.GetCRMXrefCRMSystem(clientId);
                crmSystem = crmSystem != null ? crmSystem : string.Empty;
                response.Results.Add(crmSystem);

                var client = _clientBl.GetClientSyncValues(clientId);
                response.Results.Add(client.SyncServerLocation);
                response.Results.Add(client.SyncServerLogin);
                response.Results.Add(client.SyncServerPwd);
                response.Results.Add(client.OpportunityMgmt);
                response.Results.Add(client.CorsProSyncInd);
            }
            return response;
        }

        public Response RunUpdateCRMOpportunities(OpportunityDto opportunityDto)
        {
            // Initialise variables
            Response response = Authenticate(opportunityDto.LoginInfo);
            if (response.Errors.Count > 0)
            {
                return response;
            }

            var opportunitiesToUpdate = opportunityDto.OpportunityTable;

            var xRefDef = opportunityDto.CRMXrefDefinition;
            CreateSqlAndMapper(xRefDef);

            try
            {
                var opportunityBL = new OpportunityBL();
                var utilityBl = new UtilityBL();

                // Create a list of opportunities 
                // mapping the values of the opportunity table with the ones on the Salesforce Opportunity object
                var recordsSdaUpdate = new List<OpportunityDto>();
                var recordsSdaAdd = new List<OpportunityDto>();

                int crmClientId = -1;

                foreach (DataRow row in opportunitiesToUpdate.Rows)
                {
                    int clientID = -1;
                    if (row.Table.Columns.Contains("ClientID") && row["ClientID"] != DBNull.Value)
                    {
                        int.TryParse(row["ClientID"].ToString(), out clientID);
                        crmClientId = clientID;
                    }
                    else
                    {
                        response.Errors.Add("Invalid Client Id.");
                    }

                    var strCRMOppId = string.Empty;
                    if (row.Table.Columns.Contains("CRMOppID") && row["CRMOppID"] != DBNull.Value)
                    {
                        strCRMOppId = row["CRMOppID"].ToString();
                    }

                    List<OppStatusDto> lstat = utilityBl.GetStatuses(clientID);
                    bool hasStatus = false;
                    if (row.Table.Columns.Contains("OppStatus") && row["OppStatus"] != DBNull.Value)
                    {
                        var strStatus = row["OppStatus"].ToString();
                        int statValue = 0;
                        foreach (var stat in lstat)
                        {
                            if (stat.OppStatus.ToLower().Trim().Equals(strStatus.ToLower().Trim()))
                            {
                                statValue = stat.ID;
                                break;
                            }
                        }

                        if (statValue > 0)
                        {
                            row["OppStatus"] = statValue;
                            hasStatus = true;
                        }
                    }

                    if (!hasStatus)
                    {
                        if (lstat.Count > 0)
                        {
                            var defaultStatus = lstat.Where(s => s.Default.Equals("Y")).FirstOrDefault();
                            if (defaultStatus != null)
                            {
                                row["OppStatus"] = defaultStatus.ID;
                                hasStatus = true;
                            }
                        }

                        if (!hasStatus)
                        {
                            response.Errors.Add("Invalid Opp Status.");
                        }
                    }


                    OpportunityDto opportunity = null;
                    if (!string.IsNullOrEmpty(strCRMOppId))
                    {
                        opportunity = opportunityBL.GetOpportunityByClientIDAndCRMOppID(clientID, strCRMOppId);
                    }

                    bool isNewOpportunity;
                    if (opportunity == null)
                    {
                        // Create a new opportunity
                        opportunity = new OpportunityDto();
                        isNewOpportunity = true;
                    }
                    else
                    {
                        isNewOpportunity = false;
                    }

                    // Copy the fields to the opportunity
                    var type = opportunity.GetType();
                    //loop over the rows mapping the database field with the corresponding field on the CRM
                    foreach (var mappingObject in DatabaseToCRMMap)
                    {
                        var propertyInfo = type.GetProperty(mappingObject.CRMField);
                        if (propertyInfo != null)
                        {
                            if (row[mappingObject.SdaField] != DBNull.Value)
                            {
                                utilityBl.SetProperty(opportunity, propertyInfo,
                                                            row[mappingObject.SdaField]);
                            }
                        }
                    }

                    if (isNewOpportunity)
                        recordsSdaAdd.Add(opportunity);
                    else
                        recordsSdaUpdate.Add(opportunity);
                }

                // Update the list of opportunities on Salesforce
                response = new Response();
                Response responseUpdate = opportunityBL.UpdateSdaCloudOpportunity(recordsSdaUpdate);
                Response responseAdd = opportunityBL.AddSdaCloudOpportunity(recordsSdaAdd);

                // Merge both results
                foreach (string result in responseUpdate.Results)
                {
                    response.Results.Add(result);
                }
                foreach (string result in responseAdd.Results)
                {
                    response.Results.Add(result);
                }
                //Merge both errors
                foreach (string error in responseUpdate.Errors)
                {
                    response.Errors.Add(error);
                }
                foreach (string error in responseAdd.Errors)
                {
                    response.Errors.Add(error);
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                if (ex.InnerException != null)
                {
                    response.Errors.Add(ex.InnerException.Message);
                }
            }
            return response;
        }

        /// <summary>
        /// Runs the update CRMX reference.
        /// </summary>
        /// <param name="interfaceXRefDto">The interface x reference dto.</param>
        /// <returns></returns>
        public List<InterfaceXRefDto> RunUpdateCRMXRef(InterfaceXRefDto interfaceXRefDto)
        {
            var clientBL = new ClientLoginBL();
            var sdaLastUpd = clientBL.GetCRMXrefUpdDT(interfaceXRefDto.ClientID);

            DateTime mdbLastUpd;
            if (!DateTime.TryParse(interfaceXRefDto.CRMXrefUpdDT, out mdbLastUpd))
            {
                mdbLastUpd = DateTime.MinValue;
            }
            if (sdaLastUpd > mdbLastUpd)
            {
                var interfaceXRefBl = new InterfaceXRefBL();
                var dataToUpd = interfaceXRefBl.GetRecordsWithMappedFieldsByClient(interfaceXRefDto.ClientID, interfaceXRefDto.Txn, sdaLastUpd);
                return dataToUpd;
            }
            return new List<InterfaceXRefDto>();
        }

        /// <summary>
        /// Runs the update CRM quotes.
        /// </summary>
        /// <param name="quoteDto">The quote dto.</param>
        /// <returns></returns>
        public Response RunUpdateCRMQuotes(QuoteDto quoteDto)
        {
            // Initialise variables
            Response response = Authenticate(quoteDto.LoginInfo);
            if (response.Errors.Count > 0)
            {
                return response;
            }

            var quotesToUpdate = quoteDto.QuoteTable;
            var xRefDef = quoteDto.CRMXrefDefinition;
            CreateSqlAndMapper(xRefDef);

            try
            {
                var quoteBl = new QuoteBL();
                var utilityBl = new UtilityBL();

                // Create a list of opportunities 
                // mapping the values of the opportunity table with the ones on the Salesforce Opportunity object
                var recordsSdaUpdate = new List<QuoteDto>();
                var recordsSdaAdd = new List<QuoteDto>();
                int crmClientId = -1;

                foreach (DataRow row in quotesToUpdate.Rows)
                {
                    int clientID = -1;
                    if (row["ClientID"] != DBNull.Value)
                    {
                        int.TryParse(row["ClientID"].ToString(), out clientID);
                        crmClientId = clientID;
                    }
                    else
                    {
                        response.Errors.Add("Invalid Client Id.");
                    }

                    var strQuoteId = string.Empty;
                    if (row["QuoteID"] != DBNull.Value)
                    {
                        strQuoteId = row["QuoteID"].ToString();
                    }
                    else
                    {
                        response.Errors.Add("Invalid QuoteId.");
                    }

                    var quote = quoteBl.GetQuoteByClientIDAndQuoteID(clientID, strQuoteId);

                    bool isNewQuote;
                    if (quote == null)
                    {
                        // Create a new opportunity
                        quote = new QuoteDto();
                        isNewQuote = true;
                    }
                    else
                    {
                        isNewQuote = false;
                    }

                    // Copy the fields to the opportunity
                    var type = quote.GetType();
                    //loop over the rows mapping the database field with the corresponding field on the CRM
                    foreach (var mappingObject in DatabaseToCRMMap)
                    {
                        var propertyInfo = type.GetProperty(mappingObject.CRMField);
                        if (propertyInfo != null)
                        {
                            if (row[mappingObject.SdaField] != DBNull.Value)
                            {
                                utilityBl.SetProperty(quote, propertyInfo,
                                                            row[mappingObject.SdaField]);
                            }
                        }
                    }

                    if (isNewQuote)
                        recordsSdaAdd.Add(quote);
                    else
                        recordsSdaUpdate.Add(quote);
                }

                // Update the list of opportunities on Salesforce
                response = new Response();
                Response responseUpdate = quoteBl.UpdateSdaCloudQuote(recordsSdaUpdate);
                Response responseAdd = quoteBl.AddSdaCloudQuote(recordsSdaAdd);

                // Merge both results
                foreach (string result in responseUpdate.Results)
                {
                    response.Results.Add(result);
                }
                foreach (string result in responseAdd.Results)
                {
                    response.Results.Add(result);
                }
                //Merge both errors
                foreach (string error in responseUpdate.Errors)
                {
                    response.Errors.Add(error);
                }
                foreach (string error in responseAdd.Errors)
                {
                    response.Errors.Add(error);
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Gets the user info by LoginID
        /// </summary>
        /// <param name="clientLogin">The user object</param>
        /// <returns></returns>
        public Response GetUserByLoginIDAndPassword(UserDto clientLogin)
        {
            var response = new Response();
            var userBL = new UserBL();
            var appVersionBL = new AppVersionBL();

            // Get the user data
            UserDto user = userBL.GetUserByLoginIDAndPassword(clientLogin);
            if (user != null)
            {
                response.Results.Add("Success");
                // For security only past the data required
                UserDto userToSend = new UserDto
                {
                    UserId = user.UserId,
                    ClientId = user.ClientId
                };
                // Serialize the object
                string userSerialize = JsonConvert.SerializeObject(userToSend);
                response.Results.Add(userSerialize);

                var version = appVersionBL.GetLatestSWVersion("SMDESKTOP");
                response.Results.Add(version);
            }
            else
            {
                response.Errors.Add("Error");
                response.Errors.Add("Invalid user data.");
            }
            return response;
        }

        /// <summary>
        /// Gets the user info by UserID and ClientID
        /// </summary>
        /// <param name="clientLogin">The user object</param>
        /// <returns></returns>
        public Response GetUserStatusByUserIDAndClientID(UserDto clientLogin)
        {
            var response = new Response();
            var userBL = new UserBL();
            var appVersionBL = new AppVersionBL();

            // Get the user data
            UserDto user = userBL.GetUserByUserIDAndClientID(clientLogin);
            if (user != null)
            {
                response.Results.Add("Success");
                // For security only past the data required
                UserDto userToSend = new UserDto
                {
                    DeleteInd = user.DeleteInd,
                    ClientId = user.ClientId,
                    SDAOppMgmt = user.SDAOppMgmt,
                    UserId = user.UserId
                };
                // Serialize the object
                string userSerialize = JsonConvert.SerializeObject(userToSend);
                response.Results.Add(userSerialize);

                var version = appVersionBL.GetLatestSWVersion("SMDESKTOP");
                response.Results.Add(version);
            }
            else
            {
                response.Results.Add("Error");
                response.Errors.Add("Invalid user data.");
            }
            return response;
        }

        /// <summary>
        /// Get quote info by client id and quote id
        /// </summary>
        /// <param name="clientID">The client id</param>
        /// <param name="quoteID">The quote id</param>
        /// <returns></returns>
        public Response GetQuoteByClientIDAndQuoteID(int clientID, string quoteID)
        {
            var response = new Response();
            var quoteBL = new QuoteBL();

            // Get the opportunity data
            QuoteDto quote = quoteBL.GetQuoteByClientIDAndQuoteID(clientID, quoteID);
            if (quote != null)
            {
                response.Results.Add("Success");
                // Serialize the object
                //string userSerialize = JsonConvert.SerializeObject(quote);
                string userSerialize = quote.Rollup;
                response.Results.Add(userSerialize);
            }
            else
            {
                response.Results.Add("Error");
                response.Errors.Add("Invalid opportunity data.");
            }
            return response;
        }


        /// <summary>
        /// Get quote info by client id and quote id
        /// </summary>
        /// <param name="clientID">The client id</param>
        /// <param name="quoteID">The quote id</param>
        /// <returns></returns>
        public Response GetQuoteByClientIdUserIdAndQuoteId(int clientId, int userId, string quoteID)
        {
            var response = new Response();
            var quoteBL = new QuoteBL();
            var userBL = new UserBL();

            // Get the opportunity data
            QuoteDto quote = quoteBL.GetQuoteByClientIDAndQuoteID(clientId, quoteID);
            if (quote != null)
            {
                var client = userBL.GetByUserIdAndClientId(clientId, userId);
                if (client != null)
                {
                    if (client.DeleteInd.ToString().ToUpper().Equals("N"))
                    {
                        response.Results.Add("Success");
                        // Serialize the object
                        //string userSerialize = JsonConvert.SerializeObject(quote);
                        string FilePlatformFileIDSerialize = quote.FilePlatformFileID;
                        response.Results.Add(FilePlatformFileIDSerialize);
                        return response;
                    }
                    else
                    {
                        response.Results.Add("Error");
                        response.Errors.Add("User inactive.");
                    }
                }
                else
                {
                    response.Results.Add("Error");
                    response.Errors.Add("The user does not exist");
                }
            }
            else
            {
                response.Results.Add("Error");
                response.Errors.Add("Invalid opportunity data.");
            }
            return response;
        }

        /// <summary>
        /// Get an opportunity by CRMOppID
        /// </summary>
        /// <param name="clientID">The ClientID</param>
        /// <param name="value">The value.</param>
        /// <param name="searchType">Type of the search.</param>
        /// <returns></returns>
        public Response GetOpportunityByClientIDAndSearchType(int clientID, string value, string searchType)
        {
            var response = new Response();
            var opportunityBL = new OpportunityBL();
            OpportunityDto opportunity = null;

            // Get the opportunity data
            switch (searchType.ToUpper())
            {
                case "CRMOPPID":
                    opportunity = opportunityBL.GetNonDeletedOpportunityByClientIDAndCRMOppID(clientID, value);
                    break;
                case "QUOTEID":
                    opportunity = opportunityBL.GetNonDeletedOpportunityByClientIDAndQuoteID(clientID, value);
                    break;
                case "OPPID":
                    int oppId;
                    if (int.TryParse(value, out oppId))
                    {
                        opportunity = opportunityBL.GetNonDeletedOpportunityByClientIDAndOppID(clientID, oppId);
                    }
                    break;
            }

            if (opportunity != null)
            {
                response.Results.Add("Success");
                // Serialize the object
                string userSerialize = JsonConvert.SerializeObject(opportunity);
                response.Results.Add(userSerialize);
            }
            else
            {
                response.Results.Add("Error");
                response.Errors.Add("Invalid opportunity data.");
            }
            return response;
        }

        /// <summary>
        /// Get the opportunities for client
        /// </summary>
        /// <param name="clientID">The Client ID</param>
        /// <param name="opportunityName">The opportunity name</param>
        /// <param name="companyName">The company name</param>
        /// <param name="owner">The owner</param>
        /// <param name="opportunityStatus">The opportunity status</param>
        /// <returns></returns>
        public Response GetOpportunities(int clientID, string oppId, string opportunityName, string companyName, string owner, string opportunityStatus)
        {
            var response = new Response();
            var opportunityBL = new OpportunityBL();

            List<OpportunityDto> opportunities = opportunityBL.GetOpportunities(clientID, oppId, opportunityName, companyName, owner, opportunityStatus);

            response.Results.Add("Success");

            List<SearchOpportunityDto> searchOpportunities = opportunities.Select(opp => new SearchOpportunityDto
            {
                OppID = opp.OppID,
                CRMOppID = opp.CRMOppID,
                OppName = opp.OppName,
                CompanyName = opp.CompanyName,
                OppStatus = opp.OppStatus.HasValue ? opp.OppStatus.Value.ToString() : string.Empty,
                OppStatusName = opp.OppStatusName,
                OppProbability = opp.OppProbability.HasValue ? opp.OppProbability.Value.ToString() : string.Empty,
                OppCloseDate = opp.OppCloseDate,
                OppOwner = opp.OppOwner,
                OppOwnerName = opp.OppOwnerName
            }).ToList();

            string userSerialize = JsonConvert.SerializeObject(searchOpportunities);
            response.Results.Add(userSerialize);

            return response;
        }

        /// <summary>
        /// Creates the list of SQL fields that the system needs to map from the database 
        /// and also creates a mapper object that relates the CRM fields and the database fields.
        /// </summary>
        /// <param name="dtcrmXref">The CRMxref datatable.</param>
        /// <returns></returns>
        private static void CreateSqlAndMapper(DataTable dtcrmXref)
        {
            DatabaseToCRMMap.Clear();
            foreach (var mappingObject in from DataRow row in dtcrmXref.Rows
                                          select new CrmSdaMapper
                                          {
                                              CRMField = (row["CRMField"] != DBNull.Value) ? (string)row["CRMField"] : string.Empty,
                                              CRMFieldType = (row["CRMFieldType"] != DBNull.Value) ? (string)row["CRMFieldType"] : string.Empty,
                                              CRMTable = (row["CRMTable"] != DBNull.Value) ? (string)row["CRMTable"] : string.Empty,
                                              SdaField = (row["SDAField"] != DBNull.Value) ? (string)row["SDAField"] : string.Empty,
                                              SdaFieldType = (row["SDAFieldType"] != DBNull.Value) ? (string)row["SDAFieldType"] : string.Empty,
                                              SdaTable = (row["SDATable"] != DBNull.Value) ? (string)row["SDATable"] : string.Empty
                                          })
            {
                DatabaseToCRMMap.Add(mappingObject);
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public string GetClientIdByName(string ClientName)
        {
            var ClientDefinedBl = new ClientDefinedFieldBL();
            ClientDto clientDto = ClientDefinedBl.GetClientIdByName(ClientName);
            return clientDto.ClientId.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public ClientDto GetClientById(int clientId)
        {
            var ClientDefinedBl = new ClientDefinedFieldBL();
            ClientDto clientDto = ClientDefinedBl.GetClientById(clientId);
            return clientDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        public List<ClientUpdateDBDto> GetClientUpdateDB(int clientId, int userType)
        {
            var clientUpdateBL = new ClientUpdateDBBL();
            List<ClientUpdateDBDto> ClientUpdateDBDtoList = clientUpdateBL.GetClientUpdateDB(clientId, userType);
            return ClientUpdateDBDtoList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationListName"></param>
        /// <returns></returns>
        public List<ConfigurationDto> GetConfigurationList(string configurationListName)
        {
            var configurationBL = new ConfigurationBL();
            List<ConfigurationDto> configurationList = configurationBL.GetConfigurationListByName(configurationListName);
            return configurationList;
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public bool HasPermissionToUpload(int clientId, string fileName)
        {
            var clientUpdateBL = new ClientUpdateDBBL();
            return clientUpdateBL.HasPermissionToUpload(clientId, fileName);
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="fileName"></param>
        /// <param name="AWSid"></param>
        /// <param name="AWSFilePath"></param>
        /// <param name="AWSFileName"></param>
        /// <param name="DBFileUpdDT"></param>
        /// <param name="DBUploadedDt"></param>
        public ClientUpdateDBDto UpdateClientUpdateDB(int clientId, string fileName, string AWSid, string AWSFilePath, string AWSFileName, string DBFileUpdDT,
                                                      string DBUploadedDt, bool BetaVersion, int UploaderClientID, int UploaderUserID, string UploaderUserName)
        {
            var clientUpdateBL = new ClientUpdateDBBL();
            return clientUpdateBL.UpdateClientUpdateDB(clientId, fileName, AWSid, AWSFilePath, AWSFileName, DBFileUpdDT, DBUploadedDt, BetaVersion, UploaderClientID, UploaderUserID, UploaderUserName);
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetClientNameByIdAndUserId(int clientId, int userId)
        {
            var clientUpdateBL = new ClientUpdateDBBL();
            return clientUpdateBL.GetClientNameByIdAndUserId(clientId, userId);
        }

        public void UpdateUserRegistrationDT(int userID, int clientID)
        {
            var userBL = new UserBL();
            userBL.UpdateUserRegistrationDT(userID, clientID);
        }

        /// <summary>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="clientID"></param>
        public void UpdateUserLastCheckDT(int userID, int clientID)
        {
            var userBL = new UserBL();
            userBL.UpdateUserLastCheckDT(userID, clientID);
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public ClientUpdateDBDto GetLastCloudDBFileUpdDT(int clientId, string dbName)
        {
            var clientUpdateBL = new ClientUpdateDBBL();
            return clientUpdateBL.GetLastCloudDBFileUpdDT(clientId, dbName);
        }

        //new methods
        private string[] EncryptString(string i_MessageBytes)
        {
            string str = string.Empty;
            string s = "kljsdkkdlo4454GG";
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            string str3 = "";
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = bytes;
                byte[] iV = aes.IV;
                aes.Padding = PaddingMode.PKCS7;
                str3 = Convert.ToBase64String(iV);
                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(i_MessageBytes);
                        stream2.Write(buffer, 0, buffer.Length);
                        stream2.Flush();
                        stream2.Close();
                    }
                    str = Convert.ToBase64String(stream.ToArray());
                }
            }
            return new string[] { str, str3 };
        }

        /// <summary>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<string> GetInterfaceByIdAndClientId(int userId, int clientId)
        {
            List<string> list = new List<string>();
            UserDto clientLogin = new UserDto
            {
                ClientId = clientId,
                UserId = userId
            };
            UserDto userByUserIDAndClientID = new UserBL().GetUserByUserIDAndClientID(clientLogin);
            if (userByUserIDAndClientID.DeleteInd.ToString().ToUpper().Equals("Y"))
            {
                list.Add("Error: There is not user.");
            }
            else
            {
                InterfaceDto interfaceByClientId = new InterfaceBL().GetInterfaceByClientId(userByUserIDAndClientID.ClientId, "SharePoint");
                if (interfaceByClientId == null)
                {
                    list.Add("Error: There is not Interface register yet.");
                }
                else
                {
                    list.Add(interfaceByClientId.InterfaceLogin);
                    list.AddRange(this.EncryptString(interfaceByClientId.InterfacePwd));
                }
            }
            return list;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public List<string[]> GetLastRetrievedKeys()
        {
            List<string> lastRetrievedKeys = new EtilizeKeyBL().GetLastRetrievedKeys();
            List<string[]> list2 = new List<string[]>();
            if (lastRetrievedKeys.Count > 1)
            {
                list2.Add(this.EncryptString(lastRetrievedKeys[0].ToString()));
                list2.Add(this.EncryptString(lastRetrievedKeys[1].ToString()));
            }
            else
            {
                list2.Add(new string[] { "it works" });
            }
            return list2;
        }

        /// <summary>
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public OpportunityDto GetOpportunityByQuoteId(string quoteId)
        {
            return new OpportunityBL().GetOpportunityByQuoteId(quoteId);
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="quoteId"></param>
        /// <param name="documentType"></param>
        /// <param name="filePlatformId"></param>
        /// <param name="sharedSavedLocation"></param>
        /// <returns></returns>
        public Response SaveDocumentInformation(int clientId, int userId, string quoteId, string documentType, string filePlatformId, string sharedSavedLocation)
        {
            Response response = new Response();
            try
            {
                if (clientId != 0)
                {
                    DocumentDto dto2 = new DocumentDto();
                    dto2.ClientID = (clientId);
                    dto2.CreatedBy = (userId);
                    dto2.QuoteID = (quoteId);
                    dto2.DocumentType = (documentType);
                    dto2.FilePlatformID = (filePlatformId);
                    dto2.LastFileSaveLocation = (sharedSavedLocation);
                    new DocumentBL().Add(dto2);
                }
                response.Results.Add(userId.ToString());
                response.Results.Add(sharedSavedLocation);
            }
            catch (Exception exception)
            {
                response.Errors.Add("Error: " + exception.Message);
            }
            return response;
        }

        /// <summary>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<string> GetInterfaceByIdAndClientId(string userId, string clientId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// possible results
        /// Invalid – if no record was selected
        /// ValidNoOppy – record was selected, (user.sdaoppmgmt is N or (user.sdaoppmgmt is null and (client.sdaoppmgmt is N or null))
        /// ValidOppy - record was selected, (user.sdaoppmgmt is Y or (user.sdaoppmgmt is null and client.sdaoppmgmt is Y))
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserCloudStatusDto GetUserAppStatus(int clientId, int userId) 
        {
            UserCloudStatusDto userCloudStatusDto = new UserCloudStatusDto();
            try 
            {
                var userBL = new UserBL();
                var appVersionBL = new AppVersionBL();
                var configurationBL = new ConfigurationBL();

                UserDto userDto = new UserDto() 
                {
                    ClientId = clientId,
                    UserId = userId
                };
                var user = userBL.GetUserByUserIDAndClientID(userDto);
                var client = GetClientById(clientId);

                if (user != null && client != null)
                {
                    userCloudStatusDto.ClientId = client.ClientId;
                    userCloudStatusDto.UserId = user.UserId;
                    if (!user.DeleteInd.Equals("Y"))
                    {
                        if ((Utilitary.SafeToUpper(user.SDAOppMgmt).Equals("N")) || (string.IsNullOrEmpty(Utilitary.SafeToUpper(user.SDAOppMgmt)) && (String.IsNullOrEmpty(client.SDAOppMgmt) || Utilitary.SafeToUpper(client.SDAOppMgmt).Equals("N"))))
                        {
                            userCloudStatusDto.UserStatus = "ValidNoOppy";
                        }
                        else if (Utilitary.SafeToUpper(user.SDAOppMgmt).Equals("Y") || (string.IsNullOrEmpty(user.SDAOppMgmt) && Utilitary.SafeToUpper(client.SDAOppMgmt).Equals("Y")))
                        {
                            userCloudStatusDto.UserStatus = "ValidOppy";
                        }

                        //getting application information
                        string appVersion = appVersionBL.GetLatestSWVersion("SMDESKTOP");
                        if (!string.IsNullOrEmpty(appVersion))
                        {
                            userCloudStatusDto.AppVersion = appVersion;
                        }

                        //getting configuration dates
                        var ValidUserNextCheckHours = configurationBL.GetConfigurationListByName("ValidUserNextCheckHours").FirstOrDefault();
                        var ValidUserNextCheckReqdDays = configurationBL.GetConfigurationListByName("ValidUserNextCheckReqDays").FirstOrDefault();
                        if (ValidUserNextCheckHours != null)
                        {
                            userCloudStatusDto.ValidUserNextCheckHours = ValidUserNextCheckHours.Value.ToString();
                        }
                        if (ValidUserNextCheckReqdDays != null)
                        {
                            userCloudStatusDto.ValidUserNextCheckReqDays = ValidUserNextCheckReqdDays.Value.ToString();
                        }
                        //updating user last check datetime
                        userBL.UpdateUserLastCheckDT(user);
                    }
                }

                if (string.IsNullOrEmpty(userCloudStatusDto.UserStatus))
                {
                    userCloudStatusDto.UserStatus = "Invalid";
                }
            }
            catch (Exception e) 
            {
                userCloudStatusDto.UserStatus = userCloudStatusDto.UserStatus + "Error: " + e.Message;
            }
            return userCloudStatusDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClientID"></param>
        /// <param name="UserID"></param>
        /// <param name="WindowsUserName"></param>
        /// <param name="MACAddress"></param>
        /// <param name="VersionDotNet"></param>
        /// <param name="VersionExcel"></param>
        /// <param name="VersionWord"></param>
        /// <param name="VersionSDA"></param>
        /// <param name="VersionSalesManager"></param>
        /// <param name="VersionWindows"></param>
        /// <param name="InstallType"></param>
        /// <param name="UserFullName"></param>
        /// <param name="Email"></param>
        /// <param name="CompanyLong"></param>
        /// <param name="Title"></param>
        /// <param name="Phone"></param>
        /// <param name="UserTimeZone"></param>
        /// <returns></returns>
        public string UploadUserMachineData(int ClientID, int UserID, string WindowsUserName, string MACAddress, string VersionDotNet, string VersionExcel, string VersionWord, string VersionSDA, string VersionSalesManager,
                                    string VersionWindows, string InstallType, string UserFullName, string Email, string CompanyLong, string Title, string Phone, string UserTimeZone) 
        {
            string ReturnMessage = "";
            try
            {
                UserMachineDataDto UserMachineDataDto = new UserMachineDataDto()
                {
                    ClientID = ClientID,
                    UserID = UserID,
                    WindowsUserName = WindowsUserName,
                    MACAddress = MACAddress,
                    VersionDotNet = VersionDotNet,
                    VersionExcel = VersionExcel,
                    VersionWord = VersionWord,
                    VersionSDA = VersionSDA,
                    VersionSalesManager = VersionSalesManager,
                    VersionWindows = VersionWindows,
                    InstallType = InstallType,
                    UserName = UserFullName,
                    Email = Email,
                    Company = CompanyLong,
                    Title = Title,
                    Phone = Phone,
                    UserTimeZone = UserTimeZone
                };

                var UMDDL = new UserMachineDataBL();
                var UserMachineData = UMDDL.GetUserMachineData(ClientID, UserID, WindowsUserName, MACAddress);
                if (UserMachineData == null)
                {
                    UMDDL.AddUserMachineData(UserMachineDataDto);
                    ReturnMessage = "Inserted";
                }
                else if (!Utilitary.AreObjectsEquals(UserMachineData, UserMachineDataDto))
                {
                    UMDDL.UpdateUserMachineData(UserMachineDataDto);
                    ReturnMessage = "Updated";
                }
                else 
                {
                    ReturnMessage = "Nothing to update";
                }
            }
            catch (Exception e) 
            {
                ReturnMessage = "Error: shit " + e.Message + " " + e.InnerException;
            }
            return ReturnMessage;
        }


        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserStatus(int clientId, int userId)
        {
            string ReturnMessage = "";
            var userBL = new UserBL();
            try
            {
                UserDto user = new UserDto() 
                {
                    ClientId = clientId,
                    UserId = userId
                };
                UserDto DTUser = userBL.GetUserByUserIDAndClientID(user);
                if (DTUser == null) 
                {
                    ReturnMessage = "Invalid";
                }
                else if (DTUser.DeleteInd.Equals("Y"))
                {
                    ReturnMessage = "Deleted";
                }
                else 
                {
                     ReturnMessage = "Valid";
                }
            }
            catch (Exception e) 
            {
                ReturnMessage = "Error: " + e.Message;
            }
            return ReturnMessage;
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<ClientImportOptionDto> GetClientImportOptionListByClientId(int clientId)
        {
            try
            {
                return new ClientImportOptionBL().GetListByClientId(clientId);
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="ImportOption"></param>
        /// <returns></returns>
        public int InsertClientImportOption(int ClientId, string ImportOption, string Status)
        {
            try
            {
                return new ClientImportOptionBL().InsertClientImportOption(ClientId, ImportOption, Status);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public void AddErrorLogMessage(int userId, int clientId, string errorMessage) 
        {
            try
            {
                new ApplicationLogBL().AddErrorLogMessage(userId, clientId, errorMessage);
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
        }

    }
}
