using Sinapse.DAO;
using System;
using System.Text;

namespace Aniel.T5.DAO
{
    class CORRECAO_CANAL_COMUNICACAO : AnielMigration
    {
        public CORRECAO_CANAL_COMUNICACAO()
        {
            Codigo = "20251205_1700";
            Descricao = "Correcao da Canal de Comunicacao";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_CANAL_COMUNICACAO ALTER COLUMN IMAGEM TYPE VARCHAR(250);").ToString();            
        }
    }
}
