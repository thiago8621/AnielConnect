using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Perfil_EquipeMap : ClassMapBase<Perfil_Equipe>
    {
        public Perfil_EquipeMap() : base()
        {
            Table("TB_PERFIL_EQUIPE");

            Id(x => x.Id);

            Map(x => x.Cod_Perfil_Equipe, "COD_PERFIL_EQUIPE");

            Map(x => x.Nome, "NOME").Length(30);
        }
    }
}
