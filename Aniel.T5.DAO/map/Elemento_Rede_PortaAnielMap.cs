using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class Elemento_Rede_PortaAnielMap: ClassMap<Elemento_Rede_Porta>
    {
        public Elemento_Rede_PortaAnielMap()
        {
            Table("TB_ELEMENTO_REDE_PORTA");
            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_ELEMENTO_REDE_PORTA");
            References(x => x.Caixa, "ID_CAIXA").ForeignKey("FK_ELEMENTO_REDE_PORTA_CAIXA");
            //References(x => x.Cliente_Plano_Prod, "CLIENTE_PLANO_PRODUTO").ForeignKey("FK_EL_REDE_PORTA_CLI_PL_PR");
            Map(x => x.Porta, "PORTA");
            Map(x => x.Data_Expiracao_Reserva, "DATA_EXPIRACAO_RESERVA");
            Map(x => x.Login, "LOGIN").Length(100);
            Map(x => x.IdLogin, "ID_LOGIN");
            Map(x=>x.Status,"STATUS").CustomType<Situacao_Porta>();
            Map(x => x.Cliente_Plano_Prod, "CLIENTE_PLANO_PRODUTO");
        }
    }
}
