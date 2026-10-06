using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class RelatorioProducaoModel
    {
        public virtual Int64? Id_Vendedor { get; set; }

        public virtual String Matricula { get; set; }
        public virtual String Nome_Funcionario { get; set; }
        public virtual Int64? Id_Superv { get; set; }
        public virtual String Matricula_Superv { get; set; }
        public virtual String Nome_Superv { get; set; }

        public virtual String Meta { get; set; }
        public virtual String Filial { get; set; }

        public virtual Int64? Id_Faixa_Vendedor { get; set; }
        public virtual String Faixa_Vendedor { get; set; }
        public virtual Int64? Id_Faixa_Superv { get; set; }
        public virtual String Faixa_Superv { get; set; }
        public virtual Int64? Id_Faixa_Filial { get; set; }

        public virtual String Unidade { get; set; }
        public virtual Int64 Quantidade { get; set; }
        public virtual Double Coeficiente { get; set; }
        public virtual Double Total { get; set; }
    }

    public class InfoPlanoProdutoModel
    {
        public virtual String Data_Venda { get; set; }
        public virtual String Campanha { get; set; }
        public virtual String Plano_Produto { get; set; }
        public virtual String Valor_Plano_Produto { get; set; }
    }
    public class RelatorioProducao_DataTableRequest : DataTableRequest
    {
        public Int64? Id_Filial { get; set; }
        public Int64? Id_Supervisor { get; set; }
        public Int64? Id_Vendedor { get; set; }
        public Int64? Id_Campanha { get; set; }

        public List<Int64> Ids_Plano_Produto { get; set; }

        public String Ini_Periodo { get; set; }
        public String Fim_Periodo { get; set; }
    }

    public class RelatorioProjecaoVendasModel
    {
        public string vendedor { get; set; }
        public string supervisor { get; set; }
        public string meta_atual_vinculada { get; set; }
        public string meta_mes { get; set; }
        public string dias_uteis { get; set; }
        public string meta_diaria { get; set; }
        public string realizado_mes { get; set; }
        public string pontos_pl_pr_vendidos { get; set; }
        public string vendas_os_finalizadas { get; set; }
        public string vendas_atingir_meta { get; set; }
        public string meta_dia { get; set; }
        public string vendas_perdidas { get; set; }
        public Double ticket_medio { get; set; }
    }
    

    public class RelatorioProjecao_Vendas : DataTableRequest
    {
        public Int64? Id_Filial { get; set; }
        public Int64? Id_Empresa { get; set; }
        public String Id_Supervisor { get; set; }
        public Int64? Id_Vendedor { get; set; }
        public Int64? Ano { get; set; }
        public Int64? Mes { get; set; }
    }
    public class RelatorioCheckList : DataTableRequest
    {
        public String Contrato { get; set; }
        public String Projeto { get; set; }
        public String Tecnico { get; set; }
        public String Questionario { get; set; }
        public String Nos { get; set; }
        public String Tipo_Servico { get; set; }
        public String DataInicio { get; set; }
        public String DataFim { get; set; }
        
    }

    public class RelatorioVendasNaoEfetivadasModel
    {
        public string num_venda { get; set; }
        public string tipo_cadastro { get; set; }
        public string data_criacao { get; set; }
        public string data_cancelamento { get; set; }
        public string motivo_cancelamento { get; set; }
        public string cliente_prospect { get; set; }
        public string categoria_cliente { get; set; }
        public string valor_venda { get; set; }
        public string uf_cidade { get; set; }
        public string bairro { get; set; }
        public string vendedor { get; set; }
        public string contatos_realizados { get; set; }
        public string planos_produtos { get; set; }
    }
    public class RelatorioCustoImpServicoModel
    {
        public string Nos { get; set; }
        public string DataAgendamento { get; set; }
        public string Databaixa { get; set; }
        public string NaturezaServico { get; set; }
        public string Contrato { get; set; }
        public string Tecnico { get; set; }
        public string Supervisor { get; set; }
        public string CategoriaCliente { get; set; }
        public string DescricaoEncerramento { get; set; }
        public string StatusEncerramento { get; set; }
        public string CustoMaoObra { get; set; }
        public string CustoMaterial { get; set; }
        public string CustoProducao { get; set; }

        public string CustoCombustivel { get; set; }
    }

    public class RelatorioReagendamentoOS : DataTableRequest
    {
        public String Data_Inicio { get; set; }
        public String Data_Fim { get; set; }
        public String TipoData { get; set; }
        public String NumOs { get; set; }
        public String Equipe { get; set; }
        public String Contrato { get; set; }
        public String Projeto { get; set; }
        public String TipoServico { get; set; }
        public String Area { get; set; }
        public String GrupoArea { get; set; }
        public String Localidade { get; set; }
        public String QtdeReagendamento { get; set; }
        public String primeiroValorReagendamento { get; set; }
        public String segundoValorReagendamento { get; set; }
        public String QtdeReabertura { get; set; }
        public String primeiroValorReabertura { get; set; }
        public String segundoValorReabertura { get; set; }
    }

    public class RelatorioReagendamentoOSModel
    {
        public string NumOS { get; set; }
        public Int64? QtdReabertura { get; set; }
        public string TecnicoResponsavel { get; set; }
        public string DataAgendamento { get; set; }
        public Int64? QtdReagendamento { get; set; }
        public string TipoServico { get; set; }
        public string DataCriacao { get; set; }
        public string DataEncerramento { get; set; }
        public string UsuarioEncerramento { get; set; }
        public string Cliente { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string GrupoArea { get; set; }
        public string DescricaoEncerramento { get; set; }
        public string ObservacaoTecnico { get; set; }
        public string StatusAuditoria { get; set; }
    }

    public class RelatorioVendas_Nao_Efetivadas : DataTableRequest
    {
        public String Data_Inicio { get; set; }
        public String Data_Fim { get; set; }
        public String Tipo_Filtro { get; set; }
        public Int64? Id_Vendedor { get; set; }
        public Int64? Id_Supervisor { get; set; }
        public Int64? Plano { get; set; }
        public Int64? Produto { get; set; }
        public Int64? Tipo_Cadastro { get; set; }
        public Int64? UF { get; set; }
        public Int64? Cidade { get; set; }
    }
    public class RelatorioCusto_Imp_Servico : DataTableRequest
    {
        public String Data_Inicio { get; set; }
        public String Data_Fim { get; set; }
        public String Tipo_Filtro { get; set; }
        public String Tipo_Prod { get; set; }
        public String Vendedor { get; set; }
        public String Supervisor { get; set; }
        public Int64? Empresa { get; set; }
        public Int64? Filial { get; set; }
        public String Tecnico { get; set; }
        public String Projeto { get; set; }
        public String Contrato { get; set; }
        public Int64? CentroCusto { get; set; }
        public Int64? NaturezaServico { get; set; }
        public Int64? Tipo_Servico { get; set; }
        public String Agrupamento { get; set; }

    }

    public class RelatorioSOSRequestModel: DataTableRequest
    {
        public bool? ConfirmadoCiencia { get; set; }
        public string DataHoraEnvio { get; set; }
        public string DataHoraIni { get; set; }
        public string DataHoraFim { get; set; }
        public string Tecnico { get; set; }
        public string Supervisor { get; set; }
        public string Resposta { get; set; }
        public string Monitor { get; set; }
        public string Gerou_Notificacao { get; set; }
    }

    public class RelatorioVendasReprovadasScoreRequestModel : DataTableRequest
    {
        public int? TipoData { get; set; }
        public string DataHoraIni { get; set; }
        public string DataHoraFim { get; set; }
        public string Supervisor { get; set; }
        public string Vendedor { get; set; }
        public string EtapaVenda { get; set; }
        public string Campanha { get; set; }
        public int? TipoStatusVenda { get; set; }
        public string StatusVenda { get; set; }
        public string Departamento { get; set; }
        public List<Int64?> Motivo_JustificativaVenda { get; set; } = new List<Int64?>();
        public string TipoScoreCredito { get; set; }
        public long? Valor_Unico { get; set; }
        public long? Valor_Inicial { get; set; }
        public long? Valor_Final { get; set; }
    }

    public class RelatorioSOSViewModel 
    {        
        public long Id { get; set;}
        public string Tecnico { get; set;}
        public string Supervisor { get; set;}
        public string Monitor { get; set;}
        public string ConfirmadoCiencia { get; set;}
        public string DataHoraEnvio { get; set;}
        public string DataHoraConfirmacao { get; set; }
        public string Resposta { get; set; }
        public string GerouNotificacao { get; set; }
        public string DthGerouNotificacao { get; set; }
        public string UsuarioConfirmacao { get; set; }
        public string EquipeMonitor { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    
    public class RelatorioVendasReprovadasScoreViewModel
    {
        public long Id { get; set; }
        public string DthVenda { get; set; }
        public string DthCriacao { get; set; }
        public long? IdCliente { get; set; }
        public string  DescCliente { get; set; }
        public long? IdVendedor { get; set; }
        public string DescVendedor { get; set; }
        public long? IdSupervisor { get; set; }
        public string DescSupervisor { get; set; }
        public string Status { get; set; }
        public string Score { get; set; }
        public string MotivoJustificativaVenda { get; set; }
        public string EtapaVenda { get; set; }
        public string StatusConsultaCredito { get; set; }
        public string DescCampanha { get; set; }
    }

    public class Relatorio_Contrato_Cliente
    {
        public String CodCliente { get; set; }
        public String NomeCliente { get; set; }
        public String Contrato { get; set; }
        public String DTCriacao { get; set; }
        public String DTAceite { get; set; }
        public String DTFinal { get; set; }
        public String Status { get; set; }
        public String DTSuspensao { get; set; }
        public String DTCancelado { get; set; }
        public double Valor { get; set; }
        public double Desconto { get; set; }
        public double Acressimo { get; set; }
        public double ValorTotal { get; set; }
        public String Bairro { get; set; }
        public String Cidade { get; set; }


    }
    public class Relatorio_Contrato_ClienteDataTableRequest : DataTableRequest
    {
        public String CodCliente { get; set; }
        public String NomeCliente { get; set; }
        public String Contrato { get; set; }
        public String DTCriacao { get; set; }
        public String DTAceite { get; set; }
        public String DTFinal { get; set; }
        public String Status { get; set; }
        public String DTSuspensao { get; set; }
        public String DTCancelado { get; set; }
        public Int64? Valor { get; set; }
        public String Bairro { get; set; }
        public String Cidade { get; set; }
        public String Data_Inicial { get; set; }
        public String Data_Final { get; set; }
        public Int32? TipoData { get; set; }

    }

    public class RelatorioMesmaVendaEnderecoViewModel : DataTableRequest
    {
        public string TipoData { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string TipoStatus { get; set; }
        public string Status { get; set; }
        public string EnderecoPor { get; set; }
        public string CEP { get; set; }
        public string Pais { get; set; }
        public string UF { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Campanha { get; set; }
        public string Valor { get; set; }
        public string ValorFinal { get; set; }
        public string IdCondominioPredio { get; set; }
        public RelatorioQtdeVendas? QtdeVendasMesmoEndereco { get; set; }
        public List<long?> IdsMotivos { get; set; }

    }

    public class RelatorioMesmaVendaEnderecoResponseViewModel
    {
        public string EnderecoPor { get; set; }
        public string Endereco { get; set; }
        public int QtdeVendasMesmoEndCliente { get; set; }
        public string DataCriacaoString { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string DataVendaString { get; set; }
        public DateTime? DataVenda { get; set; }
        public string StatusVenda { get; set; }
        public string MotivoJustificativaVenda { get; set; }
        public string Cliente { get; set; }
    }


    public enum RelatorioQtdeVendas : int
    {
        MaiorQue = 1,
        MenorQue,
        MaiorOuIgual,
        MenorOuIgual,
        Igual,
        Entre
    }
}