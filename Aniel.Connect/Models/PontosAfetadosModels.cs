using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aniel.Connect.Core;

namespace Aniel.Connect.Models.PontosAfetados
{

    public class AdicionaPontosAfetados
    {
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string Contrato { get; set; }
        public string NumObra { get; set; }
        public List<POPAfetado> Pops { get; set; }
        public List<ElementoRedeAfetado> ElementosRede { get; set; }
    }

    public class POPAfetado
    {
        public List<long?> Ids_Pons { get; set; }
        public List <long?> Ids_Concentradoras { get; set; }
        public List <long?> Ids_Radius { get; set; }
    }

    public class ElementoRedeAfetado
    {
        public long Id { get; set; }
        public List<long> Ids_Portas { get; set; }
    }

    public class ClientesAfetados
    {

        public long Count { get; set; }
        public List<DadosClientesAfetados> Clientes { get; set; }
    }

    public class DadosClientesAfetados
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public long Id_Login_Radius { get; set; }
        public string Login_Radius { get; set; }


    }

    public class GetClientesByOSManutencaoRequest : DataTableRequest
    {
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumDoc { get; set; }
        public string NumObra { get; set; }

    }

    public class DeletePontoAfetadoRequest
    {
        public string Tipo { get; set; }
        public long Id { get; set; }
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumDoc { get; set; }
        public string NumObra { get; set; }
    }

}