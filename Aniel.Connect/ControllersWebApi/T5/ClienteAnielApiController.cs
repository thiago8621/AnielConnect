using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class ClienteAnielApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/clienteanielapi/Gravar")]
        public async Task<PostApiRetorno<Int32>> Gravar([FromBody]ParamWebApi<ClienteDadosGeraisViewModel> value)
        {
            PostApiRetorno<Int32> retorno = new PostApiRetorno<Int32>();
            try
            {
                ClienteAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<ClienteAniel>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Cod_Cliente == Convert.ToInt32(value.Objeto.Id))).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new ClienteAniel
                            {
                                Cod_Cliente = Convert.ToInt32(value.Objeto.Id),
                                Status = Status_Cliente.Ativo,
                            };

                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Cliente.Ativo : Status_Cliente.Inativo;
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Cod_Cliente;
                    retorno.Ok = true;



                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }


            return retorno;
        }

        private void BindViewToObj(ISession session, ClienteAniel objBanco, ClienteDadosGeraisViewModel view)
        {
            objBanco.Tipo_Pessoa = view.TipoPessoa.Value;
            objBanco.NomeFantasia = objBanco.Tipo_Pessoa == Tipo_Pessoa.Fisica ? view.NomeCompleto : view.NomeFantasia;
            objBanco.Email_Principal = view.Email;
            objBanco.Telefone_Fixo = view.TelefoneFixo;
            objBanco.Telefone_Celular = view.Telefone_Celular;
            objBanco.CPF = !String.IsNullOrEmpty(view.CpfCnpj) ? new String(view.CpfCnpj.Where(Char.IsDigit).ToArray()) : view.CpfCnpj;
            objBanco.Categoria_Cliente = view.Id_Categoria_Cliente.HasValue ? new Conexao<Categoria_ClienteAniel>(session).selectFirst(p => p.Id == view.Id_Categoria_Cliente.Value).Result : null;
            objBanco.Id_Organizacao = view.Id_Organizacao;
        }

        [HttpPost()]
        [Route("api/clienteanielapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CLIENTE A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"COALESCE(A.SITUACAO, 0) = 0");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.RAZAO_SOCIAL) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.RAZAO_SOCIAL != '' AND ( {String.Join(" AND ", condicoes) } ) ");
                    }


                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_CLIENTE) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.COD_CLIENTE, A.RAZAO_SOCIAL ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.RAZAO_SOCIAL) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["RAZAO_SOCIAL"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["COD_CLIENTE"]),
                                        Text = Convert.ToString(reader["RAZAO_SOCIAL"])
                                    });

                                }
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



    }
}