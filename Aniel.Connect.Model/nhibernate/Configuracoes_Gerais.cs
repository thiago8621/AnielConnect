using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class Configuracoes_Gerais : BaseModel
    {
        public virtual DateTime? LimiteMinimoTimeline { get; set; }
        public virtual DateTime? HorarioCorteRoteirizacaoDashBoard { get; set; }
        public virtual DateTime? LimiteMaximoTimeline { get; set; }
        public virtual Int32? Max_Dias_Sem_Contato { get; set; }
        public virtual Int32? Max_Dias_Sem_Aprovacao { get; set; }
        public virtual Int32? Distancia_Exibicao_Elemento { get; set; }
        public virtual Int32? Disponibilidade { get; set; }
        public virtual Boolean Trava_Atendimento_OS { get; set; }
        public virtual Boolean Exige_Agendamento_Servico { get; set; }
        public virtual Boolean Empresa_Roteirizacao_Servico { get; set; }
        public virtual Boolean Encerrar_Atendimento_Junto_OS { get; set; }
        public virtual Boolean UtilizaRouter { get; set; }
        public virtual Boolean Acumular_Descontos_Diferentes { get; set; }       
        public virtual Boolean Acumular_Descontos_Mesmo_Tipo { get; set; }
        public virtual Boolean ApenasServicosProdutivosReincidencia { get; set; } 
        public virtual Boolean Permite_Venda_Aberta_Cliente { get; set; } 
        public virtual Boolean Permite_Venda_Aberta_Imovel { get; set; } 
        public virtual Boolean Permite_Exclusao_Cancelamento_Plano_Integrado { get; set; }
        
        public virtual Formato_Protocolo_OS? Formato_Protocolo_OS { get; set; }
        public virtual Formato_Protocolo_Atendimento? Formato_Protocolo_Atendimento { get; set; }
        public virtual Identificacao_Cliente? Identificacao_Cliente { get; set; }

        public virtual Sistema? Sistema { get; set; }
        public virtual String Nome_Cliente_Sinapse { get; set; }
        public virtual String CNPJ { get; set; }
        public virtual String Id_Ola_Cliente { get; set; }
        public virtual String OLA_URL_Logo { get; set; }
        public virtual String UrlPowerBI { get; set; }

        public virtual String Padrao_Login { get; set; }
        public virtual String Padrao_Login_Ultimo { get; set; }

        /// <summary>
        /// Usuario da Api
        /// </summary>
        public virtual String Usuario_APIANIEL { get; set; }
        /// <summary>
        /// Senha da Api
        /// </summary>
        public virtual String Senha_APIANIEL { get; set; }
        /// <summary>
        /// Token da Api
        /// </summary>
        public virtual String Token_APIANIEL { get; set; }

        public virtual String OLA_Cor_Principal { get; set; }
        public virtual String OLA_Cor_Assests { get; set; }
        public virtual String OLA_Cor_Assets_Dark { get; set; }
        public virtual String OLA_Cor_Principal_Dark { get; set; }
        public virtual String OLA_Cor_Fonte { get; set; }

        public virtual String Id_WidePay { get; set; }
        public virtual String Token_Widepay { get; set; }
        public virtual Boolean Checkin_Checkout_Auto { get; set; }
        public virtual Boolean Num_Contrato_Gerado_Auto { get; set; }

        public virtual Boolean Exige_Termo_Adesao { get; set; }
        public virtual Boolean Exige_Termo_Contrato { get; set; }
        public virtual Boolean Utiliza_Envio_Automatico_Contrato { get; set; }
        public virtual Boolean Exige_Foto_Termo { get; set; }
        public virtual String Acoes_Prova_Vida { get; set; }
        public virtual Tipo_Comprovante_Termo? Tipo_Comprovante_Termo { get; set; }
        public virtual Boolean Permite_Work_Sem_Termo { get; set; }
        public virtual Boolean Permite_Anexar_Doc_Termo { get; set; }
        public virtual int? Porcentagem_Video { get; set; }
        public virtual int? Porcentagem_Similaridade { get; set; }

        public virtual Boolean Ofertar_Plano_Produto_Cobertura { get; set; }
        public virtual Boolean ValidaNumeroTelefone { get; set; }
        public virtual long? IdModeloMensagemSMS { get; set; }
        public virtual Boolean BloquearVendaSemContatoValidado { get; set; }
        public virtual Boolean BloquearVendaSemLead { get; set; }
        public virtual Boolean Utiliza_Cadastro_Endereco { get; set; }
        public virtual Boolean Permite_OS_LEAD { get; set; }
        public virtual Boolean Atribuir_Tempo_Limite_Venda { get; set; }
        public virtual Boolean Reserva_Porta_Automatica { get; set; }
        public virtual Int32? Qtde_Dias_Avaliacao_Comercial { get; set; }
        public virtual Int32? Qtde_Meses_Visualizacao_Venda { get; set; }
        public virtual Int32? Dia_Mes_Finaliza_Medicao_Comissao { get; set; }
        public virtual Boolean Usa_Dias_Liberacao_Did { get; set; }
        public virtual Boolean ConfirmarEnderecoAgendamento { get; set; }
        public virtual int? Dias_Liberacao_Did { get; set; }

        private IList<URL_Sistema> _url_Sistemas;
        private IList<Anuncios_OLA> _anuncios_OLA;

        public virtual DateTime? Tempo_Medio_Deslocamento { get; set; }
        public virtual DateTime? Tempo_Execucao_Padrao { get; set; }
        public virtual Int32? Tempo_Atualizacao_Automatica_Painel_Acompanhamento_Tecnico { get; set; }
        public virtual Int32? Tempo_Atualizacao_Automatica_Timeline { get; set; }
        public virtual Int32? SLA_Servico_Padrao { get; set; }
        public virtual Double? ValorPadraoEstimadoLitro { get; set; }
        public virtual Double? MediaPadraoKmL { get; set; }
        public virtual Boolean? Atualiza_Ola { get; set; }
        public virtual Boolean UtilizaPoint_RoteirizacaoAgendamento { get; set; }
        public virtual Boolean UtilizaEva { get; set; }
        public virtual Boolean Criterio_Habilidade { get; set; }
        public virtual Boolean Considerar_Agendamento_Periodo { get; set; }
        public virtual Boolean Criterio_Area { get; set; }
        public virtual Boolean Criterio_SLA_Execucao { get; set; }
        public virtual Boolean Criterio_SLA_Hora { get; set; }

        /// <summary>
        /// distancia minima em metros para que seja aplicado o tempo referente ao deslocamento
        /// </summary>
        public virtual Int32? DistanciaMinimaDeslocamento { get; set; }

        /// <summary>
        /// Valro em Tempo de deslocamento para deslocamento na roteirizaçao
        /// </summary>
        public virtual DateTime? TempoBaseDeslocamentoPorKm { get; set; }
        public virtual DateTime? TempoMinimoPrimeiraJanela { get; set; }
        public virtual Boolean Distribuicao_Por_Igual { get; set; }
        public virtual Boolean Distribuicao_Preenchendo_Equipe { get; set; }
        public virtual Enum_Pessoa_Contato_Primeira_Vez Pessoa_Contato_Primeira_Vez { get; set; }
        public virtual Boolean ExibeRDOs { get; set; }
        public virtual Boolean ParametroTimeline { get; set; }

        public virtual Boolean Atendimento_Cria_Cliente { get; set; }
        public virtual Boolean CriarAtendimentoSemPlanoProduto { get; set; }
        public virtual Boolean Criacao_Atendimento_Email { get; set; }
        public virtual Boolean Utiliza_AgrupamentoDistancia { get; set; }
        public virtual Boolean Criacao_Atendimento_DominioEmail { get; set; }

        public virtual Boolean Exige_LatLong_Agendamento { get; set; }
        public virtual Boolean RoteirizarSemLatLong { get; set; }
        public virtual Boolean CriticarBaixaLocalServico { get; set; }
        public virtual Boolean ObrigaCaixaPorta { get; set; }
        public virtual Boolean RoteirizarPorCaixa { get; set; }
        public virtual Boolean ComercialUsaSms { get; set; }

        public virtual Int32? Caixa_RaioConsulta { get; set; }
        public virtual Int32? Caixa_DistanciaPorta { get; set; }
        public virtual Int32? DistanciaMaximaAgrupamento { get; set; }
        public virtual Int32? LimiteDistanciaParaDeslocamento { get; set; }
        public virtual Int32? LimiteDistanciaBaixa { get; set; }


        public virtual Boolean Utiliza_Integracao_SMS { get; set; }
        public virtual String Usuario_Integracao_SMS { get; set; }
        public virtual String Senha_Integracao_SMS { get; set; }
        public virtual String Token_Integracao_SMS { get; set; }
        public virtual Sinapse.Model.enumerados.Tipo_Integracao_SMS? Tipo_Integracao_SMS { get; set; }
        public virtual InfoOSTimelineEnum? CampoinfoOStimeline { get; set; }
        public virtual bool ExibirInfoOSTimeline { get; set; }
        public virtual bool Utiliza_Certificacao_Assinatura { get; set; }
        public virtual TipoIntegracaoAssinaturaEnum? Tipo_Integracao_Assinatura { get; set; }
        public virtual String Token_Integracao_Assinatura {get; set;}

        public virtual bool Utiliza_Armazenamento_Nuvem { get; set; }
        public virtual string ProjectId_Cloud_Storage { get; set; }
        public virtual string CredentialJson_Cloud_Storage { get; set; }

        public virtual IList<URL_Sistema> Url_Sistemas
        {
            get
            {
                if (_url_Sistemas == null)
                    _url_Sistemas = new List<URL_Sistema>();
                return _url_Sistemas;
            }
            set
            {
                _url_Sistemas = value;
            }
        }

        public virtual IList<Anuncios_OLA> Anuncios_Ola
        {
            get
            {
                if (_anuncios_OLA == null)
                    _anuncios_OLA = new List<Anuncios_OLA>();
                return _anuncios_OLA;
            }
            set
            {
                _anuncios_OLA = value;
            }
        }

        public virtual long? NumeroContratoInicial { get; set; }
        public virtual Boolean Gerar_Atendimento_Venda { get; set; }

        #region Configuração das Consultas

        public virtual bool ConsultaViabilidadeExibeLead { get; set; }
        public virtual bool ConsultaPendenciaFinanceiraExibeLead { get; set; }
        public virtual bool ConsultaPendenciaEnderecoExibeLead { get; set; }
        public virtual bool ConsultaCreditoExibeLead { get; set; }
        
        public virtual bool ConsultaViabilidadeQualificaLead { get; set; }
        public virtual bool ConsultaPendenciaFinanceiraQualificaLead { get; set; }
        public virtual bool ConsultaPendenciaEnderecoQualificaLead { get; set; }
        public virtual bool ConsultaCreditoQualificaLead { get; set; }

        public virtual bool ConsultaViabilidadeExibeVenda { get; set; }
        public virtual bool ConsultaPendenciaFinanceiraExibeVenda { get; set; }
        public virtual bool ConsultaPendenciaEnderecoExibeVenda { get; set; }
        public virtual bool ConsultaCreditoExibeVenda { get; set; }

        public virtual bool ConsultaViabilidadeExecutaAutoApp { get; set; }
        public virtual bool ConsultaPendenciaFinanceiraExecutaAutoApp { get; set; }
        public virtual bool ConsultaPendenciaEnderecoExecutaAutoApp { get; set; }
        public virtual bool ConsultaCreditoExecutaAutoApp { get; set; }

        public virtual bool ConsultaViabilidadeRestringeVenda { get; set; }
        public virtual bool ConsultaPendenciaFinanceiraRestringeVenda { get; set; }
        public virtual bool ConsultaPendenciaEnderecoRestringeVenda { get; set; }
        public virtual bool ConsultaCreditoRestringeVenda { get; set; }

        public virtual bool ConsultaViabilidadeVendedorVisualiza { get; set; }
        public virtual bool ConsultaPendenciaFinanceiraVendedorVisualiza { get; set; }
        public virtual bool ConsultaPendenciaEnderecoVendedorVisualiza { get; set; }
        public virtual bool ConsultaCreditoVendedorVisualiza { get; set; }

        public virtual int? ConsultaViabilidadeTempoValidade { get; set; }
        public virtual int? ConsultaPendenciaFinanceiraTempoValidade { get; set; }
        public virtual int? ConsultaPendenciaEnderecoTempoValidade { get; set; }
        public virtual int? ConsultaCreditoTempoValidade { get; set; }
        public virtual string TokenOla { get; set; }
        public virtual string Telefone_Suporte { get; set; }
        public virtual string Email_Suporte { get; set; }

        public virtual Boolean OcultarAssinaturaTermoPremier { get; set; }
        public virtual Boolean OcultarAgendamentoOsPremier { get; set; }
        public virtual Boolean PermiteDocVendaGaleriaPremier { get; set; }
        public virtual Boolean EnviarCheckListAuto { get; set; }

        #endregion

        public virtual bool Bloqueia_Lead { get; set; }
        public virtual int? Qtd_Dias_Bloqueio_Lead { get; set; }
        public virtual bool CriarOsSemPlanoProduto { get; set; }

        public virtual bool EnviarTermoAdesaoAuto { get; set; }
        public virtual bool Definir_Requisitos_Aprovacao_Viabilidade { get; set; }
        public virtual bool  Requisito_Vinculo_Endereco{ get; set; }
        public virtual bool Requisito_Dentro_Raio { get; set; }
        public virtual bool Requisito_Dentro_Area_Cobertura { get; set; }
        public virtual bool AtribuirOSReincTec { get; set; }
        public virtual bool UsaAlertaNotificacoes { get; set; }
        public virtual bool EnderecoInstalacaoMesmoPrincipal { get; set; }
        public virtual int? MinutosAtualizaNotificacoes { get; set; }

        public virtual bool CriticarBaixaPotenciaONU { get; set; }
        public virtual bool CriticarBaixaLoginRadiusOffline { get; set; }
        public virtual double? LimitePotenciaONUBaixa { get; set; }
        public virtual Boolean DesconsiderarDeslocamento { get; set; }
        public virtual Boolean PermitirAuditoriaOSImprod { get; set; }
        public virtual Boolean PermitirMotivoReagendamento { get; set; }
        public virtual bool Exibir_Apenas_Caixas_Atendimento_Dentro_Raio { get; set; }
        public virtual int? LimiteDistanciaCaixaAtendimento { get; set; }


        public virtual DateTime? TempoMinimoSlaRoteirizacaoAutomatico { get; set; }
        public virtual string Uuid_Provedor_Ola { get; set; }


        public virtual bool Ambiente_Teste { get; set; }
        public virtual bool Utilizar_Esquema_Cores_Padrao_Ola { get; set; }

        #region SERPRO
        public virtual Boolean ValidarDocumentoLead { get; set; }
        public virtual string ConsumerKeyCPF { get; set; }
        public virtual string ConsumerKeyCNPJ { get; set; }
        public virtual string ConsumerSecretCPF { get; set; }
        public virtual string ConsumerSecretCNPJ { get; set; }
        #endregion


        public virtual int? Tempo_Confirmacao_Ciencia_SOS { get; set; }
        public virtual int? Tempo_Atualizacao_Alerta_SOS { get; set; }

        #region Mobile
        public virtual Boolean Exige_Avaliacao_Comercial { get; set; }       
        public virtual Boolean EncaixeOsProximoVencimento { get; set; }
        #endregion

        public virtual List<LogConfigGeraisAniel> GeraLog(Configuracoes_Gerais pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogConfigGeraisAniel> logs = new List<LogConfigGeraisAniel>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Configuracoes_Gerais, LogConfigGeraisAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Configuracoes_Gerais, LogConfigGeraisAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ROTEIRIZAR_LATLONG, pOld, this, p => p.RoteirizarSemLatLong);
                Log_ComparaProperty<Configuracoes_Gerais, LogConfigGeraisAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ROTEIRIZAR_CAIXA, pOld, this, p => p.RoteirizarPorCaixa);

            }

            return logs;
        }
    }
}
