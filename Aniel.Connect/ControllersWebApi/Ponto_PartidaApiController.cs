using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
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
    public class Ponto_PartidaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Ponto_Partidaapi/GetAll")]        
        public async Task<DataTableResponse<Ponto_PartidaListaModel>> GetAll(Ponto_Partida_DataTableRequest request)
        {
            List<Ponto_PartidaListaModel> retorno = new List<Ponto_PartidaListaModel>();
            IOrderedEnumerable<Ponto_PartidaListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_Ponto_Partida ");

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

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.CEP, A.LOGRADOURO, A.NUMERO, A.BAIRRO, A.CIDADE, A.COMPLEMENTO ");                    
                    sql.AppendLine("  FROM TB_PONTO_PARTIDA A ");

                    if (!String.IsNullOrEmpty(request.Id_Equipe))
                    {
                        sql.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                        sql.AppendLine(" ON (A.ID = B.ID_PONTO_PARTIDA)  ");
                    }
                    sql.AppendLine(" WHERE A.ID > 0  ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND (A.CEP LIKE @SEARCH) OR (UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.Descricao))
                    {
                        sql.AppendLine(" AND A.DESCRICAO LIKE @DESCRICAO  ");
                        parametros.Add("@DESCRICAO", String.Format("%{0}%", request.Descricao)  );
                    }

                    if (!String.IsNullOrEmpty(request.Estado))
                    {

                        sql.AppendLine(" AND (UPPER(A.UF) = UPPER(@ESTADO))");
                        parametros.Add("@ESTADO", request.Estado);
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {

                        sql.AppendLine(" AND (UPPER(A.CIDADE) = UPPER(@CIDADE))");
                        parametros.Add("@CIDADE", request.Cidade);
                    }

                    if (!String.IsNullOrEmpty(request.Bairro) && request.Bairro != "Todos")
                    {

                        sql.AppendLine(" AND (UPPER(A.BAIRRO) = UPPER(@BAIRRO))");
                        parametros.Add("@BAIRRO", request.Bairro);
                    }

                    if (!String.IsNullOrEmpty(request.Id_Equipe))
                    {
                        sql.AppendLine(" AND (UPPER(B.EQUIPE) = @EQUIPE)");
                        parametros.Add("@EQUIPE", request.Id_Equipe);
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Ponto_PartidaListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Cep = Convert.ToString(reader["CEP"]),
                                Logradouro = Convert.ToString(reader["LOGRADOURO"]),
                                Numero = Convert.ToString(reader["NUMERO"]),
                                Bairro = Convert.ToString(reader["BAIRRO"]),
                                Cidade = Convert.ToString(reader["CIDADE"]),
                                Complemento = Convert.ToString(reader["COMPLEMENTO"]),
                                                                                                
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
                    case 1:     // VENDEDOR
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
            return new DataTableResponse<Ponto_PartidaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private Ponto_PartidaCadastroModel GetViewObj(Ponto_Partida obj)
        {            
            var retorno = new Ponto_PartidaCadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                Cep = obj.Cep,
                Logradouro = obj.Logradouro,
                Numero = obj.Numero,
                Id_Bairro = obj.Id_Bairro,
                Bairro = obj.Bairro,
                Id_Cidade = obj.Id_Cidade,
                Cidade = obj.Cidade,
                Complemento = obj.Complemento,
                Latitude = Convert.ToString(obj?.Latitude),
                Longitude = Convert.ToString(obj?.Longitude),
                UF = String.IsNullOrEmpty(obj.Uf) ? null : (UF?)Enum.Parse(typeof(UF), obj.Uf),
                Equipes = (obj.Equipes.Count() == 0) ? new List<Equipe_Ponto_PartidaListaModel>() :
                                                   obj.Equipes.Select(p => new Equipe_Ponto_PartidaListaModel
                                                   {
                                                       Id = p.Matricula,
                                                       Id_Equipe = p.Matricula ?? "0",
                                                       Descricao = p.Nome ?? String.Empty,
                                                   }).ToList()
                //Vendedor = obj.Id_Vendedor.HasValue ? new BuscaGenericaRepository<Funcionario>(session, null).FindById(view.Id_Vendedor.Value).Result : null;
            };
          
            return retorno;
        }

        private async Task BindViewToObj(ISession session, Ponto_Partida objBanco, Ponto_PartidaCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Descricao = view.Descricao;
            
            objBanco.Cep = view.Cep;

            objBanco.Logradouro = view.Logradouro;
            objBanco.Numero = view.Numero;
            objBanco.Id_Bairro = view.Id_Bairro;
            objBanco.Bairro = view.Bairro;
            objBanco.Id_Cidade = view.Id_Cidade;
            objBanco.Cidade = view.Cidade;
            objBanco.Complemento = view.Complemento;
            objBanco.Uf = (view.UF.HasValue ? HtmlEnumExtensions.GetEnumDescription<UF>(view.UF.Value) : null);

            objBanco.Latitude = view?.Latitude != null ? Convert.ToDouble(view?.Latitude?.Replace('.', ',')) : (Double?)null;
            objBanco.Longitude = view?.Longitude != null ? Convert.ToDouble(view?.Longitude?.Replace('.', ',')) : (Double?)null;

            using (var repoEquipeAniel = new Conexao<EquipeAniel>(session))
            {
                List<EquipeAniel> equipesTmp = objBanco.Equipes.ToList();
                foreach (var equipe in view.Equipes.FindAll(p => p.Id == "0" || p.Id == String.Empty))
                {
                    EquipeAniel equipeAniel = await repoEquipeAniel.selectFirst(p => p.Matricula == equipe.Id_Equipe);
                    equipeAniel.Ponto_Partida = objBanco;
                    equipesTmp.Add(equipeAniel);
                }
                objBanco.Equipes = equipesTmp;
            }


        }


        [HttpPost()]
        [Route("api/Ponto_Partidaapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_Ponto_Partida SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [HttpPost()]
        [Route("api/Ponto_Partidaapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_Ponto_Partida WHERE ID IN {_ids}");

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
        [Route("api/Ponto_PartidaApi/FindById")]
        public async Task<GetApiRetorno<Ponto_PartidaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Ponto_PartidaCadastroModel> retorno = new GetApiRetorno<Ponto_PartidaCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Ponto_Partida>(session))
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


        //[HttpGet()]
        //[Route("api/Ponto_PartidaApi/Gravar")]
        //public async Task<PostApiRetorno<Int64>> Gravar(Int64? Id)
        //{
        //    PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

        //    try
        //    {
        //        Ponto_Partida modelo = new Ponto_Partida();

        //        using (ISession session = HibernateUtil.GetSession())
        //        {
        //            using (var repository = new Conexao<Ponto_Partida>(session))
        //            {

        //                //modelo.Numero = (await repository.Max(p => p.Numero)) + 1;
        //                //modelo.Status = Status_Atendimento.A_Iniciar;
        //                modelo = Id.HasValue ? new BuscaGenericaRepository<Ponto_Partida>(session, null).FindById(Id.Value).Result : null;

        //                repository.SaveOrUpdate(modelo);
        //            }
                    
        //            retorno.Ok = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.IdObjeto = 0;
        //        retorno.Mensagem = ex.Message;
        //        if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
        //    }

        //    return retorno;
        //}


        [HttpPost()]
        //[Route("api/Ponto_PartidaApi/Gravar")]
        [Route("api/Ponto_PartidaApi/Gravar")]        
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Ponto_PartidaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Ponto_Partida modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Ponto_Partida>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Ponto_Partida
                            {
                                //Status = Status_Atendimento.A_Iniciar
                                UsuarioCriacao = value.Usuario
                            };
                            
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);


                        await repository.SaveOrUpdate(modelo);
                    }
                    
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

        [Route("api/Ponto_PartidaApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Ponto_Partida>(session))
                    {

                        Ponto_Partida modelo = null;
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


        [HttpGet()]
        [Route("api/Ponto_PartidaApi/busca")]
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOME");
                        sql.AppendLine("FROM TB_FUNCIONARIO A");
                        if (term.Length > 4)
                            sql.AppendLine("WHERE (UPPER(A.NOME) LIKE @VALOR)");
                        else
                            sql.AppendLine("WHERE ((UPPER(A.NUM_FRE) LIKE @VALOR) OR (UPPER(A.NOME) LIKE @VALOR))");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        //using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
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
