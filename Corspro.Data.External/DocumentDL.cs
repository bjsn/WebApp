using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System;
using System.Linq;
using System.Transactions;

namespace Corspro.Data.External
{
    public class DocumentDL
    {
        public void Add(DocumentDto documentDto)
        {
            try
            {
                using (SDACloudEntities sDACloudEntities = new SDACloudEntities())
                {
                    using (sDACloudEntities)
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            Document document = sDACloudEntities.Documents.FirstOrDefault((Document i) => i.ClientID == (int?)documentDto.ClientID && i.LastFileSaveLocation.Equals(documentDto.LastFileSaveLocation));
                            if (document != null)
                            {
                                document.DocumentType = documentDto.DocumentType;
                                document.FilePlatformID = documentDto.FilePlatformID;
                                document.LastFileSaveLocation = documentDto.LastFileSaveLocation;
                                document.LastUpdatedBy = new int?(documentDto.CreatedBy);
                                document.LastUpdatedDT = new DateTime?(DateTime.Now);
                                sDACloudEntities.SaveChanges();
                                transactionScope.Complete();
                            }
                            else
                            {
                                Document entity = new Document
                                {
                                    ClientID = new int?(documentDto.ClientID),
                                    QuoteID = documentDto.QuoteID,
                                    DocumentType = documentDto.DocumentType,
                                    FilePlatformID = documentDto.FilePlatformID,
                                    LastFileSaveLocation = documentDto.LastFileSaveLocation,
                                    CreatedBy = new int?(documentDto.CreatedBy),
                                    CreatedDT = new DateTime?(DateTime.Now)
                                };
                                sDACloudEntities.Documents.AddObject(entity);
                                sDACloudEntities.SaveChanges();
                                transactionScope.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(DocumentDto documentDto)
        {
            try
            {
                using (SDACloudEntities sDACloudEntities = new SDACloudEntities())
                {
                    using (sDACloudEntities)
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            Document document = sDACloudEntities.Documents.FirstOrDefault((Document i) => i.ClientID == (int?)documentDto.ClientID && i.LastFileSaveLocation.Equals(documentDto.LastFileSaveLocation));
                            document.DocumentType = documentDto.DocumentType;
                            document.FilePlatformID = documentDto.FilePlatformID;
                            document.LastFileSaveLocation = documentDto.LastFileSaveLocation;
                            document.LastUpdatedBy = new int?(documentDto.CreatedBy);
                            document.LastUpdatedDT = new DateTime?(DateTime.Now);
                            sDACloudEntities.SaveChanges();
                            transactionScope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
