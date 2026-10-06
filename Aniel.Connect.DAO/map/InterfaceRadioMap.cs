using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class InterfaceRadioMap : ClassMapBase<InterfaceRadio>
    {
        public InterfaceRadioMap() : base()
        {
            Table("TB_INTERFACE_RADIO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTERFACE_RADIO");
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            References(x => x.Ponto_Acesso_Radio, "ID_PONTO_ACESSO_RADIO").NotFound.Ignore().Fetch.Join().ForeignKey("FK_PONTO_ACESSO_INTERFACE");
            Map(x => x.Codigo_Auxiliar, "CODIGO_AUXILIAR");
           
        }
    }
}
