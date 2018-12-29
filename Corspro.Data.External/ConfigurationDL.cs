using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System.Collections.Generic;
using System.Linq;

namespace Corspro.Data.External
{
    public class ConfigurationDL
    {
        private string[] AWSConfig = new string[] { "AWSAccessKey", "AWSSecretKey", "BuketName", "RegionName" };
        /// <summary>
        /// Gets the latest software version.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public List<ConfigurationDto> GetConfigurationListByName(string name)
        {
            List<ConfigurationDto> ConfigurationList = null;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    string[] configurationListNames = GetConfigurationListSelected(name);
                    if (configurationListNames != null)
                    {
                        var configurationListDL = sdaCloudEntities.Configurations.Where(i => configurationListNames.Contains(i.Name)).ToList();
                        Mapper.CreateMap<Configuration, ConfigurationDto>();

                        if (configurationListDL.Count > 0)
                        {
                            ConfigurationList = Mapper.Map<List<Configuration>, List<ConfigurationDto>>(configurationListDL);
                            //Mapper.AssertConfigurationIsValid();
                        }
                    }
                    else 
                    {
                        ConfigurationList = new List<ConfigurationDto>();
                        var configurationListDL = sdaCloudEntities.Configurations.Where(i => i.Name.Equals(name)).ToList();
                        Mapper.CreateMap<Configuration, ConfigurationDto>();

                        if (configurationListDL.Count > 0)
                        {
                            ConfigurationList = Mapper.Map<List<Configuration>, List<ConfigurationDto>>(configurationListDL);
                            //Mapper.AssertConfigurationIsValid();
                        }
                    }
                }
            }
            return ConfigurationList;
        }

        private string[] GetConfigurationListSelected(string name)
        {
            string[] Config = null;
            switch (name)
            {
                case "AWS":
                    Config = AWSConfig;
                    break;
                default:
                    break;
            }
            return Config;
        }
    }
}
