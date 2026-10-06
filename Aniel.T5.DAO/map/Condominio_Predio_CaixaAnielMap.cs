using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;


namespace Aniel.T5.DAO.map
{
    public class Condominio_Predio_CaixaMap : ClassMapBase<Condominio_Predio_Caixa>
    {
        public Condominio_Predio_CaixaMap() : base()
        {
            Table("TB_CONDOMINIO_PREDIO_CAIXA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONDOMINIO_PREDIO_CAIXA");

            Map(x => x.Id_Condominio_Predio, "ID_CONDOMINIO_PREDIO");
            References(x => x.Id_Caixa, "ID_CAIXA").ForeignKey("FK_CONDOMINIO_PREDIO_PP_CAIXA");


        }
    }
}
