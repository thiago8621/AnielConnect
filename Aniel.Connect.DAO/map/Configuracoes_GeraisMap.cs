using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;

namespace Aniel.Connect.DAO.map
{
    public class Configuracoes_GeraisMap : ClassMapBase<Configuracoes_Gerais>
    {
        public Configuracoes_GeraisMap() : base()
        {
            Table("TB_CONFIGURACOES_GERAIS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONFIGURACOES_GERAIS");
            /*Map(x => x.SLA_Data_Abertura, "SLA_DATA_ABERTURA").CustomType<Enum_Tipo_Sla>()*/;
            Map(x => x.CNPJ, "CNPJ");
            Map(x => x.Nome_Cliente_Sinapse, "NOME_CLIENTE_SINAPSE");

            //Map(x => x.Modelo_Envio, "MODELO_ENVIO").Length(500);
            Map(x => x.OLA_Cor_Assests, "OLA_COR_ASSESTS").Length(255);
            Map(x => x.OLA_Cor_Assets_Dark, "OLA_COR_ASSETS_DARK").Length(50);
            Map(x => x.OLA_Cor_Fonte, "OLA_COR_FONTE");
            Map(x => x.OLA_Cor_Principal, "OLA_COR_PRINCIPAL").Length(50);
            Map(x => x.OLA_Cor_Principal_Dark, "OLA_COR_PRINCIPAL_DARK").Length(50);
            Map(x => x.OLA_URL_Logo, "OLA_URL_LOGO").Length(500);
            Map(x => x.Acumular_Descontos_Diferentes, "ACUMULAR_DESCONTOS_DIFERENTE");

            Map(x => x.Acumular_Descontos_Mesmo_Tipo, "ACUMULAR_DESCONTOS_NATUREZA");
            Map(x => x.Encerrar_Atendimento_Junto_OS, "ENCERRAR_ATENDIMENTO_OS");
            Map(x => x.Id_Ola_Cliente, "ID_OLA_CLIENTE").Length(50);

            Map(x => x.Usuario_APIANIEL, "USUARIO_APIANIEL").Length(50);
            Map(x => x.Senha_APIANIEL, "SENHA_APIANIEL").Length(50);
            Map(x => x.Token_APIANIEL, "TOKEN_APIANIEL").Length(50);

            Map(x => x.Max_Dias_Sem_Contato, "MAX_DIAS_SEM_CONTATO");            

            Map(x => x.Distancia_Exibicao_Elemento, "DISTANCIA_EXIBICAO_ELEMENTO");
            Map(x => x.Disponibilidade, "DISPONIBILIDADE");
            //Map(x => x.Trava_Atendimento_OS, "TRAVA_ATENDIMENTO_OS");
            Map(x => x.Exige_Agendamento_Servico, "EXIGE_AGENDAMENTO_SERVICO");
            Map(x => x.Empresa_Roteirizacao_Servico, "EMPRESA_ROTEIRIZACAO_SERVICO");
            Map(x => x.Tempo_Medio_Deslocamento, "TEMPO_MEDIO_DESLOCAMENTO");
            Map(x => x.Tempo_Execucao_Padrao, "TEMPO_EXECUCAO_PADRAO");
            Map(x => x.Tempo_Atualizacao_Automatica_Painel_Acompanhamento_Tecnico, "TEMPO_ATUALIZ_AUTO_ACOMP_TECNIC");
            Map(x => x.Tempo_Atualizacao_Automatica_Timeline, "TEMPO_ATUALIZ_AUTO_TIMELINE");
            Map(x => x.TempoMinimoSlaRoteirizacaoAutomatico, "TEMPO_SLA_ROTEIRIZACAO");
            Map(x => x.ParametroTimeline, "PARAM_DATAHORA_TIMELINE");
            Map(x => x.SLA_Servico_Padrao, "SLA_SERVICO_PADRAO");
            Map(x => x.ConfirmarEnderecoAgendamento, "CONFIRMAR_ENDERECO_AGENDAMENTO");

            Map(x => x.Id_WidePay, "ID_WIDEPAY").Length(50);
            Map(x => x.Token_Widepay, "TOKEN_WIDEPAY").Length(50);
            Map(x => x.Checkin_Checkout_Auto, "CHECKIN_CHECKOUT_AUTO");
            Map(x => x.Num_Contrato_Gerado_Auto, "NUM_CONTRATO_GERADO_AUTO");
           // Map(x => x.Exige_Avaliacao_Comercial, "EXIGE_AVALIACAO_COMERCIAL");
            Map(x => x.Exige_Termo_Adesao, "EXIGE_TERMO_ADESAO");
            Map(x => x.Exige_Termo_Contrato, "EXIGE_TERMO_CONTRATO");
            Map(x => x.Utiliza_Envio_Automatico_Contrato, "UTILIZA_ENVIO_AUTO_CTT");
            Map(x => x.Exige_Foto_Termo, "EXIGE_FOTO_TERMO");
            Map(x => x.Permite_Anexar_Doc_Termo, "PERMITE_ANEXAR_DOC_TERMO");
            Map(x => x.Porcentagem_Similaridade, "PORCENTAGEM_SIMILARIDADE");
            Map(x => x.Porcentagem_Video, "PORCENTAGEM_VIDEO");
            Map(x => x.Tipo_Comprovante_Termo, "TIPO_COMPROVANTE_TERMO").CustomType<Sinapse.Model.enumerados.Tipo_Comprovante_Termo>();
            Map(x => x.Permite_Work_Sem_Termo, "PERMITE_WORK_SEM_TERMO");
            Map(x => x.Ofertar_Plano_Produto_Cobertura, "OFERTAR_PLANO_PRODUTO_COBERTURA");
            Map(x => x.Utiliza_Cadastro_Endereco, "UTILIZA_CADASTRO_ENDERECO");
            Map(x => x.Permite_OS_LEAD, "PERMITE_CRIAR_OS_LEAD");
            
            Map(x => x.IdModeloMensagemSMS, "MODELO_MENSAGEM_SMS");
            Map(x => x.ValidaNumeroTelefone, "VALIDA_NUMERO_TELEFONE");
            Map(x => x.BloquearVendaSemContatoValidado, "BLOQUEAR_VENDA_CONTATO_V");
            Map(x => x.BloquearVendaSemLead, "BLOQUEAR_VENDA_SEM_LEAD");

            Map(x => x.Atribuir_Tempo_Limite_Venda, "ATRIBUIR_TEMPO_LIMITE_VENDA");
            Map(x => x.Reserva_Porta_Automatica, "RESERVA_PORTA_AUTOMATICA");
            Map(x => x.Permite_Venda_Aberta_Cliente, "PERMITE_VENDA_ABERTA_CLIENTE");
            Map(x => x.Permite_Venda_Aberta_Imovel, "PERMITE_VENDA_ABERTA_IMOVEL");
                        
            Map(x => x.Permite_Exclusao_Cancelamento_Plano_Integrado, "PERMITE_EXCLS_CNCLMNT_PLANO_INT");


            Map(x => x.Qtde_Dias_Avaliacao_Comercial, "QTDE_DIAS_AVALIACAO_COMERCIAL");
            Map(x => x.Qtde_Meses_Visualizacao_Venda, "QTDE_MESES_VISUALIZACAO_VENDA");
            Map(x => x.Dia_Mes_Finaliza_Medicao_Comissao, "DIA_MES_FINAL_MED_COMISSAO");

            Map(x => x.Dias_Liberacao_Did, "DIAS_LIBERACAO_DID");
            Map(x => x.Usa_Dias_Liberacao_Did, "USA_DIAS_LIBERACAO_DID");


            Map(x => x.Formato_Protocolo_OS, "FORMATO_PROTOCOLO_OS").CustomType<Formato_Protocolo_OS>();
            Map(x => x.Formato_Protocolo_Atendimento, "FORMATO_PROTOCOLO_ATENDIMENTO").CustomType<Formato_Protocolo_Atendimento>();
            Map(x => x.Identificacao_Cliente, "FILIACAO_PAI").CustomType<Identificacao_Cliente>();
            Map(x => x.Sistema, "SISTEMA").CustomType<Sistema>();

            HasMany(x => x.Url_Sistemas).KeyColumn("ID_CONFIGURACOES_GERAIS").Cascade.All();
            HasMany(x => x.Anuncios_Ola).KeyColumn("ID_CONFIGURACOES_GERAIS").Cascade.All();
            

            Map(x => x.Atualiza_Ola, "ATUALIZA_OLA");

            Map(x => x.ApenasServicosProdutivosReincidencia, "APENAS_SERV_PROD_REINC");

            Map(x => x.DistanciaMinimaDeslocamento, "DISTANCIA_MINIMA_DESLOCAMENTO");
            Map(x => x.TempoBaseDeslocamentoPorKm, "TEMPO_BASE_DESLOCAMENTO_POR_KM");
            Map(x => x.UtilizaPoint_RoteirizacaoAgendamento, "UTILIZAPOINT_ROTEIRIZACAO");
            Map(x => x.UtilizaEva, "UTILIZA_EVA");
            Map(x => x.Utiliza_AgrupamentoDistancia, "UTILIZA_AGRUPAMENTODISTANCIA");
            Map(x => x.Criterio_Habilidade, "CRITERIO_HBILIDADE");
            Map(x => x.Considerar_Agendamento_Periodo, "CONSIDERAR_AGEND_PERIODO");
            Map(x => x.Criterio_Area, "CRITERIO_AREA");
            Map(x => x.Criterio_SLA_Execucao, "CRITERIO_SLA_EXEUCAO");
            Map(x => x.Criterio_SLA_Hora, "CRITERIO_SLA_HORA");
            Map(x => x.Distribuicao_Por_Igual, "DISTRIBUICAO_POR_IGUAL");
            Map(x => x.Distribuicao_Preenchendo_Equipe, "DISTRIBUICAO_EQUIPE");
            Map(x => x.Pessoa_Contato_Primeira_Vez, "PESSOA_CONTATO_PRMEIRA_VEZ").CustomType<Enum_Pessoa_Contato_Primeira_Vez>();
            Map(x => x.RoteirizarPorCaixa, "ROTEIRIZAR_POR_CAIXA").CustomType<Boolean>();
            Map(x => x.RoteirizarSemLatLong, "ROTEIRIZAR_SEM_LATLONG").CustomType<Boolean>();
            Map(x => x.CriticarBaixaLocalServico, "CRITICAR_BAIXA_LOCAL_SERV").CustomType<Boolean>();
            Map(x => x.LimiteDistanciaBaixa, "LIMITE_DISTANCIA_BAIXA");
            Map(x => x.Exige_LatLong_Agendamento, "EXIGE_LATLONG_AGENDAMENTO").CustomType<Boolean>();
            Map(x => x.ExibeRDOs, "EXIBE_RDOS");
            Map(x => x.Criacao_Atendimento_Email, "CRIACAO_ATENDIMENTO_EMAIL");
            Map(x => x.Atendimento_Cria_Cliente, "ATENDIMENTO_CRIA_CLIENTE");
            Map(x => x.CriarAtendimentoSemPlanoProduto, "CRIAR_ATENDIMENTO_SEM_PLANO");
            Map(x => x.Criacao_Atendimento_DominioEmail, "CRIACAO_ATEND_DOMINIOEMAIL");
            Map(x => x.ValorPadraoEstimadoLitro, "VALOR_PADRAO_LITRO");
            Map(x => x.MediaPadraoKmL, "MEDIA_KM_LITRO");
            Map(x => x.Utiliza_Certificacao_Assinatura, "UTILIZA_CERTIFICACAO_ASSINATURA");
            Map(x => x.Tipo_Integracao_Assinatura, "TIPO_INTEGRACAO_ASSINATURA").CustomType<TipoIntegracaoAssinaturaEnum>();
            Map(x => x.Token_Integracao_Assinatura, "TOKEN_INTEGRACAO_ASSINATURA").Length(200);
            
            Map(x => x.Padrao_Login, "PADRAO_LOGIN");
            Map(x => x.Padrao_Login_Ultimo, "PADRAO_LOGIN_Ultimo");
            Map(x => x.ComercialUsaSms, "COMERCIAL_USA_SMS");
            Map(x => x.DistanciaMaximaAgrupamento, "DISTANCIA_MAXIMA_AGRUPAMENTO");
            Map(x => x.LimiteDistanciaParaDeslocamento, "LIMITE_DISTANCIA_DESLOCAMENTO");
            Map(x => x.UrlPowerBI, "URL_POWERBI");

            Map(x => x.LimiteMinimoTimeline, "LIMITE_MINIMO_TIMELINE");
            Map(x => x.LimiteMaximoTimeline, "LIMITE_MAXIMO_TIMELINE");
            Map(x => x.UtilizaRouter, "UTILIZA_ROUTER");

            Map(x => x.NumeroContratoInicial, "NUMERO_CONTRATO_INICIAL");
            Map(x => x.Gerar_Atendimento_Venda, "GERAR_ATEND_APOS_VENDA");

            Map(x => x.Utiliza_Integracao_SMS, "UTILIZA_INTEGRACAO_SMS");
            Map(x => x.Tipo_Integracao_SMS, "TIPO_INTEGRACAO_SMS").CustomType<Sinapse.Model.enumerados.Tipo_Integracao_SMS>();
            Map(x => x.Usuario_Integracao_SMS, "USUARIO_INTEGRACAO_SMS");
            Map(x => x.Senha_Integracao_SMS, "SENHA_INTEGRACAO_SMS");
            Map(x => x.Token_Integracao_SMS, "TOKEN_INTEGRACAO_SMS");

            Map(x => x.Utiliza_Armazenamento_Nuvem, "UTILIZA_CLOUD_STORAGE");
            Map(x => x.ProjectId_Cloud_Storage, "PROJECTID_CLOUD_STORAGE");
            Map(x => x.CredentialJson_Cloud_Storage, "JSON_CLOUD_STORAGE").Length(5000);
            Map(x => x.TempoMinimoPrimeiraJanela, "TEMPO_MIN_PRIMEIRO_AGENDAMENTO");
            Map(x => x.HorarioCorteRoteirizacaoDashBoard, "HORARIO_CORTE_DASHBOARD");

            #region Configurações das consultas 

            Map(x => x.ConsultaViabilidadeExibeLead, "CON_VIA_EXIBE_LEAD");
            Map(x => x.ConsultaPendenciaFinanceiraExibeLead, "CON_PEND_FIN_EXIBE_LEAD");
            Map(x => x.ConsultaPendenciaEnderecoExibeLead, "CON_END_EXIBE_LEAD");
            Map(x => x.ConsultaCreditoExibeLead, "CON_CRED_EXIBE_LEAD");
            
            Map(x => x.ConsultaViabilidadeQualificaLead, "CON_VIA_QUAL_LEAD");
            Map(x => x.ConsultaPendenciaFinanceiraQualificaLead, "CON_PEND_FIN_QUAL_LEAD");
            Map(x => x.ConsultaPendenciaEnderecoQualificaLead, "CON_END_QUAL_LEAD");
            Map(x => x.ConsultaCreditoQualificaLead, "CON_CRED_QUAL_LEAD");
            
            Map(x => x.ConsultaViabilidadeExibeVenda, "CON_VIA_EXIBE_VENDA");
            Map(x => x.ConsultaPendenciaFinanceiraExibeVenda, "CON_PEND_FIN_EXIBE_VENDA");
            Map(x => x.ConsultaPendenciaEnderecoExibeVenda, "CON_END_EXIBE_VENDA");
            Map(x => x.ConsultaCreditoExibeVenda, "CON_CRED_EXIBE_VENDA");

            Map(x => x.ConsultaViabilidadeExecutaAutoApp, "CON_VIA_EXECUTA_APP");
            Map(x => x.ConsultaPendenciaFinanceiraExecutaAutoApp, "CON_PEND_FIN_EXECUTA_APP");
            Map(x => x.ConsultaPendenciaEnderecoExecutaAutoApp, "CON_END_EXECUTA_APP");
            Map(x => x.ConsultaCreditoExecutaAutoApp, "CON_CRED_EXECUTA_APP");

            Map(x => x.ConsultaViabilidadeRestringeVenda, "CON_VIA_RESTRINGE_VENDA");
            Map(x => x.ConsultaPendenciaFinanceiraRestringeVenda, "CON_PEND_FIN_RESTRINGE_VENDA");
            Map(x => x.ConsultaPendenciaEnderecoRestringeVenda, "CON_END_RESTRINGE_VENDA");
            Map(x => x.ConsultaCreditoRestringeVenda, "CON_CRED_RESTRINGE_VENDA");
            
            Map(x => x.ConsultaViabilidadeVendedorVisualiza, "CON_VIA_VENDEDOR_VIS");
            Map(x => x.ConsultaPendenciaFinanceiraVendedorVisualiza, "CON_PEND_FIN_VENDEDOR_VIS");
            Map(x => x.ConsultaPendenciaEnderecoVendedorVisualiza, "CON_END_VENDEDOR_VIS");
            Map(x => x.ConsultaCreditoVendedorVisualiza, "CON_CRED_VENDEDOR_VIS");
            
            Map(x => x.ConsultaViabilidadeTempoValidade, "CON_VIA_VALIDADE");
            Map(x => x.ConsultaPendenciaFinanceiraTempoValidade, "CON_PEND_FIN_VALIDADE");
            Map(x => x.ConsultaPendenciaEnderecoTempoValidade, "CON_END_VALIDADE");
            Map(x => x.ConsultaCreditoTempoValidade, "CON_CRED_VALIDADE");

            #endregion

            Map(x => x.TokenOla, "TOKEN_OLA");
            Map(x => x.Telefone_Suporte, "TELEFONE_SUPORTE");
            Map(x => x.Email_Suporte, "EMAIL_SUPORTE");

            Map(x => x.OcultarAssinaturaTermoPremier, "HIDE_ASSIN_TERM_PREMIER");
            Map(x => x.PermiteDocVendaGaleriaPremier, "PERMITE_GALERIA_VENDA_PREMIER");
            Map(x => x.EnviarCheckListAuto, "ENVIAR_CHECKLIST_AUTO").Default("1");

            Map(x => x.Qtd_Dias_Bloqueio_Lead, "QTD_DIAS_BLOQUEIO_LEAD");
            Map(x => x.Bloqueia_Lead, "BLOQUEIA_LEAD");

            Map(x => x.ExibirInfoOSTimeline, "EXIBIR_OS_TIMELINE");
            Map(x => x.CampoinfoOStimeline, "INFO_OS_TIMELINE").CustomType<InfoOSTimelineEnum>();
            Map(x => x.CriarOsSemPlanoProduto, "CRIAR_OS_SEM_PLANO_PRODUTO");
            
            Map(x => x.EnviarTermoAdesaoAuto, "ENVIAR_TERMO_ADESAO_AUTO");
            Map(x => x.Definir_Requisitos_Aprovacao_Viabilidade, "DEFINIR_REQUISITOS_VIABILIDADE");
            Map(x => x.Requisito_Vinculo_Endereco, "REQUISITO_VINCULO_ENDERECO");
            Map(x => x.Requisito_Dentro_Raio, "REQUISITO_DENTRO_RAIO");
            Map(x => x.Requisito_Dentro_Area_Cobertura, "REQUISITO_DENTRO_AREA_COBERTURA");
            Map(x => x.AtribuirOSReincTec, "ATRIBUIR_OS_REINC_TEC");
            Map(x => x.UsaAlertaNotificacoes, "USA_ALERTA_NOTIFICACOES");
            Map(x => x.MinutosAtualizaNotificacoes, "MINUTOS_ATUALIZA_NOTIFICACOES");
            Map(x => x.EnderecoInstalacaoMesmoPrincipal, "END_INSTA_MESMO_PRINC");
            Map(x => x.CriticarBaixaPotenciaONU, "CRITICA_BAIXA_SINAL_ONU");
            Map(x => x.CriticarBaixaLoginRadiusOffline, "CRITICA_BAIXA_LOGIN_RADIUS_OFF");
            Map(x => x.ObrigaCaixaPorta, "OBRIGA_CAIXA_PORTA");
            Map(x => x.LimitePotenciaONUBaixa, "LIMITE_POTENCIA_SAIDA_BAIXA");
            Map(x => x.DesconsiderarDeslocamento, "DESCON_TEMPO_DESLOC");
            Map(x => x.PermitirAuditoriaOSImprod, "PERMITIR_AUD_OS_IMPROD");
            Map(x => x.PermitirMotivoReagendamento, "PERMITIR_MOTIVO_REAGENDAMENTO");
            Map(x => x.Exibir_Apenas_Caixas_Atendimento_Dentro_Raio, "EXIBIR_APENAS_CTO_DENTRO_RAIO");
            Map(x => x.LimiteDistanciaCaixaAtendimento, "LIMITE_DISTANCIA_RAIO_CTO");            
            Map(x => x.EncaixeOsProximoVencimento, "ENCAIXE_OS_PROX_VENC");
            Map(x => x.Uuid_Provedor_Ola, "UUID_PROVEDOR_OLA");
            Map(x => x.OcultarAgendamentoOsPremier, "HIDE_AGEND_OS_PREMIER");
            Map(x => x.Ambiente_Teste, "AMBIENTE_TESTE").CustomType<Boolean>();
            Map(x => x.Utilizar_Esquema_Cores_Padrao_Ola, "UTILIZAR_CORES_PADRAO_OLA").CustomType<Boolean>();
            Map(x => x.ValidarDocumentoLead, "VALIDAR_DOC_CADASTRO_LEAD").CustomType<Boolean>();
            Map(x => x.ConsumerKeyCNPJ, "KEY_CNPJ_SERPRO");
            Map(x => x.ConsumerSecretCNPJ, "SECRET_CNPJ_SERPRO");
            Map(x => x.ConsumerKeyCPF, "KEY_CPF_SERPRO");
            Map(x => x.ConsumerSecretCPF, "SECRET_CPF_SERPRO");
            Map(x => x.Tempo_Confirmacao_Ciencia_SOS, "TEMPO_CONFIRMACAO_CIENCIA_SOS");
            Map(x => x.Tempo_Atualizacao_Alerta_SOS, "TEMPO_ATUALIZACAO_ALERTA_SOS");
            #region Mobile
            Map(x => x.Max_Dias_Sem_Aprovacao, "MAX_DIAS_SEM_APROVACAO");            
            #endregion
        }
    }
}
