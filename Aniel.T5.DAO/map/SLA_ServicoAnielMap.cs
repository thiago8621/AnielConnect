using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class SLA_ServicoAnielMap : ClassMapBase<SLA_Servico>
    {
        public SLA_ServicoAnielMap() : base()
        {
            Table("TB_SLA_SERVICO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SLA_SERVICO");

            Map(x => x.CodCt, "CODCT");

            References(x => x.Tipo_Servico, "ID_TIPO_SERVICO").ForeignKey("FK_SLA_SERVICO_TIPO_SERV");

            References(x => x.Categoria_Cliente, "ID_CATEGORIA_CLIENTE").ForeignKey("FK_SLA_SERVICO_CAT");

            Map(x => x.Minutos, "MINUTOS");

            Map(x => x.Tipo, "TIPO_SLA").CustomType<Tipo_SLA>();                        
                       
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            References(x => x.Horario_Trabalho, "ID_HORARIO_CONNECT").ForeignKey("FK_SLA_SERVICO_HORARIO");
        }
    }
}
