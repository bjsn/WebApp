namespace Corspro.Domain.Dto
{
    using System;
    using System.Runtime.CompilerServices;

    public class DocumentDto
    {
        public int ClientID { get; set; }

        public string QuoteID { get; set; }

        public string DocumentType { get; set; }

        public string FilePlatformID { get; set; }

        public string LastFileSaveLocation { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int LastUpdatedBy { get; set; }

        public DateTime LastUpdatedDT { get; set; }
    }
}

