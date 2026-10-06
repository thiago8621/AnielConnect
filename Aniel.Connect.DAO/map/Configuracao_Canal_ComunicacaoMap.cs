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
    public class Configuracao_Canal_ComunicacaoMap : ClassMapBase<Configuracao_Canal_Comunicacao>
    {

        public Configuracao_Canal_ComunicacaoMap() : base()
        {
            Table("TB_CONFIG_CANAL_COMUNICACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONFIG_CANAL_COMUNICACAO");

            Map(x => x.MostrarNomeAtendente, "MOSTRAR_NOME_ATENDENTE");
            Map(x => x.MostrarNomeAtendenteTelegram, "MOSTRAR_NOME_ATENDENTE_TELEGRAM");
            Map(x => x.MostrarNomeAtendenteMessenger, "MOSTRAR_NOME_ATENDENTE_MESSENG");
            Map(x => x.NomePrincipal, "NOME_PRINCIPAL");
            Map(x => x.NomeSecundario, "NOME_SECUNDARIO");
        }

    }
}
