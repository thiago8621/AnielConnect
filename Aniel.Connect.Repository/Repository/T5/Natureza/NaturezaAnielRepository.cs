using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.Natureza
{
    public class NaturezaAnielRepository : INaturezaAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public NaturezaAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<Natureza_ProducaoAniel> GetAll()
        {
            using (var repoNatureza = new Conexao<Natureza_ProducaoAniel>(this._sessionT5Nh))
            {
                return repoNatureza.QueryableSelect();
            }
        }

        public Int32 GetNaturezaByCodTipoServ(Int32 pCodTipoServ, string pCodCt)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 COALESCE(COD_NATUREZA,0)AS COD_NATUREZA ");
                sql.AppendLine(" FROM TB_TIPO_SERVICO_EQUIPE A ");
                sql.AppendLine(" WHERE COD_TIPO_SERV = @pCOD_TIPO_SERV AND A.CODCT = @pCOD_CT");
                parametros.Add("@pCOD_TIPO_SERV", pCodTipoServ);
                parametros.Add("@pCOD_CT", pCodCt);
                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null).Result)
                {
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader["COD_NATUREZA"]);
                    }

                }
            }
            return 0;
        }

    }
}
