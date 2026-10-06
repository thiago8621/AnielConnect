using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.T5.Checklist
{
    public class ChecklistRepository : IChecklistRepository
    {
        private readonly ISession _sessionT5Nh;
        private readonly ISession _sessionFoto;
        private readonly ISession _sessionT5Sql;

        public ChecklistRepository(ISession sessionT5Nh, ISession sessionT5Sql)
        {
            this._sessionT5Nh = sessionT5Nh;
            this._sessionT5Sql = sessionT5Sql;
        }
        public ChecklistRepository(ISession sessionT5Nh, ISession sessionT5Sql, ISession sessionFoto)
        {
            this._sessionT5Nh = sessionT5Nh;
            this._sessionT5Sql = sessionT5Sql;
            this._sessionFoto = sessionFoto;
        }


        public ChecklistListData GetListagemChecklist(ChecklistDataTableRequest request)
        {
            var retorno = new ChecklistListData
            {
                RecordsFiltered = 0,
                Data = new List<ChecklistListModel>()
            };

            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();
                var sqlCount = new StringBuilder();

                var sqlWhere = new StringBuilder();

                #region SQL Where

                if (request.search != null && !String.IsNullOrEmpty(request.search.value))
                {
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    condicoes.Add("UPPER(A.TITULO) LIKE @SEARCH");
                }

                if (request.Tipo.HasValue)
                {
                    parametros.Add("@TIPO", request.Tipo.Value);
                    condicoes.Add("A.TIPO = @TIPO");
                }

                if (!String.IsNullOrEmpty(request.Contrato))
                {
                    parametros.Add("@CODCT", StringService.Truncate(request.Contrato, 4));
                    condicoes.Add("A.CODCT = @CODCT");
                }

                if (request.Ativo.HasValue)
                {
                    parametros.Add("@ATIVO", request.Ativo.Value == Status_Default.Ativo ? "S" : "N");
                    condicoes.Add("A.ATIVO = @ATIVO");
                }

                if (request.CheckListporNatureza.HasValue)
                {
                    parametros.Add("@CH_LIST_NAT", request.CheckListporNatureza.Value == Sinapse.Model.enumerados.Padrao.Sim ? "S" : "N");
                    condicoes.Add("A.CHECKLIST_POR_NATUREZA = @CH_LIST_NAT");
                }

                if (request.ObrigarImprodutivo.HasValue)
                {
                    parametros.Add("@OBG_IMPROD", request.ObrigarImprodutivo.Value == Sinapse.Model.enumerados.Padrao.Sim ? "S" : "N");
                    condicoes.Add("A.OBRIGAR_IMPRODUTIVO  = @OBG_IMPROD");
                }

                if (request.PreencherInicioAtividade.HasValue)
                {
                    parametros.Add("@PR_INI_ATV", request.PreencherInicioAtividade.Value == Sinapse.Model.enumerados.Padrao.Sim ? "S" : "N");
                    condicoes.Add("A.PREENCHER_NO_INICIO_ATIVIDADE = @PR_INI_ATV");
                }

                if (condicoes.Count > 0)
                {
                    sqlWhere.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
                }

                #endregion

                sqlCount.AppendLine("SELECT COUNT(A.ID) AS CONTADOR FROM TB_QUIZ A");
                sqlCount.AppendLine(sqlWhere.ToString());

                using (var reader = CommandHelper.ExecuteReader(com, sqlCount, parametros, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                    {
                        retorno.RecordsFiltered = reader["CONTADOR"] != DBNull.Value ? Convert.ToInt32(reader["CONTADOR"]) : 0;
                    }
                }

                sql.AppendLine("SELECT A.ID, A.TITULO, A.OBRIGAR_IMPRODUTIVO, A.CODCT, A.IDNATUREZA, A.TIPO,");
                sql.AppendLine("       A.ATIVO, A.CHECKLIST_POR_NATUREZA, A.PREENCHER_NO_INICIO_ATIVIDADE");
                sql.AppendLine("FROM TB_QUIZ A");
                sql.AppendLine(sqlWhere.ToString());

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        TipoChecklist? tipo = null;

                        if (reader["TIPO"] != DBNull.Value)
                        {
                            tipo = (TipoChecklist)Convert.ToInt32(reader["TIPO"]);
                        }

                        string ativo = reader["ATIVO"] != DBNull.Value ? Convert.ToString(reader["ATIVO"]) : String.Empty;
                        string checklistporNatureza = reader["CHECKLIST_POR_NATUREZA"] != DBNull.Value ? Convert.ToString(reader["CHECKLIST_POR_NATUREZA"]) : String.Empty;
                        string obrigarImprodutivo = reader["OBRIGAR_IMPRODUTIVO"] != DBNull.Value ? Convert.ToString(reader["OBRIGAR_IMPRODUTIVO"]) : String.Empty;
                        string preencherInicioAtividade = reader["PREENCHER_NO_INICIO_ATIVIDADE"] != DBNull.Value ? Convert.ToString(reader["PREENCHER_NO_INICIO_ATIVIDADE"]) : String.Empty;

                        retorno.Data.Add(new ChecklistListModel
                        {
                            Codigo = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : String.Empty,
                            Descricao = reader["TITULO"] != DBNull.Value ? Convert.ToString(reader["TITULO"]) : String.Empty,
                            Tipo = HtmlEnumExtensions.GetEnumDescription(tipo),
                            
                            Contrato = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : String.Empty,

                            ObrigarImprodutivo = obrigarImprodutivo == "S" ? RsCadastros.SIM : RsCadastros.NAO,
                            PreencherInicioAtividade = preencherInicioAtividade == "S" ? RsCadastros.SIM : RsCadastros.NAO,
                            ChecklistNatureza = checklistporNatureza == "S" ? RsCadastros.SIM : RsCadastros.NAO,
                            
                            Status = ativo == "S" ? RsCadastros.ATIVO :  RsCadastros.INATIVO,
                        });
                    }
                }
            }

            return retorno;
        }

        public IQueryable<Quiz> GetAll()
        {
            using (var repo = new Conexao<Quiz>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }

        public Quiz GetChecklistById(long id)
        {
            var originalQuiz = this.GetAll()
           .Where(q => q.Id == id)
           .FirstOrDefault();

            if (originalQuiz != null)
            {
                originalQuiz.Questoes = originalQuiz.Questoes.Where(p => p.DiaExclusao == null).ToList();
            }

            return originalQuiz;
        }

        public void Save(Quiz quiz)
        {
            using (var repo = new Conexao<Quiz>(this._sessionT5Nh))
            {
                repo.Save(quiz).Wait();
            }
        }

        public void Update(Quiz quiz)
        {
            using (var repo = new Conexao<Quiz>(this._sessionT5Nh))
            {
                repo.Update(quiz).Wait();
            }
        }

        public Quiz GetQuizVenda(long idVenda)
        {
            using (var repo = new Conexao<QuizAnswerVenda>(this._sessionT5Nh))
            {
                return repo.QueryableSelect()
                           .Where(a => a.IdVenda == idVenda)
                           .FirstOrDefault()?.Capa?.Quiz;
            }
        }
        public Quiz GetQuizOS(string codTipoServico, string codSubTipoServico) 
        {
            Quiz checklist = null;
            long Id = 0;
            using (var com = this._sessionT5Sql.Connection.CreateCommand()) 
            {                    
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT DISTINCT B.ID");
                sql.AppendLine("FROM TB_QUIZ_SUBTIPO_SERV A");
                sql.AppendLine("INNER JOIN TB_QUIZ B");
                sql.AppendLine("ON(A.ID_QUIZ = B.ID)");
                sql.AppendLine("INNER JOIN TB_QUIZ_QUESTION C");
                sql.AppendLine("ON(C.ID_QUIZ = B.ID)");
                sql.AppendLine("WHERE A.COD_TIPO_SERV = @PCOD_TIPO_SERV AND A.COD_SUBTIPO_SERV = @PCOD_SUBTIPO_SERV");

                Dictionary<string, Object> parametros = new Dictionary<string, Object>
                {
                    { "@PCOD_TIPO_SERV", codTipoServico },
                    { "@PCOD_SUBTIPO_SERV", codSubTipoServico }
                };

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                    {
                        Id = Convert.ToInt64(reader["ID"]);
                    }
                }
            }

            if (Id != 0) 
            {
                using (var repo = new Conexao<Quiz>(this._sessionT5Nh))                
                {                
                    checklist = repo.QueryableSelect().Where(p => p.Id == Id).FirstOrDefault();
                }
            }
            return checklist;
        }

        public IEnumerable<Quiz> GetQuizzes(string codTipoServico, string codSubTipoServico)
        {
            List<Quiz> checklist = new List<Quiz>();
            
            List<long> Ids = new List<long>();
            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT DISTINCT B.ID");
                sql.AppendLine("FROM TB_QUIZ_SUBTIPO_SERV A");
                sql.AppendLine("INNER JOIN TB_QUIZ B");
                sql.AppendLine("ON(A.ID_QUIZ = B.ID)");
                sql.AppendLine("INNER JOIN TB_QUIZ_QUESTION C");
                sql.AppendLine("ON(C.ID_QUIZ = B.ID)");
                sql.AppendLine("WHERE A.COD_TIPO_SERV = @PCOD_TIPO_SERV AND A.COD_SUBTIPO_SERV = @PCOD_SUBTIPO_SERV");

                Dictionary<string, Object> parametros = new Dictionary<string, Object>
                {
                    { "@PCOD_TIPO_SERV", codTipoServico },
                    { "@PCOD_SUBTIPO_SERV", codSubTipoServico }
                };

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        Ids.Add(Convert.ToInt64(reader["ID"]));
                    }
                }
            }
            if (Ids.Count > 0)
            {
                using (var repo = new Conexao<Quiz>(this._sessionT5Nh))
                {
                    foreach (var Id in Ids) 
                    {
                        checklist.AddRange(repo.QueryableSelect().Where(p => p.Id == Id).ToList());
                    }                    
                }
            }
            return checklist;
        }

        public IEnumerable<QuizAnswerItem> GetQuizItens(long Id) 
        {
            IEnumerable<QuizAnswerItem> retorno;
            using (var repository = new Conexao<QuizAnswerItem>(this._sessionT5Nh))            
            {
                retorno = repository.QueryableSelect().Where(p => p.QuizQuestion.Quiz.Id == Id).ToList();
            }                
            return retorno;
        }

        public byte[] GetAssinatura(long Id) 
        {
            byte[] retorno;
            using (var repository = new Conexao<Quiz_Foto>(this._sessionFoto))
            {
                retorno = repository.QueryableSelect().Where(p => p.Id_Quiz_Answer_Item == Id).FirstOrDefault().Arquivo;
            }
            return retorno;
        }

        public IEnumerable<QuizAnswerItemMulti> GetQuizItensMultiplo(long Id)
        {
            IEnumerable<QuizAnswerItemMulti> retorno;
            using (var repository = new Conexao<QuizAnswerItemMulti>(this._sessionT5Nh))
            {
                retorno = repository.QueryableSelect().Where(p => p.QuizAnswerItem.Id == Id).ToList();
            }
            return retorno;
        }

        public IEnumerable<QuizAnswerItem> GetAnswersByVenda(long idVenda)
        {
            IEnumerable<QuizAnswerItem> respostas = new List<QuizAnswerItem>();

            using (var repoAnswerVenda = new Conexao<QuizAnswerVenda>(this._sessionT5Nh))
            using (var repoAnswerItem = new Conexao<QuizAnswerItem>(this._sessionT5Nh))
            {
                QuizAnswerCapa capa = 
                    repoAnswerVenda.QueryableSelect()
                                   .Where(c => c.IdVenda == idVenda)
                                   .Select(c => c.Capa)
                                   .FirstOrDefault();

                if (capa != null)
                {
                    respostas = repoAnswerItem.QueryableSelect()
                                              .Where(a => a.AnswerCapa.Id == capa.Id)
                                              .ToList();
                }
            }
            
            return respostas;
        }

        public long? GetIdQuizParaVenda(
            Tipo_Pessoa tipoPessoa,
            long? idCategoriaCliente,
            long? idMeioCaptacao,
            long? idUf,
            long? idCidade
        )
        {
            long? idQuiz = null;

            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT ID,");
                sql.AppendLine("       (PONTOS_TIPO_PESSOA + PONTOS_CATEGORIA_CLIENTE + PONTOS_MEIO_CAPTACAO + PONTOS_UF + PONTOS_CIDADE) AS TOTAL_PONTOS_QUIZ");
                sql.AppendLine("FROM (");
                sql.AppendLine("    SELECT Q.ID,");
                sql.AppendLine("    	(CASE");
                sql.AppendLine("    		WHEN Q.TIPO_PESSOA IS NULL THEN 0");
                sql.AppendLine("    		WHEN Q.TIPO_PESSOA = @TIPO_PESSOA THEN 1");
                sql.AppendLine("    		ELSE 0");
                sql.AppendLine("    	END) AS PONTOS_TIPO_PESSOA,");

                sql.AppendLine("    	(CASE");
                sql.AppendLine("    		WHEN Q.ID_CATEGORIA_CLIENTE IS NULL THEN 0");
                sql.AppendLine("    		WHEN Q.ID_CATEGORIA_CLIENTE = @ID_CATEGORIA_CLIENTE THEN 1");
                sql.AppendLine("    		ELSE 0");
                sql.AppendLine("    	END) AS PONTOS_CATEGORIA_CLIENTE,");

                sql.AppendLine("    	(CASE");
                sql.AppendLine("    		WHEN Q.ID_MEIO_CAPTACAO IS NULL THEN 0");
                sql.AppendLine("    		WHEN Q.ID_MEIO_CAPTACAO = @ID_MEIO_CAPTACAO THEN 1");
                sql.AppendLine("    		ELSE 0");
                sql.AppendLine("    	END) AS PONTOS_MEIO_CAPTACAO,");

                sql.AppendLine("    	(CASE");
                sql.AppendLine("    		WHEN Q.ID_UF IS NULL THEN 0");
                sql.AppendLine("    		WHEN Q.ID_UF = @ID_UF THEN 1");
                sql.AppendLine("    		ELSE 0");
                sql.AppendLine("    	END) AS PONTOS_UF,");

                sql.AppendLine("    	(CASE");
                sql.AppendLine("    		WHEN Q.ID_CIDADE IS NULL THEN 0");
                sql.AppendLine("    		WHEN Q.ID_CIDADE = @ID_CIDADE THEN 1");
                sql.AppendLine("    		ELSE 0");
                sql.AppendLine("    	END) AS PONTOS_CIDADE");
                sql.AppendLine("    FROM TB_QUIZ Q");
                sql.AppendLine("    WHERE Q.TIPO = 1");
                sql.AppendLine("      AND Q.ATIVO = 'S'");
                sql.AppendLine("    ORDER BY ID");
                sql.AppendLine(")");

                parametros.Add("@TIPO_PESSOA", tipoPessoa);
                parametros.Add("@ID_CATEGORIA_CLIENTE", idCategoriaCliente);
                parametros.Add("@ID_MEIO_CAPTACAO", idMeioCaptacao);
                parametros.Add("@ID_UF", idUf);
                parametros.Add("@ID_CIDADE", idCidade);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null).Result)
                {
                    // Quantidade de pontos do checklist que mais obteve pontos até o momento
                    int qtdPontosQuizSelecionado = 0;

                    while (reader.Read())
                    {
                        long idQuizAtual = Convert.ToInt64(reader["ID"]);
                        int qtdPontosQuiz = reader["TOTAL_PONTOS_QUIZ"] != DBNull.Value ? Convert.ToInt32(reader["TOTAL_PONTOS_QUIZ"]) : 0;

                        if (!idQuiz.HasValue || qtdPontosQuiz > qtdPontosQuizSelecionado)
                        {
                            qtdPontosQuizSelecionado = qtdPontosQuiz;
                            idQuiz = idQuizAtual;
                        }
                    }
                }
            }

            return idQuiz;
        }

        public void SaveCapa(QuizAnswerCapa capa)
        {
            using (var repo = new Conexao<QuizAnswerCapa>(this._sessionT5Nh))
            {
                repo.Save(capa).Wait();
            }
        }

        public void SaveQuizVenda(QuizAnswerVenda quizVenda)
        {
            using (var repo = new Conexao<QuizAnswerVenda>(this._sessionT5Nh))
            {
                repo.Save(quizVenda).Wait();
            }
        }

        public void SaveAnswerItem(QuizAnswerItem answerItem)
        {
            using (var repo = new Conexao<QuizAnswerItem>(this._sessionT5Nh))
            {
                repo.Save(answerItem).Wait();
            }
        }

        public void SaveAnswerItemMulti(QuizAnswerItemMulti answerItemMulti)
        {
            using (var repo = new Conexao<QuizAnswerItemMulti>(this._sessionT5Nh))
            {
                repo.Save(answerItemMulti).Wait();
            }
        }

        public Quiz GetQuizTroca(long idTroca)
        {
            using (var repo = new Conexao<QuizAnswerVenda>(this._sessionT5Nh))
            {
                return repo.QueryableSelect()
                           .Where(a => a.IdTrocaTitularidade == idTroca)
                           .FirstOrDefault()?.Capa?.Quiz;
            }
        }

        public IEnumerable<QuizAnswerItem> GetAnswersByTroca(long idTroca)
        {
            IEnumerable<QuizAnswerItem> respostas = new List<QuizAnswerItem>();

            using (var repoAnswerVenda = new Conexao<QuizAnswerVenda>(this._sessionT5Nh))
            using (var repoAnswerItem = new Conexao<QuizAnswerItem>(this._sessionT5Nh))
            {
                QuizAnswerCapa capa =
                    repoAnswerVenda.QueryableSelect()
                                   .Where(c => c.IdTrocaTitularidade == idTroca)
                                   .Select(c => c.Capa)
                                   .FirstOrDefault();

                if (capa != null)
                {
                    respostas = repoAnswerItem.QueryableSelect()
                                              .Where(a => a.AnswerCapa.Id == capa.Id)
                                              .ToList();
                }
            }

            return respostas;
        }
    }
}
