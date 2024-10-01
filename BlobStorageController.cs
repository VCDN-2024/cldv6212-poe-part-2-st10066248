using ABCretail_CLVD.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCretail_CLVD.Controllers
{
    public class BlobStorageController : Controller
    {
        private readonly BlobStorageService _blobStorageService;

        public BlobStorageController(BlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Retrieve the list of existing files, adjust the method to get files from Blob Storage
            var existingFiles = await _blobStorageService.GetExistingFilesAsync(); // Example method to get files
            return View(existingFiles);
        }

        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var containerName = "images";
                var blobUri = await _blobStorageService.UploadImageAsync(file, containerName);
                ViewBag.Message = $"Image uploaded successfully. Blob URL: {blobUri}";
            }
            else
            {
                ViewBag.Message = "File upload failed. Please select a file.";
            }

            return View("Index"); // Show the Index page after upload
        }

        [HttpGet]
        public async Task<IActionResult> DownloadImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                ViewBag.Message = "File name is required.";
                return View();
            }

            var containerName = "images";
            var stream = await _blobStorageService.DownloadImageAsync(containerName, fileName);
            return File(stream, "application/octet-stream", fileName);
        }

        public async Task<IActionResult> ManageBlobStorage()
        {
            // Retrieve the list of blobs/files from the storage
            List<string> files = await _blobStorageService.GetExistingFilesAsync(); // Correct method

            // Pass the list of files to the view as the model
            return View(files);
        }

    }


}
