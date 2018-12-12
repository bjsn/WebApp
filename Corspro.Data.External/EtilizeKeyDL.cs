using Corspro.Domain.External;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corspro.Data.External
{
    public class EtilizeKeyDL
    {
        public List<string> GetLastRetrievedKeys()
        {
            List<string> list = new List<string>();
            try
            {
                using (SDACloudEntities sDACloudEntities = new SDACloudEntities())
                {
                    using (sDACloudEntities)
                    {
                        IOrderedQueryable<EtilizeKey> source = from x in sDACloudEntities.EtilizeKeys
                                                               orderby x.LastRetrievedDT
                                                               select x;
                        EtilizeKey etilizeKey = source.FirstOrDefault<EtilizeKey>();
                        etilizeKey.LastRetrievedDT = DateTime.UtcNow;
                        sDACloudEntities.SaveChanges();
                        if (etilizeKey != null)
                        {
                            list.Add(etilizeKey.EtilizeKeyID.ToString());
                            list.Add(etilizeKey.EtilizeSiteID.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string item = string.Concat(new object[]
                {
                    "Error: ",
                    ex.Message,
                    " ",
                    ex.InnerException
                });
                list.Add(item);
            }
            return list;
        }

        public void UpdateKeyValue(string EtilizeKeyName, int key)
        {
        }
    }
}
