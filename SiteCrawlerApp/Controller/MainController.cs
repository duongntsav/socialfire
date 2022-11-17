using HtmlAgilityPack;
using log4net;
using Microsoft.EntityFrameworkCore;
using SiteCrawlerApp.Common;
using SiteCrawlerApp.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;

namespace SiteCrawlerApp.Controller
{
    public class MainController
    {
        // Define a static logger variable so that it references the Logger instance named "MyApp".
        private static readonly ILog log = LogManager.GetLogger(typeof(MainController));
        
        // Singleton
        private static MainController instance = null;
        public static MainController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainController();
                }
                return instance;
            }
        }

        // protected MySqlConnection connection;
        SqlConnection connection;
        public DataContext DataContext { get; set; }
        public IMainInterface MainInterface { get; set; }
        public bool IsThreading { get; set; }
        public bool ApiRunning { get; set; }
        public bool IsProcessing { get; set; }
        public string TrackingFolder { get; set; }

        public List<CodeList> codeLists { get; set; }
        public List<Site> sites { get; set; }
        public List<Subject> subjects { get; set; }
        public List<Article> articles { get; set; }
        
        public List<Article> processedArticles { get; set; }
        // message to show



        public int excelDataRow { get; set; }
        public int processingRow { get; set; }
        public int duplicationRow { get; set; }
        public int CurrentRow { get; set; }
        public string processMessage { get; set; }

        public MainController()
        {
        }

        public MainController(IMainInterface iMainInterface)
        {
            MainInterface = iMainInterface;
        }

        public void OpenConnection()
        {
            try
            {
                // For mariadb only
                // string connectionString = "Server=10.62.129.107;Database=socialfire2;Uid=social;Pwd=ktnn@secret;";
                string connectionString = ConfigurationManager.ConnectionStrings["SQLServerConnection"].ConnectionString;
                log.Info($"Connection: {connectionString}");

                // check if not open                
                if(connection == null || connection.State.ToString() != "Open")
                {
                    /**
                     * Use MySqlConnection
                    connection = new MySqlConnection(connectionString);
                    connection.Open();
                    log.Info("Open connect to MariaDb");
                     */

                    connection = new SqlConnection(connectionString);
                    connection.Open();
                    log.Info("Open connect to Sqlserver");
                }
            }
            catch (Exception ex)
            {
                log.Debug("OpenConnection method - Exception: " + ex.ToString());
            }
        }

        public void loadData()
        {
            try
            {
                // Use for Mariadb
                // DbConnection that is already opened
                // dbcontext = new DataContext(connection, false);

                DataContext = new DataContext();

                // Passing an existing transaction to the context
                // context.Database.UseTransaction(transaction);
                {
                    sites = DataContext.SiteSet.OrderBy(s => s.Domain).ToList<Site>();
                    subjects = DataContext.SubjectSet.ToList<Subject>();
                    codeLists = DataContext.CodeListSet.ToList<CodeList>();

                    // this.Articles = dbcontext.CrawArticleSet.ToList<Article>();
                    // this.Articles = dbcontext.CrawArticleSet.Where(a=> a.Process ==0).ToList<Article>();
                    // this.Articles = dbcontext.ArticlesSet.ToList<Articles>();
                    // this.Sites = dbcontext.SitesSet.ToList<Sites>();

                    // make default
                    // will return all customers.
                    foreach (var data in sites)
                    {
                        log.Info($"Site ({data.SiteName}, {data.ImageSignal}, {data.DescriptionSignal})");
                    }

                    foreach (var data in subjects)
                    {
                        log.Info($"Site ({data.SubjectId}, {data.Title})");
                    }

                    foreach (var data in codeLists)
                    {
                        log.Info($"Codelist ({data.CodeGroup}, {data.Code}, {data.Name}, {data.Value})");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Debug("loadData method - Exception: " + ex.ToString());
            }
        }


        public void loadCodeList()
        {
            try
            {
                // Passing an existing transaction to the context
                // context.Database.UseTransaction(transaction);
                {
                    // will return all customers.
                    foreach (var data in codeLists)
                    {
                        log.Info($"Codelist ({data.CodeGroup}, {data.Code}, {data.Name})");
                    }

                }

            }
            catch (Exception ex)
            {
                log.Debug("loadCodeList method - Exception: " + ex.ToString());
            }
        }

        public void resetProcessData()
        {
            excelDataRow = 0;
            processingRow = 0;
            duplicationRow = 0;
            CurrentRow = 0;
            processMessage = "<blank>";
            // set for garbage collection
            processedArticles = null;
            processedArticles = new List<Article>();
        }



        public void passExcelData(List<List<string>> dataList)
        {
            IsProcessing = true;

            string NoTitle = string.Empty;
            string ResourceTypeTitle = string.Empty;
            string UrlTitle = string.Empty;
            string EmotionTitle = string.Empty;
            string TitleTitle = string.Empty;
            string SiteTitle = string.Empty;
            string TimeTitle = string.Empty;
            string DateTitle = string.Empty;
            string SubjectTitle = string.Empty;
            string ContentTitle = string.Empty;


            int StartPosition = 0;
            int ColumLength = 0;

            int NoPosition = 0;
            int ResourceTypePosition = 0;
            int UrlPosition = 0;
            int EmotionPosition = 0;
            int TitlePosition = 0;
            int SiteNamePosition = 0;
            int TimePosition = 0;
            int DatePosition = 0;
            int SubjectNamePosition = 0;
            int ContentPosition = 0;

            // 1. Dynamic get position by tile in excel file. Title in codelist
            // 1.1. Load  from code list
            List<CodeList> headerCodeGroupList = codeLists.Where(a => a.CodeGroup == Constants.CODEGROUP_EXCEL_HEADER).ToList<CodeList>();
            foreach (var data in headerCodeGroupList)
            {
                // start position and min length
                if (data.Code == Constants.CODE_STARTPOSITION)
                    StartPosition = Convert.ToInt32(data.Value);
                if (data.Code == Constants.CODE_COLUMNLENGTH)
                    ColumLength = Convert.ToInt32(data.Value);
                // Get tile name form codelist
                if (data.Code == Constants.CODE_NO)
                    NoTitle = data.Name;
                if (data.Code == Constants.CODE_RESOURCETYPE)
                    ResourceTypeTitle = data.Name;
                if (data.Code == Constants.CODE_URL)
                    UrlTitle = data.Name;
                if (data.Code == Constants.CODE_EMOTION)
                    EmotionTitle = data.Name;
                if (data.Code == Constants.CODE_TITLE)
                    TitleTitle = data.Name;
                if (data.Code == Constants.CODE_SITENAME)
                    SiteTitle = data.Name;
                if (data.Code == Constants.CODE_TIME)
                    TimeTitle = data.Name;
                if (data.Code == Constants.CODE_DATE)
                    DateTitle = data.Name;
                if (data.Code == Constants.CODE_SUBJECTNAME)
                    SubjectTitle = data.Name;
                if (data.Code == Constants.CODE_CONTENT)
                    ContentTitle = data.Name;
            }


            // 1.2. Thứ tự các cột thường xuyên thay đổi, vậy cấu hình cần thay đổi theo 
            // Vì thế Tìm tự động các cột sẽ tốt hơn, Dựa vào tên tiêu đề
            var headerList = dataList[StartPosition - 2];
            if (headerList.Count >= ColumLength)
            {
                for (int i = 0; i < headerList.Count; i++)
                {
                    if (headerList[i] == null)
                        continue;

                    if (headerList[i].Trim() == NoTitle)
                        NoPosition = i;
                    if (headerList[i].Trim() == ResourceTypeTitle)
                        ResourceTypePosition = i;
                    if (headerList[i].Trim() == UrlTitle)
                        UrlPosition = i;
                    if (headerList[i].Trim() == EmotionTitle)
                        EmotionPosition = i;
                    if (headerList[i].Trim() == TitleTitle)
                        TitlePosition = i;
                    if (headerList[i].Trim() == SiteTitle)
                        SiteNamePosition = i;
                    if (headerList[i].Trim() == TimeTitle)
                        TimePosition = i;
                    if (headerList[i].Trim() == DateTitle)
                        DatePosition = i;
                    if (headerList[i].Trim() == SubjectTitle)
                        SubjectNamePosition = i;
                    if (headerList[i].Trim() == ContentTitle)
                        ContentPosition = i;
                }
            }

            // 3. Parse excel data
            processedArticles = new List<Article>();
            // Article
            if (dataList.Count <= StartPosition)
            {
                log.Info("Excel content is not correct");
            }

            for (int i = StartPosition-1; i< dataList.Count; i++ )
            {
                var rowList = dataList[i];
                if (rowList.Count >= ColumLength)
                {
                    Article article = new Article();
                    article.No = Convert.ToInt32(rowList[NoPosition]);
                    article.ResourceType = rowList[ResourceTypePosition];
                    article.Url = rowList[UrlPosition];
                    article.Emotion = rowList[EmotionPosition];
                    article.Title = rowList[TitlePosition];
                    article.Domain = rowList[SiteNamePosition];
                    article.Time = rowList[TimePosition];
                    article.SubjectName = rowList[SubjectNamePosition];
                    article.Content = rowList[ContentPosition];

                    try
                    {
                        DateTime oDate = DateTime.ParseExact(rowList[DatePosition] + " " + rowList[TimePosition], "yyyy-MM-dd HH:mm:ss", null);
                        article.Date = oDate;
                    }
                    catch(Exception e)
                    {
                        DateTime oDate = DateTime.ParseExact(rowList[DatePosition], "yyyy-MM-dd", null);
                        article.Date = oDate;
                    }

                    string host = article.Domain.ToLower();
                    // add siteid and subjectid
                    var siteTemp = sites.Where(s => s.Domain.ToLower().Equals(host));

                    // find exist subject
                    var subjectTemp = subjects.Where(s => (article.SubjectName.Contains( s.Title)));
                    if ( siteTemp != null && siteTemp.Count() > 0)
                    {
                        article.Process = Constants.ARTICLE_STATUS_NONE;
                        article.SiteId = siteTemp.First<Site>().SiteId;
                        if( subjectTemp != null && subjectTemp.Count() > 0)
                        {
                            article.SubjectId = subjectTemp.First<Subject>().SubjectId;
                        }

                        processedArticles.Add(article);
                    }
                }
            }

            excelDataRow = dataList.Count;
            IsProcessing = false;
        }


        public void checkDataExist()
        {
            try
            {
                // 1. To process duplicated news

                // 1.1 Make title list
                List<string> titleLst = new List<string>();
                foreach (var article in processedArticles)
                    titleLst.Add(article.Title.Trim());

                // 1.2 Get duplicate item
                // var distinctList = titleLst.Distinct().ToList();
                var duplicateList = titleLst.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                List<bool> checkLst = new List<bool>();
                // Add signal ussing
                for (int i = 0; i < duplicateList.Count; i++)
                    checkLst.Add(false);

                // 1.3 Finding duplicated item, to keep first one and remove next
                for (int i = processedArticles.Count()-1; i >=0 ; i--)
                {
                    var article = processedArticles.ElementAt(i);

                    int index = duplicateList.FindIndex(str => str.Equals( article.Title.Trim()));
                    // If duplication is first find
                    if (index >= 0)
                    {
                        // Title finding is checked using
                        if (checkLst.ElementAt(index) == false)
                            checkLst[index] = true;
                        // Next finding in removed to avoid duplicated
                        else
                            processedArticles.RemoveAt(i);
                    }
                }

                // will return all customers.
                foreach (var article in processedArticles)
                {
                    IEnumerable<Article> articleTemp = null;
                    if(article.Url != null)
                    // articleTemp = articles.Where(s => s.url.Equals(article.url));
                    articleTemp = DataContext.ArticleSet.Where(a => a.Url.Equals(article.Url));
                    if (articleTemp != null && articleTemp.Count() > 0)
                    {
                        article.Process = Constants.ARTICLE_STATUS_EXISTED;
                        continue;
                    }

                    articleTemp = DataContext.ArticleSet.Where(a => a.Title.Equals(article.Title.Trim()));
                    if (articleTemp != null && articleTemp.Count() > 0)
                    {
                        article.Process = Constants.ARTICLE_STATUS_EXISTED;
                    }
                }

                duplicationRow = duplicateList.Count;
                processingRow = processedArticles.Count;

                // for checking
                log.Debug("STARTING TO SHOW DUPLICATED ROW");
                foreach (var article in duplicateList)
                    log.Info(article);
                log.Debug("ENDING TO SHOW DUPLICATED ROW");

                log.Debug("STARTING TO SHOW PROCESS ROW");
                foreach (var article in processedArticles)
                    log.Info(article);
                log.Debug("ENDING TO SHOW PROCESS ROW");
            }
            catch (Exception ex)
            {
                log.Debug("checkDataExist method - Exception: " + ex.ToString());
            }
        }

        public void crawlerData()
        {
            log.Info($"Start crawlerData method");
            IsProcessing = true;
            int slowLoading = 5; // 5s
            CurrentRow = 0;
            try
            {
                HtmlAgilityPack.HtmlDocument document = null;
                string imageUrl = string.Empty;
                string description = string.Empty;
                string content = string.Empty;

                DateTime preDateTime = DateTime.Now;
                DateTime curDateTime = DateTime.Now;

                // Update all site have slow-loading before -> active. When in case of crawling set status again
                var slowLoadingSite = sites.Where(s => s.Status.Equals(Constants.SITE_STATUS_SLOWLOADING));
                foreach (var item in slowLoadingSite)
                {
                    item.Status = Constants.SITE_STATUS_ACTIVE;
                    DataContext.Entry(item).State = EntityState.Modified;
                    DataContext.SaveChanges();
                }
                // load site again
                sites = DataContext.SiteSet.ToList<Site>();
                /////////
                foreach (var article in processedArticles)
                {
                    // Set here because in case of error to continue to crawl next one
                    try
                    {

                        CurrentRow++;
                        if (article.Process == null || article.Process == Constants.ARTICLE_STATUS_NONE)
                        {
                            var siteTemp = sites.Where(s => s.SiteId.Equals(article.SiteId) && s.Status == Constants.SITE_STATUS_ACTIVE);
                            if (siteTemp == null || siteTemp.Count() == 0)
                                continue;


                            log.Info($"Crawl data row {CurrentRow}, url: {article.Url}");
                            var site = siteTemp.First<Site>();
                            {
                                DateTime dateTime1 = DateTime.Now;
                                // Get docment one time
                                document = getDocumentData(article.Url);
                                if (document == null)
                                    continue;

                                curDateTime = DateTime.Now;

                                // Web load chậm hơn 5s
                                if (curDateTime > dateTime1.AddSeconds(slowLoading))
                                {
                                    site.Note = "Trang web load rất chậm";
                                    site.Status = Constants.SITE_STATUS_SLOWLOADING;
                                    DataContext.Entry(site).State = EntityState.Modified;
                                    DataContext.SaveChanges();
                                    continue;
                                }

                                imageUrl = crawlImage(article, site, document);
                                description = crawlDescription(article, site, document);


                                // After crawler to remove redudance text have same text is tile, description
                                List<string> textList = new List<string>(description.Trim().Split(Constants.JOINING_CHAR));
                                textList.Add(article.Title.Trim());

                                content = crawlerContent(article, site, document, textList);


                                
                                // content = HtmlUtils.RemoveRedundanceContent(content, textList);


                                if (Constants.CONTENTTYPE_UTF8.Equals(site.ContentType))
                                {
                                    imageUrl = Encoding.UTF8.GetString(imageUrl.Select(c => (byte)c).ToArray());
                                    description = Encoding.UTF8.GetString(description.Select(c => (byte)c).ToArray());
                                    content = Encoding.UTF8.GetString(content.Select(c => (byte)c).ToArray());
                                }
                                else if (Constants.CONTENTTYPE_HTMLENCODE.Equals(site.ContentType))
                                {
                                    imageUrl = HttpUtility.HtmlDecode(imageUrl);
                                    description = HttpUtility.HtmlDecode(description);
                                    content = HttpUtility.HtmlDecode(content);
                                }

                                // Format of image No 1. https://
                                // No 2. /path
                                // No 3. //cdn.baothanhhoa.vn/path
                                var ImagePath = string.IsNullOrEmpty(site.ImageServerPath) ? site.SiteUrl : site.ImageServerPath;
                                if ((!string.IsNullOrEmpty(imageUrl)) && (!imageUrl.ToLower().StartsWith("http")))
                                {
                                    // If imageUrl have domain, domain mean: .vn, .com, .org, .net
                                    // check style No 2
                                    if (imageUrl.Contains(".vn") || imageUrl.Contains(".com") || imageUrl.Contains(".org") || imageUrl.Contains(".net"))
                                    {
                                        if (ImagePath.StartsWith("https"))
                                            imageUrl = "https:" + imageUrl;
                                        else
                                            imageUrl = "http:" + imageUrl;
                                    }
                                    else
                                        imageUrl = ImagePath.Trim() + imageUrl;
                                }

                                // remove redudance path
                                article.Image = imageUrl;
                                article.Description = description;
                                article.ContentHtml = content;

                                // update site can not crawler
                                if (string.IsNullOrEmpty(imageUrl) && string.IsNullOrEmpty(description))
                                {
                                    site.CrawlerNote = "Dữ liệu lấy về empty Kiểm tra lại thông tin site này";
                                    site.Updated = DateTime.Now;
                                    DataContext.Entry(site).State = EntityState.Modified;

                                    // DataContext.SaveChanges();
                                }


                                if ((!string.IsNullOrEmpty(imageUrl)) || (!string.IsNullOrEmpty(description)))
                                    article.Process = Constants.ARTICLE_STATUS_PROCESSED;

                                log.Info("Data get: " + "\n" + imageUrl + "\n" + description);
                                // if(CurrentRow % recordShowdata == 0 || CurrentRow == processedArticles.Count)
                                curDateTime = DateTime.Now;
                                if (curDateTime > preDateTime.AddSeconds(10))
                                {
                                    preDateTime = curDateTime;
                                    MainInterface.showStatus();
                                }
                                // MessageBox.Show("Dữ liệu được lấy về: " + "\n" + imageUrl + "\n" + description + "\n" + Article.Url, "Title");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Info("crawlerData method - error process crawling " + ex.ToString() + "\n");
                    }
                }
                // Now set here
                DataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Info("crawlerData method - error " + ex.ToString() + "\n");
            }

            IsProcessing = false;
            log.Info($"End crawlerData method");
        }


        public void crawlerContentData()
        {
            log.Info($"Start crawlerContentData method");
            IsProcessing = true;
            int recordShowdata = 200;
            CurrentRow = 0;
            try
            {
                HtmlAgilityPack.HtmlDocument document = null;
                foreach (var article in processedArticles)
                {
                    CurrentRow++;
                    // 
                    if (article.Process != Constants.ARTICLE_STATUS_NONE)
                        continue;
                    
                    var siteTemp = sites.Where(s => s.SiteId.Equals(article.SiteId));
                    // No site exist continue
                    if (siteTemp == null && siteTemp.Count<Site>() == 0)
                        continue;

                    log.Info($"Crawl data row {CurrentRow}, url: {article.Url}");
                    Site site = siteTemp.First();

                    // status is not ready and ContentSignal is not exist
                    if (site.Status != Constants.SITE_STATUS_ACTIVE)
                        continue;

                    {
                        // Get docment one time
                        document = getDocumentData(article.Url);
                        if (document == null)
                            continue;

                        if(string.IsNullOrEmpty(article.Image))
                        {
                            var image = crawlImage(article, site, document);
                            // blank is not problem in this case
                            article.Image = image;
                        }

                        // Descrition is not correct provided by Reputa api
                        var description = crawlDescription(article, site, document);

                        // After crawler to remove redudance text have same text is tile, description
                        List<string> textList = new List<string>(description.Split(Constants.JOINING_CHAR));
                        textList.Add("");

                        var contentHtml = crawlerContent(article, site, document, textList);

                        if (Constants.CONTENTTYPE_UTF8.Equals(site.ContentType))
                        {
                            description = Encoding.UTF8.GetString(description.Select(c => (byte)c).ToArray());
                            contentHtml = Encoding.UTF8.GetString(contentHtml.Select(c => (byte)c).ToArray());
                        }
                        else if (Constants.CONTENTTYPE_HTMLENCODE.Equals(site.ContentType))
                        {
                            description = HttpUtility.HtmlDecode(description);
                            contentHtml = HttpUtility.HtmlDecode(contentHtml);
                        }

                        // When get api, description and content is empty
                        // update site can not crawler
                        if (string.IsNullOrEmpty(description))
                        {
                            site.CrawlerNote = "Dữ liệu lấy về empty Kiểm tra lại thông tin site này";
                            DataContext.Entry(site).State = EntityState.Modified;
                            // DataContext.SiteSet.AnyAsync(s => s.SiteId == site.SiteId);

                            DataContext.SaveChanges();
                        }

                        article.Description = description;
                        article.ContentHtml = contentHtml;
                        article.Process = Constants.ARTICLE_STATUS_PROCESSED;

                        log.Info($"Content get url {article.Url}: \n {contentHtml}");
                        if (CurrentRow % recordShowdata == 0 || (CurrentRow == processedArticles.Count))
                        {
                            MainInterface.showStatus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info("crawlerContentData method - Exception: " + ex.ToString() + "\n");
            }

            IsProcessing = false;
            log.Info($"End crawlerContentData method");
        }

        public void saveData()
        {
            log.Info("Starting SaveData method");
            IsProcessing = true;
            Article articleTmp = null;

            foreach (var article in processedArticles)
            {
                log.Info($"Save article info: {article.Url}");
                if (article.Process == Constants.ARTICLE_STATUS_PROCESSED)
                {
                    articleTmp = article;
                    // var result = DataContext.ArticleSet.SingleOrDefault(b => b.Url == article.Url);
                    // if (result == null)
                    {
                        // If error then next article will be saved
                        // IN some case image encoded too long so i need cut of
                        // and description too

                        if (string.IsNullOrEmpty(articleTmp.ResourceType) && articleTmp.ResourceType.Length > Constants.MAXLENGTH_RESOURCETYPE)
                            articleTmp.ResourceType = articleTmp.ResourceType.Substring(0, Constants.MAXLENGTH_RESOURCETYPE);

                        if (string.IsNullOrEmpty(articleTmp.Url) && articleTmp.Url.Length > Constants.MAXLENGTH_URL)
                                articleTmp.Url = articleTmp.Url.Substring(0, Constants.MAXLENGTH_URL);

                        if (string.IsNullOrEmpty(articleTmp.Emotion) && articleTmp.Emotion.Length > Constants.MAXLENGTH_EMOTION)
                            articleTmp.Emotion = articleTmp.Emotion.Substring(0, Constants.MAXLENGTH_EMOTION);

                        if (string.IsNullOrEmpty(articleTmp.Title) && articleTmp.Title.Length > Constants.MAXLENGTH_TITLE)
                            articleTmp.Title = articleTmp.Title.Substring(0, Constants.MAXLENGTH_TITLE);

                        if (string.IsNullOrEmpty(articleTmp.Domain) && articleTmp.Domain.Length > Constants.MAXLENGTH_DOMAIN)
                            articleTmp.Domain = articleTmp.Domain.Substring(0, Constants.MAXLENGTH_DOMAIN);

                        if (string.IsNullOrEmpty(articleTmp.SubjectName) && articleTmp.SubjectName.Length > Constants.MAXLENGTH_SUBJECT)
                            articleTmp.SubjectName = articleTmp.SubjectName.Substring(0, Constants.MAXLENGTH_SUBJECT);

                        if (string.IsNullOrEmpty(articleTmp.Image) && articleTmp.Image.Length > Constants.MAXLENGTH_IMAGE)
                                articleTmp.Image = articleTmp.Image.Substring(0, Constants.MAXLENGTH_IMAGE);

                            articleTmp.No = null;
                            articleTmp.Updated = DateTime.Now;
                            // DataContext.ArticleSet.AddAsync(articleTmp);
                            DataContext.ArticleSet.Add(articleTmp);
                        DataContext.SaveChanges();
                    }
                }
            }

            // DataContext.SaveChanges();
            // DataContext.SaveChangesAsync();

            IsProcessing = false;
            log.Info("Ending SaveData method");
        }

        public void UpdateSite(Site site)
        {
            try
            {
                var result = DataContext.SiteSet.SingleOrDefault(s => s.SiteId == site.SiteId);
                if (result == null)
                {
                    result.ImageSignal = site.ImageSignal;
                    result.ImageServerPath = site.ImageServerPath;
                    result.DescriptionSignal = site.DescriptionSignal;
                    result.ContentSignal = site.ContentSignal;
                    result.Note = site.Note;
                }

                // DataContext.Entry(site).State = EntityState.Modified;
                // DataContext.SiteSet.AddRange(result);
                DataContext.SiteSet.AnyAsync(s => s.SiteId == site.SiteId);

                DataContext.SaveChangesAsync();

                log.Info("Save site data: " + "\n" + site);
            }
            catch (Exception ex)
            {
                log.Debug($"UpdateSite method - Error: {ex.ToString()}");
            }
        }

        public void UpdateCodeList(CodeList code)
        {
            try
            {
                var result = DataContext.CodeListSet.SingleOrDefault(x => x.CodeGroup == code.CodeGroup && x.Code == code.Code);
                if (result == null)
                {
                    result.Name = code.Name;
                    result.Value = code.Value;
                }

                // DataContext.Entry(result).State = EntityState.Modified;
                DataContext.Entry(code).State = EntityState.Modified;

                DataContext.SaveChanges();

                log.Info("Save data: " + "\n" + code);
            }
            catch (Exception ex)
            {
                log.Debug($"UpdateCodeList method - Error: {ex.ToString()}");
            }
        }

        public CodeList GetCodeList(int codeGroup, int code)
        {
            try
            {
                return DataContext.CodeListSet.SingleOrDefault(x => x.CodeGroup == codeGroup && x.Code == code);
            }
            catch (Exception ex)
            {
                log.Debug($"GetCodeList method - Error: {ex.ToString()}");
            }

            return null;
        }



        public void ToCSV(string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers
            string headerCSV= "id,siteId,slug,url,title,description,time,content,text,thumbnail,createdAt,site,stt";
            string[] headerAry = headerCSV.Split(",");
            for (int i = 0; i < headerAry.Length; i++)
            {
                sw.Write(headerAry[i]);
                if (i < headerAry.Length - 1)
                {
                    sw.Write(",");
                }
            }

            sw.Write(sw.NewLine);
            int No = 0;
            foreach (Article articles in processedArticles)
            {
                No++;
                string id = generateIdCSV();
                string title = Regex.Replace(articles.Title, @"\t|\n|\r", ""); 
                string description = Regex.Replace(articles.Description, @"\t|\n|\r", ""); 
                string content = Regex.Replace(articles.Content, @"\t|\n|\r", " ");
                string text = Regex.Replace(articles.Content, @"\t|\n|\r", " ");

                string line = $"\"{id}\",\"{articles.SiteId}\",\"\",\"{articles.Url}\",\"{title}\",\"{description}\",";
                line = line + $"\"{articles.Time}\",\"{content}\",\"{text}\",\"{articles.Image}\",";
                line = line + $"\"{articles.Date}\",\"\",{No}";

                sw.Write(line);
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        // "11130738-f9ab-4353-ba04-98044702da11"
        private string generateIdCSV()
        {
            string length8 = StringUtils.RandomString(8);
            string length40 = StringUtils.RandomString(4);
            string length41 = StringUtils.RandomString(4);
            string length42 = StringUtils.RandomString(4);
            string length12 = StringUtils.RandomString(12);

            return @$"{length8}-{length40}-{length41}-{length42}-{length12}";
        }

        public void showStatus()
        {
            MainInterface.showStatus();
        }



        #region crawler
        
        /**
         * Get document
         */
        public HtmlAgilityPack.HtmlDocument getDocumentData(string Url, int type = 0)
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = null;
            HtmlWeb web = null;
            try
            {
                switch (type)
                {
                    // standart process
                    case 1:
                        // Change code
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                        request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 70.0.3538.77 Safari / 537.36";
                        request.Accept = "text/html";
                        request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                        //request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                        // request.Headers.Add(HttpRequestHeader.Connection, "keep-alive");

                        var response = (HttpWebResponse)(request.GetResponse());
                        HttpStatusCode code = response.StatusCode;
                        if (code == HttpStatusCode.OK)
                        {
                            // var encoding = ASCIIEncoding.ASCII;
                            var encoding = ASCIIEncoding.UTF8;
                            using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
                            {
                                htmlDoc = new HtmlAgilityPack.HtmlDocument();
                                htmlDoc.Load(sr);
                            }
                        }
                        break;

                    // Rest client
                    case 2:
                        // OLDER CODING
                        web = new HtmlWeb();
                        web.OverrideEncoding = Encoding.UTF8;
                        web.UseCookies = true;
                        web.UsingCache = true;
                        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:106.0) Gecko/20100101 Firefox/106.0";
                        htmlDoc = web.Load(Url);
                        break;

                    // Load url
                    default:
                        var restClient = new RestSharp.RestClient(Url);
                        // client. = -1;
                        // client.Options.FollowRedirects = true;

                        var restRequest = new RestSharp.RestRequest();
                        // request.addHeader("Cookie", "vru_=6Lf6Jl0UAAAAAIx9uhYeUWGHSeoBiCLlhL6xGU1_");

                        RestSharp.RestResponse restResponse = restClient.Execute(restRequest);
                        // response.StatusCode = System.Net.HttpStatusCode.Moved;

                        web = new HtmlWeb();
                        htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        htmlDoc.LoadHtml(restResponse.Content);
                        break;
                }

                return htmlDoc;
            }
            catch (Exception ex)
            {
                log.Debug( @$"getDocumentData method - Error: { ex.ToString()}");
                return htmlDoc;
            }
        }


        /**
         * Get image url
         */
        private string crawlImage(Article Article, Site Site, HtmlAgilityPack.HtmlDocument document)
        {
            string resImage = string.Empty;
            string Url = Article.Url;
            string imageSignal = Site.ImageSignal;

            // Call it for same testing and real get
            resImage = crawlDetailImage(imageSignal, document);

            return resImage;
        }


        private string crawlDescription(Article Article, Site Site, HtmlAgilityPack.HtmlDocument document)
        {
            string resDescription = string.Empty;
            string Url = Article.Url;
            string descriptionSignal = Site.DescriptionSignal;
            string SiteUrl = Site.SiteUrl;

            // Call it for get same way
            resDescription = crawlDetailDescription(descriptionSignal, document);

            return resDescription;
        }

        public string crawlDetailImage(String imageExpression, HtmlAgilityPack.HtmlDocument document)
        {
            string resImage = string.Empty;
            try
            {
                var nodeSet = document.DocumentNode.SelectNodes(imageExpression);
                // var nodeSet = document.DocumentNode.Descendants(0).Where(n => n.HasClass(imageSignal));
                if (nodeSet.Count() == 0)
                {
                    log.Debug($"crawlerImageTesting method- Link :Url can not read image");
                    return resImage;
                }


                // replace code here instead comment below
                var imageNode = nodeSet.First().SelectNodes(".//img");
                if( imageNode != null)
                {
                    // lazy loading data-src
                    var dataSrc = imageNode.First().Attributes["data-src"]?.Value.Trim();
                    if ((dataSrc != null) &&
                        (dataSrc.ToLower().Contains(".png") || dataSrc.ToLower().Contains(".jpg") || dataSrc.ToLower().Contains(".jpeg") || dataSrc.ToLower().Contains(".gif")))
                        return dataSrc;

                    // src
                    resImage = imageNode.First().Attributes["src"]?.Value;
                    if ((resImage != null && !resImage.ToLower().Contains("lazy")) &&
                        (resImage.ToLower().Contains(".png") || resImage.ToLower().Contains(".jpg") || resImage.ToLower().Contains(".jpeg") || resImage.ToLower().Contains(".gif")))
                        return resImage.Trim();

                    // solve lazy load image in other attr
                    var allAtribute = imageNode.First().Attributes;
                    foreach (var item in allAtribute)
                    {
                        if (!item.Name.ToLower().Equals("src"))
                            if (item.Value.ToLower().Contains(".png") || item.Value.ToLower().Contains(".jpg") ||
                            item.Value.ToLower().Contains(".jpeg") || item.Value.ToLower().Contains(".gif"))
                                return item.Value.Trim();
                    }
                }

                //string textNode = imageNode.InnerHtml;
                //// Create a pattern for a word that format src="abcde"
                //string pattern = @"src=[',""]([^'""]+?)[',""]";
                //// Create a Regex
                //Regex rg = new Regex(pattern, RegexOptions.IgnoreCase);
                //MatchCollection matchedAuthors = rg.Matches(textNode);

                //if (matchedAuthors.Count > 0)
                //{
                //    resImage = matchedAuthors[0].Value.Replace("\"", "").Replace("'", "").Replace("src=", "");

                //    log.Info($"Match : " + resImage);
                //    // MessageBox.Show($"Match : " + resImage, "Title");
                //}

                return resImage;
            }
            catch (Exception ex)
            {
                log.Info($"crawlerImageTesting method- Exception : " + ex.ToString());
                return resImage;
            }
        }

        private string crawlerContent(Article Article, Site Site, HtmlAgilityPack.HtmlDocument document, List<string> textList)
        {
            string resContent = string.Empty;
            string Url = Article.Url;
            string contentSignal = Site.ContentSignal;
            string SiteUrl = Site.SiteUrl;

            resContent = crawlDetailContent(contentSignal, document, textList, true);
            return resContent;
        }

        public string crawlDetailDescription(string desExpression, HtmlAgilityPack.HtmlDocument document)
        {
            string resDescription = string.Empty;
            try
            {
                // string textNode = node2.InnerText;
                var nodeSet = document.DocumentNode.SelectNodes(desExpression);
                // var nodeSet = document.DocumentNode.Descendants(0).Where(n => n.HasClass(desExpression));
                if (nodeSet.Count() == 0)
                {
                    log.Debug($"crawlDescriptionTesting method- Link :Url can not read description");
                    return resDescription;
                }

                HtmlNode node = nodeSet.First();
                // upgrade
                string textNode = node.InnerText.Trim();
                // string[] aryString = textNode.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                string[] aryString = textNode.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                textNode = aryString[0];
                if (textNode.Trim().Length <= Constants.MINLENGTH_DESCRIPTION && aryString.Length > 1)
                    textNode = textNode + Constants.JOINING_CHAR + aryString[1];

                resDescription = textNode;

                return resDescription;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString(), "Title");
                log.Info($"crawlDescriptionTesting method- Exception : " + ex.ToString());
                return resDescription;
            }
        }

        public string crawlDetailContent(string contentExpression, HtmlAgilityPack.HtmlDocument document , List<string> textList, bool cropable)
        {
            string resContent = string.Empty;
            try
            {
                // string textNode = node2.InnerText;
                var nodeSet = document.DocumentNode.SelectNodes(contentExpression);
                // var nodeSet = document.DocumentNode.Descendants(0).Where(n => n.HasClass(desExpression));
                if (nodeSet.Count() == 0)
                {
                    log.Debug($"Link :Url can not read content");
                    return resContent;
                }

                // to remove tag to do here, not working in RemoveRedundanceContent
                HtmlNode node = nodeSet.First();
                node.SelectNodes("//style|//script|//iframe|//figure|//input").ToList().ForEach(n => n.Remove());

                // remove unused tag, image is not formal
                resContent = HtmlUtils.RemoveRedundanceContent(node, textList);

                return resContent;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString(), "Title");
                log.Info($"crawlDetailContent method- Exception : " + ex.ToString());
                return resContent;
            }
        }

        #endregion crawler

        #region thereading

        public static void RunTask(int i)
        {
            
            
            
        }



        #endregion

    }
}
