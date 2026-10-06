using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;
using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.Repository.IntegracaoExterna
{
    public class IntegracaoExternaRepository : IIntegracaoExternaRepository
    {
        private readonly ISession _sessionNh;

        public IntegracaoExternaRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        /// <summary>
        /// Retorna integrações externas cadastradas nas configurações da empresa
        /// </summary>
        public IQueryable<Aniel.Connect.Model.nhibernate.IntegracoesExternas> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.IntegracoesExternas>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public DadosAcessoIntegracaoPlayhub GetDadosIntegracaoPlayhub()
        {
            return this.GetAll()
                       .Where(i => i.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.PLAYHUB)
                       .Select(i => new DadosAcessoIntegracaoPlayhub
                       {
                           ApiKey = i.ApiKey,
                           ApiSecret = i.ApiSecret
                       })
                       .FirstOrDefault();
        }

        public bool UtilizaMkConsultaPendenciaEndereco()
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_ENDERECO &&
                                   p.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.MK_SOLUTION)
                       .Count() > 0;
        }

        public bool UtilizaMkConsultaPendenciaFinanceira()
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_FINANCEIRA &&
                                   p.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.MK_SOLUTION)
                       .Count() > 0;
        }

        public bool UtilizaAlcansConsultaPendenciaFinanceira()
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_FINANCEIRA &&
                                   p.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.ALCANS)
                       .Count() > 0;
        }

        public bool UtilizaVoalleConsultaPendenciaFinanceira()
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_FINANCEIRA &&
                                   p.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.VOALLE)
                       .Count() > 0;
        }
        public bool UtilizaIXCConsultaPendenciaFinanceira()
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_FINANCEIRA &&
                                   p.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.IXC)
                       .Count() > 0;
        }
        public Aniel.Connect.Model.nhibernate.IntegracoesExternas GetIntegracaPrequal() 
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.CONSULTA_CREDITO &&
                                   p.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.PREQUAL).FirstOrDefault();
        }

        public Aniel.Connect.Model.nhibernate.IntegracoesExternas GetIntegracaoProvisionamento()
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.PROVISIONAMENTO).FirstOrDefault();
        }

        public void save(Aniel.Connect.Model.nhibernate.IntegracoesExternas integracao) 
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.IntegracoesExternas>(this._sessionNh))
            {
                repo.Save(integracao).Wait();
            }
            
        }

        public bool UtilizaIXCConsultaPendenciaEndereco()
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_ENDERECO &&
                                   p.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.IXC)
                       .Count() > 0;
        }

        public bool UtilizaAlcansConsultaPendenciaEndereco()
        {
            return this.GetAll()
                       .Where(p => p.TipoIntegracao == Aniel.Connect.Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_ENDERECO &&
                                   p.IntegracaoExterna == Aniel.Connect.Model.enumerado.IntegracaoExterna.ALCANS)
                       .Count() > 0;
        }
    }
}
