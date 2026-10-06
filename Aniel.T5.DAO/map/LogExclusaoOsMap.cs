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
    public class LogExclusaoOsMap : ClassMap<LogExclusaoOs>
    {
        public LogExclusaoOsMap() : base()
        {
            Table("TB_LOG_EXCLUSAO_OS");
            Id(p => p.Id, "ID").GeneratedBy.Sequence("GEN_LOG_EXCLUSAO_OS");
            Map(x => x.NumObra, "NUM_OBRA").Length(50);
            Map(x => x.Projeto, "PROJETO").Length(20);
            Map(x => x.NumDoc, "NUM_DOC");
            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.NumObraOriginal, "NUM_OBRA_ORIGINAL").Length(50);
            Map(x => x.Usuario, "USUARIO").Length(10);
            Map(x => x.UsuarioExclusao, "USUARIO_EXCLUSAO").Length(10);
            Map(x => x.IpMaquinaOrigem, "IP_MAQUINA_ORIGEM").Length(20);
            Map(x => x.Hostname, "HOSTNAME").Length(20);
            Map(x => x.Hora, "HORA");
            Map(x => x.HoraExclusao, "HORA_EXCLUSAO");
            Map(x => x.DataMaxima, "DATA_MAXIMA");
            Map(x => x.Dia, "DIA");
            Map(x => x.DiaExclusao, "DIA_EXCLUSAO");
            Map(x => x.obs, "OBS").Length(500);

        }
    }
}
