using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{ //Incompleto
    public class Nota_FiscalApiController : BaseApiController
    {
        /// <summary>
        /// Integração para o Aniel receber do CIGAM as notas fiscais emitidas referente aos faturamentos gerados
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/FaturaApi/ReceberNfFaturamento")]
        public async Task ReceberNfFaturamento(ReceberNFRequest request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var nfRepo = new Conexao<Nota_Fiscal_Mestre>(session))
            using (var com = session.Connection.CreateCommand())
            {

                var novaNota = new Nota_Fiscal_Mestre
                {
                    Data_Emissao = request.DataEmissao,
                    Serie_Nota_Fiscal = request.NumeroNotaFiscal,
                    Chave_Cancelamento = request.Chave,
                    IdFatura = request.IdFatura
                };

                await nfRepo.Save(novaNota);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("ID", novaNota.Id + novaNota.IdFatura);
                parametros.Add("IDMESTRE", novaNota.Id);
                parametros.Add("VALOR", request.Valor);

                StringBuilder sql = new StringBuilder(" INSERT INTO TB_NOTA_FISCAL_DETALHE (ID,ID_NOTA_FISCAL_MESTRE,VALOR_BRUTO) ");
                sql.AppendLine(" VALUES(@ID, @IDMESTRE, @VALOR ) ");

                await CommandHelper.ExecuteNonQuery(com, sql, session.Connection, null);
            }
        }

        /// <summary>
        /// Integração para o Aniel receber todos os dados das notas fiscais de entrada de material no estoque do almoxarifado que foram recebidas pelo CIGAM
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 
        [HttpPost()]
        [Route("api/Nota_Fiscalapi/EntradaMaterialEstoque")]
        public async Task EntradaMaterialEstoque(EntradaMaterialEstoqueRequest request)
        {
            long idReceb, idRecebItem = 0;
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            parametros.Add("DATA_RECEBE", request.DataRecebimento.ToString("dd/MM/yyyy"));
            parametros.Add("HORA_RECEBE", request.DataRecebimento.ToString("HH/mm"));
            parametros.Add("PEDIDO", request.Pedido);
            parametros.Add("FORNECEDOR", request.CodigoFornecedor);
            parametros.Add("PROJETO", request.Projeto);
            parametros.Add("CONTRATO", request.Contrato);
            parametros.Add("MATERIAL", request.CodigoMaterial);
            parametros.Add("QUANTIDADE_RECEBIDA", request.QuantidadeRecebida);
            parametros.Add("NF", request.NotaFiscal);
            parametros.Add("VALOR_TOTAL", request.ValorTotal);

            StringBuilder sqlCount = new StringBuilder();
            sqlCount.AppendLine(" SELECT COUNT (ID_RECEB) FROM TB_RECEB_INTEGRA ");

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
            {
                idReceb = reader["COUNT"] != DBNull.Value ? Convert.ToInt64(reader["COUNT"]) : 0;
            }

            sqlCount.Clear();
            sqlCount.AppendLine(" SELECT COUNT (ID_RECEB) FROM TB_RECEB_INTEGRA_ITEM ");

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
            {
                idRecebItem = reader["COUNT"] != DBNull.Value ? Convert.ToInt64(reader["COUNT"]) : 0;
            }

            parametros.Add("idReceb", idReceb);
            parametros.Add("idRecebItem", idRecebItem);

            sql.AppendLine(" INSERT INTO TB_RECEB_INTEGRA ");
            sql.AppendLine(" VALUES(@DATA_RECEBE, @HORA_RECEBE, NULL, @PEDIDO, NULL, 'API ANIEL',@idReceb, @FORNECEDOR) ");

            sql.AppendLine(" INSERT INTO TB_RECEB_INTEGRA_ITEM ");
            sql.AppendLine(" VALUES(@DATA_RECEBE, @DATA_RECEBE, @HORA_RECEBE, @PROJETO, 'API ANIEL', @CONTRATO, @idRecebItem, @idReceb, @MATERIAL, NULL, NULL, @QUANTIDADE_RECEBIDA, NULL, NULL, @NF, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, @VALOR_TOTAL) ");

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

        }

        /// <summary>
        /// Retorna a Listagem de Nota fiscal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 
        [HttpPost()]
        [Route("api/Nota_Fiscalapi/GetAll")]
        public async Task<ApiResponse<Nota_FiscalListaModel>> GetAll(NF_ApiRequest request)
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

                List<Nota_FiscalListaModel> retorno = new List<Nota_FiscalListaModel>();
                Int32 recordsTotal = 0;

                using (ISession session = HibernateUtil.GetSession())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_CONTA_RECEBER A ");//
                        sql.AppendLine(" WHERE A.STATUS IN (0,1) ");
                        sql.AppendLine("   AND EXISTS(SELECT ID FROM TB_NOTA_FISCAL_MESTRE ");
                        sql.AppendLine("                  WHERE ID_CONTA_RECEBER = A.ID) ");

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
                        StringBuilder sqlFrom = new StringBuilder();

                        sqlFrom.AppendLine(" FROM TB_CONTA_RECEBER A ");

                        sqlFrom.AppendLine(" INNER JOIN TB_FORMA_PAGAMENTO B ");
                        sqlFrom.AppendLine(" ON(B.ID = A.ID_PAGTO_CLIENTE) ");

                        sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE C ");
                        sqlFrom.AppendLine(" ON(C.ID = A.ID_CLIENTE) ");

                        sqlFrom.AppendLine(" LEFT JOIN TB_FORNECEDOR D ");
                        sqlFrom.AppendLine(" ON(D.ID = A.ID_FORNECEDOR) ");

                        sqlFrom.AppendLine(" LEFT JOIN TB_FATURAMENTO_CLIENTE E ");
                        sqlFrom.AppendLine(" ON(E.ID = A.ID_FATURAMENTO_CLIENTE) ");

                        sqlFrom.AppendLine(" LEFT JOIN TB_FATURAMENTO_CLIENTE_PLANO F ");
                        sqlFrom.AppendLine(" ON(F.ID_FATURAMENTO_CLIENTE = E.ID) ");

                        sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO G ");
                        sqlFrom.AppendLine(" ON(G.ID_CLIENTE = E.ID_CLIENTE AND G.ID = F.ID_PLANO_PRODUTO) ");

                        sqlFrom.AppendLine(" LEFT JOIN TB_PLANO H ");
                        sqlFrom.AppendLine(" ON(H.ID = G.ID_PLANO) ");

                        sqlFrom.AppendLine(" LEFT JOIN TB_PRODUTO I ");
                        sqlFrom.AppendLine(" ON(I.ID = G.ID_PRODUTO) ");

                        sqlFrom.AppendLine(" LEFT JOIN TB_NOTA_FISCAL_MESTRE J");
                        sqlFrom.AppendLine(" ON(A.ID = J.ID_CONTA_RECEBER)");

                        sqlFrom.AppendLine("INNER JOIN TB_NOTA_FISCAL_LOG K");
                        sqlFrom.AppendLine("ON ( K.ID = A.ID)");
                        sqlFrom.AppendLine("INNER JOIN TB_NOTA_FISCAL_CONTA_RECEBER L");
                        sqlFrom.AppendLine("ON (L.ID = A.ID)");


                        /*************
                         *  FILTROS  *
                         *************/

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID AS ID_CONTA_RECEBER, A.DESCRICAO AS DESCRICAO_CONTA_RECEBER, A.ID_FATURAMENTO_CLIENTE, ");
                        sql.AppendLine("        B.DESCRICAO AS FORMA_PAGAMENTO, C.NOMEFANTASIA, C.CPF, D.RAZAO_SOCIAL,  ");
                        sql.AppendLine("        A.DATA_VENCIMENTO, A.VALOR_PREVISTO, A.DOCUMENTO,  ");
                        sql.AppendLine("        A.DATA_RECEBIMENTO, A.VALOR_RECEBIDO, A.DATA_EMISSAO ");
                        sql.AppendLine(" , (SELECT CNPJ FROM TB_CONFIGURACOES_GERAIS) AS CNPJ_NOTA ");
                        sql.AppendLine(" , (SELECT NOME_CLIENTE_SINAPSE FROM TB_CONFIGURACOES_GERAIS) AS EMPRESA_NOTA, J.STATUS_NOTA_FISCAL, J.MODELO_NOTA_FISCAL,");
                        sql.AppendLine(" L.URL_NOTA_FISCAL, L.URL_XML_NOTA_FISCAL  ");

                        /*************
                         *  FILTROS  *
                         *************/

                        sql.AppendLine(sqlFrom.ToString());

                        sql.AppendLine("GROUP BY A.ID , A.DESCRICAO , A.ID_FATURAMENTO_CLIENTE,");
                        sql.AppendLine("B.DESCRICAO , C.NOMEFANTASIA, C.CPF, D.RAZAO_SOCIAL, ");
                        sql.AppendLine("A.DATA_VENCIMENTO, A.VALOR_PREVISTO, A.DOCUMENTO,  ");
                        sql.AppendLine("A.DATA_RECEBIMENTO, A.VALOR_RECEBIDO, A.DATA_EMISSAO, J.STATUS_NOTA_FISCAL, J.MODELO_NOTA_FISCAL, L.URL_NOTA_FISCAL, L.URL_XML_NOTA_FISCAL  ");

                        //Ordenacao
                        if (request.settings.order != null && request.settings.order.column.HasValue)
                        {
                            int sortColumnIndex = request.settings.order.column.Value;
                            string sortDirection = request.settings.order.dir;
                            switch (sortColumnIndex)
                            {
                                case 1:     // Descrição
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY UPPER(A.DESCRICAO) ");
                                        else
                                            sql.AppendLine(" ORDER BY UPPER(A.DESCRICAO) DESC ");
                                        break;
                                    }
                            }
                        }

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Nota_FiscalListaModel
                                {
                                    //Id = Convert.ToInt64(reader["ID_CONTA_RECEBER"]),
                                    CNPJ_Emissor = Convert.ToString(reader["CNPJ_NOTA"]),
                                    CPF_CNPJ_Receptor = Convert.ToString(reader["CPF"]),
                                    Valor = reader["VALOR_PREVISTO"] != DBNull.Value ? String.Format("{0:N2}", Convert.ToDouble(reader["VALOR_PREVISTO"])) : String.Empty,
                                    Link_NF = Convert.ToString(reader["URL_NOTA_FISCAL"]),
                                    Link_Nota_XML = Convert.ToString(reader["URL_XML_NOTA_FISCAL"]),
                                    Numero_Nota = Convert.ToString(reader["DOCUMENTO"]),

                                });
                            }
                        }

                    }


                    //Page e retornando DataTable
                    return new ApiResponse<Nota_FiscalListaModel>
                    {
                        draw = request.settings.length,
                        recordsFiltered = recordsTotal,
                        data = retorno,
                        error = ""
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Nota_FiscalListaModel>
                {
                    error = ex.Message
                };
            }
        }

    }

    public class EntradaMaterialEstoqueRequest
    {
        public string Contrato { get; set; }
        public string Projeto { get; set; }
        public string Pedido { get; set; }
        public string NotaFiscal { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataRecebimento { get; set; }
        public string CodigoFornecedor { get; set; }
        public string CodigoMaterial { get; set; }
        public int QuantidadeRecebida { get; set; }
        public double ValorTotal { get; set; }
    }

    public class ReceberNFRequest
    {
        public string NumeroNotaFiscal { get; set; }
        public long IdFatura { get; set; }
        public double Valor { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Chave { get; set; }
        public string LinkNotaFiscal { get; set; }
    }

}