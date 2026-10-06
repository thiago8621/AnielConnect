using Aniel.Connect.Model.nhibernate;
using System.Threading.Tasks;
using NHibernate;
using System;

namespace Aniel.Connect.ControllersWebApi.Ativacao.Interfaces
{
    public interface IntegracaoConcentradora
    {
        Task ConfiguracaoPlanoCliente(ISession session, LoginRadius login);
        Task ReduzirVelocidade(ISession session, ISession sessionConnect, LoginRadius login);
        Task NormalizarVelocidade(ISession session, ISession sessionConnect, LoginRadius login);

        /// <summary>
        ///     Envia um pacote CoA (Change of Authorization) para a concentradora, que permite alterar a 
        ///     velocidade da internet do cliente sem precisar o desconectar.
        /// </summary>
        void AplicarModificacaoVelocidade(Model.nhibernate.Radius servidor, string usuario, string ipConcentradora, string velocidade);

        Task DesbloqueioConfianca(ISession session, Cliente_PlanoProduto planoProduto, String usuario);
        Task EditarPlanoCliente(ISession session, Plano plano);
        Task DeletarPlano(ISession session, long IdClientePlanoProduto);
    }
}