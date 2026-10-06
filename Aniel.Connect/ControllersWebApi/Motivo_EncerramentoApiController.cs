using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
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
    public class Motivo_EncerramentoApiController : BaseApiController
    {
        //[HttpPost()]
        //public async Task<DataTableResponse<Motivo_EncerramentoListaModel>> GetAll(Motivo_Encerramento_DataTableRequest request)
        //{
        //    DataTableResponse<Motivo_EncerramentoListaModel> retorno = new DataTableResponse<Motivo_EncerramentoListaModel>();

        //    using (ISession session = HibernateUtil.GetSession())
        //    {
        //        try
        //        {
        //            Motivo_Encerramento Motivo_Encerramento = null;                   
        //            Script_Atendimento script = null;

        //            IQueryOver<Motivo_Encerramento, Motivo_Encerramento> queryCount = new Conexao<Motivo_Encerramento>(session).queryover();

        //            Int64 recordsTotal = queryCount.RowCount();

        //            IQueryOver<Motivo_Encerramento, Motivo_Encerramento> query = new Conexao<Motivo_Encerramento>(session).queryover(() => Motivo_Encerramento);

        //            //Filtrando
        //            if (!String.IsNullOrEmpty(request.search.value))
        //            {
        //                var searchText = String.Format("%{0}%", request.search.value.Replace(" ", "%"));

        //                query.Where(Restrictions.Disjunction().Add(Restrictions.On(() => script.Nome).IsLike(searchText))
        //                                                      .Add(Restrictions.On(() => Motivo_Encerramento.Descricao).IsLike(searchText)));
        //            }

        //            //Ordenacao
        //            if (request.order.Any())
        //            {
        //                int sortColumnIndex = request.order[0].column;
        //                string sortDirection = request.order[0].dir;

        //                switch (sortColumnIndex)
        //                {
        //                    case 1:
        //                        {
        //                            if (sortDirection == "asc")
        //                                query.OrderBy(p => p.Descricao).Asc();
        //                            else
        //                                query.OrderBy(p => p.Descricao).Desc();
        //                            break;
        //                        }
        //                }
        //            }

        //            //Paginação
        //            var list = await query
        //                            .Skip(request.start)
        //                            .Take(request.length)
        //                            .ListAsync();

        //            IEnumerable<Motivo_EncerramentoListaModel> objs = list.Select(s =>
        //                                                new Motivo_EncerramentoListaModel
        //                                                {
        //                                                    Id = s.Id,
        //                                                    Descricao = s.Descricao
        //                                                }
        //                                         );


        //            return new DataTableResponse<Motivo_EncerramentoListaModel>
        //            {
        //                draw = request.draw,
        //                recordsTotal = recordsTotal,
        //                recordsFiltered = Convert.ToInt32(recordsTotal),
        //                data = objs,
        //                error = ""
        //            };

        //        }
        //        catch (Exception ex)
        //        {
        //            retorno.error = ex.Message;
        //            if (ex.InnerException != null) retorno.error += ex.InnerException.Message;
        //        }
        //    }
        //    return retorno;
        //}

        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/motivo_Encerramentoapi/GetAll")]
        public async Task<DataTableResponse<Motivo_EncerramentoListaModel>> GetAll(Motivo_Encerramento_DataTableRequest request)
        {
            List<Motivo_EncerramentoListaModel> retorno = new List<Motivo_EncerramentoListaModel>();
            IOrderedEnumerable<Motivo_EncerramentoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MOTIVO_ENCERRAMENTO WHERE STATUS = 1");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS ");
                    sql.AppendLine("FROM TB_MOTIVO_ENCERRAMENTO A");

                    sql.AppendLine("WHERE A.STATUS = 1 ");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        //sql.AppendLine("AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                        //parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Motivo_EncerramentoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = Convert.ToBoolean(reader["STATUS"]),
                                
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
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Motivo_EncerramentoListaModel>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/Motivo_EncerramentoApi/FindById")]
        public async Task<GetApiRetorno<Motivo_Encerramento>> FindById(Int64 Id)
        {
            GetApiRetorno<Motivo_Encerramento> retorno = new GetApiRetorno<Motivo_Encerramento>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    retorno.Objeto = await new BuscaGenericaRepository<Motivo_Encerramento>(session, null).FindById(Id);
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
        [Route("api/Motivo_EncerramentoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Motivo_EncerramentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Motivo_Encerramento modelo = null;

                //using (ISession session = HibernateUtil.GetSession())
                //{
                //    using (var repository = new BuscaGenericaRepository<Motivo_Encerramento>(session, null))
                //    {
                //        if (value.Objeto.Id != 0)
                //            modelo = await repository.FindById(value.Objeto.Id);

                //        if (modelo == null)
                //        {
                //            modelo = new Motivo_Encerramento();
                //            modelo.Status = true;
                //        }

                //        modelo.Descricao = value.Objeto.Descricao;

                //        if (value.Objeto.Id != 0)
                //            modelo.Status = value.Objeto.Status;

                //        //await repository.InsertOrUpdate(modelo, value);

                //    }
                //    retorno.IdObjeto = modelo.Id;
                //    retorno.Ok = true;
                //}

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Encerramento>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Motivo_Encerramento
                            {
                                Status = true,
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            //modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
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

        private void BindViewToObj(ISession session, ref Motivo_Encerramento objBanco, Motivo_EncerramentoCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Descricao = view.Descricao;
            //if (objBanco.Id != 0)
            //{
            //    objBanco.Status = view.Status;
            //}

            objBanco.Status = objBanco.Id != 0 ? view.Status : objBanco.Status;

        }

        [Route("api/Motivo_EncerramentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Encerramento>(session))
                    {

                        Motivo_Encerramento modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.Resources.NAOENCONTRADO);
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
