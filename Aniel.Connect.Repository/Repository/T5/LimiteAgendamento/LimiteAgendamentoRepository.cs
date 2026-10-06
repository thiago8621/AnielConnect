using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.T5.LimiteAgendamento
{
    public class LimiteAgendamentoRepository : ILimiteAgendamentoAnielRepository
    {
        private readonly ISession _sessionT5Sql;
        private readonly ISession _sessionT5Nh;

        public LimiteAgendamentoRepository(ISession sessionT5Sql, ISession sessionT5Nh)
        {
            this._sessionT5Sql = sessionT5Sql;
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<LimiteAgendamentoAniel> GetAll()
        {
            using (var repoLimAgend = new Conexao<LimiteAgendamentoAniel>(this._sessionT5Nh))
            {
                return repoLimAgend.QueryableSelect();
            }
        }
        
        public void Save(LimiteAgendamentoAniel limite)
        {
            using (var repoLimAgend = new Conexao<LimiteAgendamentoAniel>(this._sessionT5Nh))
            {
                repoLimAgend.Save(limite).Wait();
            }
        }
        
        public void Update(LimiteAgendamentoAniel limite)
        {
            using (var repoLimAgend = new Conexao<LimiteAgendamentoAniel>(this._sessionT5Nh))
            {
                repoLimAgend.Update(limite).Wait();
            }
        }

        public void Remove(LimiteAgendamentoAniel limite)
        {
            using (var repoLimAgend = new Conexao<LimiteAgendamentoAniel>(this._sessionT5Nh))
            {
                repoLimAgend.Remove(limite).Wait();
            }
        }

        public LimiteAgendamentoAniel GetPercNaturezaByAreaData(string codEscritorio, string codArea, int idNatureza, DateTime data)
        {
            return this.GetAll()
                       .Where(p => p.CodEscritorio == codEscritorio &&
                                   p.CodArea == codArea &&
                                   p.IdNatureza == idNatureza &&
                                   (p.DataInicio <= data && data <= p.DataFim))
                       .FirstOrDefault();
        }

        public List<LimiteAgendamentoAniel> GetPercNaturezaByArea(string codEscritorio, string codArea, int idNatureza)
        {
            return this.GetAll()
                       .Where(p => p.CodEscritorio == codEscritorio &&
                                   p.CodArea == codArea &&
                                   p.IdNatureza == idNatureza).ToList();
        }

        public List<LimiteAgendamentoAniel> GetPercByAreaData(string codEscritorio, string codArea, DateTime data)
        {
            return this.GetAll()
                       .Where(p => p.CodEscritorio == codEscritorio &&
                                   p.CodArea == codArea &&
                                   (p.DataInicio <= data && data <= p.DataFim)).ToList();
        }


        public LimiteAgendamentoListResponse GetListagem(LimiteAgendamentoDataTableRequest filtros)
        {
            var response = new LimiteAgendamentoListResponse();

            var sqlFrom = new StringBuilder();
            var param = new Dictionary<string, object>();

            #region SQL From

            sqlFrom.AppendLine("FROM TB_LIMITE_AGENDAMENTO L");

            sqlFrom.AppendLine("INNER JOIN TB_ESCRITORIO E");
            sqlFrom.AppendLine("ON E.COD_ESCRITORIO = L.COD_ESCRITORIO");

            sqlFrom.AppendLine("INNER JOIN TB_AREA A");
            sqlFrom.AppendLine("ON A.COD_ESCRITORIO = L.COD_ESCRITORIO AND A.COD_AREA = L.COD_AREA");

            sqlFrom.AppendLine("INNER JOIN TB_NATUREZA_PRODUCAO N");
            sqlFrom.AppendLine("ON N.ID = L.ID_NATUREZA");

            #region Filtros

            var condicoes = new List<string>();

            if (!String.IsNullOrEmpty(filtros.CodEscritorio))
            {
                condicoes.Add("L.COD_ESCRITORIO = @COD_ESCRITORIO");
                param.Add("@COD_ESCRITORIO", filtros.CodEscritorio);

                if (!String.IsNullOrEmpty(filtros.CodArea))
                {
                    condicoes.Add("L.COD_AREA = @COD_AREA");
                    param.Add("@COD_AREA", filtros.CodArea);
                }
            }

            if (filtros.IdNatureza.HasValue)
            {
                condicoes.Add("L.ID_NATUREZA = @ID_NATUREZA");
                param.Add("@ID_NATUREZA", filtros.IdNatureza.Value);
            }

            DateTime? dataInicioFiltro = !String.IsNullOrEmpty(filtros.DataInicio) ? Convert.ToDateTime(filtros.DataInicio) : (DateTime?)null;
            DateTime? dataFimFiltro = !String.IsNullOrEmpty(filtros.DataFim) ? Convert.ToDateTime(filtros.DataFim) : (DateTime?)null;

            if (dataInicioFiltro.HasValue && dataFimFiltro.HasValue)
            {
                condicoes.Add("@DATA_INICIO <= L.DATA_FIM AND @DATA_FIM >= L.DATA_INICIO");
                param.Add("@DATA_INICIO", dataInicioFiltro.Value);
                param.Add("@DATA_FIM", dataFimFiltro.Value);
            }

            #endregion

            if (condicoes.Count > 0)
            {
                sqlFrom.AppendLine($@"WHERE {String.Join(" AND ", condicoes)}");
            }

            #endregion

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                /* 
                 * Como a listagem pega os registros da TB_LIMITE_AGENDAMENTO, onde está gravado a data do início e
                 * do fim do período configurado, e exibe detalhado por data a contagem dos registros que serão exibidas 
                 * tem que ser feita baseada na quantidade de dias dentro de cada período presente nos registro filtrados
                 * da tabela.
                 */

                sql.AppendLine("SELECT COALESCE(");
                sql.AppendLine("    SUM(");
                sql.AppendLine("        (CASE");
                sql.AppendLine("            WHEN L.DATA_FIM > @DATA_FIM THEN @DATA_FIM");
                sql.AppendLine("            ELSE L.DATA_FIM");
                sql.AppendLine("         END) -");
                sql.AppendLine("        (CASE");
                sql.AppendLine("            WHEN L.DATA_INICIO < @DATA_INICIO THEN @DATA_INICIO");
                sql.AppendLine("            ELSE L.DATA_INICIO");
                sql.AppendLine("         END) + 1), 0) AS QTD_REGISTROS");
                sql.AppendLine(sqlFrom.ToString());
                
                if (this._sessionT5Sql.Transaction != null)
                {
                    this._sessionT5Sql.Transaction.Enlist(com);
                }

                using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionT5Sql.Connection, com.Transaction).Result)
                {
                    if (reader.Read())
                    {
                        response.Total = Convert.ToInt32(reader["QTD_REGISTROS"]);
                    }
                }
            }

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();
                var limites = new List<LimiteAgendamentoListModel>();

                sql.AppendLine("SELECT L.ID, L.COD_ESCRITORIO, L.COD_AREA, L.ID_NATUREZA, E.DESCRICAO AS GRUPO_AREA,");
                sql.AppendLine("       A.DESCRICAO AS AREA, N.DESCRICAO AS NATUREZA, L.PERCENTUAL, L.DATA_INICIO, L.DATA_FIM");
                sql.AppendLine(sqlFrom.ToString());
                sql.AppendLine("ORDER BY E.DESCRICAO, A.DESCRICAO, N.DESCRICAO, L.DATA_INICIO");

                if (this._sessionT5Sql.Transaction != null)
                {
                    this._sessionT5Sql.Transaction.Enlist(com);
                }

                using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionT5Sql.Connection, com.Transaction).Result)
                {
                    while (reader.Read())
                    {
                        var dataInicioLim = Convert.ToDateTime(reader["DATA_INICIO"]);
                        var dataFimLim = Convert.ToDateTime(reader["DATA_FIM"]);

                        if (dataInicioFiltro.HasValue && dataFimFiltro.HasValue)
                        {
                            if (dataInicioLim < dataInicioFiltro.Value)
                                dataInicioLim = dataInicioFiltro.Value;

                            if (dataFimLim > dataFimFiltro.Value)
                                dataFimLim = dataFimFiltro.Value;
                        } 

                        var perc = Convert.ToInt32(reader["PERCENTUAL"]);

                        for (DateTime data = dataInicioLim; data <= dataFimLim; data = data.AddDays(1))
                        {
                            var lim = new LimiteAgendamentoListModel();

                            lim.Id = Convert.ToInt64(reader["ID"]);

                            lim.CodEscritorio = Convert.ToString(reader["COD_ESCRITORIO"]);
                            lim.CodArea = Convert.ToString(reader["COD_AREA"]);
                            lim.IdNatureza = Convert.ToInt32(reader["ID_NATUREZA"]);

                            lim.GrupoArea = Convert.ToString(reader["GRUPO_AREA"]);
                            lim.Area = Convert.ToString(reader["AREA"]);
                            lim.Natureza = Convert.ToString(reader["NATUREZA"]);

                            lim.Data = data.ToString("dd/MM/yyyy");

                            lim.Percentual = String.Format("{0}%", perc);

                            limites.Add(lim);
                        }
                    }
                }

                response.Data = 
                    limites
                        .Skip(filtros.start)
                        .Take(filtros.length);
            }

            return response;
        }
    }
}
