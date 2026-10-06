using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class Configuracoes_GeraisListaModel
    {
        public virtual Int64 Id { get; set; }
        //public virtual String Trava_Atendimento_OS { get; set; }
        public virtual String Exige_Agendamento_Criacao_Servico { get; set; }
        public virtual String Empresa_Roteirizacao_Servico { get; set; }
        public virtual String Encerrar_Atendimento_Junto_OS { get; set; }
        public virtual String Acumular_Descontos_Diferente { get; set; }
        public virtual String Acumular_Descontos_Mesma_Natureza { get; set; }
        public virtual String Formato_Protocolo_OS { get; set; }
        public virtual String Formato_Protocolo_Atendimento { get; set; }
        public virtual String Identificacao_Cliente { get; set; }
        public virtual String Nome_Cliente_Sinapse { get; set; }
        public virtual String CNPJ { get; set; }
        public virtual String Anuncios { get; set; }
        public virtual String Id_Ola_Cliente { get; set; }
        public virtual String OLA_URL_Logo { get; set; }
        public virtual String OLA_Cor_Principal { get; set; }
        public virtual String OLA_Cor_Assests { get; set; }
        public virtual String OLA_Cor_Principal_Dark { get; set; }
        public virtual String OLA_Cor_Fonte { get; set; }

        public virtual String Id_WidePay { get; set; }
        public virtual String Token_Widepay { get; set; }
        public virtual Boolean Checkin_Checkout_Auto { get; set; }
        public virtual Boolean Num_Contrato_Gerado_Auto { get; set; }        
        public virtual Boolean Exige_Termo_Adesao { get; set; }
        public virtual Int32 Dia_Mes_Finaliza_Medicao_Comissao { get; set; }

        // Consultas de pendencias
        public virtual Boolean ConsultaDeCredito { get; set; }
        public virtual Boolean ConsultaPendenciaEndereco { get; set; }
        public virtual Boolean ConsultaPendenciaFinanceira { get; set; }
        public virtual Boolean ConsultaViabilidade { get; set; }
    }

    public enum Enum_Distribuir_Servico_Roteirizado_Por
    {
        Igual = 0,
        Enchendo_Equipe = 1
    }

    public class Configuracoes_GeraisCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        [Display(Name = "COMERCIAL_USA_SMS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean ComercialUsaSms { get; set; }
        [Display(Name = "OCULTAR_ASSINATURA_TERMO_PREMIER", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean OcultarAssinaturaTermoPremier { get; set; }

        [Display(Name = "OCULTAR_AGENDAMENTO_OS_PREMIER", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean OcultarAgendamentoOsPremier { get; set; }


        [Display(Name = "PERMITE_DOC_VENDA_GALERIA_PREMIER", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean PermiteDocVendaGaleriaPremier { get; set; }
        [Display(Name = "ENVIAR_CHECKLIST_AUTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean EnviarCheckListAuto { get; set; }
        [Display(Name = "TOKEN_INTEGRACAO_SMS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Token_Integracao_SMS { get; set; }
        public virtual string Usuario_Integracao_SMS { get; set; }
        public virtual string Senha_Integracao_SMS { get; set; }
        [Display(Name = "TIPO_INTEGRACAO_SMS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Tipo_Integracao_SMS? Tipo_Integracao_SMS { get; set; }
        [Display(Name = "UTILIZA_INTEGRACAO_SMS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Utiliza_Integracao_SMS { get; set; }

        //[Display(Name = "TRAVA_ATENDIMENTO_OS", ResourceType = typeof(Resources.RsCadastros))]
        //public virtual Boolean Trava_Atendimento_OS { get; set; }

        [Display(Name = "EXIGE_AGENDAMENTO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Exige_Agendamento_Servico { get; set; }

        [Display(Name = "UTILIZAR_CRIACAO_EMAIL_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Utilizar_Criacao_Email_Atendimento { get; set; }
        public virtual Boolean Utilizar_Integracao_Externa { get; set; }
        public virtual IntegracaoExterna? IntegracaoExterna { get; set; }
        public virtual EnumTipoIntegracao? TipoIntegracaoExterna { get; set; }
        public virtual String IntegracaoExternaDesc { get; set; }
        public virtual String UsuarioIntegracaoExterna { get; set; }
        public virtual String SenhaIntegracaoExterna { get; set; }
        public virtual String TokenIntegracaoExterna { get; set; }
        public virtual String UrlApiIntegracaoExterna { get; set; }
        public virtual String Anuncios { get; set; }
        public virtual String ApiKey { get; set; }
        public virtual String ApiSecret { get; set; }
        public virtual String HeaderIntegracaoExterna1 { get; set; }
        public virtual Int64? Prazo_ultima_restricao { get; set; }
        public virtual Int64? Quantidade_restricoes_permitidas { get; set; }
        public virtual Boolean Utilizar_Criacao_Dominio_Email_Atendimento { get; set; }
        public virtual Boolean Utilizar_SSL { get; set; }
        public virtual Boolean EmailPrincipal{ get; set; }
        public virtual Boolean ConfirmarEndereco{ get; set; }
        public virtual AuthMethodImap AuthMethodImap { get; set; }

        [Display(Name = "EMPRESA_ROTEIRIZACAO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Empresa_Roteirizacao_Servico { get; set; }

        [Display(Name = "ENCERRAR_ATENDIMENTO_JUNTO_OS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Encerrar_Atendimento_Junto_OS { get; set; }

        [Display(Name = "ACUMULAR_DESCONTOS_DIFERENTE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Acumular_Descontos_Diferente { get; set; }

        [Display(Name = "ACUMULAR_DESCONTOS_MESMO_TIPO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Acumular_Descontos_Mesmo_Tipo { get; set; }
        public virtual Boolean ApenasServicosProdutivosReincidencia { get; set; }
        public virtual Boolean CriarOsSemPlanoProduto { get; set; }

        [Display(Name = "FORMATO_PROTOCOLO_OS", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual Formato_Protocolo_OS? Formato_Protocolo_OS { get; set; }

        [Display(Name = "FORMATO_PROTOCOLO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual Formato_Protocolo_Atendimento? Formato_Protocolo_Atendimento { get; set; }

        [Display(Name = "TEMPO_MEDIO_DESLOCAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Tempo_Medio_Deslocamento { get; set; }

        [Display(Name = "TEMPO_EXECUCAO_PADRAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Tempo_Execucao_Padrao { get; set; }
        
        public virtual String TempoMinimoPrimeiraJanela { get; set; }

        [Display(Name = "TEMPO_ATUALIZACAO_AUTOMATICA_PAINEL_ACOMPANHAMENTO_TECNICO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Tempo_Atualizacao_Automatica_Painel_Acompanhamento_Tecnico { get; set; }

        [Display(Name = "TEMPO_ATUALIZACAO_AUTOMATICA_TIMELINE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Tempo_Atualizacao_Automatica_Timeline { get; set; }

        [Display(Name = "MEDIA_PADRAO_KM_L", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String MediaPadraoKmL { get; set; }

        [Display(Name = "VALOR_PADRAO_ESTIMADO_LITRO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String ValorPadraoEstimadoLitro { get; set; }

        [Display(Name = "SLA_SERVICO_PADRAO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual String SLA_Servico_Padrao { get; set; }

        [Display(Name = "IDENTIFICACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual Identificacao_Cliente? Identificacao_Cliente { get; set; }

        [Display(Name = "NOME_CLIENTE_SINAPSE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Nome_Cliente_Sinapse { get; set; }

        [Display(Name = "CNPJ", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String CNPJ { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "ID_OLA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Id_Ola_Cliente { get; set; }



        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String OLA_URL_Logo { get; set; }
        public virtual Enum_Tipo_Sla_ SLA_Data_Abertura { get; set; }

        public virtual String UsuarioApiAniel { get; set; }
        public virtual String SenhaApiAniel { get; set; }
        public virtual String TokenApiAniel { get; set; }
        public virtual Boolean UtilizarRouter { get; set; }


        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "OLA_COR_PRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String OLA_Cor_Principal { get; set; }

        [MaxLength(255, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "OLA_COR_ASSESTS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String OLA_Cor_Assests { get; set; }

        [Display(Name = "OLA_COR_ASSETS_DARK", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String OLA_Cor_Assets_Dark { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "OLA_COR_PRINCIPAL_DARK", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String OLA_Cor_Principal_Dark { get; set; }

        [Display(Name = "OLA_COR_FONTE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String OLA_Cor_Fonte { get; set; }

        [Display(Name = "TOKEN", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Token { get; set; }


        // atributos da classe url_Sistema
        [Display(Name = "SISTEMA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Sistema? Sistema { get; set; }

        [Display(Name = "URL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String URL { get; set; }

        [Display(Name = "TELEFONE_SUPORTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public virtual String Telefone_Suporte { get; set; }

        [Display(Name = "EMAIL_SUPORTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Email_Suporte { get; set; }


        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]

        // Atributos da classe Anuncios_OLA

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String descricaoAnuncio { get; set; }

        [Display(Name = "URL_DESTINO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String urlDestino { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]

        //[Display(Name = "URL_IMAGEM_ANUNCIO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual byte[] imagemAnuncio { get; set; }

        [Display(Name = "ID_WIDEPAY", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Id_WidePay { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "TOKEN_WIDEPAY", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Token_Widepay { get; set; }

        [Display(Name = "CHECKIN_CHECKOUT_AUTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Checkin_Checkout_Auto { get; set; }

        [Display(Name = "NUM_CONTRATO_GERADO_AUTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Num_Contrato_Gerado_Auto { get; set; }
        [Display(Name = "EXIGE_TERMO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Exige_Termo_Contrato { get; set; }
        [Display(Name = "UTILIZA_ENVIO_AUTOMATICO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Utiliza_Envio_Automatico_Contrato { get; set; }
        [Display(Name = "EXIGE_TERMO_ADESAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Exige_Termo_Adesao { get; set; }

        [Display(Name = "PERMITE_ANEXAR_DOC_TERMO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Permite_Anexar_Doc_Termo { get; set; }

        [Display(Name = "PORCENTAGEM_VIDEO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual int? Porcentagem_Video { get; set; }

        [Display(Name = "PORCENTAGEM_SIMILARIDADE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual int? Porcentagem_Similaridade { get; set; }

        [Display(Name = "PERMITE_WORK_TERMO_SEM_TERMO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Permite_Work_Sem_Termo { get; set; }
        
        public bool Bloqueia_Lead { get; set; }
        public int? Qtd_Dias_Bloqueio_Lead { get; set; }

        [Display(Name = "EXIGE_FOTO_TERMO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Exige_Foto_Termo { get; set; }
        
        public virtual String Acoes_Prova_Vida { get; set; }

        [Display(Name = "TIPO_COMPROVACAO_TERMO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Tipo_Comprovante_Termo? Tipo_Comprovante_Termo { get; set; }

        [Display(Name = "UTILIZA_CERTIFICACAO_ASSINATURA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Utiliza_Certificacao_Assinatura { get; set; }

        [Display(Name = "TIPO_INTEGRACAO_ASSINATURA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual TipoIntegracaoAssinaturaEnum? Tipo_Integracao_Assinatura { get; set; }

        [Display(Name = "TOKEN_INTEGRACAO_ASSINATURA", ResourceType = typeof(Resources.RsCadastros))]
        public string Token_Integracao_Assinatura { get; set; }

        [Display(Name = "OFERTAR_PLANO_PRODUTO_COBERTURA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Ofertar_Plano_Produto_Cobertura { get; set; }

        [Display(Name = "UTILIZA_CADASTRO_ENDERECO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Utiliza_Cadastro_Endereco { get; set; }
        [Display(Name = "PERMITE_OS_LEAD", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Permite_OS_LEAD { get; set; }
        [Display(Name = "GERAR_ATENDIMENTO_APOS_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Gerar_Atendimento_Apos_Venda { get; set; }

        [Display(Name = "ATRIBUIR_TEMPO_LIMITE_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Atribuir_Tempo_Limite_Venda { get; set; }

        [Display(Name = "RESERVA_PORTA_AUTOMATICA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Reserva_Porta_Automatica { get; set; }
        
        [Display(Name = "PERMITE_VENDA_ABERTA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Permite_Venda_Aberta_Cliente { get; set; }
        
        [Display(Name = "PERMITE_VENDA_ABERTA_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Permite_Venda_Aberta_Imovel { get; set; }

        [Display(Name = "PERMITE_EXCLUSAO_CANCELAMENTO_PLANO_INTEGRADO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Permite_Exclusao_Cancelamento_Plano_Integrado { get; set; }

        [Display(Name = "QTDE_DIAS_AVALIACAO_COMERCIAL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int32? Qtde_Dias_Avaliacao_Comercial { get; set; }

        [Display(Name = "QTDE_MESES_VISUALIZACAO_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int32? Qtde_Meses_Visualizacao_Venda { get; set; }

        [Display(Name = "DIA_MES_FINALIZA_MEDICAO_COMISSAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int32? Dia_Mes_Finaliza_Medicao_Comissao { get; set; }
        public virtual String Padrao_Login { get; set; }
        public virtual String UrlPowerBI { get; set; }
        public virtual String Padrao_Login_Ultimo { get; set; }
        [Display(Name = "MAX_DIAS_SEM_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int32? Max_Dias_Sem_Contato { get; set; }
        [Display(Name = "MAX_DIAS_SEM_APROVACAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int32? Max_Dias_Sem_Aprovacao { get; set; }

        [Display(Name = "EXIBIR_CAIXA_PORTA_DISPONIVEL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int32? Disponibilidade { get; set; }

        [Display(Name = "DISTANCIA_EXIBICAO_CAIXA_PORTA_DISPONIVEL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int32? Distancia_Exibicao_Elemento { get; set; }

        [Display(Name = "UTILIZA_ARMAZENAMENTO_NUVEM", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Utiliza_Armazenamento_Nuvem { get; set; }

        [Display(Name = "PROJECTID_CLOUD_STORAGE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String ProjectId_Cloud_Storage { get; set; }

        [Display(Name = "CREDENTIAL_JSON_CLOUD_STORAGE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String CredentialJson_Cloud_Storage { get; set; }
        public virtual Boolean RoteirizarSemLatLong { get; set; }
        public virtual Boolean CriticarBaixaLocalServico { get; set; }
        public virtual Boolean Exige_LatLong_Agendamento { get; set; }
        public virtual Boolean Envia_Msg_Ao_Iniciar_Deslocamento { get; set; }
        public virtual Boolean Permite_Motivo_Reagendamento { get; set; }
        public virtual String Modelo_Envio { get; set; }
        public virtual InfoOSTimelineEnum? CampoinfoOStimeline { get; set; }
        public virtual bool ExibirInfoOSTimeline { get; set; }
        public virtual Boolean RoteirizarPorCaixa { get; set; }

        public virtual Boolean ValidaNumeroTelefone { get; set; }
        public virtual long? IdModeloMensagemSMS { get; set; }
        public virtual Boolean BloquearVendaSemContatoValidado { get; set; }
        public virtual Boolean Usa_Dias_Liberacao_Did { get; set; }
        public virtual String Dias_Liberacao_Did { get; set; }

        public virtual Int32 Id_Sistema { get; set; }

        private List<URL_SistemaListaModel> URL_Sistemas_Lista;
        private List<Anuncios_OLAConfiguracoesGeraisModel> Anuncios_OLA_Lista;
        private List<EmailListaModel> Emails_Lista;
        private List<IntegracoesExternasListaModel> Integracoes_Lista;

        public virtual Boolean Criterio_Habilidade { get; set; }
        public virtual Boolean Considerar_Agendamento_Periodo { get; set; }
        public virtual Boolean Criterio_Area { get; set; }
        public virtual Boolean Criterio_SLA_Execucao { get; set; }
        public virtual Boolean Criterio_SLA_Hora { get; set; }

        public virtual Boolean UtilizaPoint_RoteirizacaoAgendamento { get; set; }
        public virtual Boolean UtilizaEva { get; set; }
        public virtual Boolean Utiliza_AgrupamentoDistancia { get; set; }
        public virtual String DistanciaMinimaDeslocamento { get; set; }
        public virtual String TempoBaseDeslocamentoPorKm { get; set; }
        public virtual String Padrao_Login64 { get; set; }
        public virtual String Padrao_Login_Ultimo64 { get; set; }

        public virtual Enum_Distribuir_Servico_Roteirizado_Por? Distribuicao_Servico_Por { get; set; }
        public virtual Enum_Pessoa_Contato_Primeira_Vez? Pessoa_Contato_Primeira_Vez { get; set; }
        public virtual Boolean ExibeRDOs { get; set; }
        public virtual Boolean ParametroTimeline { get; set; }
        public virtual Boolean Atendimento_Cria_Cliente { get; set; }
        public virtual Boolean CriarAtendimentoSemPlanoProduto { get; set; }
        public virtual string DistanciaMaximaAgrupamento { get; set; }
        public virtual string HorarioCorteRoteirizacaoDashBoard { get; set; }
        public virtual string LimiteDistanciaParaDeslocamento { get; set; }
        public virtual string LimiteDistanciaBaixa { get; set; }
        public virtual string LimiteMinimoTimeline { get; set; }

        public virtual Boolean AtribuirOSReincTec { get; set; }
        public virtual Boolean PermitirAuditoriaOSImprod { get; set; }

        public virtual string LimiteMaximoTimeline { get; set; }
        public virtual bool EnderecoInstalacaoMesmoPrincipal { get; set; }
        public virtual Boolean CriticarBaixaPotenciaONU { get; set; }
        public virtual Boolean CriticarBaixaLoginRadiusOffline { get; set; }
        public virtual Boolean ConfigParalisaOS { get; set; }
        public virtual Boolean ObrigaCaixaPorta { get; set; }
        public virtual Boolean AutorizaBaixaServicoBackoffice { get; set; }
        public virtual Boolean ValidarDocumentoLead { get; set; }
        public virtual string ConsumerKeyCPF { get; set; }
        public virtual string ConsumerKeyCNPJ { get; set; }
        public virtual string ConsumerSecretCPF { get; set; }
        public virtual string ConsumerSecretCNPJ { get; set; }
        public virtual string LimitePotenciaONUBaixa { get; set; }
        public virtual bool Ambiente_Teste { get; set; }
        public virtual bool Utilizar_Esquema_Cores_Padrao_Ola { get; set; }
        public virtual int? Tempo_Confirmacao_Ciencia_SOS { get; set; }
        public virtual int? Tempo_Atualizacao_Alerta_SOS { get; set; }
        public virtual List<URL_SistemaListaModel> URL_Sistemas

        {
            get
            {
                if (URL_Sistemas_Lista == null)
                    URL_Sistemas_Lista = new List<URL_SistemaListaModel>();
                return URL_Sistemas_Lista;
            }
            set
            {
                URL_Sistemas_Lista = value;
            }
        }

        public virtual List<Anuncios_OLAConfiguracoesGeraisModel> Anuncios_OLA

        {
            get
            {
                if (Anuncios_OLA_Lista == null)
                    Anuncios_OLA_Lista = new List<Anuncios_OLAConfiguracoesGeraisModel>();
                return Anuncios_OLA_Lista;
            }
            set
            {
                Anuncios_OLA_Lista = value;
            }
        }

        public virtual List<EmailListaModel> Emails
        {
            get
            {
                if (Emails_Lista == null)
                    Emails_Lista = new List<EmailListaModel>();
                return Emails_Lista;
            }
            set
            {
                Emails_Lista = value;
            }
        }

        public virtual List<IntegracoesExternasListaModel> Integracoes
        {
            get
            {
                if (Integracoes_Lista == null)
                    Integracoes_Lista = new List<IntegracoesExternasListaModel>();
                return Integracoes_Lista;
            }
            set
            {
                Integracoes_Lista = value;
            }
        }

        public virtual Boolean Criacao_Atendimento_DominioEmail { get; set; }

        public long? NumeroContratoInicial { get; set; }
        public bool EnviarTermoAdesaoAuto { get; set; }

        #region Configuração das Consultas

        public bool ConsultaViabilidadeExibeLead { get; set; }
        public bool ConsultaPendenciaFinanceiraExibeLead { get; set; }
        public bool ConsultaPendenciaEnderecoExibeLead { get; set; }
        public bool ConsultaCreditoExibeLead { get; set; }

        public bool ConsultaViabilidadeQualificaLead { get; set; }
        public bool ConsultaPendenciaFinanceiraQualificaLead { get; set; }
        public bool ConsultaPendenciaEnderecoQualificaLead { get; set; }
        public bool ConsultaCreditoQualificaLead { get; set; }

        public bool ConsultaViabilidadeExibeVenda { get; set; }
        public bool ConsultaPendenciaFinanceiraExibeVenda { get; set; }
        public bool ConsultaPendenciaEnderecoExibeVenda { get; set; }
        public bool ConsultaCreditoExibeVenda { get; set; }

        public virtual bool ConsultaViabilidadeExecutaAutoApp { get; set; }
        public virtual bool ConsultaPendenciaFinanceiraExecutaAutoApp { get; set; }
        public virtual bool ConsultaPendenciaEnderecoExecutaAutoApp { get; set; }
        public virtual bool ConsultaCreditoExecutaAutoApp { get; set; }

        public bool ConsultaViabilidadeRestringeVenda { get; set; }
        public bool ConsultaPendenciaFinanceiraRestringeVenda { get; set; }
        public bool ConsultaPendenciaEnderecoRestringeVenda { get; set; }
        public bool ConsultaCreditoRestringeVenda { get; set; }

        public bool ConsultaViabilidadeVendedorVisualiza { get; set; }
        public bool ConsultaPendenciaFinanceiraVendedorVisualiza { get; set; }
        public bool ConsultaPendenciaEnderecoVendedorVisualiza { get; set; }
        public bool ConsultaCreditoVendedorVisualiza { get; set; }

        public int? ConsultaViabilidadeTempoValidade { get; set; }
        public int? ConsultaPendenciaFinanceiraTempoValidade { get; set; }
        public int? ConsultaPendenciaEnderecoTempoValidade { get; set; }
        public int? ConsultaCreditoTempoValidade { get; set; }

        #endregion
        public virtual bool Definir_Requisitos_Aprovacao_Viabilidade { get; set; }
        public virtual bool Requisito_Vinculo_Endereco { get; set; }
        public virtual bool Requisito_Dentro_Raio { get; set; }
        public virtual bool Requisito_Dentro_Area_Cobertura { get; set; }
        public virtual bool UsaAlertaNotificacoes { get; set; }
        public virtual int? MinutosAtualizaNotificacoes { get; set; }
        public virtual Boolean DesconsiderarDeslocamento { get; set; }
        public string TempoMinimoSlaRoteirizacaoAutomatico { get; set; }
        public virtual bool Exibir_Apenas_Caixas_Atendimento_Dentro_Raio { get; set; }
        public virtual string LimiteDistanciaCaixaAtendimento { get; set; }        
        public virtual Boolean EncaixeOsProximoVencimento { get; set; }
        public virtual Boolean BloquearVendaSemLead { get; set; }
    }
    public class EmailListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Email { get; set; }
        public virtual String Senha { get; set; }
        public virtual String ServidorRecebimento { get; set; }
        public virtual String PortaRecebimento { get; set; }
        public virtual String ServidorEnvio { get; set; }
        public virtual String PortaEnvio { get; set; }
        public virtual Boolean Utilizar_SSL_Imap { get; set; }
        public virtual Boolean Principal { get; set; }
        public virtual Boolean Utilizar_SSL_Smtp { get; set; }
        public virtual AuthMethodImap AuthMethodImap { get; set; }
        public virtual String AuthMethodImap_Descricao { get; set; }

      
    }

    public class IntegracoesExternasListaModel
    {
        public long Id { get; set; }
        public string UrlApi { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Token { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string HeaderIntegracaoExterna1 { get; set; }
        public bool EnviarTodosParametros { get; set; }
        public string PortaEnvio { get; set; }
        public IntegracaoExterna Integracao { get; set; }
        public string IntegracaoDesc { get; set; }
        public EnumTipoIntegracao TipoIntegracao { get; set; }
        public string TipoIntegracaoDesc { get; set; }

    }

    public class PerfilVisualizacaoServicoComboConfig
    {
        /// <summary>
        ///     Define se a combo deve levar em consideração os parâmetros
        ///     configurados no perfil de visualização de serviços associado
        ///     ao usuário
        /// </summary>
        public bool UsaPerfilVisualizacaoServico { get; set; }
    }

    public class Configuracoes_Gerais_DataTableRequest : DataTableRequest
    {

    }
    public class ConfiguracoesRestricoesLocalBaixaOs 
    {
        public bool CriticarBaixaLocalServico { get; set; }
        public Int32? LimiteDistanciaBaixa { get; set; }
    }
    public class Acoes_Prova_Vida
    {
        public string Ordem { get; set; }
        public string Valor { get; set; }
    }
}
