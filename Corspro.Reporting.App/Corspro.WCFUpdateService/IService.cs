using System.ServiceModel;
using Corspro.Domain;
using Corspro.Domain.Dto;
using System.Collections.Generic;

namespace Corspro.WCFUpdateService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
    [ServiceContract]
    [ServiceKnownType(typeof(OpportunityDto))]
    [ServiceKnownType(typeof(ClientLoginDto))]
    [ServiceKnownType(typeof(UserDto))]
    [ServiceKnownType(typeof(QuoteDto))]
    [ServiceKnownType(typeof(Response))]
    public interface IService
    {
        [OperationContract]
        Response RunUpdateCRMOpportunities(OpportunityDto opportunitiesToUpdate);

        [OperationContract]
        Response RunUpdateCRMQuotes(QuoteDto quotesToUpdate);

        [OperationContract]
        Response GetCRMXRefUpdDT(int clientId, string cloudCRMXrefUpdDT);

        [OperationContract]
        List<InterfaceXRefDto> RunUpdateCRMXRef(InterfaceXRefDto interfaceXRefDto);

        [OperationContract]
        Response GetUserByLoginIDAndPassword(string loginID, string password);

        [OperationContract]
        Response GetUserStatusByUserIDAndClientID(string userID, int clientID);

        [OperationContract]
        Response GetQuoteByClientIDAndQuoteID(int clientID, string quoteID);

        [OperationContract]
        Response GetQuoteByClientIdUserIdAndQuoteId(int clientId, int userId, string quoteID);

        [OperationContract]
        Response GetOpportunityByClientIDAndSearchType(int clientID, string value, string searchType);

        [OperationContract]
        Response GetOpportunities(int clientID, string oppId, string opportunityName, string companyName, string owner, string opportunityStatus);

        [OperationContract]
        string GetCLientIdByName(string ClientName);

        [OperationContract]
        ClientDto GetClientById(int ClientId);

        [OperationContract]
        string GetClientNameByIdAndUserId(int clientId, int userId);

        [OperationContract]
        List<ClientUpdateDBDto> GetClientUpdateDB(int clientId, int userType);

        [OperationContract]
        List<ConfigurationDto> GetConfigurationList(string configurationListName);

        [OperationContract]
        bool HasPermissionToUpload(int clientId, string fileName);

        [OperationContract]
        ClientUpdateDBDto UpdateClientUpdateDB(int clientId, string fileName, string AWSid, string AWSFilePath, string AWSFileName, string DBFileUpdDT, string DBUploadedDt, bool BetaVersion, int UploaderClientID, int UploaderUserID, string UploaderUserName);

        [OperationContract]
        void UpdateUserRegistrationDT(int userID, int clientID);

        [OperationContract]
        void UpdateUserLastCheckDT(int userID, int clientID);

        [OperationContract]
        ClientUpdateDBDto GetLastCloudDBFileUpdDT(int clientId, string dbName);

        //new methods
        [OperationContract]
        List<string> GetInterfaceByIdAndClientId(int userId, int clientId);

        [OperationContract]
        List<string[]> GetLastRetrievedKeys();

        [OperationContract]
        OpportunityDto GetOpportunityByQuoteId(string quoteId);

        [OperationContract]
        Response SaveDocumentInformation(int clientId, int userId, string quoteId, string documentType, string filePlatformId, string sharedSavedLocation);
        //end new methods

        [OperationContract]
        UserCloudStatusDto GetUserAppStatus(int clientId, int userId);

        [OperationContract]
        string UploadUserMachineData(int ClientId, int UserId, string WindowsUserName, string MacAddress, string VersionDotNet, string VersionExcel, string VersionWord, string VersionSDA, string VersionSalesManager,
                                   string VersionWindows, string InstallType, string UserFullName, string Email, string CompanyLong, string Title, string Phone, string UserTimeZone); 
    }
}