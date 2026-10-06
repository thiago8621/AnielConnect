using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map.Aniel
{
    public class AtendimentoCamposMap : ClassMapBase<AtendimentoCampos>
    {

        public AtendimentoCamposMap() : base()
        {
            Table("TB_ATENDIMENTO_CAMPOS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO_CAMPOS");

            Map(x => x.CampoObrigatorio, "CAMPO_OBRIGATORIO");
            Map(x => x.Titulo, "TITULO");
            Map(x => x.TipoCampoGravar, "TIPO_CAMPO").CustomType<TipoCampoGravar>();
            Map(x => x.Status, "STATUS");
            Map(x => x.VisualizacaoTotal, "VISUALIZACAO_TOTAL");
        }
    } 
}
