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
    public class URL_SistemaMap : ClassMapBase<URL_Sistema>
    {
        public URL_SistemaMap() : base()
        {
            Table("TB_URL_SISTEMA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_URL_SISTEMA");

            Map(x => x.Sistema , "SISTEMA").CustomType<Sistema>();
            Map(x => x.URL, "URL");

            Map(x => x.Id_Configuracoes_Gerais, "ID_CONFIGURACOES_GERAIS");
        }
    }
}
