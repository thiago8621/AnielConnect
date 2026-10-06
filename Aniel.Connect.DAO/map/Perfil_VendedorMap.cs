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
    public class Perfil_VendedorMap : ClassMapBase<Perfil_Vendedor>
    {
        public Perfil_VendedorMap():base()
        {
            Table("TB_PERFIL_VENDEDOR");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COBRANCA_SERVICO");
            Map(m => m.Descricao, "DESCRICAO").Length(150);
            Map(m => m.Area_Cobertura, "AREA_COBERTURA").Length(150);
            Map(m => m.Tipo_Pessoa, "TIPO_PESSOA").Length(150);
            Map(m => m.Campanha_Regiao, "CAMPANHA_REGIAO").Length(150);
            Map(m => m.Categoria_Cliente, "CATEGORIA_CLIENTE").Length(150);
            Map(m => m.Meio_Capitacao, "MEIO_CAPITACAO").Length(150);
             Map(m => m.Autorizacao_Vendas, "Autorizacao_Vendas").Length(150);
            
            Map(m => m.Status, "STATUS").CustomType<Status_Default>();

        }
    }
}
