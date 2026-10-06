using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using Org.BouncyCastle.Asn1.Mozilla;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Dashboard_Atendimento_Models
    {
        public virtual String Atendimentos_Pendentes { get; set; }
        public virtual String Total_Atendimentos_Mes_Usuario { get; set; }
        public virtual String Total_Atendimentos_Mes_Geral { get; set; }
        public virtual String Tempo_Medio_Atendimento { get; set; }
        public virtual String Atendimentos_Pendentes_Equipe { get; set; }
        public virtual String A_Iniciar { get; set; }
        public virtual String Em_Atendimento { get; set; }
        public virtual String Aguardando { get; set; }


        public virtual String Risco_Perda_Prazo { get; set; }
        public virtual String Prazo_Vencido { get; set; }
        public virtual String Vencimento_Proximo { get; set; }

        public virtual String Atencao { get; set; }
        public virtual String Dentro_Prazo { get; set; }


        public virtual List<FlagsAtendimento> Flags_Atendimento { get; set; }


        public virtual Boolean Chat { get; set; }
        public virtual Boolean Novidades_Sinapse { get; set; }
    }

    public class Rankprod
    {
        public virtual String Nome { get; set; }
        public virtual Int32 Qtde_OS { get; set; }

    }

    public class Dashboard_Servicos_Request
    {
        public virtual String DataReferencia { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Monitor { get; set; }
        public virtual String NaturezaServico { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual TipoFiltroData? TipoFiltroData { get; set; }

    }

    public class TopEncerramentos_Request : DataTableRequest
    {
        public virtual String DataReferencia { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Monitor { get; set; }
        public virtual String Tecnico { get; set; }
        public virtual String NaturezaServico { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String TipoStatusExecucao { get; set; }
        public virtual TipoFiltroData? TipoFiltroData { get; set; }
    }

    public class ServicosProxVencimentoPerdaPrazoModel_Request : DataTableRequest
    {
        public virtual String DataReferencia { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Tecnico { get; set; }
        public virtual String NaturezaServico { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual TipoFiltroData? TipoFiltroData { get; set; }
    }

    public class PainelIndicadoresModel_Request : DataTableRequest
    {
        public virtual String DataReferencia { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Tecnico { get; set; }
        public virtual String NaturezaServico { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String tipoFiltroRegional { get; set; }
        public virtual TipoFiltroData? TipoFiltroData { get; set; }
    }


    public class ServicosStatusExecucaoModel_Request
    {
        public virtual String DataReferencia { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Tecnico { get; set; }
        public virtual String NaturezaServico { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual TipoFiltroData? TipoFiltroData { get; set; }
    }
    public class ServicosFechadosProdImprodModel_Request : DataTableRequest
    {
        public virtual String DataReferencia { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Tecnico { get; set; }
        public virtual String NaturezaServico { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual TipoFiltroData? TipoFiltroData { get; set; }
    }

    public enum TipoFiltroData
    {
        DataAtual = 0,
        EstaSemana = 1,
        UltimosSeteDias = 2,
        EsteMes = 3,
        UltimosTrintaDias = 4
    }

    public class Dashboard_Servicos_Models
    {
        public virtual String Servicos_Sem_Agendamento { get; set; }
        public virtual String Servicos_Nao_Roteirizados { get; set; }
        public virtual String Servicos_Nao_Iniciados { get; set; }
        public virtual String Servicos_Paralisados { get; set; }

        public virtual String Servicos_Em_Execucao { get; set; }
        public virtual String Servicos_Fora_Prazo { get; set; }
        public virtual String Servicos_Encerrados { get; set; }

        public virtual String Produtividade_Mes { get; set; }
        public virtual String Seta_Prod_Mes { get; set; }
        public virtual String Custo_Medio_Servico { get; set; }
        public virtual String Seta_Custo_Medio_Servico { get; set; }

        public virtual String Percentual_Servico_Fora_Prazo { get; set; }
        public virtual String Seta_Pct_Servico_Fora_Prazo { get; set; }

        public virtual String Media_Atraso { get; set; }
        public virtual String Seta_Media_Atraso { get; set; }

        public virtual List<Rankprod> Ranking_Produtividade_Mes { get; set; }
        public virtual List<Rankprod> Ranking_Produtividade_Mes_Supervisor { get; set; }

        public virtual Boolean Chat { get; set; }
        public virtual Boolean Novidades_Sinapse { get; set; }
        public virtual String MetaBacklogReparo { get; set; }
        public virtual String MetaBancoInstalacao { get; set; }
        public virtual String MetaEficienciaMesInstalacao { get; set; }
        public virtual String MetaEficienciaMesReparo { get; set; }
        public virtual String MetaEficienciaDiaInstalacao { get; set; }
        public virtual String MetaEficienciaDiaReparo { get; set; }
        public virtual String MetaEficienciaDiaMultiskill { get; set; }
        public virtual String MetaEficienciaDiaTotal { get; set; }
        public virtual String MetaReclamacaoInicial { get; set; }
        public virtual String MetaRepetidas { get; set; }
        public virtual String MetaAtendimentoJanelaInstalacao { get; set; }
        public virtual String MetaAtendimentoJanelaReparo { get; set; }
        public virtual String MetaAtendimentoJanelaMultiskill { get; set; }
        public virtual String MetaCustoMedioInstalacao { get; set; }
        public virtual String MetaCustoMedioReparo { get; set; }
        public virtual String DataReferencia { get; set; }
    }

    public class TecnicosHabilidadesModel
    {
        public virtual int Instalacao { get; set; }
        public virtual int Reparo { get; set; }
        public virtual int Multiskill { get; set; }

    }

    public class TopEncerramentosModel
    {
        public virtual string Descricao { get; set; }
        public virtual int Total { get; set; }
        public virtual string Percentual { get; set; }
    }
    public class Resultado
    {
        public Turno_ServicoAniel Turno { get; set; }
        public int DocProdCount { get; set; }
    }
    public class ResultadoFinal
    {
        public string Id { get; set; }
        public int Valor { get; set; }
    }
    public class DocumentosPainelIndicadores
    {
        public virtual string Cod { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Escritorio { get; set; }
        public virtual string PorcRev1 { get; set; }
        public virtual string PorcRev3 { get; set; }
        public virtual string PorcSLA { get; set; }
        public virtual string PorcRTI { get; set; }
        public virtual string PorcTEC1 { get; set; }
        public virtual string PorcAGE1 { get; set; }
        public virtual string PorcProdu { get; set; }
        public virtual string Id { get; set; }
    }

    public class ResultadoAgrupado
    {
        public string Natureza { get; set; }
        public string Descricao { get; set; }
        public Turno_ServicoAniel Turno { get; set; }
        public int DocProdCount { get; set; }
    }

    public class ServicosStatusExecucaoModel
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public bool ativo { get; set; }
        public string total_os { get; set; }
        public string total_os_encaixe { get; set; }
        public string rev1 { get; set; }
        public string rev3 { get; set; }
        public string[] periodos { get; set; }
        public List<ResultadoFinal> lista { get; set; }
    }


    public class ServicosProxVencimentoPerdaPrazoModel
    {
        public virtual string Id_Natureza { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Sla_hoje { get; set; }
        public virtual string Roteirizado { get; set; }
        public virtual string SemRoteirizar { get; set; }
        public virtual string Backlog { get; set; }
        public virtual string Backlog4 { get; set; }
        public virtual string SemAgendamento { get; set; }

    }
    public class ServicosIndicadoresModel
    {
        public virtual string Descricao { get; set; }
        public virtual string Produtiva { get; set; }
        public virtual string Rti { get; set; }
        public virtual string Rev1 { get; set; }
        public virtual string Rev3 { get; set; }
        public virtual string Sla { get; set; }
        public virtual string Tec1 { get; set; }

    }

    public class BancoInstalacaoModel
    {
        public virtual int Total { get; set; }
        public virtual int SeteDias { get; set; }
        public virtual int DezDias { get; set; }
        public virtual int QuinzeDias { get; set; }
        public virtual int TrintaDias { get; set; }
        public virtual int MaisTrintaDias { get; set; }
        public virtual int MetaCumprida { get; set; }

    }

    public class BacklogReparoModel
    {
        public virtual int Total { get; set; }
        public virtual int VinteQuatroHoras { get; set; }
        public virtual int TrintaSeisHoras { get; set; }
        public virtual int QuarentaOitoHoras { get; set; }
        public virtual int SetentaDuasHoras { get; set; }
        public virtual int CentoVinteHoras { get; set; }
        public virtual int MaisCentoVinteHoras { get; set; }
        public virtual int MetaCumprida { get; set; }

    }

    public class EficienciaMesModel
    {
        public virtual int Total { get; set; }
        public virtual double SeteDias { get; set; }
        public virtual double QuinzeDias { get; set; }
        public virtual int? MetaSeteDias { get; set; }
        public virtual int? MetaQuinzeDias { get; set; }
        public virtual double MediaProdutiva { get; set; }
        public virtual int MetaCumprida { get; set; }

    }

    public class EficienciaDiaModel
    {
        public virtual double Instalacao { get; set; }
        public virtual int? MetaInstalacao { get; set; }
        public virtual double Reparo { get; set; }
        public virtual int? MetaReparo { get; set; }
        public virtual double Outros { get; set; }
        public virtual int? MetaOutros { get; set; }
        public virtual double Total { get; set; }
        public virtual int? MetaTotal { get; set; }

    }

    public class RankingTecnicos_Model
    {
        public virtual string Matricula { get; set; }
        public virtual string Tecnico { get; set; }
        public virtual string Supervisor { get; set; }
        public virtual string Monitor { get; set; }
        public virtual string Empresa { get; set; }
        public virtual string Filial { get; set; }
        public virtual string CentroCusto { get; set; }
        public virtual string QuantidadeOS { get; set; }

    }

    public class MediaServicos_Model
    {
        public virtual string Matricula { get; set; }
        public virtual string Tecnico { get; set; }
        public virtual string DiasTrabalhados { get; set; }
        public virtual string ServicosProdutivos { get; set; }
        public virtual string Media { get; set; }
        public virtual string Supervisor { get; set; }
        public virtual string Empresa { get; set; }
        public virtual string Filial { get; set; }

    }

    public class RankingTecnicos_DataTableRequest : DataTableRequest
    {
        public virtual string Ano { get; set; }
        public virtual string Mes { get; set; }
        public virtual bool Improdutiva { get; set; }
        public virtual bool Produtiva { get; set; }
        public virtual bool VisitaTecnica { get; set; }
        public virtual string Empresa { get; set; }
        public virtual string Filial { get; set; }
        public virtual string Supervisor { get; set; }
        public virtual string Monitor { get; set; }
        public virtual string CentroCusto { get; set; }
        public virtual string GrupoArea { get; set; }
        public virtual string Area { get; set; }
        public virtual string Localidade { get; set; }
        public virtual string Natureza { get; set; }
        public virtual string Uf { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string Agrupamento { get; set; }

    }

    public class RelatorioMediaServicoExecutadoRequest : DataTableRequest
    {
        public virtual string Inicial { get; set; }
        public virtual string Final { get; set; }
        public virtual string Tecnico { get; set; }
        public virtual string TipoServico { get; set; }
        public virtual string PlanoProduto { get; set; }
        public virtual string Empresa { get; set; }
        public virtual string Filial { get; set; }
        public virtual string Supervisor { get; set; }
        public virtual string GrupoArea { get; set; }
        public virtual string Area { get; set; }
        public virtual string Localidade { get; set; }
        public virtual string Natureza { get; set; }
        public virtual string Uf { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string Agrupamento { get; set; }

    }

    public class Dashboard_Vendas_Models
    {
        public virtual Double Minhas_Vendas { get; set; }


        public virtual Double Vendas_Pendentes_Mes { get; set; }
        public virtual Double Vendas_Pendentes_Equipe_Mes { get; set; }

        public virtual Double Vendas_Aprovadas { get; set; }

        public virtual Double Vendas_Aprovadas_Mes { get; set; }
        public virtual Double Vendas_Aprovadas_Equipe { get; set; }

        public virtual Double Vendas_Aprovadas_Equipe_Mes { get; set; }

        public virtual String Vendas_Perdidas { get; set; }
        public virtual String Vendas_Perdidas_Mes { get; set; }
        public virtual String Vendas_Reprovadas_Mes { get; set; }
        public virtual String Vendas_Canceladas_Mes { get; set; }

        public virtual Double Vendas_Equipe { get; set; }

        public virtual String Vendas_Perdidas_Equipe { get; set; }
        public virtual String Vendas_Perdidas_Equipe_Mes { get; set; }
        public virtual String Vendas_Reprovadas_Equipe_Mes { get; set; }
        public virtual String Vendas_Canceladas_Equipe_Mes { get; set; }

        public virtual Boolean Chat { get; set; }
        public virtual Boolean Novidades_Sinapse { get; set; }
        public Enum_VisualizacaoDashboard? VisualizacaoDashboard { get; set; }
    }

    public class FlagsAtendimento
    {
        public virtual String Cor { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Double Valor { get; set; }
    }

    public class SLA_Count_Horas
    {
        public virtual Int32 Minutos { get; set; }
        public virtual Int32 Count { get; set; }
    }


    public class Dashboard_Inicial_Models
    {
        public virtual String Atendimentos_Pendentes { get; set; }
        public virtual String Total_Atendimentos_Mes_Usuario { get; set; }
        public virtual String Total_Atendimentos_Mes_Geral { get; set; }
        public virtual String Tempo_Medio_Atendimento { get; set; }
        public virtual String Atendimentos_Pendentes_Equipe { get; set; }
        public virtual String A_Iniciar { get; set; }
        public virtual String Em_Atendimento { get; set; }
        public virtual String Aguardando { get; set; }


        public virtual String Risco_Perda_Prazo { get; set; }
        public virtual String Prazo_Vencido { get; set; }
        public virtual String Vencimento_Proximo { get; set; }

        public virtual String Atencao { get; set; }
        public virtual String Dentro_Prazo { get; set; }


        public virtual List<FlagsAtendimento> Flags_Atendimento { get; set; }


        public virtual Boolean Chat { get; set; }
        public virtual Boolean Novidades_Sinapse { get; set; }
    }
    public class DashBoardVendasFiltro 
    {
        public EnumPeriodoFitlroDashVendas DataFiltrada { get; set; } 
        public int FormatoVisualizacao { get; set; } 
    }
}