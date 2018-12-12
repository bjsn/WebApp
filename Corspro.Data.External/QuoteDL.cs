using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using AutoMapper;
using Corspro.Domain;
using Corspro.Domain.External;
using Corspro.Domain.Dto;

namespace Corspro.Data.External
{
    public class QuoteDL
    {
        readonly OpportunityDL _opportunityDl = new OpportunityDL();
        readonly ApplicationLogDL _applicationLogDL = new ApplicationLogDL();

        /// <summary>
        /// Gets the quotes.
        /// </summary>
        /// <param name="opportunityId">The opportunity identifier.</param>
        /// <returns></returns>
        public List<QuoteDto> GetQuotes(int clientId, int opportunityId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingQuotes = sdaCloudEntities.Quotes.Where(q => q.ClientID == clientId && q.OppID == opportunityId && q.DeleteInd == "N").ToList();

                    Mapper.CreateMap<Quote, QuoteDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.QuoteTable, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.QuoteIDMainSite, opt => opt.Ignore())
                        .ForMember(dest => dest.SDAUpdatedByName, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>());

                    List<QuoteDto> mappedQuotes = new List<QuoteDto>();
                    if (existingQuotes.Count > 0)
                    {
                        mappedQuotes = Mapper.Map<List<Quote>, List<QuoteDto>>(existingQuotes);

                        foreach (var quoteDto in mappedQuotes)
                        {
                            int sdaUpdatedBy = quoteDto.SDALastUpdBy;
                            User updatedUser = sdaCloudEntities.Users.FirstOrDefault(u => u.UserID == sdaUpdatedBy);

                            if (updatedUser != null)
                                quoteDto.SDAUpdatedByName = updatedUser.FirstName + " " + updatedUser.LastName;
                        }
                        //Mapper.AssertConfigurationIsValid();
                    }
                    return mappedQuotes;
                }
            }
        }

        /// <summary>
        /// Gets the quote by client identifier and quote identifier.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="quoteID">The quote identifier.</param>
        /// <returns></returns>
        public QuoteDto GetQuoteByClientIDAndQuoteID(int clientID, string quoteID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    // Get all the fields
                    Quote existingQuote = sdaCloudEntities.Quotes.FirstOrDefault(q => q.ClientID == clientID && q.QuoteID == quoteID);
                    Mapper.CreateMap<Quote, QuoteDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.QuoteTable, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.QuoteIDMainSite, opt => opt.Ignore())
                        .ForMember(dest => dest.SDAUpdatedByName, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>());

                    QuoteDto result = null;

                    if (existingQuote != null)
                    {
                        var peopleVm = Mapper.Map<Quote, QuoteDto>(existingQuote);

                        int sdaUpdatedBy = peopleVm.SDALastUpdBy;
                        User updatedUser = sdaCloudEntities.Users.FirstOrDefault(u => u.UserID == sdaUpdatedBy);

                        if (updatedUser != null)
                            peopleVm.SDAUpdatedByName = updatedUser.FirstName + " " + updatedUser.LastName;

                        result = peopleVm;

                        //Mapper.AssertConfigurationIsValid();
                    }
                    return result;
                }
            }
        }

        public QuoteDto GetQuoteByLastFileSavedLocation(string LastFileSavedLocation)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    // Get all the fields
                    Quote existingQuote = sdaCloudEntities.Quotes.FirstOrDefault(q => q.LastFileSavedLocation == LastFileSavedLocation);

                    Mapper.CreateMap<Quote, QuoteDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.QuoteTable, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.QuoteIDMainSite, opt => opt.Ignore())
                        .ForMember(dest => dest.SDAUpdatedByName, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.FilePlatformFileID, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>());
                    QuoteDto result = null;

                    if (existingQuote != null)
                    {
                        var peopleVm = Mapper.Map<Quote, QuoteDto>(existingQuote);
                        int sdaUpdatedBy = peopleVm.SDALastUpdBy;
                        User updatedUser = sdaCloudEntities.Users.FirstOrDefault(u => u.UserID == sdaUpdatedBy);
                        if (updatedUser != null)
                            peopleVm.SDAUpdatedByName = updatedUser.FirstName + " " + updatedUser.LastName;

                        result = peopleVm;
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Updates the quote rollup.
        /// </summary>
        /// <param name="quoteID">The quote identifier.</param>
        /// <param name="rollup">The rollup.</param>
        /// <returns></returns>
        public int UpdateQuoteRollup(string quoteID, string rollup)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingQuote = sdaCloudEntities.Quotes.FirstOrDefault(i => i.QuoteID == quoteID);
                        if (existingQuote != null)
                        {
                            existingQuote.Rollup = rollup;
                        }
                        result = sdaCloudEntities.SaveChanges();
                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Deletes the quotes.
        /// </summary>
        /// <param name="quoteIdList">The quote identifier list.</param>
        /// <param name="opportunityIdList">The opportunity identifier list.</param>
        /// <returns></returns>
        public int DeleteQuotes(string quoteIdList, ref List<int?> opportunityIdList)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<string> tagIds = quoteIdList.Split(',').ToList();
                List<Quote> quotes = sdaCloudEntities.Quotes.Where(q => tagIds.Contains(q.QuoteID)).ToList();

                foreach (Quote quote in quotes)
                {
                    opportunityIdList.Add(quote.OppID);
                    quote.DeleteInd = "Y";
                }

                result = sdaCloudEntities.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Reassigns the quotes.
        /// </summary>
        /// <param name="quoteIdList">The quote identifier list.</param>
        /// <param name="newOpportunityID">The new opportunity identifier.</param>
        /// <param name="newCRMOppID">The new CRM opp identifier.</param>
        /// <param name="opportunityIdList">The opportunity identifier list.</param>
        /// <returns></returns>
        public int ReassignQuotes(int clientId, string quoteIdList, int newOpportunityID, string newCRMOppID, ref List<int?> opportunityIdList)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<string> tagIds = quoteIdList.Split(',').ToList();

                List<Quote> quotes = sdaCloudEntities.Quotes.Where(q => tagIds.Contains(q.QuoteID) && q.ClientID == clientId).ToList();

                foreach (Quote quote in quotes)
                {
                    opportunityIdList.Add(quote.OppID);
                    quote.OppID = newOpportunityID;
                    quote.CRMOppID = newCRMOppID;
                }

                result = sdaCloudEntities.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Updates the opportunity.
        /// </summary>
        /// <param name="quoteDto">The quote dto.</param>
        /// <returns></returns>
        public int UpdateQuote(QuoteDto quoteDto)
        {
            int result;
            string quoteID = quoteDto.QuoteID.ToString(CultureInfo.InvariantCulture);
            List<int> opportunityIDList = new List<int>();

            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingQuote = sdaCloudEntities.Quotes.FirstOrDefault(i => i.ClientID == quoteDto.ClientID && i.QuoteID == quoteID);

                        if (existingQuote != null)
                        {
                            DateTime resultDate;
                            DateTime.TryParse(quoteDto.SDALastUpdDT, out resultDate);
                            if (resultDate > existingQuote.SDALastUpdDT)
                            {
                                // Add the opp to calculate rollups for the old value
                                opportunityIDList.Add(existingQuote.OppID);
                                //if (quoteDto.OppID>0)
                                //    existingQuote.OppID = quoteDto.OppID;
                                if (!string.IsNullOrEmpty(quoteDto.CRMOppID))
                                    existingQuote.CRMOppID = quoteDto.CRMOppID;
                                if (!string.IsNullOrEmpty(quoteDto.QuoteSiteDesc))
                                    existingQuote.QuoteSiteDesc = quoteDto.QuoteSiteDesc;
                                if (quoteDto.QuotedAmount != null)
                                    existingQuote.QuotedAmount = quoteDto.QuotedAmount ?? 0;
                                if (quoteDto.QuotedCost != null)
                                    existingQuote.QuotedCost = quoteDto.QuotedCost ?? 0;
                                if (quoteDto.QuoteMargin != null)
                                    existingQuote.QuoteMargin = quoteDto.QuoteMargin ?? 0;
                                if (!string.IsNullOrEmpty(quoteDto.Rollup))
                                    existingQuote.Rollup = quoteDto.Rollup;
                                existingQuote.FilePlatformFileID = quoteDto.FilePlatformFileID;
                                existingQuote.DeleteInd = "N";

                                if (!string.IsNullOrEmpty(quoteDto.LastFileSavedLocation))
                                    existingQuote.LastFileSavedLocation = quoteDto.LastFileSavedLocation;

                                existingQuote.SDALastUpdBy = quoteDto.SDALastUpdBy;
                                existingQuote.SDALastUpdDT = resultDate;

                                existingQuote.CloudLastUpdBy = quoteDto.CloudLastUpdById;
                                existingQuote.CloudLastUpdDT = DateTime.UtcNow;

                                existingQuote.CreateDT = DateTime.TryParse(quoteDto.CreateDT,
                                    out resultDate)
                                    ? resultDate
                                    : DateTime.MinValue;
                                existingQuote.CreateBy = quoteDto.CreateBy;

                                existingQuote.ClientDefinedText1 = quoteDto.ClientDefinedText1;
                                existingQuote.ClientDefinedText2 = quoteDto.ClientDefinedText2;
                                existingQuote.ClientDefinedText3 = quoteDto.ClientDefinedText3;
                                existingQuote.ClientDefinedText4 = quoteDto.ClientDefinedText4;
                                existingQuote.ClientDefinedText5 = quoteDto.ClientDefinedText5;


                                if (quoteDto.ClientDefinedTotal1 != null)
                                    existingQuote.ClientDefinedTotal1 = quoteDto.ClientDefinedTotal1 ?? 0;
                                if (quoteDto.ClientDefinedTotal2 != null)
                                    existingQuote.ClientDefinedTotal2 = quoteDto.ClientDefinedTotal2 ?? 0;
                                if (quoteDto.ClientDefinedTotal3 != null)
                                    existingQuote.ClientDefinedTotal3 = quoteDto.ClientDefinedTotal3 ?? 0;
                                if (quoteDto.ClientDefinedTotal4 != null)
                                    existingQuote.ClientDefinedTotal4 = quoteDto.ClientDefinedTotal4 ?? 0;
                                if (quoteDto.ClientDefinedTotal5 != null)
                                    existingQuote.ClientDefinedTotal5 = quoteDto.ClientDefinedTotal5 ?? 0;
                                if (quoteDto.ClientDefinedTotal6 != null)
                                    existingQuote.ClientDefinedTotal6 = quoteDto.ClientDefinedTotal6 ?? 0;
                                if (quoteDto.ClientDefinedTotal7 != null)
                                    existingQuote.ClientDefinedTotal7 = quoteDto.ClientDefinedTotal7 ?? 0;
                                if (quoteDto.ClientDefinedTotal8 != null)
                                    existingQuote.ClientDefinedTotal8 = quoteDto.ClientDefinedTotal8 ?? 0;
                                if (quoteDto.ClientDefinedTotal9 != null)
                                    existingQuote.ClientDefinedTotal9 = quoteDto.ClientDefinedTotal9 ?? 0;
                                if (quoteDto.ClientDefinedTotal10 != null)
                                    existingQuote.ClientDefinedTotal10 = quoteDto.ClientDefinedTotal10 ?? 0;

                                Opportunity existingOpportunity;

                                if (quoteDto.CRMOppID != null)
                                {
                                    quoteDto.CRMOppID = quoteDto.CRMOppID.Trim();
                                }

                                if (!string.IsNullOrEmpty(quoteDto.CRMOppID))
                                {

                                    existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(i => i.ClientID == quoteDto.ClientID && i.CRMOppID == quoteDto.CRMOppID);

                                    //Update the opp ID 
                                    //It could be different than the old and new value
                                    if (existingOpportunity != null)
                                    {
                                        existingQuote.OppID = existingOpportunity.OppID;
                                        existingOpportunity.DeleteInd = "N";

                                        // Add the opp to calculate rollups for the updated
                                        opportunityIDList.Add(existingOpportunity.OppID);
                                    }
                                    else
                                    {
                                        var newOpportunity = new Opportunity
                                        {
                                            CRMOppID = quoteDto.CRMOppID,
                                            ClientID = quoteDto.ClientID,
                                            OppStatus = 1,
                                            NumofQuotes = 0,
                                            DeleteInd = "N",
                                            CloudLastUpdBy = quoteDto.CloudLastUpdById,
                                            CloudLastUpdDT = DateTime.UtcNow
                                        };

                                        sdaCloudEntities.Opportunities.AddObject(newOpportunity);
                                        sdaCloudEntities.SaveChanges();

                                        existingQuote.OppID = newOpportunity.OppID;
                                        // Add the opp to calculate rollups for the new
                                        opportunityIDList.Add(newOpportunity.OppID);
                                    }
                                }
                                else
                                {
                                    if (quoteDto.QuoteIDMainSite != null)
                                    {
                                        quoteDto.QuoteIDMainSite = quoteDto.QuoteIDMainSite.Trim();
                                    }

                                    if (!string.IsNullOrEmpty(quoteDto.QuoteIDMainSite))
                                    {
                                        existingOpportunity =
                                            sdaCloudEntities.Opportunities.FirstOrDefault(
                                                i => i.ClientID == quoteDto.ClientID && i.QuoteIDMainSite == quoteDto.QuoteIDMainSite);

                                        if (existingOpportunity != null)
                                        {
                                            existingQuote.OppID = existingOpportunity.OppID;
                                            existingQuote.CRMOppID = existingOpportunity.CRMOppID;
                                            existingOpportunity.DeleteInd = "N";

                                            // Add the opp to calculate rollups for the dummy
                                            opportunityIDList.Add(existingOpportunity.OppID);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                string message = "Cannot update Quote with Quote Id:" + quoteDto.QuoteID + ", because the UserLastUpdDT was earlier than the existing value.";

                                throw new OutdatedQuoteException(message);
                            }
                        }

                        result = sdaCloudEntities.SaveChanges();

                        transactionScope.Complete();
                    }
                }
            }

            foreach (int opportunityID in opportunityIDList)
            {
                try
                {
                    _opportunityDl.CalculateOpportunityRollups(opportunityID);
                }
                catch (Exception e)
                {
                    _applicationLogDL.AddErrorMessage(0, "Calculate rollup returned error", e.Message);
                }
            }

            return result;
        }

        public string AddQuote(QuoteDto quoteDto)
        {
            string result;
            List<int> opportunityIDList = new List<int>();

            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(i => i.ClientID == quoteDto.ClientID && i.CRMOppID == quoteDto.CRMOppID);

                    DateTime resultDate;

                    var existingQuote = new Quote
                    {
                        QuoteID = quoteDto.QuoteID,
                        ClientID = quoteDto.ClientID,
                        OppID = quoteDto.OppID,
                        CRMOppID = quoteDto.CRMOppID,
                        QuoteSiteDesc = quoteDto.QuoteSiteDesc,
                        QuotedAmount = quoteDto.QuotedAmount ?? 0,
                        QuotedCost = quoteDto.QuotedCost ?? 0,
                        QuoteMargin = quoteDto.QuoteMargin ?? 0,
                        Rollup = quoteDto.Rollup == string.Empty ? "Y" : quoteDto.Rollup,
                        DeleteInd = "N",
                        LastFileSavedLocation = quoteDto.LastFileSavedLocation,
                        FilePlatformFileID = quoteDto.FilePlatformFileID,
                        CloudLastUpdBy = quoteDto.CloudLastUpdById,
                        CloudLastUpdDT = DateTime.UtcNow,
                        SDALastUpdBy = quoteDto.SDALastUpdBy,
                        SDALastUpdDT = DateTime.TryParse(quoteDto.SDALastUpdDT, out resultDate) ? resultDate : DateTime.MinValue,

                        CreateDT = DateTime.TryParse(quoteDto.CreateDT,
                                    out resultDate)
                                    ? resultDate
                                    : DateTime.MinValue,
                        CreateBy = quoteDto.CreateBy,

                        ClientDefinedText1 = quoteDto.ClientDefinedText1,
                        ClientDefinedText2 = quoteDto.ClientDefinedText2,
                        ClientDefinedText3 = quoteDto.ClientDefinedText3,
                        ClientDefinedText4 = quoteDto.ClientDefinedText4,
                        ClientDefinedText5 = quoteDto.ClientDefinedText5,

                        ClientDefinedTotal1 = quoteDto.ClientDefinedTotal1 ?? 0,
                        ClientDefinedTotal2 = quoteDto.ClientDefinedTotal2 ?? 0,
                        ClientDefinedTotal3 = quoteDto.ClientDefinedTotal3 ?? 0,
                        ClientDefinedTotal4 = quoteDto.ClientDefinedTotal4 ?? 0,
                        ClientDefinedTotal5 = quoteDto.ClientDefinedTotal5 ?? 0,
                        ClientDefinedTotal6 = quoteDto.ClientDefinedTotal6 ?? 0,
                        ClientDefinedTotal7 = quoteDto.ClientDefinedTotal7 ?? 0,
                        ClientDefinedTotal8 = quoteDto.ClientDefinedTotal8 ?? 0,
                        ClientDefinedTotal9 = quoteDto.ClientDefinedTotal9 ?? 0,
                        ClientDefinedTotal10 = quoteDto.ClientDefinedTotal10 ?? 0
                    };

                    if (quoteDto.CRMOppID != null)
                    {
                        quoteDto.CRMOppID = quoteDto.CRMOppID.Trim();
                    }

                    if (!string.IsNullOrEmpty(quoteDto.CRMOppID))
                    {
                        //Update the opp ID 
                        //It could be different than the old and new value
                        if (existingOpportunity != null)
                        {
                            existingQuote.OppID = existingOpportunity.OppID;
                            existingOpportunity.DeleteInd = "N";
                            // Add the opp to calculate rollups for the updated
                            opportunityIDList.Add(existingOpportunity.OppID);
                        }
                        else
                        {
                            var newOpportunity = new Opportunity
                            {
                                CRMOppID = quoteDto.CRMOppID,
                                ClientID = quoteDto.ClientID,
                                CloudLastUpdDT = DateTime.UtcNow,
                                OppStatus = 1,
                                NumofQuotes = 0,
                                DeleteInd = "N",
                                CloudLastUpdBy = quoteDto.CloudLastUpdById
                            };

                            sdaCloudEntities.Opportunities.AddObject(newOpportunity);
                            sdaCloudEntities.SaveChanges();

                            existingQuote.OppID = newOpportunity.OppID;
                            // Add the opp to calculate rollups for the new
                            opportunityIDList.Add(newOpportunity.OppID);
                        }
                    }
                    else
                    {
                        if (quoteDto.QuoteIDMainSite != null)
                        {
                            quoteDto.QuoteIDMainSite = quoteDto.QuoteIDMainSite.Trim();
                        }

                        if (!string.IsNullOrEmpty(quoteDto.QuoteIDMainSite))
                        {
                            existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(i => i.ClientID == quoteDto.ClientID && i.QuoteIDMainSite == quoteDto.QuoteIDMainSite);
                            if (existingOpportunity != null)
                            {
                                existingQuote.OppID = existingOpportunity.OppID;
                                existingQuote.CRMOppID = existingOpportunity.CRMOppID;
                                existingOpportunity.DeleteInd = "N";
                                // Add the opp to calculate rollups for the dummy
                                opportunityIDList.Add(existingOpportunity.OppID);
                            }
                            else
                            {
                                throw new Exception("Opportunity with QuoteIDMainSite: " + quoteDto.QuoteIDMainSite + " was not found.");
                            }
                        }
                    }

                    sdaCloudEntities.Quotes.AddObject(existingQuote);

                    sdaCloudEntities.SaveChanges();

                    result = existingQuote.QuoteID;

                    transactionScope.Complete();
                }
            }

            foreach (int opportunityID in opportunityIDList)
            {
                try
                {
                    _opportunityDl.CalculateOpportunityRollups(opportunityID);
                }
                catch (Exception e)
                {
                    _applicationLogDL.AddErrorMessage(0, "Calculate rollup returned error for oppId:" + opportunityID, e.Message);
                }
            }

            return result;
        }
    }
}
