using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class FaturamentoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a listagem Faturamentos 
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/FaturamentoApi/GetAll")]
        public async Task<ApiResponse<FaturamentoListModel>> GetAll(FaturamentoApiRequest request)
        {
            List<FaturamentoListModel> retorno = new List<FaturamentoListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(request.IdCliente))
                parametros.Add("IDCLIENTE", request.IdCliente);
            if (!string.IsNullOrEmpty(request.DataInicial))
                parametros.Add("DATAINICIO", request.DataInicial);
            if (!string.IsNullOrEmpty(request.DataFinal))
                parametros.Add("DATAFINAL", request.DataFinal);


            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT D.NUM_PROTOCOLO,B.ID ID_CLIENTE, B.CPF,A.ID ID_FATURA ,A.DATAHORACRIACAO,D.DATA_FECHAMENTO, ");
                    sql.AppendLine("A.DATA_VENCIMENTO,I.VALOR, H.CNPJ FILIAL ");
                    sql.AppendLine("FROM TB_FATURAMENTO_CLIENTE A ");
                    sql.AppendLine("INNER JOIN TB_CLIENTE B ");
                    sql.AppendLine("ON(A.ID_CLIENTE = B.ID) ");
                    sql.AppendLine("INNER JOIN TB_FATURAMENTO_FORMA_PAGAMENTO C ");
                    sql.AppendLine("ON(A.ID_FATURAMENTO_FORMA = C.ID) ");
                    sql.AppendLine("INNER JOIN TB_FATURAMENTO_PROTOCOLO D ");
                    sql.AppendLine("ON(C.ID_FATURAMENTO_PROTOCOLO = D.ID) ");
                    sql.AppendLine("INNER JOIN TB_CONTA_RECEBER E ");
                    sql.AppendLine("ON(E.ID_FATURAMENTO_CLIENTE = A.ID) ");
                    sql.AppendLine("INNER JOIN TB_FORMA_PAGAMENTO_CLIENTE G ");
                    sql.AppendLine("ON(G.ID_CLIENTE = B.ID) ");
                    sql.AppendLine("INNER JOIN TB_FORMA_PAGAMENTO F ");
                    sql.AppendLine("ON(G.ID_FORMA_PAGAMENTO = F.ID) ");
                    sql.AppendLine("INNER JOIN TB_FILIAL H ");
                    sql.AppendLine("ON(H.ID = E.ID_FILIAL) ");
                    sql.AppendLine("INNER JOIN TB_COBRANCA_FATURAMENTO I ");
                    sql.AppendLine("ON(I.ID_FATURAMENTO_CLIENTE = A.ID) ");
                    sql.AppendLine(" WHERE A.STATUS = 3 ");

                    if (!string.IsNullOrEmpty(request.IdCliente))
                        sql.AppendLine(" AND A.ID_CLIENTE = @IDCLIENTE");
                    if (!string.IsNullOrEmpty(request.DataInicial))
                        sql.AppendLine(" AND A.DATAHORACRIACAO > @DATAINICIO");
                    if (!string.IsNullOrEmpty(request.DataFinal))
                        sql.AppendLine(" AND D.DATA_FECHAMENTO > @DATAFINAL");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new FaturamentoListModel
                            {
                                Protocolo = reader["NUM_PROTOCOLO"] != DBNull.Value ? Convert.ToString(reader["NUM_PROTOCOLO"]) : "",
                                Valor = reader["VALOR"] != DBNull.Value ? Convert.ToDouble(reader["VALOR"]).ToString("0.##") : "",
                                CNPJFilial = reader["FILIAL"] != DBNull.Value ? Convert.ToString(reader["FILIAL"]) : "",
                                CPFCNPJ = reader["CPF"] != DBNull.Value ? Convert.ToString(reader["CPF"]) : "",
                                DataEmissao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy") : "",
                                DataFechamento = reader["DATA_FECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_FECHAMENTO"]).ToString("dd/MM/yyyy") : "",
                                DataVencimento = reader["DATA_VENCIMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENCIMENTO"]).ToString("dd/MM/yyyy") : "",
                                IdCliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["ID_CLIENTE"]) : "",
                                IdFatura = reader["ID_FATURA"] != DBNull.Value ? Convert.ToString(reader["ID_FATURA"]) : ""
                            });
                        }
                    }
                }

                return new ApiResponse<FaturamentoListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<FaturamentoListModel>()
                {
                    error = e.Message,
                };
            }
        }

        /// <summary>
        ///  Retorna Composição de faturamento 
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/FaturamentoApi/ComposicaoFatura")]
        public async Task<ApiResponse<ComposicaoFaturaListModel>> ComposicaoFatura(ComposicaoApiRequest request)
        {
            List<ComposicaoFaturaListModel> retorno = new List<ComposicaoFaturaListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            if (request.IdFatura != 0)
                parametros.Add("@ID", request.IdFatura);

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID ID_FATURA, A.ID_PLANO_PRODUTO ,D.ID_CFOP, C.MODELONOTAFISCAL, ");
                    sql.AppendLine(" C.VALOR_DISCRIMINADO VALOR, C.ID_CONFIGURACAO_TRIBUTARIA, D.ID IDPLANO, ");
                    sql.AppendLine(" D.VALORINSTALACAO INSTALACAOPLANO, A.VALOR_INSTALACAO  INSTALACAOCLIENTE ");
                    sql.AppendLine(" FROM TB_FATURAMENTO_CLIENTE_PLANO A ");
                    sql.AppendLine(" JOIN TB_CLIENTE_PLANO_PRODUTO B ON(B.ID = A.ID_PLANO_PRODUTO) ");
                    sql.AppendLine(" JOIN TB_PLANO D on (D.ID = B.ID_PLANO) ");
                    sql.AppendLine(" JOIN TB_CONFIG_NF_PLANOPRODUTO C ON(C.ID_PLANO = B.ID_PLANO) ");
                    sql.AppendLine(" JOIN TB_FATURAMENTO_CLIENTE F ON (F.ID = A.ID_FATURAMENTO_CLIENTE) ");
                    sql.AppendLine(" WHERE A.ID = @ID");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            double valorInstalacaoCliente = reader["INSTALACAOPLANO"] != DBNull.Value ? Convert.ToDouble(reader["INSTALACAOPLANO"]) : 0;

                            if (valorInstalacaoCliente > 0)
                            {                                
                                retorno = await VerificaInstalacaoPlano(session, request.IdFatura);
                            }
                            else
                            {
                                retorno = await VerificaValorPlano(session, request.IdFatura);
                            }

                        }
                    }
                }

                return new ApiResponse<ComposicaoFaturaListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<ComposicaoFaturaListModel>()
                {
                    error = e.Message,
                };
            }
        }

        /// <summary>
        /// Retorna a Listagem de Formas de Pagamento Alterados e Atualiza o Campo ALTERADO para 0
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FormaPagamentoApi/GetIntegracao")]
        public async Task<ApiResponseObj<List<long>>> GetIntegracao(ApiRequest request)
        {
            List<Faturamento_Cliente> listaBanco = new List<Faturamento_Cliente>();
            List<long> retorno = new List<long>();

            using (ISession session = HibernateUtil.GetSession())
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

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    using (var formas = new Conexao<Faturamento_Cliente>(session))
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT ID ");
                        sql.AppendLine(" FROM TB_FATURAMENTO_CLIENTE A ");
                        sql.AppendLine(" WHERE A.ALTERADO = 1 ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.start, request.settings.length))
                        {
                            while (await reader.ReadAsync())
                            {
                                var forma = formas.select(x => x.Id == Convert.ToInt64(reader["ID"])).Result.FirstOrDefault();
                                forma.Alterado = false;

                                listaBanco.Add(forma);

                                retorno.Add(Convert.ToInt64(reader["ID"]));
                            }
                        }

                        await formas.SaveOrUpdate(listaBanco);

                    }

                    return new ApiResponseObj<List<long>>() { ok = true, data = retorno };
                }
                catch (Exception e)
                {
                    return new ApiResponseObj<List<long>>() { ok = false, error = e.Message };
                }
            }
        }

        async Task<List<ComposicaoFaturaListModel>> VerificaInstalacaoPlano(ISession session, long idFatura)
        {
            List<ComposicaoFaturaListModel> retorno = new List<ComposicaoFaturaListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("FATURA", idFatura);

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT A.ID ID_FATURA, A.ID_PLANO_PRODUTO ,D.ID_CFOP, C.MODELONOTAFISCAL, ");
                sql.AppendLine(" C.VALOR_DISCRIMINADO VALORDISCRIMINADO, C.ID_CONFIGURACAO_TRIBUTARIA, D.ID IDPLANO, ");
                sql.AppendLine(" D.VALORINSTALACAO INSTALACAOPLANO, A.VALOR_INSTALACAO  INSTALACAOCLIENTE ");
                sql.AppendLine(" FROM TB_FATURAMENTO_CLIENTE_PLANO A ");
                sql.AppendLine(" JOIN TB_CLIENTE_PLANO_PRODUTO B ON(B.ID = A.ID_PLANO_PRODUTO) ");
                sql.AppendLine(" JOIN TB_PLANO D on (D.ID = B.ID_PLANO) ");
                sql.AppendLine(" JOIN TB_CONFIG_NF_PLANOPRODUTO C ON(C.ID_PLANO = B.ID_PLANO) ");
                sql.AppendLine(" JOIN TB_FATURAMENTO_CLIENTE F ON (F.ID = A.ID_FATURAMENTO_CLIENTE) ");
                sql.AppendLine(" WHERE A.ID = @FATURA AND C.INSTALACAO = 1");

                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                {
                    while (await reader.ReadAsync())
                    {

                        double valorDiscriminado = Convert.ToDouble(reader["VALORDISCRIMINADO"]);//TB_CONFIG_NF_PLANOPRODUTO
                        double valorInstalacaoPlano = Convert.ToDouble(reader["INSTALACAOPLANO"]);//TB_PLANO
                        double valorCalculado = valorDiscriminado / valorInstalacaoPlano;

                        double valorInstalacaoCliente = Convert.ToDouble(reader["INSTALACAOCLIENTE"]);//TB_FATURAMENTO_CLIENTE_PLANO
                        double valorFinal = valorCalculado * valorInstalacaoCliente;

                        retorno.Add(new ComposicaoFaturaListModel
                        {
                            Valor = valorFinal,
                            IdFatura = Convert.ToString(reader["ID_FATURA"]),
                            CFOP = Convert.ToString(reader["ID_CFOP"]),
                            IdClassificacaoTributaria = Convert.ToString(reader["ID_CONFIGURACAO_TRIBUTARIA"]),
                            ModeloNF = HtmlEnumExtensions.GetEnumDescription<Modelo_NotaFiscal>(Convert.ToString(reader["MODELONOTAFISCAL"]))
                        });
                    }
                }
            }

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT A.ID ID_FATURA, A.ID_PLANO_PRODUTO ,D.ID_CFOP, C.MODELONOTAFISCAL, A.VALOR_CLIENTE, ");
                sql.AppendLine(" C.VALOR_DISCRIMINADO VALORDISCRIMINADO, C.ID_CONFIGURACAO_TRIBUTARIA, D.ID IDPLANO, ");
                sql.AppendLine(" D.VALORINSTALACAO INSTALACAOPLANO, A.VALOR_INSTALACAO  INSTALACAOCLIENTE, D.VALORPLANO ");
                sql.AppendLine(" FROM TB_FATURAMENTO_CLIENTE_PLANO A ");
                sql.AppendLine(" JOIN TB_CLIENTE_PLANO_PRODUTO B ON(B.ID = A.ID_PLANO_PRODUTO) ");
                sql.AppendLine(" JOIN TB_PLANO D on (D.ID = B.ID_PLANO) ");
                sql.AppendLine(" JOIN TB_CONFIG_NF_PLANOPRODUTO C ON(C.ID_PLANO = B.ID_PLANO) ");
                sql.AppendLine(" JOIN TB_FATURAMENTO_CLIENTE F ON (F.ID = A.ID_FATURAMENTO_CLIENTE) ");
                sql.AppendLine(" WHERE A.ID = @FATURA AND C.INSTALACAO = 0");

                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                {
                    while (await reader.ReadAsync())
                    {

                        double valorDiscriminado = Convert.ToDouble(reader["VALORDISCRIMINADO"]);//TB_CONFIG_NF_PLANOPRODUTO
                        double valorPlano = Convert.ToDouble(reader["VALORPLANO"]);//TB_PLANO
                        double valorCalculado = valorDiscriminado / valorPlano;

                        double valorCliente = Convert.ToDouble(reader["VALOR_CLIENTE"]);//TB_FATURAMENTO_CLIENTE_PLANO
                        double valorFinal = valorCalculado * valorCliente;

                        retorno.Add(new ComposicaoFaturaListModel
                        {
                            Valor = valorFinal,
                            IdFatura = Convert.ToString(reader["ID_FATURA"]),
                            CFOP = Convert.ToString(reader["ID_CFOP"]),
                            IdClassificacaoTributaria = Convert.ToString(reader["ID_CONFIGURACAO_TRIBUTARIA"]),
                            ModeloNF = HtmlEnumExtensions.GetEnumDescription<Modelo_NotaFiscal>(Convert.ToString(reader["MODELONOTAFISCAL"]))
                        });
                    }
                }
            }

            return retorno;
        }
        
        async Task<List<ComposicaoFaturaListModel>> VerificaValorPlano(ISession session, long idFatura)
        {
            List<ComposicaoFaturaListModel> retorno = new List<ComposicaoFaturaListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("FATURA", idFatura);

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT A.ID ID_FATURA, A.ID_PLANO_PRODUTO ,D.ID_CFOP, C.MODELONOTAFISCAL, ");
                sql.AppendLine(" C.VALOR_DISCRIMINADO VALORDISCRIMINADO, C.ID_CONFIGURACAO_TRIBUTARIA, D.ID IDPLANO ");
                sql.AppendLine(" D.VALORINSTALACAO INSTALACAOPLANO, A.VALOR_INSTALACAO  INSTALACAOCLIENTE, D.VALORPLANO ");
                sql.AppendLine(" FROM TB_FATURAMENTO_CLIENTE_PLANO A ");
                sql.AppendLine(" JOIN TB_CLIENTE_PLANO_PRODUTO B ON(B.ID = A.ID_PLANO_PRODUTO) ");
                sql.AppendLine(" JOIN TB_PLANO D on (D.ID = B.ID_PLANO) ");
                sql.AppendLine(" JOIN TB_CONFIG_NF_PLANOPRODUTO C ON(C.ID_PLANO = B.ID_PLANO) ");
                sql.AppendLine(" JOIN TB_FATURAMENTO_CLIENTE F ON (F.ID = A.ID_FATURAMENTO_CLIENTE) ");
                sql.AppendLine(" WHERE A.ID = @ID AND C.INSTALACAO = 0");

                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                {
                    while (await reader.ReadAsync())
                    {

                        double valorDiscriminado = Convert.ToDouble(reader["VALORDISCRIMINADO"]);//TB_CONFIG_NF_PLANOPRODUTO
                        double valorPlano = Convert.ToDouble(reader["VALORPLANO"]);//TB_PLANO
                        double valorCalculado = valorDiscriminado / valorPlano;

                        double valorCliente = Convert.ToDouble(reader["INSTALACAOCLIENTE"]);//TB_FATURAMENTO_CLIENTE_PLANO
                        double valorFinal = valorCalculado * valorCliente;

                        retorno.Add(new ComposicaoFaturaListModel
                        {
                            Valor = valorFinal,
                            IdFatura = Convert.ToString(reader["ID_FATURA"]),
                            CFOP = Convert.ToString(reader["ID_CFOP"]),
                            IdClassificacaoTributaria = Convert.ToString(reader["ID_CONFIGURACAO_TRIBUTARIA"]),
                            ModeloNF = HtmlEnumExtensions.GetEnumDescription<Modelo_NotaFiscal>(Convert.ToString(reader["MODELONOTAFISCAL"]))
                        });
                    }
                }
            }

            return retorno;
        }

        public class ComposicaoApiRequest : ApiRequest
        {
            public long IdFatura { get; set; }
        }

    }
}