using Aniel.Connect.Model.nhibernate;

namespace Aniel.Connect.Repository.ConfiguracoesGerais
{
    public interface IConfiguracoesGeraisRepository
    {
        /// <summary>
        ///     Retorna o registro com as configurações do Connect para a empresa
        /// </summary>
        Configuracoes_Gerais GetConfiguracoesGerais();

        string GetUrlOla();

        Configuracoes_Gerais GetFirstConfigGeral();
    }
}
