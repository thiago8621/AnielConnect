//using Aniel.ConnectAPI.Dal.Aniel;
using Aniel.ConnectAPI.Model.Params;
using Aniel.ConnectAPI.Controllers.Base;
using Aniel.ConnectAPI.Model;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.ConnectAPI.Dal.Aniel;
using AnielTV.Hibernate.Persistence;
using NHibernate;
using Aniel.Hibernate.Entity;
using Aniel.ConnectAPI.Model.Aniel;
using Aniel.ConnectAPI.Dal;
using System.Reflection;

namespace Aniel.ConnectAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [ApiController]
    public class JobsController : BaseController
    {
        private readonly ISession session;

        public JobsController(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Atualiza todos os registros de todos objetos que ainda não foram executados
        /// </summary>
        /// <returns></returns>
        [HttpGet("AtualizaTodos")]
        public Response AtualizarTodos()
        {
            Response resposta = new Response();

            try
            {
                var types = from type in this.GetType().Assembly.GetTypes()
                        where typeof(IAtualizacaoAutomatica).IsAssignableFrom(type)
                        select type;

                foreach (Type type in types)
                {
                    ConstructorInfo ctor = type.GetConstructor(new[] { typeof(ISession) });
                    IAtualizacaoAutomatica instance = (IAtualizacaoAutomatica)ctor.Invoke(new object[] { session });

                    instance.AtualizarTodos();
                }
            }
            catch (Exception ex)
            {
                TrataRetornoErro(this.GetType().Assembly, out resposta, ex, null);
            }

            return null;
        }

    }    

}
