using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Documento_ProducaoController : BaseController
    {

        public Documento_ProducaoController() : base("Documento_Producao", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Documento_Producaoapi/FindById_HabilitarServico?equipe={0}",
                                                   "api/Documento_Producaoapi/Gravar",
                                                   "api/Documento_Producaoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Documento_Producao

        public ActionResult Index()
        {
            return View();
        }

        // GET: Documento_Producao
        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return View();
        }


        [HttpPost()]
        public ActionResult Cadastro(Documento_ProducaoCadastroModel value)
        {
            return View();
        }

        protected override void FillListasView<T>(T pModel)
        {
           


            
        }

        [HttpGet()]
        public async Task<ActionResult> GetMotivoParalizacao(String codCt, String numObra, int contrato, String projeto, String tecnico)
        {
            var documentoApi = new Documento_ProducaoApiController();
            var value = new MotivoParalizacaoRequest() 
            {
                CodCt = codCt,
                NumObra = numObra,
                Contrato = contrato,
                Projeto = projeto,
                Tecnico = tecnico
            };
            return View(await documentoApi.GetMotivoParalizacao(value)); 
        }

        // GET: Documento_Producao/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Documento_ProducaoCadastroModel, Int64>(id.Value);
        }

        // POST: Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Documento_ProducaoCadastroModel pModel)
        {
            return base.DeleteBase<Documento_ProducaoCadastroModel, Int64>(pModel);
        }

        
    }
}