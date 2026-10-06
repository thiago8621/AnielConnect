using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class TipoTarefaController : BaseController
    {

        public TipoTarefaController() : base("TipoTarefa",
                                                  CONSTANTES.URLPortalInterno,
                                                  String.Empty,
                                                  "api/TipoTarefaApi/FindById?id={0}",
                                                  "api/TipoTarefaApi/Gravar",
                                                  "api/TipoTarefaApi/Excluir",
                                                  String.Empty)
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<TipoTarefaCadastroModel, Int64?>(id);
            return retorno;
        }

        [HttpPost]
        public ActionResult Cadastro(TipoTarefaCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<TipoTarefaCadastroModel, Int64>(pModel);
            return retorno;
        }


    }


}