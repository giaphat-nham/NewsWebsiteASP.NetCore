using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Helpers;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;
using X.PagedList.Extensions;

namespace NewsWebsite.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly NewswebsiteContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(NewswebsiteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Categories
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? page)
        {
            var categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            var pageNumber = page ?? 1;
            var pageSize = 6;

            ViewBag.CategoryPage = categories.ToPagedList(pageNumber, pageSize);

            return View(categories);
        }

        // GET: Categories/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] CategoryCreateVM model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    var category = _mapper.Map<CategoryCreateVM, Category>(model);
                    category.IsDeleted = false;

                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occured while adding a new category at [HttpPost]Create");
                }
            }
            return View(model);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var editCategory = _mapper.Map<Category, CategoryEditVM>(category);
            if (editCategory == null)
            {
                return NotFound();
            }
            return View(editCategory);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CategoryEditVM model)
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
                    var category = _mapper.Map<CategoryEditVM, Category>(model);
                    category.IsDeleted = false;

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error occured while editing a category at [HttpPost]Edit");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
