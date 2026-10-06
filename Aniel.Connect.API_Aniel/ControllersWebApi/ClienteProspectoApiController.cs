using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using Microsoft.AspNet.Identity;
using MySqlX.XDevAPI;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Geocoding.Google;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;



namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ClienteProspectoApiController : BaseApiController
    {
        /// <summary>
        ///Cadastrar Novos Clientes Prospecto via Api 
        ///</summary>
        ///<param name = "request"> </param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/ClienteProspectoApiController/Cadastrar")]
        public async Task<ApiResponseObj<long>> Cadastrar(Cliente_ProspectoApiRequest request)
        {
            List<Cliente_ProspectoApi> retorno = new List<Cliente_ProspectoApi>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }


                #region Condicao
                var cpf = request.Cpf;
                if (String.IsNullOrEmpty(cpf) == false)
                { cpf = cpf.Replace(".", "").Replace("-", "").Replace("/", ""); }

                DateTime dataAbertura;
                DateTime dataNascimento;

                DateTime.TryParse(request.DataAbertura, out dataAbertura);
                DateTime.TryParse(request.DataNascimento, out dataNascimento);
                if (String.IsNullOrEmpty(request.Nome) || request.Nome.Length > 100)
                { throw new Exception("Houve um problema no campo de Nome."); }
                #endregion

                Cliente dados = new Cliente();
                Endereco_Cliente endereco = new Endereco_Cliente();

                #region BindModel
                await this.BindEndereco(request, endereco);

                using (var session = HibernateUtil.GetSession())
                {
                    using (var repoCliente_Prospecto = new Conexao<Cliente>(session))
                    using (var repoCategoriaCliente_Prospecto = new Conexao<Categoria_Cliente>(session))
                    using (var repoMeioCapCliente_Prospecto = new Conexao<Meio_Captacao>(session))
                    {
                        if (request.Id.HasValue)
                            dados = await repoCliente_Prospecto.selectFirst(p => p.Id == request.Id);

                        if (!String.IsNullOrEmpty(request.Cpf))
                        {
                            var cpflead = await repoCliente_Prospecto.selectFirst(p => p.CpfCnpj == cpf);
                            if (cpflead != null)
                                throw new Exception("Cpf ou Cnpj do Cliente já está cadastrado");

                            if (request.TipoPessoa == Sinapse.Model.enumerados.Tipo_Pessoa.Juridica)
                            {
                                if (!StringService.ValidarCnpj(request.Cpf))
                                    throw new Exception("CNPJ inválido.");
                            }
                            else if (request.TipoPessoa == Sinapse.Model.enumerados.Tipo_Pessoa.Fisica)
                            {
                                if (!StringService.ValidarCpf(request.Cpf))
                                    throw new Exception("Cpf inválido.");
                            }

                        }

                        dados.UsuarioCriacao = request.settings.user;
                        dados.CpfCnpj = cpf;
                        dados.Informacao_Complementares = request.Informacoes_Complementares;
                        dados.Data_Abertura_Empresa = dataAbertura;
                        dados.Data_Nascimento = dataNascimento;
                        dados.NomeFantasia = request.Nome;
                        dados.Filiacao_Mae = request.FiliacaoMae;
                        dados.Filiacao_Pai = request.FiliacaoPai;
                        dados.Endereco_Prospecto = endereco;
                        dados.Endereco_Principal = endereco;
                        dados.Email_Principal = request.Email;
                        dados.Categoria_Cliente = await repoCategoriaCliente_Prospecto.selectFirst(p => p.Id == request.Categoria);
                        dados.Status = request.Status;
                        dados.Telefone_Celular = request.TelefoneCelular;
                        dados.Telefone_Fixo = request.TelefoneFixo;
                        dados.Telefone_Recado = request.TelefoneRecado;
                        dados.Tipo_Cliente = request.TipoCliente;
                        dados.Tipo_Pessoa = request.TipoPessoa;
                        dados.Pessoa_Tel_Recado = request.NomeRecado;
                        dados.Numero_Contrato = String.IsNullOrEmpty(request.Numero_contrato) ? Convert.ToString(repoCliente_Prospecto.MaxString("NUMERO_CONTRATO", "TB_CLIENTE").Result + 1) : request.Numero_contrato;
                        dados.IdCampanha = request.IdCampanha.HasValue ? request.IdCampanha.Value : (Int64?)null;
                        dados.Qual_Meio = await repoMeioCapCliente_Prospecto.selectFirst(p => p.Id == request.Meio_Captacao);

                        IClienteRepository repoCliente = new ClienteRepository(session);
                        var clienteBusiness = new ClienteBusiness(repoCliente);
                        clienteBusiness.GetStatusProspecto(dados);
                        await repoCliente_Prospecto.Save(dados);

                        return new ApiResponseObj<long>()
                        {
                            ok = true,
                            data = dados.Id,
                            error = ""
                        };
                    }
                    #endregion  
                }
            }

            catch (Exception erro)
            {
                CONSTANTES.log4Net.ErrorFormat("message : {0} \r\n InnerException : {1} StackTrace : {2}\r\n",
                                                erro.Message,
                                                erro?.InnerException?.Message,
                                                erro.StackTrace);

                return new ApiResponseObj<long>()
                {
                    ok = false,
                    data = 0,
                    error = erro.Message
                };
            }
        }
        private async Task BindEndereco(Cliente_ProspectoApiRequest request, Endereco_Cliente endereco)
        {
            using (ISession session = HibernateUtil.GetSession())

            using (var repoPaisCliente_Prospecto = new Conexao<Pais>(session))
            using (var repoEnderecoCliente_Prospecto = new Conexao<Endereco_Cliente>(session))
            using (var repoTipoCliente_Prospecto = new Conexao<Tipo_Imovel>(session))
            using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoCondominio = new Conexao<Condominio_Predio>(session))
            using (var repoTipoCidade_Prospecto = new Conexao<Cidade>(session))
            using (var repoUF_Prospecto = new Conexao<Uf>(session))
            using (var repoPai_Prospecto = new Conexao<Pais>(session))
            {
                var parametros = new Dictionary<string, bool>
                {
                    { "Cidade", request.Cidade != 0 },
                    { "EnderecoLogradouro", request.IdEnderecoLogradouro != null },
                    { "CondominioPredio", request.IdCondominioPredio != null }
                };

                var FormasEndereco = parametros.Where(p => p.Value == true).Count();
                KeyValuePair<string, bool> parametroEscolhido = new KeyValuePair<string, bool>();
                switch (FormasEndereco)
                {
                    case 1:
                        parametroEscolhido = parametros.Where(p => p.Value == true).FirstOrDefault();
                        break;
                    default:
                        parametroEscolhido = parametros.Where(p => p.Value == true).LastOrDefault();
                        break;
                }
                //------            Preenchimento de Cidade                
                if (parametroEscolhido.Value == false)
                {
                    return;
                }
                else if (parametroEscolhido.Value == parametros["Cidade"])
                {
                    Cidade cidade = await repoTipoCidade_Prospecto.selectFirst(p => p.Id == request.Cidade);
                    Uf uf = await repoUF_Prospecto.selectFirst(p => p.Id == request.UF);
                    Pais pais = await repoPai_Prospecto.selectFirst(p => p.Id == uf.Pais.Id);


                    if (cidade != null)
                    {
                        endereco.Cidade = cidade.Descricao;
                        endereco.Id_Cidade = cidade?.Id;
                    }
                    if (uf != null)
                    {
                        endereco.UF = uf.Descricao;
                        endereco.Id_UF = uf?.Id;
                    }
                    if (pais != null)
                    {
                        endereco.Pais = await repoPaisCliente_Prospecto.selectFirst(p => p.Id == uf.Pais.Id);
                    }

                    endereco.Logradouro = request.Logradouro;
                    endereco.Bairro = request.Bairro;
                    endereco.Numero = request.Numero;
                    endereco.Complemento = request.Complemento;
                    endereco.Latitude = request.Latitude;
                    endereco.Longitude = request.Longitude;
                    endereco.Cep = request.Cep;
                    endereco.Ponto_Referencia = request.Ponto_Referencia;

                    endereco.Tipo_Imovel = await repoTipoCliente_Prospecto.selectFirst(p => p.Cod_Tipo_Imovel == request.Tipo_Imovel);
                }
                //------    Preenchimento por Cadastro de Endereço e Logradouro                
                else if (parametroEscolhido.Value == parametros["EnderecoLogradouro"])
                {
                    Endereco_Logradouro logradouro = await repoEnderecoLogradouro.selectFirst(p => p.Id == request.IdEnderecoLogradouro);

                    endereco.Numero = request.Numero;
                    endereco.Complemento = request.Complemento;
                    endereco.Latitude = request.Latitude;
                    endereco.Longitude = request.Longitude;
                    endereco.Ponto_Referencia = request.Ponto_Referencia;
                    endereco.Tipo_Imovel = await repoTipoCliente_Prospecto.selectFirst(p => p.Cod_Tipo_Imovel == request.Tipo_Imovel);
                    endereco.Logradouro = logradouro.Descricao;
                    endereco.Bairro = logradouro.Bairro.Descricao;
                    endereco.Busca_Endereco = Model.enumerado.Busca_Endereco.Endereco_Logradouro;
                    endereco.UF = logradouro.UF.Sigla;
                    endereco.Cep = logradouro.Cep;
                    endereco.Pais = logradouro.Pais;
                    endereco.Endereco_Logradouro = logradouro;

                }
                //------    Preenchimento por Cadastro de Condomínio e Prédio
                else if (parametroEscolhido.Value == parametros["CondominioPredio"])
                {
                    Condominio_Predio condominio = await repoCondominio.selectFirst(p => p.Id == request.IdCondominioPredio);
                    endereco.Endereco_Logradouro = condominio?.Endereco_Logradouro;
                    endereco.Logradouro = condominio?.Endereco_Logradouro?.Descricao;
                    endereco.Bairro = condominio?.Endereco_Logradouro.Bairro.Descricao;
                    endereco.UF = condominio?.Endereco_Logradouro?.UF?.Sigla;
                    endereco.Busca_Endereco = Model.enumerado.Busca_Endereco.Condominio_Predio;
                    endereco.Numero = condominio?.Numero;
                    endereco.Complemento = request.Complemento;
                    endereco.Latitude = request.Latitude;
                    endereco.Longitude = request.Longitude;
                    endereco.Ponto_Referencia = request.Ponto_Referencia;
                    endereco.Cep = condominio?.Endereco_Logradouro?.Cep;
                    endereco.Pais = condominio?.Endereco_Logradouro?.Pais;
                    endereco.Tipo_Imovel = await repoTipoCliente_Prospecto.selectFirst(p => p.Cod_Tipo_Imovel == request.Tipo_Imovel);
                    endereco.Condominio_Predio = condominio;
                }
            }

            if (!endereco.Latitude.HasValue || !endereco.Longitude.HasValue || endereco.Latitude == 0 || endereco.Longitude == 0)
            {
                var googleGeocodingService = new GoogleGeocodingService(CONSTANTES.log4Net, CONSTANTES.TokenGoogleAPI);
                var geocodingResult = await googleGeocodingService.GetCoordinatesAsync($"{endereco.Logradouro}, {endereco.Numero} - {endereco.Bairro}, {endereco.UF}, {endereco.Cep}", CancellationToken.None);
                endereco.Latitude = geocodingResult?.Latitude;
                endereco.Longitude = geocodingResult?.Longitude;
            }

        }


        [HttpPost()]
        [Route("api/ClienteProspectoApiController/GetList")]
        public async Task<ApiResponse<ClienteListViewModel>> GetList(ClienteleadApiRequest request)
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

                    sqlFrom.AppendLine(@"FROM TB_CLIENTE A 
                                        LEFT OUTER JOIN TB_ENDERECO_CLIENTE B
                                        ON(A.ID_ENDERECO_PROSPECTO = B.ID)
                                        LEFT JOIN TB_ENDERECO_LOGRADOURO F
                                        ON(F.ID = B.ID_ENDERECO_LOGRADOURO)
                                        LEFT JOIN TB_CIDADE G
                                        ON(G.ID = F.ID_CIDADE)
                                        LEFT JOIN TB_UF H
                                        ON(H.ID = F.ID_UF)
                                        LEFT JOIN TB_BAIRRO I
                                        ON(I.ID = F.ID_BAIRRO)
                                        LEFT OUTER JOIN TB_CATEGORIA_CLIENTE C
                                        ON(A.ID_CATEGORIA = C.ID)
                                        LEFT OUTER JOIN AspNetUsers D
                                        ON(A.ID_IDENTITY = D.ID)
                                        LEFT JOIN TB_TIPO_IMOVEL E
                                        ON(B.ID_TIPO_IMOVEL = E.COD_TIPO_IMOVEL)                       
                        ");

                    string cepFormatado = request.Cep;

                    if (!string.IsNullOrWhiteSpace(cepFormatado))
                    {
                        cepFormatado = cepFormatado.Replace("-", "").Replace(" ", "");

                        if (cepFormatado.Length == 8)
                        {
                            cepFormatado = cepFormatado.Insert(5, "-");
                        }
                    }

                    List<string> condicoes = new List<string>();


                    if (!String.IsNullOrEmpty(request.Telefone) || 
                        !String.IsNullOrEmpty(request.Cep) || 
                        !String.IsNullOrEmpty(request.NomeFantasia) || 
                        !String.IsNullOrEmpty(request.CpfCnpj))
                    {
                        sqlWhere.AppendLine(" WHERE A.ID > 0 AND A.STATUS_PROSPECTO IS NOT NULL AND A.STATUS_PROSPECTO <> 4 AND ");
                        if (!String.IsNullOrEmpty(request.NomeFantasia))
                        {
                            condicoes.Add(" UPPER(A.NOMEFANTASIA) LIKE UPPER(@NOMEFANTASIA) ");
                            parametros.Add("@NOMEFANTASIA", $"%{request.NomeFantasia.ToUpperInvariant()}%");
                        }

                        if (!String.IsNullOrEmpty(request.CpfCnpj))
                        {
                            var cpfCnpj = request.CpfCnpj;
                            if (String.IsNullOrEmpty(cpfCnpj) == false)
                                cpfCnpj = cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

                            condicoes.Add(" A.CPF = @CPF_CNPJ ");
                            parametros.Add("@CPF_CNPJ", StringService.Truncate(cpfCnpj, 18));
                        }

                        if (!String.IsNullOrEmpty(request.Telefone))
                        {
                            condicoes.Add(" ( REPLACE(REPLACE(REPLACE(REPLACE(A.TELEFONE_FIXO,'(', ''), ')', ''), '-', ''), ' ', '') = @TELEFONE" +
                                               " OR REPLACE(REPLACE(REPLACE(REPLACE(A.TELEFONE_CELULAR, '(', ''), ')', ''), '-', ''), ' ', '') = @TELEFONE " +
                                               " OR REPLACE(REPLACE(REPLACE(REPLACE(A.TELEFONE_RECADO,   '(', ''), ')', ''), '-', ''), ' ', '')  = @TELEFONE" +
                                               " OR REPLACE(REPLACE(REPLACE(REPLACE(A.PESSOA_TEL_RECADO, '(', ''), ')', ''), '-', ''), ' ', '') = @TELEFONE)");

                            parametros["@TELEFONE"] = request.Telefone;
                        }
                        if (!String.IsNullOrEmpty(request.Cep))
                        {
                            condicoes.Add(" COALESCE(F.CEP,B.ENDERECO_CEP) = @CEP ");


                            parametros["@CEP"] = cepFormatado;
                        }

                        if (condicoes.Count > 1)
                            sqlWhere.AppendLine(string.Join(" AND ", condicoes.ToList()));
                        else
                            sqlWhere.AppendLine(condicoes.First());
                    }
                    else
                    {
                        sqlWhere.AppendLine(" WHERE A.ID > 0 AND A.STATUS_PROSPECTO IS NOT NULL AND A.STATUS_PROSPECTO <> 4 ");
                    }


                    #region Count

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                        sql.AppendLine(sqlFrom.ToString());
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
                        sql.AppendLine("A.ESTADO_CIVIL, A.TELEFONE_CELULAR, A.TELEFONE_RECADO, COALESCE(G.DESCRICAO,B.ENDERECO_CIDADE) ENDERECO_CIDADE, COALESCE(H.SIGLA,B.ENDERECO_UF) AS ENDERECO_UF,COALESCE(F.CEP,B.ENDERECO_CEP) ENDERECO_CEP, ");
                        sql.AppendLine("B.ENDERECO_COMPLEMENTO, COALESCE(F.DESCRICAO,B.ENDERECO_LOGRADOURO) AS ENDERECO_LOGRADOURO, B.ENDERECO_NUMERO, COALESCE(i.DESCRICAO,B.ENDERECO_BAIRRO) ENDERECO_BAIRRO,  ");
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


    }
}
