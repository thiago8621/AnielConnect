using Aniel.Connect.Core;
using System;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class IntegracaoConfigIntegratorController : BaseController
    {

        public IntegracaoConfigIntegratorController() : base("IntegracaoConfigIntegrator",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/IntegracaoConfigIntegratorapi/FindById?id={0}",
                                                   "api/IntegracaoConfigIntegratorapi/Gravar",
                                                   "api/IntegracaoConfigIntegratorapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Agente
        public ActionResult Log()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public FileResult DownloadLogFile(string Name)
        {
            using (var fs = new FileStream(@$"{CONSTANTES.CaminhoLogIntegracaoConfigIntegrator}/" + Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fs, Encoding.Default))
            using (var memoryStream = new MemoryStream())
            {
                streamReader.BaseStream.CopyTo(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                return File(bytes, "application/octet-stream", Name);
            }
        }

    }
}