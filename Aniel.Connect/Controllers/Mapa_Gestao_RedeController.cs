using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Mapa_Gestao_RedeController : BaseController

    {
        public Mapa_Gestao_RedeController() : base("Mapa_Gestao_Rede",
                                           CONSTANTES.URLPortalInterno

                                           )
        {

        }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        [HttpPost]
        public ActionResult Index(String latlng)
        {
            Posicao_ElementoString LatLong = JsonService.DeserializeObject<Posicao_ElementoString>(latlng);
            Posicao_Elemento posicao_Elemento = new Posicao_Elemento
            {
                latitude = Convert.ToDouble(LatLong.latitude),
                longitude = Convert.ToDouble(LatLong.longitude)

            };

            ViewBag.Areas = Areas;
            ViewBag.LatLong = LatLong;
            return View();
        }
 
    }
}