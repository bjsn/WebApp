using Corspro.Domain;
using Corspro.Domain.Dto;
using System.Collections.Generic;

namespace Corspro.Services
{
    /// <summary>
    /// IUpdateCRMDataService is an interface that defines the different functionalities of the application
    /// </summary>
    public interface IUpdateCRMDataService
    {
        /// <summary>
        /// Authenticates the specified p client login dto.
        /// </summary>
        /// <param name="pClientLoginDto">The p client login dto.</param>
        /// <returns></returns>
        Response Authenticate(ClientLoginDto pClientLoginDto);

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="cloudCRMXrefUpdDT"></param>
        /// <returns></returns>
        Response GetCRMXRefUpdDT(int clientId, string cloudCRMXrefUpdDT);

        /// <summary>
        /// Runs the update CRM opportunities.
        /// </summary>
        /// <param name="opportunityDto">The opportunity dto.</param>
        /// <returns></returns>
        Response RunUpdateCRMOpportunities(OpportunityDto opportunityDto);

        /// <summary>
        /// Runs the update CRM quotes.
        /// </summary>
        /// <param name="quoteDto">The quote dto.</param>
        /// <returns></returns>
        Response RunUpdateCRMQuotes(QuoteDto quoteDto);

        /// <summary>
        /// Gets the user info by LoginID
        /// </summary>
        /// <param name="clientLogin">The user object</param>
        /// <returns></returns>
        Response GetUserByLoginIDAndPassword(UserDto clientLogin);

        /// <summary>
        /// Gets the user info by UserID and ClientID
        /// </summary>
        /// <param name="clientLogin">The user object</param>
        /// <returns></returns>
        Response GetUserStatusByUserIDAndClientID(UserDto clientLogin);

        /// <summary>
        /// Get quote info by client id and quote id
        /// </summary>
        /// <param name="clientID">The client id</param>
        /// <param name="quoteID">The quote id</param>
        /// <returns></returns>
        Response GetQuoteByClientIDAndQuoteID(int clientID, string quoteID);

        /// <summary>
        /// Get an opportunity by CRMOppID
        /// </summary>
        /// <param name="clientID">The ClientID</param>
        /// <param name="value">The value.</param>
        /// <param name="searchType">Type of the search.</param>
        /// <returns></returns>
        Response GetOpportunityByClientIDAndSearchType(int clientID, string value, string searchType);

        /// <summary>
        /// Get the opportunities for client
        /// </summary>
        /// <param name="clientID">The Client ID</param>
        /// <param name="opportunityName">The opportunity name</param>
        /// <param name="companyName">The company name</param>
        /// <param name="owner">The owner</param>
        /// <param name="opportunityStatus">The opportunity status</param>
        /// <returns></returns>
        Response GetOpportunities(int clientID, string oppId, string opportunityName, string companyName, string owner, string opportunityStatus);

        /// <summary>
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        string GetClientIdByName(string ClientName);

        /// <summary>
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        ClientDto GetClientById(int ClientId);

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        List<ClientUpdateDBDto> GetClientUpdateDB(int clientId, int userType);

        /// <summary>
        /// </summary>
        /// <param name="configurationListName"></param>
        /// <returns></returns>
        List<ConfigurationDto> GetConfigurationList(string configurationListName);

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        bool HasPermissionToUpload(int clientId, string clientName);

        /// <summary>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="clientID"></param>
        void UpdateUserRegistrationDT(int userID, int clientID);

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        ClientUpdateDBDto GetLastCloudDBFileUpdDT(int clientId, string dbName);

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="quoteID"></param>
        /// <returns></returns>
        Response GetQuoteByClientIdUserIdAndQuoteId(int clientId, int userId, string quoteID);

        //new methods

        /// <summary>
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        List<string> GetInterfaceByIdAndClientId(string userId, string clientId);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<string[]> GetLastRetrievedKeys();

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="QuoteId"></param>
        /// <param name="DocumentType"></param>
        /// <param name="FilePlatformId"></param>
        /// <param name="SharedSavedLocation"></param>
        /// <returns></returns>
        Response SaveDocumentInformation(int clientId, int userId, string QuoteId, string DocumentType, string FilePlatformId, string SharedSavedLocation);

        /// <summary>
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        OpportunityDto GetOpportunityByQuoteId(string quoteId);


        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserCloudStatusDto GetUserAppStatus(int clientId, int userId);

        /// <summary>
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="UserId"></param>
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
        string UploadUserMachineData(int ClientId, int UserId, string WindowsUserName, string MacAddress, string VersionDotNet, string VersionExcel, string VersionWord, string VersionSDA, string VersionSalesManager,
                                    string VersionWindows, string InstallType, string UserFullName, string Email, string CompanyLong, string Title, string Phone, string UserTimeZone);

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUserStatus(int clientId, int userId);
        
        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        List<ClientImportOptionDto> GetClientImportOptionListByClientId(int clientId);
    }
}
