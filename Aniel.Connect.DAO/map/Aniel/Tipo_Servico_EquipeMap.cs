using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Tipo_Servico_EquipeMap : ClassMapBase<Tipo_Servico_Equipe>
    {
        public Tipo_Servico_EquipeMap() : base()
        {
            Table("TB_TIPO_SERVICO_EQUIPE");

            Id(x => x.Id);

            Map(x => x.Cod_Tipo_Serv, "COD_TIPO_SERV");
            Map(x => x.Descricao, "DESCRICAO").Length(150);
            Map(x => x.Descricao_Aux, "DESCRICAO_AUX").Length(50);
            Map(x => x.Cod_Natureza, "COD_NATUREZA");
        }
    }
}
