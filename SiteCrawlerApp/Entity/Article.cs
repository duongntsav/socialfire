using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawlerApp.Entity
{
    [Table("article")]
    public class Article
    {
        [Key]
        public long? No { get; set; }
        public string? ResourceType { get; set; }
        public string? Url { get; set; }
        public string? Emotion { get; set; }
        public string? Title { get; set; }
        public string? Domain { get; set; }
        public string? Time { get; set; }
        public DateTime? Date { get; set; }
        public string? SubjectName { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public int? Process { get; set; }

        /// <summary>
        /// For
        /// </summary>
        public int SiteId { get; set; }
        public int SubjectId { get; set; }

        public string ContentHtml { get; set; }
        public DateTime? Updated { get; set; }

        public override string ToString()
        {
            // return $@"Url: {Url.Length}, Title:{Title.Length}, SubjectName:{SubjectName.Length}, Image:" + (Image != null ? Image.Length : 0) + ", Description:" + (Description != null? Description.Length: 0)+ $", Content:{Content.Length}, ContentHtml:" + (ContentHtml != null ? ContentHtml.Length : 0);
            return $@"{Url}, {Title.Length}, {SubjectName.Length}, {Image}, {Description} ";
            // +"INSERT INTO  [dbo].[article] VALUES ('{ResourceType}', '{Url}', '{Emotion}', '{Title}', '{Domain}', '{Time}', '" + Date?.ToString("yyyy-MM-dd") + @$"', '{SubjectName}', '{Content}', '{Image}', '{Description}', {Process}, {SiteId}, {SubjectId}, '{ContentHtml}', '" + Updated?.ToString("yyyy-MM-dd") + ")'";
        }
        
















    }
}
