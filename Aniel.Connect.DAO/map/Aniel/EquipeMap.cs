using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class EquipeMap : ClassMapBase<Equipe>
    {
        public EquipeMap() : base()
        {
            Table("TB_EQUIPE");

            Id(x => x.Id);

            Map(x => x.Matricula, "MATRICULA").Length(10);
            Map(x => x.EquipeAux, "EQUIPEAUX").Length(10);
            Map(x => x.Nome, "NOME").Length(50);
            Map(x => x.Encerramento, "ENCERRAMENTO");

            References(x => x.Funcionario, "ID_FUNCIONARIO");
            References(x => x.Supervisor, "ID_SUPERVISOR");
            References(x => x.Perfil_Equipe, "ID_PERFIL_EQUIPE");
            References(x => x.Regiao, "ID_REGIAO");
        }
    }
}
