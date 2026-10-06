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
    public class Log_Lanc_ServicoMap : ClassMap<Log_Lanc_Servico>
    {
        public Log_Lanc_ServicoMap() : base()
        {
            Table("TB_LOG_LANC_SERVICO");

            Id(x => x.Id, "ID").GeneratedBy.Native();

            Map(x => x.CodCt, "CODCT").Length(4).Index("IDX_OTIMIZA_LOG");
            Map(x => x.Projeto, "PROJETO").Length(20).Index("IDX_OTIMIZA_LOG");
            Map(x => x.Num_Obra, "NUM_OBRA").Length(50).Index("IDX_OTIMIZA_LOG");
            Map(x => x.Contrato, "NUM_DOC").Index("IDX_OTIMIZA_LOG");
            Map(x => x.Dia, "DIA");
            Map(x => x.Hora, "HORA").CustomType("Time"); ;
            Map(x => x.Usuario, "USUARIO").Length(10);
            Map(x => x.Valor_Ant, "VALOR_ANT").Length(800);
            Map(x => x.Alteracao, "ALTERACAO").Length(200);
            Map(x => x.Data_Maxima, "DATA_MAXIMA");
            Map(x => x.Cod_Tipo_Serv, "COD_TIPO_SERV");
            Map(x => x.Motivo_Exclusao, "MOTIVO_EXCLUSAO").Length(50);
            Map(x => x.Valor_Atualizado, "VALOR_ATUALIZADO").Length(800);
            Map(x => x.Obs_Cobranca_Status, "OBS_STATUS_COBRANCA").Length(5000);

            Map(x => x.CodMotivoReagendamento, "COD_MOTIVO_REAGENDAMENTO");
            Map(x => x.ObsMotivoReagendamento, "OBSERVACAO_MOTIVO_REAGENDAMENTO").Length(110);
        }
    }
}
