using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWeb.Models
{
    [Table("subject")]
    public class Subject
    {
        [Key]
        public int subjectId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string keywords { get; set; }
        public DateTime? created { get; set; }
        public DateTime? updated { get; set; }
        public int order { get; set; }

        // reputa topicId
        public long topicId { get; set; }
    }
}
