using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class Configuracao_Servico_OLTController : BaseController
    {
        public Configuracao_Servico_OLTController() : base("Configuracao_Servico_OLT",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Configuracao_Servico_OLTApi/FindById?id={0}",
                                                   "api/Configuracao_Servico_OLTApi/Gravar",
                                                   String.Empty)
        {

        }

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }


        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            var selectList = Enum.GetValues(typeof(Marca_Modelo_OLT))
                        .Cast<Marca_Modelo_OLT>()
                        .Where(e => e == Marca_Modelo_OLT.Huawei || e == Marca_Modelo_OLT.Fiberhome_A_B || e == Marca_Modelo_OLT.Fiberhome_A1 || e == Marca_Modelo_OLT.Datacom)
                        .Select(e => new SelectListItem
                        {
                            Value = ((int)e).ToString(),
                            Text = e.ToString()
                        });
            ViewBag.Modelos_OLT_Compativeis = selectList;

            return base.CadastroBase<Configuracao_Servico_OLTModel, Int64?>(id);
        }    

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Cadastro(Configuracao_Servico_OLTModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Configuracao_Servico_OLTModel, Int64>(pModel);

            return retorno;
        }
    }
}