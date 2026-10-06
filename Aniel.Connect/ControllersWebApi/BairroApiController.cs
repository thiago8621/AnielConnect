using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Cidade;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
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
    public class BairroApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Bairroapi/GetAll")]
        public async Task<DataTableResponse<BairroListaModel>> GetAll(Bairro_DataTableRequest request)
        {
            List<BairroListaModel> retorno = new List<BairroListaModel>();
            IOrderedEnumerable<BairroListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_BAIRRO");

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

                    StringBuilder sql = new StringBuilder()
                         .AppendLine(" SELECT A.ID, A.DESCRICAO, A.STATUS, C.NOME AS PAIS, COALESCE(B.SIGLA,D.UF) AS UF, D.DESCRICAO AS CIDADE  ")
                         .AppendLine(" FROM TB_BAIRRO A ")
                         .AppendLine(" Left JOIN TB_UF B ")
                         .AppendLine(" ON A.ID_UF = B.ID  ")
                         .AppendLine(" LEFT JOIN TB_PAIS C")
                         .AppendLine(" ON A.ID_PAIS = C.ID")
                         .AppendLine(" LEFT JOIN TB_CIDADE D")
                         .AppendLine(" ON A.ID_CIDADE = D.ID")
                         .AppendLine(" WHERE A.ID > 0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sql.AppendLine("AND (B.SIGLA = @SIGLA)");
                        parametros.Add("@SIGLA", Convert.ToString(request.UF));
                    }

                    if (!String.IsNullOrEmpty(request.Pais))
                    {
                        sql.AppendLine("AND (A.ID_PAIS = @ID_PAIS)");
                        parametros.Add("@ID_PAIS", Convert.ToInt32(request.Pais));
                    }
                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sql.AppendLine("AND (D.DESCRICAO = @CIDADE)");
                        parametros.Add("@CIDADE", Convert.ToString(request.Cidade));
                    }
                    
                    if (!String.IsNullOrEmpty(request.Status_Filtro))
                    {
                        sql.AppendLine("AND (A.STATUS = @STATUS)");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status_Filtro));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new BairroListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                UF = Convert.ToString(reader["UF"]),
                                Pais = Convert.ToString(reader["PAIS"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
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
                    case 1:     // Id
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }

                    case 2:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }

                    case 3:     // UF
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.UF);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.UF);
                            break;
                        }
                    case 4:     // Pais
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Pais);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Pais);
                            break;
                        } 
                    case 5:     // Status
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<BairroListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private BairroCadastroModel GetViewObj(Bairro obj)
        {
            return new BairroCadastroModel
            {
                Id = obj.Id,
                Cidade = obj.Cidade?.Id,
                Descricao = obj.Descricao,
                UF = obj.UF?.Id,
                Pais = obj.Pais?.Id,
                Status = obj.Status == Status_Default.Ativo,
                CodigoAuxiliar = obj?.CodigoAuxiliar
            };
        }


        private void BindViewToObj(ISession session, ref Bairro objBanco, BairroCadastroModel view)
        {
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            {

                objBanco.Descricao = view.Descricao;

                objBanco.Pais = view.Pais != null ? repoPais.selectFirst(p => p.Id == view.Pais).Result : null;
                objBanco.UF = view.UF != null ? repoUf.selectFirst(p => p.Id == view.UF).Result : null;
                objBanco.Cidade = view.Cidade != null ? repoCidade.selectFirst(p => p.Id == view.Cidade).Result : null;
                objBanco.CodigoAuxiliar = view.CodigoAuxiliar;

                if (objBanco.Id != 0)
                {
                    objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
                }
            }

        }

        [HttpPost()]
        [Route("api/Bairroapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_BAIRRO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Bairroapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_UF WHERE ID IN {_ids}");

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
        [Route("api/Bairroapi/FindById")]
        public async Task<GetApiRetorno<BairroCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<BairroCadastroModel> retorno = new GetApiRetorno<BairroCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Bairro>(session))
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
        [Route("api/Bairroapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<BairroCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            Int64? id_bairro = 0;
            try
            {
                Bairro modelo = null, modelo_Anterior = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Bairro>(session))
                    using (var sessionLog = HibernateUtil.GetSessionLog())
                    {
                        String usuario = value.Objeto.ViaCep ? "VIACEP" : value.Usuario;
                        String nomeUsuario = value.Objeto.ViaCep ? "VIACEP" : value.NomeUsuario;

                        if (value.Objeto.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                            modelo_Anterior = (Bairro)modelo.Clone();
                        }
                        else
                        {
                            var descricaoigual = repository.selectFirst(p => p.Descricao == value.Objeto.Descricao && p.Cidade.Id == value.Objeto.Cidade).Result;
                            if (descricaoigual != null)
                            {
                                id_bairro = descricaoigual.Id;
                                throw new Exception("Bairro já cadastrado");
                            }
                        }

                        if (modelo == null)
                        {
                            modelo = new Bairro
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        if (modelo_Anterior != null)
                        {
                            using (var repositoryLog = new Conexao<Log_Bairro>(sessionLog))
                            {
                                modelo.UsuarioAlteracao = usuario;
                                var log = modelo.GeraLog(modelo_Anterior, usuario, nomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLog.Save(log);
                            }
                        }
                        else
                        {
                            using (var repositoryLog = new Conexao<Log_Bairro>(sessionLog))
                            {
                                var log = modelo.GeraLog(modelo_Anterior, usuario, nomeUsuario, Sinapse.ModelLog.TipoOperLog.Insercao);
                                await repositoryLog.Save(log);
                            }

                        }
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;

                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = id_bairro.HasValue ? Convert.ToInt64(id_bairro) : 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Bairroapi/busca")]
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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO");
                        sql.AppendLine("FROM TB_BAIRRO A");
                        sql.AppendLine("WHERE (UPPER(A.DESCRICAO) LIKE @VALOR) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["DESCRICAO"])
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

        [Route("api/Bairroapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Bairro>(session))
                    {

                        Bairro modelo = null;
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

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<BairroListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<BairroListaModel>> retorno = new GetApiRetorno<IEnumerable<BairroListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Bairro>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new BairroListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao
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

        [HttpPost()]
        [Route("api/Bairroapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_BAIRRO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }
                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CIDADE = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
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
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
                                };
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

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, BairroImportacao item, Bairro bairros)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (ISession sessionSql = HibernateUtil.GetSession())
            using (Conexao<Bairro> Bairro = new Conexao<Bairro>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (CidadeApiController Cidade_Api = new CidadeApiController())
            {
                ICidadeRepository repoCidade = new CidadeRepository(session, sessionSql);
                var Uf = repoUf.selectFirst(p => p.Sigla == item.Sigla_UF.ToUpper()).Result;
                var cidade = repoCidade.GetByNameUpperCase(item.Cidade,item.Sigla_UF);

                if (Bairro.selectFirst(p => p.Descricao == item.Descricao_Bairro.ToUpper()
                && p.UF == Uf && p.Cidade == cidade).Result != null)
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Descricao_Bairro,
                        Erro = "Bairro já existente. Bairro não foi importado."
                    });
                };

                #region verificar no banco
                if (cidade == null) retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Cidade,
                    Erro = "Cidade não encontrada no banco de dados."
                });
                if (Uf == null) retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Sigla_UF,
                    Erro = "UF não encontrado no banco de dados."
                });
                #endregion

                if (String.IsNullOrEmpty(item.Cidade))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Cidade,
                        Erro = "Descrição não foi importado."
                    });

                if (String.IsNullOrEmpty(item.Sigla_UF))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Sigla_UF,
                        Erro = "Sigla_UF não foi importado."
                    });
                if (String.IsNullOrEmpty(item.Pais))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Pais,
                        Erro = "País não foi importado."
                    });


                if (retorno.Count() > 0)
                    return retorno;


                bairros.Status = Status_Default.Ativo;
                bairros.Descricao = item.Descricao_Bairro.ToUpper();

                bairros.Pais = item.Pais.Length <= 2 ?
                    repoPais.selectFirst(p => p.Id == Convert.ToInt64(item.Pais)).Result :
                        bairros.Pais = repoPais.selectFirst(p => p.Nome == item.Pais.ToUpper()).Result;

                bairros.Cidade = cidade;
                bairros.UF = repoUf.selectFirst(p => p.Sigla == item.Sigla_UF).Result;

            }
            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModelo(BairroImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Bairro> Bairro = new Conexao<Bairro>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            {
                Bairro bairros = new Bairro();

                retorno = await ImportacaoModeloBase(session, item, bairros);

                bairros.Status = Status_Default.Ativo;
                bairros.Descricao = item.Descricao_Bairro.ToUpper();


                bairros.Pais = item.Pais.Length <= 2 ?
                    repoPais.selectFirst(p => p.Id == Convert.ToInt64(item.Pais)).Result :
                         bairros.Pais = repoPais.selectFirst(p => p.Nome == item.Pais.ToUpper()).Result;

                if (retorno.Count() > 0)
                    return retorno;

                await Bairro.SaveOrUpdate(bairros);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Descricao_Bairro,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }
    }
}
