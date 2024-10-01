using Azure;
using Azure.Storage.Files.Shares;

namespace ABCretail_CLVD.Services
{
    public class FileStorageService
    {
        private readonly ShareClient _shareClient;

        public FileStorageService(string connectionString, string shareName)
        {
            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists(); // Ensure the file share exists
        }

        public async Task<List<string>> GetFilesAsync(string directoryName)
        {
            var directoryClient = _shareClient.GetDirectoryClient(directoryName);

            // Create the directory if it doesn't exist
            await directoryClient.CreateIfNotExistsAsync();

            var fileNames = new List<string>();

            await foreach (var item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (item.IsDirectory == false)
                {
                    fileNames.Add(item.Name);
                }
            }

            return fileNames;
        }

        public async Task UploadFileAsync(string directoryName, string fileName, IFormFile file)
        {
            var directoryClient = _shareClient.GetDirectoryClient(directoryName);

            // Create the directory if it doesn't exist
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(fileName);
            using (var stream = file.OpenReadStream())
            {
                await fileClient.CreateAsync(stream.Length);
                await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
            }
        }

        public async Task<Stream> DownloadFileAsync(string directoryName, string fileName)
        {
            var directoryClient = _shareClient.GetDirectoryClient(directoryName);

            // Create the directory if it doesn't exist
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(fileName);
            var downloadInfo = await fileClient.DownloadAsync();
            return downloadInfo.Value.Content;
        }
    }
}
