using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.Cidade
{
    public class CidadeRepository : ICidadeRepository
    {
        private readonly ISession _sessionNh;
        private readonly ISession _sessionSql;

        public CidadeRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public CidadeRepository(ISession sessionNh, ISession sessionSql)
        {
            this._sessionNh = sessionNh;
            this._sessionSql = sessionSql;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.Cidade> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Cidade>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Aniel.Connect.Model.nhibernate.Cidade GetByName(string nomeCidade, string siglaUF, string nomePais)
        {
            return this.GetAll()
                       .Where(c => c.Pais.Nome == nomePais &&
                                   c.Id_UF.Sigla == siglaUF &&
                                   c.Descricao == nomeCidade)
                       .FirstOrDefault();
        }
        
        public Aniel.Connect.Model.nhibernate.Cidade GetById(string idCidade, string idUF)
        {
            return this.GetAll()
                       .Where(c => c.Id_UF.Id == Convert.ToInt32(idUF) &&
                                   c.Id == Convert.ToInt32(idCidade))
                       .FirstOrDefault();
        }

        public Aniel.Connect.Model.nhibernate.Cidade GetById(string idCidade)
        {
            return this.GetAll()
                       .Where(c => c.Id == Convert.ToInt32(idCidade))
                       .FirstOrDefault();
        }

        public Aniel.Connect.Model.nhibernate.Cidade GetByNameUpperCase(string nomeCidade, string siglaUF)
        {
            using (var com = this._sessionSql.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                long Id = 0;
                Aniel.Connect.Model.nhibernate.Cidade retorno = null;

                sql.AppendLine("SELECT A.ID,A.DESCRICAO ")
                   .AppendLine(" FROM TB_CIDADE A ")
                   .AppendLine(" LEFT JOIN TB_UF B ")
                   .AppendLine(" ON (A.ID_UF = B.ID) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("WHERE upper(A.DESCRICAO) = UPPER(@CIDADE) AND UPPER(B.SIGLA) = UPPER(@UF)");
                parametros.Add("@CIDADE", nomeCidade);
                parametros.Add("@UF", siglaUF);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionSql.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        Id = Convert.ToInt64(_reader["ID"]);     
                    }
                }

                if (Id != 0)
                {
                    retorno = this.GetAll().Where(c => c.Id == Id).FirstOrDefault();
                }

                return retorno;
            }
        }
    }
}
