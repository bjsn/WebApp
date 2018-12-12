using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System.Reflection;

namespace Corspro.Data.External
{
    public class OppStatusDL
    {
        /// <summary>
        /// Gets the client defined fields.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public List<OppStatusDto> GetOppStatuses(int clientID)
        {
            List<OppStatusDto> oppStatusList = new List<OppStatusDto>();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingStatus = sdaCloudEntities.OppStatus.Where(i => i.ClientID == clientID && (i.DeleteInd == null || i.DeleteInd != "Y")).OrderBy(x => x.Order).ToList();
                    Mapper.CreateMap<OppStatu, OppStatusDto>()
                        .ForMember(dest => dest.DeleteInd, opt => opt.ResolveUsing(src => src.DeleteInd.ToUpper() == "Y"));

                    if (existingStatus.Count > 0)
                    {
                        oppStatusList = Mapper.Map<List<OppStatu>, List<OppStatusDto>>(existingStatus);
                        //Mapper.AssertConfigurationIsValid();

                    }
                }
            }
            return oppStatusList;
        }

        /// <summary>
        /// Validates the order.
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="oppStatusId">The opp status identifier.</param>
        /// <param name="newOrder">The new order.</param>
        /// <returns></returns>
        public bool ValidateOrder(int clientID, int oppStatusId, int newOrder)
        {
            bool result = true;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingOppStatus = sdaCloudEntities.OppStatus.FirstOrDefault(i => i.ClientID == clientID && i.Order == newOrder && (i.DeleteInd == null || i.DeleteInd != "Y"));

                    if (existingOppStatus != null)
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Adds the opp status.
        /// </summary>
        /// <param name="oppStatusDto">The opp status dto.</param>
        /// <returns></returns>
        public int AddOppStatus(OppStatusDto oppStatusDto)
        {
            int result = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingOppStat = sdaCloudEntities.OppStatus.FirstOrDefault(i => i.ClientID == oppStatusDto.ClientID && i.Order == oppStatusDto.Order);

                    if (existingOppStat == null)
                    {
                        existingOppStat = new OppStatu
                        {
                            ClientID = oppStatusDto.ClientID,
                            OppStatus = oppStatusDto.OppStatus,
                            Order = oppStatusDto.Order,
                            OppProbability = oppStatusDto.OppProbability,
                            DeleteInd = "N",
                            StageType = oppStatusDto.StageType,
                            Default = oppStatusDto.Default
                        };

                        //sdaCloudEntities.AddToOppStatus(existingOppStat);
                        result = sdaCloudEntities.SaveChanges();
                        if (result > 0)
                        {
                            existingOppStat = sdaCloudEntities.OppStatus.FirstOrDefault(i => i.ClientID == oppStatusDto.ClientID && i.Order == oppStatusDto.Order);
                            result = existingOppStat.ID;
                        }
                    }
                    else
                    {
                        if (existingOppStat.DeleteInd == "Y")
                        {
                            existingOppStat.OppStatus = oppStatusDto.OppStatus;
                            existingOppStat.OppProbability = oppStatusDto.OppProbability;
                            existingOppStat.DeleteInd = "N";
                            existingOppStat.StageType = oppStatusDto.StageType;
                            existingOppStat.Default = oppStatusDto.Default;
                            sdaCloudEntities.SaveChanges();
                            result = existingOppStat.ID;
                        }
                        else
                        {
                            result = -2;
                        }
                    }

                    if (result > 0 && oppStatusDto.Default.Equals("Y"))
                    {
                        existingOppStat = sdaCloudEntities.OppStatus.FirstOrDefault(i => i.ClientID == oppStatusDto.ClientID && i.Default == "Y" && i.ID != result);
                        if (existingOppStat != null)
                        {
                            existingOppStat.Default = "N";
                            sdaCloudEntities.SaveChanges();
                        }
                    }

                    transactionScope.Complete();
                }
            }
            return result;
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        /// <summary>
        /// Updates the opp status field.
        /// </summary>
        /// <param name="cdfId">The CDF identifier.</param>
        /// <param name="field">The field.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        public int UpdateOppStatusField(int cdfId, string field, string newValue)
        {
            int result = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingOppStatus = sdaCloudEntities.OppStatus.FirstOrDefault(i => i.ID == cdfId);

                        if (existingOppStatus != null)
                        {
                            if (field.Equals("Order") || field.Equals("OppProbability"))
                            {
                                object val = Int32.Parse(newValue.ToString());
                                Type t = existingOppStatus.GetType();
                                PropertyInfo info = t.GetProperty(field);

                                if (info == null)
                                    return -1;
                                if (!info.CanWrite)
                                    return -1;

                                //Convert.ChangeType does not handle conversion to nullable types
                                //if the property type is nullable, we need to get the underlying type of the property
                                var targetType = IsNullableType(info.PropertyType) ? Nullable.GetUnderlyingType(info.PropertyType) : info.PropertyType;

                                //Returns an System.Object with the specified System.Type and whose value is
                                //equivalent to the specified object.
                                val = Convert.ChangeType(val, targetType);

                                //Set the value of the property
                                info.SetValue(existingOppStatus, val, null);
                            }
                            else
                            {
                                Type t = existingOppStatus.GetType();
                                PropertyInfo info = t.GetProperty(field);

                                if (info == null)
                                    return -1;
                                if (!info.CanWrite)
                                    return -1;
                                info.SetValue(existingOppStatus, newValue, null);
                            }

                        }

                        if (result < 2000)
                        {
                            result = sdaCloudEntities.SaveChanges();
                        }

                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }

        public int UpdateOppStatusDefault(int clientId, int cdfId)
        {
            int result = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var collection = sdaCloudEntities.OppStatus.Where(i => i.ClientID == clientId);
                        collection.ToList().ForEach(c => c.Default = "N");
                        sdaCloudEntities.SaveChanges();

                        var existingOppStatus = sdaCloudEntities.OppStatus.FirstOrDefault(i => i.ID == cdfId);

                        if (existingOppStatus != null)
                        {
                            existingOppStatus.Default = "Y";
                            result = sdaCloudEntities.SaveChanges();
                        }

                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Deletes the opp statuses.
        /// </summary>
        /// <param name="uList">The u list.</param>
        /// <returns></returns>
        public string DeleteOppStatuses(string uList)
        {
            string result = string.Empty;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<string> tagIds = uList.Split(',').ToList();

                foreach (var uv in tagIds)
                {
                    int uvId;
                    Int32.TryParse(uv, out uvId);
                    var oppStatus = sdaCloudEntities.OppStatus.Where(q => q.ID == uvId).SingleOrDefault();
                    if (oppStatus != null) //&& oppStatus.Opportunities.Count == 0
                    {
                        oppStatus.DeleteInd = "Y";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(result)) result += ",";
                        result += uv;
                    }
                }

                sdaCloudEntities.SaveChanges();
            }
            return result;
        }
    }
}
