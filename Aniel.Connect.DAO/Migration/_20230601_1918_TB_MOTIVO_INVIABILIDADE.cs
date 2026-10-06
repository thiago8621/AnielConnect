using Microsoft.AspNet.Identity;
using NHibernate.Engine;
using Sinapse.DAO;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace Aniel.Connect.DAO
{
    public  class _20230601_1918_TB_MOTIVO_INVIABILIDADE : AnielMigration 
    {
        public _20230601_1918_TB_MOTIVO_INVIABILIDADE() 
        {
            Codigo = "20230601_1918";
            Descricao = "Motivos de Inviabilidade";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }
        public override string GetSql()
        {            
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("EXECUTE BLOCK AS DECLARE VARIABLE GENE_ID INTEGER; ");
            sql.AppendLine("BEGIN");

            sql.AppendLine("UPDATE OR INSERT INTO TB_MOTIVO_INVIABILIDADE (ID, MOTIVO, STATUS, USUARIOCRIACAO, DATAHORACRIACAO, TOKEN) VALUES(1, 'Caixas totalmente ocupadas', 1, 'ADMIN', '2023-06-01 18:18:00', NULL) MATCHING(ID);");            
            sql.AppendLine("UPDATE OR INSERT INTO TB_MOTIVO_INVIABILIDADE (ID, MOTIVO, STATUS, USUARIOCRIACAO, DATAHORACRIACAO, TOKEN) VALUES(2, 'Caixas não instaladas', 1, 'ADMIN', '2023-06-01 18:21:22', NULL) MATCHING(ID);");
            sql.AppendLine("UPDATE OR INSERT INTO TB_MOTIVO_INVIABILIDADE (ID, MOTIVO, STATUS, USUARIOCRIACAO, DATAHORACRIACAO, TOKEN) VALUES(3, 'Cliente fora da área de cobertura', 1, 'ADMIN', '2023-06-01 18:20:37', NULL) MATCHING(ID);");
            sql.AppendLine(" SELECT COALESCE(MAX(ID), 0) FROM     TB_MOTIVO_INVIABILIDADE     INTO GENE_ID;  EXECUTE STATEMENT 'SET GENERATOR     GN_MOTIVO_INVIABILIDADE     TO ' || GENE_ID;");

            sql.AppendLine("END");

            return sql.ToString();                
        }
    }
    
}
