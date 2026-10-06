using Aniel.Connect.Model;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class RecebimentoIntegraMap : ClassMapBase<RecebimentoIntegra>
    {
        public RecebimentoIntegraMap() : base()
        {
            Table("TB_RECEB_INTEGRA");

            Map(x => x.CodEmpresa, "CODEMPRESA");
            Map(x => x.Dia, "DIA");
            Map(x => x.Pedido, "PEDIDO");
            Map(x => x.CodFornecedor, "COD_FORNECEDOR");
            Map(x => x.Hora, "HORA");
            Map(x => x.CodFilial, "CODFILIAL");
            Map(x => x.NotaFiscal, "NOTA_FISCAL");
            Map(x => x.Contrato, "CODCT");
            Map(x => x.Projeto, "PROJETO");
            Map(x => x.Usuario, "USUARIO");
        }
    }

    public class RecebimentoIntegraItemMap : ClassMapBase<RecebimentoIntegraItem>
    {
        public RecebimentoIntegraItemMap() : base()
        {
            Table("TB_RECEB_INTEGRA_ITEM");

            Map(x => x.CodContrato, "CODCT");
            Map(x => x.CodMaterial, "CODMAT");
            Map(x => x.CodComplementar, "CODCOMPLEMENTAR");
            Map(x => x.Dia, "DIA");
            Map(x => x.Hora, "HORA");
            Map(x => x.NotaFiscal, "NOTA_FISCAL");
            Map(x => x.Quantidade, "QTDE_RECEB");
            Map(x => x.ValorUnitario, "VALOR");
            Map(x => x.ValorTotal, "VALOR_TOTAL");
            Map(x => x.DataRecebimento, "DATA_RECEBIMENTO");
            Map(x => x.Id_Receb, "ID_RECEB");
            Map(x => x.Projeto, "PROJETO");
            Map(x => x.Usuario, "USUARIO");
        }
    }
}
