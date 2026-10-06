using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Usuario;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class ContratoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Contratoapi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboConfig<string, PerfilVisualizacaoServicoComboConfig> request)
        {
            var retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                //Filtros 
                var parametros = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder("FROM TB_CONTRATO A WHERE ");

                var condicoes = new List<String>();

                if (request.soativos)
                {
                    condicoes.Add($"TERMREAL IS NULL");
                }

                if (!String.IsNullOrEmpty(request.id))
                {
                    condicoes.Add($"UPPER(A.CODCT) = '{request.id.ToUpperInvariant()}'");
                }

                if (request != null && !String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add($"( UPPER(A.CODCT) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 4)}%' OR " +
                                  $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                }

                #region Filtro do Perfil de Visualização de Serviço

                if (request.config != null && request.config.UsaPerfilVisualizacaoServico)
                {
                    using (ISession session = HibernateUtil.GetSession())
                    {
                        IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                        var businessUsuario = new UsuarioBusiness(repoUsuario);

                        IdentityUser user = businessUsuario.GetById(User.Identity.GetUserId());

                        if (user.PerfilVisualizacaoServico != null && user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                        {
                            var listParamsContrato = new List<string>();

                            foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                            {
                                listParamsContrato.Add($"A.CODCT = '{paramContratoProj.CodCt}'");
                            }

                            condicoes.Add($"({String.Join(" OR ", listParamsContrato)})");
                        }  
                    }
                }

                #endregion

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
                }

                using (var com = sessionT5.Connection.CreateCommand())
                {
                    var sql = new StringBuilder("SELECT COUNT(A.CODCT) AS CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                            retorno.total_count = Convert.ToInt32(reader["CO"]);
                    }
                }

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    var sql = new StringBuilder("SELECT A.CODCT,A.DESCRICAO ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine("ORDER BY UPPER(A.CODCT)");

                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessionT5.Connection, null, request.page))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ObjComboResult<String>
                            {
                                Id = Convert.ToString(reader["CODCT"]),
                                Text = $"{Convert.ToString(reader["CODCT"])} - {Convert.ToString(reader["DESCRICAO"])}"
                            };

                            retorno.results.Add(obj);
                        }
                    }
                }
            }

            return retorno;
        }
    }
}