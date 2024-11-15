using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NewsWebsite.Models;
using X.PagedList.Extensions;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NewsWebsite.ViewModels;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NewsWebsite.Helpers;
using Microsoft.Identity.Client;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using iText.Layout.Font;
using NewsWebsite.Services;

namespace NewsWebsite.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly NewswebsiteContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public ArticlesController(NewswebsiteContext context, IMapper mapper, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
        }

        // GET: Articles
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Index(int? page, string? key, string? sort)
        {
            var articles = await _context.Articles.Include(a => a.Account).Include(a => a.Category).Where(a => a.IsDeleted == false).OrderBy(a => a.Date).ToListAsync();

            if (sort != null && sort != "asc")
            {
                articles = await _context.Articles.Include(a => a.Account).Include(a => a.Category).Where(a => a.IsDeleted == false).OrderByDescending(a => a.Date).ToListAsync();
            }

            var pageNumber = page ?? 1;
            var pageSize = 6;

            if (key != null)
            {
                var searchResult = articles.Select(a => new { Article = a, ProcessedTitle = RemoveAccents(a.Title) }).OrderBy(a => a.Article.Date).Where(result => result.ProcessedTitle.Contains(RemoveAccents(key), StringComparison.OrdinalIgnoreCase)).Select(result => result.Article).ToList();

                if (sort != null && sort != "asc")
                {
                    searchResult = articles.Select(a => new { Article = a, ProcessedTitle = RemoveAccents(a.Title) }).OrderByDescending(a => a.Article.Date).Where(result => result.ProcessedTitle.Contains(RemoveAccents(key), StringComparison.OrdinalIgnoreCase)).Select(result => result.Article).ToList();
                }

                ViewBag.ArticlePage = searchResult.ToPagedList(pageNumber, pageSize);
                return View(searchResult);
            }



            ViewBag.ArticlePage = articles.ToPagedList(pageNumber, pageSize);

            return View(articles);
        }

        //GET: Articles/Articles/{id}
        public async Task<IActionResult> Articles(int? id, int? page)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articles = await _context.Articles.Include(a => a.Account).Include(a => a.Category).Where(a => a.CategoryId == id && a.IsDeleted == false && a.Published == true).ToListAsync();

            if (articles == null)
            {
                return NotFound();
            }

            if (!articles.Any())
            {
                return NotFound();
            }

            var pageNumber = page ?? 1;
            var pageSize = 6;

            ViewBag.ArticlePage = articles.ToPagedList(pageNumber, pageSize);

            return View(articles);
        }

        public async Task<IActionResult> Trending(int? page)
        {
            var articles = await _context.Articles.Include(a => a.Account).Include(a => a.Category).Where(a => a.IsDeleted == false && a.Published == true).OrderByDescending(a => a.Views).ToListAsync();

            if (articles == null)
            {
                return NotFound();
            }

            var pageNumber = page ?? 1;
            var pageSize = 6;

            ViewBag.ArticlePage = articles.ToPagedList(pageNumber, pageSize);

            return View(articles);
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Account)
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            //Increase view
            article.Views += 1;
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();

            return View(article);
        }

        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> ViewArticle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Account)
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        public static string RemoveAccents(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<IActionResult> Search(string key, int? page)
        {
            var articles = await _context.Articles.Where(a => a.IsDeleted == false && a.Published == true).ToListAsync();

            var searchResult = articles.Select(a => new { Article = a, ProcessedTitle = RemoveAccents(a.Title) }).Where(result => result.ProcessedTitle.Contains(RemoveAccents(key), StringComparison.OrdinalIgnoreCase)).Select(result => result.Article).ToList();

            if (searchResult != null)
            {

                var pageNumber = page ?? 1;
                var pageSize = 6;

                ViewBag.ArticlePage = searchResult.ToPagedList(pageNumber, pageSize);
            }
            return View("Search", key);
        }

        [Authorize(Roles = "Admin, Moderator")]
        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = _context.Tags.ToList();
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title, Description, Htext, Published, CategoryId")] ArticleCreateVM model, IFormFile? Thumbnail, List<int>? TagIds)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            var accountId = Convert.ToInt32(User.FindFirstValue("AccountId"));
            if (ModelState.IsValid)
            {
                try
                {
                    var article = _mapper.Map<ArticleCreateVM, Article>(model);
                    article.IsDeleted = false;
                    article.Views = 0;
                    article.Date = DateTime.Now;
                    article.AccountId = accountId;

                    if (TagIds != null)
                    {
                        foreach (var id in TagIds)
                        {
                            var tag = await _context.Tags.FindAsync(id);

                            if (tag != null)
                            {
                                article.Tags.Add(tag);
                            }
                        }
                    }

                    if (Thumbnail != null)
                    {
                        article.Thumbnail = Ultilities.UploadImage(Thumbnail, "articles");
                    }
                    else
                    {
                        article.Thumbnail = "logo_vydhdt.png";
                    }

                    _context.Add(article);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occured while adding a new article at [HttpPost]Create");
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            ViewData["Tags"] = _context.Tags.ToList();
            return View(model);
        }

        // GET: Articles/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.Include(a => a.Tags).FirstOrDefaultAsync(a => a.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            var editArticle = _mapper.Map<Article, ArticleEditVM>(article);
            if (editArticle == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", article.CategoryId);
            ViewData["Tags"] = _context.Tags.ToList();
            return View(editArticle);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Htext,Thumbnail,Published,CategoryId, AccountId, Date, Views")] ArticleEditVM model, IFormFile? Image, List<int>? TagIds)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    var article = await _context.Articles.Include(a => a.Tags).FirstOrDefaultAsync(a => a.Id == id);
                    if (article == null)
                    {
                        return NotFound();
                    }

                    article.Title = model.Title;
                    article.Htext = model.Htext;
                    article.Description = model.Description;
                    article.Published = model.Published;

                    if (Image != null && Image.Name != model.Thumbnail)
                    {
                        article.Thumbnail = Ultilities.UploadImage(Image, "articles");
                    }
                    else
                    {
                        article.Thumbnail = model.Thumbnail;
                    }

                    if (TagIds != null)
                    {
                        ICollection<Tag> updatedTags = new List<Tag>();
                        foreach (var tagId in TagIds)
                        {
                            var tag = await _context.Tags.FindAsync(tagId);
                            if (tag != null)
                            {
                                updatedTags.Add(tag);
                            }
                        }

                        article.Tags = updatedTags;
                    }

                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error occured while editing an article at [HttpPost]Edit");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            ViewData["Tags"] = _context.Tags.ToList();
            return View(model);
        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            var article = await _context.Articles
                .Include(a => a.Account)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            var articleToDelete = _mapper.Map<Article, ArticleDeleteVM>(article);
            articleToDelete.Author = article.Account.DisplayName;
            return View(articleToDelete);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                article.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public IActionResult Print(Article article, string? author)
        {
            if (article == null)
            {
                return NotFound();
            }

            MemoryStream stream = new MemoryStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdfDoc = new PdfDocument(writer);

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "articles", article.Thumbnail);
            var htmlContent = $"<h1>{article.Title}</h1><br></br>" +
                $"<img src='{fullPath}' /><br></br>" +
                $"<p style='font-family: Times New Roman'>{article.Description}</p>" +
                $"<div style='font-family: Times New Roman'>{article.Htext}</div>" +
                $"<div style='font-family: Times New Roman'>Tác giả: {author}</div>" +
                $"<div style='font-family: Times New Roman'>{article.Date.ToString("dd/MM/yyyy")}</div>";

            // Convert HTML to PDF
            HtmlConverter.ConvertToPdf(htmlContent, pdfDoc, null);

            pdfDoc.Close();

            byte[] bytes = stream.ToArray();


            return File(bytes, "application/pdf", $"{article.Title}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> Comment(int? ArticleId, string? Title, string? FullName, string? Email, string? Comment)
        {
            if (ArticleId != null && !String.IsNullOrWhiteSpace(Title) && !String.IsNullOrWhiteSpace(Comment))
            {
                await _emailService.SendEmailAsync("phatnham3010@gmail.com", $"Nhận xét về bài viết \"{Title}\"", $"<p><b>Tên người nhận xét:</b> {(!String.IsNullOrWhiteSpace(FullName) ? FullName : "Không có")}</p>" +
                    $"<p><b>Địa chỉ Email:</b> {(!String.IsNullOrWhiteSpace(Email) ? Email : "Không có")}</p>" +
                    $"<p><b>Nhận xét về bài viết số {ArticleId}:</b> \"{Title}\"</p>" +
                    $"<p><b>Nội dung:</b></p>" +
                    $"<p>{Comment}</p>");
                return Redirect($"/Articles/Details/{ArticleId}");
            }
            return Redirect($"/Articles/Details/{ArticleId}");
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
