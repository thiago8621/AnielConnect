using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Usuario;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class ProjetoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Projetoapi/GetCombo")]
        public async Task<ObjCombo<string>> GetComboProjeto(RequestObjComboPaiConfig<string, string, PerfilVisualizacaoServicoComboConfig> request)
        {
            var retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                //Filtros
                var parametros = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder("FROM TB_PROJETO A WHERE ");

                var condicoes = new List<string>();

                if (request.soativos)
                {
                    condicoes.Add($"SITUACAO = 'A'");
                }

                if (!String.IsNullOrEmpty(request.id))
                {
                    //decodificando caracteres especiais 'Ç' 'Ã' etc
                    string decodedId = HttpUtility.HtmlDecode(request.id);
                    condicoes.Add($"UPPER(A.PROJETO) = '{decodedId.ToUpperInvariant()}'");
                }

                if (!String.IsNullOrEmpty(request.idpai))
                {
                    condicoes.Add($"UPPER(A.CODCT) = '{StringService.Truncate(request.idpai.ToUpperInvariant(), 4)}'");
                }

                if (request != null && !String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add($"UPPER(A.PROJETO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 20)}%'");
                }

                #region Filtro do Perfil de Visualização de Serviço

                if (request.config != null && request.config.UsaPerfilVisualizacaoServico)
                {
                    using (ISession session = HibernateUtil.GetSession())
                    {
                        IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                        var businessUsuario = new UsuarioBusiness(repoUsuario);

                        IdentityUser user = businessUsuario.GetById(User.Identity.GetUserId());

                        if (user.PerfilVisualizacaoServico != null &&
                            user.PerfilVisualizacaoServico
                                .ParamsContratoProjeto
                                .Where(p => !String.IsNullOrEmpty(p.Projeto))
                                .Count() > 0)
                        {
                            var listParamsProjeto = new List<string>();

                            foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto.Where(p => !String.IsNullOrEmpty(p.Projeto)))
                            {
                                listParamsProjeto.Add($"A.PROJETO = '{paramContratoProj.Projeto}'");
                            }

                            condicoes.Add($"({String.Join(" OR ", listParamsProjeto)})");
                        }
                    }
                }

                #endregion

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                }

                using (var com = sessionT5.Connection.CreateCommand())
                {
                    var sql = new StringBuilder("SELECT COUNT(A.PROJETO) AS CO ");
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
                    var sql = new StringBuilder("SELECT A.PROJETO ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine("ORDER BY UPPER(A.PROJETO)");

                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessionT5.Connection, null, request.page))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ObjComboResult<string>
                            {
                                Id = Convert.ToString(reader["PROJETO"]),
                                Text = $"{Convert.ToString(reader["PROJETO"])}"
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