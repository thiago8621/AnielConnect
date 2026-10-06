using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.PontosAfetados;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Foto.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Acompanhamento_ServicoController : BaseController
    {

        public Acompanhamento_ServicoController() : base("Acompanhamento_Servico",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Acompanhamento_Servicoapi/FindById?Num_Obra={0}",
                                                   "api/Acompanhamento_Servicoapi/Gravar",
                                                   "api/Acompanhamento_Servicoapi/Excluir",
                                                   String.Empty)
        {

        }

        [HttpPost]
        public async Task<String> UploadAquivo(Fotos_ArquivosListaModel model)
        {
            PostApiRetorno<Int64?> retorno = new PostApiRetorno<Int64?>();
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var session = HibernateUtil.GetSessionFoto())
                using (var repoLogLanc = new Conexao<Log_Lanc_Servico>(sessionT5))
                using (var repoFoto = new Conexao<Arquivo_DocProd>(session))
                {
                    var AcompanhamentoApi = new Acompanhamento_ServicoApiController();
                    var log_Lanc = new Log_Lanc_Servico
                    {
                        CodCt = model.Cod_ct,
                        Usuario = User.Identity.GetUserId(),
                        Projeto = model.Projeto,
                        Valor_Ant = "",
                        Contrato = Convert.ToInt32(model.Num_doc),
                        Num_Obra = AcompanhamentoApi.getOsReaberta(model.Num_Obra, model.Cod_ct, model.Projeto, Convert.ToInt32(model.Num_doc)).Result,
                        Alteracao = "Upload de Arquivo/Foto",
                        Hora = DateTime.Now,
                        Dia = DateTime.Now.Date,
                        Valor_Atualizado = Request.Files[0].FileName
                    };
                    List<Arquivo_DocProd> temp = new List<Arquivo_DocProd>();
                    for (var i = 0; i < Request.Files.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(Request.Files[i].FileName))
                        {
                            byte[] allBytes = new byte[Request.Files[i].InputStream.Length];
                            Request.Files[i].InputStream.Read(allBytes, 0, (int)Request.Files[i].InputStream.Length);

                            temp.Add(new Arquivo_DocProd
                            {
                                Id = (await repoFoto.Max(p => p.Id)) + 1,
                                Chave = $"{model.Cod_ct}&{model.Projeto}&{model.Num_doc}",
                                Descricao = model.Descricao,
                                Tipo = Path.GetExtension(Request.Files[i].FileName).Replace(".", ""),
                                Arquivo = allBytes,
                                Data = DateTime.Now.Date,
                                Hora = DateTime.Now,
                                Usuario = User.Identity.GetUserId(),
                                Num_Obra = model.Num_Obra
                            });
                        }
                    }
                    await repoLogLanc.SaveOrUpdate(log_Lanc);
                    await repoFoto.SaveOrUpdate(temp);
                    retorno.Ok = true;
                    retorno.IdObjeto = temp.First()?.Id;

                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return JsonConvert.SerializeObject(retorno);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> UploadFotoQuiz(ImagemQuizRequest model)
        {
            try
            {
                if (Request.Files == null || Request.Files.Count == 0 || Request.Files[0] == null)
                {
                    return Json(new PostApiRetorno
                    {
                        Ok = false,
                        Mensagem = "Nenhum arquivo foi enviado."
                    });
                }

                var arquivo = Request.Files[0];

                if (arquivo.ContentLength <= 0)
                {
                    return Json(new PostApiRetorno
                    {
                        Ok = false,
                        Mensagem = "Arquivo inválido."
                    });
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
                using (var repo = new Conexao<Tb_Quiz_Foto>(sessionFoto))
                using (var com = session.Connection.CreateCommand())
                {
                    byte[] allBytes;
                    using (var ms = new MemoryStream())
                    {
                        await arquivo.InputStream.CopyToAsync(ms);
                        allBytes = ms.ToArray();
                    }

                    long idQuestionAnswerItem = Convert.ToInt64(model.IdQuestionAnswerItem);
                    long idQuestion = Convert.ToInt64(model.IdQuestion);
                    long idQuestionAnswerCapa = Convert.ToInt64(model.IdQuestionAnswerCapa);
                    long idQuestionAnswer = Convert.ToInt64(model.IdQuestionAnswer);

                    Aniel.T5.Model.enumerado.EnumQuizTipo? tipoItem = null;

                    var parametrosTipo = new Dictionary<string, object>
                     {
                         { "@ID", idQuestionAnswerItem }
                     };

                    var sqlTipo = new StringBuilder();
                    sqlTipo.AppendLine("SELECT TIPO");
                    sqlTipo.AppendLine("FROM TB_QUIZ_ANSWER_ITEM");
                    sqlTipo.AppendLine("WHERE ID = @ID");

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlTipo, parametrosTipo, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            if (reader["TIPO"] != DBNull.Value)
                            {
                                tipoItem = (Aniel.T5.Model.enumerado.EnumQuizTipo)Convert.ToInt32(reader["TIPO"]);
                            }
                        }
                    }

                    if (!tipoItem.HasValue)
                    {
                        return Json(new PostApiRetorno
                        {
                            Ok = false,
                            Mensagem = "Item do checklist não encontrado."
                        });
                    }

                    var fotoBanco = new Tb_Quiz_Foto
                    {
                        IdAnswerCapa = idQuestionAnswerCapa,
                        IdAnswerItem = idQuestionAnswerItem,
                        IdQuestion = idQuestion,
                        Arquivo = allBytes
                    };

                    await repo.Save(fotoBanco);

                    if (tipoItem.Value == Aniel.T5.Model.enumerado.EnumQuizTipo.AssinaturaCliente)
                    {
                        var parametrosUpdate = new Dictionary<string, object>
                       {
                           { "@ID_ITEM", idQuestionAnswerItem },
                           { "@ID_ASSINATURA", fotoBanco.Id }
                       };

                        var sqlUpdate = new StringBuilder();
                        sqlUpdate.AppendLine("UPDATE TB_QUIZ_ANSWER_ITEM");
                        sqlUpdate.AppendLine("SET ID_ASSINATURA = @ID_ASSINATURA");
                        sqlUpdate.AppendLine("WHERE ID = @ID_ITEM");

                        await CommandHelper.ExecuteNonQuery(com, sqlUpdate, parametrosUpdate, session.Connection, null);
                    }
                    else
                    {
                        var parametrosInsert = new Dictionary<string, object>
                      {
                          { "@ID_ITEM", idQuestionAnswerItem },
                          { "@ID_QUESTION_ANSWER", model.IdQuestionAnswer},
                          { "@ID_FOTO", fotoBanco.Id }
                      };

                        var sqlInsert = new StringBuilder();
                        sqlInsert.AppendLine("INSERT INTO TB_QUIZ_ANSWER_ITEM_MULTI (ID_QUIZ_ANSWER_ITEM, ID_QUESTION_ANSWER, RESPOSTA_FOTO)");
                        sqlInsert.AppendLine("VALUES (@ID_ITEM, @ID_QUESTION_ANSWER, @ID_FOTO)");

                        await CommandHelper.ExecuteNonQuery(com, sqlInsert, parametrosInsert, session.Connection, null);
                    }

                    return Json(new PostApiRetorno<long>
                    {
                        Ok = true,
                        IdObjeto = fotoBanco.Id,
                        Mensagem = "Imagem enviada com sucesso."
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new PostApiRetorno
                {
                    Ok = false,
                    Mensagem = e.Message
                });
            }
        }

        public async Task<ActionResult> VisualizarChecklist(String codct, String projeto, String numobra, string numdoc, long? IdChecklist)
        {
            ViewBag.Areas = Areas;
            using (QuizApiController api = new QuizApiController())
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionSQL = HibernateUtil.GetSessionT5())
            using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
            {
                var repository = new DocumentoProducaoAnielRepository(session, sessionSQL);
                var num_obra_original = numobra.Split('/')[0];
                var lista_os = repository.GetAll().Where(p => p.CodCt == codct
                                                                       && p.Projeto == projeto
                                                                       && p.Num_Doc == Convert.ToInt32(numdoc)
                                                                       && p.Numero_Obra_Original == num_obra_original).ToList();

                GetApiRetorno<List<QuizAnswer>> model = new GetApiRetorno<List<QuizAnswer>>();
                model.Objeto = new List<QuizAnswer>();

                foreach (var os in lista_os)
                {
                    var resultado = new GetApiRetorno<List<QuizAnswer>>();
                    if (os.Sigla_Modulo == "MCO")
                    {
                        var numManutencaoOToR = repository.GetManutencao_OT_OR(codct, projeto, os.Num_Obra, Convert.ToInt32(numdoc));
                        resultado = await api.GetQuizAnswer(codct, projeto, numManutencaoOToR, os.Tarefa, Convert.ToInt32(numdoc));
                    }
                    else
                        resultado = await api.GetQuizAnswer(codct, projeto, os.Num_Obra, "", Convert.ToInt32(numdoc));


                    foreach (var item in resultado.Objeto)
                    {
                        if (IdChecklist.HasValue && item.idA == IdChecklist)
                        {
                            item.respostas = await api.GetQuizAnswerItem(sessionSQL, sessionFoto, item.id.Value);
                        }
                    }

                    model.Objeto.AddRange(resultado.Objeto);
                }

                if (IdChecklist.HasValue)
                {
                    model.Objeto = model.Objeto.FindAll(p => p.idA == IdChecklist);
                }

                return View(model.Objeto);
            }
        }

        public ActionResult Index(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc)
        {

            bool disableDetalhes = false;
            var retorno = new GetApiRetorno<AcompanhamentoListaModel>();

            using (ISession sessionNh = HibernateUtil.GetSessionT5())          
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Master>(sessionNh))
            using (var repository = new Conexao<Configuracoes_Gerais>(session))
            {
                var config = repoConfig.selectFirst().Result;
                ViewBag.UtilizaParalisaOSAS = config.ConfigParalisaOS;
                ViewBag.UtilizaCadastroEndereco = repository.selectFirst().Result.Utiliza_Cadastro_Endereco;
            }

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var acompanhamento_api = new Acompanhamento_ServicoApiController())
            {
                try
                {
                    if (!String.IsNullOrEmpty(Convert.ToString(Num_Obra)))
                    {
                        var num_obra_original = acompanhamento_api.getOsReaberta(Num_Obra, CodCt, Projeto, Num_Doc.Value).Result;
                        if (String.IsNullOrEmpty(num_obra_original))
                        {
                            AddModelError(Resources.RsCadastros.SERVICO_NAO_ENCONTRADO);

                            retorno.Objeto = new AcompanhamentoListaModel();

                        }
                        else
                        {
                            retorno = acompanhamento_api.FindById(CodCt, Projeto, num_obra_original, Num_Doc).Result;
                            var cliente = acompanhamento_api.GetDadosGerais_Cliente(CodCt, Projeto, num_obra_original, Num_Doc).Result.Objeto;

                            if (!retorno.Ok)
                            {
                                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, retorno.Mensagem, retorno.InnerException);
                            }                            

                            retorno.Objeto.IdCliente = cliente?.Id_Cliente;
                            retorno.Objeto.IdPlanoProduto = cliente?.Id_PlanoProduto;
                            retorno.Objeto.Id_Troca_Plano = cliente?.Id_Troca_Plano;
                            retorno.Objeto.Descricao_Plano_Anterior = cliente?.Descricao_Plano_Anterior;
                        }
                    }
                    else
                    {
                        retorno.Objeto = new AcompanhamentoListaModel();
                    }
                }
                catch (Exception ex)
                {
                    AddModelError(ex.Message, ex.InnerException?.Message);
                }
                finally
                {
                    FillListasView(retorno.Objeto);
                }
            }

            ViewBag.Areas = Areas;

            return View(retorno.Objeto);
        }


        public ActionResult StatusOs(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc)
        {

            try
            {
                if (!String.IsNullOrEmpty(Convert.ToString(Num_Obra)))
                {
                    //GetApiRetorno<AcompanhamentoListaModel> retorno =
                    //    GetByUrl<GetApiRetorno<AcompanhamentoListaModel>>(String.Format("api/Acompanhamento_Servicoapi/FindById?Codct={0}&Projeto={1}&Num_Obra={2}&Num_Doc={3}",
                    //    CodCt, Projeto, Num_Obra , Num_Doc));

                    var acompanhamento_api = new Acompanhamento_ServicoApiController();
                    var num_obra_original = acompanhamento_api.getOsReaberta(Num_Obra, CodCt, Projeto, Num_Doc.Value).Result;
                    if (String.IsNullOrEmpty(num_obra_original))
                    {
                        AddModelError(Resources.RsCadastros.SERVICO_NAO_ENCONTRADO);

                        var obj = new AcompanhamentoListaModel();

                        FillListasView(obj);

                        return View("StatusOs", obj);
                    }
                    else
                    {
                        var retorno = acompanhamento_api.FindById(CodCt, Projeto, num_obra_original, Num_Doc).Result;

                        if (!retorno.Ok)
                        {
                            AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, retorno.Mensagem, retorno.InnerException);
                        }

                        FillListasView(retorno.Objeto);

                        return View("StatusOs", retorno.Objeto);
                    }
                }
                else
                {
                    var obj = new AcompanhamentoListaModel();

                    FillListasView(obj);

                    return View("StatusOs", obj);
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View("StatusOs");
            }
        }

        [HttpGet()]
        public async Task<ActionResult> Dados_Gerais(String Cod_Ct, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var conf_geraisRepo = new Conexao<Configuracoes_Gerais>(session))
            {
                var conf_gerais = await conf_geraisRepo.selectFirst();
                ViewBag.RealizaAgendamento = conf_gerais.Exige_Agendamento_Servico;
                ViewBag.Considerar_Agendamento_Periodo = conf_gerais.Considerar_Agendamento_Periodo;
                ViewBag.Criterio_Area = conf_gerais.Criterio_Area;
                ViewBag.Exige_LatLong_Agendamento = conf_gerais.Exige_LatLong_Agendamento;
                ViewBag.Utiliza_Cadastro_Endereco = Convert.ToBoolean(conf_gerais.Utiliza_Cadastro_Endereco);
                ViewBag.PermitirMotivoReagendamento = Convert.ToBoolean(conf_gerais.PermitirMotivoReagendamento);
            }

            ViewBag.AtualizarPagina = false;
            ViewBag.Areas = Areas;
            var acompanhamento_api = new Acompanhamento_ServicoApiController();
            ViewBag.DisableDetalhes = acompanhamento_api.GetDesbloqueioDetalhes(Cod_Ct, Projeto, Num_Obra, Num_Doc).Result;

            var num_obra_original = acompanhamento_api.getOsReaberta(Num_Obra, Cod_Ct, Projeto, Num_Doc).Result;
            DadosGerais_CadastroModel pModel = (await new Acompanhamento_ServicoApiController().GetDadosGerais_Cliente(Cod_Ct, Projeto, num_obra_original, Num_Doc)).Objeto ?? new DadosGerais_CadastroModel();
            var OSCompleta = acompanhamento_api.getOsCompletaReaberta(Num_Obra, Cod_Ct, Projeto, Num_Doc).Result;
            ViewBag.Planos_Produtos = (pModel != null && pModel.Id_Cliente != 0 && pModel.Id_PlanoProduto != null) ?
                                      (await new PlanoProdutoApiController().getByCliente(pModel.Id_Cliente)).FirstOrDefault(p => p.Value == pModel.Id_PlanoProduto) :
                                       new PlanoProdutoComboAntendimentoViewModel();

            var campos = (await new CamposObrigatoriosDocumentoProducaoApiController().GetCampos());
            var campobrigatorios = (await new CamposObrigatoriosDocumentoProducaoApiController().GetCampos()).ToList<ICamposObrigatoriosModel>();
            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campobrigatorios);
            ViewBag.CamposBloqueados = CamposObrigatoriosController.dictCamposBloqueados(campobrigatorios);
            ViewBag.CamposTitulo = CamposObrigatoriosController.dictCamposTitulo(campos);
            pModel.ConfirmaEnderecoAgendamento = (await new Configuracoes_GeraisApiController().FindById(1)).Objeto.ConfirmarEndereco;
            ViewBag.HistoricoAtendimento = acompanhamento_api.HistoricoVincular(new HistoricoVincularOSResponse_DataTableRequest {CodCt = Cod_Ct,Projeto= Projeto,Num_Obra = num_obra_original, Num_Doc= Num_Doc }).Result.data.Count() > 0;

            return View(pModel);
        }

        [HttpGet()]
        public ActionResult Tecnico(String Cod_Ct, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            ViewBag.Areas = Areas;

            var acompanhamento_api = new Acompanhamento_ServicoApiController();
            var num_obra_original = acompanhamento_api.getOsReaberta(Num_Obra, Cod_Ct, Projeto, Num_Doc).Result;
            TecnicoCadastroModel pModel = acompanhamento_api.GetTecnico(Cod_Ct, Projeto, num_obra_original, Num_Doc).Result.Objeto;
            return View(pModel);
        }

        [HttpPost()]
        public ActionResult Tecnico(TecnicoCadastroModel pmodel)
        {
            ViewBag.Areas = Areas;
            pmodel.Usuario = User.Identity.GetUserId();

            PostApiRetorno<String> retorno = new Acompanhamento_ServicoApiController().GravarTecnico(pmodel, TemAcesso("139")).Result;
            ViewBag.AtualizarStatus = true;

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem);
                pmodel = new Acompanhamento_ServicoApiController().GetTecnico(pmodel.CodCt, pmodel.Projeto, pmodel.Id, pmodel.Num_Doc).Result.Objeto;
                return View(pmodel);
            }

            if (String.IsNullOrEmpty(pmodel.Redirecionamento) == false)
            {
                ViewBag.Redirecionar = pmodel.Redirecionamento;
            }

            pmodel = new Acompanhamento_ServicoApiController().GetTecnico(pmodel.CodCt, pmodel.Projeto, pmodel.Id, pmodel.Num_Doc).Result.Objeto;
            return View(pmodel);
        }

        public override bool Validacoes<T>(T pModel)
        {
            List<CamposObrigatoriosDocumento_ProducaoModels> campos = new CamposObrigatoriosDocumentoProducaoApiController().GetCampos().Result;

            Dictionary<String, String> erros = new Dictionary<String, String>();

            DadosGerais_CadastroModel _model = pModel as DadosGerais_CadastroModel;

            foreach (var campo in campos)
            {
                // OBS.: o campo Origem da OS já é verificado na view
                // OBS.: o campo Prioridade do Cliente não é preenchido nessa tela, logo não é necessário validação
                if (campo.Obrigatorio)
                {
                    switch (campo.Campo)
                    {
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.SUBTIPO_SERVICO:
                            if (String.IsNullOrEmpty(_model.Subtipo_Servico))
                                erros.Add("Id_SubTipo_servico", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.SUBTIPO_SERVICO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.SEGMENTO:
                            if (String.IsNullOrEmpty(_model.Segmento))
                                erros.Add("Segmento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.SEGMENTO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.OBSERVACAO:
                            if (String.IsNullOrEmpty(_model.Obs_Criacao_Servico))
                                erros.Add("Obs_Criacao_Servico", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.OBSERVACAO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_1:
                            if (String.IsNullOrEmpty(_model.Adcional_1) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_1", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_2:
                            if (String.IsNullOrEmpty(_model.Adcional_2) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_2", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_3:
                            if (String.IsNullOrEmpty(_model.Adcional_3) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_3", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_4:
                            if (String.IsNullOrEmpty(_model.Adcional_4) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_4", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_5:
                            if (String.IsNullOrEmpty(_model.Adcional_5) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_5", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_6:
                            if (String.IsNullOrEmpty(_model.Adcional_6) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_6", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_7:
                            if (String.IsNullOrEmpty(_model.Adcional_7) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_7", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_8:
                            if (String.IsNullOrEmpty(_model.Adcional_8) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_8", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_9:
                            if (String.IsNullOrEmpty(_model.Adcional_9) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_9", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_10:
                            if (String.IsNullOrEmpty(_model.Adcional_10) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_10", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.POP:
                            if (_model.Id_Pop.HasValue == false && _model.Tipo == "M")
                                erros.Add("Id_Pop", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.POP));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.OLT:
                            if (_model.Id_Olt.HasValue == false && _model.Tipo == "M")
                                erros.Add("Id_Olt", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.OLT));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ELEMENTO_REDE:
                            if (_model.Id_Elemento_Rede.HasValue == false && _model.Tipo == "M")
                                erros.Add("Id_Elemento_Rede", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ELEMENTO_REDE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.DEPARTAMENTO:
                            if (string.IsNullOrEmpty(_model.IdDepartamento))
                                erros.Add("IdDepartamento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DEPARTAMENTO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.PRIORIDADE:
                            if (_model.Prioridade_Cliente.HasValue == false)
                                erros.Add("Prioridade_Cliente", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PRIORIDADE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.CEP_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Cep) && _model.Tipo == "M")
                                erros.Add("Cep", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CEP));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ENDERECO_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Logradouro) && _model.Tipo == "M")
                                erros.Add("Logradouro", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LOGRADOURO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.NUMERO_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Cep) && _model.Tipo == "M")
                                erros.Add("Numero", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ELEMENTO_REDE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.UF_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Uf) && _model.Tipo == "M")
                                erros.Add("Uf", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.UF));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.CIDADE_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Cidade))
                                erros.Add("Cidade", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CIDADE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.BAIRRO_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Bairro))
                                erros.Add("Bairro", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.BAIRRO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.PAIS:
                            if (string.IsNullOrEmpty(_model.Pais))
                                erros.Add("Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.TIPO_IMOVEL:
                            if (_model.Cod_Tipo_Imovel.HasValue == false)
                                erros.Add("Cod_Tipo_Imovel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TIPO_IMOVEL));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.COMPLEMENTO_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Complemento) && _model.Tipo == "M")
                                erros.Add("Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.PONTO_REFERENCIA_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Ponto_Referencia) && _model.Tipo == "M")
                                erros.Add("Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTOREFERENCIA));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.LATITUDE:
                            if (string.IsNullOrEmpty(_model.Latitude_Str))
                                erros.Add("Latitude_Str", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LATITUDE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.LONGITUDE:
                            if (string.IsNullOrEmpty(_model.Longitude_Str))
                                erros.Add("Longitude_Str", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LONGITUDE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.GRUPO_AREA:
                            if (string.IsNullOrEmpty(_model.Cod_Escritorio))
                                erros.Add("Cod_Escritorio", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.GRUPO_AREA));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.AREA:
                            if (string.IsNullOrEmpty(_model.Cod_Area))
                                erros.Add("Cod_Area", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.AREA));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.LOCALIDADE:
                            if (string.IsNullOrEmpty(_model.Cod_Localidade))
                                erros.Add("Cod_Localidade", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LOCALIDADE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.NOME_CONTATO_INSTALACAO:
                            if (string.IsNullOrEmpty(_model.NomeInstalacao) && _model.Tipo == "C")
                                erros.Add("NomeInstalacao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NOME_CONTATO_INSTALACAO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.CPF_CONTATO_INSTALACAO:
                            if (string.IsNullOrEmpty(_model.CpfInstalacao) && _model.Tipo == "C")
                                erros.Add("CpfInstalacao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CPF_CONTATO_INSTALACAO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.TELEFONE_CONTATO_INSTALACAO:
                            if (string.IsNullOrEmpty(_model.TelefoneInstalacao) && _model.Tipo == "C")
                                erros.Add("TelefoneInstalacao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONE_CONTATO_INSTALACAO));
                            break;
                    }
                }
            }

            if (erros.Count > 0)
            {
                AddModelError(erros);
            }

            return erros.Count == 0;
        }

        [HttpPost()]
        public ActionResult Dados_Gerais(DadosGerais_CadastroModel pmodel)
        {
            ViewBag.AtualizarPagina = false;
            ViewBag.TipoServico = new Tipo_Servico_EquipeApiController().GetList().Result.Objeto;
            ViewBag.SubTipoServico = new List<ObjBusca<String>>();
            ViewBag.Areas = Areas;

            var campos = new CamposObrigatoriosDocumentoProducaoApiController().GetCampos().Result;
            var campobrigatorios = new CamposObrigatoriosDocumentoProducaoApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();


            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campobrigatorios);
            ViewBag.CamposBloqueados = CamposObrigatoriosController.dictCamposBloqueados(campobrigatorios);
            ViewBag.CamposTitulo = CamposObrigatoriosController.dictCamposTitulo(campos);

            using (ISession session = HibernateUtil.GetSession())
            {
                var conf_gerais = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result;
                ViewBag.RealizaAgendamento = conf_gerais.Exige_Agendamento_Servico;
                ViewBag.Considerar_Agendamento_Periodo = conf_gerais.Considerar_Agendamento_Periodo;
                ViewBag.Criterio_Area = conf_gerais.Criterio_Area;
                ViewBag.Exige_LatLong_Agendamento = conf_gerais.Exige_LatLong_Agendamento;
            }

            var acompanhamento_api = new Acompanhamento_ServicoApiController();
            ViewBag.DisableDetalhes = acompanhamento_api.GetDesbloqueioDetalhes(pmodel.CodCt, pmodel.Projeto, pmodel.Num_Obra, Convert.ToInt32(pmodel.Num_Doc)).Result;
            var num_obra_original = acompanhamento_api.getOsReaberta(pmodel.Num_Obra, pmodel.CodCt, pmodel.Projeto, Convert.ToInt32(pmodel.Num_Doc)).Result;
            pmodel.Num_Obra = num_obra_original;


            if (ModelState.IsValid && Validacoes<DadosGerais_CadastroModel>(pmodel))
            {
                PostApiRetorno<String> retorno = acompanhamento_api.GravarDadosGerais(pmodel).Result;
                DadosGerais_CadastroModel pModelAcompanhamento = new Acompanhamento_ServicoApiController().GetDadosGerais_Cliente(pmodel.CodCt, pmodel.Projeto, pmodel.Num_Obra, Convert.ToInt32(pmodel.Num_Doc)).Result.Objeto;
                if (!retorno.Ok)
                {
                    AddModelError(retorno.Mensagem);
                    return View(pModelAcompanhamento);
                }
                if (String.IsNullOrEmpty(pmodel.Redirecionamento) == false)
                {
                    ViewBag.Redirecionar = pmodel.Redirecionamento;
                }

                ViewBag.AtualizarPagina = true;
            }
            else
            {
                ViewBag.AtualizarPagina = false;
                return View(pmodel);
            }


            //return RedirectToAction("Index", "Acompanhamento_Servico", new {CodCt = pmodel.CodCt,  Projeto = pmodel.Projeto,  Num_Obra = pmodel.Num_Obra,  Num_Doc = pmodel.Num_Doc });

            DadosGerais_CadastroModel pModel = new Acompanhamento_ServicoApiController().GetDadosGerais_Cliente(pmodel.CodCt, pmodel.Projeto, pmodel.Num_Obra, Convert.ToInt32(pmodel.Num_Doc)).Result.Objeto;
            ViewBag.Planos_Produtos = new PlanoProdutoApiController().getByCliente(pmodel.Id_Cliente).Result.FirstOrDefault(p => p.Label == pModel.Plano_Produto);

            return View(pModel);
        }

        public ActionResult Materiais_Aplicados(String CodCt, String Projeto, String Num_Obra, String Num_Doc, String Equipe, String CodMat, String CodCpl, String Data_Aplic)
        {
            ViewBag.Areas = Areas;
            using (var acompanhamento_api = new Acompanhamento_ServicoApiController())
            {
                var os = acompanhamento_api.getOsCompletaReaberta(Num_Obra, CodCt, Projeto, Convert.ToInt32(Num_Doc)).Result;
                if (String.IsNullOrEmpty(os.Equipe))
                {
                    AddModelError(String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.EQUIPE));
                }

                return View(new Material_AplicadoListaModel
                {
                    Status_Auditoria = Convert.ToInt32(os.Status_Auditoria),
                    CodCt = CodCt,
                    Projeto = Projeto,
                    Num_Obra = Num_Obra,
                    Num_Doc = Convert.ToInt32(Num_Doc),
                    Equipe = os.Equipe,
                    Codigo = CodMat,
                    Codigo_Complementar = CodCpl,
                    Data_Aplic = Data_Aplic
                });
            }
        }

        public ActionResult Materiais_Retirados(String CodCt, String Projeto, String Num_Obra, String Num_Doc, String Equipe, String CodMat, String CodCpl, String Data_Aplic)
        {
            ViewBag.Areas = Areas;
            using (var acompanhamento_api = new Acompanhamento_ServicoApiController())
            {
                var os = acompanhamento_api.getOsCompletaReaberta(Num_Obra, CodCt, Projeto, Convert.ToInt32(Num_Doc)).Result;
                if (String.IsNullOrEmpty(os.Equipe))
                {
                    AddModelError(String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.EQUIPE));
                }

                return View(new Material_AplicadoListaModel
                {
                    Status_Auditoria = Convert.ToInt32(os.Status_Auditoria),
                    CodCt = CodCt,
                    Projeto = Projeto,
                    Num_Obra = Num_Obra,
                    Num_Doc = Convert.ToInt32(Num_Doc),
                    Equipe = os.Equipe,
                    Codigo = CodMat,
                    Codigo_Complementar = CodCpl,
                    Data_Aplic = Data_Aplic
                });
            }
        }

        public ActionResult Prod_Lancada(String CodCt, String Projeto, String Num_Obra, String Num_Doc, String Unidade_Custo, String Tarefa, String Equipe, Int16? Cod_Tipo_Serv,
                                         String Cod_Sub_Tipo_Serv, String Tipo_Producao, String Codigo, String Data_Prod)
        {
            using (var acompanhamento_api = new Acompanhamento_ServicoApiController())
            {
                var os = acompanhamento_api.getOsCompletaReaberta(Num_Obra, CodCt, Projeto, Convert.ToInt32(Num_Doc)).Result;
                if (String.IsNullOrEmpty(os.Equipe))
                {
                    AddModelError(String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.EQUIPE));
                }

                return View(new Prod_LancadaListaModel
                {
                    Status_Auditoria = Convert.ToInt32(os.Status_Auditoria),
                    CodCt = CodCt,
                    Projeto = Projeto,
                    Num_Obra = Num_Obra,
                    Num_Doc = Convert.ToInt32(Num_Doc),
                    Codigo = Codigo,
                    Cod_Tipo_Serv = Cod_Tipo_Serv,
                    Cod_Sub_Tipo_Serv = Cod_Sub_Tipo_Serv,
                    Equipe = os.Equipe,
                    Tipo_Producao = Tipo_Producao,
                    Unidade_Custo = Unidade_Custo,
                    Tarefa = Tarefa,
                    Data_Prod = Data_Prod
                });
            }
        }


        public ActionResult Equipe_Auxiliar(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            ViewBag.Areas = Areas;

            using (var acompanhamento_api = new Acompanhamento_ServicoApiController())
            {
                var num_obra = acompanhamento_api.getOsReaberta(Num_Obra, CodCt, Projeto, Num_Doc).Result;

                return View(new EquipeAuxiliaresModel
                {
                    CodCt = CodCt,
                    Projeto = Projeto,
                    Num_Obra = num_obra,
                    Num_Doc = Num_Doc

                });
            }
        }

        //[HttpGet()]
        public ActionResult Historico_Servico(String Cod_Ct, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            GetApiRetorno<Historico_ServicoCadastroModel> retorno = new Acompanhamento_ServicoApiController().FindById_Historico(Cod_Ct, Projeto, Num_Obra, Num_Doc).Result;

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem);
            }
            else
            if (retorno.Objeto.Historico_Servico.Count == 0)
            {
                AddModelError("Nenhum Historico Encontrado");
            }

            if (retorno.Objeto == null)
                retorno.Objeto = new Historico_ServicoCadastroModel();
            return View(retorno.Objeto); ;
        }

        [HttpGet()]
        public ActionResult Checklist(String Cod_Ct, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            ViewBag.Areas = Areas;
            ViewBag.Codct = Cod_Ct;
            ViewBag.Projeto = Projeto;
            ViewBag.NumObra = Num_Obra;
            ViewBag.NumDoc = Num_Doc;

            return View();
        }

        public ActionResult Fotos_Arquivos(Fotos_ArquivosListaModel pModel, String Num_Obra, String Cod_Ct, String Projeto, String Num_Doc)
        {
            if (pModel == null)
            {
                pModel = new Fotos_ArquivosListaModel();
            }

            pModel.Num_Obra = Num_Obra;
            pModel.Num_doc = Num_Doc;
            pModel.Cod_ct = Cod_Ct;
            pModel.Projeto = Projeto;
            return View(pModel);
        }

        public ActionResult Cancelar_OS(String Cod_Ct, String Projeto, String Num_Obra, Int32? Num_Doc, String Usuario)
        {
            PostApiRetorno<Int64> retorno = new Acompanhamento_ServicoApiController().AlterarStatus(Cod_Ct, Projeto, Num_Obra, Usuario, Num_Doc.Value).Result;

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem);
            }

            return View(retorno);
        }

        public ActionResult BuscarGeolocalizacao(bool dialog = false)
        {
            var inicializacao = new BuscarGeolocalizacaoModels();
            if(dialog)        
            {
                inicializacao.OrigemDialog = dialog;
            }
            return View(inicializacao);
        }

        [HttpGet()]
        public ActionResult VerClientesAfetados(string codct, string projeto, string numDoc, string numObra)
        {

            return View( new GetClientesByOSManutencaoRequest
            {
                Codct = codct,
                Projeto = projeto,
                NumDoc = numDoc,
                NumObra = numObra
            });
		}

        public ActionResult VincularAtendimento(Int64? Id_Cliente)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;

            return View(new VincularAtendimento_DataTableRequest
            {
                Dt_Inicial_Filtro = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"),
                Dt_Final_Filtro = DateTime.Now.ToString("dd/MM/yyyy"),
                Status = null,
                Id_Cliente = Id_Cliente
            });
        }
        public ActionResult HistoricoVincularAtend(String projeto, String codCt, Int32? num_Doc ,String num_Obra)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;

            return View(new HistoricoVincularOSResponse_DataTableRequest
            {
                CodCt = codCt,
                Projeto = projeto,
                Num_Doc = num_Doc,
                Num_Obra = num_Obra,

            });
        }
    }
}