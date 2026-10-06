using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Model.enumerado;

using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Workflow_VendasMap : ClassMapBase<Workflow_Vendas>
    {
        public Workflow_VendasMap() : base()
        {
            Table("TB_WORKFLOW_VENDA");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_WORKFLOW_VENDA");
            Map(x => x.Descricao, "DESCRICAO").Length(150);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.TipoPessoa, "TIPOPESSOA").CustomType<Tipo_Pessoa>();
            Map(x => x.Categoria, "CATEGORIA").Length(100);
            Map(x => x.Captacao, "CAPTACAO").Length(100);
            Map(x => x.Uf, "UF").CustomType<UF>();
            Map(x => x.Cidade, "CIDADE");
            Map(x => x.Prazo, "PRAZO");
            //Map(x => x.Tipodias, "TIPODIAS").CustomType<Tipo_SLA>();
            HasMany(x => x.Atividades).KeyColumn("ID_WORKFLOW_VENDAS").Cascade.All().Inverse();


        }

    }
}

