using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ViabilidadeController : BaseController
    {
        public ViabilidadeController() : base("Viabilidade",
                                               CONSTANTES.URLPortalInterno
                                              )
        {


        }

        [HttpGet()]
        public ActionResult Index(double? latitude,
                                  double? longitude,
                                  long? idVenda,
                                  long? idClientePlanoProduto,
                                  long? idCliente,
                                  long? idEnderecoLogradouro,
                                  long? idCondominio,
                                  string numero,
                                  string logradouro,
                                  long? idCidade)
        {
            ViewBag.Areas = Areas;
            var Model = new ViabilidadeResponse();

            using (var api = new Consulta_ViabilidadeApiController()) 
            {
                if (latitude.HasValue && longitude.HasValue) 
                {
                    var retorno = api.GetDadosViabilidadeByLatLong(Convert.ToDouble(string.Format("{0:F7}", latitude)),
                                                                   Convert.ToDouble(string.Format("{0:F7}", longitude)),
                                                                   idVenda,
                                                                   idClientePlanoProduto,
                                                                   idCliente,
                                                                   idEnderecoLogradouro,
                                                                   idCondominio,
                                                                   numero,
                                                                   logradouro,
                                                                   idCidade);
                    if (retorno.Ok && retorno.Objeto != null) 
                    {
                        Model.PossuiDisponibilidade = retorno.Objeto.PossuiDisponibilidade;
                        Model.Longitude = retorno.Objeto.Longitude;
                        Model.Latitude = retorno.Objeto.Latitude;
                        Model.Motivos = retorno.Objeto.Motivos;
                        Model.Caixas = retorno.Objeto.Caixas;
                        Model.IdConsultaViabilidade = retorno.Objeto.IdConsultaViabilidade;
                        Model.Passos = retorno.Objeto.Passos;
                        Model.ClientePlanoProduto = idClientePlanoProduto;
                    }
                }
            }

            return View(Model);
        }

        [HttpPost()]
        public String MoverObjeto(String ObjJson)
        {
            var obj_venda = Request.Params["objeto_da_Venda"];
            //var JFilho = new JsonObj();
            //JFilho.ObjJson = x;

            return obj_venda;
        }

        #region
        //[HttpPost()]
        //public ActionResult Index(Mapa_Analise_ComercialListModel request)
        //{
        //    return base.CadastroBase<Mapa_Analise_ComercialListModel, Int64>(request);
        //}
        //[HttpPost()]
        //public ActionResult mover_Objeto()
        //{

        //    //Index(x);
        //    //RedirectToAction("Index",new {ObjJson = x });
        //    //var retorno = new GetApiRetorno<Object>();
        //    ViewResult retorno = (ViewResult)base.CadastroBase<JsonObj, String>(x);
        //    return View();


        //}

        //[HttpGet()]
        //public ActionResult Index(String obj)
        //{
        //    ViewBag.Areas = Areas;
        //    var JFilho = new JsonObj();
        //    JFilho.ObjJson = obj;

        //    return View(JFilho);
        //}
        #endregion
    }
}
