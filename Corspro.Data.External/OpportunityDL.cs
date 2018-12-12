using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace Corspro.Data.External
{
    //define the global formatter
    public class DateStringFormatter : IValueFormatter
    {
        public string FormatValue(ResolutionContext context)
        {
            if (context.SourceValue == null)
                return null;

            if (!(context.SourceValue is DateTime))
                return context.SourceValue ==
                                     null ? String.Empty : context.SourceValue.ToString();

            return FormatValueCore(((DateTime)context.SourceValue).ToLocalTime());
        }

        protected string FormatValueCore(DateTime value)
        {
            return value.ToString("MM/dd/yyyy");
        }
    }

    public class RegularDateStringFormatter : IValueFormatter
    {
        public string FormatValue(ResolutionContext context)
        {
            if (context.SourceValue == null)
                return null;

            if (!(context.SourceValue is DateTime))
                return context.SourceValue ==
                                     null ? String.Empty : context.SourceValue.ToString();

            return FormatValueCore(((DateTime)context.SourceValue));
        }

        protected string FormatValueCore(DateTime value)
        {
            return value.ToString("MM/dd/yyyy");
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (_map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }


    public static class Utility
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }
    }

    public class OpportunityDL
    {
        /// <summary>
        /// Gets the opportunities.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<OpportunityDto> GetOpportunities(int clientId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    var existingOpportunities = sdaCloudEntities.Opportunities.Where(o => o.ClientID == clientId && (o.DeleteInd == null || o.DeleteInd != "Y")).ToList();

                    Mapper.CreateMap<Opportunity, OpportunityDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                        .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                        .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        ;

                    List<OpportunityDto> peopleVm = new List<OpportunityDto>();
                    if (existingOpportunities.Count > 0)
                    {
                        peopleVm = Mapper.Map<List<Opportunity>, List<OpportunityDto>>(existingOpportunities);

                        Mapper.CreateMap<Quote, QuoteDto>()
                            .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                            .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                            .ForMember(dest => dest.QuoteTable, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                            .ForMember(dest => dest.QuoteIDMainSite, opt => opt.Ignore())
                            .ForMember(dest => dest.SDAUpdatedByName, opt => opt.Ignore())
                            .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            ;

                        foreach (var opportunityDto in peopleVm)
                        {
                            OpportunityDto dto = opportunityDto;
                            var existingQuotes = sdaCloudEntities.Quotes.Where(o => o.OppID == dto.OppID).ToList();
                            opportunityDto.Quotes = Mapper.Map<List<Quote>, List<QuoteDto>>(existingQuotes);
                            foreach (var quoteDto in opportunityDto.Quotes)
                            {
                                int sdaUpdatedBy = quoteDto.SDALastUpdBy;
                                User updatedUser = sdaCloudEntities.Users.FirstOrDefault(u => u.UserID == sdaUpdatedBy);

                                if (updatedUser != null)
                                    quoteDto.SDAUpdatedByName = updatedUser.FirstName + " " + updatedUser.LastName;
                            }
                        }
                        //Mapper.AssertConfigurationIsValid();
                    }
                    return peopleVm;
                }
            }
        }

        public List<OppGridDto> GetOpportunitiesfromView(int clientId, int userId, DateTime initialDate, DateTime finalDate, string stages, List<int> liUsers)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    //var existingUsers = sdaCloudEntities.returnUsers(userId).Select(c=>c.UserID).ToList();
                    //existingUsers.Add(userId);

                    List<string> stageArray = stages.Split('/').ToList();
                    initialDate = new DateTime(2015, 2, 26);

                    var existingOpportunities = sdaCloudEntities.opportunitygrids.Where(o => o.ClientID == clientId
                                    && liUsers.Contains(o.OppOwner.Value)
                                    && o.OppCloseDate >= initialDate && o.OppCloseDate < finalDate && stageArray.Contains(o.StageType)).ToList();

                    Mapper.CreateMap<opportunitygrid, OppGridDto>();
                    List<OppGridDto> peopleVm = new List<OppGridDto>();
                    if (existingOpportunities.Count > 0)
                    {
                        peopleVm = Mapper.Map<List<opportunitygrid>, List<OppGridDto>>(existingOpportunities);

                        //Mapper.AssertConfigurationIsValid();
                    }
                    return peopleVm;
                }
            }
        }

        /// <summary>
        /// Gets the non closed opportunities.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<OpportunityDto> GetNonClosedOpportunities(int clientId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    var existingOpportunities = sdaCloudEntities.Opportunities.Where(o => o.ClientID == clientId && (o.DeleteInd == null || o.DeleteInd != "Y")).ToList();

                    Mapper.CreateMap<Opportunity, OpportunityDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                        .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                        .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        ;

                    List<OpportunityDto> peopleVm = new List<OpportunityDto>();
                    if (existingOpportunities.Count > 0)
                    {
                        peopleVm = Mapper.Map<List<Opportunity>, List<OpportunityDto>>(existingOpportunities);

                        Mapper.CreateMap<Quote, QuoteDto>()
                            .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                            .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                            .ForMember(dest => dest.QuoteTable, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                            .ForMember(dest => dest.QuoteIDMainSite, opt => opt.Ignore())
                            .ForMember(dest => dest.SDAUpdatedByName, opt => opt.Ignore())
                            .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            ;

                        foreach (var opportunityDto in peopleVm)
                        {
                            OpportunityDto dto = opportunityDto;
                            var existingQuotes = sdaCloudEntities.Quotes.Where(o => o.OppID == dto.OppID).ToList();
                            opportunityDto.Quotes = Mapper.Map<List<Quote>, List<QuoteDto>>(existingQuotes);
                            foreach (var quoteDto in opportunityDto.Quotes)
                            {
                                int sdaUpdatedBy = quoteDto.SDALastUpdBy;
                                User updatedUser = sdaCloudEntities.Users.FirstOrDefault(u => u.UserID == sdaUpdatedBy);

                                if (updatedUser != null)
                                    quoteDto.SDAUpdatedByName = updatedUser.FirstName + " " + updatedUser.LastName;
                            }
                        }
                        Mapper.AssertConfigurationIsValid();
                    }
                    return peopleVm;
                }
            }
        }

        /// <summary>
        /// Gets the non deleted opportunity by client identifier and quote identifier.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="quoteID">The quote identifier.</param>
        /// <returns></returns>
        public OpportunityDto GetNonDeletedOpportunityByClientIDAndQuoteID(int clientID, string quoteID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    // Get all the fields
                    Quote existingQuote = sdaCloudEntities.Quotes.FirstOrDefault(q => q.ClientID == clientID && q.QuoteID == quoteID);
                    OpportunityDto result = null;

                    if (existingQuote != null)
                    {
                        Opportunity existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(opp => opp.ClientID == clientID
                                                                                                               && opp.OppID == existingQuote.OppID
                                                                                                               &&
                                                                                                               (opp.DeleteInd == null ||
                                                                                                                opp.DeleteInd == "N"));


                        Mapper.CreateMap<Opportunity, OpportunityDto>()
                            .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                            .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                            .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                            .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                            .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                            .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                            .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                            .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                            .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            ;

                        if (existingOpportunity != null)
                        {
                            var peopleVm = Mapper.Map<Opportunity, OpportunityDto>(existingOpportunity);
                            int oppStatusID = peopleVm.OppStatus.Value;
                            //Complete the status name
                            OppStatu oppStatus =
                                sdaCloudEntities.OppStatus.FirstOrDefault(st => st.ID == oppStatusID);
                            if (oppStatus != null)
                                peopleVm.OppStatusName = oppStatus.OppStatus;

                            // Complete the owner's info
                            OpportunityDto dto = peopleVm;
                            User existingUser = sdaCloudEntities.Users.FirstOrDefault(us => us.UserID == dto.OppOwner);

                            if (existingUser != null)
                            {
                                peopleVm.OppOwnerName = existingUser.FirstName + " " + existingUser.LastName;
                            }

                            result = peopleVm;

                            Mapper.AssertConfigurationIsValid();
                        }
                    }
                    return result;
                }
            }
        }

        public OpportunityDto GetNonDeletedOpportunityByClientIDAndCRMOppID(int clientID, string crmOppID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    // Get all the fields
                    Opportunity existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(opp => opp.ClientID == clientID
                                                                                                           && opp.CRMOppID == crmOppID
                                                                                                           && (opp.DeleteInd == null || opp.DeleteInd == "N"));

                    Mapper.CreateMap<Opportunity, OpportunityDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                        .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                        .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        ;

                    OpportunityDto result = null;
                    if (existingOpportunity != null)
                    {
                        var peopleVm = Mapper.Map<Opportunity, OpportunityDto>(existingOpportunity);

                        int oppStatusID = peopleVm.OppStatus.Value;
                        //Complete the status name
                        OppStatu oppStatus = sdaCloudEntities.OppStatus.FirstOrDefault(st => st.ID == oppStatusID);
                        if (oppStatus != null)
                            peopleVm.OppStatusName = oppStatus.OppStatus;

                        // Complete the owner's info
                        OpportunityDto dto = peopleVm;
                        User existingUser = sdaCloudEntities.Users.FirstOrDefault(us => us.UserID == dto.OppOwner);

                        if (existingUser != null)
                        {
                            peopleVm.OppOwnerName = existingUser.FirstName + " " + existingUser.LastName;
                        }

                        result = peopleVm;

                        Mapper.AssertConfigurationIsValid();
                    }
                    return result;
                }
            }
        }

        public OpportunityDto GetNonDeletedOpportunityByClientIDAndOppID(int clientID, int oppID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    // Get all the fields
                    Opportunity existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(opp => opp.ClientID == clientID
                                                                                                           && opp.OppID == oppID
                                                                                                           && (opp.DeleteInd == null || opp.DeleteInd == "N"));

                    Mapper.CreateMap<Opportunity, OpportunityDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                        .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                        .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        ;

                    OpportunityDto result = null;
                    if (existingOpportunity != null)
                    {
                        var peopleVm = Mapper.Map<Opportunity, OpportunityDto>(existingOpportunity);

                        int oppStatusID = peopleVm.OppStatus ?? 0;
                        //Complete the status name
                        OppStatu oppStatus = sdaCloudEntities.OppStatus.FirstOrDefault(st => st.ID == oppStatusID);
                        if (oppStatus != null)
                            peopleVm.OppStatusName = oppStatus.OppStatus;

                        // Complete the owner's info
                        OpportunityDto dto = peopleVm;
                        User existingUser = sdaCloudEntities.Users.FirstOrDefault(us => us.UserID == dto.OppOwner);

                        if (existingUser != null)
                        {
                            peopleVm.OppOwnerName = existingUser.FirstName + " " + existingUser.LastName;
                        }

                        result = peopleVm;

                        Mapper.AssertConfigurationIsValid();
                    }
                    return result;
                }
            }
        }

        public OpportunityDto GetOpportunityByClientIDAndCRMOppID(int clientID, string crmOppID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    // Get all the fields
                    Opportunity existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(opp => opp.ClientID == clientID
                                                                                                           && opp.CRMOppID == crmOppID);

                    Mapper.CreateMap<Opportunity, OpportunityDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                        .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                        .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        ;

                    OpportunityDto result = null;
                    if (existingOpportunity != null)
                    {
                        var peopleVm = Mapper.Map<Opportunity, OpportunityDto>(existingOpportunity);
                        result = peopleVm;

                        Mapper.AssertConfigurationIsValid();
                    }
                    return result;
                }
            }
        }

        public List<OpportunityDto> GetOpportunities(int clientID, string oppId, string opportunityName, string companyName, string owner, string opportunityStatus)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    Expression<Func<Opportunity, bool>> clientWhere = opp => opp.ClientID.Equals(clientID) && (opp.DeleteInd == null || opp.DeleteInd == opportunityStatus);

                    if (!string.IsNullOrEmpty(owner))
                    {
                        List<int> userIds = sdaCloudEntities.Users.Where(us => us.LastName.Contains(owner)).Select(u => u.UserID).ToList();
                        Expression<Func<Opportunity, bool>> newPred = opp => userIds.Contains(opp.OppOwner.Value);
                        clientWhere = clientWhere.And(newPred);
                    }

                    if (!string.IsNullOrEmpty(oppId))
                    {
                        Expression<Func<Opportunity, bool>> newPred = opp => opp.CRMOppID.Contains(oppId);
                        clientWhere = clientWhere.And(newPred);
                    }

                    if (!string.IsNullOrEmpty(opportunityName))
                    {
                        Expression<Func<Opportunity, bool>> newPred = opp => opp.OppName.Contains(opportunityName);
                        clientWhere = clientWhere.And(newPred);
                    }

                    if (!string.IsNullOrEmpty(companyName))
                    {
                        Expression<Func<Opportunity, bool>> newPred = opp => opp.CompanyName.Contains(companyName);
                        clientWhere = clientWhere.And(newPred);
                    }


                    // Get all the fields
                    var existingOpportunities = sdaCloudEntities.Opportunities.Where(clientWhere).Take(100).ToList();

                    Mapper.CreateMap<Opportunity, OpportunityDto>()
                        .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                        .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                        .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                        .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                        .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                        ;

                    var result = new List<OpportunityDto>();
                    if (existingOpportunities.Count > 0)
                    {
                        var peopleVm = Mapper.Map<List<Opportunity>, List<OpportunityDto>>(existingOpportunities);

                        foreach (var opportunityDto in peopleVm)
                        {
                            int oppStatusID = opportunityDto.OppStatus ?? 0;
                            //Complete the status name
                            OppStatu oppStatus =
                                sdaCloudEntities.OppStatus.FirstOrDefault(st => st.ID == oppStatusID);
                            if (oppStatus != null)
                                opportunityDto.OppStatusName = oppStatus.OppStatus;

                            // Complete the owner's info
                            OpportunityDto dto = opportunityDto;
                            User existingUser = sdaCloudEntities.Users.FirstOrDefault(us => us.UserID == dto.OppOwner);

                            if (existingUser != null)
                            {
                                opportunityDto.OppOwnerName = existingUser.FirstName + " " + existingUser.LastName;
                            }

                            result.Add(opportunityDto);
                        }

                        Mapper.AssertConfigurationIsValid();
                    }
                    return result;
                }
            }
        }

        public List<OppStatusDto> GetStatuses(int clientId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    //sdaCloudEntities.Connection.Open();

                    var existingUsers =
                        sdaCloudEntities.OppStatus.Where(u => u.ClientID == clientId).OrderBy(st => st.Order).ToList();

                    Mapper.CreateMap<OppStatu, OppStatusDto>()
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y")); ;
                    List<OppStatusDto> peopleVm = new List<OppStatusDto>();
                    if (existingUsers.Count > 0)
                    {
                        peopleVm = Mapper.Map<List<OppStatu>, List<OppStatusDto>>(existingUsers);
                        Mapper.AssertConfigurationIsValid();
                    }
                    return peopleVm;
                }
            }
        }

        /// <summary>
        /// Updates the opportunity by identifier.
        /// </summary>
        /// <param name="opportunityDto">The opportunity dto.</param>
        /// <returns></returns>
        public int UpdateOpportunityById(OpportunityDto opportunityDto)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(i => i.OppID == opportunityDto.OppID);

                        if (existingOpportunity != null)
                        {
                            Mapper.CreateMap<Opportunity, OpportunityDto>()
                            .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                            .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                            .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                            .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                            .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                            .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                            .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                            .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            ;


                            var exOpp = Mapper.Map<Opportunity, OpportunityDto>(existingOpportunity);

                            Mapper.AssertConfigurationIsValid();


                            existingOpportunity.OppName = opportunityDto.OppName;
                            existingOpportunity.CompanyName = opportunityDto.CompanyName;
                            if (existingOpportunity.NumofQuotes == 0)
                            {
                                existingOpportunity.QuotedAmount = opportunityDto.QuotedAmount;
                            }

                            DateTime resultDate;
                            existingOpportunity.OppCloseDate = DateTime.TryParse(opportunityDto.OppCloseDate, out resultDate) ? resultDate : DateTime.MinValue;
                            existingOpportunity.OppOwner = opportunityDto.OppOwner;
                            existingOpportunity.OppStatus = opportunityDto.OppStatus ?? GetDefaultStatus(opportunityDto.ClientID);

                            existingOpportunity.OppProbability = ((opportunityDto.OppProbability == null) || (opportunityDto.OppProbability <= 0))
                                ? GetDefaultProbabilityByStatus(existingOpportunity.ClientID, existingOpportunity.OppStatus) : opportunityDto.OppProbability;

                            existingOpportunity.CloudLastUpdBy = opportunityDto.CloudLastUpdById;
                            existingOpportunity.CloudLastUpdDT = DateTime.UtcNow;

                            existingOpportunity.ClientDefinedText1 = opportunityDto.ClientDefinedText1;
                            existingOpportunity.ClientDefinedText2 = opportunityDto.ClientDefinedText2;
                            existingOpportunity.ClientDefinedText3 = opportunityDto.ClientDefinedText3;
                            existingOpportunity.ClientDefinedText4 = opportunityDto.ClientDefinedText4;
                            existingOpportunity.ClientDefinedText5 = opportunityDto.ClientDefinedText5;


                            if (opportunityDto.ClientDefinedTotal1 != null)
                                existingOpportunity.ClientDefinedTotal1 = opportunityDto.ClientDefinedTotal1 ?? 0;
                            if (opportunityDto.ClientDefinedTotal2 != null)
                                existingOpportunity.ClientDefinedTotal2 = opportunityDto.ClientDefinedTotal2 ?? 0;
                            if (opportunityDto.ClientDefinedTotal3 != null)
                                existingOpportunity.ClientDefinedTotal3 = opportunityDto.ClientDefinedTotal3 ?? 0;
                            if (opportunityDto.ClientDefinedTotal4 != null)
                                existingOpportunity.ClientDefinedTotal4 = opportunityDto.ClientDefinedTotal4 ?? 0;
                            if (opportunityDto.ClientDefinedTotal5 != null)
                                existingOpportunity.ClientDefinedTotal5 = opportunityDto.ClientDefinedTotal5 ?? 0;
                            if (opportunityDto.ClientDefinedTotal6 != null)
                                existingOpportunity.ClientDefinedTotal6 = opportunityDto.ClientDefinedTotal6 ?? 0;
                            if (opportunityDto.ClientDefinedTotal7 != null)
                                existingOpportunity.ClientDefinedTotal7 = opportunityDto.ClientDefinedTotal7 ?? 0;
                            if (opportunityDto.ClientDefinedTotal8 != null)
                                existingOpportunity.ClientDefinedTotal8 = opportunityDto.ClientDefinedTotal8 ?? 0;
                            if (opportunityDto.ClientDefinedTotal9 != null)
                                existingOpportunity.ClientDefinedTotal9 = opportunityDto.ClientDefinedTotal9 ?? 0;
                            if (opportunityDto.ClientDefinedTotal10 != null)
                                existingOpportunity.ClientDefinedTotal10 = opportunityDto.ClientDefinedTotal10 ?? 0;


                            var interfaceXRefDL = new InterfaceXRefDL();
                            var mapToRecords = interfaceXRefDL.GetRecordsWithMappedFields(opportunityDto.ClientID, "SendOppUpdates");

                            bool exist = false;
                            foreach (var mappingObject in mapToRecords)
                            {
                                var propInfo = exOpp.GetType().GetProperty(mappingObject.SDASMField);
                                var newValue = propInfo.GetValue(exOpp, null);
                                if (newValue != DBNull.Value)
                                {
                                    var propInfo2 = opportunityDto.GetType().GetProperty(mappingObject.SDASMField);
                                    var newValue2 = propInfo2.GetValue(opportunityDto, null);
                                    if (newValue != newValue2)
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                            }

                            var client =
                                    sdaCloudEntities.Clients.FirstOrDefault(i => i.ClientID == opportunityDto.ClientID);
                            if (client != null)
                            {
                                if (client.SendCRMOppValueUpdates == "Y" && exist)
                                {
                                    existingOpportunity.LastCRMValuesUpdDT = DateTime.Now;
                                }
                            }
                        }

                        result = sdaCloudEntities.SaveChanges();

                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Updates the opportunity.
        /// </summary>
        /// <param name="opportunityDto">The opportunity dto.</param>
        /// <returns></returns>
        public int UpdateOpportunity(OpportunityDto opportunityDto)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        //sdaCloudEntities.Connection.Open();

                        var existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(i => i.ClientID == opportunityDto.ClientID && i.CRMOppID == opportunityDto.CRMOppID);

                        if (existingOpportunity != null)
                        {

                            Mapper.CreateMap<Opportunity, OpportunityDto>()
                            .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                            .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                            .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                            .ForMember(dest => dest.Quotes, opt => opt.Ignore())
                            .ForMember(dest => dest.OppOwnerName, opt => opt.Ignore())
                            .ForMember(dest => dest.OppStatusName, opt => opt.Ignore())
                            .ForMember(dest => dest.OppCloseDate, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                            .ForMember(dest => dest.CloudLastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                            .ForMember(dest => dest.SDALastUpdDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            .ForMember(dest => dest.CreateDT, opt => opt.AddFormatter<RegularDateStringFormatter>())
                            ;

                            var exOpp = Mapper.Map<Opportunity, OpportunityDto>(existingOpportunity);

                            Mapper.AssertConfigurationIsValid();


                            if (opportunityDto.OppName != string.Empty)
                                existingOpportunity.OppName = opportunityDto.OppName;

                            existingOpportunity.OppStatus = opportunityDto.OppStatus ??
                                                            GetDefaultStatus(opportunityDto.ClientID);

                            existingOpportunity.OppProbability = ((opportunityDto.OppProbability == null) || (opportunityDto.OppProbability <= 0))
                                ? GetDefaultProbabilityByStatus(existingOpportunity.ClientID, existingOpportunity.OppStatus) : opportunityDto.OppProbability;

                            if (opportunityDto.QuoteIDMainSite != string.Empty)
                                existingOpportunity.QuoteIDMainSite = opportunityDto.QuoteIDMainSite;
                            DateTime resultDate;
                            if (opportunityDto.OppCloseDate != string.Empty)
                            {
                                existingOpportunity.OppCloseDate = DateTime.TryParse(opportunityDto.OppCloseDate,
                                    out resultDate)
                                    ? resultDate
                                    : DateTime.MinValue;
                            }
                            if (opportunityDto.CompanyName != string.Empty)
                                existingOpportunity.CompanyName = opportunityDto.CompanyName;

                            existingOpportunity.DeleteInd = "N";

                            existingOpportunity.SDALastUpdBy = opportunityDto.SDALastUpdBy;
                            existingOpportunity.SDALastUpdDT = DateTime.TryParse(opportunityDto.SDALastUpdDT, out resultDate)
                                ? resultDate
                                : DateTime.MinValue;

                            existingOpportunity.CloudLastUpdBy = opportunityDto.CloudLastUpdById;
                            existingOpportunity.CloudLastUpdDT = DateTime.UtcNow;
                            existingOpportunity.CreateDT = DateTime.TryParse(opportunityDto.CreateDT,
                                    out resultDate)
                                    ? resultDate
                                    : DateTime.MinValue;
                            existingOpportunity.CreateBy = opportunityDto.CreateBy;
                            existingOpportunity.ClientDefinedText1 = opportunityDto.ClientDefinedText1;
                            existingOpportunity.ClientDefinedText2 = opportunityDto.ClientDefinedText2;
                            existingOpportunity.ClientDefinedText3 = opportunityDto.ClientDefinedText3;
                            existingOpportunity.ClientDefinedText4 = opportunityDto.ClientDefinedText4;
                            existingOpportunity.ClientDefinedText5 = opportunityDto.ClientDefinedText5;


                            if (opportunityDto.ClientDefinedTotal1 != null)
                                existingOpportunity.ClientDefinedTotal1 = opportunityDto.ClientDefinedTotal1 ?? 0;
                            if (opportunityDto.ClientDefinedTotal2 != null)
                                existingOpportunity.ClientDefinedTotal2 = opportunityDto.ClientDefinedTotal2 ?? 0;
                            if (opportunityDto.ClientDefinedTotal3 != null)
                                existingOpportunity.ClientDefinedTotal3 = opportunityDto.ClientDefinedTotal3 ?? 0;
                            if (opportunityDto.ClientDefinedTotal4 != null)
                                existingOpportunity.ClientDefinedTotal4 = opportunityDto.ClientDefinedTotal4 ?? 0;
                            if (opportunityDto.ClientDefinedTotal5 != null)
                                existingOpportunity.ClientDefinedTotal5 = opportunityDto.ClientDefinedTotal5 ?? 0;
                            if (opportunityDto.ClientDefinedTotal6 != null)
                                existingOpportunity.ClientDefinedTotal6 = opportunityDto.ClientDefinedTotal6 ?? 0;
                            if (opportunityDto.ClientDefinedTotal7 != null)
                                existingOpportunity.ClientDefinedTotal7 = opportunityDto.ClientDefinedTotal7 ?? 0;
                            if (opportunityDto.ClientDefinedTotal8 != null)
                                existingOpportunity.ClientDefinedTotal8 = opportunityDto.ClientDefinedTotal8 ?? 0;
                            if (opportunityDto.ClientDefinedTotal9 != null)
                                existingOpportunity.ClientDefinedTotal9 = opportunityDto.ClientDefinedTotal9 ?? 0;
                            if (opportunityDto.ClientDefinedTotal10 != null)
                                existingOpportunity.ClientDefinedTotal10 = opportunityDto.ClientDefinedTotal10 ?? 0;

                            var interfaceXRefDL = new InterfaceXRefDL();
                            var mapToRecords = interfaceXRefDL.GetRecordsWithMappedFields(opportunityDto.ClientID, "SendOppUpdates");

                            ApplicationLogDL appDl = new ApplicationLogDL();
                            bool exist = false;
                            foreach (var mappingObject in mapToRecords)
                            {
                                var propInfo = exOpp.GetType().GetProperty(mappingObject.SDASMField);
                                var newValue = propInfo.GetValue(exOpp, null);
                                if (newValue != DBNull.Value)
                                {
                                    var propInfo2 = opportunityDto.GetType().GetProperty(mappingObject.SDASMField);
                                    var newValue2 = propInfo2.GetValue(opportunityDto, null);
                                    if (newValue != newValue2)
                                    {
                                        appDl.AddErrorMessage(opportunityDto.ClientID, "Prop:" + mappingObject.SDASMField + ",before:" + newValue.ToString() + ",after" + newValue2.ToString(), string.Empty);
                                        exist = true;
                                        break;
                                    }
                                }
                            }

                            var client =
                                    sdaCloudEntities.Clients.FirstOrDefault(i => i.ClientID == opportunityDto.ClientID);
                            if (client != null)
                            {
                                if ((!string.IsNullOrEmpty(client.CRMSystem) && client.CRMSystem.ToUpper().Equals("SALESFORCE")) && (string.IsNullOrEmpty(existingOpportunity.CRMSync)))
                                {
                                    existingOpportunity.CRMSync = "P";
                                }

                                if (client.SendCRMOppValueUpdates == "Y" && exist)
                                {
                                    existingOpportunity.LastCRMValuesUpdDT = DateTime.Now;
                                }
                            }
                        }

                        result = sdaCloudEntities.SaveChanges();

                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Adds the opportunity.
        /// </summary>
        /// <param name="opportunityDto">The opportunity dto.</param>
        /// <param name="manageOpportunitiesInCRM">if set to <c>true</c> [manage opportunities in CRM].</param>
        /// <returns></returns>
        public int AddOpportunity(OpportunityDto opportunityDto, bool manageOpportunitiesInCRM)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        DateTime lookupDT;
                        DateTime.TryParse(opportunityDto.SDALastUpdDT,
                            out lookupDT);
                        var existingOpportunity = sdaCloudEntities.Opportunities.FirstOrDefault(i => i.ClientID == opportunityDto.ClientID
                            && i.QuoteIDMainSite == opportunityDto.QuoteIDMainSite && i.OppName == opportunityDto.OppName
                            && i.CompanyName == opportunityDto.CompanyName && i.SDALastUpdBy == opportunityDto.SDALastUpdBy && i.SDALastUpdDT == lookupDT);

                        if (existingOpportunity == null)
                        {
                            DateTime resultDate;
                            existingOpportunity = new Opportunity
                            {
                                ClientID = opportunityDto.ClientID,
                                CRMOppID = opportunityDto.CRMOppID,
                                OppName = opportunityDto.OppName,
                                OppProbability = opportunityDto.OppProbability,
                                OppStatus = opportunityDto.OppStatus ?? GetDefaultStatus(opportunityDto.ClientID),
                                OppCloseDate = DateTime.TryParse(opportunityDto.OppCloseDate,
                                    out resultDate)
                                    ? resultDate
                                    : DateTime.MinValue,
                                CompanyName = opportunityDto.CompanyName,
                                QuotedAmount = opportunityDto.QuotedAmount,
                                OppOwner = opportunityDto.OppOwner,
                                QuoteIDMainSite = opportunityDto.QuoteIDMainSite,
                                CloudLastUpdBy = opportunityDto.CloudLastUpdById,
                                CloudLastUpdDT = DateTime.UtcNow,
                                SDALastUpdBy = opportunityDto.SDALastUpdBy,
                                SDALastUpdDT = DateTime.TryParse(opportunityDto.SDALastUpdDT,
                                    out resultDate)
                                    ? resultDate
                                    : DateTime.MinValue,
                                NumofQuotes = 0,
                                DeleteInd = "N",
                                CreateDT = DateTime.TryParse(opportunityDto.CreateDT,
                                    out resultDate)
                                    ? resultDate
                                    : DateTime.UtcNow,
                                CreateBy = opportunityDto.CreateBy,
                                ClientDefinedTotal1 = opportunityDto.ClientDefinedTotal1 ?? 0,
                                ClientDefinedTotal2 = opportunityDto.ClientDefinedTotal2 ?? 0,
                                ClientDefinedTotal3 = opportunityDto.ClientDefinedTotal3 ?? 0,
                                ClientDefinedTotal4 = opportunityDto.ClientDefinedTotal4 ?? 0,
                                ClientDefinedTotal5 = opportunityDto.ClientDefinedTotal5 ?? 0,
                                ClientDefinedTotal6 = opportunityDto.ClientDefinedTotal6 ?? 0,
                                ClientDefinedTotal7 = opportunityDto.ClientDefinedTotal7 ?? 0,
                                ClientDefinedTotal8 = opportunityDto.ClientDefinedTotal8 ?? 0,
                                ClientDefinedTotal9 = opportunityDto.ClientDefinedTotal9 ?? 0,
                                ClientDefinedTotal10 = opportunityDto.ClientDefinedTotal10 ?? 0,
                                ClientDefinedText1 = opportunityDto.ClientDefinedText1,
                                ClientDefinedText2 = opportunityDto.ClientDefinedText2,
                                ClientDefinedText3 = opportunityDto.ClientDefinedText3,
                                ClientDefinedText4 = opportunityDto.ClientDefinedText4,
                                ClientDefinedText5 = opportunityDto.ClientDefinedText5
                            };

                            if ((existingOpportunity.OppProbability == null) || (existingOpportunity.OppProbability == 0))
                            {
                                existingOpportunity.OppProbability = GetDefaultProbabilityByStatus(existingOpportunity.ClientID, existingOpportunity.OppStatus);
                            }

                            sdaCloudEntities.Opportunities.AddObject(existingOpportunity);

                            sdaCloudEntities.SaveChanges();

                            var client =
                                    sdaCloudEntities.Clients.FirstOrDefault(i => i.ClientID == opportunityDto.ClientID);
                            if (client != null)
                            {
                                if (!string.IsNullOrEmpty(client.CRMSystem) && client.CRMSystem.ToUpper().Equals("SALESFORCE"))
                                {
                                    existingOpportunity.CRMSync = "P";
                                }

                                if (!manageOpportunitiesInCRM && string.IsNullOrEmpty(existingOpportunity.CRMOppID))
                                {
                                    int nextCRMOppID = client.NextCRMOppID ?? 1;
                                    existingOpportunity.CRMOppID = nextCRMOppID.ToString(CultureInfo.InvariantCulture);
                                    client.NextCRMOppID = nextCRMOppID + 1;
                                }

                                if (client.SendCRMOppValueUpdates == "Y")
                                {
                                    existingOpportunity.LastCRMValuesUpdDT = DateTime.Now;
                                }

                                sdaCloudEntities.SaveChanges();
                            }

                            result = existingOpportunity.OppID;

                        }
                        else
                        {
                            result = -1;
                        }

                        transactionScope.Complete();
                    }
                }
            }

            return result;
        }

        private int GetDefaultStatus(int clientID)
        {
            int defaultStatus = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var oppstatus = sdaCloudEntities.OppStatus.FirstOrDefault(i => i.ClientID == clientID && i.Order == 1);
                    if (oppstatus != null) defaultStatus = oppstatus.ID;
                }
            }
            return defaultStatus;
        }

        /// <summary>
        /// Gets the default probability by status.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public int GetDefaultProbabilityByStatus(int clientID, int status)
        {
            int defaultProbability = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var oppstatus = sdaCloudEntities.OppStatus.FirstOrDefault(i => i.ClientID == clientID && i.ID == status);
                    if (oppstatus != null) defaultProbability = oppstatus.OppProbability ?? 0;
                }
            }
            return defaultProbability;
        }

        /// <summary>
        /// Deletes the opportunity.
        /// </summary>
        /// <param name="opportunityId">The opportunity identifier.</param>
        /// <returns></returns>
        public int DeleteOpportunity(int opportunityId)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    //sdaCloudEntities.Connection.Open();

                    var existingOpportunity = sdaCloudEntities.Opportunities.SingleOrDefault(i => i.OppID == opportunityId);
                    if (existingOpportunity != null) existingOpportunity.DeleteInd = "Y";
                    result = sdaCloudEntities.SaveChanges();
                    transactionScope.Complete();
                }
            }
            return result;
        }

        public int CalculateOpportunityRollups(int opportunityID)
        {
            int result = 0;
            bool works = false;
            do
            {
                try
                {
                    using (var sdaCloudEntities = new SDACloudEntities())
                    {
                        using (sdaCloudEntities)
                        {
                            using (var transactionScope = new TransactionScope())
                            {
                                //sdaCloudEntities.Connection.Open();

                                // Get the opportuniy
                                Opportunity opportunity = sdaCloudEntities.Opportunities.FirstOrDefault(opp => opp.OppID == opportunityID);

                                if (opportunity != null)
                                {
                                    // Get the quotes
                                    List<Quote> existingQuotes = sdaCloudEntities.Quotes.Where(q => q.ClientID == opportunity.ClientID && q.OppID == opportunityID && q.Rollup == "Y" && q.DeleteInd == "N").ToList();

                                    // Reset the values
                                    opportunity.QuotedAmount = 0;
                                    opportunity.QuotedCost = 0;
                                    opportunity.QuotedMargin = 0;
                                    opportunity.ClientDefinedTotal1 = 0;
                                    opportunity.ClientDefinedTotal2 = 0;
                                    opportunity.ClientDefinedTotal3 = 0;
                                    opportunity.ClientDefinedTotal4 = 0;
                                    opportunity.ClientDefinedTotal5 = 0;
                                    opportunity.ClientDefinedTotal6 = 0;
                                    opportunity.ClientDefinedTotal7 = 0;
                                    opportunity.ClientDefinedTotal8 = 0;
                                    opportunity.ClientDefinedTotal9 = 0;
                                    opportunity.ClientDefinedTotal10 = 0;

                                    // Recalculte the values
                                    foreach (Quote quote in existingQuotes)
                                    {
                                        opportunity.QuotedAmount += quote.QuotedAmount;
                                        opportunity.QuotedCost += quote.QuotedCost;
                                        opportunity.ClientDefinedTotal1 += quote.ClientDefinedTotal1.HasValue ? quote.ClientDefinedTotal1.Value : 0;
                                        opportunity.ClientDefinedTotal2 += quote.ClientDefinedTotal2.HasValue ? quote.ClientDefinedTotal2.Value : 0;
                                        opportunity.ClientDefinedTotal3 += quote.ClientDefinedTotal3.HasValue ? quote.ClientDefinedTotal3.Value : 0;
                                        opportunity.ClientDefinedTotal4 += quote.ClientDefinedTotal4.HasValue ? quote.ClientDefinedTotal4.Value : 0;
                                        opportunity.ClientDefinedTotal5 += quote.ClientDefinedTotal5.HasValue ? quote.ClientDefinedTotal5.Value : 0;
                                        opportunity.ClientDefinedTotal6 += quote.ClientDefinedTotal6.HasValue ? quote.ClientDefinedTotal6.Value : 0;
                                        opportunity.ClientDefinedTotal7 += quote.ClientDefinedTotal7.HasValue ? quote.ClientDefinedTotal7.Value : 0;
                                        opportunity.ClientDefinedTotal8 += quote.ClientDefinedTotal8.HasValue ? quote.ClientDefinedTotal8.Value : 0;
                                        opportunity.ClientDefinedTotal9 += quote.ClientDefinedTotal9.HasValue ? quote.ClientDefinedTotal9.Value : 0;
                                        opportunity.ClientDefinedTotal10 += quote.ClientDefinedTotal10.HasValue ? quote.ClientDefinedTotal10.Value : 0;
                                    }
                                    opportunity.NumofQuotes = existingQuotes.Count;

                                    if (opportunity.QuotedAmount == 0)
                                    {
                                        opportunity.QuotedMargin = 0;
                                    }
                                    else
                                    {
                                        opportunity.QuotedMargin = (opportunity.QuotedAmount - opportunity.QuotedCost) / opportunity.QuotedAmount;
                                    }

                                    Client client = sdaCloudEntities.Clients.FirstOrDefault(c => c.ClientID == opportunity.ClientID);

                                    if (client.SendCRMOppValueUpdates == "Y")
                                    {
                                        opportunity.LastCRMValuesUpdDT = DateTime.Now;
                                    }
                                }

                                result = sdaCloudEntities.SaveChanges();
                                transactionScope.Complete();

                                works = false;
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1205 || ex.Number == 1206 || ex.Number == 1213)
                    {
                        works = true;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            } while (works);

            return result;
        }

        public OpportunityDto GetOpportunityByQuoteId(string QuoteId)
        {
            OpportunityDto result = null;
            using (SDACloudEntities sDACloudEntities = new SDACloudEntities())
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    Quote existingQuote = sDACloudEntities.Quotes.SingleOrDefault((Quote i) => i.QuoteID == QuoteId);
                    if (existingQuote != null)
                    {
                        Opportunity opportunity = sDACloudEntities.Opportunities.SingleOrDefault((Opportunity i) => i.OppID == existingQuote.OppID);
                        if (opportunity != null)
                        {
                            result = new OpportunityDto
                            {
                                CRMOppID = opportunity.CRMOppID,
                                OppName = opportunity.OppName,
                                OppOwner = opportunity.OppOwner.Value,
                                CompanyName = opportunity.CompanyName
                            };
                        }
                    }
                    transactionScope.Complete();
                }
            }
            return result;
        }
    }
}
