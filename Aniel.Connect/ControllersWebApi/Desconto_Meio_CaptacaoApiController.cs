using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Desconto_Meio_CaptacaoApiController : BaseApiController
    {
        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<DescontoMeioCaptacaoListModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<DescontoMeioCaptacaoListModel>> retorno = new GetApiRetorno<IEnumerable<DescontoMeioCaptacaoListModel>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    var lista = await new Conexao<Desconto_Meio_Captacao>(session).queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new DescontoMeioCaptacaoListModel
                    {
                        Id_Desconto = p.Desconto.Id,
                        Id_Meio_Captacao = p.Meio_Captacao.Id
                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [HttpGet()]
        [Route("api/DescontoMeioCaptacaoApi/GetDescontoCaptacao")]
        public async Task<GetApiRetorno<DescontoMeioCaptacao>> GetDescontoCaptacao(Int64 Id_Meio_Captacao)
        {
            GetApiRetorno<DescontoMeioCaptacao> retorno = new GetApiRetorno<DescontoMeioCaptacao>();
            retorno.Objeto = new DescontoMeioCaptacao();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        //sql.AppendLine(" SELECT COUNT(A.ID) CO ");
                        sql.AppendLine(" SELECT C.DESCONTO_CAPTACAO, C.ID AS ID_DESCONTO, C.DESCRICAO, C.VALOR_DESCONTO, ");
                        sql.AppendLine(" C.PERCENTUAL_DESCONTO , D.DESCRICAO AS TIPO_DESCONTO ");
                        sql.AppendLine(" FROM TB_MEIO_CAPTACAO A ");

                        sql.AppendLine(" LEFT JOIN TB_DESCONTO_MEIO_CAPTACAO B ");
                        sql.AppendLine(" ON(B.ID_MEIO_CAPTACAO = A.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_DESCONTO C ");
                        sql.AppendLine(" ON(B.ID_DESCONTO = C.ID) ");
                        sql.AppendLine(" INNER JOIN TB_TIPO_DESCONTO D");
                        sql.AppendLine(" ON (C.ID_TIPO_DESCONTO = D.ID)");

                        sql.AppendLine(" WHERE A.ID = @MEIO_CAPTACAO ");
                        //sql.AppendLine(" AND C.DESCONTO_CAPTACAO IN(1,2) ");

                        parametros.Add("@MEIO_CAPTACAO", Id_Meio_Captacao);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.Objeto = new DescontoMeioCaptacao
                                {
                                    desconto_Captacao = (Desconto_Captacao)reader["DESCONTO_CAPTACAO"],
                                    Id = Convert.ToInt64(reader["ID_DESCONTO"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Valor_Desconto = reader["VALOR_DESCONTO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_DESCONTO"]).ToString() : null,
                                    Percentual_Desconto = reader["PERCENTUAL_DESCONTO"] != DBNull.Value ? Convert.ToDouble(reader["PERCENTUAL_DESCONTO"]).ToString() : null,
                                    Tipo_Desconto = Convert.ToString(reader["TIPO_DESCONTO"])
                                };
                            }
                        }
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null) 
                        retorno.InnerException += ex.InnerException.Message;
                    
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        [HttpGet()]
        [Route("api/DescontoMeioCaptacaoApi/BuscaDescontoByMeio")]
        public async Task<GetApiRetorno<List<DescontoMeioCaptacaoListModel>>> BuscaDescontoByMeio(Int64 Id_Meio_Captacao, Int64 Id_Cliente)
        {
            GetApiRetorno<List<DescontoMeioCaptacaoListModel>> retorno = new GetApiRetorno<List<DescontoMeioCaptacaoListModel>>();
            retorno.Objeto = new List<DescontoMeioCaptacaoListModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT D.ID_DESCONTO, A.ID AS ID_MEIO_CAPTACAO ");

                        sql.AppendLine(" FROM TB_MEIO_CAPTACAO A ");

                        sql.AppendLine(" LEFT JOIN TB_DESCONTO_MEIO_CAPTACAO B ");
                        sql.AppendLine(" ON(B.ID_MEIO_CAPTACAO = A.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_DESCONTO C ");
                        sql.AppendLine(" ON(B.ID_DESCONTO = C.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_CLIENTE_DESCONTO D ");
                        sql.AppendLine(" ON(D.ID_DESCONTO = C.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_CLIENTE E ");
                        sql.AppendLine(" ON(D.ID_CLIENTE = E.ID) ");

                        sql.AppendLine(" WHERE A.ID = @MEIO_CAPTACAO ");
                        sql.AppendLine(" AND D.ID_CLIENTE = @CLIENTE ");

                        parametros.Add("@MEIO_CAPTACAO", Id_Meio_Captacao);
                        parametros.Add("@CLIENTE", Id_Cliente);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new DescontoMeioCaptacaoListModel
                                {
                                    Id_Desconto = Convert.ToInt64(reader["ID_DESCONTO"]),
                                    Id_Meio_Captacao = Convert.ToInt64(reader["ID_MEIO_CAPTACAO"]),
                                    //Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"])
                                });
                            }
                        }
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;

        }
    }
}
