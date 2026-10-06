using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class OLT_Grupo_AreaApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/OLT_Grupo_AreaApi/ExcluirGrid")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(ExclusaoEmMassa values)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<OLT_Grupo_Area>(session))
                    {

                        OLT_Grupo_Area modelo = null;
                        foreach (int value in values?.ids)
                        {
                            if (value == 0)
                                continue;

                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();
                            modelo.Status = Sinapse.Model.enumerados.Status_Default.Inativo;
                            modelo.UsuarioAlteracao = User.Identity.GetUserId();
                            
                            await repository.SaveOrUpdate(modelo);
                            
                            //repository.Remove(modelo);
                            
                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
    }
}