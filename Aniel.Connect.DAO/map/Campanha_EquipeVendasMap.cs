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
    public class Campanha_EquipeVendasMap : ClassMapBase<Campanha_EquipeVendas>
    {
        public Campanha_EquipeVendasMap() : base()
        {
            Table("TB_CAMPANHA_EQUIPE_VENDAS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CAMPANHA_EQUIPE_VENDAS");

            Map(x => x.Id_Funcionario, "ID_FUNCIONARIO");
            Map(x => x.Id_Supervisor, "ID_SUPERVISOR");
            Map(x => x.Meta, "META");
            Map(x => x.Setor_Departamento, "SETOR_DEPARTAMENTO");
            References(x => x.Campanha, "ID_CAMPANHA").ForeignKey("FK_CAMPANHA");
            //References(x => x.Plano, "ID_PLANO").ForeignKey("FK_CAMPANHA_PP_PLANO");
            //References(x => x.Produto, "ID_PRODUTO").ForeignKey("FK_CAMPANHA_PP_PRODUTO");


        }
    }
}
