using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Areas.Admin.Models;
using MyPortfolio.Entities.Concrete;
using MyPortfolio.Models;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        // 1. Ayar dosyasını okuyacak aracı ekliyoruz
        private readonly IConfiguration _configuration;

        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration; // 2. Constructor'da içeri alıyoruz
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
                // --- 3. PROFESYONEL LİSANS KONTROLÜ ---
                // Şifreyi koddan değil, appsettings.json dosyasından okuyoruz.
                var requiredKey = _configuration["SetupSettings:Key"];

                // Eğer kullanıcı boş girdiyse veya yanlış girdiyse hata ver
                if (string.IsNullOrEmpty(p.SetupKey) || p.SetupKey != requiredKey)
                {
                    ModelState.AddModelError("", "Hatalı Kurulum Anahtarı! Lütfen yazılım sahibi ile iletişime geçin.");
                    return View(p);
                }
                // --------------------------------------

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