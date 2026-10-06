using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.API_Aniel.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System.Text;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate.Util;
using Sinapse.Core;
using System.Web;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class PlanoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Planos/Produtos
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PlanoApi/GetAll")]
        public async Task<ApiResponseObj<List<PlanoIndexModel>>> GetAll(PlanoApiRequest request)
        {
            List<PlanoIndexModel> retorno = new List<PlanoIndexModel>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                {
                    var com = session.Connection.CreateCommand();
                    {
                        using (var repoONU = new Conexao<ONU>(session))
                        using (var repo = new Conexao<Cliente_PlanoProduto>(session))
                        using (var repoCliente = new Conexao<Cliente>(session))
                        using (var repoPlano = new Conexao<Plano>(session))
                        using (var repoLog = new Conexao<LoginRadius>(session))
                        {
                            Cliente cliente = await repoCliente.selectFirst(p => p.CpfCnpj == request.Cpf_Cnpj);
                            Plano plano = await repoPlano.selectFirst(p => p.Codigo == request.Cod_Plano);
                            Cliente_PlanoProduto cpp = await repo.selectFirst(p => p.Cliente == cliente && p.Plano == plano);

                            ONU onu = await repoONU.selectFirst(p => p.Cliente_PlanoProduto == cpp);
                            if (String.IsNullOrEmpty(onu.Id.ToString()))
                            {
                                throw new Exception("ONU não localizado");
                            }
                            var purl = CONSTANTES.URLConnect;
                            var StatusOnu = GetByUrl<List<StatusONUModel>>(purl, $"api/IntegracaoAtivacaoApiController/GetStatusONUsByPON?IdOnu={onu.Id}");



                            var obj = new PlanoIndexModel
                            {
                                Plano = plano.Descricao,
                                Situacao = HtmlEnumExtensions.GetEnumDescription(cpp.Status)
                            };

                            if (StatusOnu.Objeto.Count() > 0)
                            {

                                var stat = StatusOnu.Objeto.Select(p => p.Status).First();
                                obj.Status = stat == StatusONU.Online ? StatusONU.Online : StatusONU.Offline;
                            }
                            else
                            {
                            }

                            retorno.Add(obj);



                        }

                        return new ApiResponseObj<List<PlanoIndexModel>> { data = retorno, ok = true };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<PlanoIndexModel>> { error = ex.Message };
            }

        }
        #region desbloqueio
        [HttpPost]
        [Route("api/PlanoApi/GetDesbloqueio")]
        public async Task<ApiResponseObj<List<DesbloqueioPlano>>> GetDesbloqueio(PlanoApiRequest request)
        {
            List<DesbloqueioPlano> retorno = new List<DesbloqueioPlano>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                {
                    var com = session.Connection.CreateCommand();
                    {
                        using (var repo = new Conexao<Cliente_PlanoProduto>(session))
                        using (var repoCliente = new Conexao<Cliente>(session))
                        using (var repoPlano = new Conexao<Plano>(session))
                        using (var repoLogin = new Conexao<LoginRadius>(session))
                        {
                            #region Objetos
                            Cliente cliente = await repoCliente.selectFirst(p => p.CpfCnpj == request.Cpf_Cnpj);
                            Plano plano = await repoPlano.selectFirst(p => p.Codigo == request.Cod_Plano);
                            Cliente_PlanoProduto cpp = await repo.selectFirst(p => p.Cliente == cliente && p.Plano == plano);
                            LoginRadius radio = await repoLogin.selectFirst(p => p.Cliente_PlanoProduto == cpp);

                            PlanoProdutoRadiusRequest planprod = new PlanoProdutoRadiusRequest();
                            planprod.IdClientePlanoProduto = cpp.Id;
                            planprod.Usuario = radio.Login; 
                            #endregion

                            if (String.IsNullOrEmpty(cliente.CpfCnpj))
                            {
                                throw new Exception("Cliente Não Localizado");
                            }
                            
                            var purl = CONSTANTES.URLConnect;

                            var desbloqueio = Post(purl, $"api/IntegracaoRadiusApiController/DesbloqueioConfianca?request={planprod}", planprod);


                            if (desbloqueio.InnerException.Length == 0 )
                                retorno.Add(new DesbloqueioPlano
                                {
                                    Plano = plano.Descricao,
                                    Situacao = HtmlEnumExtensions.GetEnumDescription(Status_PlanoProduto.Habilitado),
                                    Justificativa = "Sucesso"
                                });
                            else 
                            {
                                retorno.Add(new DesbloqueioPlano
                                {
                                    Plano = plano.Descricao,
                                    Situacao = HtmlEnumExtensions.GetEnumDescription(cpp.Status),
                                    Justificativa = "Tivemos um erro"
                                });
                            }

                        }

                        return new ApiResponseObj<List<DesbloqueioPlano>> { data = retorno, ok = true };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<DesbloqueioPlano>> { error = ex.Message };
            }

            #endregion
        }
    }
}

