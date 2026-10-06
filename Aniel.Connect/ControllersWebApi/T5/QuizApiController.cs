using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using DocumentFormat.OpenXml.Spreadsheet;
using FirebirdSql.Data.FirebirdClient;
using NHibernate;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class QuizApiController : BaseApiController
    {

        [HttpPost]
        [Route("api/Quizapi/ExcluirFoto")]
        public async Task<PostApiRetorno> ExcluirFoto(string id, string idItem)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
                using (var com = session.Connection.CreateCommand())
                using (var comDelete = sessionFoto.Connection.CreateCommand())
                {
                    string idFoto = "";
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlDelete = new StringBuilder();

                    var item = await GetQuizAnswerItem(session, sessionFoto, null, Convert.ToInt64(idItem));

                    if (item.FirstOrDefault()?.tipo == EnumQuizTipo.AssinaturaCliente)
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>()
                        {
                            { "@ID",idItem }
                        };

                        sql = new StringBuilder("UPDATE TB_QUIZ_ANSWER_ITEM SET ID_ASSINATURA = NULL WHERE ID = @ID");
                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                    }
                    else
                    {

                        Dictionary<String, Object> parametros = new Dictionary<string, object>()
                        {
                            { "id",id }
                        };

                        sql.AppendLine(" SELECT RESPOSTA_FOTO FROM TB_QUIZ_ANSWER_ITEM_MULTI WHERE ID = @id ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                idFoto = reader["RESPOSTA_FOTO"].ToString();
                            }

                            parametros.Add("idFoto", idFoto);
                            sqlDelete.AppendLine("DELETE FROM TB_QUIZ_FOTO WHERE ID = @idFoto");

                            sql = new StringBuilder("DELETE FROM TB_QUIZ_ANSWER_ITEM_MULTI WHERE ID = @id");

                            await CommandHelper.ExecuteNonQuery(comDelete, sqlDelete, parametros, sessionFoto.Connection, null);
                            await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                        }
                    }
                }
                return new PostApiRetorno { Ok = true };
            }
            catch (Exception e)
            {
                return new PostApiRetorno { Ok = false, Mensagem = e.Message };
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Quizapi/SalvaTextoChecklist")]
        public async Task<PostApiRetorno> SalvaTextoChecklist(SalvarQuiz salvar)
        {
            if (salvar == null)
            {
                return new PostApiRetorno { Ok = true };
            }

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql;

                    foreach (var item in salvar.Respostas)
                    {
                        var parametros = new Dictionary<string, object>();
                        sql = new StringBuilder();

                        parametros.Add("ID", item[0]);
                        parametros.Add("RESPOSTA", item[1]);

                        sql.AppendLine(" UPDATE TB_QUIZ_ANSWER_ITEM SET RESPOSTA_TEXTO = @RESPOSTA WHERE ID = @ID ");

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }
                }
                return new PostApiRetorno { Ok = true };
            }
            catch (Exception e)
            {
                return new PostApiRetorno { Ok = false, Mensagem = e.Message };
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Quizapi/SalvaValorChecklist")]
        public async Task<PostApiRetorno> SalvaValorChecklist(SalvarQuiz salvar)
        {
            if (salvar == null)
            {
                return new PostApiRetorno { Ok = true };
            }

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql;

                    foreach (var item in salvar.Respostas)
                    {
                        var parametros = new Dictionary<string, object>();
                        sql = new StringBuilder();

                        string respostaStr = item[1];

                        if (!String.IsNullOrEmpty(respostaStr))
                        {
                            double resposta;

                            if (!Double.TryParse(respostaStr, out resposta))
                            {
                                throw new Exception(String.Format("O valor \"{0}\" não é um número decimal válido"));
                            }

                            sql.AppendLine(" UPDATE TB_QUIZ_ANSWER_ITEM SET RESPOSTA_VALOR = @RESPOSTA WHERE ID = @ID ");

                            parametros.Add("ID", item[0]);
                            parametros.Add("RESPOSTA", resposta);
                        }
                        else
                        {
                            sql.AppendLine(" UPDATE TB_QUIZ_ANSWER_ITEM SET RESPOSTA_VALOR = NULL WHERE ID = @ID ");

                            parametros.Add("ID", item[0]);
                        }

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }
                }
                return new PostApiRetorno { Ok = true };
            }
            catch (Exception e)
            {
                return new PostApiRetorno { Ok = false, Mensagem = e.Message };
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Quizapi/SalvaDataChecklist")]
        public async Task<PostApiRetorno> SalvaDataChecklist(SalvarQuiz salvar)
        {
            if (salvar == null)
            {
                return new PostApiRetorno { Ok = true };
            }

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql;

                    foreach (var item in salvar.Respostas)
                    {
                        var parametros = new Dictionary<string, object>();
                        sql = new StringBuilder();

                        string respostaStr = item[1];

                        if (!String.IsNullOrEmpty(respostaStr))
                        {
                            DateTime resposta;

                            if (!DateTime.TryParse(respostaStr, out resposta))
                            {
                                throw new Exception(String.Format("O valor \"{0}\" não é uma data válida"));
                            }

                            sql.AppendLine(" UPDATE TB_QUIZ_ANSWER_ITEM SET RESPOSTA_DATA = @RESPOSTA WHERE ID = @ID ");

                            parametros.Add("ID", item[0]);
                            parametros.Add("RESPOSTA", resposta);
                        }
                        else
                        {
                            sql.AppendLine(" UPDATE TB_QUIZ_ANSWER_ITEM SET RESPOSTA_DATA = NULL WHERE ID = @ID ");
                            parametros.Add("ID", item[0]);
                        }

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }
                }

                return new PostApiRetorno { Ok = true };
            }
            catch (Exception e)
            {
                return new PostApiRetorno { Ok = false, Mensagem = e.Message };
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Quizapi/SalvaEscolhaMultiplaChecklist")]
        public async Task<PostApiRetorno> SalvaEscolhaMultiplaChecklist(SalvarQuizRequest salvar)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    if (salvar != null && salvar.Multi != null)
                    {
                        foreach (var item in salvar.Multi)
                        {
                            var paramDelete = new Dictionary<string, object>();
                            var sqlDelete = new StringBuilder();

                            sqlDelete.AppendLine(" DELETE FROM TB_QUIZ_ANSWER_ITEM_MULTI WHERE ID_QUIZ_ANSWER_ITEM = @ID_QUIZ_ANSWER_ITEM ");
                            paramDelete.Add("ID_QUIZ_ANSWER_ITEM", item.IdItem);

                            await CommandHelper.ExecuteNonQuery(com, sqlDelete, paramDelete, session.Connection, null);


                            if (item.Respostas != null)
                            {
                                foreach (var resposta in item.Respostas)
                                {
                                    var paramResposta = new Dictionary<string, object>();

                                    var sqlInsert = new StringBuilder();

                                    sqlInsert.AppendLine("INSERT INTO TB_QUIZ_ANSWER_ITEM_MULTI (ID_QUIZ_ANSWER_ITEM, ID_QUESTION_ANSWER, RESPOSTA, RESPOSTA_FOTO)");
                                    sqlInsert.AppendLine("VALUES(@ID_QUIZ_ANSWER_ITEM, @ID_QUESTION_ANSWER, @RESPOSTA, NULL)");

                                    paramResposta.Add("ID_QUIZ_ANSWER_ITEM", item.IdItem);
                                    paramResposta.Add("ID_QUESTION_ANSWER", resposta[0]);
                                    paramResposta.Add("RESPOSTA", resposta[1]);

                                    await CommandHelper.ExecuteNonQuery(com, sqlInsert, paramResposta, session.Connection, null);
                                }
                            }
                        }
                    }
                }

                return new PostApiRetorno { Ok = true };
            }
            catch (Exception e)
            {
                return new PostApiRetorno { Ok = false, Mensagem = e.Message };
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Quizapi/SalvaEscolhaChecklist")]
        public async Task<PostApiRetorno> SalvaEscolhaChecklist(SalvarQuiz salvar)
        {
            if (salvar == null)
            {
                return new PostApiRetorno { Ok = true };
            }

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql;

                    foreach (var item in salvar.Respostas)
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        sql = new StringBuilder();

                        parametros.Add("ID", item[0]);
                        parametros.Add("RESPOSTA", item[1]);

                        sql.AppendLine(" UPDATE TB_QUIZ_ANSWER_ITEM SET RESPOSTA = @RESPOSTA WHERE ID = @ID ");

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }
                }
                return new PostApiRetorno { Ok = true };
            }
            catch (Exception e)
            {
                return new PostApiRetorno { Ok = false, Mensagem = e.Message };
            }
        }

        [HttpGet()]
        [Route("api/Quizapi/GetList")]
        public async Task<GetApiRetorno<List<QuizAnswer>>> GetQuizAnswer(String CodCt, String Projeto, String Num_Obra, String Tarefa, Int32 numDoc)
        {
            GetApiRetorno<List<QuizAnswer>> retorno = new GetApiRetorno<List<QuizAnswer>>();
            retorno.Objeto = new List<QuizAnswer>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        if (String.IsNullOrEmpty(Tarefa))
                        {
                            sql.AppendLine("SELECT A.ID IDA,B.ID, B.ID_QUIZ, B.TITULO, B.PROTOCOLO, B.USUARIO_RESPOSTA, B.DIA_INICIO,B.QUIZ_INICIO_ATENDIMENTO,");
                            sql.AppendLine("       B.HORA_INICIO, A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC,C.TITULO AS TITULO_QUIZ ");
                            sql.AppendLine("FROM TB_QUIZ_ANSWER_DOCPROD A");
                            sql.AppendLine("INNER JOIN TB_QUIZ_ANSWER_CAPA B");
                            sql.AppendLine("ON(A.ID_ANSWER_CAPA = B.ID)");
                            sql.AppendLine("INNER JOIN TB_QUIZ C");
                            sql.AppendLine("ON(B.ID_QUIZ = C.ID)");
                            sql.AppendLine("WHERE A.CODCT = @PCODCT AND A.PROJETO = @PPROJETO AND A.NUM_OBRA = @PNUM_OBRA AND");
                            sql.AppendLine("      A.NUM_DOC = @PNUM_DOC");

                            parametros.Add("@PCODCT", CodCt);
                            parametros.Add("@PPROJETO", Projeto);
                            parametros.Add("@PNUM_OBRA", Num_Obra);
                            parametros.Add("@PNUM_DOC", numDoc);
                        }
                        else
                        {
                            sql.AppendLine("SELECT A.ID IDA ,B.ID, B.ID_QUIZ, B.TITULO, B.PROTOCOLO, B.USUARIO_RESPOSTA, B.DIA_INICIO,B.QUIZ_INICIO_ATENDIMENTO,");
                            sql.AppendLine("       B.HORA_INICIO, A.CODCT, A.PROJETO, A.NUM_OBRA, A.TAREFA,C.TITULO AS TITULO_QUIZ");
                            sql.AppendLine("FROM TB_QUIZ_ANSWER_TAREFA A");
                            sql.AppendLine("INNER JOIN TB_QUIZ_ANSWER_CAPA B");
                            sql.AppendLine("ON(A.ID_ANSWER_CAPA = B.ID)");
                            sql.AppendLine("INNER JOIN TB_QUIZ C");
                            sql.AppendLine("ON(B.ID_QUIZ = C.ID)");
                            sql.AppendLine("WHERE A.CODCT = @PCODCT AND A.PROJETO = @PPROJETO AND A.NUM_OBRA = @PNUM_OBRA AND");
                            sql.AppendLine("COALESCE(A.TAREFA,'') = @PTAREFA");


                            parametros.Add("@PCODCT", CodCt);
                            parametros.Add("@PPROJETO", Projeto);
                            parametros.Add("@PNUM_OBRA", Num_Obra);
                            parametros.Add("@PTAREFA", Tarefa);

                        }


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                QuizAnswer quiz = new QuizAnswer();

                                quiz.idA = Convert.ToInt64(reader["IDA"]);
                                quiz.id = Convert.ToInt64(reader["ID"]);
                                quiz.idQuiz = Convert.ToInt64(reader["ID_QUIZ"]);
                                quiz.titulo = Convert.ToString(reader["TITULO"]);
                                quiz.tituloQuiz = Convert.ToString(reader["TITULO_QUIZ"]);
                                quiz.protocolo = Convert.ToString(reader["PROTOCOLO"]);
                                quiz.usuario = Convert.ToString(reader["USUARIO_RESPOSTA"]);
                                quiz.diaInicio = Convert.ToDateTime(reader["DIA_INICIO"]);
                                quiz.horaInicio = reader["HORA_INICIO"] == null ? (DateTime?)null : DateTime.Parse(reader["HORA_INICIO"].ToString());
                                quiz.codct = Convert.ToString(reader["CODCT"]);
                                quiz.projeto = Convert.ToString(reader["PROJETO"]);
                                quiz.num_obra = Convert.ToString(reader["NUM_OBRA"]);

                                if (String.IsNullOrEmpty(Tarefa))
                                    quiz.numDoc = Convert.ToInt32(Convert.ToString(reader["NUM_DOC"]));
                                else
                                    quiz.tarefa = Convert.ToString(reader["TAREFA"]);

                                quiz.respostas = await GetQuizAnswerItem(session, sessionFoto, Convert.ToInt64(reader["ID"]));
                                quiz.materiais = await GetQuizAnswerMaterials(session, Convert.ToInt64(reader["ID"]));
                                quiz.producoes = await GetQuizAnswerProducoes(session, Convert.ToInt64(reader["ID"]));

                                if (Convert.ToString(reader["QUIZ_INICIO_ATENDIMENTO"]) == "S")
                                {
                                    quiz.respostas.Add(new QuizAnswerItemViewModel()
                                    {
                                        id = 0,
                                        idQuestion = 0,
                                        idQuestionAnswer = 0,
                                        pergunta = "ASSINATURA",
                                        tipo = EnumQuizTipo.Foto,
                                        ordem = quiz.respostas.Max(p => p.ordem) + 1,
                                        resposta = null,
                                        multi = await GetAssinatura(session, CodCt, Projeto, Num_Obra, Tarefa, numDoc),
                                        isRespondido = true,
                                        obrigatoria = "S",
                                        liberaGaleria = true,
                                        opcoesResposta = JsonService.DeserializeObject<List<QuizQuestionAnswerViewModel>>("[]")
                                    });
                                }

                                retorno.Objeto.Add(quiz);
                            }
                        }
                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpGet()]
        [Route("api/Quizapi/GetQuizAnswerVenda")]
        public async Task<GetApiRetorno<List<QuizAnswer>>> GetQuizAnswerVenda(long Venda, long? Troca = null)
        {
            var retorno = new GetApiRetorno<List<QuizAnswer>>
            {
                Objeto = new List<QuizAnswer>()
            };

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionFotoSql = HibernateUtil.GetSessionFoto())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    IEtapaVendaRepository repoEtapa = new EtapaVendaRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                    var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapa, repoConfig);

                    IEnumerable<EtapaVendaViewModel> etapasVenda =
                                                     Troca.HasValue
                                                        ? etapaVendaBusiness.GetDadosEtapasTrocaTitularidade(Troca.Value)
                                                       : etapaVendaBusiness.GetDadosEtapasVenda(Venda);

                    var etapasDict = etapasVenda?
                                    .GroupBy(x => x.Etapa).ToDictionary(g => g.Key, g => g.FirstOrDefault())
                                     ?? new Dictionary<int, EtapaVendaViewModel>();

                    //if (Troca != null)
                    //    etapasVenda = etapaVendaBusiness.GetDadosEtapasTrocaTitularidade(Troca.Value);
                    //else
                    //    etapasVenda = etapaVendaBusiness.GetDadosEtapasVenda(Venda);

                    //Select
                    using (var com = sessionT5Sql.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID IDA,B.ID, B.ID_QUIZ, B.TITULO, B.PROTOCOLO, B.USUARIO_RESPOSTA, B.DIA_INICIO,B.QUIZ_INICIO_ATENDIMENTO,");
                        sql.AppendLine("B.HORA_INICIO, C.TITULO AS TITULO_QUIZ ");
                        sql.AppendLine("FROM TB_QUIZ_ANSWER_VENDA A");
                        sql.AppendLine("INNER JOIN TB_QUIZ_ANSWER_CAPA B");
                        sql.AppendLine("ON(A.ID_ANSWER_CAPA = B.ID)");
                        sql.AppendLine("INNER JOIN TB_QUIZ C");
                        sql.AppendLine("ON(B.ID_QUIZ = C.ID)");

                        if (Troca != null)
                            sql.AppendLine("WHERE A.ID_TROCA_TITULAR = @ID");
                        else
                            sql.AppendLine("WHERE A.ID_VENDA = @ID");


                        parametros.Add("@ID", Troca ?? Venda);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5Sql.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                var idCapa = Convert.ToInt64(reader["ID"]);

                                var quiz = new QuizAnswer
                                {
                                    idVenda = Venda,
                                    idA = Convert.ToInt64(reader["IDA"]),
                                    id = idCapa,
                                    idQuiz = Convert.ToInt64(reader["ID_QUIZ"]),
                                    titulo = Convert.ToString(reader["TITULO"]),
                                    tituloQuiz = Convert.ToString(reader["TITULO_QUIZ"]),
                                    protocolo = Convert.ToString(reader["PROTOCOLO"]),
                                    usuario = Convert.ToString(reader["USUARIO_RESPOSTA"]),
                                    diaInicio = reader["DIA_INICIO"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["DIA_INICIO"])
                                        : (DateTime?)null,
                                    horaInicio = reader["HORA_INICIO"] != DBNull.Value
                                        ? DateTime.Parse(reader["HORA_INICIO"].ToString())
                                        : (DateTime?)null
                                };

                                quiz.respostas = await GetQuizAnswerItem(sessionT5Sql, sessionFotoSql, Convert.ToInt64(reader["ID"]));
                                quiz.materiais = await GetQuizAnswerMaterials(sessionT5Sql, Convert.ToInt64(reader["ID"]));
                                quiz.producoes = await GetQuizAnswerProducoes(sessionT5Sql, Convert.ToInt64(reader["ID"]));

                                //QuizAnswer quiz = new QuizAnswer();

                                //quiz.idVenda = Venda;
                                //quiz.idA = Convert.ToInt64(reader["IDA"]);
                                //quiz.id = Convert.ToInt64(reader["ID"]);
                                //quiz.idQuiz = Convert.ToInt64(reader["ID_QUIZ"]);
                                //quiz.titulo = Convert.ToString(reader["TITULO"]);
                                //quiz.tituloQuiz = Convert.ToString(reader["TITULO_QUIZ"]);
                                //quiz.protocolo = Convert.ToString(reader["PROTOCOLO"]);
                                //quiz.usuario = Convert.ToString(reader["USUARIO_RESPOSTA"]);
                                //quiz.diaInicio = Convert.ToDateTime(reader["DIA_INICIO"]);
                                //quiz.horaInicio = reader["HORA_INICIO"] == null ? (DateTime?)null : DateTime.Parse(reader["HORA_INICIO"].ToString());



                                if (Convert.ToString(reader["QUIZ_INICIO_ATENDIMENTO"]) == "S")
                                {

                                    long proximaOrdem = (quiz.respostas != null && quiz.respostas.Any()) ? quiz.respostas.Max(p => p.ordem) + 1 : 1;

                                    quiz.respostas ??= new List<QuizAnswerItemViewModel>();

                                    quiz.respostas.Add(new QuizAnswerItemViewModel()
                                    {
                                        id = 0,
                                        idQuestion = 0,
                                        idQuestionAnswer = 0,
                                        pergunta = "ASSINATURA",
                                        tipo = EnumQuizTipo.Foto,
                                        ordem = quiz.respostas.Max(p => p.ordem) + 1,
                                        resposta = null,
                                        //multi = await GetAssinatura(session, CodCt, Projeto, Num_Obra, Tarefa, numDoc),
                                        isRespondido = true,
                                        obrigatoria = "S",
                                        liberaGaleria = true,
                                        opcoesResposta = JsonService.DeserializeObject<List<QuizQuestionAnswerViewModel>>("[]")
                                    });
                                }

                                // Verificando as questões vinculadas a restrição
                                if (quiz.respostas != null && quiz.respostas.Count > 0)
                                {
                                    var restricoesParaRemover = ObterRestricoesParaRemover(etapasDict);

                                    if (restricoesParaRemover.Count > 0)
                                    {
                                        quiz.respostas = quiz.respostas
                                            .Where(r => !r.restricao.HasValue || !restricoesParaRemover.Contains(r.restricao.Value))
                                            .ToList();
                                    }
                                }

                                retorno.Objeto.Add(quiz);
                            }
                        }
                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        private static HashSet<ConsultasRestricaoVenda> ObterRestricoesParaRemover(
    Dictionary<int, EtapaVendaViewModel> etapasDict)
        {
            var restricoes = new HashSet<ConsultasRestricaoVenda>();

            if (EtapaPendenteOuAprovada(etapasDict, (int)EnumEtapaVenda.ConsultaViabilidade))
                restricoes.Add(ConsultasRestricaoVenda.ConsultaViabilidade);

            if (EtapaPendenteOuAprovada(etapasDict, (int)EnumEtapaVenda.ConsultaPendenciaFinanceira))
                restricoes.Add(ConsultasRestricaoVenda.ConsultaPendenciaFinanceira);

            if (EtapaPendenteOuAprovada(etapasDict, (int)EnumEtapaVenda.ConsultaPendenciaEndereco))
                restricoes.Add(ConsultasRestricaoVenda.ConsultaPendenciaEndereco);

            if (EtapaPendenteOuAprovada(etapasDict, (int)EnumEtapaVenda.ConsultaCredito))
                restricoes.Add(ConsultasRestricaoVenda.ConsultaCredito);

            return restricoes;
        }

        private static bool EtapaPendenteOuAprovada(
            Dictionary<int, EtapaVendaViewModel> etapasDict,
            int etapa)
        {
            if (!etapasDict.TryGetValue(etapa, out var etapaVenda) || etapaVenda == null)
                return true;

            return etapaVenda.Status == (int)StatusEtapaVenda.Pendente
                || etapaVenda.Status == (int)StatusEtapaVenda.Aprovado;
        }

        public async Task<List<QuizAnswerItemMultiViewModel>> GetAssinatura(ISession session, String CodCt, String Projeto, String Num_Obra, String Tarefa, Int32 numDoc = 0)
        {
            String Equipe = null;
            List<QuizAnswerItemMultiViewModel> retorno = new List<QuizAnswerItemMultiViewModel>();
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, Object> parametros = new Dictionary<string, Object>();

                sql.AppendLine("SELECT A.EQUIPE  ");
                sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");
                sql.AppendLine("WHERE A.CODCT = @PCODCT AND A.PROJETO = @PPROJETO AND A.NUM_OBRA = @PNUM_OBRA AND");
                sql.AppendLine("      A.NUM_DOC = @PNUM_DOC");

                parametros.Add("@PCODCT", CodCt);
                parametros.Add("@PPROJETO", Projeto);
                parametros.Add("@PNUM_OBRA", Num_Obra);
                parametros.Add("@PNUM_DOC", numDoc);
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            Equipe = Convert.ToString(reader["EQUIPE"]);
                        }
                    }
                }



                String _urlAplicacao = CONSTANTES.CaminhoProjetoWebApi ?? throw new Exception("CaminhoProjetoWebApi não definido no web.config");
                String _urlFoto = null;

                retorno.Add(new QuizAnswerItemMultiViewModel
                {
                    id = 0,
                    idQuestionAnswerItem = 0,
                    idQuestionAnswer = 0,
                    idBDFoto = 0,
                    resposta = Equipe,
                    urlfoto = _urlFoto = String.Format(@"{0}/Fotos/AssinaturaEquipe/{1}.png", _urlAplicacao, Equipe)
                });
            }

            return retorno;

        }

        public async Task<List<QuizAnswerItemViewModel>> GetQuizAnswerItem(ISession session, ISession sessionFoto, Int64? IdAnswer, Int64? IdItem = null)
        {
            List<QuizAnswerItemViewModel> retorno = new List<QuizAnswerItemViewModel>();

            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.ID_QUESTION, A.ID_QUESTION_ANSWER, A.PERGUNTA, COALESCE(A.TIPO,0) AS TIPO, A.ORDEM,A.RESPOSTA_CODBAR,");
                    sql.AppendLine("       A.RESPOSTA, A.RESPOSTA_TEXTO, A.RESPOSTA_VALOR, A.RESPOSTA_DATA, A.RESPOSTA_FOTO, ");
                    sql.AppendLine("       COALESCE(A.RESPONDIDO, 'N') AS RESPONDIDO, COALESCE(A.OPCOES_RESPOSTA, '[]') AS OPCAORESPOSTA, ");
                    sql.AppendLine("       COALESCE(A.OBRIGATORIA, 'N') AS OBRIGATORIA, ");
                    sql.AppendLine("       COALESCE(A.LIBERA_GALERIA, 'N') AS LIBERA_GALERIA, A.VINCULA_RESTRICAO, A.RESTRICAO");
                    sql.AppendLine("FROM TB_QUIZ_ANSWER_ITEM A");

                    if (IdItem.HasValue)
                    {
                        sql.AppendLine("WHERE A.ID = @PID AND A.TIPO <> 6");
                        parametros.Add("@PID", IdItem);
                    }
                    else if (IdAnswer.HasValue)
                    {
                        sql.AppendLine("WHERE A.ID_ANSWER_CAPA = @PID_ANSWER_CAPA AND A.TIPO <> 6");
                        parametros.Add("@PID_ANSWER_CAPA", IdAnswer);
                    }
                    else
                    {
                        throw new Exception("informa ids para busca");
                    }


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                EnumQuizTipo _tipo = (EnumQuizTipo)Enum.Parse(typeof(EnumQuizTipo), Convert.ToString(reader["TIPO"]));
                                Object _resposta = null;


                                List<QuizAnswerItemMultiViewModel> _multi = null;
                                string idAssinatura = null;
                                switch (_tipo)
                                {
                                    case EnumQuizTipo.Data:
                                        _resposta = reader["RESPOSTA_DATA"] == DBNull.Value ? null : Convert.ToDateTime(reader["RESPOSTA_DATA"]).ToShortDateString();
                                        break;
                                    case EnumQuizTipo.Foto:
                                        _multi = reader["ID"] == DBNull.Value ? null : await GetQuizAnswerItemMulti(session, sessionFoto, Convert.ToInt64(reader["ID"]));
                                        break;
                                    case EnumQuizTipo.UnicaEscolha:
                                        _resposta = reader["RESPOSTA"] == DBNull.Value ? null : Convert.ToString(reader["RESPOSTA"]);
                                        break;
                                    case EnumQuizTipo.MultiplaEscolha:
                                        _multi = await GetQuizAnswerItemMulti(session, sessionFoto, Convert.ToInt64(reader["ID"]));
                                        break;
                                    case EnumQuizTipo.Texto:
                                        _resposta = reader["RESPOSTA_TEXTO"] == DBNull.Value ? null : Convert.ToString(reader["RESPOSTA_TEXTO"]);
                                        break;
                                    case EnumQuizTipo.Valor:
                                        _resposta = reader["RESPOSTA_VALOR"] == DBNull.Value ? null : Convert.ToString(reader["RESPOSTA_VALOR"]);
                                        break;
                                    case EnumQuizTipo.CodigoBarra:
                                        _resposta = reader["RESPOSTA_CODBAR"] == DBNull.Value ? null : Convert.ToString(reader["RESPOSTA_CODBAR"]);
                                        break;
                                    case EnumQuizTipo.AssinaturaCliente:
                                        var assinaturaResult = GetQuizAnswerAssinatura(session, sessionFoto, Convert.ToInt64(reader["ID"]));
                                        idAssinatura = assinaturaResult.idAssinatura;
                                        _resposta = reader["ID"] == DBNull.Value ? null : assinaturaResult.url;
                                        break;
                                    default:
                                        _resposta = reader["RESPOSTA_TEXTO"] == DBNull.Value ? null : Convert.ToString(reader["RESPOSTA_TEXTO"]);
                                        break;
                                }

                                retorno.Add(
                                    new QuizAnswerItemViewModel()
                                    {
                                        id = Convert.ToInt64(reader["ID"]),
                                        idQuestion = reader["ID_QUESTION"] != DBNull.Value ? Convert.ToInt64(reader["ID_QUESTION"]) : (Int64?)null,
                                        idQuestionAnswer = (reader["ID_QUESTION_ANSWER"].ToString() != String.Empty) ?
                                                            Convert.ToInt64(reader["ID_QUESTION_ANSWER"]) : (Int64?)null,
                                        pergunta = Convert.ToString(reader["PERGUNTA"]),
                                        tipo = _tipo,
                                        ordem = Convert.ToInt32(reader["ORDEM"]),
                                        resposta = _resposta,
                                        multi = _multi,
                                        isRespondido = Convert.ToString(reader["RESPONDIDO"]) == "S",
                                        obrigatoria = Convert.ToString(reader["OBRIGATORIA"]),
                                        liberaGaleria = Convert.ToString(reader["LIBERA_GALERIA"]).ToUpper() == "S",
                                        opcoesResposta = JsonService.DeserializeJasonMapper<List<QuizQuestionAnswerViewModel>>(reader["OPCAORESPOSTA"].ToString()),
                                        idAssinatura = idAssinatura,
                                        vinculaRestricao = reader["VINCULA_RESTRICAO"] != DBNull.Value ? Convert.ToString(reader["VINCULA_RESTRICAO"]) == "S" : false,
                                        restricao =
                                            reader["RESTRICAO"] != DBNull.Value ?
                                                (ConsultasRestricaoVenda)Convert.ToInt32(reader["RESTRICAO"]) :
                                                (ConsultasRestricaoVenda?)null
                                    }
                                );
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        private async Task<List<QuizAnswerItemMultiViewModel>> GetQuizAnswerItemMulti(ISession sessionT5, ISession sessionFoto, Int64? IdQuestionItem)
        {
            if (!IdQuestionItem.HasValue) return null;

            List<QuizAnswerItemMultiViewModel> retorno = new List<QuizAnswerItemMultiViewModel>();


            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.ID_QUIZ_ANSWER_ITEM, A.ID_QUESTION_ANSWER, A.RESPOSTA, A.RESPOSTA_FOTO");
                    sql.AppendLine("FROM TB_QUIZ_ANSWER_ITEM_MULTI A");
                    sql.AppendLine("WHERE A.ID_QUIZ_ANSWER_ITEM = @ID_QUIZ_ANSWER_ITEM");

                    parametros.Add("@ID_QUIZ_ANSWER_ITEM", IdQuestionItem.Value);

                    String _urlAplicacao = CONSTANTES.CaminhoProjetoWebApi ?? throw new Exception("CaminhoProjetoWebApi não definido no web.config");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            String _urlFoto = null;

                            String caminho = $@"{CONSTANTES.CaminhoProjetoTouch}\Fotos\Quiz\{reader["RESPOSTA_FOTO"]}.png";

                            QuizAnswerItemMultiViewModel foto = new QuizAnswerItemMultiViewModel()
                            {
                                id = Convert.ToInt64(reader["ID"]),
                                idQuestionAnswerItem = Convert.ToInt64(reader["ID_QUIZ_ANSWER_ITEM"]),
                                idQuestionAnswer = reader["ID_QUESTION_ANSWER"].ToString() == String.Empty ? (Int64?)null : Convert.ToInt64(reader["ID_QUESTION_ANSWER"]),
                                idBDFoto = reader["RESPOSTA_FOTO"].ToString() == String.Empty ? (Int64?)null : Convert.ToInt64(reader["RESPOSTA_FOTO"]),
                                resposta = Convert.ToString(reader["RESPOSTA"]),
                                urlfoto = _urlFoto = String.Format(@"{0}/Fotos/Quiz/{1}.png", _urlAplicacao, reader["RESPOSTA_FOTO"])
                            };

                            if (reader["RESPOSTA_FOTO"] != DBNull.Value)
                            {
                                var arquivoByte = await quiz_getArquivoFoto(sessionFoto, Convert.ToInt64(reader["RESPOSTA_FOTO"]));

                                if (!File.Exists(caminho) && reader["RESPOSTA_FOTO"] != DBNull.Value)
                                {
                                    if (String.IsNullOrEmpty(CONSTANTES.CaminhoProjetoTouch))
                                    {
                                        throw new Exception("CaminhoProjetoTouch não definido em configurações");
                                    }

                                    if (arquivoByte != null)
                                        File.WriteAllBytes(caminho, arquivoByte);
                                }
                                if (arquivoByte != null)
                                    foto.foto64 = Convert.ToBase64String(arquivoByte);
                            }

                            retorno.Add(foto);
                        }

                        return retorno.Count() == 0 ? null : retorno;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/QuizApi/ObterFotoQuiz")]
        public async Task<IHttpActionResult> ObterFotoQuiz(long id)
        {
            using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
            {
                var arquivo = await quiz_getArquivoFoto(sessionFoto, id);

                if (arquivo == null || arquivo.Length == 0)
                    return NotFound();

                var response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new System.Net.Http.ByteArrayContent(arquivo)
                };

                response.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(30)
                };

                return ResponseMessage(response);
            }
        }
        private async Task<List<QuizAnswerMaterialViewModel>> GetQuizAnswerMaterials(ISession session, Int64 IdAnswer)
        {
            List<QuizAnswerMaterialViewModel> retorno = new List<QuizAnswerMaterialViewModel>();


            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.CODMAT, A.QUANTIDADE");
                    sql.AppendLine("FROM TB_QUIZ_ANSWER_MATERIAL A");
                    sql.AppendLine("WHERE A.ID_QUIZ_ANSWER = @PID_QUIZ_ANSWER");

                    parametros.Add("@PID_QUIZ_ANSWER", IdAnswer);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(
                                new QuizAnswerMaterialViewModel()
                                {
                                    id = Convert.ToInt64(reader["ID"]),
                                    codmat = Convert.ToString(reader["CODMAT"]),
                                    quantidade = reader["QUANTIDADE"] != DBNull.Value ? Convert.ToDouble(reader["QUANTIDADE"]) : 0
                                }
                            );
                        }

                        return retorno;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<byte[]> quiz_getArquivoFoto(ISession sessionFoto, long id)
        {
            using (var com = sessionFoto.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT A.ARQUIVO");
                sql.AppendLine("FROM TB_QUIZ_FOTO A");
                sql.AppendLine("WHERE A.ID = @ID");

                var parametros = new Dictionary<string, object>
        {
            { "@ID", id }
        };

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionFoto.Connection, null))
                {
                    if (!await reader.ReadAsync())
                        return null;

                    if (reader["ARQUIVO"] == DBNull.Value)
                        return null;

                    return (byte[])reader["ARQUIVO"];
                }
            }
        }

        public async Task<List<QuizAnswerProducaoViewModel>> GetQuizAnswerProducoes(ISession session, Int64 IdAnswer)
        {
            List<QuizAnswerProducaoViewModel> retorno = new List<QuizAnswerProducaoViewModel>();

            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.UNIDADE_CUSTO, A.CODIGO, A.QUANT_PROD");
                    sql.AppendLine("FROM TB_QUIZ_ANSWER_PRODUCAO A");
                    sql.AppendLine("WHERE A.ID_QUIZ_ANSWER = @PID_QUIZ_ANSWER");

                    parametros.Add("@PID_QUIZ_ANSWER", IdAnswer);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(
                                new QuizAnswerProducaoViewModel()
                                {
                                    id = Convert.ToInt64(reader["ID"]),
                                    unidadeCusto = Convert.ToString(reader["UNIDADE_CUSTO"]),
                                    codigo = Convert.ToString(reader["CODIGO"]),
                                    quant_prod = reader["QUANT_PROD"] != DBNull.Value ? Convert.ToDouble(reader["QUANT_PROD"]) : 0
                                }
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        public (string url, string idAssinatura) GetQuizAnswerAssinatura(ISession sessionT5, ISession sessionFoto, Int64? IdQuestionItem)
        {
            if (!IdQuestionItem.HasValue) return (null, null);

            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>
            {
                { "@ID", Convert.ToString(IdQuestionItem) },
            };

            string _retorno = null;
            using (var com = sessionT5.Connection.CreateCommand())
            {

                sql.AppendLine("SELECT A.ID_ASSINATURA");
                sql.AppendLine("FROM TB_QUIZ_ANSWER_ITEM A");
                sql.AppendLine("WHERE A.ID = @ID");



                using (var reader2 = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null).Result)
                {
                    while (reader2.Read())
                    {
                        _retorno = reader2["ID_ASSINATURA"] != DBNull.Value ? reader2["ID_ASSINATURA"].ToString() : null;
                    }
                }
            }

            sql.Clear();
            parametros.Clear();

            using (var com = sessionFoto.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT ARQUIVO FROM TB_QUIZ_FOTO");
                sql.AppendLine("WHERE ID = @pID");

                parametros.Add("@pID", _retorno);
                string urlfoto = null;
                String _urlAplicacao = CONSTANTES.CaminhoProjetoWebApi ?? throw new Exception("CaminhoProjetoWebApi não definido no web.config");
                using (var readerFoto = CommandHelper.ExecuteReader(com, sql, parametros, sessionFoto.Connection, null).Result)
                {
                    while (readerFoto.Read())
                    {
                        String caminho = $@"{CONSTANTES.CaminhoProjetoTouch}\Fotos\Quiz\{_retorno}.png";
                        var arquivoByte = quiz_getArquivoFoto(sessionFoto, Convert.ToInt64(_retorno)).Result;
                        if (arquivoByte != null)
                        {
                            urlfoto = Convert.ToBase64String(arquivoByte);
                        }

                    }
                }

                return (urlfoto, _retorno);
            }


        }

        [HttpPost()]
        [Route("api/Quizapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_QUIZ A ");

                    List<String> condicoes = new List<String>();

                    condicoes.Add($"A.ID IS NOT NULL");

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.TITULO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count > 0)
                    {
                        sqlFrom.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
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


    }

    public class SalvarQuiz
    {
        public string[][] Respostas { get; set; }
    }

    public class SalvarQuizRequest
    {
        public List<SalvarQuizMulti> Multi { get; set; }
    }

    public class SalvarQuizMulti
    {
        public string IdItem { get; set; }
        public string[][] Respostas { get; set; }
    }
}