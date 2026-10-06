using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class VinculoAtendimentoCamposMap : ClassMapBase<VinculoAtendimentoCampos>
    {

        public VinculoAtendimentoCamposMap() : base()
        {
            Table("TB_VINCULO_ATENDIMENTO_CAMPOS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_VINCULO_ATENDIMENTO_CAMPOS");

            Map(x => x.Resposta, "RESPOSTA_PERSONALIZADA").Length(1000);

            References(x => x.AtendimentoCampos, "ID_ATENDIMENTO_CAMPO").Nullable().Cascade.SaveUpdate().ForeignKey("FK_ATENDIMENTO_CAMPO");
            References(x => x.Atendimento, "ID_ATENDIMENTO").Nullable().Cascade.SaveUpdate().ForeignKey("FK_ATENDIMENTO_ATT");
            References(x => x.AtendimentoCamposRespostas, "ID_ATENDIMENTO_CAMPO_RESPOSTA").Nullable().Cascade.SaveUpdate().ForeignKey("FK_ATT_CAMPO_RESPOSTA");
        }
    }
}
