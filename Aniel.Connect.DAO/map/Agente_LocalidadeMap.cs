using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Agente_LocalidadeMap : ClassMapBase<Agente_Localidade>
    {
        public Agente_LocalidadeMap() : base()
        {
            Table("TB_AGENTE_LOCALIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_AGENTE_LOCALIADE");
            References(u => u.Agente, "ID_AGENTE").ForeignKey("FK_AGENTE_LOCALIDADE");
            Map(u => u.Cod_Escritorio, "COD_ESCRITORIO");
            Map(u => u.Cod_Area, "COD_AREA");

            Map(u => u.Cod_Localidade, "COD_LOCALIDADE");
            Map(u => u.Desc_Localidade, "DESC_LOCALIDADE");
            Map(u => u.Todas_Localidades, "TODAS_LOCALIDADES");
            Map(u => u.Todas_Areas, "TODAS_AREAS");
        }
    }
}
