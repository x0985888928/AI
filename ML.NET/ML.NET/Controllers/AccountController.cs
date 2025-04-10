using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ML.NET.Models;

namespace MyWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager,
                                 UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // 若驗證失敗（欄位沒填、格式有誤...）直接回畫面
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 呼叫 SignInManager 進行密碼驗證與登入
            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return string.IsNullOrEmpty(returnUrl)
                    ? RedirectToAction("Index", "Home")
                    : LocalRedirect(returnUrl);
            }
            else
            {
                // 登入失敗時，新增一條錯誤訊息給前端
                ModelState.AddModelError(string.Empty, "登入失敗，請檢查使用者名稱或密碼。");
                return View(model);
            }
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 新建 IdentityUser 實體
            var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
            // 建立使用者帳號（自動對密碼進行雜湊處理）
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // 建立成功後，直接讓使用者登入
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 若建立失敗，將錯誤訊息回傳給前端
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
