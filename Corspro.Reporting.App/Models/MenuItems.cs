using System;

namespace Corspro.Reporting.App.Models
{
    [Serializable]
    public class MenuItems
    {
        public string id { get; set; }

        public string parentid { get; set; }

        public string text { get; set; }

        public string subMenuWidth { get; set; }

    }
}