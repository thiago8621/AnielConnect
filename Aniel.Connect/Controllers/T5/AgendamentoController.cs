using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Microsoft.AspNet.Identity;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class AgendamentoController : BaseController
    {
        public AgendamentoController() : base("Agendamento",
                                              CONSTANTES.URLPortalInterno)
        {

        }

        public async Task<ActionResult> Index(String CodCt, String Projeto, String Num_Obra, Int32 Num_Doc, String Pagina, String DataInicial)
        {
            AgendamentoCadastroModel retorno = new AgendamentoCadastroModel
            {
                CodCt = CodCt,
                Projeto = Projeto,
                Num_Obra = Num_Obra,
                Num_Doc = Num_Doc,
                Pagina = Pagina,
                Datas = new List<ObjBusca<String>>(),
                Horas = new List<ObjBusca<String>>(),
                Objetos = new List<AgendamentoDiaModel>(),
                Documento = new Models.T5.DocumentoProducaoModel(),
                Data = String.IsNullOrEmpty(DataInicial) ? DateTime.Today.ToString("dd/MM/yyyy") : DataInicial
            };

            try
            {
                using (var configuracoesApi = new Configuracoes_GeraisApiController())
                using (var acompanhamento_api = new Acompanhamento_ServicoApiController())
                {
                    var configuracoes = await configuracoesApi.FindById(1);
                    ViewBag.UtilizaRoteirizacao = configuracoes.Objeto.Empresa_Roteirizacao_Servico;
                    ViewBag.Considerar_Agendamento_Periodo = configuracoes.Objeto.Considerar_Agendamento_Periodo;
                    ViewBag.Tempo_Min = configuracoes.Objeto.TempoMinimoPrimeiraJanela;
                    retorno.ConfirmaEndereco = configuracoes.Objeto.ConfirmarEndereco;
                    ViewBag.PermitirMotivoReagendamento = Convert.ToBoolean(configuracoes.Objeto.Permite_Motivo_Reagendamento);

                    Num_Obra = await acompanhamento_api.getOsReaberta(Num_Obra, CodCt, Projeto, Num_Doc);

                    var dadosGerais = (await acompanhamento_api.GetDadosGerais_Cliente(CodCt, Projeto, Num_Obra, Num_Doc)).Objeto;

                    retorno.Documento.Cod_Localidade = Convert.ToInt32(dadosGerais.Cod_Localidade);
                    retorno.Documento.Cod_Area = dadosGerais.Cod_Area;
                    retorno.Documento.Cod_Grupo_Area = dadosGerais.Cod_Grupo_Area;
                    retorno.Documento.Endereco_Cidade = dadosGerais.Cidade;
                    retorno.Documento.Endereco_UF = dadosGerais.Uf;
                    retorno.Documento.Endereco_Pais = dadosGerais.Pais;
                    retorno.Documento.Endereco_Logradouro = dadosGerais.Logradouro;
                    retorno.Documento.Endereco_Numero = dadosGerais.Numero;
                    retorno.Documento.Id_Tipo_Imovel = dadosGerais.Cod_Tipo_Imovel;
                    retorno.Documento.Endereco_CEP = dadosGerais.Cep;
                    retorno.Documento.Endereco_Complemento = dadosGerais.Complemento;
                    retorno.Documento.Endereco_PontoReferencia = dadosGerais.Ponto_Referencia;
                    retorno.Documento.Endereco_Latitude = dadosGerais.Latitude_Str;
                    retorno.Documento.Endereco_Longitude = dadosGerais.Longitude_Str;



                    if (String.IsNullOrEmpty(Num_Obra))
                    {
                        AddModelError(RsCadastros.SERVICO_NAO_ENCONTRADO);
                    }
                    else
                    {
                        var documentos = await new Documento_ProducaoApiController().GetDocumentoAgendamento(CodCt, Projeto, Num_Obra, Num_Doc);

                        if (documentos.Count() == 0)
                        {
                            AddModelError("Documento não encontrado");

                            return View(retorno);
                        }
                        var documento = documentos.FirstOrDefault();

                        if (documento.Status_OS == HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Fechada_Produtiva) ||
                            documento.Status_OS == HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Fechada_Improdutiva) ||
                            documento.Status_OS == HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Atendimento_Finalizado))
                        {
                            throw new Exception("Não será possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                        }

                        //if(configuracoes.Objeto.Exige_LatLong_Agendamento)
                        //{
                        //    if(String.IsNullOrEmpty(documento.Endereco_Latitude) || String.IsNullOrEmpty(documento.Endereco_Longitude))
                        //    {
                        //        throw new Exception("Latitude e/ou Longitude não informados na OS.");
                        //    }
                        //}

                        ViewBag.Areas = this.Areas;
                        retorno.Documento = documentos.FirstOrDefault();

                        double? tempoExecucaoEmMinutos = documento.TempoExecucao.HasValue ? documento.TempoExecucao.Value.TimeOfDay.TotalMinutes : (double?)null;
                        var retornoApi = await new Agendamento_EquipeApiController().GetDatasDisponiveis(documento.Cod_Tipo_Serv, documento.Cod_Natureza, documento.Cod_Grupo_Area,
                                                                                                         documento.Cod_Area, documento.Cod_Localidade, documento.Localidade, tempoExecucaoEmMinutos, String.IsNullOrEmpty(DataInicial) ? DateTime.Today : Convert.ToDateTime(DataInicial));

                        retorno.Documento.Cod_Localidade = Convert.ToInt32(dadosGerais.Cod_Localidade);
                        retorno.Documento.Cod_Area = dadosGerais.Cod_Area;
                        retorno.Documento.Cod_Grupo_Area = dadosGerais.Cod_Grupo_Area;
                        retorno.Documento.Endereco_Cidade = dadosGerais.Cidade;
                        retorno.Documento.Endereco_UF = dadosGerais.Uf;
                        retorno.Documento.Endereco_Pais = dadosGerais.Pais;
                        retorno.Documento.Endereco_Logradouro = dadosGerais.Logradouro;
                        retorno.Documento.Endereco_Numero = dadosGerais.Numero;
                        retorno.Documento.Id_Tipo_Imovel = dadosGerais.Cod_Tipo_Imovel;
                        retorno.Documento.Endereco_CEP = dadosGerais.Cep;
                        retorno.Documento.Endereco_Complemento = dadosGerais.Complemento;
                        retorno.Documento.Endereco_PontoReferencia = dadosGerais.Ponto_Referencia;
                        retorno.Documento.Endereco_Latitude = dadosGerais.Latitude_Str;
                        retorno.Documento.Endereco_Longitude = dadosGerais.Longitude_Str;


                        if (!retornoApi.Ok)
                        {
                            AddModelError(retornoApi.Mensagem, retornoApi.InnerException);
                        }
                        else
                        {
                            //retorno.Datas = retornoApi.Objeto.Select(p => new ObjBusca<String>
                            //{
                            //    Label = p.Data,
                            //    Value = p.Data
                            //}).ToList();

                            //LIMPANDO LISTA NAO USADA NA VIEW PARA DIMINUIR O TAMANHO DO JSON
                            if (retornoApi.Objeto != null)
                            {
                                retornoApi.Objeto = retornoApi.Objeto.Select(p => { p.HorasCapacidade = new List<AgendamentoHorasDisponiveis>(); return p; }).ToList();
                            }

                            retorno.Objetos = retornoApi.Objeto;
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
            }

            if (retorno.Objetos == null)
                retorno.Objetos = new List<AgendamentoDiaModel>();

            if (retorno.Documento == null)
                retorno.Documento = new Models.T5.DocumentoProducaoModel();

            return View(retorno);
        }

        [HttpPost]
        public async Task<ActionResult> Index(AgendamentoCadastroModel pModel)
        {

            pModel.Num_Obra = await new Acompanhamento_ServicoApiController().getOsReaberta(pModel.Num_Obra, pModel.CodCt, pModel.Projeto, pModel.Num_Doc.Value);
            var retorno = await new Agendamento_EquipeApiController().Gravar(
                new ParamWebApi<AgendamentoCadastroModel>
                {
                    Objeto = pModel,
                    Usuario = User.Identity.GetUserId(),
                    //Token = GetTokenByUser(User.Identity.Name)
                });

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
                return await Index(pModel.CodCt, pModel.Projeto, pModel.Num_Obra, pModel.Num_Doc.Value, null, DateTime.Today.ToString("dd/MM/yyyy"));
            }
            else
            {

                if (pModel.Pagina == "acompanhamento")
                    return RedirectToAction("Acompanhamento_Equipes", "Gestao_Equipe");


                return RedirectToAction("Index", "Acompanhamento_Servico",
                       new { CodCt = pModel.CodCt, Projeto = pModel.Projeto, Num_Obra = pModel.Num_Obra, Num_Doc = pModel.Num_Doc });
            }

        }

        public ActionResult PlanejamentoCapacidade()
        {
            return View();
    }
    }
}