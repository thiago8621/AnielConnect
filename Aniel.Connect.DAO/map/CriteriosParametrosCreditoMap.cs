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
    public class CriteriosParametrosCreditoMap : ClassMapBase<CriteriosParamCredito>
    {
        public CriteriosParametrosCreditoMap() : base()
        {
            Table("TB_CRITERIO_PARAM_CREDITO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CRITERIO_PARAM_CREDITO");

            Map(u => u.Criterio, "CRITERIO");
            Map(u => u.Valor, "VALOR");
            Map(u => u.Operador, "OPERADOR");
            Map(u => u.Pontos, "PONTOS");
            Map(u => u.Eliminatorio, "ELIMINATORIO");
            References(u => u.Id_ParametrosConsultaCredito, "ID_PARAM_CONSULTA").ForeignKey("FK_PARAM_CONSULTA");



        }
    }
}
