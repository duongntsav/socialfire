using System;
using System.Collections.Generic;
using System.Text;

namespace SiteCrawlerApp.Entity
{
    public class JsonSearchMessage
    {

        public int code { get; set; }
        public string message { get; set; }
        public JsonSearchBody data { get; set; }

    }
}
