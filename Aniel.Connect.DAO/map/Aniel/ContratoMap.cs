using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ContratoMap : ClassMapBase<Contrato>
    {
        public ContratoMap() : base()
        {
            Table("TB_CONTRATO");

            Id(x => x.Id);

            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Descricao, "DESCRICAO").Length(30);
            Map(x => x.Localidade, "LOCALIDADE").Length(40);
            Map(x => x.Fornec_Mat_Padrao, "FORNEC_MAT_PADRAO").Length(1);
            Map(x => x.TipoSaldo, "TIPOSALDO").Length(10);
            Map(x => x.TipoSaldoAplic, "TIPOSALDOAPLIC").Length(10);

            References(x => x.Projeto_Regulador, "ID_PROJETOREGULADOR");
            References(x => x.Filial, "ID_FILIAL");
        }
    }
}
