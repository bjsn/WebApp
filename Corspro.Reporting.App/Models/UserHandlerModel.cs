using System;

namespace Corspro.Reporting.App.Models
{
    [Serializable]
    public class UserHandlerModel
    {
        public string UserViewId { get; set; }

        public int MgUserId { get; set; }

        public string StrMessage { get; set; }
    }
}