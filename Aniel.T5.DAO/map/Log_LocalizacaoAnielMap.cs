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
    public class Log_LocalizacaoAnielMap : ClassMap<Log_LocalizacaoAniel>
    {
        public Log_LocalizacaoAnielMap()
        {
            Table("TB_LOG_LOCALIZACAO");

            Id(u => u.Id, "ID").GeneratedBy.Assigned();
            Map(x => x.DataHora_Servidor, "DATAHORA_SERVIDOR");

            References(x => x.Equipe, "EQUIPE").ForeignKey("FK_LOG_LOCALIZACAO_EQUIPE");

            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Projeto, "PROJETO").Length(20);
            Map(x => x.Num_Obra, "NUM_OBRA").Length(50);
            Map(x => x.Contrato, "CONTRATO");
            Map(x => x.Data, "DATA").Index("IDX_LOG_LOCALIZACAO_DATA");
            Map(x => x.Hora, "HORA").CustomType("Time");
            Map(x => x.Hora_Fim, "HORA_FIM").CustomType("Time");

            Map(x => x.Status, "STATUS").CustomType<StatusExecucaoAniel>();

            Map(x => x.Direcao, "DIRECAO");
            Map(x => x.Velocidade, "VELOCIDADE");
            Map(x => x.Altitude, "ALTITUDE");
            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");
            Map(x => x.TransferenciaMongo, "TRANSFERENCIA_MONGO");

            Map(p => p.UsuarioCriacao).Length(30);
            Map(p => p.DataHoraCriacao);
            Map(p => p.UsuarioAlteracao).Length(30);
            Map(p => p.DataHoraAlteracao);

            Map(p => p.Token).Length(50);

        }
    }
}
