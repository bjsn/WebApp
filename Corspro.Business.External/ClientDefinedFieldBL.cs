using System;
using System.Collections.Generic;
using Corspro.Data.External;
using Corspro.Domain;
using Corspro.Domain.Dto;

namespace Corspro.Business.External
{
    public class ClientDefinedFieldBL
    {

        /// <summary>
        /// Gets the client defined fields.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public List<ClientDefinedFieldDto> GetClientDefinedFields(int clientID, string table)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.GetClientDefinedFields(clientID, table);
        }

        /// <summary>
        /// Gets the client defined fields.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <returns></returns>
        public List<ClientDefinedFieldDto> GetClientDefinedFields(int clientID)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.GetClientDefinedFields(clientID);
        }

        public List<ClientDefinedFieldDto> GetClientDefinedFields(int clientID, string table, string field)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.GetClientDefinedFields(clientID, table, field);
        }

        public bool ValidateFieldToUpdate(int clientID, int cdfId, string field, string newValue)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.ValidateFieldToUpdate(clientID, cdfId, field, newValue);
        }

        public int UpdateCDFField(int userId, string field, string newValue)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.UpdateCDFField(userId, field, newValue);
        }

        public int AddClientDefinedField(ClientDefinedFieldDto clientDefinedField)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.AddClientDefinedField(clientDefinedField);
        }

        public string DeleteCDFs(string uList)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.DeleteCDFs(uList);
        }

        public ClientDto GetClientIdByName(string clientName)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.GetClientIdByName(clientName);
        }

        public ClientDto GetClientById(int clientId)
        {
            var clientDefinedFieldDl = new ClientDefinedFieldDL();
            return clientDefinedFieldDl.GetClientById(clientId);
        }
    }
}
