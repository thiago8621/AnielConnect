using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class EquipeStatusAnielMap : ClassMapBase<EquipeStatusAniel>
    {
        public EquipeStatusAnielMap() : base()
        {
            Table("TB_EQUIPE_STATUS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_EQUIPE_STATUS");

            References(x => x.Equipe, "EQUIPE").ForeignKey("FK_EQUIPESTATUS_EQUIPE");

            Map(x => x.CEP, "CEP").Length(10);
            Map(x => x.Logradouro, "LOGRADOURO").Length(50);
            Map(x => x.Bairro, "BAIRRO").Length(30);

            Map(x => x.Cidade, "CIDADE").Length(30);
            Map(x => x.Numero, "NUMERO").Length(10);
            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");

            Map(x => x.Logado, "LOGADO");
            Map(x => x.DataHora_Login, "DATAHORA_LOGIN");
            Map(x => x.Precisao, "PRECISAO");
            Map(x => x.Bateria, "BATERIA");
            Map(x => x.StatusGPS, "STATUSGPS").Length(50);
            Map(x => x.Dispositivo, "DISPOSITIVO").Length(50);
            Map(x => x.Versao_Android, "VERSAO_ANDROID").Length(20);
            Map(x => x.Versao_App, "VERSAO_APP").Length(20);

            Map(x => x.Justificativa_Ausencia, "JUSTIFICATIVA");

            Map(x => x.Tipo, "TIPO").CustomType<EquipeStatusTipo>();

            Map(x => x.InternetMovel, "INTERNET_MOVEL").Length(20);

            Map(x => x.WiFi, "WIFI");
            Map(x => x.IMEI, "IMEI").Length(20);
        }
    }
}
