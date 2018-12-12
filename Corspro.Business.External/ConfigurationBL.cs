using Corspro.Data.External;
using Corspro.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corspro.Business.External
{
    public class ConfigurationBL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<ConfigurationDto> GetConfigurationListByName(string name)
        {
            var configurationDL = new ConfigurationDL();
            return configurationDL.GetConfigurationListByName(name);
        }
    }
}
