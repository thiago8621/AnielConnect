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
    class Coluna_PainelServicoMap : ClassMapBase<Coluna_PainelServico>
    {
        public Coluna_PainelServicoMap() : base()
        {
            Table("TB_COLUNA_PAINEL_SERVICO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COLUNA_PAINEL_SERVICO");

            References(x => x.Modelo_PainelServico, "ID_MODELO").ForeignKey("FK_COLUNA_MOD_SERV");
            Map(x => x.Coluna, "COLUNA").CustomType<Coluna_PainelServicoEnum>();

        }
    }
}
