using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Util;
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
    public class Forma_PagamentoApiController : BaseApiController
    {
        
        [Route("api/Forma_PagamentoApi/GetList")]
        [HttpPost()]
        public async Task<GetApiRetorno<List<Forma_PagamentoCadastroModel>>> GetList()
        {
            GetApiRetorno<List<Forma_PagamentoCadastroModel>> retorno = new GetApiRetorno<List<Forma_PagamentoCadastroModel>>()
            {
                Objeto = new List<Forma_PagamentoCadastroModel>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    var lista = await new Conexao<Forma_Pagamento>(session).queryover().ListAsync();

                    foreach (var obj in lista)
                    {
                        var tmp = new Forma_PagamentoCadastroModel
                        {
                            Id = obj.Id,
                            Descricao = obj.Descricao,
                            Tipo_Forma_Pagamento = obj.Tipo_Forma_Pagamento,
                            Forma_Pagamento_Vencimento = GetVencimentos(obj.Forma_Pagamento_Vencimento),
                            Status = obj.Status,
                            CodigoIntegracao = obj.CodigoIntegracao
                        };

                        if (obj.Forma_Pagamento_Vencimento.Count() > 0)
                            retorno.Objeto.Add(tmp);
                    }

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

        [HttpPost()]
        [Route("api/Forma_PagamentoApi/GetCombo")]
        public async Task<ObjCombo<Int64?>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64?> retorno = new ObjCombo<Int64?>
            {
                results = new List<IObjComboResult<Int64?>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FORMA_PAGAMENTO A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                       condicoes.Add($"A.STATUS = 1");
                    }


                    if (request.idpai.HasValue) {
                        condicoes.Add($"A.TIPO_FORMA_PAGAMENTO = {request.idpai}");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO, A.TIPO_FORMA_PAGAMENTO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Forma_PagamentoComboModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Tipo_Forma_Pagamento = reader["TIPO_FORMA_PAGAMENTO"] != DBNull.Value ? (Tipo_Forma_Pagamento)Enum.Parse(typeof(Tipo_Forma_Pagamento), Convert.ToString(reader["TIPO_FORMA_PAGAMENTO"])) : (Tipo_Forma_Pagamento?)null,
                                    Forma_Pagamento_Vencimento = await GetVencimentos(session, Convert.ToInt64(reader["ID"]))
                                };

                                if (obj.Forma_Pagamento_Vencimento.Count() > 0)
                                    retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        private List<Forma_PagamentoVencimentoCadastroModel> GetVencimentos(IEnumerable<Forma_Pagamento_Vencimento> values)
        {
            var retorno = new List<Forma_PagamentoVencimentoCadastroModel>();

            foreach (var obj in values)
            {
                retorno.Add(new Forma_PagamentoVencimentoCadastroModel
                {
                    Id = obj.Id,
                    IdVencimento = obj.Vencimento.Id,
                    DiaVencimento = obj.Vencimento.DiaVencimento
                });
            }

            return retorno;
        }

        private async Task<List<Forma_PagamentoVencimentoCadastroModel>> GetVencimentos(ISession session, Int64 Id_Forma_Pagamento)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();


            var retorno = new List<Forma_PagamentoVencimentoCadastroModel>();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder("SELECT A.ID, A.ID_VENCIMENTO, B.DIA_VENCIMENTO ")
                    .AppendLine("FROM TB_FORMA_PAGAMENTO_VENCIMENTO A ")
                    .AppendLine("INNER JOIN TB_VENCIMENTO B ")
                    .AppendLine("ON (A.ID_VENCIMENTO = B.ID) ")
                    .AppendLine("WHERE A.ID_FORMA_PAGAMENTO = @ID_FORMA_PAGAMENTO ");
                parametros.Add("@ID_FORMA_PAGAMENTO", Id_Forma_Pagamento);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new Forma_PagamentoVencimentoCadastroModel
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            IdVencimento = reader["ID_VENCIMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_VENCIMENTO"]) : 0,
                            DiaVencimento = reader["DIA_VENCIMENTO"] != DBNull.Value ? Convert.ToInt16(reader["DIA_VENCIMENTO"]) : Convert.ToInt16(0)
                        });
                    }
                }
            }


            return retorno;
        }


        [HttpPost()]
        [Route("api/Forma_PagamentoApi/GetListFormaPagamento")]
        public async Task<GetApiRetorno<List<object>>> GetListFormaPagamento()
        {
            GetApiRetorno<List<object>> retorno = new GetApiRetorno<List<object>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    var lista = await new Conexao<Forma_Pagamento>(session).queryover().ListAsync();

                    retorno.Objeto = new List<object>(lista.Select(s => new
                    {
                        id = s.Id,
                        descricao = s.Descricao
                    }));

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
    }
}