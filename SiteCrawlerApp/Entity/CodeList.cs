using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SiteCrawlerApp.Entity
{

    [Table("codeList")]
    public class CodeList
    {
        [Key]
        public int CodeListID { get; set; }
        public int CodeGroup { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
