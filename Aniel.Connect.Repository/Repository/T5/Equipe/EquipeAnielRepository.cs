using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using RestSharp.Validation;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.Repository.T5.Equipe
{
    public class EquipeAnielRepository : IEquipeAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public EquipeAnielRepository(ISession sessionT5Nh)
        {
            _sessionT5Nh = sessionT5Nh;
        }

        public bool AdicionarTecnicoAuxiliar(string Equipe, Documento_ProducaoAniel documento)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var comIns = _sessionT5Nh.Connection.CreateCommand())
            {
                parametros.Add("@DIA", DateTime.Now.ToString("yyyy-MM-dd"));

                parametros.Add("@HORA", DateTime.Now.ToString("HH-mm-ss"));
                parametros.Add("@NUM_OBRA", documento.Num_Obra);
                parametros.Add("@EQUIPE", Equipe);
                parametros.Add("@PROJETO", documento.Projeto);
                parametros.Add("@NUM_DOC", documento.Num_Doc);
                parametros.Add("@CODCT", documento.CodCt);

                StringBuilder sqlIns = new StringBuilder($"INSERT INTO TB_MOBILE_PARTICIPANTES_RDO (DIA_CRIACAO ,HORA_CRIACAO,NUM_OBRA, EQUIPE,PROJETO,CODCT,NUM_DOC)VALUES(CURRENT_DATE, CURRENT_TIME,@NUM_OBRA, @EQUIPE,@PROJETO,@CODCT,@NUM_DOC)  ");
                CommandHelper.ExecuteNonQuery(comIns, sqlIns, parametros, _sessionT5Nh.Connection, null).Wait();
                return true;
            }
        }

        public IQueryable<EquipeAniel> GetAll()
        {
            using (var repoEquipe = new Conexao<EquipeAniel>(_sessionT5Nh))
            {
                return repoEquipe.QueryableSelect();
            }
        }

        public string GetNome(string equipe)
        {
            return GetAll()
                    .Where(e => e.Matricula == equipe)
                    .Select(e => e.Nome)
                    .FirstOrDefault();
        }


        public System.Collections.Generic.List<EquipeAuxiliaresModel> GetTecnicosAuxiliares(EquipeAuxiliaresModel request)
        {
            List<EquipeAuxiliaresModel> retorno = new List<EquipeAuxiliaresModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<string> conditions = new List<string>();

                if (!string.IsNullOrEmpty(request.DataInicio))
                {
                    if (request.TipoData.ToLower() == "agendamento")
                    {
                        conditions.Add("C.DATA_MAXIMA  BETWEEN @DataInicio AND @DATA_FIM ");
                        parametros.Add("@DataInicio", request.DataInicio);
                        parametros.Add("@DATA_FIM", request.DataFim);
                    }
                    else
                    {
                        conditions.Add("C.DATA_PROD BETWEEN @DataInicio AND @DATA_FIM ");
                        parametros.Add("@DataInicio", request.DataInicio);
                        parametros.Add("@DATA_FIM", request.DataFim);
                    }

                }



                if (!string.IsNullOrEmpty(request.CodCt))
                {
                    conditions.Add("A.CODCT = @CODCT");
                    parametros.Add("@CODCT", request.CodCt);
                }

                if (!string.IsNullOrEmpty(request.Projeto))
                {
                    conditions.Add("A.PROJETO = @PROJETO");
                    parametros.Add("@PROJETO", request.Projeto);
                }
                if (!string.IsNullOrEmpty(request.TipoServico))
                {
                    conditions.Add("G.DESCRICAO >= @DESCRICAO");
                    parametros.Add("@DESCRICAO", request.TipoServico);
                }
                if (!string.IsNullOrEmpty(request.Supervisor))
                {
                    conditions.Add("B.COD_SUPERV >= @SUPERVISOR");
                    parametros.Add("@SUPERVISOR", request.Supervisor);
                }
                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder($@"SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.TAREFA, A.EQUIPE, B.NOME, C.NUM_DOC, F.DESCRICAO
                                                                FROM TB_MOBILE_PARTICIPANTES_RDO A
                                                                INNER JOIN TB_DOCUMENTO_PRODUCAO C ON C.NUM_OBRA = A.NUM_OBRA 
                                                                LEFT JOIN TB_EQUIPE B ON(A.EQUIPE = B.EQUIPE)
                                                                LEFT JOIN TB_EQUIPE_AUX D ON D.EQUIPE = A.EQUIPE
                                                                LEFT JOIN TB_TIPO_SERVICO_EQUIPE F 
                                                                ON(C.COD_TIPO_SERV = F.COD_TIPO_SERV)
                                                                LEFT JOIN TB_FUNCIONARIO K 
                                                                ON(K.NUM_FRE = B.COD_SUPERV)");


                    if (conditions.Count > 0)
                    {
                        sql.Append(" WHERE ");
                        sql.Append(string.Join(" AND ", conditions));
                    }

                    //Busca com paginacao
                    using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null).Result)
                    {
                        while (reader.Read())
                        {
                            retorno.Add(new EquipeAuxiliaresModel
                            {
                                CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : String.Empty,
                                Num_Obra = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : String.Empty,
                                Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : String.Empty,
                                Tarefa = reader["TAREFA"] != DBNull.Value ? Convert.ToString(reader["TAREFA"]) : String.Empty,
                                Nome = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : String.Empty,
                                Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (int?)null,
                                TipoServico = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
                                Equipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : String.Empty

                            });
                        }
                    }

                }
            }
            return retorno;
        }

        public DataTableResponse<EquipeAuxiliaresModel> GetTecnicosAuxiliaresRelatorio(EquipeAuxiliaresModelRequest request)
        {
            List<EquipeAuxiliaresModel> retorno = new List<EquipeAuxiliaresModel>();
            int? recordsTotal = 0;
            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<string> conditions = new List<string>();

                if (!string.IsNullOrEmpty(request.DataInicio))
                {
                    if (request.TipoData.ToLower() == "agendamento")
                    {
                        conditions.Add("C.DATA_MAXIMA  BETWEEN @DataInicio AND @DATA_FIM ");
                        parametros.Add("@DataInicio", request.DataInicio);
                        parametros.Add("@DATA_FIM", request.DataFim);
                    }
                    else
                    {
                        conditions.Add("C.DATA_PROD BETWEEN @DataInicio AND @DATA_FIM ");
                        parametros.Add("@DataInicio", request.DataInicio);
                        parametros.Add("@DATA_FIM", request.DataFim);
                    }

                }

                if (!string.IsNullOrEmpty(request.Num_Obra))
                {
                    conditions.Add("A.Num_Obra = @Num_Obra");
                    parametros.Add("@Num_Obra", request.Num_Obra);
                }

                if (!string.IsNullOrEmpty(request.Equipe))
                {
                    conditions.Add("A.EQUIPE = @EQUIPE");
                    parametros.Add("@EQUIPE", request.Equipe);
                }


                if (!string.IsNullOrEmpty(request.CodCt))
                {
                    conditions.Add("A.CODCT = @CODCT");
                    parametros.Add("@CODCT", request.CodCt);
                }

                if (!string.IsNullOrEmpty(request.Projeto))
                {
                    conditions.Add("A.PROJETO = @PROJETO");
                    parametros.Add("@PROJETO", request.Projeto);
                }
                if (!string.IsNullOrEmpty(request.CodTipoServ))
                {
                    conditions.Add("C.COD_TIPO_SERV >= @COD_TIPO_SERV");
                    parametros.Add("@COD_TIPO_SERV", request.CodTipoServ);
                }
                if (!string.IsNullOrEmpty(request.Supervisor))
                {
                    conditions.Add("B.COD_SUPERV >= @SUPERVISOR");
                    parametros.Add("@SUPERVISOR", request.Supervisor);
                }
                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder($@"SELECT COUNT(*) OVER() AS TOTAL_REG,A.CODCT, A.PROJETO, C.NUM_OBRA, A.TAREFA, A.EQUIPE, B.NOME, C.NUM_DOC, F.DESCRICAO
                                                                FROM TB_MOBILE_PARTICIPANTES_RDO A
                                                                INNER JOIN TB_DOCUMENTO_PRODUCAO C ON C.NUM_OBRA = A.NUM_OBRA 
                                                                LEFT JOIN TB_EQUIPE B ON(A.EQUIPE = B.EQUIPE)
                                                                LEFT JOIN TB_EQUIPE_AUX D ON D.EQUIPE = A.EQUIPE
                                                                LEFT JOIN TB_TIPO_SERVICO_EQUIPE F 
                                                                ON(C.COD_TIPO_SERV = F.COD_TIPO_SERV)
                                                                LEFT JOIN TB_FUNCIONARIO K 
                                                                ON(K.NUM_FRE = B.COD_SUPERV)");


                    if (conditions.Count > 0)
                    {
                        sql.Append(" WHERE ");
                        sql.Append(string.Join(" AND ", conditions));
                    }

                    //Busca com paginacao
                    using (var reader = CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start).Result)
                    {
                        while (reader.Read())
                        {
                            recordsTotal = Convert.ToInt32(reader["TOTAL_REG"]);
                            retorno.Add(new EquipeAuxiliaresModel
                            {
                                CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : String.Empty,
                                Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : String.Empty,
                                Num_Obra = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : String.Empty,
                                Tarefa = reader["TAREFA"] != DBNull.Value ? Convert.ToString(reader["TAREFA"]) : String.Empty,
                                Nome = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : String.Empty,
                                Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (int?)null,
                                TipoServico = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
                                Equipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : String.Empty

                            });
                        }
                    }

                }
            }
            return new DataTableResponse<EquipeAuxiliaresModel>
            {
                data = retorno,
                recordsFiltered = recordsTotal
            };
        }

       

        public bool RemoverTecnicoAuxiliar(string Equipe, Documento_ProducaoAniel documento)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (var comDelete = _sessionT5Nh.Connection.CreateCommand())
            {
                parametros.Add("@NUM_OBRA", documento.Num_Obra);
                parametros.Add("@CODCT", documento.CodCt);
                parametros.Add("@PROJETO", documento.Projeto);
                parametros.Add("@NUMDOC", documento.Num_Doc);
                parametros.Add("@EQUIPE", Equipe);
                

                StringBuilder sqlDelete = new StringBuilder($@"DELETE FROM TB_MOBILE_PARTICIPANTES_RDO 
                                                               WHERE EQUIPE = @EQUIPE AND 
                                                               CODCT = @CODCT AND 
                                                               PROJETO = @PROJETO AND
                                                               NUM_DOC = @NUMDOC AND 
                                                               NUM_OBRA = @NUM_OBRA");
                CommandHelper.ExecuteNonQuery(comDelete, sqlDelete, parametros, _sessionT5Nh.Connection, null).Wait();

                return true;
              }
         }


        public EquipeIntegracaoModel GetEquipe(String Matricula)
        {
            StringBuilder sql = new StringBuilder();

            EquipeIntegracaoModel retorno = null;

            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT FIRST 1 A.EQUIPE, A.EQUIPE_INTEGRATOR, A.NOME , C.CODCT_PADRAO, C.PROJETO_PADRAO, A.CODCT_INTEGRATOR, A.PROJETO_INTEGRATOR, C.LATITUDE, C.LONGITUDE ");
                sql.AppendLine("FROM TB_EQUIPE A ");
                sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B ");
                sql.AppendLine("ON (A.NUM_FRE = B.NUM_FRE)");
                sql.AppendLine("LEFT JOIN TB_FILIAL C ");
                sql.AppendLine("ON (C.CODEMPRESA = B.CODEMPRESA AND C.CODFILIAL = B.CODFILIAL) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("WHERE (A.EQUIPE = @EQUIPE OR A.EQUIPEAUX = @EQUIPE) ");
                parametros.Add("@EQUIPE", Matricula);

                using (var _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new EquipeIntegracaoModel()
                        {
                            Matricula = Convert.ToString(_reader["EQUIPE"]),
                            Nome = Convert.ToString(_reader["NOME"]),
                            CodCtPadrao = String.IsNullOrEmpty(Convert.ToString(_reader["CODCT_INTEGRATOR"])) ? Convert.ToString(_reader["CODCT_PADRAO"]) : Convert.ToString(_reader["CODCT_INTEGRATOR"]),
                            ProjetoPadrao = String.IsNullOrEmpty(Convert.ToString(_reader["PROJETO_INTEGRATOR"])) ? Convert.ToString(_reader["PROJETO_PADRAO"]) : Convert.ToString(_reader["PROJETO_INTEGRATOR"]),
                            CodAux = Convert.ToString(_reader["EQUIPE_INTEGRATOR"]),
                            Latitude = _reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["LATITUDE"]) : (double?)null,
                            Longitude = _reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["LONGITUDE"]) : (double?)null,
                        };
                    }
                }

                return retorno;

            }
        }
    }
}
