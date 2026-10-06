using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ConsultaFinanceiraIntegracaoApiController : BaseApiController
    {

        /// <summary>
        /// Endpoint para inserir cpf da base do MkSolution para o connect para verificar pendencias internas 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ConsultaFinanceiraIntegracaoApi/GravarPendenciaFinanceira")]
        public async Task<PostApiRetorno> InsertPendenciaFinanceira(ObjApiRequest<PendenciaFinanceiraApiRequestModels> request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    ObjApiRequest<PendenciaFinanceiraApiRequestModels> request1 = request;
                    FillDictionary(request.Obj, parametros);

                    parametros = parametros.Where(p => p.Value != null).ToDictionary(p => p.Key, p => p.Value);
                    List<String> arguments = parametros.Keys.Select(p => { return p.Replace("@", ""); }).ToList();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"UPDATE OR INSERT INTO TB_CONSULTA_PENDENCIA_FIN ({String.Join(",", arguments)})")
                       .AppendLine($" VALUES({String.Join(",", parametros.Keys)}) ");

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                }
                retorno.Ok = true;
                retorno.Mensagem = "Operação executada com êxito.";
            }
            catch (Exception erro)
            {
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Ok = false;
            }
            return retorno;

        }
        private void FillDictionary(PendenciaFinanceiraApiRequestModels obj, Dictionary<String, Object> dict)
        {
            dict.Add("@COD_CLIENTE", StringService.Truncate($"{Convert.ToInt64(obj?.Cod_Cliente)}", 5));
            dict.Add("@COD_CONTRATO", StringService.Truncate($"{Convert.ToInt64(obj?.Cod_Contrato)}", 5));
            dict.Add("@COD_CONEXAO", StringService.Truncate($"{Convert.ToInt64(obj?.CodConexao)}", 5));
            dict.Add("@CLIENTE", StringService.Truncate($"{Convert.ToInt64(obj?.Cod_Cliente)}", 5));
            dict.Add("@CPF", StringService.Truncate(obj?.Cpf.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty), 18));
            dict.Add("@CNPJ", StringService.Truncate(obj?.Cnpj.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty), 18));
            dict.Add("@CONTRATO_CANCELADO", StringService.Truncate(obj?.Contrato_Cancelado, 150));
            dict.Add("@CONEXAO_BLOQUEADA", StringService.Truncate(obj?.Conexao_Bloqueada, 25));

            dict.Add("@LOGRADOURO", StringService.Truncate(obj?.Logradouro, 50));
            dict.Add("@NUMERO", StringService.Truncate(obj?.Numero, 50));
            dict.Add("@BAIRRO", StringService.Truncate(obj?.Bairro, 255));

            dict.Add("@CIDADE", StringService.Truncate(obj?.Cidade, 25));
            dict.Add("@COD_FATURA", StringService.Truncate(obj?.CodFatura, 18));
            dict.Add("@VALOR_TOTAL", StringService.Truncate(obj?.Valor_Total, 18));
            dict.Add("@LIQUIDADO", StringService.Truncate(obj?.Liquidado, 100));
            dict.Add("@DESCRICAO", obj?.Descricao);
            dict.Add("@FATURA_SUSPENSA", obj?.Fatura_Suspensa);
            dict.Add("@VENCIMENTO", obj?.Data_Vencimento);
            dict.Add("@MOTIVO_SUSPENSAO", obj?.Motivo_Suspensao);
        }        
    }
}
