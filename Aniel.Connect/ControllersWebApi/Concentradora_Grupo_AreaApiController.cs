using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
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
    public class Concentradora_Grupo_AreaApiController: BaseApiController
    {
        [HttpGet]
        [Route("api/Concentradora_Grupo_AreaApi/ExcluirGrid")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value, String usuario)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Concentradora_Grupo_Area>(session))
                    {

                        Concentradora_Grupo_Area modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();
                            modelo.Status = Sinapse.Model.enumerados.Status_Default.Inativo;
                            modelo.UsuarioAlteracao = usuario;

                            await repository.SaveOrUpdate(modelo);

                            //repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
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