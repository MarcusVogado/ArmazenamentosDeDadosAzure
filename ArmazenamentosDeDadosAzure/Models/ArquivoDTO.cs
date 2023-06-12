using System.Reflection.Metadata.Ecma335;

namespace ArmazenamentosDeDadosAzure.Models
{
    public class ArquivoDTO
    {
        public string Nome { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Uri {get; set; }=null!; 
    }
}
