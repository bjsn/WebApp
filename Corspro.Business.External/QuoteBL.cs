using System;
using System.Collections.Generic;
using System.Linq;
using Corspro.Data.External;
using Corspro.Domain;
using Corspro.Domain.Dto;

namespace Corspro.Business.External
{
    public class QuoteBL
    {
        readonly ApplicationLogDL _applicationLogDL = new ApplicationLogDL();
        readonly UtilityBL _utilityBl = new UtilityBL();

        /// <summary>
        /// Gets the quotes.
        /// </summary>
        /// <param name="opportunityId">The opportunity identifier.</param>
        /// <returns></returns>
        public List<QuoteDto> GetQuotes(int clientId, int opportunityId)
        {
            var quoteDl = new QuoteDL();
            return quoteDl.GetQuotes(clientId, opportunityId);
        }

        /// <summary>
        /// Gets the quote by client identifier and quote identifier.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="quoteID">The quote identifier.</param>
        /// <returns></returns>
        public QuoteDto GetQuoteByClientIDAndQuoteID(int clientID, string quoteID)
        {
            var quoteDl = new QuoteDL();
            return quoteDl.GetQuoteByClientIDAndQuoteID(clientID, quoteID);
        }

        public QuoteDto GetQuoteByLastFileSavedLocation(string LastFileSavedLocation)
        {
            var quoteDl = new QuoteDL();
            return quoteDl.GetQuoteByLastFileSavedLocation(LastFileSavedLocation);
        }

        /// <summary>
        /// Updates the quote rollup.
        /// </summary>
        /// <param name="opportunityID">The opportunity identifier.</param>
        /// <param name="quoteID">The quote identifier.</param>
        /// <param name="rollup">The rollup.</param>
        /// <returns></returns>
        public int UpdateQuoteRollup(int opportunityID, string quoteID, string rollup)
        {
            var quoteDl = new QuoteDL();
            int result = quoteDl.UpdateQuoteRollup(quoteID, rollup);

            try
            {
                var opportunityDl = new OpportunityDL();
                result += opportunityDl.CalculateOpportunityRollups(opportunityID);
            }
            catch (Exception e)
            {
                _applicationLogDL.AddErrorMessage(0, "Calculate rollup returned error", e.Message);
            }

            return result;
        }

        /// <summary>
        /// Deletes the quotes.
        /// </summary>
        /// <param name="quoteIdList">The quote identifier list.</param>
        /// <returns></returns>
        public int DeleteQuotes(string quoteIdList)
        {
            List<int?> opportunityIdList = new List<int?>();

            var quoteDl = new QuoteDL();
            int result = quoteDl.DeleteQuotes(quoteIdList, ref opportunityIdList);

            var opportunityDl = new OpportunityDL();
            foreach (int opportunityID in opportunityIdList)
            {
                try
                {
                    result += opportunityDl.CalculateOpportunityRollups(opportunityID);
                }
                catch (Exception e)
                {
                    _applicationLogDL.AddErrorMessage(0, "Calculate rollup returned error for oppId:" + opportunityID, e.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Reassigns the quotes.
        /// </summary>
        /// <param name="quoteIdList">The quote identifier list.</param>
        /// <param name="newOpportunityID">The new opportunity identifier.</param>
        /// <param name="newCRMOppID">The new CRM opp identifier.</param>
        /// <returns></returns>
        public int ReassignQuotes(int clientId, string quoteIdList, int newOpportunityID, string newCRMOppID)
        {
            List<int?> opportunityIdList = new List<int?>();

            var quoteDl = new QuoteDL();
            int result = quoteDl.ReassignQuotes(clientId, quoteIdList, newOpportunityID, newCRMOppID, ref opportunityIdList);

            var opportunityDl = new OpportunityDL();
            foreach (int opportunityID in opportunityIdList)
            {
                try
                {
                    result += opportunityDl.CalculateOpportunityRollups(opportunityID);
                }
                catch (Exception e)
                {
                    _applicationLogDL.AddErrorMessage(0, "Calculate rollup returned error for oppId:" + opportunityID, e.Message);
                }
            }
            try
            {
                result += opportunityDl.CalculateOpportunityRollups(newOpportunityID);
            }
            catch (Exception e)
            {
                _applicationLogDL.AddErrorMessage(0, "Calculate rollup returned error for oppId:" + newOpportunityID, e.Message);
            }

            return result;
        }

        /// <summary>
        /// Add a list of quotes
        /// </summary>
        /// <param name="quotes">The quotes.</param>
        /// <returns></returns>
        public Response AddSdaCloudQuote(List<QuoteDto> quotes)
        {
            var response = new Response();
            // Initialise the Salesforce web service
            var quoteDl = new QuoteDL();

            foreach (var quoteDto in quotes)
            {
                string message;
                try
                {
                    var result = quoteDl.AddQuote(quoteDto);
                    if (result != string.Empty)
                    {
                        message = "ADD-SDACloud Quote Id:" + quoteDto.QuoteID + ", has been successfully added";
                        response.Results.Add(message);
                        _applicationLogDL.AddTransactionMessage(quoteDto.ClientID, message, _utilityBl.ConvertRecordToString(quoteDto));
                    }
                    else
                    {
                        message = "ADD-SDACloud Quote Id:" + quoteDto.QuoteID + ", could not be added";
                        var nls = _applicationLogDL.AddErrorMessage(quoteDto.ClientID, message, _utilityBl.ConvertRecordToString(quoteDto));
                        if (nls)
                        {
                            response.Results.Add(message);
                        }
                        else
                        {
                            response.Errors.Add(message);
                        }
                    }
                }
                catch (Exception exception)
                {
                    String innerMessage = (exception.InnerException != null)
                          ? exception.InnerException.Message
                          : "";
                    message = "SDACloud Quote Id:" + quoteDto.QuoteID + "," + exception.Message + innerMessage;
                    var nls = _applicationLogDL.AddErrorMessage(quoteDto.ClientID, message, _utilityBl.ConvertRecordToString(quoteDto));
                    if (nls)
                    {
                        response.Results.Add(message);
                    }
                    else
                    {
                        response.Errors.Add(message);
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Updates the sda cloud quote.
        /// </summary>
        /// <param name="quotes">The quotes.</param>
        /// <returns></returns>
        public Response UpdateSdaCloudQuote(List<QuoteDto> quotes)
        {
            var response = new Response();

            // Initialise the Salesforce web service
            var quoteDl = new QuoteDL();

            foreach (var quoteDto in quotes)
            {
                string message;
                try
                {
                    var result = quoteDl.UpdateQuote(quoteDto);
                    if (result > 0)
                    {
                        message = "UPDATE-SDACloud Quote Id:" + quoteDto.QuoteID + ", has been successfully updated";
                        response.Results.Add(message);
                        _applicationLogDL.AddTransactionMessage(quoteDto.ClientID, message, _utilityBl.ConvertRecordToString(quoteDto));
                    }
                    else
                    {
                        message = "UPDATE-SDACloud Quote Id:" + quoteDto.QuoteID + ", could not be updated";
                        var nls = _applicationLogDL.AddErrorMessage(quoteDto.ClientID, message, _utilityBl.ConvertRecordToString(quoteDto));
                        if (nls)
                        {
                            response.Results.Add(message);
                        }
                        else
                        {
                            response.Errors.Add(message);
                        }
                    }
                }
                catch (OutdatedQuoteException exception)
                {
                    message = exception.Message;
                    response.Results.Add(message);
                    _applicationLogDL.AddErrorMessage(quoteDto.ClientID, message, _utilityBl.ConvertRecordToString(quoteDto));
                }
                catch (Exception exception)
                {
                    String innerMessage = (exception.InnerException != null)
                          ? exception.InnerException.Message
                          : "";
                    message = "SDACloud Quote Id:" + quoteDto.QuoteID + "," + exception.Message + innerMessage;
                    var nls = _applicationLogDL.AddErrorMessage(quoteDto.ClientID, message, _utilityBl.ConvertRecordToString(quoteDto));
                    if (nls)
                    {
                        response.Results.Add(message);
                    }
                    else
                    {
                        response.Errors.Add(message);
                    }
                }
            }

            return response;
        }
    }
}
