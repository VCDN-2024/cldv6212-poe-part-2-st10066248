using Azure.Storage.Blobs;

namespace ABCretail_CLVD.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(string connectionString)
        {
            // Initialize the BlobServiceClient with the provided connection string
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<List<string>> GetExistingFilesAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("images"); // Adjust to your container name
            var blobNames = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobNames.Add(blobItem.Name);
            }

            return blobNames;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string images)
        {
            // Get the container client using the container name (in your case, "images")
            var containerClient = _blobServiceClient.GetBlobContainerClient(images);

            // Ensure the container exists, create it if it doesn't
            await containerClient.CreateIfNotExistsAsync();

            // Get the blob client using the file name
            var blobClient = containerClient.GetBlobClient(file.FileName);

            // Upload the file stream to the blob
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // Return the URL of the uploaded blob
            return blobClient.Uri.ToString();
        }

        public async Task<Stream> DownloadImageAsync(string images, string fileName)
        {
            // Get the container client using the container name (in your case, "images")
            var containerClient = _blobServiceClient.GetBlobContainerClient(images);

            // Get the blob client using the file name
            var blobClient = containerClient.GetBlobClient(fileName);

            // Download the blob and return its content as a stream
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }
    }

}
