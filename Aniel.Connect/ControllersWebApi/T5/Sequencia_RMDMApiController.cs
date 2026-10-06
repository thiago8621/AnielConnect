using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Sequencia_RMDMApiController : BaseApiController
    {
        public async Task<Boolean> CriticaCplProgramadoDM(ISession session, String CodCt, String Equipe, String CodMat, String CodCpl)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();


            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(@"SELECT FIRST 1 A.PROJETO, A.SEQRMDM FROM TB_MOVIMENTO_RMDM A
                                        INNER JOIN TB_SEQUENCIA_RMDM B 
                                            ON (A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.SEQRMDM = B.SEQRMDM AND B.BAIXADA = 'N' AND B.TIPO_MOVIMENTO = 'D')
                                        WHERE A.CODCT = @CODCT AND B.EQUIPE = @EQUIPE AND A.CODMAT = @CODMAT AND A.CODCPL = @CODCPL AND COALESCE(A.CANCELADO, 'N') = 'N'");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@EQUIPE", Equipe);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        throw new Exception(String.Format(RsCadastros.EX_DEVOLUCAO_MATERIAL, reader["PROJETO"], reader["SEQRMDM"]));
                    }
                    return false;
                }
            }
        }
        
    }
}