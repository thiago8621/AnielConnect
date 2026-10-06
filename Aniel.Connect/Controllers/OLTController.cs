using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
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
    public class OLTController : BaseController
    {

        public OLTController() : base("OLT", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/OLTapi/FindById?id={0}",
                                                   "api/OLTapi/Gravar",
                                                   "api/OLTapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: OLT
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;

            FillListasView();

            return View();
        }

    

        protected void FillListasView()
        {
            
            
            var _retornoOLT = new OLTApiController().GetListOLT().Result;

            if (!_retornoOLT.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoOLT.Mensagem, _retornoOLT.InnerException);

            ViewBag.OLT = _retornoOLT.Objeto;


            var _retornoIp = new OLTApiController().GetListIp().Result;

            if (!_retornoIp.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoIp.Mensagem, _retornoIp.InnerException);

            ViewBag.IP = _retornoIp.Objeto;


            var _retornoEmpresa = new EmpresaApiController().GetList().Result;

            if (!_retornoEmpresa.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoEmpresa.Mensagem, _retornoEmpresa.InnerException);

            ViewBag.Empresa = _retornoEmpresa.Objeto;


            var _retornoFilial = new FilialApiController().GetListVerificacaoVazio().Result;

            if (!_retornoFilial.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoFilial.Mensagem, _retornoFilial.InnerException);

            ViewBag.Filial = _retornoFilial.Objeto;


            //var _retornoVendedor = new FuncionarioApiController().GetListVendedor().Result;

            //if (!_retornoVendedor.Ok)
            //    AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoVendedor.Mensagem, _retornoVendedor.InnerException);

            //ViewBag.Vendedor = _retornoVendedor.Objeto;

        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<OLTCadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: OLT/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(OLTCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<OLTCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();            
        }        

        // GET: OLT/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<OLTCadastroModel, Int64>(id.Value);
        }

        // POST: OLT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(OLTCadastroModel pModel)
        {
            return base.DeleteBase<OLTCadastroModel, Int64>(pModel);
        }


        public async Task<ActionResult> Importacao(String Arquivo)
        {

            ExcelServiceLog<OLTImportacao> modelo = new ExcelServiceLog<OLTImportacao>
            {
                Items = new List<OLTImportacao>(),
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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(OLTImportacao)));
                        modelo = ExcelService.CastDataTableImportacao<OLTImportacao>(dt);


                        using (OLTController Ponto_Venda_api = new OLTController())
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
                                        modelo.Log.AddRange(await ImportacaoModeloOLT(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modelo.Log.Add(new ExcelServiceLogItem { Chave = item.Id.ToString(), Erro = ex.Message });
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
            ExcelServiceLog<OLTImportacao> modelo = new ExcelServiceLog<OLTImportacao>
            {
                Items = new List<OLTImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };
           
            return View(modelo);
        }
        [HttpPost()]
        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloOLT(OLTImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<OLT> repoOlt = new Conexao<OLT>(session))
            using (Conexao<POP> repoPop = new Conexao<POP>(session))
            {
                OLT olt = new OLT();
                olt.POP = new POP();
                OLT idOlt = new OLT();

                if (item.Id != 0)
                    idOlt = (await repoOlt.selectFirst(p => p.Id == item.Id));                

                if (idOlt != null)                
                    olt = idOlt;
                
                if (item.IdPOP != 0)
                    olt.POP = await repoPop.selectFirst(p => p.Id == item.IdPOP);
                
                if (Enum.IsDefined(typeof(Marca_Modelo_OLT), item.Marca_Modelo))
                   olt.Marca_Modelo = (Marca_Modelo_OLT)item.Marca_Modelo;

                if (Enum.IsDefined(typeof(ONU_Tipo_Identificacao), item.Tipo_Identificacao))                
                    olt.Tipo_Identificacao = (ONU_Tipo_Identificacao)item.Tipo_Identificacao;

                olt.Status = item.Status == false ? Status_Default.Inativo : Status_Default.Ativo;

                olt.IP = item.Ip;
                olt.IpTL1 = item.IpTL1;
                olt.Login = item.Login;
                olt.LoginTL1 = item.LoginTL1;

                olt.NumOLT = item.NumOLT;
                olt.NumOLT = item.OLTID;
                olt.PortaAPI = item.PortaAPI;
                olt.PortaHTTP = item.PortaHTTP;
                olt.PortaSSH = item.PortaSSH;
                olt.PortaTelnet = item.PortaTelnet;
                olt.PortaTL1 = item.PortaTL1;
                olt.Senha = item.Senha;
                olt.SenhaTL1  = item.SenhaTL1;
                
                olt.Subrack = item.Subrack;
                olt.VLAN_Uplink = item.VLAN_Uplink;
                olt.CodigoAuxiliar = item.CodigoAuxiliar;
                olt.Descricao = item.Descricao;
               
                await repoOlt.SaveOrUpdate(olt);
            }

            return retorno;
        }

    }

}
