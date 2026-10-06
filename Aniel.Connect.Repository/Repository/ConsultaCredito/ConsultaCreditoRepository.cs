using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System.Linq;

namespace Aniel.Connect.Repository.ConsultaCredito
{
    public class ConsultaCreditoRepository : IConsultaCreditoRepository
    {
        private readonly ISession _sessionNh;

        public ConsultaCreditoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<ConsultaSerasaDados> GetAll()
        {
            using (var repoConsultaSerasa = new Conexao<ConsultaSerasaDados>(this._sessionNh))
            {
                return repoConsultaSerasa.QueryableSelect();
            }
        }

        public ConsultaSerasaDados GetById(long id) 
        {
            return this.GetAll()
                       .Where(p => p.Id == id)                  
                       .FirstOrDefault();
        }

        public ConsultaSerasaDados GetByCpf(string cpf) 
        {
            cpf = cpf.Replace("-", string.Empty)
                     .Replace(".", string.Empty);

            return this.GetAll()
                       .Where(p => p.cpf == cpf)
                       .OrderByDescending(p => p.DataHoraCriacao)                       
                       .FirstOrDefault();
        }

        public ConsultaSerasaDados GetByCnpj(string cnpj) 
        {
            cnpj = cnpj.Replace("-", string.Empty)
                       .Replace(".", string.Empty)
                       .Replace("/", string.Empty);

            return this.GetAll()
                       .Where(p => p.cnpj == cnpj)
                       .OrderByDescending(p => p.DataHoraCriacao)                       
                       .FirstOrDefault();
        }

        public void Save(ConsultaSerasaDados consultaSerasa)
        {
            using (var repoConsultaSerasa = new Conexao<ConsultaSerasaDados>(this._sessionNh))
            {
                repoConsultaSerasa.Save(consultaSerasa).Wait();
            }
        }
    }
}
