using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;


namespace Aniel.T5.DAO.map
{
    public class Perfil_EquipeAnielMap : ClassMap<Perfil_EquipeAniel>
    {
        public Perfil_EquipeAnielMap()
        {
            Table("TB_PERFIL_EQUIPE");

            Id(x => x.Cod_Perfil_Equipe, "COD_PERFIL_EQUIPE");

            Map(x => x.Nome, "NOME").Length(30);
        }
    }
}
