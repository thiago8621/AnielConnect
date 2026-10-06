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
    public class Modelo_PainelAtendimentoMap : ClassMapBase<Modelo_PainelAtendimento>
    {
        public Modelo_PainelAtendimentoMap() : base()
        {
            Table("TB_MODELO_PAINEL_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MODELO_PAINEL_ATENDIMENTO");
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Visibilidade, "VISIBILIDADE").CustomType<Visibilidade>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Coluna_Ordenar, "COLUNA_ORDENAR").CustomType<Coluna_PainelAtendimentoEnum>();
            Map(x => x.Padrao, "PADRAO").CustomType<Sinapse.Model.enumerados.Padrao>();
            HasMany(x => x.Filtros).KeyColumn("ID_MODELO").Cascade.All().Inverse();
            HasMany(x => x.Colunas).KeyColumn("ID_MODELO").Cascade.All().Inverse();
        }
    }
}
