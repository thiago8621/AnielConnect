using System.Collections.Generic;
using System.Linq;
using Aniel.Connect.Model.nhibernate;
using NHibernate.Mapping;

namespace Aniel.Connect.Repository.PesquisaSatisfacaoRepository
{
    public interface IPesquisaSatisfacaoRepository
    {
        /// <summary>
        /// Inserir dados da pesquisa
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="usuario"></param>
        /// <param name="idPesquisa"></param>
        void InsertPesquisaResposta(Aniel.Connect.Model.nhibernate.Cliente cliente, string usuario, long idPesquisa, long? idAtendimento);

        IQueryable<Pesquisa_Satisfacao> GetPesquisaRespostaList();
    }
}
