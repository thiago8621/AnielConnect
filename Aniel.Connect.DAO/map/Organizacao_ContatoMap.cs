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
    public class Organizacao_ContatoMap : ClassMapBase<Organizacao_Contato>
    {
        public Organizacao_ContatoMap() : base()
        {
            Table("TB_ORGANIZACAO_CONTATO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ORGANIZACAO_CONTATO");
            Map(m => m.Contato, "Contato");
            Map(m => m.Informacao_Adicional, "INFORMACAO_ADICIONAL");
            Map(m => m.Tipo_Contato, "TIPO_CONTATO").CustomType<Tipo_Contato>();
            References(m => m.Organizacao, "ID_ORGANIZACAO").ForeignKey("FK_ORGANIZACAO_CONTATO"); ;
        }
    }
}
