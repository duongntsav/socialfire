using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWeb.Models
{
    [Table("site")]
    public class Site
    {
        [Key]
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public string Domain { get; set; }
        public string? ImageSignal { get; set; }
        public string? ImageServerPath { get; set; }
        public string? DescriptionSignal { get; set; }
        public string? ContentSignal { get; set; }
        public string? ContentType { get; set; }
        public string? Note { get; set; }
        public string? CrawlerNote { get; set; }
        public string? UrlTesting { get; set; }
        public int Status { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
