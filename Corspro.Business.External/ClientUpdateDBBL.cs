using Corspro.Data.External;
using Corspro.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corspro.Business.External
{
    public class ClientUpdateDBBL
    {
        public List<ClientUpdateDBDto> GetClientUpdateDB(int clientId, int userType)
        {
            var clientUpdateDl = new ClientUpdateDBDL();
            return clientUpdateDl.GetClientUpdateDB(clientId, userType);
        }

        public bool HasPermissionToUpload(int clientId, string fileName)
        {
            var clientUpdateDl = new ClientUpdateDBDL();
            return clientUpdateDl.HasPermissionToUpload(clientId, fileName);
        }

        public ClientUpdateDBDto UpdateClientUpdateDB(int clientId, string fileName, string AWSid, string AWSFilePath, string AWSFileName, string DBFileUpdDT, string DBUploadedDt, bool BetaVersion, 
            int UploaderClientID, int UploaderUserID, string UploaderUserName)
        {
            var clientUpdateBL = new ClientUpdateDBDL();
            return clientUpdateBL.UpdateClientUpdateDB(clientId, fileName, AWSid, AWSFilePath, AWSFileName, DBFileUpdDT, DBUploadedDt, BetaVersion, UploaderClientID, UploaderUserID, UploaderUserName);
        }

        public string GetClientNameByIdAndUserId(int clientId, int userId)
        {
            var clientUpdateBL = new ClientUpdateDBDL();
            return clientUpdateBL.GetClientNameByIdAndUserId(clientId, userId);
        }

        public ClientUpdateDBDto GetLastCloudDBFileUpdDT(int clientId, string dbName)
        {
           var clientUpdateBL = new ClientUpdateDBDL();
            return clientUpdateBL.GetLastCloudDBFileUpdDT(clientId, dbName);
        }
    }
}
