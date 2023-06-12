using Azure;
using Azure.Data.Tables;

namespace AzureWebTable.Models
{
    public class Contato : ITableEntity
    {

        public string Nome { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PartitionKey { get ; set ; }=null!;
        public string RowKey { get; set; } = null!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
