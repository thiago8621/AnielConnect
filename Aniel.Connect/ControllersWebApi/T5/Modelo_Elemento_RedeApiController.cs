using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aniel.Connect.Core.Helpers;
using System.Web.Http;
using System.Threading.Tasks;
using Aniel.Connect.Models.T5;
using NHibernate;
using Sinapse.DAO.repository;
using System.Text;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Util;
using Aniel.T5.Model.enumerado;
using Sinapse.Model;

namespace Aniel.Connect.ControllersWebApi.T5
{
    
    [Authorize]
    public class Modelo_Elemento_RedeApiController : BaseApiController, IModelo_Elemento_RedeApi
    {

        [HttpPost()]
        [Route("api/Modelo_Elemento_RedeApiController/GetAll")]
        #region GetAll
        public async Task<DataTableResponse<Modelo_Elemento_RedeListaModels>> GetAll(Modelo_Elemento_RedeRequest request)
        {
            List<Modelo_Elemento_RedeListaModels> retorno = new List<Modelo_Elemento_RedeListaModels>();
            Int32? RecordsTotal = 0;
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    StringBuilder sqlWhere = new StringBuilder();

                    //Filtros 
                    #region Filtros
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        condicoes.Add("UPPER(A.DESCRICAO) LIKE @SEARCH");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpperInvariant()));
                    }

                    if (request.Status.HasValue)
                    {
                        condicoes.Add("A.STATUS = @STATUS");
                        parametros.Add("@STATUS", request.Status.Value ? 1 : 0);
                    }

                    if (condicoes.Count > 0)
                    {
                        sqlWhere.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
                    }

                    #endregion



                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT (A.ID) CO ")
                            .AppendLine("FROM TB_MODELO_ELEMENTO_REDE A ");

                        sql.AppendLine(sqlWhere.ToString());


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                RecordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT")

                        .AppendLine(" A.ID, A.DESCRICAO,")
                        .AppendLine("A.STATUS")

                        .AppendLine("FROM TB_MODELO_ELEMENTO_REDE A");


                        sql.AppendLine(sqlWhere.ToString());


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Modelo_Elemento_RedeListaModels
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                                });

                            }
                        }
                    }
                    return new DataTableResponse<Modelo_Elemento_RedeListaModels>
                    {
                        draw = request.draw,
                        data = retorno,
                        error = "",
                        recordsFiltered = RecordsTotal
                    };
                }
            }
            catch (Exception e)
            {
                return new DataTableResponse<Modelo_Elemento_RedeListaModels>
                {

                    error = e.Message,

                };
            }
        }
        #endregion


        [HttpPost()]
        [Route("api/Modelo_Elemento_RedeApi/GetCombo")]
        #region GetCombo
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionNH = HibernateUtil.GetSessionT5())
            using (var repositoryColuna = new Conexao<ColunaElementoRede>(sessionNH))
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MODELO_ELEMENTO_REDE A ");


                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                long id = Convert.ToInt64(reader["ID"]);
                                retorno.results.Add(new ModeloElementoRedeComboResult
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Colunas = repositoryColuna.select(p => p.Modelo_Elemento_Rede.Id == id).Result.Select(p => new ColunaModeloRedeListaModel
                                    {
                                        Coluna = p.Coluna,
                                        Obrigatorio = p.Obrigatorio
                                    }).ToList()
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
        #endregion


        [HttpPost()]
        [Route("api/Modelo_Elemento_RedeApi/Gravar")]
        #region Gravar
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Modelo_Elemento_RedeCadastroModel> request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            Modelo_Elemento_Rede modelo = null;

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {

                    using (var repoModelo = new Conexao<Modelo_Elemento_Rede>(sessionT5))
                    using (var repoColuna = new Conexao<ColunaElementoRede>(sessionT5))
                    using (var repoCampoDinamico = new Conexao<Modelo_CampoDinamico>(sessionT5))
                    {
                        if (request.Objeto.Id != 0)
                        {
                            modelo = await repoModelo.selectFirst(p => p.Id == request.Objeto.Id);
                            modelo.Status = request.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }
                        else
                        {
                            modelo = new Modelo_Elemento_Rede();
                            modelo.Status = Status_Default.Ativo;
                            modelo.UsuarioCriacao = request.Usuario;
                        }

                        modelo.Descricao = request.Objeto.Descricao;

                        List<ColunaElementoRede> colunas = request.Objeto.Colunas.Select(p =>
                        {
                            return new ColunaElementoRede
                            {
                                Id = p.Id,
                                Coluna = p.Coluna,
                                Modelo_Elemento_Rede = modelo,
                                UsuarioCriacao = request.Usuario,
                                CampoDinamco = p.IdColunaDinamica != null ? repoCampoDinamico.selectFirst(y => y.Id == p.IdColunaDinamica).Result : null,
                                Obrigatorio = p.Obrigatorio,
                                DataHoraCriacao = DateTime.Now
                            };
                        }).ToList();
                        await repoModelo.Save(modelo);
                        await repoColuna.SaveOrUpdate(colunas);
                    }

                }
                retorno.IdObjeto = modelo.Id;
                retorno.Mensagem = "Salvo com Sucesso";
                retorno.Ok = true;
                return retorno;
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.Ok = false;
                return retorno;
            }
        }
        #endregion


        [HttpGet()]
        [Route("api/Modelo_Elemento_RedeApi/FindById")]
        #region FindById
        public async Task<GetApiRetorno<Modelo_Elemento_RedeCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Modelo_Elemento_RedeCadastroModel> retorno = new GetApiRetorno<Modelo_Elemento_RedeCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Modelo_Elemento_Rede>(session))
            using (var repositoryColuna = new Conexao<ColunaElementoRede>(session))
            using (var repoCampo = new Conexao<Modelo_CampoDinamico>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

                    var colunasBanco = repositoryColuna.select(p => p.Modelo_Elemento_Rede.Id == retorno.Objeto.Id).Result.ToList();

                    List<Coluna_ElementoRedeModel> temp = new List<Coluna_ElementoRedeModel>();
                    foreach (var p in colunasBanco)
                    {
                        var campoDinamico = p.CampoDinamco != null ? repoCampo.selectFirst(y => y.Id == p.CampoDinamco.Id).Result : null;
                        temp.Add(new Coluna_ElementoRedeModel
                        {
                            Id = p.Id,
                            IdColunaDinamica = campoDinamico?.Id,
                            Coluna = p.Coluna,
                            Descricao = campoDinamico == null ? HtmlEnumExtensions.GetEnumDescription<ColunaModeloElementoEnum>((ColunaModeloElementoEnum)p.Coluna) : campoDinamico?.Descricao,
                            Obrigatorio = p.Obrigatorio
                        });
                    }
                    retorno.Objeto.Colunas = temp;

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
        #endregion

        #region GetViewObj
        private Modelo_Elemento_RedeCadastroModel GetViewObj(Modelo_Elemento_Rede modelo)
        {
            return new Modelo_Elemento_RedeCadastroModel
            {
                Id = modelo.Id,
                Descricao = modelo.Descricao,
                Status = modelo.Status == Status_Default.Ativo
            };
        }
        #endregion


        [HttpPost()]
        [Route("api/Tipo_Elemento_RedeApi/Excluir")]
        #region Excluir
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                Modelo_Elemento_Rede Modelo = null;
                List<ColunaElementoRede> Colunas = null;

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repo_Colunas = new Conexao<ColunaElementoRede>(sessionT5))
                    using (var repositorio = new Conexao<Modelo_Elemento_Rede>(sessionT5))
                    {
                        Modelo = await repositorio.selectFirst(p => p.Id == value.Objeto);
                        Colunas = (await repo_Colunas.select(prop => prop.Modelo_Elemento_Rede == Modelo)).ToList();

                        await repo_Colunas.Remove(Colunas);
                        await repositorio.Remove(Modelo);

                        retorno.IdObjeto = Modelo.Id;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro?.InnerException?.Message;
            }
            return retorno;

        }
        #endregion

        [HttpPost()]
        [Route("api/Modelo_Elemento_RedeApi/AlterarStatus")]
        #region AlterarStatus
        public async Task<PostApiRetorno<Modelo_Elemento_RedeListaModels>> AlterarStatus(AlterarStatusViewModel request)
        {
            PostApiRetorno<Modelo_Elemento_RedeListaModels> retorno = new PostApiRetorno<Modelo_Elemento_RedeListaModels>();
            Dictionary<String, object> parametros = new Dictionary<string, Object>();

            try
            {

                StringBuilder sqlUp = new StringBuilder(" UPDATE TB_MODELO_ELEMENTO_REDE ");
                StringBuilder sql = new StringBuilder();
                using (var session = HibernateUtil.GetSessionT5())
                {
                    string _ids = $"({String.Join(",", request.ids)})";
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(sqlUp.ToString());
                        sql.AppendLine($"  SET STATUS = @STATUS WHERE ID IN {_ids}  ");

                        parametros.Add("@STATUS", request.status);

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }

                }
                //retorno.Objeto.Id = Id;
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Modelo_Elemento_RedeApi/GetCampoDinamico")]
        #region GetAllCampoDinamico
        public async Task<GetApiRetorno<Modelo_Elemento_RedeCampoDinamicoViewModel>> GetCampoDinamico(Modelo_CampoRequest request)
        {
            var retorno = new GetApiRetorno<Modelo_Elemento_RedeCampoDinamicoViewModel>();

            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    using (var sessionCampo = new Conexao<Modelo_CampoDinamico>(session))
                    {
                        IEnumerable<Modelo_DinamicoCampo> Lista = null;

                        Lista = request != null ? (await sessionCampo.select(p => p.Id == request.Id_)).Select(p =>
                        {
                            return new Modelo_DinamicoCampo
                            {
                                Id = p.Id,
                                Descricao = p.Descricao,
                                Gravado = p.Gravado,
                                Tipo = p.Tipo,
                                TipoStr = HtmlEnumExtensions.GetEnumDescription(p.Tipo),
                                OpcoesStr = "",
                                Opcoes = new List<Opcoes_CampoDinamicoView>(),
                            };
                        }).ToList() : (await sessionCampo.select(p => p.Id != 0)).Select(p =>
                        {
                            return new Modelo_DinamicoCampo
                            {
                                Id = p.Id,
                                Descricao = p.Descricao,
                                Gravado = p.Gravado,
                                Tipo = p.Tipo,
                                TipoStr = HtmlEnumExtensions.GetEnumDescription(p.Tipo),
                                OpcoesStr = "",
                                Opcoes = new List<Opcoes_CampoDinamicoView>(),
                            };
                        }).ToList();


                        retorno.Objeto = new Modelo_Elemento_RedeCampoDinamicoViewModel { Lista = Lista };
                        retorno.Ok = true;
                    }
                }

            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;

            }
            return (retorno);
        }
        #endregion

        [HttpGet()]
        [Route("api/Modelo_Elemento_RedeApi/GetOpcoesDinamico")]
        #region GetAllOpcoesCampoDinamico
        public async Task<GetApiRetorno<Modelo_Elemento_RedeCampoDinamicoViewModel>> GetOpcoes(long? Id)
        {
            var retorno = new GetApiRetorno<Modelo_Elemento_RedeCampoDinamicoViewModel>();

            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    using (var sessionCampo = new Conexao<Opcoes_CampoDinamico>(session))
                    {

                        var Opcoes = (await sessionCampo.select(p => p.Campo.Id == Id)).Select(p =>
                        {
                            return new Opcoes_CampoDinamicoView
                            {
                                Id = p.Id,
                                Resposta = p.Resposta,
                                Id_Campo = p.Campo.Id,
                            };
                        }).ToList();
                        if (Opcoes.Count > 0)
                        {
                            retorno.Objeto = new Modelo_Elemento_RedeCampoDinamicoViewModel { Opcao = Opcoes };
                        }
                        else
                        {
                            retorno.Objeto = new Modelo_Elemento_RedeCampoDinamicoViewModel
                            { Opcao = new List<Opcoes_CampoDinamicoView>() };
                        }

                    }
                    retorno.Ok = true;

                }

            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;

            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Modelo_Elemento_RedeApi/GetComboCampoDinamico")]
        #region GetComboCampoDinamico
        public async Task<ObjCombo<String>> GetComboCampoDinamico(RequestObjCombo<String> request)
        {
            var retorno = new ObjCombo<String>();
            int contagem = 0;
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            var parametros = new Dictionary<String, Object>();

            sqlfrom.AppendLine("FROM TB_MODELO_CAMPO_DINAMICO A");
            sqlwhere.AppendLine("WHERE ");

            using (var session = HibernateUtil.GetSession())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    sql.AppendLine("SELECT COUNT(A.ID) IDS");
                    sql.AppendLine(sqlfrom.ToString());
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            contagem = Convert.ToInt32(reader["IDS"]);
                        }
                    }
                }
            }
            using (var session = HibernateUtil.GetSession())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    sql.AppendLine("SELECT A.ID,A.DESCRICAO");
                    sql.AppendLine(sqlfrom.ToString());
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ObjComboResult<String>
                            {
                                Id = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "-",
                                Text = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",
                            };
                            retorno.results.Add(obj);
                        }
                    }
                }
            }
            return retorno;
        }
        #endregion


        [HttpPost]
        [Route("api/Modelo_Elemento_RedeApi/GravarCampoDinamico")]
        #region GravarCampoDinamico
        public async Task<GetApiRetorno<IEnumerable<Modelo_DinamicoCampo>>> GravarCampoDinamico(Modelo_Elemento_RedeCampoDinamicoViewModel request)
        {
            foreach (var opt in request.Lista)
            {
                if (!String.IsNullOrEmpty(opt.OpcoesStr))
                {
                    List<Opcoes_CampoDinamicoView> JsonToObj = JsonService.DeserializeObject<List<Opcoes_CampoDinamicoView>>(opt.OpcoesStr);
                    opt.Opcoes = JsonToObj;
                }

            };

            GetApiRetorno<IEnumerable<Modelo_DinamicoCampo>> retorno = new GetApiRetorno<IEnumerable<Modelo_DinamicoCampo>>();
            try
            {
                using (ISession sessiont5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoColunas = new Conexao<ColunaElementoRede>(sessiont5))
                    using (var repoOpcoes = new Conexao<Opcoes_CampoDinamico>(sessiont5))
                    using (var repoCamposDyn = new Conexao<Modelo_CampoDinamico>(sessiont5))
                    {
                        var Campos = request.Lista.Where(x => x.Id == 0).Select(p =>
                          {
                              var modelo = new Modelo_CampoDinamico
                              {

                                  Descricao = p.Descricao,
                                  Gravado = true,
                                  Tipo = p.Tipo,

                              };
                              modelo.Opcoes = p.Opcoes.Select(y =>
                              {
                                  return new Opcoes_CampoDinamico
                                  {
                                      Resposta = y.Resposta,
                                      Campo = modelo

                                  };
                              }).ToList();

                              return modelo;
                          }).ToList();

                        //await repoCamposDyn.SaveOrUpdate(Campos);
                        var camposLista = repoCamposDyn.select(p => p.Tipo == Tipo_CampoModelo.Lista);
                        var parametros = new Dictionary<String, Object>();

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(@$"INSERT INTO TB_MODELO_CAMPO_DINAMICO (ID,DESCRICAO,TIPO_CAMPO,GRAVADO) 
                        VALUES((SELECT COALESCE(MAX(ID), 0) +1 FROM TB_MODELO_CAMPO_DINAMICO), @DESCRICAO, @TIPO_CAMPO, @GRAVADO)  ")
                        .AppendLine("RETURNING ID");

                        foreach (var Value in Campos)
                        {
                            using (var sessionAniel = HibernateUtil.GetSessionT5())
                            using (var com = sessionAniel.Connection.CreateCommand())
                            {
                                parametros.Add("@DESCRICAO", Value.Descricao);
                                parametros.Add("@GRAVADO", Value.Gravado);
                                parametros.Add("@TIPO_CAMPO", Convert.ToInt32(Value.Tipo));//Html.EnumExtensions.GetEnumDescription(Value.Tipo)
                                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionAniel.Connection, null);


                                Int64 id_resposta = await new Conexao<Modelo_CampoDinamico>(sessiont5).Max(p => p.Id);
                                Value.Id = id_resposta;
                            }
                        }
                        //Stringbuilder para gravação 



                        //ExecutenonQuery
                        var lista = Campos.FindAll(p => p.Tipo == Tipo_CampoModelo.Lista && (p.Opcoes != null && p.Opcoes.Count > 0));
                        if (lista.Count > 0)
                        {
                            List<Opcoes_CampoDinamicoView> listaOpcoes = new List<Opcoes_CampoDinamicoView>();
                            foreach (var item in lista)
                            {
                                foreach (var opcao in item.Opcoes)
                                {
                                    listaOpcoes.Add(new Opcoes_CampoDinamicoView
                                    {
                                        Id = 0,
                                        Id_Campo = item.Id,
                                        Resposta = opcao.Resposta
                                    });
                                }

                            }
                            //var Lista = JsonService.SerializeObject(lista);
                            await GravarOpcoes(listaOpcoes);


                        }

                    }
                }
                retorno.Objeto = request.Lista;
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
                retorno.Ok = false;
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Modelo_Elemento_RedeApi/GravarOpcoes")]
        #region Gravar_Opcoes
        public async Task<PostApiRetorno> GravarOpcoes(List<Opcoes_CampoDinamicoView> request)
        {

            var retorno = new PostApiRetorno();
            //var req = JsonService.DeserializeObject<List<Opcoes_CampoDinamicoView>>(request);
            try
            {
                using (ISession sessiont5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoColunas = new Conexao<ColunaElementoRede>(sessiont5))
                    using (var repoOpcoes = new Conexao<Opcoes_CampoDinamico>(sessiont5))
                    using (var repoCamposDyn = new Conexao<Modelo_CampoDinamico>(sessiont5))

                    {
                        var Opcoes = request.Select(p =>
                        {
                            return new Opcoes_CampoDinamico
                            {
                                Resposta = p.Resposta,
                                Campo = repoCamposDyn.selectFirst(y => y.Id == p.Id_Campo).Result,

                            };
                        }).ToList();
                        if (Opcoes.Count == 0) { throw new Exception("Dados não foram encontrados"); }
                        await repoOpcoes.SaveOrUpdate(Opcoes);
                    }
                }
            }

            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
                retorno.Ok = false;
            }
            return retorno;

        }
        #endregion


        [HttpGet()]
        [Route("api/Modelo_Elemento_RedeApi/DeletarCampoDinamico")]
        #region DeletarCampoDinamico
        public async Task<DeleteApiRetorno<long>> DeletarCampoDinamico(Int64? value)
        {
            var retorno = new DeleteApiRetorno<long>();
            try
            {
                using (var sessiont5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoCampoDyn = new Conexao<Modelo_CampoDinamico>(sessiont5))
                    {
                        if (value.HasValue)
                        {
                            var campoSelecionado = await repoCampoDyn.selectFirst(prop => prop.Id == value);
                            await repoCampoDyn.Remove(campoSelecionado);
                        }
                        else { throw new Exception(Resources.RsBase.NAOENCONTRADO); }

                    }
                }
                retorno.Ok = true;
            }
            catch (Exception error)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = error.Message;
                retorno.InnerException += error.InnerException.Message;
            }
            return retorno;

        }
        #endregion


        [Route("api/Modelo_Elemento_RedeApi/ExcluirGridColuna")]
        [HttpGet()]
        #region ExcluirGridColuna
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridColuna(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<ColunaElementoRede>(session))
                    {

                        ColunaElementoRede modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

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
        #endregion
    }
    public interface IModelo_Elemento_RedeApi
    {
        /// <summary>
        /// Api para Listagem  dos Modelos de Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<DataTableResponse<Modelo_Elemento_RedeListaModels>> GetAll(Modelo_Elemento_RedeRequest request);
        /// <summary>
        /// Api para GetCombo dos Registros de Modelo de Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<String> request);
        /// <summary>
        /// Api para Gravar o Modelo de Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PostApiRetorno<Int64>> Gravar(ParamWebApi<Modelo_Elemento_RedeCadastroModel> request);
        /// <summary>
        /// Api para Encontrar o Modelo de Elemento de Rede utilizando seu Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<Modelo_Elemento_RedeCadastroModel>> FindById(Int64 Id);
        /// <summary>
        /// Deleta registros por Id da Modelo de Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<DeleteApiRetorno<Int64>> Excluir(ParamWebApi<Int64> value);
        /// <summary>
        /// Api para Alterar o Status de um Modelo de Elemento de Rede
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public Task<PostApiRetorno<Modelo_Elemento_RedeListaModels>> AlterarStatus(AlterarStatusViewModel request);
        public Task<GetApiRetorno<Modelo_Elemento_RedeCampoDinamicoViewModel>> GetCampoDinamico(Modelo_CampoRequest request);
        /// <summary>
        /// Api para ter o combo de Campos Dinamicos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ObjCombo<String>> GetComboCampoDinamico(RequestObjCombo<String> request);
        /// <summary>
        ///  Api para Gravar campos Dinamicos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<IEnumerable<Modelo_DinamicoCampo>>> GravarCampoDinamico(Modelo_Elemento_RedeCampoDinamicoViewModel request);
        /// <summary>
        /// Api para Gravação das opções dinamicas
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PostApiRetorno> GravarOpcoes(List<Opcoes_CampoDinamicoView> request);
        /// <summary>
        /// Api para Excluir campos Dinâmicos
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<DeleteApiRetorno<long>> DeletarCampoDinamico(Int64? value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<DeleteApiRetorno<Int64>> ExcluirGridColuna(Int64 value);

    }
}