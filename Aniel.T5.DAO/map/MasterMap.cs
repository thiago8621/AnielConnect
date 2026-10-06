using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Aniel.T5.Model.enumerado;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class MasterMap : ClassMap<Master>
    {
        public MasterMap() : base()
        {
            Table("TB_MASTER");

            Id(x => x.Id, "ID").GeneratedBy.Native();

            Map(x => x.Config_Doc, "CONFIG_DOC").Length(5000);

            Map(x => x.SLA_Servico_Padrao, "SLA_SERVICOS_PADRAO");

            Map(x => x.SLA_Data_Abertura, "SLA_DATA_ABERTURA").CustomType<Enum_Tipo_Sla_>();

            Map(x => x.Usa_Roteirizacao, "USA_ROTEIRIZACAO").Length(1);

            Map(x => x.Encerrar_Atendimento_OS, "ENCERRAR_ATENDIMENTO_OS").Length(1);

            Map(x => x.Deslocamento_Dinamico, "DESLOCAMENTO_DINAMICO").Length(1);
            
            Map(x => x.ConfigParalisaOS, "CONFIG_PARALISA_OS").Length(1);

            Map(x => x.Autoriza_Baixa_Servico_Backoffice, "AUTORIZA_BAIXA_OS_BACKOFFICE").Length(1);

            Map(x => x.Tipo_Integracao_CTO, "TIPO_INTEGRACAO_CTO");

        }
    }
}
