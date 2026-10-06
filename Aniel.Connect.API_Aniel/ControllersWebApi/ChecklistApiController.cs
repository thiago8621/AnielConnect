using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.T5.Checklist;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.Venda;
using Aniel.T5.DAO;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ChecklistApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/checklistApi/GetPdfChecklistOS")]
        public async Task<IHttpActionResult> GetPdfChecklistOS(ChecklistApiRequest request)
        {
            ArquivoChecklist retorno = null;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                    {
                        //repositories
                        IChecklistRepository repo = new ChecklistRepository(sessionT5Nh, sessionT5);
                        IEtapaVendaRepository repoEtapa = new EtapaVendaRepository(session);
                        IDocumentoProducaoAnielRepository repoDoc = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5);
                        //Business
                        var business = new ChecklistBusiness(repo, repoEtapa);

                        Documento_ProducaoAniel document = null;
                        StringBuilder sb = new StringBuilder();

                        if (String.IsNullOrEmpty(request.CodCt) == false &&
                           String.IsNullOrEmpty(request.Projeto) == false &&
                           String.IsNullOrEmpty(request.NumObra) == false)
                        {
                            document = repoDoc.GetAll().Where(p => p.CodCt == request.CodCt &&
                                                                   p.Projeto == request.Projeto &&
                                                                   p.Num_Obra == request.NumObra).FirstOrDefault();
                        }

                        if (document == null && String.IsNullOrEmpty(request.NumObraOriginal) == false )
                        {
                            document = repoDoc.GetAll().Where(p => p.Numero_Obra_Original == request.NumObraOriginal)
                                                                      .FirstOrDefault();
                        }
                        if (!String.IsNullOrEmpty(request.NumOtOr)) 
                        {
                            var numDocOrigemOt_Or = repoDoc.GetManutencao_OT_OR(request.CodCt, request.Projeto, request.NumOtOr);
                            document = repoDoc.GetAll().Where(p => p.CodCt == request.CodCt &&
                                                                   p.Projeto == request.Projeto &&
                                                                   p.Num_Obra == request.NumTarefa && 
                                                                   p.Num_Doc == numDocOrigemOt_Or).FirstOrDefault();
                        }
                        if (document != null)
                        {
                            string otOr = null;
                            if (!String.IsNullOrEmpty(request.NumOtOr)) 
                            {                                
                                otOr = repoDoc.GetManutencao_OT_OR(document.CodCt, document.Projeto, document.Num_Obra, document.Num_Doc);
                            }

                            var tarefa = this.GetTarefa(document.CodCt, document.Projeto, document.Num_Obra, Convert.ToString(document.Num_Doc)).Result;                            
                            string sistema = String.IsNullOrEmpty(tarefa) ? "MGS" : "MCO";
                            if ((sistema == "MGS" && document.Num_Doc == 0) || (sistema == "MCO" && String.IsNullOrEmpty(tarefa))) 
                            {
                                throw new Exception("Tipo de Checklist não contém o parametro informado");
                            }
                          
                            sb.Append($"Index?sistema={sistema}")
                              .Append($"&codct={document.CodCt}")
                              .Append($"&projeto={document.Projeto}");
                              
                            string url = string.Concat(CONSTANTES.AnielRelatorios, "Quiz/", sb.ToString());                            
                            if (!string.IsNullOrEmpty(otOr))
                            {
                                url = $"{url}&numObra={request.NumOtOr}&tarefa={request.NumTarefa}&descricao={document.OXBDPEE}";
                            }
                            else 
                            {
                                url = sistema == "MGS" ? $"{url}&numObra={document.Num_Obra}&numdoc={document.Num_Doc}" : $"{url}&numObra={tarefa}&tarefa={document.Num_Obra}&descricao={document.OXBDPEE}";
                            }

                            var uploadFileVoalle = this.ConverterHtmlPdf(url, $"{request?.NumObraOriginal}.pdf");
                            var checklist = business.GetChecklistOS(Convert.ToString(document.Cod_Tipo_Serv), document.Cod_Sub_Tipo_Serv);
                            string path = CONSTANTES.CaminhoSavePdf;

                            retorno = new ArquivoChecklist
                            {
                                Id = checklist?.Id,
                                Descricao = $"{request?.NumObraOriginal}.pdf",
                                Arquivo = uploadFileVoalle,
                                Tipo = checklist != null? HtmlEnumExtensions.GetEnumDescription<TipoChecklist>(checklist.Tipo): string.Empty,
                            };
                            return new ArquivoResult(new MemoryStream(retorno.Arquivo), new HttpRequestMessage(), retorno.Descricao);
                        }
                        else throw new Exception("OS não encontrada!");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex?.InnerException?.Message, ex.StackTrace);
                return new ArquivoChecklistResult(new MemoryStream(byte.MinValue), new HttpRequestMessage(), "", HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Api para retorno de Informações do checklist
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/checklistApi/GetInformacoesChecklist")]
        public async Task<GetApiRetorno<List<ResponseChecklist>>> GetInformacoesChecklist(ChecklistApiRequest request)
        {
            var retorno = new GetApiRetorno<List<ResponseChecklist>> { Objeto = new List<ResponseChecklist>()};
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionFoto = HibernateUtil.GetSessionFoto())                
                {
                    Documento_ProducaoAniel documento = null;

                    IDocumentoProducaoAnielRepository repoDocument = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    IChecklistRepository repository = new ChecklistRepository(sessionT5, sessionT5, sessionFoto);
                    if (String.IsNullOrEmpty(request.CodCt) == false &&
                             String.IsNullOrEmpty(request.Projeto) == false &&
                             String.IsNullOrEmpty(request.NumObra) == false)
                    {
                        documento = repoDocument.GetAll().Where(p => p.CodCt == request.CodCt &&
                                                               p.Projeto == request.Projeto &&
                                                               p.Num_Obra == request.NumObra).FirstOrDefault();
                    }

                    if (documento == null && String.IsNullOrEmpty(request.NumObraOriginal) == false)
                    {
                        documento = repoDocument.GetAll().Where(p => p.Numero_Obra_Original == request.NumObraOriginal)
                                                                  .FirstOrDefault();
                    }
                    if (!String.IsNullOrEmpty(request.NumOtOr))
                    {
                        var numDocOrigemOt_Or = repoDocument.GetManutencao_OT_OR(request.CodCt, request.Projeto, request.NumOtOr);
                        documento = repoDocument.GetAll().Where(p => p.CodCt == request.CodCt &&
                                                               p.Projeto == request.Projeto &&
                                                               p.Num_Obra == request.NumTarefa &&
                                                               p.Num_Doc == numDocOrigemOt_Or).FirstOrDefault();
                    }
                    else
                    {
                        throw new Exception("nenhuma chave especificada para busca de OS");
                    }
                    var business = new ChecklistBusiness(repository);
                    var checklists = business.GetChecklistsOS(Convert.ToString(documento.Cod_Tipo_Serv), documento.Cod_Sub_Tipo_Serv);

                    foreach (var checklist in checklists) 
                    {
                        var response = new ResponseChecklist();

                        response.Id = checklist.Id;
                        var itens = repository.GetQuizItens(checklist.Id).ToList();
                        foreach (var p in itens) 
                        {
                            var listagemOpcoesResposta = new List<ResponseChecklistItemResposta>();
                            if (p.Tipo == EnumQuizTipo.MultiplaEscolha || p.Tipo == EnumQuizTipo.UnicaEscolha || p.Tipo == EnumQuizTipo.Foto || p.Tipo == EnumQuizTipo.Assinatura || p.Tipo == EnumQuizTipo.AssinaturaCliente)
                            {
                                string arquivo = p.Tipo == EnumQuizTipo.Assinatura || p.Tipo == EnumQuizTipo.AssinaturaCliente ? JsonService.SerializeObject(repository.GetAssinatura(p.Id)) : String.Empty;
                                var listagem = repository.GetQuizItensMultiplo(p.Id).ToList();
                                foreach (var resposta in listagem) 
                                {
                                    listagemOpcoesResposta.Add(new ResponseChecklistItemResposta
                                    {
                                        Id = p.Id,
                                        IdItem = (p.Tipo == EnumQuizTipo.MultiplaEscolha || p.Tipo == EnumQuizTipo.MultiplaEscolha) ? resposta.Id :
                                             (p.Tipo == EnumQuizTipo.Foto) ? resposta.RespostaFoto.Value :
                                             (p.Tipo == EnumQuizTipo.Assinatura || p.Tipo == EnumQuizTipo.AssinaturaCliente) ? p.Id_Assinatura : 0,
                                        Tipo = HtmlEnumExtensions.GetEnumDescription(p.Tipo.GetValueOrDefault()),
                                        IsResposta = resposta.QuizAnswerItem.Respondido == "S" && (p.Tipo == EnumQuizTipo.MultiplaEscolha || p.Tipo == EnumQuizTipo.UnicaEscolha ? resposta.Resposta == p.Resposta : p.Tipo == EnumQuizTipo.Foto ? p.RespostaFoto == resposta.RespostaFoto : p.Tipo == EnumQuizTipo.Assinatura || p.Tipo ==  EnumQuizTipo.AssinaturaCliente ? p.Id_Assinatura != 0 : false),
                                        Valor = (p.Tipo == EnumQuizTipo.MultiplaEscolha || p.Tipo == EnumQuizTipo.MultiplaEscolha) ? resposta.Resposta : arquivo
                                    });
                                }                                
                            }                            
                            else
                            {
                                
                                switch (p.Tipo)
                                {
                                    case EnumQuizTipo.Texto:
                                        listagemOpcoesResposta.Add(new ResponseChecklistItemResposta
                                        {
                                            Id = p.Id,
                                            IsResposta = !String.IsNullOrEmpty(p.RespostaTexto),
                                            Tipo = HtmlEnumExtensions.GetEnumDescription(p.Tipo.GetValueOrDefault()),
                                            Valor = p.RespostaTexto
                                        });
                                        break;
                                    case EnumQuizTipo.Valor:
                                        listagemOpcoesResposta.Add(new ResponseChecklistItemResposta
                                        {
                                            Id = p.Id,
                                            IsResposta = p.RespostaValor.HasValue,
                                            Tipo = HtmlEnumExtensions.GetEnumDescription(p.Tipo.GetValueOrDefault()),
                                            Valor = p.RespostaValor
                                        });
                                        break;                                                                  
                                    case EnumQuizTipo.Data:
                                        listagemOpcoesResposta.Add(new ResponseChecklistItemResposta
                                        {
                                            Id = p.Id,
                                            IsResposta = p.RespostaData.HasValue,
                                            Tipo = HtmlEnumExtensions.GetEnumDescription(p.Tipo.GetValueOrDefault()),
                                            Valor = p.RespostaData
                                        });
                                        break;                                    
                                    case EnumQuizTipo.CodigoBarra:
                                        listagemOpcoesResposta.Add(new ResponseChecklistItemResposta
                                        {
                                            Id = p.Id,
                                            IsResposta = !String.IsNullOrEmpty(p.RespostaCodBarra),
                                            Tipo = HtmlEnumExtensions.GetEnumDescription(p.Tipo.GetValueOrDefault()),
                                            Valor = p.RespostaCodBarra
                                        });
                                        break;
                                }
                            }
                            response.Perguntas.Add(new ResponseChecklistItem
                            {
                                Id = p.Id,
                                Respondido = p.Respondido == "S" ? true : false,
                                OpcoesResposta = listagemOpcoesResposta,
                            }); 
                        }                        
                        
                        retorno.Objeto.Add(response);
                    }                    
                }
                retorno.Ok = true;                
            }
            catch (Exception ex)
            {
                retorno.Objeto = new List<ResponseChecklist>();
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex?.InnerException?.Message, ex.StackTrace);
            }
            return retorno;
        }

        /// <summary>
        ///  URL para preenchimento do checklist comercial utilizando o filtro pelo ID da venda
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/checklistApi/GerarLinkChecklist")]
        public async Task<ApiResponseObj<String>> GerarLinkChecklist(GerarLinkByVenda request)
        {
            var retorno = new ApiResponseObj<String>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (var repoVenda = new Conexao<Venda>(session))
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }

                    IChecklistRepository repository = new ChecklistRepository(sessionT5Nh, sessionT5);
                    IEtapaVendaRepository repositoryEtapa = new EtapaVendaRepository(session);
                    IVendaRepository repositoryVenda = new VendaRepository(session);
                    IClienteRepository repositoryCliente = new ClienteRepository(session);
                    var business = new ChecklistBusiness(repository, repositoryEtapa);
                    Venda venda = repositoryVenda.GetById(request.Venda);

                    if (venda != null)
                    {
                        if (business.GetChecklistVenda(request.Venda) == null)
                        {

                            if (venda != null)
                            {
                                var idUf = venda.EnderecoInstalacao?.Endereco_Logradouro?.UF?.Id ?? venda?.EnderecoInstalacao?.Id_UF;
                                var idCidade = venda.EnderecoInstalacao?.Endereco_Logradouro?.Cidade?.Id ?? venda?.EnderecoInstalacao?.Id_Cidade;

                                Cliente cliente = repositoryCliente.GetById(venda.Id_Cliente.Value);

                                business.AssociarChecklistVenda(venda.Id, cliente.Tipo_Pessoa.Value, cliente?.Categoria_Cliente?.Id,
                                                                cliente?.Qual_Meio?.Id, idUf, idCidade);

                                Quiz checklist = business.GetChecklistVenda(request.Venda);
                                if (checklist == null)
                                {
                                    throw new Exception("Sem Checklist possível para ser associado a venda.");
                                }
                            }
                        }

                        var criptografia = Criptografia_AES256.doEncryptAES(Convert.ToString(venda.Id));
                        retorno.data = $@"{CONSTANTES.GetBaseUrlConnect()}/pt/Venda/EmailChecklist?Id_Venda={criptografia}&Email=S";
                        retorno.ok = true;
                    }
                    else
                    {
                        throw new Exception("Venda não encontrada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.data = "";
                retorno.ok = false;
                retorno.error = ex.Message;
                if (ex.InnerException != null) retorno.error += "\n" + ex.InnerException.Message;
            }
            return retorno;
        }

        private byte[] ConverterHtmlPdf(string url, string nome)
        {
            string caminhoFisico = CONSTANTES.CaminhoSavePdf;
            var retornoBytes = new List<byte>(); 
            if (!Directory.Exists(CONSTANTES.CaminhoSavePdf))
            {
                Directory.CreateDirectory(CONSTANTES.CaminhoSavePdf);
            }
            
                var html = ConvertToPdfService.getHtmlFromExternalSource(url);
            html = html.Replace(@"col-sm-12 col-md-6 col-lg-4", "");
                var pdf = ConvertToPdfService.getPdfBytesFromHtmlString(html);
                File.WriteAllBytes($"{CONSTANTES.CaminhoSavePdf}/{nome}", pdf);
                retornoBytes.AddRange(File.ReadAllBytes($"{CONSTANTES.CaminhoSavePdf}/{nome}"));
            
            return retornoBytes.ToArray();
        }

        private async Task<string> GetTarefa(string contrato, string projeto, string numObra, string numDoc) 
        {
            var parametros = new Dictionary<string, object> { { "@CONTRATO", contrato }, { "@PROJETO", projeto }, 
                                                              { "@NUM_OBRA", numObra }, { "@NUM_DOC", numDoc} };
            var retorno = string.Empty;
            try 
            {
                if (string.IsNullOrEmpty(contrato) || String.IsNullOrEmpty(projeto)) 
                {
                    throw new Exception("parâmetros contrato e projeto não definidos");
                }
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("SELECT A.NUM_OT_OR AS TAREFA FROM TB_MANUTENCAO_OT_OR A INNER JOIN TB_DOCUMENTO_PRODUCAO B ")
                      .AppendLine("ON A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_EMPRESA = B.NUM_DOC WHERE ")
                      .AppendLine("A.CODCT = @CONTRATO AND A.PROJETO = @PROJETO AND B.NUM_OBRA = @NUM_OBRA AND B.NUM_DOC = @NUM_DOC");
                    using (var reader = await CommandHelper.ExecuteReader(com, sb, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = Convert.ToString(reader["TAREFA"]);
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                throw ex;
            }           
            return retorno;
        }
    }
}