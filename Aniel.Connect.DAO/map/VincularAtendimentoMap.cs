using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class VincularAtendimentoMap : ClassMapBase<VincularAtendimento>
    {
        public VincularAtendimentoMap() : base()
        {
            Table("TB_VINCULAR_ATENDIMENTO");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_VINCULAR_ATENDIMENTO");            
            References(u => u.Atendimento, "ID_ATENDIMENTO").ForeignKey("FK_VINCULAR_ATENDIMENTO");
            Map(u => u.CodCT, "CODCT");
            Map(u => u.Projeto, "PROJETO");
            Map(u => u.Num_Doc, "NUM_DOC");
            Map(u => u.Num_Obra, "NUM_OBRA");
            Map(u => u.Numero_Atendimento, "NUN_ATENDIMENTO");

        }
    }
}
