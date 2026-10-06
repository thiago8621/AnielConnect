using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ModeloMensagemController : BaseController
    {

        public ModeloMensagemController() : base("ModeloMensagem",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/ModeloMensagemapi/FindById?id={0}",
                                                   "api/ModeloMensagemapi/Gravar",
                                                   "api/ModeloMensagemapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: ModeloMensagem
        public ActionResult Index()
        {

            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
            {
                var listaCanalComunicacao = repoCanalComunicacao
                .select(p => p.Tipo_Canal_Comunicacao == Tipo_Canal_Comunicacao.Whatsapp
                         && p.WhatsAppBusinessAccount != null)
                .Result
                .ToList();

                var arrayWhatsAppBusinessAccount = listaCanalComunicacao
                    .Select(p => p.WhatsAppBusinessAccount)
                    .ToArray();

                ViewBag.Lista = arrayWhatsAppBusinessAccount;
            }

            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<ModeloMensagemCadastroModel, Int64?>(id);
            
            return retorno;
        }

        // POST: ModeloMensagem/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(ModeloMensagemCadastroModel pModel)
        {
            ViewBag.Areas = Areas;
            return base.CadastroBase<ModeloMensagemCadastroModel, Int64>(pModel);
        }

        
    }
}