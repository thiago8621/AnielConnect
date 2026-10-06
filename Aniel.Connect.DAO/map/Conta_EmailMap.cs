using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Conta_EmailMap : ClassMapBase<Conta_Email>
    {
        public Conta_EmailMap() : base()
        {
            Table("TB_CONTA_EMAIL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONTA_EMAIL");

            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Email, "EMAIL");
            Map(x => x.Porta_Imap, "PORTA_IMAP");
            Map(x => x.Senha, "SENHA");
            Map(x => x.Servidor_Imap, "SERVIDOR_IMAP");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.AuthMethodImap, "AUTHMETHODIMAP").CustomType<Status_Default>();
            Map(x => x.Utilizar_Ssl_Imap, "UTILIZAR_SSL_IMAP");
            Map(x => x.Utilizar_Ssl_Smtp, "UTILIZAR_SSL_SMTP");
            Map(x => x.Servidor_Smtp, "SERVIDOR_SMTP");
            Map(x => x.Porta_Smtp, "PORTA_SMTP");
            Map(x => x.Principal, "PRINCIPAL");
            Map(x => x.UltimaData, "ULTIMA_DATA");

        }
    }
}

