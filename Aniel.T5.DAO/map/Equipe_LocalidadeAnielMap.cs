using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class Equipe_LocalidadeAnielMap : ClassMapBase<Equipe_LocalidadeAniel>
    {
        public Equipe_LocalidadeAnielMap() : base()
        {
            Table("TB_EQUIPE_LOCALIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_EQUIPE_LOCALIDADE");

            References(x => x.Equipe, "EQUIPE").ForeignKey("FK_EQUIPE_LOCALIDADE_EQUIPE");

            Map(x => x.Cod_Escritorio, "COD_ESCRITORIO").Index("IDX_EQUIPE_LOCALIDADE");
            Map(x => x.Cod_Area, "COD_AREA").Index("IDX_EQUIPE_LOCALIDADE");
            Map(x => x.Cod_Localidade, "COD_LOCALIDADE").Index("IDX_EQUIPE_LOCALIDADE");
            Map(x => x.Nivel, "NIVEL");
            Map(x => x.Todas_Localidades, "TODAS_LOCALIDADES");
        }
    }
}
