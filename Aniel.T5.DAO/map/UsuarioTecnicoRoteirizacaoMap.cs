using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class UsuarioTecnicoRoteirizacaoMap : ClassMap<UsuarioTecnicoRoteirizacao>
    {
        public UsuarioTecnicoRoteirizacaoMap()
        {
            Table("TB_USUARIO_TECNICOROTEIRIZACAO");
            Id(x => x.Id).GeneratedBy.Sequence("GN_USUARIO_TECNICOROTEIRIZACAO");
            Map(x => x.Equipe, "EQUIPE").Length(10);
            Map(x => x.Usuario, "USUARIO").Length(10).Index("IDX_USUARIO_TECNICOROTEIRIZACAO");
            Map(x => x.DataHoraCriacao);

        }
    }
}
