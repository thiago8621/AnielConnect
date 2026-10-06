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
    public class Perfil_AgenteMap : ClassMapBase<Perfil_Agente>
    {
        public Perfil_AgenteMap() : base()
        {
            Table("TB_AGENTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_AGENTE_CATEGORIA");
            Map(u => u.Status, "STATUS").CustomType<Status_Default>();
            Map(u => u.Descricao, "DESCRICAO");
            HasMany(x => x.Categorias).KeyColumn("ID_AGENTE").Cascade.All().Inverse().Not.LazyLoad();
            HasMany(x => x.Tipo_atendimento).KeyColumn("ID_AGENTE").Cascade.All().Inverse().Not.LazyLoad();
            HasMany(x => x.Localidades).KeyColumn("ID_AGENTE").Cascade.All().Inverse().Not.LazyLoad();

            //Chat - Mensagens Padrões de Atendimento
            Map(u => u.Perfil_Atende_Chat, "PERFIL_ATENDE_CHAT");
            Map(u => u.Cliente_Aguardando_Atendimento, "CLIENTE_AGUARDANDO_ATEND");
            Map(u => u.Inicio_Atendimento, "INICIO_ATENDIMENTO");
            Map(u => u.Termino_Atendimento, "TERMINO_ATENDIMENTO");
            Map(u => u.Todos_Agentes_Offline, "TODOS_AGENTES_OFFLINE");
            Map(u => u.Ativar_Inatividade, "ATIVAR_INATIVIDADE");
            Map(u => u.Mensagem_Notif_Conclusao_Inatividade, "MSG_NOTIF_CONCLU_INATIV");
            Map(u => u.Mensagem_Enviada_Concluir_Inatividade, "MSG_ENV_CONCLU_INATIV");
            Map(u => u.Enviar_Notif_Cliente_Conclusao_Auto_Apos_x_Minutos, "ENV_NOTIF_CLI_CONCLU_AUTO");
            Map(u => u.Concluir_Atendi_Auto_Apos_x_Minutos_Envio_Notif_Inatividade, "CONCLU_ATEND_AUTO_ENV_NOTI");
            Map(u => u.Numero_Max_Atendi_Simultaneo_Por_Agente, "NUM_MAX_ATEND_SIMULT_AGENT");

            //Bot
            References(u => u.Bot, "ID_BOT").ForeignKey("FK_AGENTE_CATEGORIA");

        }
    }
}
