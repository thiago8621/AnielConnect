using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Checkin_VendedorMap : ClassMapBase<Checkin_Vendedor>
    {
        public Checkin_VendedorMap() : base()
        {
            Table("TB_CHECKIN_VENDEDOR");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CHECKIN_VENDEDOR");

            References(x => x.Vendedor, "ID_VENDEDOR").ForeignKey("FK__CHECKIN_VENDEDOR");

            Map(x => x.Data, "DATA");
            Map(x => x.DataInicial, "DATA_INICIAL");
            Map(x => x.DataFinal, "DATA_FINAL");
            Map(x => x.Endereco_Checkin, "ENDERECO_CHECKIN").Length(500);
            Map(x => x.Latitude_Checkin, "LATITUDE_CHECKIN");
            Map(x => x.Longitude_Checkin, "LONGITUDE_CHECKIN");
            Map(x => x.Longitude_Checkout, "LONGITUDE_CHECKOUT");
            Map(x => x.Latitude_Checkout, "LATITUDE_CHECKOUT");

            References(x => x.Ponto_Venda, "ID_PONTO_VENDA").ForeignKey("FK_CHEKING_V_PV");
            
            Map(x => x.Endereco_Checkout, "ENDERECO_CHECKOUT");
            Map(x => x.Atualizado_Em, "ATUALIZADO_EM");
            Map(x => x.Precisao, "PRECISAO");
            Map(x => x.Bateria, "BATERIA");
            Map(x => x.Status_Gps, "STATUS_GPS");
            Map(x => x.Dispositivo, "DISPOSITIVO");
            Map(x => x.Versao_Android, "VERSAO_ANDROID");
            Map(x => x.Versao_App, "VERSAO_APP");

    }
    }
}
