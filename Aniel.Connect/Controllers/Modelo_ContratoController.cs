using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
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
    public class Modelo_ContratoController : BaseController
    {

        public Modelo_ContratoController() : base("Modelo_Contrato",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Modelo_Contratoapi/GetArquivoContrato?id={0}",
                                                   "api/Modelo_Contratoapi/Gravar",
                                                   "api/Modelo_Contratoapi/Excluir",
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
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<Modelo_ContratoCadastroModel, Int64?>(id);
            ViewBag.url_ArquivoPadrao = String.Format(@"{0}/Arquivos/Modelo_Contrato/Modelo_padrao_Contrato.docx", CONSTANTES.URLPortal);

            return retorno;
        }

        [HttpPost]
        public ActionResult Cadastro(Modelo_ContratoCadastroModel pModel)
        {
            ViewBag.Areas = Areas;

            try
            {
                if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                {
                    pModel.Arquivo_Doc = Request.Files[0].FileName;
                    pModel.Extensao_Arquivo = pModel.Arquivo_Doc.Split('.').Last();
                }

                pModel.ArquivoAlterado = !String.IsNullOrEmpty(Request.Files[0].FileName);
                
                
                PostApiRetorno<Int64> retornoPost = base.PostByUrl<Modelo_ContratoCadastroModel, Int64>(pModel, CONSTANTES.URLPortalInterno, "api/Modelo_Contratoapi/Gravar");

                if (!retornoPost.Ok)
                {
                    AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                    // Preenchendo o nome do arquivo do contrato com o nome do arquivo antigo caso ocorra algum erro
                    // durante a gravação da atualização de um modelo de contrato já cadastrado
                    if (pModel.Id != 0)
                    {
                        var oldModelRequest = base.GetByUrl<Modelo_ContratoCadastroModel>(CONSTANTES.URLPortalInterno, String.Format(base.UrlGetById, pModel.Id));

                        pModel.Arquivo_Doc = oldModelRequest.Objeto?.Arquivo_Doc;
                        pModel.Extensao_Arquivo = oldModelRequest.Objeto?.Extensao_Arquivo;
                    }

                    return View(pModel);
                }
                else
                {
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato"; // \{Model.Id}
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

            return base.DeleteBase<Modelo_ContratoCadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Modelo_ContratoCadastroModel pModel)
        {
            return base.DeleteBase<Modelo_ContratoCadastroModel, Int64>(pModel);
        }

        public ActionResult ModeloContratoAux(Modelo_ContratoListaModel Model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    Model.Arquivo_Doc = Request.Files[0].FileName;
                    Model.Extensao_Arquivo = Model.Arquivo_Doc.Split('.').Last();
                }

                PostApiRetorno<Int64> retornoPost = base.PostByUrl<Modelo_ContratoListaModel, Int64>(Model, CONSTANTES.URLPortalInterno, "api/Modelo_ContratoApi/Gravar");

                GetListaModeloContrato();

                if (!retornoPost.Ok)
                {
                    AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                    return View(Model);
                }
                else
                {
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato"; // \{Model.Id}
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

        private void GetListaModeloContrato()
        {
            ViewBag.Areas = Areas;

            GetApiRetorno<List<Modelo_ContratoListaModel>> retornoLista = base.GetAll<Modelo_ContratoListaModel>($"api/Modelo_ContratoApi/GetAll");

            if (!retornoLista.Ok)
            {
                AddModelError(retornoLista.Mensagem);
                ViewBag.Lista = new List<Modelo_ContratoListaModel>();
            }
            else
            {
                ViewBag.Lista = retornoLista.Objeto;
            }

        }
    }
}