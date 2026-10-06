using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.Cliente
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ISession _sessionNh;

        public ClienteRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.Cliente> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Cliente>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Aniel.Connect.Model.nhibernate.Cliente GetById(long id)
        {
            return this.GetAll()
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }

        public Aniel.Connect.Model.nhibernate.Cliente GetByCpfCnpj(string cpfCnpj)
        {
            return this.GetAll()
                       .Where(c => c.CpfCnpj == cpfCnpj)
                       .FirstOrDefault();
        }

        public long GetMaxNumeroContrato()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Cliente>(this._sessionNh))
            {
                return repo.MaxString("NUMERO_CONTRATO", "TB_CLIENTE").Result;
            }
        }

        public void Update(Aniel.Connect.Model.nhibernate.Cliente cliente)
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Cliente>(this._sessionNh))
            {
                repo.Update(cliente).Wait();
            }
        }

        public void AtualizarIntegradoOla(long IdCliente)
        {
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("UPDATE TB_CLIENTE SET INTEGRADO_OLA = 1 ");
            sql.AppendLine("WHERE ID = @IDCLIENTE");

            parametros.Add("@IDCLIENTE", IdCliente);

            using (var com = _sessionNh.Connection.CreateCommand())            
            {
                CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionNh.Connection, null).Wait();
            }
        }
    }
}
