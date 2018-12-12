using Corspro.Data.External;
using Corspro.Domain;
using Corspro.Domain.Dto;
using System;
using System.Collections.Generic;


namespace Corspro.Business.External
{
    public class OpportunityBL
    {
        private readonly ApplicationLogDL _applicationLogDL = new ApplicationLogDL();
        private readonly UtilityBL _utilityBl = new UtilityBL();

        public int AddOpportunity(OpportunityDto opportunityDto)
        {
            bool manageOpportunitiesInCRM = false;
            ClientLoginDto dto = new ClientLoginDL().ManageOpportunitiesInCRM(opportunityDto.ClientID);
            if (dto.ManageOppysInCRM)
            {
                manageOpportunitiesInCRM = dto.ManageOppysInCRM;
            }
            return new OpportunityDL().AddOpportunity(opportunityDto, manageOpportunitiesInCRM);
        }

        public Response AddSdaCloudOpportunity(List<OpportunityDto> opportunities)
        {
            Response response = new Response();
            OpportunityDL ydl = new OpportunityDL();
            ClientLoginDL ndl = new ClientLoginDL();
            foreach (OpportunityDto dto in opportunities)
            {
                string str;
                try
                {
                    bool manageOpportunitiesInCRM = false;
                    ClientLoginDto dto2 = ndl.ManageOpportunitiesInCRM(dto.ClientID);
                    if (dto2.ManageOppysInCRM)
                    {
                        manageOpportunitiesInCRM = dto2.ManageOppysInCRM;
                    }
                    int oppID = ydl.AddOpportunity(dto, manageOpportunitiesInCRM);
                    if (oppID < 0)
                    {
                        str = "ADD-SDACloud Duplicated record with Quote Id Main Site:" + dto.QuoteIDMainSite + ", could not be added";
                        if (this._applicationLogDL.AddErrorMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto)))
                        {
                            response.Results.Add(str);
                        }
                    }
                    else if (oppID <= 0)
                    {
                        str = "ADD-SDACloud CRM Opportunity Id:" + dto.CRMOppID + ", could not be added";
                        if (this._applicationLogDL.AddErrorMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto)))
                        {
                            response.Results.Add(str);
                        }
                        else
                        {
                            response.Errors.Add(str);
                        }
                    }
                    else if (!manageOpportunitiesInCRM && string.IsNullOrEmpty(dto.CRMOppID))
                    {
                        string[] strArray = new string[] { "ADD-SDACloud CRM Opportunity Id:", ydl.GetNonDeletedOpportunityByClientIDAndOppID(dto.ClientID, oppID).CRMOppID, ":Quote Id Main Site:", dto.QuoteIDMainSite, ", has been successfully added" };
                        str = string.Concat(strArray);
                        response.Results.Add(str);
                        this._applicationLogDL.AddTransactionMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto));
                    }
                    else if (!string.IsNullOrEmpty(dto.CRMOppID))
                    {
                        str = "ADD-SDACloud CRM Opportunity Id:" + dto.CRMOppID + ", has been successfully added";
                        response.Results.Add(str);
                        this._applicationLogDL.AddTransactionMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto));
                    }
                    else
                    {
                        str = "ADD-SDACloud Opportunity with Quote Id Main Site:" + dto.QuoteIDMainSite + ", has been successfully added";
                        response.Results.Add(str);
                        this._applicationLogDL.AddTransactionMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto));
                    }
                }
                catch (Exception exception)
                {
                    string[] strArray2 = new string[] { "SDACloud CRM Opportunity Id:", dto.CRMOppID, ":Quote Id Main Site:", dto.QuoteIDMainSite, ",", exception.Message, (exception.InnerException != null) ? exception.InnerException.Message : "" };
                    str = string.Concat(strArray2);
                    if (this._applicationLogDL.AddErrorMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto)))
                    {
                        response.Results.Add(str);
                        continue;
                    }
                    response.Errors.Add(str);
                }
            }
            return response;
        }

        public int DeleteOpportunity(int opportunityId)
        {
            return new OpportunityDL().DeleteOpportunity(opportunityId);
        }

        public int GetDefaultProbabilityByStatus(int clientID, int status)
        {
            return new OpportunityDL().GetDefaultProbabilityByStatus(clientID, status);
        }

        public List<OpportunityDto> GetNonClosedOpportunities(int clientId)
        {
            return new OpportunityDL().GetNonClosedOpportunities(clientId);
        }

        public OpportunityDto GetNonDeletedOpportunityByClientIDAndCRMOppID(int clientID, string crmOppID)
        {
            return new OpportunityDL().GetNonDeletedOpportunityByClientIDAndCRMOppID(clientID, crmOppID);
        }

        public OpportunityDto GetNonDeletedOpportunityByClientIDAndOppID(int clientID, int oppID)
        {
            return new OpportunityDL().GetNonDeletedOpportunityByClientIDAndOppID(clientID, oppID);
        }

        public OpportunityDto GetNonDeletedOpportunityByClientIDAndQuoteID(int clientID, string quoteID)
        {
            return new OpportunityDL().GetNonDeletedOpportunityByClientIDAndQuoteID(clientID, quoteID);
        }

        public List<OpportunityDto> GetOpportunities(int clientId)
        {
            return new OpportunityDL().GetOpportunities(clientId);
        }

        public List<OpportunityDto> GetOpportunities(int clientID, string oppId, string opportunityName, string companyName, string owner, string opportunityStatus)
        {
            return new OpportunityDL().GetOpportunities(clientID, oppId, opportunityName, companyName, owner, opportunityStatus);
        }

        public List<OppGridDto> GetOpportunitiesfromView(int clientId, int userId, DateTime initialDate, DateTime finalDate, string stages, List<int> liUsers)
        {
            return new OpportunityDL().GetOpportunitiesfromView(clientId, userId, initialDate, finalDate, stages, liUsers);
        }

        public OpportunityDto GetOpportunityByClientIDAndCRMOppID(int clientID, string crmOppID)
        {
            return new OpportunityDL().GetOpportunityByClientIDAndCRMOppID(clientID, crmOppID);
        }

        public OpportunityDto GetOpportunityByQuoteId(string QuoteId)
        {
            return new OpportunityDL().GetOpportunityByQuoteId(QuoteId);
        }

        public int UpdateOpportunityById(OpportunityDto opportunityDto)
        {
            return new OpportunityDL().UpdateOpportunityById(opportunityDto);
        }

        public Response UpdateSdaCloudOpportunity(List<OpportunityDto> opportunities)
        {
            Response response = new Response();
            OpportunityDL ydl = new OpportunityDL();
            foreach (OpportunityDto dto in opportunities)
            {
                string str;
                try
                {
                    if (ydl.UpdateOpportunity(dto) > 0)
                    {
                        str = "UPDATE-SDACloud CRM Opportunity Id:" + dto.CRMOppID + ", has been successfully updated";
                        response.Results.Add(str);
                        this._applicationLogDL.AddTransactionMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto));
                        continue;
                    }
                    str = "UPDATE-SDACloud CRM Opportunity Id:" + dto.CRMOppID + ", could not be updated";
                    if (this._applicationLogDL.AddErrorMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto)))
                    {
                        response.Results.Add(str);
                        continue;
                    }
                    response.Errors.Add(str);
                }
                catch (Exception exception)
                {
                    string[] strArray = new string[] { "SDACloud CRM Opportunity Id:", dto.CRMOppID, ",", exception.Message, (exception.InnerException != null) ? exception.InnerException.Message : "" };
                    str = string.Concat(strArray);
                    if (this._applicationLogDL.AddErrorMessage(dto.ClientID, str, this._utilityBl.ConvertRecordToString(dto)))
                    {
                        response.Results.Add(str);
                        continue;
                    }
                    response.Errors.Add(str);
                }
            }
            return response;
        }
    }
}

