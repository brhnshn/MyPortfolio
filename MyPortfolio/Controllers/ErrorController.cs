using Microsoft.AspNetCore.Mvc;

namespace MyPortfolio.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Aradığınız sayfa bulunamadı.";
                    return View("404");
                case 500:
                    ViewBag.ErrorMessage = "Sunucu tarafında bir hata oluştu.";
                    return View("500");
                default:
                    return View("Error");
            }
        }
    }
}
