using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class PlanoProdutoAnielMap : ClassMap<PlanoProdutoAniel>
    {
        public PlanoProdutoAnielMap() : base()
        {
            Table("TB_PLANO_PRODUTO");

            Id(u => u.Id, "ID").GeneratedBy.Assigned();
            Map(p => p.UsuarioCriacao).Length(30);
            Map(p => p.DataHoraCriacao);
            Map(p => p.UsuarioAlteracao).Length(30);
            Map(p => p.DataHoraAlteracao);
            Map(p => p.Token).Length(50);
            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Tipo, "TIPO").CustomType<Tipo_PlanoProduto>();
            Map(x => x.Status, "STATUS").CustomType<Status_PlanoProduto>();
        }
    }
}
