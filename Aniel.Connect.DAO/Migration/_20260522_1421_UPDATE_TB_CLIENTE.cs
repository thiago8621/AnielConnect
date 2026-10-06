using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20260522_1421_UPDATE_TB_CLIENTE : AnielMigration
    {
        public _20260522_1421_UPDATE_TB_CLIENTE()
        {
            Codigo = "20260522_1421";
            Descricao = "Atualização do tamanho do campo PESSOA_TEL_RECADO";
            Tipo = TipoAnielMigration.ScriptUnico;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(
            @"
               ALTER TABLE TB_CLIENTE
               ALTER COLUMN PESSOA_TEL_RECADO TYPE VARCHAR(255)
            ").ToString();
        }
    }
}
