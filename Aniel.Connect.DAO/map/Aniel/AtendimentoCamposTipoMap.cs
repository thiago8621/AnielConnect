using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map.Aniel
{
    public class AtendimentoCamposTipoMap : ClassMapBase<AtendimentoCamposTipo>
    {

        public AtendimentoCamposTipoMap() : base()
        {
            Table("TB_ATENDIMENTO_CAMPOS_TIPO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO_CAMPOS_TIPO");
            Map(u => u.TipoAtendimento, "ID_TIPO_ATENDIMENTO");
            References(x => x.AtendimentoCampos, "ID_ATENDIMENTO_CAMPO").ForeignKey("FK_ATENDIMENTO_CAMPO"); ;
        }
    }
}
