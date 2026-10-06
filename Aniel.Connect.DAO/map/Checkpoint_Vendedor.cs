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
    public class Checkpoint_VendedorMap : ClassMapBase<Checkpoint_Vendedor>
    {
        public Checkpoint_VendedorMap() : base()
        {
            Table("TB_CHECKPOINT_VENDEDOR");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CHECKPOINT_VENDEDOR");

            References(x => x.Vendedor, "ID_VENDEDOR").ForeignKey("FK_CHECKPOINT_VEND_VEND");

            Map(x => x.Data_Hora, "DATA_HORA");
            Map(x => x.Endereco, "ENDERECO").Length(500);
            Map(x => x.Longitude, "LONGITUDE");
            Map(x => x.Latitude, "LATITUDE");

            References(x => x.Tipo, "ID_TIPO").ForeignKey("FK_CHECKPOINT_TIPO");
            
            Map(x => x.Observacao, "OBSERVACAO");
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
