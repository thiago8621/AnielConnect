using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.LimiteAgendamento;
using Aniel.Connect.Repository.T5.Natureza;
using NHibernate;
using Sinapse.DAO.repository;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{

    [Authorize]
    public class LimiteAgendamentoApiController : ApiController
    {
        [HttpPost]
        [Route("api/LimiteAgendamentoApi/GetAll")]
        public DataTableResponse<LimiteAgendamentoListModel> GetAll(LimiteAgendamentoDataTableRequest request)
        {
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            {
                ILimiteAgendamentoAnielRepository repoLimAgend = new LimiteAgendamentoRepository(sessionT5Sql, sessionT5Nh);
                INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5Nh);
                
                var limAgendBusiness = new LimiteAgendamentoAnielBusiness(repoLimAgend, repoNatureza);

                var response = limAgendBusiness.GetAll(request);

                return new DataTableResponse<LimiteAgendamentoListModel>
                {
                    draw = request.draw,
                    recordsFiltered = response.Total,
                    data = response.Data
                };
            }
        }
    }
}
