using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWeb;
using NewsWeb.Models;
using NewsWeb.Utils;

namespace NewsWeb.Controllers
{
    public class ArticleController : Controller
    {
        private readonly DataContext _context;

        public const int ITEMS_PER_PAGE = 100;

        public ArticleController(DataContext context)
        {
            _context = context;
        }

        // GET: Article
        public async Task<IActionResult> Index(int page, int site, int subject)
        {
            if (page == 0)
                page = 1;

            var sites = _context.SiteSet.OrderBy(s => s.Domain).ToList<Site>();
            var subjects = _context.SubjectSet.OrderBy(s => s.order).ToList<Subject>(); 

            Site tmpSite = new Site();
            tmpSite.SiteId = 0;
            tmpSite.SiteName = "--Select all--";
            sites.Insert(0, tmpSite);

            
            Subject tmpSubject = new Subject();
            tmpSubject.subjectId = 0;
            tmpSubject.title = "--Select all--";
            subjects.Insert(0, tmpSubject);

            IQueryable<Article> articles = null;
            if( site ==0)
            {
                // articles = _context.ArticleSet.OrderByDescending(p => p.Date);
                if (subject == 0)
                    articles = _context.ArticleSet.OrderByDescending(p => p.Date);
                else
                    articles = _context.ArticleSet.Where(p => p.SubjectId == subject).OrderByDescending(p => p.Date);
            }
            else
                articles = _context.ArticleSet.Where(p => p.SiteId == site).OrderByDescending(p => p.Date);


            // Lấy tổng số dòng dữ liệu
            var totalItems = articles.Count();
            // Tính số trang hiện thị (mỗi trang hiện thị ITEMS_PER_PAGE mục)
            int totalPages = (int)Math.Ceiling((double)totalItems / ITEMS_PER_PAGE);
            
            if (page > totalPages && totalPages > 0)
                return RedirectToAction(nameof(ArticleController.Index), new { page = totalPages });
            
            var articleDisplay = await articles
                    .Skip(ITEMS_PER_PAGE * (page - 1))
                    .Take(ITEMS_PER_PAGE)
                    .ToListAsync();


            // return View (await listPosts.ToListAsync());
            ViewData["currentPage"] = page;
            ViewData["totalPages"] = totalPages;
            ViewBag.currentPage = page;
            ViewBag.totalPages = totalPages;

            // Tạo SelectList
            SelectList siteList = new SelectList(sites, "SiteId", "SiteName");
            ViewBag.siteList = siteList;
            ViewBag.site = site;

            SelectList subjectList = new SelectList(subjects, "subjectId", "title");
            ViewBag.subjectList = subjectList;
            ViewBag.subject = subject;


            return View(articleDisplay.AsEnumerable());
        }

        // GET: Article/Report
        public async Task<IActionResult> Report()
        {
            var sites = _context.SiteSet.OrderBy(s => s.Domain).ToList<Site>();
            var subjects = _context.SubjectSet.OrderBy(s => s.order).ToList<Subject>();

            List<Article> articles = new List<Article>();

            var sql1 = @$"SELECT DISTINCT [Domain] FROM [dbo].[article] 
                WHERE [Image] <> '' AND [description] <> ''
                ORDER BY
                [Domain]";

            var sql2 = @$"SELECT DISTINCT [Domain] FROM [dbo].[article] 
                WHERE [Image] = '' AND [description] <> ''
                ORDER BY
                [Domain]";

            var article1List = _context.ArticleSet.FromSqlRaw (sql1);
            var article2List = _context.ArticleSet.FromSqlRaw(sql2);


            // var result = Helper.RawSqlQuery(sql1, new { Name = (string)x[0], Count = (int)x[1] }, _context);



            // return View (await listPosts.ToListAsync());
            ViewBag.article1List = article1List;
            ViewBag.article2List = article2List;

            return View(articles.AsEnumerable());
        }


        // GET: Article/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.ArticleSet
                .FirstOrDefaultAsync(m => m.No == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Article/ViewDetails/5
        public async Task<IActionResult> ViewDetail(long? id)
        {
            var article = await _context.ArticleSet.FirstOrDefaultAsync(m => m.No == id);
            // model.IsActive = true;
            if (article == null)
            {
                return NotFound();
            }
            
            return PartialView("_ViewDetails", article);
        }

        // GET: Article/ViewDetails/5
        public async Task<IActionResult> ViewDetails(long? id)
        {
            var article = await _context.ArticleSet.FirstOrDefaultAsync(m => m.No == id);
            // model.IsActive = true;
            if (article == null)
            {
                return NotFound();
            }

            return PartialView("_ViewDetails", article);
        }


        // GET: Article/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Article/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("No,ResourceType,Url,Emotion,Title,Domain,Time,Date,SubjectName,Content,Image,Description,Process,SiteId,SubjectId,ContentHtml")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Article/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.ArticleSet.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Article/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("No,ResourceType,Url,Emotion,Title,Domain,Time,Date,SubjectName,Content,Image,Description,Process,SiteId,SubjectId,ContentHtml")] Article article)
        {
            if (id != article.No)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.No))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Article/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.ArticleSet
                .FirstOrDefaultAsync(m => m.No == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var article = await _context.ArticleSet.FindAsync(id);
            _context.ArticleSet.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(long? id)
        {
            return _context.ArticleSet.Any(e => e.No == id);
        }
    }
}
