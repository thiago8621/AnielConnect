using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.DAO.map;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Testing.Values;
using log4net;
using MySqlX.XDevAPI;
using NetTopologySuite.Algorithm;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using PlanoProdutoEnum = Aniel.Connect.Model.enumerado.PlanoProdutoEnum;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{

    public class ClienteApiController : BaseApiController
    {

        /// <summary>
        /// Retorna a Listagem de Clientes
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ClienteApi/GetList")]
        public async Task<ApiResponse<ClienteListViewModel>> GetList(ClienteApiRequest request)
        {
            try
            {
                ValidateModel();

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                List<ClienteListViewModel> retorno = new List<ClienteListViewModel>();

                Int32 recordsTotal = 0;

                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();

                    sqlFrom.AppendLine(@"FROM TB_CLIENTE A LEFT OUTER JOIN TB_ENDERECO_CLIENTE B
                                                    ON(A.ID_ENDERECO_PRINCIPAL = B.ID)
                                                    LEFT OUTER JOIN TB_CATEGORIA_CLIENTE C
                                                    ON(A.ID_CATEGORIA = C.ID)
                                                    LEFT OUTER JOIN AspNetUsers D
                                                    ON(A.ID_IDENTITY = D.ID)
                                                    LEFT JOIN TB_TIPO_IMOVEL E
                                                    ON(B.ID_TIPO_IMOVEL = E.COD_TIPO_IMOVEL) ");

                    if (request.Alterado || !String.IsNullOrEmpty(request.CpfCnpj) || !String.IsNullOrEmpty(request.NomeFantasia))
                    {
                        List<string> condicoes = new List<string>();
                        sqlWhere.AppendLine(" WHERE ");

                        if (request.Alterado)
                        {
                            condicoes.Add(" A.ALTERADO = 1 ");
                        }

                        if (!String.IsNullOrEmpty(request.CpfCnpj))
                        {
                            var cpfcnpj = request.CpfCnpj.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty).Replace(" ", String.Empty);
                            condicoes.Add(" A.CPF LIKE @CPFCNPJ ");
                            parametros.Add("@CPFCNPJ", $"%{cpfcnpj}%");
                        }

                        if (!String.IsNullOrEmpty(request.NomeFantasia))
                        {
                            condicoes.Add(" UPPER(A.NOMEFANTASIA) LIKE @NOMEFANTASIA ");
                            parametros.Add("@NOMEFANTASIA", $"%{request.NomeFantasia.ToUpperInvariant()}%");
                        }

                        if (condicoes.Count > 1)
                            sqlWhere.AppendLine(string.Join(" AND ", condicoes.ToList()));
                        else
                            sqlWhere.AppendLine(condicoes.First());
                    }

                    #region Count

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_CLIENTE A ");
                        sql.AppendLine(sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }
                    #endregion

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.COD_CLIENTE_ANIEL, C.DESCRICAO AS CATEGORIA_CLIENTE, COALESCE(DATA_NASC, DATA_ABERTURA_EMPRESA) AS DATA_NASC_ABERTURA,");
                        sql.AppendLine(" A.CPF, A.TIPOPESSOA, A.TELEFONE_FIXO, A.SEXO, A.GRAU_INSTRUCAO,  A.EMAIL_PRINCIPAL, PESSOA_TEL_RECADO,");
                        sql.AppendLine("A.ESTADO_CIVIL, A.TELEFONE_CELULAR, A.TELEFONE_RECADO, B.ENDERECO_CEP, B.ENDERECO_CIDADE, B.ENDERECO_UF, ");
                        sql.AppendLine("B.ENDERECO_COMPLEMENTO, B.ENDERECO_LOGRADOURO, B.ENDERECO_NUMERO, B.ENDERECO_BAIRRO,  ");
                        sql.AppendLine("B.ENDERECO_PONTO_REFERENCIA, B.ENDERECO_LATITUDE, B.ENDERECO_LONGITUDE, B.ID_PAIS, B.ID_TIPO_IMOVEL, ");
                        sql.AppendLine("A.NOMEFANTASIA, A.STATUS, D.USERNAME, E.DESCRICAO AS TIPO_IMOVEL, C.DESCRICAO AS CATEGORIA,");
                        sql.AppendLine(@" (SELECT FIRST 1 CASE
		                                    WHEN F.STATUS IN(0, 4) 
		                                    AND I.STATUS_NEGOCIACAO <> 2 OR I.STATUS_NEGOCIACAO IS NULL
		                                    AND COALESCE(H.DIAS_COMPENSACAO, 0) < DATEDIFF(DAY, F.DATA_VENCIMENTO, CURRENT_DATE)
		                                    AND F.DATA_VENCIMENTO < DATEADD(DAY, -1, COALESCE(CAST(I.DATAHORACRIACAO AS DATE), CURRENT_DATE)) THEN 'Inadimplente'
		                                    ELSE 'Não inadimplente'
		                                    END STATUS_INADIMPLENTE 
		                                    FROM TB_CONTA_RECEBER F
		                                    LEFT JOIN TB_FORMA_PAGAMENTO_CLIENTE G
	                                        ON(G.ID_CLIENTE = A.ID)
		                                    INNER JOIN TB_FORMA_PAGAMENTO H
	                                        ON(H.ID = G.ID_FORMA_PAGAMENTO)
		                                    LEFT JOIN TB_NEGOCIACAO I
	                                        ON(F.ID_NEGOCIACAO = I.ID)
	                                        WHERE F.ID_CLIENTE = A.ID ORDER BY I.DATAHORACRIACAO,H.DATAHORACRIACAO DESC) 
	                                        AS STATUS_INADIMPLENTE ");

                        /*sql.AppendLine("CASE");
                        sql.AppendLine("WHEN F.STATUS IN(0, 4) ");
                        sql.AppendLine("AND I.STATUS_NEGOCIACAO <> 2 OR I.STATUS_NEGOCIACAO IS NULL");
                        sql.AppendLine("AND   COALESCE(H.DIAS_COMPENSACAO, 0) < DATEDIFF(DAY, F.DATA_VENCIMENTO, CURRENT_DATE)");
                        sql.AppendLine("AND F.DATA_VENCIMENTO < DATEADD(DAY, -1, COALESCE(CAST(I.DATAHORACRIACAO AS DATE), CURRENT_DATE)) THEN 'Inadimplente'");
                        sql.AppendLine("ELSE 'Não inadimplente'");
                        sql.AppendLine("END STATUS_INADIMPLENTE");*/

                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ClienteListViewModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    CpfCnpj = Convert.ToString(reader["CPF"]),
                                    NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Cliente>(Convert.ToString(reader["STATUS"])),
                                    Data_nascimento = Convert.ToString(reader["DATA_NASC_ABERTURA"]),
                                    TelefonePrincipal = Convert.ToString(reader["TELEFONE_FIXO"]),
                                    TelefoneCelular = Convert.ToString(reader["TELEFONE_CELULAR"]),
                                    TelefoneRecado = Convert.ToString(reader["TELEFONE_RECADO"]),
                                    Nome_Recado = Convert.ToString(reader["PESSOA_TEL_RECADO"]),
                                    Email = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                    Cep = Convert.ToString(reader["ENDERECO_CEP"]),
                                    Logradouro = Convert.ToString(reader["ENDERECO_LOGRADOURO"]),
                                    Numero = Convert.ToString(reader["ENDERECO_NUMERO"]),
                                    Estado = Convert.ToString(reader["ENDERECO_UF"]),
                                    Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                    Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                    Tipo_Imovel = Convert.ToString(reader["ID_TIPO_IMOVEL"]),
                                    Pais = Convert.ToString(reader["ID_PAIS"]),
                                    Complemento = Convert.ToString(reader["ENDERECO_COMPLEMENTO"]),
                                    Latitude = Convert.ToString(reader["ENDERECO_LATITUDE"]),
                                    Longitude = Convert.ToString(reader["ENDERECO_LONGITUDE"]),
                                    Ponto_Ref = Convert.ToString(reader["ENDERECO_PONTO_REFERENCIA"]),
                                    Cod_Cliente_Aniel = reader["COD_CLIENTE_ANIEL"] != DBNull.Value ? Convert.ToInt64(reader["COD_CLIENTE_ANIEL"]) : (long?)null,
                                });
                            }
                        }
                    }
                }

                //Page e retornando DataTable
                return new ApiResponse<ClienteListViewModel>
                {
                    draw = request.settings.length,
                    recordsFiltered = retorno.Count,
                    data = retorno,
                    quantidadeTotalRegistros = recordsTotal,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClienteListViewModel>
                {
                    error = ex.Message
                };
            }
        }

        /// <summary>
        /// Retorna o Cliente mais detalhado
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ClienteApi/FindById")]
        public async Task<ApiResponseObj<ClienteDadosGeraisViewModel>> FindById(FindByIdApiRequest request)
        {
            ApiResponseObj<ClienteDadosGeraisViewModel> retorno = new ApiResponseObj<ClienteDadosGeraisViewModel>();

            try
            {
                ValidateModel();

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }


                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repoCliente = new Conexao<Cliente>(session))
                    {
                        var cliente = await repoCliente.selectFirst(p => p.Id == request.id);
                        retorno.data = await GetViewObj(session, cliente);
                        retorno.ok = true;

                    }
                }
            }
            catch (Exception ex)
            {
                retorno.error = $"{ex.Message} {ex.InnerException?.Message}";
            }

            return retorno;
        }

        /// <summary>
        /// Atualiza latitude e longitude do cliente
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ClienteApi/AtualizaLatLong")]
        public async Task<PostApiRetorno> AtualizaLatLong(ObjApiRequest<ClienteApiLatLong> request)
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


                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                    //using (var repoPlanoProd = new Conexao<Cliente_PlanoProduto>(session))
                    {
                        var cpf = StringService.Truncate(request.Obj.cpf.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty), 18);
                        var cliente = (await repoCliente.selectFirst(p => p.CpfCnpj.IsLike(cpf, MatchMode.Anywhere)));
                        if (cliente != null)
                        {
                            /*var planoProd = await repoPlanoProd.select(p => p.Cliente.Id == cliente.Id && p.ClienteContrato.NumeroContrato == request.Obj.contrato);
                            foreach (var item in planoProd)
                            {
                                item.Endereco_Instalacao.Latitude = Convert.ToDouble(request.Obj.latitude);
                                item.Endereco_Instalacao.Longitude = Convert.ToDouble(request.Obj.longitude);

                                await repoPlanoProd.SaveOrUpdate(planoProd);
                            }*/

                            var clienteContrato = await repoClienteContrato.select(p => p.Cliente.Id == cliente.Id && p.NumeroContrato == request.Obj.contrato);
                            foreach (var listaCliente in clienteContrato)
                            {
                                if (listaCliente.Cliente.Endereco_Instalacao != null)
                                {
                                    listaCliente.Cliente.Endereco_Instalacao.Latitude = request.Obj.latitude;
                                    listaCliente.Cliente.Endereco_Instalacao.Longitude = request.Obj.longitude;
                                }
                                else
                                {
                                    LogManager.GetLogger("Aniel.Connect.API_Aniel").Error($"[AtualizaLatLong] - Cliente não possui endereço cadastrado no Connect.\n Json Request: {JsonService.SerializeObject(request)} {Environment.NewLine}");

                                }
                                await repoClienteContrato.SaveOrUpdate(listaCliente);
                            }

                            retorno.Ok = true;
                        }
                        else
                        {
                            LogManager.GetLogger("Aniel.Connect.API_Aniel").Error($"[AtualizaLatLong] - Cliente não encontrado no Connect.\n Json Request: {JsonService.SerializeObject(request)} {Environment.NewLine}");

                        }
                    }
                    retorno.Ok = true;
                    retorno.Mensagem = "Operação executada com êxito.";
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.Mensagens = new List<PostApiMensagem> {
                    new PostApiMensagem { Mensagem = ex.StackTrace }
                };
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }

        #region [ContratoCliente]

        /// <summary>
        /// Retorna o(s) contrato(s) vinculado ao cliente
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ClienteApi/ContratoCliente")]
        public async Task<ApiResponseObj<ClienteViewModel>> ContratoCliente(ClienteCpfCnpj request)
        {
            ApiResponseObj<ClienteViewModel> retorno = new ApiResponseObj<ClienteViewModel>();

            try
            {
                ValidateModel();

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    {
                        var cliente = await repoCliente.selectFirst(p => p.CpfCnpj == request.cpf_cnpj);
                        List<ClienteContrato> cliente_contrato = repoClienteContrato.select(x => x.Cliente.Id == cliente.Id).Result.ToList();

                        var retorno_cliente = await GetViewObjCliente(cliente);
                        retorno_cliente.ClienteContrato = new List<ClienteContratoViewModel>();

                        for (int i = 0; i < cliente_contrato.Count; i++)
                        {
                            var retorno_cliente_contrato = await GetViewObjClienteContrato(cliente_contrato[i], cliente);

                            retorno_cliente.ClienteContrato.Add(retorno_cliente_contrato);
                        }

                        retorno.data = retorno_cliente;
                        retorno.ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.error = $"{ex.Message} {ex.InnerException?.Message}";
            }

            return retorno;
        }
        #endregion

        #region [ContratoPlanoProduto]

        /// <summary>
        /// Retorna o(s) contrato(s) de plano/produto vinculado ao cliente
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ClienteApi/ContratoPlanoProduto")]
        public async Task<ApiResponseObj<ClienteViewModel>> ContratoPlanoProduto(ClienteCpfCnpj request)
        {
            ApiResponseObj<ClienteViewModel> retorno = new ApiResponseObj<ClienteViewModel>();

            try
            {
                ValidateModel();

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoEscritorio = new Conexao<EscritorioAniel>(sessionT5))
                    using (var repoArea = new Conexao<AreaAniel>(sessionT5))
                    {
                        var cliente = await repoCliente.selectFirst(p => p.CpfCnpj == request.cpf_cnpj);
                        List<Cliente_PlanoProduto> cliente_plano_produto = repoClientePlanoProduto.select(x => x.Cliente.Id == cliente.Id).Result.ToList();

                        var retorno_cliente = await GetViewObjCliente(cliente);
                        retorno_cliente.ClientePlanoProduto = new List<ClientePlanoProdutoViewModel>();

                        for (int i = 0; i < cliente_plano_produto.Count; i++)
                        {
                            var retorno_cliente_plano_produto = await GetViewObjPlanoProdutoContrato(cliente_plano_produto[i], cliente);
                            retorno_cliente_plano_produto.Desc_Area = repoArea.selectFirst(p => p.Cod_Area == retorno_cliente_plano_produto.Cod_Area).Result.Descricao;
                            retorno_cliente_plano_produto.Desc_Escritorio = repoEscritorio.selectFirst(p => p.Cod_Escritorio == retorno_cliente_plano_produto.Cod_Escritorio).Result.Descricao;

                            retorno_cliente.ClientePlanoProduto.Add(retorno_cliente_plano_produto);
                        }

                        retorno.data = retorno_cliente;
                        retorno.ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.error = $"{ex.Message} {ex.InnerException?.Message}";
            }

            return retorno;
        }
        #endregion

        #region [Métodos Privados]

        #region [GetViewObjCliente]
        private async Task<ClienteViewModel> GetViewObjCliente(Cliente cliente)
        {
            ClienteViewModel retorno = new ClienteViewModel();

            /* ClienteViewModel */
            DateTime dtNasc = (DateTime)cliente.Data_Nascimento;

            retorno.NomeFantasia = cliente.NomeFantasia;
            retorno.Data_Nascimento = dtNasc.ToString("dd/MM/yyyy");
            retorno.CPF_CNPJ = cliente.CpfCnpj;
            retorno.Nacionalidade = cliente.Nacionalidade;
            retorno.Profissao = cliente.Profissao;
            retorno.Tipo_Cliente = cliente.Tipo_Cliente;
            retorno.Status_Cliente = cliente.Status;
            retorno.Tipo_Pessoa = cliente.Tipo_Pessoa;
            retorno.Estado_Civil = cliente.Estado_Civil;
            retorno.Grau_Instrucao = cliente.Grau_Instrucao;
            retorno.Outro_Meio = cliente.Outro_Meio;
            retorno.Porte = cliente.Porte;
            retorno.Status_Prospecto = cliente.Status_Prospecto;

            return await Task.FromResult(retorno);
        }
        #endregion

        #region [GetViewObjClienteContrato]
        private async Task<ClienteContratoViewModel> GetViewObjClienteContrato(ClienteContrato cliente_contrato, Cliente cliente)
        {
            ClienteContratoViewModel retorno = new ClienteContratoViewModel();

            /* ClienteContratoViewModel */

            retorno.Id = cliente_contrato.Id;
            retorno.Id_Cliente = cliente.Id;
            retorno.Numero_Contrato = cliente_contrato.NumeroContrato;
            retorno.Descricao = cliente_contrato.Descricao;
            retorno.Contrato_Automatico = cliente_contrato.ContratoAutomatico;
            retorno.Tem_Carencia = cliente_contrato.TemCarencia;
            retorno.Periodo_Carencia = cliente_contrato.PeriodoCarencia;
            retorno.Multa_Carencia = cliente_contrato.MultaCarencia;
            retorno.Tem_Reajuste = cliente_contrato.TemReajuste;
            retorno.Renovar_Automaticamente = cliente_contrato.RenovarAutomaticamente;
            retorno.Renovar_Carencia = cliente_contrato.RenovaCarenciaAutomaticamente;
            retorno.Tipo_Reajuste = cliente_contrato.TipoReajuste;
            retorno.Frequencia_Reajuste = cliente_contrato.FrequenciaReajuste;
            retorno.Cancela_Plano_Carencia = cliente_contrato.CancelaContratoAutomaticoFinalCarencia;
            retorno.Tem_Pro_Rata = cliente_contrato.TemProRata;
            retorno.Multa_Fidelidade = cliente_contrato.MultaFidelidade;
            retorno.Percentual_Reajuste = cliente_contrato.PercentualReajusteContratual;
            retorno.Valor_Reajuste = cliente_contrato.ValorReajusteContratual;
            retorno.Impedir_Bloqueio_Automatico = cliente_contrato.ImpedirBloqueioAutomatico;
            retorno.Impedir_Bloqueio_Segunda = cliente_contrato.ImpedirBloqueioSegunda;
            retorno.Impedir_Bloqueio_Terca = cliente_contrato.ImpedirBloqueioTerca;
            retorno.Impedir_Bloqueio_Quarta = cliente_contrato.ImpedirBloqueioQuarta;
            retorno.Impedir_Bloqueio_Quinta = cliente_contrato.ImpedirBloqueioQuinta;
            retorno.Impedir_Bloqueio_Sexta = cliente_contrato.ImpedirBloqueioSexta;
            retorno.Impedir_Bloqueio_Sabado = cliente_contrato.ImpedirBloqueioSabado;
            retorno.Impedir_Bloqueio_Domingo = cliente_contrato.ImpedirBloqueioDomingo;
            retorno.Impedir_Bloqueio_Feriado = cliente_contrato.ImpedirBloqueioFeriados;
            retorno.Teste_Gratis = cliente_contrato.TesteGratis;
            retorno.Dias_Teste = cliente_contrato.DiasTeste;
            retorno.Utiliza_Desbloqueio_Conf = cliente_contrato.UtilizaDesbloqueioConfianca;
            retorno.Frequencia_Desbloqueio = cliente_contrato.FrequenciaDesbloqueio;
            retorno.Qtd_Dias_Desbloqueados = cliente_contrato.QtdDiasDesbloqueados;
            retorno.Status_Contrato = cliente_contrato.Status_Contrato;

            return await Task.FromResult(retorno);
        }
        #endregion

        #region [GetViewObjPlanoProdutoContrato]
        private async Task<ClientePlanoProdutoViewModel> GetViewObjPlanoProdutoContrato(Cliente_PlanoProduto cliente_plano_produto, Cliente cliente)
        {
            ClientePlanoProdutoViewModel retorno = new ClientePlanoProdutoViewModel();

            /* ClientePlanoProdutoViewModel */

            DateTime dtHabilitado = cliente_plano_produto.Data_Habilitado != null ? (DateTime)cliente_plano_produto.Data_Habilitado : new DateTime(0001, 01, 01);
            DateTime dtDesabilitado = cliente_plano_produto.Data_Desabilitado != null ? (DateTime)cliente_plano_produto.Data_Desabilitado : new DateTime(0001, 01, 01);

            retorno.Id = cliente_plano_produto.Id;
            retorno.Descricao = cliente_plano_produto.Plano.Descricao;
            retorno.Valor_Real = cliente_plano_produto.Valor_Real;
            retorno.Valor_Instalacao_Real = cliente_plano_produto.Valor_Instalacao_Real;
            retorno.Valor_Instalacao_Final = cliente_plano_produto.Valor_Instalacao_Final;
            retorno.Valor_Final_Total = cliente_plano_produto.Valor_Final_Total;
            retorno.Valor_Final = cliente_plano_produto.Valor_Final;
            retorno.Parcelas_Instalacao = cliente_plano_produto.Parcelas_Instalacao;
            retorno.Dias_Teste_Gratis = cliente_plano_produto.Dias_Teste_Gratis;
            retorno.Periodo_Carencia = cliente_plano_produto.Periodo_Carencia;
            retorno.Meses_Carencia = cliente_plano_produto.Meses_Carencia;
            retorno.End_Instalacao_Principal = cliente_plano_produto.End_Instalacao_Igual_Principal;
            retorno.Data_Habilitado = dtHabilitado.ToString("dd/MM/yyyy");
            retorno.Data_Desabilitado = dtDesabilitado.ToString("dd/MM/yyyy");
            retorno.Cod_Escritorio = cliente_plano_produto.Cod_Escritorio;
            retorno.Cod_Area = cliente_plano_produto.Cod_Area;
            retorno.Cod_Localidade = cliente_plano_produto.Cod_Localidade;
            retorno.Localidade = cliente_plano_produto.Localidade;
            retorno.Numero_Contrato = cliente.Numero_Contrato;
            retorno.Status_Plano_Produto = cliente_plano_produto.Status;
            retorno.Id_Cliente = cliente.Id;
            retorno.Id_Plano = cliente_plano_produto.Plano != null ? cliente_plano_produto.Plano.Id : 0;
            retorno.Id_Produto = cliente_plano_produto.Produto != null ? cliente_plano_produto.Produto.Id : 0;
            retorno.Id_Endereco_Instalacao = cliente_plano_produto.Endereco_Instalacao != null ? cliente_plano_produto.Endereco_Instalacao.Id : 0;
            retorno.Id_Campanha = cliente_plano_produto.Campanha != null ? cliente_plano_produto.Campanha.Id : 0;
            retorno.Id_Motivo_Cancelamento = cliente_plano_produto.Motivo_Cancelamento != null ? cliente_plano_produto.Motivo_Cancelamento.Id : 0;
            retorno.Data_Desbloqueio_Confianca = cliente_plano_produto.Data_Desbloqueio_Confianca;
            retorno.Velocidade_Reduzida = cliente_plano_produto.Velocidade_Reduzida;
            retorno.ReducaoVelocidade = cliente_plano_produto.ReducaoVelocidade;
            retorno.QtdDiasAtraso = cliente_plano_produto.QtdDiasAtraso;
            retorno.VelocidadeDownloadReduzida = cliente_plano_produto.VelocidadeDownloadReduzida;
            retorno.VelocidadeUploadReduzida = cliente_plano_produto.VelocidadeUploadReduzida;
            retorno.ImpedirBloqueio_Automatico = cliente_plano_produto.ImpedirBloqueio_Automatico;
            retorno.ImpedirBloqueio_Sexta = cliente_plano_produto.ImpedirBloqueio_Sexta;
            retorno.ImpedirBloqueio_Sabado = cliente_plano_produto.ImpedirBloqueio_Sabado;
            retorno.ImpedirBloqueio_Domingo = cliente_plano_produto.ImpedirBloqueio_Domingo;
            retorno.ImpedirBloqueio_Feriados = cliente_plano_produto.ImpedirBloqueio_Feriados;
            retorno.UtilizaDesbloqueioConfianca = cliente_plano_produto.UtilizaDesbloqueioConfianca;
            retorno.UtilizaBloqueioDebito = cliente_plano_produto.UtilizaBloqueioDebito;
            retorno.QtdDiasbloqueados = cliente_plano_produto.QtdDiasbloqueados;
            retorno.QtdDiasdesbloqueados = cliente_plano_produto.QtdDiasdesbloqueados;
            retorno.FrequenciaDesbloqueio = cliente_plano_produto.FrequenciaDesbloqueio;
            retorno.Telefonia = cliente_plano_produto.Telefonia;
            retorno.Localidade_Anatel = cliente_plano_produto.Localidade_Anatel;
            retorno.Multa_Carencia = cliente_plano_produto.Multa_Carencia_Reais;
            retorno.Cancel_Contrato_Carencia = cliente_plano_produto.CancelaContratoFimCarencia;
            retorno.Frequencia_Reajuste = cliente_plano_produto.Frequencia_Reajuste;
            retorno.Renovar_Carencia = cliente_plano_produto.Renovar_CarenciaAuto;
            retorno.Login_Radius = cliente_plano_produto.Login_Radius;
            retorno.Senha_Radius = cliente_plano_produto.Senha_Radius;
            retorno.Quantidade_Produto_Variavel = cliente_plano_produto?.Quantidade_Produto_Variavel;
            retorno.Valor_Total_Plano_Variavel = cliente_plano_produto?.Valor_Total_Plano_Variavel;
            return await Task.FromResult(retorno);
        }
        #endregion

        #endregion

        /// <summary>
        /// Retorna a Listagem de Clientes e suas Formas de Pagamento Alterados e Atualiza o Campo ALTERADO para 0
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ClienteApi/GetIntegracao")]
        public async Task<ApiResponseObj<List<object>>> GetIntegracao(ApiRequest request)
        {
            List<Cliente> listClientes = new List<Cliente>();
            List<Forma_Pagamento_Cliente> listaFormas = new List<Forma_Pagamento_Cliente>();
            List<object> retorno = new List<object>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionNH = HibernateUtil.GetSession())
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
                    using (var clientes = new Conexao<Cliente>(sessionNH))
                    using (var formas = new Conexao<Forma_Pagamento_Cliente>(sessionNH))
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, B.ID IDFORMA ");
                        sql.AppendLine(" FROM TB_CLIENTE A ");
                        sql.AppendLine(" JOIN TB_FORMA_PAGAMENTO_CLIENTE B on (B.ID_CLIENTE = A.ID) ");
                        sql.AppendLine(" WHERE A.ALTERADO = 1 OR B.ALTERADO = 1 ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.start, request.settings.length))
                        {
                            while (await reader.ReadAsync())
                            {
                                var cliente = clientes.select(x => x.Id == Convert.ToInt64(reader["ID"])).Result.FirstOrDefault();
                                cliente.Alterado = false;

                                var forma = formas.select(x => x.Id == Convert.ToInt64(reader["IDFORMA"])).Result.FirstOrDefault();
                                forma.Alterado = false;

                                if (!listClientes.Where(x => x.Id == cliente.Id).Any())
                                    listClientes.Add(cliente);

                                if (!listaFormas.Where(x => x.Id == cliente.Id).Any())
                                    listaFormas.Add(forma);

                                retorno.Add(new
                                {
                                    IdCliente = Convert.ToInt64(reader["ID"]),
                                    IdFormaPagamentoCliente = Convert.ToInt64(reader["IDFORMA"])
                                });
                            }
                        }

                        await clientes.SaveOrUpdate(listClientes);
                        await formas.SaveOrUpdate(listaFormas);

                    }

                    return new ApiResponseObj<List<object>>() { ok = true, data = retorno };
                }
                catch (Exception e)
                {
                    return new ApiResponseObj<List<object>>() { ok = false, error = e.Message };
                }
            }
        }

        private async Task<ClienteDadosGeraisViewModel> GetViewObj(ISession session, Cliente obj)
        {
            ClienteDadosGeraisViewModel retorno = new ClienteDadosGeraisViewModel();
            retorno.Identificacao_Cliente = obj.Numero_Contrato;
            retorno.TipoCliente = obj.Tipo_Cliente;
            retorno.Cargo_Responsavel = obj.Cargo_Responsavel;
            retorno.CPFResponsavel = obj.Cpf_Responsavel;
            retorno.Representante = obj.Nome_Resp_Financeiro;
            retorno.Inscricao_Municipal = obj.Inscricao_Municipal;
            retorno.Inscricao_Estadual = obj.Inscricao_Estadual;
            retorno.Porte = obj.Porte;
            retorno.Data_Abertura = obj.Data_Abertura_Empresa;
            retorno.NomeFantasia = obj.NomeFantasia;
            retorno.Ramo_Atividade = obj.Ramo_Atividade;
            retorno.Razao_Social = obj.Razao_Social;
            //retorno.Forma_Envio = obj.Forma_Envio;

            retorno.Id = obj.Id;
            retorno.CpfCnpj = obj.CpfCnpj;
            retorno.NomeCompleto = obj.NomeFantasia;
            retorno.DataNascimento = Convert.ToString(obj.Data_Nascimento);
            retorno.Nacionalidade = obj.Nacionalidade;
            retorno.Sexo = obj.Sexo;
            retorno.EstadoCivil = obj.Estado_Civil;
            retorno.RG = obj.Rg;
            retorno.DataEmissao = Convert.ToString(obj.Data_Emissao_Rg);
            retorno.Orgao = obj.Orgao_Emissao_Rg;
            retorno.Instrucao = obj.Grau_Instrucao;
            retorno.Profissao = obj.Profissao;
            retorno.Pai = obj.Filiacao_Pai;
            retorno.Mae = obj.Filiacao_Mae;
            retorno.Qual_Meio = obj.Outro_Meio;
            retorno.TipoPessoa = obj.Tipo_Pessoa;
            retorno.Id_Organizacao = obj.Organizacao?.Id;
            retorno.Alterado = obj.Alterado;

            //retorno.Id_Vencimento = obj.Vencimento?.Id;
            //retorno.Dia_Vencimento = obj.Vencimento?.DiaVencimento.ToString();

            //retorno.End_Cobranca_Igual_Principal = obj.End_Cobranca_Igual_Principal;

            retorno.Status = obj.Status == Status_Cliente.Ativo ? true : false;
            retorno.Email = obj.Email_Principal;
            retorno.TelefoneFixo = obj.Telefone_Fixo;
            retorno.TelefoneRecado = obj.Telefone_Recado;
            retorno.NomeRecado = obj.Pessoa_Tel_Recado;
            //retorno.Desc_Banco = obj.Banco?.Descricao;

            retorno.Telefone_Celular = obj.Telefone_Celular;
            retorno.TelefoneRecado = obj.Telefone_Recado;


            retorno.Quem_Indicou = obj.Quem_Indicou?.NomeFantasia;

            retorno.Id_Categoria_Cliente = obj.Categoria_Cliente?.Id;
            retorno.Id_Meio_Captacao = obj.Qual_Meio?.Id;
            retorno.Meio_Captacao = obj.Qual_Meio?.Descricao;
            retorno.Id_Quem_Indicou = obj.Quem_Indicou?.Id;

            retorno.EnderecoPrincipal = new EnderecoClienteViewModel
            {
                Id_Pais = obj.Endereco_Principal?.Pais?.Id,
                Id_Tipo_Imovel = obj.Endereco_Principal?.Tipo_Imovel?.Cod_Tipo_Imovel,
                Id_Bairro = obj.Endereco_Principal?.Id_Bairro,
                Bairro = obj.Endereco_Principal?.Bairro,
                CEP = obj.Endereco_Principal?.Cep,
                Id_Cidade = obj.Endereco_Principal?.Id_Cidade,
                Cidade = obj.Endereco_Principal?.Cidade,
                Complemento = obj.Endereco_Principal?.Complemento,
                Id_Endereco = obj.Endereco_Principal != null ? (obj.Endereco_Principal.Id == 0 ? 0 : obj.Endereco_Principal.Id) : (Int64)0,
                Latitude = Convert.ToString(obj.Endereco_Principal?.Latitude),
                Logradouro = obj.Endereco_Principal?.Logradouro,
                Longitude = Convert.ToString(obj.Endereco_Principal?.Longitude),
                Numero = obj.Endereco_Principal?.Numero,
                Ponto_Referencia = obj.Endereco_Principal?.Ponto_Referencia,

                Tipo = Tipo_EnderecoCliente.Principal,
                UF = obj.Endereco_Principal?.UF
            };

            using (var formasPgmt = new Conexao<Forma_Pagamento_Cliente>(session))
            {
                obj.Formas_Pagamento_Cliente = await formasPgmt.select(x => x.Cliente.Id == obj.Id);
            }

            /*if (obj.End_Cobranca_Igual_Principal)
            {
                retorno.EnderecoCobranca = new EnderecoClienteViewModel
                {
                    Id_Pais = obj.Endereco_Principal?.Pais?.Id,
                    Id_Tipo_Imovel = obj.Endereco_Principal?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Id_Bairro = obj.Endereco_Principal?.Id_Bairro,
                    Bairro = obj.Endereco_Principal?.Bairro,
                    CEP = obj.Endereco_Principal?.Cep,
                    Id_Cidade = obj.Endereco_Principal?.Id_Cidade,
                    Cidade = obj.Endereco_Principal?.Cidade,
                    Complemento = obj.Endereco_Principal?.Complemento,
                    Id_Endereco = obj.Endereco_Principal != null ? (obj.Endereco_Principal.Id == 0 ? (Int64)0 : obj.Endereco_Principal.Id) : (Int64)0,
                    Latitude = Convert.ToString(obj.Endereco_Principal?.Latitude),
                    Logradouro = obj.Endereco_Principal?.Logradouro,
                    Longitude = Convert.ToString(obj.Endereco_Principal?.Longitude),
                    Numero = obj.Endereco_Principal?.Numero,
                    Ponto_Referencia = obj.Endereco_Principal?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Cobranca,
                    UF = obj.Endereco_Principal?.UF
                };
            }
            else
            {
                retorno.EnderecoCobranca = new EnderecoClienteViewModel
                {
                    Id_Pais = obj.Endereco_Cobranca?.Pais?.Id,
                    Id_Tipo_Imovel = obj.Endereco_Cobranca?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Id_Bairro = obj.Endereco_Cobranca?.Id_Bairro,
                    Bairro = obj.Endereco_Cobranca?.Bairro,
                    CEP = obj.Endereco_Cobranca?.Cep,
                    Id_Cidade = obj.Endereco_Cobranca?.Id_Cidade,
                    Cidade = obj.Endereco_Cobranca?.Cidade,
                    Complemento = obj.Endereco_Cobranca?.Complemento,
                    Id_Endereco = obj.Endereco_Cobranca != null ? (obj.Endereco_Cobranca.Id == 0 ? (Int64)0 : obj.Endereco_Cobranca.Id) : (Int64)0,
                    Latitude = Convert.ToString(obj.Endereco_Cobranca?.Latitude),
                    Logradouro = obj.Endereco_Cobranca?.Logradouro,
                    Longitude = Convert.ToString(obj.Endereco_Cobranca?.Longitude),
                    Numero = obj.Endereco_Cobranca?.Numero,
                    Ponto_Referencia = obj.Endereco_Cobranca?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Cobranca,
                    UF = obj.Endereco_Cobranca?.UF
                };
            }*/

            /*retorno.Id_Forma_Pagamento = obj.Forma_Pagamento?.Id;
            retorno.Conta_Corrente = obj.Conta_Corrente;
            retorno.Agencia = obj.Agencia;
            retorno.Digito = obj.Digito;
            retorno.Id_Vencimento = obj.Vencimento?.Id;
            retorno.Id_Banco = obj.Banco?.Id;*/

            retorno.Descontos = (obj.Descontos.Count() == 0) ? new List<DescontoListaModel>() :

            obj.Descontos.Select(p => new DescontoListaModel
            {

                Id = p.Id,
                Id_Desconto = p.Desconto != null ? p.Desconto.Id : 0,
                Descricao = p.Desconto?.Descricao,
                Valor_Desconto = Convert.ToString(p.Desconto?.Valor_Desconto),
                Percentual_Desconto = Convert.ToString(p.Desconto?.Percentual_Desconto),
                Tipo_Desconto = p.Desconto?.Tipo_Desconto?.Descricao

            }).ToList();

            //foreach (var item in obj.PlanosProdutos)
            //{
            //    if (item.End_Instalacao_Igual_Principal)
            //    {
            //        retorno.Planos = (obj.PlanosProdutos.Count() == 0) ? new List<PlanoProdutoListModel>() :
            //        obj.PlanosProdutos.ToList().Select(p => new PlanoProdutoListModel
            //        {
            //            Id_Endereco = p.Endereco_Instalacao.Id == 0 ? (Int64)0 : p.Endereco_Instalacao.Id,
            //            Id_Bairro = p.Endereco_Instalacao?.Id_Bairro,
            //            Bairro = p.Endereco_Instalacao?.Bairro,
            //            CEP = p.Endereco_Instalacao?.Cep,
            //            Id_Cidade = p.Endereco_Instalacao?.Id_Cidade,
            //            Cidade = p.Endereco_Instalacao?.Cidade,
            //            Ponto_Referencia = p.Endereco_Instalacao?.Ponto_Referencia,
            //            Complemento = p.Endereco_Instalacao?.Complemento,
            //            Logradouro = p.Endereco_Instalacao?.Logradouro,
            //            Numero = p.Endereco_Instalacao?.Numero,
            //            UF = p.Endereco_Instalacao?.UF,

            //        }).ToList();
            //    }
            //}

            //Pega Planos e produtos diferentes de cancelado

            obj.PlanosProdutos = obj.PlanosProdutos.Where(p => p.Status != Status_PlanoProduto.Venda_Cancelada
                                 && p.Status != Status_PlanoProduto.Aguardando_Aprovacao_Venda).ToList();

            if (obj.PlanosProdutos.Count() == 0)
                retorno.Planos = new List<PlanoProdutoListModel>();
            else
            {
                using (var repoOnu = new Conexao<ONU>(session)) 
                { 
                    retorno.Planos = obj.PlanosProdutos.Select(p => BindPlanoProduto(session, p,repoOnu).Result).ToList();
                }
            }

            if (obj.Formas_Pagamento_Cliente.Count() == 0)
                retorno.Formas_Pagamento = new List<Forma_Pagamento_ClienteModels>();

            return retorno;
        }

        private async Task<PlanoProdutoListModel> BindPlanoProduto(ISession session, Cliente_PlanoProduto planoProduto, Conexao<ONU> repoONU)
        {
            return new PlanoProdutoListModel
            {
                Id = planoProduto.Id,
                Id_Venda = planoProduto.Id_Venda,
                Status_Venda = planoProduto.Id_Venda.HasValue ? (new Conexao<Venda>(session).selectFirst(o => o.Id == planoProduto.Id_Venda))?.Result?.Status : (Status_Venda?)null,
                Id_Plano_Produto = planoProduto.Plano != null ? planoProduto.Plano.Id : planoProduto.Produto.Id,
                Id_Troca_Plano = planoProduto.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null,
                Descricao = planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao,
                ValorPlanoProduto = String.Format("{0:N2}", planoProduto.Valor_Real),
                ValorInstalacao_Final = String.Format("{0:N2}", planoProduto.Valor_Instalacao_Final),
                ValorPlanoProduto_Final = String.Format("{0:N2}", planoProduto.Valor_Final),
                ValorInstalacao = String.Format("{0:N2}", planoProduto.Valor_Instalacao_Real),
                ValorTotal = String.Format("{0:N2}", planoProduto.Valor_Final_Total),
                Qtde_Dias = planoProduto.Dias_Teste_Gratis == null ? 0 : planoProduto.Dias_Teste_Gratis,
                Meses_Carencia = planoProduto.Meses_Carencia,
                Tipo = (PlanoProdutoEnum)(planoProduto.Plano == null ? PlanoProdutoEnum.Produto : PlanoProdutoEnum.Plano),
                Status = planoProduto.Status,
                Status_PlanoProduto_Str = planoProduto.Status != null ? HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(planoProduto.Status.Value) : String.Empty,
                Numero_Contrato = planoProduto?.ClienteContrato?.NumeroContrato,
                Id_Modelo_Contrato = planoProduto?.ClienteContrato?.ModeloContrato?.Id,

                Velocidade_Reduzida = planoProduto.Velocidade_Reduzida,

                Numero_Parcelas = planoProduto.Parcelas_Instalacao == null ? 0 : planoProduto.Parcelas_Instalacao,
                End_Instalacao_Igual_Principal = planoProduto.End_Instalacao_Igual_Principal,

                Teste_Gratis = planoProduto.Dias_Teste_Gratis > 0,
                Periodo_Carencia = planoProduto.Periodo_Carencia,

                Nas = planoProduto.Nas,
                Login_Radius = planoProduto.Login_Radius,
                Senha_Radius = planoProduto.Senha_Radius,

                Id_Onu = (await repoONU.selectFirst(y => y.Cliente_PlanoProduto.Id == planoProduto.Id))?.Id,

                Id_Endereco = (planoProduto.Endereco_Instalacao == null || planoProduto.Endereco_Instalacao?.Id == 0) ? (Int64)0 : planoProduto.Endereco_Instalacao.Id,
                Id_Bairro = planoProduto.Endereco_Instalacao?.Id_Bairro,
                Bairro = planoProduto.Endereco_Instalacao?.Bairro,
                CEP = planoProduto.Endereco_Instalacao?.Cep,
                Id_Cidade = planoProduto.Endereco_Instalacao?.Id_Cidade,
                Cidade = planoProduto.Endereco_Instalacao?.Cidade,
                Ponto_Referencia = planoProduto.Endereco_Instalacao?.Ponto_Referencia,
                Complemento = planoProduto.Endereco_Instalacao?.Complemento,
                Logradouro = planoProduto.Endereco_Instalacao?.Logradouro,
                Numero = planoProduto.Endereco_Instalacao?.Numero,
                UF = planoProduto.Endereco_Instalacao?.UF,

                Latitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Latitude),
                Longitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Longitude),
                Id_Pais = planoProduto.Endereco_Instalacao?.Pais?.Id,
                Id_Tipo_Imovel = planoProduto.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel,

                Cod_Escritorio = planoProduto.Cod_Escritorio,
                Cod_Area = planoProduto.Cod_Area,
                Cod_Localidade = planoProduto.Cod_Localidade,
                Localidade = planoProduto.Localidade,

                Id_Forma_De_Pagamento = planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Id,
                Id_Forma_Pagamento_Cliente = planoProduto.Forma_Pagamento_Cliente?.Id,
                Desc_Forma_Pagto = !String.IsNullOrEmpty(planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao) ? planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao : "Sem Forma de Pagamento"

            };
        }

        private Forma_Pagamento_ClienteModels BindFormaPagamentoCliente(Forma_Pagamento_Cliente f)
        {
            return new Forma_Pagamento_ClienteModels
            {
                Id = f.Id,
                Id_Forma_Pagamento = f.Forma_Pagamento.Id,
                Desc_Forma_Pagamento = f.Forma_Pagamento.Descricao,
                Forma_Envio = f.Forma_Envio,
                Desc_Forma_Envio = f.Forma_Envio.GetDisplayName(),
                Id_Vencimento = f.Vencimento.Id,
                Desc_Banco = !String.IsNullOrEmpty(f?.Banco?.Descricao) ? f?.Banco?.Descricao : "-",
                Id_Banco = f?.Banco?.Id,
                Conta_Corrente = !String.IsNullOrEmpty(f.Conta_Corrente) ? f.Conta_Corrente : "-",
                Agencia = !String.IsNullOrEmpty(f.Agencia) ? f.Agencia : "-",
                Digito = !String.IsNullOrEmpty(f.Digito) ? f.Digito : "-",
                Id_Cliente = f.Cliente.Id,
                Dia_Vencimento = f.Dia_Vencimento,
                Id_EnderecoCobranca = f.Endereco_Cobranca.Id,
                Id_Tipo_ImovelCobranca = f.Endereco_Cobranca.Tipo_Imovel.Id,
                Id_PaisCobranca = f.Endereco_Cobranca.Pais.Id,
                LatitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Latitude),
                LongitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Longitude),
                CEPCobranca = f.Endereco_Cobranca.Cep,
                LogradouroCobranca = f.Endereco_Cobranca.Logradouro,
                NumeroCobranca = f.Endereco_Cobranca.Numero,
                BairroCobranca = f.Endereco_Cobranca.Bairro,
                Id_BairroCobranca = f.Endereco_Cobranca.Id_Bairro,
                Id_CidadeCobranca = f.Endereco_Cobranca.Id_Cidade,
                CidadeCobranca = f.Endereco_Cobranca.Cidade,
                UFCobranca = f.Endereco_Cobranca.UF,
                ComplementoCobranca = f.Endereco_Cobranca.Complemento,
                Ponto_ReferenciaCobranca = f.Endereco_Cobranca.Ponto_Referencia,
                Status = f.Status

            };
        }

        /// <summary>
        /// Endpoint para Alterar o status do cliente baseado na base do IXC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ClienteApi/AlterarStatusClienteIXC")]
        public async Task<PostApiRetorno> AlterarStatusCliente(ObjApiRequest<AlteracaoStatusCliente> request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (UsuarioApiController api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                using (ISession session = HibernateUtil.GetSession())
                using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"Update Into TB_CLIENTE SET STATUS = {request.Obj.Status}")
                       .AppendLine($"Where ID = {request.Obj.Id}");

                    await CommandHelper.ExecuteNonQuery(com, sql, session.Connection, null);
                }
                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                      $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
            }
            return retorno;
        }


        /// <summary>
        /// Endpoint de Captura de Listagem da Base Aniel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ClienteApi/GetListIntegracaoIXC")]
        public async Task<GetApiRetorno<IEnumerable<ClienteListViewModel>>> GetListIXC(ObjApiRequest<bool> request)
        {
            GetApiRetorno<IEnumerable<ClienteListViewModel>> retorno = new GetApiRetorno<IEnumerable<ClienteListViewModel>>
            {
                Objeto = new List<ClienteListViewModel>()
            };
            try
            {
                ClienteApiRequest clienteRequest = new ClienteApiRequest();
                clienteRequest.settings = request.settings;
                clienteRequest.Alterado = request.Obj;

                retorno.Objeto = (await this.GetList(clienteRequest)).data;
            }
            catch (Exception erro)
            {
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                      $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Ok = false;
            }


            return retorno;

        }

        /// <summary>
        /// Endpoint de Integração de Cliente no IXC, buscando por id na base do Aniel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ClienteApi/GetClienteIntegracaoIXC")]
        public async Task<GetApiRetorno<ClienteDadosGeraisViewModel>> GetClienteIXC(ObjApiRequest<long> request)
        {
            GetApiRetorno<ClienteDadosGeraisViewModel> retorno = new GetApiRetorno<ClienteDadosGeraisViewModel>();
            try
            {
                FindByIdApiRequest findByIdRequest = new FindByIdApiRequest
                {
                    id = request.Obj,
                    password = request.settings.password,
                    user = request.settings.user,
                    token = request.settings.token
                };

                ClienteDadosGeraisViewModel obj = (await this.FindById(findByIdRequest)).data;

                retorno.Objeto = obj;
                retorno.Ok = true;
                retorno.Mensagem = "concluído com êxito";
            }
            catch (Exception erro)
            {
                retorno.Mensagem = String.IsNullOrEmpty(erro.Message) ? $"Erro: {erro.Message}" :
                                   $"Erros:\r\n{erro.Message}\r\n{erro.InnerException.Message}";
                retorno.Ok = false;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/ClienteApi/GetClienteIntegracaoOzMap")]
        public async Task<GetApiRetorno<Cliente>> GetClienteOzMapId(ObjApiRequest<string> request)
        {
            {
                GetApiRetorno<Cliente> retorno = new GetApiRetorno<Cliente>();

                try
                {
                    using (ISession session = HibernateUtil.GetSession ())
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        using (UsuarioApiController usuarioapi = new UsuarioApiController())
                        {
                            if (!(await usuarioapi.GetUsuarioValido(request.settings)).Ok)
                            {
                                throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                            }
                        }
                        string Id = request.Obj;
                        StringBuilder sql = new StringBuilder();

                        Dictionary<String, Object> parametros = new Dictionary<string, object>();

                        sql.AppendLine(@"SELECT  A.NOMEFANTASIA, A.ID, A.ID_INTEGRADOR FROM TB_CLIENTE A  WHERE A.ID_INTEGRADOR = @ID");
                        parametros.Add("@ID", Id);

                        using (DbDataReader _reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 1, null))
                        {
                            if (await _reader.ReadAsync())
                            {
                                Cliente obj = new Cliente()
                                {
                                    Id = Convert.ToInt64(_reader["ID"]),
                                    Id_Integrador = _reader["ID_INTEGRADOR"].ToString(),
                                    NomeFantasia =_reader["NOMEFANTASIA"].ToString(),
                                };
                                retorno.Objeto = obj;
                                retorno.Ok = retorno.Objeto != null ? true : false;

                            }
                        }

                    }
                }
                catch (Exception Erro)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = String.IsNullOrEmpty(Erro.InnerException?.Message) ? $"Errso: {Erro.Message}" :
                       $"Erros: \r\n{Erro.Message}\r\n{Erro.InnerException?.Message}";
                }

                return retorno;
            }


        }
        
        /// <summary>
        /// Endpoint para inserir um novo cliente da base do IXC para o connect
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/ClienteApi/InsertClienteIXC")]
        public async Task<PostApiRetorno> InsertCliente(ObjApiRequest<ClienteDadosGeraisViewModel> request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (UsuarioApiController api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                using (ISession session = HibernateUtil.GetSession())
                using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    FillDictionary(request.Obj, parametros);

                    parametros = parametros.Where(p => p.Value != null).ToDictionary(p => p.Key, p => p.Value);
                    List<String> arguments = parametros.Keys.Select(p => { return p.Replace("@", ""); }).ToList();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"UPDATE OR INSERT INTO TB_CLIENTE ({String.Join(",", arguments)})")
                       .AppendLine($" VALUES({String.Join(",", parametros.Keys)}) ");

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                }
                retorno.Ok = true;
                retorno.Mensagem = "Operação executada com êxito.";
            }
            catch (Exception erro)
            {
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Ok = false;
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/ClienteApi/InsertClienteOzMap")]
        public async Task<PostApiRetorno> InsertClienteOzMAp(ObjApiRequest<ClienteOzMap> request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (UsuarioApiController api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                using (ISession session = HibernateUtil.GetSession())
                using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    ClienteOzMap param = request.Obj;
                    
                    FillDictionary(param, parametros);
                    parametros = parametros.Where(p => p.Value != null).ToDictionary(p => p.Key, p => p.Value);
                    List<String> arguments = parametros.Keys.Select(p => { return p.Replace("@", ""); }).ToList();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"UPDATE OR INSERT INTO TB_CLIENTE ({String.Join(",", arguments)})")
                       .AppendLine($" VALUES({String.Join(",", parametros.Keys)}) ");

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                }
                retorno.Ok = true;
                retorno.Mensagem = "Operação executada com êxito.";
            }
            catch (Exception erro)
            {
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Ok = false;
            }
            return retorno;

        }
        private void FillDictionary(ClienteOzMap obj, Dictionary<String, Object> dict)
        {
            dict.Add("@ID_INTEGRADOR", obj.id_Integrador);
            dict.Add("@NOMEFANTASIA", obj.NomeFantasia);
            dict.Add("@ID", obj.Id);
            dict.Add("@ALTERADO", StringService.Truncate($"{Convert.ToInt64(obj?.Alterado)}", 5));

        }

        private void FillDictionary(ClienteDadosGeraisViewModel obj, Dictionary<String, Object> dict)
        {
            dict.Add("@ID", obj.Id);
            dict.Add("@ALTERADO", StringService.Truncate($"{Convert.ToInt64(obj?.Alterado)}", 5));
            dict.Add("@CPF", StringService.Truncate(obj?.CpfCnpj.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty), 18));
            dict.Add("@EMAIL_PRINCIPAL", StringService.Truncate(obj?.Email, 150));
            dict.Add("@SENHA", StringService.Truncate(obj?.Senha, 25));

            //dict.Add("@DATA_NASC", obj?.DataNascimento);
            dict.Add("@FILIACAO_PAI", StringService.Truncate(obj?.Pai, 50));
            dict.Add("@FILIACAO_MAE", StringService.Truncate(obj?.Mae, 50));
            dict.Add("@NOMEFANTASIA", StringService.Truncate(obj?.NomeFantasia, 255));

            dict.Add("@NACIONALIDADE", StringService.Truncate(obj?.Nacionalidade, 25));
            dict.Add("@TELEFONE_FIXO", StringService.Truncate(obj?.TelefoneFixo, 18));
            dict.Add("@TELEFONE_RECADO", StringService.Truncate(obj?.TelefoneRecado, 18));
            dict.Add("@RAZAO_SOCIAL", StringService.Truncate(obj?.Razao_Social, 100));

            dict.Add("@ESTADO_CIVIL", obj?.EstadoCivil);
            dict.Add("@SEXO", obj?.Sexo);
            dict.Add("@TIPOPESSOA", obj?.TipoPessoa);

        }



    }



    public class ClienteApiRequest : ApiRequest
    {
        public bool Alterado { get; set; }
        public string CpfCnpj { get; set; }
        public string NomeFantasia { get; set; }
        public string Cep { get; set; }
        public string Telefone { get; set; }

    }
    public class ClienteleadApiRequest : ApiRequest
    {    
        public string NomeFantasia { get; set; }
        public string Cep { get; set; }
        public string Telefone { get; set; }
        public string CpfCnpj { get; set; }

    }


    public class ClienteApiLatLong : ApiRequest
    {
        public string cpf { get; set; }
        public string contrato { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
    }


}
