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
    public class AccountsController : Controller
    {
        private readonly NewswebsiteContext _context;
        private readonly IMapper _mapper;

        public AccountsController(NewswebsiteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Accounts
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? page)
        {
            var accounts = await _context.Accounts.Include(a => a.Role).ToListAsync();

            var pageNumber = page ?? 1;
            var pageSize = 6;

            ViewBag.AccountPage = accounts.ToPagedList(pageNumber, pageSize);

            return View(accounts);
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [Authorize]
        public async Task<IActionResult> Personal()
        {
            if (User.FindFirstValue("AccountId") != null)
            {
                var loggedIn = Convert.ToInt32(User.FindFirstValue("AccountId"));
                var account = await _context.Accounts
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(m => m.Id == loggedIn);
                if (account == null)
                {
                    return NotFound();
                }
                return View(account);
            }

            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> EditInfo()
        {
            if (User.FindFirstValue("AccountId") == null)
            {
                return NotFound();
            }

            var id = Convert.ToInt32(User.FindFirstValue("AccountId"));
            var account = await _context.Accounts
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            var accountToEdit = _mapper.Map<Account, AccountEditInfoVM>(account);
            accountToEdit.Role = account.Role.Name;

            if (accountToEdit == null)
            {
                return NotFound();
            }
            return View(accountToEdit);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditInfo([Bind("DisplayName, Email, Username, Role")] AccountEditInfoVM model)
        {
            if(ModelState.IsValid)
            {
                if (User.FindFirstValue("AccountId") == null)
                {
                    return NotFound();
                }

                var id = Convert.ToInt32(User.FindFirstValue("AccountId"));
                var account = await _context.Accounts
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (account == null)
                {
                    return NotFound();
                }

                account.Email = model.Email;
                account.DisplayName = model.DisplayName;

                _context.Update(account);
                await _context.SaveChangesAsync();
                return Redirect("/Accounts/Personal");
            }
            return View();
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([Bind("CurrentPassword, NewPassword")] AccountChangePassVM model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                if (User.FindFirstValue("AccountId") != null)
                {

                    var id = Convert.ToInt32(User.FindFirstValue("AccountId"));
                    var account = await _context.Accounts.FindAsync(id);

                    if (account == null)
                    {
                        return NotFound();
                    }

                    if (account.Password != model.CurrentPassword.ToMd5Hash(account.RandomKey))
                    {
                        ModelState.AddModelError("Lỗi", "Sai mật khẩu hiện tại");
                    }

                    account.Password = model.NewPassword.ToMd5Hash(account.RandomKey);
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    return Redirect("/Accounts/Personal");

                }
                else
                {
                    return NotFound();
                }
            }
            return View();
        }


        // GET: Accounts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,RoleId,DisplayName,IsDeleted,Email,RandomKey")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", account.RoleId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }
            var accountToEdit = _mapper.Map<Account, AccountEditVM>(account);

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", accountToEdit.RoleId);
            return View(accountToEdit);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,RoleId,DisplayName,Email, RandomKey, IsDeleted")] AccountEditVM model, string? newPassword)
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
                    var account = _mapper.Map<AccountEditVM, Account>(model);

                    if (newPassword != null)
                    {
                        account.Password = newPassword.ToMd5Hash(model.RandomKey);

                    }

                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error occured while editing an account at [HttpPost]Edit");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Categories, "Id", "Name", model.RoleId);
            return View(model);
        }

        // GET: Accounts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (account == null)
            {
                return NotFound();
            }

            var articleToDelete = _mapper.Map<Account, AccountDeleteVM>(account);
            articleToDelete.Role = account.Role.Name;
            return View(articleToDelete);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.IsDeleted = !account.IsDeleted;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
