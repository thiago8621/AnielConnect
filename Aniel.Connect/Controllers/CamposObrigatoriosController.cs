using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver.Core.Clusters.ServerSelectors;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class CamposObrigatoriosController : BaseController
    {

        public CamposObrigatoriosController() : base("CamposObrigatorios", CONSTANTES.URLPortalInterno)
        {

        }

        public ActionResult Index()
        {
            var Obrigatorios = new CamposObrigatoriosListModel();

            using (var apiCliente = new CamposObrigatoriosClienteApiController())
            using (var apiOS = new CamposObrigatoriosDocumentoProducaoApiController())
            using (var apiAtendimento = new CamposObrigatoriosAtendimentoApiController())
            using (var apiLead = new CamposObrigatoriosLeadApiController())
            using (var apiVenda = new CamposObrigatoriosVendaApiController())
            {

                Obrigatorios.Campos_Venda = apiVenda.GetCampos().Result.Select(p => new CamposObrigatoriosVendaModels
                {
                    Id = p.Id,
                    Campo = p.Campo,
                    CampoString = p.CampoString,
                    Obrigatorio = p.Obrigatorio,
                    Bloqueado = p.Bloqueado,
                }).ToList();

                Obrigatorios.Campos_Atendimento = apiAtendimento.GetCampos().Result.Select(p => new CamposObrigatoriosAtendimentoModels
                {
                    Id = p.Id,
                    Campo = p.Campo,
                    CampoString = p.CampoString,
                    Obrigatorio = p.Obrigatorio,
                    Bloqueado = p.Bloqueado,
                }).ToList();

                Obrigatorios.Campos_Documento_Producao = apiOS.GetCampos().Result.Select(p => new CamposObrigatoriosDocumento_ProducaoModels
                {
                    Id = p.Id,
                    Campo = p.Campo,
                    CampoString = p.CampoString,
                    Obrigatorio = p.Obrigatorio,
                    Bloqueado = p.Bloqueado,
                    Titulo = p.Titulo
                }).ToList();

                Obrigatorios.Campos_Lead = apiLead.GetCampos().Result.Select(p => new CamposObrigatoriosLeadModels
                {
                    Id = p.Id,
                    Campo = p.Campo,
                    CampoString = p.CampoString,
                    Obrigatorio = p.Obrigatorio,
                    Bloqueado = p.Bloqueado,
                }).ToList();

                Obrigatorios.Campos_Cliente = apiCliente.GetCampos().Result.Select(p => new CamposObrigatoriosClienteModels
                {
                    Id = p.Id,
                    Campo = p.Campo,
                    CampoString = p.CampoString,
                    Obrigatorio = p.Obrigatorio,
                    Bloqueado = p.Bloqueado,
                }).ToList();
            }

            return View(Obrigatorios);
        }

        [HttpPost()]
        public async Task<ActionResult> Index(CamposObrigatoriosListModel Model)
        {
            //TODO Review ViewBag Campos Obrigatorios
            //List<CamposObrigatoriosClienteModels> campobrigatorios = new CamposObrigatoriosClienteApiController().GetCampos().Result;
            //ViewBag.CamposObrigatorios = campobrigatorios;

            var rotinas = new List<PostApiRetorno<long>>();

            //Sessions
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            //Transactions
            using (ITransaction transConnect = session.BeginTransaction())
            using (ITransaction transT5 = sessionT5.BeginTransaction())
            //Repositories                
            using (var apiCliente = new CamposObrigatoriosClienteApiController())
            using (var apiOS = new CamposObrigatoriosDocumentoProducaoApiController())
            using (var apiAtendimento = new CamposObrigatoriosAtendimentoApiController())
            using (var apiLead = new CamposObrigatoriosLeadApiController())
            using (var apiVenda = new CamposObrigatoriosVendaApiController())
            {
                try
                {
                    //TODO acertar rotina de gravação em cada api de Campos Obrigatórios
                    rotinas.Add(await apiCliente.Gravar(Model, session));
                    rotinas.Add(await apiAtendimento.Gravar(Model, session));
                    rotinas.Add(await apiOS.Gravar(Model, sessionT5));
                    rotinas.Add(await apiVenda.Gravar(Model, session));
                    rotinas.Add(await apiLead.Gravar(Model, session));

                    if (rotinas.Any(p => !p.Ok))
                    {
                        var mensagens = rotinas.Where(retorno => !retorno.Ok).Select(retorno => retorno.Mensagem);

                        //Rollback das Transactions
                        transConnect.Rollback();
                        transT5.Rollback();
                        //End Rollback

                        AddModelError(mensagens);
                    }
                    else
                    {
                        transConnect.Commit();
                        transT5.Commit();
                    }

                }
                catch
                {
                    //                          ----        ProTip      ----
                    //      ----    Use Shift+F9 para capturar a exceção caso precise       -----

                    transConnect.Rollback();
                    transT5.Rollback();
                }
            }

            return Index();
        }
        public static Dictionary<string, bool> dictCamposBloqueados(List<ICamposObrigatoriosModel> campos)
        {
            var fields = new Dictionary<string, bool>();
            var camposFiltered = campos.Where(p => p.CampoString != RsCadastros.STRING_VAZIA);
            foreach (var item in camposFiltered.Where(p => p.Bloqueado))
            {
                if (fields.ContainsKey(item.CampoString) == false)
                {
                    fields.Add(item.CampoString, item.Bloqueado);
                }
            }

            return fields;
        }

        public static Dictionary<string, bool> dictCamposObrigatorios(List<ICamposObrigatoriosModel> campos)
        {
            var fields = new Dictionary<string, bool>();
            var camposFiltered = campos.Where(p => p.CampoString != RsCadastros.STRING_VAZIA);
            foreach (var item in camposFiltered.Where(p => p.Obrigatorio))
            {
                if (fields.ContainsKey(item.CampoString) == false)
                {
                    fields.Add(item.CampoString, item.Bloqueado);
                }
            }

            return fields;
        }

        public static Dictionary<string, string> dictCamposTitulo(List<CamposObrigatoriosDocumento_ProducaoModels> campos)
        {
            var fields = new Dictionary<string, string>();
            var camposFiltered = campos.Where(p => p.CampoString != RsCadastros.STRING_VAZIA);
            foreach (var item in camposFiltered)
            {
                if (fields.ContainsKey(item.CampoString) == false)
                {
                    fields.Add(item.CampoString, item.Titulo);
                }
            }

            return fields;
        }

    }
}