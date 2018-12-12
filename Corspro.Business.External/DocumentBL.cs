namespace Corspro.Business.External
{
    using Corspro.Data.External;
    using Corspro.Domain.Dto;
    using System;

    public class DocumentBL
    {
        public void Add(DocumentDto documentDto)
        {
            try
            {
                new DocumentDL().Add(documentDto);
            }
            catch (Exception exception1)
            {
                throw new Exception(exception1.Message);
            }
        }
    }
}

