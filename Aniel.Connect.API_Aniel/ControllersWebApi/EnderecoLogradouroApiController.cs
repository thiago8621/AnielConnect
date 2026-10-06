using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    [RoutePrefix("api/EnderecoLogradouroApi")]
    public class EnderecoLogradouroApiController : ApiController
    {
        /// <summary>
        /// Retorna a Listagem de Logradouros cadastrados no connect
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("GetAllCondominio")]
        public async Task<ApiResponseObj<List<CondominioPredioListModel>>> GetAllEnderecos(EnderecoCondominioApiRequest request)
        {
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                List<CondominioPredioListModel> retorno = new List<CondominioPredioListModel>();

                using (ISession session = HibernateUtil.GetSession())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    var condicoes = new List<string>();
                    StringBuilder sqlFROM = new StringBuilder(@"FROM TB_CONDOMINIO_PREDIO A 
                                                                LEFT JOIN TB_ENDERECO_LOGRADOURO C 
                                                                ON C.ID = A.ID_ENDERECO_LOGRADOURO
                                                                LEFT JOIN TB_CIDADE B 
                                                                ON B.ID = C.ID_CIDADE 
                                                                LEFT JOIN TB_BAIRRO D
                                                                ON D.ID = C.ID_BAIRRO
                                                                LEFT JOIN TB_UF E
                                                                ON E.ID = C.ID_UF
                                                                 ");

                    string cepFormatado = request.Cep;

                    if (!string.IsNullOrWhiteSpace(cepFormatado))
                    {
                        cepFormatado = cepFormatado.Replace("-", "").Replace(" ", "");

                        if (cepFormatado.Length == 8)
                        {
                            cepFormatado = cepFormatado.Insert(5, "-");
                        }
                    }

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.Cep))
                    {
                        condicoes.Add("C.CEP LIKE @CEP");
                        parametros.Add("@CEP", $"%{cepFormatado}%");
                    }

                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        condicoes.Add("UPPER(D.DESCRICAO) LIKE @BAIRRO");
                        parametros.Add("@BAIRRO", $"%{request.Bairro.ToUpperInvariant()}%");
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        condicoes.Add("UPPER(B.DESCRICAO) LIKE @CIDADE");
                        parametros.Add("@CIDADE", $"%{request.Cidade.ToUpperInvariant()}%");
                    }

                    if (!String.IsNullOrEmpty(request.Logradouro))
                    {
                        condicoes.Add("UPPER(C.DESCRICAO) LIKE UPPER(@LOGRADOURO)");
                        parametros.Add("@LOGRADOURO", $"%{request.Logradouro}%");
                    }

                    if (!String.IsNullOrEmpty(request.settings.search))
                    {
                        condicoes.Add("(UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search.ToUpperInvariant()));
                    }
                    if (condicoes.Count > 0)
                    {
                        sqlFROM.AppendLine($" WHERE {String.Join(" AND ", condicoes)}");
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, C.ID_PAIS,B.DESCRICAO AS CIDADE , A.DESCRICAO, D.DESCRICAO AS BAIRRO, ")
                           .AppendLine(" E.SIGLA AS UF, C.CEP, A.STATUS, C.DESCRICAO AS LOGRADOURO, A.NUMERO,A.STATUS, C.ID AS ID_ENDERECO_LOGRADOURO, B.ID AS ID_CIDADE ")
                           .AppendLine(sqlFROM.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new CondominioPredioListModel
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                    IdPais = reader["ID_PAIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_PAIS"]) : 0,
                                    Cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : "",
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                    Bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : "",
                                    Uf = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : "",
                                    Logradouro = reader["LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["LOGRADOURO"]) : "",
                                    Numero = reader["NUMERO"] != DBNull.Value ? Convert.ToString(reader["NUMERO"]) : "",
                                    Cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : "",
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>((Status_Default)reader["STATUS"]),
                                    IdEnderecoLogradouro = reader["ID_ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ENDERECO_LOGRADOURO"]) : 0,
                                    IdCidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : 0
                                });
                            }
                        }
                    }
                }
                //Page e retornando DataTable
                return new ApiResponseObj<List<CondominioPredioListModel>>
                {
                    ok = true,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new ApiResponseObj<List<CondominioPredioListModel>>
                {
                    error = ex.Message,
                    ok = false
                };
            }
        }

        [HttpPost()]
        [Route("GetAll")]
        public async Task<ApiResponseObj<List<EnderecoLogradouroListModel>>> GetAll(EnderecoLogradouroApiRequest request)
        {
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                List<EnderecoLogradouroListModel> retorno = new List<EnderecoLogradouroListModel>();

                using (ISession session = HibernateUtil.GetSession())
                {
                    string cepFormatado = request.CEP;

                    if (!string.IsNullOrWhiteSpace(cepFormatado))
                    {
                        cepFormatado = cepFormatado.Replace("-", "").Replace(" ", "");

                        if (cepFormatado.Length == 8)
                        {
                            cepFormatado = cepFormatado.Insert(5, "-");
                        }
                    }

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    var condicoes = new List<string>();
                    StringBuilder sqlFROM = new StringBuilder(" FROM TB_ENDERECO_LOGRADOURO A LEFT JOIN TB_BAIRRO B ON B.ID = A.ID_BAIRRO ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.CEP))
                    {
                        condicoes.Add("A.CEP LIKE @CEP");
                        parametros.Add("@CEP", $"%{cepFormatado}%");
                    }

                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        condicoes.Add("UPPER(B.DESCRICAO) LIKE UPPER(@BAIRRO)");

                        parametros.Add("@BAIRRO", $"%{request.Bairro.ToUpperInvariant()}%");

                    }

                    if (!String.IsNullOrEmpty(request.Logradouro))
                    {
                        condicoes.Add("UPPER(A.DESCRICAO) LIKE UPPER(@LOGRADOURO)");
                        parametros.Add("@LOGRADOURO", $"%{request.Logradouro.ToUpperInvariant()}%");

                    }


                    if (!String.IsNullOrEmpty(request.settings.search))
                    {
                        condicoes.Add("(UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search.ToUpperInvariant()));
                    }
                    if (condicoes.Count > 0)
                    {
                        sqlFROM.AppendLine($" WHERE {String.Join(" AND ", condicoes)}");
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, A.ID_PAIS, A.ID_CIDADE, A.DESCRICAO, A.ID_BAIRRO,")
                           .AppendLine(" A.ID_UF, A.CEP, A.STATUS, A.COD_ESCRITORIO, A.COD_AREA, A.COD_LOCALIDADE,")
                           .AppendLine(" A.TIPO, A.TIPO_ZONA")
                           .AppendLine(sqlFROM.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new EnderecoLogradouroListModel
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                    IdPais = reader["ID_PAIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_PAIS"]) : 0,
                                    IdCidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : 0,
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                    IdBairro = reader["ID_BAIRRO"] != DBNull.Value ? Convert.ToInt64(reader["ID_BAIRRO"]) : 0,
                                    IdUf = reader["ID_UF"] != DBNull.Value ? Convert.ToInt64(reader["ID_UF"]) : 0,
                                    CodigoEscritorio = reader["COD_ESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["COD_ESCRITORIO"]) : "",
                                    CodigoArea = reader["COD_AREA"] != DBNull.Value ? Convert.ToString(reader["COD_AREA"]) : "",
                                    CodigoLocalidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["COD_LOCALIDADE"]) : "",
                                    Cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : "",
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>((Status_Default)reader["STATUS"]),
                                    Tipo = HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>((Tipo_Endereco_Logradouro)reader["TIPO"]),
                                    Tipo_Zona = HtmlEnumExtensions.GetEnumDescription<Tipo_Atendimento_Zona>((Tipo_Atendimento_Zona)reader["TIPO_ZONA"]),
                                });
                            }
                        }
                    }
                }
                //Page e retornando DataTable
                return new ApiResponseObj<List<EnderecoLogradouroListModel>>
                {
                    ok = true,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new ApiResponseObj<List<EnderecoLogradouroListModel>>
                {
                    error = ex.Message,
                    ok = false
                };
            }
        }


        [HttpPost]
        [Route("CadastrarUF")]
        public async Task<ApiResponseObj<long>> CadastrarUF(UFApiRequest request)
        {
            List<Uf> retorno = new List<Uf>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Uf dados = new Uf();


                #region BindModel


                using (var session = HibernateUtil.GetSession())
                {
                    using (var repoUf = new Conexao<Uf>(session))
                    using (var repoPais = new Conexao<Pais>(session))
                    {

                        if (request.Descricao == null)
                            throw new Exception("Nome da UF não informado");
                        if (request.Sigla == null)
                            throw new Exception("Sigla da UF não informada");

                        request.Sigla = request.Sigla.Trim().ToUpperInvariant();

                        var validacao = await repoUf.selectFirst(p => p.Sigla == request.Sigla);
                        var pais = await repoPais.selectFirst(p => p.Id == request.Pais);

                        if (validacao != null)
                            throw new Exception("Já existe uma UF cadastrada com a sigla informada");
                        if (pais == null)
                            throw new Exception("Pais não encontrado para o Id informado");


                        dados.UsuarioCriacao = request.settings.user;
                        dados.Descricao = request.Descricao;
                        dados.Sigla = request.Sigla;
                        dados.Codigo_IBGE = request.Codigo_Ibge;
                        dados.CodAuxiliar = request.CodigoAuxiliar;
                        dados.Status = Sinapse.Model.enumerados.Status_Default.Ativo;
                        dados.Pais = pais;

                        await repoUf.Save(dados);

                        return new ApiResponseObj<long>()
                        {
                            ok = true,
                            data = dados.Id,
                            error = ""
                        };
                    }
                    #endregion  
                }
            }

            catch (Exception erro)
            {
                CONSTANTES.log4Net.ErrorFormat("message : {0} \r\n InnerException : {1} StackTrace : {2}\r\n",
                                                erro.Message,
                                                erro?.InnerException?.Message,
                                                erro.StackTrace);

                return new ApiResponseObj<long>()
                {
                    ok = false,
                    data = 0,
                    error = erro.Message
                };
            }
        }

        [HttpPost]
        [Route("CadastrarCidade")]
        public async Task<ApiResponseObj<long>> CadastrarCidade(CidadeApiRequest request)
        {
            List<Cidade> retorno = new List<Cidade>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Cidade dados = new Cidade();


                #region BindModel


                using (var session = HibernateUtil.GetSession())
                {
                    using (var repoCidade = new Conexao<Cidade>(session))
                    using (var repoUf = new Conexao<Uf>(session))
                    using (var repoPais = new Conexao<Pais>(session))
                    {

                        if (request.Descricao == null)
                            throw new Exception("Nome da Cidade não informado");

                        request.Descricao = request.Descricao.Trim().ToUpperInvariant();

                        var uf = await repoUf.selectFirst(p => p.Id == request.IdUf);

                        var lista = await busca(request.Descricao, request.IdUf);


                        if (uf == null)
                            throw new Exception("UF não encontrada para o Id informado");
                        if (lista.Any())
                            throw new Exception("Já Existe Cidade Cadastrada para essa UF");

                        dados.UsuarioCriacao = request.settings.user;
                        dados.Descricao = request.Descricao;
                        dados.Id_UF = await repoUf.selectFirst(p => p.Id == request.IdUf);
                        dados.UF = uf.Sigla;
                        dados.Codigo_IBGE = request.Codigo_Ibge;
                        dados.CodigoAuxiliar = request.CodigoAuxiliar;
                        dados.Status = Sinapse.Model.enumerados.Status_Default.Ativo;
                        dados.Pais = uf.Pais;

                        await repoCidade.Save(dados);

                        return new ApiResponseObj<long>()
                        {
                            ok = true,
                            data = dados.Id,
                            error = ""
                        };
                    }
                    #endregion  
                }
            }

            catch (Exception erro)
            {
                CONSTANTES.log4Net.ErrorFormat("message : {0} \r\n InnerException : {1} StackTrace : {2}\r\n",
                                                erro.Message,
                                                erro?.InnerException?.Message,
                                                erro.StackTrace);

                return new ApiResponseObj<long>()
                {
                    ok = false,
                    data = 0,
                    error = erro.Message
                };
            }
        }

        [HttpPost]
        [Route("CadastrarBairro")]
        public async Task<ApiResponseObj<long>> CadastrarBairro(BairroApiRequest request)
        {
            List<Bairro> retorno = new List<Bairro>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Bairro dados = new Bairro();

                #region BindModel


                using (var session = HibernateUtil.GetSession())
                {
                    using (var repoCidade = new Conexao<Cidade>(session))
                    using (var repoUf = new Conexao<Uf>(session))
                    using (var repoPais = new Conexao<Pais>(session))
                    using (var repoBairro = new Conexao<Bairro>(session))
                    {
                        if (request.Descricao == null)
                            throw new Exception("Nome da Cidade não informado");


                        var cidade = await repoCidade.selectFirst(p => p.Id == request.IdCidade);

                        if (cidade == null)
                            throw new Exception("Cidade não encontrada para o Id informado");

                        request.Descricao = request.Descricao.Trim().ToUpperInvariant();
                        var lista = await buscaBairro(request.Descricao, cidade?.Id_UF?.Id, request.IdCidade);

                        


                        if (lista.Any())
                            throw new Exception("Já Existe Bairro Cadastrada para essa UF e Cidade");

                        dados.UsuarioCriacao = request.settings.user;
                        dados.DataHoraCriacao = DateTime.Now;
                        dados.Descricao = request.Descricao;
                        dados.UF = cidade.Id_UF;
                        dados.Cidade = cidade;
                        dados.CodigoAuxiliar = request.CodigoAuxiliar;
                        dados.Status = Sinapse.Model.enumerados.Status_Default.Ativo;
                        dados.Pais = cidade.Id_UF?.Pais;

                        await repoBairro.Save(dados);
                        session.Flush();

                        return new ApiResponseObj<long>()
                        {
                            ok = true,
                            data = dados.Id,
                            error = ""
                        };
                    }
                    #endregion  
                }
            }

            catch (Exception erro)
            {
                var sb = new System.Text.StringBuilder();
                var e = erro;
                while (e != null)
                {
                    sb.AppendLine($"{e.GetType().FullName}: {e.Message}");
                    e = e.InnerException;
                }

                CONSTANTES.log4Net.Error(sb.ToString());
                return new ApiResponseObj<long> { ok = false, data = 0, error = erro.Message };
            }
        }


        [HttpPost]
        [Route("InsertOrUpdateLogradouro")]
        public async Task<ApiResponseObj<long>> InsertOrUpdateLogradouro(LogradouroCadastrarApiRequest request)
        {
            List<Endereco_Logradouro> retorno = new List<Endereco_Logradouro>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Endereco_Logradouro dados = new Endereco_Logradouro();


                #region BindModel


                using (var session = HibernateUtil.GetSession())
                {
                    using (var repoCidade = new Conexao<Cidade>(session))
                    using (var repoUf = new Conexao<Uf>(session))
                    using (var repoPais = new Conexao<Pais>(session))
                    using (var repoBairro = new Conexao<Bairro>(session))
                    using (var repoLogradouro = new Conexao<Endereco_Logradouro>(session))
                    {
                        var bairro = await repoBairro.selectFirst(p => p.Id == request.IdBairro);
                        var logExistente = await repoLogradouro.selectFirst(p => p.Id == request.IdLogradouro);


                        if (request.Logradouro == null)
                            throw new Exception("Logradouro não informado");
                        if (bairro?.Cidade?.Id_UF == null)
                            throw new Exception("UF não encontrada para o Id informado");
                        if (bairro?.Cidade?.Id_UF?.Pais == null)
                            throw new Exception("Pais não encontrada para o Id informado");
                        if (bairro?.Cidade == null)
                            throw new Exception("Cidade não encontrada para o Id informado");
                        if (bairro == null)
                            throw new Exception("Bairro não encontrado para o Id informado");


                        string cepFormatado = request.Cep;

                        if (!string.IsNullOrWhiteSpace(cepFormatado))
                        {
                            cepFormatado = cepFormatado.Replace("-", "").Replace(" ", "");

                            if (cepFormatado.Length == 8)
                            {
                                cepFormatado = cepFormatado.Insert(5, "-");
                            }
                        }
                        if (logExistente != null)
                        {
                            logExistente.Descricao = request.Logradouro;
                            logExistente.Cep = cepFormatado;
                            logExistente.Pais = bairro?.Cidade?.Id_UF?.Pais;
                            logExistente.UF = bairro?.Cidade?.Id_UF;
                            logExistente.Cidade = bairro?.Cidade;
                            logExistente.Bairro = bairro;
                            if (request.Cod_Area != null)
                                logExistente.Cod_Area = request.Cod_Area;
                            if (request.Cod_Escritorio != null)
                                logExistente.Cod_Escritorio = request.Cod_Escritorio;
                            if (request.Cod_Localidade != null)
                                logExistente.Cod_Localidade = Convert.ToString(request.Cod_Localidade);
                            if (request.Tipo != null)
                                logExistente.Tipo = request.Tipo;
                            if (request.Tipo_Zona != null)
                                logExistente.Tipo_Zona = request.Tipo_Zona;
                            if (request.Cod_Auxiliar != null)
                                logExistente.CodAuxiliar = request.Cod_Auxiliar;
                            logExistente.UsuarioAlteracao = request.settings.user;
                            dados.DataHoraAlteracao = DateTime.Now;

                            await repoLogradouro.Update(logExistente);

                            return new ApiResponseObj<long>()
                            {
                                ok = true,
                                data = logExistente.Id,
                                error = ""
                            };
                        }
                        else
                        {
                            var lista = await buscaLogradouro(request.Logradouro, bairro?.Cidade?.Id_UF?.Id, bairro?.Cidade?.Id, bairro?.Id);
                            if (lista.Any())
                                throw new Exception("Logradouro já Cadastrado");

                            dados.UsuarioCriacao = request.settings.user;
                            dados.DataHoraCriacao = DateTime.Now;
                            dados.Descricao = request.Logradouro;
                            dados.Cep = cepFormatado;
                            dados.Status = Sinapse.Model.enumerados.Status_Default.Ativo;
                            dados.Pais = bairro?.Cidade?.Id_UF?.Pais;
                            dados.UF = bairro?.Cidade?.Id_UF;
                            dados.Cidade = bairro?.Cidade;
                            dados.Bairro = bairro;
                            dados.Cod_Area = request.Cod_Area;
                            dados.Cod_Escritorio = request.Cod_Escritorio;
                            dados.Cod_Localidade = Convert.ToString(request.Cod_Localidade);
                            dados.Tipo = request.Tipo;
                            dados.Tipo_Zona = request.Tipo_Zona;
                            dados.CodAuxiliar = request.Cod_Auxiliar;

                            await repoLogradouro.Save(dados);

                            return new ApiResponseObj<long>()
                            {
                                ok = true,
                                data = dados.Id,
                                error = ""
                            };
                        }
                    }
                    #endregion  
                }
            }

            catch (Exception erro)
            {
                CONSTANTES.log4Net.ErrorFormat("message : {0} \r\n InnerException : {1} StackTrace : {2}\r\n",
                                                erro.Message,
                                                erro?.InnerException?.Message,
                                                erro.StackTrace);

                return new ApiResponseObj<long>()
                {
                    ok = false,
                    data = 0,
                    error = erro.Message
                };
            }
        }

        [HttpPost]
        [Route("InsertOrUpdateCondominio")]
        public async Task<ApiResponseObj<long>> InsertOrUpdateCondominio(CondominioCadastrarApiRequest request)
        {
            List<Endereco_Logradouro> retorno = new List<Endereco_Logradouro>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Condominio_Predio dados = new Condominio_Predio();


                #region BindModel


                using (var session = HibernateUtil.GetSession())
                {
                    using (var repoCondominio = new Conexao<Condominio_Predio>(session))
                    using (var repoLogradouro = new Conexao<Endereco_Logradouro>(session))
                    {
                        var logradouro = await repoLogradouro.selectFirst(p => p.Id == request.IdLogradouro);
                        var condExistente = await repoCondominio.selectFirst(p => p.Id == request.IdCondominio);

                        if (logradouro == null)
                            throw new Exception("UF não encontrada para o Id informado");
                        if (request.Descricao == null)
                            throw new Exception("Descrição não informado");


                        if (condExistente != null)
                        {
                            condExistente.Descricao = request.Descricao;
                            condExistente.Endereco_Logradouro = logradouro;
                            condExistente.Numero = request.Numero;
                            condExistente.UsuarioAlteracao = request.settings.user;
                            condExistente.DataHoraAlteracao = DateTime.Now;

                            await repoCondominio.Update(condExistente);

                            return new ApiResponseObj<long> { ok = true, data = condExistente.Id, error = "" };
                        }
                        else
                        {
                            var lista = await buscaCondominio(request.Descricao, request.IdLogradouro);
                            if (lista.Any())
                                throw new Exception("Condominio já Cadastrado");

                            dados.UsuarioCriacao = request.settings.user;
                            dados.DataHoraCriacao = DateTime.Now;
                            dados.Descricao = request.Descricao;
                            dados.Numero = request.Numero;
                            dados.Endereco_Logradouro = logradouro;
                            dados.Status = Sinapse.Model.enumerados.Status_Default.Ativo;

                            await repoCondominio.Save(dados);

                            return new ApiResponseObj<long> { ok = true, data = dados.Id, error = "" };
                        }

                    }
                    #endregion  
                }
            }

            catch (Exception erro)
            {
                CONSTANTES.log4Net.ErrorFormat("message : {0} \r\n InnerException : {1} StackTrace : {2}\r\n",
                                                erro.Message,
                                                erro?.InnerException?.Message,
                                                erro.StackTrace);

                return new ApiResponseObj<long>()
                {
                    ok = false,
                    data = 0,
                    error = erro.Message
                };
            }
        }

        private async Task<List<ObjBusca<long>>> buscaBairro(string term, long? idUf, long? idCidade)
        {
            var retorno = new List<ObjBusca<long>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO");
                    sql.AppendLine("FROM TB_BAIRRO A");
                    sql.AppendLine("WHERE (UPPER(A.DESCRICAO) = @VALOR)");

                    parametros.Add("@VALOR", $"{(term ?? "").Trim().ToUpperInvariant()}");

                    if (idUf.HasValue)
                    {
                        sql.AppendLine("AND A.ID_UF = @IDUF");
                        parametros.Add("@IDUF", idUf.Value);
                    }
                    if (idCidade.HasValue)
                    {
                        sql.AppendLine("AND A.ID_CIDADE = @ID_CIDADE");
                        parametros.Add("@ID_CIDADE", idCidade.Value);
                    }


                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ObjBusca<long>
                            {
                                Value = Convert.ToInt64(reader["ID"]),
                                Label = Convert.ToString(reader["DESCRICAO"])
                            });
                        }
                    }
                }
            }

            return retorno;
        }

        private async Task<List<ObjBusca<long>>> busca(string term, long? idUf = null)
        {
            var retorno = new List<ObjBusca<long>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO");
                    sql.AppendLine("FROM TB_CIDADE A");
                    sql.AppendLine("WHERE (UPPER(A.DESCRICAO) = @VALOR)");

                    parametros.Add("@VALOR", $"{(term ?? "").Trim().ToUpperInvariant()}");

                    if (idUf.HasValue)
                    {
                        sql.AppendLine("AND A.ID_UF = @IDUF");
                        parametros.Add("@IDUF", idUf.Value);
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ObjBusca<long>
                            {
                                Value = Convert.ToInt64(reader["ID"]),
                                Label = Convert.ToString(reader["DESCRICAO"])
                            });
                        }
                    }
                }
            }

            return retorno;
        }
        private async Task<List<ObjBusca<long>>> buscaCondominio(string term, Int64? logradouro)
        {
            var retorno = new List<ObjBusca<long>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO");
                    sql.AppendLine(" FROM TB_CONDOMINIO_PREDIO  A");
                    sql.AppendLine("WHERE (UPPER(A.DESCRICAO) = @VALOR)");

                    parametros.Add("@VALOR", $"{(term ?? "").Trim().ToUpperInvariant()}");

                    if (logradouro.HasValue)
                    {
                        sql.AppendLine("AND A.ID_ENDERECO_LOGRADOURO = @ID_ENDERECO_LOGRADOURO");
                        parametros.Add("@ID_ENDERECO_LOGRADOURO", logradouro.Value);
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ObjBusca<long>
                            {
                                Value = Convert.ToInt64(reader["ID"]),
                                Label = Convert.ToString(reader["DESCRICAO"])
                            });
                        }
                    }
                }
            }

            return retorno;
        }
        private async Task<List<ObjBusca<long>>> buscaLogradouro(string term, Int64? uf, Int64? cidade, Int64? bairro)
        {
            var retorno = new List<ObjBusca<long>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO");
                    sql.AppendLine(" FROM TB_ENDERECO_LOGRADOURO  A");
                    sql.AppendLine("WHERE (UPPER(A.DESCRICAO) = @VALOR)");

                    parametros.Add("@VALOR", $"{(term ?? "").Trim().ToUpperInvariant()}");

                    if (bairro.HasValue)
                    {
                        sql.AppendLine("AND A.ID_BAIRRO = @ID_BAIRRO");
                        parametros.Add("@ID_BAIRRO", bairro.Value);
                    }
                    if (cidade.HasValue)
                    {
                        sql.AppendLine("AND A.ID_CIDADE = @ID_CIDADE");
                        parametros.Add("@ID_CIDADE", cidade.Value);
                    }
                    if (uf.HasValue)
                    {
                        sql.AppendLine("AND A.ID_UF = @ID_UF");
                        parametros.Add("@ID_UF", uf.Value);
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ObjBusca<long>
                            {
                                Value = Convert.ToInt64(reader["ID"]),
                                Label = Convert.ToString(reader["DESCRICAO"])
                            });
                        }
                    }
                }
            }

            return retorno;
        }


    }
}