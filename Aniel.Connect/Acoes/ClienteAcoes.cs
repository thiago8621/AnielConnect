using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.Acoes
{
    public class ClienteAcoes : BaseApiController
    {
        public async Task<String> FindTipoPessoa(Int64 Id)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Cliente>(session))
            {
                try
                {
                    var cliente = await repository.selectFirst(p => p.Id == Id);

                    if (cliente.Tipo_Pessoa == null) { throw new Exception("Informe o Tipo Pessoa"); }

                    if (cliente.Tipo_Pessoa == Tipo_Pessoa.Juridica)
                    {
                        return "Pessoa Jurídica";
                    }
                    else
                    {
                        return "Pessoa Física";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<String> MaiorIdade(Int64 Id)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Cliente>(session))
            {
                try
                {
                    var cliente = await repository.selectFirst(p => p.Id == Id);
                    var hoje = DateTime.Today.Year;

                    if (cliente.Data_Nascimento == null) { throw new Exception("Informe a data de nascimento"); }

                    if ((hoje - cliente.Data_Nascimento.Value.Year) >= 18)
                    {
                        return "Pessoa Jurídica";
                    }
                    else
                    {
                        return "Pessoa Física";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<Int32> ExisteCpf(String cpf)
        {
            var clienteApi = new ClienteApiController();

            return await clienteApi.Existecpf(cpf);
        }

        public String ValidaCpf(String cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11) { return "Cpf Inválido"; }

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            resto = soma % 11;

            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digito = resto.ToString();
            tempCpf += digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digito += resto.ToString();

            if (cpf.EndsWith(digito))
            {
                return "Cpf Válido";
            }

            return "Cpf Inválido";
        }

        public async Task<DataTableResponse<ClienteListViewModel>> ConsultaCliente(AcaoConsultaCliente request)
        {
            List<ClienteListViewModel> retorno = new List<ClienteListViewModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_CLIENTE A ")
                                                .AppendLine("LEFT OUTER JOIN TB_ENDERECO_CLIENTE B ")
                                                .AppendLine("ON(A.ID_ENDERECO_PRINCIPAL = B.ID) ")
                                                .AppendLine("LEFT OUTER JOIN TB_CATEGORIA_CLIENTE C ")
                                                .AppendLine("ON(A.ID_CATEGORIA = C.ID) ")
                                                .AppendLine("LEFT OUTER JOIN AspNetUsers D ")
                                                .AppendLine("ON(A.ID_IDENTITY = D.ID) ")
                                                .AppendLine("LEFT JOIN TB_TIPO_IMOVEL E ")
                                                .AppendLine("ON (B.ID_TIPO_IMOVEL = E.COD_TIPO_IMOVEL) ")
                                                .AppendLine("WHERE A.ID > 0 AND TIPO_CLIENTE = 0");

                //Filtrando
                if (!String.IsNullOrEmpty(request.Identificacao))
                {
                    sqlFrom.AppendLine("AND (UPPER(A.NUMERO_CONTRATO) = @NUMERO_CONTRATO)");
                    parametros.Add("@NUMERO_CONTRATO", request.Identificacao);
                }

                if (!String.IsNullOrEmpty(request.Cpf_Cnpj))
                {
                    sqlFrom.AppendLine("AND (UPPER(A.CPF) = @CPF)");
                    parametros.Add("@CPF", request.Cpf_Cnpj);
                }

                if (!String.IsNullOrEmpty(request.Rg))
                {
                    sqlFrom.AppendLine("AND (UPPER(A.RG) = @RG)");
                    parametros.Add("@RG", request.Rg);
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

                    sql.AppendLine("SELECT A.ID, C.DESCRICAO AS CATEGORIA_CLIENTE, A.CPF, A.TIPOPESSOA, A.TELEFONE_FIXO, ");
                    sql.AppendLine("A.SEXO, A.GRAU_INSTRUCAO, A.ESTADO_CIVIL,");
                    sql.AppendLine("A.NOMEFANTASIA, B.ENDERECO_BAIRRO,B.ENDERECO_CIDADE, B.ENDERECO_UF, A.STATUS, D.USERNAME, E.DESCRICAO AS TIPO_IMOVEL, C.DESCRICAO AS CATEGORIA ");

                    sql.AppendLine(sqlFrom.ToString());

                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 2:     // Estado
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(A.CPF)");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(A.CPF) DESC");
                                    break;
                                }
                            case 3:     // Estado
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(A.RG)");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(A.RG) DESC");
                                    break;
                                }
                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader/*First*/(com, sql, parametros, session.Connection, null/*, request.length, request.start*/))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ClienteListViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Pendencia = false,
                                Categoria = (Convert.ToString(reader["CATEGORIA"])),
                                CpfCnpj = Convert.ToString(reader["CPF"]),
                                TipoPessoa = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])),
                                TelefonePrincipal = Convert.ToString(reader["TELEFONE_FIXO"]),
                                NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                Estado = Convert.ToString(reader["ENDERECO_UF"]),
                                Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                Estado_Civil = HtmlEnumExtensions.GetEnumDescription<Estado_Civil>(Convert.ToString(reader["ESTADO_CIVIL"])),
                                Grau_Instrucao = HtmlEnumExtensions.GetEnumDescription<Grau_Instrucao>(Convert.ToString(reader["GRAU_INSTRUCAO"])),
                                Sexo = HtmlEnumExtensions.GetEnumDescription<Sexo>(Convert.ToString(reader["SEXO"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Cliente>(Convert.ToString(reader["STATUS"])),
                                Login = Convert.ToString(reader["USERNAME"]),
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ClienteListViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        public async Task<PostApiRetorno<Int64>> AlterarEnderecoPrincipal(int clienteId, EnderecoClienteViewModel enderecoRequest)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            var clienteApi = new ClienteApiController();

            ParamWebApi<EnderecoClienteViewModel> webRequest = new ParamWebApi<EnderecoClienteViewModel>
            {
                Objeto = enderecoRequest
            };

            var endereco = await clienteApi.GravarEndereco(webRequest);

            if (endereco.Ok)
            {
                Cliente cliente = null;
                Endereco_Cliente enderecoPrincipal = null;

                using ISession session = HibernateUtil.GetSession();
                using var clienteRepository = new Conexao<Cliente>(session);

                cliente = (await clienteRepository.select(p => p.Id == clienteId)).FirstOrDefault();

                BindViewToObj(session, enderecoPrincipal, enderecoRequest);

                cliente.Endereco_Principal = enderecoPrincipal;

                await clienteRepository.SaveOrUpdate(cliente);

                retorno.Ok = true;
            }

            return retorno;
        }

        public async Task<GetApiRetorno<Int64>> CancelarPlano(AlterarStatusViewModel value)
        {
            value.status = 5;
            var planoApi = new PlanoProdutoApiController();
            return await planoApi.AlterarStatus(value);
        }

        public async Task<GetApiRetorno<Int64>> SuspenderPlano(AlterarStatusViewModel value)
        {
            value.status = 3;
            var planoApi = new PlanoProdutoApiController();
            return await planoApi.AlterarStatus(value);
        }

        //Validar dados
        public async Task<PostApiRetorno<Int64>> CadastrarCliente(ClienteDadosGeraisViewModel value)
        {
            var param = new ParamWebApi<ClienteDadosGeraisViewModel>()
            {
                Objeto = value
            };

            var clienteApi = new ClienteApiController();

            return await clienteApi.Gravar(param);
        }

        private void BindViewToObj(ISession session, Endereco_Cliente objBanco, EnderecoClienteViewModel view)
        {
            using var repoTipo_Imovel = new Conexao<Tipo_Imovel>(session);
            using var repoPais = new Conexao<Pais>(session);
            using var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session);
            using var repoCondominioPredio = new Conexao<Condominio_Predio>(session);
            objBanco.Bairro = view.Bairro;
            objBanco.Cep = view.CEP;
            objBanco.Cidade = view.Cidade;
            objBanco.Complemento = view.Complemento;
            objBanco.Id = view.Id_Endereco;
            objBanco.Id_Bairro = view.Id_Bairro;
            objBanco.Id_Cidade = view.Id_Cidade;
            objBanco.Latitude = view.Latitude != null ? Convert.ToDouble(view.Latitude?.Replace('.', ',')) : (Double?)null;
            objBanco.Longitude = view.Longitude != null ? Convert.ToDouble(view.Longitude?.Replace('.', ',')) : (Double?)null;
            objBanco.Logradouro = view.Logradouro;
            objBanco.Numero = view.Numero;
            objBanco.Ponto_Referencia = view.Ponto_Referencia;
            objBanco.UF = view.UF;
            objBanco.Tipo_Imovel = repoTipo_Imovel.selectFirst(p => p.Cod_Tipo_Imovel == view.Id_Tipo_Imovel.Value).Result;
            objBanco.Pais = repoPais.selectFirst(p => p.Id == view.Id_Pais.Value).Result;

            objBanco.Cod_Area = view.Cod_Area;
            objBanco.Cod_Escritorio = view.Cod_Escritorio;
            objBanco.Cod_Localidade = !String.IsNullOrEmpty(view.Cod_Localidade) ? Convert.ToInt32(view.Cod_Localidade) : (Int32?)null;
            objBanco.Localidade = view.Localidade;
            objBanco.Busca_Endereco = (Busca_Endereco)(view.Busca_Endereco != null ? view.Busca_Endereco : null);
            objBanco.Condominio_Predio = view.Busca_Endereco == Busca_Endereco.Condominio_Predio ? repoCondominioPredio.selectFirst(p => p.Id == view.Id_Condominio_Predio).Result : null;
            objBanco.Endereco_Logradouro = view.Busca_Endereco == Busca_Endereco.Endereco_Logradouro ? repoEnderecoLogradouro.selectFirst(p => p.Id == view.Id_Endereco_Logradouro).Result : null;
        }

        [Route("api/ClienteAcoes/GetAllFaturasAberto")]
        [HttpPost()]
        public async Task<List<string>> GetAllFaturasAberto(string idCliente)
        {
            List<string> retorno = new List<string>();
            string mensagemRetorno = "";

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine("FROM TB_CONTA_RECEBER E");

                    sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_CLIENTE A");
                    sqlFrom.AppendLine("ON(E.ID_FATURAMENTO_CLIENTE = A.ID)");
                    sqlFrom.AppendLine("INNER JOIN TB_CLIENTE B");
                    sqlFrom.AppendLine("ON(A.ID_CLIENTE = B.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_FORMA_PAGAMENTO C");
                    sqlFrom.AppendLine("ON(A.ID_FATURAMENTO_FORMA = C.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_PROTOCOLO D");
                    sqlFrom.AppendLine("ON(C.ID_FATURAMENTO_PROTOCOLO = D.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_COBRANCA_FATURAMENTO F");
                    sqlFrom.AppendLine("ON(F.ID_FATURAMENTO_CLIENTE = A.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FORMA_PAGAMENTO G");
                    sqlFrom.AppendLine("ON(G.ID = C.ID_FORMA_PAGAMENTO)");
                    sqlFrom.AppendLine(" WHERE E.STATUS IN (0, 1)");
                    sqlFrom.AppendLine(" AND E.DATA_RECEBIMENTO IS NULL");

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ID, A.VALOR_FINAL,B.CPF, E.VALOR_RECEBIDO,E.DATA_RECEBIMENTO,A.DATA_VENCIMENTO,");
                    sql.AppendLine("A.ID_CLIENTE, E.DOCUMENTO, D.NUM_PROTOCOLO , E.STATUS, G.DESCRICAO AS FORMA_PAGAMENTO,");
                    sql.AppendLine("F.ID_BOLETO, F.LINHA_DIGITAVEL, F.LINK,");
                    sql.AppendLine("B.NOMEFANTASIA AS NOME_CLIENTE, E.ID AS ID_CONTA_RECEBER");
                    sql.AppendLine(sqlFrom.ToString());

                    if (String.IsNullOrEmpty(idCliente))
                    {
                        sql.AppendLine("AND E.ID_CLIENTE = @ID_CLIENTE ");
                        parametros.Add("@ID_CLIENTE", String.Format("{0}", idCliente));
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = reader["ID"]?.ToString();
                            var dataVencimento = reader["DATA_VENCIMENTO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATA_VENCIMENTO"]).ToString("dd/MM/yyyy")
                                : "Não informado";
                            var valorPrevisto = reader["VALOR_FINAL"] != DBNull.Value
                                ? Convert.ToDouble(reader["VALOR_FINAL"]).ToString("N2")
                                : "Indisponível";
                            var protocolo = reader["NUM_PROTOCOLO"]?.ToString() ?? "Não informado";
                            var idContaReceber = reader["ID_CONTA_RECEBER"]?.ToString() ?? "Não informado";
                            var formaPagamento = reader["FORMA_PAGAMENTO"]?.ToString() ?? "Não informado";

                            var dadosBoleto = string.Empty;
                            if (formaPagamento == "Boleto")
                            {
                                var pdf = reader["LINK"]?.ToString() ?? "Link indisponível";
                                var linhaDigitavel = reader["LINHA_DIGITAVEL"]?.ToString() ?? "Linha digitável indisponível";

                                dadosBoleto = $"*Boleto:* {pdf}\r\n*Linha digitável:* {linhaDigitavel}";
                            }

                            // Montar mensagem de retorno
                            if (!string.IsNullOrEmpty(id))
                            {
                                mensagemRetorno += $@"
                                                    *Protocolo:* {protocolo}
                                                    *Valor:* {valorPrevisto}
                                                    *Data de Vencimento:* {dataVencimento}
                                                    *Forma Pagamento:* {formaPagamento}
                                                    {dadosBoleto}
                                                    ";
                            }
                        }

                        retorno.Add(mensagemRetorno);

                    }

                    return retorno;
                }
            }
        }


        [Route("api/ClienteAcoes/ListaFaturasSegundaVia")]
        [HttpPost()]
        public async Task<List<string>> ListaFaturasSegundaVia(string idCliente)
        {
            List<string> retorno = new List<string>();
            string mensagemRetorno = "";

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine("FROM TB_CONTA_RECEBER E");

                    sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_CLIENTE A");
                    sqlFrom.AppendLine("ON(E.ID_FATURAMENTO_CLIENTE = A.ID)");
                    sqlFrom.AppendLine("INNER JOIN TB_CLIENTE B");
                    sqlFrom.AppendLine("ON(A.ID_CLIENTE = B.ID)");
                    sqlFrom.AppendLine("INNER JOIN TB_FATURAMENTO_FORMA_PAGAMENTO C");
                    sqlFrom.AppendLine("ON(A.ID_FATURAMENTO_FORMA = C.ID)");
                    sqlFrom.AppendLine("INNER JOIN TB_FATURAMENTO_PROTOCOLO D");
                    sqlFrom.AppendLine("ON(C.ID_FATURAMENTO_PROTOCOLO = D.ID)");
                    sqlFrom.AppendLine("INNER JOIN TB_COBRANCA_FATURAMENTO F");
                    sqlFrom.AppendLine("ON(F.ID_FATURAMENTO_CLIENTE = A.ID)");
                    sqlFrom.AppendLine("INNER JOIN TB_FORMA_PAGAMENTO G");
                    sqlFrom.AppendLine("ON(G.ID = C.ID_FORMA_PAGAMENTO)");

                    //Busca as faturas dos últimos 60 dias
                    var dataFim = DateTime.Now; // Data de hoje
                    var dataInicio = dataFim.AddDays(-60); // Data de 60 dias atrás
                    sqlFrom.AppendLine("WHERE E.DATA_VENCIMENTO BETWEEN @INICIO AND @FIM");
                    parametros.Add("@INICIO", dataInicio.ToString("yyyy-MM-dd HH:mm:ss"));
                    parametros.Add("@FIM", dataFim.ToString("yyyy-MM-dd HH:mm:ss"));


                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ID, A.VALOR_FINAL,B.CPF, E.VALOR_RECEBIDO,E.DATA_RECEBIMENTO,A.DATA_VENCIMENTO,");
                    sql.AppendLine("A.ID_CLIENTE, E.DOCUMENTO, D.NUM_PROTOCOLO , E.STATUS, G.DESCRICAO AS FORMA_PAGAMENTO,");
                    sql.AppendLine("F.ID_BOLETO, F.LINHA_DIGITAVEL, F.LINK,");
                    sql.AppendLine("B.NOMEFANTASIA AS NOME_CLIENTE, E.ID AS ID_CONTA_RECEBER");
                    sql.AppendLine(sqlFrom.ToString());

                    if (String.IsNullOrEmpty(idCliente))
                    {
                        sql.AppendLine("AND E.ID_CLIENTE = @ID_CLIENTE ");
                        parametros.Add("@ID_CLIENTE", String.Format("{0}", idCliente));
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = reader["ID"]?.ToString();
                            var dataVencimento = reader["DATA_VENCIMENTO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATA_VENCIMENTO"]).ToString("dd/MM/yyyy")
                                : "Não informado";
                            var valorPrevisto = reader["VALOR_FINAL"] != DBNull.Value
                                ? Convert.ToDouble(reader["VALOR_FINAL"]).ToString("N2")
                                : "Indisponível";
                            var protocolo = reader["NUM_PROTOCOLO"]?.ToString() ?? "Não informado";
                            var idContaReceber = reader["ID_CONTA_RECEBER"]?.ToString() ?? "Não informado";
                            var formaPagamento = reader["FORMA_PAGAMENTO"]?.ToString() ?? "Não informado";

                            var dadosBoleto = string.Empty;
                            if (formaPagamento == "Boleto")
                            {
                                var pdf = reader["LINK"]?.ToString() ?? "Link indisponível";
                                var linhaDigitavel = reader["LINHA_DIGITAVEL"]?.ToString() ?? "Linha digitável indisponível";

                                dadosBoleto = $"*Boleto:* {pdf}\r\n*Linha digitável:* {linhaDigitavel}";
                            }

                            // Montar mensagem de retorno
                            if (!string.IsNullOrEmpty(id))
                            {
                                mensagemRetorno += $@"
                                                    *Protocolo:* {protocolo}
                                                    *Valor:* {valorPrevisto}
                                                    *Data de Vencimento:* {dataVencimento}
                                                    *Forma Pagamento:* {formaPagamento}
                                                    {dadosBoleto}
                                                    ";
                            }
                        }

                        retorno.Add(mensagemRetorno);

                    }

                    return retorno;
                }
            }
        }

    }
}