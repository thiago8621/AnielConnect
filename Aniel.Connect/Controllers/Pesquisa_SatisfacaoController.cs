using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Pesquisa_SatisfacaoController : BaseController
    {

        public Pesquisa_SatisfacaoController() : base("Pesquisa_Satisfacao",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/pesquisa_Satisfacaoapi/FindById?id={0}",
                                                   "api/pesquisa_Satisfacaoapi/Gravar",
                                                   "api/pesquisa_Satisfacaoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Pesquisa_Satisfacao
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult Opinioes()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        [AllowAnonymous]        
        public async Task<ActionResult> Formulario(Int64 Id)
        {
            using (var api = new Pesquisa_SatisfacaoApiController())
            {
                var retorno = await api.FindByIdResposta(Id);

                if (retorno == null)
                {
                    AddModelError("Formulário não encontrado");
                }

                var pesquisaBanco = await api.FindByIdFormulario(retorno.Pesquisa_Satisfacao.Id);

                return View(new Pesquisa_Satisfacao_FormularioModel
                {
                    Pesquisa = pesquisaBanco.Objeto,
                    Id = retorno.Id,
                    Id_Cliente = retorno.Cliente.Id,
                    JaRespondido =retorno.DataHora_Respondida.HasValue,
                    Tipo_Resposta = pesquisaBanco.Objeto.Tipo_Resposta.ToString(),
                });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Formulario(Pesquisa_Satisfacao_FormularioModel Model)
        {
            var retorno = await new Pesquisa_SatisfacaoApiController().GravarFormulario(new ParamWebApi<Pesquisa_Satisfacao_FormularioModel>
            {
                Usuario = User.Identity.GetUserId(),
                Objeto = Model
            });

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem);
            }

            return RedirectToAction("FormularioConcluido", new { Titulo_Da_Pesquisa = Model.Pesquisa.Titulo_Da_Pesquisa } );
        }
        [AllowAnonymous]
        public async Task<ActionResult> FormularioConcluido(String Titulo_Da_Pesquisa)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                var config = await new Conexao<Configuracoes_Gerais>(session).selectFirst();

                return View(new Pesquisa_Satisfacao_FormularioConcluidoModel
                {
                    Titulo_Da_Pesquisa = Titulo_Da_Pesquisa,
                    URLLogo = config.OLA_URL_Logo,
                    NomeEmpresa = config.Nome_Cliente_Sinapse
                });
            }
        }

        public ActionResult ListaCliente(String Ids_Pesquisa_Satisfacao)
        {
            ViewBag.Areas = Areas;

            ViewBag.Planos = new List<PlanoProdutoListModel>();
            var _retornoPlanos = new PlanoProdutoApiController().GetList().Result;
            var _retornoCategorias = new Categoria_ClienteApiController().GetList().Result;

            if (!_retornoPlanos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPlanos.Mensagem, _retornoPlanos.InnerException);

            if (!_retornoCategorias.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoCategorias.Mensagem, _retornoCategorias.InnerException);

            ViewBag.Planos = _retornoPlanos.Objeto;
            ViewBag.Categorias = _retornoCategorias.Objeto;
            ViewBag.UserId = User.Identity.GetUserId();

            List<String> descricoesPesquisa = new List<String>();
            foreach (var idPesquisa in Ids_Pesquisa_Satisfacao.Split(','))
            {
                var descricao = new Pesquisa_SatisfacaoApiController().FindById(Convert.ToInt32(idPesquisa)).Result?.Objeto?.Descricao;
                if (String.IsNullOrEmpty(descricao))
                    descricoesPesquisa.Add(descricao);
            }

            return View(new ClientePesquisa_DataTableRequest
            {
                Ids_Pesquisa_Satisfacao = Ids_Pesquisa_Satisfacao,
                Descricoes_Pesquisa_Satisfacao = descricoesPesquisa
            });
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Pesquisa_SatisfacaoCadastroModel, Int64?>(id);
        }

        // POST: Pesquisa_Satisfacao/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Pesquisa_SatisfacaoCadastroModel pModel)
        {
            return base.CadastroBase<Pesquisa_SatisfacaoCadastroModel, Int64>(pModel);
        }

        protected override void FillListasView<T>(T pModel)
        {
            //var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            //if (!_retornoScripts.Ok)
            //    AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            //ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: Pesquisa_Satisfacao/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Pesquisa_SatisfacaoCadastroModel, Int64>(id.Value);
        }

        // POST: Pesquisa_Satisfacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Pesquisa_SatisfacaoCadastroModel pModel)
        {
            return base.DeleteBase<Pesquisa_SatisfacaoCadastroModel, Int64>(pModel);
        }

        public ActionResult ConsultaPesqSatisfCadCliente(Int64? Id_Cliente)
        {
            ViewBag.Id_Cliente = Id_Cliente ?? 0;

            return View();
                        
        }

        public async Task<ActionResult> Indicadores(Int64 Id)
        {
            IndicadoresModel model = new IndicadoresModel();

            var pesquisaApi = new Pesquisa_SatisfacaoApiController();

            var pesquisa = (await pesquisaApi.FindById(Id)).Objeto;

            model.Id_Pesquisa_Satisfacao = pesquisa.Id;

            if (!String.IsNullOrEmpty(pesquisa.Descricao_Pesquisa))
                model.Descricao_Pesquisa = pesquisa.Descricao_Pesquisa;

            model.Titulo_Pesquisa = pesquisa.Titulo_Da_Pesquisa;
            model.KeyPainel = "test_grafana/pesquisa-resposta?theme=light&kiosk=tv";
            model.VariavelIdpesquisa = "&var-Id_Pesquisa_Satisfacao=";
            model.VariavelDescricao = "&var-descricao=";
            model.VariavelGrafico1 = "&var-grafico1=";
            model.VariavelGrafico2 = "&var-grafico2=";
            model.VariavelGrafico3 = "&var-grafico3=";
            model.PainelGrafana1 = "&viewPanel=16";
            model.PainelGrafana2 = "&viewPanel=15";
            model.Pergunta = "&var-pergunta=";


            model.Url = String.Concat(CONSTANTES.URLGRAFANA,model.KeyPainel);
            model.Url_Local = String.Concat(CONSTANTES.URLGRAFANALOCAL, model.KeyPainel);

            using (ISession session = HibernateUtil.GetSession())
            {
                var pesquisa_resposta = await new Conexao<Pesquisa_Resposta>(session).select(p => p.Pesquisa_Satisfacao.Id == pesquisa.Id);

                var pesquisas_com_comentario = pesquisa_resposta.Count(p => !String.IsNullOrEmpty(p.Comentario));
                model.Tem_Comentario = pesquisas_com_comentario > 0;
            }

            model.Qtde_Envios_Respostas = (await pesquisaApi.GetIndicadorQtdeEnvios(Id)).Objeto;

            model.Perguntas = (await pesquisaApi.GetRespostasIndicadores(Id, 12, null)).Objeto;

            return View(model);
        }

        public ActionResult Historico_Alteracao(Int64? Ids_Pesquisa_Satisfacao)
        {
            ViewBag.Chave = "Ids_Pesquisa_Satisfacao";
            ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                        //new SelectListItem{ Value = "3", Text = Resources.RsBase.EXCLUSAO },
                        new SelectListItem{ Value = "4", Text = Resources.RsBase.BAIXA },
                    };
            return View();
        }

        public async Task<ActionResult> VisualizarRespostas(Int64 Id_Pesquisa_Resposta)
        {
            var Model = new VisualizarRespostasModel();

            using (ISession session = HibernateUtil.GetSession())
            using (var api = new Pesquisa_SatisfacaoApiController())
            {
                var pesquisa_resposta = (await api.FindByIdResposta(Id_Pesquisa_Resposta));

                ViewBag.Id_Cliente = pesquisa_resposta.Cliente.Id;

                ViewBag.Cliente_Preenche = pesquisa_resposta.Pesquisa_Satisfacao.QuemPreenche == PesquisaSatisfacaoQuemPreenche.Cliente;

                if (ViewBag.Cliente_Preenche)
                {
                    ViewBag.Quem_Respondeu = pesquisa_resposta.Cliente.NomeFantasia;
                }
                else
                {
                    var nome_atendedor = (await new Conexao<IdentityUser>(session).selectFirst(p => p.Id == pesquisa_resposta.UsuarioCriacao)).Nome;
                    ViewBag.Quem_Respondeu = nome_atendedor;
                }
                
                //ViewBag.Quem_Respondeu = ViewBag.Cliente_Preenche ? pesquisa_resposta.Cliente.NomeFantasia : pesquisa_resposta.UsuarioCriacao;

                Model.Id_Pesquisa_Satisfacao = pesquisa_resposta.Pesquisa_Satisfacao.Id;
                Model.Titulo_Pesquisa = pesquisa_resposta.Pesquisa_Satisfacao.Titulo_Da_Pesquisa;

                var respostas = (await api.GetRespostasPesquisa(Id_Pesquisa_Resposta)).Objeto;
                Model.Perguntas = respostas;

                if (pesquisa_resposta.Pesquisa_Satisfacao.Tem_Caixa_Texto)
                {
                    Model.Categoria_Comentario = pesquisa_resposta.Categoria;
                    Model.Comentario_Cliente = pesquisa_resposta.Comentario;
                }
            }

            return View(Model);
        }
        //public ActionResult Indicadores(string parametro)
        //{
        //    GetApiRetorno<GrafanaRequestModel> retorno = null;
        //    using (var satisfacao = new Pesquisa_SatisfacaoApiController()) 
        //    {
        //        retorno = satisfacao.GetRespostasIndicadoresNovo();
        //    }
        //        return View(retorno.Objeto);
        //}
    }
}