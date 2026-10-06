using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Ponto_VendaMap : ClassMapBase<Ponto_Venda>
    {
        public Ponto_VendaMap() : base()
        {
            Table("TB_PONTO_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PONTO_VENDA");

            Map(x => x.Cep, "CEP").Length(100);
            Map(x => x.Logradouro, "LOGRADOURO").Length(100);            
            Map(x => x.Numero, "NUMERO").Length(100);
            Map(x => x.Bairro, "BAIRRO").Length(200);            
            Map(x => x.Cidade, "CIDADE").Length(100);
            Map(x => x.Complemento, "COMPLEMENTO").Length(200);
            Map(x => x.Nome, "NOME").Length(100);

            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");
            Map(x => x.Uf, "UF").Length(2);

            Map(x => x.Id_Bairro, "Id_Bairro");
            Map(x => x.Id_Cidade, "Id_Cidade");
            Map(x => x.Codigo, "CODIGO").Length(50);

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
