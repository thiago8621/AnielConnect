using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class Caderno_EncargosAnielMap : ClassMap<Caderno_EncargosAniel>
    {
        public Caderno_EncargosAnielMap() : base()
        {
            Table("TB_CADERNO_ENCARGOS");

            CompositeId()
            .KeyProperty(x => x.Unidade_Custo, "UNIDADE_CUSTO")
            .KeyProperty(x => x.Codigo, "CODIGO");

            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Unidade, "UNIDADE");
            Map(x => x.Classe_MO, "CLASSE_MO");
            Map(x => x.Quantidade, "QUANTIDADE");
        }
    }
}
