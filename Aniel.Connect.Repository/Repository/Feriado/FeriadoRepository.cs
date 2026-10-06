using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.Feriado
{
    public class FeriadoRepository : IFeriadoRepository
    {
        private readonly ISession _sessionNh;
        private readonly ISession _sessionSql;

        public FeriadoRepository(ISession sessionNh, ISession sessionSql)
        {
            this._sessionNh = sessionNh;
            this._sessionSql = sessionSql;
        }

        public IQueryable<Connect.Model.nhibernate.Feriado> GetAll()
        {
            using (var repoFeriado = new Conexao<Aniel.Connect.Model.nhibernate.Feriado>(this._sessionNh))
            {
                return repoFeriado.QueryableSelect();
            }
        }

        public Aniel.Connect.Model.nhibernate.Feriado GetById(long id)
        {
            return this.GetAll()
                       .Where(f => f.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Aniel.Connect.Model.nhibernate.Feriado> GetFeriadosDia(DateTime dia, string uf, string cidade)
        {
            IList<Aniel.Connect.Model.nhibernate.Feriado> feriados =
                new List<Aniel.Connect.Model.nhibernate.Feriado>();

            using (var com = this._sessionSql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();
                var param = new Dictionary<string, object>();

                sql.AppendLine("SELECT F.ID");
                sql.AppendLine("FROM TB_FERIADO F");
                sql.AppendLine("WHERE F.STATUS = 1");
                sql.AppendLine("  AND (F.ABRANGENCIA = 0 OR F.ABRANGENCIA = 1)");
                sql.AppendLine("  AND EXTRACT(DAY FROM F.\"DATA\") = @DIA_FERIADO");
                sql.AppendLine("  AND EXTRACT(MONTH FROM F.\"DATA\") = @MES_FERIADO");

                param.Add("@DIA_FERIADO", dia.Day);
                param.Add("@MES_FERIADO", dia.Month);

                if (!String.IsNullOrEmpty(uf))
                {
                    sql.AppendLine("UNION");

                    sql.AppendLine("SELECT F.ID");
                    sql.AppendLine("FROM TB_FERIADO F");
                    sql.AppendLine("WHERE F.STATUS = 1");
                    sql.AppendLine("  AND F.ABRANGENCIA = 3");
                    sql.AppendLine("  AND UPPER(F.UF) = UPPER(@UF)");
                    sql.AppendLine("  AND EXTRACT(DAY FROM F.\"DATA\") = @DIA_FERIADO");
                    sql.AppendLine("  AND EXTRACT(MONTH FROM F.\"DATA\") = @MES_FERIADO");

                    param.Add("@UF", uf);

                    if (!String.IsNullOrEmpty(cidade))
                    { 
                        sql.AppendLine("UNION");

                        sql.AppendLine("SELECT F.ID");
                        sql.AppendLine("FROM TB_FERIADO F");
                        sql.AppendLine("WHERE F.STATUS = 1");
                        sql.AppendLine("  AND F.ABRANGENCIA = 2");
                        sql.AppendLine("  AND UPPER(F.UF) = UPPER(@UF)");
                        sql.AppendLine("  AND UPPER(F.CIDADE) = UPPER(@CIDADE)");
                        sql.AppendLine("  AND EXTRACT(DAY FROM F.\"DATA\") = @DIA_FERIADO");
                        sql.AppendLine("  AND EXTRACT(MONTH FROM F.\"DATA\") = @MES_FERIADO");

                        param.Add("@CIDADE", cidade);
                    }
                }

                using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionSql.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        long idFeriado = Convert.ToInt64(reader["ID"]);
                        Aniel.Connect.Model.nhibernate.Feriado feriado = this.GetById(idFeriado);

                        feriados.Add(feriado);
                    } 
                }
            }

            return feriados;
        }
    }
}
