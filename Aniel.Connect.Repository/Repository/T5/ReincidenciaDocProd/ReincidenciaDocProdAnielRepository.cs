using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.T5.ReincidenciaDocProd
{
    public class ReincidenciaDocProdAnielRepository : IReincidenciaDocProdAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public ReincidenciaDocProdAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<ReincidenciaDocProdAniel> GetAll()
        {
            using (var repoReincidencia = new Conexao<ReincidenciaDocProdAniel>(this._sessionT5Nh))
            {
                return repoReincidencia.QueryableSelect();
            }
        }

        public void Gravar(ReincidenciaDocProdAniel reincidencia)
        {
            using (var repoReincidencia = new Conexao<ReincidenciaDocProdAniel>(this._sessionT5Nh))
            {
                repoReincidencia.SaveOrUpdate(reincidencia).Wait();
            }
        }

        public int QtdServicosReincidentesNaturezaPeriodo(string equipe, string natureza, DateTime dataInicio, DateTime dataFim)
        {
            return this
                .GetAll()
                .Where(r => r.ServicoAnterior.Equipe == equipe &&
                            r.ServicoAnterior.TipoServico.Natureza.Descricao == natureza &&
                            r.ServicoReincidente.Status == StatusExecucaoAniel.Fechada_Produtiva &&
                            dataInicio <= r.ServicoReincidente.Dia && r.ServicoReincidente.Dia <= dataFim)
                .Count();
        }
    }
}
