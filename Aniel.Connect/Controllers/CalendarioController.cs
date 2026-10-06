using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class CalendarioController : BaseController
    {

        public CalendarioController() : base("Calendario",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Calendarioapi/FindById?id={0}",
                                                   "api/Calendarioapi/Gravar",
                                                   "api/Calendarioapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Calendario
        public ActionResult Index()
        {
            return base.CadastroBase<CalendarioCadastroModel, Int64?>(1);
        }

        [HttpPost]
        public ActionResult Index(CalendarioCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<CalendarioCadastroModel, Int64>(pModel);

            return retorno;
        }
    }
}