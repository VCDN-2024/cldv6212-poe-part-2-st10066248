using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class QueueStorageFunction
    {
        //Define a function named "QueueMessage" to handle incoming queue messages
        [FunctionName("QueueMessage")]

        public void Run(

            // This function is triggered when a new message is added to the specified queue
            [QueueTrigger("your -queue-name", Connection = "AzureWebJobsStorage")] string myQueueItem,
            ILogger log,

        //ICollector is used to collect and write messages to a different queue (sample queue)
          [Queue("sample-queue", Connection = "QueueStorageConnectionString")] ICollector<string> processedQueue)
        {
            try
            {
                // Log the incoming queue message that is being processed
                log.LogInformation($"Processing queue message: {myQueueItem}");



                //Prepare a new message for writing to the second queue 
                string processedMessage = $"Processed message: {myQueueItem}";

                //log the message that will be writtten to the second queue
                log.LogInformation($"Writing message to processed queue:{processedMessage}");

                //Add the processed message to the second queue ( sample- queue)
                processedQueue.Add(processedMessage);
            }
            catch (Exception ex)
            {
                log.LogError($"Error processing queue message: {ex.Message}");

            }
        }
    }
}
