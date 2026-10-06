using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Checklist;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static Aniel.Connect.ControllersWebApi.VendaApiController;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class ChecklistApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/ChecklistApi/GetAll")]
        public DataTableResponse<ChecklistListModel> GetAll(ChecklistDataTableRequest request)
        {
            var listagem = new ChecklistListData();

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);

                var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                listagem = checklistBusiness.GetListagemChecklist(request);
            }

            return new DataTableResponse<ChecklistListModel>
            {
                draw = request.draw,
                recordsFiltered = listagem.RecordsFiltered,
                data = listagem.Data,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/CheckListPerguntaapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_QUIZ_ANSWER_ITEM A WHERE A.ID IS NOT NULL");

                    List<String> condicoes = new List<String>();


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.PERGUNTA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.PERGUNTA ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["PERGUNTA"])
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

        [HttpPost]
        [Route("api/ChecklistApi/ExcluirQuestao")]
        public GetApiRetorno ExcluirQuestao(ExcluirQuestaoChecklistRequest request)
        {
            var retorno = new GetApiRetorno();
            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
                {
                    IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);

                    var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                    checklistBusiness.RemoveQuestao(request.IdChecklist, request.IdQuestao,User.Identity.GetUserId());
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/ChecklistApi/AlterarStatus")]
        public GetApiRetorno AlterarStatus(AlterarStatusChecklistRequest request)
        {
            var retorno = new GetApiRetorno();
            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
                {
                    IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);

                    var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                    foreach (long idChecklist in request.Ids)
                    {
                        checklistBusiness.AlterarStatus(idChecklist, request.Status);
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/ChecklistApi/FindById")]
        public GetApiRetorno<ChecklistCadastroModel> FindById(long id)
        {
            var retorno = new GetApiRetorno<ChecklistCadastroModel>();
            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
                {
                    IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);

                    var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                    retorno.Objeto = checklistBusiness.GetViewObj(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/ChecklistApi/Gravar")]
        public PostApiRetorno<long> Gravar([FromBody] ParamWebApi<ChecklistCadastroModel> request)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
                {
                    IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);

                    var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                    retorno.IdObjeto = checklistBusiness.GravarChecklist(request.Objeto);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/ChecklistApi/ChecklistPreenchido")]
        public GetApiRetorno<bool> ChecklistPreenchido(long idVenda)
        {
            var retorno = new GetApiRetorno<bool>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
                {
                    IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);

                    var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                    retorno.Objeto = checklistBusiness.ChecklistPreenchido(idVenda);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ChecklistApi/GetChecklistVinculoComercial")]
        public async Task<DataTableResponse<DatatableChecklistVenda>> GetChecklistVinculoComercial(DatatableChecklistRequest request)
        {
            var retorno = new DataTableResponse<DatatableChecklistVenda>();

            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();

            var listagem = new List<DatatableChecklistVenda>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var comm = session.Connection.CreateCommand())
            {
                try
                {

                    //SELECT 
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlCount = new StringBuilder();

                    StringBuilder sqlFrom = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();

                    if (request.Tipo_Pessoa.HasValue)
                    {
                        parametros.Add("@TIPO_PESSOA", request.Tipo_Pessoa);
                        condicoes.Add("A.TIPO_PESSOA = @TIPO_PESSOA ");
                    }

                    if (request.MeioCaptacao.HasValue)
                    {
                        parametros.Add("@MEIO_CAPTACAO", request.MeioCaptacao);
                        condicoes.Add(" A.ID_MEIO_CAPTACAO = @MEIO_CAPTACAO");
                    }

                    if (request.CategoriaCliente.HasValue)
                    {
                        parametros.Add("@CATEGORIA_CLIENTE", request.CategoriaCliente);
                        condicoes.Add("ID_CATEGORIA_CLIENTE = @CATEGORIA_CLIENTE");
                    }

                    if (request.Uf.HasValue)
                    {
                        parametros.Add("@UF", request.Uf);
                        condicoes.Add("A.ID_UF = @UF");
                    }

                    if (request.Cidade.HasValue)
                    {
                        parametros.Add("@CIDADE", request.Cidade);
                        condicoes.Add("A.ID_CIDADE = @CIDADE");
                    }

                    sqlCount.AppendLine("SELECT COUNT (A.ID) AS CONTADOR");

                    sql.AppendLine("SELECT A.ID, A.ID_CATEGORIA_CLIENTE, A.ID_MEIO_CAPTACAO, A.ID_UF, A.ID_CIDADE, A.TIPO, A.TIPO_PESSOA, A.ATIVO, A.TITULO, COUNT(B.ID) AS PERGUNTAS ");

                    #region From
                    sqlFrom.AppendLine("FROM TB_QUIZ A")
                           .AppendLine("INNER JOIN TB_QUIZ_QUESTION B ON B.ID_QUIZ = A.ID");

                    //sqlFrom.AppendLine("ORDER BY A.ID");
                    #endregion

                    #region Where 
                    sqlWhere.AppendLine("WHERE A.TIPO = 1");
                    if (condicoes.Count > 0)
                    {
                        sqlWhere.AppendLine($"AND {String.Join(" AND ", condicoes)}");
                    }
                    #endregion

                    sqlCount.AppendLine(sqlFrom.ToString())
                            .AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(comm, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {

                            retorno.recordsFiltered = reader["CONTADOR"] != DBNull.Value ? Convert.ToInt32(reader["CONTADOR"]) : 0;
                        }
                    }

                    sql.AppendLine(sqlFrom.ToString())
                       .AppendLine(sqlWhere.ToString())

                       .AppendLine("GROUP BY A.ID, A.ID_CATEGORIA_CLIENTE, A.ID_MEIO_CAPTACAO, A.ID_UF, A.ID_CIDADE, A.TIPO, A.TIPO_PESSOA, A.ATIVO, A.TITULO")
                       .AppendLine("ORDER BY A.ID");

                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            String ativo = reader["ATIVO"] != DBNull.Value ? Convert.ToString(reader["ATIVO"]) : String.Empty;

                            listagem.Add(new DatatableChecklistVenda
                            {
                                Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,

                                Tipo = reader["TIPO"] != DBNull.Value ? Convert.ToInt64(reader["TIPO"]) : 0,
                                Tipo_Pessoa = reader["TIPO_PESSOA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Sinapse.Model.enumerados.Tipo_Pessoa)reader["TIPO_PESSOA"]) : String.Empty,
                                Ativo = ativo == "S" ? RsCadastros.ATIVO : ativo == "N" ? RsCadastros.INATIVO : ativo,
                                Titulo = reader["TITULO"] != DBNull.Value ? Convert.ToString(reader["TITULO"]) : String.Empty,

                                IdMeioCaptacao = reader["ID_MEIO_CAPTACAO"] != DBNull.Value ? Convert.ToInt64(reader["ID_MEIO_CAPTACAO"]) : (long?)null,
                                CategoriaCliente = reader["ID_CATEGORIA_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CATEGORIA_CLIENTE"]) : (long?)null,
                                Id_Uf = reader["ID_UF"] != DBNull.Value ? Convert.ToInt64(reader["ID_UF"]) : (long?)null,
                                Id_Cidade = reader["ID_UF"] != DBNull.Value ? Convert.ToInt64(reader["ID_UF"]) : (long?)null,
                                Perguntas = reader["PERGUNTAS"] != DBNull.Value ? Convert.ToInt64(reader["PERGUNTAS"]) : 0,
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    listagem.Clear();
                    retorno.error = ex.Message;
                }
                finally
                {
                    retorno.data = listagem;
                }
                return retorno;
            }
        }

        [HttpGet]
        [Route("api/ChecklistApi/GetChecklistVenda")]
        public async Task<DataTableResponse<ChecklistListaModel>> GetCheckListVenda(long IdVenda)
        {
            List<ChecklistListaModel> retorno = new List<ChecklistListaModel>();

            GetApiRetorno<List<QuizAnswer>> lista = await new QuizApiController().GetQuizAnswerVenda(IdVenda);

            foreach (var item in lista.Objeto)
            {
                ChecklistListaModel novo = new ChecklistListaModel();
                var criacao = item.diaInicio;

                if (criacao.HasValue && item.horaInicio.HasValue)
                    criacao = criacao.Value.Add(item.horaInicio.Value.TimeOfDay);

                novo.Id = item.idA.ToString();
                novo.Titulo = item.titulo;
                novo.Tipo = item.tituloQuiz;
                novo.Perguntas = item.respostas.Count.ToString();
                novo.Respondidas = item.respostas.Where(x => x.resposta != null || x.multi.Count > 0).Count().ToString();
                novo.UsuarioCriacao = item.usuario;
                novo.UsuarioUltimaAlteracao = "";
                novo.DataHoraCriacao = criacao.HasValue ? criacao?.ToString("dd/MM/yyyy HH:mm") : String.Empty;
                novo.DataHoraUltimaAlteracao = "";

                retorno.Add(novo);
            }

            return new DataTableResponse<ChecklistListaModel> { data = retorno.OrderBy(x => x.Titulo), recordsFiltered = retorno.Count };
        }

        [HttpPost()]
        [Route("api/CheckListApi/Modelo01")]
        public async Task<DataTableResponse<dynamic>> RelatorioCheckListModelo01(RelatorioCheckList request)
        {
            DateTime dataInicio = DateTime.ParseExact(request.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dataFim = DateTime.ParseExact(request.DataFim, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            try
            {

                List<dynamic> retorno = new List<dynamic>();
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        StringBuilder sql = new StringBuilder();

                        parametros.Add("DATA_INI", dataInicio);
                        parametros.Add("DATA_FIM", dataFim);

                        sql.AppendLine(" SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.DATA_MAXIMA, " +
                                         " A.EQUIPE || ' ' || E.NOME AS EQUIPE, A.COD_TIPO_SERV, F.DESCRICAO AS TIPO_SERVICO, " +
                                         " A.COD_SUB_TIPO_SERV, G.DESCRICAO AS SUB_TIPO_SERVICO, A.TITULAR AS CLIENTE, " +
                                         " C.TITULO, C.PROTOCOLO, D.PERGUNTA,D.ORDEM, CAST(COALESCE(RESPOSTA, RESPOSTA_TEXTO, RESPOSTA_VALOR, RESPOSTA_DATA, NULL) AS VARCHAR(300)) AS RESPOSTA,D.ID  ");

                        sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                        sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_DOCPROD B ");
                        sql.AppendLine(" ON(A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND A.NUM_DOC = B.NUM_DOC) ");
                        sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_CAPA C ");
                        sql.AppendLine(" ON(B.ID_ANSWER_CAPA = C.ID) ");
                        sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_ITEM D");
                        sql.AppendLine(" ON(C.ID = D.ID_ANSWER_CAPA)  ");
                        sql.AppendLine(" LEFT OUTER JOIN TB_EQUIPE E ");
                        sql.AppendLine(" ON(A.EQUIPE = E.EQUIPE) ");
                        sql.AppendLine(" LEFT OUTER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                        sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                        sql.AppendLine(" LEFT OUTER JOIN TB_SUB_TIPO_SERVICO G ");
                        sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) LEFT JOIN TB_QUIZ Q ON (Q.ID = C.ID_QUIZ) ");
                        sql.AppendLine(" WHERE D.TIPO <> 3  ");
                        //sql.AppendLine(" ORDER BY A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, C.PROTOCOLO, D.ORDEM ");


                        if (!String.IsNullOrEmpty(request.Contrato))
                        {
                            sql.AppendLine("AND (A.CODCT = @CODCT)");
                            parametros.Add("@CODCT", request.Contrato);
                        }

                        if (!String.IsNullOrEmpty(request.Projeto))
                        {
                            sql.AppendLine("AND (A.PROJETO = @PROJETO)");
                            parametros.Add("@PROJETO", request.Projeto);
                        }

                        if (!String.IsNullOrEmpty(request.Tecnico))
                        {
                            sql.AppendLine("AND (E.EQUIPE = @TECNICO)");
                            parametros.Add("@TECNICO", request.Tecnico);
                        }

                        if (!String.IsNullOrEmpty(request.Questionario))
                        {
                            sql.AppendLine("AND (C.ID_QUIZ = @ID_QUIZ)");
                            parametros.Add("@ID_QUIZ", request.Questionario);
                        }

                        if (!String.IsNullOrEmpty(request.Nos))
                        {
                            sql.AppendLine($"AND (A.NUM_OBRA LIKE '%{request.Nos}%')");
                        }

                        if (!String.IsNullOrEmpty(request.Tipo_Servico))
                        {
                            sql.AppendLine("AND (F.COD_TIPO_SERV = @COD_TIPO_SERV)");
                            parametros.Add("@COD_TIPO_SERV", request.Tipo_Servico);
                        }

                        sql.AppendLine("AND CAST(A.DATA_MAXIMA AS date)  BETWEEN @DATA_INI AND @DATA_FIM");

                        sql.AppendLine(" ORDER BY A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, C.PROTOCOLO, D.ORDEM ");

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new
                                {
                                    CodCt = Convert.ToString(reader["CODCT"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                    Num_Doc = Convert.ToString(reader["NUM_DOC"]),
                                    DATA = Convert.ToString(reader["DATA_MAXIMA"]),
                                    Tecnico = Convert.ToString(reader["EQUIPE"]),
                                    Tipo_Servico = Convert.ToString(reader["TIPO_SERVICO"]),
                                    Sub_Tipo_Servico = Convert.ToString(reader["SUB_TIPO_SERVICO"]),
                                    Cliente = Convert.ToString(reader["CLIENTE"]),
                                    Titulo = Convert.ToString(reader["TITULO"]),
                                    Protocolo = Convert.ToString(reader["PROTOCOLO"]),
                                    Ordem = Convert.ToString(reader["ORDEM"]),
                                    Pergunta = Convert.ToString(reader["PERGUNTA"]),
                                    Resposta = Convert.ToString(reader["RESPOSTA"]),
                                });
                            }
                        }

                    }
                }

                return new DataTableResponse<dynamic>
                {
                    data = retorno,
                    recordsFiltered = retorno.Count
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponse<dynamic>
                {
                    data = null,
                    recordsFiltered = 0
                };
            }  
        }

        [HttpPost()]
        [Route("api/CheckListApi/Modelo02")]
        public async Task<DataTableResponse<dynamic>> RelatorioCheckListModelo02(RelatorioCheckList request)
        {
            DateTime dataInicio = DateTime.ParseExact(request.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dataFim = DateTime.ParseExact(request.DataFim, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            List<dynamic> retorno = new List<dynamic>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();

                    parametros.Add("DATA_INI", dataInicio.Date);
                    parametros.Add("DATA_FIM", dataFim.Date);

                    sql.AppendLine("SELECT DISTINCT A.CODCT, A.PROJETO, A.NUM_OBRA_ORIGINAL, A.NUM_DOC,");
                    sql.AppendLine("    CAST(A.DATA_PROD ||' - '|| A.HORA_ENCERRAMENTO AS TIMESTAMP) AS DATA_HORAENC,");
                    sql.AppendLine("    A.NUM_OBRA_ORIGINAL,");
                    sql.AppendLine("    (SELECT I.SGCI FROM  TB_MANUTENCAO_OT_OR I");
                    sql.AppendLine("    WHERE I.CODCT = A.CODCT AND I.PROJETO = A.PROJETO AND I.NUM_OT_OR = A.NUM_OBRA)  AS SGCI,");
                    sql.AppendLine("    A.DIA AS DATACRIACAO,");
                    sql.AppendLine("    A.EQUIPE ||' - '|| E.NOME AS EQUIPE,");
                    sql.AppendLine("    (SELECT EN.DESCRICAO FROM TB_CODIGO_ENCERRAMENTO EN WHERE EN.CODCT = A.CODCT AND EN.COD_ENCERRAMENTO = A.COD_ENCERRAMENTO) AS COD_ENCERRAMENTO,");
                    sql.AppendLine("    CAST((SELECT FIRST 1 LIST(AA.CODMAT||' - '|| BB.DESCRICAO||' - '||BB.UNID ||' - '||CAST(AA.QUANTIDADE AS DECIMAL(18, 2)),', ')  FROM TB_MATERIAL_TAREFA AA");
                    sql.AppendLine("    INNER JOIN TB_MATERIAL BB ON BB.CODMAT = AA.CODMAT");
                    sql.AppendLine("    WHERE AA.CODCT = A.CODCT AND AA.PROJETO = A.PROJETO AND AA.NUM_OBRA = A.NUM_OBRA ORDER BY 1) AS VARCHAR(1000)) AS MATERIAL,");
                    sql.AppendLine("    L.LOCALIDADE");
                    sql.AppendLine("FROM");
                    sql.AppendLine("    TB_DOCUMENTO_PRODUCAO A");
                    sql.AppendLine("LEFT JOIN TB_LOCALIDADE L ON (L.COD_LOCALIDADE = A.COD_LOCALIDADE AND L.CODCT = A.CODCT");
                    sql.AppendLine("     AND L.PROJETO = A.PROJETO AND L.COD_ESCRITORIO = A.COD_ESCRITORIO)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_QUIZ_ANSWER_DOCPROD B ON (A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND A.NUM_DOC = B.NUM_DOC)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_QUIZ_ANSWER_CAPA C ON (B.ID_ANSWER_CAPA = C.ID)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_QUIZ_ANSWER_ITEM D ON (C.ID = D.ID_ANSWER_CAPA)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_EQUIPE E ON (A.EQUIPE = E.EQUIPE)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_TIPO_SERVICO_EQUIPE F ON (A.COD_TIPO_SERV = F.COD_TIPO_SERV)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_SUB_TIPO_SERVICO G ON (A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV)");
                    sql.AppendLine("LEFT JOIN");
                    sql.AppendLine("    TB_QUIZ Q ON (Q.ID = C.ID_QUIZ)");
                    sql.AppendLine("WHERE D.TIPO <> 3");

                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sql.AppendLine("AND (A.CODCT = @CODCT)");
                        parametros.Add("@CODCT", request.Contrato);
                    }

                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        sql.AppendLine("AND (A.PROJETO = @PROJETO)");
                        parametros.Add("@PROJETO", request.Projeto);
                    }

                    if (!String.IsNullOrEmpty(request.Tecnico))
                    {
                        sql.AppendLine("AND (E.EQUIPE = @TECNICO)");
                        parametros.Add("@TECNICO", request.Tecnico);
                    }

                    if (!String.IsNullOrEmpty(request.Questionario))
                    {
                        sql.AppendLine("AND (C.ID_QUIZ = @ID_QUIZ)");
                        parametros.Add("@ID_QUIZ", request.Questionario);
                    }

                    if (!String.IsNullOrEmpty(request.Nos))
                    {
                        sql.AppendLine($"AND (A.NUM_OBRA LIKE '%{request.Nos}%')");
                    }

                    if (!String.IsNullOrEmpty(request.Tipo_Servico))
                    {
                        sql.AppendLine("AND (F.COD_TIPO_SERV = @COD_TIPO_SERV)");
                        parametros.Add("@COD_TIPO_SERV", request.Tipo_Servico);
                    }

                    sql.AppendLine("    AND CAST(A.DATA_MAXIMA AS date)  BETWEEN @DATA_INI AND @DATA_FIM");

                    sql.AppendLine("ORDER BY A.DATA_PROD, A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, D.ORDEM;");

                    //Busca com paginacao
                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var numDoc = Convert.ToString(reader["NUM_DOC"]);
                            var numObraOriginal = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]);

                            var listaRespostas = await BuscaPerguntaResposta(request.Questionario, numDoc, numObraOriginal);

                            retorno.Add(new
                            {
                                carimboDtHr = Convert.ToString(reader["DATA_HORAENC"]),
                                num_Os = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                                data = reader["DATACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATACRIACAO"]).ToString("dd/MM/yyyy") : "",
                                tecnico = Convert.ToString(reader["EQUIPE"]),
                                sgci = Convert.ToString(reader["SGCI"]),
                                codEncerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                materiais = Convert.ToString(reader["MATERIAL"]),
                                localidade = Convert.ToString(reader["LOCALIDADE"]),
                                listaRespostas = listaRespostas,
                            });
                        }

                        retorno = retorno.OrderBy(p => p.data).ToList();
                    }

                }
            }

            return new DataTableResponse<dynamic>
            {
                data = retorno,
                recordsFiltered = retorno.Count
            };
        }

        private async Task<List<dynamic>> BuscaPerguntaResposta(string idQuiz, string numDoc, string numObraOriginal)
        {
            List<dynamic> retorno = new List<dynamic>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine("SELECT D.PERGUNTA,D.ORDEM, CAST(COALESCE(RESPOSTA, RESPOSTA_TEXTO, RESPOSTA_VALOR, RESPOSTA_DATA, NULL) AS VARCHAR(300)) AS RESPOSTA,D.ID");
                    sql.AppendLine("FROM");
                    sql.AppendLine("    TB_DOCUMENTO_PRODUCAO A");
                    sql.AppendLine("LEFT JOIN TB_LOCALIDADE L ON (L.COD_LOCALIDADE = A.COD_LOCALIDADE AND L.CODCT = A.CODCT");
                    sql.AppendLine("     AND L.PROJETO = A.PROJETO AND L.COD_ESCRITORIO = A.COD_ESCRITORIO)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_QUIZ_ANSWER_DOCPROD B ON (A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND A.NUM_DOC = B.NUM_DOC)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_QUIZ_ANSWER_CAPA C ON (B.ID_ANSWER_CAPA = C.ID)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_QUIZ_ANSWER_ITEM D ON (C.ID = D.ID_ANSWER_CAPA)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_EQUIPE E ON (A.EQUIPE = E.EQUIPE)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_TIPO_SERVICO_EQUIPE F ON (A.COD_TIPO_SERV = F.COD_TIPO_SERV)");
                    sql.AppendLine("LEFT OUTER JOIN");
                    sql.AppendLine("    TB_SUB_TIPO_SERVICO G ON (A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV)");
                    sql.AppendLine("LEFT JOIN");
                    sql.AppendLine("    TB_QUIZ Q ON (Q.ID = C.ID_QUIZ)");
                    sql.AppendLine("WHERE D.TIPO <> 3");
                    sql.AppendLine("  AND Q.ID = @ID_QUIZ AND A.NUM_DOC = @NUM_DOC AND A.NUM_OBRA_ORIGINAL = @NUM_OBRA");

                    parametros.Add("@NUM_DOC", numDoc);
                    parametros.Add("@ID_QUIZ", idQuiz);
                    parametros.Add("@NUM_OBRA", numObraOriginal);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new
                            {
                                pergunta = Convert.ToString(reader["PERGUNTA"]),
                                resposta = Convert.ToString(reader["RESPOSTA"]),
                                numObraOriginal = numObraOriginal,
                                ordem = Convert.ToString(reader["ORDEM"]),
                            });
                        }

                        retorno = retorno.OrderBy(p => Convert.ToInt16(p.ordem)).ToList();
                    }

                }
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/ChecklistApi/ExcluirChecklistVenda")]
        public async Task<PostApiRetorno> ExcluirChecklist(int id)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            string idCapa = "";

            try
            {
                if (id != 0)
                {
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sqlBusca = new StringBuilder();


                        sqlBusca.AppendLine(" SELECT ID_ANSWER_CAPA FROM TB_QUIZ_ANSWER_VENDA WHERE ID = @id");
                        parametros.Add("@id", id);

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlBusca, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                idCapa = reader["ID_ANSWER_CAPA"].ToString();
                            }
                        }

                        parametros.Add("@idRes", idCapa);


                        StringBuilder sqlDeleteVenda = new StringBuilder();

                        sqlDeleteVenda.AppendLine(" DELETE FROM TB_QUIZ_ANSWER_VENDA WHERE ID = @id ");

                        await CommandHelper.ExecuteNonQuery(com, sqlDeleteVenda, parametros, sessionT5.Connection, null);

                        if (!string.IsNullOrEmpty(idCapa))
                        {
                            StringBuilder sqlDeleteCapa = new StringBuilder();

                            sqlDeleteCapa.AppendLine(" DELETE FROM TB_QUIZ_ANSWER_CAPA WHERE ID = @idRes ");

                            await CommandHelper.ExecuteNonQuery(com, sqlDeleteCapa, parametros, sessionT5.Connection, null);
                        }
                    }
                }

                return new PostApiRetorno { Ok = true };
            }
            catch (Exception e)
            {
                return new PostApiRetorno { Ok = false, Mensagem = e.Message, InnerException = e.InnerException.Message };
            }
        }

        [HttpPost()]
        [Route("api/ChecklistApi/GetComboTipo")]
        public async Task<ObjCombo<Int64>> GetComboTipoComercial(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_QUIZ A WHERE A.ID IS NOT NULL");

                    List<String> condicoes = new List<String>();


                    if (request.id.HasValue)
                    {
                        sqlFrom.AppendLine($" AND A.ID = {request.id}");
    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        sqlFrom.AppendLine($" AND ( UPPER(A.TITULO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%') ");
                    }

                    sqlFrom.AppendLine(" AND A.TIPO = 1 AND A.ATIVO = 'S' ");

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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.TITULO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["TITULO"])
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



        public class ChecklistComboResult : IObjComboResult<Int64>
        {
            public long Id { get; set; }
            public string Text { get; set; }
        }
    }
}