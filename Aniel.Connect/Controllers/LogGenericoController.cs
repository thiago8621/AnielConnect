
using Aniel.Connect.Models;
using Resources;

using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class LogGenericoController : Controller
    {
        // GET: LogGenerico
        public ActionResult Index(LogGenericoEnum Modelo)
        {
            var obj = new LogGenericoParamListModel
            {
                DataIni = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy"),
                DataFim = DateTime.Now.ToString("dd/MM/yyyy")
            };

            switch (Modelo)
            {
                case LogGenericoEnum.Atendimento:
                    ViewBag.Title = RsCadastros.ATENDIMENTO;
                    ViewBag.Chave = "Id";
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                        //new SelectListItem{ Value = "3", Text = Resources.RsBase.EXCLUSAO },
                        new SelectListItem{ Value = "4", Text = Resources.RsBase.BAIXA },
                    };
                    break;
                case LogGenericoEnum.Franquia:
                    ViewBag.Title = RsCadastros.FRANQUIA;
                    ViewBag.Chave = "Id";
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                    };
                    break;
                case LogGenericoEnum.RotaTelefonia:
                    ViewBag.Title = RsCadastros.ROTA_TELEFONIA;
                    ViewBag.Chave = "Id";
                    
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                    };
                    break;
                case LogGenericoEnum.Did:
                    ViewBag.Chave = "Código";
                    ViewBag.Alteracao = "Tipo de Alteração";
                    ViewBag.Title = RsCadastros.DID;
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                    };
                    break;

                case LogGenericoEnum.Uf:
                    ViewBag.Title = RsCadastros.UF;
                    ViewBag.Chave = "Codigo";
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },

                    };
                    break;

                case LogGenericoEnum.Cidade:
                    ViewBag.Title = RsCadastros.CIDADE;
                    ViewBag.Chave = "Codigo";
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },

                    };
                    break;

                case LogGenericoEnum.Bairro:
                    ViewBag.Title = RsCadastros.BAIRRO;
                    ViewBag.Chave = "Codigo";
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },

                    };
                    break;

                case LogGenericoEnum.Endereco_Logradouro:
                    ViewBag.Title = RsCadastros.ENDERECO_LOGRADOURO;
                    ViewBag.Chave = "Codigo";
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },

                    };
                    break;
                    
                case LogGenericoEnum.Condominio_Predio:
                    ViewBag.Title = RsCadastros.CONDOMINIO_PREDIO;
                    ViewBag.Chave = "Codigo";
                    ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },

                    };
                    break;

                default:
                    break;
            }

            return View(obj);
        }
    }
}