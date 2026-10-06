using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Email_AtendimentoMap : ClassMapBase<Email_Atendimento>
    {
        public Email_AtendimentoMap() : base()
        {
            Table("TB_EMAIL_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_EMAIL_ATENDIMENTO");
            Map(x => x.Enviado, "ENVIADO");
            Map(x => x.Mudanca_Status, "MUDANCA_STATUS").CustomType<Status_Atendimento?>();
            Map(x => x.DataHoraEnvio, "DATAHORAENVIO");
            Map(x => x.Mensagem, "MENSAGEM").Length(900);
            Map(x => x.ReferenciaMessageID, "REFERENCIA_MESSAGE_ID").Length(300);
            Map(x => x.ReferenciaUltimoEnvioCLI, "REF_ULTIMO_ENVIO");
            References(x => x.Conta_Email, "ID_CONTA_EMAIL");
            References(x => x.Atendimento, "ID_ATENDIMENTO");
            References(x => x.Atendimento_Acao, "ID_ATENDIMENTO_ACAO");

        }
    }
}
