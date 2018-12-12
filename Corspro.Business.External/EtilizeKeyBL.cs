namespace Corspro.Business.External
{
    using Corspro.Data.External;
    using System;
    using System.Collections.Generic;

    public class EtilizeKeyBL
    {
        public List<string> GetLastRetrievedKeys()
        {
            return new EtilizeKeyDL().GetLastRetrievedKeys();
        }
    }
}

