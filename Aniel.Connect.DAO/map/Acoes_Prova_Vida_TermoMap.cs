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
    public class Acoes_Prova_Vida_TermoMap : ClassMapBase<Acoes_Prova_Vida_Termo>
    {
        public Acoes_Prova_Vida_TermoMap() : base()
        {
            Table("TB_ACOES_PROVA_VIDA_TERMO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ACOES_PROVA_VIDA_TERMO");

            Map(x => x.Ordem, "ORDEM");
            Map(x => x.Valor, "VALOR"); 
        }
    }
}
