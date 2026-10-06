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
    public class Elemento_RedeCamposAnielMap : ClassMapBase<Elemento_RedeCampos>
    {
        public Elemento_RedeCamposAnielMap()
        {
            Table("TB_ELEMENTO_REDE_CAMPOS");
            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_ELEMENTO_REDE_CAMPOS");
            Map(p => p.Id_Campo_Dinamico, "ID_CAMPO_DINAMICO").Length(50);
            References(p => p.Elemento, "ID_ELEMENTO_REDE").ForeignKey("FK_ELEMENTO_REDE_ID");
            Map(p=>p.Tipo,"TIPO").Length(50);
            Map(p=>p.Descricao,"DESCRICAO").Length(50);
            Map(p=>p.Valor,"VALOR").Length(50);
            //References(p => p.Modelo, "ID_MODELO").ForeignKey("FK_ELEMENTO_REDE_MODELO");

        }
    }
}
