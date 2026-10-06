using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.PesquisaSatisfacaoRepository;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using Grafana.Models;
using Grafana.Services.Impl;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Pesquisa_SatisfacaoApiController : BaseApiController
    {
        [HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/pesquisa_Satisfacaoapi/GetAll")]
        public async Task<DataTableResponse<Pesquisa_SatisfacaoListaModel>> GetAll(Pesquisa_Satisfacao_DataTableRequest request)
        {
            List<Pesquisa_SatisfacaoListaModel> retorno = new List<Pesquisa_SatisfacaoListaModel>();
            IOrderedEnumerable<Pesquisa_SatisfacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_PESQUISA_SATISFACAO "); /*WHERE STATUS = 1*/

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.TITULO_DA_PESQUISA, A.STATUS, A.DATAHORACRIACAO, A.QUEMPREENCHE,DISPAROAUTO ");
                    sql.AppendLine("FROM TB_PESQUISA_SATISFACAO A ");

                    sql.AppendLine("WHERE A.ID > 0");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        //sql.AppendLine("AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                        //parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    if (!String.IsNullOrEmpty(request.StatusPesquisa) && request.StatusPesquisa != "2")
                    {                   
                        sql.AppendLine("AND A.STATUS = @STATUS");
                        parametros.Add("@STATUS", Convert.ToInt64(request.StatusPesquisa));
                    }

                    if (!String.IsNullOrEmpty(request.QuemPreenche))
                    {
                        sql.AppendLine("AND A.QUEMPREENCHE = @QUEMPREENCHE");
                        parametros.Add("@QUEMPREENCHE", Convert.ToInt64(request.QuemPreenche));
                    }

                    if (!String.IsNullOrEmpty(request.DisparoAuto))
                    {
                        sql.AppendLine("AND A.DISPAROAUTO = @DISPAROAUTO");
                        parametros.Add("@DISPAROAUTO", Convert.ToInt64(request.DisparoAuto));
                    }

                    if (request.SomentePesquisa)
                    {
                        sql.AppendLine(" AND A.TEM_CAIXA_TEXTO = 1");
                    }
                    if (request.SomentePesquisaSem)
                    {
                        sql.AppendLine(" AND A.TEM_CAIXA_TEXTO = 0");
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Pesquisa_SatisfacaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Titulo_Da_Pesquisa = Convert.ToString(reader["TITULO_DA_PESQUISA"]),

                                Status = Convert.ToInt32(reader["STATUS"]) == 1 ? Resources.RsCadastros.ATIVO : Resources.RsCadastros.INATIVO,
                                Data_Hora_Criacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                Quem_Preenche = reader["QUEMPREENCHE"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<PesquisaSatisfacaoQuemPreenche>(Convert.ToString(reader["QUEMPREENCHE"])) : null,
                                Tipo_Disparo = reader["DISPAROAUTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<PesquisaSatisfacaoDisparoAuto>(Convert.ToString(reader["DISPAROAUTO"])) : null,
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Pesquisa_SatisfacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/GetAllComentarios")]
        public async Task<DataTableResponse<OpiniaoListaModel>> GetAllComentarios(Opinioes_DataTableRequest request)
        {
            List<OpiniaoListaModel> retorno = new List<OpiniaoListaModel>();
            IOrderedEnumerable<OpiniaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_PESQUISA_RESPOSTA WHERE COMENTARIO IS NOT NULL ");
                    sql.AppendLine($@" AND DATAHORA_RESPONDIDA IS NOT NULL AND ID_PESQUISA_SATISFACAO = {request.Id_Pesquisa_Satisfacao}");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine(" SELECT A.DATAHORA_RESPONDIDA, A.CATEGORIA, A.COMENTARIO, B.NOMEFANTASIA, ");
                    sql.AppendLine("        B.ID AS ID_CLIENTE, C.ID AS ID_FOTO_PERFIL, C.EXTENSAO_ARQUIVO ");
                    sql.AppendLine(" FROM TB_PESQUISA_RESPOSTA A ");

                    sql.AppendLine(" LEFT JOIN TB_CLIENTE B ");
                    sql.AppendLine(" ON B.ID = A.ID_CLIENTE ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE_ANEXO C ");
                    sql.AppendLine(" ON(C.ID_CLIENTE = B.ID AND C.FOTO_PERFIL = 1) ");

                    sql.AppendLine(" WHERE A.COMENTARIO IS NOT NULL AND A.DATAHORA_RESPONDIDA IS NOT NULL ");
                    sql.AppendLine(" AND A.ID_PESQUISA_SATISFACAO = @ID ");


                    parametros.Add("@ID", request.Id_Pesquisa_Satisfacao);

                    //Filtrando
                    if (request.Categoria_Opiniao.HasValue)
                    {
                        sql.AppendLine($"AND A.CATEGORIA = {Convert.ToInt32(request.Categoria_Opiniao.Value)}");
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(B.NOMEFANTASIA) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            String id_foto = Convert.ToString(reader["ID_FOTO_PERFIL"]);
                            String extensao_arquivo = Convert.ToString(reader["EXTENSAO_ARQUIVO"]);

                            retorno.Add(new OpiniaoListaModel
                            {
                                Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"]),
                                Nome = Convert.ToString(reader["NOMEFANTASIA"]),
                                Categoria = HtmlEnumExtensions.GetEnumDescription<PesquisaSatisfacaoCategoriaCaixaTexto>(Convert.ToString(reader["CATEGORIA"])),
                                Comentario = Convert.ToString(reader["COMENTARIO"]),
                                Data_Resposta = reader["DATAHORA_RESPONDIDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_RESPONDIDA"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Comentario_Truncate = StringService.TruncateTresPontinhos(Convert.ToString(reader["COMENTARIO"]), 100),
                                Nome_Arquivo_Foto_Perfil = (!String.IsNullOrEmpty(id_foto) && !String.IsNullOrEmpty(extensao_arquivo)) ? String.Format("{0}.{1}", id_foto, extensao_arquivo) : null
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Comentario);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Comentario);
                            break;
                        }
                    default:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Comentario);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Comentario);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<OpiniaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Pesquisa_SatisfacaoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Pesquisa_SatisfacaoListaModel>> retorno = new GetApiRetorno<IEnumerable<Pesquisa_SatisfacaoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    var lista = await new Conexao<Pesquisa_Satisfacao>(session).queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Pesquisa_SatisfacaoListaModel
                    {
                        Id = p.Id,
                        //Nome = p.Nome
                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }
        private async Task<Pesquisa_SatisfacaoCadastroModel> GetViewObj(Pesquisa_Satisfacao obj, String URLLogo, String NomEmpresa, long? idAtendimento = null)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Configuracao_Envio_Mensagens> repoEnvio = new Conexao<Configuracao_Envio_Mensagens>(session))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))

            {
                Configuracao_Envio_Mensagens modeloenvio = (await repoEnvio.selectFirst(me => me.Id == Convert.ToInt64(obj.Modelo_Envio)));

                Pesquisa_SatisfacaoCadastroModel retorno = new Pesquisa_SatisfacaoCadastroModel();


                retorno.Id = obj.Id;
                retorno.Titulo_Da_Pesquisa = obj.Titulo_Da_Pesquisa;
                retorno.Descricao_Pesquisa = obj.Descricao;
                retorno.Status = obj.Status == Status_Default.Ativo;
                retorno.QuemPreenche = obj.QuemPreenche;
                retorno.DisparoAuto = obj.DisparoAuto;
                retorno.Status_Pesquisa_Pergunta = obj.Status == Status_Default.Ativo;
                retorno.Tem_Caixa_Texto = obj.Tem_Caixa_Texto;
                retorno.Envia_Msg_Emai_Sms = obj.Envia_Msg_Emai_Sms;
                retorno.Modelo_Envio = obj?.Modelo_Envio;
                retorno.TipoCaixaTexto = obj.TipoCaixaTexto;
                retorno.NomeEmpresa = NomEmpresa;
                retorno.URLLogo = URLLogo;
                retorno.AssuntoPesquisa = modeloenvio?.Assunto_Email;
                retorno.CorpoPesquisa = (modeloenvio?.Mensagem_Corpo_Email);
                retorno.IdAtendimento = idAtendimento;

                retorno.PesquisaJaEnviada = TemPesquisaRespondida(obj.Id).Result;
                retorno.Perguntas = obj.Perguntas.Select(p => new Pesquisa_PerguntasItemModel
                {
                    Id = p.Id,
                    Descricao = p.Descricao,
                    Status_Pesquisa_Pergunta = p.Status_Pesquisa_Pergunta,
                    Obrigatorio = p.Obrigatorio,
                    Escala_Linear_Ini = p.Escala_Linear_Ini,
                    Escala_Linear_Fin = p.Escala_Linear_Fin,
                    Tipo_Resposta = p.Tipo_Resposta,
                    Ordem = p.Ordem,
                    Respostas_Possiveis = p.Respostas_Possiveis,
                    Escala_Emoticos = p.Escala_Emoticos
                }).OrderBy(p => p.Ordem).ToList();



                return retorno;

            }
        }
        private async Task<Pesquisa_SatisfacaoCadastroModel> GetViewObjFormulario(Pesquisa_Satisfacao obj, String URLLogo, String NomEmpresa)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (Conexao<Configuracao_Envio_Mensagens> repoEnvio = new Conexao<Configuracao_Envio_Mensagens>(session))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            using (Conexao<Pesquisa_Resposta> repoResposta = new Conexao<Pesquisa_Resposta>(session))
            using (Conexao<ClienteContrato> repoContrato = new Conexao<ClienteContrato>(session))
            using (Conexao<Documento_ProducaoAniel> repoOs = new Conexao<Documento_ProducaoAniel>(sessionT5))

            {
                Configuracao_Envio_Mensagens modeloenvio = (await repoEnvio.selectFirst(me => me.Id == Convert.ToInt64(obj.Modelo_Envio)));
                Pesquisa_Resposta pesquisaresposta = (await repoResposta.selectFirst(pr => pr.Pesquisa_Satisfacao.Id == obj.Id));
                Cliente cliente = (await repoCliente.selectFirst(c => c.Id == pesquisaresposta.Cliente.Id));
                ClienteContrato contrato = (await repoContrato.selectFirst(ct => ct.Cliente.Id == pesquisaresposta.Cliente.Id));
                Documento_ProducaoAniel os = (await repoOs.selectFirst(o => o.Cliente.Cod_Cliente == pesquisaresposta.Cliente.Id));

                Pesquisa_SatisfacaoCadastroModel retorno = new Pesquisa_SatisfacaoCadastroModel();


                retorno.Id = obj.Id;
                retorno.Titulo_Da_Pesquisa = obj.Titulo_Da_Pesquisa;
                retorno.Descricao_Pesquisa = obj.Descricao;
                retorno.Status = obj.Status == Status_Default.Ativo;
                retorno.QuemPreenche = obj.QuemPreenche;
                retorno.DisparoAuto = obj.DisparoAuto;
                retorno.Status_Pesquisa_Pergunta = obj.Status == Status_Default.Ativo;
                retorno.Tem_Caixa_Texto = obj.Tem_Caixa_Texto;
                retorno.Envia_Msg_Emai_Sms = obj.Envia_Msg_Emai_Sms;
                retorno.Modelo_Envio = obj?.Modelo_Envio;
                retorno.TipoCaixaTexto = obj.TipoCaixaTexto;
                retorno.NomeEmpresa = NomEmpresa;
                retorno.URLLogo = URLLogo;
                retorno.AssuntoPesquisa = modeloenvio?.Assunto_Email == null ? null : modeloenvio?.Assunto_Email;
                retorno.CorpoPesquisa = modeloenvio?.Mensagem_Corpo_Email == null ? null : (modeloenvio.Mensagem_Corpo_Email.Replace("<NOME_CLIENTE>", cliente.NomeFantasia)
                                                                      .Replace("<CPF>", cliente.CpfCnpj)
                                                                      .Replace("<RG>", cliente.Rg)
                                                                      .Replace("<NUMERO_CONTRATO>", (contrato == null ? "" : contrato.Descricao))
                                                                      .Replace("<VAR_NUMERO_OS>", (os == null ? "" : Convert.ToString(os.Num_Doc)))
                                                                      .Replace("<VAR_NOME_TECNICO>", (os == null ? "" : Convert.ToString(os.Usuario_Baixa)))
                                                                      .Replace("<VAR_DATA_HORA_AGENDAMENTO>", (os == null ? "" : Convert.ToString(os.DiaHora_Agendamento)))
                                                                      .Replace("<VAR_DATA_HORA_AGENDAMENTO>", (os == null ? "" : Convert.ToString(os.DiaHora_Agendamento)))
                                                                      .Replace("<VAR_PESQUISA>", obj.Descricao));

                retorno.PesquisaJaEnviada = TemPesquisaRespondida(obj.Id).Result;
                retorno.Perguntas = obj.Perguntas.Select(p => new Pesquisa_PerguntasItemModel
                {
                    Id = p.Id,
                    Descricao = p.Descricao,
                    Status_Pesquisa_Pergunta = p.Status_Pesquisa_Pergunta,
                    Obrigatorio = p.Obrigatorio,
                    Escala_Linear_Ini = p.Escala_Linear_Ini,
                    Escala_Linear_Fin = p.Escala_Linear_Fin,
                    Tipo_Resposta = p.Tipo_Resposta,
                    Ordem = p.Ordem,
                    Respostas_Possiveis = p.Respostas_Possiveis,
                    Escala_Emoticos = p.Escala_Emoticos
                }).OrderBy(p => p.Ordem).ToList();



                return retorno;

            }
        }

        private void BindViewToObj(ISession session, Pesquisa_Satisfacao objBanco, Pesquisa_SatisfacaoCadastroModel view)
        {
            objBanco.Id = view.Id;

            objBanco.Titulo_Da_Pesquisa = view.Titulo_Da_Pesquisa;
            objBanco.Descricao = view.Descricao_Pesquisa;

            var disparo_auto_atend = new Conexao<Pesquisa_Satisfacao>(session).select(p => p.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Envio_Automatico_Atendimento
                                                                                        && p.Status == Status_Default.Ativo
                                                                                        && p.Id != objBanco.Id).Result.Count;
            var disparo_auto_servico = new Conexao<Pesquisa_Satisfacao>(session).select(p => p.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Envio_Automatico_Servico
                                                                                          && p.Status == Status_Default.Ativo
                                                                                          && p.Id != objBanco.Id).Result.Count;

            if ((disparo_auto_atend != 0) && (view.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Envio_Automatico_Atendimento))
            {
                throw new Exception("Já existe uma pesquisa padrão de finalização de atendimento");
            }
            else if ((disparo_auto_servico != 0) && (view.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Envio_Automatico_Servico))
            {
                throw new Exception("Já existe uma pesquisa padrão de finalização de serviço");
            }
            else
            {
                objBanco.DisparoAuto = view.DisparoAuto;
            }

            objBanco.QuemPreenche = view.QuemPreenche;
            objBanco.Envia_Msg_Emai_Sms = view.Envia_Msg_Emai_Sms;
            objBanco.Modelo_Envio = view.Modelo_Envio;
            objBanco.Tem_Caixa_Texto = view.Tem_Caixa_Texto;
            objBanco.TipoCaixaTexto = view.TipoCaixaTexto;
            if (objBanco.Id != 0)
            {
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
            }

            objBanco.Perguntas = view.Perguntas.Select(p => new Pesquisa_Pergunta
            {
                Id = p.Id,
                Descricao = p.Descricao,
                Pesquisa_Satisfacao = objBanco,
                Obrigatorio = p.Obrigatorio,
                Status_Pesquisa_Pergunta = p.Status_Pesquisa_Pergunta,
                Escala_Linear_Ini = p.Escala_Linear_Ini != "null" ? p.Escala_Linear_Ini : "0",
                Escala_Linear_Fin = p.Escala_Linear_Fin != "null" ? p.Escala_Linear_Fin : "0",
                Respostas_Possiveis = p.Respostas_Possiveis,
                Escala_Emoticos = p.Escala_Emoticos,
                Tipo_Resposta = p.Tipo_Resposta,
                Ordem = p.Ordem
            }).ToList();
        }

        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PESQUISA_SATISFACAO SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_PESQUISA_SATISFACAO WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/pesquisa_SatisfacaoApi/FindById")]
        public async Task<GetApiRetorno<Pesquisa_SatisfacaoCadastroModel>> FindById(Int64 Id, long? idAtendimento = null)
        {
            GetApiRetorno<Pesquisa_SatisfacaoCadastroModel> retorno = new GetApiRetorno<Pesquisa_SatisfacaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                {
                    try
                    {
                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        retorno.Objeto = await GetViewObj(await new Conexao<Pesquisa_Satisfacao>(session).selectFirst(p => p.Id == Id), config.OLA_URL_Logo, config.Nome_Cliente_Sinapse, idAtendimento);
                        retorno.Ok = true;
                    }
                    catch (Exception ex)
                    {
                        retorno.Mensagem = ex.Message;
                        if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    }
                }
            }
            return retorno;
        }
        [HttpGet()]
        [Route("api/pesquisa_SatisfacaoApi/FindByIdFormulario")]
        public async Task<GetApiRetorno<Pesquisa_SatisfacaoCadastroModel>> FindByIdFormulario(Int64 Id)
        {
            GetApiRetorno<Pesquisa_SatisfacaoCadastroModel> retorno = new GetApiRetorno<Pesquisa_SatisfacaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                {
                    try
                    {
                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        retorno.Objeto = await GetViewObjFormulario(await new Conexao<Pesquisa_Satisfacao>(session).selectFirst(p => p.Id == Id), config.OLA_URL_Logo, config.Nome_Cliente_Sinapse);
                        retorno.Ok = true;
                    }
                    catch (Exception ex)
                    {
                        retorno.Mensagem = ex.Message;
                        if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    }
                }
            }
            return retorno;
        }

        public async Task<Pesquisa_Resposta> FindByIdResposta(Int64 Id)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                return await new Conexao<Pesquisa_Resposta>(session).selectFirst(p => p.Id == Id);
            }
        }

        [HttpPost()]
        [Route("api/pesquisa_SatisfacaoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Pesquisa_SatisfacaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Pesquisa_Satisfacao modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Pesquisa_Satisfacao>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Pesquisa_Satisfacao
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/pesquisa_SatisfacaoApi/GravarFormulario")]
        public async Task<PostApiRetorno<Int64>> GravarFormulario(ParamWebApi<Pesquisa_Satisfacao_FormularioModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Pesquisa_Resposta modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    Pesquisa_Satisfacao pesquisa = await new Conexao<Pesquisa_Satisfacao>(session).selectFirst(p => p.Id == value.Objeto.Pesquisa.Id);
                    Cliente cliente = await new Conexao<Cliente>(session).selectFirst(p => p.Id == value.Objeto.Id_Cliente);

                    using (var repository = new Conexao<Pesquisa_Resposta>(session))
                    using (var repositoryResposta = new Conexao<Pesquisa_Pergunta_Resposta>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Pesquisa_Resposta
                            {
                                UsuarioCriacao = value.Usuario,
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        modelo.DataHora_Respondida = DateTime.Now;
                        modelo.Pesquisa_Satisfacao = pesquisa;
                        modelo.Cliente = cliente;
                        modelo.Categoria = value.Objeto.Categoria;
                        modelo.Comentario = value.Objeto.Comentario;

                        List<Pesquisa_Pergunta_Resposta> respostas = new List<Pesquisa_Pergunta_Resposta>();

                        using (var repositoryRespostas = new Conexao<Pesquisa_Resposta>(session))
                        {
                            foreach (var pergunta in pesquisa.Perguntas)
                            {
                                var respostaView = value.Objeto.Pesquisa.Perguntas.Find(p => p.Id == pergunta.Id);

                                if (pergunta.Tipo_Resposta == Pesquisa_Tipo_Resposta.Multipla_Escolha)
                                {
                                    Int32 indice = 0;
                                    foreach (var respMulti in pergunta.Respostas_Possiveis.Split(';'))
                                    {
                                        if (respostaView.Respostas[indice])
                                        {
                                            Pesquisa_Pergunta_Resposta resposta = new Pesquisa_Pergunta_Resposta
                                            {
                                                Pesquisa_Resposta = modelo,
                                                Pesquisa_Pergunta = pergunta,
                                                Resposta = respMulti
                                            };
                                            respostas.Add(resposta);
                                        }

                                        indice += 1;
                                    }
                                }
                                else if (pergunta.Tipo_Resposta == Pesquisa_Tipo_Resposta.Texto)
                                {
                                    Pesquisa_Pergunta_Resposta resposta = new Pesquisa_Pergunta_Resposta
                                    {
                                        
                                        Pesquisa_Resposta = modelo,
                                        Pesquisa_Pergunta = pergunta,
                                        Resposta = respostaView.RespostaTexto // usa o campo do form
                                    };
                                    respostas.Add(resposta);
                                }
                                else if (pergunta.Tipo_Resposta == Pesquisa_Tipo_Resposta.Avaliacao_Emoticons)
                                {
                                    if (pergunta.Escala_Emoticos == Padrao.Sim)
                                    {
                                        Pesquisa_Pergunta_Resposta resposta = new Pesquisa_Pergunta_Resposta
                                        {

                                            Pesquisa_Resposta = modelo,
                                            Pesquisa_Pergunta = pergunta,
                                            Resposta = respostaView.RespostaTextoEmoticon
                                        };
                                        respostas.Add(resposta);
                                    }
                                }
                                else
                                {
                                    Pesquisa_Pergunta_Resposta resposta = new Pesquisa_Pergunta_Resposta
                                    {
                                        Pesquisa_Resposta = modelo,
                                        Pesquisa_Pergunta = pergunta,
                                        Resposta = respostaView.Resposta
                                    };
                                    respostas.Add(resposta);
                                }

                            }

                        }

                        using (ITransaction transaction = session.BeginTransaction())
                        {
                            await repository.SaveOrUpdate(modelo);
                            await repositoryResposta.SaveOrUpdate(respostas);
                            transaction.Commit();
                        }
                    }

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/pesquisa_SatisfacaoApi/Enviar")]
        public async Task<PostApiRetorno<Int64>> Enviar(Int64? Id_Cliente, Int64 Id_Pesquisa, String Usuario, String CodCt, String Projeto, String Num_Obra, Int32 Num_Doc, Int64? IdAtendimento)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Pesquisa_Resposta modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Pesquisa_Resposta>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var repoConfigEnvio = new Conexao<Configuracao_Envio_Mensagens>(session))
                    using (var repoAtendimento = new Conexao<Atendimento>(session))
                    using (Conexao<Pesquisa_Resposta> repoResposta = new Conexao<Pesquisa_Resposta>(session))
                    using (Conexao<ClienteContrato> repoContrato = new Conexao<ClienteContrato>(session))
                    using (Conexao<Documento_ProducaoAniel> repoOs = new Conexao<Documento_ProducaoAniel>(sessionT5))
                    {

                        Cliente cliente = await new Conexao<Cliente>(session).selectFirst(p => p.Id == Id_Cliente);
                        Pesquisa_Satisfacao pesquisa = await new Conexao<Pesquisa_Satisfacao>(session).selectFirst(p => p.Id == Id_Pesquisa);
                        Configuracao_Envio_Mensagens modeloenvio = await new Conexao<Configuracao_Envio_Mensagens>(session).selectFirst(p => p.Id == Convert.ToInt64(pesquisa.Modelo_Envio));
                        Pesquisa_Resposta pesquisaresposta = (await repoResposta.selectFirst(pr => pr.Pesquisa_Satisfacao.Id == Id_Pesquisa));
                        ClienteContrato contrato = (await repoContrato.selectFirst(ct => ct.Cliente.Id == Id_Cliente));
                        Documento_ProducaoAniel os = (await repoOs.selectFirst(o => o.Cliente.Cod_Cliente == Id_Cliente));
                        Atendimento att = await repoAtendimento.selectFirst(a => a.Id == IdAtendimento);

                        modelo = new Pesquisa_Resposta
                        {
                            Pesquisa_Satisfacao = pesquisa,
                            Cliente = cliente,
                            UsuarioCriacao = Usuario,
                            CodCt = CodCt,
                            Projeto = Projeto,
                            Num_Obra = Num_Obra,
                            Num_Doc = Num_Doc,
                            Atendimento = att
                        };

                        await repository.SaveOrUpdate(modelo);

                        Configuracoes_Gerais config = await repoConfig.selectFirst();
                        EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);
                        String urlPesquisa = $@"{CONSTANTES.URLPortal}/pt/Pesquisa_Satisfacao/Formulario/{modelo.Id}";

                        using (StreamReader stream = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_pesquisa_satisfacao.html"))
                        {
                            String arquivo_email = stream.ReadToEnd();
                            String arquivo_modificado = arquivo_email
                                                                     .Replace("{{TITULO_PESQUISA}}", pesquisa.Titulo_Da_Pesquisa)
                                                                     .Replace("{{URL_PESQUISA}}", urlPesquisa)
                                                                     .Replace("{{URL_LOGO}}", config.OLA_URL_Logo);

                            if (!String.IsNullOrEmpty(cliente.Email_Principal))
                            {
                                emailService.EnviarMensagem(cliente.Email_Principal, (modeloenvio.Assunto_Email.Replace("<NOME_CLIENTE>", cliente.NomeFantasia)
                                                                                                               .Replace("<CPF>", cliente.CpfCnpj)
                                                                                                               .Replace("<RG>", cliente.Rg)
                                                                                                               .Replace("<NUMERO_CONTRATO>", (contrato == null ? "" : contrato.Descricao))
                                                                                                               .Replace("<VAR_NUMERO_OS>", (os == null ? "" : Convert.ToString(os.Num_Doc)))
                                                                                                               .Replace("<VAR_NOME_TECNICO>", (os == null ? "" : Convert.ToString(os.Usuario_Baixa)))
                                                                                                               .Replace("<VAR_DATA_HORA_AGENDAMENTO>", (os == null ? "" : Convert.ToString(os.DiaHora_Agendamento)))
                                                                                                               .Replace("<VAR_DATA_HORA_AGENDAMENTO>", (os == null ? "" : Convert.ToString(os.DiaHora_Agendamento)))
                                                                                                               .Replace("<VAR_PESQUISA>", pesquisa.Descricao)),
                                                                                    arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                            }
                            else
                            {
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.PESQUISA_SATISFACAO_CLIENTE_SEM_EMAIL, pesquisa.Titulo_Da_Pesquisa, cliente.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }
                        }

                    }

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/pesquisa_SatisfacaoApi/Enviar")]
        public async Task<PostApiRetorno<Int64>> Enviar(Int64 Id_Cliente, Int64 Id_Pesquisa, String Usuario, long? IdAtendimento)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    Pesquisa_Resposta modelo = null;


                    using (var repository = new Conexao<Pesquisa_Resposta>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var repoAtt = new Conexao<Atendimento>(session))
                    {

                        Cliente cliente = await new Conexao<Cliente>(session).selectFirst(p => p.Id == Id_Cliente);
                        if (String.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            throw new Exception("Cliente não possui email vinculado");
                        }

                        Pesquisa_Satisfacao pesquisa = await new Conexao<Pesquisa_Satisfacao>(session).selectFirst(p => p.Id == Id_Pesquisa);

                        if (pesquisa == null)
                        {
                            throw new Exception("Pesquisa não encontrada");
                        }
                     
                        Int64 id_resposta = await new Conexao<Pesquisa_Resposta>(session).Max(p => p.Id);

                        modelo = new Pesquisa_Resposta
                        {
                            Pesquisa_Satisfacao = pesquisa,
                            Cliente = cliente,
                            UsuarioCriacao = Usuario,
                            Id = id_resposta,
                        };

                        Atendimento atendimento = null;

                        if (IdAtendimento != null)
                        {
                            atendimento = await repoAtt.selectFirst(x => x.Id == IdAtendimento);

                            if (atendimento != null)
                                modelo.Atendimento = atendimento;
                        }

                        await repository.SaveOrUpdate(modelo);

                        Configuracoes_Gerais config = await repoConfig.selectFirst();
                        EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);
                        String urlPesquisa = $@"{CONSTANTES.URLPortal}/pt/Pesquisa_Satisfacao/Formulario/{modelo.Id}";

                        using (StreamReader stream = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_pesquisa_satisfacao.html"))
                        {
                            String arquivo_email = stream.ReadToEnd();
                            String arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia)
                                                                     .Replace("{{TITULO_PESQUISA}}", pesquisa.Titulo_Da_Pesquisa)
                                                                     .Replace("{{URL_PESQUISA}}", urlPesquisa)
                                                                     .Replace("{{URL_LOGO}}", config.OLA_URL_Logo);

                            if (!String.IsNullOrEmpty(cliente.Email_Principal))
                            {
                                emailService.EnviarMensagem(cliente.Email_Principal, "Pesquisa de Satisfação", arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                            }
                            else
                            {
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.PESQUISA_SATISFACAO_CLIENTE_SEM_EMAIL, pesquisa.Titulo_Da_Pesquisa, cliente.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }
                        }

                    }

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }


            return retorno;
        }


        [HttpPost()]
        [Route("api/pesquisa_SatisfacaoApi/EnviarEmMassa")]
        public async Task<PostApiRetorno<Int64>> EnviarEmMassa(Pesquisa_Satisfacao_PesquisaEmMassaModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);

                foreach (var id in value.ids)
                {
                    foreach (var pesquisa in value.ids_pesquisa)
                    {
                        using (ISession session = HibernateUtil.GetSession())
                        {
                            Pesquisa_SatisfacaoCadastroModel pesquisa_satisfacao = (await FindById(pesquisa)).Objeto;
                            Cliente cliente = await new Conexao<Cliente>(session).selectFirst(p => p.Id == id);
                            IPesquisaSatisfacaoRepository pesquisaRepository = new PesquisaSatisfacaoRepository(session);
                            PesquisaSatisfacaoBusiness pesquisaSatisfacaoBusiness = new PesquisaSatisfacaoBusiness(pesquisaRepository);
                            pesquisaSatisfacaoBusiness.InsertPesquisaResposta(cliente, value.usuario, pesquisa, pesquisa_satisfacao.IdAtendimento);
                            Int64 id_resposta = await new Conexao<Pesquisa_Resposta>(session).Max(p => p.Id);
                            String urlPesquisa = $@"{CONSTANTES.URLPortal}/pt/Pesquisa_Satisfacao/Formulario/{id_resposta}";
                            pesquisaSatisfacaoBusiness.EnviarEmail(cliente,
                                                                   pesquisa_satisfacao.Titulo_Da_Pesquisa,
                                                                   pesquisa_satisfacao.URLLogo,
                                                                   urlPesquisa,
                                                                   emailService,
                                                                   CONSTANTES.CaminhoFisico);
                        }
                    }
                }

                retorno.Ok = true;

            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Pesquisa_Satisfacao/PreencherPesquisaAtendedor")]
        public async Task<GetApiRetorno<Pesquisa_Satisfacao_Atendedor>> PreencherPesquisaAtendedor(Preenchimento_Atendedor_Model value)
        {
            GetApiRetorno<Pesquisa_Satisfacao_Atendedor> retorno = new GetApiRetorno<Pesquisa_Satisfacao_Atendedor>();
            retorno.Objeto = new Pesquisa_Satisfacao_Atendedor();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" INSERT INTO TB_PESQUISA_RESPOSTA (ID, USUARIOCRIACAO, DATAHORACRIACAO, ID_CLIENTE, ID_PESQUISA_SATISFACAO) ");
                        sql.AppendLine(" VALUES((SELECT COALESCE(MAX(ID), 0) + 1 FROM TB_PESQUISA_RESPOSTA), @USUARIOCRIACAO, CURRENT_TIMESTAMP, @ID_CLIENTE, @ID_PESQUISA_SATISFACAO)");

                        parametros.Add("@USUARIOCRIACAO", User.Identity.GetUserId());
                        parametros.Add("@ID_CLIENTE", value.Id_Cliente);
                        parametros.Add("@ID_PESQUISA_SATISFACAO", value.Id_Pesquisa);

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                        Pesquisa_Satisfacao pesquisa = await new Conexao<Pesquisa_Satisfacao>(session).selectFirst(p => p.Id == value.Id_Pesquisa);
                        Int64 id_resposta = await new Conexao<Pesquisa_Resposta>(session).Max(p => p.Id);

                        retorno.Objeto = new Pesquisa_Satisfacao_Atendedor
                        {
                            Caminho_Pesquisa = $@"{CONSTANTES.URLPortal}/pt/Pesquisa_Satisfacao/Formulario/{id_resposta}"
                        };
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [Route("api/pesquisa_SatisfacaoApi/Excluir")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> Excluir(Int64 id)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Pesquisa_Satisfacao>(session))
                    {

                        Pesquisa_Satisfacao modelo = null;
                        if (id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == id)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }



        [HttpGet()]
        [Route("api/pesquisa_Satisfacaoapi/TemPesquisaRespondida")]
        public async Task<Boolean> TemPesquisaRespondida(Int64 Id_Pesquisa)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT FIRST 1 A.ID_PESQUISA_SATISFACAO ")
                                                .AppendLine("FROM TB_PESQUISA_RESPOSTA A ")
                                                .AppendLine("WHERE A.ID_PESQUISA_SATISFACAO = @ID_PESQUISA_SATIFACAO AND A.DATAHORA_RESPONDIDA IS NOT NULL");

                    parametros.Add("ID_PESQUISA_SATIFACAO", Id_Pesquisa);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/BuscaPorCliente")]
        public async Task<DataTableResponse<Pesquisa_SatisfacaoListaModel>> BuscaPorCliente(Pesquisa_Satisfacao_DataTableRequest request)
        {
            List<Pesquisa_SatisfacaoListaModel> retorno = new List<Pesquisa_SatisfacaoListaModel>();
            IOrderedEnumerable<Pesquisa_SatisfacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_PESQUISA_RESPOSTA A ");

                sqlFrom.AppendLine(" INNER JOIN TB_PESQUISA_SATISFACAO B ");
                sqlFrom.AppendLine(" ON(B.ID = A.ID_PESQUISA_SATISFACAO) ");

                sqlFrom.AppendLine(" LEFT JOIN ASPNETUSERS C ");
                sqlFrom.AppendLine(" ON(C.ID = A.USUARIOCRIACAO) ");

                sqlFrom.AppendLine(" INNER JOIN TB_PESQUISA_SATISFACAO D ");
                sqlFrom.AppendLine(" ON(D.ID = A.ID_PESQUISA_SATISFACAO) ");

                sqlFrom.AppendLine(" WHERE A.ID_CLIENTE = @ID_CLIENTE ");

                parametros.Add("@ID_CLIENTE", request.Id_Cliente);

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, B.TITULO_DA_PESQUISA, D.QUEMPREENCHE, B.STATUS AS STATUS_PESQUISA, A.DATAHORACRIACAO AS DATAHORA_ENVIO, ");
                    sql.AppendLine("        A.DATAHORA_RESPONDIDA, C.NOME AS USUARIO_ENVIO ");

                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Pesquisa_SatisfacaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),

                                Quem_Preenche = HtmlEnumExtensions.GetEnumDescription((PesquisaSatisfacaoQuemPreenche)reader["QUEMPREENCHE"]),
                                Usuario_Criacao = Convert.ToString(reader["USUARIO_ENVIO"]),

                                Titulo_Da_Pesquisa = Convert.ToString(reader["TITULO_DA_PESQUISA"]),
                                Status = Convert.ToInt32(reader["STATUS_PESQUISA"]) == 1 ? Resources.RsCadastros.ATIVO : Resources.RsCadastros.INATIVO,
                                Data_Hora_Criacao = Convert.ToString(reader["DATAHORA_ENVIO"]),
                                Data_Hora_Respondida = reader["DATAHORA_RESPONDIDA"] != DBNull.Value ? Convert.ToString(reader["DATAHORA_RESPONDIDA"]) : "-"
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderByDescending(p => DateTime.Parse(p.Data_Hora_Criacao));
                            else
                                listaOrdenada = retorno.OrderBy(p => DateTime.Parse(p.Data_Hora_Criacao));
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Pesquisa_SatisfacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        public async Task<GetApiRetorno<List<Pesquisa_PerguntasItemModel>>> GetRespostasPesquisa(Int64 Id_Pesquisa_Resposta)
        {
            GetApiRetorno<List<Pesquisa_PerguntasItemModel>> retorno = new GetApiRetorno<List<Pesquisa_PerguntasItemModel>>();
            retorno.Objeto = new List<Pesquisa_PerguntasItemModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT B.ID_PESQUISA_PERGUNTA AS ID_PERGUNTA, C.ORDEM, C.TIPO_RESPOSTA, ");
                    sql.AppendLine("        C.DESCRICAO, B.RESPOSTA AS RESPOSTA_PERGUNTA ");

                    sql.AppendLine(" FROM TB_PESQUISA_RESPOSTA A ");

                    sql.AppendLine(" INNER JOIN TB_PESQUISA_PERGUNTA_RESPOSTA B ");
                    sql.AppendLine(" ON(B.ID_PESQUISA_RESPOSTA = A.ID) ");

                    sql.AppendLine(" INNER JOIN TB_PESQUISA_PERGUNTA C ");
                    sql.AppendLine(" ON(C.ID = B.ID_PESQUISA_PERGUNTA) ");

                    sql.AppendLine(" INNER JOIN TB_PESQUISA_SATISFACAO D ");
                    sql.AppendLine(" ON(D.ID = A.ID_PESQUISA_SATISFACAO) ");

                    sql.AppendLine(" WHERE A.ID = @ID_RESPOSTA ");
                    sql.AppendLine("   AND C.STATUS = 1 ");

                    parametros.Add("@ID_RESPOSTA", Id_Pesquisa_Resposta);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new Pesquisa_PerguntasItemModel
                            {
                                Id = Convert.ToInt64(reader["ID_PERGUNTA"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),

                                Resposta = Convert.ToString(reader["RESPOSTA_PERGUNTA"]),
                                Tipo_Resposta = (Pesquisa_Tipo_Resposta)reader["TIPO_RESPOSTA"],

                                Ordem = Convert.ToInt32(reader["ORDEM"])
                            });
                        }
                    }
                }
            }

            retorno.Objeto = retorno.Objeto.OrderBy(p => p.Ordem).ToList();

            return retorno;
        }

        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/GetAllRespostas")]
        //Task<GetApiRetorno<Pesquisa_SatisfacaoCadastroModel>>
        public async Task<DataTableResponse<Pesquisa_SatisfacaoListaModel>> GetAllRespostas(Pesquisa_SatisfacaoConsulta_DataTableRequest request)
        {
            List<Pesquisa_SatisfacaoListaModel> retorno = new List<Pesquisa_SatisfacaoListaModel>();
            IOrderedEnumerable<Pesquisa_SatisfacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_PESQUISA_SATISFACAO A ")
                .AppendLine("LEFT JOIN TB_PESQUISA_RESPOSTA B ")
                .AppendLine("  ON A.ID = B.ID_PESQUISA_SATISFACAO ");

                if (!String.IsNullOrEmpty(request.dataIni))
                {
                    sqlFrom.AppendLine(" WHERE A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM");

                    if (!String.IsNullOrEmpty(request.status_Pesquisa))
                    {
                        sqlFrom.AppendLine(" AND A.STATUS = @STATUS");
                        parametros.Add("@STATUS", Convert.ToInt32(request.status_Pesquisa));
                    }

                    parametros.Add("@DATAINI", DateTime.Parse(request.dataIni));
                    parametros.Add("@DATAFIM", DateTime.Parse(request.dataFin));
                }
                else if (!String.IsNullOrEmpty(request.status_Pesquisa))
                {
                    sqlFrom.AppendLine(" WHERE A.STATUS = @STATUS");
                    parametros.Add("@STATUS", Convert.ToInt32(request.status_Pesquisa));
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.TITULO_DA_PESQUISA, A.DISPAROAUTO, A.QUEMPREENCHE, A.STATUS, A.DATAHORACRIACAO,A.USUARIOCRIACAO , ")
                              .AppendLine(" COUNT(B.ID) AS QTDE_ENVIOS, SUM(CASE WHEN B.DATAHORA_RESPONDIDA IS NOT NULL THEN 1 ")
                              .AppendLine(" ELSE 0 END) AS QTDE_RESPOSTAS ")
                    .AppendLine(sqlFrom.ToString())
                    .AppendLine("GROUP BY A.ID, A.TITULO_DA_PESQUISA, A.DISPAROAUTO, A.QUEMPREENCHE, A.STATUS, A.DATAHORACRIACAO,A.USUARIOCRIACAO ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            Int32 _qtde_envios = Convert.ToInt32(reader["QTDE_ENVIOS"]);
                            Int32 _qtde_resposta = Convert.ToInt32(reader["QTDE_RESPOSTAS"]);
                            Double _percentual = _qtde_envios == 0 ? 0 : _qtde_resposta * 100 / _qtde_envios;

                            var obj = new Pesquisa_SatisfacaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Titulo_Da_Pesquisa = Convert.ToString(reader["TITULO_DA_PESQUISA"]),
                                Quem_Preenche = reader["QUEMPREENCHE"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<PesquisaSatisfacaoQuemPreenche>(Convert.ToString(reader["QUEMPREENCHE"])) : String.Empty,
                                Status = reader["STATUS"] == DBNull.Value ? Resources.RsCadastros.INATIVO : Convert.ToInt32(reader["STATUS"]) == 1 ? Resources.RsCadastros.ATIVO : Resources.RsCadastros.INATIVO,
                                DisparoAuto = reader["DISPAROAUTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<PesquisaSatisfacaoDisparoAuto>(Convert.ToString(reader["DISPAROAUTO"])) : String.Empty,
                                Data_Hora_Criacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                Qtde_Envios = _qtde_envios.ToString(),
                                Qtde_Respostas = _qtde_resposta.ToString(),
                                Usuario_Criacao = Convert.ToString(reader["TITULO_DA_PESQUISA"])
                            };
                            obj.Percentual = _percentual.ToString("N2") + "%";

                            retorno.Add(obj);
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Pesquisa_SatisfacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/BuscaPaginaCliente")]
        //Task<GetApiRetorno<Pesquisa_SatisfacaoCadastroModel>>
        public async Task<DataTableResponse<Pesquisa_SatisfacaoListaModel>> BuscaPaginaCliente(Pesquisa_Satisfacao_DataTableRequest request)
        {
            List<Pesquisa_SatisfacaoListaModel> retorno = new List<Pesquisa_SatisfacaoListaModel>();
            IOrderedEnumerable<Pesquisa_SatisfacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_PESQUISA_RESPOSTA A ")
                    .AppendLine(" JOIN TB_CLIENTE B ON A.ID_CLIENTE = B.ID ")
                    .AppendLine(" JOIN TB_PESQUISA_SATISFACAO C ON C.ID = A.ID_PESQUISA_SATISFACAO ")
                    .AppendLine(" WHERE C.STATUS = 1");

                if (request.Id_Cliente != 0)
                    sqlFrom.AppendLine(" AND A.ID_CLIENTE = " + request.Id_Cliente);

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT C.ID, B.NOMEFANTASIA, C.TITULO_DA_PESQUISA, C.STATUS, C.DATAHORACRIACAO, A.DATAHORA_RESPONDIDA ")
                    .AppendLine(sqlFrom.ToString());

                    //Filtrando
                    //if (!String.IsNullOrEmpty(request.search.value))
                    //{
                    //    //sql.AppendLine("AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                    //    //parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    //}


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Pesquisa_SatisfacaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Titulo_Da_Pesquisa = Convert.ToString(reader["TITULO_DA_PESQUISA"]),
                                Nome = Convert.ToString(reader["NOMEFANTASIA"]),
                                Data_Hora_Criacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                Data_Hora_Respondida = reader["DATAHORA_RESPONDIDA"] != DBNull.Value ? Convert.ToString(reader["DATAHORA_RESPONDIDA"]) : null
                            });
                        }
                    }

                }
            }

            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Pesquisa_SatisfacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        public async Task<GetApiRetorno<Qtde_Envios_Respostas_Complex_Data>> GetIndicadorQtdeEnvios(Int64 Id_Pesquisa_Satisfacao)
        {
            GetApiRetorno<Qtde_Envios_Respostas_Complex_Data> retorno = new GetApiRetorno<Qtde_Envios_Respostas_Complex_Data>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    Qtde_Envios_Respostas_Complex_Data Bar_Chart = new Qtde_Envios_Respostas_Complex_Data();
                    Bar_Chart.Labels = new List<String>();

                    ComplexDataset complexDatasetEnvios = new ComplexDataset
                    {
                        Label = RsCadastros.QTDE_ENVIOS,
                        FillColor = "#FF9900",
                        StrokeColor = "#FF9900",
                        PointColor = "#FF9900",
                        PointStrokeColor = "#FFF",
                        PointHighlightFill = "#FFF",
                        PointHighlightStroke = "#FF9900"
                    };

                    ComplexDataset complexDatasetRespostas = new ComplexDataset
                    {
                        Label = RsCadastros.QTDE_RESPOSTAS,
                        FillColor = "#228822",
                        StrokeColor = "#228822",
                        PointColor = "#228822",
                        PointStrokeColor = "#FFF",
                        PointHighlightFill = "#FFF",
                        PointHighlightStroke = "#228822"
                    };

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT EXTRACT(MONTH FROM B.DATAHORACRIACAO) AS MES, EXTRACT(YEAR FROM B.DATAHORACRIACAO) AS ANO, COUNT(B.ID) AS QTDE_ENVIADO, ");
                    sql.AppendLine(" SUM(CASE ");
                    sql.AppendLine("    WHEN B.DATAHORA_RESPONDIDA IS NOT NULL ");
                    sql.AppendLine("        THEN 1 ");

                    sql.AppendLine("        ELSE 0 ");
                    sql.AppendLine("    END) as QTDE_RESPOSTAS ");

                    sql.AppendLine(" FROM TB_PESQUISA_SATISFACAO A ");

                    sql.AppendLine(" INNER JOIN TB_PESQUISA_RESPOSTA B ");
                    sql.AppendLine(" ON(B.ID_PESQUISA_SATISFACAO = A.ID) ");

                    sql.AppendLine(" WHERE A.ID = @ID_PESQUISA_SATISFACAO AND ");
                    sql.AppendLine(" B.DATAHORACRIACAO BETWEEN DATEADD(MONTH, -12, @DATA_INI_MES) AND CURRENT_TIMESTAMP ");

                    sql.AppendLine(" GROUP BY A.ID, MES, ANO ");

                    DateTime _dateNow = DateTime.Now;

                    parametros.Add("@DATA_INI_MES", new DateTime(_dateNow.Year, _dateNow.Month, 1));
                    parametros.Add("@ID_PESQUISA_SATISFACAO", Id_Pesquisa_Satisfacao);

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, 12))
                    {
                        while (await reader.ReadAsync())
                        {
                            Int32 _mes = Convert.ToInt32(reader["MES"]);
                            Int32 _ano = Convert.ToInt32(reader["ANO"]);

                            DateTime _date = new DateTime(_ano, _mes, 1);

                            Bar_Chart.Labels.Add(_date.ToString("MMM/yyyy"));

                            Double qtdeEnviado = Convert.ToDouble(reader["QTDE_ENVIADO"]);
                            complexDatasetEnvios.Data.Add(qtdeEnviado);

                            Double qtdeRespostas = Convert.ToDouble(reader["QTDE_RESPOSTAS"]);
                            complexDatasetRespostas.Data.Add(qtdeRespostas);
                        }
                    }

                    Bar_Chart.Datasets = new List<ComplexDataset>
                    {
                        complexDatasetEnvios,
                        complexDatasetRespostas
                    };

                    retorno.Objeto = Bar_Chart;
                    retorno.Ok = true;
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Pesquisa_SatisfacaoApi/GetRespostasIndicadores")]
        public async Task<GetApiRetorno<List<Data_Pergunta>>> GetRespostasIndicadores(Int64 Id_Pesquisa_Satisfacao, Int64 Periodo_Filtro, Int64? Id_Pergunta)
        {
            GetApiRetorno<List<Data_Pergunta>> retorno = new GetApiRetorno<List<Data_Pergunta>>();
            retorno.Objeto = new List<Data_Pergunta>();

            var pergunta_resposta = new List<Pergunta_Resposta>();

            IList<String> cores = new List<String>
            {
                "#ED7D31",
                "#2F5597",
                "#BF0000",
                "#FFC000",
                "#6AA443",
                "#7030A0",
                "#997300",
                "#FF3399",
                "#262626",
                "#A5A5A5"
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.TIPO_RESPOSTA, B.ID_PESQUISA_PERGUNTA, B.RESPOSTA ");

                    sql.AppendLine(" FROM TB_PESQUISA_PERGUNTA A ");

                    sql.AppendLine(" INNER JOIN TB_PESQUISA_PERGUNTA_RESPOSTA B ");
                    sql.AppendLine(" ON(A.ID = B.ID_PESQUISA_PERGUNTA) ");

                    sql.AppendLine(" INNER JOIN TB_PESQUISA_RESPOSTA C ");
                    sql.AppendLine(" ON(C.ID = B.ID_PESQUISA_RESPOSTA) ");

                    sql.AppendLine(" WHERE A.ID_PESQUISA_SATISFACAO = @ID_PESQUISA_SATISFACAO ");

                    if (Periodo_Filtro != -1)
                    {
                        sql.AppendLine(" AND C.DATAHORA_RESPONDIDA BETWEEN DATEADD(MONTH, @PERIODO_FILTRO, @DATA_INI_MES) AND CURRENT_TIMESTAMP ");

                        DateTime _data = DateTime.Now;

                        parametros.Add("@PERIODO_FILTRO", -Periodo_Filtro);
                        parametros.Add("@DATA_INI_MES", new DateTime(_data.Year, _data.Month, 1));
                    }

                    if (Id_Pergunta.HasValue)
                    {
                        sql.AppendLine(" AND B.ID_PESQUISA_PERGUNTA = @ID_PERGUNTA ");
                        parametros.Add("@ID_PERGUNTA", Id_Pergunta.Value);
                    }

                    parametros.Add("@ID_PESQUISA_SATISFACAO", Id_Pesquisa_Satisfacao);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            pergunta_resposta.Add(new Pergunta_Resposta
                            {
                                Id_pergunta = Convert.ToInt64(reader["ID_PESQUISA_PERGUNTA"]),
                                Resposta = Convert.ToString(reader["RESPOSTA"])
                            });
                        }
                    }

                    Int32 indice_cor = 0;

                    var ids_unicos = pergunta_resposta.Select(p => p.Id_pergunta).Distinct();

                    foreach (var id in ids_unicos)
                    {
                        var _obj = new Data_Pergunta();

                        _obj.Obj_Grafico = new List<SimpleData>();
                        _obj.Obj_Lista_Valores = new List<Value_Label>();

                        _obj.Id_Pergunta = id;

                        var pesquisa_pergunta = await new Conexao<Pesquisa_Pergunta>(session).selectFirst(p => p.Id == id);

                        _obj.Descricao_Pergunta = pesquisa_pergunta.Descricao;
                        _obj.Tipo_Pergunta = HtmlEnumExtensions.GetEnumDescription<Pesquisa_Tipo_Resposta>(pesquisa_pergunta.Tipo_Resposta);

                        var respostas_diferentes = pergunta_resposta.Where(p => p.Id_pergunta == id).Select(p => p.Resposta).Distinct();

                        foreach (var resposta in respostas_diferentes)
                        {
                            var qtd_respostas_iguais = pergunta_resposta.Count(p => p.Id_pergunta == id && p.Resposta == resposta);

                            _obj.Obj_Grafico.Add(new SimpleData
                            {
                                Color = cores[indice_cor],
                                Highlight = cores[indice_cor],
                                Label = resposta,
                                Value = qtd_respostas_iguais
                            });

                            _obj.Obj_Lista_Valores.Add(new Value_Label
                            {
                                Label = resposta,
                                Value = qtd_respostas_iguais
                            });

                            indice_cor++;
                        }

                        indice_cor = 0;

                        _obj.Obj_Lista_Valores = _obj.Obj_Lista_Valores.OrderByDescending(p => p.Value).ToList();
                        retorno.Objeto.Add(_obj);
                    }

                }
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/Pesquisa_SatisfacaoApi/GetRespostasIndicadoresNovo")]
        public GetApiRetorno<GrafanaRequestModel> GetRespostasIndicadoresNovo(string param = "test_grafana")
        {
            var retorno = new GetApiRetorno<GrafanaRequestModel>();
            try
            {
                var uri = new Uri(CONSTANTES.URLGRAFANA);

                //grafana configurations /org/apikeys
                var apiKey = CONSTANTES.APIKEY_GRAFANA;

                var annotationService = new AnnotationService(uri, apiKey);

                var dashboardService = new DashboardService(uri, apiKey);

                //find dashboard id
                //Uid may be edited in dashboard settings
                var dashboardResponse = dashboardService.GetDashboardByUid(param);

                var dashboardId = dashboardResponse.Dashboard.Id;

                //Epoch datetime in milliseconds
                //var start = DateTime.UtcNow.AddMinutes(-15).ToGrafanaTimestamp();

                //Epoch datetime in milliseconds
                //var stop = DateTime.UtcNow.ToGrafanaTimestamp();

                //create annotation 
                var messageResponse = annotationService.Create(new CreateAnnotationRequest
                {
                    DashboardId = dashboardId,
                    IsRegion = true,
                    Tags = new[] { "deploy", Environment.MachineName },
                    Text = $"Deploy on {Environment.MachineName}",
                    //Time = start,
                    //TimeEnd = stop
                });
                retorno.Objeto = new GrafanaRequestModel { servicoAnotacoes = messageResponse, DashboardAnotacoes = dashboardResponse };
            }
            catch (Exception exception)
            {
                retorno.Mensagem = exception?.Message;
                retorno.InnerException = exception?.InnerException?.Message;
                retorno.Objeto = new GrafanaRequestModel();
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Pesquisa_SatisfacaoApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PESQUISA_SATISFACAO A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID,A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["DESCRICAO"])}"
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }




        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/Historico_Envio")]
        public async Task<DataTableResponse<ClientePesquisaListViewModel>> Historico_Envio(ClientePesquisa_DataTableRequest request)
        {
            List<ClientePesquisaListViewModel> retorno = new List<ClientePesquisaListViewModel>();
            IOrderedEnumerable<ClientePesquisaListViewModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_PESQUISA_RESPOSTA A ")
                                                  .AppendLine("INNER JOIN TB_CLIENTE B  ")
                                                  .AppendLine(" ON B.ID =A.ID_CLIENTE ")
                                                  .AppendLine("INNER JOIN TB_PESQUISA_SATISFACAO C  ")
                                                  .AppendLine(" ON C.ID =A.ID_PESQUISA_SATISFACAO ")
                                                  .AppendLine("WHERE A.ID > 0");




                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID,A.DATAHORACRIACAO,B.NOMEFANTASIA,B.EMAIL_PRINCIPAL ,C.DESCRICAO,A.OBS ");

                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ClientePesquisaListViewModel
                            {
                                Id_Pesquisa_Satisfacao = Convert.ToInt64(reader["ID"]),
                                NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                Email = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Obs = Convert.ToString(reader["OBS"]),
                                DataCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm:ss") : null,

                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 0:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id_Pesquisa_Satisfacao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id_Pesquisa_Satisfacao);
                            break;
                        }
                    case 1:     // TipoPessoa
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.NomeFantasia);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.NomeFantasia);
                            break;
                        }
                    case 2:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                    case 3:     // Obs
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Obs);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Obs);
                            break;
                        }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ClientePesquisaListViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/GetGrafanaEnviada")]
        public async Task<DataTableResponse<GarfanaEnviadaRequestModel>> GetGrafanaEnviada(Int64 id, DateTime dataInicial, DateTime dataFinal)
        {
            List<GarfanaEnviadaRequestModel> retorno = new List<GarfanaEnviadaRequestModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_PESQUISA_RESPOSTA A ")
                                                .AppendLine("WHERE A.ID > 0 ");

                ////Filtrando
                if (!String.IsNullOrEmpty(Convert.ToString(dataInicial)))
                {
                    sqlFrom.AppendLine("AND CAST(A.DATAHORACRIACAO AS DATE)  BETWEEN  @DATAINCIAL AND @DATAFINAL");
                    parametros.Add("@DATAINCIAL", dataInicial);
                    parametros.Add("@DATAFINAL", dataFinal);
                }

                if (!String.IsNullOrEmpty(Convert.ToString(id)))
                {
                    sqlFrom.AppendLine("AND (A.ID_PESQUISA_SATISFACAO  = @IDPESQUISA)");
                    parametros.Add("@IDPESQUISA", Convert.ToInt32(id));
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT SUM(CASE WHEN DATAHORA_RESPONDIDA IS NOT NULL THEN 1 ELSE 0 END)AS RESPONDIDA , COUNT(A.ID)AS ENVIADA ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new GarfanaEnviadaRequestModel
                            {
                                Respondida = Convert.ToString(reader["RESPONDIDA"]),
                                Enviada = Convert.ToString(reader["ENVIADA"]),

                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<GarfanaEnviadaRequestModel>
            {
                //draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/GetGrafanaEnviadaPeriodo")]
        public async Task<DataTableResponse<GarfanaEnviadaRequestModel>> GetGrafanaEnviadaPeriodo(Int64 id)
        {
            List<GarfanaEnviadaRequestModel> retorno = new List<GarfanaEnviadaRequestModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_PESQUISA_RESPOSTA A ")
                                                .AppendLine("WHERE A.ID > 0 AND A.DATAHORACRIACAO IS NOT NULL  ");

                ////Filtrando


                if (!String.IsNullOrEmpty(Convert.ToString(id)))
                {
                    sqlFrom.AppendLine("AND (A.ID_PESQUISA_SATISFACAO  = @IDPESQUISA)");
                    parametros.Add("@IDPESQUISA", Convert.ToInt32(id));
                }

                sqlFrom.AppendLine("GROUP BY (extract(month  from CAST(A.DATAHORACRIACAO AS DATE))|| '/'|| extract(year  from CAST(A.DATAHORACRIACAO AS DATE))) ");

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT SUM(CASE WHEN DATAHORA_RESPONDIDA IS NOT NULL THEN 1 ELSE 0 END)AS RESPONDIDA , COUNT(A.ID)AS ENVIADA,\t(extract(month  from CAST(A.DATAHORACRIACAO AS DATE))|| '/'|| extract(year  from CAST(A.DATAHORACRIACAO AS DATE))) AS MESANO ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new GarfanaEnviadaRequestModel
                            {
                                Respondida = Convert.ToString(reader["RESPONDIDA"]),
                                Enviada = Convert.ToString(reader["ENVIADA"]),
                                MesAno = Convert.ToString(reader["MESANO"]),

                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<GarfanaEnviadaRequestModel>
            {
                //draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/pesquisa_Satisfacaoapi/GetGrafanaPerguntas")]
        public async Task<DataTableResponse<GarfanaRequestModel>> GetGrafanaPerguntas(Int64 id, DateTime dataInicial, DateTime dataFinal)
        {
            List<GarfanaRequestModel> retorno = new List<GarfanaRequestModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_PESQUISA_PERGUNTA A ")
                                                .AppendLine("INNER  JOIN TB_PESQUISA_PERGUNTA_RESPOSTA B ")
                                                .AppendLine("ON B.ID_PESQUISA_PERGUNTA = A.ID ")
                                                .AppendLine("LEFT  JOIN TB_PESQUISA_RESPOSTA C ")
                                                .AppendLine("ON B.ID_PESQUISA_RESPOSTA = C.ID ")
                                                .AppendLine("WHERE A.ID > 0 ");


                ////Filtrando
                if (!String.IsNullOrEmpty(Convert.ToString(id)))
                {
                    sqlFrom.AppendLine("AND (A.ID_PESQUISA_SATISFACAO  = @IDPESQUISA)");
                    parametros.Add("@IDPESQUISA", Convert.ToInt32(id));
                }

                if (!String.IsNullOrEmpty(Convert.ToString(dataInicial)))
                {
                    sqlFrom.AppendLine("AND CAST(B.DATAHORACRIACAO AS DATE)  BETWEEN  @DATAINCIAL AND @DATAFINAL");
                    parametros.Add("@DATAINCIAL", dataInicial);
                    parametros.Add("@DATAFINAL", dataFinal);
                }
                sqlFrom.AppendLine("GROUP BY A.TIPO_RESPOSTA,B.ID_PESQUISA_PERGUNTA,A.DESCRICAO,B.RESPOSTA ");

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.TIPO_RESPOSTA,B.ID_PESQUISA_PERGUNTA,A.DESCRICAO,B.RESPOSTA,COUNT(B.RESPOSTA) AS TOTAL ");
                    sql.AppendLine(sqlFrom.ToString());



                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new GarfanaRequestModel
                            {
                                Id_Pergunta = Convert.ToInt64(reader["ID_PESQUISA_PERGUNTA"]),
                                Tipo_Pergunta = Convert.ToString(reader["DESCRICAO"]),
                                Id_Resposta = Convert.ToInt64(reader["TIPO_RESPOSTA"]),
                                Tipo_Resposta = Convert.ToString(reader["RESPOSTA"]),
                                Total = Convert.ToInt64(reader["TOTAL"]),
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<GarfanaRequestModel>
            {
                //draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }






    }
}
