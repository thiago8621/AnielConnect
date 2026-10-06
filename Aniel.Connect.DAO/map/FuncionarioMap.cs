using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class FuncionarioMap : ClassMapBase<Funcionario>
    {
        public FuncionarioMap() : base()
        {
            Table("TB_FUNCIONARIO");

            Map(x => x.Num_Fre, "NUM_FRE");
            Map(x => x.Nome, "NOME").Length(100);
            Map(x => x.Tel_Celular, "TEL_CELULAR").Length(15);
            Map(x => x.Situacao, "SITUACAO");

            Map(x => x.Vendedor, "VENDEDOR");
            References(x => x.Supervisor, "ID_SUPERVISOR");

            Map(x => x.E_SUPERVISOR, "E_SUPERVISOR");
            References(x => x.Meta, "ID_META");
            References(x => x.Faixa, "ID_FAIXA_COMISSIONAMENTO");

            References(x => x.Departamento, "ID_DEPARTAMENTO");
            References(x => x.Filial, "ID_FILIAL");
        }
    }
}
