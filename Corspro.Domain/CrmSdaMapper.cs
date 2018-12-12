namespace Corspro.Domain
{
    /// <summary>
    /// Mapper is a class that helps to map the values on the CRM vs the values on a specific database table
    /// this relation is stablished in the table CRMXref
    /// </summary>
    public class CrmSdaMapper
    {
        /// <summary>
        /// Gets or sets the CRM table.
        /// </summary>
        /// <value>
        /// The CRM table.
        /// </value>
        public string CRMTable { get; set; }

        /// <summary>
        /// Gets or sets the CRM field.
        /// </summary>
        /// <value>
        /// The CRM field.
        /// </value>
        public string CRMField { get; set; }

        /// <summary>
        /// Gets or sets the type of the CRM field.
        /// </summary>
        /// <value>
        /// The type of the CRM field.
        /// </value>
        public string CRMFieldType { get; set; }

        /// <summary>
        /// Gets or sets the SDA table.
        /// </summary>
        /// <value>
        /// The SDA table.
        /// </value>
        public string SdaTable { get; set; }

        /// <summary>
        /// Gets or sets the SDA field.
        /// </summary>
        /// <value>
        /// The SDA field.
        /// </value>
        public string SdaField { get; set; }

        /// <summary>
        /// Gets or sets the type of the SDA field.
        /// </summary>
        /// <value>
        /// The type of the SDA field.
        /// </value>
        public string SdaFieldType { get; set; }
    }
}
