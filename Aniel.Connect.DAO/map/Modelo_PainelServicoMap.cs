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
    class Modelo_PainelServicoMap : ClassMapBase<Modelo_PainelServico>
    {
        public Modelo_PainelServicoMap() : base()
        {
            Table("TB_MODELO_PAINEL_SERVICO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MODELO_PAINEL_SERVICO");
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Visibilidade, "VISIBILIDADE").CustomType<Visibilidade>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Coluna_Ordenar, "COLUNA_ORDENAR").CustomType<ColunaOrdenarPainelServicoEnum>();
            Map(x => x.Padrao, "PADRAO").CustomType<Sinapse.Model.enumerados.Padrao>();
            HasMany(x => x.Filtros).KeyColumn("ID_MODELO").Cascade.All().Inverse();
            HasMany(x => x.Colunas).KeyColumn("ID_MODELO").Cascade.All().Inverse();
        }
    }
}
