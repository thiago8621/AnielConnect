using Aniel.Connect.ModelLog;
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
    public class Log_RoteirizacaoAnielMap : ClassMapBase<Log_RoteirizacaoAniel>
    {
        public Log_RoteirizacaoAnielMap() : base()
        {
            Table("TB_LOG_ROTEIRIZACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_ROTEIRIZACAO");

            Map(x => x.DataHoraFinalizacao, "DATAHORA_FINALIZACAO").Length(4);
            Map(x => x.Status, "STATUS").CustomType<StatusRoteirizacao>();
            Map(x => x.Simulacao, "SIMULACAO");
            Map(x => x.DataRoteirizacao, "DATA_ROTEIRIZACAO");

            Map(x => x.ConfiguracoesGerais, "CONFIGURACOES_GERAIS").Length(5000);
            Map(x => x.ConfiguracoesRoteirizacao, "CONFIGURACOES_ROTEIRIZACAO").Length(5000);
            Map(x => x.TecnicosRoteirizacao, "TECNICOS_ROTEIRIZACAO").Length(5000);
            Map(x => x.DocumentosValidos, "DOCUMENTOS_VALIDOS").Length(5000);
        }
    }
}
