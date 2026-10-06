//using Aniel.Connect.ControllersWebApi.Ativacao.Concentradoras;
using Aniel.Connect.ControllersWebApi.Ativacao.Repositorio;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
//using Aniel.Connect.Models.Ativacao;
using NHibernate;
using Sinapse.Core;
using Sinapse.DAO.repository;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
//using Aniel.Connect.ControllersWebApi.Ativação.Concentradoras;
using NetTools;
using System.Net;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Model.enumerados;
using Sinapse.Model;
using Sinapse.Integracao.Ativacao.Concentradoras;
using System.Management.Automation.Language;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class IntegracaoRadiusApiController : BaseApiController
    {
        #region Concentradora

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/CadastrarConcentradora")]
        public Response CadastrarConcentradora(Concentradora concentradora)
        {
            var response = new Response();

            var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(concentradora.Radius);

            try
            {
                using (ISession session = HibernateUtil.GetSessionRadius())
                {
                    if (String.IsNullOrEmpty(concentradora.Ip))
                        throw new EAnielException("Endereço IP não informado para concentradora.");
                    if (concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");
                    if (concentradora.Senha == null)
                        throw new EAnielException("Secret não informado da concentradora.");

                    switch (concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.CadastrarConcentradora(anielRadius, session, concentradora);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.CadastrarConcentradora(anielRadius, session, concentradora);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.CadastrarConcentradora(anielRadius, session, concentradora);
                            break;
                    }

                }
                response.result = true;
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/EditarConcentradora")]
        public Response EditarConcentradora(Concentradora concentradora)
        {
            var response = new Response();
            var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(concentradora.Radius);

            try
            {
                using (ISession session = HibernateUtil.GetSessionRadius())
                {
                    if (String.IsNullOrEmpty(concentradora.Ip))
                        throw new EAnielException("Endereço IP não informado para concentradora.");
                    if (concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");
                    if (concentradora.Secret == null)
                        throw new EAnielException("Secret não informado da concentradora.");

                    switch (concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.EditarConcentradora(anielRadius, session, concentradora);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.EditarConcentradora(anielRadius, session, concentradora);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.EditarConcentradora(anielRadius, session, concentradora);
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/DeletarConcentradora")]
        public Response DeletarConcentradora(Concentradora concentradora)
        {
            var response = new Response();
            var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(concentradora.Radius);

            try
            {
                using (ISession session = HibernateUtil.GetSessionRadius())
                {
                    if (concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    switch (concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.DeletarConcentradora(anielRadius, session, concentradora.Id);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.DeletarConcentradora(anielRadius, session, concentradora.Id);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.DeletarConcentradora(anielRadius, session, concentradora.Id);
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        #endregion

        #region Usuário

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/CadastrarUsuario")]
        public async Task<Response> CadastrarUsuario(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {

                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    if (loginRadius.Concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.CadastrarUsuario(
                                anielRadius,
                                sessionRadius,
                                BindObjToLoginRadiusModel(loginRadius)
                            );
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.CadastrarUsuario(
                                anielRadius,
                                sessionRadius,
                                BindObjToLoginRadiusModel(loginRadius)
                            );
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.CadastrarUsuario(
                                anielRadius,
                                sessionRadius,
                                BindObjToLoginRadiusModel(loginRadius)
                            );
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/CadastrarUsuarioSimplificado")]
        public async Task<Response> CadastrarUsuarioSimplificado(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.CadastrarUsuario(
                                anielRadius,
                                sessionRadius,
                                loginRadius.Login,
                                loginRadius.SenhaPPPOE,
                                loginRadius.Cliente.Id
                            );
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.CadastrarUsuario(
                                anielRadius,
                                sessionRadius,
                                loginRadius.Login,
                                loginRadius.SenhaPPPOE,
                                loginRadius.Cliente.Id
                            );
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.CadastrarUsuario(
                                anielRadius,
                                sessionRadius,
                                loginRadius.Login,
                                loginRadius.SenhaPPPOE,
                                loginRadius.Cliente.Id
                            );
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }        

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/EditarUsuario")]
        public async Task<Response> EditarUsuario(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    if (loginRadius.Concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    var modelLogin = BindObjToLoginRadiusModel(loginRadius);

                    if(!String.IsNullOrEmpty(request.LoginAntigo))
                    {
                        modelLogin.LoginAntigo = request.LoginAntigo;
                    }

                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.EditarUsuario(
                                anielRadius,
                                sessionRadius,
                                modelLogin
                            );
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.EditarUsuario(
                                anielRadius,
                                sessionRadius,
                                modelLogin
                            );
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.EditarUsuario(
                                anielRadius,
                                sessionRadius,
                                modelLogin
                            );
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/EditarUsuarioSimplificado")]
        public async Task<Response> EditarUsuarioSimplificado(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.EditarUsuario(
                                anielRadius,
                                sessionRadius,
                                loginRadius.Login,
                                loginRadius.SenhaPPPOE,
                                loginRadius.Cliente.Id,
                                loginRadius.Cliente_PlanoProduto.Id
                            );
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.EditarUsuario(
                                anielRadius,
                                sessionRadius,
                                loginRadius.Login,
                                loginRadius.SenhaPPPOE,
                                loginRadius.Cliente.Id,
                                loginRadius.Cliente_PlanoProduto.Id
                            );
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.EditarUsuario(
                                anielRadius,
                                sessionRadius,
                                loginRadius.Login,
                                loginRadius.SenhaPPPOE,
                                loginRadius.Cliente.Id,
                                loginRadius.Cliente_PlanoProduto.Id
                            );
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }


        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/DeletarUsuario")]
        public async Task<Response> DeletarUsuario(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    if (loginRadius.Concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");


                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.DeletarUsuario(anielRadius, sessionRadius, loginRadius.Cliente.Id);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.DeletarUsuario(anielRadius, sessionRadius, loginRadius.Cliente.Id);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.DeletarUsuario(anielRadius, sessionRadius, loginRadius.Cliente.Id);
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }


        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/BloquearUsuario")]
        public async Task<Response> BloquearUsuario(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                using (var repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);
                    if (loginRadius == null)
                        throw new EAnielException("Login não informado para o plano.");
                    if (loginRadius?.Concentradora == null)
                        throw new EAnielException("Concentradora não informada para o plano.");
                    if (loginRadius?.Concentradora?.Radius == null)
                        throw new EAnielException("Radius não informado para o plano.");


                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    if (loginRadius.Concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    if (loginRadius.Cliente_PlanoProduto.Plano.BloqueioPoolIp) //Bloqueia Usuário Radius por Pool com faixa de IPs sem rede
                    {
                        var poolsBloqueio = await repoPoolIp.select(p => p.IsPoolBloqueio == true && p.Concentradora.Id == loginRadius.Concentradora.Id);

                        string poolBloqueio = RadiusRepository.GetPoolBloqueioDisponivel(anielRadius, sessionRadius, poolsBloqueio.Select(p => p.IsPoolConcentradora ? p.Nome_Pool_Concentradora : p.Descricao).ToList());

                        switch (loginRadius.Concentradora.Tipo)
                        {
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                                RadiusRepository.BloquearSinalPorPool(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente.Id, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, poolBloqueio, CONSTANTES.UsaRadiusSemContainer);
                                break;
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                                RadiusRepository.BloquearSinalPorPool(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente.Id, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, poolBloqueio, CONSTANTES.UsaRadiusSemContainer);
                                break;
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                                RadiusRepository.BloquearSinalPorPool(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente.Id, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, poolBloqueio, CONSTANTES.UsaRadiusSemContainer);
                                break;
                        }
                    }
                    else if (loginRadius.Cliente_PlanoProduto.Plano.BloqueioDomainName && loginRadius.Concentradora.Tipo == Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei) //Bloqueia usuário por Domain Name (Somente HUAWEI)
                    {                      
                        RadiusRepository.BloquearSinalPorDomain(anielRadius,
                            sessionRadius,
                            loginRadius.Login,
                            loginRadius.Concentradora.Ip,
                            loginRadius.Concentradora.PortaIncoming,
                            loginRadius.Cliente_PlanoProduto.Plano.IdDomainName,
                            loginRadius.Cliente.Id,
                            CONSTANTES.UsaRadiusSemContainer);                       
                    }
                    else //Bloqueia Usuário Radius por plano
                    {
                        switch (loginRadius.Concentradora.Tipo)
                        {
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                                RadiusRepository.BloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.SenhaPPPOE, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, loginRadius.Concentradora.Secret, CONSTANTES.UsaRadiusSemContainer);
                                break;
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                                RadiusRepository.BloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.SenhaPPPOE, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, loginRadius.Concentradora.Secret, CONSTANTES.UsaRadiusSemContainer);
                                break;
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                                RadiusRepository.BloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.SenhaPPPOE, loginRadius.Concentradora.Ip, loginRadius.Concentradora.PortaIncoming, loginRadius.Concentradora.Secret, CONSTANTES.UsaRadiusSemContainer);
                                break;
                        }

                        /*if (loginRadius.Cliente != null && loginRadius.Cliente_PlanoProduto != null && loginRadius?.Cliente_PlanoProduto?.Plano?.Id != null)
                            using (var webHook = new WebHookApiController())
                            {
                                await webHook.ExecutaBloqueioPlano(User.Identity.GetUserId(), loginRadius.Cliente.Id, loginRadius.Cliente_PlanoProduto.Plano.Id, DateTime.Now);
                            }*/
                    }

                    loginRadius.Status = Status_Default.Inativo;
                    loginRadius.DataHoraAlteracao = DateTime.Now;
                    
                    await repoLoginRadius.SaveOrUpdate(loginRadius);

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/DesbloquearUsuario")]
        public async Task<Response> DesbloquearUsuario(PlanoProdutoRadiusRequest request, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null, string usuario = null, string detalhe = null)
        {
            var response = new Response();
           

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);


                    if (loginRadius == null)
                        throw new EAnielException("Login não informado para o plano.");
                    if (loginRadius?.Concentradora == null)
                        throw new EAnielException("Concentradora não informada para o plano.");
                    if (loginRadius?.Concentradora?.Radius == null)
                        throw new EAnielException("Radius não informado para o plano.");

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    if (loginRadius.Concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    if (loginRadius.Cliente_PlanoProduto.Plano.BloqueioPoolIp)
                    {                                               
                        await RadiusRepository.DesbloquearSinalPorPool(anielRadius, sessionRadius, BindObjToLoginRadiusModel(loginRadius));                                                       
                    }
                    else if (loginRadius.Cliente_PlanoProduto.Plano.BloqueioDomainName && loginRadius.Concentradora.Tipo == Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei) //Somente para HUAWEI
                    {                       
                       await RadiusRepository.DesbloquearSinalPorDomain(anielRadius, sessionRadius, sessionConnect, BindObjToLoginRadiusModel(loginRadius), CONSTANTES.UsaRadiusSemContainer);                     
                    }
                    else
                    {
                        switch (loginRadius.Concentradora.Tipo)
                        {
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                                await RadiusRepository.DesbloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);
                                break;
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                                await RadiusRepository.DesbloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);
                                break;
                            case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                                await RadiusRepository.DesbloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);
                                break;
                        }

                        /*if (loginRadius.Cliente != null && loginRadius.Cliente_PlanoProduto != null && loginRadius?.Cliente_PlanoProduto?.Plano?.Id != null)
                            using (var webHook = new WebHookApiController())
                            {
                                await webHook.ExecutaDesbloqueioPlano(User.Identity.GetUserId(), loginRadius.Cliente.Id, loginRadius.Cliente_PlanoProduto.Plano.Id, DateTime.Now);
                            } 
                        */
                    }

                    loginRadius.Status = Status_Default.Ativo;
                    loginRadius.DataHoraAlteracao = DateTime.Now;

                    await repoLoginRadius.SaveOrUpdate(loginRadius);

                    response.result = true;

                    if(enviarLog != null && response.result == true)
                    {
                        var idLog = await enviarLog($"Radius [DesbloquearUsuario] -> Plano Produto: {request.IdClientePlanoProduto}, Login: {loginRadius.Login}", usuario, detalhe, null);
                        await enviarLog("Login ativado com sucesso!", usuario, detalhe, idLog);
                    }
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
                if (enviarLog != null)
                {
                    var idLog = enviarLog($"Radius [DesbloquearUsuario] -> Plano Produto: {request.IdClientePlanoProduto}", usuario, detalhe, null);
                    await enviarLog(response.message, usuario, detalhe, idLog.Result);
                }
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/TesteLogin")]
        public async Task<Response<bool>> TesteLogin(PlanoProdutoRadiusRequest request)
        {
            var response = new Response<bool>();

            try
            {
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    if (loginRadius == null || String.IsNullOrEmpty(loginRadius?.Login))
                        throw new EAnielException("Login não informado para o plano.");
                    if (loginRadius?.Concentradora == null)
                        throw new EAnielException("Concentradora não informada para o plano.");
                    if (loginRadius?.Concentradora?.Radius == null)
                        throw new EAnielException("Radius não informado para o plano.");
                    if(loginRadius.Status == Status_Default.Inativo)
                        throw new EAnielException("Não é possível realizar um teste de conexão pois o Login está inativo.");

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);                    

                    response.result = RadiusRepository.TesteLogin(anielRadius, loginRadius.Concentradora.Ip, loginRadius.Login, loginRadius.SenhaPPPOE, CONSTANTES.UsaRadiusSemContainer);
                }
            }
            catch (Exception ex)
            {
                response.result = false;

                response.exception = ex.ToString();
                response.message = ex.Message;
            }

            return response;
        }


        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/DesconectarLogin")]
        public async Task<Response> DesconectarLogin(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    if (loginRadius == null || String.IsNullOrEmpty(loginRadius?.Login))
                        throw new EAnielException("Login não informado para o plano.");
                    if (loginRadius?.Concentradora == null)
                        throw new EAnielException("Concentradora não informada para o plano.");
                    if (loginRadius?.Concentradora?.Radius == null)
                        throw new EAnielException("Radius não informado para o plano.");

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    
                    
                    bool retornoRadius = await RadiusRepository.DesconectarLogin(anielRadius, BindObjToLoginRadiusModel(loginRadius), CONSTANTES.UsaRadiusSemContainer);

                    if (!retornoRadius)
                        throw new Exception($"Não foi possível desconectar o Login: {loginRadius.Login}.");

                    response.result = true;
                }                
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error($"[AnielRadius]", ex);
                response.result = false;

                response.exception = ex.ToString();
                response.message = ex.Message;
            }

            return response;
        }

        #endregion

        #region Plano

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/AssociaUsuarioAoPlano")]
        public async Task<Response> AssociaUsuarioAoPlano(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                using (var repoConcentradora = new Conexao<Concentradora>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);
                    var nomePlano = loginRadius.Cliente_PlanoProduto.Plano != null ? loginRadius.Cliente_PlanoProduto.Plano.Descricao : loginRadius.Cliente_PlanoProduto.Produto.Descricao;

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    if (loginRadius.Concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.AssociaUsuarioAoPlano(anielRadius, sessionRadius, loginRadius.Login, nomePlano, "1", request.IdClientePlanoProduto);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.AssociaUsuarioAoPlano(anielRadius, sessionRadius, loginRadius.Login, nomePlano, "1", request.IdClientePlanoProduto);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.AssociaUsuarioAoPlano(anielRadius, sessionRadius, loginRadius.Login, nomePlano, "1", request.IdClientePlanoProduto);
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/ConfigurarPlanoUsuario")]
        public async Task<Response> ConfigurarPlanoUsuario(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {

                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                    if (loginRadius.Concentradora.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");



                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.ConfigurarPlanoCliente(new Mikrotik(), sessionRadius, BindObjToLoginRadiusModel(loginRadius));
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.ConfigurarPlanoCliente(new Huawei(), sessionRadius, BindObjToLoginRadiusModel(loginRadius));
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.ConfigurarPlanoCliente(new Juniper(), sessionRadius, BindObjToLoginRadiusModel(loginRadius));
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/EditarPlano")]
        public async Task<Response> EditarPlano(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLogin = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var login = await repoLogin.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    if (login.Concentradora?.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    switch (login.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.EditarPlano(new Mikrotik(), sessionRadius, BindObjToLoginRadiusModel(login));
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.EditarPlano(new Huawei(), sessionRadius, BindObjToLoginRadiusModel(login));
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.EditarPlano(new Juniper(), sessionRadius, BindObjToLoginRadiusModel(login));
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/DeletarPlano")]
        public async Task<Response> DeletarPlano(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                using (var repoConcentradora = new Conexao<Concentradora>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    if (loginRadius.Concentradora?.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.DeletarPlano(new Mikrotik(), sessionRadius, loginRadius.Cliente_PlanoProduto.Id);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.DeletarPlano(new Huawei(), sessionRadius, loginRadius.Cliente_PlanoProduto.Id);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.DeletarPlano(new Juniper(), sessionRadius, loginRadius.Cliente_PlanoProduto.Id);
                            break;
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/ReduzirVelocidadeEmMassa")]
        public async Task<Response> ReduzirVelocidadeEmMassa(ClientesRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var taskList = new List<Task>();

                    foreach (long id in request.ListIdCliente)
                    {
                        var listPlanoProduto = (await repoPlanoProduto.select(p => p.Cliente.Id == id)).ToList().Select(p => p.Id);
                        foreach (var idPlanoProduto in listPlanoProduto)
                        {
                            var taskReduzir = Task.Run(() => ReduzirVelocidade(new PlanoProdutoRadiusRequest
                            {
                                IdClientePlanoProduto = idPlanoProduto
                            }));

                            taskList.Add(taskReduzir);
                        }
                    }

                    Task.WaitAll(taskList.ToArray());

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/ReduzirVelocidade")]
        public async Task<Response> ReduzirVelocidade(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                using (var repoConcentradora = new Conexao<Concentradora>(sessionConnect))
                using (var repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(sessionConnect))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    if (loginRadius == null)
                        throw new EAnielException("Login não informado para o plano.");

                    if (loginRadius.Concentradora?.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.ReduzirVelocidade(new Mikrotik(), sessionRadius, sessionConnect, BindObjToLoginRadiusModel(loginRadius));
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.ReduzirVelocidade(new Huawei(), sessionRadius, sessionConnect, BindObjToLoginRadiusModel(loginRadius));
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.ReduzirVelocidade(new Juniper(), sessionRadius, sessionConnect, BindObjToLoginRadiusModel(loginRadius));
                            break;
                    }

                    var planoProduto = loginRadius.Cliente_PlanoProduto;
                    var userName = loginRadius.Login;
                    planoProduto.Velocidade_Reduzida = true;
                    await repoClientePlanoProduto.Update(planoProduto);

                    if (loginRadius.Cliente_PlanoProduto.Plano.UtilizaPoolReducao)
                    {
                        var poolDisponiveis = await repoPoolIp.select(p => p.IsPoolReducaoVelocidade);
                        var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                        var poolReducao = RadiusRepository.GetPoolReducaoVelocidadeDisponivel(anielRadius, sessionRadius, poolDisponiveis.Select(p => p.IsPoolConcentradora ? p.Nome_Pool_Concentradora : p.Descricao).ToList());

                        RadiusRepository.AtualizarPoolIpUsuario(anielRadius, sessionRadius, loginRadius.Cliente.Id, poolReducao, userName); //Define uma pool disponível para redução de velocidade
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/NormalizarVelocidade")]
        public async Task<Response> NormalizarVelocidade(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                using (var repoConcentradora = new Conexao<Concentradora>(sessionConnect))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    if (loginRadius == null)
                        throw new EAnielException("Login não informado para o plano.");

                    if (loginRadius.Concentradora?.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.NormalizarVelocidade(new Mikrotik(), sessionRadius, sessionConnect, BindObjToLoginRadiusModel(loginRadius));
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.NormalizarVelocidade(new Huawei(), sessionRadius, sessionConnect, BindObjToLoginRadiusModel(loginRadius));
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.NormalizarVelocidade(new Juniper(), sessionRadius, sessionConnect, BindObjToLoginRadiusModel(loginRadius));
                            break;
                    }

                    var planoProduto = loginRadius.Cliente_PlanoProduto;
                    planoProduto.Velocidade_Reduzida = false;
                    var userName = loginRadius.Login;
                    await repoClientePlanoProduto.Update(planoProduto);

                    if (loginRadius.Cliente_PlanoProduto.Plano.UtilizaPoolReducao)
                    {
                        var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);
                        RadiusRepository.AtualizarPoolIpUsuario(anielRadius, sessionRadius, loginRadius.Cliente.Id, loginRadius.Cliente_PlanoProduto.Pool_Ip.Descricao, userName);  //Atualiza com a Pool original do Plano
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/DesbloqueioConfianca")]
        [Obsolete("Este endpoint desbloqueia apenas um único plano porém o processo deve ser realizado em cima do contrato inteiro. Se basear no método DesbloqueioConfiancaOla() do ClienteContratoApiController")]
        public async Task<Response> DesbloqueioConfianca(PlanoProdutoRadiusRequest request)
        {
            var response = new Response();

            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                using (var repoConcentradora = new Conexao<Concentradora>(sessionConnect))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    if (loginRadius == null)
                        throw new EAnielException("Login não informado para o plano.");

                    if (loginRadius.Concentradora?.Tipo == null)
                        throw new EAnielException("Marca não informado da concentradora.");

                    List<AreaModuloModel> areas = new List<AreaModuloModel>();

                    using (var acessoApi = new AcessoApiController())
                    {
                        areas = (await acessoApi.GetArea_ModuloUsuario(request.Usuario)).Objeto;
                    }

                    if (areas.Where(p => p.Cod_Area == "183").Count() == 0)
                    {
                        if (loginRadius.Cliente_PlanoProduto.Data_Desbloqueio_Confianca.HasValue)
                        {
                            var periodicidadeDesbloqueio = loginRadius.Cliente_PlanoProduto.Plano?.QtdDiasdesbloqueados; // TODO: Ler o novo campo a ser cadastrado no plano
                            var qtdDiasDesbloqueio = DateTime.Now.Date - loginRadius.Cliente_PlanoProduto.Data_Desbloqueio_Confianca.Value;

                            if (qtdDiasDesbloqueio.TotalDays <= periodicidadeDesbloqueio)
                                throw new EAnielException("O cliente efetuou um desbloqueio em confiança recentemente.");
                        }
                    }

                    //BLOCO COMENTADO POIS ESTE MÉTODO DO RADIUS FAZ UM PROCESSO INCORRETO
                    /*switch (loginRadius.Concentradora.Tipo)
                    {
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                            RadiusRepository.DesbloqueioConfianca(new Mikrotik(), sessionRadius, BindObjToLoginRadiusModel(loginRadius), request.Usuario);                            
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                            RadiusRepository.DesbloqueioConfianca(new Huawei(), sessionRadius, BindObjToLoginRadiusModel(loginRadius), request.Usuario);
                            break;
                        case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                            RadiusRepository.DesbloqueioConfianca(new Juniper(), sessionRadius, BindObjToLoginRadiusModel(loginRadius), request.Usuario);
                            break;
                    }*/

                    if(loginRadius.Cliente_PlanoProduto?.Plano?.AutenticacaoRadius == true)
                    {
                        await new IntegracaoRadiusApiController().DesbloquearUsuario(new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest
                        {
                            IdClientePlanoProduto = loginRadius.Cliente_PlanoProduto.Id
                        });
                    }                    

                    loginRadius.Cliente_PlanoProduto.Status = Sinapse.Model.enumerados.Status_PlanoProduto.Habilitado;
                    loginRadius.Cliente_PlanoProduto.Data_Desbloqueio_Confianca = DateTime.Now.Date;
                    await repoClientePlanoProduto.SaveOrUpdate(loginRadius.Cliente_PlanoProduto);
                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }

        #endregion

        #region Pool Ip

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/CadastrarPoolIp")]
        public Response CadastrarPoolIp(Model.nhibernate.Pool_Ip value)
        {
            Response retorno = new Response();

            try
            {

                using (ISession session = HibernateUtil.GetSessionRadius())
                {
                    if (String.IsNullOrEmpty(value.Ip_Inicio_Range))
                        throw new EAnielException("Endereço IP inicial do range não informado para o Pool.");
                    if (String.IsNullOrEmpty(value.Ip_Fim_Range) && String.IsNullOrEmpty(value.Mascara_CIDR))
                        throw new EAnielException("Endereço IP final do range ou Máscara do IP não informado para o Pool.");
                    if (value.Radius == null)
                        throw new EAnielException("Servidor Radius não informado.");
                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(value.Radius);

                    IPAddressRange range;
                    if (!String.IsNullOrEmpty(value.Mascara_CIDR)) //Cálculo do range por máscara
                    {
                        var network = IPNetwork.Parse(String.Format("{0}/{1}", value.Ip_Inicio_Range, value.Mascara_CIDR));
                        range = new IPAddressRange(network.FirstUsable, network.LastUsable);
                    }
                    else //Cálculo do range por Ip Inicial e Ip Final do Cadastro de Pool
                    {
                        range = new IPAddressRange(IPAddress.Parse(value.Ip_Inicio_Range), IPAddress.Parse(value.Ip_Fim_Range));
                    }

                    RadiusRepository.CadastrarPoolIp(anielRadius, session, value.Descricao, range);
                }


                retorno.result = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.result = false;
                retorno.message = ex.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/EditarPoolIp")]
        public Response EditarPoolIp(Model.nhibernate.Pool_Ip value, String Novo_PoolName)
        {
            Response retorno = new Response();

            try
            {
                using (ISession session = HibernateUtil.GetSessionRadius())
                {
                    if (String.IsNullOrEmpty(value.Ip_Inicio_Range))
                        throw new EAnielException("Endereço IP inicial do range não informado para o Pool.");
                    if (String.IsNullOrEmpty(value.Ip_Fim_Range) && String.IsNullOrEmpty(value.Mascara_CIDR))
                        throw new EAnielException("Endereço IP final do range ou Máscara do IP não informado para o Pool.");
                    if (value.Radius == null)
                        throw new EAnielException("Servidor Radius não informado.");
                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(value.Radius);

                    IPAddressRange range;
                    if (!String.IsNullOrEmpty(value.Mascara_CIDR)) //Cálculo do range por máscara
                    {
                        var network = IPNetwork.Parse(String.Format("{0}/{1}", value.Ip_Inicio_Range, value.Mascara_CIDR));
                        range = new IPAddressRange(network.FirstUsable, network.LastUsable);
                    }
                    else //Cálculo do range por Ip Inicial e Ip Final do Cadastro de Pool
                    {
                        range = new IPAddressRange(IPAddress.Parse(value.Ip_Inicio_Range), IPAddress.Parse(value.Ip_Fim_Range));
                    }

                    RadiusRepository.EditarPoolIp(anielRadius, session, value.Descricao, range, !String.IsNullOrEmpty(Novo_PoolName) ? Novo_PoolName : String.Empty);

                }

                retorno.result = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.result = false;
                retorno.message = ex.Message;
            }

            return retorno;
        }        
        #endregion

        public LoginRadiusModel BindObjToLoginRadiusModel(Model.nhibernate.LoginRadius obj)
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
            retorno.usaRadiusSemContainer = CONSTANTES.UsaRadiusSemContainer;
            retorno.MAC = obj.MAC;
            if(obj?.Cliente_PlanoProduto?.Pool_Ip != null)
                retorno.Pool = BindObjToPool_IpModel(obj?.Cliente_PlanoProduto?.Pool_Ip);
            retorno.Id_Cliente_Plano_Produto = obj.Cliente_PlanoProduto.Id;

            return retorno;
        }

        private Pool_IpModel BindObjToPool_IpModel(Model.nhibernate.Pool_Ip obj)
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

        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/VerificaStatusLogin")]
        public async Task<Sinapse.Model.enumerados.StatusLoginRadius> VerificaStatusLogin(PlanoProdutoRadiusRequest request)
        {
            Sinapse.Model.enumerados.StatusLoginRadius response;

            try
            {
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    if (loginRadius == null || String.IsNullOrEmpty(loginRadius?.Login))
                        throw new EAnielException("Login não informado para o plano.");
                    if (loginRadius?.Concentradora == null)
                        throw new EAnielException("Concentradora não informada para o plano.");
                    if (loginRadius?.Concentradora?.Radius == null)
                        throw new EAnielException("Radius não informado para o plano.");
                    if (loginRadius.Status == Status_Default.Inativo)
                        throw new EAnielException("Não é possível realizar um teste de conexão pois o Login está inativo.");

                    var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                   response = await RadiusRepository.VerificaStatusLogin(anielRadius, loginRadius.Login);
                }
            }
            catch (Exception ex)
            {
               response = StatusLoginRadius.Erro;
            }

            return response;

        }


        [HttpPost]
        [Route("api/IntegracaoRadiusApiController/GetIPLoginSessao")]
        public async Task<string> GetIPLoginSessao(PlanoProdutoRadiusRequest request)
        {
            string response;

            try
            {
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                using (var repoPoolIP = new Conexao<Model.nhibernate.Pool_Ip>(sessionConnect))
                using (var repoPoolIpEndereco = new Conexao<Model.nhibernate.Pool_Ip_Endereco>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

                    

                    if (loginRadius == null || String.IsNullOrEmpty(loginRadius?.Login))
                        throw new EAnielException("Login não informado para o plano.");
                    if (loginRadius?.Concentradora == null)
                        throw new EAnielException("Concentradora não informada para o plano.");
                    if (loginRadius?.Concentradora?.Radius == null)
                        throw new EAnielException("Radius não informado para o plano.");
                    if (loginRadius.Status == Status_Default.Inativo)
                        throw new EAnielException("Não é possível realizar um teste de conexão pois o Login está inativo.");

                    Model.nhibernate.Pool_Ip verificaPoolIpFixo = null;

                    var getPoolIPEndereco = await repoPoolIpEndereco.selectFirst(p => p.Login == loginRadius);
                    if(getPoolIPEndereco != null )
                    {
                        verificaPoolIpFixo = await repoPoolIP.selectFirst(p => p.Id == getPoolIPEndereco.Pool_Ip.Id);
                    }
                    

                    if(verificaPoolIpFixo != null && verificaPoolIpFixo.IsPoolIpFixo == true)
                    {
                        response = getPoolIPEndereco.Ip;
                    }
                    else
                    {
                        var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                        response = await RadiusRepository.GetIPLoginSessao(anielRadius, loginRadius.Login);
                    }                  
                }
            }
            catch (Exception ex)
            {
                response = "";
            }

            return response;

        }
    }
}