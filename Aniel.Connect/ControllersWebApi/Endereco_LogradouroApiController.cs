using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Endereco_LogradouroApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/EnderecoLogradouroapi/GetAll")]
        public async Task<DataTableResponse<Endereco_LogradouroListaModel>> GetAll(Endereco_Logradouro_DataTableRequest request)
        {
            List<Endereco_LogradouroListaModel> retorno = new List<Endereco_LogradouroListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_ENDERECO_LOGRADOURO");

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

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS, A.CEP, A.TIPO,  A.ID_PAIS, B.NOME AS PAIS, C.SIGLA AS UF, D.DESCRICAO AS CIDADE, E.DESCRICAO AS BAIRRO")
                       .AppendLine("FROM TB_ENDERECO_LOGRADOURO A")

                       .AppendLine("LEFT JOIN TB_PAIS B")
                       .AppendLine("ON A.ID_PAIS = B.ID")

                       .AppendLine("LEFT JOIN TB_UF C")
                       .AppendLine("ON A.ID_UF = C.ID")

                       .AppendLine("LEFT JOIN TB_CIDADE D")
                       .AppendLine("ON A.ID_CIDADE = D.ID")

                       .AppendLine("LEFT JOIN TB_BAIRRO E")
                       .AppendLine("ON A.ID_BAIRRO = E.ID")

                       .AppendLine("WHERE A.ID > 0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sql.AppendLine("AND (C.SIGLA = @SIGLA)");
                        parametros.Add("@SIGLA", Convert.ToString(request.UF));
                    }

                    if (!String.IsNullOrEmpty(request.Logradouro))
                    {
                        sql.AppendLine("AND (A.DESCRICAO = @DESCRICAO)");
                        parametros.Add("@DESCRICAO", Convert.ToString(request.Logradouro));
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

                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        sql.AppendLine("AND (E.DESCRICAO = @BAIRRO)");
                        parametros.Add("@BAIRRO", Convert.ToString(request.Bairro));
                    }

                    if (!String.IsNullOrEmpty(request.Tipo_Endereco_Logradouro))
                    {
                        sql.AppendLine("AND (A.TIPO = @TIPO_ENDERECO_LOGRADOURO)");
                        parametros.Add("@TIPO_ENDERECO_LOGRADOURO", Convert.ToString(request.Tipo_Endereco_Logradouro));
                    }

                    if (!String.IsNullOrEmpty(request.Cep))
                    {
                        sql.AppendLine("AND (A.CEP LIKE @CEP) ");
                        parametros.Add("@CEP", String.Format("%{0}%", request.Cep));
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
                            retorno.Add(new Endereco_LogradouroListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Cep = Convert.ToString(reader["CEP"]),
                                Tipo = HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(Convert.ToString(reader["TIPO"])),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Bairro = Convert.ToString(reader["BAIRRO"]),
                                Cidade = Convert.ToString(reader["CIDADE"]),
                                Uf = Convert.ToString(reader["UF"]),
                                Pais = Convert.ToString(reader["PAIS"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Endereco_LogradouroListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        private Endereco_LogradouroCadastroModel GetViewObj(Endereco_Logradouro obj)
        {
            return new Endereco_LogradouroCadastroModel
            {
                Id = obj.Id,
                Cep = obj.Cep,
                Tipo_Endereco_Logradouro = obj.Tipo,
                Descricao = obj.Descricao,
                Bairro = obj.Bairro.Id,
                Cidade = obj.Cidade.Id,
                UF = obj.UF.Id,
                Pais = obj.Pais.Id,
                Tipo_Zona = obj.Tipo_Zona,
                Status = obj.Status == Status_Default.Ativo,
                Cod_Area = obj.Cod_Area,
                Cod_Escritorio = obj.Cod_Escritorio,
                Cod_Localidade = obj.Cod_Localidade,
                CodigoAuxiliar = obj.CodAuxiliar
            };
        }

        private void BindViewToObj(ISession session, ref Endereco_Logradouro objBanco, Endereco_LogradouroCadastroModel view)
        {
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoBairro = new Conexao<Bairro>(session))
            {

                objBanco.Cep = view.Cep;
                objBanco.Tipo = view.Tipo_Endereco_Logradouro;
                objBanco.Descricao = view.Descricao;
                objBanco.Tipo_Zona = (Tipo_Atendimento_Zona)view.Tipo_Zona;

                objBanco.Pais = view.Pais != null ? repoPais.selectFirst(p => p.Id == view.Pais).Result : null;
                objBanco.UF = view.UF != null ? repoUf.selectFirst(p => p.Id == view.UF).Result : null;
                objBanco.Cidade = view.Cidade != null ? repoCidade.selectFirst(p => p.Id == view.Cidade).Result : null;
                objBanco.Bairro = view.Bairro != null ? repoBairro.selectFirst(p => p.Id == view.Bairro).Result : null;

                objBanco.Cod_Area = view.Cod_Area != null ? view.Cod_Area : null;
                objBanco.Cod_Escritorio = view.Cod_Escritorio != null ? view.Cod_Escritorio : null;
                objBanco.Cod_Localidade = view.Cod_Localidade != null ? view.Cod_Localidade : null;
                objBanco.CodAuxiliar = view.CodigoAuxiliar;


                if (objBanco.Id != 0)
                {
                    objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
                }
            }

        }

        [HttpPost()]
        [Route("api/EnderecoLogradouroapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_ENDERECO_LOGRADOURO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/EnderecoLogradouroapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_ENDERECO_LOGRADOURO WHERE ID IN {_ids}");

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
        [Route("api/EnderecoLogradouroapi/FindById")]
        public async Task<GetApiRetorno<Endereco_LogradouroCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Endereco_LogradouroCadastroModel> retorno = new GetApiRetorno<Endereco_LogradouroCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Endereco_Logradouro>(session))
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
        [Route("api/EnderecoLogradouroapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Endereco_LogradouroCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Endereco_Logradouro modelo = null, modelo_Anterior = null;

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionSql = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Endereco_Logradouro>(session))
                    using (var sessionLog = HibernateUtil.GetSessionLog())
                    {
                        String usuario = value.Objeto.ViaCep ? "VIACEP" : value.Usuario;
                        String nomeUsuario = value.Objeto.ViaCep ? "VIACEP" : value.NomeUsuario;

                        if (value.Objeto.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                            modelo_Anterior = (Endereco_Logradouro)modelo.Clone();
                        }

                        if (modelo == null)
                        {
                            if (!String.IsNullOrEmpty(value.Objeto.Cep))
                            {
                                using (var com = sessionSql.Connection.CreateCommand())
                                {
                                    var cep = value.Objeto.Cep.Replace(".", "").Replace("-", "");
                                    Int64 IdEnderecoLogradouro = 0;
                                    StringBuilder sql = new StringBuilder(@$"SELECT FIRST 1 ID FROM TB_ENDERECO_LOGRADOURO WHERE REPLACE(REPLACE(CEP, '.', ''), '-', '') = {cep} AND STATUS = 1 ORDER BY DATAHORACRIACAO DESC");
                                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionSql.Connection, null))
                                    {
                                        if (await reader.ReadAsync())
                                        {
                                            IdEnderecoLogradouro = Convert.IsDBNull(reader["ID"]) ? 0 : Convert.ToInt64(reader["ID"]);
                                        }
                                    }

                                    if (IdEnderecoLogradouro > 0)
                                    {
                                        modelo = (await repository.select(p => p.Id == IdEnderecoLogradouro)).FirstOrDefault();
                                        modelo_Anterior = (Endereco_Logradouro)modelo.Clone();
                                    }
                                    else
                                    {
                                        modelo = new Endereco_Logradouro
                                        {
                                            Status = Status_Default.Ativo,
                                            UsuarioCriacao = value.Usuario
                                        };
                                    }
                                }
                            }
                            else
                            {
                                modelo = new Endereco_Logradouro
                                {
                                    Status = Status_Default.Ativo,
                                    UsuarioCriacao = value.Usuario
                                };
                            }
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        if (modelo_Anterior != null)
                        {
                            using (var repositoryLog = new Conexao<Log_Endereco_Logradouro>(sessionLog))
                            {
                                modelo.UsuarioAlteracao = usuario;
                                var log = modelo.GeraLog(modelo_Anterior, usuario, nomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLog.Save(log);
                            }
                        }
                        else
                        {
                            using (var repositoryLog = new Conexao<Log_Endereco_Logradouro>(sessionLog))
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
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/EnderecoLogradouroapi/busca")]
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
                        sql.AppendLine("FROM TB_ENDERECO_LOGRADOURO A");
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

        [Route("api/EnderecoLogradouroapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Endereco_Logradouro>(session))
                    {

                        Endereco_Logradouro modelo = null;
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
        public async Task<GetApiRetorno<IEnumerable<Endereco_LogradouroListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Endereco_LogradouroListaModel>> retorno = new GetApiRetorno<IEnumerable<Endereco_LogradouroListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Endereco_Logradouro>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Endereco_LogradouroListaModel
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
        [Route("api/EnderecoLogradouroapi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ENDERECO_LOGRADOURO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai != null)
                    {
                        condicoes.Add(" A.CEP LIKE UPPER(@SEARCH) ");

                        parametros.Add("@SEARCH", String.Format("%{0}%", request.idpai));
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.DESCRICAO, A.TIPO, B.NOME AS PAIS,C.SIGLA AS UF,D.DESCRICAO AS CIDADE, E.DESCRICAO AS BAIRRO, A.CEP
                                                               FROM TB_ENDERECO_LOGRADOURO A                                                              
                                                               LEFT JOIN TB_PAIS B
                                                               ON A.ID_PAIS = B.ID
                                                               LEFT JOIN TB_UF C
                                                               ON A.ID_UF = C.ID
                                                               LEFT JOIN TB_CIDADE D
                                                               ON A.ID_CIDADE = D.ID
                                                               LEFT JOIN TB_BAIRRO E
                                                               ON A.ID_BAIRRO = E.ID
                                                               WHERE");
                        sql.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                retorno.results.Add(new ObjComboResultTipoEndereco<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Pais = Convert.ToString(reader["PAIS"]),
                                    Cidade = Convert.ToString(reader["CIDADE"]),
                                    Bairro = Convert.ToString(reader["BAIRRO"]),
                                    Cep = Convert.ToString(reader["CEP"]),
                                    Uf = Convert.ToString(reader["UF"]),
                                    Tipo_Endereco_Logradouro = HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(Convert.ToString(reader["TIPO"])),

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

        [HttpPost()]
        [Route("api/EnderecoLogradouroapi/GetComboEndereco")]
        public async Task<ObjCombo<String>> GetComboEndereco(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ENDERECO_LOGRADOURO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    //TODO: Descobrir IdPai necessário para esse combo
                    if (request.idpai != null)
                    {
                        //condicoes.Add(" A.CEP LIKE UPPER(@SEARCH) ");

                        //parametros.Add("@SEARCH", String.Format("%{0}%", request.idpai));
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        var cepNumerico = new string(request.q.Where(char.IsDigit).ToArray());
                        var isCepValido = !string.IsNullOrEmpty(cepNumerico);
                        if (isCepValido)
                            condicoes.Add($"(REPLACE(REPLACE(A.CEP, '.', ''), '-', '') LIKE '%{cepNumerico}%')");
                        else
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.DESCRICAO, A.CEP, A.TIPO,B.ID AS ID_PAIS, B.NOME AS PAIS,C.SIGLA AS UF,D.ID AS ID_CIDADE,D.DESCRICAO AS CIDADE, E.ID AS ID_BAIRRO, E.DESCRICAO AS BAIRRO,
                                                               A.COD_AREA, A.COD_ESCRITORIO, A.COD_LOCALIDADE 
                                                               FROM TB_ENDERECO_LOGRADOURO A                                                              
                                                               LEFT JOIN TB_PAIS B
                                                               ON A.ID_PAIS = B.ID
                                                               LEFT JOIN TB_UF C
                                                               ON A.ID_UF = C.ID
                                                               LEFT JOIN TB_CIDADE D
                                                               ON A.ID_CIDADE = D.ID
                                                               LEFT JOIN TB_BAIRRO E
                                                               ON A.ID_BAIRRO = E.ID
                                                               WHERE");
                        sql.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                try
                                {
                                    retorno.results.Add(new ObjComboResultTipoEndereco<String>
                                    {
                                        Id = Convert.ToString(reader["ID"]),
                                        Text = Convert.ToString(reader["DESCRICAO"]) + ',' + Convert.ToString(reader["BAIRRO"]) + ',' + Convert.ToString(reader["CIDADE"]) + ',' + Convert.ToString(reader["UF"]) + ',' + Convert.ToString(reader["PAIS"]),

                                        Id_Pais = Convert.ToInt64(reader["ID_PAIS"]),
                                        Id_Cidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : 0,

                                        Pais = Convert.ToString(reader["PAIS"]),
                                        Cidade = Convert.ToString(reader["CIDADE"]),
                                        Bairro = Convert.ToString(reader["BAIRRO"]),
                                        Id_Bairro = reader["ID_BAIRRO"] != DBNull.Value ? Convert.ToInt64(reader["ID_BAIRRO"]) : 0,
                                        Uf = Convert.ToString(reader["UF"]),
                                        Descricao = Convert.ToString(reader["DESCRICAO"]),
                                        Cep = Convert.ToString(reader["CEP"]),
                                        Tipo_Endereco_Logradouro = HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(Convert.ToString(reader["TIPO"])),
                                        CodArea = reader["COD_AREA"] != DBNull.Value ? Convert.ToString(reader["COD_AREA"]) : String.Empty,
                                        CodGrupoArea = reader["COD_ESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["COD_ESCRITORIO"]) : String.Empty,
                                        Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["COD_LOCALIDADE"]) : String.Empty,

                                    });
                                }
                                catch (Exception e)
                                {

                                    throw;
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
        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, Endereco_LogradouroImportacao item, Endereco_Logradouro enderecoLogradouros)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<Endereco_Logradouro> Endereco_Logradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoBairro = new Conexao<Bairro>(session))
            using (CidadeApiController Cidade_Api = new CidadeApiController())
            {
                var Uf = repoUf.selectFirst(p => p.Sigla == item.Sigla_UF).Result;
                var cidade = repoCidade.selectFirst(p => p.Descricao == item.Cidade && p.Id_UF == Uf && p.Codigo_IBGE != null).Result;
                var bairro = repoBairro.selectFirst(p => p.Descricao == item.Bairro && p.Cidade == cidade).Result;

                if (Endereco_Logradouro.selectFirst(p => p.Descricao == item.Descricao_Logradouro.ToUpper()
                && p.Bairro == bairro && p.Cidade == cidade && p.UF == Uf).Result != null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Descricao_Logradouro,
                        Erro = "O Endereço_Logradouro já está criado. Endereço_Logradouro não foi importado."
                    });

                #region verificar no banco
                if (cidade == null) retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Cidade,
                    Erro = "Cidade não encontrada no banco de dados."
                });
                if (bairro == null) retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Bairro,
                    Erro = "Bairro não encontrado no banco de dados."
                });
                if (Uf == null) retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Sigla_UF,
                    Erro = "UF não encontrado no banco de dados."
                });
                #endregion

                if (String.IsNullOrEmpty(item.Descricao_Logradouro))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Descricao_Logradouro,
                        Erro = "Descrição não foi importado."
                    });

                if (String.IsNullOrEmpty(item.Sigla_UF))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Sigla_UF,
                        Erro = "UF não foi importado."
                    });
                if (String.IsNullOrEmpty(item.Pais))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Pais,
                        Erro = "País não foi importado."
                    });
                if (String.IsNullOrEmpty(item.Cidade))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Cidade,
                        Erro = "Cidade não foi importada."
                    });
                if (Enum.IsDefined(typeof(Tipo_Endereco_Logradouro), item.Tipo_Endereco_Logradouro))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Tipo_Endereco_Logradouro,
                        Erro = "Tipo de Endereço/Logradouro não foi importado."
                    });


                if (retorno.Count() > 0)
                    return retorno;

                enderecoLogradouros.Cep = item.Cep;
                enderecoLogradouros.Descricao = item.Descricao_Logradouro.ToUpper();
                enderecoLogradouros.Status = Status_Default.Ativo;
                enderecoLogradouros.Tipo_Zona = Enum.TryParse<Tipo_Atendimento_Zona>(item.Tipo_Zona ?? "", true, out var tipoZona) ? tipoZona : Tipo_Atendimento_Zona.Urbano;
                enderecoLogradouros.Tipo = (Tipo_Endereco_Logradouro)Enum.Parse(typeof(Tipo_Endereco_Logradouro), item.Tipo_Endereco_Logradouro);

                enderecoLogradouros.Pais = item.Pais.Length <= 2 ?
                    repoPais.selectFirst(p => p.Id == Convert.ToInt64(item.Pais)).Result :
                        enderecoLogradouros.Pais = repoPais.selectFirst(p => p.Nome == item.Pais).Result;

                enderecoLogradouros.UF = Uf;
                enderecoLogradouros.Bairro = bairro;
                enderecoLogradouros.Cidade = cidade;

            }
            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModelo(Endereco_LogradouroImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Endereco_Logradouro> Endereco_Logradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoBairro = new Conexao<Bairro>(session))
            {
                var Uf = repoUf.selectFirst(p => p.Sigla == item.Sigla_UF).Result;
                var cidade = repoCidade.selectFirst(p => p.Descricao == item.Cidade && p.Id_UF == Uf && p.Codigo_IBGE != null).Result;
                var bairro = repoBairro.selectFirst(p => p.Descricao == item.Bairro && p.Cidade == cidade).Result;

                Endereco_Logradouro enderecoLogradouros = new Endereco_Logradouro();

                retorno = await ImportacaoModeloBase(session, item, enderecoLogradouros);
                enderecoLogradouros.Cep = item.Cep;
                enderecoLogradouros.Descricao = item.Descricao_Logradouro.ToUpper();
                enderecoLogradouros.Status = Status_Default.Ativo;
                enderecoLogradouros.Tipo = (Tipo_Endereco_Logradouro)Enum.Parse(typeof(Tipo_Endereco_Logradouro), item.Tipo_Endereco_Logradouro);

                enderecoLogradouros.Pais = item.Pais.Length <= 2 ?
                    repoPais.selectFirst(p => p.Id == Convert.ToInt64(item.Pais)).Result :
                        enderecoLogradouros.Pais = repoPais.selectFirst(p => p.Nome == item.Pais).Result;

                enderecoLogradouros.UF = Uf;
                enderecoLogradouros.Cidade = cidade;
                enderecoLogradouros.Bairro = bairro;

                if (retorno.Count() > 0)
                    return retorno;


                await Endereco_Logradouro.SaveOrUpdate(enderecoLogradouros);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Descricao_Logradouro,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/EnderecoLogradouroapi/GetComboElem_Rede")]
        public async Task<ObjCombo<String>> GetComboElem_Rede(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ENDERECO_LOGRADOURO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai != null)
                    {
                        condicoes.Add(" A.CEP LIKE UPPER(@SEARCH) ");

                        parametros.Add("@SEARCH", String.Format("%{0}%", request.idpai));
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.DESCRICAO, A.TIPO, A.TIPO_ZONA, A.STATUS,A.CEP, B.NOME AS PAIS, C.SIGLA AS UF, D.DESCRICAO AS CIDADE, E.DESCRICAO AS BAIRRO
                                                               FROM TB_ENDERECO_LOGRADOURO A                                                              
                                                               LEFT JOIN TB_PAIS B
                                                               ON A.ID_PAIS = B.ID
                                                               LEFT JOIN TB_UF C
                                                               ON A.ID_UF = C.ID
                                                               LEFT JOIN TB_CIDADE D
                                                               ON A.ID_CIDADE = D.ID
                                                               LEFT JOIN TB_BAIRRO E
                                                               ON A.ID_BAIRRO = E.ID
                                                               WHERE");
                        sql.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                retorno.results.Add(new ObjComboResultEndereco_Logradouro_Elem<String>
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : String.Empty,
                                    Text = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
                                    Pais = reader["PAIS"] != DBNull.Value ? Convert.ToString(reader["PAIS"]) : String.Empty,
                                    Cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : String.Empty,
                                    Bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : String.Empty,
                                    Uf = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : String.Empty,
                                    Tipo_Endereco_Logradouro = reader["TIPO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(Convert.ToString(reader["TIPO"])) : String.Empty,
                                    Tipo_Atendimento_Zona = reader["TIPO_ZONA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Atendimento_Zona>(Convert.ToString(reader["TIPO_ZONA"])) : String.Empty,
                                    Status = reader["TIPO_ZONA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])) : String.Empty,
                                    Cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : String.Empty
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

        //public async Task<GetApiRetorno<object>> Get_Registro_Logradouro(string cidade, string logradouro)
        //{
        //    GetApiRetorno<Endereco_Logradouro> retorno = new GetApiRetorno<Endereco_Logradouro>();
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();

        //        Dictionary<string, object> parametros = new Dictionary<string, object>();
        //        using (var session = HibernateUtil.GetSession()) 
        //        {

        //        }
        //            parametros.Add("@LOGRADOURO", logradouro);
        //        parametros.Add("@CIDADE", cidade);
        //        //parametros.Add("@BAIRRO", bairro);

        //        sql.AppendLine("SELECT FIRST 1 A.ID, A.DESCRICAO, A.ID_BAIRRO, A.ID_CIDADE, A.ID_UF, A.ID_PAIS");

        //        sql.AppendLine("FROM TB_ENDERECO_LOGRADOURO A ");
        //        sql.AppendLine("INNER JOIN TB_CIDADE B ON (B.ID = A.ID_CIDADE)");
        //        sql.AppendLine("INNER JOIN TB_BAIRRO C ON (C.ID = A.ID_BAIRRO)");

        //        sql.AppendLine("WHERE UPPER(A.DESCRICAO) = UPPER(@LOGRADOURO)");
        //        sql.AppendLine("AND UPPER(B.DESCRICAO) = UPPER(@CIDADE)");
        //        sql.AppendLine("AND UPPER(C.DESCRICAO) = UPPER(@BAIRRO)");

        //        using (var reader = await CommandHelper.ExecuteReader(sql, parametros, this.GetConnection(), this.GetTransaction()))
        //        {
        //            if (reader.Read())
        //            {
        //                retorno.Objeto = new Endereco_Logradouro
        //                {
        //                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
        //                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
        //                    Id_Bairro = reader["ID_BAIRRO"] != DBNull.Value ? Convert.ToInt64(reader["ID_BAIRRO"]) : 0,
        //                    Id_Cidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : 0,
        //                    Id_Uf = reader["ID_UF"] != DBNull.Value ? Convert.ToInt64(reader["ID_UF"]) : 0,
        //                    Id_pais = reader["ID_PAIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_PAIS"]) : 0,
        //                    //Codigo_IBGE = reader["CODIGO_IBGE"] != DBNull.Value ? Convert.ToString(reader["CODIGO_IBGE"]) : String.Empty,
        //                };
        //            }

        //        }
        //        retorno.Ok = true;
        //    }
        //    catch (Exception error)
        //    {
        //        retorno.Ok = false;
        //        retorno.Mensagem = error.Message;
        //    }

        //    return retorno;
        //}

        #region Mobile

        [HttpPost]
        [Route("api/EnderecoLogradouroapi/GetListaLogradouroMobile")]
        public async Task<GetApiRetorno<List<EnderecoLogradouroMobileModel>>> GetListaLogradouroMobile(ParamWebApi<string> value)
        {
            GetApiRetorno<List<EnderecoLogradouroMobileModel>> retorno = new GetApiRetorno<List<EnderecoLogradouroMobileModel>>();

            var listaLogradouro = new List<EnderecoLogradouroMobileModel>();
            try
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.STATUS, A.CEP, A.TIPO, B.NOME AS PAIS, C.SIGLA AS UF, A.DESCRICAO AS LOGRADOURO, ");
                    sql.AppendLine("E.DESCRICAO AS BAIRRO, F.DESCRICAO AS CIDADE, A.ID_PAIS , A.ID_UF , A.ID_CIDADE , A.ID_BAIRRO ");
                    sql.AppendLine("FROM TB_ENDERECO_LOGRADOURO A ");
                    sql.AppendLine("INNER JOIN TB_PAIS B ON (A.ID_PAIS = B.ID) ");
                    sql.AppendLine("INNER  JOIN TB_UF C  ON (A.ID_UF = C.ID) ");
                    sql.AppendLine("INNER JOIN TB_BAIRRO E ON (A.ID_BAIRRO = E.ID) ");
                    sql.AppendLine("INNER JOIN TB_CIDADE F ON (A.ID_CIDADE = F.ID) ");
                    sql.AppendLine("WHERE A.STATUS = 1 AND UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");

                    parametros.Add("@SEARCH", String.Format("%{0}%", value.Objeto));


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            listaLogradouro.Add(new EnderecoLogradouroMobileModel
                            {
                                Id_Logradouro = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                Cep = Convert.ToString(reader["CEP"]),
                                Cidade = Convert.ToString(reader["CIDADE"]),
                                Uf = Convert.ToString(reader["UF"]),
                                Pais = Convert.ToString(reader["PAIS"]),
                                Bairro = Convert.ToString(reader["BAIRRO"]),
                                Logradouro = Convert.ToString(reader["LOGRADOURO"]),
                                TipoLogradouro = (Tipo_Endereco_Logradouro)Enum.Parse(typeof(Tipo_Endereco_Logradouro), Convert.ToString(reader["TIPO"])),
                                Id_Pais = reader["ID_PAIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_PAIS"]) : 0,
                                Id_Uf = reader["ID_UF"] != DBNull.Value ? Convert.ToInt64(reader["ID_UF"]) : 0,
                                Id_Cidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : 0,
                                Id_Bairro = reader["ID_BAIRRO"] != DBNull.Value ? Convert.ToInt64(reader["ID_BAIRRO"]) : 0,
                            });
                        }
                    }
                }

                retorno.Objeto = listaLogradouro;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        #endregion
    }
}
