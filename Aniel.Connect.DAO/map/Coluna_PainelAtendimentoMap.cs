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
    public class Coluna_PainelAtendimentoMap : ClassMapBase<Coluna_PainelAtendimento>
    {
        public Coluna_PainelAtendimentoMap() : base()
        {
            Table("TB_COLUNA_PAINEL_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COLUNA_PAINEL_ATENDIMENTO");

            References(x => x.Modelo_PainelAtendimento, "ID_MODELO").ForeignKey("FK_COLUNA_MODELO");
            Map(x => x.Coluna, "COLUNA").CustomType<Coluna_PainelAtendimentoEnum>();
            
        }
    }
}
