using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class DidMap : ClassMapBase<Did>
    {
        public DidMap() : base()
        {
            Table("TB_DID");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DID");

            Map(x => x.Numero , "NUMERO").Length(20);
            Map(x => x.Valor, "VALOR");
            Map(x => x.StatusDid, "STATUS_DID").CustomType<StatusDid>();
            Map(x => x.DiaLiberado, "DIA_LIBERADO");
            Map(x => x.UF, "UF");
            Map(x => x.ChamadaCobrar, "CHAMADA_COBRAR");
            Map(x => x.IdVenda, "ID_VENDA");
            Map(x => x.MotivoIndisponivel, "MOTIVO_INDISPONIVEL");
            Map(x => x.TipoDiscagem, "TIPO_DISCAGEM").CustomType<TipoDiscagem>();
            Map(x => x.CodigoAuxiliar, "CODIGO_AUXILIAR");
            Map(x => x.Reservado_Ddr, "RESERVADO_DDR");


            References(x => x.Cidade, "ID_CIDADE").ForeignKey("FK_DID_CIDADE");
            References(x => x.RotaTelefonia, "ID_ROTA_TELEFONIA").ForeignKey("FK_DID_ROTA_TELEFONIA");
            References(x => x.Pais, "ID_PAIS").ForeignKey("FK_DID_PAIS");
            References(x => x.Circuito, "ID_CIRCUITO_TELEFONIA").ForeignKey("FK_DID_CIRCUITO");
        }
    }
}
