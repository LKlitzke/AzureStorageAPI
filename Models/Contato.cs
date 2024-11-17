using Azure;
using Azure.Data.Tables;

namespace AzureStorageAPI.Models
{
    public class Contato : ITableEntity
    {
        public string Nome {  get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }

        // ITableEntity properties
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}