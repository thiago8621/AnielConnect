using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System.Linq;

namespace Aniel.Connect.DAO.map
{
    public class Cobranca_FaturamentoMap : ClassMapBase<CobrancaFaturamento>
    {
        public Cobranca_FaturamentoMap() : base()
        {
            Table("TB_COBRANCA_FATURAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COBRANCA_FATURAMENTO");

            Map(x => x.Id_Boleto, "ID_BOLETO");
            Map(x => x.desconto, "DESCONTO");
            Map(x => x.Nosso_Numero, "NOSSO_NUMERO");
            Map(x => x.juros, "JUROS");
            Map(x => x.multa, "MULTA");
            Map(x => x.valor, "VALOR");
            Map(x => x.vencimento, "DATA_VENCIMENTO");
            Map(x => x.DataHora_Registro, "DATAHORA_REGISTRO");
            Map(x => x.IdGateWay, "ID_GATEWAY");

            Map(x => x.instrucoesCaixa, "INSTRUCOES_CAIXA");
            Map(x => x.linhaDigitavel, "LINHA_DIGITAVEL");
            Map(x => x.Link_Boleto, "LINK");
            Map(x => x.html, "HTML");
            Map(x => x.pdf, "PDF").LazyLoad();
            Map(x => x.recebedor, "ID_RECEBEDOR");
            Map(x => x.pagador, "ID_PAGADOR");
            Map(x => x.Status, "STATUS").CustomType<Cobranca_StatusPagamento>();
            References(x => x.FaturamentoCliente, "ID_FATURAMENTO_CLIENTE").ForeignKey("FK_COBRANCA_FAT_FATCLI");
        }
    }
}
