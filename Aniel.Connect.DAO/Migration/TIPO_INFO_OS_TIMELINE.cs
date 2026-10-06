using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class TIPO_INFO_OS_TIMELINE : AnielMigration
    {
        public TIPO_INFO_OS_TIMELINE()
        {
            Codigo = "20260508_1514";
            Descricao = "Aumento do tamanho da coluna INFO_OS_TIMELINE";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_CONFIGURACOES_GERAIS ADD INFO_OS_TIMELINE_NEW INTEGER;
                                                    UPDATE TB_CONFIGURACOES_GERAIS SET INFO_OS_TIMELINE_NEW = 0 WHERE INFO_OS_TIMELINE = 'SEGMENTO';
                                                    UPDATE TB_CONFIGURACOES_GERAIS SET INFO_OS_TIMELINE_NEW = 1 WHERE INFO_OS_TIMELINE = 'CATEGORIA_CLIENTE';
                                                    UPDATE TB_CONFIGURACOES_GERAIS SET INFO_OS_TIMELINE_NEW = 2 WHERE INFO_OS_TIMELINE = 'TIPO_SERVICO';
                                                    UPDATE TB_CONFIGURACOES_GERAIS SET INFO_OS_TIMELINE_NEW = 3 WHERE INFO_OS_TIMELINE = 'NATUREZA_SERVICO';
                                                    UPDATE TB_CONFIGURACOES_GERAIS SET INFO_OS_TIMELINE_NEW = 0 WHERE INFO_OS_TIMELINE_NEW IS NULL;
                                                    ALTER TABLE TB_CONFIGURACOES_GERAIS DROP INFO_OS_TIMELINE;
                                                    ALTER TABLE TB_CONFIGURACOES_GERAIS ADD INFO_OS_TIMELINE INTEGER;
                                                    UPDATE TB_CONFIGURACOES_GERAIS 
                                                    SET INFO_OS_TIMELINE = INFO_OS_TIMELINE_NEW;
                                                    ALTER TABLE TB_CONFIGURACOES_GERAIS DROP INFO_OS_TIMELINE_NEW;
                                                    ")
                                                  .ToString();
        }
    }
}
