using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class Criterio_RoteirizacaoAnielMap : ClassMapBase<Criterio_RoteirizacaoAniel>
    {
        public Criterio_RoteirizacaoAnielMap()
        {
            Table("TB_CRITERIO_ROTEIRIZACAO");
            
            Map(x => x.Criterio, "CRITERIO").CustomType<Criterio_RoteirizacaoTipoAniel>();
            Map(x => x.Ordem, "ORDEM");
        }
    }
}
