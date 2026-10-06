using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Sub_Tipo_ServicoMap : ClassMapBase<Sub_Tipo_Servico>
    {
        public Sub_Tipo_ServicoMap() : base()
        {
            Table("TB_SUB_TIPO_SERVICO");

            Id(x => x.Id);

            Map(x => x.Codigo, "COD_SUB_TIPO_SERV").Length(6);
            Map(x => x.Descricao, "DESCRICAO").Length(70);

            References(x => x.Tipo_Servico_Equipe, "ID_TIPO_SERV");
        }
    }
}
