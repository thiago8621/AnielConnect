using Sinapse.DAO;
using System;
using System.Text;

namespace Aniel.T5.DAO
{
    class AUMENTO_CAMPOS_DOC : AnielMigration
    {
        public AUMENTO_CAMPOS_DOC()
        {
            Codigo = "20240611_1720";
            Descricao = "Aumento dos campos de Bloco e Apartamento";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_DOCUMENTO_PRODUCAO ALTER COLUMN BLOCO TYPE VARCHAR(50); ALTER TABLE TB_DOCUMENTO_PRODUCAO ALTER COLUMN APARTAMENTO TYPE VARCHAR(50);").ToString();
            
        }
    }
}
