using System.Net;
using Azure;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class FileStorageFunction
    {
        private readonly ShareClient _shareClient;

        public FileStorageFunction()
        {
            //Get the Azure Storage connection string from enviroment variables
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            //Intialize the ShareClient to interact with a specifc file share in Azure storage
            _shareClient = new ShareClient(connectionString, "your-share-name");

            // Create the file share if it doesnt already exist
            _shareClient.CreateIfNotExists();
        }

        // Function that handles HTTP POST request to upload a file to Azure Files
        [Function("UploadFileToShare")]

        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            //Get a logger instance to log information about the functions execution 
            var logger = executionContext.GetLogger("UploadFileToShare");

            //Log that the function is starting the file upload process
            logger.LogInformation("Uploading file to Azure Files...");

            //Get a DirectoryClient  for a specific directory within the file share (replace "your-directory-name" with actual directory name
            var directoryClient = _shareClient.GetDirectoryClient(" your - directory-name");

            //create the directory if it doesnt already exist 
            await directoryClient.CreateIfNotExistsAsync();

            //Get a FileClient for a specific file within the directory (" replace "your-file name) with the actual file name 
            var fileClient = directoryClient.GetFileClient("your file name");

            //read the incoming file stream from the HTTP request body 
            using (var stream = req.Body)
            {
                //Create the File in azure files with specifeid length 
                await fileClient.CreateAsync(stream.Length);

                //Upload the context of the stream to the file 
                await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteStringAsync(" File uploaded to Azure Files successfully ");


        }
    }
}
