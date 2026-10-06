using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ProjetoMap : ClassMapBase<Projeto>
    {
        public ProjetoMap() : base()
        {
            Table("TB_PROJETO");

            Id(x => x.Id);

            Map(x => x.Codigo, "PROJETO").Length(20);
            Map(x => x.Localidade, "LOCALIDADE").Length(50);

            Map(x => x.IniPrev, "INIPREV");
            Map(x => x.TermPrev, "TERMPREV");
            Map(x => x.IniReal, "INIREAL");
            Map(x => x.TermReal, "TERMREAL");
            Map(x => x.TermReal2, "TERMREAL2");

            References(x => x.Contrato, "ID_CONTRATO");

            References(x => x.Projeto_Regulador, "ID_PROJETOREGULADOR");
            References(x => x.Filial, "ID_FILIAL");
        }
    }
}
