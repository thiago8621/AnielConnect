using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class SenhaAnielMap : ClassMap<SenhaAniel>
    {
        public SenhaAnielMap() : base()
        {
            Table("TB_SENHA");
            Id(x => x.Usuario, "USUARIO");
            Map(x => x.Nome, "NOME");
            Map(x => x.Senha, "SENHA");
            Map(x => x.Administrador, "ADMINISTRADOR");
            Map(x => x.Perfil_Dash_Cherry, "PERFIL_DASH_CHERRY");
            Map(x => x.Desc_Perfil_Dash_Cherry, "DESC_PERFIL_DASH_CHERRY");
            
            Map(x => x.NumFre, "NUM_FRE");
            References(x => x.Funcionario, "NUM_FRE").ReadOnly();
        }
    }
}
