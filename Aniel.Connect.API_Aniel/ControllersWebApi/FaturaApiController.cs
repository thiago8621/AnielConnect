using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class FaturaApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a listagem Faturas 
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/FaturaApi/GetAll")]
        public async Task<ApiResponse<FaturaListModel>> GetAll(Fatura_ApiRequest request)
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

                List<FaturaListModel> retorno = new List<FaturaListModel>();
                using (ISession session = HibernateUtil.GetSession())
                {


                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        #region Where
                        StringBuilder sqlWhere = new StringBuilder();

                        if (!String.IsNullOrEmpty(request.DataInicial) || !String.IsNullOrEmpty(request.DataFinal))
                        {
                            DateTime? Data_Final = Convert.ToDateTime(request.DataFinal);
                            DateTime? Data_Inicial = Convert.ToDateTime(request.DataInicial);

                            if ((Data_Final.Value - Data_Inicial.Value).TotalDays >= 90)
                            {
                                throw new Exception("Informe um período dentro de 90 dias");
                            }
                            else
                            {
                                sqlWhere.AppendLine("WHERE (A.DATAHORACRIACAO between @datainicial and @datafinal)");
                                parametros.Add("@datainicial", request.DataInicial);
                                parametros.Add("@datafinal", request.DataFinal);
                            }

                        }

                        if (!String.IsNullOrEmpty(request.Cpf_Cnpj))
                        {
                            if (!(request.Cpf_Cnpj.Length >= 20))
                            {
                                sqlWhere.AppendLine("AND D.CPF = @CPF");
                                parametros.Add("@CPF", request.Cpf_Cnpj);
                            }
                            else
                            {
                                throw new Exception("CPF/CNPJ Inválido.");
                            }
                        }
                        #endregion

                        #region From
                        StringBuilder sqlFrom = new StringBuilder();
                        sqlFrom.AppendLine("FROM TB_COBRANCA_FATURAMENTO A");
                        sqlFrom.AppendLine("LEFT JOIN TB_PLANO B");
                        sqlFrom.AppendLine("ON B.DESCRICAO = B.DESCRICAO");
                        sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_CLIENTE C");
                        sqlFrom.AppendLine("ON A.ID_FATURAMENTO_CLIENTE = C.ID");
                        sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE D");
                        sqlFrom.AppendLine("ON C.ID_CLIENTE = D.ID");

                        #endregion

                        #region Sql
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(@"SELECT A.ID AS ID_BOLETO, C.ID AS ID_FATURAMENTO_CLIENTE,B.DESCRICAO AS PLANO,
                        A.DATA_VENCIMENTO, A.LINK, A.VALOR, A.STATUS,A.LINHA_DIGITAVEL,
                        A.DATAHORACRIACAO, C.ID_CLIENTE,D.CPF");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());
                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new FaturaListModel
                                {
                                    Código_Barras = Convert.ToString(reader["LINHA_DIGITAVEL"]),
                                    Data_Pagamento = Convert.ToString(reader["DATAHORACRIACAO"]),
                                    Data_Vencimento = Convert.ToString(reader["DATA_VENCIMENTO"]),
                                    Id_Fatura = Convert.ToString(reader["ID_BOLETO"]),
                                    Cpf_Cnpj_Cliente = Convert.ToString(reader["CPF"]),
                                    Link_Fatura = Convert.ToString(reader["LINK"]),
                                    Valor = Convert.ToString(reader["VALOR"]),
                                    Id_Cliente = Convert.ToString(reader["ID_CLIENTE"]),
                                });
                            }
                        }
                    }

                    return new ApiResponse<FaturaListModel>()
                    {
                        data = retorno,
                        quantidadeTotalRegistros = retorno.Count,
                        recordsFiltered = retorno.Count,
                        error = "",
                    };
                }
            }
            catch (Exception e)
            {
                return new ApiResponse<FaturaListModel>()
                {
                    error = e.Message,
                };
            }
        }

        /// <summary>
        /// Integração para o Aniel receber do CIGAM a informação de pagamento das faturas 
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/FaturaApi/ReceberFaturaAniel")]
        public async Task ReceberFaturaAniel(FaturaAnielRequest request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("ID", request.IdFatura);
                parametros.Add("VALOR", request.Valor);
                parametros.Add("DATA", request.DataPagamento);

                sql.AppendLine(" UPDATE TB_CONTA_RECEBER ");
                sql.AppendLine(" SET DATA_RECEBIMENTO = @DATA, VALOR_RECEBIDO = @VALOR, STATUS = 1");
                sql.AppendLine(" WHERE ID_FATURAMENTO_PRODUCAOANIEL = @ID ");


                await CommandHelper.ExecuteNonQuery(com, sql, session.Connection, null);
            }

        }

        [HttpPost]
        [Route("api/Faturaapi/GravarFatura")]
        public async Task<ApiResponse<CobrancaFaturamento>> GravarFatura(Fatura_Cadastro_Model request)

        {
            var objRetorno = new List<CobrancaFaturamento>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                using (var cobrancaRepo = new Conexao<CobrancaFaturamento>(session))
                using (var clienteRepo = new Conexao<Cliente>(session))
                {
                    if (request.IdFaturaNegociada > 0)
                    {
                        var nova = new CobrancaFaturamento()
                        {
                            FaturamentoCliente = new Faturamento_Cliente { Cliente = await clienteRepo.selectFirst(x => x.CpfCnpj == request.Cpf_Cliente) },
                            vencimento = Convert.ToDateTime(request.Data_Vencimento),
                            Link_Boleto = request.URL,
                            linhaDigitavel = request.CodigoBarras
                        };

                        await cobrancaRepo.Save(nova);

                        var antiga = await cobrancaRepo.selectFirst(x => x.Id == request.IdFaturaNegociada);
                        antiga.Status = Model.enumerado.Cobranca_StatusPagamento.Negociado;
                        antiga.DataHoraAlteracao = Convert.ToDateTime(request.DataNegociacao);

                        await cobrancaRepo.Update(antiga);

                        objRetorno.Add(nova);
                    }
                    else
                    {
                        var nova = new CobrancaFaturamento()
                        {
                            FaturamentoCliente = new Faturamento_Cliente { Cliente = await clienteRepo.selectFirst(x => x.CpfCnpj == request.Cpf_Cliente) },
                            vencimento = Convert.ToDateTime(request.Data_Vencimento),
                            Link_Boleto = request.URL,
                            linhaDigitavel = request.CodigoBarras
                        };

                        await cobrancaRepo.Save(nova);
                        objRetorno.Add(nova);
                    }

                    return new ApiResponse<CobrancaFaturamento>
                    {
                        data = objRetorno,
                    };
                }
            }
            catch (Exception e)
            {

                return new ApiResponse<CobrancaFaturamento>()
                {
                    draw = request.settings.length,
                    data = null,
                    error = e.Message
                };
            }
        }
    }
    public class FaturaAnielRequest
    {
        public long IdFatura { get; set; }
        public double Valor { get; set; }
        public DateTime DataPagamento { get; set; }
    }

}