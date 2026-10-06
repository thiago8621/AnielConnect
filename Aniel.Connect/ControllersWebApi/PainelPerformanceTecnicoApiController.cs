using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ReincidenciaDocProd;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PainelPerformanceTecnicoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/PainelPerfomanceProdutivaApiController/GetAll")]
        public DataTableResponse<PainelPerformanceTecnicoListModel> GetAll(PainelPerformanceTecnicoDataTableRequest request)
        {
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionT5NH = HibernateUtil.GetSessionT5())
            using (ISession sessionConnectNH = HibernateUtil.GetSession())
            using (var repositoryHorario = new Conexao<Horario_Connect>(sessionT5NH))
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            {
                List<Horario_Connect> horarios = (repositoryHorario.select(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                IPainelPerformanceTecnicoRepository repoPainelPerformanceTecnico = new PainelPerformanceTecnicoRepository(sessionT5NH, sessionT5, sessionConnectNH,conexaoMongo);

                var businessProdutividadeTecnico = new PerformanceTecnicoAnielBusiness(repoPainelPerformanceTecnico);

                PainelPerformanceTecnicoResponse relatorio = businessProdutividadeTecnico.GetRelatorio(request);

                return new DataTableResponse<PainelPerformanceTecnicoListModel>
                {
                    draw = request.draw,
                    recordsFiltered = relatorio.Total,
                    data = relatorio.Data
                }; 
            }
        }
    }
}