using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20220328_1703_TB_PAIS : AnielMigration
    {
        public _20220328_1703_TB_PAIS()
        {
            Codigo = "20220328_1703";
            Descricao = "Registro Regular para insercao na TB_PAIS";
            Tipo = TipoAnielMigration.ScriptUnico;
        }
        public override String GetSql() { return @"
            UPDATE OR INSERT INTO TB_PAIS(ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, NOME) 
            VALUES(1, 'ADMIN', NULL, NULL, NULL, NULL, 'BRASIL');"; }



    }
}
