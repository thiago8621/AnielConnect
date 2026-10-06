using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class TipoServicoMap : ClassMapBase<TipoServico_Cobranca>
    {
        public TipoServicoMap() : base()
        {
            Table("TB_TIPO_SERVICO_COBRANCA");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPOSERVICO");
            Map(m => m.Segmento, "SEGMENTO");
            Map(m => m.Tipo_Servico, "TIPO_SERVICO");
            Map(m => m.Sub_Tipo_Servico, "SUB_TIPO_SERVICO");
            Map(m => m.TodasSubTipoServico, "TODAS_SUB_TIPO_SERVICO");
            Map(m => m.TodasCod_Encerramento, "TODAS_COD_ENCERRAMENTO");
            Map(m => m.Cod_Encerramento, "COD_ENCERRAMENTO");
            References(m => m.Cobranca_Servico, "ID_COBRANCA_SERVICO").ForeignKey("FK_TIPO_SERVICO_COBRANCA");


        }
    }
}
