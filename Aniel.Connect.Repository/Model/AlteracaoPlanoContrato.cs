using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class AlteracaoPlanoContratoListModel
    {
        public long Id { get; set; }        
        public string NomeFantasia { get; set; }
        public string Documento { get; set; }
        public string CategoriaCliente { get; set; }
        public string Vendedor { get; set; }
        public string Supervisor { get; set; }
        public string Departamento { get; set; }
        public string TermoAdesaoAssinado { get; set; }
        public string DataHoraAprovacao { get; set; }
        public string DataHoraCancelamento { get; set; }
        public string DataHoraCriacao { get; set; }
        public string DataAssinatura { get; set; }
        public long? NumeroContrato { get; set; }
        public string TipoAlteracao { get; set; }
        public string Status { get; set; }

    }
    public class AlteracaoPlanoContratoListResponseModel
    {
        private long? _RecordsFiltered;
        public long RecordsFiltered 
        {
            get 
            {        
                return _RecordsFiltered.GetValueOrDefault(0); 
            }
            set { _RecordsFiltered = value; } 
        }
        private IEnumerable<AlteracaoPlanoContratoListModel> _Records { get; set; }
        public IEnumerable<AlteracaoPlanoContratoListModel> Records { 
            get 
            { 
                if (_Records == null) 
                { 
                    _Records = new List<AlteracaoPlanoContratoListModel>();
                }
                return _Records;
            } 
            set { _Records = value; }
        }
    }
    public class AlteracaoPlanoContratoCadastroModel 
    {
        public VendaDadosGeraisViewModel VendaBase { get; set; }
        public Int64? Id_Cliente { get; set; }
        public long IdVenda { get; set; }        
        public long? IdPlanoAnterior { get; set; }
        public long? IdPlanoProduto { get; set; }
        public string numeroOS { get; set; }
        public string cod_ct { get; set; }
        public string projeto { get; set; }
        public string num_obra { get; set; }
        public int? num_doc { get; set; }
        public Int64? Id_Vendedor { get; set; }
        public Int64? Id_Departamento { get; set; }
        public Int64? Id_Supervisor { get; set; }
        public PlanoProdutoListModel PlanoProduto { get; set; }
        public string Status { get; set; }
        public Status_Venda Status_Venda { get; set; }
        public string DataCriacao { get; set; }
        public StatusEtapaVenda? EtapaTermoAdesao { get; set; }
        public bool NecessitaCriarOS { get; set; }
        public StatusEtapaVenda? EtapaCriacaoOS { get; set; }
        public StatusEtapaVenda? EtapaPendenciaFinanceira { get; set; }
        public TipoAlteracaoPlanoContratoEnum? TipoAlteracao { get; set; }     
    }

    public class AlteracaoPlanoContratoDataTableRequestModel : DataTableRequest
    {
        public long? IdVendedor { get; set; } 
        public long? IdSupervisor { get; set; }
        public long? IdContrato { get; set; }
        public bool? TermoAdesaoAssinado { get; set; }
        public Status_Venda? StatusVenda { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }

    }
    public class InfoEmpresaTermoAdesao
    {
        public string Nome { get; set; }
        public string RazaoSocial { get; set; }
        public byte[] Logo { get; set; }
        public string Cnpj { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
    public class InfoTipoAlteracao 
    {
        public long Id { get; set; }
        public long IdMudanca { get; set; }
        public double ValorFinalTotal { get; set; }
        public double ValorFinalTotalTroca { get; set; }
    }
}
