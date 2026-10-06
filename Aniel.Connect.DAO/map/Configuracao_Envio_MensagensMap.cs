using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Configuracao_Envio_MensagensMap: ClassMapBase<Configuracao_Envio_Mensagens>
    {
        public Configuracao_Envio_MensagensMap()
        {
            Table("TB_CONFIGURACAO_ENVIO_MSG");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONFIGURACAO_ENVIO_MSG");

            Map(x => x.Envia_Msg_Ao_Iniciar_Deslocamento, "ENVIAR_MSG_DESLOCAMENTO_OS");
            Map(x => x.Status, "STATUS");
            Map(x => x.Utiliza_Envio_Email, "UTILIZA_ENVIO_EMAIL");
            Map(x => x.Utiliza_Envio_SMS, "UTILIZA_ENVIO_SMS");
            Map(x => x.Mensagem_Corpo_SMS, "MENSAGEM_CORPO_SMS").Length(5000);
            Map(x => x.Assunto_Email, "ASSUNTO_EMAIL");
            Map(x => x.Mensagem_Corpo_Email, "MENSAGEM_CORPO_EMAIL").Length(5000);
            Map(x => x.Descricao, "DESCRICAO").Length(5000);
        }
    }
}
