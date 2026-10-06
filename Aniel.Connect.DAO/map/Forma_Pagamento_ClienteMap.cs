using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    class Forma_Pagamento_ClienteMap : ClassMapBase<Forma_Pagamento_Cliente>
    {
        public Forma_Pagamento_ClienteMap() : base()
        {
            Table("TB_FORMA_PAGAMENTO_CLIENTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FORMA_PAGAMENTO_CLIENTE");

            Map(x => x.Conta_Corrente, "CONTA_CORRENTE").Length(25);
            Map(x => x.Agencia, "AGENCIA").Length(8);
            Map(x => x.Digito, "DIGITO").Length(1);
            Map(x => x.Alterado, "ALTERADO");

            References(x => x.Vencimento, "ID_VENCIMENTO").ForeignKey("FK_FORMPAGTO_CLIENTE_VENC");
            References(x => x.Banco, "ID_BANCO").ForeignKey("FK_FORMPAGTO_CLIENTE_BANCO");
            References(x => x.Forma_Pagamento, "ID_FORMA_PAGAMENTO").ForeignKey("FK_FORMPAGTO_CLIENTE_FORM");
            References(x => x.Cliente, "ID_CLIENTE").NotFound.Ignore().Fetch.Join().ForeignKey("FK_FORMPAGTOCLI_CLIENTE");
            //References(x => x.Cliente_PlanoProduto, "ID_CLIENTE_PLANOPRODUTO").ForeignKey("FK_FORMPAGCLI_CLIPLANOPROD");

            Map(x => x.Forma_Envio, "FORMA_ENVIO").CustomType<Cliente_FormaEnvio>();

            Map(x => x.Dia_Vencimento, "DIA_VENCIMENTO");

            References(x => x.Endereco_Cobranca, "ID_ENDERECO_COBRANCA").Cascade.SaveUpdate().ForeignKey("FK_FORMAPAGTOCLI_END_COBR");
            Map(x => x.End_Cobranca_Igual_Principal, "END_COBRANCA_IGUAL_PRINCIPAL");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.StatusValidoCartao, "STATUS_VALIDO_CARTAO").CustomType<StatusValidacaoCartao>();            
        }
    }
}
