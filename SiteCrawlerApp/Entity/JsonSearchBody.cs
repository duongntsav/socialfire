using System;
using System.Collections.Generic;
using System.Text;

namespace SiteCrawlerApp.Entity
{
    public class JsonSearchBody
    {
        
        public int total { get; set; }
        public int count { get; set; }
        public List<JsonHitsBody> hits { get; set; }
    }
}
