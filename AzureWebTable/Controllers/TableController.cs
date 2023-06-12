using Azure.Data.Tables;
using AzureWebTable.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureWebTable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TableController:ControllerBase
    {
        private readonly string _tableConnection;
        private readonly string _tableName;

        public TableController(IConfiguration configuration)
        {
            _tableConnection = configuration.GetValue<string>("ConnectionStringTable");
            _tableName = configuration.GetValue<string>("AzureTableName");
        }

        [HttpPost]
        public IActionResult CriarContato(Contato contato)
        {
            var tabelClient= GetTableClient();
            contato.RowKey = Guid.NewGuid().ToString();
            contato.PartitionKey = contato.RowKey;
            tabelClient.UpsertEntity(contato);

            return Ok(contato);
        }


        private TableClient GetTableClient()
        {
            var serviceClient = new TableServiceClient(_tableConnection);//FAZENDO A CONEXÃO COM A TABELA
            var tableClient = serviceClient.GetTableClient(_tableName);//BUSCANDO A TABELA POR NOME
            tableClient.CreateIfNotExists();// se a tabela não existir ele irá criar a tabela e se ela já existir ele nã fara nada

            return tableClient;// retornando a referência da tabela
        }
    }
}
