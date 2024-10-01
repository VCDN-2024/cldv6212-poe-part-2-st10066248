using System.Net;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class TableStorageFunction
    {

        //Declare a private TabelClient object to interact with azure
        private TableClient _tableClient;

        public TableStorageFunction()
        {
            //Get the Azure storage connection string from the enviroment variables
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            //Create a service Client to interact with the table service in the azure storage account
            var ServiceClient = new TableServiceClient(connectionString);

            //Intialize the TableClient to interact with specific table
            _tableClient = ServiceClient.GetTableClient("YOUR TABLE NAME ");

            // create the table if it doesnt exist 
            _tableClient.CreateIfNotExists();
        }

        //Function that handles the HTTP POST  requests to store data in Table storage
        [Function("StoreToTable")]

        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            //Get a logger instance to log information about the functions execution
            var logger = executionContext.GetLogger("StoreToTable");

            //Log that the function is starting the process of storing data to Table Storage
            logger.LogInformation("Storing data to Azure Table Storage...");

            //Create a new TableEntity with a specified PartitionKey and RowKey
            var entity = new TableEntity("PartitionKey", "RowKey")
            {
                {"PropertyName", "PropertyValue" }  //Add custom properties to the entity
            };

            //Add the entity to the Azure Table Storage asynchronously
            await _tableClient.AddEntityAsync(entity);

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteStringAsync(" Data stored successfully");
        }
    }

}

