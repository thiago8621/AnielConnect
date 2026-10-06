using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Padrao = Sinapse.Model.enumerados.Padrao;

namespace Aniel.Connect.Models
{
    public class AtendimentoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Numero { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Script { get; set; }

        public virtual String Prioridade_Padrao { get; set; }

        public virtual String Status { get; set; }
        public virtual String StatusAuditoria { get; set; }

        public virtual String Categoria_Atendimento { get; set; }

        public virtual Int64 Id_Motivo { get; set; }

        public virtual String Problema_Relatado { get; set; }

        public virtual String Assunto { get; set; }

        public virtual String Pessoa_Contato { get; set; }



        public virtual String Cliente { get; set; }
        public virtual Int64? Id_Categoria_Cliente { get; set; }
        public virtual String Categoria_Cliente { get; set; }

        public virtual String Origem_Atendimento { get; set; }
        public virtual String Origem { get; set; }

        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual String Tipo_Atendimento { get; set; }

        public virtual String Setor { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual String Motivo_Status { get; set; }

        public virtual String DataHoraCriacao { get; set; }

        public virtual String DataHoraFechamento { get; set; }

        public virtual String NomeAtendimento { get; set; }

        public virtual String Status_Atendimento { get; set; }

        public virtual String Sla { get; set; }

        public virtual Int32? SlaOrdem { get; set; }

        public virtual String Login_Radius { get; set; }
        public virtual String Email_Cliente { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Celular { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Produto { get; set; }
        public virtual String Id_Motivo_Status { get; set; }

        public virtual Boolean Tecnico_Ativos { get; set; }

        public virtual Boolean Tecnico_Em_Execucao { get; set; }
        public virtual String Etapa_Atual { get; set; }
        public virtual String Prazo_Atual { get; set; }
        public virtual String Tempo_Gasto { get; set; }
        public virtual String Data_Sla_Etapa { get; set; }
        public virtual String NumeroTrocaTitularidade { get; set; }
        public virtual string Organizacao_Cliente { get; set; }
        public virtual long? Id_Organizacao_Cliente { get; set; }
        public bool PossuiOs { get; set; }
    }

    public class AtendimentoChatListModel
    {
        public virtual int Id { get; set; }
        public virtual String Protocolo { get; set; }
        public virtual String Hora { get; set; }
        public virtual String UltimaMensagem { get; set; }
        public virtual int QuantidadeMensagens { get; set; }
        public virtual int ordenacao { get; set; }
        public virtual DateTime dataUltimaMensagem { get; set; }
        public virtual DateTime? dataAbertura { get; set; }
        public virtual String htmlMensagem { get; set; }
    }

    public class AcompanhamentoAgenteListaModel
    {

        public virtual String Status_Chat { get; set; }

        public virtual String Agente { get; set; }
        public virtual String Motivo { get; set; }

        public virtual Int64? Qtd_Atendimento { get; set; }

        public virtual String Setor { get; set; }

        public virtual Int64? Numero_Max_Atendimento { get; set; }
    }


    public class RespostaCampoPersonalizado
    {
        public string IdSelecionado { get; set; }
        public string IdCampoSelecionado { get; set; }
        public string RespostaCampo { get; set; }
    }


    public class AtendimentoCadastroModel : IViewModel<Int64>
    {
        public virtual String ValorSubTotal { get; set; }
        public virtual List<RespostaCampoPersonalizado> RespostaCamposPersonalizados { get; set; }
        public virtual List<RespostaCampoPersonalizado> RespostasCamposPersonalizadosUnico { get; set; }

        public virtual String ValorAPagarSemInstalacao { get; set; }
        public virtual Int64 Id { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_Venda { get; set; }
        public virtual Int64? Id_TrocaTitularidade { get; set; }
        public virtual String Sla { get; set; }
        public virtual String NomeUsuario { get; set; }
        public virtual String DataHoraIndicador { get; set; }
        public virtual Boolean NovoResponsavelAceite { get; set; }
        public virtual Boolean CriarOsSemPlanoProduto { get; set; }
        public virtual Status_Cliente? Cliente_Status { get; set; }
        public virtual StatusAuditoria? StatusAuditoria { get; set; }

        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Nome_Cliente { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String CPF_CNPJ_Cliente { get; set; }
        public virtual String RG_Cliente { get; set; }
        public virtual String Data_Nascimento { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Data_Cadastro_Cliente { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Categoria_Cliente { get; set; }

        [MaxLength(12, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Telefone_Fixo { get; set; }

        [MaxLength(12, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Telefone_Celular { get; set; }

        [MaxLength(12, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Telefone_Recado { get; set; }

        public virtual String Nome_Recado { get; set; }

        [MaxLength(40, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Email_Principal { get; set; }

        public virtual Boolean Permite_OS_LEAD { get; set; }
        public virtual String Id_Motivo_Status { get; set; }
        public virtual String Desc_Motivo_Status { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }


        public virtual String Endereco_Principal { get; set; }
        public virtual String Endereco_Instalacao { get; set; }
        public virtual Int64? Id_Endereco { get; set; }

        // Plano/Produto        
        public virtual String Forma_Envio { get; set; }
        public virtual String Forma_Envio_Contrato { get; set; }
        public virtual String Banco { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Conta_Corrente { get; set; }
        public virtual String Digito_Verificador { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual String Data_Vencimento_Contrato { get; set; }
        public virtual String Dias_Teste_Contrato { get; set; }
        public virtual String Dias_Teste_Gratis { get; set; }
        public virtual String Periodo_Carencia { get; set; }
        public virtual String Periodo_Carencia_Contrato { get; set; }
        public virtual Int64? Meses_Carencia { get; set; }
        public virtual long? Meses_Carencia_Carencia { get; set; }
        
        public virtual String Multa_Rescisao { get; set; }
        public virtual Double? Multa_Rescisao_Contrato { get; set; }

        // Plano/Produto - Informações Técnicas
        public virtual long? Id_Login_Radius { get; set; }
        public virtual String Login_Radius { get; set; }
        public virtual String Senha_Radius { get; set; }
        public virtual String POP { get; set; }
        public virtual String ONU { get; set; }
        public virtual long? Id_ONU { get; set; }
        public virtual String OLT { get; set; }
        public virtual long? Id_OLT { get; set; }
        public virtual String Slot{ get; set; }
        public virtual String PON{ get; set; }
        public virtual long? Id_PON { get; set; }
        public virtual String Caixa_FTTH { get; set; }
        public virtual String Porta_FTTH { get; set; }
        public virtual String Velocidade_Download { get; set; }
        public virtual String Velocidade_Upload { get; set; }
        public virtual String Velocidade_Nominal { get; set; }
        public virtual String Velocidade_Download_Reduzida { get; set; }
        public virtual String Velocidade_Download_Reduzida_Txt { get; set; }
        public virtual String Velocidade_Upload_Reduzida { get; set; }
        public virtual String Pool_Cliente { get; set; }
        public virtual String Ip_Cliente { get; set; }
        public virtual String Observacoes { get; set; }
        public virtual String EtapaAtual { get; set; }
        public virtual String Plano_ProdutoAtri { get; set; }
        public virtual Int64? IdProximaEtapa { get; set; }
        public virtual Int64? Formato_Protocolo_Atendimento { get; set; }
        public virtual String ProximaEtapa { get; set; }
        public virtual String CodigoContrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Id_Tipo_Servico { get; set; }
        public virtual Int64? Sequencia { get; set; }
        public virtual String ProximaEtapaDesc { get; set; }
        public virtual bool Encerrar_Atendimento { get; set; }
        public virtual bool EtapaAtualDecisoria { get; set; }
        public virtual String Prazo_Etapa_Atual { get; set; }
        public virtual String Tempo_Gasto_Etapa_Atual { get; set; }
        public virtual String Sla_Etapa_Atual { get; set; }
        public virtual Boolean Velocidade_Reduzida { get; set; }
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }
        public virtual String Id_Cliente_Contrato { get; set; }

        public virtual Int64? Id_Plano_Produto { get; set; }

        public virtual String Nome_Plano_Produto { get; set; }

        [Display(Name = "LOGIN_DO_RADIUS", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        
        public virtual String CepInstalacao { get; set; }
        public virtual String LogradouroInstalacao { get; set; }
        public virtual String NumeroInstalacao { get; set; }
        public virtual String UFInstalacao { get; set; }
        public virtual String CidadeInstalacao { get; set; }
        public virtual String BairroInstalacao { get; set; }
        public virtual String TipoImovelInstalacao { get; set; }
        public virtual String PaisInstalacao { get; set; }
        public virtual String ComplementoInstalacao { get; set; }
        public virtual String PontoReferenciaInstalacao { get; set; }

        public virtual String Valor_Final_Plano { get; set; }
        public virtual String Valor_Instalacao { get; set; }


        // Sobre o Atendimento 
        public virtual String Numero { get; set; }

        public virtual String Protocolo { get; set; }

        public virtual Int64? Id_Origem_Atendimento { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Assunto { get; set; }

        public virtual Int64? Id_Tipo_Atendimento { get; set; }

        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }
        public virtual Boolean Possui_Etapas_Tipo_Atendimento { get; set; }

        public virtual Boolean Criar_Os_Encerrar { get; set; }

        public virtual Boolean Criar_Os_Encerrar_Parametro_Controller { get; set; }
        public virtual String DataEncerramento_Chat { get; set; }
        public virtual Boolean Chat_Via_WhatsApp { get; set; }
        public virtual Boolean Atendimento_Reaberto { get; set; }
        public virtual Int64? Id_Categoria_Atendimento { get; set; }
        public virtual Int64? Id_Setor { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Id_Responsavel { get; set; }
        public virtual Boolean UsuarioConvidado { get; set; }
        public virtual Prioridade_Atendimento? Prioridade { get; set; }

        public virtual Tipo_Pessoa_Contato Tipo_Pessoa_Contato { get; set; }

        public TipoPreenchimentoAtendimento TipoPreenchimentoAtendimento { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual long? IdPessoaContato { get; set; }
        public virtual String Pessoa_Contato { get; set; }
        public virtual String ContatosCopia { get; set; }

        //public virtual String Grau_Ligacao { get; set; }

        public virtual Grau_Ligacao? Grau_Ligacao { get; set; }

        public virtual String Nome_Outra_Pessoa_Contato { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Problema_Relatado { get; set; }

        


        public virtual String Arquivo { get; set; }

        [MaxLength(40, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual List<ArquivoAcaoModel> Arquivos { get; set; }

        public virtual String NomeAtendimento { get; set; }

        public virtual String Data_Hora_Criacao { get; set; }
        public virtual String Data_Hora_Abertura { get; set; }
        public virtual String Data_Hora_Fechamento { get; set; }

        public virtual String Motivo_Status { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Status_Atendimento Status_Atendimento { get; set; }

        public virtual Int32 Tipo_Cliente { get; set; }

        public virtual String Cliente_Ha { get; set; }
        public virtual Int64? Id_Email_Abertura { get; set; }
        public virtual Boolean AcaoGeraEmail { get; set; }

        //public virtual String Login_Radius { get; set; }

        public virtual Status_PlanoProduto? Status_PlanoProduto { get; set; }
        public virtual Status_Contrato? Status_Contrato { get; set; }
        public virtual String Status_PlanoProdutoStr { get; set; }
        public virtual String Status_ContratoStr { get; set; }
        public string Numero_Contrato { get; set; }
        public string ValorSomatorio { get; set; }
        public String Data_Aceite { get; set; }

        //public DateTime Data_Aceite { get; set; }
        public bool ScriptAtendimento { get; set; }
        public virtual bool PlanoTelefonia { get; set; }
        public virtual bool PossuiAlteracaoEtapa { get; set; }
        public virtual String Desconto { get; set; }

        public virtual long? Id_Organizacao_Cliente { get; set; }
        public virtual string Organizacao_Cliente { get; set; }


        private IEnumerable<AtendimentoHistoricoAcoesCadastroModel> historico_atendimento;
       
        

        public virtual IEnumerable<AtendimentoHistoricoAcoesCadastroModel> Historico_Atendimento

        {
            get
            {
                if (historico_atendimento == null)
                    historico_atendimento = new List<AtendimentoHistoricoAcoesCadastroModel>();
                return historico_atendimento;
            }
            set
            {
                historico_atendimento = value;
            }
        }

        private IEnumerable<AtendimentoPlanoContratoModel> planos_contratos;

        public virtual IEnumerable<AtendimentoPlanoContratoModel> Plano_Contrato

        {
            get
            {
                if (planos_contratos == null)
                    planos_contratos = new List<AtendimentoPlanoContratoModel>();
                return planos_contratos;
            }
            set
            {
                planos_contratos = value;
            }
        }

        private IEnumerable<Atendimento_Script_AtendimentoModel> respostasScript;

        public virtual IEnumerable<Atendimento_Script_AtendimentoModel> RespostasScript

        {
            get
            {
                if (respostasScript == null)
                    respostasScript = new List<Atendimento_Script_AtendimentoModel>();
                return respostasScript;
            }
            set
            {
                respostasScript = value;
            }
        }
    }

    public class AtendimentoOla
    {
        public virtual String Observacao { get; set; }
        public virtual StatusExecucaoAniel? StatusAtendimento { get; set; }
        public virtual String DataMaxima { get; set; }
        public virtual String HoraMaxima { get; set; }
        public virtual Int64 IdCliente { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual DateTime? DiaCriacao { get; set; }
        public virtual DateTime? HoraCriacao { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual AtendimentoHistoricoCadastroModel historico { get; set; }

    }
    public class AnexosRecebidosChat
    {
        public string url { get; set;}
    }

    [BsonIgnoreExtraElements]
    public class AtendimentoChat
    {
        public string protocolo { get; set; }
        public string telegramChatId { get; set; }
        public string messengerChatId { get; set; }
        public string instagramChatId { get; set; }
        public string whatsAppChatId { get; set; }
        public string conversationSid { get; set; }
        public string telegramToken { get; set; }
        public string messengerToken { get; set; }
        public string backOfficeId { get; set; }
        public string instagramToken { get; set; }
        public string whatsAppToken { get; set; }
        public string motivo { get; set; }
        public string descricaoProblema { get; set; }
        public string dataAbertura { get; set; }
        public string dataEncerramento { get; set; }
        public bool bot { get; set; }
        public long? idUltimoEvento { get; set; }
        public long? idCanal { get; set; }
        public string dataHoraEnviadoAtendente { get; set; }
        [BsonSerializer(typeof(EmptyStringAsNullSerializer<CRUD_Generico>))]
        public CRUD_Generico crud_telegram_whatsapp { get; set; }
        public ClienteChat_Mobile cliente { get; set; }
        public SuporteChatModels suporte { get; set; }
        public String TwilioAccountId { get; set; }
        public String TwilioAuthToken { get; set; }
        //json contendo armazenamento de variaveis para chat
        public String JsonArmazenamento { get; set; }
        public List<string> anexos { get; set; }
        public List<Conversas_Chat> conversas { get; set; }
        public List<Mensagem_ChatModels> mensagens { get; set; }
        private List<AtendimentoUsuario> _usuarios;
        public List<AtendimentoUsuario> usuarios {
            get {
                return (_usuarios ?? new List<AtendimentoUsuario>());
            }
            set {
                _usuarios = value;
            } 
        }

        private List<UsuarioConvidado> _usuariosConvidados;
        public List<UsuarioConvidado> usuariosConvidados {
            get {
                return (_usuariosConvidados ?? new List<UsuarioConvidado>());
            }
            set {
                _usuariosConvidados = value;
            } 
        }

        public List<EnvioAtendimentoUsuario> usuariosEnvioNotificao { get; set; }
        

    }

    [BsonIgnoreExtraElements]
    public class AtendimentoChatMongo : AtendimentoChat
    {
        [BsonId()]
        public ObjectId _id { get; set; }
        
    }

    [BsonIgnoreExtraElements]
    public class MensagemPerdidaAtendimentoMongo
    {
        [BsonId()]
        public ObjectId _id { get; set; }
        public virtual string id { get; set; }
        public virtual string mensagem { get; set; }
        public virtual string datahora { get; set; }
        public virtual string conversationid { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class AtendimentoUsuario
    {
        public virtual string userid { get; set; }
        public virtual string connectionid { get; set; }
        public virtual bool suporte { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class EnvioAtendimentoUsuario
    {
        public virtual string userid { get; set; }
        public virtual string datahoraEnvio { get; set; }
        public virtual bool recusado { get; set; }
    }

    public class UsuarioConvidado
    {
        public virtual string usuarioConvite { get; set; }
        public virtual string dataConvite { get; set; }
        public virtual string usuarioAceite { get; set; }
        public virtual string dataAceite { get; set; }
        public virtual string connectionid { get; set; }
        public virtual bool? aceite { get; set; }
    }

    public class AtendimentoFiltroModel
    {
        public virtual Int64 Id_Tipo_Atend { get; set; }
        public virtual String Tipo_Atendimento { get; set; }
        public virtual String Etapa_Atual { get; set; }

        public virtual String Responsavel_Atual { get; set; }


        public virtual Int64 Id_Categoria_Cliente { get; set; }
        public virtual String Categoria_Cliente { get; set; }

        public virtual Int64 Id_Categoria_Atendimento { get; set; }
        public virtual String Categoria_Atendimento { get; set; }

        public virtual Int64 Id_Setor { get; set; }
        public virtual String Setor { get; set; }


        public virtual Status_Atendimento? Status_Atendimento { get; set; }
        public virtual StatusAuditoria? StatusAuditoria { get; set; }
        public virtual PesquisaRapidaAtendimento? PesquisaRapidaAtendimento { get; set; }

        public virtual Int64 Id_Motivo_Status { get; set; }
        public virtual String Motivo_Status { get; set; }

        public virtual String Origem { get; set; }

        public virtual String Plano_Produto { get; set; }

        public virtual String ProximaEtapa { get; set; }
        public virtual Int64? Num_Os { get; set; }

        public virtual Int64? Id_Tipo_Atendimento { get; set; }


        //Período
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Final { get; set; }


        //Prioridade de Atendimento
        public virtual Boolean Baixa { get; set; }
        public virtual Boolean Media { get; set; }
        public virtual Boolean Alta { get; set; }


        //SLA de Atendimento
        public virtual Boolean Dentro_Prazo { get; set; }
        public virtual Boolean Atencao { get; set; }
        public virtual Boolean Vencimento_Proximo { get; set; }
        public virtual Boolean Risco_Perda_Prazo { get; set; }
        public virtual Boolean Prazo_Vencido { get; set; }
    }

    public class AtendimentoMotivoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Id_Motivo { get; set; }
        public virtual String Desc_Motivo { get; set; }
    }

    public class AtendimentoHistoricoCadastroModel 
    {
        public virtual Int64 id { get; set; }

        public virtual String numero { get; set; }
        public virtual String assunto { get; set; }

        public virtual String EtapaAtual { get; set; }
        public virtual String ProximaEtapa { get; set; }
        public virtual String tipo_Atendimento { get; set; }


        public virtual String data_Limite { get; set; }

        public virtual String setor { get; set; }

        public virtual String extensao { get; set; }

        public virtual String responsavel { get; set; }

        public virtual Status_Atendimento status { get; set; }

        private IEnumerable<AtendimentoHistoricoAcoesCadastroModel> historico_atendimento;

        public virtual IEnumerable<AtendimentoHistoricoAcoesCadastroModel> historico_Atendimento

        {
            get
            {
                if (historico_atendimento == null)
                    historico_atendimento = new List<AtendimentoHistoricoAcoesCadastroModel>();
                return historico_atendimento;
            }
            set
            {
                historico_atendimento = value;
            }
        }
    }

    public class Atendimento_ModeloAttribute : Attribute
    {
        public Recurso_PainelAtendimentoEnum Recurso { get; set; }
        public Atendimento_ModeloAttribute(Recurso_PainelAtendimentoEnum value)
        {
            Recurso = value;
        }
    }

    public class AcompanhamentoAgente_DataTableRequest : DataTableRequest
    {

        public virtual Enum_Status_Chat? Status_Chat { get; set; }

        public virtual String Agente { get; set; }
        public virtual String Motivo { get; set; }

        public virtual Int64? Qtd_Atendimento { get; set; }

        public virtual String Setor { get; set; }

        public virtual Int64? Numero_Max_Atendimento { get; set; }
    }

    public class Atendimento_DataTableRequest : DataTableRequest
    {
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Tipo_Atendimento)]
        public virtual Int64? Tipo_Atendimento { get; set; }
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Categoria_Cliente)]
        public virtual Int64? Categoria_Cliente { get; set; }
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Categoria_Atendimento)]
        public virtual Int64? Categoria_Atendimento { get; set; }
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Setor)]
        public virtual Int64? Setor { get; set; }

        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Status)]
        public virtual Status_Atendimento? Status_Atendimento { get; set; }
        public virtual PesquisaRapidaAtendimento? PesquisaRapidaAtendimento { get; set; }
        public virtual StatusAuditoria? StatusAuditoria { get; set; }
        
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Motivo_Status)]
        public virtual Int64? Motivo_Status { get; set; }
        public virtual List<Int64?> Lista_PrioridadeAtendimento { get; set; }
        public virtual Int64? Num_Os { get; set; }

        //Período
        public virtual Int32? Tipo_Data { get; set; }
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Final { get; set; }

        //Prioridade de Atendimento
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Prioridade_Atendimento)]
        public virtual Prioridade_Atendimento? Prioridade_Atendimento { get; set; }

        //SLA de Atendimento
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.SLA_Atendimento)]
        public virtual Status_SLA_Atendimento? SLA_Atendimento { get; set; }

        public virtual Boolean Dentro_Prazo { get; set; }
        public virtual Boolean Atencao { get; set; }
        public virtual Boolean Vencimento_Proximo { get; set; }
        public virtual Boolean Risco_Perda_Prazo { get; set; }
        public virtual Boolean Prazo_Vencido { get; set; }

        public virtual Int64? Id_Modelo_PainelAtendimento { get; set; }
        public virtual String Usuario_Logado { get; set; }
        public virtual Int64? Id_DepartamentoUsuario_Logado { get; set; }
        public virtual Int64? Id_Origem_Atendimento { get; set; }

        public virtual String Id_Usuario_Responsavel { get; set; }

        public virtual Int64? Id_Cliente_Prospecto { get; set; }

        public virtual Int64? id_Plano_Produto { get; set; }

        public virtual Int64? id_Etapa_Atual { get; set; }
        public virtual Int64? id_Organizacao { get; set; }
        public virtual String Sequencia { get; set; }
    }


    public class AtendimentoChat_DataTableRequest : DataTableRequest
    {
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Final { get; set; }
        public virtual String tipo_Filtro_Data { get; set; }
        public virtual String status_Atendimento { get; set; }
        public virtual String status_Mensagem { get; set; }
        public virtual String tem_Acesso { get; set; }
        public virtual String Protocolo { get; set; }
    }


    public class AtendimentoHistoricoAcoesCadastroModel
    {
        public virtual Int64 id { get; set; }

        public virtual String responsavel { get; set; }
        public virtual String status { get; set; }

        public virtual String cor_SLA { get; set; }

        public virtual String setor { get; set; }

        public virtual String descricaoAcao { get; set; }
        public virtual String html { get; set; }
        public virtual String origem { get; set; }
        public virtual TipoAtividadeAtendimento tipoAtividade  { get; set; }
        public virtual Boolean mensagens_Chat { get; set; }

        public virtual String dataCriacao { get; set; }
        public virtual DateTime? dataCriacaoDateTime { get; set; }

        public virtual String Usuario { get; set; }
        public virtual List<ArquivoAcaoModel> arquivos { get; set; }
        public virtual List<AtendimentoPlanoContratoModel> contrato_planoproduto { get; set; }
        public virtual List<Mensagem_ChatModels> mensagenschat { get; set; }

    }
      public class AtendimentoPlanoContratoModel
    {
        public virtual Int64 id { get; set; }

        public virtual String Nome_Plano_Produto { get; set; }
        public virtual String Status_PlanoProdutoStr { get; set; }
        public virtual Status_PlanoProduto? Status_PlanoProduto { get; set; }
       
        public virtual String Numero_Contrato { get; set; }
        public virtual String Data_Aceite { get; set; }

        public virtual String Valor_Final_Plano { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }
        public virtual String Banco { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual String Forma_Envio { get; set; }
        public virtual String Dias_Teste_Gratis { get; set; }
        public virtual String Periodo_Carencia { get; set; }
        public virtual Int64? Meses_Carencia { get; set; }
        public virtual String Multa_Rescisao { get; set; }
       

    }

    public class ArquivoAcaoModel
    {
        public virtual Int64 id { get; set; }
        public virtual String arquivo { get; set; }
        public virtual String extensao { get; set; }
        public virtual String nome_Arquivo { get; set; }

    }

    public class Atendimento_Script_AtendimentoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Resposta_Pergunta { get; set; }

        public virtual Int64 Id_Tipo_Atendimento { get; set; }
        public virtual String Descricao_Tipo_Atendimento { get; set; }

        public virtual Int64? Id_Atendimento { get; set; }

        public virtual Int32 Row_Pergunta { get; set; }

        public virtual Int64 Id_Modelo_Pergunta { get; set; }
        public virtual String Pergunta { get; set; }

        public virtual String Instrucao_Atendente { get; set; }
        public virtual String Instrucao_Cliente { get; set; }

        public virtual Script_Tipo_Resposta Tipo_Resposta { get; set; }

        public virtual String Resposta { get; set; }

        public virtual List<Boolean> RespostasMultiplas { get; set; }

        public virtual List<String> RespostasPossiveis { get; set; }

        public virtual String Observacao_Atendimento { get; set; }
    }



    public class ConsultaClienteAtendimentoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Venda { get; set; }

        public virtual Int64? Numero { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Script { get; set; }

        public virtual String Prioridade_Padrao { get; set; }

        //public virtual String Status { get; set; }

        public virtual String Categoria_Atendimento { get; set; }

        public virtual Int64 Id_Motivo { get; set; }

        public virtual String Problema_Relatado { get; set; }

        public virtual String Vendedor { get; set; }

        public virtual String Pessoa_Contato { get; set; }

        public virtual String Ultima_Acao { get; set; }

        public virtual String Cliente { get; set; }

        public virtual String Origem_Atendimento { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Produto { get; set; }
        public virtual String Tipo_Atendimento { get; set; }

        public virtual String Setor { get; set; }
        public virtual String Supervisor { get; set; }
         public virtual String Responsavel { get; set; }
         public virtual String Assunto { get; set; }

        public virtual String Motivo_Status { get; set; }

        public virtual String DataHoraCriacao { get; set; }
        public virtual String Data_Hora_Fechamento { get; set; }

        public virtual String NomeAtendimento { get; set; }

        public virtual String Status { get; set; }

    }

    public class DadosAtendimentoChatModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Nome_Cliente { get; set; }
        public virtual String Cpf  { get; set; }
        public virtual String Categoria_Cliente { get; set; }
        public virtual String Plano_Produto { get; set; }
        public virtual Int64? Id_Categoria_Atendimento { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual Int64? Id_Setor { get; set; }
        public virtual String Id_Responsavel { get; set; }
    }

    public class GravarAcaoAtendimentoModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 IdAtendimento { get; set; }
        public virtual String Numero { get; set; }
        public virtual String DescricaoAcao { get; set; }
        public virtual String Usuario { get; set; }
        public virtual TipoAtividadeAtendimento TipoAtividade { get; set; }
        public virtual List<ArquivoAcaoModel> Arquivos { get; set; }
        public virtual List<HttpPostedFileBase> file { get; set; }

    }

    public class ConsultaClienteAtendimento_DataTableRequest : DataTableRequest
    {
        public virtual String Dt_Inicial_Filtro { get; set; }
        public virtual String Dt_Final_Filtro { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual Status_Atendimento? Status { get; set; }
        public virtual Status_SLA_Atendimento? Sla { get; set; }

        public virtual Status_Venda? Status_Venda { get; set; }
        public virtual Padrao? Termo_Adessao { get; set; }

        public String DataInicial { get; set; }
        public String DataFinal { get; set; }



    }

    public class HistoricoAlteracao_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
    }

    public class OrdensServico_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id_Atendimento { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_ClientePlanoProduto { get; set; }
        public virtual StatusExecucaoAniel? Status { get; set; }
        public virtual String DataInicial { get; set; }
        public virtual String DataFinal { get; set; }

    }

    public class HistoricoAtendimento_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_TipoAtendimento { get; set; }
        public virtual String StatusAtendimento { get; set; }
        public virtual String DataInicial { get; set; }
        public virtual String DataFinal { get; set; }
        public virtual String StatusVenda { get; set; }
        public virtual String TermoAdesao { get; set; }

    }

    public class OrdensServicoRequest
    {
        public virtual Int64 Id_Cliente { get; set; }
        public virtual String DataInicial { get; set; }
        public virtual String DataFinal { get; set; }
        public virtual StatusExecucaoAniel? Status { get; set; }
        public virtual String Ola_Token { get; set; }
    }

    public class OrdensServicoResponse {
        public virtual String numObra { get; set; }
        public virtual String Codct { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Doc { get; set; }
        public virtual String Numero_Obra_Original { get; set; }
        public virtual String dataAgendamento { get; set; }
        public virtual String tipoServico { get; set; }
        public virtual String sla { get; set; }
        public virtual String status { get; set; }
        public virtual Int32 nrStatus { get; set; }
        public virtual String tecnico { get; set; }
        public virtual String dataHoraCriacao { get; set; }
        public virtual String dataHoraEncerramento { get; set; }
        public virtual String enderecoInstalacao { get; set; }
        public virtual String UF { get; set; }
        public virtual String cidade { get; set; }
        public virtual String logradouro { get; set; }
        public virtual String bairro { get; set; }
        public virtual String numero { get; set; }
        public virtual double? latitude { get; set; }
        public virtual double? longitude { get; set; }
        public long? idClientePlanoProduto { get; internal set; }
        public long? idAtendimento { get; internal set; }
        public virtual String codEncerramento { get; set; }
    }

    public class DadosTecnicoRequest 
    {
        public virtual string equipe { get; set; }
    }

    public class DadosTecnicoResponse
    {
        public virtual string equipe { get; set; }
        public virtual string nomeTecnico { get; set; }
        public virtual string fotoTecnico64 { get; set; }
    }

    public class OrdensServicoAtendimentoResponse {
        public virtual String numObra { get; set; }
        public virtual String codCt { get; set; }
        public virtual String projeto { get; set; }
        public virtual String numDoc { get; set; }
        public virtual String status { get; set; }
        public virtual String numObraOriginal { get; set; }
    }

    public class Atendimento_OrdemServicoResponse
    {
        public List<OrdensServicoResponse> ListaOs { get; set; }
        public virtual Int64 IdPlano { get; set; }
        public virtual Boolean PlanoComOs { get; set; }
        public virtual Boolean Mudanca_Endereco { get; set; }
        public virtual Boolean ExistePendencia { get; set; }
    }

    public class TransferirAtendimentoRequestModel
    {
        public virtual String usuario { get; set; }
        public virtual String NomeUsuario { get; set; }
        public virtual String protocolo { get; set; }
        public virtual String novoAtendente { get; set; }
        public virtual Int64 novoDepartamento { get; set; }
    }

    public class ConvidarAtendenteRequestModel
    {
        public virtual String usuarioConvidado { get; set; }
        public virtual String protocolo { get; set; }
    }

    public class HistoricoAtendimentoResponse
    {
        public virtual Int64? id{ get; set; }
        public virtual Int64? venda{ get; set; }

        public virtual String protocolo { get; set; }
        public virtual String vendedor { get; set; }
        public virtual String supervisor { get; set; }
         public virtual String status_venda { get; set; }
         public virtual String termo_assinado { get; set; }

        public virtual String status { get; set; }
        public virtual String motivoStatus { get; set; }
        public virtual String tipoAtendimento { get; set; }
        public virtual String responsavel { get; set; }
        public virtual String dataHoraCriacao { get; set; }
        public virtual String assunto { get; set; }
        public virtual String dataHoraEncerramento { get; set; }
        public virtual String numero { get; set; }
        public virtual String origemAtendimento { get; set; }

    }

    public class AcompanhamentoAtendimentoModel
    {
        public virtual String protocolo { get; set; }
        public virtual String cliente { get; set; }
        public virtual String categoriaCliente { get; set; }
        public virtual String tipoAtendimento { get; set; }
        public virtual String tempoEspera { get; set; }
        public virtual String origem { get; set; }
        public virtual String sla { get; set; }
        public virtual String chat { get; set; }
        public virtual String agente { get; set; }
        public virtual Boolean? bot { get; set; }
        public virtual String nomeBot { get; set; }
        public virtual String etapa_Atual { get; set; }
        public virtual String prazo_Atual { get; set; }
        public virtual String data_Sla_Etapa { get; set; }

        public virtual String tempo_Gasto { get; set; }
        public virtual string organizacaoCliente { get; set; }
    }

   

    public class AtendimentoUnificarRequest
    {
        public virtual List<Int64> ids { get; set; }
        
    }
       

    public class PrioridadeAtendimento
    {
        
        public virtual Int64 Id { get; set; }
        public virtual String id_categoria_cliente { get; set; }
        public virtual Int64? Tipo_AtendimentoSelecionado { get; set; }
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Prioridade_Atendimento { get; set; }
        
    }

    public class InboundConversationRequest
    {
        public string RetryCount { get; set; }
        public string EventType { get; set; }
        public string Attributes { get; set; }
        public DateTime DateCreated { get; set; }
        public string Author { get; set; }
        public string Index { get; set; }
        public string MessageSid { get; set; }
        public string ParticipantSid { get; set; }
        public string Body { get; set; }
        public string AccountSid { get; set; }
        public string Source { get; set; }
        public string ConversationSid { get; set; }
        public string MessagingServiceSid { get; set; }
        public string State { get; set; }
        public string ChatServiceSid { get; set; }
        public string NumeroWhatsappTwilio { get; set; }
        public MessagingBinding MessagingBinding { get; set; }
        public object Media { get; set; }

    }

    public class TestarBotRequestModel
    {
        public string mensagem { get; set; }
        public Int64? IdBot { get; set; }
        public Int64? UltimoEvento { get; set; }
        public AtendimentoChatMongo atendimento { get; set; }

    }

    public class Media
    {
        public string Sid { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public string Size { get; set; }
    }

    public class MessagingBinding {
        public string ProxyAddress { get; set; }
        public string Address { get; set; }
    }

    public class MudarEtapaFluxoModel
    {
        public long IdAtendimento { get; set; }
        public long? IdProximaEtapa { get; set; }
    }

    public class RetornoMudarEtapaFluxoModel
    {
        public bool EncerraAtendimento { get; set; }
        public bool CriaOs { get; set; }
        public String Id_Tipo_Servico { get; set; }
        public Int16 Id_Departamento_Padrao { get; set; }
        public String Codigo_Contrato { get; set; }
        public Boolean Etapa_CriaOS { get; set; }
        public String Desc_Etapa_Atual { get; set; }
        public String Projeto { get; set; }
    }    

    public class OrdensServicoResponseMobile
    {
        public virtual String numObra { get; set; }
        public virtual String Codct { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Doc { get; set; }
        public virtual String Numero_Obra_Original { get; set; }
        public virtual String dataAgendamento { get; set; }
        public virtual String tipoServico { get; set; }
        public virtual short? codTipoServico { get; set; }
        public virtual String sla { get; set; }
        public virtual int? statusEnum { get; set; }
        public virtual String status { get; set; }
        public virtual String tecnico { get; set; }
        public virtual String dataHoraCriacao { get; set; }
        public virtual String dataHoraEncerramento { get; set; }
        public virtual String enderecoInstalacao { get; set; }
        public virtual String UF { get; set; }
        public virtual String cidade { get; set; }
        public virtual String logradouro { get; set; }
        public virtual String bairro { get; set; }
        public virtual String numero { get; set; }
        public virtual double? latitude { get; set; }
        public virtual double? longitude { get; set; }
        public long? idClientePlanoProduto { get; internal set; }
        public long? idPlanoProduto { get; internal set; }
        public long? idAtendimento { get; internal set; }

        public virtual String nomeTecnico { get; set; }
        public virtual String descricaoPlano { get; set; }
        public virtual String descricaoCodTurno { get; set; }
       	public virtual String nomeTipoServico { get; set; }
        public virtual int? cod_turno_serv { get; set; }
        public virtual bool? turno_por_periodo { get; set; }
    }

    public class AtendimentoChatMongoProtocolo
    {
        public virtual Int64 id { get; set; }
        public virtual String protocolo { get; set; }
        public virtual Boolean em_atendimento { get; set; }
    }

    public class VincularOsResponse
    {
        public virtual Int64? id { get; set; }
        public virtual Int64? venda { get; set; }

        public virtual String Num_Doc { get; set; }
        public virtual String vendedor { get; set; }
        public virtual String num_Os { get; set; }
        public virtual String projeto { get; set; }
        public virtual String codCt { get; set; }

        public virtual String status { get; set; }
        public virtual String motivoStatus { get; set; }
        public virtual String tipoServico { get; set; }
        public virtual String responsavel { get; set; }
        public virtual String dataHoraCriacao { get; set; }
        public virtual String subTipoServico { get; set; }
        public virtual String dataSla { get; set; }
        public virtual String numero { get; set; }
        public virtual String cliente { get; set; }

    }
    public class HistoricoVincularResponse
    {
        public virtual Int64? id { get; set; }
        public virtual Int64? venda { get; set; }
        public virtual String Num_Doc { get; set; } 
        public virtual String num_Os { get; set; }
        public virtual String projeto { get; set; }
        public virtual String codCt { get; set; }
        public virtual String status { get; set; }
        public virtual String usuario { get; set; }
        public virtual String responsavel { get; set; }
        public virtual String dataHoraCriacao { get; set; }
        public virtual String SubTipoServico { get; set; }
        public virtual String dataSla { get; set; }
        public virtual String TipoServico { get; set; }
        public virtual String cliente { get; set; }
        public virtual String DataCriacao { get; set; }


    }

    public class HistoricoVincularResponse_DataTableRequest : DataTableRequest
    {      
        public virtual String NumeroAtendimento { get; set; }

    }

    public class NotificacaoPendenteAtualizacaoRequestModel
    {
        public long Id { get; set; }
        public string Texto_notificacao { get; set; }
        public long Status { get; set; }
        public string Data_envio { get; set; }
        public string Chave { get; set; }
        public long Id_notificacoes { get; set; }
        public string Acao { get; set; }
    }

    public class EmptyStringAsNullSerializer<T> : SerializerBase<T> where T : class
    {
        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();

            if (bsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            if (bsonType == BsonType.String)
            {
                var str = context.Reader.ReadString();
                // qualquer string (vazia ou não) vira null
                return null;
            }

            // se for documento, deserializa normalmente
            return BsonSerializer.Deserialize<T>(context.Reader);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                BsonSerializer.Serialize(context.Writer, value);
            }
        }
    }

    public class PostApiRetornoAtendimento : PostApiRetorno
    {
        public Int64 IdObjeto { get; set; }
        public bool EtapaAtualizada { get; set; }
        public bool Concluido_Cancelado { get; set; }
    }

}