using System.Linq;
using Corspro.Domain.External;
using Corspro.Domain.Dto;
using System;

namespace Corspro.Data.External
{
    public class ClientLoginDL
    {
        /// <summary>
        /// Validates the client login.
        /// </summary>
        /// <param name="clientLogin">The client login.</param>
        /// <returns></returns>
        public bool ValidateClientLogin(ClientLoginDto clientLogin)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingClient =
                        sdaCloudEntities.Clients.FirstOrDefault(
                            i =>
                            i.ClientLoginID == clientLogin.ClientLoginId &&
                            i.ClientLoginPwd == clientLogin.ClientLoginPwd);

                    return existingClient != null;
                }
            }
        }

        /// <summary>
        /// Manages the opportunities in CRM.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <returns></returns>
        public ClientLoginDto ManageOpportunitiesInCRM(int clientID)
        {
            var client = new ClientLoginDto();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingClient =
                        sdaCloudEntities.Clients.FirstOrDefault(
                            i => i.ClientID == clientID);

                    if (existingClient == null)
                    {
                        return client;
                    }
                    else
                    {
                        client.CRMData = existingClient.CRMData;
                        client.ManageOppysInCRM = (existingClient.ManageOppysInCRM == "Y");
                    }
                }
            }
            return client;
        }

        /// <summary>
        /// Gets the CRM xref upd dt.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <returns></returns>
        public DateTime GetCRMXrefUpdDT(int clientID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var crmXrefUpdDT =
                        sdaCloudEntities.Clients.FirstOrDefault(
                            i => i.ClientID == clientID);

                    if (crmXrefUpdDT != null && crmXrefUpdDT.CRMXrefUpdDT.HasValue)
                        return crmXrefUpdDT.CRMXrefUpdDT.Value;

                    return DateTime.MinValue;
                }
            }
        }


        public string GetCRMXrefCRMSystem(int clientID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var crmXrefUpdDT =
                        sdaCloudEntities.Clients.FirstOrDefault(
                            i => i.ClientID == clientID);

                    return crmXrefUpdDT.CRMSystem;
                }
            }
        }

        public ClientLoginDto GetClientSyncValues(int clientID)
        {
            ClientLoginDto client = new ClientLoginDto();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var crmXrefUpdDT =
                        sdaCloudEntities.Clients.FirstOrDefault(
                            i => i.ClientID == clientID);

                    //verify this
                    /*client.CorsProSyncInd     = crmXrefUpdDT.CorsProSyncInd;
                    client.SyncServerLocation = crmXrefUpdDT.SyncServerLocation;
                    client.SyncServerLogin    = crmXrefUpdDT.SyncServerLogin;
                    client.SyncServerPwd      = crmXrefUpdDT.SyncServerPwd;
                    client.OpportunityMgmt    = (crmXrefUpdDT.OpportunityMgmt);
                    client.LastSDAVer         = crmXrefUpdDT.LastSDAVer;
                    client.LastSDAVerWithDBUpdates = crmXrefUpdDT.LastSDAVerWithDBUpdates;*/
                    return client;
                }
            }
        }
    }
}
