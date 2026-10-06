using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20210908_1750_UPDATE_LANC_EQUIPE : AnielMigration
    {
        public _20210908_1750_UPDATE_LANC_EQUIPE()
        {
            Codigo = "20210908_1750";
            Descricao = "Update Lançamento Equipe";
            Tipo = TipoAnielMigration.ScriptUnico;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE TB_LOG_LANC_SERVICO A SET A.PROJETO = 
(SELECT FIRST 1 B.PROJETO FROM TB_DOCUMENTO_PRODUCAO B WHERE B.CODCT = A.CODCT AND B.NUM_DOC = A.NUM_DOC AND A.NUM_OBRA = B.NUM_OBRA)
WHERE A.PROJETO  = '';").ToString();
        }
    }
}
