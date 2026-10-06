using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class CamposImportacaoMap : ClassMap<CampoImportacaoModel>
    {
        public CamposImportacaoMap() : base()
        {

            Table("TB_CONFIG_IMPORTACAO_NET");

            Id(x => x.Id,"ID");
            Map(x => x.CampoAniel, "NOME_CAMPO_ANIEL");
            Map(x => x.CampoPlanilha, "CAMPO_PLANILHA");
            Map(x => x.Planilha, "PLANILHA");
            Map(x => x.Interno, "INTERNO");

        }
    }
}
