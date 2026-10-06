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
  public class Modelo_CampoDinamicoAnielMap:ClassMap<Modelo_CampoDinamico>
    {
        public Modelo_CampoDinamicoAnielMap() 
        {
            Table("TB_MODELO_CAMPO_DINAMICO");

            Id(x => x.Id,"ID").GeneratedBy.Sequence("GN_MODELO_CAMPO_DINAMICO");
            Map(x=>x.Descricao,"DESCRICAO").Length(100);
            Map(x=>x.Tipo,"TIPO_CAMPO").CustomType<Tipo_CampoModelo>();
            Map(x => x.Gravado, "GRAVADO");
            References(x=>x.Modelo,"ID_MODELO").ForeignKey("FK_MOD_CAMPO_DINAMICO_MOD");
        } 

    }
}
