using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawlerApp.Entity
{
    [Table("subject")]
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public int Order { get; set; }

        // reputa topicId
        public long TopicId { get; set; }
    }
}
