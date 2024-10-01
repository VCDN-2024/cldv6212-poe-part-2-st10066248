using Azure;
using Azure.Data.Tables;
using System.Threading.Tasks;

namespace ABCretail_CLVD.Services
{
    public class TableStorageService<T> where T : class, ITableEntity, new()
    {
        private readonly TableClient _tableClient;

        public TableStorageService(string connectionString, string tableName)
        {
            _tableClient = new TableClient(connectionString, tableName);
            _tableClient.CreateIfNotExists();
        }

        // Insert or merge an entity into the table
        public async Task InsertOrMergeEntityAsync(T entity)
        {
            if (string.IsNullOrEmpty(entity.PartitionKey) || string.IsNullOrEmpty(entity.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set before inserting or merging the entity.");
            }

            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Merge);
        }

        // Retrieve a specific entity by partition key and row key
        public async Task<T> RetrieveEntityAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        // Retrieve all entities from the table
        public async Task<List<T>> RetrieveAllEntitiesAsync()
        {
            var entities = new List<T>();

            // Use a query to retrieve all entities
            await foreach (var entity in _tableClient.QueryAsync<T>())
            {
                entities.Add(entity);
            }

            return entities; // Return the list of all entities
        }

        // Delete an entity from the table
        public async Task DeleteEntityAsync(string partitionKey, string rowKey)
        {
            try
            {
                await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                throw new Exception("Entity not found");
            }
        }
    }

}
