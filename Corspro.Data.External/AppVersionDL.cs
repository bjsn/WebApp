using System;
using System.Linq;
using System.Transactions;
using Corspro.Domain.External;

namespace Corspro.Data.External
{
    public class AppVersionDL
    {
        /// <summary>
        /// Gets the latest software version.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public string GetLatestSWVersion(string applicationName)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingVersion =
                        sdaCloudEntities.AppVersions.FirstOrDefault(
                            i =>
                            i.ApplicationName.ToUpper().Equals(applicationName));

                    if (existingVersion != null)
                        return existingVersion.Version;

                    return string.Empty;
                }
            }
        }
    }
}
