using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class VincularPerfilMap : ClassMapBase<VincularPerfilAniel>
    {
        public VincularPerfilMap() : base()
        {
            Table("TB_VINCULAR_PERFIL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_VINCULAR_PERFIL");

            Map(x => x.Tecnico, "TECNICO").Length(100);            
        }
    }
}
