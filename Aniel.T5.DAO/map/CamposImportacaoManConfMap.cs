using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class CamposImportacaoManConfMap : ClassMap<CampoImportacaoMantConfModel>
    {
        public CamposImportacaoManConfMap() : base()
        {

            Table("TB_CONFIG_IMPORT_MANUT_DADOS");

            Id(x => x.Id,"ID");
            Map(x => x.CodCt, "CODCT");
            Map(x => x.Projeto, "PROJETO");
            Map(x => x.IdConfig, "ID_CONFIG ");
            Map(x => x.Planilha, "PLANILHA");
            Map(x => x.Interno, "CAMPO_PLANILHA");
            Map(x => x.GerarSequencial, "GERA_SEQUENCIAL");

        }
    }
}
