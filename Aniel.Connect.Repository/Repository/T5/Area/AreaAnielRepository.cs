using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Linq;
using System.Text;
using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.Repository.T5.Area
{
    public class AreaAnielRepository : IAreaAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public AreaAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<AreaAniel> GetAll()
        {
            using (var repoArea = new Conexao<AreaAniel>(this._sessionT5Nh))
            {
                return repoArea.QueryableSelect();
            }
        }

        public AreaAniel GetByCodigo(string codEscritorio, string codArea)
        {
            return this.GetAll()
                       .Where(a => a.Cod_Escritorio == codEscritorio &&
                                   a.Cod_Area == codArea)
                       .FirstOrDefault();
        }

        public AreaResponse GetByCidade(String Desc_Area)
        {
            using (var com = this._sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                AreaResponse retorno = null;

                sql.AppendLine("SELECT A.COD_CLIENTE, A.COD_ESCRITORIO, A.COD_AREA, A.DESCRICAO, A.CODCT, A.PROJETO, B.DESCRICAO AS DESCRICAO_ESCRITORIO ")
                   .AppendLine(" FROM TB_AREA A ")
                   .AppendLine(" INNER JOIN TB_ESCRITORIO B ")
                   .AppendLine(" ON (A.COD_ESCRITORIO = B.COD_ESCRITORIO) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("WHERE upper(A.DESCRICAO) = @DESCRICAO ");
                parametros.Add("@DESCRICAO", Desc_Area.ToUpper());

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new AreaResponse()
                        {
                            codCliente = _reader["COD_CLIENTE"] != DBNull.Value ? Convert.ToInt32(_reader["COD_CLIENTE"]) : 0,
                            codEscritorio = Convert.ToString(_reader["COD_ESCRITORIO"]),
                            codArea = Convert.ToString(_reader["COD_AREA"]),
                            descricaoArea = Convert.ToString(_reader["DESCRICAO"]),
                            codCt = Convert.ToString(_reader["CODCT"]),
                            projeto = Convert.ToString(_reader["PROJETO"]),
                            descricaoEscritorio = Convert.ToString(_reader["DESCRICAO_ESCRITORIO"]),
                        };
                    }
                }

                return retorno;
            }
        }
    }
}
