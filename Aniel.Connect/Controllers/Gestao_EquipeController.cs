using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using Remotion.Linq.Clauses.ResultOperators;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Gestao_EquipeController : BaseController
    {

        public Gestao_EquipeController() : base("Gestao_Equipes",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Gestao_Equipesapi/FindById?id={0}",
                                                   "api/Gestao_Equipesapi/Gravar",
                                                   "api/Gestao_Equipesapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Timeline_Servicos
        [HttpGet()]
        public ActionResult Index()
        {          

            if (Areas.Find(p => p.Cod_Area == "124") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.Areas = Areas;        

            return View();
        }

        protected override void FillListasView<T>(T pModel)
        {          
            using (var grupoApi = new Grupo_AreaAnielApiController())
            using (var naturezaApi = new Natureza_ProducaoApiController()) 
            {
                ViewBag.GruposArea = grupoApi.GetList().Result.Objeto;
                ViewBag.Natureza = naturezaApi.GetList().Result.Objeto;
            }
          
            ViewBag.Areas = new List<AreaModuloModel>();
            ViewBag.Localidades = new List<String>();
            ViewBag.Jornadas = new List<String>();
            ViewBag.Natureza = new Natureza_ProducaoApiController().GetList().Result.Objeto;
        }


        [HttpGet()]
        public async Task<ActionResult> Timeline_Servicos(Int32? TipoFiltro, Int32? FormatoTimeline, Int32? TipoVisualizacao, Int32? Prazo, String Num_Fre_Supervisor, String Id_Monitor,
            String Cod_Escritorio, String Cod_Area, Int32? Cod_Localidade, Int64? Id_Natureza, String Data,String TipoServico, String DataFinal, String Tecnico, string Contrato, string Projeto)
        {
            ContratoAniel contrato = null;
            ProjetoAniel projeto = null;

            using (ISession sessionNh = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Master>(sessionNh))            
            {
                var config = repoConfig.selectFirst().Result;
                ViewBag.UtilizaParalisaOSTL = config.ConfigParalisaOS;                
            }

            if (Areas.Find(p => p.Cod_Area == "134") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            DateTime? DataConsulta = String.IsNullOrEmpty(Data) ? DateTime.Now : Convert.ToDateTime(Data);
            DateTime? DataConsultaFim = String.IsNullOrEmpty(DataFinal) ? DateTime.Now : Convert.ToDateTime(DataFinal);

            using (ISession session = HibernateUtil.GetSession())
            using (var repo = new Conexao<Configuracoes_Gerais>(session))
            {
                var conf_gerais = repo.selectFirst().Result;
                ViewBag.TimerAtualizacaoTimeline = conf_gerais.Tempo_Atualizacao_Automatica_Timeline;
                ViewBag.Param_datahora_timeline = conf_gerais.ParametroTimeline;
                ViewBag.InicioTimelineHoras = conf_gerais.LimiteMinimoTimeline.HasValue ? conf_gerais.LimiteMinimoTimeline.Value.TimeOfDay.Hours : 0;
                ViewBag.InicioTimelineMinutos = conf_gerais.LimiteMinimoTimeline.HasValue ? conf_gerais.LimiteMinimoTimeline.Value.TimeOfDay.Minutes : 0;
                ViewBag.FimTimelineHoras = conf_gerais.LimiteMaximoTimeline.HasValue ? conf_gerais.LimiteMaximoTimeline.Value.TimeOfDay.Hours : 23;
                ViewBag.FimTimelineMinutos = conf_gerais.LimiteMaximoTimeline.HasValue ? conf_gerais.LimiteMaximoTimeline.Value.TimeOfDay.Minutes : 59;
            }

            Timeline_ServicosModels model = new Timeline_ServicosModels
            {
                TipoFiltro = TipoFiltro ?? 0,
                TipoVisualizacao = TipoVisualizacao ?? 0,
                FormatoTimeline = FormatoTimeline ?? 0,
                Tecnico = Tecnico,
                Prazo = Prazo ?? 0,
                Num_Fre_Supervisor = Num_Fre_Supervisor,
                Id_Monitor = Id_Monitor,
                Cod_Escritorio = Cod_Escritorio,
                Cod_Area = Cod_Area,
                Contrato = Contrato,
                Projeto = Projeto,
                Cod_Localidade = Cod_Localidade,
                Id_Natureza = Id_Natureza,
                TipoServico = TipoServico,
                Data = DataConsulta.HasValue ? DataConsulta.Value.ToString("dd/MM/yyyy") : String.Empty,
                DataFinal = DataConsultaFim.HasValue ? DataConsultaFim.Value.ToString("dd/MM/yyyy") : String.Empty,
                Equipes = new List<Timeline_ServicosEquipeModels>()
            };

            FillListasView<Timeline_ServicosModels>(model);
            ViewBag.Areas = Areas;

            return View(model);
        }

        [HttpPost()]
        public ActionResult Timeline_Servicos(Timeline_ServicosModels Model)
        {
            return RedirectToAction("Timeline_Servicos", new
            {
                Model.TipoVisualizacao,
                Model.FormatoTimeline,
                Model.Prazo,
                Model.TipoFiltro,
                Model.Num_Fre_Supervisor,
                Model.Id_Monitor,
                Model.Cod_Escritorio,
                Model.Cod_Area,
                Model.Cod_Localidade,
                Model.Id_Natureza,
                Model.TipoServico,
                Model.Data,
                Model.DataFinal,
                Model.Tecnico,
            });
        }

        [HttpGet()]
        public ActionResult Acompanhamento_Equipes()
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Sinapse.Model.Identity.IdentityUser>(session))
            using (var repositoryT5 = new Conexao<FuncionarioAniel>(sessionT5))
            {
                var conf_gerais = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result;

                //Perfil Usuário Connect
                var perfil_usuario = repository.selectFirst(p => p.Id == User.Identity.GetUserId()).Result;
                //ViewBag.Secao_Connect = perfil_usuario.Secao_Connect;
                //ViewBag.Tipo_Usuario_Connect = perfil_usuario.Tipo_Usuario;
                var secao_connect = perfil_usuario.Secao_Connect;
                var tipo_usuario_connect = perfil_usuario.Tipo_Usuario;

                switch (secao_connect)
                {
                    case 0:
                        ViewBag.Secao_Connect = SecaoConnectPerfilEnum.ATENDIMENTO.ToString();
                        break;
                    case 1:
                        ViewBag.Secao_Connect = SecaoConnectPerfilEnum.COMERCIAL.ToString();
                        break;
                    case 2:
                        ViewBag.Secao_Connect = SecaoConnectPerfilEnum.SERVICOS.ToString();
                        break;
                    case 3:
                        ViewBag.Secao_Connect = "NENHUM";
                        break;
                    case null:
                        ViewBag.Secao_Connect = "NULL";
                        break;
                    default:
                        ViewBag.Secao_Connect = "Seção não cadastradada";
                        break;
                }

                switch (tipo_usuario_connect)
                {
                    case 0:
                        ViewBag.Tipo_Usuario_Connect = TipoUsuarioEnum.ATENDENTE.ToString();
                        break;
                    case 1:
                        ViewBag.Tipo_Usuario_Connect = TipoUsuarioEnum.SUPERVISOR_ATENDENTE.ToString();
                        break;
                    case 2:
                        ViewBag.Tipo_Usuario_Connect = TipoUsuarioEnum.SUPERVISOR_COMERCIAL.ToString();
                        break;
                    case 3:
                        ViewBag.Tipo_Usuario_Connect = TipoUsuarioEnum.VENDEDOR.ToString();
                        break;
                    case 4:
                        ViewBag.Tipo_Usuario_Connect = TipoUsuarioEnum.MONITOR_EQUIPE.ToString();
                        break;
                    case 5:
                        ViewBag.Tipo_Usuario_Connect = TipoUsuarioEnum.SUPERVISOR_EQUIPE.ToString();
                        break;
                    case 6:
                        ViewBag.Tipo_Usuario_Connect = "NENHUM";
                        break;
                    case null:
                        ViewBag.Tipo_Usuario_Connect = "NULL";
                        break;
                    default:
                        ViewBag.Tipo_Usuario_Connect = "Tipo de usuário não cadastrada";
                        break;
                }

                //Monitor
                var usuario = repository.selectFirst(p => p.Id == User.Identity.GetUserId()).Result;
                ViewBag.num_fre_usuario = usuario?.Funcionario?.Num_Fre;
                ViewBag.Monitor = usuario == null ? false : usuario.E_Monitor;


                if (usuario?.Funcionario?.Num_Fre != null)
                {
                    var usuarioSupervisor = repositoryT5.selectFirst(p => p.Num_Fre == usuario.Funcionario.Num_Fre).Result;
                    ViewBag.Supervisor = usuarioSupervisor == null ? false : usuarioSupervisor.Supervisor == "S";
                }
                else
                    ViewBag.Supervisor = false;



                ViewBag.RealizaAgendamento = conf_gerais.Exige_Agendamento_Servico;
                ViewBag.Areas = Areas;
                ViewBag.TimerAtualizacaoTecnico = conf_gerais.Tempo_Atualizacao_Automatica_Painel_Acompanhamento_Tecnico;

            }
            return View(new Acompanhamento_Equipes
            {
                Tecnico_Ativos = true,
                Tecnico_Em_Execucao = false,
                DataAbertoIni = DateTime.Now.ToString("dd/MM/yyyy"),
                DataAbertoFim = DateTime.Now.ToString("dd/MM/yyyy"),
                DataEncerradosIni = DateTime.Now.ToString("dd/MM/yyyy"),
                DataEncerradosFim = DateTime.Now.ToString("dd/MM/yyyy"),
                DataParalisadosIni = DateTime.Now.ToString("dd/MM/yyyy"),
                DataParalisadosFim = DateTime.Now.ToString("dd/MM/yyyy")
            });
        }

        //Teste Atualizando Hora
        [HttpGet()]
        public ActionResult Load()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult PlanejamentoCapacidadeRoteirizacao()
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repo = new Conexao<Configuracoes_Gerais>(session))
            using (var repoHorario = new Conexao<Horario_Connect>(session))
            using (var horarioApi = new HorarioApiController())
            {
                var config = repo.selectFirst().Result;
                ViewBag.TempoExecucaoPadrao = config.Tempo_Execucao_Padrao.HasValue ? config.Tempo_Execucao_Padrao.Value.TimeOfDay.TotalMinutes : 0;
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT VALOR, DESCRICAO FROM TB_TIPO_HABILIDADE_NATUREZA ");


                    using (var reader = CommandHelper.ExecuteReader(com, sql, sessionT5.Connection, null).Result)
                    {
                        while (reader.ReadAsync().Result)
                        {

                            if (Convert.ToString(reader["VALOR"]) == "0")
                                ViewBag.DescricaoTipoHabilidade0 = Convert.ToString(reader["DESCRICAO"]);
                            if (Convert.ToString(reader["VALOR"]) == "1")
                                ViewBag.DescricaoTipoHabilidade1 = Convert.ToString(reader["DESCRICAO"]);
                            if (Convert.ToString(reader["VALOR"]) == "2")
                                ViewBag.DescricaoTipoHabilidade2 = Convert.ToString(reader["DESCRICAO"]);

                        }
                    }


                }

            }
            return View();
        }

        [HttpGet()]
        public ActionResult PainelGestaoOperacional()
        {
            return View();
        }


        [HttpGet()]
        public ActionResult Mapa_Equipes(string Equipe, DateTime? Data, String Nome_Equipe, String Pagina)
        {
            if (String.IsNullOrEmpty(Equipe))
            {
                return View(new Equipe_MapaItemModel());
            }

            var retorno = new Mapa_EquipesApiController().GetEquipeNoMapa(Equipe, Data).Result;

            if (!retorno.Ok)
            {
                retorno.Objeto = retorno.Objeto == null ? new Equipe_MapaItemModel() : retorno.Objeto;
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }
            else
            {
                retorno.Objeto.Data_Busca = Data != null ? Convert.ToDateTime(Data).ToString("dd/MM/yyyy") : String.Empty;
            }

            return View(retorno.Objeto);

        }

        [HttpGet()]
        public ActionResult PainelGestaoOperacionalExtratoAnalitico()
        {
            return View();
        }     
    }
}