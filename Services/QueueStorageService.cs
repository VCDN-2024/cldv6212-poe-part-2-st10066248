using Azure.Storage.Queues;

namespace ABCretail_CLVD.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queueClient;

        public QueueStorageService(string connectionString, string queueName)
        {
            _queueClient = new QueueClient(connectionString, queueName, new QueueClientOptions
            {
                Diagnostics = { IsLoggingContentEnabled = true }
            });

            _queueClient.CreateIfNotExists(); // Ensures the queue is created if it doesn't already exist
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                await _queueClient.SendMessageAsync(message); // Sends a message to the queue
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Failed to send message to the queue: {ex.Message}");
                throw new Exception("Failed to send message to the queue.", ex);
            }
        }

        public async Task<string> ReceiveMessageAsync()
        {
            try
            {
                var message = await _queueClient.ReceiveMessageAsync();
                if (message.Value != null)
                {
                    await _queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
                    return message.Value.MessageText; // Returns the message text
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Failed to receive message from the queue: {ex.Message}");
                throw new Exception("Failed to receive message from the queue.", ex);
            }

            return null; // If no message is found, return null
        }

        public async Task<List<string>> ReceiveAllMessagesAsync()
        {
            var messages = new List<string>();
            try
            {
                while (true)
                {
                    var message = await ReceiveMessageAsync();
                    if (message != null)
                    {
                        messages.Add(message);
                    }
                    else
                    {
                        break; // No more messages
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Failed to retrieve messages from the queue: {ex.Message}");
                throw new Exception("Failed to retrieve messages from the queue.", ex);
            }

            return messages; // Return the list of messages
        }
    }
}
