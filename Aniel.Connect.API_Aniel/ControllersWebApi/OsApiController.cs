using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfigUrl;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PesquisaSatisfacaoRepository;
using Aniel.Connect.Repository.Repository.T5.DocumentoProducaoFoto;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using MySqlX.XDevAPI;
using NHibernate;
using NHibernate.Cache;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ISession = NHibernate.ISession;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class OsApiController : BaseApiController
    {
        private static readonly MemoryCache _cache = MemoryCache.Default;

        /// <summary>
        ///Api para Criar Novas Os
        ///</summary>
        ///<param name = "request"> </param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/OsApiController/CriarOrdemServico")]
        public async Task<PostApiRetorno> CriarOs(OsApiRequest request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    DocumentoProducaoAnielBusiness docBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                    var responseCriarOsIntegracao = await docBusiness.CriarOsIntegracao(BindToOsApiModel(request));
                    retorno.Ok = responseCriarOsIntegracao.result;
                    retorno.Mensagem = responseCriarOsIntegracao.message;
                }
            }
            catch (Exception erro)
            {
                CONSTANTES.log4Net.Error(erro.Message, erro);
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Ok = false;
            }
            return retorno;
        }

        private OsApiModel BindToOsApiModel(OsApiRequest request)
        {
            if (request == null) return null;

            return new OsApiModel
            {
                Cpf = request.Cpf,
                CodigoIntegratorTipoServico = request.CodigoIntegratorTipoServico,
                TipoServico = request.TipoServico,
                SubTipoServico = request.SubTipoServico,
                Origem = request.Origem,
                Projeto = request.Projeto,
                CodCt = request.CodCt,
                NumOS = request.NumOS,
                Uni = request.Uni,
                DataHoraAgendamento = request.DataHoraAgendamento,
                TipoImovel = request.TipoImovel,
                GrupoArea = request.GrupoArea,
                Area = request.Area,
                Localidade = request.Localidade,
                Endereco = request.Endereco,
                NumeroEndereco = request.NumeroEndereco,
                Cep = request.Cep,
                Complemento = request.Complemento,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                PontoReferencia = request.PontoReferencia,
                UF = request.UF,
                Observacao = request.Observacao,
                Facilidade = request.Facilidade,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Tecnico = request.Tecnico,
                NomeCliente = request.NomeCliente,
                TelefoneCelularCliente = request.TelefoneCelularCliente,
                TelefoneFixoCliente = request.TelefoneFixoCliente,
                EmailCliente = request.EmailCliente,
                LoginRadius = request.LoginRadius,
                SenhaRadius = request.SenhaRadius,
                ContratoCliente = request.ContratoCliente,
                ContratoClienteAnterior = request.ContratoClienteAnterior,
                CategoriaCliente = request.CategoriaCliente,
                NumDoc = request.NumDoc,
                CadastrarSequencia = request.CadastrarSequencia,
                Wo = request.Wo,
                DataCadastro = request.DataCadastro,
                DataEncerramento = request.DataEncerramento,
                DataAgendamento = request.DataAgendamento,
                DataVencimento = request.DataVencimento,
                DataAbertura = Convert.ToDateTime(request.DataAbertura),
                Segmento = request.Segmento,
                Departamento = request.Departamento,
                Prioridade = request.Prioridade,
                Plano = request.Plano,
                DescricaoPlanoProduto = request.DescricaoPlanoProduto,
                PlanoAnterior = request.PlanoAnterior,
                PlanosAdicionais = request.PlanosAdicionais,
                Adicional1 = request.Adicional1,
                Adicional2 = request.Adicional2,
                Adicional3 = request.Adicional3,
                Adicional4 = request.Adicional4,
                Adicional5 = request.Adicional5,
                Adicional6 = request.Adicional6,
                Adicional7 = request.Adicional7,
                Adicional8 = request.Adicional8,
                Adicional9 = request.Adicional9,
                Adicional10 = request.Adicional10,
                Cod_Encerramento_TOA = request.Cod_Encerramento_TOA,
                Tipo = request.Tipo,
                IdPop = request.IdPop,
                IdOlt = request.IdOlt,
                IdElementoRede = request.IdElementoRede,
                CaixaFTTH = request.CaixaFTTH,
                PortaFTTH = request.PortaFTTH,
                UsarNumeroSequencial = request.UsarNumeroSequencial,
                CodigoAuxiliarLogin = request.CodigoAuxiliarLogin,
                UsuarioIntegracao = String.IsNullOrEmpty(request.UsuarioCriacao) ? request.settings.user : request.UsuarioCriacao,
                InformacoesComplementares = request.InformacoesComplementares,
                Tecnologia = request.Tecnologia,
                NaoCriaOsSemTipo = request.NaoCriarOsSemTipo
            };
        }

        [HttpPost()]
        [Route("api/OsApiController/ConsultarOS")]
        public async Task<DataTableResponse<OsApiConsulta>> ConsultarOS(OsConsultaApiRequest request)
        {
            List<OsApiConsulta> retorno = new List<OsApiConsulta>();

            try
            {
                Int32 recordsTotal = 0;
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                string chaveCache = $"Filtros";

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_DOCUMENTO_PRODUCAO A ")
                                                    .AppendLine("LEFT JOIN TB_AREA B   ")
                                                    .AppendLine("ON(B.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = B.COD_ESCRITORIO AND A.COD_AREA = B.COD_AREA)  ")
                                                    .AppendLine("LEFT JOIN TB_LOCALIDADE C  ")
                                                    .AppendLine("ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO AND A.COD_AREA = C.COD_AREA AND A.COD_LOCALIDADE = C.COD_LOCALIDADE)  ")
                                                    .AppendLine("LEFT JOIN TB_ESCRITORIO D ")
                                                    .AppendLine("ON D.COD_ESCRITORIO =A.COD_ESCRITORIO ")
                                                    .AppendLine("LEFT JOIN TB_PLANO_PRODUTO E ")
                                                    .AppendLine("ON E.ID =A.ID_PLANOPRODUTO  ")
                                                    .AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE F  ")
                                                    .AppendLine("ON (F.COD_TIPO_SERV = A.COD_TIPO_SERV) ")
                                                    .AppendLine("LEFT JOIN TB_SUB_TIPO_SERVICO G  ")
                                                    .AppendLine("ON (G.COD_TIPO_SERV =A.COD_TIPO_SERV AND G.COD_SUB_TIPO_SERV = A.COD_SUB_TIPO_SERV) ")
                                                    .AppendLine("WHERE A.NUM_DOC > 0 ");

                    ////Filtrando
                    if (!String.IsNullOrEmpty(request.CodCt))
                    {
                        sqlFrom.AppendLine("AND ( A.CODCT = @CODCT)");
                        parametros.Add("@CODCT", StringService.Truncate(Convert.ToString(request.CodCt), 4));
                        chaveCache = string.Concat(chaveCache, $"/{request.CodCt}");
                    }

                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        sqlFrom.AppendLine("AND ( A.PROJETO = @PROJETO)");
                        parametros.Add("@PROJETO", StringService.Truncate(Convert.ToString(request.Projeto), 20));
                        chaveCache = string.Concat(chaveCache, $"/{request.Projeto}");
                    }

                    if (!String.IsNullOrEmpty(request.NumObra))
                    {
                        sqlFrom.AppendLine("AND ( A.NUM_OBRA = @NUM_OBRA)");
                        parametros.Add("@NUM_OBRA", StringService.Truncate(Convert.ToString(request.NumObra), 50));
                        chaveCache = string.Concat(chaveCache, $"/{request.NumObra}");
                    }

                    if ((request.NumDoc != null))
                    {
                        sqlFrom.AppendLine("AND ( A.NUM_DOC = @NUM_DOC)");
                        parametros.Add("@NUM_DOC", Convert.ToInt64(request.NumDoc));
                        chaveCache = string.Concat(chaveCache, $"/{request.NumDoc}");
                    }

                    if (!String.IsNullOrEmpty(request.Equipe))
                    {
                        sqlFrom.AppendLine("AND ( A.EQUIPE = @EQUIPE)");
                        parametros.Add("@EQUIPE", StringService.Truncate(Convert.ToString(request.Equipe), 10));
                        chaveCache = string.Concat(chaveCache, $"/{request.Equipe}");
                    }

                    if (!String.IsNullOrEmpty(request.Area))
                    {
                        sqlFrom.AppendLine("AND ( A.COD_AREA = @COD_AREA)");
                        parametros.Add("@COD_AREA", StringService.Truncate(Convert.ToString(request.Area), 4));
                        chaveCache = string.Concat(chaveCache, $"/{request.Area}");
                    }

                    if (!String.IsNullOrEmpty(request.Localidade))
                    {
                        sqlFrom.AppendLine("AND ( A.COD_LOCALIDADE = @COD_LOCALIDADE)");
                        parametros.Add("@COD_LOCALIDADE", Convert.ToString(request.Localidade));
                        chaveCache = string.Concat(chaveCache, $"/{request.Localidade}");
                    }

                    if (!String.IsNullOrEmpty(request.TipoServico))
                    {
                        sqlFrom.AppendLine("AND ( A.COD_TIPO_SERV = @COD_TIPO_SERV)");
                        parametros.Add("@COD_TIPO_SERV", Convert.ToString(request.TipoServico));
                        chaveCache = string.Concat(chaveCache, $"/{request.TipoServico}");
                    }

                    if (!String.IsNullOrEmpty(request.SubTipoServico))
                    {
                        sqlFrom.AppendLine("AND ( G.DESCRICAO = @SUB_TIPO_SERV)"); // TB_SUB_TIPO_SERVICO
                        parametros.Add("@SUB_TIPO_SERV", StringService.Truncate(Convert.ToString(request.SubTipoServico), 230));
                        chaveCache = string.Concat(chaveCache, $"/{request.SubTipoServico}");
                    }

                    if (!String.IsNullOrEmpty(request.Dia))
                    {
                        sqlFrom.AppendLine("AND ( A.DIA = @DIA)");
                        parametros.Add("@DIA", Convert.ToDateTime(request.Dia));
                        chaveCache = string.Concat(chaveCache, $"/{request.Dia}");
                    }

                    if (!String.IsNullOrEmpty(request.Hora))
                    {
                        sqlFrom.AppendLine("AND ( A.HORA = @HORA)");
                        parametros.Add("@HORA", Convert.ToDateTime(request.Hora));
                        chaveCache = string.Concat(chaveCache, $"/{request.Hora}");
                    }


                    if (!String.IsNullOrEmpty(request.HoraMaxima))
                    {
                        sqlFrom.AppendLine("AND ( A.HORA_MAXIMA = @HORA_MAXIMA)");
                        parametros.Add("@HORA_MAXIMA", Convert.ToDateTime(request.HoraMaxima));
                        chaveCache = string.Concat(chaveCache, $"/{request.HoraMaxima}");
                    }

                    if (!String.IsNullOrEmpty(request.DataMaxima))
                    {
                        sqlFrom.AppendLine("AND ( A.DATA_MAXIMA = @DATA_MAXIMA)");
                        parametros.Add("@DATA_MAXIMA", Convert.ToDateTime(request.DataMaxima));
                        chaveCache = string.Concat(chaveCache, $"/{request.DataMaxima}");
                    }

                    if (!String.IsNullOrEmpty(request.Escritorio))
                    {
                        sqlFrom.AppendLine("AND ( A.COD_ESCRITORIO = @COD_ESCRITORIO)");
                        parametros.Add("@COD_ESCRITORIO", StringService.Truncate(Convert.ToString(request.Escritorio), 4));
                        chaveCache = string.Concat(chaveCache, $"/{request.Escritorio}");
                    }

                    if (!String.IsNullOrEmpty(request.TipoImovel))
                    {
                        sqlFrom.AppendLine("AND ( A.ENDERECO_TIPO = @ENDERECO_TIPO)");
                        parametros.Add("@ENDERECO_TIPO", StringService.Truncate(Convert.ToString(request.TipoImovel), 100));
                        chaveCache = string.Concat(chaveCache, $"/{request.TipoImovel}");
                    }

                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        sqlFrom.AppendLine("AND ( A.ENDERECO_BAIRRO = @ENDERECO_BAIRRO)");
                        parametros.Add("@ENDERECO_BAIRRO", StringService.Truncate(Convert.ToString(request.Bairro), 100));
                        chaveCache = string.Concat(chaveCache, $"/{request.Bairro}");
                    }


                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sqlFrom.AppendLine("AND ( A.ENDERECO_UF = @ENDERECO_UF)");
                        parametros.Add("@ENDERECO_UF", StringService.Truncate(Convert.ToString(request.UF), 2));
                        chaveCache = string.Concat(chaveCache, $"/{request.UF}");
                    }

                    if (!String.IsNullOrEmpty(request.Num_Obra_Original))
                    {
                        sqlFrom.AppendLine("AND ( A.NUM_OBRA_ORIGINAL = @NUM_OBRA_ORIGINAL)");
                        parametros.Add("@NUM_OBRA_ORIGINAL", StringService.Truncate(Convert.ToString(request.Num_Obra_Original), 50));
                        chaveCache = string.Concat(chaveCache, $"/{request.Num_Obra_Original}");
                    }

                    if (!String.IsNullOrEmpty(request.Status_Execucao))
                    {
                        sqlFrom.AppendLine(" AND ( A.STATUS_EXECUCAO = @STATUS_EXECUCAO) ");
                        parametros.Add("@STATUS_EXECUCAO", Convert.ToString(request.Status_Execucao));
                        chaveCache = string.Concat(chaveCache, $"/{request.Status_Execucao}");
                    }

                    if (!String.IsNullOrEmpty(request.Responsabilidade))
                    {
                        sqlFrom.AppendLine("AND ( A.RESPONSABILIDADE= @RESPONSABILIDADE)");
                        parametros.Add("@RESPONSABILIDADE", Convert.ToString(request.Responsabilidade));
                        chaveCache = string.Concat(chaveCache, $"/{request.Responsabilidade}");
                    }

                    if (request.IdCliente != null)
                    {
                        sqlFrom.AppendLine("AND ( A.ID_CLIENTE= @ID_CLIENTE)");
                        parametros.Add("@ID_CLIENTE", Convert.ToString(request.IdCliente));
                        chaveCache = string.Concat(chaveCache, $"/{request.IdCliente}");
                    }

                    if (request.IdAtendimentoConnect != null)
                    {
                        sqlFrom.AppendLine("AND ( A.ID_ATENDIMENTO_CONNECT= @ID_ATENDIMENTO_CONNECT)");
                        parametros.Add("@ID_ATENDIMENTO_CONNECT", Convert.ToString(request.IdAtendimentoConnect));
                        chaveCache = string.Concat(chaveCache, $"/{request.IdAtendimentoConnect}");
                    }

                    if (request.IdVenda != null)
                    {
                        sqlFrom.AppendLine("AND ( A.ID_VENDA= @ID_VENDA)");
                        parametros.Add("@ID_VENDA", Convert.ToString(request.IdVenda));
                        chaveCache = string.Concat(chaveCache, $"/{request.IdVenda}");
                    }

                    if (request.IdLogin != null)
                    {
                        sqlFrom.AppendLine("AND ( A.ID_LOGIN= @ID_LOGIN)");
                        parametros.Add("@ID_LOGIN", Convert.ToString(request.IdLogin));
                        chaveCache = string.Concat(chaveCache, $"/{request.IdLogin}");
                    }

                    if (request.IdAtendimentoConnect != null)
                    {
                        sqlFrom.AppendLine("AND ( A.ID_CLIENTE_CONTRATOCONNECT= @ID_CLIENTE_CONTRATOCONNECT)");
                        parametros.Add("@ID_CLIENTE_CONTRATOCONNECT", Convert.ToString(request.IdAtendimentoConnect));
                        chaveCache = string.Concat(chaveCache, $"/{request.IdAtendimentoConnect}");
                    }

                    if (_cache.Contains(chaveCache))
                    {
                        var listaCache = (List<OsApiConsulta>)_cache.Get(chaveCache);

                        return new DataTableResponse<OsApiConsulta>
                        {
                            recordsFiltered = listaCache.Count(),
                            data = listaCache,
                            error = ""
                        };
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.USUARIO_ALTERACAO,A.DIA_ALTERACAO,A.HORA_ALTERACAO,A.DIA,A.HORA,A.CODCT,A.PROJETO,A.NUM_OBRA, ");
                        sql.AppendLine("A.NUM_DOC,A.NUM_PROTOCOLO,A.ID_CLIENTE,A.TITULAR,A.CPF_CLIENTE,A.RG_CIENTE,A.RG_ASSINANTE, ");
                        sql.AppendLine("A.EMAIL_ASSINANTE,A.VINCULO_CIENTE,A.DOCUMENTO_CIENTE,A.ENDERECO,A.ENDERECO_NUMERO,A.ENDERECO_COMPL, ");
                        sql.AppendLine("A.ENDERECO_CPL,A.ENDERECO_BAIRRO,A.ENDERECO_UF,A.ENDERECO_PAIS,A.ENDERECO_TIPO,A.LONGITUDE_STR, ");
                        sql.AppendLine("A.LATITUDE_STR,A.COD_AREA,B.DESCRICAO AS AREA,A.COD_LOCALIDADE,C.LOCALIDADE AS LOCALIDADE, ");
                        sql.AppendLine("A.COD_ESCRITORIO,D.DESCRICAO AS ESCRITORIO,A.TEL_CIENTE,A.TEL_TITULAR,A.CONTATO,A.CONTATO_TELEFONE, ");
                        sql.AppendLine("A.TEL_COMERCIAL,A.TELEFONE_INSTALACAO,A.ID_PLANOPRODUTO,A.ID_CLIENTE_PLANOPRODUTOCONNECT, ");
                        sql.AppendLine("E.DESCRICAO AS PLANOPRODUTO,A.DESCRICAO_PLANO_ASSINANTE,A.LOCAL,A.ID_LOGIN,A.DSLAN, ");
                        sql.AppendLine("A.PPPOE,A.PORTA,A.CX_DE,A.CX_PARA,A.CABO_DE,A.CABO_PARA,A.ESTACAO,A.ESTACAO_PARA, ");
                        sql.AppendLine("A.SS_DE,A.SS_PARA,A.EQN_DE,A.EQN_PARA,A.PARPRIM_PARA,A.STATUS_PROD,A.LIC_DE,A.LIC_PARA, ");
                        sql.AppendLine("A.END_CAIXA,A.END_ARM,A.ROTERIZAR,A.DATAHORA_DESLOCAMENTO,A.DATAHORA_INI_ATEND,A.DATAHORA_FIM_ATEND, ");
                        sql.AppendLine("A.USUARIO_CANCELOU,A.DATA_HORA_CANCELOU,A.TEMPO_REAL_DESLOCAMENTO,A.TEMPO_DESLOCAMENTO,A.GEOSEGMENTO, ");
                        sql.AppendLine("A.DATA_PROD,A.VALIDADO,A.HORA_MAXIMA,A.DATA_MAXIMA,A.COD_TIPO_SERV,F.DESCRICAO AS TIPOSERVICO,A.COD_SUB_TIPO_SERV, ");
                        sql.AppendLine("G.DESCRICAO AS SUBTIPOSERVICO,A.COD_ENCERRAMENTO,A.HORA_ENCERRAMENTO,A.OBSERVACAO,A.USUARIO,A.USUARIO_BAIXA, ");
                        sql.AppendLine("A.UNIDADE_CUSTO,A.NUM_RVT,A.TAREFA,A.EQUIPE,A.ARD,A.TIPO_NOTA,A.COMUNIDADE,A.NUM_DOC_ENTREGA,A.POSTO, ");
                        sql.AppendLine("A.TEM_TESTE_ELETRICO,A.TESTE_ELETRICOS,A.OBSERVACAO_TESTE,A.DATA_INDICADOR,A.INSAPOIO55,A.OBSERVACAO_GASTO, ");
                        sql.AppendLine("A.OXBDPEE,A.SIGLA_MODULO,A.ORIGEM,A.SEGMENTO,A.HORA_INDICADOR,A.NUM_TAREFA,A.TIPO_RECLAMACAO,A.SEQ, ");
                        sql.AppendLine("A.NUM_OBRA_ORIGINAL,A.STATUS_EXECUCAO,A.PROTOCOLO_NE,A.COD_TURNO_SERV,A.DEVOLVIDA,A.OBSERVACAO_AUX, ");
                        sql.AppendLine("A.LNBF,A.GRUPO,A.OBSERVACAO_DOCUMENTO,A.TX,A.RX,A.TIPO_BAIXA,A.RETORNO_TELCO,A.SENHA_BAIXA,A.RESPONSABILIDADE, ");
                        sql.AppendLine("A.DATAHORA_PAUSOU,A.ID_ATENDIMENTO_CONNECT,A.ADICIONAL_1,A.ADICIONAL_2,A.ADICIONAL_3,A.ADICIONAL_4,A.ADICIONAL_5, ");
                        sql.AppendLine("A.ADICIONAL_6,A.ADICIONAL_7,A.ADICIONAL_8,A.ADICIONAL_9,A.ADICIONAL_10,A.ID_CLIENTE_PLANOPRODUTOCONNECT,A.OBSERVACAO_PAUSA, ");
                        sql.AppendLine("A.WO,A.ID_VENDA,A.COD_SUPERV,A.PORTA_FTTH,A.CAIXA_FTTH,A.PRODUTO,A.PONTO_ACESSO,A.NOME_INSTALACAO,A.STATUS_AUDITORIA,A.OBSERVACAO_OITV,A.TEM_OSDIGITAL, ");
                        sql.AppendLine("A.WIFI_PASSWORD,A.WIFI_SSID ");
                        sql.AppendLine(sqlFrom.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new OsApiConsulta
                                {
                                    UsuarioAlteracao = Convert.ToString(reader["USUARIO_ALTERACAO"]),
                                    DiaAlteracao = Convert.ToString(reader["DIA_ALTERACAO"]),
                                    HoraAlteracao = Convert.ToString(reader["HORA_ALTERACAO"]),
                                    Dia = Convert.ToString(reader["DIA"]),
                                    Hora = Convert.ToString(reader["HORA"]),
                                    CodContrato = Convert.ToString(reader["CODCT"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    NumeroObra = Convert.ToString(reader["NUM_OBRA"]),
                                    NumeroDocumento = Convert.ToString(reader["NUM_DOC"]),
                                    NumeroProtocolo = Convert.ToString(reader["NUM_PROTOCOLO"]),
                                    CodigoCliente = Convert.ToString(reader["ID_CLIENTE"]),
                                    Cliente = Convert.ToString(reader["TITULAR"]),
                                    Cpf = Convert.ToString(reader["CPF_CLIENTE"]),
                                    Rg = Convert.ToString(reader["RG_CIENTE"]),
                                    RgAssinante = Convert.ToString(reader["RG_ASSINANTE"]),
                                    EmailAssinante = Convert.ToString(reader["EMAIL_ASSINANTE"]),
                                    VinculoCliente = Convert.ToString(reader["VINCULO_CIENTE"]),
                                    DocumentoCliente = Convert.ToString(reader["DOCUMENTO_CIENTE"]),
                                    Endereco = Convert.ToString(reader["ENDERECO"]),
                                    EnderecoNumero = Convert.ToString(reader["ENDERECO_NUMERO"]),
                                    EnderecoComplemento = Convert.ToString(reader["ENDERECO_COMPL"]),
                                    Cep = Convert.ToString(reader["ENDERECO_CPL"]),
                                    Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                    Uf = Convert.ToString(reader["ENDERECO_UF"]),
                                    Pais = Convert.ToString(reader["ENDERECO_PAIS"]),
                                    EnderecoTipo = Convert.ToString(reader["ENDERECO_TIPO"]),
                                    Logitude = Convert.ToString(reader["LONGITUDE_STR"]),
                                    Latitude = Convert.ToString(reader["LATITUDE_STR"]),
                                    CodArea = Convert.ToString(reader["COD_AREA"]),
                                    Area = Convert.ToString(reader["AREA"]),
                                    CodLocalidade = Convert.ToString(reader["COD_LOCALIDADE"]),
                                    Localidade = Convert.ToString(reader["LOCALIDADE"]),
                                    CodEscritorio = Convert.ToString(reader["COD_ESCRITORIO"]),
                                    Escritorio = Convert.ToString(reader["ESCRITORIO"]),
                                    TelCliente = Convert.ToString(reader["TEL_CIENTE"]),
                                    TelTitular = Convert.ToString(reader["TEL_TITULAR"]),
                                    Contato = Convert.ToString(reader["CONTATO"]),
                                    ContatoTelefone = Convert.ToString(reader["CONTATO_TELEFONE"]),
                                    TelComercial = Convert.ToString(reader["TEL_COMERCIAL"]),
                                    TelInstalacao = Convert.ToString(reader["TELEFONE_INSTALACAO"]),
                                    CodPlano = Convert.ToString(reader["ID_PLANOPRODUTO"]),
                                    IdPlanoProdutoConnect = Convert.ToString(reader["ID_CLIENTE_PLANOPRODUTOCONNECT"]),
                                    Plano = Convert.ToString(reader["PLANOPRODUTO"]),
                                    PlanoAssinante = Convert.ToString(reader["DESCRICAO_PLANO_ASSINANTE"]),
                                    Local = Convert.ToString(reader["LOCAL"]),
                                    Login = Convert.ToString(reader["ID_LOGIN"]),
                                    Dslan = Convert.ToString(reader["DSLAN"]),
                                    Ppoe = Convert.ToString(reader["PPPOE"]),
                                    Porta = Convert.ToString(reader["PORTA"]),
                                    CaixaDe = Convert.ToString(reader["CX_DE"]),
                                    CaixaPara = Convert.ToString(reader["CX_PARA"]),
                                    CaboDe = Convert.ToString(reader["CABO_DE"]),
                                    CaboPara = Convert.ToString(reader["CABO_PARA"]),
                                    Estacao = Convert.ToString(reader["ESTACAO"]),
                                    EstacaoPara = Convert.ToString(reader["ESTACAO_PARA"]),
                                    Ssde = Convert.ToString(reader["SS_DE"]),
                                    SsPara = Convert.ToString(reader["SS_PARA"]),
                                    Eqnde = Convert.ToString(reader["EQN_DE"]),
                                    EqnPara = Convert.ToString(reader["EQN_PARA"]),
                                    ParprimPara = Convert.ToString(reader["PARPRIM_PARA"]),
                                    StatusProd = Convert.ToString(reader["STATUS_PROD"]),
                                    Licde = Convert.ToString(reader["LIC_DE"]),
                                    Licpara = Convert.ToString(reader["LIC_PARA"]),
                                    EndCaixa = Convert.ToString(reader["END_CAIXA"]),
                                    EndArm = Convert.ToString(reader["END_ARM"]),
                                    Roterizar = Convert.ToString(reader["ROTERIZAR"]),
                                    DataHoraDeslocamento = Convert.ToString(reader["DATAHORA_DESLOCAMENTO"]),
                                    DataHoraInicioAtendimento = Convert.ToString(reader["DATAHORA_INI_ATEND"]),
                                    DataHoraFimAtendimento = Convert.ToString(reader["DATAHORA_FIM_ATEND"]),
                                    UsuarioCancelou = Convert.ToString(reader["USUARIO_CANCELOU"]),
                                    DataHoraCancelou = Convert.ToString(reader["DATA_HORA_CANCELOU"]),
                                    TempoRealDeslocamento = Convert.ToString(reader["TEMPO_REAL_DESLOCAMENTO"]),
                                    TempoDeslocamento = Convert.ToString(reader["TEMPO_DESLOCAMENTO"]),
                                    GeoSegmento = Convert.ToString(reader["GEOSEGMENTO"]),
                                    DataProd = Convert.ToString(reader["DATA_PROD"]),
                                    Validado = Convert.ToString(reader["VALIDADO"]),
                                    HoraMaxima = Convert.ToString(reader["HORA_MAXIMA"]),
                                    DataMaxima = Convert.ToString(reader["DATA_MAXIMA"]),
                                    CodTipoServico = Convert.ToString(reader["COD_TIPO_SERV"]),
                                    TipoServico = Convert.ToString(reader["TIPOSERVICO"]),
                                    CodSubTipoServico = Convert.ToString(reader["COD_SUB_TIPO_SERV"]),
                                    SubTipoServico = Convert.ToString(reader["SUBTIPOSERVICO"]),
                                    CodEncerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                    HoraEncerramento = Convert.ToString(reader["HORA_ENCERRAMENTO"]),
                                    Observacao = Convert.ToString(reader["OBSERVACAO"]),
                                    Usuario = Convert.ToString(reader["USUARIO"]),
                                    UsuarioBaixa = Convert.ToString(reader["USUARIO_BAIXA"]),
                                    UnidadeCusto = Convert.ToString(reader["UNIDADE_CUSTO"]),
                                    NumRVT = Convert.ToString(reader["NUM_RVT"]),
                                    Tarefa = Convert.ToString(reader["TAREFA"]),
                                    Equipe = Convert.ToString(reader["EQUIPE"]),
                                    Ard = Convert.ToString(reader["ARD"]),
                                    TipoNota = Convert.ToString(reader["TIPO_NOTA"]),
                                    Comunidade = Convert.ToString(reader["COMUNIDADE"]),
                                    NumDocEntrega = Convert.ToString(reader["NUM_DOC_ENTREGA"]),
                                    Posto = Convert.ToString(reader["POSTO"]),
                                    TemTesteEletrico = Convert.ToString(reader["TEM_TESTE_ELETRICO"]),
                                    TesteEletrico = Convert.ToString(reader["TESTE_ELETRICOS"]),
                                    ObsTeste = Convert.ToString(reader["OBSERVACAO_TESTE"]),
                                    DataIndicador = Convert.ToString(reader["DATA_INDICADOR"]),
                                    Insapoioss = Convert.ToString(reader["INSAPOIO55"]),
                                    ObsGasto = Convert.ToString(reader["OBSERVACAO_GASTO"]),
                                    Oxbdpee = Convert.ToString(reader["OXBDPEE"]),
                                    SiglaModulo = Convert.ToString(reader["SIGLA_MODULO"]),
                                    Origem = Convert.ToString(reader["ORIGEM"]),
                                    Segmento = Convert.ToString(reader["SEGMENTO"]),
                                    HoraIndicador = Convert.ToString(reader["HORA_INDICADOR"]),
                                    NumeroTarefa = Convert.ToString(reader["NUM_TAREFA"]),
                                    TipoReclamacao = Convert.ToString(reader["TIPO_RECLAMACAO"]),
                                    Seq = Convert.ToString(reader["SEQ"]),
                                    NumObraOriginal = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                                    StatusExecucao = Convert.ToString(reader["STATUS_EXECUCAO"]),
                                    ProtocoloNe = Convert.ToString(reader["PROTOCOLO_NE"]),
                                    CodTurnoServ = Convert.ToString(reader["COD_TURNO_SERV"]),
                                    Devolvida = Convert.ToString(reader["DEVOLVIDA"]),
                                    ObsAux = Convert.ToString(reader["OBSERVACAO_AUX"]),
                                    Lnbf = Convert.ToString(reader["LNBF"]),
                                    Grupo = Convert.ToString(reader["GRUPO"]),
                                    ObsDocumento = Convert.ToString(reader["OBSERVACAO_DOCUMENTO"]),
                                    Tx = Convert.ToString(reader["TX"]),
                                    Rx = Convert.ToString(reader["RX"]),
                                    TipoBaixa = Convert.ToString(reader["TIPO_BAIXA"]),
                                    RetornoTeclo = Convert.ToString(reader["RETORNO_TELCO"]),
                                    SenhaBaixa = Convert.ToString(reader["SENHA_BAIXA"]),
                                    Responsabilidade = Convert.ToString(reader["RESPONSABILIDADE"]),
                                    DataHoraPassou = Convert.ToString(reader["DATAHORA_PAUSOU"]),
                                    IdAtendimentoConnect = Convert.ToString(reader["ID_ATENDIMENTO_CONNECT"]),
                                    Adicional1 = Convert.ToString(reader["ADICIONAL_1"]),
                                    Adicional2 = Convert.ToString(reader["ADICIONAL_2"]),
                                    Adicional3 = Convert.ToString(reader["ADICIONAL_3"]),
                                    Adicional4 = Convert.ToString(reader["ADICIONAL_4"]),
                                    Adicional5 = Convert.ToString(reader["ADICIONAL_5"]),
                                    Adicional6 = Convert.ToString(reader["ADICIONAL_6"]),
                                    Adicional7 = Convert.ToString(reader["ADICIONAL_7"]),
                                    Adicional8 = Convert.ToString(reader["ADICIONAL_8"]),
                                    Adicional9 = Convert.ToString(reader["ADICIONAL_9"]),
                                    Adicional10 = Convert.ToString(reader["ADICIONAL_10"]),
                                    ObsPausa = Convert.ToString(reader["OBSERVACAO_PAUSA"]),
                                    Wo = Convert.ToString(reader["WO"]),
                                    IdVenda = Convert.ToString(reader["ID_VENDA"]),
                                    CodSupervisor = Convert.ToString(reader["COD_SUPERV"]),
                                    PortaFtth = Convert.ToString(reader["PORTA_FTTH"]),
                                    CaixaFtth = Convert.ToString(reader["CAIXA_FTTH"]),
                                    Produto = Convert.ToString(reader["PRODUTO"]),
                                    PontoAcesso = Convert.ToString(reader["PONTO_ACESSO"]),
                                    NomeInstalacao = Convert.ToString(reader["NOME_INSTALACAO"]),
                                    StatusAuditoria = Convert.ToString(reader["STATUS_AUDITORIA"]),
                                    ObsOitv = Convert.ToString(reader["OBSERVACAO_OITV"]),
                                    TemOsDigital = Convert.ToString(reader["TEM_OSDIGITAL"]),
                                    WifiPassword = Convert.ToString(reader["WIFI_PASSWORD"]),
                                    WifiSsid = Convert.ToString(reader["WIFI_SSID"]),
                                });
                            }
                        }

                    }
                }

                _cache.Add(chaveCache, retorno, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(50) });

                //Page e retornando DataTable
                return new DataTableResponse<OsApiConsulta>
                {
                    //draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new DataTableResponse<OsApiConsulta>
                {
                    error = ex.Message
                };
            }
        }

        [HttpPost]
        [Route("api/OsApiController/AjustarDataAtendimento")]
        public async Task<ApiResponse<bool>> AjustarDataAtendimento(AjustarDataAtendimentoApiRequest request)
        {
            List<OsApi> retorno = new List<OsApi>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                DateTime dataInico;
                DateTime dataFim;

                if (DateTime.TryParse(request.DataInicio, out dataInico) == false)
                {
                    throw new Exception("Data de inicio Invalida");
                }
                if (DateTime.TryParse(request.DataFim, out dataFim) == false)
                {
                    throw new Exception("Data de fim Invalida");
                }

                using (var session = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
                {
                    //IMongoCollection<DocumentoProducaoMongo> _collection = repositoryMongo.GetCollection();
                    var documentos = await repository.select(p => p.Data_Maxima >= Convert.ToDateTime(request.DataInicio) && p.Data_Maxima <= Convert.ToDateTime(request.DataFim));
                    foreach (var documento in documentos)
                    {
                        DocumentoProducaoMongo mongoDocumento = null;// _collection
                                                                     //.Aggregate()
                                                                     //.Match(p => p.codCt == documento.CodCt
                                                                     //&& p.numObra == documento.Num_Obra
                                                                     //&& p.contrato == documento.Num_Doc
                                                                     //&& p.projeto == documento.Projeto)
                                                                     //.ToList().FirstOrDefault();

                        if (mongoDocumento != null)
                        {
                            StatusDocumentoProducao registroFimAtendimento = mongoDocumento.status.Where(p => p.statusExecucao == 8).OrderBy(p => p.dataHoraServidor).LastOrDefault();
                            StatusDocumentoProducao registroInicioAtendimento = mongoDocumento.status.Where(p => p.statusExecucao == 3).OrderBy(p => p.dataHoraServidor).LastOrDefault();
                            StatusDocumentoProducao registroDeslocamentoAtendimento = mongoDocumento.status.Where(p => p.statusExecucao == 6).OrderBy(p => p.dataHoraServidor).LastOrDefault();
                            StatusDocumentoProducao registroPausaAtendimento = mongoDocumento.status.Where(p => p.statusExecucao == 10).OrderBy(p => p.dataHoraServidor).LastOrDefault();
                            bool salvar = false;

                            if (registroInicioAtendimento != null)
                            {
                                if (documento.DataHora_Ini_Atend.HasValue)
                                {
                                    if (Math.Abs(registroInicioAtendimento.dataHoraServidor.Subtract(documento.DataHora_Ini_Atend.Value).TotalMinutes) > 1)
                                    {
                                        documento.DataHora_Ini_Atend = registroInicioAtendimento.dataHoraServidor;
                                        salvar = true;
                                    }
                                }
                                else
                                {
                                    documento.DataHora_Ini_Atend = registroInicioAtendimento.dataHoraServidor;
                                    salvar = true;
                                }
                            }

                            if (registroFimAtendimento != null)
                            {
                                if (documento.DataHora_Fim_Atend.HasValue)
                                {
                                    if (Math.Abs(registroFimAtendimento.dataHoraServidor.Subtract(documento.DataHora_Fim_Atend.Value).TotalMinutes) > 1)
                                    {
                                        documento.DataHora_Fim_Atend = registroFimAtendimento.dataHoraServidor;
                                        salvar = true;
                                    }
                                }
                                else
                                {
                                    documento.DataHora_Fim_Atend = registroFimAtendimento.dataHoraServidor;
                                    salvar = true;
                                }
                            }

                            if (registroPausaAtendimento != null)
                            {
                                if (documento.DataHora_Pausou.HasValue)
                                {
                                    if (Math.Abs(registroPausaAtendimento.dataHoraServidor.Subtract(documento.DataHora_Pausou.Value).TotalMinutes) > 1)
                                    {
                                        documento.DataHora_Pausou = registroPausaAtendimento.dataHoraServidor;
                                        salvar = true;
                                    }
                                }
                                else
                                {
                                    documento.DataHora_Pausou = registroPausaAtendimento.dataHoraServidor;
                                    salvar = true;
                                }
                            }

                            if (registroDeslocamentoAtendimento != null)
                            {
                                if (documento.DataHora_Deslocamento.HasValue)
                                {
                                    if (Math.Abs(registroDeslocamentoAtendimento.dataHoraServidor.Subtract(documento.DataHora_Deslocamento.Value).TotalMinutes) > 1)
                                    {
                                        documento.DataHora_Deslocamento = registroDeslocamentoAtendimento.dataHoraServidor;
                                        salvar = true;
                                    }
                                }
                                else
                                {
                                    documento.DataHora_Deslocamento = registroDeslocamentoAtendimento.dataHoraServidor;
                                    salvar = true;
                                }
                            }

                            if (salvar)
                                await repository.Update(documento);
                        }

                    }
                }

                return new ApiResponse<bool>()
                {
                    draw = request.settings.length,
                    error = null,
                };

            }
            catch (Exception erro)
            {
                return new ApiResponse<bool>()
                {
                    draw = request.settings.length,
                    error = erro.Message + $" | stack: {erro.StackTrace}"

                };

            }
        }

        [HttpPost]
        [Route("api/OsApiController/ConsultarRVT")]
        public async Task<IHttpActionResult> ConsultarRVT(ArquivoOrdemServicoRequest request)
        {
            ArquivoOrdemServico retorno = null;

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                if (string.IsNullOrEmpty(request.NumeroOS))
                    throw new Exception("Numero da OS obrigatorio");

                using (var session = HibernateUtil.GetSessionT5())
                using (var sessionFoto = HibernateUtil.GetSessionFoto())
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "numObra", request.NumeroOS }
                    };

                    StringBuilder sql = new StringBuilder("SELECT ID_ARQUIVO_DOCPROD_RVT FROM TB_DOCUMENTO_PRODUCAO WHERE NUM_OBRA = @numObra ");

                    #region Filtros opcionais

                    if (!string.IsNullOrEmpty(request.CodCt))
                    {
                        parametros.Add("codCt", request.CodCt);
                        sql.AppendLine(" AND CODCT = @codCt ");
                    }

                    if (!string.IsNullOrEmpty(request.Projeto))
                    {
                        parametros.Add("projeto", request.Projeto);
                        sql.AppendLine(" AND PROJETO = @projeto ");
                    }

                    if (!string.IsNullOrEmpty(request.NumDoc))
                    {
                        parametros.Add("numDoc", request.NumDoc);
                        sql.AppendLine(" AND NUM_DOC = @numDoc ");
                    }

                    #endregion

                    long IdRTV = 0;

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                            IdRTV = Convert.ToInt64(reader["ID_ARQUIVO_DOCPROD_RVT"]);
                        else
                            throw new Exception($"Ordem de servico {request.NumeroOS} nao encontrada");
                    }

                    if (IdRTV == 0)
                        throw new Exception($"Ordem de servico {request.NumeroOS} nao possui arquivo RVT");

                    Dictionary<String, Object> parametrosRVT = new Dictionary<string, object>();
                    StringBuilder sqlRVT = new StringBuilder();

                    sqlRVT.AppendLine(@"SELECT * FROM TB_ARQUIVO_DOCPROD
                             WHERE id = @id");

                    parametrosRVT.Add("id", IdRTV);

                    using (var comFoto = sessionFoto.Connection.CreateCommand())
                    using (var reader = await CommandHelper.ExecuteReader(comFoto, sqlRVT, parametrosRVT, sessionFoto.Connection, null))
                    {
                        if (reader.Read())
                        {
                            retorno = new ArquivoOrdemServico
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Tipo = Convert.ToString(reader["TIPO"]),
                                Arquivo = (byte[])reader.GetValue(4)
                            };
                        }
                    }

                    return new ArquivoResult(new MemoryStream(retorno.Arquivo), new HttpRequestMessage(), $"{retorno.Descricao}");
                }
            }
            catch (Exception ex)
            {
                return new ArquivoResult(null, new HttpRequestMessage(), "", HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        [Route("api/OsApiController/ConsultarRVTBytes")]
        public async Task<IHttpActionResult> ConsultarRVTBytes(ArquivoOrdemServicoRequest request)
        {
            ArquivoOrdemServico retorno = null;

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                if (string.IsNullOrEmpty(request.NumeroOS))
                    throw new Exception("Numero da OS obrigatorio");

                using (var session = HibernateUtil.GetSessionT5())
                using (var sessionFoto = HibernateUtil.GetSessionFoto())
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "numObra", request.NumeroOS }
                    };

                    StringBuilder sql = new StringBuilder("SELECT ID_ARQUIVO_DOCPROD_RVT FROM TB_DOCUMENTO_PRODUCAO WHERE NUM_OBRA = @numObra ");

                    #region Filtros opcionais


                    if (!string.IsNullOrEmpty(request.CodCt))
                    {
                        parametros.Add("codCt", request.CodCt);
                        sql.AppendLine(" AND CODCT = @codCt ");
                    }

                    if (!string.IsNullOrEmpty(request.Projeto))
                    {
                        parametros.Add("projeto", request.Projeto);
                        sql.AppendLine(" AND PROJETO = @projeto ");
                    }

                    if (!string.IsNullOrEmpty(request.NumDoc))
                    {
                        parametros.Add("numDoc", request.NumDoc);
                        sql.AppendLine(" AND NUM_DOC = @numDoc ");
                    }

                    #endregion

                    long IdRTV = 0;

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                            IdRTV = reader["ID_ARQUIVO_DOCPROD_RVT"] != DBNull.Value ? Convert.ToInt64(reader["ID_ARQUIVO_DOCPROD_RVT"]) : 0;
                        else
                            throw new Exception($"Ordem de servico {request.NumeroOS} nao encontrada");
                    }

                    if (IdRTV == 0)
                        throw new Exception($"Ordem de servico {request.NumeroOS} nao possui arquivo RVT");

                    Dictionary<String, Object> parametrosRVT = new Dictionary<string, object>();
                    StringBuilder sqlRVT = new StringBuilder();

                    sqlRVT.AppendLine(@"SELECT ID, DESCRICAO, TIPO, ARQUIVO FROM TB_ARQUIVO_DOCPROD
                             WHERE id = @id");

                    parametrosRVT.Add("id", IdRTV);

                    using (var comFoto = sessionFoto.Connection.CreateCommand())
                    using (var reader = await CommandHelper.ExecuteReader(comFoto, sqlRVT, parametrosRVT, sessionFoto.Connection, null))
                    {
                        if (reader.Read())
                        {
                            retorno = new ArquivoOrdemServico
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Tipo = Convert.ToString(reader["TIPO"]),
                                Arquivo = reader["ARQUIVO"] != DBNull.Value ? (byte[])reader["ARQUIVO"] : null
                            };
                        }
                    }

                    return Ok(retorno);
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error($"[ConsultarRVTBytes] - Erro: {ex.Message} \n Json Request: {JsonService.SerializeObject(request)}", ex);
                return new ArquivoResult(null, new HttpRequestMessage(), "", HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        //[HttpPost]
        //[Route("api/OsApiController/ConsultarRVTBytes")]
        //public async Task<IHttpActionResult> ConsultarRVTBytes(ArquivoOrdemServicoRequest request)
        //{
        //    ArquivoOrdemServico retorno = null;

        //    try
        //    {
        //        using (var api = new UsuarioApiController())
        //        {
        //            if (!(await api.GetUsuarioValido(request.settings)).Ok)
        //            {
        //                throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
        //            }
        //        }

        //        if (string.IsNullOrEmpty(request.NumeroOS))
        //            throw new Exception("Numero da OS obrigatorio");

        //        using (var session = HibernateUtil.GetSessionT5())
        //        using (var sessionFoto = HibernateUtil.GetSessionFoto())
        //        using (var com = session.Connection.CreateCommand())
        //        {
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>
        //            {
        //                { "numObra", request.NumeroOS }
        //            };

        //            StringBuilder sql = new StringBuilder("SELECT ID_ARQUIVO_DOCPROD_RVT FROM TB_DOCUMENTO_PRODUCAO WHERE NUM_OBRA = @numObra ");

        //            #region Filtros opcionais


        //            if (!string.IsNullOrEmpty(request.CodCt))
        //            {
        //                parametros.Add("codCt", request.CodCt);
        //                sql.AppendLine(" AND CODCT = @codCt ");
        //            }

        //            if (!string.IsNullOrEmpty(request.Projeto))
        //            {
        //                parametros.Add("projeto", request.Projeto);
        //                sql.AppendLine(" AND PROJETO = @projeto ");
        //            }

        //            if (!string.IsNullOrEmpty(request.NumDoc))
        //            {
        //                parametros.Add("numDoc", request.NumDoc);
        //                sql.AppendLine(" AND NUM_DOC = @numDoc ");
        //            }

        //            #endregion

        //            long IdRTV = 0;

        //            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //            {
        //                if (await reader.ReadAsync())
        //                    IdRTV = Convert.ToInt64(reader["ID_ARQUIVO_DOCPROD_RVT"]);
        //                else
        //                    throw new Exception($"Ordem de servico {request.NumeroOS} nao encontrada");
        //            }

        //            if (IdRTV == 0)
        //                throw new Exception($"Ordem de servico {request.NumeroOS} nao possui arquivo RVT");

        //            Dictionary<String, Object> parametrosRVT = new Dictionary<string, object>();
        //            StringBuilder sqlRVT = new StringBuilder();

        //            sqlRVT.AppendLine(@"SELECT * FROM TB_ARQUIVO_DOCPROD
        //                     WHERE id = @id");

        //            parametrosRVT.Add("id", IdRTV);

        //            using (var comFoto = sessionFoto.Connection.CreateCommand())
        //            using (var reader = await CommandHelper.ExecuteReader(comFoto, sqlRVT, parametrosRVT, sessionFoto.Connection, null))
        //            {
        //                if (reader.Read())
        //                {
        //                    retorno = new ArquivoOrdemServico
        //                    {
        //                        Id = Convert.ToInt64(reader["ID"]),
        //                        Descricao = Convert.ToString(reader["DESCRICAO"]),
        //                        Tipo = Convert.ToString(reader["TIPO"]),
        //                        Arquivo = (byte[])reader.GetValue(4)
        //                    };
        //                }
        //            }

        //            return Ok(retorno);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ArquivoResult(null, new HttpRequestMessage(), "", HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}


        [HttpPost]
        [Route("api/OsApiController/AgendarOs")]
        public async Task<ApiResponse<bool>> AgendarOs(AgendamentoApiRequest request)
        {
            List<OsApi> retorno = new List<OsApi>();
            (DateTime dataInicial, DateTime dataFinal) dataHoraReferenciaServico;
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (var session = HibernateUtil.GetSessionT5())
                using (var sessionConnect = HibernateUtil.GetSession())
                using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                using (var repositoryContrato = new Conexao<ContratoAniel>(session))
                using (AgendamentoApiController agendamentoApiController = new AgendamentoApiController())
                using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                using (var repositoryEscritorio = new Conexao<EscritorioAniel>(session))
                using (var repositoryArea = new Conexao<AreaAniel>(session))
                using (var repositoryLocalidade = new Conexao<LocalidadeAniel>(session))
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(sessionConnect))
                using (var repoTurno = new Conexao<Turno_ServicoAniel>(session))
                using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(session))
                using (var repoTipo_ImovelAniel = new Conexao<Tipo_ImovelAniel>(session))
                {
                    DateTime data;
                    DateTime.TryParse(request.Data, out data);

                    if (data == null || data < DateTime.Today)
                    {
                        throw new Exception("Data informada não é válida");
                    }
                    IDocumentoProducaoAnielRepository documentoProducaoAnielRepository = new DocumentoProducaoAnielRepository(session, session);
                    var queryDocumento = documentoProducaoAnielRepository.GetAll();
                    queryDocumento = queryDocumento.Where(p => p.Numero_Obra_Original == request.Os);


                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        queryDocumento = queryDocumento.Where(p => p.CodCt == request.Contrato);
                    }
                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        queryDocumento = queryDocumento.Where(p => p.Projeto == request.Projeto);
                    }
                    if (request.CodigoCliente != 0)
                    {
                        queryDocumento = queryDocumento.Where(p => p.Num_Doc == request.CodigoCliente);
                    }

                    var servico = queryDocumento.OrderByDescending(p => p.Seq).FirstOrDefault();

                    if (servico == null)
                    {
                        throw new Exception("Serviço não encontrado");
                    }

                    if (servico.Status == StatusExecucaoAniel.Cancelado ||
                        servico.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                        servico.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                        servico.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                    {
                        throw new Exception("Serviço não pode ser agendada devido ao status. ");
                    }

                    EscritorioAniel escritorio = await repositoryEscritorio.selectFirst(p => p.Cod_Escritorio == servico.Cod_Escritorio);

                    if (escritorio == null)
                        throw new Exception("Escritório não Encontrada");

                    AreaAniel area = await repositoryArea.selectFirst(p => p.Cod_Escritorio == escritorio.Cod_Escritorio &&
                                                                           p.Cod_Area == servico.Cod_Area);

                    if (area == null)
                        throw new Exception("Area não Encontrada");

                    LocalidadeAniel localidade = await repositoryLocalidade.selectFirst(p => p.Cod_Escritorio == escritorio.Cod_Escritorio &&
                                                                                             p.Cod_Area == area.Cod_Area &&
                                                                                             p.Cod_Localidade == servico.Cod_Localidade);



                    List<Agendamento_EquipeAniel> agendamentos = (await repositoryAgendamento.select(p => p.CodCt == servico.CodCt && p.Projeto == servico.Projeto &&
                                                                                            p.Num_Obra == servico.Num_Obra && p.Num_Doc == servico.Num_Doc)).ToList();

                    var retornoApi = await agendamentoApiController.ConsultaDisponibilidadeAgendamento(new ConsultaDisponibilidadeAgendamentoApiRequest
                    {
                        settings = request.settings,
                        Data = data.ToString("dd/MM/yyyy"),
                        GrupoArea = escritorio.Descricao,
                        Area = area.Descricao,
                        Localidade = localidade.LocalidadeStr,
                        TipoServico = servico.TipoServico.Descricao,
                        Contrato = request.Contrato
                    });

                    if (retornoApi.error != null)
                        throw new Exception(retornoApi.error);

                    var horariosDisponiveis = retornoApi.data.Select(p => p.AgendamentoModelDetalhes);
                    bool disponivel = true;
                    Configuracoes_Gerais config = await repoConfigGerais.selectFirst();
                    AgendamentoHorasDisponiveis horario = null;

                    if (config.Considerar_Agendamento_Periodo == false)
                    {
                        var DataHora_Inicio = Convert.ToDateTime(String.Concat(request.Data, ' ', request.Janela));
                        horario = horariosDisponiveis.FirstOrDefault(p => p.Hora_Inicio == DataHora_Inicio);
                        disponivel = horario != null;
                    }
                    else
                    {
                        horario = horariosDisponiveis.FirstOrDefault(p => p.Turno == request.Janela && p.Hora_Inicio.Date == data);
                    }

                    disponivel = horario != null;
                    if (disponivel == false)
                    {
                        throw new Exception(Resources.RsCadastros.AGENDAMENTO_NAO_DISPONIVEL);
                    }

                    await repositoryAgendamento.Remove(agendamentos);

                    DateTime diaReferencia = Convert.ToDateTime(request.Data);

                    dataHoraReferenciaServico = (Convert.ToDateTime(diaReferencia.ToString("yyyy-MM-dd") + ' ' + horario.Hora_InicioStr),
                                                 Convert.ToDateTime(diaReferencia.ToString("yyyy-MM-dd") + ' ' + horario.Hora_FinalStr));

                    var agendamento = new Agendamento_EquipeAniel
                    {

                        CodCt = servico.CodCt,
                        Projeto = servico.Projeto,
                        Num_Obra = servico.Num_Obra,
                        Num_Doc = servico.Num_Doc,
                        Cod_Escritorio = servico.Cod_Escritorio,
                        Cod_Area = servico.Cod_Area,
                        Cod_Localidade = servico.Cod_Localidade,
                        DataHora_Inicio = dataHoraReferenciaServico.dataInicial,
                        DataHora_Fim = dataHoraReferenciaServico.dataFinal,
                        Flexivel = false,
                        Deslocamento = horario.Deslocamento,
                        Equipe = null,
                        Equipe_Tmp = horario.Equipe_Tmp,
                        Tipo_Servico = servico.Cod_Tipo_Serv.HasValue ? await repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == servico.Cod_Tipo_Serv) : null,
                        Tipo_Imovel = servico.Cod_Tipo_Imovel.HasValue ? await repoTipo_ImovelAniel.selectFirst(p => p.Cod_Tipo_Imovel == servico.Cod_Tipo_Imovel) : null,
                        UsuarioCriacao = request.settings.user,
                        Origem = OrigemAgendamentoOS.Agendamento,
                        Latitude = servico.GetLatitude_Str(),
                        Longitude = servico.GetLongitude_Str(),

                    };

                    await repositoryAgendamento.Save(agendamento);
                    MotivoReagendamentoOSApiController motivo = new MotivoReagendamentoOSApiController();
                    ConsultaMotivoReagendamentoRequest consultaMotivoReagendamentoRequest = new ConsultaMotivoReagendamentoRequest();
                    ConsultaMotivoReagendamento motivoretorno = new ConsultaMotivoReagendamento();
                    if (request.CodMotivoReagendamento.HasValue)
                    {
                        consultaMotivoReagendamentoRequest.settings.user = request.settings.user;
                        //consultaMotivoReagendamentoRequest.Status = 1;
                        consultaMotivoReagendamentoRequest.settings.password = request.settings.password;
                        consultaMotivoReagendamentoRequest.settings.token = request.settings.token;
                        consultaMotivoReagendamentoRequest.Cod_Motivo_Reagendamentos = Convert.ToInt32(request.CodMotivoReagendamento);
                        motivoretorno = motivo.ConsultarMotivo(consultaMotivoReagendamentoRequest).Result.data.FirstOrDefault();
                    }

                    var status_old = servico.Status;
                    servico.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                    servico.Data_Maxima = Convert.ToDateTime(agendamento.DataHora_Inicio.Date);
                    servico.Hora_Maxima = Convert.ToDateTime(agendamento.DataHora_Inicio);
                    servico.Usuario_Agendamento = request.settings.user;
                    servico.DiaHora_Agendamento = DateTime.Now;
                    servico.Equipe = null;
                    servico.Responsabilidade = 0;
                    servico.Roterizar = "N";

                    if (request.CodMotivoReagendamento.HasValue)
                    {
                        servico.CodMotivoReagendamento = request.CodMotivoReagendamento;
                        if (!string.IsNullOrEmpty(motivoretorno.Descricao))
                        {
                            servico.ObsMotivoReagendamento = motivoretorno.Descricao;
                        }
                    }

                    if (config.Considerar_Agendamento_Periodo)
                    {
                        servico.CodTurnoServ = (await repoTurno.selectFirst(p => p.Descricao == horario.Display)).Id;
                    }

                    servico.Agendamento = agendamento;
                    await repository.SaveOrUpdate(servico);
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(session, session);
                    repoDocProd.InsertLog_Lanc_Servico(request.CodMotivoReagendamento, motivoretorno.Descricao, servico.CodCt, servico.Projeto, servico.Num_Obra, servico.Num_Doc,
                                                             String.Format("Agendamento do serviço {0} (Integração)", agendamento.DataHora_Inicio.ToString("dd/MM/yyyy HH:mm")),
                                                             servico.Data_Maxima.HasValue ? servico.Data_Maxima.Value.ToString("dd/MM/yyyy") : "",
                                                             request.settings.user, agendamento.DataHora_Inicio.Date, null, agendamento.DataHora_Inicio.ToString("dd/MM/yyyy HH:mm"), null);


                    if (status_old != servico.Status)
                    {
                        repoDocProd.InsertLog_Lanc_Servico(request.CodMotivoReagendamento, motivoretorno.Descricao, servico.CodCt,
                                                     servico.Projeto,
                                                     servico.Num_Obra,
                                                     servico.Num_Doc,
                                                     "Mudança de Status",
                                                     HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(status_old.Value),
                                                     request.settings.user,
                                                     null,
                                                     null,
                                                     HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(servico.Status.Value),
                                                     null);
                    }
                }

                return new ApiResponse<bool>()
                {
                    draw = request.settings.length,
                    error = null,
                };

            }
            catch (Exception erro)
            {
                return new ApiResponse<bool>()
                {
                    draw = request.settings.length,
                    error = erro.Message + $" | stack: {erro.StackTrace}"

                };
            }
        }

        [HttpPost()]
        [Route("api/OsApiController/CancelarOs")]
        public async Task<PostApiRetorno<Int64>> CancelarOs(DocumentoProducaoRequestModels request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {

                using (ISession session2 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(session2))
                {
                    try
                    {
                        using (var api = new UsuarioApiController())
                        {
                            if (!(await api.GetUsuarioValido(request.settings)).Ok)
                            {
                                throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                            }
                        }

                        IDocumentoProducaoAnielRepository documentoProducaoAnielRepository = new DocumentoProducaoAnielRepository(session, session);
                        var queryDocumento = documentoProducaoAnielRepository.GetAll();
                        queryDocumento = queryDocumento.Where(p => p.Numero_Obra_Original == request.Os);

                        if (!String.IsNullOrEmpty(request.Contrato))
                        {
                            queryDocumento = queryDocumento.Where(p => p.CodCt == request.Contrato);
                        }
                        if (!String.IsNullOrEmpty(request.Projeto))
                        {
                            queryDocumento = queryDocumento.Where(p => p.Projeto == request.Projeto);
                        }
                        if (request.CodigoCliente != 0)
                        {
                            queryDocumento = queryDocumento.Where(p => p.Num_Doc == request.CodigoCliente);
                        }

                        var os = queryDocumento.OrderByDescending(p => p.Seq).FirstOrDefault();

                        if (os == null)
                        {
                            retorno.Ok = false;
                            retorno.Mensagem = RsCadastros.SERVICO_NAO_ENCONTRADO;
                            return retorno;
                        }

                        if (os.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                            os.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                            os.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                        {
                            throw new Exception("Serviço se encontra baixado, por isso não pode ser cancelado. ");
                        }

                        if (os.Status == StatusExecucaoAniel.Cancelado)
                        {
                            throw new Exception("Serviço já se encontra cancelado. ");
                        }

                        IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(session, session);
                        if (os.Responsabilidade == ResponsabilidadeOS.TECNICO)
                        {
                            throw new Exception("Não é possível cancelar a ordem de serviço pois ela esta com a responsabilidade do técnico.");
                        }

                        repoDocProd.InsertLog_Lanc_Servico
                            (null, null, os.CodCt, os.Projeto, os.Num_Obra, os.Num_Doc, String.Concat("Cancelamento"), HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(os.Status.Value), request.settings.user, os.Data_Maxima, os.Cod_Tipo_Serv, HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Cancelado), null, session.Transaction);


                        //DbTransaction  dbTransaction = new DbTransaction();
                        var parametros = new Dictionary<string, object>();
                        using (var com = session.Connection.CreateCommand())
                        {
                            var sql = new StringBuilder();
                            sql.AppendLine("UPDATE TB_DOCUMENTO_PRODUCAO SET STATUS_EXECUCAO=9,RESPONSABILIDADE=1 WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC");
                            parametros.Add("@CODCT", os.CodCt);
                            parametros.Add("@PROJETO", os.Projeto);
                            parametros.Add("@NUM_OBRA", os.Num_Obra);
                            parametros.Add("@NUM_DOC", os.Num_Doc);

                            //transaction.Enlist(com);

                            await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, com.Transaction);
                        }

                        using (var com = session.Connection.CreateCommand())
                        {
                            var sql = new StringBuilder();
                            //transaction.Enlist(com);
                            sql.AppendLine($"DELETE FROM TB_AGENDAMENTO_EQUIPEANIEL WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                            await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, com.Transaction);
                        }


                        //transaction.Commit();
                        retorno.Ok = true;
                    }
                    catch (Exception ex)
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = ex.Message;
                        retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                    }
                }

            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/OsApiController/ExcluirServico")]
        public async Task<PostApiRetorno> ExcluirServico(DocumentoProducaoRequestModels request)
        {
            PostApiRetorno retorno = new PostApiRetorno();

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
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))

                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    IDocumentoProducaoAnielRepository documentoProducaoAnielRepository = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);

                    var queryDocumento = documentoProducaoAnielRepository.GetAll();
                    queryDocumento = queryDocumento.Where(p => p.Numero_Obra_Original == request.Os);

                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        queryDocumento = queryDocumento.Where(p => p.CodCt == request.Contrato);
                    }
                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        queryDocumento = queryDocumento.Where(p => p.Projeto == request.Projeto);
                    }
                    if (request.CodigoCliente != 0)
                    {
                        queryDocumento = queryDocumento.Where(p => p.Num_Doc == request.CodigoCliente);
                    }

                    var documento = queryDocumento.OrderByDescending(p => p.Seq).FirstOrDefault();

                    if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                        documento.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                        documento.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                    {
                        throw new Exception($"Ordem de Servio {documento.Numero_Obra_Original} se encontra baixada. NÆo  poss¡vel excluir a Ordem de Servio.");
                    }

                    if (documento.Responsabilidade == ResponsabilidadeOS.TECNICO)
                    {
                        throw new Exception($"Ordem de Servio {documento.Numero_Obra_Original} se encontra com a responsabilidade tcnico. NÆo  poss¡vel excluir a Ordem de Servio.");
                    }



                    if (documento == null)
                    {
                        throw new Exception($"Ordem de Servio {documento.Numero_Obra_Original} nÆo foi encontrada");
                    }

                    var erros = await ExcluiOrdemServico(documento);
                    if (erros.Count > 0)
                    {
                        throw new Exception(erros.First());
                    }
                }

                retorno.Ok = true;
            }

            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/OsApiController/BaixarOs")]
        public async Task<PostApiRetorno<Int64>> BaixarOs(DocumentoProducaoBaixaRequestModels request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionConnect = HibernateUtil.GetSession())
            {
                using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                using (var repositoryContrato = new Conexao<ContratoAniel>(session))
                using (var repositoryCodigoEncerramento = new Conexao<Codigo_EncerramentoAniel>(session))
                using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
                using (var repositoryVinculoAt = new Conexao<VincularAtendimento>(sessionConnect))
                using (var repositoryAt = new Conexao<Model.nhibernate.Atendimento>(sessionConnect))
                using (var apiNot = new NotificacoesApiController())
                {
                    try
                    {
                        using (var api = new UsuarioApiController())
                        {
                            if (!(await api.GetUsuarioValido(request.settings)).Ok)
                            {
                                throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                            }
                        }

                        INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(session);
                        var naturezaBusiness = new NaturezaAnielBusiness(repoNatureza);
                        IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(session, session);
                        var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                        var os = (await repository.select(p => p.CodCt == request.Contrato &&
                                                  p.Projeto == request.Projeto &&
                                                  p.Num_Doc == request.CodigoCliente &&
                                                  p.Numero_Obra_Original == request.Os)).OrderByDescending(p => p.Seq).FirstOrDefault();

                        if (os == null)
                        {
                            retorno.Ok = false;
                            retorno.Mensagem = RsCadastros.SERVICO_NAO_ENCONTRADO;
                            return retorno;
                        }

                        var config = await repositoryConfig.selectFirst();
                        var equipeAntiga = os.Equipe;

                        if (os.Status == StatusExecucaoAniel.Fechada_Produtiva
                    || os.Status == StatusExecucaoAniel.Fechada_Improdutiva
                    || os.Status == StatusExecucaoAniel.Atendimento_Finalizado
                    || !String.IsNullOrEmpty(os.Cod_Encerramento))
                        {
                            throw new Exception("Ordem de Serviço já esta baixada");
                        }

                        if (os.Status == StatusExecucaoAniel.Cancelado)
                        {
                            throw new Exception("Ordem de Serviço já esta cancelada");
                        }

                        if (os.Data_Maxima == null)
                        {
                            throw new Exception("Baixa da ordem de serviço não foi realizada , informe data de agendamento");
                        }

                        if (os.Responsabilidade == ResponsabilidadeOS.TECNICO)
                        {
                            throw new Exception("Não é possível baixar o serviço, pois está sob responsabilidade do técnico.");
                        }

                        Codigo_EncerramentoAniel encerramentoAniel = await repositoryCodigoEncerramento.selectFirst(p => p.Cod_Encerramento == request.Codigo_Encerramento && p.CodCt == request.Contrato);
                        if (encerramentoAniel == null)
                        {
                            throw new Exception("Encerramento nao foi encontrado");
                        }

                        if (encerramentoAniel.Status_Execucao == 2 && String.IsNullOrEmpty(request.Equipe))
                        {
                            throw new Exception("Baixa produtiva da ordem de serviço não foi realizada , informe técnico");
                        }

                        DateTime? DataHoraInicio;
                        DateTime? DataHoraFim;

                        try
                        {
                            DataHoraInicio = (String.IsNullOrEmpty(request.Hora_Ini_Atend) || String.IsNullOrEmpty(request.Data_Ini_Atend)) ? (DateTime?)null : Convert.ToDateTime($"{request.Data_Ini_Atend} {request.Hora_Ini_Atend}");
                        }
                        catch
                        {
                            throw new Exception(String.Format(RsBase.INVALIDO_FORMAT, RsCadastros.DATAHORA_INICIO_ATENDIMENTO));
                        }
                        try
                        {
                            DataHoraFim = (String.IsNullOrEmpty(request.Hora_Fim_Atend) || String.IsNullOrEmpty(request.Data_Fim_Atend)) ? (DateTime?)null : Convert.ToDateTime($"{request.Data_Fim_Atend} {request.Hora_Fim_Atend}");
                        }
                        catch
                        {
                            throw new Exception(String.Format(RsBase.INVALIDO_FORMAT, RsCadastros.DATAHORA_FIM_ATENDIMENTO));
                        }

                        var contrato = await repositoryContrato.selectFirst(p => p.CodCt == os.CodCt);
                        if (contrato.Exigir_Mudanca_Status == "S")
                        {
                            if (DataHoraInicio.HasValue == false || DataHoraFim.HasValue == false)
                            {
                                throw new Exception("data/hora início do atendimento e fim do atendimento são obrigatórios, pois o contrato utilizar a configuração Exigir Mudança de Status.");
                            }
                        }

                        os.Cod_Encerramento = encerramentoAniel.Cod_Encerramento;
                        os.Status = (StatusExecucaoAniel?)encerramentoAniel.Status_Execucao;
                        os.Data_Prod = DateTime.Today;
                        os.Hora_Prod = DateTime.Now;
                        os.Responsabilidade = ResponsabilidadeOS.COP;
                        os.Usuario_Baixa = request.settings.user;
                        os.Observacao_Teste = request.Observacao;
                        os.Modulo_Baixa = "CONNECT";
                        os.Roterizar = "N";

                        if (!String.IsNullOrEmpty(request.Equipe))
                        {
                            os.Equipe = request.Equipe;
                        }

                        os.DataHora_Ini_Atend = DataHoraInicio;
                        os.DataHora_Fim_Atend = DataHoraFim;
                        os.CaixaFTTH = request.CaixaFTTH;
                        os.PortaFTTH = request.PortaFTTH;

                        Cobranca_Servico cobrancaEncontrada = documentoBusiness.BuscarCobrancaDocumento(os);

                        if (cobrancaEncontrada != null)
                        {
                            if (cobrancaEncontrada.Escolha_Cobranca == Escolha_Cobranca.Aguarda_Avaliacao)
                                os.Status_Cobranca = Status_CobrancaAniel.Aguardando_Avaliacao_Cobranca;
                            else
                                os.Status_Cobranca = Status_CobrancaAniel.Automatica;
                        }

                        await repository.Update(os);

                        // ******* Alterando o status dos Planos/Produtos da OS  *******

                        Cliente_PlanoProduto planoProduto;
                        using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                        {
                            planoProduto = (await repoClientePlanoProduto.selectFirst(p => p.Id == os.Id_Cliente_PlanoProdutoConnect));
                        }

                        if (planoProduto?.Status == Status_PlanoProduto.Aguardando_Instalacao)
                        {
                            planoProduto.Status = Status_PlanoProduto.Habilitado;
                            await new Conexao<Cliente_PlanoProduto>(session).SaveOrUpdate(planoProduto);
                        }

                        // ***********************************************************

                        if (os.Status == StatusExecucaoAniel.Fechada_Produtiva)
                        {
                            documentoBusiness.AplicaProducaoPrevista(os, request.settings.user);
                            documentoBusiness.AtualizarDataProducao(os);
                        }

                        if (os.Equipe != equipeAntiga)
                        {
                            documentoBusiness.InsertLog_Lanc_Servico(os.CodMotivoReagendamento, os.ObsMotivoReagendamento, os.CodCt, os.Projeto, os.Num_Obra, os.Num_Doc,
                                    os.Equipe == null ? "Remoção de Equipe" : "Equipe", $"{equipeAntiga}",
                                   os.Usuario_Baixa, null, null, os.Equipe, null);
                        }

                        documentoBusiness.InsertLog_Lanc_Servico(os.CodMotivoReagendamento, os.ObsMotivoReagendamento, os.CodCt, os.Projeto, os.Num_Obra, os.Num_Doc, "Baixou OS", os.Num_Obra + $" - Encerramento: {encerramentoAniel.Cod_Encerramento} - {encerramentoAniel.Descricao}", os.Usuario_Baixa, os.Data_Maxima, os.Cod_Tipo_Serv, null, request.Observacao);

                        DocumentoProducao_GetConfigFluxoBaixa _getConfigFluxoBaixa = new DocumentoProducao_GetConfigFluxoBaixa();
                        Int32 _codNatureza = naturezaBusiness.GetNaturezaByCodTipoServ(Convert.ToInt32(os.Cod_Tipo_Serv), os.CodCt);
                        _getConfigFluxoBaixa.codCt = os.CodCt;
                        _getConfigFluxoBaixa.projeto = os.Projeto;
                        _getConfigFluxoBaixa.codEncerramento = os.Cod_Encerramento;
                        _getConfigFluxoBaixa.codNatureza = _codNatureza;
                        _getConfigFluxoBaixa.numObraOriginal = os.Numero_Obra_Original;
                        _getConfigFluxoBaixa.numDoc = os.Num_Doc;
                        _getConfigFluxoBaixa = documentoBusiness.GetConfigFluxoBaixa(_getConfigFluxoBaixa);

                        //Adicionado para o processo do novo fluxo de baixa ...

                        var oldCodTipoServ = os.Cod_Tipo_Serv;
                        var oldCodDepto = os.CodDepto;
                        Int16? codTipoServFluxo = null;
                        Int16? codDeptoFluxo = null;

                        if (_getConfigFluxoBaixa.fluxoAoEncerrar == "S")
                        {
                            if (_getConfigFluxoBaixa.comportamentoTipoServ == 1)
                            {
                                //documento.Cod_Tipo_Serv = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);
                                codTipoServFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);
                            }
                            else if (_getConfigFluxoBaixa.comportamentoTipoServ == 2)
                            {
                                _getConfigFluxoBaixa = documentoBusiness.GetTipoServDepOS("TIPOSERV", "LAST", _getConfigFluxoBaixa);
                                codTipoServFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);
                            }
                            else if (_getConfigFluxoBaixa.comportamentoTipoServ == 3)
                            {
                                _getConfigFluxoBaixa = documentoBusiness.GetTipoServDepOS("TIPOSERV", "FIRST", _getConfigFluxoBaixa);
                                codTipoServFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);
                            }

                            if (_getConfigFluxoBaixa.comportamentoSetor == 1)
                            {
                                codDeptoFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codDepto);
                            }
                            else if (_getConfigFluxoBaixa.comportamentoSetor == 2)
                            {
                                _getConfigFluxoBaixa = documentoBusiness.GetTipoServDepOS("SETOR", "LAST", _getConfigFluxoBaixa);
                                codDeptoFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codDepto);
                            }
                            else if (_getConfigFluxoBaixa.comportamentoSetor == 3)
                            {
                                _getConfigFluxoBaixa = documentoBusiness.GetTipoServDepOS("SETOR", "FIRST", _getConfigFluxoBaixa);
                                codDeptoFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codDepto);
                            }

                        }

                        if (encerramentoAniel.Encerra_OS_Apos_Baixa == "N")
                        {
                            var novoDocumento = documentoBusiness.GerarOrdemCopia(os, documentoBusiness.GerarSeq(os.CodCt, os.Projeto, os.Numero_Obra_Original, os.Num_Doc));

                            if (codTipoServFluxo != null)
                                novoDocumento.Cod_Tipo_Serv = codTipoServFluxo;

                            if (codDeptoFluxo != null)
                                novoDocumento.CodDepto = codDeptoFluxo;

                            await repository.Save(novoDocumento);
                            documentoBusiness.InsertLog_Lanc_Servico(novoDocumento.CodMotivoReagendamento, novoDocumento.ObsMotivoReagendamento, novoDocumento.CodCt, novoDocumento.Projeto, novoDocumento.Num_Obra, novoDocumento.Num_Doc, "Criação do Serviço", null,
                                   os.Usuario_Baixa, novoDocumento.Data_Maxima, novoDocumento.Cod_Tipo_Serv, null, null);
                            //FluxoBaixa();

                        }
                        else if (encerramentoAniel.Encerra_OS_Apos_Baixa == "S")
                        {
                            var novoDocumento = documentoBusiness.GerarOrdemCopia(os, documentoBusiness.GerarSeq(os.CodCt, os.Projeto, os.Numero_Obra_Original, os.Num_Doc));
                            var novoNumObra = documentoBusiness.GetNum_Obra(config);
                            novoDocumento.Num_Obra = novoNumObra + "/1";
                            novoDocumento.Numero_Obra_Original = novoNumObra;
                            novoDocumento.Seq = 1;
                            novoDocumento.CodDepto = Convert.ToInt16(_getConfigFluxoBaixa.codDepto);
                            novoDocumento.Cod_Tipo_Serv = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);

                            if (_getConfigFluxoBaixa.fluxoAoEncerrar == "S")
                            {
                                documentoBusiness.InsertLog_Lanc_Servico(novoDocumento.CodMotivoReagendamento, novoDocumento.ObsMotivoReagendamento, novoDocumento.CodCt, novoDocumento.Projeto, os.Num_Obra, novoDocumento.Num_Doc, "Fluxo - Criação Aut. Serviço", null,
                                       os.Usuario_Baixa, novoDocumento.Data_Maxima, novoDocumento.Cod_Tipo_Serv, novoDocumento.Num_Obra, null);

                                if ((oldCodDepto != novoDocumento.CodDepto) && (_getConfigFluxoBaixa.codDepto != null))
                                {
                                    documentoBusiness.InsertLog_Lanc_Servico(novoDocumento.CodMotivoReagendamento, novoDocumento.ObsMotivoReagendamento, novoDocumento.CodCt, novoDocumento.Projeto, os.Num_Obra, novoDocumento.Num_Doc, "Alteração no Departamento na criação do documento: " + novoDocumento.Num_Obra, Convert.ToString(oldCodDepto),
                                       os.Usuario_Baixa, novoDocumento.Data_Maxima, novoDocumento.Cod_Tipo_Serv, Convert.ToString(novoDocumento.CodDepto), null);
                                }

                                if ((oldCodTipoServ != novoDocumento.Cod_Tipo_Serv) && (_getConfigFluxoBaixa.codTipoServ != null))
                                {
                                    documentoBusiness.InsertLog_Lanc_Servico(novoDocumento.CodMotivoReagendamento, novoDocumento.ObsMotivoReagendamento, novoDocumento.CodCt, novoDocumento.Projeto, os.Num_Obra, novoDocumento.Num_Doc, "Alteração do Tipo de Serviço na criação do documento: " + novoDocumento.Num_Obra, Convert.ToString(oldCodTipoServ),
                                       os.Usuario_Baixa, novoDocumento.Data_Maxima, novoDocumento.Cod_Tipo_Serv, Convert.ToString(novoDocumento.Cod_Tipo_Serv), null);
                                }
                            }
                        }

                        var listagemAtendimentosVinculados = (await repositoryVinculoAt.select(p =>
                       p.CodCT == os.CodCt &&
                       p.Projeto == os.Projeto &&
                       p.Num_Obra == os.Num_Obra &&
                       p.Num_Doc == Convert.ToString(os.Num_Doc)
                       )).Select(p => p.Atendimento).ToList();
                        var atendimento = await repositoryAt.selectFirst(p => p.Id == os.Id_Atendimento);
                        if (atendimento != null)
                        {
                            listagemAtendimentosVinculados.Add(atendimento);
                        }
                        var statusViaveisNotificacao = new List<StatusExecucaoAniel?>
                        {
                            StatusExecucaoAniel.Fechada_Produtiva,
                            StatusExecucaoAniel.Fechada_Improdutiva
                        };

                        if (statusViaveisNotificacao.Contains(os?.Status) && listagemAtendimentosVinculados.Count > 0 &&
                            listagemAtendimentosVinculados.Any(p => (p.Status_Atendimento == Status_Atendimento.Em_Atendimento || p.Status_Atendimento == Status_Atendimento.Aguardando) && p.Responsavel != null))
                        {
                            await apiNot.EnviaNotificacoesServico(45, os.CodCt, os.Projeto, os.Num_Obra, os.Num_Doc, null);
                        }

                        await EncerrarAtendimentoJuntoOS(os, request.settings.user, session, sessionConnect);

                        //foi avisado ao gestor Renato que a melhor pratica seria ter acesso a essa rotina atraves do ConnectBusiness 
                        //porem foi escolhido por ele fazer requisição por api pra evitar impactos na rotina de roteirizacao
                        using (var post = new PostHttp(CONSTANTES.log4Net))
                        {
                            IConfigUrlRepository repoConfigUrl = new ConfigUrlRepository(session);
                            var urlConnect = repoConfigUrl.GetUrlConnect();

                            string retornoRouter = post.ExecuteString<object>(urlConnect + "/api/DocumentoProducao/RotinaRouter", JsonService.SerializeObject(new
                            {
                                CodCt = os.CodCt,
                                Num_Obra = os.Num_Obra,
                                Num_Doc = os.Num_Doc,
                                Projeto = os.Projeto,
                                Num_Obra_Original = os.Numero_Obra_Original
                            }));
                        }

                        retorno.Ok = true;
                    }
                    catch (Exception ex)
                    {
                        retorno.Mensagem = ex.Message;
                        retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                    }
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/OsApiController/AlterarOs")]
        public async Task<PostApiRetorno<Int64>> AlterarOs(AlterarOsApiRequest request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                using (var repoAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                {
                    try
                    {
                        using (var api = new UsuarioApiController())
                        {
                            if (!(await api.GetUsuarioValido(request.settings)).Ok)
                            {
                                throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                            }
                        }

                        Documento_ProducaoAniel servico = null;
                        Agendamento_EquipeAniel agendamentoServico = null;

                        if (!String.IsNullOrEmpty(request.Filtros.Codct) && !String.IsNullOrEmpty(request.Filtros.Projeto) && request.Filtros.NumDoc != 0 && !String.IsNullOrEmpty(request.Filtros.NumObra))
                        {
                            servico = (await repository.select(p => p.CodCt == request.Filtros.Codct &&
                                                      p.Projeto == request.Filtros.Projeto &&
                                                      p.Num_Doc == request.Filtros.NumDoc &&
                                                      p.Numero_Obra_Original == request.Filtros.NumObra)).OrderByDescending(p => p.Seq).FirstOrDefault();
                        }
                        else
                        {
                            servico = (await repository.select(p => p.Numero_Obra_Original == request.Filtros.NumObra)).
                                                        OrderByDescending(p => p.Seq).FirstOrDefault();
                        }

                        #region Excecoes relativas a OS 
                        if (servico == null)
                        {
                            throw new Exception("OS não encontrada.");
                        }


                        #endregion
                        DocumentoProducaoAnielRepository documentoProducaoAnielRepository = new DocumentoProducaoAnielRepository(session, session);
                        DocumentoProducaoAnielBusiness documentoBusiness = new DocumentoProducaoAnielBusiness(documentoProducaoAnielRepository);

                        if (servico.Status != StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado &&
                            servico.Status != StatusExecucaoAniel.Atendimento_Finalizado &&
                            servico.Status != StatusExecucaoAniel.Fechada_Produtiva &&
                            servico.Status != StatusExecucaoAniel.Fechada_Improdutiva &&
                            servico.Status != StatusExecucaoAniel.Cancelado)
                        {
                            var objAlteracao = new RequestAlteraOs
                            {
                                Usuario = request.settings.user,
                                Area = request.Area,
                                Bairro = request.Bairro,
                                CamposAdicionais = request.CamposAdicionais,
                                Cep = request.Cep,
                                Cidade = request.Cidade,
                                CodigoCliente = request.Filtros.NumDoc,
                                Localidade = request.Localidade,
                                Complemento = request.Complemento,
                                Contrato = request.Filtros.Codct,
                                Data_HoraAgendamento = request.Data_HoraAgendamento,
                                DetalhesOS = request.DetalhesOS,
                                GrupoArea = request.GrupoArea,
                                InformacoesComplementares = request.InformacoesComplementares,
                                ObservacaoTeste = request.ObservacaoTeste,
                                Latitude = Convert.ToString(request.Latitude),
                                Logradouro = request.Logradouro,
                                Longitude = request.Longitude,
                                Numero = request.Numero,
                                Os = request.Filtros.NumObra,
                                Periodo = request.Periodo,
                                Prioridade = request.Prioridade,
                                Projeto = request.Filtros.Projeto,
                                Segmento = request.Segmento,
                                Setor = request.Setor,
                                SubtipoServico = request.SubtipoServico,
                                Tecnico = request.Tecnico,
                                TipoImovel = request.TipoImovel,
                                TipoServico = request.TipoServico,
                                UF = request.UF,
                                Responsabilidade = request.Responsabilidade,


                            };

                            documentoBusiness.ApiAlterarOS(servico, objAlteracao, ref agendamentoServico);
                            await repository.Update(servico);


                            if (request.Data_HoraAgendamento != null)
                            {
                                Agendamento_EquipeAniel agendamentoBanco = await repoAgendamento.selectFirst(p => p.CodCt == servico.CodCt && p.Projeto == servico.Projeto && p.Num_Doc == servico.Num_Doc && p.Num_Obra == servico.Num_Obra);

                                if (agendamentoBanco != null)
                                    await repoAgendamento.Remove(agendamentoBanco);

                                if (agendamentoServico != null)
                                    await repoAgendamento.SaveOrUpdate(agendamentoServico);

                            }

                        }
                        else
                        {
                            throw new Exception("OS não pode ser alterada pois está no status: " + HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(servico.Status.Value));
                        }

                        retorno.Ok = true;
                    }
                    catch (Exception ex)
                    {
                        retorno.Mensagem = ex.Message;
                        retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/OsApiController/InterromperDeslocamentoOs")]
        public async Task<PostApiRetorno<Int64>> InterromperDeslocamentoOs(DocumentoProducaoRequestModels request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                {
                    try
                    {
                        using (var api = new UsuarioApiController())
                        {
                            if (!(await api.GetUsuarioValido(request.settings)).Ok)
                            {
                                throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                            }
                        }

                        IDocumentoProducaoAnielRepository repositorio = new DocumentoProducaoAnielRepository(session, session);
                        var business = new DocumentoProducaoAnielBusiness(repositorio);
                        await business.InterromperDeslocamento(request.Contrato, request.Projeto, request.CodigoCliente, request.Os, request.settings.user);

                    }
                    catch (Exception ex)
                    {
                        retorno.Mensagem = ex.Message;
                        retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }
                }
            }
            return retorno;
        }


        private async Task<PostApiRetorno<Int64>> EncerrarAtendimentoJuntoOS(Documento_ProducaoAniel documento, string usuario, ISession sessionT5, ISession session)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<long>
            {
                Ok = true
            };

            using (var repoDocs = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repoConfGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            {
                Configuracoes_Gerais Conf_Gerais = await repoConfGerais.selectFirst();
                //Habilita Plano/Produto
                if (documento.Id_Cliente_PlanoProdutoConnect.HasValue)
                {
                    IList<Cliente_PlanoProduto> Plano_Produto = await repoClientePlanoProduto.select(p => p.Id == documento.Id_Cliente_PlanoProdutoConnect);

                    var has_status = Plano_Produto.Where(p => p.Status.HasValue);
                    var habilitados = has_status.Where(p => p.Status == Status_PlanoProduto.Aguardando_Instalacao || p.Status == Status_PlanoProduto.Aguardando_Criacao_OS).
                                  Select(p => { p.Status = Status_PlanoProduto.Habilitado; return p; }).ToList();

                    await repoClientePlanoProduto.SaveOrUpdate(habilitados);
                }

                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                Cobranca_Servico cobrancaEncontrada = repoDocProd.BuscarCobrancaDocumento(documento);

                if (cobrancaEncontrada != null)
                {
                    if (cobrancaEncontrada.Escolha_Cobranca == Escolha_Cobranca.Aguarda_Avaliacao)
                        documento.Status_Cobranca = Status_CobrancaAniel.Aguardando_Avaliacao_Cobranca;
                    else
                        documento.Status_Cobranca = Status_CobrancaAniel.Automatica;

                    await repoDocs.Update(documento);
                }


                if (documento.Cliente?.Cod_Cliente != null)
                {
                    var pesquisa_disparo_auto_servico = new Conexao<Pesquisa_Satisfacao>(session).select(p => p.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Envio_Automatico_Servico && p.Status == Status_Default.Ativo).Result;
                    if (pesquisa_disparo_auto_servico.Count != 0)
                    {
                        await EnviarEmMassa(session,
                                            new List<long> { documento.Cliente.Cod_Cliente },
                                            pesquisa_disparo_auto_servico.Select(p => p.Id).ToList(),
                                            usuario);
                    }
                }

                retorno.Mensagem = "Operação bem sucedida";
                retorno.IdObjeto = documento.Num_Doc;

            }
            return retorno;
        }

        private async Task<PostApiRetorno<Int64>> EnviarEmMassa(ISession session, List<long> clientes, List<long> pesquisas, string usuario)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);
                using (Conexao<Pesquisa_Satisfacao> repo = new Conexao<Pesquisa_Satisfacao>(session))
                using (Conexao<Configuracoes_Gerais> repoConfig = new Conexao<Configuracoes_Gerais>(session))
                {
                    foreach (var id in clientes)
                    {
                        foreach (var pesquisa in pesquisas)
                        {
                            Configuracoes_Gerais config = await repoConfig.selectFirst();

                            var pesquisa_satisfacao = await repo.selectFirst(p => p.Id == pesquisa);
                            Cliente cliente = await new Conexao<Cliente>(session).selectFirst(p => p.Id == id);
                            IPesquisaSatisfacaoRepository pesquisaRepository = new PesquisaSatisfacaoRepository(session);
                            PesquisaSatisfacaoBusiness pesquisaSatisfacaoBusiness = new PesquisaSatisfacaoBusiness(pesquisaRepository);
                            pesquisaSatisfacaoBusiness.InsertPesquisaResposta(cliente, usuario, pesquisa);
                            Int64 id_resposta = await new Conexao<Pesquisa_Resposta>(session).Max(p => p.Id);
                            String urlPesquisa = $@"{CONSTANTES.URLPortal}/pt/Pesquisa_Satisfacao/Formulario/{id_resposta}";
                            pesquisaSatisfacaoBusiness.EnviarEmail(cliente,
                                                                   pesquisa_satisfacao.Titulo_Da_Pesquisa,
                                                                   config.OLA_URL_Logo,
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
        [Route("api/Custos_TarefaApi/Inserir")]
        public async Task<PostApiRetorno<ProducaoModels>> Inserir(ParamWebApi<ProducaoModels> value)
        {
            PostApiRetorno<ProducaoModels> retorno = new PostApiRetorno<ProducaoModels>();
            try
            {
                Custos_TarefaAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Custos_TarefaAniel>(session))
                    using (var repositoryDoc = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryCaderno = new Conexao<Caderno_EncargosAniel>(session))
                    //using (var apiEquipe = new EquipeApiController())
                    //using (var apiFechamento = new FechamentoPeriodoApiController())
                    //using (var apiDocumento = new Documento_ProducaoApiController())
                    {
                        var documento = await repositoryDoc.selectFirst(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Numero_Obra_Original == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc);

                        if (String.IsNullOrEmpty(value.Objeto.Codigo))
                            throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.CODIGO));

                        modelo = (await repository.selectFirst(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Obra == documento.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc &&
                                                                    p.Codigo == value.Objeto.Codigo));

                        if (modelo == null)
                        {
                            var caderno = await repositoryCaderno.selectFirst(p => p.Codigo == value.Objeto.Codigo && p.Unidade_Custo == documento.Unidade_Custo);

                            // var cod_precoPadrao = await GetCodPrecoPadrao(session, documento.CodCt, documento.Projeto, documento.Unidade_Custo, caderno.Classe_MO);
                            var data_Prod = documento.Data_Prod ?? documento.Data_Maxima ?? DateTime.Now;

                            if (String.IsNullOrEmpty(documento.Equipe))
                                throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.EQUIPE));

                            //if (await apiEquipe.Afastado(session, documento.Equipe, null))
                            //    throw new Exception(String.Format(RsCadastros.EQUIPE_ENCERRADA, documento.Equipe));

                            //if (String.IsNullOrEmpty(cod_precoPadrao))
                            //    throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.CODPRECO_PADRAO));

                            if (documento.Roterizar == "S")
                                throw new Exception(RsCadastros.TANOMOBILE);


                            //var periodo = await apiFechamento.PeriodoFechado(session, documento.CodCt, documento.Projeto, DateTime.Now);
                            //if (periodo.Item1 != null)
                            //    throw new Exception(String.Format(RsCadastros.CONTRATO_PROJETO_PERIODO_FECHADO, documento.CodCt, documento.Projeto, periodo.Item1.Value.ToString("dd/MM/yyyy"), periodo.Item2.Value.ToString("dd/MM/yyyy")));

                            modelo = new Custos_TarefaAniel
                            {
                                CodCt = documento.CodCt,
                                Projeto = documento.Projeto,
                                Num_Obra = documento.Num_Obra,
                                Num_Doc = documento.Num_Doc,
                                Data_Prod = data_Prod,
                                Cod_Tipo_Serv = documento.Cod_Tipo_Serv,
                                Cod_Sub_Tipo_Serv = documento.Cod_Sub_Tipo_Serv ?? "-",
                                Unidade_Custo = documento.Unidade_Custo,
                                Tipo_Producao = documento.Tipo_Producao ?? "AC",
                                //Cod_Preco = cod_precoPadrao,
                                Equipe = documento.Equipe,
                                Tarefa = "-",
                                Codigo = value.Objeto.Codigo,
                                Quant_Catal = 0,
                                Quant_Prod = Convert.ToDouble(value.Objeto.Quant_Prod),
                                Dia = DateTime.Now,
                                Hora = DateTime.Now,
                                Usuario = value.Usuario
                            };

                            retorno.IdObjeto = new ProducaoModels
                            {
                                CodCt = modelo.CodCt,
                                Projeto = modelo.Projeto,
                                Num_Obra = modelo.Num_Obra,
                                Num_Doc = modelo.Num_Doc,
                                Unidade_Custo = modelo.Unidade_Custo,
                                Tarefa = modelo.Tarefa,
                                Equipe = modelo.Equipe,
                                Cod_Tipo_Serv = modelo.Cod_Tipo_Serv,
                                Cod_Sub_Tipo_Serv = modelo.Cod_Sub_Tipo_Serv,
                                Tipo_Producao = modelo.Tipo_Producao,
                                Codigo = modelo.Codigo,
                                Data_Prod = modelo.Data_Prod?.ToString("dd/MM/yyyy"),
                                Cod_Preco = modelo.Cod_Preco,
                                Descricao = caderno.Descricao,
                                Dia = modelo.Dia?.ToString("dd/MM/yyyy"),
                                Hora = modelo.Hora?.ToString("HH:mm"),
                                //   NomeEquipe = $"{modelo.Equipe} - {await apiEquipe.GetNome(session, modelo.Equipe)}",
                                Quant_Catal = "0.00",
                                Quant_Prod = modelo.Quant_Prod.HasValue ? modelo.Quant_Prod.Value.ToString("N2") : "0.00",
                                Usuario = modelo.Usuario,
                                Valor = caderno.Quantidade.HasValue ? caderno.Quantidade.Value.ToString("N2") : "0.00"
                            };
                        }
                        else
                        {
                            throw new Exception(RsCadastros.CADERNO_ENCARGOS_JA_LANCADO);
                        }

                        await repository.SaveOrUpdate(modelo);

                        //await apiDocumento.InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, "Alteração - Incluiu Produção", null, value.Usuario, documento.Data_Maxima, documento.Cod_Tipo_Serv, null, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/OsApiController/GetEquipe")]
        public EquipeIntegracaoModel GetEquipe(String Matricula)
        {
            StringBuilder sql = new StringBuilder();
            EquipeIntegracaoModel retorno = null;

            using (var session = HibernateUtil.GetSessionT5())
            {
                IEquipeAnielRepository equipeAnielRepository = new EquipeAnielRepository(session);
                retorno = equipeAnielRepository.GetEquipe(Matricula);
            }

            return retorno;
        }

        private async Task<List<string>> ExcluiOrdemServico(Documento_ProducaoAniel docProd)
        {
            List<string> erros = new List<string>();
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                string _obs = null;
                string _temOs = "N";
                string numObra = null;
                string dataMaxima = null;
                string usuarioCriacao = null;
                string diaCriacao = null;
                string horaCriacao = null;

                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" SELECT A.NUM_OBRA,A.DATA_MAXIMA,A.USUARIO,A.DIA,A.HORA,COALESCE(A.STATUS_EXECUCAO,0)AS STATUS_EXECUCAO,");
                sql.AppendLine("(SELECT FIRST 1 B.CODMAT FROM TB_MATERIAL_TAREFA B");
                sql.AppendLine("WHERE B.CODCT = @CODCT AND B.PROJETO = @PROJETO");
                sql.AppendLine("AND B.NUM_OBRA = A.NUM_OBRA AND B.NUM_DOC = @NUM_DOC) AS MATERIAL");
                sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A");
                sql.AppendLine(" WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA_ORIGINAL = @NUM_OBRA_ORIGINAL ");
                sql.AppendLine(" AND A.NUM_DOC = @NUM_DOC AND A.DIA = @DIA ");

                parametros.Add("@CODCT", docProd.CodCt);
                parametros.Add("@PROJETO", docProd.Projeto);
                parametros.Add("@NUM_OBRA_ORIGINAL", docProd.Numero_Obra_Original);
                parametros.Add("@NUM_DOC", docProd.Num_Doc);
                parametros.Add("@DIA", docProd.Dia);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    _temOs = "N";
                    while (await reader.ReadAsync())
                    {
                        _temOs = "S";
                        numObra = Convert.ToString(reader["NUM_OBRA"]);
                        dataMaxima = Convert.ToString(reader["DATA_MAXIMA"]);
                        usuarioCriacao = Convert.ToString(reader["USUARIO"]);
                        diaCriacao = Convert.ToString(reader["DIA"]);
                        horaCriacao = Convert.ToString(reader["HORA"]);

                        var temMaterial = String.IsNullOrEmpty(Convert.ToString(reader["MATERIAL"])) == false;
                        var osFechada = Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 1 || Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 2 || Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 8;
                        var osCancelada = Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 9;

                        if (osFechada == false)
                        {
                            if (osCancelada == false)
                            {
                                if (temMaterial == false)
                                {
                                    using (var comExclusao = session.Connection.CreateCommand())
                                    {
                                        sql.Clear();
                                        sql.AppendLine(" DELETE FROM TB_CUSTOS_TAREFA ");
                                        sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        parametros.Add("@NUM_OBRA", numObra);
                                        await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);

                                        //sql.Clear();
                                        //sql.AppendLine(" DELETE FROM TB_MATERIAL_TAREFA ");
                                        //sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        //sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        //await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);

                                        sql.Clear();
                                        sql.AppendLine(" DELETE FROM TB_DOCUMENTO_PRODUCAO ");
                                        sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);
                                    }
                                    _obs = $"A Ordem de Servico {numObra} foi excluída com Sucesso.";
                                    //erros.Add($"A Ordem de Servico {numObra} foi excluída com Sucesso.");
                                }
                                else
                                {
                                    _obs = $"A Ordem de Servico {numObra} possui lançamento de materiais.";
                                    erros.Add($"Ordem de Servico {numObra} possui lançamento de materiais.");
                                }
                            }
                            else
                            {
                                _obs = $"A Ordem de Servico {numObra} encontra-se cancelada.";
                                erros.Add($"Ordem de Servico {numObra} encontra-se cancelada.");
                            }
                        }
                        else
                        {
                            _obs = $"A Ordem de Servico {numObra} encontra-se fechada.";
                            erros.Add($"Ordem de Servico {numObra} encontra-se fechada.");
                        }


                        DocumentoProducaoLog pObjLogProd = new DocumentoProducaoLog();

                        pObjLogProd.codCt = docProd.CodCt;
                        pObjLogProd.projeto = docProd.Projeto;
                        pObjLogProd.numObra = numObra;
                        pObjLogProd.numObraOriginal = docProd.Numero_Obra_Original;
                        pObjLogProd.numDoc = Convert.ToString(docProd.Num_Doc);
                        pObjLogProd.dataMaxima = String.IsNullOrEmpty(dataMaxima) ? null : dataMaxima;
                        pObjLogProd.usuarioCriacao = usuarioCriacao;
                        pObjLogProd.diaCriacao = String.IsNullOrEmpty(diaCriacao) ? null : diaCriacao;
                        pObjLogProd.horaCriacao = String.IsNullOrEmpty(horaCriacao) ? null : horaCriacao;
                        pObjLogProd.usuarioExclusao = User.Identity.GetUserId();
                        pObjLogProd.nomeMaquina = Dns.GetHostName();
                        IPAddress[] ip = Dns.GetHostAddresses(pObjLogProd.nomeMaquina);
                        if (ip != null && ip.Length >= 10)
                        {
                            pObjLogProd.ipMaquina = ip[9].ToString();
                        }
                        pObjLogProd.obs = _obs;
                        await InsereLog(session, pObjLogProd);
                    }

                    if (_temOs == "N")
                    {
                        numObra = docProd.Numero_Obra_Original;
                        _obs = $"A Ordem de Servico {docProd.Numero_Obra_Original} não encontrada no sistema.";
                        erros.Add($"A Ordem de Servico {docProd.Numero_Obra_Original} não encontrada no sistema.");

                        DocumentoProducaoLog pObjLogProd = new DocumentoProducaoLog();

                        pObjLogProd.codCt = docProd.CodCt;
                        pObjLogProd.projeto = docProd.Projeto;
                        pObjLogProd.numObra = numObra;
                        pObjLogProd.numObraOriginal = docProd.Numero_Obra_Original;
                        pObjLogProd.numDoc = Convert.ToString(docProd.Num_Doc);
                        pObjLogProd.dataMaxima = String.IsNullOrEmpty(dataMaxima) ? null : dataMaxima;
                        pObjLogProd.usuarioCriacao = usuarioCriacao;
                        pObjLogProd.diaCriacao = String.IsNullOrEmpty(diaCriacao) ? null : diaCriacao;
                        pObjLogProd.horaCriacao = String.IsNullOrEmpty(horaCriacao) ? null : horaCriacao;
                        pObjLogProd.usuarioExclusao = User.Identity.GetUserId();
                        pObjLogProd.nomeMaquina = Dns.GetHostName();
                        IPAddress[] ip = Dns.GetHostAddresses(pObjLogProd.nomeMaquina);
                        if (ip != null && ip.Length >= 10)
                        {
                            pObjLogProd.ipMaquina = ip[9].ToString();
                        }
                        pObjLogProd.obs = _obs;
                        await InsereLog(session, pObjLogProd);
                    }
                }
            }

            return erros;
        }

        private async Task InsereLog(ISession session, DocumentoProducaoLog docProdLog)
        {
            using (var com = session.Connection.CreateCommand())
            {
                string _msn = null;
                try
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    sql.Clear();
                    sql.AppendLine(" INSERT INTO TB_LOG_EXCLUSAO_OS (CODCT,PROJETO,NUM_OBRA,NUM_OBRA_ORIGINAL,NUM_DOC,DATA_MAXIMA,USUARIO, ");
                    sql.AppendLine("                                 DIA,HORA,USUARIO_EXCLUSAO,DIA_EXCLUSAO,HORA_EXCLUSAO, ");
                    sql.AppendLine("                                 HOSTNAME,IP_MAQUINA_ORIGEM,OBS) ");
                    sql.AppendLine("             VALUES (@pCODCT,@pPROJETO,@pNUM_OBRA,@pNUM_OBRA_ORIGINAL,@pNUM_DOC,@pDATA_MAXIMA,@pUSUARIO, ");
                    sql.AppendLine("                     @pDIA,@pHORA,@pUSUARIO_EXCLUSAO,CURRENT_DATE,CURRENT_TIME, ");
                    sql.AppendLine("                     @pHOSTNAME,@pIP_MAQUINA_ORIGEM,@pOBS) ");
                    parametros.Clear();
                    parametros.Add("pCODCT", docProdLog.codCt);
                    parametros.Add("pPROJETO", docProdLog.projeto);
                    parametros.Add("pNUM_OBRA", docProdLog.numObra);
                    parametros.Add("pNUM_OBRA_ORIGINAL", docProdLog.numObraOriginal);
                    parametros.Add("pNUM_DOC", docProdLog.numDoc);
                    parametros.Add("pDATA_MAXIMA", Convert.ToDateTime(docProdLog.dataMaxima));
                    parametros.Add("pUSUARIO", docProdLog.usuarioCriacao);
                    parametros.Add("pDIA", Convert.ToDateTime(docProdLog.diaCriacao));
                    parametros.Add("pHORA", docProdLog.horaCriacao);
                    parametros.Add("pUSUARIO_EXCLUSAO", docProdLog.usuarioExclusao);
                    parametros.Add("pIP_MAQUINA_ORIGEM", docProdLog.ipMaquina);
                    parametros.Add("pHOSTNAME", docProdLog.nomeMaquina);
                    parametros.Add("pOBS", docProdLog.obs);
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
                catch (Exception ex)
                {
                    _msn = ex.Message;
                }
            }
        }

        [HttpPost]
        [Route("api/OsApiController/ParalisarOs")]
        public async Task<PostApiRetorno> ParalisarOs(DocumentoProducaoParalisarRequestModels request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDocumentoProducaoAnielRepository repo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    await new DocumentoProducaoAnielBusiness(repo).ParalisarAtendimento(request.Contrato,
                                                                                        request.Projeto,
                                                                                        request.CodigoCliente,
                                                                                        request.Os,
                                                                                        request.Justificativa,
                                                                                        request.settings.user);
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

        /// <summary>
        /// Api de Reabrtura de OS dado o parâmetro de NumObraOriginal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OsApiController/ReabrirOrdemServico")]
        public async Task<PostApiRetorno> ReabrirOs(OrdemServicoApiRequest request)
        {

            var retorno = new PostApiRetorno();
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
                {
                    IDocumentoProducaoAnielRepository repositorio = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    var business = new DocumentoProducaoAnielBusiness(repositorio);
                    Documento_ProducaoAniel documento = null;
                    if (String.IsNullOrEmpty(request.Codct) == false &&
                         String.IsNullOrEmpty(request.Projeto) == false &&
                         String.IsNullOrEmpty(request.NumObra) == false)
                    {
                        documento = repositorio.GetAll().Where(p => p.CodCt == request.Codct &&
                                                                    p.Projeto == request.Projeto &&
                                                                    p.Num_Obra == request.NumObra).FirstOrDefault();
                    }
                    else if (!String.IsNullOrEmpty(request.NumObraOriginal))
                    {
                        documento = repositorio.GetAll().Where(p => p.Numero_Obra_Original == request.NumObraOriginal).
                                                         OrderByDescending(p => p.Seq).
                                                         FirstOrDefault();
                    }
                    else
                    {
                        throw new Exception("nenhuma chave especificada para busca de OS");
                    }

                    if (documento.Status != StatusExecucaoAniel.Fechada_Produtiva &&
                        documento.Status != StatusExecucaoAniel.Fechada_Improdutiva &&
                        documento.Status != StatusExecucaoAniel.Cancelado)
                    {
                        throw new Exception("OS não pode ser reaberta porque não se encontra fechada.");
                    }

                    if (documento.Status == StatusExecucaoAniel.Cancelado)
                    {
                        throw new Exception("OS não pode ser reaberta porque se encontra cancelada.");
                    }

                    business.ReabrirOS(documento, request.settings.user);
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Message:{0} \r\n InnerException:{1} \r\n StackTrace:{2}",
                                                ex.Message,
                                                ex.InnerException?.Message,
                                                ex.StackTrace);
            }
            return retorno;
        }

        /// <summary>
        /// Api de Inserção de Anexos.
        /// <param name="request">parametro oculto no swagger "request" é necessário para preenchimento no formulário junto do arquivo.</param>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OsApiController/InserirAnexosOS")]
        public async Task<PostApiRetorno> InserirAnexosOS()
        {
            var retorno = new PostApiRetorno();
            HttpContext context = HttpContext.Current;

            //ajustes para logar enviio do helix
            var requestForm = JsonService.DeserializeObject<OrdemServicoAnexoApiRequest>(context.Request.Form["request"]);
            var files = context.Request.Files;

            List<string> fileNames = files.AllKeys.ToList();
            List<string> extensions = new List<string> { "img", "webp", "jpg", "jpeg", "jfif", "png", "bmp" };
            try
            {
                using (var api = new UsuarioApiController())
                {

                    if (!(await api.GetUsuarioValido(requestForm.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                if (files.Count == 0)
                {
                    throw new Exception("Listagem de Arquivos para Inserção está vazia");
                }
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
                using (ITransaction transactionLancLog = sessionT5.BeginTransaction())
                {
                    try
                    {
                        IDocumentoProducaoAnielRepository repositorio = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                        IDocumentoProducaoFotoAnielRepository repoFoto = new DocumentoProducaoFotoAnielRepository(sessionT5, sessionFoto);
                        var business = new DocumentoProducaoAnielBusiness(repositorio, repoFoto);
                        Documento_ProducaoAniel documento = null;
                        var dicionarioAnexoTemInconsistencias = new Dictionary<string, (byte[] array, bool isFoto, bool erro)>();

                        if (String.IsNullOrEmpty(requestForm.Codct) == false &&
                           String.IsNullOrEmpty(requestForm.Projeto) == false &&
                           String.IsNullOrEmpty(requestForm.NumObra) == false)
                        {
                            documento = repositorio.GetAll().Where(p => p.CodCt == requestForm.Codct &&
                                                                        p.Projeto == requestForm.Projeto &&
                                                                        p.Num_Obra == requestForm.NumObra).FirstOrDefault();
                        }
                        else if (!String.IsNullOrEmpty(requestForm.NumObraOriginal))
                        {
                            documento = repositorio.GetAll().Where(p => p.Numero_Obra_Original == requestForm.NumObraOriginal)
                                                            .OrderByDescending(p => p.Seq).FirstOrDefault();
                        }
                        else
                        {
                            throw new Exception("nenhuma chave especificada para busca de OS");
                        }
                        var arquivos = new List<Arquivo_DocProd>();
                        var fotos = new List<Imagem_DocPord_Mobile>();

                        foreach (var arquivo in files.AllKeys)
                        {
                            byte[] array;
                            try
                            {
                                using (var ms = new MemoryStream())
                                {
                                    await files[arquivo].InputStream.CopyToAsync(ms);
                                    array = ms.ToArray();
                                }
                            }
                            catch (Exception)
                            {
                                array = System.Array.Empty<byte>();
                            }
                            string ext = Path.GetExtension(files[arquivo].FileName.ToLower()).Replace(".", "");
                            bool isFoto = extensions.Where(p => p == ext).Count() > 0;

                            dicionarioAnexoTemInconsistencias.Add(arquivo, (array, isFoto, array == System.Array.Empty<byte>()));
                        }

                        if (dicionarioAnexoTemInconsistencias.Any(p => p.Value.erro == true))
                        {
                            string inconsistencias = String.Join(" || \r\n", dicionarioAnexoTemInconsistencias.Where(p => p.Value.erro == true).Select(p => p.Key).ToList());
                            throw new Exception($"Arquivos preenchidos de forma incorreta. \r\n{inconsistencias}");
                        }

                        fotos.AddRange(dicionarioAnexoTemInconsistencias.Where(p => p.Value.erro == false &&
                                                                                    p.Value.isFoto == true)
                            .Select(arquivo => new Imagem_DocPord_Mobile
                            {
                                Id = repoFoto.GetMaxIdFoto() + 1,
                                CodCt = documento.CodCt,
                                Projeto = documento.Projeto,
                                Num_Doc = documento.Num_Doc,
                                Num_Obra = documento.Num_Obra,
                                Arquivo = arquivo.Value.array,
                                Tipo = Path.GetExtension(files[arquivo.Key].FileName).Replace(".", ""),
                                Dia = DateTime.Now.Date,
                                Hora = DateTime.Now,
                                Usuario = requestForm.settings.user,
                            }).ToList());

                        arquivos.AddRange(dicionarioAnexoTemInconsistencias.Where(p => p.Value.erro == false &&
                                                                                       p.Value.isFoto == false)
                            .Select(arquivo => new Arquivo_DocProd
                            {
                                Id = repoFoto.GetMaxIdArquivo() + 1,
                                Chave = $"{documento.CodCt}&{documento.Projeto}&{documento.Num_Doc}",
                                Arquivo = arquivo.Value.array,
                                Tipo = Path.GetExtension(files[arquivo.Key].FileName).Replace(".", ""),
                                Descricao = Path.GetFileNameWithoutExtension(files[arquivo.Key].FileName),
                                Data = DateTime.Now.Date,
                                Hora = DateTime.Now,
                                Usuario = requestForm.settings.user,
                                Num_Obra = documento.Num_Obra
                            }).ToList());

                        business.InserirAnexos(documento, arquivos, fotos, requestForm.settings.user, transactionLancLog);
                    }
                    catch (Exception ex)
                    {
                        transactionLancLog.Rollback();
                        throw ex;
                    }
                    transactionLancLog.Commit();
                }
                retorno.Ok = true;
                retorno.Mensagem = "Arquivos inseridos com sucesso!";
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}",
                                                ex.Message,
                                                ex?.InnerException?.Message,
                                                ex.StackTrace);
            }
            return retorno;
        }
    }

    public class ArquivoResult : IHttpActionResult
    {
        MemoryStream arquivoStuff;
        string nomeDoArquivo, msg;
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
        HttpStatusCode httpStatusCode;

        public ArquivoResult(MemoryStream data, HttpRequestMessage request, string filename, HttpStatusCode statusCode = HttpStatusCode.OK, string mensagem = "")
        {
            arquivoStuff = data;
            httpRequestMessage = request;
            nomeDoArquivo = filename;
            httpStatusCode = statusCode;
            msg = mensagem;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (httpStatusCode != HttpStatusCode.OK)
            {
                var obj = new { mensagem = msg, ok = false };

                httpResponseMessage = httpRequestMessage.CreateResponse(httpStatusCode, obj, new HttpConfiguration());
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }
            else
            {
                httpResponseMessage = httpRequestMessage.CreateResponse(httpStatusCode);
                httpResponseMessage.Content = new StreamContent(arquivoStuff);
                httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = nomeDoArquivo;
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            }

            return Task.FromResult(httpResponseMessage);
        }




    }

}
