using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System;
using NHibernate;
using System.Linq;

namespace Aniel.Connect.ControllersWebApi
{
    public class SigaMeApiController : BaseApiController
    {


        /// <summary>
        /// Retorna todos os registros de SigaMe do banco de dados
        /// </summary>
        [HttpPost()]
        [Route("api/SigaMeApi/GetAll")]
        public async Task<DataTableResponse<SigaMeListModel>> GetAll(DatatableRequestSigaMe request)
        {
            List<SigaMeListModel> retorno = new List<SigaMeListModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.NUMERO_ORIGEM, A.NUMERO_DESTINO, A.OBSERVACAO ");
                sql.AppendLine(" FROM TB_SIGAME A ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new SigaMeListModel
                        {
                            Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                            NumeroOrigem = reader["NUMERO_ORIGEM"] != DBNull.Value ? Convert.ToInt32(reader["NUMERO_ORIGEM"]) : 0,
                            NumeroDestino = reader["NUMERO_DESTINO"] != DBNull.Value ? Convert.ToInt32(reader["NUMERO_DESTINO"]) : 0,
                            DefinirHorario = reader["DEFINIR_HORARIO"] != DBNull.Value ? Convert.ToBoolean(reader["DEFINIR_HORARIO"]) : false,
                            Segunda = reader["SEGUNDA"] != DBNull.Value ? Convert.ToBoolean(reader["SEGUNDA"]) : false,
                            Terca = reader["TERCA"] != DBNull.Value ? Convert.ToBoolean(reader["TERCA"]) : false,
                            Quarta = reader["QUARTA"] != DBNull.Value ? Convert.ToBoolean(reader["QUARTA"]) : false,
                            Quinta = reader["QUINTA"] != DBNull.Value ? Convert.ToBoolean(reader["QUINTA"]) : false,
                            Sexta = reader["SEXTA"] != DBNull.Value ? Convert.ToBoolean(reader["SEXTA"]) : false,
                            Sabado = reader["SABADO"] != DBNull.Value ? Convert.ToBoolean(reader["SABADO"]) : false,
                            Domingo = reader["DOMINGO"] != DBNull.Value ? Convert.ToBoolean(reader["DOMINGO"]) : false,
                            HorarioInicial = reader["HORARIO_INICIAL"] != DBNull.Value ? Convert.ToDateTime(reader["HORARIO_INICIAL"]).ToString("dd/MM/yyyy") : null,
                            HorarioFinal = reader["HORARIO_FINAL"] != DBNull.Value ? Convert.ToDateTime(reader["HORARIO_FINAL"]).ToString("dd/MM/yyyy") : null,
                         });
                    }
                }
            }
            return new DataTableResponse<SigaMeListModel>
            {
                draw = request.draw,
                data = retorno,
                error = ""
            };

        }

        /// <summary>
        /// Retorna todos os registros buscados por Id de SigaMe do banco de dados
        /// </summary>
        [HttpGet()]
        [Route("api/SigaMeApi/FindById")]
        public async Task<GetApiRetorno<SigaMeCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<SigaMeCadastroModel> retorno = new GetApiRetorno<SigaMeCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<SigaMe>(session))
            using (var repoplano = new Conexao<Plano>(session))
            {
                try
                {
                    SigaMe param = await repository.selectFirst(p => p.Id == Id);

                    if (param == null)
                    {
                        param = new SigaMe();
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

        private SigaMeCadastroModel GetViewObj(SigaMe obj, ISession session)
        {
            using (var repo = new Conexao<SigaMe>(session))
            using (var repoPlano = new Conexao<Plano>(session))
            {
                var retorno = new SigaMeCadastroModel();

                var SigaMe = repo.select(p => p.Plano.Id == obj.Plano.Id).Result;

                if (SigaMe != null)
                {
                    retorno.SigaMeLista = SigaMe.Select(o => new SigaMeListModel
                    {
                        Id = o.Id,
                        NumeroOrigem = o.NumeroOrigem,
                        NumeroDestino = o.NumeroDestino,
                        DefinirHorario = o.DefinirHorario,
                        Segunda = o.Segunda,
                        Terca = o.Terca,
                        Quarta = o.Quarta,
                        Quinta = o.Quinta,
                        Sexta = o.Sexta,
                        Sabado = o.Sabado,
                        Domingo = o.Domingo,
                        HorarioInicial = Convert.ToString(o.HorarioInicial),
                        HorarioFinal = Convert.ToString(o.HorarioFinal),

                    }).ToList();
                }

                return retorno;
            }
        }

        /// <summary>
        /// Gravar item de SigaMe
        /// </summary>

        [HttpPost()]
        [Route("api/SigaMeApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<SigaMeCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    SigaMe modelo = null;
                    using (var repository = new Conexao<SigaMe>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            modelo = new SigaMe
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


        private void BindViewToObj(ISession session, SigaMe objBanco, SigaMeCadastroModel view)
        {
            using (var repoPlano = new Conexao<Plano>(session))
            using (var repo = new Conexao<SigaMe>(session))
            {
                objBanco.Id = view.Id;
                objBanco.NumeroOrigem = view.NumeroOrigem;
                objBanco.NumeroDestino = view.NumeroDestino;
                objBanco.DefinirHorario = view.DefinirHorario;
                objBanco.HorarioInicial = view.HorarioInicial;
                objBanco.HorarioFinal = view.HorarioFinal;
                objBanco.Segunda = view.Segunda;
                objBanco.Terca = view.Terca;
                objBanco.Quarta = view.Quarta;
                objBanco.Quinta = view.Quinta;
                objBanco.Sexta = view.Sexta;
                objBanco.Sabado = view.Sabado;
                objBanco.Domingo = view.Domingo;
           
                objBanco.Plano = repoPlano.selectFirst(p => p.Id == view.idPlano).Result;
            }
        }


        /// <summary>
        /// Exclui item Datatable SigaMe
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("api/SigaMeApi/Excluir")]
        public async Task<DeleteApiRetorno<Int64>> Excluir(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<SigaMe>(session))
                {

                    SigaMe modelo = null;
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