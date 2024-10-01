using ABCretail_CLVD.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCretail_CLVD.Controllers
{
    public class FileStorageController : Controller
    {
        private readonly FileStorageService _fileStorageService;

        public FileStorageController(FileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
            {
                ViewBag.Message = "Please provide a directory name.";
                return View(new List<string>()); // Return an empty list
            }

            ViewBag.DirectoryName = directoryName; // Pass the directory name to the view
            var files = await _fileStorageService.GetFilesAsync(directoryName);
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(string directoryName, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                await _fileStorageService.UploadFileAsync(directoryName, file.FileName, file);
                ViewBag.Message = "File uploaded successfully.";
            }
            else
            {
                ViewBag.Message = "File upload failed. Please select a file.";
            }

            return RedirectToAction("Index", new { directoryName });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string directoryName, string fileName)
        {
            var stream = await _fileStorageService.DownloadFileAsync(directoryName, fileName);
            return File(stream, "application/octet-stream", fileName);
        }
    }


}
