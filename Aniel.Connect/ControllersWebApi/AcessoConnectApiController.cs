using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.T5;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public partial class AcessoConnectApiController : AcessoApiController
    {

        [HttpGet()]
        [Route("api/AcessoConnectApi/GetMenu_IFrame")]
        public async Task<IEnumerable<IMenu_IFrame>> GetMenu_IFrame(String id_usuario)
        {
            List<IMenu_IFrame> retorno = new List<IMenu_IFrame>();

            GetApiRetorno<List<AreaModuloModel>> retorno_acessos_usuario = await GetArea_ModuloUsuario(id_usuario);

            if (retorno_acessos_usuario.Ok)
            {
                List<AreaModuloModel> acessos = (List<AreaModuloModel>)retorno_acessos_usuario.Objeto;

                if (!(await GetConfiguracoes()).Empresa_Roteirizacao_Servico)
                {
                    var obj = acessos.Find(p => p.Cod_Area == "128");
                    if (obj != null)
                        acessos.Remove(obj);
                }

                var comAcesso = from o in MENU.GetMenus(User.Identity.GetUserId())
                                where (o.cod_area == null || (acessos.Find(p => p.Cod_Area == o.cod_area) != null))
                                select o;

                retorno.AddRange(comAcesso);
            }
            else
            {
                throw new Exception($"{retorno_acessos_usuario.Mensagem} {retorno_acessos_usuario.InnerException}");
            }

            return retorno;
        }

        private async Task<Configuracoes_Gerais> GetConfiguracoes()
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                return await new Conexao<Configuracoes_Gerais>(session).selectFirst();
            }
        }

        private async Task<GetApiRetorno<List<AreaModuloModel>>> GetArea_Modulo(String id_usuario)
        {
            GetApiRetorno<List<AreaModuloModel>> retorno = new GetApiRetorno<List<AreaModuloModel>>();

            try
            {
                return await GetArea_ModuloUsuario(id_usuario);
            }
            catch (Exception ex)
            {
                retorno.Objeto = new List<AreaModuloModel>();
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;

        }

    }
}