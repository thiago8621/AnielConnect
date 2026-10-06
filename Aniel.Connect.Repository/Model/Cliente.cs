using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Repository.Model.Azza;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Repository.Model
{
    public class EnderecoClienteViewModel : IViewModel<Int64>
    {
        public Int64 Id
        {
            get
            {
                return Id_Endereco;
            }
            set
            {
                Id_Endereco = value;
            }
        }

        public Int64 Id_Endereco { get; set; }
        public Int64? Id_Atendimento { get; set; }
        public string Latitude { get; set; }
        public Int64? Id_Cliente { get; set; }
        public Int64? Id_ClientePlanoProduto { get; set; }
        public string Longitude { get; set; }
        public string CEP { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public Int32? Id_Tipo_Imovel { get; set; }
        public String Desc_Tipo_Imovel { get; set; }
        public Int64? Id_Tipo_Imovel_End_Cobranca { get; set; }
        public String Desc_Tipo_Imovel_End_Cobranca { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public Int64? Id_Pais { get; set; }
        public String Pais { get; set; }
        public string Logradouro { get; set; }
        [MaxLength(8, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public string Numero { get; set; }
        public Busca_Endereco? Busca_Endereco { get; set; }
        public Int64? Id_Bairro { get; set; }
        public Int64? Id_Endereco_Logradouro { get; set; }
        public Int64? Id_Condominio_Predio { get; set; }
        public Int64? Id_Bloco { get; set; }
        public Int64? Id_Apartamento { get; set; }
        public string Descricao_Condominio { get; set; }
        public string Bairro { get; set; }
        public Int64? Id_Cidade { get; set; }
        public string Cidade { get; set; }
        public long? Id_UF { get; set; }
        //public long? Campanha_Id_UF { get; set; }
        public String UF { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Complemento { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Ponto_Referencia { get; set; }

        public Tipo_EnderecoCliente? Tipo { get; set; }
        public Tipo_Endereco_Logradouro? TipoLogradouro { get; set; }
        public string DescTipoLogradouro { get; set; }
        public string Cod_Escritorio { get; set; }
        public string Cod_Area { get; set; }
        public string Cod_Localidade { get; set; }

        // Grupo Área, Área e Localidade definidos no cadastro do lead
        // utilizados para preencher o endereço de instalação caso o usuário
        // opte por usar o endereço do lead como endereço de instalação

        public string Cod_Escritorio_Prospect { get; set; }
        public string Cod_Area_Prospect { get; set; }
        public string Cod_Localidade_Prospect { get; set; }

        public String Localidade { get; set; }
        public Regime_Anatel? Regime { get; set; }

        public bool? EnderecoInstalacaoIgualPrincipal { get; set; }
        public bool? EnderecoInstalacaoIgualProspecto { get; set; }

        public Cliente_ProspectoDadosGeraisViewModel Model_View_Cliente_Prospecto { get; set; }
    }

    public class UsuarioSetRequest
    {
        public String tokenOperacao { get; set; }
        public List<UsuarioSetModels> usuarios { get; set; }
    }

    public class UsuarioSetModels
    {
        public Int64 idCliente { get; set; }

        public String cnpjProvedor { get; set; }

        public String cpf_cnpj { get; set; }

        public String nome { get; set; }

        public String status { get; set; }

        public String email { get; set; }
        public String telefone { get; set; }
        public string uuidProvedor { get; set; }
    }

    public class ClienteRequest : RequestIXC
    {

    }

    public class ClienteIXCResponse
    {
        public string id { get; set; }
        public string razao { get; set; }
        public string fantasia { get; set; }
        public string tipo_assinante { get; set; }
        public string cpf_pai { get; set; }
        public string cpf_mae { get; set; }
        public string id_tipo_cliente { get; set; }
        public string tipo_pessoa { get; set; }
        public string cnpj_cpf { get; set; }
        public string ie_identidade { get; set; }
        public string rg_orgao_emissor { get; set; }
        public string inscricao_municipal { get; set; }
        public string email { get; set; }
        public string ativo { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string telefone_celular { get; set; }
        public string telefone_comercial { get; set; }
        public string fone { get; set; }
        public string apartamento { get; set; }
        public string bloco { get; set; }
        public string referencia { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string senha { get; set; }
        public string nome_pai { get; set; }
        public string nome_mae { get; set; }
        public string data_nascimento { get; set; }
        public string nacionalidade { get; set; }
        public string estado_civil { get; set; }
        public string sexo { get; set; }
        public string cep_cob { get; set; }
        public string endereco_cob { get; set; }
        public string numero_cob { get; set; }
        public string bairro_cob { get; set; }
        public string cidade_cob { get; set; }
        public string complemento_cob { get; set; }
        public string uf_cob { get; set; }
        public string tipo_cliente_scm { get; set; }
        public string pais { get; set; }
        public string contribuinte_icms { get; set; }
        public string cidade_naturalidade { get; set; }
        public string estado_nascimento { get; set; }
        public string profissao { get; set; }
        public string isuf { get; set; }
        public string filial_id { get; set; }
        public string filtra_filial { get; set; }
        public string idx { get; set; }
        public string data_cadastro { get; set; }
        public string ativo_serasa { get; set; }
        public string convert_cliente_forn { get; set; }
        public string atualizar_cadastro_galaxPay { get; set; }
        public string grau_satisfacao { get; set; }
        public string id_condominio { get; set; }
        public string cif { get; set; }
        public string moradia { get; set; }
        public string tipo_localidade { get; set; }
        public string referencia_cob { get; set; }
        public string ramal { get; set; }
        public string id_operadora_celular { get; set; }
        public string whatsapp { get; set; }
        public string email_opa { get; set; }
        public string contato { get; set; }
        public string website { get; set; }
        public string skype { get; set; }
        public string facebook { get; set; }
        public string hotsite_email { get; set; }
        public string acesso_automatico_central { get; set; }
        public string alterar_senha_primeiro_acesso { get; set; }
        public string senha_hotsite_md5 { get; set; }
        public string hotsite_acesso { get; set; }
        public string crm { get; set; }
        public string id_candato_tipo { get; set; }
        public string id_campanha { get; set; }
        public string id_concorrente { get; set; }
        public string id_perfil { get; set; }
        public string responsavel { get; set; }
        public string indicado_por { get; set; }
        public string cadastrado_via_viabilidade { get; set; }
        public string status_prospeccao { get; set; }
        public string crm_data_novo { get; set; }
        public string crm_data_sondagem { get; set; }
        public string crm_data_apresentando { get; set; }
        public string crm_data_negociando { get; set; }
        public string crm_data_vencemos { get; set; }
        public string crm_data_perdemos { get; set; }
        public string crm_data_abortamos { get; set; }
        public string crm_data_sem_porta_disponivel { get; set; }
        public string crm_data_sem_viabilidade { get; set; }
        public string pipe_id_organizacao { get; set; }
        public string foto_cartao { get; set; }
        public string participa_cobranca { get; set; }
        public string num_dias_cob { get; set; }
        public string participa_pre_cobranca { get; set; }
        public string cob_envia_email { get; set; }
        public string cob_envia_sms { get; set; }
        public string id_conta { get; set; }
        public string cond_pagamento { get; set; }
        public string id_vendedor { get; set; }
        public string tabela_preco { get; set; }
        public string deb_automatico { get; set; }
        public string deb_agencia { get; set; }
        public string deb_conta { get; set; }
        public string codigo_operacao { get; set; }
        public string tipo_pessoa_titular_conta { get; set; }
        public string cnpj_cpf_titular_conta { get; set; }
        public string ultima_atualizacao { get; set; }
        public string identidade_pai { get; set; }
        public string nascimento_pai { get; set; }
        public string identidade_mae { get; set; }
        public string nascimento_mae { get; set; }
        public string quantidade_dependentes { get; set; }
        public string nome_conjuge { get; set; }
        public string fone_conjuge { get; set; }
        public string cpf_conjuge { get; set; }
        public string rg_conjuge { get; set; }
        public string data_nascimento_conjuge { get; set; }
        public string nome_contador { get; set; }
        public string telefone_contador { get; set; }
        public string orgao_publico { get; set; }
        public string im { get; set; }
        public string nome_representante_1 { get; set; }
        public string cpf_representante_1 { get; set; }
        public string identidade_representante_1 { get; set; }
        public string nome_representante_2 { get; set; }
        public string cpf_representante_2 { get; set; }
        public string identidade_representante_2 { get; set; }
        public string emp_empresa { get; set; }
        public string emp_cnpj { get; set; }
        public string emp_cep { get; set; }
        public string emp_endereco { get; set; }
        public string emp_cidade { get; set; }
        public string emp_fone { get; set; }
        public string emp_cargo { get; set; }
        public string emp_remuneracao { get; set; }
        public string emp_data_admissao { get; set; }
        public string iss_classificacao_padrao { get; set; }
        public string pis_retem { get; set; }
        public string cofins_retem { get; set; }
        public string csll_retem { get; set; }
        public string irrf_retem { get; set; }
        public string desconto_irrf_valor_inferior { get; set; }
        public string cli_desconta_iss_retido_total { get; set; }
        public string dica_imposto_retido_cliente { get; set; }
        public string ref_com_empresa1 { get; set; }
        public string ref_com_fone1 { get; set; }
        public string ref_com_empresa2 { get; set; }
        public string ref_com_fone2 { get; set; }
        public string ref_pes_nome1 { get; set; }
        public string ref_pes_fone1 { get; set; }
        public string ref_pes_nome2 { get; set; }
        public string ref_pes_fone2 { get; set; }
        public string obs { get; set; }
        public string alerta { get; set; }
        public bool flagCEP { get; set; }
    }

    public class ClienteContratoIXCResponse
    {
        public string id_instalador { get; set; }
        public string indicacao_contrato_id { get; set; }
        public string id_indexador_reajuste { get; set; }
        public string data_desistencia { get; set; }
        public string motivo_desistencia { get; set; }
        public string obs_desistencia { get; set; }
        public string obs_contrato { get; set; }
        public string alerta_contrato { get; set; }
        public string ids_contratos_recorrencia { get; set; }
        public string id_responsavel_desistencia { get; set; }
        public string id_responsavel_cancelamento { get; set; }
        public string id_responsavel_negativacao { get; set; }
        public string id { get; set; }
        public string id_filial { get; set; }
        public string status { get; set; }
        public string status_internet { get; set; }
        public string id_cliente { get; set; }
        public string data_assinatura { get; set; }
        public string data_ativacao { get; set; }
        public string data { get; set; }
        public string data_renovacao { get; set; }
        public string bloqueio_automatico { get; set; }
        public string imp_carteira { get; set; }
        public string data_expiracao { get; set; }
        public string isentar_contrato { get; set; }
        public string pago_ate_data { get; set; }
        public string id_vd_contrato { get; set; }
        public string contrato { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string comissao { get; set; }
        public string bairro { get; set; }
        public string tipo { get; set; }
        public string descricao_aux_plano_venda { get; set; }
        public string aviso_atraso { get; set; }
        public string id_tipo_contrato { get; set; }
        public string id_carteira_cobranca { get; set; }
        public string obs { get; set; }
        public string id_modelo { get; set; }
        public string status_velocidade { get; set; }
        public string id_vendedor { get; set; }
        public string cc_previsao { get; set; }
        public string nao_avisar_ate { get; set; }
        public string nao_bloquear_ate { get; set; }
        public string id_tipo_documento { get; set; }
        public string tipo_doc_opc { get; set; }
        public string desconto_fidelidade { get; set; }
        public string tipo_doc_opc2 { get; set; }
        public string tipo_doc_opc3 { get; set; }
        public string taxa_improdutiva { get; set; }
        public string tipo_doc_opc4 { get; set; }
        public string desbloqueio_confianca { get; set; }
        public string data_negativacao { get; set; }
        public string data_acesso_desativado { get; set; }
        public string motivo_cancelamento { get; set; }
        public string data_cancelamento { get; set; }
        public string obs_cancelamento { get; set; }
        public string id_vendedor_ativ { get; set; }
        public string fidelidade { get; set; }
        public string tipo_cobranca { get; set; }
        public string desbloqueio_confianca_ativo { get; set; }
        public string id_responsavel { get; set; }
        public string taxa_instalacao { get; set; }
        public string protocolo_negativacao { get; set; }
        public string dt_ult_bloq_auto { get; set; }
        public string dt_ult_bloq_manual { get; set; }
        public string dt_ult_des_bloq_conf { get; set; }
        public string dt_ult_ativacao { get; set; }
        public string avalista_1 { get; set; }
        public string dt_ult_finan_atraso { get; set; }
        public string avalista_2 { get; set; }
        public string dt_utl_negativacao { get; set; }
        public string data_cadastro_sistema { get; set; }
        public string ultima_atualizacao { get; set; }
        public string complemento { get; set; }
        public string cidade { get; set; }
        public string imp_importacao { get; set; }
        public string renovacao_automatica { get; set; }
        public string imp_rede { get; set; }
        public string imp_bkp { get; set; }
        public string motivo_inclusao { get; set; }
        public string imp_treinamento { get; set; }
        public string liberacao_bloqueio_manual { get; set; }
        public string imp_status { get; set; }
        public string contrato_suspenso { get; set; }
        public string imp_obs { get; set; }
        public string imp_realizado { get; set; }
        public string imp_motivo { get; set; }
        public string imp_inicial { get; set; }
        public string imp_final { get; set; }
        public string ativacao_numero_parcelas { get; set; }
        public string ativacao_vencimentos { get; set; }
        public string ativacao_valor_parcela { get; set; }
        public string id_tipo_doc_ativ { get; set; }
        public string id_produto_ativ { get; set; }
        public string id_cond_pag_ativ { get; set; }
        public string endereco_padrao_cliente { get; set; }
        public string cep { get; set; }
        public string referencia { get; set; }
        public string id_condominio { get; set; }
        public string nf_info_adicionais { get; set; }
        public string assinatura_digital { get; set; }
        public string tipo_produtos_plano { get; set; }
        public string bloco { get; set; }
        public string apartamento { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string num_parcelas_atraso { get; set; }
        public string dt_ult_desiste { get; set; }
        public string id_contrato_principal { get; set; }
        public string gerar_finan_assin_digital_contrato { get; set; }
        public string credit_card_recorrente_token { get; set; }
        public string credit_card_recorrente_bandeira_cartao { get; set; }
        public string id_motivo_negativacao { get; set; }
        public string id_motivo_inclusao { get; set; }
        public string obs_negativacao { get; set; }
        public string restricao_auto_desbloqueio { get; set; }
        public string motivo_restricao_auto_desbloq { get; set; }
        public string nao_susp_parc_ate { get; set; }
        public string liberacao_suspensao_parcial { get; set; }
        public string utilizando_auto_libera_susp_parc { get; set; }
        public string restricao_auto_libera_susp_parcial { get; set; }
        public string motivo_restri_auto_libera_parc { get; set; }
        public string data_inicial_suspensao { get; set; }
        public string data_final_suspensao { get; set; }
        public string data_retomada_contrato { get; set; }
        public string dt_ult_liberacao_susp_parc { get; set; }
        public string base_geracao_tipo_doc { get; set; }
        public string integracao_assinatura_digital { get; set; }
        public string url_assinatura_digital { get; set; }
        public string token_assinatura_digital { get; set; }
        public string testemunha_assinatura_digital { get; set; }
        public string document_photo { get; set; }
        public string selfie_photo { get; set; }
        public string longitude_novo { get; set; }
        public string tipo_localidade { get; set; }
        public string latitude_novo { get; set; }
        public string referencia_novo { get; set; }
        public string complemento_novo { get; set; }
        public string cidade_novo { get; set; }
        public string bairro_novo { get; set; }
        public string numero_novo { get; set; }
        public string endereco_novo { get; set; }
        public string cep_novo { get; set; }
        public string apartamento_novo { get; set; }
        public string bloco_novo { get; set; }
        public string condominio_novo { get; set; }
        public string endereco_padrao_alert { get; set; }
        public string motivo_adicional { get; set; }
        public string concorrente_mot_adicional { get; set; }
        public string dica_motivo_adicional_cliente_contrato { get; set; }
        public string venc_personalizado { get; set; }
        public string com_entrada { get; set; }
        public string dia_fixo_vencimento { get; set; }
        public string tipo_condicao_pag { get; set; }
        public string dica_data_expiracao { get; set; }
        public string btn_nf_ativacao { get; set; }
        public string moeda { get; set; }
        public bool flagCEP { get; set; }
    }

    public class BoletoIXCCliente
    {
        public string id_remessa { get; set; }
        public string gateway_link { get; set; }
        public string nn_boleto { get; set; }
        public string boleto { get; set; }
        public string data_inicial { get; set; }
        public string data_final { get; set; }
        public string id { get; set; }
        public string liberado { get; set; }
        public string filial_id { get; set; }
        public string status { get; set; }
        public string data_emissao { get; set; }
        public string data_vencimento { get; set; }
        public string valor { get; set; }
        public string obs { get; set; }
        public string valor_recebido { get; set; }
        public string valor_aberto { get; set; }
        public string id_cliente { get; set; }
        public string pagamento_valor { get; set; }
        public string pagamento_data { get; set; }
        public string id_carteira_cobranca { get; set; }
        public string credito_data { get; set; }
        public string baixa_data { get; set; }
        public string numero_parcela_recorrente { get; set; }
        public string documento { get; set; }
        public string id_saida { get; set; }
        public string tipo_recebimento { get; set; }
        public string tipo_renegociacao { get; set; }
        public string valor_cancelado { get; set; }
        public string data_cancelamento { get; set; }
        public string id_mot_cancelamento { get; set; }
        public string id_renegociacao { get; set; }
        public string id_cobranca { get; set; }
        public string previsao { get; set; }
        public string id_renegociacao_novo { get; set; }
        public string libera_periodo { get; set; }
        public string impresso { get; set; }
        public string forma_recebimento { get; set; }
        public string arquivo_remessa_baixado { get; set; }
        public string nparcela { get; set; }
        public string id_nota_gerada_opc4 { get; set; }
        public string status_cobranca { get; set; }
        public string tipo_cobranca { get; set; }
        public string id_contrato_principal { get; set; }
        public string id_contrato_avulso { get; set; }
        public string id_contrato { get; set; }
        public string id_nota_gerada { get; set; }
        public string id_im_imovel { get; set; }
        public string parcela_proporcional { get; set; }
        public string linha_digitavel { get; set; }
        public string duplicata { get; set; }
        public string tipo_pagamento_cartao { get; set; }
        public string id_sip { get; set; }
        public string titulo_protestado { get; set; }
        public string desconto_condicional_valor { get; set; }
        public string gerencianet_token { get; set; }
        public string validade_desconto_condicional { get; set; }
        public string id_conta { get; set; }
        public string motivo_alteracao { get; set; }
        public string id_remessa_alteracao { get; set; }
        public string cancelamento_id_operador { get; set; }
        public string baixa_id_operador { get; set; }
        public string titulo_importado { get; set; }
        public string origem_importacao { get; set; }
        public string ultima_atualizacao { get; set; }
        public string aguardando_confirmacao_pagamento { get; set; }
        public string parcelado_cartao { get; set; }
        public string pix_txid { get; set; }
        public string id_nota_gerada_opc2 { get; set; }
        public string id_nota_gerada_opc3 { get; set; }
        public string recebido_via_pix { get; set; }
        public string id_lote_geracao_financeiro_fatura { get; set; }
    }

    public class TipoClienteIXCResponse
    {
        public string id { get; set; }
        public string tipo_cliente { get; set; }
        public string tipo_cliente_scm { get; set; }
        public string contribuinte_icms { get; set; }
        public string tipo_assinante { get; set; }
    }
}
