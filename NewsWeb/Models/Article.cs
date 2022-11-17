using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWeb.Models
{
    [Table("article")]
    public class Article
    {
        [Key]
        public long? No { get; set; }
        public string ResourceType { get; set; }
        public string Url { get; set; }
        public string Emotion { get; set; }
        public string Title { get; set; }
        public string Domain { get; set; }
        public string Time { get; set; }
        public DateTime? Date { get; set; }
        public string SubjectName { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int? Process { get; set; }

        /// <summary>
        /// For
        /// </summary>
        public int SiteId { get; set; }
        public int SubjectId { get; set; }
        public string ContentHtml { get; set; }
    }
}
