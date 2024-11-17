using Azure.Data.Tables;
using AzureStorageAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContatosController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public ContatosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("StorageAccountConnectionString");
            _tableName = configuration.GetValue<string>("AzureTableName");
        }

        private TableClient GetTableClient()
        {
            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient(_tableName);

            tableClient.CreateIfNotExists();
            return tableClient;
        }

        [HttpPost("Criar")]
        public IActionResult CriarContato(Contato contato)
        {
            var tableClient = GetTableClient();
            contato.RowKey = Guid.NewGuid().ToString();
            contato.PartitionKey = contato.RowKey;


            tableClient.UpsertEntity<Contato>(contato);

            return Ok(contato);
        }

        [HttpPut("Atualizar")]
        public IActionResult AtualizarContato(string id, Contato contato)
        {
            var tableClient = GetTableClient();
            var contatoTable = tableClient.GetEntity<Contato>(id, id).Value;

            contatoTable.Nome = contato.Nome;
            contatoTable.Telefone = contato.Telefone;
            contatoTable.Email = contato.Email;

            tableClient.UpsertEntity<Contato>(contatoTable);

            return Ok(contatoTable);
        }

        
        [HttpGet("Listar")]
        public IActionResult ListarContatos()
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contato>();

            return Ok(contatos);
        }

        [HttpGet("Listar/{nome}")]
        public IActionResult ListarContatosByNome(string nome)
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contato>(c => c.Nome == nome).ToList();

            return Ok(contatos);
        }

        [HttpDelete("Excluir")]
        public IActionResult DeletarContato(string id)
        {
            var tableClient = GetTableClient();
            tableClient.DeleteEntity(id,id);

            return NoContent();
        }
    }
}
