using log4net;
using OfficeOpenXml;
using SiteCrawlerApp.Common;
using SiteCrawlerApp.Controller;
using SiteCrawlerApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace SiteCrawlerApp
{
    public partial class frmMain : Form, IMainInterface
    {
        // Define a static logger variable so that it references the Logger instance named "MyApp".
        private static readonly ILog log = LogManager.GetLogger(typeof(frmMain));
        protected string FolderPath { get; set; }
        public bool IsProcessing { get; set; }
        public Thread trackThread { get; set; }
        public Task reputaAPITask { get; set; }

        public MainController MainController { get; set; }


        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Load log4net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));


            // this.WindowState = FormWindowState.Maximized;
            if (MainController == null)
            {
                MainController = MainController.Instance;
                MainController.MainInterface = this;
                MainController.OpenConnection();
                MainController.loadData();
                dataGridSite.DataSource = MainController.sites.ToArray();
                dataGridSubject.DataSource = MainController.subjects.ToArray();
            }

            // load combobox
            cboSite.DisplayMember = "siteName";
            cboSite.ValueMember = "siteId";
            cboSite.DataSource = MainController.sites;
        }


        private void btnReadFile_Click(object sender, EventArgs e)
        {
            if (MainController.IsProcessing)
            {
                MessageBox.Show("Dữ liệu đang được xử lý");
                return;
            }

            lblAppStatus.Text = "Processing!";
            string filePath = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Excel Files",

                CheckFileExists = true,
                CheckPathExists = true,

                //DefaultExt = "xls",
                //Filter = "xlsx files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    try
                    {
                        List<List<string>> dataList = EPPlusUtil.ReadExcelSheet(package, 0);
                        MainController.passExcelData(dataList);
                        MainController.checkDataExist();

                        dataGridArticle.DataSource = MainController.processedArticles.ToArray();
                    }
                    catch (Exception ex)
                    {
                        log.Debug(ex.ToString());
                    }
                }
            }

            lblAppStatus.Text = "Finished!";
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            if (!MainController.IsThreading)
            {
                MainController.IsThreading = true;
                btnTracking.Text = "Stop tracking folder";
                btnTracking.BackColor = Color.Green;

                if(trackThread== null || (!trackThread.IsAlive))
                {
                    trackThread = new Thread(frmMain.trackingDirectory);
                    // Bắt đầu Thread (start thread).
                    trackThread.Start();
                }
            }
            else
            {
                MainController.IsThreading = false;
                btnTracking.Text = "Tracking folder (xlsx file)";
                btnTracking.BackColor = SystemColors.ButtonFace;
            }
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            if (MainController.IsProcessing)
            {
                MessageBox.Show("Dữ liệu đang được xử lý");
                return;
            }

            lblAppStatus.Text = "Processing!";
            MainController.loadData();
            if (MainController.processedArticles != null && MainController.processedArticles.Count > 0)
                dataGridArticle.DataSource = MainController.processedArticles.ToArray();
            dataGridSite.DataSource = MainController.sites.ToArray();
            dataGridSubject.DataSource = MainController.subjects.ToArray();

            lblAppStatus.Text = "Finished!";
        }

        private void btnCrawler_Click(object sender, EventArgs e)
        {
            if (MainController.IsProcessing)
            {
                MessageBox.Show("Dữ liệu đang được xử lý");
                return;
            }

            lblAppStatus.Text = "Processing!";
            MainController.crawlerData();
            dataGridArticle.DataSource = MainController.processedArticles.ToArray();
            lblAppStatus.Text = "Finished!";
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            if (MainController.IsProcessing)
            {
                MessageBox.Show("Dữ liệu đang được xử lý");
                return;
            }

            lblAppStatus.Text = "Processing!";
            MainController.saveData();
            lblAppStatus.Text = "Finished!";
        }

        private void btnCallReputaApi_Click(object sender, EventArgs e)
        {
            if (!MainController.ApiRunning)
            {
                MainController.ApiRunning = true;
                btnCallReputaApi.Text = "Stop calling API";
                btnCallReputaApi.BackColor = Color.Green;

                CodeList DomainCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_DOMAIN);
                CodeList SearchCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_SEARCHAPI);
                CodeList AuthorizationCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_AUTHORIZATION);
                CodeList PayloadCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PAYLOAD);

                txtReputaAuthorization.Text = AuthorizationCodeList.Value;
                txtReputaDomain.Text = DomainCodeList.Value;
                txtReputaSearch.Text = SearchCodeList.Value;

                // Replace page, startdate, enddate. startdate, enddate updated aboard
                string Payload = PayloadCodeList.Value;
                Payload = Payload.Replace(@$"2022/01/01", DateTime.Now.ToString("yyyy/MM/dd"));
                txtReputaPayload.Text = Payload;

                if(reputaAPITask== null || reputaAPITask.IsCompleted)
                {
                    MainController.processMessage = "To call Reputa API";
                    MainController.showStatus();

                    // CallApiAsync();
                    reputaAPITask = Task.Run(async () => await CallReputaApiAsync());
                    // var result = task.WaitAndUnwrapException();
                }
            }
            else
            {
                MainController.ApiRunning = false;
                btnCallReputaApi.Text = "Calling Reputa API";
                btnCallReputaApi.BackColor = SystemColors.ButtonFace;
            }
        }


        public static void trackingDirectory()
        {
            log.Info("start method");
            MainController MainController = MainController.Instance;
            // FolderPath read from codelist
            int PERIOD_TRACKING = 60000;
            string trackFolder = "C:/temp/In";
            string outputFolder = "C:/temp/Out";

            CodeList InputCode = MainController.GetCodeList(Constants.CODEGROUP_TRACKING_FOLDER, Constants.CODE_TRACKING_INPUT);
            CodeList OutputCode = MainController.GetCodeList(Constants.CODEGROUP_TRACKING_FOLDER, Constants.CODE_TRACKING_OUTPUT);
            CodeList PeriodTomeCode = MainController.GetCodeList(Constants.CODEGROUP_TRACKING_FOLDER, Constants.CODE_TRACKING_PERIOD);
            trackFolder = InputCode.Value;
            outputFolder = OutputCode.Value;
            PERIOD_TRACKING = Convert.ToInt32(PeriodTomeCode.Value);

            // Check folder is exit
            if (!Directory.Exists(trackFolder))
                // Directory exits!
                return;

            while (MainController.IsThreading)
            {
                // List excel file
                IList<string> trackList = new List<string>();
                string[] fileEntries = Directory.GetFiles(trackFolder);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToLower().Contains(".xlsx"))
                        trackList.Add(fileName);
                }

                //if( trackList.Count == 0)
                //{
                //    log.Info("Tracking folder is empty");
                //}

                log.Info("Number of excel files:" + trackList.Count);
                foreach (string fileName in trackList)
                {
                    // Neu thead yeu cau ngung hoac dang co xu ly du lieu
                    if (!MainController.IsThreading && MainController.IsProcessing)
                        return;

                    MainController.processMessage = $"To read excel file: {fileName}";
                    MainController.showStatus();
                    // Need control variable and release it
                    try
                    {
                        var package = new ExcelPackage(new FileInfo(fileName));

                        List<List<string>> dataList = EPPlusUtil.ReadExcelSheet(package, 0);
                        log.Info("STEP 1: passExcelData run");
                        
                        // release memory
                        // saveChange has cache so reset
                        MainController.loadData();
                        MainController.resetProcessData();
                        MainController.passExcelData(dataList);
                        // release memory
                        dataList = null;

                        log.Info("STEP 2: checkDataExist run");
                        MainController.processMessage = $"To check article exist";
                        MainController.showStatus();
                        MainController.checkDataExist();

                        log.Info("STEP 3: crawlerData run");
                        MainController.processMessage = $"To crawl artical data";
                        MainController.showStatus();
                        MainController.crawlerData();

                        log.Info("STEP 4: saveData run");
                        MainController.processMessage = $"To save artical";
                        MainController.showStatus();
                        MainController.saveData();

                        log.Info("STEP 4: Finish");
                        MainController.processMessage = $"Finish";
                        MainController.showStatus();

                        // To move a file or folder to a new location:
                        FileInfo f = new FileInfo(fileName);
                        System.IO.File.Move(fileName, outputFolder + "/" + DateTime.Now.ToString("yyyyMMddHH") + "_" + f.Name);
                    }
                    catch (Exception ex)
                    {
                        log.Info("Error " + ex.ToString());
                    }

                    // Hope GB release 
                    Thread.Sleep(1000);
                    if (!MainController.IsThreading)
                        break;
                }

                // Loop forever when stop signal
                for(int i = 0; i < PERIOD_TRACKING/ 1000; i++)
                {
                    if ( !MainController.IsThreading)
                        break;
                    Thread.Sleep(1000);
                }
            }

        }

        public async Task CallReputaApiAsync()
        {
            CodeList DomainCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_DOMAIN);
            CodeList SearchCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_SEARCHAPI);
            CodeList AuthorizationCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_AUTHORIZATION);
            CodeList PayloadCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PAYLOAD);
            ////////////////
            CodeList PeriodCallCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PERIOD_CALLING);
            CodeList PeriodSleepingCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PERIOD_SLEEPING);
            int PERIOD_CALLINGAPI = Convert.ToInt32(PeriodCallCodeList.Value);
            int THREAD_SLEEPING = Convert.ToInt32(PeriodSleepingCodeList.Value);

            // show reset info
            MainController.resetProcessData();
            MainController.showStatus();

            while (MainController.ApiRunning)
            {
                try
                {
                    ReputaApiController reputaApiController = new ReputaApiController();
                    reputaApiController.Domain = DomainCodeList.Value;
                    reputaApiController.SearchAPI = SearchCodeList.Value;
                    reputaApiController.Authorization = AuthorizationCodeList.Value;

                    // Replace page, startdate, enddate. startdate, enddate updated aboard
                    string Payload = PayloadCodeList.Value;
                    Payload = Payload.Replace(@$"2022/01/01", DateTime.Now.ToString("yyyy/MM/dd"));
                    reputaApiController.Payload = Payload;

                    log.Info("STEP 1: CALL callApiSearch");
                    var jsonObj = reputaApiController.callApiSearch();
                    List<Article> processedArticles = new List<Article>();

                    if (jsonObj.code == 0)
                    {
                        int total = jsonObj.data.total;
                        int count = jsonObj.data.count;
                        int loop = (total + count - 1) / count;
                        for (int i = 0; i < loop; i++)
                        {
                            reputaApiController.Payload = Payload.Replace(@$"page"":0", @$"page"":{i}");
                            Thread.Sleep(PERIOD_CALLINGAPI);
                            log.Info($"Sending page: {i}");
                            var jsonDataObj = reputaApiController.callApiSearch();
                            log.Info(jsonDataObj);

                            // process here
                            List<JsonHitsBody> jsonHitBody = jsonDataObj.data.hits;
                            List<Site> sites = MainController.sites;
                            List<Subject> subjects = MainController.subjects;
                            if (jsonHitBody.Count > 0)
                            {
                                foreach (var data in jsonHitBody)
                                {
                                    Article article = new Article();
                                    article.No = Convert.ToInt64(data.id);
                                    article.Domain = data.domain;
                                    article.Title = data.title;
                                    article.Description = data.summary;
                                    article.Content = data.content;
                                    article.Url = data.url;
                                    article.Emotion = "";

                                    // add image to article
                                    List<string> imageSources = data.image_sources;
                                    if (imageSources != null && imageSources.Count > 0)
                                    {
                                        article.Image = imageSources[0];
                                    }

                                    // add date to article
                                    try
                                    {
                                        DateTime oDate = DateTime.ParseExact(data.published_time, "yyyy/MM/dd HH:mm:ss", null);
                                        article.Date = oDate;
                                        // article.time = oDate.GetDateTimeFormats("HH:mm:ss");
                                    }
                                    catch (Exception e)
                                    { }

                                    // add siteId to article
                                    var siteTemp = sites.Where(s => s.Domain.ToLower().Equals(data.domain));
                                    // Not process if data is not in site list
                                    if (siteTemp == null || siteTemp.Count() == 0)
                                        continue;

                                    article.SiteId = siteTemp.First<Site>().SiteId;

                                    // add subjectId to article
                                    List<long> matched_topics = data.matched_topics;
                                    Subject subjectMatch = null;
                                    if (matched_topics.Count > 0)
                                    {
                                        foreach (var subject in subjects)
                                        {
                                            if( matched_topics.Contains(subject.TopicId))
                                            {
                                                subjectMatch = subject;
                                                break;
                                            }
                                        }
                                    }

                                    if (subjectMatch != null)
                                    {
                                        article.SubjectId = subjectMatch.SubjectId;
                                        article.SubjectName = subjectMatch.Title;
                                    }

                                    // Set default status
                                    article.Process = Constants.ARTICLE_STATUS_NONE;
                                    processedArticles.Add(article);
                                    break;
                                }
                            }

                            MainController.showStatus();
                        } // end loop

                        try
                        {
                            log.Info("STEP 2: CALL checkDataExist");
                            MainController.processedArticles = processedArticles;
                            MainController.processMessage = "To check artical exist";
                            MainController.showStatus();
                            MainController.checkDataExist();

                            log.Info("STEP 3: CALL crawlerContentData");
                            MainController.processMessage = "To crawl artical data";
                            MainController.showStatus();
                            MainController.crawlerContentData();

                            log.Info("STEP 4: CALL saveData");
                            MainController.processMessage = "To save article";
                            MainController.showStatus();
                            MainController.saveData();

                            log.Info("STEP 5: Finish");
                            MainController.processMessage = "Finish";
                            MainController.showStatus();
                        }
                        catch (Exception ex)
                        {
                            log.Info("Error " + ex.ToString());
                        }

                    }

                    log.Info(jsonObj);
                }
                catch (Exception ex)
                {
                    log.Info("EXCEPTION: \n" + ex.ToString());
                }

                // Loop forever when stop signal
                for (int i = 0; i < THREAD_SLEEPING / 1000; i++)
                {
                    if (!MainController.IsThreading)
                        break;
                    Thread.Sleep(1000);
                }
                Thread.Sleep(THREAD_SLEEPING);
            }



        }


        #region testing
        private void btnTestingCrawler_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text;
            string ImageExpression = txtImageExpression.Text;
            string DescriptionExpression = txtDescriptionExpression.Text;
            string ContentExpression = txtContentExpression.Text;

            HtmlAgilityPack.HtmlDocument document = MainController.getDocumentData(url);
            if (document == null)
                return;

            // Solve url ridirect
            //if( HtmlUtils.checkRedirectURL( document))
            //{
            //    string finalUrl = HtmlUtils.GetFinalRedirect(url);
            //    document = MainController.getDocumentTesting(url);
            //    if (document == null)
            //        return;
            //}

            string imageUrl = MainController.crawlDetailImage(ImageExpression, document);
            string description = MainController.crawlDetailDescription(DescriptionExpression, document);
            // string content = MainController.crawlDetailContent(ContentExpression, document, chkCropable.Checked);

            // After crawler to remove redudance text have same text is tile, description
            List<string> textList = new List<string>(description.Split(Constants.JOINING_CHAR));
            textList.Add("");
            string content = MainController.crawlDetailContent(ContentExpression, document, textList, chkCropable.Checked);

            // Can not load document from parth of html
            // content = HtmlUtils.RemoveRedundanceContent(content, textList);

            if (radioUTF8.Checked)
            {
                imageUrl = Encoding.UTF8.GetString(imageUrl.Select(c => (byte)c).ToArray());
                description = Encoding.UTF8.GetString(description.Select(c => (byte)c).ToArray());
                content = Encoding.UTF8.GetString(content.Select(c => (byte)c).ToArray());
            }
            else if (radioHtml.Checked)
            {
                imageUrl = HttpUtility.HtmlDecode(imageUrl);
                description = HttpUtility.HtmlDecode(description);
                content = HttpUtility.HtmlDecode(content);
            }

            // remove redudance path
            string imageUrl2 = StringUtils.removeRedudancePath( imageUrl);
            {
                int siteId = Convert.ToInt32(cboSite.SelectedValue);
                Site site = MainController.sites.Where<Site>(s => s.SiteId == siteId).FirstOrDefault();
                var ImagePath = string.IsNullOrEmpty(site.ImageServerPath) ? site.SiteUrl : site.ImageServerPath;
                if ((!string.IsNullOrEmpty(imageUrl2)) && (!imageUrl2.ToLower().StartsWith("http")))
                {
                    // If imageUrl have domain, domain mean: .vn, .com, .org, .net
                    // check style No 2
                    if (imageUrl2.Contains(".vn") || imageUrl2.Contains(".com") || imageUrl2.Contains(".org") || imageUrl2.Contains(".net"))
                    {
                        if (imageUrl2.StartsWith("https"))
                            imageUrl2 = "https:" + imageUrl2;
                        else
                            imageUrl2 = "http:" + imageUrl2;
                    }
                    else
                        imageUrl2 = ImagePath.Trim() + imageUrl2;
                }
            }

            string newLine = Environment.NewLine;
            txtResponse.Text = @$"imageUrl: {imageUrl} {newLine} imageUrl crawler: {imageUrl2} {newLine} description: {description} {newLine} content: {content}";
        }

        private void btnTestingReputaApi_Click(object sender, EventArgs e)
        {
            // CallApiAsync();
            var task = Task.Run(async () => await TestingReputaApiAsync());
            // var result = task.WaitAndUnwrapException();
        }



        public async Task TestingReputaApiAsync()
        {
            try
            {
                CodeList DomainCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_DOMAIN);
                CodeList SearchCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_SEARCHAPI);
                CodeList AuthorizationCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_AUTHORIZATION);
                CodeList PayloadCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PAYLOAD);

                ReputaApiController reputaApiController = new ReputaApiController();

                reputaApiController.Domain = DomainCodeList.Value;
                reputaApiController.SearchAPI = SearchCodeList.Value;
                reputaApiController.Authorization = AuthorizationCodeList.Value;
                reputaApiController.Payload = PayloadCodeList.Value;
                // Replace page, startdate, enddate

                var jsonObj = reputaApiController.callApiSearch();
            }
            catch (Exception ex)
            {
                log.Info("EXCEPTION: \n" + ex.ToString());
            }
        }
        #endregion testing

        private void btnSaveExpression_Click(object sender, EventArgs e)
        {
            //dep = db.Departments.Where(d => d.Name == "Accounts").First();
            //dep.Descr = "This is Accounts Department";
            //db.SaveChanges();

            int siteId = Convert.ToInt32(cboSite.SelectedValue);
            Site site = MainController.sites.Where<Site>(s => s.SiteId == siteId).FirstOrDefault();

            site.ImageSignal = txtImageExpression.Text;
            site.DescriptionSignal = txtDescriptionExpression.Text;
            site.ContentSignal = txtContentExpression.Text;
            site.ImageServerPath = txtImageServerPath.Text;
            site.Note = txtNote.Text;
            site.UrlTesting = txtUrl.Text;
            site.Updated = DateTime.Now;

            if (radioUTF8.Checked)
                site.ContentType = Constants.CONTENTTYPE_UTF8;
            else if (radioHtml.Checked)
                site.ContentType = Constants.CONTENTTYPE_HTMLENCODE;
            else
                site.ContentType = "";

            MainController.UpdateSite(site);
        }

        private void cboSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            int siteId = Convert.ToInt32(cboSite.SelectedValue);
            Site site = MainController.sites.Where<Site>(s => s.SiteId == siteId).FirstOrDefault();
            txtImageExpression.Text = site.ImageSignal;
            txtDescriptionExpression.Text = site.DescriptionSignal;
            txtContentExpression.Text = site.ContentSignal;
            txtImageServerPath.Text = site.ImageServerPath;
            txtNote.Text = site.Note;
            txtUrl.Text = site.UrlTesting;

            if (Constants.CONTENTTYPE_UTF8.Equals(site.ContentType))
                radioUTF8.Checked = true;
            else if (Constants.CONTENTTYPE_HTMLENCODE.Equals(site.ContentType))
                radioHtml.Checked = true;
            else
                radioNone.Checked = true;


            txtResponse.Text = "";
        }


        public void showStatus()
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                dataGridSite.DataSource = MainController.sites.ToArray();
                dataGridSubject.DataSource = MainController.subjects.ToArray();

                //Access your controls
                lblAppStatus.Text = $"{MainController.processMessage}";

                if(MainController.processedArticles != null)
                {
                    dataGridArticle.DataSource = MainController.processedArticles.ToArray();
                    lblProcessStatus.Text = $"Process at: {MainController.CurrentRow}/{MainController.processingRow}; Excel data row: {MainController.excelDataRow}; Duplicated row: {MainController.duplicationRow}";
                }
            }));


        }

        private void btnSavingReputa_Click(object sender, EventArgs e)
        {
            CodeList DomainCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_DOMAIN);
            CodeList SearchCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_SEARCHAPI);
            CodeList AutheriationCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_AUTHORIZATION);
//            CodeList PayloadCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PAYLOAD);
            CodeList PeriodCallCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PERIOD_CALLING);
            CodeList PeriodSleepingCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PERIOD_SLEEPING);


            AutheriationCodeList.Value = txtReputaAuthorization.Text;
            MainController.UpdateCodeList(AutheriationCodeList);

            DomainCodeList.Value = txtReputaDomain.Text;
            MainController.UpdateCodeList(DomainCodeList);

            SearchCodeList.Value = txtReputaSearch.Text;
            MainController.UpdateCodeList(SearchCodeList);

            PeriodCallCodeList.Value = txtReputaPeriodCall.Text;
            MainController.UpdateCodeList(PeriodCallCodeList);

            PeriodSleepingCodeList.Value = txtReputaSleeping.Text;
            MainController.UpdateCodeList(PeriodSleepingCodeList);
        }

        private void btnLoadReputaInfo_Click(object sender, EventArgs e)
        {
            CodeList DomainCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_DOMAIN);
            CodeList SearchCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_SEARCHAPI);
            CodeList AutheriationCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_AUTHORIZATION);
            CodeList PayloadCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PAYLOAD);
            CodeList PeriodCallCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PERIOD_CALLING);
            CodeList PeriodSleepingCodeList = MainController.GetCodeList(Constants.CODEGROUP_REPUTA_API, Constants.CODE_REPUTA_PERIOD_SLEEPING);

            txtReputaDomain.Text = DomainCodeList.Value;
            txtReputaSearch.Text = SearchCodeList.Value;
            txtReputaAuthorization.Text = AutheriationCodeList.Value;
            txtReputaPayload.Text = PayloadCodeList.Value;
            txtReputaPeriodCall.Text = PeriodCallCodeList.Value;
            txtReputaSleeping.Text = PeriodSleepingCodeList.Value;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void btnTestingResponse_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text;
            
            try
            {
                HtmlAgilityPack.HtmlDocument document = null;
                document = MainController.getDocumentData(url);
                string value = document.DocumentNode.OuterHtml;
                txtResponse.Text = "HTML RETURN: " + value;

                document = MainController.getDocumentData(url, 1);
                string value2 = document.DocumentNode.OuterHtml;

                txtResponse.Text = txtResponse.Text + "\n\r\n\r\n\r\n\r HTML RETURN 2: " + value2;

                document = MainController.getDocumentData(url, 2);
                string value3 = document.DocumentNode.OuterHtml;

                txtResponse.Text = txtResponse.Text + "\n\r\n\r\n\r\n\r HTML RETURN 3: " + value3;
            }
            catch (Exception ex)
            {
                log.Debug($"Link :{url} can not load data. Exception : " + ex.ToString());
            }
        }
    }
}
