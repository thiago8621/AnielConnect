using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class Opcoes_CampoDinamicoAnielMap : ClassMap<Opcoes_CampoDinamico> {
        public Opcoes_CampoDinamicoAnielMap() 
        {
            Table("TB_OPCOES_CAMPO_DINAMICO");

            Id(t => t.Id, "ID").GeneratedBy.Sequence("GN_OPCOES_CAMPO_DINAMICO");

            Map(t => t.Resposta, "RESPOSTA_CAMPO").Length(50);

            References(t => t.Campo, "ID_CAMPO_DINAMICO").ForeignKey("FK_OPCOES_CAMPO_DIN_CAMPO");

        }
    }


}
