using ArmazenamentosDeDadosAzure.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArmazenamentosDeDadosAzure.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArquivosController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public ArquivosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("BlobConnectionString");// buscando no arquivo json a string de conexão
            _containerName = configuration.GetValue<string>("BlobContainerName");
        }
        // blob é Binary Large Object , um objeto binario grande que pode ser um arquivo imagens vídeo etc..

        [HttpPost("Upload")]
        public IActionResult UploadArquivo(IFormFile arquivo) // Método para fazer o Upload no Azure Storage
        {
            BlobContainerClient container = new(_containerName, _connectionString);
            BlobClient blob = container.GetBlobClient(arquivo.FileName);

            using var data = arquivo.OpenReadStream();
            blob.Upload(data, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = arquivo.ContentType }
            });
            return Ok(blob.Uri.ToString());
        }

        [HttpGet("Download/{nomeArquivo}")]
        public IActionResult DownloadArquivo (string nomeArquivo)// Método para fazer o Download do arquivo
        {
            BlobContainerClient container = new(_containerName, _connectionString);
            BlobClient blob = container.GetBlobClient(nomeArquivo);
            if (!blob.Exists()) return BadRequest($"O arquivo de nome: {nomeArquivo} , não foi encontrado!");
            var retorno = blob.DownloadContent();

            return File(retorno.Value.Content.ToArray(),retorno.Value.Details.ContentType,blob.Name);
        }

        [HttpDelete("Deletar/{nomeArquivo}")]
        public IActionResult DeletarArquivo(string nomeArquivo)//Método para deletar o arquivo 
        {
            BlobContainerClient container = new(_containerName, _connectionString);
            BlobClient blob = container.GetBlobClient(nomeArquivo);
            blob.DeleteIfExists(); // Verifica se o arquivo existe 
            return NoContent();
        }
        [HttpGet("Listar")]
        public IActionResult ListarArquivos()
        {
            List<ArquivoDTO>arquivoDTOs = new List<ArquivoDTO>();
            BlobContainerClient container = new(_containerName, _connectionString);
            foreach(var arquivo in container.GetBlobs())
            {
                arquivoDTOs.Add(new ArquivoDTO
                {
                    Nome = arquivo.Name,
                    Tipo = arquivo.Properties.ContentType,
                    Uri= container.Uri.AbsoluteUri + "/" + arquivo.Name
                }) ;
            }
            return Ok(arquivoDTOs);
        }
    }
}
