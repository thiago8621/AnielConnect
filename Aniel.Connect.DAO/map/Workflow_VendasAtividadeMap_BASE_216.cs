using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Workflow_VendasAtividadeMap : ClassMapBase<Workflow_VendasAtividade>
    {
        public Workflow_VendasAtividadeMap() : base()
        {
            Table("TB_WORKFLOW_ATIVIDADE");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_WORKFLOW_ATIVIDADE");
            Map(x => x.Ativ_Descricao, "ATIV_DESCRICAO").Length(150);
            Map(x => x.Ativ_Prazo_Minutos, "ATIV_PRAZO_MINUTOS");
            Map(x => x.Ativ_Tipodias, "ATIV_TIPODIAS").CustomType<Tipo_HORA>();
            Map(x => x.Ativ_StatusVenda, "ATIV_STATUSVENDA").Length(100);
            Map(x => x.Horario_Atendimento, "HORA_ATENDIMENTO").Length(100);
            Map(x => x.Ativ_Decisao, "ATIV_DECISAO").CustomType<Model.enumerado.Atividade_Decisoria>(); 
            Map(x => x.Ativ_TipoDecisao, "ATIV_TIPODECISAO").CustomType<Tipo_Atividade_Decisoria>();
            Map(x => x.Ativ_NAtivAprovada, "ATIV_NATIVAPROVADA").Length(100);
            Map(x => x.Ativ_NAtivReprovada, "ATIV_NATIVREPROVADA").Length(100);
            Map(x => x.Ativ_Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Ativ_AgendaTarefa, "AGENDARTAREFA");
            References(x => x.WorkFlow, "ID_WORKFLOW_VENDAS").ForeignKey("FK_TB_WORKFLOW_VENDAS_ID_WORKFLOW");

            

    }

    }
}


