using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.Funcionario
{
    public class FuncionarioAnielRepository : IFuncionarioAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public FuncionarioAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<FuncionarioAniel> GetAll()
        {
            using (var repoFuncionario = new Conexao<FuncionarioAniel>(this._sessionT5Nh))
            {
                return repoFuncionario.QueryableSelect();
            }
        }

        public FuncionarioAniel GetByNumFre(string numFre)
        {
            return this.GetAll()
                       .Where(f => f.Num_Fre == numFre)
                       .FirstOrDefault();
        }

        public async Task<List<string>> GetEmailsFuncionario(String tipoEvento)
        {
            StringBuilder sql = new StringBuilder();

            List<string> retorno = new List<string>();

            sql.AppendFormat(@"SELECT C.EMAIL
                                FROM TB_GRUPO_EVENTOS A
                                LEFT JOIN TB_GRUPO_FUNC B
                                ON (A.ID_GRUPO = B.ID_GRUPO)
                                LEFT JOIN TB_FUNCIONARIO C
                                ON (B.NUM_FRE = C.NUM_FRE) ");

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            sql.Append(" WHERE A.ID_TIPO_EVENTO = @TIPOEVENTO");
            parametros.Add("@TIPOEVENTO", tipoEvento);

            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                using (DbDataReader _reader = await CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null))
                {
                    while (_reader.Read())
                    {
                        retorno.Add(Convert.ToString(_reader["EMAIL"]));
                    }
                }
            }
            return retorno;
        }
    }
}
