using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.T5.SubtipoServico
{
    public class SubtipoServicoAnielRepository : ISubtipoServicoAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public SubtipoServicoAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public Sub_Tipo_ServicoAniel GetByDescricaoTipoServ(string descricao,short codTipoServ)
        {
            using (var repoSubtipoServico = new Conexao<Sub_Tipo_ServicoAniel>(this._sessionT5Nh))
            {
                return repoSubtipoServico.selectFirst(p => p.Descricao == descricao && p.Codigo_Tipo == codTipoServ).Result;
            }
        }

        public Sub_Tipo_ServicoAniel GetByCodigo(string codSubtipo,short codTipo)
        {
            using (var repoTipoServico = new Conexao<Sub_Tipo_ServicoAniel>(this._sessionT5Nh))
            {
                return repoTipoServico.selectFirst(p => p.Codigo_Tipo == codTipo && p.Codigo_Sub_Tipo == codSubtipo).Result;
            }
        }
    }
}
