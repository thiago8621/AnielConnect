using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.Models;
using System.Web.Http;
using System.Text;
using NHibernate;
using Sinapse.DAO.repository;
using System.Threading.Tasks;

namespace Aniel.Connect.ControllersWebApi
{
    public class ImportacaoOsApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/ImportacaoOsApi/GetCamposImportacao")]
        public async Task<GetApiRetorno<List<ImportacaoOsCampo>>> GetCamposImportacao(ImportacaoCampoRequest request)
        {
            GetApiRetorno<List<ImportacaoOsCampo>> retorno = new GetApiRetorno<List<ImportacaoOsCampo>>
            {
                Objeto = new List<ImportacaoOsCampo>()
            };

            retorno.Objeto.Add(new ImportacaoOsCampo { CampoAniel = "NUM_OBRA" , Chave = true });
            retorno.Objeto.Add(new ImportacaoOsCampo { CampoAniel = "NUM_DOC" , Chave = true });
            retorno.Objeto.Add(new ImportacaoOsCampo { CampoAniel = "PROJETO" , Chave = true });
            retorno.Objeto.Add(new ImportacaoOsCampo { CampoAniel = "CODCT", Chave = true });

            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT A.RDB$FIELD_NAME AS CAMPO ");
                sql.AppendLine("FROM RDB$RELATION_FIELDS A ");
                sql.AppendLine("WHERE A.RDB$RELATION_NAME = 'TB_DOCUMENTO_PRODUCAO' and A.RDB$SYSTEM_FLAG = 0 ");
                sql.AppendLine("AND A.RDB$FIELD_NAME <> 'NUM_OBRA' AND A.RDB$FIELD_NAME <> 'NUM_DOC' AND A.RDB$FIELD_NAME <> 'PROJETO' ");
                sql.AppendLine("AND A.RDB$FIELD_NAME <> 'CODCT' ");
                sql.AppendLine("ORDER BY A.RDB$FIELD_NAME ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Objeto.Add(new ImportacaoOsCampo { CampoAniel  = reader["CAMPO"].ToString(), Chave = false });
                    }
                }
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/ImportacaoOsApi/SalvaConfiguracao")]
        public void SalvaConfiguracao(ConfiguracaoSaveRequest request)
        {

        }

    }

    public class ImportacaoCampoRequest
    {
        public string Projeto { get; set; }
        public string Contrato { get; set; }
    }
    
    public class ConfiguracaoSaveRequest
    {
        public string Projeto { get; set; }
        public string Contrato { get; set; }
        public string[][] Campos { get; set; }
    }
}