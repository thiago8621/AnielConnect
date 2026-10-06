using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class ProjetoAnielMap : ClassMap<ProjetoAniel>
    {
        public ProjetoAnielMap()
        {
            Table("TB_PROJETO");

            CompositeId()
            .KeyProperty(x => x.Codigo, "PROJETO")
            .KeyProperty(x => x.CodCt, "CODCT");

            Map(x => x.Localidade, "LOCALIDADE");
            Map(x => x.IniPrev, "INICPREV");
            Map(x => x.TermPrev, "TERMPREV");
            Map(x => x.IniReal, "INICREAL");
            Map(x => x.TermReal, "TERMREAL");
            Map(x => x.TermReal2, "TERMREAL2");

            Map(x => x.CodCt_Regulador, "CODCT_REGULADOR");
            Map(x => x.Situacao, "SITUACAO");
            Map(x => x.Projeto_Regulador, "PROJETO_REGULADOR");

            Map(x => x.CodEmpresa, "CODEMPRESA");
            Map(x => x.CodFilial, "CODFILIAL");
            Map(x => x.CodEscritorio, "COD_ESCRITORIO");
        }
    }
}
