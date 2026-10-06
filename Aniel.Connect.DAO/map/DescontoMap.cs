using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class DescontoMap : ClassMapBase<Desconto>
    {
        public DescontoMap() : base()
        {
            Table("TB_DESCONTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DESCONTO");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Desconto_Captacao, "DESCONTO_CAPTACAO").CustomType<Desconto_Captacao>();
            Map(x => x.Valor_Desconto, "VALOR_DESCONTO");
            Map(x => x.Percentual_Desconto, "PERCENTUAL_DESCONTO");
            Map(x => x.Repeticoes_Desconto, "REPETICOES_DESCONTO");
            Map(x => x.Desconto_Tempo_Indeterminado, "DESCONTO_TEMPO_INDETERMINADO");
            Map(x => x.Valor_Unico, "VALOR_UNICO");
            Map(x => x.Automatico, "AUTOMATICO");
            Map(x => x.CodigoAuxiliar, "CODIGO_AUXILIAR");
            Map(x => x.Status , "STATUS").CustomType<Status_Default>();

            References(x => x.Tipo_Desconto, "ID_TIPO_DESCONTO").ForeignKey("FK_DESCONTO_TIPO_DESC");

            HasMany(x => x.Valores).KeyColumn("ID_DESCONTO").Cascade.All().Inverse();
            HasMany(x => x.Regioes).KeyColumn("ID_DESCONTO").Cascade.All().Inverse();
            HasMany(x => x.Condicoes).KeyColumn("ID_DESCONTO").Cascade.All().Inverse();
            HasMany(x => x.Condominios_Predios).KeyColumn("ID_DESCONTO").Cascade.All().Inverse();
            //HasMany(x => x.Meios).KeyColumn("ID_DESCONTO").Cascade.All().Inverse();


        }


    }
}
