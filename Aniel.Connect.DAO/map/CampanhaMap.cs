//using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class CampanhaMap : ClassMapBase<Campanha>
    {
        public CampanhaMap() : base()
        {
            Table("TB_CAMPANHA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CAMPANHA");

            Map(x => x.Titulo, "TITULO").Length(100);
            Map(x => x.Data_Inicio, "DATA_INICIO");
            Map(x => x.Data_Final, "DATA_FINAL");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Camp_Tempo_Determinado, "CAMP_TEMPO_DETERMINADO").CustomType<Status_Default>();
            Map(x => x.Definir_Campanha_Regioes, "DEFINIR_CAMPANHA_REGIOES").CustomType<Status_Default>();
            Map(x => x.Definir_Area_Cobertura, "DEFINIR_AREA_COBERTURA").CustomType<Status_Default>();
            Map(x => x.Definir_CampanhaEquipeVenda, "DEFINIR_CAMPANHA_EQUIPE_VENDA").CustomType<Status_Default>();
            Map(x => x.CampanhaUtilizadaPor, "CAMPANHA_UTILIZADA_POR").CustomType<CampanhaUtilizadaPor>();
            Map(x => x.PrefixoCupom, "PREFIXO_CUPOM").Length(50).Unique();
            Map(x => x.Id_Meio_Captacao, "ID_MEIO_CAPTACAO");

            #region Mobile
            Map(x => x.Combo, "COMBO");
            #endregion

            HasMany(x => x.PlanosProdutos).KeyColumn("ID_CAMPANHA").Cascade.All().Inverse();
            HasMany(x => x.Regioes).KeyColumn("ID_CAMPANHA").Cascade.All().Inverse();
            HasMany(x => x.AreaCobertura).KeyColumn("ID_CAMPANHA").Cascade.All().Inverse();
            HasMany(x => x.EquipeVendas).KeyColumn("ID_CAMPANHA").Cascade.All().Inverse();

            /*//Descomentado - RENAN - PREMIER
            References(x => x.Filial, "ID_FILIAL").ForeignKey("FK_CAMPANHA_FILIAL");
            References(x => x.Funcionario, "ID_FUNCIONARIO").ForeignKey("FK_CAMPANHA_FUNC");
            References(x => x.Supervisor, "ID_SUPERVISOR").ForeignKey("FK_CAMPANHA_SUPERVISOR");*/


        }
    }
}
