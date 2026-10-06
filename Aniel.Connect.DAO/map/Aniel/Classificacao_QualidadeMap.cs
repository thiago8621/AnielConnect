using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Classificacao_QualidadeMap : ClassMapBase<Classificacao_Qualidade>
    {
        public Classificacao_QualidadeMap() : base()
        {
            Table("TB_CLASSIFICACAO_QUALIDADE");

            Id(x => x.Id);

            Map(x => x.Qualidade, "QUALIDADE").Length(50);
        }
    }
}
