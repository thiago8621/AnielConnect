//using Aniel.Connect.ControllersWebApi.Ativação.Concentradoras;
//using Aniel.Connect.ControllersWebApi.Ativacao.Interfaces;
//using Aniel.Connect.ControllersWebApi.Ativacao.Radius;
//using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
//using Aniel.Connect.Models;
//using Aniel.Connect.Models.Ativacao;
using NetTools;
using NHibernate;
using Sinapse.Integracao.Ativacao.Interfaces;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Model.enumerados;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sinapse.DAO.MongoDB;

namespace Aniel.Connect.ControllersWebApi.Ativacao.Repositorio
{
    public class RadiusRepository
    {
        #region Concentradora

        public static void CadastrarConcentradora(IntegracaoRadius integracao, ISession session, Concentradora concentradora) 
        {
            integracao.CadastrarConcentradora(session, concentradora);
        }
        public static void EditarConcentradora(IntegracaoRadius integracao, ISession session, Concentradora concentradora)
        {
            integracao.EditarConcentradora(session, concentradora);
        }
        public static void DeletarConcentradora(IntegracaoRadius integracao, ISession session, Int64 idConcentradora)
        {
            integracao.DeletarConcentradora(session, idConcentradora);
        }

        #endregion

        #region Usuário

        public static void CadastrarUsuario(IntegracaoRadius integracao, ISession session, LoginRadiusModel login)
        {
            integracao.CadastrarUsuario(session, login);
        }
        public static void CadastrarUsuario(IntegracaoRadius integracao, ISession session, String LoginRadius, String SenhaRadius, Int64 idCliente)
        {
            integracao.CadastrarUsuario(session, LoginRadius, SenhaRadius, idCliente);
        }        

        public static void EditarUsuario(IntegracaoRadius integracao, ISession session, LoginRadiusModel login)
        {
            integracao.EditarUsuario(session, login);
        }
        public static void EditarUsuario(IntegracaoRadius integracao, ISession session, String LoginRadius, String SenhaRadius, Int64 idCliente, Int64 idClientePlanoProduto)
        {
            integracao.EditarUsuario(session, LoginRadius, SenhaRadius, idCliente, idClientePlanoProduto);
        }
        public static void DeletarUsuario(IntegracaoRadius integracao, ISession session, Int64 idCliente)
        {
            integracao.DeletarUsuario(session, idCliente);
        }
        
        public static void BloquearUsuario(IntegracaoRadius integracao, ISession session, String Login, String Senha,String IpConcentradora,String PortaIncoming,String Secret, bool UsaRadiusSemContainer)
        {
            integracao.BloquearUsuario(session, Login, IpConcentradora, PortaIncoming, Secret, UsaRadiusSemContainer);
        }
        public static async Task DesbloquearUsuario(IntegracaoRadius integracao, ISession session, String Login, String Plano)
        {
            await integracao.DesbloquearUsuario(session, Login, Plano);
        }
        
        public static void BloquearSinalPorPool(IntegracaoRadius integracao, ISession session, String Login, long idCliente, String IpConcentradora, String PortaIncoming, String NomePoolBloqueio, bool UsaRadiusSemContainer)
        {
            integracao.BloquearSinalPorPool(session, Login, idCliente, IpConcentradora, PortaIncoming, NomePoolBloqueio, UsaRadiusSemContainer);
        }

        public static async Task DesbloquearSinalPorPool(IntegracaoRadius integracao, ISession session, LoginRadiusModel login)
        {
            await integracao.DesbloquearSinalPorPool(session, login);
        }

        public static void BloquearSinalPorDomain(IntegracaoRadius integracao, ISession session, String Login, String IpConcentradora, String PortaIncoming, String IdDomainName, long IdCliente, bool UsaRadiusSemContainer)
        {
            integracao.BloquearSinalPorDomain(session, Login, IpConcentradora, PortaIncoming, IdDomainName, IdCliente, UsaRadiusSemContainer);
        }

        public static async Task DesbloquearSinalPorDomain(IntegracaoRadius integracao, ISession session, ISession sessionConnect, LoginRadiusModel login, bool UsaRadiusSemContainer)
        {
            await integracao.DesbloquearSinalPorDomain(session, sessionConnect, login, UsaRadiusSemContainer);
        }

        public static bool TesteLogin(IntegracaoRadius integracao, String IpConcentradora, String Usuario, String Senha, bool UsaRadiusSemContainer)
        {
            return integracao.TesteLogin(IpConcentradora, Usuario, Senha, UsaRadiusSemContainer);
        }

        public static async Task<Sinapse.Model.enumerados.StatusLoginRadius> VerificaStatusLogin(IntegracaoRadius integracao, string login)
        {
            return await integracao.VerificaStatusLogin(login);
        }
        public static async Task<string> GetIPLoginSessao(IntegracaoRadius integracao, string login)
        {
            return await integracao.GetIPLoginSessao(login);
        }

        public static Task<bool> DesconectarLogin(IntegracaoRadius integracao, LoginRadiusModel login, bool UsaRadiusSemContainer)
        {
           return integracao.DesconectarLogin(login, UsaRadiusSemContainer);
        }
        #endregion

        #region PoolIp
        public static void CadastrarPoolIp(IntegracaoRadius integracao, ISession session, String PoolName, IPAddressRange RangeIP)
        {          
            integracao.CadastrarPoolIp(session, PoolName, RangeIP);
            
        }

        public static void EditarPoolIp(IntegracaoRadius integracao, ISession session, String PoolName, IPAddressRange Novo_RangeIP, String Novo_PoolName)
        {
            integracao.EditarPoolIp(session, PoolName, Novo_RangeIP, Novo_PoolName);
        }

        public static void DeletarPoolIp(IntegracaoRadius integracao, ISession session, String PoolName)
        {
            integracao.DeletarPoolIp(session, PoolName);
        }

        public static void AtualizarPoolIpUsuario(IntegracaoRadius integracao, ISession session, long Id_Cliente, String Novo_PoolName, String userName)
        {
            integracao.AtualizarPoolIpUsuario(session, Id_Cliente, Novo_PoolName, userName);
        }
        
        public static async Task AtualizarPoolIpUsuario(IntegracaoRadius integracao, ISession session, LoginRadiusModel login)
        {
            await integracao.AtualizarPoolIpUsuario(session, login);
        }

        public static string GetPoolBloqueioDisponivel(IntegracaoRadius integracao, ISession session, List<String> Pools_Bloqueio)
        {
            return integracao.GetPoolBloqueioDisponivel(session, Pools_Bloqueio).Result;
        }

        public static string GetPoolReducaoVelocidadeDisponivel(IntegracaoRadius integracao, ISession session, List<String> Pools_Bloqueio)
        {
            return integracao.GetPoolReducaoVelocidadeDisponivel(session, Pools_Bloqueio).Result;
        }
        #endregion

        #region Plano

        public static void ConfigurarPlanoCliente(IntegracaoConcentradora integracao, ISession session, LoginRadiusModel plano) 
        {
            integracao.ConfiguracaoPlanoCliente(session, plano);
        }

        public static void ReduzirVelocidade(IntegracaoConcentradora integracao, ISession session, ISession sessionConnect, LoginRadiusModel plano)
        {
            integracao.ReduzirVelocidade(session, sessionConnect, plano);
        }

        public static void NormalizarVelocidade(IntegracaoConcentradora integracao, ISession session, ISession sessionConnect, LoginRadiusModel plano)
        {
            integracao.NormalizarVelocidade(session, sessionConnect, plano);
        }

        public static void DesbloqueioConfianca(IntegracaoConcentradora integracao, ISession session, LoginRadiusModel planoProduto, String usuario)
        {
            integracao.DesbloqueioConfianca(session, planoProduto, usuario);
        }

        public static void AssociaUsuarioAoPlano(IntegracaoRadius integracao, ISession session, String login, String NomePlano, String prioridadePlano, Int64 idClientePlanoProduto)
        {
            integracao.AssociaUsuarioAoPlano(session, login, NomePlano,prioridadePlano, idClientePlanoProduto);
        }

        public static void EditarPlano(IntegracaoConcentradora integracao, ISession session, LoginRadiusModel plano)
        {
            integracao.EditarPlanoCliente(session, plano);
        }

        public static void DeletarPlano(IntegracaoConcentradora integracao, ISession session, Int64 IdClientePlanoProduto) 
        {
            integracao.DeletarPlano(session, IdClientePlanoProduto);
        }

        #endregion

        #region relatorios
        public static async Task<GetHistoricoConexaoRadius_Response> GetHistoricoConexao(IntegracaoRadius integracao, ISession session, GetHistoricoConexaoRadius_DataTableRequest request)
        {
            return await integracao.GetHistoricoConexao(session, request);
        }

        public static async Task<List<ConsumoPeriodoModel>> GetConsumoPeriodo(IntegracaoRadius integracao, ISession session, String Login_Radius, HistoricoConsumoPeriodoEnum Periodo_Consumo, DateTime? Data_Inicio, DateTime? Data_Fim)
        {
            return await integracao.GetConsumoPeriodo(session, Login_Radius, Periodo_Consumo, Data_Inicio, Data_Fim);
        }

        public static async Task<ConsumoPeriodoModel> GetConexaoTempoReal(IntegracaoRadius integracao, ISession session, String Login_Radius, Medida_Velocidade_ConexaoEnum Unidade_Medida_Velocidade, double LastTotalDownloadOctects, double LastTotalUploadOctects)
        {
            return await integracao.GetConexaoTempoReal(session, Login_Radius, Unidade_Medida_Velocidade, LastTotalDownloadOctects, LastTotalUploadOctects);
        }

        public static async Task<List<TempoOnlineOfflineModel>> GetTempoOnline(IntegracaoRadius integracao, ISession session, String Login_Radius, DateTime Data_Inicio, DateTime Data_Fim)
        {
            return await integracao.GetTempoOnline(session, Login_Radius, Data_Inicio, Data_Fim);
        }
        public static async Task<List<TempoOnlineOfflineModel>> GetTempoOffline(IntegracaoRadius integracao, ISession session, String Login_Radius, DateTime Data_Inicio, DateTime Data_Fim)
        {
            return await integracao.GetTempoOffline(session, Login_Radius, Data_Inicio, Data_Fim);
        }       
        #endregion
    }
}