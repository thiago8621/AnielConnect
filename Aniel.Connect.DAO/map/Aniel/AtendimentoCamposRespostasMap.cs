using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map.Aniel
{
    public class AtendimentoCamposRespostasMap : ClassMapBase<AtendimentoCamposRespostas>
    {

        public AtendimentoCamposRespostasMap() : base()
        {
            Table("TB_ATENDIMENTO_CAMPOS_RESPOSTAS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO_CAMPOS_RESPOSTAS");

            Map(x => x.RespostaPersonalizada, "RESPOSTA_PERSONALIZADA").Length(1000);

            References(x => x.AtendimentoCampos, "ID_ATENDIMENTO_CAMPO").ForeignKey("FK_ATENDIMENTO_CAMPO"); ;
        }
    } 
}
