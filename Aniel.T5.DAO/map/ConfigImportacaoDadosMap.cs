using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class ConfigImportacaoDadosMap : ClassMap<ConfigImportacaoDados>
    {
        public ConfigImportacaoDadosMap() : base()
        {

            Table("TB_CONFIG_IMPORT_NET_DADOS");

            CompositeId()
                .KeyProperty(x => x.CodCt, "CODCT")
                .KeyProperty(x => x.Projeto, "PROJETO")
                .KeyProperty(x => x.IdConfig, "ID_CONFIG");

            Map(x => x.CampoPlanilha, "CAMPO_PLANILHA");
            Map(x => x.Planilha, "PLANILHA");

        }
    }
}
