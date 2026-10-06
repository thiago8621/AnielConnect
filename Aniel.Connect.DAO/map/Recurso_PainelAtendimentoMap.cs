using Aniel.Connect.Model.enumerado;
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
    public class Recurso_PainelAtendimentoMap : ClassMapBase<Recurso_PainelAtendimento>
    {
        public Recurso_PainelAtendimentoMap() : base()
        {
            Table("TB_RECURSO_PAINEL_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_RECURSO_PAINEL_ATENDIMENTO");

            Map(x => x.Valor, "VALOR");
            References(x => x.Modelo_PainelAtendimento, "ID_MODELO").ForeignKey("FK_RECURSO_MODELO"); ;
            Map(x => x.Recurso, "RECURSO").CustomType<Recurso_PainelAtendimentoEnum>();
            
        }
    }
}
