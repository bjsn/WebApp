using Corspro.Domain;
using Corspro.Domain.Dto;
using Corspro.Services;
using System.Collections.Generic;

namespace Corspro.WCFUpdateService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
    [System.ServiceModel.ServiceBehavior()]
    public class Service : IService
    {
        /// <summary>
        /// Runs the update CRM opportunities.
        /// </summary>
        /// <param name="opportunitiesToUpdate">The opportunities to update.</param>
        /// <returns></returns>
        public Response RunUpdateCRMOpportunities(OpportunityDto opportunitiesToUpdate)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.RunUpdateCRMOpportunities(opportunitiesToUpdate);
        }

        public List<InterfaceXRefDto> RunUpdateCRMXRef(InterfaceXRefDto interfaceXRefDto)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.RunUpdateCRMXRef(interfaceXRefDto);
        }

        public Response GetCRMXRefUpdDT(int clientId, string cloudCRMXrefUpdDT)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetCRMXRefUpdDT(clientId, cloudCRMXrefUpdDT);
        }

        /// <summary>
        /// Runs the update CRM quotes.
        /// </summary>
        /// <param name="quotesToUpdate">The quotes to update.</param>
        /// <returns></returns>
        public Response RunUpdateCRMQuotes(QuoteDto quotesToUpdate)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.RunUpdateCRMQuotes(quotesToUpdate);
        }

        /// <summary>
        /// Gets the user info by LoginID
        /// </summary>
        /// <param name="loginID">The login identifier.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public Response GetUserByLoginIDAndPassword(string loginID, string password)
        {
            UserDto user = new UserDto
            {
                LoginID = loginID,
                Password = password
            };

            var dataService = new UpdateCRMDataService();
            return dataService.GetUserByLoginIDAndPassword(user);
        }

        /// <summary>
        /// Gets the user info by UserID and ClientID
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="clientID">The client identifier.</param>
        /// <returns></returns>
        public Response GetUserStatusByUserIDAndClientID(string userID, int clientID)
        {
            UserDto user = new UserDto();
            int result;
            if (int.TryParse(userID, out result))
            {
                user.UserId = result;
                user.ClientId = clientID;

                var dataService = new UpdateCRMDataService();
                return dataService.GetUserStatusByUserIDAndClientID(user);
            }
            var resp = new Response();
            resp.Errors.Add("UserId must be numeric");
            return resp;
        }

        /// <summary>
        /// Get quote info by client id and quote id
        /// </summary>
        /// <param name="clientID">The client id</param>
        /// <param name="quoteID">The quote id</param>
        /// <returns></returns>
        public Response GetQuoteByClientIDAndQuoteID(int clientID, string quoteID)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetQuoteByClientIDAndQuoteID(clientID, quoteID);
        }

        /// <summary>
        /// Get opportunity by ClientID and QuoteID
        /// </summary>
        /// <param name="clientID">The ClientID</param>
        /// <param name="value">The value.</param>
        /// <param name="searchType">Type of the search.</param>
        /// <returns></returns>
        public Response GetOpportunityByClientIDAndSearchType(int clientID, string value, string searchType)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetOpportunityByClientIDAndSearchType(clientID, value, searchType);
        }

        /// <summary>
        /// Get the opportunities by ClientID, Opportunity Name, Company Name, Owner Last Name and Opportunity Status
        /// </summary>
        /// <param name="clientID">The ClientID</param>
        /// <param name="opportunityName">The opportunity name</param>
        /// <param name="companyName">The company name</param>
        /// <param name="owner">The owner's last name</param>
        /// <param name="opportunityStatus">The opportunity status</param>
        /// <returns></returns>
        public Response GetOpportunities(int clientID, string oppId, string opportunityName, string companyName, string owner, string opportunityStatus)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetOpportunities(clientID, oppId, opportunityName, companyName, owner, opportunityStatus);
        }

        /// <summary>
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public string GetCLientIdByName(string ClientName)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetClientIdByName(ClientName);
        }

        /// <summary>
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        public ClientDto GetClientById(int ClientId)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetClientById(ClientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetClientNameByIdAndUserId(int clientId, int userId)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetClientNameByIdAndUserId(clientId, userId);
        }

        /// <summary>
        /// Service that returns a list of clientUpdates
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<ClientUpdateDBDto> GetClientUpdateDB(int clientId, int userType)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetClientUpdateDB(clientId, userType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationListName"></param>
        /// <returns></returns>
        public List<ConfigurationDto> GetConfigurationList(string configurationListName)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetConfigurationList(configurationListName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool HasPermissionToUpload(int clientId, string fileName)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.HasPermissionToUpload(clientId, fileName);
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
        /// <param name="BetaVersion"></param>
        /// <returns></returns>
        public ClientUpdateDBDto UpdateClientUpdateDB(int clientId, string fileName, string AWSid, string AWSFilePath, string AWSFileName, string DBFileUpdDT, string DBUploadedDt, bool BetaVersion, int UploaderClientID, int UploaderUserID, string UploaderUserName)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.UpdateClientUpdateDB(clientId, fileName, AWSid, AWSFilePath, AWSFileName, DBFileUpdDT, DBUploadedDt, BetaVersion, UploaderClientID, UploaderUserID, UploaderUserName);
        }

        /// <summary>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="clientID"></param>
        public void UpdateUserRegistrationDT(int userID, int clientID)
        {
            var dataService = new UpdateCRMDataService();
            dataService.UpdateUserRegistrationDT(userID, clientID);
        }

        /// <summary>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="clientID"></param>
        public void UpdateUserLastCheckDT(int userID, int clientID)
        {
            var dataService = new UpdateCRMDataService();
            dataService.UpdateUserLastCheckDT(userID, clientID);
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="dbName"></param>
        public ClientUpdateDBDto GetLastCloudDBFileUpdDT(int clientId, string dbName)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetLastCloudDBFileUpdDT(clientId, dbName);
        }

        public Response GetQuoteByClientIdUserIdAndQuoteId(int clientId, int userId, string quoteID)
        {
            var dataService = new UpdateCRMDataService();
            return dataService.GetQuoteByClientIdUserIdAndQuoteId(clientId, userId, quoteID);
        }

        //new methods
        public List<string> GetInterfaceByIdAndClientId(int userId, int clientId)
        {
            return new UpdateCRMDataService().GetInterfaceByIdAndClientId(userId, clientId);
        }

        public List<string[]> GetLastRetrievedKeys()
        {
            return new UpdateCRMDataService().GetLastRetrievedKeys();
        }

        public OpportunityDto GetOpportunityByQuoteId(string quoteId)
        {
            return new UpdateCRMDataService().GetOpportunityByQuoteId(quoteId);
        }

        public Response SaveDocumentInformation(int clientId, int userId, string quoteId, string documentType, string filePlatformId, string sharedSavedLocation)
        {
            return new UpdateCRMDataService().SaveDocumentInformation(clientId, userId, quoteId, documentType, filePlatformId, sharedSavedLocation);
        }
        //end new methods

        //SDA methods
        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserCloudStatusDto GetUserAppStatus(int clientId, int userId) 
        {
            return new UpdateCRMDataService().GetUserAppStatus(clientId, userId);
        }

        /// <summary>
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="UserId"></param>
        /// <param name="WindowsUserName"></param>
        /// <param name="MacAddress"></param>
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
        public string UploadUserMachineData(int ClientId, int UserId, string WindowsUserName, string MacAddress, string VersionDotNet, string VersionExcel, string VersionWord, string VersionSDA, string VersionSalesManager,
                                   string VersionWindows, string InstallType, string UserFullName, string Email, string CompanyLong, string Title, string Phone, string UserTimeZone) 
        {
            return new UpdateCRMDataService().UploadUserMachineData(ClientId, UserId, WindowsUserName, MacAddress, VersionDotNet, VersionExcel, VersionWord, VersionSDA, VersionSalesManager, 
                                             VersionWindows, InstallType, UserFullName, Email, CompanyLong, Title, Phone, UserTimeZone);
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserStatus(int clientId, int userId) 
        {
            return new UpdateCRMDataService().GetUserStatus(clientId, userId);
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<ClientImportOptionDto> GetClientImportOptionListByClientId(int clientId) 
        {
            return new UpdateCRMDataService().GetClientImportOptionListByClientId(clientId);
        }

        public int InsertClientImportOption(int ClientId, string ImportOption, string Status) 
        {
            return new UpdateCRMDataService().InsertClientImportOption(ClientId, ImportOption, Status);
        }

        //end SDA methods
    }
}
