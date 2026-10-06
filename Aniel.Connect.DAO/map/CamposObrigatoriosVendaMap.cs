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
    public class CamposObrigatoriosVendaMap : ClassMapBase<CamposObrigatoriosVenda>
    {
        public CamposObrigatoriosVendaMap() : base()
        {
            Table("TB_CAMPOS_OBRIG_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CAMPOS_OBRIG_VENDA");

            Map(x => x.Campo, "CAMPO").CustomType<CamposObrigatoriosVendaEnum>();

            Map(x => x.Bloqueado , "VISIVEL");
            Map(x => x.Obrigatorio, "OBRIGATORIO");
        }
    }
}
