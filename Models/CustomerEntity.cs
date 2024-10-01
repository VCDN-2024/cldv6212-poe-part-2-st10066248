using Azure;
using Azure.Data.Tables;
using System;

namespace ABCretail_CLVD.Models
{
    public class CustomerEntity : ITableEntity
    {
        // PartitionKey and RowKey are required for Azure Table Storage
        public string PartitionKey { get; set; } // Will be set as LastName
        public string RowKey { get; set; } // Will be set as Email or a GUID
        public DateTimeOffset? Timestamp { get; set; } // System-defined, should not be manually changed
        public ETag ETag { get; set; } // Used for concurrency control

       
        public string FirstName { get; set; } // Customer's First Name
        public string LastName { get; set; } // Customer's Last Name
        public string Email { get; set; } // Customer's Email

        // Constructor to ensure PartitionKey and RowKey are set properly
        public CustomerEntity()
        {
            // Use LastName as PartitionKey (or a default if LastName is null)
            PartitionKey = LastName ?? "DefaultPartition";

            // Use Email as RowKey, or generate a GUID if Email is not provided
            RowKey = Email ?? Guid.NewGuid().ToString();
        }

        // You can also add a constructor to allow setting properties directly
        public CustomerEntity(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;

            // Set PartitionKey and RowKey based on provided values
            PartitionKey = LastName ?? "DefaultPartition";
            RowKey = Email ?? Guid.NewGuid().ToString();
        }
    }
}

