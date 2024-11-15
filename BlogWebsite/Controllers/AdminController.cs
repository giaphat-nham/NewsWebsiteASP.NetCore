using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Helpers;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace NewsWebsite.Controllers
{
    public class AdminController : Controller
    {
        private readonly NewswebsiteContext _context;
        private readonly IMapper _mapper;

        public AdminController(NewswebsiteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind("Username, Password")] LoginVM model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var account = _context.Accounts.Include(a => a.Role).Where(a => a.IsDeleted != true).SingleOrDefault(acc => acc.Username == model.Username);

                if(account == null)
                {
                    ModelState.AddModelError("Lỗi", "Thông tin đăng nhập không hợp lệ.");
                }
                else
                {
                    if (account.IsDeleted == true)
                    {
                        ModelState.AddModelError("Lỗi", "Tài khoản này đã bị khóa.");
                    }
                    else
                    {
                        if (account.Password != model.Password.ToMd5Hash(account.RandomKey))
                        {
                            ModelState.AddModelError("Lỗi", "Thông tin đăng nhập không hợp lệ.");
                        }
                        else
                        {
                            var claims = new List<Claim> { 
                                new Claim(ClaimTypes.Role, account.Role.Name),
                                new Claim(ClaimTypes.Name, account.DisplayName),
                                new Claim("AccountId", account.Id.ToString()),
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);

                            if(Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else if (User.FindFirstValue(ClaimTypes.Role) == "Admin")
                            {
                                return Redirect("/Admin/Dashboard");
                            }
                            else
                            {
                                return Redirect("/Articles");
                            }
                        }
                    }
                }
            }

            return View("Login");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Admin");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([Bind("Username, Password, DisplayName, RoleId, Email")]RegisterVM model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                if (AccountExists(model.Username))
                {
                    ModelState.AddModelError("Lỗi", "Tên tài khoản đã tồn tại");
                }
                try { 
                var account = _mapper.Map<Account>(model);
                account.RandomKey = Ultilities.GenerateRandomKey();
                account.Password = model.Password.ToMd5Hash(account.RandomKey);
                account.IsDeleted = false;

                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("Register","Admin");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occured while adding a new account at [HttpPost]Register");
                }
            }

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private bool AccountExists(string username)
        {
            return _context.Accounts.Any(e => e.Username == username);
        }
    }
}
