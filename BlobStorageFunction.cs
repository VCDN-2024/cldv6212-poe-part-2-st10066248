using System.Net;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class BlobStorageFunction
    {
        //Declare a private BlobServiceClient object to interact with Azure Blob Storage
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageFunction()
        {
            //Get the Azure Storage connection string enviroment variables
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");


            // Intialize the  BlobServiceClient using the connection string
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        //Function that handles HTT POST requests to upload data to Blob Storage
        [Function("UploadToBlob")]

        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            //get a logger instance to log infromation about the functions execution
            var logger = executionContext.GetLogger("UploadToBlob");

            logger.LogInformation("Uploading to Blob Storage.....");

            //Get a BlobContainerClient for a specific container (replace "your-container-name" with actual container name)
            var containerClient = _blobServiceClient.GetBlobContainerClient(" Your - container - name ");

            //create the blob container if it doesnt  already exist 
            await containerClient.CreateIfNotExistsAsync();

            //Get a BlobClient for the specifc blob ( replace " your - -blob - name " with your actual blob name )
            var blobClient = containerClient.GetBlobClient("your-blob-name");

            //Upload the HTTP request body stream to the blob storage , overwriting any exisiting blob with the same name

            using (var stream = req.Body)
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteStringAsync("Blob uploaded successfully");


        }
    }
}

