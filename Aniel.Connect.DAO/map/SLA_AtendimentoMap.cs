using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Model.enumerado;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class SLA_AtendimentoMap : ClassMapBase<SLA_Atendimento>
    {
        public SLA_AtendimentoMap() : base()
        {
            Table("TB_SLA_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SLA_ATENDIMENTO");

            Map(x => x.Minutos, "MINUTOS");

            Map(x => x.Calendario_Padrao, "CALENDARIO_PADRAO");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Sla_PlanoProduto, "SLA_PLANOPRODUTO").CustomType<Status_Default>();

            References(x => x.Tipo_Atendimento, "ID_TIPO_ATENDIMENTO").ForeignKey("FK_SQL_ATEND_TIPO_ATEND");

            Map(x => x.Prioridade_Atendimento, "PRIORIDADE_ATENDIMENTO").CustomType<Prioridade_Atendimento>();

            Map(x => x.Id_Horario, "ID_HORARIO");

            Map(x => x.Contagem_Prazo, "CONTAGEM_PRAZO");

            References(x => x.Categoria_Cliente, "ID_CATEGORIA_CLIENTE").ForeignKey("FK_SLA_ATEND_CAT_CLIENT");

            HasMany(x => x.PlanosProdutos).KeyColumn("ID_SLA_ATENDIMENTO").Cascade.All().Inverse();
        }
    }
}
