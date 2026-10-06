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
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.repository;
using System.IO;
using System.Threading.Tasks;
using Sinapse.Util;
using Sinapse.Model;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Tipo_Elemento_IconeController : BaseController
    {
        public Tipo_Elemento_IconeController() : base("Tipo_Elemento_Icone",
                                                      CONSTANTES.URLPortalInterno,
                                                      "api/Tipo_Elemento_IconeApi/GetAll",
                                                      "Tipo_Elemento_Icone/Gravar_Icone",
                                                      String.Empty,
                                                      String.Empty)
        {
        }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        [HttpPost]
        public ActionResult Gravar_Icon(Tipo_ElementoIconesViewModel model) 
        {
            return View();
        }

        [HttpPost]
        public async Task<String> Gravar_Icone(Tipo_ElementoIconesViewModel model)
        {
            //Request;
            var Arq = Request.Params["File_Json"];
            var ArqJson = Request.Params["Posted_Arquivo_JSON"];
            var Path = Request.Params["Path"];
      
            Tipo_ElementoIconesViewModel parsed = new Tipo_ElementoIconesViewModel();
            Tipo_ElementoIcone icon = null;


            //Json
            if (!String.IsNullOrEmpty(model.File_Json))
            {
                List<Tipo_ElementoIconeViewModel> parse = JsonService.DeserializeObject<List<Tipo_ElementoIconeViewModel>>(model.File_Json);
                //List<String> paths = JsonService.DeserializeObject<List<String>>(Request.Params["Path"]);
                //for(var i=0;i<parse.Count();i++)
                //parse[i].Caminho = paths[i];
                parsed.Files = parse;
            }
               
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();
            try
            {

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoIcones = new Conexao<Tipo_ElementoIcone>(sessionT5))
                {

                    var pasta = @$"{CONSTANTES.CaminhoFisico}\Tmp\Icones";
                    if (!Directory.Exists(pasta))
                    {
                        Directory.CreateDirectory(pasta);
                    }
                    foreach (var arquivo in parsed.Files)
                    {
                        icon = await repoIcones.selectFirst(p => p.Id == arquivo.Id || p.Descricao == arquivo.Descricao);
                        if (icon == null)
                        {
                            icon = new Tipo_ElementoIcone
                            {
                                DataHoraCriacao = DateTime.Now
                            };
                            
                        }
                        BindIcone(arquivo, icon);

                        for (var i = 0; i < Request.Files.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(Request.Files[i].FileName))
                            {
                                var xmltoString = new System.Xml.XmlDocument();

                                byte[] allBytes = new byte[Request.Files[i].InputStream.Length];
                                int bytesRead = Request.Files[i].InputStream.Read(allBytes, 0, (int)Request.Files[i].InputStream.Length);
                                String _arquivo = _arquivo = $@"{pasta}\{arquivo.Descricao}.{arquivo.Tipo}";
                                System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                                icon.Arquivo = new StreamReader(new MemoryStream(allBytes)).ReadToEnd();
                                xmltoString.LoadXml(icon.Arquivo);
                                xmltoString.GetElementsByTagName("path");

                                foreach (System.Xml.XmlNode x in xmltoString.GetElementsByTagName("path"))
                                {
                                    if (!String.IsNullOrEmpty(x.Attributes.GetNamedItem("d").Value))
                                        icon.Caminho = x.Attributes.GetNamedItem("d").Value;

                                    break;
                                }                            
                                icon.Url = _arquivo;

                            }
                        }
                        await repoIcones.SaveOrUpdate(icon);
                        retorno.Mensagem = "OK";
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.IdObjeto = null;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return JsonService.SerializeObject(retorno);
        }
     
        public void BindIcone(Tipo_ElementoIconeViewModel request, Tipo_ElementoIcone icon)
        {
        
            icon.Descricao = request.Descricao;
            icon.Tipo = request.Tipo;
            icon.Caminho = request.Caminho;
        }
    }
}
