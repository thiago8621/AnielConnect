using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    public class BlacklistApiController : BaseApiController
    {

        /// <summary>
        /// Retorna todos os registros de Blacklist do banco de dados
        /// </summary>
        [HttpPost()]
        [Route("api/BlacklistApi/GetAll")]
        public async Task<DataTableResponse<BlacklistModelLista>> GetAll(BlacklistDatatableRequest request)
        {
            List<BlacklistModelLista> retorno = new List<BlacklistModelLista>();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.NUMERO_ORIGEM, A.NUMERO_DESTINO, A.OBSERVACAO ");
                sql.AppendLine(" FROM TB_BLACKLIST A ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new BlacklistModelLista
                        {
                            Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                            NumeroOrigem = reader["NUMERO_ORIGEM"] != DBNull.Value ? Convert.ToInt32(reader["NUMERO_ORIGEM"]) : 0,
                            NumeroDestino = reader["NUMERO_DESTINO"] != DBNull.Value ? Convert.ToInt32(reader["NUMERO_DESTINO"]) : 0,
                            Observacao = reader["OBSERVACAO"] != DBNull.Value ? Convert.ToString(reader["OBSERVACAO"]) : "Não Consta"
                        });
                    }
                }
            }
            return new DataTableResponse<BlacklistModelLista>
            {
                draw = request.draw,
                data = retorno,
                error = ""
            };

        }

        /// <summary>
        /// Retorna todos os registros buscados por Id de Blacklist do banco de dados
        /// </summary>
        [HttpGet()]
        [Route("api/BlacklistApi/FindById")]
        public async Task<GetApiRetorno<BlacklistCadastroModel>> FindById(Int64 Id_Plano)
        {
            GetApiRetorno<BlacklistCadastroModel> retorno = new GetApiRetorno<BlacklistCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Blacklist>(session))
            using (var repoplano = new Conexao<Plano>(session))
            {
                try
                {
                    Blacklist param = await repository.selectFirst(p => p.Plano.Id == Id_Plano);

                    if (param == null)
                    {
                        param = new Blacklist();
                    }

                    retorno.Objeto = GetViewObj(param, session);

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

        private BlacklistCadastroModel GetViewObj(Blacklist obj, ISession session)
        {
            using (var repo = new Conexao<Blacklist>(session))
            {
                var retorno = new BlacklistCadastroModel();

                var blacklist = repo.select(p => p.Plano.Id == obj.Plano.Id).Result;
                //var blacklist = repo.select(p => p.Id == obj.Id).Result;

                if (blacklist != null)
                {
                    retorno.Lista_BlackLists = blacklist.Select(o => new BlacklistModelLista
                    {
                        Id = o.Id,
                        NumeroOrigem = o.Numero_Origem,
                        NumeroDestino = o.Numero_Destino,
                        Observacao = o.Observacao
                    }).ToList();
                } 
                else
                {
                    new BlacklistCadastroModel();
                }

                return retorno;
            }
        }

        /// <summary>
        /// Gravar item de Blacklist
        /// </summary>

        [HttpPost()]
        [Route("api/BlacklistApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<BlacklistCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    Blacklist modelo = null;
                    using (var repository = new Conexao<Blacklist>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            modelo = new Blacklist
                            {
                                UsuarioCriacao = value.Usuario,
                                DataHoraCriacao = DateTime.Now                                
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            
                        }
                       
                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }

            }
            return retorno;
        }


        private void BindViewToObj(ISession session, Blacklist objBanco, BlacklistCadastroModel view)
        {
            using (var repo = new Conexao<Blacklist>(session))
            using (var repoPlano = new Conexao<Plano>(session))
            {
                objBanco.Id = view.Id;
                objBanco.Numero_Origem = view.NumeroOrigem;
                objBanco.Numero_Destino = view.NumeroDestino;
                objBanco.Observacao = view.Observacao;
                objBanco.Plano = (Plano)repoPlano.select(p => p.Id == view.IdPlano).Result;
               
            }
        }


        /// <summary>
        /// Exclui item Datatable Blacklist
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("api/BlacklistApi/Excluir")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirCriterios(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Blacklist>(session))
                {

                    Blacklist modelo = null;
                    if (value != 0)
                    {
                        modelo = (await repo.select(p => p.Id == value)).FirstOrDefault();

                        await repo.Remove(modelo);
                        retorno.IdObjeto = modelo.Id;
                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);
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