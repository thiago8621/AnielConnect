using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Faturamento_ClienteMap : ClassMapBase<Faturamento_Cliente>
    {
        public Faturamento_ClienteMap()
        {
            Table("TB_FATURAMENTO_CLIENTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FATURAMENTO_CLIENTE");

            References(x => x.Cliente, "ID_CLIENTE").ForeignKey("FK_FAT_CLIENTE_CLIENTE");
            References(x => x.Faturamento_Forma_Pagto, "ID_FATURAMENTO_FORMA").ForeignKey("FK_FAT_CLIENTE_FATFORMA");

            References(x => x.Forma_Pagamento_Cliente, "ID_FORMA_PAGAMENTO_CLIENTE").ForeignKey("FK_FAT_CLI_FORMAPAGTOCLI");

            Map(x => x.Vencimento, "DATA_VENCIMENTO");
            Map(x => x.Data_Pagamento, "DATA_PAGAMENTO");
            Map(x => x.Detalhes, "INSTRUCOES").Length(2000);
            Map(x => x.Alterado, "ALTERADO");
            Map(x => x.Valor_Total, "VALOR_TOTAL");
            //Map(x => x.Descontos_Total_Antecipacao, "DESCONTOS_TOT_ANTECIPACAO");
            Map(x => x.Descontos_Total_Plano, "DESCONTOS_TOT_PLANO");
            Map(x => x.Descontos_Cliente_Valor, "DESCONTOS_CLIENTE_VALOR");
            Map(x => x.Descontos_Cliente_Perc, "DESCONTOS_CLIENTE_PERC");
            Map(x => x.Descontos_Credito, "DESCONTOS_CREDITO");
            Map(x => x.Multa_Carencia, "MULTA_CARENCIA");
            Map(x => x.Multa, "MULTA");
            Map(x => x.Juros, "JUROS");
            Map(x => x.Valor_Final, "VALOR_FINAL");
            //Map(x => x.Valor_Final_Antecipado, "VALOR_FINAL_ANTECIPADO");
            Map(x => x.Valor_Pago, "VALOR_PAGO");
            Map(x => x.Valor_Negociacao, "VALOR_NEGOCIACAO");
            Map(p => p.FaturaGerada, "FATURA_GERADA").CustomSqlType("BLOB SUB_TYPE TEXT");
            Map(x => x.Valor_Instalacao, "VALOR_INSTALACAO");
            Map(x => x.Status, "STATUS").CustomType<Status_Faturamento_Cliente>();

            References(x => x.Conta_Financeira, "ID_CONTA_FINANCEIRA").ForeignKey("FK_FAT_CLIENTE_CONTAFIN");

            HasMany(x => x.Planos).KeyColumn("ID_FATURAMENTO_CLIENTE").Cascade.All().Inverse();
        }
    }
}
