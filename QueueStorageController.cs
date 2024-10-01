using ABCretail_CLVD.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCretail_CLVD.Controllers
{
    public class QueueStorageController : Controller
    {
        private readonly QueueStorageService _queueService;

        public QueueStorageController(QueueStorageService queueService)
        {
            _queueService = queueService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var messages = await _queueService.ReceiveAllMessagesAsync(); // Get all messages
            return View(messages);
        }

        [HttpGet]
        public IActionResult SendMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string messageContent)
        {
            if (!string.IsNullOrEmpty(messageContent))
            {
                try
                {
                    await _queueService.SendMessageAsync(messageContent);
                    ViewBag.Message = "Message sent successfully.";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Error sending message: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "Please enter a message.";
            }

            return RedirectToAction("Index"); // Redirect back to the Index action after sending a message
        }

        [HttpGet]
        public async Task<IActionResult> ReceiveMessage()
        {
            try
            {
                var message = await _queueService.ReceiveMessageAsync();
                if (message != null)
                {
                    ViewBag.Message = $"Received message: {message}";
                }
                else
                {
                    ViewBag.Message = "No messages in the queue.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error receiving message: {ex.Message}";
            }

            return RedirectToAction("Index"); // Redirect back to the Index action after receiving the message
        }
    }

}
