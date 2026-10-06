using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
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
    public class MapaController : BaseController
    {
        public MapaController(): base ("Mapa", CONSTANTES.URLPortalInterno)
        {

        }

        // GET: Mapa
        public ActionResult Endereco(String Endereco,String Latitude,String Longitude)
        {
            return View(new MapaModelView
            {
                Endereco = Endereco
            });
        }

       
        public async Task<ActionResult> Comercial(Int64 Id_Vendedor, String Data)
        {
            //List<Comercial_MapaModel> retorno = new List<Comercial_MapaModel>();
            Comercial_MapaModel retorno = new Comercial_MapaModel();
            try
            { 
                if (String.IsNullOrEmpty(Data))
                    Data = DateTime.Now.ToString("dd/MM/yyyy");

                retorno = await new MapaApiController().GetComercial(Id_Vendedor, Data);
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(retorno);

                //AddModelError("Meta não pode ser igual a do supervisor");
                //return View();
            }

            return View(retorno);
        }


        public async Task<ActionResult> Equipes(String Num_Fre_Supervisor, String Id_Monitor, String Equipe, String Cod_Escritorio, String Cod_Area, Int32? Cod_Localidade,
            Int64? Id_Natureza,String Endereco)
        {

            Equipe_MapaModel retorno = new Equipe_MapaModel
            {
                Itens = new List<Equipe_MapaItemModel>()
            };
            try
            {
                try
                {
                    retorno.Itens = await new MapaApiController().GetEquipes(Num_Fre_Supervisor, Id_Monitor, Equipe, Cod_Escritorio, Cod_Area, Cod_Localidade, Id_Natureza);
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message);
                }

                ViewBag.Areas = this.Areas;

                ViewBag.Natureza = new Natureza_ProducaoApiController().GetList().Result.Objeto;

                retorno.Endereco = Endereco;
                retorno.Cod_Area = Cod_Area;
                retorno.Cod_Escritorio = Cod_Escritorio;
                retorno.Cod_Localidade = Cod_Localidade;
                retorno.Equipe = Equipe;
                retorno.Id_Monitor = Id_Monitor;
                retorno.Id_Natureza = Id_Natureza;
                retorno.Num_Fre_Supervisor = Num_Fre_Supervisor;

                return View(retorno);
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
            }

            return View(retorno);
        }

        [HttpPost]
        public ActionResult Equipes(Equipe_MapaModel Model)
        {
            return RedirectToAction("Equipes", new { Model.Num_Fre_Supervisor, Model.Id_Monitor, Equipe = Model.Equipe, Model.Cod_Escritorio, Model.Cod_Area, Model.Cod_Localidade, Model.Id_Natureza, Model.Endereco });
        }
    }
}