using Corspro.Data.External;
using Corspro.Domain.Dto;
using System;

namespace Corspro.Business.External
{
    public class AppVersionBL
    {
        /// <summary>
        /// Gets the latest software version.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public string GetLatestSWVersion(string applicationName)
        {
            var versioningDl = new AppVersionDL();
            return versioningDl.GetLatestSWVersion(applicationName);
        }
    }
}
