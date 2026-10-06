using Aniel.Connect.Core;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class EventoController : BaseController
    {

        public EventoController() : base("Evento",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/EventoApi/FindById?id={0}",
                                                   "api/EventoApi/Gravar",
                                                   "api/EventoApi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Evento
        [HttpGet()]
        public ActionResult Index(Int64? IdBot,String DescricaoBot)
        {
            ViewBag.Areas = Areas;
            

                return View();
        }

            [HttpGet()]
        public ActionResult Cadastro(Int64? id, Int64? IdBot, String DescricaoBot)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<BotEvento_CadastroModel, Int64?>(id);
            return retorno;
        }


        [HttpPost]
        public ActionResult Cadastro(BotEvento_CadastroModel pModel)
        {
            //var retorno = base.CadastroBase<BotEvento_CadastroModel, Int64>(pModel);

            using (var api = new EventoApiController())
            {
                var retorno = api.Gravar(new ParamWebApi<BotEvento_CadastroModel>
                {
                    Objeto = pModel,
                    Usuario = User.Identity.GetUserId()
                }).Result;

                if (!retorno.Ok)
                {
                    AddModelError(retorno.Mensagem, retorno.InnerException);
                    return View(pModel);
                }


            }

            //if (pModel.Id != 0)
            //{
            //    if (Request.Files.Count > 0)
            //        FileUpload();
            //}

            return RedirectToAction("Index", "Evento", new { IdBot = pModel.IdBot, DescricaoBot = pModel.NomeBot });
        }

        public ActionResult ListaAnexos(Int64 IdBotEvento)
        {
            return View();
        }

        public Response FileUpload()
        {
            Response retorno = new Response();
            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Atendimento";
            try
            {
                int arquivosSalvos = 0;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase arquivo = Request.Files[i];



                    //Salva o arquivo
                    if (arquivo.ContentLength > 0)
                    {
                        string caminhoArquivo = Path.Combine(_pasta, Path.GetFileName(arquivo.FileName));

                        if (!Directory.Exists(_pasta))
                        {
                            Directory.CreateDirectory(_pasta);
                        }
                        arquivo.SaveAs(caminhoArquivo);
                        arquivosSalvos++;
                    }
                }

                ViewData["Message"] = String.Format("{0} arquivo(s) salvo(s) com sucesso.",
                    arquivosSalvos);
                retorno.result = true;
            }
            catch (Exception e)
            {
                retorno.exception = e.Message;
                retorno.result = false;
            }
            return retorno;
        }
    }


}