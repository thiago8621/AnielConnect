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
    [Authorize]
    public class Historico_NfController : BaseController
    {

        public Historico_NfController() : base("Historico_Nf",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Historico_Nfapi/FindById?id={0}",
                                                   "api/Historico_Nfapi/Gravar",
                                                   "api/Historico_Nfapi/Excluir",
                                                   String.Empty)
        {

        }
        
        public ActionResult Index(Int64 Id_Cliente)
        {

            return View(new Historico_NfViewModel {

                Data_Fim = DateTime.Now.ToString("dd/MM/yyyy"),
                Data_Ini = DateTime.Now.AddDays(-90).ToString("dd/MM/yyyy")
            });
        }

    }
}