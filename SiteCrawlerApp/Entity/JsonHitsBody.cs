using System;
using System.Collections.Generic;
using System.Text;

namespace SiteCrawlerApp.Entity
{
    public class JsonHitsBody
    {
        // public string topicNames { get; set; }
        public string index_name { get; set; }
        public string type_name { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string domain { get; set; }
        public string source_id { get; set; }
        public string source_id_v1 { get; set; }
        public string published_time { get; set; }
        public string created_time { get; set; }
        public string last_updated_time { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string content { get; set; }
        public List<string> image_sources { get; set; }
        public int similar_master { get; set; }
        public string similar_group_id { get; set; }
        public int spam_level { get; set; }
        public int real_sentiment { get; set; }
        public bool is_pin { get; set; }
        public int sentiment { get; set; }
        public int hue_sentiment { get; set; }
        public int sentiment_score { get; set; }
        public int page_category_id { get; set; }
        public int time_type { get; set; }
        // public string users_processed { get; set; }
        public int similar { get; set; }
        public int removed_by_host { get; set; }
        public int version { get; set; }
        public int changed_count { get; set; }
        public double different_percent { get; set; }
        public int mic_threshold { get; set; }
        public string article_type { get; set; }
        public int article_privacy { get; set; }
        public string post_id { get; set; }
        public int like_count { get; set; }
        public int unlike_count { get; set; }
        public int share_count { get; set; }
        public int comment_count { get; set; }
        public int reply_count { get; set; }
        public int view_count { get; set; }
        public string author_avatar { get; set; }
        public string author_display_name { get; set; }
        public string wall_display_name { get; set; }
        public int author_year_of_birth { get; set; }
        public int author_gender { get; set; }
        public int newspaper_page_index { get; set; }
        public int newspaper_page_count { get; set; }
        public List<string> tags { get; set; }

        public List<long> matched_topics { get; set; }
        
    }
}
