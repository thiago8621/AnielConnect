using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ImportacaoLogMap : ClassMapBase<LogImportacao>
    {
        public ImportacaoLogMap() : base()
        {
            Table("TB_LOG_IMPORTACAO");

            Id(x => x.Id);

            Map(x => x.Data,"DATA");
            Map(x => x.Hora,"HORA");
            Map(x => x.Erro,"ERRO");
            Map(x => x.OS,"OPERACAO");
            Map(x => x.Usuario,"USUARIO");
        }
    }
}
