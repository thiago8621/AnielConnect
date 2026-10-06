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
    public class Categoria_ClienteMap : ClassMapBase<Categoria_Cliente>
    {
        public Categoria_ClienteMap() : base()
        {
            Table("TB_CATEGORIA_CLIENTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CATEGORIA_CLIENTE");
            Map(x => x.Id_Cfop, "Id_Cfop");

            Map(x => x.Descricao, "DESCRICAO").Length(100);            
            Map(m => m.Tipo_Prioridade, "TIPO_PRIORIDADE").CustomType<Tipo_Prioridade>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.CodigoTipoClienteIntegracao, "CODIGO_TIPO_CLIENTE_INTEGRACAO");
        }
    }
}
