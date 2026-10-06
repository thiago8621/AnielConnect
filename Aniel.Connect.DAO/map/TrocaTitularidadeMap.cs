using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class TrocaTitularidadeMap : ClassMapBase<TrocaTitularidade>                               
    {
        public TrocaTitularidadeMap() : base()
        {
            Table("TB_TROCA_TITULARIDADE");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_TROCA_TITULARIDADE");

            Map(x => x.NumeroContrato, "NUMERO_CONTRATO");
            Map(x => x.DataAprovacao, "DATA_APROVACAO");
            Map(x => x.DataCancelamento, "DATA_CANCELAMENTO");
            Map(x => x.DataAssinaturaTermoAdesao, "DATA_ASSINATURA_TERMO");
            Map(x => x.TermoAdesaoAssinado, "TERMO_ADESAO_ASSINADO");
            Map(x => x.ChecklistAssinado, "CHECKLIST_ASSINADO");
            Map(x => x.DataAssinaturaChecklist, "DATA_ASSINATURA_CHECKLIST");
            Map(x => x.IdConsultaCredito, "ID_CONSULTA_CREDITO");
            Map(x => x.IdConsultaEndereco, "ID_CONSULTA_ENDERECO");
            Map(x => x.IdConsultaPendenciaFinanceira, "ID_CONSULTA_PEND_FINAN");
            Map(x => x.UsuarioAprovacao, "USUARIOAPROVACAO");
            Map(x => x.UsuarioCancelamento, "USUARIOCANCELAMENTO");

            Map(x => x.IpOrigemAceite, "IP_ORIGEM_ACEITE");
            Map(x => x.UserAgentTermoAceite, "USER_AGENT_TERMO");
            Map(x => x.CodigoHashTermoAceite, "HASH_TERMO_ACEITE");
            Map(x => x.AdicionadoAceiteDigital, "ADC_ACEITE_DIGITAL");
            Map(x => x.UrlAceiteDigital, "URL_ACEITE_CTT");
            Map(x => x.PortaOrigemAceite, "PORTA_ACEITE_DIGITAL");

            Map(x => x.StatusTrocaTitularidade, "STATUS_TROCA").CustomType<StatusTrocaTitularidade>();

            References(x => x.ClienteTitularAtual, "ID_CLIENTE_TITULAR_ATUAL").ForeignKey("FK_CLIENTE_ATUAL");
            References(x => x.ClienteTitularNovo, "ID_CLIENTE_TITULAR_NOVO").ForeignKey("FK_CLIENTE_NOVO");
            References(x => x.Etapa_Atual, "ID_ETAPA_TROCA").ForeignKey("FK_ETAPA_TROCA_ATUAL");
        }
    }
}
