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
    public class SLA_ExecucaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/SLA_Execucaoapi/GetAll")]
        public async Task<DataTableResponse<SLA_ExecucaoListaModel>> GetAll(SLA_Execucao_DataTableRequest request)
        {
            List<SLA_ExecucaoListaModel> retorno = new List<SLA_ExecucaoListaModel>();
            IOrderedEnumerable<SLA_ExecucaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.MINUTOS, B.NOME AS TIPO_ATENDIMENTO, ");
                    sql.AppendLine(" C.DESCRICAO AS CATEGORIA_CLIENTE ");
                    sql.AppendLine("FROM TB_SLA_EXECUCAO A ");

                    sql.AppendLine("LEFT JOIN TB_TIPO_ATENDIMENTO B ");
                    sql.AppendLine("ON (A.ID_TIPO_ATENDIMENTO = B.ID) ");

                    sql.AppendLine("LEFT JOIN TB_CATEGORIA_CLIENTE C ");
                    sql.AppendLine("ON (A.ID_CATEGORIA_CLIENTE = C.ID) ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND (UPPER(B.NOME) LIKE UPPER(@SEARCH))");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new SLA_ExecucaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Tipo_Atendimento = Convert.ToString(reader["TIPO_ATENDIMENTO"]),
                                Categoria_Cliente = Convert.ToString(reader["CATEGORIA_CLIENTE"]),
                                Minutos = reader["MINUTOS"] == DBNull.Value ? "00:00" : IntService.ToTimeString(Convert.ToInt32(reader["MINUTOS"]))
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Tipo_Atendimento
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Tipo_Atendimento);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tipo_Atendimento);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<SLA_ExecucaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private SLA_ExecucaoCadastroModel GetViewObj(SLA_Execucao obj)
        {
            return new SLA_ExecucaoCadastroModel
            {
                Id = obj.Id,
                Id_Tipo_Atendimento = obj.Tipo_Atendimento.Id,
                Tipo_Atendimento_Desc = obj.Tipo_Atendimento.Nome,
                Id_Categoria_Cliente = obj.Categoria_Cliente.Id,
                Categoria_Cliente_Desc = obj.Categoria_Cliente.Descricao,
                Horas = IntService.ToTimeString(obj.Minutos),
                Calendario_Padrao = obj.Calendario_Padrao,

                Domingo_Entrada = obj?.Domingo_Entrada?.ToString("HH:mm"),
                Domingo_Saida = obj?.Domingo_Saida?.ToString("HH:mm"),
                Segunda_Entrada = obj?.Segunda_Entrada?.ToString("HH:mm"),
                Segunda_Saida = obj?.Segunda_Saida?.ToString("HH:mm"),
                Terca_Entrada = obj?.Terca_Entrada?.ToString("HH:mm"),
                Terca_Saida = obj?.Terca_Saida?.ToString("HH:mm"),
                Quarta_Entrada = obj?.Quarta_Entrada?.ToString("HH:mm"),
                Quarta_Saida = obj?.Quarta_Saida?.ToString("HH:mm"),
                Quinta_Entrada = obj?.Quinta_Entrada?.ToString("HH:mm"),
                Quinta_Saida = obj?.Quinta_Saida?.ToString("HH:mm"),
                Sexta_Entrada = obj?.Sexta_Entrada?.ToString("HH:mm"),
                Sexta_Saida = obj?.Sexta_Saida?.ToString("HH:mm"),
                Sabado_Entrada = obj?.Sabado_Entrada?.ToString("HH:mm"),
                Sabado_Saida = obj?.Sabado_Saida?.ToString("HH:mm"),
            };
        }

        private void BindViewToObj(ISession session, ref SLA_Execucao objBanco, SLA_ExecucaoCadastroModel view)
        {
            objBanco.Id = view.Id;
            using (var repoTipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
                objBanco.Tipo_Atendimento = repoTipo_Atendimento.selectFirst(p => p.Id == view.Id_Tipo_Atendimento).Result;
            using (var repoCategoria_Cliente = new Conexao<Categoria_Cliente>(session))
                objBanco.Categoria_Cliente = repoCategoria_Cliente.selectFirst(p => p.Id == view.Id_Categoria_Cliente).Result;
            objBanco.Minutos = IntService.GetMinutos(view.Horas);
            objBanco.Calendario_Padrao = view.Calendario_Padrao;

            if (objBanco.Calendario_Padrao) { 

                objBanco.Domingo_Entrada = (DateTime?)null;
                objBanco.Domingo_Saida = (DateTime?)null;
                objBanco.Segunda_Entrada = (DateTime?)null;
                objBanco.Segunda_Saida = (DateTime?)null;
                objBanco.Terca_Entrada = (DateTime?)null;
                objBanco.Terca_Saida = (DateTime?)null;
                objBanco.Quarta_Entrada = (DateTime?)null;
                objBanco.Quarta_Saida = (DateTime?)null;
                objBanco.Quinta_Entrada = (DateTime?)null;
                objBanco.Quinta_Saida = (DateTime?)null;
                objBanco.Sexta_Entrada =  (DateTime?)null;
                objBanco.Sexta_Saida =  (DateTime?)null;
                objBanco.Sabado_Entrada =  (DateTime?)null;
                objBanco.Sabado_Saida = (DateTime?)null;
                
            }
            else {

                objBanco.Domingo_Entrada = !String.IsNullOrEmpty(view.Domingo_Entrada) ? Convert.ToDateTime(view.Domingo_Entrada) : (DateTime?)null;
                objBanco.Domingo_Saida = !String.IsNullOrEmpty(view.Domingo_Saida) ? Convert.ToDateTime(view.Domingo_Saida) : (DateTime?)null;
                objBanco.Segunda_Entrada = !String.IsNullOrEmpty(view.Segunda_Entrada) ? Convert.ToDateTime(view.Segunda_Entrada) : (DateTime?)null;
                objBanco.Segunda_Saida = !String.IsNullOrEmpty(view.Segunda_Saida) ? Convert.ToDateTime(view.Segunda_Saida) : (DateTime?)null;
                objBanco.Terca_Entrada = !String.IsNullOrEmpty(view.Terca_Entrada) ? Convert.ToDateTime(view.Terca_Entrada) : (DateTime?)null;
                objBanco.Terca_Saida = !String.IsNullOrEmpty(view.Terca_Saida) ? Convert.ToDateTime(view.Terca_Saida) : (DateTime?)null;
                objBanco.Quarta_Entrada = !String.IsNullOrEmpty(view.Quarta_Entrada) ? Convert.ToDateTime(view.Quarta_Entrada) : (DateTime?)null;
                objBanco.Quarta_Saida = !String.IsNullOrEmpty(view.Quarta_Saida) ? Convert.ToDateTime(view.Quarta_Saida) : (DateTime?)null;
                objBanco.Quinta_Entrada = !String.IsNullOrEmpty(view.Quinta_Entrada) ? Convert.ToDateTime(view.Quinta_Entrada) : (DateTime?)null;
                objBanco.Quinta_Saida = !String.IsNullOrEmpty(view.Quinta_Saida) ? Convert.ToDateTime(view.Quinta_Saida) : (DateTime?)null;
                objBanco.Sexta_Entrada = !String.IsNullOrEmpty(view.Sexta_Entrada) ? Convert.ToDateTime(view.Sexta_Entrada) : (DateTime?)null;
                objBanco.Sexta_Saida = !String.IsNullOrEmpty(view.Sexta_Saida) ? Convert.ToDateTime(view.Sexta_Saida) : (DateTime?)null;
                objBanco.Sabado_Entrada = !String.IsNullOrEmpty(view.Sabado_Entrada) ? Convert.ToDateTime(view.Sabado_Entrada) : (DateTime?)null;
                objBanco.Sabado_Saida = !String.IsNullOrEmpty(view.Sabado_Saida) ? Convert.ToDateTime(view.Sabado_Saida) : (DateTime?)null;
            }

            objBanco.Tipo_Calendario = Tipo_Calendario.SLA_Execucao;
        }

        [HttpPost()]
        [Route("api/SLA_Execucaoapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_SLA_Execucao WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/SLA_ExecucaoApi/FindById")]
        public async Task<GetApiRetorno<SLA_ExecucaoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<SLA_ExecucaoCadastroModel> retorno = new GetApiRetorno<SLA_ExecucaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<SLA_Execucao>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));
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
        [Route("api/SLA_ExecucaoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<SLA_ExecucaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                SLA_Execucao modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<SLA_Execucao>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new SLA_Execucao
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Id;
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

        [Route("api/SLA_ExecucaoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<SLA_Execucao>(session))
                    {

                        SLA_Execucao modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
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
    }
}
