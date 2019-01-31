using Corspro.Data.External;
using Corspro.Domain.Dto;
using System;

namespace Corspro.Business.External
{
    public class ApplicationLogBL
    {
        public void AddErrorLogMessage(int userId, int clientId, string errorMessage) 
        {
            new ApplicationLogDL().AddErrorLogMessage(userId, clientId, errorMessage);
        }
    }
}
