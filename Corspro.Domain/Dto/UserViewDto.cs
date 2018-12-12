using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corspro.Domain.Dto
{
    public class UserViewDto
    {
        public int UserViewID { get; set; }

        public int ClientID { get; set; }

        public int UserID { get; set; }

        public string View { get; set; }

        public string ViewName { get; set; }
    }
}
