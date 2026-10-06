using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class VendaMap : ClassMapBase<Venda>
    {
        public VendaMap() : base()
        {
            Table("TB_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_VENDA");

            References(x => x.Campanha, "ID_CAMPANHA").ForeignKey("FK_VENDA_CAMPANHA");
            
            References(x => x.EnderecoInstalacao, "ID_ENDERECO_INSTALACAO").Cascade.SaveUpdate().ForeignKey("FK_VENDA_END_INST");
            References(x => x.Vendedor, "ID_VENDEDOR").ForeignKey("FK_VENDA_VENDEDOR");
            References(x => x.Supervisor, "ID_SUPERVISOR").ForeignKey("FK_VENDA_SUPERVISOR");
            References(x => x.Etapa_Atual, "ID_ETAPA_VENDA").ForeignKey("FK_ETAPA_VENDA_ATUAL");

            Map(x => x.Id_Cliente, "ID_CLIENTE");

            Map(x => x.Data_Encerramento, "DATA_ENCERRAMENTO");
            Map(x => x.Data_Venda, "DATA_VENDA");
            Map(x => x.Data_Alteracao_Status, "DATA_ALTERACAO_STATUS");
            Map(x => x.Data_Ultimo_Contato, "DATA_ULTIMO_CONTATO");
            Map(x => x.Justificativa_Avaliacao_Comercial, "JUSTIFICATIVA_AVALIACAO");

            Map(x => x.Id_Meta, "ID_META");
            Map(x => x.Id_Faixa, "ID_FAIXA");
            Map(x => x.Id_Comissionamento, "ID_COMISSIONAMENTO");
            Map(x => x.Forma_Envio, "FORMA_ENVIO").CustomType<Cliente_FormaEnvio>();            

            Map(x => x.Status, "STATUS").Index("IDX_VENDA_STATUS").CustomType<Status_Venda>();
            Map(x => x.StatusVenda, "STATUSVENDA").Length(100);
            Map(x => x.Id_Forma_Pagamento_Cliente_Preferida, "ID_FORMAPAGCLI_PREFERIDA"); 
            Map(x => x.IdFormaPagamento, "ID_FORMA_PAGTO"); 

            Map(x => x.IpOrigemAceite, "IP_ORIGEM_ACEITE"); 
            Map(x => x.UserAgentTermoAceite, "USER_AGENT_TERMO"); 
            Map(x => x.CodigoHashTermoAceite, "HASH_TERMO_ACEITE"); 
            Map(x => x.AdicionadoAceiteDigital, "ADC_ACEITE_DIGITAL"); 
            Map(x => x.UrlAceiteDigital, "URI_ACEITE_DIGITAL"); 
            Map(x => x.PortaOrigemAceite, "PORTA_ACEITE_DIGITAL");
            Map(x => x.ScoreConsultaCredito, "SCORE_CONSULTA_CREDITO");

            References(x => x.Motivo_Desistencia_Venda, "ID_MOTIVO_DESISTENCIA_VENDA").ForeignKey("FK_MOTIVO_DESISTENCIA_VENDA");

            Map(x => x.IdLoginIntegracao, "ID_LOGIN_INTEGRACAO");

            Map(x => x.Termo_Adesao_Assinado, "TERMO_ADESAO_ASSINADO");
            Map(x => x.DataHoraAssinaturaTermo, "DATA_TERMO_ASSINADO");

            Map(x => x.IdAtendimentoIntegracao, "ID_ATENDIMENTO_INTEGRACAO");

            Map(x => x.Consulta_Viabilidade_Realizada, "CONSULTA_VIABILIDADE_REALIZADA");
            Map(x => x.Consulta_Credito_Realizada, "CONSULTA_CREDITO_REALIZADA");
            Map(x => x.Consulta_Endereco_Realizada, "CONSULTA_ENDERECO_REALIZADA");
            Map(x => x.Consulta_Financeiro_Realizada, "CONSULTA_FINANCEIRO_REALIZADA");
            Map(x => x.IdAtendimento, "ID_ATENDIMENTO");
            Map(x => x.StatusAuditoria, "STATUS_AUDITORIA").CustomType<StatusAuditoria>();
            Map(x => x.Status_Liberacao, "STATUS_LIBERACAO").CustomType<Status_Venda>();
            Map(x => x.StatusCredito, "STATUS_CONSULTA_CREDITO").CustomType<AprovacaoCriterioParametroCredito>(); 

            Map(x => x.Tipo_Integracao_Assinatura, "TIPO_INTEGRACAO_ASSINATURA").CustomType<Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum>();
            Map(x => x.Url_Assinar_Documento, "URL_ASSINAR_DOC_INTEGRACAO").Length(250);
            Map(x => x.Token_Documento_Assinatura, "TOKEN_DOC_ASSINATURA").Length(250);
            Map(x => x.Url_Documento_Assinado, "URL_DOC_ASSINADO_INTEGRACAO").Length(250);

            Map(x => x.End_Instalacao_Igual_Principal, "END_INSTALACAO_IGUAL_PRINCIPAL");
            Map(x => x.End_Instalacao_Igual_Prospecto, "END_INSTALACAO_IGUAL_PROSPECTO");
            
            Map(x => x.IdConsultaViabilidade, "ID_CON_VIABILIDADE");
            Map(x => x.IdConsultaPendenciaEndereco, "ID_CON_PEND_ENDERECO");
            Map(x => x.IdConsultaPendenciaFinanceira, "ID_CON_PEND_FINANCEIRA");
            Map(x => x.IdConsultaCredito, "ID_CON_CREDITO");

            Map(x => x.MotivoAprovManualViabilidade, "MOTIVO_APROV_VIABILIDADE");
            Map(x => x.MotivoAprovManualESC, "MOTIVO_APROV_VIABILIDADE_ID");

            Map(x => x.InformacoesComplementares, "INFORMACOES_COMPLEMENTARES").Length(5000);

            Map(x => x.End_CadastroCondominioPrincipal, "END_PRINCIPAL_DENTRO_COND");
            Map(x => x.End_CadastroCondominioInstalacao, "END_INSTALACAO_DENTRO_COND");
            Map(x => x.End_CadastroCondominioCobranca, "END_COBRANCA_DENTRO_COND");

            Map(x => x.Porta_Reservada, "PORTA_RESERVADA").CustomType<Sinapse.Model.enumerados.Padrao>();
            Map(x => x.EtapaIntegracao, "ETAPA_INTEGRACAO");

            References(x => x.Workflow_Vendas, "ID_WORKFLOW_VENDAS").ForeignKey("FK_VENDA_WORKFLOW_VENDA");
            References(x => x.Workflow_VendasAtividade, "ID_WORKFLOW_VENDAS_ATIVIDADE").ForeignKey("FK_VENDA_ATIV_WORKFLOW");
            References(x => x.Departamento, "ID_DEPARTAMENTO").ForeignKey("FK_DEPARTAMENTO");
            References(x => x.Id_Responsavel, "ID_RESPONSAVEL").ForeignKey("FK_RESPONSAVEL");
            References(x => x.Id_Setor_Departamento, "ID_SETOR_DEPARTAMENTO").ForeignKey("FK_SETOR_RESPONSAVEL");

            References(x => x.PerfilVendedor, "ID_PERFIL_VENDEDOR").ForeignKey("FK_PERFIL_VENDEDOR");
            References(x => x.FormaPagamentoCliente, "ID_FORMAPGTO_CLIENTE").ForeignKey("FK_FORMAPGTO_CLIENTE");
            
            Map(x => x.Origem_Venda, "ORIGEM_VENDA").Length(50);
            Map(x => x.Mudanca_Plano, "MUDANCA_PLANO");
            Map(x => x.Tipo_Alteracao, "TIPO_ALTERACAO").CustomType<TipoAlteracaoPlanoContratoEnum>();

            HasMany(x => x.AnexosContrato).KeyColumn("ID_VENDA").Cascade.All().Inverse();
        }
    }
}
