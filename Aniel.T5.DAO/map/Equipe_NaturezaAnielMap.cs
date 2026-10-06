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
    public class Equipe_NaturezaAnielMap : ClassMapBase<Equipe_NaturezaAniel>
    {
        public Equipe_NaturezaAnielMap() : base()
        {
            Table("TB_EQUIPE_NATUREZA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_EQUIPE_NATUREZA");

            References(x => x.Equipe, "EQUIPE").ForeignKey("FK_EQUIPE_NATUREZA_EQUIPE");
            References(x => x.Natureza, "ID_NATUREZA").ForeignKey("FK_EQUIPE_NATUREZA_NATUREZA");
        }
    }
}
