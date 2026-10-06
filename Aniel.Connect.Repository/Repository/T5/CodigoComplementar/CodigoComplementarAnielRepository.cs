using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.CodigoComplementar
{
    public class CodigoComplementarAnielRepository : ICodigoComplementarAnielRepository
    {
        private readonly ISession _sessionT5;
        public CodigoComplementarAnielRepository(ISession sessionT5)
        {
            _sessionT5 = sessionT5;
        }

        public async Task GetById(ProductToApproval model, Requisicao_SequenciaRmdmAniel obj,MaterialAniel materialAniel)
        {
            using (var _repoCodigoComplementar = new Conexao<Codigo_ComplementarAniel>(this._sessionT5))
            {
                var objCC = await _repoCodigoComplementar.selectFirst(x => x.CodCpl == model.Serie);
                if (objCC == null)
                {
                    var _objCC = new Codigo_ComplementarAniel()
                    {
                        CodCt = obj.Codct,
                        Projeto = obj.Projeto,
                        CodMat = materialAniel.CodMat,
                        CodCpl = model.Serie,
                        Sinal_Ativo = "N",
                        Localizacao = model.ComplementarAuxiliar
                    };

                    await _repoCodigoComplementar.SaveOrUpdate(_objCC);
                }
            }
        }
    }
}
