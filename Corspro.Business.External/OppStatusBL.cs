using System;
using System.Collections.Generic;
using Corspro.Data.External;
using Corspro.Domain;
using Corspro.Domain.Dto;

namespace Corspro.Business.External
{
    public class OppStatusBL
    {
        public List<OppStatusDto> GetOppStatuses(int clientID)
        {
            var oppStatusDl = new OppStatusDL();
            return oppStatusDl.GetOppStatuses(clientID);
        }

        public int AddOppStatus(OppStatusDto oppStatusDto)
        {
            var oppStatusDl = new OppStatusDL();
            return oppStatusDl.AddOppStatus(oppStatusDto);
        }

        public bool ValidateOrder(int clientID, int oppStatusId, int newOrder)
        {
            var oppStatusDl = new OppStatusDL();
            return oppStatusDl.ValidateOrder(clientID, oppStatusId, newOrder);
        }

        public string DeleteOppStatuses(string uList)
        {
            var oppStatusDl = new OppStatusDL();
            return oppStatusDl.DeleteOppStatuses(uList);
        }

        public int UpdateOppStatusField(int cdfId, string field, string newValue)
        {
            var oppStatusDl = new OppStatusDL();
            return oppStatusDl.UpdateOppStatusField(cdfId, field, newValue);
        }

        public int UpdateOppStatusDefault(int clientId, int cdfId)
        {
            var oppStatusDl = new OppStatusDL();
            return oppStatusDl.UpdateOppStatusDefault(clientId, cdfId);
        }
    }
}
