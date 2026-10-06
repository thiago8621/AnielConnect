using Aniel.Connect.Repository.T5.CodigoEncerramento;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class CodigoEncerramentoAnielBusiness
    {
        private readonly ICodigoEncerramentoAnielRepository _repoEncerramento;

        public CodigoEncerramentoAnielBusiness(ICodigoEncerramentoAnielRepository repoEncerramento)
        {
            this._repoEncerramento = repoEncerramento;
        }

        public string GetDescricao(string codEncerramento, string codCt)
        {
            return this._repoEncerramento.GetDescricao(codEncerramento, codCt);
        }
    }
}
