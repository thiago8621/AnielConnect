using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class EmpresaAnielMap : ClassMap<EmpresaAniel>
    {
        public EmpresaAnielMap() : base()
        {
            Table("TB_EMPRESA");

            Id(x => x.CodEmpresa, "CODEMPRESA");
            Map(x => x.Razao_Social, "RAZAO_SOCIAL").Length(60);
            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Logotipo, "LOGOTIPO");
            Map(x => x.Data_Alteracao, "DATA_ALTERACAO");
        }
    }
}
