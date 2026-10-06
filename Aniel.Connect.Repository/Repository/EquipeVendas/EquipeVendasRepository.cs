using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.EquipeVendas
{
    public class EquipeVendasRepository : IEquipeVendasRepository
    {
        private readonly ISession _sessionNh;
        private readonly ISession _sessionSql;

        public EquipeVendasRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public EquipeVendasRepository(ISession sessionNh, ISession sessionSql)
        {
            this._sessionNh = sessionNh;
            this._sessionSql = sessionSql;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.EquipeVenda> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.EquipeVenda>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Aniel.Connect.Model.nhibernate.EquipeVenda GetById(string idEquipeVendas)
        {
            return this.GetAll()
                       .Where(c => c.Id == Convert.ToInt16(idEquipeVendas))
                       .FirstOrDefault();
        }

        //public Aniel.Connect.Model.nhibernate.EquipeVenda GetByNameUpperCase(string nomeEquipeVendas, string siglaUF)
        //{
        //    using (var com = this._sessionSql.Connection.CreateCommand())
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        long Id = 0;
        //        Aniel.Connect.Model.nhibernate.EquipeVenda retorno = null;

        //        sql.AppendLine("SELECT A.ID,A.DESCRICAO ")
        //           .AppendLine(" FROM TB_EquipeVendas A ")
        //           .AppendLine(" LEFT JOIN TB_UF B ")
        //           .AppendLine(" ON (A.ID_UF = B.ID) ");

        //        Dictionary<String, Object> parametros = new Dictionary<string, object>();
        //        sql.AppendLine("WHERE upper(A.DESCRICAO) = UPPER(@EquipeVendas) AND UPPER(B.SIGLA) = UPPER(@UF)");
        //        parametros.Add("@EquipeVendas", nomeEquipeVendas);
        //        parametros.Add("@UF", siglaUF);

        //        using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionSql.Connection, null).Result)
        //        {
        //            if (_reader.Read())
        //            {
        //                Id = Convert.ToInt64(_reader["ID"]);     
        //            }
        //        }

        //        if (Id != 0)
        //        {
        //            retorno = this.GetAll().Where(c => c.Id == Id).FirstOrDefault();
        //        }

        //        return retorno;
        //    }
        //}
    }
}
