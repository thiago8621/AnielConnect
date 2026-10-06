using Aniel.Connect.Core;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System.Collections.Generic;

namespace Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade.Interface
{
    public interface IEtapaViabilidade
    {
        EtapasValidacaoConsultaViabilidadeEnum Etapa { get; }
        GetApiRetorno<List<Elemento_Rede>> ExecutarConsulta();
        double Latitude { get; set; }
        double Longitude { get; set; }
        long? ClientePlanoProduto { get; set; }
    }
}
