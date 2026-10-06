
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
    public class Modelo_ReciboMap : ClassMapBase<Modelo_Recibo>
    {
        public Modelo_ReciboMap() : base()
        {
            Table("TB_MODELO_RECIBO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MODELO_RECIBO");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Nome_Arquivo, "NOME_ARQUIVO").Length(100);
            Map(x => x.Extensao_Arquivo, "EXTENSAO_ARQUIVO").Length(5);
        }
    }
}
