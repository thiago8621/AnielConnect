using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;
using NHibernate.Id;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class Ordem_ServicoAnielMap : ClassMap<Ordem_ServicoAniel>
    {
        public Ordem_ServicoAnielMap()
        {
            Table("TB_ORDEM_SERVICO");
            CompositeId()
                .KeyProperty(x => x.Projeto,
                             column => column.ColumnName("PROJETO")
                                             .Length(20))

                .KeyProperty(x => x.CodCt,
                             column => column.ColumnName("CODCT")
                                             .Length(4))

                .KeyProperty(x => x.CodOs,
                             column => column.ColumnName("CODOS")
                                             .Length(5));

            Map(x => x.InicPrev, "INICPREV");
            Map(x => x.TermPrev, "TERMPREV");

            Map(x => x.InicReal, "INICREAL");
            Map(x => x.TermReal, "TERMREAL");
        }
    }
}
