using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Linq;
using System.Text;
using Aniel.Connect.Repository.Model;
using Sinapse.Model;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Repository.T5.Porta;

namespace Aniel.Connect.Repository.T5.Porta
{
    public class PortaAnielRepository : IPortaAnielRepository
    {
       
        private readonly ISession _sessionT5Nh;

        public PortaAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<Elemento_Rede_Porta> GetAll()
        {
            using (var repoElemento_Porta = new Conexao<Elemento_Rede_Porta>(this._sessionT5Nh))
            {
                return repoElemento_Porta.QueryableSelect();
            }
        }

        public Elemento_Rede_Porta GetPortabyCliente(Int64 idCliente)
        {
            return this.GetAll()
                       .Where(a => a.Cliente_Plano_Prod == idCliente )
                       .FirstOrDefault();
        }

        public void Update(Elemento_Rede_Porta elemento)
        {
            using (var repo = new Conexao<Elemento_Rede_Porta>(this._sessionT5Nh))
            {
                repo.Update(elemento).Wait();
            }
        }




    }
}
