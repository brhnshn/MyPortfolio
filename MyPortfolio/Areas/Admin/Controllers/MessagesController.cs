using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

namespace MyPortfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IGenericRepository<Message> _messageRepository;

        public MessagesController(IGenericRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public IActionResult Index()
        {
            // Tarihe göre sırala (En yeni en üstte)
            var values = _messageRepository.GetList().OrderByDescending(x => x.Date).ToList();
            return View(values);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var value = _messageRepository.GetById(id);
            if (value != null)
            {
                _messageRepository.Delete(value);
            }
            return RedirectToAction("Index");
        }

        [IgnoreAntiforgeryToken]
        // AJAX ile Okundu İşaretleme
        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            var message = _messageRepository.GetById(id);
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                _messageRepository.Update(message);
                return Ok(); // 200 OK döner
            }
            return BadRequest(); // Hata veya zaten okunmuş
        }
    }
}