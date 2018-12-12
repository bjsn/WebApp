using System;
using System.Collections.Generic;
using System.Reflection;
using Corspro.Data.External;
using Corspro.Domain.Dto;

namespace Corspro.Business.External
{
    public class UtilityBL
    {
        #region Utilities
        /// <summary>
        /// Converts the record to string.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public string ConvertRecordToString(Object data)
        {
            Type t = data.GetType().BaseType;


            if (t != null)
            {
                FieldInfo[] fields = t.GetFields(BindingFlags.Public |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Instance);
                String str = string.Empty;
                foreach (FieldInfo f in fields)
                {
                    if (!string.IsNullOrEmpty(str)) str += ",";
                    str += f.Name + " = " + f.GetValue(data);
                }
                return str.Replace("k__BackingField", string.Empty);
            }
            return String.Empty;
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="opportunity">The opportunity object.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="value">The value.</param>
        public void SetProperty(OpportunityDto opportunity, PropertyInfo propertyInfo, object value)
        {
            string name;
            if (propertyInfo.PropertyType.IsGenericType)
            {
                var args = propertyInfo.PropertyType.GetGenericArguments();
                name = args[0].Name;
            }
            else
            {
                name = propertyInfo.PropertyType.Name;
            }

            switch (name)
            {
                case "Int32":
                    propertyInfo.SetValue(opportunity, Convert.ToInt32(value), null);
                    break;
                case "String":
                    propertyInfo.SetValue(opportunity, value.ToString(), null);
                    break;
                case "DateTime":
                    propertyInfo.SetValue(opportunity, Convert.ToDateTime(value), null);
                    break;
                case "Double":
                    propertyInfo.SetValue(opportunity, Convert.ToDouble(value), null);
                    break;
                case "Decimal":
                    propertyInfo.SetValue(opportunity, Convert.ToDecimal(value), null);
                    break;
            }
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="value">The value.</param>
        public void SetProperty(QuoteDto quote, PropertyInfo propertyInfo, object value)
        {
            string name;
            if (propertyInfo.PropertyType.IsGenericType)
            {
                var args = propertyInfo.PropertyType.GetGenericArguments();
                name = args[0].Name;
            }
            else
            {
                name = propertyInfo.PropertyType.Name;
            }

            switch (name)
            {
                case "Int32":
                    propertyInfo.SetValue(quote, Convert.ToInt32(value), null);
                    break;
                case "String":
                    propertyInfo.SetValue(quote, value.ToString(), null);
                    break;
                case "DateTime":
                    propertyInfo.SetValue(quote, Convert.ToDateTime(value), null);
                    break;
                case "Double":
                    propertyInfo.SetValue(quote, Convert.ToDouble(value), null);
                    break;
                case "Decimal":
                    propertyInfo.SetValue(quote, Convert.ToDecimal(value), null);
                    break;
            }
        }

        /// <summary>
        /// Gets the statuses.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<OppStatusDto> GetStatuses(int clientId)
        {
            var opportunityDl = new OpportunityDL();
            return opportunityDl.GetStatuses(clientId);
        }
        #endregion Utilities
    }
}
