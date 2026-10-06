using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.T5.ElementoRede
{
    public class ElementoRedeAnielRepository : IElementoRedeAnielRepository
    {
        private readonly ISession _sessionT5Nh;
        private IIntegracaoElementoRede _integracao;
        private IIntegracaoLogin _integracaoLogin;

        public ElementoRedeAnielRepository(ISession sessionT5Nh, IIntegracaoElementoRede integracao)
        {
            this._sessionT5Nh = sessionT5Nh;
            this._integracao = integracao;
        }
        public ElementoRedeAnielRepository(ISession sessionT5Nh) 
        {
            this._sessionT5Nh = sessionT5Nh;
        }
        #region  Queryables       
        public IQueryable<Elemento_Rede> GetAll()
        {
            using (var repo = new Conexao<Elemento_Rede>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }
        public IQueryable<Elemento_Rede_Enderecos> GetAllEnderecos()
        {
            using (var repo = new Conexao<Elemento_Rede_Enderecos>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }
        public IQueryable<Elemento_RedeCampos> GetAllCamposDinamicos()
        {
            using (var repo = new Conexao<Elemento_RedeCampos>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }
        public IQueryable<Elemento_Rede_Fibra> GetAllFibras()
        {
            using (var repo = new Conexao<Elemento_Rede_Fibra>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }
        public IQueryable<Elemento_RedeFotosArquivos> GetAllFiles()
        {
            using (var repo = new Conexao<Elemento_RedeFotosArquivos>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }
        #endregion
        public Elemento_Rede GetById(long id)
        {
            return this.GetAll()
                       .Where(e => e.Id == id)
                       .FirstOrDefault();
        }
        public IEnumerable<Elemento_Rede_Enderecos> GetEnderecosByElement(long id)
        {
            return this.GetAllEnderecos()
                       .Where(p => p.Elemento_Rede.Id == id)
                       .ToList();
        }
        public IEnumerable<Elemento_RedeCampos> GetCamposDinamicosByElement(long id)
        {
            return this.GetAllCamposDinamicos()
                       .Where(p => p.Elemento.Id == id)
                       .ToList();
        }
        public IEnumerable<Elemento_Rede_Fibra> GetFibrasByElement(long id)
        {
            return this.GetAllFibras()
                       .Where(p => p.Elemento_Cabo.Id == id)
                       .OrderByDescending(p => p.DataHoraCriacao)
                       .ToList();
        }
        public IEnumerable<Elemento_RedeFotosArquivos> GetFilesByElement(long id)
        {

            return this.GetAllFiles()
                       .Where(p => p.Elemento.Id == id)
                       .ToList();
        }
        public void saveEnderecos(IEnumerable<Elemento_Rede_Enderecos> enderecos)
        {
            using (var repo = new Conexao<Elemento_Rede_Enderecos>(this._sessionT5Nh))
            {
                repo.SaveOrUpdate(enderecos).Wait();
            }
        }

        public void SaveOrUpdate(Elemento_Rede elemento)
        {
            using (var repo = new Conexao<Elemento_Rede>(this._sessionT5Nh))
            {
                repo.SaveOrUpdate(elemento).Wait();
            }
        }

        public void SaveOrUpdatePorta(Elemento_Rede_Porta porta)
        {
            using (var repo = new Conexao<Elemento_Rede_Porta>(this._sessionT5Nh))
            {
                repo.SaveOrUpdate(porta).Wait();
            }
        }

        public void SaveOrUpdateLogin(LoginRadius loginRadius)
        {
            using (var repo = new Conexao<LoginRadius>(this._sessionT5Nh))
            {
                repo.SaveOrUpdate(loginRadius).Wait();
            }
        }

        public Elemento_Rede GetByDescription(string descricao)
        {
            return this.GetAll()
                       .Where(e => e.Descricao == descricao)
                       .FirstOrDefault();
        }

        public IEnumerable<ImportacaoElementoHubSoft> ImportacaoIntegracao(string urlLogin, string elementoRede, LoginHubsoftModel login)
        {
            List<ImportacaoElementoHubSoft> listagem = new List<ImportacaoElementoHubSoft>();            
            listagem.AddRange(this._integracao.GetCaixas(urlLogin, login, elementoRede));
            return listagem;
        }

        public Tipo_Elemento_Rede GetTipoElementoRedeByCodAuxiliar(string codigoAuxiliar)
        {
            using (var repo = new Conexao<Tipo_Elemento_Rede>(this._sessionT5Nh))
            {
                return repo.QueryableSelect()
                           .Where(t => t.Codigo_Auxiliar == codigoAuxiliar)
                           .FirstOrDefault();
            }
        }

        public Tipo_Elemento_Rede GetTipoElementoRedeById(long Id)
        {
            using (var repo = new Conexao<Tipo_Elemento_Rede>(this._sessionT5Nh))
            {
                return repo.QueryableSelect()
                           .Where(t => t.Id == Id)
                           .FirstOrDefault();
            }
        }

        public IQueryable<Elemento_Rede_Porta> GetAllPortas()
        {
            using (var repo = new Conexao<Elemento_Rede_Porta>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }

        
    }   
}
