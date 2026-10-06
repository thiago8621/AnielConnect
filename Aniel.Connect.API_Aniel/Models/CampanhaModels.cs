using System;
using System.Collections.Generic;

namespace Aniel.Connect.API_Aniel.Models
{
    public class CampanhaListModel
    {
        public long Codigo { get; set; }
        public string Titulo { get; set; }
        public bool CampanhaTempoDeterminado { get; set; }
        public bool Status { get; set; }
        public string DataInicio { get; set; }
        public string DataFinal { get; set; }
        public List<EquipeVendaCampanhaList> EquipeVendaCampanha { get; set; }
        public List<RegioesCampanhaList> RegioesCampanha { get; set; }
        public List<AreaCoberturaCampanhaList> AreaCoberturaCampanha { get; set; }
        public List<PlanoProdutoCampanhaList> PlanoProdutoCampanha { get; set; }
    }

    public class CampanhaApiRequest : ApiRequest
    {
        public string PrefixoCupom { get; set; }
    }

    public class EquipeVendaCampanhaList
    {
        public Int32 Id { get; set; }
        public string Setor { get; set; }
        public string Vendedor { get; set; }
        public string Supervisor { get; set; }
        public string CadastroMeta { get; set; }
    }

    public class RegioesCampanhaList
    {
        public int Id { get; set; }
        public string Pais { get; set; }
        public string UF { get; set; }
        public string Cidade { get; set; }
    }

    public class AreaCoberturaCampanhaList
    {
        public int Id { get; set; }
        public string AreaCobertura { get; set; }
    }

    public class PlanoProdutoCampanhaList
    {
        public int Id { get; set; }
        public string NomeExibicao { get; set; }
        public string PlanoProduto { get; set; }
        public string ValorPlanoProduto { get; set; }
        public string ValorInstalacao { get; set; }
        public bool TemValidade { get; set; }
        public string DataValidade { get; set; }
        public int NumeroParcelasInstalacao { get; set; }
    }

}
