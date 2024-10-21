using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;

namespace NewsWebsite.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly NewswebsiteContext _context;

        public ArticlesController(NewswebsiteContext context)
        {
            _context = context;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var newswebsiteContext = _context.Articles.Include(a => a.Account).Include(a => a.Category);
            return View(await newswebsiteContext.ToListAsync());
        }

        //GET: Articles/Articles/{id}
        public async Task<IActionResult> Articles(int? id)
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

            return View(articles);
        }

        public async Task<IActionResult> Trending()
        {
            var articles = await _context.Articles.Include(a => a.Account).Include(a => a.Category).Where(a => a.IsDeleted == false && a.Published == true).OrderByDescending(a => a.Views).ToListAsync();

            if (articles == null)
            {
                return NotFound();
            }

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

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Description,Htext,Views,Thumbnail,Published,AccountId,CategoryId")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", article.AccountId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", article.AccountId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", article.CategoryId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date,Description,Htext,Views,Thumbnail,Published,AccountId,CategoryId")] Article article)
        {
            if (id != article.Id)
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
                    if (!ArticleExists(article.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", article.AccountId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Account)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
