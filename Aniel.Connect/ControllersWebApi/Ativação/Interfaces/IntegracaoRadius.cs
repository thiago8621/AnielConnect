using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.nhibernate;
using NetTools;
using Aniel.Connect.Models;
using Aniel.Connect.Models.Ativacao;
using Aniel.Connect.Model.enumerado;

namespace Aniel.Connect.ControllersWebApi.Ativacao.Interfaces
{
    public interface IntegracaoRadius
    {
        ISession GetSessionRadius();
        #region Concentradora
        
        Task CadastrarConcentradora(ISession session, Concentradora concentradora);
        Task EditarConcentradora(ISession session,Concentradora concentradora);
        Task DeletarConcentradora(ISession session, long idConcentradora);

        #endregion

        #region Usuário

        Task CadastrarUsuario(ISession session, string usuario, string senha, string nasIp, long idCliente, Pool_Ip pool = null, string mac = null);
        Task CadastrarUsuario(ISession session, string usuario, string senha, long idCliente);
        Task FinalizarCadastroUsuario(ISession session, string usuario, string nasIp, long idCliente, Pool_Ip pool = null, string mac = null);
        
        Task EditarUsuario(ISession session, string usuario, string senha, string nasIp, long idCliente, Pool_Ip pool = null, string mac = null);
        Task EditarUsuario(ISession session, string usuario, string senha, long idCliente);
        Task DeletarUsuario(ISession session, long idCliente);
        
        Task BloquearUsuario(ISession session, string usuario, string ipConcentradora, string portaIncoming, string secret);
        Task DesbloquearUsuario(ISession session, string Usuario, string Plano);

        Task BloquearSinalPorPool(ISession session, string usuario, string ipConcentradora, string portaIncoming, string nomePoolBloqueio);
        Task DesbloquearSinalPorPool(ISession session, string usuario, string NomePool);

        Task BloquearSinalPorDomain(ISession session, string Login, string IpConcentradora, string PortaIncoming, string IdDomainName, long IdCliente);
        Task DesbloquearSinalPorDomain(ISession session, ISession sessionConnect, LoginRadius login);

        Task AssociaUsuarioAoPlano(ISession session, string UsuarioPPPOE, string NomePlano, string Prioridade, long IdClientePlanoProduto);

        Task CadastrarPoolIp(ISession session, string PoolName, IPAddressRange RangeIP);
        Task EditarPoolIp(ISession session, string PoolName, IPAddressRange Novo_RangeIP, string Novo_PoolName = null);
        Task DeletarPoolIp(ISession session, string PoolName);
        Task AtualizarPoolIpUsuario(ISession session, long Id_Cliente, string Novo_PoolName);

        Task<string> GetPoolBloqueioDisponivel(ISession session, List<string> Pools_Bloqueio);
        Task<string> GetPoolReducaoVelocidadeDisponivel(ISession session, List<String> Pools_Reducao);

        bool TesteLogin(string ipConcentradora, string usuario, string senha);

        #endregion

        void ReiniciarServicoRadius();

        Task<GetHistoricoConexaoRadius_Response> GetHistoricoConexao(ISession session, GetHistoricoConexaoRadius_DataTableRequest request);
        Task<List<ConsumoPeriodoModel>> GetConsumoPeriodo(ISession session, String Login_Radius, HistoricoConsumoPeriodoEnum Periodo_Consumo, DateTime? Data_Inicio, DateTime? Data_Fim);
        Task<ConsumoPeriodoModel> GetConexaoTempoReal(ISession session, String Login_Radius, Medida_Velocidade_ConexaoEnum Unidade_Medida_Velocidade);
        Task<List<TempoOnlineOfflineModel>> GetTempoOnline(ISession session, String Login_Radius);
        Task<List<TempoOnlineOfflineModel>> GetTempoOffline(ISession session, String Login_Radius);
    }
}
