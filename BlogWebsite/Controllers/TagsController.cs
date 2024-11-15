using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;
using X.PagedList.Extensions;

namespace NewsWebsite.Controllers
{
    public class TagsController : Controller
    {
        private readonly NewswebsiteContext _context;
        private readonly IMapper _mapper;

        public TagsController(NewswebsiteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Tags
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? page)
        {
            var tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            var pageNumber = page ?? 1;
            var pageSize = 6;

            ViewBag.TagPage = tags.ToPagedList(pageNumber, pageSize);

            return View(tags);
        }

        // GET: Tags/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] TagCreateVM model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    var tag = _mapper.Map<TagCreateVM, Tag>(model);
                    tag.IsDeleted = false;

                    _context.Add(tag);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occured while adding a new tag at [HttpPost]Create");
                }
            }
            return View(model);
        }

        // GET: Tags/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            var editTag = _mapper.Map<Tag, TagEditVM>(tag);
            if (editTag == null)
            {
                return NotFound();
            }
            return View(editTag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TagEditVM model)
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
                    var tag = _mapper.Map<TagEditVM, Tag>(model);
                    tag.IsDeleted = false;
                    

                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error occured while editing a tag at [HttpPost]Edit");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Tags/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}
