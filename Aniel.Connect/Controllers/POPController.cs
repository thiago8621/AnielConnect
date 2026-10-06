using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class POPController : BaseController
    {
        public POPController() : base("POP",
                                       CONSTANTES.URLPortalInterno,
                                       String.Empty,
                                       "api/POPapi/FindById?id={0}",
                                       "api/POPapi/Gravar",
                                       String.Empty,
                                       String.Empty)
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Cadastro(Int64? Id)
        {
            return base.CadastroBase<POPCadastroModel, Int64?>(Id);
        }

        [HttpPost]
        public ActionResult Cadastro(POPCadastroModel model)
        {
            return base.CadastroBase<POPCadastroModel, Int64>(model);
        }

        public async Task<ActionResult> Importacao(String Arquivo)
        {

            ExcelServiceLog<POPImportacao> modelo = new ExcelServiceLog<POPImportacao>
            {
                Items = new List<POPImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };



            try
            {
                if (!(Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName)))
                {
                    AddModelError("Informe Planilha a ser Importada");
                    return View();
                }

                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                    {

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(POPImportacao)));
                        modelo = ExcelService.CastDataTableImportacao<POPImportacao>(dt);


                        using (POPController Ponto_Venda_api = new POPController())
                        {
                            if (dt.Rows.Count == 0)
                            {
                                throw new Exception("A planilha carregada está vazia!");
                            }

                            if (dt.Rows.Count > 0)
                            {
                                foreach (var item in modelo.Items)
                                {
                                    try
                                    {
                                        modelo.Log.AddRange(await ImportacaoModeloPOP(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modelo.Log.Add(new ExcelServiceLogItem { Chave = item.ID.ToString(), Erro = ex.Message });
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }

                            }
                        }
                    }

                    modelo.Log.AddRange(modelo.Log);
                    ViewBag.Sucesso = modelo.Log.Count == 0 ? "S" : "N";
                    return View(modelo);

                }
                else
                {
                    return View(modelo);
                }

            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(modelo);
            }
        }

        [HttpGet()]
        public ActionResult Importacao()
        {
            ExcelServiceLog<POPImportacao> modelo = new ExcelServiceLog<POPImportacao>
            {
                Items = new List<POPImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };
            return View(modelo);
        }
        [HttpPost()]
        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloPOP(POPImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<POP> repoPop = new Conexao<POP>(session))
            using (Conexao<POP> pop_obj = new Conexao<POP>(session))
            using (Conexao<Cidade> repoCidade = new Conexao<Cidade>(session))
            using (Conexao<Pais> repoPais = new Conexao<Pais>(session))
            using (Conexao<Empresa> repoEmpresa = new Conexao<Empresa>(session))
            using (Conexao<Filial> repoFilial = new Conexao<Filial>(session))
            {
                POP pop = new POP();

                POP idPop = new POP();
                if (item.ID != 0)
                {
                    idPop = (await repoPop.selectFirst(p => p.Id == item.ID));
                }
                if (idPop != null)
                {
                    pop.Id = idPop.Id;
                }

                pop.Projeto = item.PROJETO;

                pop.Pais = item.PAIS;
                pop.IdPais = (await repoPais.selectFirst(p => p.Nome.IsInsensitiveLike(item.PAIS)))?.Id;
                pop.Nome = item.NOME;
                pop.Bairro = item.BAIRRO;
                pop.Cep = item.CEP;
                pop.Endereco = item.ENDERECO;
                pop.Numero = item.NUMERO;
                pop.Projeto = item.PROJETO;
                pop.Cidade = item.CIDADE;
                pop.IdCidade = (await repoCidade.selectFirst(p => p.Descricao.IsInsensitiveLike(item.CIDADE)))?.Id;
                pop.CodCt = item.CODCT;
                pop.Complemento = item.COMPLEMENTO;
                pop.Latitude = Convert.ToDouble(item.LATITUDE);
                pop.Longitude = Convert.ToDouble(item.LONGITUDE);
                pop.Empresa = await repoEmpresa.selectFirst(p => p.Id == item.ID_EMPRESA);
                pop.Filial = await repoFilial.selectFirst(p => p.Id == item.ID_FILIAL);

                if (item.STATUS == false)
                {

                    pop.Status = Status_Default.Inativo;

                }
                else
                {
                    pop.Status = Status_Default.Ativo;
                }

                pop.UF = item.UF;
                
                await pop_obj.SaveOrUpdate(pop);

            }

            return retorno;
        }

    }

}