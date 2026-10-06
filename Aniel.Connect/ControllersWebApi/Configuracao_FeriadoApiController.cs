using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
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
    public class FeriadoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Feriadoapi/GetAll")]
        public async Task<DataTableResponse<FeriadoListaModel>> GetAll(Feriado_DataTableRequest request)
        {
            List<FeriadoListaModel> retorno = new List<FeriadoListaModel>();
            IOrderedEnumerable<FeriadoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_FERIADO"); // WHERE STATUS = 1

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

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.DATA, A.TIPO, A.STATUS, A.PAIS, A.UF, A.CIDADE,A.ABRANGENCIA ");
                    sql.AppendLine(" FROM TB_FERIADO A ");
                    sql.AppendLine(" WHERE A.ID > 0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (request.TipoFiltro.HasValue)
                    {
                        sql.AppendLine(" AND A.TIPO = @TIPO ");
                        parametros.Add("@TIPO", request.TipoFiltro.Value);
                    }

                    if (request.AbrangenciaFiltro.HasValue)
                    {
                        sql.AppendLine(" AND A.ABRANGENCIA = @ABRANGENCIA ");
                        parametros.Add("@ABRANGENCIA", request.AbrangenciaFiltro.Value);

                    }

                    if (request.UF_Filtro.HasValue)
                    {
                        sql.AppendLine(" AND A.UF = @UF ");
                        parametros.Add("@UF", request.UF_Filtro.Value);

                    }

                    if (!String.IsNullOrEmpty(request.Cidade_Filtro))
                    {
                        sql.AppendLine(" AND (UPPER(A.CIDADE) LIKE UPPER(@CIDADE)) ");
                        parametros.Add("@CIDADE", String.Format("{0}", request.Cidade_Filtro));
                    }

                    if (!String.IsNullOrEmpty(request.Pais_Filtro))
                    {
                        sql.AppendLine(" AND (UPPER(A.PAIS) LIKE UPPER(@PAIS)) ");
                        parametros.Add("@PAIS", String.Format("{0}", request.Pais_Filtro));
                    }

                    if (request.Status_Filtro.HasValue)
                    {
                        sql.AppendLine(" AND A.STATUS IN(@STATUS)");
                        parametros.Add("@STATUS", request.Status_Filtro.Value);
                    }

                    if (!String.IsNullOrEmpty(request.Data_Inicial) && !String.IsNullOrEmpty(request.Data_Final))
                    {
                        sql.AppendLine("AND (EXTRACT(MONTH FROM A.DATA) * 100) + ")
                           .AppendLine(" EXTRACT(DAY FROM A.DATA) BETWEEN @DATAINI AND @DATAFIM ");

                        DateTime DataIni = Convert.ToDateTime(request.Data_Inicial);
                        DateTime DataFim = Convert.ToDateTime(request.Data_Final);

                        parametros.Add("@DATAINI", (DataIni.Month * 100) + DataIni.Day);
                        parametros.Add("@DATAFIM", (DataFim.Month * 100) + DataFim.Day);
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new FeriadoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Data = Convert.ToDateTime(reader["DATA"]).ToString("dd/MM"),
                                DataOrdem = Convert.ToInt64(Convert.ToDateTime(reader["DATA"]).ToString("MMdd")),
                                Tipo = HtmlEnumExtensions.GetEnumDescription<Tipo_Feriado>(Convert.ToString(reader["TIPO"])),
                                Abrangencia = HtmlEnumExtensions.GetEnumDescription<Abrangencia>(Convert.ToString(reader["ABRANGENCIA"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Pais = Convert.ToString(reader["PAIS"]),
                                UF = Convert.ToString(reader["UF"]),
                                Cidade = Convert.ToString(reader["CIDADE"])
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
                    //case 1:     // CODIGO
                    //    {
                    //        if (sortDirection == "asc")
                    //            listaOrdenada = retorno.OrderBy(p => p.Codigo);
                    //        else
                    //            listaOrdenada = retorno.OrderByDescending(p => p.Codigo);
                    //        break;
                    //    }
                    case 1:     // DESCRICAO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                    case 2:     // DATA
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.DataOrdem);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.DataOrdem);
                            break;
                        }
                    case 3:     // TIPO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Tipo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tipo);
                            break;
                        }
                    case 4:     // STATUS
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                    case 5:     // STATUS
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                    case 6:     // UF
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.UF);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.UF);
                            break;
                        }
                    case 7:     // CIDADE
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cidade);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cidade);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<FeriadoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        /// <summary>
        ///     Retorna os dias de feriado de um mês de um determinado ano
        /// </summary>
        public async Task<IEnumerable<int>> GetDiasFeriadosMesAno(int mes, int ano)
        {
            var diasFeriado = new List<int>();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                var sql = new StringBuilder();
                var param = new Dictionary<string, object>();

                sql.AppendLine("SELECT EXTRACT(DAY FROM F.DATA) AS DIA");
                sql.AppendLine("FROM TB_FERIADO F");
                sql.AppendLine("WHERE F.STATUS = 1");
                sql.AppendLine("  AND EXTRACT(MONTH FROM F.DATA) = @MES");
                sql.AppendLine("  AND (CASE");
                // Caso o feriado seja fixo, o ano cadastrado é desconsiderado pois o feriado ocorre todo ano
                sql.AppendLine("    WHEN F.TIPO = 0 THEN @ANO");
                // Caso o feriado seja flexível, o ano cadastrado é utilizado na comparação
                sql.AppendLine("    WHEN F.TIPO = 1 THEN EXTRACT(YEAR FROM F.DATA)");
                sql.AppendLine("END) = @ANO");

                param.Add("@MES", mes);
                param.Add("@ANO", ano);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                {
                    while (reader.Read())
                    {
                        diasFeriado.Add(Convert.ToInt32(reader["DIA"]));
                    } 
                }
            }

            return diasFeriado;
        }

        private FeriadoCadastroModel GetViewObj(ISession session, Feriado obj)
        {
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoCidade = new Conexao<Cidade>(session))            
            {
                var view = new FeriadoCadastroModel();

                var Uf = repoUf.select(p => p.Sigla == obj.UF).Result;
                var cidade = obj.Cidade != null ? repoCidade.selectFirst(p => p.Descricao == obj.Cidade && p.Id_UF == Uf).Result : null;
                var pais = repoPais.selectFirst(p => p.Nome == obj.Pais).Result;

                view.Id = obj.Id;
                view.Tipo = obj.Tipo_Feriado;
                
                if (view.Tipo == Tipo_Feriado.Fixo)
                    view.Data = obj.Data.Value.ToShortDateString();
                else
                    view.DataVariavel = obj.Data.Value.ToShortDateString();
                
                view.Ano = obj.Data.Value.Year.ToString();
                
                view.Descricao = obj.Descricao;
                view.Abrangencia = obj.Abrangencia;
                view.Status = obj.Status == Status_Default.Ativo;
                view.UF = obj.UF == "" ? null : (UF?)Enum.Parse(typeof(UF), obj.UF);
                view.Pais = pais?.Id;
                view.Id_Cidade = cidade != null ? cidade.Id : (Int64?)null;
                
                return view;
            }
        }

        private void BindViewToObj(ISession session, ref Feriado objBanco, FeriadoCadastroModel view)
        {
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            {

                objBanco.Descricao = view.Descricao;
                objBanco.Tipo_Feriado = view.Tipo;
                objBanco.Abrangencia = view.Abrangencia;
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
                
                if (objBanco.Tipo_Feriado.Value == Tipo_Feriado.Fixo)
                {
                    string data = view.Data + "/" + DateTime.Now.Date.Year;
                    objBanco.Data = Convert.ToDateTime(data);
                }
                else
                {
                    objBanco.Data = Convert.ToDateTime(view.DataVariavel);
                }

                objBanco.Pais = view.Pais.HasValue ? repoPais.selectFirst(p => p.Id == Convert.ToInt64(view.Pais)).Result.Nome : String.Empty;

                objBanco.UF = view.UF.HasValue ? HtmlEnumExtensions.GetEnumDescription<UF>(view.UF.Value) : String.Empty;

                var _cidadeObj = view.Id_Cidade.HasValue ? repoCidade.selectFirst(p =>p.Id == view.Id_Cidade.Value).Result : null;
                objBanco.Cidade = _cidadeObj != null ? _cidadeObj.Descricao : null;
            }
        }

        [HttpPost()]
        [Route("api/Feriadoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_FERIADO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Feriadoapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_FERIADO WHERE ID IN {_ids}");

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
        [Route("api/FeriadoApi/FindById")]
        public async Task<GetApiRetorno<FeriadoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<FeriadoCadastroModel> retorno = new GetApiRetorno<FeriadoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Feriado>(session))
            {
                try
                {
                    Feriado feriado = await repository.selectFirst(p => p.Id == Id);

                    retorno.Objeto = GetViewObj(session, feriado);
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
        [Route("api/FeriadoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<FeriadoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Feriado modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Feriado>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Feriado
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        Int32 _feriados = repository.queryover().Where(p => p.Data == modelo.Data && p.Id != modelo.Id).List().Count;

                        Int32 _outros_feriados = repository.queryover().Where(
                                                                                                   p => p.Data == modelo.Data &&
                                                                                                   p.Id != modelo.Id &&
                                                                                                   p.Abrangencia != modelo.Abrangencia).List().Count;

                        switch (modelo.Abrangencia)
                        {
                            case Abrangencia.Estadual:

                                Int32 _feriados_uf = repository.queryover().Where(
                                                                                                    p => p.Data == modelo.Data &&
                                                                                                    p.Id != modelo.Id &&
                                                                                                    p.Abrangencia == modelo.Abrangencia &&
                                                                                                    p.UF == modelo.UF).List().Count;

                                if (_feriados_uf == 0 && _outros_feriados == 0)
                                    await repository.SaveOrUpdate(modelo);
                                else
                                    throw new Exception(RsCadastros.FERIADO_JA_CADASTRO);

                                break;

                            case Abrangencia.Municipal:

                                Int32 _feriados_municipais = repository.queryover().Where(
                                                                                                    p => p.Data == modelo.Data &&
                                                                                                    p.Id != modelo.Id &&
                                                                                                    p.Abrangencia == modelo.Abrangencia &&
                                                                                                    p.Cidade == modelo.Cidade).List().Count;

                                if (_feriados_municipais == 0 && _outros_feriados == 0)
                                    await repository.SaveOrUpdate(modelo);
                                else
                                    throw new Exception(RsCadastros.FERIADO_JA_CADASTRO);

                                break;

                            default:

                                if (_feriados == 0)
                                    await repository.SaveOrUpdate(modelo);
                                else
                                    throw new Exception(RsCadastros.FERIADO_JA_CADASTRO);

                                break;
                        }

                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<FeriadoAniel>(session))
                    {
                        FeriadoAniel feriado = (await repository.select(p => p.Id == modelo.Id)).FirstOrDefault();

                        if (feriado == null)
                        {
                            feriado = new FeriadoAniel();
                        }

                        feriado.Abrangencia = modelo.Abrangencia;
                        feriado.Cidade = modelo.Cidade;
                        feriado.Data = modelo.Data;
                        feriado.DataHoraAlteracao = modelo.DataHoraAlteracao;
                        feriado.DataHoraCriacao = modelo.DataHoraCriacao;
                        feriado.Descricao = modelo.Descricao;
                        feriado.Id = modelo.Id;
                        feriado.Pais = modelo.Pais;
                        feriado.Status = modelo.Status;
                        feriado.Tipo_Feriado = modelo.Tipo_Feriado;
                        feriado.Token = modelo.Token;
                        feriado.UF = modelo.UF;
                        feriado.UsuarioAlteracao = modelo.UsuarioAlteracao;
                        feriado.UsuarioCriacao = modelo.UsuarioCriacao;

                        await repository.SaveOrUpdate(feriado);
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

        [Route("api/FeriadoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Feriado>(session))
                    {

                        Feriado modelo = null;
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


        /// <summary>
        /// Retorna os dias dos feriados no mes/ano
        /// </summary>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/FeriadoApi/getList")]
        public async Task<GetApiRetorno<List<Int32>>> GetList(Int32 mes)
        {
            GetApiRetorno<List<Int32>> retorno = new GetApiRetorno<List<Int32>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var repository = new Conexao<Feriado>(session))
                    {

                        List<Feriado> lista = (await (repository.select(p => p.Data.Value.Month == mes  && p.Abrangencia == Abrangencia.Nacional))).ToList();

                        retorno.Objeto = lista.GroupBy(p => p.Data).Select(p => p.Key.Value.Day).ToList();

                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

    }
}
