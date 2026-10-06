using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Controllers;
using Sinapse.DAO.repository;
using Aniel.T5.Model.nhibernate;
using Aniel.Connect.ControllersWebApi.T5;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Area_CoberturaController : BaseController
    {
        public Area_CoberturaController() : base("Area_Cobertura",
                                                CONSTANTES.URLPortalInterno,
                                                "api/Area_CoberturaApi/GetAll",
                                                "api/Area_CoberturaApi/FindById?Id={0}",
                                                "api/Area_CoberturaApi/Gravar",
                                                String.Empty)

        {

        }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? Id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Area_CoberturaCadastroModel, Int64?>(Id);
            return retorno;
        }
        [HttpPost()]
        public ActionResult Cadastro(Area_CoberturaCadastroModel request)
        {

            return CadastroArea<Area_CoberturaCadastroModel, Int64>(request);
        }
        [HttpGet()]
        public ActionResult MapaCobertura(Int64? Id)
        {
            using (Area_CoberturaApicontroller area = new Area_CoberturaApicontroller()) {

                var AreaSelecionada = area.FindById(Id).Result;
                ViewBag.Cor = AreaSelecionada.Objeto.Cor;
                ViewBag.Id_Area_Cobertura = Id;
                var resultado = area.FindAreas().Result;
                
                return View(resultado.Objeto);
            }
                
        }
        [HttpPost()]
        public ActionResult MapaCobertura(Mapa_Area_CoberturaModel request)
        {
            return base.CadastroBase<Mapa_Area_CoberturaModel, Int64>(request);
        }

        public ActionResult CadastroArea<T, K>(T pModel)
        {
            //try
            //{
            //    SetTokenByUser();
            //}
            //catch
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            try
            {
                if (ModelState.IsValid && Validacoes(pModel))
                {
                    PostApiRetorno<Int64> retorno = PostByUrl<T, Int64>(pModel, UrlPortal, UrlPost);

                    //Verifica se esta nulo e se é diferente do valor default
                    //if (!EqualityComparer<K>.Default.Equals(retorno.IdObjeto, default(K))) //Edição
                    if (retorno.Ok) //Edição
                    {
                        using (var session = HibernateUtil.GetSessionT5())
                        using (var repo = new Conexao<Area_Cobertura>(session))
                        {

                            var area = repo.selectFirst(p => p.Id == retorno.IdObjeto).Result;
                            if (String.IsNullOrEmpty(area.Coordenadas) || area.Coordenadas == "[]")
                            {
                                return RedirectToAction("MapaCobertura", new { Id = retorno.IdObjeto });
                            }
                            else {
                                return RedirectToAction("Index");
                            }
                        }

                    }
                    else
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                    }

                }


            }
            catch (Exception ex)
            {
                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }
            return View();
        }

    }
}