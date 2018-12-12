using Corspro.Data.External;
using Corspro.Domain.Dto;
using System;

namespace Corspro.Business.External
{
    public class ClientLoginBL
    {
        /// <summary>
        /// Validates the client login.
        /// </summary>
        /// <param name="clientLogin">The client login.</param>
        /// <returns></returns>
        public bool ValidateClientLogin(ClientLoginDto clientLogin)
        {
            var clientLoginDl = new ClientLoginDL();
            return clientLoginDl.ValidateClientLogin(clientLogin);
        }

        /// <summary>
        /// Manages the opportunities in CRM.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <returns></returns>
        public ClientLoginDto ManageOpportunitiesInCRM(int clientID)
        {
            var clientLoginDl = new ClientLoginDL();
            return clientLoginDl.ManageOpportunitiesInCRM(clientID);
        }

        /// <summary>
        /// Gets the CRM xref upd dt.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <returns></returns>
        public DateTime GetCRMXrefUpdDT(int clientID)
        {
            var clientLoginDl = new ClientLoginDL();
            return clientLoginDl.GetCRMXrefUpdDT(clientID);
        }

        public string GetCRMXrefCRMSystem(int clientID)
        {
            var clientLoginDl = new ClientLoginDL();
            return clientLoginDl.GetCRMXrefCRMSystem(clientID);
        }

        public ClientLoginDto GetClientSyncValues(int clientID)
        {
            var clientLoginDl = new ClientLoginDL();
            return clientLoginDl.GetClientSyncValues(clientID);
        }
    }
}
