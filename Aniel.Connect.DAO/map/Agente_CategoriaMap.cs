using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Agente_CategoriaMap : ClassMapBase<Agente_Categoria>
    {
        public Agente_CategoriaMap() : base()
        {
            Table("TB_AGENTE_CATEGORIA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_AGENTE_CATEGORIA");
            References(u => u.Agente, "ID_AGENTE").ForeignKey("FK_AGENTE_CATEGORIA");
            References(u => u.Categoria_Cliente, "ID_CATEGORIA_CLIENTE").ForeignKey("FK_CATEGORIA_AGENTE");



        }
    }
}
