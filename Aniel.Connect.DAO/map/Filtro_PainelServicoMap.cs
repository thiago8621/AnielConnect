using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    class Filtro_PainelServicoMap : ClassMapBase<Filtro_PainelServico>
    {
        public Filtro_PainelServicoMap() : base()
        {
            Table("TB_RECURSO_PAINEL_SERVICO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_RECURSO_PAINEL_SERVICO");

            Map(x => x.Valor, "VALOR");
            Map(x => x.ValorPai, "VALOR_PAI");
            Map(x => x.ValorAvo, "VALOR_AVO");
            References(x => x.Modelo_PainelServico, "ID_MODELO").ForeignKey("FK_FILTRO_MOD_SERV");
            Map(x => x.Filtro, "FILTRO").CustomType<Filtro_PainelServicoEnum>();
            Map(x => x.Data, "DATA").CustomType<ModeloPainel_Data?>();

        }
    }
}
