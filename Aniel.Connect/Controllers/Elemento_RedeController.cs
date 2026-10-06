using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elemento_RedeFotosArquivos = Sinapse.Model.Elemento_RedeFotosArquivos;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Elemento_RedeController : BaseController
    {


        public Elemento_RedeController() : base("Elemento_Rede",
                                                CONSTANTES.URLPortalInterno,
                                                "api/Elemento_RedeApi/GetAll",
                                                "api/Elemento_RedeApi/FindById?Id={0}",
                                                "api/Elemento_RedeApi/Gravar",
                                                String.Empty)
        {

        }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        //public ActionResult Cadastro()
        //{
        //    ViewBag.Areas = Areas;
        //    return View();
        //}
        [HttpGet]
        public ActionResult Cadastro(Int64? Id, Int64? IdPoste)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Elemento_RedeCadastroModel, Int64?>(Id);

            if (IdPoste.HasValue)
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var repo = new Conexao<Elemento_Rede>(session))
                {
                    var posteObj = repo.selectFirst(p => p.Id == IdPoste).Result;
                    var model = (Elemento_RedeCadastroModel)retorno.Model;
                    model.Poste = IdPoste;
                    model.Latitude = posteObj.Latitude;
                    model.Longitude = posteObj.Longitude;
                    model.Contrato = posteObj.Contrato;
                    model.Projeto = posteObj.Projeto;
                    model.Id_POP = posteObj.Id_POP;
                    model.Endereco = posteObj.Endereco;
                    model.Numero = posteObj.Numero;
                    model.Cep = posteObj.CEP;
                    model.Bairro = posteObj.Bairro;
                    model.Pais = posteObj.Pais;
                    model.Cidade = posteObj.Cidade;
                    model.Complemento = posteObj.Complemento;
                    model.Id = Id;
                }
            }

            return retorno;
        }



        [HttpGet()]
        public ActionResult Importacao()
        {
            ExcelServiceLog<Elemento_Rede_Importacao> modelo = new ExcelServiceLog<Elemento_Rede_Importacao>
            {
                Items = new List<Elemento_Rede_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }


        public async Task<ActionResult> Importacao(String Arquivo)
        {

            ExcelServiceLog<Elemento_Rede_Importacao> modelo = new ExcelServiceLog<Elemento_Rede_Importacao>
            {
                Items = new List<Elemento_Rede_Importacao>(),
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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Elemento_Rede_Importacao)));
                        modelo = ExcelService.CastDataTableImportacao<Elemento_Rede_Importacao>(dt);


                        using (Elemento_RedeApiController Ponto_Venda_api = new Elemento_RedeApiController())
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
                                        modelo.Log.AddRange(await ImportacaoModeloElementoRede(item));
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



        [HttpPost()]
        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloElementoRede(Elemento_Rede_Importacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSessionT5())
            using (Conexao<Elemento_Rede> repoElementoRede = new Conexao<Elemento_Rede>(session))
            using (Conexao<Tipo_Elemento_Rede> tipoElementoRedeRepo = new Conexao<Tipo_Elemento_Rede>(session))
            {
                Elemento_Rede elementoRede = new Elemento_Rede();

                Elemento_Rede idElementoRede = new Elemento_Rede();
                if (item.Id != 0)
                {
                    idElementoRede = (await repoElementoRede.selectFirst(p => p.Id == item.Id));
                }
                if (idElementoRede != null)
                {
                    elementoRede = idElementoRede;
                }

                elementoRede.Tipo = await tipoElementoRedeRepo.selectFirst(p => p.Id == item.IdTipo);
                if(elementoRede.Tipo == null)
                {
                    throw new Exception("Tipo de elemento de rede não encontrado");
                }


                elementoRede.Modelo = elementoRede.Tipo?.Modelo;
                elementoRede.Cabo = await repoElementoRede.selectFirst(p => p.Id == item.IdCabo);
                elementoRede.Caixa = await repoElementoRede.selectFirst(p => p.Id == item.IdCaixa);
                elementoRede.Poste = await repoElementoRede.selectFirst(p => p.Id == item.IdPoste);

                elementoRede.Bairro = item.Bairro;
                elementoRede.Pais = item.Pais;
                elementoRede.Cod_Caixa = item.Cod_Caixa;
                elementoRede.Capacidade = item.Capacidade;
                elementoRede.CEP = item.CEP;
                elementoRede.Certificado = item.Certificado;
                elementoRede.Cidade = item.Cidade;
                elementoRede.Complemento = item.Complemento;
                elementoRede.Contrato = item.Contrato;
                elementoRede.Cor_linha_Ativa = item.Cor_linha_Ativa;
                elementoRede.Cor_linha_Inativa = item.Cor_linha_Inativa;
                elementoRede.Descricao = item.Descricao;
                elementoRede.Endereco = item.Endereco;
                elementoRede.Id_POP = item.Id_POP;
                elementoRede.Id_OLT = item.Id_OLT;
                elementoRede.Id_Slot = item.Id_Slot;
                elementoRede.Id_Pon = item.Id_Pon;
                elementoRede.Latitude = item.Latitude;
                elementoRede.Longitude = item.Longitude;
                elementoRede.Numero = item.Numero;
                elementoRede.Observacao = item.Observacao;
                elementoRede.Projeto = item.Projeto;

                if (Enum.IsDefined(typeof(Status_Elemento), item.Situacao))
                {
                    elementoRede.Situacao = (Status_Elemento)item.Situacao;
                }


                
                elementoRede.UF = item.UF;
                elementoRede.Coordenadas = item.Coordenadas;
                elementoRede.AreaAtendimento = item.AreaAtendimento;
                elementoRede.Num_Coordenadas = item.Num_Coordenadas;
                elementoRede.Metragem = item.Metragem;

                if (Enum.IsDefined(typeof(Padrao_Cor), item.Padrao_Cores))
                {
                    elementoRede.Padrao_Cores = (Padrao_Cor)item.Padrao_Cores;
                }


                
                await repoElementoRede.SaveOrUpdate(elementoRede);

                return retorno;
            }
        }




        [HttpPost]
        public async Task<ActionResult> Cadastro(Elemento_RedeCadastroModel model)
        {
            ActionResult retorno = base.CadastroBase<Elemento_RedeCadastroModel, Int64?>(model);

            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                    using (var repo = new Conexao<Elemento_RedeFotosArquivos>(sessionT5))
                    {
                        List<Elemento_RedeFotosArquivos> elements = new List<Elemento_RedeFotosArquivos>();
                        var element = await repo.selectFirst(p => p.Id == model.Id);


                        var _pasta = PdfLocation;
                        if (!Directory.Exists(_pasta))
                        {
                            Directory.CreateDirectory($"{_pasta}");

                        }

                        foreach (var arquivo in model.Arquivos)
                        {
                            for (var i = 0; i < Request.Files.Count; i++)
                            {
                                if (!String.IsNullOrEmpty(Request.Files[i].FileName))
                                {
                                    if (arquivo.Descricao + arquivo.Tipo == Request.Files[i].FileName)
                                    {
                                        byte[] allBytes = new byte[Request.Files[i].InputStream.Length];
                                        int bytesRead = Request.Files[i].InputStream.Read(allBytes, 0, (int)Request.Files[i].InputStream.Length);
                                        String _arquivo = _arquivo = $@"{_pasta}\{arquivo.Id}{arquivo.Tipo}";
                                        System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                                    }
                                }
                            }


                        }
                    }
                }
            }
            catch (Exception erro)
            {
                _ = erro.Message;
            }

            return retorno;
        }
        [HttpGet()]
        public async Task<ActionResult> DiagramaFusao(Int64 Id)
        {
            //using (ISession session = HibernateUtil.GetSession())

            DiagramaFusaoConexoesModel retorno = null;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoElemento = new Conexao<Elemento_Rede>(sessionT5))

            using (var Api = new Elemento_RedeApiController())
            {
                retorno = (await Api.Ellaborate_DiagramaFusao(Id))?.Objeto;

            }
            return View(retorno);
        }

        [HttpPost()]
        public async Task<ActionResult> DiagramaFusao(Diagrama_FusaoSetFile model)
        {

            using (var api = new Elemento_RedeApiController())
            {
                await api.SaveDiagram(model);
            }

            return DiagramaFusao(model.Id).Result;
        }

        private static String PdfLocation => $"{CONSTANTES.CaminhoFisico}/Tmp/Elemento_de_Rede";
    }



}
