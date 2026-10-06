using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Modelo_ReciboController : BaseController
    {

        public Modelo_ReciboController() : base("Modelo_Recibo",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Modelo_Reciboapi/FindById?id={0}",
                                                   "api/Modelo_Reciboapi/Gravar",
                                                   "api/Modelo_Reciboapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Campanha
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Modelo_ReciboCadastroModel, Int64?>(id);

            return retorno;
        }

        [HttpPost]
        public ActionResult Cadastro(Modelo_ReciboCadastroModel pModel)
        {

            try
            {
                if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                {
                    pModel.Nome_Arquivo = Request.Files[0].FileName;
                    pModel.Extensao_Arquivo = pModel.Nome_Arquivo.Split('.').Last();
                }

                if (ModelState.IsValid)
                {
                    pModel.ArquivoAlterado = !String.IsNullOrEmpty(Request.Files[0].FileName);

                    PostApiRetorno<Int64> retornoPost = base.PostByUrl<Modelo_ReciboCadastroModel, Int64>(pModel, CONSTANTES.URLPortalInterno, "api/Modelo_Reciboapi/Gravar");

                    if (!retornoPost.Ok)
                    {
                        AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                        return View(pModel);
                    }
                    else
                    {
                        String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Recibo"; // \{Model.Id}
                        String _arquivo = $@"{_pasta}\{retornoPost.IdObjeto}.{pModel.Extensao_Arquivo}";

                        if (!Directory.Exists(_pasta))
                        {
                            Directory.CreateDirectory(_pasta);
                        }

                        if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                        {
                            Request.Files[0].SaveAs(_arquivo);
                        }

                        return RedirectToAction("Index");
                    }
                }

                else
                {
                    return View(pModel);
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(pModel);
            }
        }

        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Modelo_ReciboCadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Modelo_ReciboCadastroModel pModel)
        {
            return base.DeleteBase<Modelo_ReciboCadastroModel, Int64>(pModel);
        }

        public ActionResult ModeloReciboAux(Modelo_ReciboListaModel Model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    Model.Nome_Arquivo = Request.Files[0].FileName;
                    Model.Extensao_Arquivo = Model.Nome_Arquivo.Split('.').Last();
                }

                PostApiRetorno<Int64> retornoPost = base.PostByUrl<Modelo_ReciboListaModel, Int64>(Model, CONSTANTES.URLPortalInterno, "api/Modelo_Reciboapi/Gravar");

                GetListaModeloRecibo();

                if (!retornoPost.Ok)
                {
                    AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                    return View(Model);
                }
                else
                {
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Recibo"; // \{Model.Id}
                    String _arquivo = $@"{_pasta}\{retornoPost.IdObjeto}.{Model.Extensao_Arquivo}";

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    if (Request.Files.Count > 0)
                    {
                        Request.Files[0].SaveAs(_arquivo);
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(Model);
            }
        }

        private void GetListaModeloRecibo()
        {
            ViewBag.Areas = Areas;

            GetApiRetorno<List<Modelo_ReciboListaModel>> retornoLista = base.GetAll<Modelo_ReciboListaModel>($"api/Modelo_Reciboapi/GetAll");

            if (!retornoLista.Ok)
            {
                AddModelError(retornoLista.Mensagem);
                ViewBag.Lista = new List<Modelo_ReciboListaModel>();
            }
            else
            {
                ViewBag.Lista = retornoLista.Objeto;
            }

        }
    }
}