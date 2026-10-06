using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.HubDesenvolvedor;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public enum ContratoEnum
    {
        Novo = 0,
        Existente = 1
    }
    public class VendaPasso1ViewModel
    {

        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        public Tipo_Pessoa TipoPessoa { get; set; }

        [Display(Name = "CPF", ResourceType = typeof(Resources.RsCadastros))]
        public String CPF { get; set; }

        [Display(Name = "CNPJ", ResourceType = typeof(Resources.RsCadastros))]
        public String CNPJ { get; set; }

        [Display(Name = "DATANASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public String DataNascimento { get; set; }

        public ResponseHub<ResponseCpf> retornoCPF { get; set; }

        public ResponseHub<ResponseCnpj> retornoCNPJ { get; set; }
    }

    public class VendaListaBuscaAtendimento : ObjBusca<Int64>
    {
        public virtual String CPF_CNPJ_Cliente { get; set; }

        public virtual String Categoria_Cliente { get; set; }

        public virtual String Telefone_1 { get; set; }
        public virtual String Telefone_Celular { get; set; }
        public virtual String Telefone_Recado { get; set; }
        public virtual String Email_Principal { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }

        public virtual String Endereco_Instalacao { get; set; }
        public virtual String Login_Radius { get; set; }
        public virtual String Valor_Final_Total { get; set; }
        public virtual String Id_Endereco_Cliente { get; set; }

        public virtual List<PlanoProdutoComboAntendimentoViewModel> PlanosProdutos { get; set; }
    }

    public class HistoricoAtendimentoModel
    {
        public virtual String DataHoraAtendimento { get; set; }
        public virtual String Assunto { get; set; }
        public virtual String StatusVenda { get; set; }
        public virtual String Dt_Inicial_Filtro { get; set; }
        public virtual String Dt_Final_Filtro { get; set; }
    }

    public class Historico_UltimosContatos_AcaoStatusModel
    {
        public Int64 Id_venda { get; set; }
        public String IdCliente { get; set; }

        public virtual String DataHoraAtendimento { get; set; }
        public virtual String Assunto { get; set; }
        public virtual String StatusVenda { get; set; }
        public virtual String Usuario { get; set; }

        public virtual String Data_Ultimo_Contato { get; set; }
        public virtual String Forma_Contato { get; set; }
        public virtual String Observacao { get; set; }

        public virtual String Acao { get; set; }
        public virtual String Valor { get; set; }
        public virtual String Valor_Atualizado { get; set; }

        public virtual String Dt_Inicial_Filtro { get; set; }
        public virtual String Dt_Final_Filtro { get; set; }
    }

    public class VendaListViewModel
    {
        public Int64 Id { get; set; }

        public Boolean Pendencia { get; set; }
        public String Supervisor { get; set; }
        public String Vendedor { get; set; }
        public Int64 IdVendedor { get; set; }

        public String Departamento { get; set; }
        public String UsuarioAprovacao { get; set; }
        public String UsuarioCancelamento { get; set; }
        public String UsuarioAprovacaoCancelamento { get; set; }
        public String Plano { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        public String StatusAtutomatico { get; set; }
        public Status_Venda StatusManual { get; set; }

        public String Data_Encerramento { get; set; }
        public String DataHoraAprovacao { get; set; }
        public String DataHoraCancelamento { get; set; }
        public String DiasSemContato { get; set; }

        [Display(Name = "NOMEFANTASIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string NomeFantasia { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string CpfCnpj { get; set; }

        [Display(Name = "CATEGORIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Categoria { get; set; }

        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Login { get; set; }

        [Display(Name = "ESTADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Estado { get; set; }

        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Cidade { get; set; }

        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Bairro { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }

        [Display(Name = "TIPO_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Tipo_Imovel { get; set; }
        public string Data_Expiracao { get; set; }
        public string Avaliacao_Expirada { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Categoria_Cliente { get; set; }
        public Perfil_Vendedor Perfil_Vendedor_Id { get; set; }
        [Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Forma_Pagamento { get; set; }

        [Display(Name = "IDENTIFICACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Identificacao_Cliente { get; set; }

        [Display(Name = "TELEFONEPRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefonePrincipal { get; set; }
        public String TelefoneCelular { get; set; }
        public String Email { get; set; }
        public String Profissao { get; set; }

        [Display(Name = "DATA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String DataVenda { get; set; }
        public String DataMudEtapa { get; set; }

        public String DataUltimoContato { get; set; }

        public String Meio_Captacao { get; set; }

        public string Quem_Indicou { get; set; }
        public String Data_Hora_Criacao { get; set; }
        public String Tipo_Cliente { get; set; }
        public String Tipo_Pessoa { get; set; }

        public Int64 Id_Cliente { get; set; }

        [Display(Name = "TERMO_ADESAO_ASSINADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Termo_Adesao_Assinado { get; set; }
        public String Data_Termo_Assinado { get; set; }
        public String EquipeVenda { get; set; }
        public String Setor_Responsavel { get; set; }
        public String Responsavel { get; set; }

        public String Etapa_Atual { get; set; }
        public String Motivo_Desistencia_Venda { get; set; }
        public String Checklist { get; set; }
        public String Data_Preenchimento_Checklist { get; set; }
        public String Id_Contrato { get; set; }
        public String Valor_Total_Venda { get; set; }
        public String Valor_Instalacao { get; set; }
        public String Status_Auditoria { get; set; }
    }

    public class ConsultaVendasOrdemServicoListModel
    {
        public long IdVenda { get; set; }

        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public StatusExecucaoAniel? Status { get; set; }
    }

    public class ServicosVendaListModel
    {
        public string CodCt { get; set; }
        public string Projeto { get; set; }
        public string NumObra { get; set; }
        public string NumObraOriginal { get; set; }
        public string NumDoc { get; set; }

        public string DataAgendamento { get; set; }

        public string TipoServico { get; set; }
        public string Tecnico { get; set; }
        public string Sla { get; set; }

        public string Status { get; set; }

        public string InicioAtendimento { get; set; }
        public string FimAtendimento { get; set; }
    }

    public class ServicosVendaDataTableRequest : DataTableRequest
    {
        public long IdVenda { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string Status { get; set; }
    }

    public class Venda_DataTableRequest : DataTableRequest
    {
        public String StatusAuditoria { get; set; }
        public String StatusEtapa { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.StatusVenda)]
        public String Status { get; set; }
        public String StatusVenda { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.EtapaVenda)]
        public String EtapaVenda { get; set; }

        public String Estado { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Cidade)]
        public String Cidade { get; set; }

        public String Bairro { get; set; }

        public String Data_Inicial { get; set; }

        public Int32? TipoData { get; set; }

        public String Data_Final { get; set; }

        public String Ultimo_Contato { get; set; }
        public String SetorResponsavel { get; set; }
        public String Responsavel { get; set; }
        public String Contrato { get; set; }
        public String Porta_Reservada { get; set; }        

        public String Vencimento { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Vendedor)]
        public String Vendedor { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Supervisor)]
        public String Supervisor { get; set; }

        public String Meio_Captacao { get; set; }
        public long? Campanha { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Tipo_Imovel)]
        public String Tipo_Imovel { get; set; }

        public Int64? Id_Plano_Produto { get; set; }
        public Int64? Id_Combo { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Plano)]
        public Int64? Id_Plano { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Produto)]
        public Int64? Id_Produto { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Categoria_Cliente)]
        public String Categoria_Cliente { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Motivo_Desistencia_Venda)]
        public String Motivo_Desistencia_Venda { get; set; }

        public Int64? Id_Cliente { get; set; }
        public Int64? Id_Modelo { get; set; }

        public String termo_Adesao_Assinado { get; set; }
        public String qtde_Dias_Sem_Contato { get; set; }
        public String AcessoUsuario { get; set; }
        public String AcessoProspect { get; set; }
        public virtual String Tipo_Pessoa { get; set; }
        public String acessoTempoVisualização { get; set; }
        public String AcessoDepartamento_Setor { get; set; }
        public String EquipeVenda { get; set; }    
        public String Etapa_Atual { get; set; }
        public bool ApenasInstalados { get; set; }
        public bool OrigemDashBoard { get; set; }
        public List<string> ConsultaRestricao { get; set; } = new List<string>();
    }

    public class Venda_ModeloAttribute : Attribute
    {
        public Aniel.Connect.Model.enumerado.Filtro_PainelVendaEnum Recurso { get; set; }
        public Venda_ModeloAttribute(Filtro_PainelVendaEnum value)
        {
            Recurso = value;
        }
    }

    public class VendaAlterarStatusViewModel
    {
        public Int64 id { get; set; }
        public Int16 status { get; set; }
        public String usuario { get; set; }
        public Int64? id_motivo_cancelamento { get; set; }
        public String justificativa { get; set; }
        public String status_atual { get; set; }
        public String numero_contrato_cliente { get; set; }
        public Int64? motivo_desistencia_venda { get; set; }
        public Int64? motivo_aprovacao_venda { get; set; }
        public int? situacao { get; set; }
        public Int64? idWorkflowAtividade { get; set; }
        public String descricaoAtividadeStatusVenda { get; set; }
    }
    public class IntegrarClienteComercial
    {
        public long Venda { get; set; }
        public IntegracaoExterna Integracao { get; set; }
    }
    public class IntegraClienteIXC : IntegrarClienteComercial
    {
    }
    public class VendaAlterarStatusEmMassaViewModel
    {
        public List<VendaAlterarStatusViewModel> listaVenda { get; set; }
    }

    public class VendasNaoCanceladas
    {
        public Int64 NumeroVenda { get; set; }
        public string Status { get; set; }
        public string TermoAdesaoAssinado { get; set; }
        public string OS { get; set; }
        public string Motivo { get; set; }


        public VendasNaoCanceladas()
        {
            OS = "N/A";
        }

    }


    #region DashBoard de Vendas

    public class Dashboard_VendasListModel
    {
        public Int64 Id { get; set; }
        public String Nome { get; set; }
        public String Supervisor { get; set; }
        public String Meta { get; set; }
        public Double Pontuacao_Mensal { get; set; }
        public Double Pontuacao_Diaria { get; set; }
        public Double Dados { get; set; }
        public String Meta_Dia { get; set; }
        public String PorVenda_Mes { get; set; }
        public String PorVenda_Dia { get; set; }
        public String Vendas_Mes { get; set; }
        public String Vendas_Hab_Mes { get; set; }
        public String Vendas_Dia { get; set; }
        public String Vendas_Hab_Dia { get; set; }
        public String Vendas_Can_Dia { get; set; }
        public String Data_Venda { get; set; }

        public String Localizacao { get; set; }
    }

    public class Dashboard_Vendas_ItemListModel
    {
        public Int64? Id_Funcionario { get; set; }
        public String Pontos { get; set; }
        public String Valor_Dinheiro { get; set; }
        public String Nome_Plano_Produto { get; set; }
        public String Nome_Campanha { get; set; }
    }

    public class Dashboard_Vendas_DataTableRequest : DataTableRequest
    {

        public Int64? Id_Funcionario { get; set; }
        public String Supervisor { get; set; }

    }

    #endregion

    #region MOBILE ConsultaVendas 
    public class ConsultaVenda
    {

        public String Cpf { get; set; }
        public String Nome { get; set; }
        public String Bairro { get; set; }
        public String Cidade { get; set; }
        public String Contato { get; set; }
        public int Status { get; set; }
        public String nome_vendedor { get; set; }
    }
    #endregion


    #region MOBILE ConsultaVendas
    public class ConsultaVendaPorDataOuCPFOuNome
    {

        public String Cpf { get; set; }
        public String Nome { get; set; }
        public String Bairro { get; set; }
        public String Cidade { get; set; }
        public String Contato { get; set; }
        public int Status { get; set; }
        public String nome_vendedor { get; set; }
        public String data_inicio { get; set; }
        public String data_fim { get; set; }

    }
    #endregion

    public class Minhas_Pendencias_DadosUltimoContato
    {
        public Int64 Id_venda { get; set; }
        public DateTime DataUltimoContato { get; set; }
        public String Assunto { get; set; }
        public String Observacao { get; set; }
    }

    public class Json_VendaTelefonia
    {
        public TelefoniaViewObject Lc { get; set; }
        public TelefoniaViewObject Ldi { get; set; }
        public TelefoniaViewObject Ldn { get; set; }
        public TelefoniaViewObject Vc1 { get; set; }
        public TelefoniaViewObject Vc2 { get; set; }
        public TelefoniaViewObject Vc3 { get; set; }
        //public TelefoniaViewObject Bina { get; set; }
        //public TelefoniaViewObject BlackList { get; set; }
        public long? Numero_dids { get; set; }
        public Tipo_Tecnologia Tipo_Tecnologia { get; set; }
    }
    public class TelefoniaViewObject
    {
        public virtual bool? Habilitar { get; set; }
        public virtual double? Valor { get; set; }
        public virtual String cadencia { get; set; }
    }

    public class HistoricoAlteracaoVenda_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
    }

    public class ConsultaVendaHistorico_DataTableRequest : DataTableRequest
    {
        public virtual String Dt_Inicial_Filtro { get; set; }
        public virtual String Dt_Final_Filtro { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int32? Id_Venda { get; set; }
        public virtual Int64? Id_Log { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual Status_Atendimento? Status { get; set; }
        public virtual Status_SLA_Atendimento? Sla { get; set; }

    }

    public class RestricoesConsultaCredito
    {
        public string cpf { get; set; }
    }

    public class RestricoesConsultaCreditoVenda
    {
        public long IdVenda { get; set; }
        public long IdTroca { get; set; }
    }

    public class RetornoRestricaoCreditoListCpf
    {        
        public string N200SituacaoDocumento { get; set; }
        public String scoreSerasa { get; set; }
        public String debitosTotais { get; set; }
        public String dataultimadebito { get; set; }
        public String inadimplenciaProbab { get; set; }
        public String msgScore { get; set; }
        public String faixaScore { get; set; }
        public String modalidade { get; set; }
        public String avalista { get; set; }
        public String moeda { get; set; }
        public String valorDebito { get; set; }
        public String valortotalDebito { get; set; }
        public String contrato { get; set; }
        public String origem { get; set; }
        public String totalOcorrenciasDebitos { get; set; }
        public String subtipo { get; set; }
        public String T999mensagem { get; set; }
        public String dataOcorrencia { get; set; }
        public String numCheque { get; set; }
        public String alineaCheque { get; set; }
        public String qdadeCCFparaBanco { get; set; }
        public String numBanco { get; set; }
        public String nomeBanco { get; set; }
        public String numAgencia { get; set; }
        public String ufAgencia { get; set; }
        public String valorTotalCheque { get; set; }
        public String cidadeAgencia { get; set; }
        public String N250ProtestoDataOcorrencia { get; set; }
        public String N250ProtestoCartorio { get; set; }
        public String N250ProtestoCidadeOrigem { get; set; }
        public String N250ProtestoUf { get; set; }
        public String N250ProtestosMoeda { get; set; }
        public String N250ProtestosValorTotal { get; set; }
        public String protestoMsgSubjudice { get; set; }
        public String N250ProtestoProtestoTotalOcorrencia { get; set; }
        public String N250ProtestoDataOcorrenciaAntiga { get; set; }
        public String N250ProtestoDataOcorrenciaRecente { get; set; }        
    }
    public class RetornoRestricaoCreditoListCnpj
    {
        public string N200SituacaoDocumento { get; set; }
        public String N250ProtestoDataOcorrencia { get; set; }
        public String N250ProtestoCartorio { get; set; }
        public String N250ProtestoCidadeOrigem { get; set; }
        public String N250ProtestoUf { get; set; }
        public String N250ProtestosMoeda { get; set; }
        public String N250ProtestosValorTotal { get; set; }
        public String protestoMsgSubjudice { get; set; }
        public String N250ProtestoProtestoTotalOcorrencia { get; set; }
        public String N250ProtestoDataOcorrenciaAntiga { get; set; }
        public String N250ProtestoDataOcorrenciaRecente { get; set; }
        public String N440ConsultasSerasaCheques { get; set; }
        public String N440ConsultasSerasaMercado { get; set; }
        public String N440ConsultasSerasaMercadoRefComercial { get; set; }
        public String N440TotalChequesEmitidosaVista15Dias { get; set; }
        public String N440TotalChequesEmitidosaPrazo30Dias { get; set; }
        public String N440TotalChequesEmitidosaPrazo60Dias { get; set; }
        public String N440TotalChequesEmitidosaPrazo { get; set; }
        public String N440ConsultasSerasaSemCheques { get; set; }
        public String N700SocioDocumento { get; set; }
        public String N700SocioNome { get; set; }
        public String N700TipoPessoa { get; set; }
        public String N700SocioPercentualCapital { get; set; }
        public String N700SocioRestricao { get; set; }
        public String N700Mensagem { get; set; }
        public String N705 { get; set; }
        public String scoreSerasa { get; set; }
        public String debitosTotais { get; set; }
        public String dataultimadebito { get; set; }
        public String inadimplenciaProbab { get; set; }
        public String msgScore { get; set; }
        public String faixaScore { get; set; }
        public String modalidade { get; set; }
        public String avalista { get; set; }
        public String moeda { get; set; }
        public String valorDebito { get; set; }
        public String contrato { get; set; }
        public String origem { get; set; }
        public String totalOcorrenciasDebitos { get; set; }
        public String subtipo { get; set; }
        public String ChequedataOcorrencia { get; set; }
        public String numCheque { get; set; }
        public String valorTotalCheque { get; set; }
        public String alineaCheque { get; set; }
        public String qdadeCCFparaBanco { get; set; }
        public String ChequenumBanco { get; set; }
        public String ChequenomeBanco { get; set; }
        public String ChequenumAgencia { get; set; }
        public String ChequeufAgencia { get; set; }
        public String ChequevalorTotalCheque { get; set; }
        public String ChequecidadeAgencia { get; set; }
        public String ScoreRetornoInformacoes { get; set; }
        public String ScoreDataConsulta { get; set; }
        public String ScoreHoraConsulta { get; set; }
        public String ClasseTabelaScore { get; set; }
        public String MsgInformativaPj { get; set; }
        public String CalculoRisco { get; set; }
        public String N240ContemOcorrencia { get; set; }
        public String N250ContemOcorrencia { get; set; }
        public String N270ContemOcorrencia { get; set; }
        public String N440ContemOcorrencia { get; set; }
        public String N700ContemOcorrencia { get; set; }
        public String F900ContemOcorrencia { get; set; }
        public String T999mensagem { get; set; }
        public String valortotalDebito { get; set; }
    }
    public class Consulta_SerasaCnpjRequestModels
    {
        public string cnpj { get; set; }
    }

    public class VincularAtendimentoVendaRequestModels : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public Venda IdVenda { get; set; }
        public Atendimento IdAtendimento { get; set; }

    }
    public class Consulta_SerasaCpfRequestModels
    {
        public long? IdVenda { get; set; }
        public long? IdTroca { get; set; }
        public string cpf { get; set; }
    }
    public class Consulta_SerasaContagemRegistrosModel
    {
        public int ContagemRegistros { get; set; }
        public bool ConsultaMesmaHora { get; set; }
        public String PeriodoUltimaConsulta { get; set; }
    }

    #region MOBILE ExtratoVendas
    public class ExtratoVendas
    {
        public Int64 IdVenda { get; set; }
        public Int64 IdCampanha { get; set; }
        public String Titulo { get; set; }
        public String PlanoProduto { get; set; }
        public String NomeCliente { get; set; }
        public String Cpf { get; set; }
        public DateTime DataVenda { get; set; }
        public Int64? Pontos { get; set; }
        public String Vendedor { get; set; }

    }
    #endregion

    public class VendasTermoDatatableRequest : DataTableRequest 
    {
        public long? IdVenda { get; set; }
        public long? IdTroca { get; set; }
        public long? IdCliente { get; set; }
    }
    public class AnexosTermo
    {
        public Int64 Id { get; set; }
        public String Nome { get; set; }
        public String Extensao { get; set; }
        public String IdVenda { get; set; }
        public String TipoDocumento { get; set; }
        public String Status { get; set; }
        public String Lado { get; set; }
        public String Url { get; set; }
    }
    public class GravarMotivoAprovacaoVDManualRequest
    {
        public long IdVenda { get; set; }
        public long IdTroca { get; set; }
        public long Etapa { get; set; }
        public string MotivoAprovacao { get; set; }

    }
    public class VendasTermo
    {
        public Int64 Id { get; set; }
        public String NomeCliente { get; set; }
        public String CpfCnpj { get; set; }
        public bool TermoAssinado { get; set; }
        public bool AnexoCriado { get; set; }
        public String DataAceite { get; set; }
        public String Ip { get; set; }
        public String Status { get; set; }
        public string UrlTermo { get; internal set; }
        public List<Object> ListaClientesContratos { get; set; }
        public Int64? Status_Venda { get; set; }
        public Int64? Status_Etapa_Venda { get; set; }
    }
    public class AssociaChecklistVendaRequest 
    {
        public long Id { get; set; }
    }

    public class AtualizaContrato
    {
        public long? IdContrato { get; set; }
        public string CodIntegracao { get; set; }
        public string CodConexao { get; set; }
        public string CodAtentimento { get; set; }
        public string CodOS { get; set; }
        public long IdClientePlanoProduto { get; set; }
        public string CodigoAgrupamento { get; set; }
    }

    public class EtapasIntegracaoIXC
    {
        public bool EtapaClienteIXC { get; set; }
        public IEnumerable<bool> EtapaContratoIXC { get; set; }
        public IEnumerable<bool> EtapaLoginIXC { get; set; }
        public IEnumerable<bool> EtapaAtendimentoIXC { get; set; }
        public string Mensagem { get; set; }
        public List<bool> EtapaOrdemServico { get; internal set; }
        public String Cliente { get; set; }
        public List<String> ListaContratos { get; set; }
        public List<String> ListaLogins { get; set; }
        public List<String> ListaAtendimentos { get; set; }
        public List<String> ListaOrdemServico { get; set; }
    }
    public class AnexosContratoAceiteListModel 
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public string NomeArquivo { get; set; }
        public bool ExigeAceite { get; set; }
        public string Extensao { get; set; }
        public string Url { get; set; }
    }
    public class AnexosContratoAceiteDatatableRequest: DataTableRequest
    {
        public long IdVenda { get; set; }
    }
    public class AnexosContratoAceiteRequestModel
    {
        public long IdVenda { get; set; }
        public string Descricao { get; set; }
        public bool ExigeAceite { get; set; }
        private List<AnexoContratoAceite> _arquivos { get; set; }
        public List<AnexoContratoAceite> Arquivos 
        { 
            get 
            { 
                if (_arquivos == null) 
                { 
                    _arquivos = new List<AnexoContratoAceite>(); 
                }
                return _arquivos;
            } 
            set { _arquivos = value; }
        }
    }
    public class AnexoContratoAceite
    {
        public string NomeArquivo { get; set; }
        public string Extensao { get; set; }
        public byte[] Dados { get; set; }
    }
    public class AlterarStatusAnexoRequest
    {
        public long Venda { get; set; }
        public StatusAnexoContrato Status { get; set; }
    }

    public class VendaOlaModel
    {
        public VendaOlaModel()
        {
            EnderecoInstalacao = new Repository.Model.EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Instalacao };
        }

        public Int64 Id { get; set; }
        public String DataCriacao { get; set; }
        public String StatusVenda { get; set; }
        public String Nome_Vendedor { get; set; }
        public Boolean Exige_Termo_Adesao { get; set; }
        public String urlAssinarTermoOla { get; set; }
        public Boolean Termo_Adesao_Assinado { get; set; }

        private List<Repository.Model.PlanoProdutoListModel> PlanosLista;

        public virtual List<Repository.Model.PlanoProdutoListModel> Planos

        {
            get
            {
                if (PlanosLista == null)
                    PlanosLista = new List<Repository.Model.PlanoProdutoListModel>();
                return PlanosLista;
            }
            set
            {
                PlanosLista = value;
            }
        }

        private List<Repository.Model.Forma_Pagamento_ClienteModels> Formas_PagamentoLista;

        public virtual List<Repository.Model.Forma_Pagamento_ClienteModels> Formas_Pagamento
        {
            get
            {
                if (Formas_PagamentoLista == null)
                    Formas_PagamentoLista = new List<Repository.Model.Forma_Pagamento_ClienteModels>();
                return Formas_PagamentoLista;
            }
            set
            {
                Formas_PagamentoLista = value;
            }
        }

        public Int64? Numero_Parcelas_Instalacao { get; set; }

        private List<Repository.Model.ClienteContratoListModel> ClienteContratoLista;

        public virtual List<Repository.Model.ClienteContratoListModel> ClienteContratos
        {
            get
            {
                if (ClienteContratoLista == null)
                    ClienteContratoLista = new List<Repository.Model.ClienteContratoListModel>();
                return ClienteContratoLista;
            }
            set
            {
                ClienteContratoLista = value;
            }
        }

        private List<Repository.Model.DescontoListaModel> DescontosLista;

        public virtual List<Repository.Model.DescontoListaModel> Descontos

        {
            get
            {
                if (DescontosLista == null)
                    DescontosLista = new List<Repository.Model.DescontoListaModel>();
                return DescontosLista;
            }
            set
            {
                DescontosLista = value;
            }
        }

        public String Numero_Contrato { get; set; }
        public string Valor_Final { get; set; }
        public Repository.Model.EnderecoClienteViewModel EnderecoInstalacao { get; set; }
    }    
}