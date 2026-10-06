using Aniel.Connect.ModelLog.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class Log_RoteirizacaoItemAnielMap : ClassMapBase<Log_RoteirizacaoItemAniel>
    {
        public Log_RoteirizacaoItemAnielMap() : base()
        {
            Table("TB_LOG_ROTEIRIZACAO_ITEM");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_ROTEIRIZACAO_ITEM");

            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Projeto, "PROJETO").Length(20);
            Map(x => x.Num_Obra, "NUM_OBRA").Length(50);
            Map(x => x.Num_Obra_Original, "NUM_OBRA_ORIGINAL").Length(50);
            Map(x => x.Num_Doc, "NUM_DOC");

            Map(x => x.Motivo, "MOTIVO").Length(1000);

            Map(x => x.Cod_tipo_serv, "COD_TIPO_SERV");
            Map(x => x.Desc_tipo_serv, "DESC_TIPO_SERV");
            Map(x => x.Cliente, "CLIENTE");
            Map(x => x.Id_Cliente, "ID_CLIENTE");
            Map(x => x.Equipe, "EQUIPE");
            Map(x => x.Nome_equipe, "NOME_EQUIPE");
            Map(x => x.Data_hora_ini_agendamento, "DATA_HORA_INI_AGENDAMENTO");
            Map(x => x.Data_hora_fim_agendamento, "DATA_HORA_FIM_AGENDAMENTO");
            Map(x => x.Deslocamento, "DESLOCAMENTO");
            Map(x => x.Cod_escritorio, "COD_ESCRITORIO");
            Map(x => x.Cod_area, "COD_AREA");
            Map(x => x.Cod_localidade, "COD_LOCALIDADE");
            Map(x => x.Localidade, "LOCALIDADE");
            Map(x => x.Desc_escritorio, "DESC_ESCRITORIO");
            Map(x => x.Desc_area, "DESC_AREA");
            Map(x => x.Posicao_Fila, "POSICAO_FILA");
            Map(x => x.SLA, "SLA");
            Map(x => x.Data_Vencimento, "DATA_VENCIMENTO");

            Map(x => x.Usuario_Monitor, "USUARIO_MONITOR");
            Map(x => x.Nome_Monitor, "NOME_MONITOR");
            Map(x => x.NumFre_Supervisor, "NUMFRE_SUPERVISOR");
            Map(x => x.Nome_Supervisor, "NOME_SUPERVISOR");
            Map(x => x.PossuiLatLong, "POSSUILATLONG");
            Map(x => x.LatitudeOS, "LATITUDE_OS");
            Map(x => x.LongitudeOS, "LONGITUDE_OS");
            Map(x => x.LatitudeCaixa, "LATITUDE_CAIXA");
            Map(x => x.LongitudeCaixa, "LONGITUDE_CAIXA");
            Map(x => x.LatitudeLocalidade, "LATITUDE_LOCALIDADE");
            Map(x => x.LongitudeLocalidade, "LONGITUDE_LOCALIDADE");
            Map(x => x.Desc_natureza, "DESC_NATUREZA");

            Map(x => x.OrdemDistribuida, "ORDEM_DISTRIBUIDA");
            Map(x => x.NivelRodadaRoteirizada, "NIVEL_RODADA_ROTEIRIZADA");
            Map(x => x.AgrupamentoRodadaRoteirizada, "AGRUPAMENTO_RODADA_ROTEIRIZADA");
            Map(x => x.PontoPartida, "PONTO_PARTIDA");
            Map(x => x.LatitudePontoPartida, "LATITUDE_PONTO_PARTIDA");
            Map(x => x.LongitudePontoPartida, "LONGITUDE_PONTO_PARTIDA");
            Map(x => x.OrigemAgendamento, "ORIGEM_AGENDAMENTO");

            References(x => x.Log, "ID_LOG").ForeignKey("FK_LOG_ROTERIZACAO_LOG");
        }
    }
}
