using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawlerApp.Common
{
    public class Constants
    {
        public static int SITE_STATUS_ACTIVE = 0;
        public static int SITE_STATUS_SLOWLOADING = 1;
        public static int SITE_STATUS_INACTIVE = 2;

        public static int MINLENGTH_DESCRIPTION = 50;
        public static int MAXLENGTH_RESOURCETYPE = 20;
        public static int MAXLENGTH_URL = 500;
        public static int MAXLENGTH_EMOTION = 50;
        public static int MAXLENGTH_TITLE = 500;
        public static int MAXLENGTH_DOMAIN = 50;
        public static int MAXLENGTH_SUBJECT = 500;
        public static int MAXLENGTH_IMAGE = 500;

        // this character need Html special char to slip correct descrition for tream
        public static string JOINING_CHAR = "&#45;"; // Hyphen character
        public static string IMG_SOURCE_DEFAULT = "https://www.sav.gov.vn/Content/publishing/img/body.gif";


        // article status
        public static int CODEGROUP_STATUS_ARTICLE = 11;
        public static int ARTICLE_STATUS_NONE = 1;
        public static int ARTICLE_STATUS_EXISTED = 2;
        public static int ARTICLE_STATUS_PROCESSED = 0;



        ///////////////////////////////////////////////////////////////////////
        // Tracking folder
        public static int CODEGROUP_TRACKING_FOLDER = 9;
        public static int CODE_TRACKING_INPUT = 1;
        public static int CODE_TRACKING_OUTPUT = 2;
        public static int CODE_TRACKING_PERIOD = 3;


        ///////////////////////////////////////////////////////////////////////
        // Excel header
        public static int CODEGROUP_EXCEL_HEADER = 10;

        // Title to know position in reputa excel
        public static string NO_NAME = "STT";
        public static string RESOURCE_NAME = "Nguồn";
        public static string URL_NAME = "URL";
        public static string EMOTION_NAME = "Sắc thái";
        public static string TITLE_NAME = "Tiêu đề";
        public static string SITENAME_NAME = "Tên miền";
        public static string TIME_NAME = "Thời gian";
        public static string DATE_NAME = "Ngày";
        public static string SUBJECT_NAME = "Chủ đề";
        public static string CONTENT_NAME = "Nội dung";

        // Next step get Name form codelist. Configure in codelist no need rebuild constants
        public static int CODE_NO = 1;
        public static int CODE_RESOURCETYPE = 2;
        public static int CODE_URL = 3;
        public static int CODE_EMOTION = 4;
        public static int CODE_TITLE = 5;
        public static int CODE_SITENAME = 6;
        public static int CODE_TIME = 7;
        public static int CODE_DATE = 8;
        public static int CODE_SUBJECTNAME = 9;
        public static int CODE_CONTENT = 10;

        public static int CODE_STARTPOSITION = 20;
        public static int CODE_COLUMNLENGTH = 21;


        ///////////////////////////////////////////////////////////////////////
        // reputa api
        public static int CODEGROUP_REPUTA_API = 12;
        public static int CODE_REPUTA_DOMAIN = 1;
        public static int CODE_REPUTA_SEARCHAPI = 2;

        public static int CODE_REPUTA_AUTHORIZATION = 10;
        public static int CODE_REPUTA_PAYLOAD = 11;

        // Maybe can used in excel reading
        public static int CODE_REPUTA_PERIOD_CALLING = 5;
        public static int CODE_REPUTA_PERIOD_SLEEPING = 6;

        //public static int PERIOD_CALLINGAPI = 1000;
        //public static int THREAD_SLEEPING = 30000;

        ///////////////////////////////////////////////////////////////////////
        // CONTENT TYPE MESSAGE
        public static string CONTENTTYPE_UTF8 = "UTF-8";
        public static string CONTENTTYPE_HTMLENCODE = "HTML";


        // JSON RETURNED RESULT
        public static int CODE_SUCCESS = 0;
        public static string MESSAGE_SUCCESS = "SUCCESS";
    }


    public class JSON
    {
        public static string CODE = "code";
        public static string MESSAGE = "message";
        public static string DATA = "data";
        public static string ISWARNING = "isWarning";
        public static string ISBLOCK = "isBlock";
        public static string DATEBLOCKED = "dateBlocked";
        public static string MONTH_DATA = "month_data";
        public static string AVERAGE = "average";
        public static string MONTH_DATA_PACKAGE = "month_data_package";


        ///////////////////////
        ///
        public static string TOTAL = "total";
        public static string COUNT = "count";
        public static string HITS = "hits";

        public static string TOPICNAMES = "topicnames";
        public static string INDEX_NAME = "index_name";
        public static string TYPE_NAME = "type_name";
        public static string ID = "id";
        public static string URL = "url";
        public static string DOMAIN = "domain";
        public static string SOURCE_ID = "source_id";
        public static string SOURCE_ID_V1 = "source_id_v1";
        public static string PUBLISHED_TIME = "published_time";
        public static string CREATED_TIME = "created_time";
        public static string LAST_UPDATED_TIME = "last_updated_time";
        public static string TITLE = "title";
        public static string SUMMARY = "summary";
        public static string CONTENT = "content";
        public static string SIMILAR_MASTER = "similar_master";
        public static string SIMILAR_GROUP_ID = "similar_group_id";
        public static string SPAM_LEVEL = "spam_level";
        public static string REAL_SENTIMENT = "real_sentiment";
        public static string IS_PIN = "is_pin";
        public static string SENTIMENT = "sentiment";
        public static string HUE_SENTIMENT = "hue_sentiment";
        public static string SENTIMENT_SCORE = "sentiment_score";
        public static string PAGE_CATEGORY_ID = "page_category_id";
        public static string TIME_TYPE = "time_type";
        public static string USERS_PROCESSED = "users_processed";
        public static string SIMILAR = "similar";
        public static string REMOVED_BY_HOST = "removed_by_host";
        public static string VERSION = "version";
        public static string CHANGED_COUNT = "changed_count";
        public static string DIFFERENT_PERCENT = "different_percent";
        public static string MIC_THRESHOLD = "mic_threshold";
        public static string ARTICLE_TYPE = "article_type";
        public static string ARTICLE_PRIVACY = "article_privacy";
        public static string POST_ID = "post_id";
        public static string LIKE_COUNT = "like_count";
        public static string UNLIKE_COUNT = "unlike_count";
        public static string SHARE_COUNT = "share_count";
        public static string COMMENT_COUNT = "comment_count";
        public static string REPLY_COUNT = "reply_count";
        public static string VIEW_COUNT = "view_count";
        public static string AUTHOR_AVATAR = "author_avatar";
        public static string AUTHOR_DISPLAY_NAME = "author_display_name";
        public static string WALL_DISPLAY_NAME = "wall_display_name";
        public static string AUTHOR_YEAR_OF_BIRTH = "author_year_of_birth";
        public static string AUTHOR_GENDER = "author_gender";
        public static string NEWSPAPER_PAGE_INDEX = "newspaper_page_index";
        public static string NEWSPAPER_PAGE_COUNT = "newspaper_page_count";
        public static string TAGS = "tags";
    }





    public interface IMainInterface
    {
        public void showStatus();
    }
}
