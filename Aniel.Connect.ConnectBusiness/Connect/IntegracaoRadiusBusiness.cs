using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Integracao.Ativacao.Radius;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class IntegracaoRadiusBusiness
    {
        private AnielRadius anielRadius { get; set; }

        public IntegracaoRadiusBusiness(AnielRadius _anielRadius)
        {

            this.anielRadius = _anielRadius;

        }

        #region [EditarUsuario]
        public async Task<bool> EditarUsuario(LoginRadius loginRadius, bool Constantes)
        {
            try
            {

                if (loginRadius.Concentradora.Tipo == null)
                    throw new EAnielException("Marca não informado da concentradora.");

                switch (loginRadius.Concentradora.Tipo)
                {
                    case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                        await anielRadius.EditarUsuario(
                            this.anielRadius.GetSessionRadius(),
                            BindObjToLoginRadiusModel(loginRadius, Constantes)
                        );
                        break;
                    case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                        await anielRadius.EditarUsuario(
                            this.anielRadius.GetSessionRadius(),
                            BindObjToLoginRadiusModel(loginRadius, Constantes)
                        );
                        break;
                    case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                        await anielRadius.EditarUsuario(
                            this.anielRadius.GetSessionRadius(),
                            BindObjToLoginRadiusModel(loginRadius, Constantes)
                        );
                        break;

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region [BloquearUsuario]
        public async Task<bool> BloquearUsuario(LoginRadius loginRadius, bool Constantes, IEnumerable<Pool_Ip> poolsBloqueio)
        {
            try
            {

                if (loginRadius == null)
                    throw new EAnielException("Login não informado para o plano.");
                if (loginRadius?.Concentradora == null)
                    throw new EAnielException("Concentradora não informada para o plano.");
                if (loginRadius?.Concentradora?.Radius == null)
                    throw new EAnielException("Radius não informado para o plano.");

                

                if (loginRadius.Concentradora.Tipo == null)
                    throw new EAnielException("Marca não informado da concentradora.");

                if (loginRadius.Cliente_PlanoProduto.Plano.BloqueioPoolIp) //Bloqueia Usuário Radius por Pool com faixa de IPs sem rede
                {
                    string poolBloqueio = await anielRadius.GetPoolBloqueioDisponivel(this.anielRadius.GetSessionRadius(), poolsBloqueio.Select(p => p.IsPoolConcentradora ? p.Nome_Pool_Concentradora : p.Descricao).ToList());
                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            await anielRadius.BloquearSinalPorPool(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Cliente.Id, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, poolBloqueio, Constantes);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            await anielRadius.BloquearSinalPorPool(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Cliente.Id, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, poolBloqueio, Constantes);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            await anielRadius.BloquearSinalPorPool(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Cliente.Id, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, poolBloqueio, Constantes);
                            break;
                    }
                }
                else if (loginRadius.Cliente_PlanoProduto.Plano.BloqueioDomainName && loginRadius.Concentradora.Tipo == Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei) //Bloqueia usuário por Domain Name (Somente HUAWEI)
                {
                    await anielRadius.BloquearSinalPorDomain(
                        this.anielRadius.GetSessionRadius(),
                        loginRadius.Login,
                        loginRadius.Concentradora.Ip,
                        loginRadius.Concentradora.PortaIncoming,
                        loginRadius.Cliente_PlanoProduto.Plano.IdDomainName,
                        loginRadius.Cliente.Id,
                        Constantes);
                }
                else //Bloqueia Usuário Radius por plano
                {
                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            await anielRadius.BloquearUsuario(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, loginRadius.Concentradora.Secret, Constantes);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            await anielRadius.BloquearUsuario(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, loginRadius.Concentradora.Secret, Constantes);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            await anielRadius.BloquearUsuario(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, loginRadius.Concentradora.Secret, Constantes);
                            break;
                    }
                }

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region [DesbloquearUsuario]
        public async Task<bool> DesbloquearUsuario(LoginRadius loginRadius, bool Constantes)
        {
            try
            {

                if (loginRadius == null)
                    throw new EAnielException("Login não informado para o plano.");
                if (loginRadius?.Concentradora == null)
                    throw new EAnielException("Concentradora não informada para o plano.");
                if (loginRadius?.Concentradora?.Radius == null)
                    throw new EAnielException("Radius não informado para o plano.");

                

                if (loginRadius.Concentradora.Tipo == null)
                    throw new EAnielException("Marca não informado da concentradora.");

                if (loginRadius.Cliente_PlanoProduto.Plano.BloqueioPoolIp)
                {
                    await anielRadius.DesbloquearSinalPorPool(this.anielRadius.GetSessionRadius(), BindObjToLoginRadiusModel(loginRadius, Constantes));
                }
                else if (loginRadius.Cliente_PlanoProduto.Plano.BloqueioDomainName && loginRadius.Concentradora.Tipo == Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei) //Somente para HUAWEI
                {
                    await anielRadius.DesbloquearSinalPorDomain(this.anielRadius.GetSessionRadius(), null, BindObjToLoginRadiusModel(loginRadius, Constantes), Constantes);
                }
                else
                {
                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            await anielRadius.DesbloquearUsuario(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            await anielRadius.DesbloquearUsuario(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            await anielRadius.DesbloquearUsuario(this.anielRadius.GetSessionRadius(), loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);
                            break;
                    }

                }

                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region [Metodos privados]
        private LoginRadiusModel BindObjToLoginRadiusModel(LoginRadius obj, bool constantes)
        {
            LoginRadiusModel retorno = new LoginRadiusModel();

            retorno.Concentradora = obj.Concentradora;
            retorno.DescricaoPlano = obj.Cliente_PlanoProduto.Plano.Descricao;
            retorno.EnderecoIPV4 = obj.EnderecoIPV4;
            retorno.EnderecoIPV6 = obj.EnderecoIPV6;
            retorno.IdCliente = obj.Cliente.Id;
            retorno.IdPlano = obj.Cliente_PlanoProduto.Plano.Id;
            retorno.IdQoSProfile = obj.Cliente_PlanoProduto.Plano.IdQoSProfile;
            retorno.IdQoSProfileReduzido = obj.Cliente_PlanoProduto.Plano.IdQoSProfileReduzido;
            retorno.Login = obj.Login;
            retorno.Senha = obj.SenhaPPPOE;
            retorno.UtilizaQoSProfile = obj.Cliente_PlanoProduto.Plano.UtilizaQoSProfile;
            retorno.VelocidadeDownloadPlano = obj.Cliente_PlanoProduto.Plano.VelocidadeDownload;
            retorno.VelocidadeUploadPlano = obj.Cliente_PlanoProduto.Plano.VelocidadeUpload;
            retorno.VelocidadeDownloadReduzidaPlano = obj.Cliente_PlanoProduto.Plano.VelocidadeDownloadReduzida;
            retorno.VelocidadeUploadReduzidaPlano = obj.Cliente_PlanoProduto.Plano.VelocidadeUploadReduzida;
            retorno.Senha_WIFI = obj.Senha_WIFI;
            retorno.IdPolicyNameDownload = obj.Cliente_PlanoProduto.Plano.IdPolicyNameDownload;
            retorno.IdPolicyNameDownloadReduzido = obj.Cliente_PlanoProduto.Plano.IdPolicyNameDownloadReduzido;
            retorno.IdPolicyNameUpload = obj.Cliente_PlanoProduto.Plano.IdPolicyNameUpload;
            retorno.IdPolicyNameUploadReduzido = obj.Cliente_PlanoProduto.Plano.IdPolicyNameUploadReduzido;
            retorno.SituacaoPlanoAtivo = obj.Cliente_PlanoProduto.Plano.SituacaoPlano == Model.enumerado.Situacao_plano.Ativo;
            retorno.usaRadiusSemContainer = constantes;
            retorno.MAC = obj.MAC;
            if (obj?.Cliente_PlanoProduto?.Pool_Ip != null)
                retorno.Pool = BindObjToPool_IpModel(obj?.Cliente_PlanoProduto?.Pool_Ip);
            retorno.Id_Cliente_Plano_Produto = obj.Cliente_PlanoProduto.Id;

            return retorno;
        }

        private Pool_IpModel BindObjToPool_IpModel(Pool_Ip obj)
        {
            Pool_IpModel retorno = new Pool_IpModel();

            retorno.Descricao = obj.Descricao;
            retorno.IsDistribuicaoBlocoIp = obj.IsDistribuicaoBlocoIp;
            retorno.IsPoolBloqueio = obj.IsPoolBloqueio;
            retorno.IsPoolConcentradora = obj.IsPoolConcentradora;
            retorno.IsPoolIpFixo = obj.IsPoolIpFixo;
            retorno.IsPoolReducaoVelocidade = obj.IsPoolReducaoVelocidade;
            retorno.NomePoolConcentradora = obj.Nome_Pool_Concentradora;

            return retorno;
        }
        #endregion
    }
}
