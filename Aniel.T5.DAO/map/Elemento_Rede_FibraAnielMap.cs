using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model;
using Sinapse.Model.enumerados;
namespace Aniel.T5.DAO.map
{
    public class Elemento_Rede_FibraAnielMap : ClassMapBase<Elemento_Rede_Fibra>
    {
        public Elemento_Rede_FibraAnielMap()
        {
            Table("TB_ELEMENTO_REDE_FIBRA");
            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_ELEMENTO_REDE_FIBRA");

            Map(p => p.numFibra, "FIBRA");
            Map(p => p.numLoose, "TUBO_LOOSE");

            Map(p => p.Ponta_A, "PONTA_ENTRADA");
            Map(p => p.Ponta_B, "PONTA_SAIDA");

            Map(p => p.Conexao_Ponta_A, "CONEXAO_PONTA_ENTRADA");
            Map(p => p.Conexao_Ponta_B, "CONEXAO_PONTA_SAIDA");

            References(p => p.Elemento_Cabo, "ID_CABO").ForeignKey("FK_ELEMENTO_REDE_CABO");

            //References(p => p.Modelo, "ID_MODELO").ForeignKey("FK_ELEMENTO_REDE_MODELO");

        }
    }
}
