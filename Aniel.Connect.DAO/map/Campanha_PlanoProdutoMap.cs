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
    public class Campanha_PlanoProdutoMap : ClassMapBase<Campanha_PlanoProduto>
    {
        public Campanha_PlanoProdutoMap() : base()
        {
            Table("TB_CAMPANHA_PLANO_PRODUTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CAMPANHA_PLANO_PRODUTO");
            Map(x => x.Id_Plano_Produto, "ID_PLANO_PRODUTO");
            Map(x => x.Novo_Nome_Plano_Produto, "N_NOME_PLANO_PRODUTO");
            Map(x => x.Valor_Plano_Produto, "VALOR_PLANO_PRODUTO");
            Map(x => x.Valor_Instalacao, "VALOR_INSTALACAO");
            //Map(x => x.Validade_Plano_Produto, "VALIDADE_PLANO_PRODUTO");
            Map(x => x.Data_Validade, "DATA_VALIDADE");
            Map(x => x.Validade_Plano_Produto, "VALIDADE_PLANO_PRODUTO").CustomType<Atividade_Decisoria>();
            Map(x=>x.Parcela_Instalacao,"PARCELAS_INSTALACAO");
            Map(x=>x.IsPlanProd,"E_PLANO_PRODUTO").CustomType<PlanoProdutoEnum>();

            References(x => x.Campanha, "ID_CAMPANHA").ForeignKey("FK_CAMPANHA");
            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_CAMPANHA_PP_PLANO");
            References(x => x.Produto, "ID_PRODUTO").ForeignKey("FK_CAMPANHA_PP_PRODUTO");


        }
    }
}
