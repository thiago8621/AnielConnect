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
using Sinapse.Model;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ONUApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Planos/Produtos
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ONUApi/GetAll")]
        public async Task<ApiResponseObj<List<OnuList>>> GetAll(OnuRequest request)
        {
            List<OnuList> retorno = new List<OnuList>();
            var equip = new AtivaEquipamento();

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

                    using (var com = session.Connection.CreateCommand())
                    {
                        using (var repo = new Conexao<ONU>(session))
                        using (var repoPon = new Conexao<PON>(session))
                        using (var repoOlt = new Conexao<OLT>(session))
                        using (var repoclienteplano = new Conexao<Cliente_PlanoProduto>(session))
                        using (var repocliente = new Conexao<Cliente>(session))
                        using (var repoPOP = new Conexao<POP>(session))
                        {
                            #region Objetos

                            Cliente cliente = await repocliente.selectFirst(p => p.CpfCnpj == request.Cpf_Cnpj);
                            Cliente_PlanoProduto clipp = await repoclienteplano.selectFirst(p => p.Cliente == cliente);
                            ONU onu = await repo.selectFirst(p => p.Cliente_PlanoProduto == clipp);
                            
                            OLT olt = await repoOlt.selectFirst(p => p.PortaTelnet == onu.PortaTelnet);
                           
                            PON pon = await repoPon.selectFirst(p => p.OLT == olt);
                            #endregion

                            #region assimilação das informações de equip
                            equip.IdOLT = olt.Id;
                            equip.IdONU = onu.Id;
                            equip.IdPON = pon.Id;
                            equip.ScriptOLT = Convert.ToString(onu.ScriptOLT);
                            equip.ScriptONU = Convert.ToString(onu.ScriptONU);
                            #endregion
                            var purl = CONSTANTES.URLConnect;
                            var reboot = Post(purl, "api/IntegracaoAtivacaoApiController/ReiniciarONU", equip);
                            var ret = new OnuList
                            {
                                Onu = reboot.Mensagem,
                            };
                            if (String.IsNullOrEmpty(reboot.Mensagem))
                            { retorno.Add(ret); }
                            else 
                            { retorno.Add(ret); }


                        }
                    }
                }

                return new ApiResponseObj<List<OnuList>> { data = retorno, ok = true };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<OnuList>> { error = ex.Message };
            }

        }
    }
}
