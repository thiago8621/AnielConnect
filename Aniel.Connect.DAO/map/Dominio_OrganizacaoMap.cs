using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Dominio_OrganizacaoMap : ClassMapBase<Dominio_Organizacao>
    {
        public Dominio_OrganizacaoMap() : base()
        {
            Table("TB_DOMINIO_ORGANIZACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DOMINIO_ORGANIZACAO");
            Map(m => m.Dominio,"DOMINIO");
            References(m => m.Organizacao, "ID_ORGANIZACAO").ForeignKey("FK_DOMINIO_ORGANIZACAO"); ;
        }
    }
}
