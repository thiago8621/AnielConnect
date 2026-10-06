using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    class Codigo_EncerramentoMap : ClassMapBase<Codigo_Encerramento>
    {
        public Codigo_EncerramentoMap() : base()
        {
            Table("TB_CODIGO_ENCERRAMENTO");

            Id(x => x.Id);

            Map(x => x.Cod_Encerramento, "COD_ENCERRAMENTO").Length(6);
            Map(x => x.Descricao, "DESCRICAO").Length(95);
            Map(x => x.Status_Execucao, "STATUS_EXECUCAO");

            References(x => x.Contrato, "ID_CONTRATO");
        }
    }
}
