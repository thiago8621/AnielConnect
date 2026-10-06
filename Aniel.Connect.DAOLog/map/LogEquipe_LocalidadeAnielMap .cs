using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class LogEquipe_LocalidadeAnielMap : ClassMapBaseLog<LogEquipe_LocalidadeAniel>
    {
        public LogEquipe_LocalidadeAnielMap() : base()
        {
            Table("TB_LOG_EQUIPELOCALIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_EQUIPENATUREZA");
            Map(p => p.Equipe, "EQUIPE");
            Map(p => p.Cod_Escritorio, "COD_ESCRITORIO");
            Map(p => p.Cod_Area, "COD_AREA");
            Map(p => p.Cod_Localidade, "COD_LOCALIDADE");
            Map(p => p.Nome_Equipe, "NOME_EQUIPE");
            Map(p => p.Descricao_Escritorio, "DESCRICAO_ESCRITORIO");
            Map(p => p.Descricao_Area, "DESCRICAO_AREA");
            Map(p => p.Descricao_Localidade, "DESCRICAO_LOCALIDADE");
            Map(p => p.Todas_Localidades, "TODAS_LOCALIDADES");
            Map(p => p.Todas_Areas, "TODAS_AREAS");
        }
    }

}
