using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PerfilVisualizacaoServico;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamArea;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamContratoProjeto;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamMonitor;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamSetorDepto;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamSupervisor;
using Aniel.Connect.Repository.T5.Area;
using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.T5.Funcionario;
using Aniel.Connect.Repository.T5.GrupoArea;
using Aniel.Connect.Repository.T5.Localidade;
using Aniel.Connect.Repository.T5.Projeto;
using Aniel.Connect.Repository.T5.Setor;
using Aniel.Connect.Repository.T5.Usuario;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PerfilVisualizacaoServicoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/PerfilVisualizacaoServicoApi/GetAll")]
        public async Task<DataTableResponse<PerfilVisualizacaoServicoListModel>> GetAll(DataTableRequest request)
        {
            int recordsTotal = 0;
            var retorno = new List<PerfilVisualizacaoServicoListModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                var sqlFrom = new StringBuilder();
                var param = new Dictionary<string, object>();

                sqlFrom.AppendLine("FROM TB_PERFIL_VIS_SERV PV");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine("WHERE UPPER(PV.DESCRICAO) LIKE UPPER(@SEARCH)");
                    param.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT COUNT(*) AS QTD_PERFIS");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["QTD_PERFIS"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                { 
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT PV.ID, PV.DESCRICAO, PV.STATUS");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, param, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new PerfilVisualizacaoServicoListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<PerfilVisualizacaoServicoListModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpPost]
        [Route("api/PerfilVisualizacaoServicoApi/Gravar")]
        public PostApiRetorno<long> Gravar([FromBody] ParamWebApi<PerfilVisualizacaoServicoCadastroModel> param)
        {
            var retorno = new PostApiRetorno<long>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ITransaction transaction = sessionNh.BeginTransaction())
            {
                try
                {
                    IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                    IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                    IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                    IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                    IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                    var perfilBusiness =
                        new PerfilVisualizacaoServicoBusiness(
                            repository,
                            repoParamContratoProjeto,
                            repoParamArea,
                            repoParamSetor,
                            repoParamSuperv,
                            repoParamMonitor,
                            repoContrato,
                            repoProjeto,
                            repoGrupoArea,
                            repoArea,
                            repoLocalidade,
                            repoSetor,
                            repoFuncionario,
                            repoUsuario);

                    PerfilVisualizacaoServico model = perfilBusiness.Gravar(param.Objeto, param.Usuario);
                    transaction.Commit();

                    retorno.Ok = true;
                    retorno.IdObjeto = model.Id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoServicoApi/FindById")]
        public GetApiRetorno<PerfilVisualizacaoServicoCadastroModel> FindById(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoServicoCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                    IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                    IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                    IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                    IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                    var perfilBusiness =
                        new PerfilVisualizacaoServicoBusiness(
                            repository,
                            repoParamContratoProjeto,
                            repoParamArea,
                            repoParamSetor,
                            repoParamSuperv,
                            repoParamMonitor,
                            repoContrato,
                            repoProjeto,
                            repoGrupoArea,
                            repoArea,
                            repoLocalidade,
                            repoSetor,
                            repoFuncionario,
                            repoUsuario);

                    retorno.Objeto = perfilBusiness.FindById(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/PerfilVisualizacaoServicoApi/GetCombo")]
        public ObjCombo<long> GetComboProjeto(RequestObjCombo<long?> request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            {
                IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                var perfilBusiness =
                    new PerfilVisualizacaoServicoBusiness(
                        repository,
                        repoParamContratoProjeto,
                        repoParamArea,
                        repoParamSetor,
                        repoParamSuperv,
                        repoParamMonitor,
                        repoContrato,
                        repoProjeto,
                        repoGrupoArea,
                        repoArea,
                        repoLocalidade,
                        repoSetor,
                        repoFuncionario,
                        repoUsuario);

                return perfilBusiness.GetCombo(request, CONSTANTES.PaginacaoRegistros);
            }
        }

        [HttpPost]
        [Route("api/PerfilVisualizacaoServicoApi/AlterarStatus")]
        public GetApiRetorno AlterarStatus(ParamVisualizacaoServicoAlterarStatusRequest request)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoServicoCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                    IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                    IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                    IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                    IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                    var perfilBusiness =
                        new PerfilVisualizacaoServicoBusiness(
                            repository,
                            repoParamContratoProjeto,
                            repoParamArea,
                            repoParamSetor,
                            repoParamSuperv,
                            repoParamMonitor,
                            repoContrato,
                            repoProjeto,
                            repoGrupoArea,
                            repoArea,
                            repoLocalidade,
                            repoSetor,
                            repoFuncionario,
                            repoUsuario);

                    foreach (long id in request.Ids)
                    {
                        perfilBusiness.AlterarStatus(id, request.Status);
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoServicoApi/ExcluirParamContratoProjeto")]
        public GetApiRetorno ExcluirParamContratoProjeto(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoServicoCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                    IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                    IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                    IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                    IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                    var perfilBusiness =
                        new PerfilVisualizacaoServicoBusiness(
                            repository,
                            repoParamContratoProjeto,
                            repoParamArea,
                            repoParamSetor,
                            repoParamSuperv,
                            repoParamMonitor,
                            repoContrato,
                            repoProjeto,
                            repoGrupoArea,
                            repoArea,
                            repoLocalidade,
                            repoSetor,
                            repoFuncionario,
                            repoUsuario);

                    perfilBusiness.ExcluirParamContratoProjeto(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoServicoApi/ExcluirParamArea")]
        public GetApiRetorno ExcluirParamArea(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoServicoCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                    IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                    IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                    IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                    IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                    var perfilBusiness =
                        new PerfilVisualizacaoServicoBusiness(
                            repository,
                            repoParamContratoProjeto,
                            repoParamArea,
                            repoParamSetor,
                            repoParamSuperv,
                            repoParamMonitor,
                            repoContrato,
                            repoProjeto,
                            repoGrupoArea,
                            repoArea,
                            repoLocalidade,
                            repoSetor,
                            repoFuncionario,
                            repoUsuario);

                    perfilBusiness.ExcluirParamArea(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoServicoApi/ExcluirParamSetor")]
        public GetApiRetorno ExcluirParamSetor(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoServicoCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                    IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                    IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                    IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                    IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                    var perfilBusiness =
                        new PerfilVisualizacaoServicoBusiness(
                            repository,
                            repoParamContratoProjeto,
                            repoParamArea,
                            repoParamSetor,
                            repoParamSuperv,
                            repoParamMonitor,
                            repoContrato,
                            repoProjeto,
                            repoGrupoArea,
                            repoArea,
                            repoLocalidade,
                            repoSetor,
                            repoFuncionario,
                            repoUsuario);

                    perfilBusiness.ExcluirParamSetorDepto(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoServicoApi/ExcluirParamSupervisor")]
        public GetApiRetorno ExcluirParamSupervisor(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoServicoCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                    IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                    IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                    IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                    IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                    var perfilBusiness =
                        new PerfilVisualizacaoServicoBusiness(
                            repository,
                            repoParamContratoProjeto,
                            repoParamArea,
                            repoParamSetor,
                            repoParamSuperv,
                            repoParamMonitor,
                            repoContrato,
                            repoProjeto,
                            repoGrupoArea,
                            repoArea,
                            repoLocalidade,
                            repoSetor,
                            repoFuncionario,
                            repoUsuario);

                    perfilBusiness.ExcluirParamSupervisor(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoServicoApi/ExcluirParamMonitor")]
        public GetApiRetorno ExcluirParamMonitor(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoServicoCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoServicoRepository repository = new PerfilVisualizacaoServicoRepository(sessionNh);

                    IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto = new PerfilVisualizacaoServicoParamContratoProjetoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamAreaRepository repoParamArea = new PerfilVisualizacaoServicoParamAreaRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoServicoParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv = new PerfilVisualizacaoServicoParamSupervisorRepository(sessionNh);
                    IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor = new PerfilVisualizacaoServicoParamMonitorRepository(sessionNh);

                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5Nh);
                    IProjetoAnielRepository repoProjeto = new ProjetoAnielRepository(sessionT5Nh);

                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5Nh);

                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);

                    IFuncionarioAnielRepository repoFuncionario = new FuncionarioAnielRepository(sessionT5Nh);
                    IUsuarioAnielRepository repoUsuario = new UsuarioAnielRepository(sessionT5Nh);

                    var perfilBusiness =
                        new PerfilVisualizacaoServicoBusiness(
                            repository,
                            repoParamContratoProjeto,
                            repoParamArea,
                            repoParamSetor,
                            repoParamSuperv,
                            repoParamMonitor,
                            repoContrato,
                            repoProjeto,
                            repoGrupoArea,
                            repoArea,
                            repoLocalidade,
                            repoSetor,
                            repoFuncionario,
                            repoUsuario);

                    perfilBusiness.ExcluirParamMonitor(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
    }
}
