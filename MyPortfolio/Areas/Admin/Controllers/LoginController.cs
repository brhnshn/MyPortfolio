using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Areas.Admin.Models;
using MyPortfolio.Entities.Concrete;
using MyPortfolio.Models;
using MyPortfolio.Services;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private static readonly Dictionary<string, (string Code, DateTime Expiry, string UserId)> _resetCodes = new();

        public LoginController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            EmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserLoginViewModel p)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(p.Username, p.Password, true, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Theme", new { area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError("", "Hatalı kullanıcı adı veya şifre!");
                }
            }
            return View();
        }

        // ========== ŞİFREMİ UNUTTUM ==========

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Error = "Lütfen kullanıcı adınızı girin.";
                return View();
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                ViewBag.Error = "Bu kullanıcı adı bulunamadı veya e-posta adresi tanımlı değil.";
                return View();
            }

            // 6 haneli kod
            var code = new Random().Next(100000, 999999).ToString();
            _resetCodes[username.ToLower()] = (code, DateTime.Now.AddMinutes(10), user.Id.ToString());

            // Maili maskele: b****@gmail.com
            var emailParts = user.Email.Split('@');
            var maskedEmail = emailParts[0][0] + new string('*', Math.Max(emailParts[0].Length - 1, 3)) + "@" + emailParts[1];

            try
            {
                var body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #302b63; text-align: center;'>Şifre Sıfırlama</h2>
                        <p style='color: #555;'>Merhaba <strong>{user.Name}</strong>,</p>
                        <p style='color: #555;'>Şifrenizi sıfırlamak için aşağıdaki kodu kullanın:</p>
                        <div style='background: linear-gradient(135deg, #667eea, #764ba2); color: white; text-align: center; padding: 20px; border-radius: 10px; margin: 20px 0;'>
                            <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px;'>{code}</span>
                        </div>
                        <p style='color: #999; font-size: 12px; text-align: center;'>Bu kod 10 dakika geçerlidir.</p>
                    </div>";

                await _emailService.SendEmailAsync(user.Email, "Şifre Sıfırlama Kodu - MyPortfolio", body);
                TempData["ResetUsername"] = username;
                TempData["MaskedEmail"] = maskedEmail;
                return RedirectToAction("VerifyCode");
            }
            catch (Exception)
            {
                ViewBag.Error = "E-posta gönderilemedi. Lütfen daha sonra tekrar deneyin.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult VerifyCode()
        {
            if (TempData.Peek("ResetUsername") == null)
                return RedirectToAction("ForgotPassword");

            ViewBag.MaskedEmail = TempData.Peek("MaskedEmail");
            return View();
        }

        [HttpPost]
        public IActionResult VerifyCode(string code)
        {
            var username = TempData["ResetUsername"]?.ToString();
            var maskedEmail = TempData["MaskedEmail"]?.ToString();

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("ForgotPassword");

            if (!_resetCodes.TryGetValue(username.ToLower(), out var stored))
            {
                ViewBag.Error = "Geçersiz veya süresi dolmuş kod.";
                TempData["ResetUsername"] = username;
                TempData["MaskedEmail"] = maskedEmail;
                return View();
            }

            if (DateTime.Now > stored.Expiry)
            {
                _resetCodes.Remove(username.ToLower());
                ViewBag.Error = "Kodun süresi dolmuş. Lütfen yeni kod isteyin.";
                return View();
            }

            if (code?.Trim() != stored.Code)
            {
                ViewBag.Error = "Yanlış kod girdiniz.";
                TempData["ResetUsername"] = username;
                TempData["MaskedEmail"] = maskedEmail;
                return View();
            }

            TempData["ResetUserId"] = stored.UserId;
            _resetCodes.Remove(username.ToLower());
            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (TempData.Peek("ResetUserId") == null)
                return RedirectToAction("ForgotPassword");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            var userId = TempData["ResetUserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("ForgotPassword");

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Şifreler uyuşmuyor.";
                TempData["ResetUserId"] = userId;
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("ForgotPassword");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Şifreniz başarıyla değiştirildi! Yeni şifrenizle giriş yapabilirsiniz.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ViewBag.Error = error.Description;
                }
                TempData["ResetUserId"] = userId;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel p)
        {
            if (ModelState.IsValid)
            {
                var requiredKey = _configuration["SetupSettings:Key"];
                if (string.IsNullOrEmpty(p.SetupKey) || p.SetupKey != requiredKey)
                {
                    ModelState.AddModelError("", "Hatalı Kurulum Anahtarı!");
                    return View(p);
                }

                AppUser user = new AppUser()
                {
                    Name = p.Name,
                    Surname = p.Surname,
                    Email = p.Email,
                    UserName = p.UserName
                };

                var result = await _userManager.CreateAsync(user, p.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(p);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}
