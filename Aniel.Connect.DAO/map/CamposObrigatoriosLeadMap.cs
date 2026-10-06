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
    public class CamposObrigatoriosLeadMap : ClassMapBase<CamposObrigatoriosLead>
    {
        public CamposObrigatoriosLeadMap() : base()
        {
            Table("TB_CAMPOS_OBRIG_LEAD");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CAMPOS_OBRIG_LEAD");

            Map(x => x.Campo, "CAMPO").CustomType<CamposObrigatoriosLeadEnum>();

            Map(x => x.Bloqueado , "VISIVEL");
            Map(x => x.Obrigatorio, "OBRIGATORIO");
        }
    }
}
