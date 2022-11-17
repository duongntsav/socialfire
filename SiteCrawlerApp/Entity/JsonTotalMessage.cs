using System;
using System.Collections.Generic;
using System.Text;

namespace SiteCrawlerApp.Entity
{
    class JsonTotalMessage
    {

        public int code { get; set; }
        public string message { get; set; }
        public JsonTotalBody data { get; set; }

    }
}
