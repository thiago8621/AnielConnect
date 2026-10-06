using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
namespace Aniel.T5.DAO.map
{
   public class ColunaElementoRedeMap : ClassMapBase<ColunaElementoRede>
    {
        public ColunaElementoRedeMap()
        {
            Table("TB_COLUNA_ELEMENTO_REDE");
            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_COLUNA_ELEMENTO_REDE");

            Map(p => p.Coluna, "COLUNA").CustomType<ColunaModeloElementoEnum>();
            Map(p => p.Obrigatorio, "OBRIGATORIO");
            References(x => x.Modelo_Elemento_Rede, "ID_MODELO").ForeignKey("FK_COLREDE_IDMODELO");
            References(x => x.CampoDinamco, "ID_CAMPO_DINAMICO").ForeignKey("FK_COLREDE_DINAMICO");
        }
    }
}
