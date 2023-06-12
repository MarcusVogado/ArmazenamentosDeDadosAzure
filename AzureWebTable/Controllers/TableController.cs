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
        [HttpPut("Atulizar")]
        public IActionResult Atualizar(string id,Contato contato)
        {
            var tabelClient= GetTableClient();
            var contatoTable = tabelClient.GetEntity<Contato>(id, id).Value;
            contatoTable.Nome = contato.Nome;
            contatoTable.Email = contato.Email;
            contatoTable.Telefone = contato.Telefone;

            tabelClient.UpsertEntity(contatoTable);
            return Ok();
        }
        [HttpGet("Listar")]
        public IActionResult ListarTodos()
        {
            var tableClient = GetTableClient();
            var contatos= tableClient.Query<Contato>().ToList();
            return Ok(contatos);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult ObterPorNome(string nome)
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contato>(c=>c.Nome==nome).ToList();
            return Ok(contatos);
        }

        [HttpDelete("Deletar/{id}")]
        public IActionResult DeletarContato(string id)
        {
            var tabelClient = GetTableClient();
            tabelClient.DeleteEntity(id, id);
            return NoContent(); 
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
