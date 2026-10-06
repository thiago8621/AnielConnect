using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ClienteContrato;
using Aniel.Connect.Repository.ClientePlanoProduto;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.ClienteContrato;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml;
using Microsoft.AspNet.Identity;
using Microsoft.Win32;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Resources;
using Sinapse.Core;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.Certificacao_Assinatura.Integracoes;
using Sinapse.Integracao.Certificacao_Assinatura.Repositorio;
using Sinapse.Integracao.MostqContenctExtractionModels.Models;
using Sinapse.Integracao.MostqVioExtractionModels.Models;
using Sinapse.Integracao.ProvaVida.Integracoes;
using Sinapse.Integracao.ProvaVida.Models;
using Sinapse.Integracao.ProvaVida.Repositorio;
using Sinapse.Integracao.ReconhecimentoFacial.Integracoes;
using Sinapse.Integracao.ReconhecimentoFacial.Models;
using Sinapse.Integracao.ReconhecimentoFacial.Repositorio;
using Sinapse.Integracao.ValidacaoDocumento.Integracoes;
using Sinapse.Integracao.ValidacaoDocumento.Repositorio;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using Elemento_Rede_Porta = Sinapse.Model.Elemento_Rede_Porta;
using Status_Conta_Receber = Aniel.Connect.Model.enumerado.Status_Conta_Receber;

namespace Aniel.Connect.ControllersWebApi
{

    [Authorize]
    public class ClienteApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/clienteapi/GetAll")]
        public async Task<DataTableResponse<ClienteListViewModel>> GetAll(Cliente_DataTableRequest request)
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
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "cpf")
                    {
                        sqlFrom.AppendLine("AND (A.CPF LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty)));
                    }
                    else if (request.search.option == "nome")
                    {
                        sqlFrom.AppendLine("AND (UPPER(A.NOMEFANTASIA) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "cidade")
                    {
                        sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_CIDADE) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "estado")
                    {
                        sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_UF) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "bairro")
                    {
                        sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_BAIRRO) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "categoria")
                    {
                        sqlFrom.AppendLine("AND (UPPER(C.DESCRICAO) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "telefone")
                    {
                        sqlFrom.AppendLine("AND (UPPER(A.TELEFONE_FIXO) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "razao_social")
                    {
                        sqlFrom.AppendLine("AND (UPPER(A.RAZAO_SOCIAL) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }


                }

                if (!String.IsNullOrEmpty(request.Tipo_Pessoa))
                {
                    sqlFrom.AppendLine("AND (A.TIPOPESSOA = @TIPOPESSOA)");
                    parametros.Add("@TIPOPESSOA", Convert.ToInt32(request.Tipo_Pessoa));
                }

                if (!String.IsNullOrEmpty(request.Status))
                {
                    sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                    parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                }

                if (!String.IsNullOrEmpty(request.Estado))
                {
                    sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_UF) = @ESTADO)");
                    parametros.Add("@ESTADO", request.Estado);
                }

                if (!String.IsNullOrEmpty(request.Cidade))
                {
                    sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_CIDADE) = @CIDADE)");
                    parametros.Add("@CIDADE", request.Cidade);
                }

                if (!String.IsNullOrEmpty(request.Bairro))
                {
                    sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_BAIRRO) = @BAIRRO)");
                    parametros.Add("@BAIRRO", request.Bairro);
                }

                if (!String.IsNullOrEmpty(request.Tipo_Imovel))
                {
                    sqlFrom.AppendLine("AND (UPPER(E.TIPO_IMOVEL) = @TIPO_IMOVEL)");
                }


                if (!String.IsNullOrEmpty(request.Login_Radius))
                {
                    sqlFrom.AppendLine("AND(0 < (SELECT COUNT(E.ID_CLIENTE) FROM TB_CLIENTE_PLANO_PRODUTO E WHERE E.ID_CLIENTE = A.ID AND E.LOGIN_RADIUS = @LOGIN_RADIUS))");
                    parametros.Add("@LOGIN_RADIUS", request.Login_Radius);
                }


                if (!String.IsNullOrEmpty(request.Id_Plano_Produto))
                {
                    sqlFrom.AppendLine("AND(0 < (SELECT COUNT(E.ID_CLIENTE) FROM TB_CLIENTE_PLANO_PRODUTO E WHERE E.ID_CLIENTE = A.ID AND (E.ID_PLANO = @ID_PLANO_PRODUTO OR E.id_produto = @ID_PLANO_PRODUTO )))");
                    parametros.Add("@ID_PLANO_PRODUTO", request.Id_Plano_Produto);
                }

                if (!String.IsNullOrEmpty(request.Mes_Aniversario))
                {
                    Int32 mes = Convert.ToInt32(request.Mes_Aniversario) + 1;
                    sqlFrom.AppendLine("AND (EXTRACT(MONTH FROM A.DATA_NASC) = @MES_NASCIMENTO)");
                    parametros.Add("@MES_NASCIMENTO", mes);
                }

                if (!String.IsNullOrEmpty(request.Categoria_Cliente))
                {
                    sqlFrom.AppendLine("AND (A.ID_CATEGORIA = @ID_CATEGORIA)");
                    parametros.Add("@ID_CATEGORIA", request.Categoria_Cliente);
                }


                if (!String.IsNullOrEmpty(request.Sexo))
                {
                    sqlFrom.AppendLine("AND (A.SEXO = @SEXO)");
                    parametros.Add("@SEXO", request.Sexo);
                }

                if (!String.IsNullOrEmpty(request.Estado_Civil))
                {
                    sqlFrom.AppendLine("AND (A.ESTADO_CIVIL = @ESTADO_CIVIL)");
                    parametros.Add("@ESTADO_CIVIL", request.Estado_Civil);
                }

                if (!String.IsNullOrEmpty(request.Grau_Instrucao))
                {
                    sqlFrom.AppendLine("AND (A.GRAU_INSTRUCAO = @GRAU_INSTRUCAO)");
                    parametros.Add("@GRAU_INSTRUCAO", request.Grau_Instrucao);
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
                    sql.AppendLine("A.SEXO, A.GRAU_INSTRUCAO, A.ESTADO_CIVIL,C.TIPO_PRIORIDADE,");
                    sql.AppendLine("A.NOMEFANTASIA, A.RAZAO_SOCIAL, B.ENDERECO_BAIRRO,B.ENDERECO_CIDADE, B.ENDERECO_UF, A.STATUS, D.USERNAME, E.DESCRICAO AS TIPO_IMOVEL, C.DESCRICAO AS CATEGORIA ");
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Status_Default>("A.STATUS", "STATUS_ORDENAR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Tipo_Pessoa>("A.TIPOPESSOA", "TIPOPESSOA_ORDENAR"));

                    sql.AppendLine(sqlFrom.ToString());

                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;
                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {
                            case "status":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY STATUS_ORDENAR ");

                                    else
                                        sql.AppendLine(" ORDER BY STATUS_ORDENAR DESC ");
                                    break;
                                }
                            case "tipoPessoa":     // TipoPessoa
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY TIPOPESSOA_ORDENAR");
                                    else
                                        sql.AppendLine(" ORDER BY TIPOPESSOA_ORDENAR DESC");
                                    break;
                                }
                            case "nomeFantasia":     // NomeFantasia
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA) COLLATE PT_BR");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA) COLLATE PT_BR DESC");
                                    break;
                                }
                            case "cpfCnpj":     // CpfCnpj
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.CPF");
                                    else
                                        sql.AppendLine(" ORDER BY A.CPF DESC");
                                    break;
                                }
                            case "categoria":     // Categoria
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(C.DESCRICAO) COLLATE PT_BR");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(C.DESCRICAO) COLLATE PT_BR DESC");
                                    break;
                                }
                            case "login":     // Login
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(D.USERNAME) COLLATE PT_BR");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(D.USERNAME) COLLATE PT_BR DESC");
                                    break;
                                }
                            case "estado":     // Estado
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(B.ENDERECO_UF)");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(B.ENDERECO_UF) DESC");
                                    break;
                                }
                            case "cidade":     // Cidade
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(B.ENDERECO_CIDADE) COLLATE PT_BR");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(B.ENDERECO_CIDADE) COLLATE PT_BR DESC");
                                    break;
                                }
                            case "telefonePrincipal":     // Telefone
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.TELEFONE_FIXO");
                                    else
                                        sql.AppendLine(" ORDER BY A.TELEFONE_FIXO DESC");
                                    break;
                                }
                            case "bairro":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY B.ENDERECO_BAIRRO");
                                    else
                                        sql.AppendLine(" ORDER BY B.ENDERECO_BAIRRO DESC");
                                    break;
                                }
                            case "sexo":     // Telefone
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.SEXO");
                                    else
                                        sql.AppendLine(" ORDER BY A.SEXO DESC");
                                    break;
                                }
                            case "tipo_Prioridade":     // Telefone
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY C.TIPO_PRIORIDADE");
                                    else
                                        sql.AppendLine(" ORDER BY C.TIPO_PRIORIDADE DESC");
                                    break;
                                }
                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
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
                                RazaoSocial = Convert.ToString(reader["RAZAO_SOCIAL"]),
                                Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                Estado = Convert.ToString(reader["ENDERECO_UF"]),
                                Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                Estado_Civil = HtmlEnumExtensions.GetEnumDescription<Estado_Civil>(Convert.ToString(reader["ESTADO_CIVIL"])),
                                Tipo_Prioridade = HtmlEnumExtensions.GetEnumDescription<Tipo_Prioridade>(Convert.ToString(reader["TIPO_PRIORIDADE"])),
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

        [HttpPost()]
        [Route("api/clienteapi/GetAllUnificarCliente")]
        public async Task<DataTableResponse<UnificarClienteListViewModel>> GetAllUnificarCliente(UnificarCliente_DataTableRequest request)
        {
            List<UnificarClienteListViewModel> listagem = new List<UnificarClienteListViewModel>();
            Int32 recordsTotal = 0;
            var retorno2 = new DataTableResponse<UnificarClienteListViewModel>();
            try
            {
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
                                                    .AppendLine("LEFT JOIN TB_ORGANIZACAO ORG ")
                                                    .AppendLine("ON (ORG.ID = A.ID_ORGANIZACAO) ")
                                                    .AppendLine("WHERE A.ID > 0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sqlFrom.AppendLine("AND ((A.CPF LIKE @SEARCH) OR (UPPER(A.NOMEFANTASIA) LIKE @SEARCH))");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                    if (!String.IsNullOrEmpty(request.TipoPessoa))
                    {
                        sqlFrom.AppendLine("AND (A.TIPOPESSOA = @TIPOPESSOA)");
                        parametros.Add("@TIPOPESSOA", Convert.ToInt32(request.TipoPessoa));
                    }

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                    }

                    if (!String.IsNullOrEmpty(request.TipoCadastro))
                    {
                        sqlFrom.AppendLine("AND (A.TIPO_CLIENTE = @TIPO_CLIENTE)");
                        parametros.Add("@TIPO_CLIENTE", Convert.ToInt32(request.TipoCadastro));
                    }

                    if (request.Organizacao.HasValue)
                    {
                        sqlFrom.AppendLine("AND (ORG.ID = @ORGANIZACAO)");
                        parametros.Add("@ORGANIZACAO", Convert.ToInt64(request.Organizacao));

                        sqlFrom.AppendLine("AND A.TIPOPESSOA = 1");
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.CPF, A.TIPOPESSOA, A.TIPO_CLIENTE, A.STATUS,");
                        sql.AppendLine("A.NOMEFANTASIA ");

                        sql.AppendLine(sqlFrom.ToString());

                        ////Ordenacao
                        if (request.order != null && request.order.Any())
                        {
                            int sortColumnIndex = request.order[0].column;
                            string sortDirection = request.order[0].dir;
                            switch (sortColumnIndex)
                            {
                                case 2:
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.STATUS ");

                                        else
                                            sql.AppendLine(" ORDER BY A.STATUS DESC ");
                                        break;
                                    }
                                case 3:     // TipoPessoa
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.TIPOPESSOA");
                                        else
                                            sql.AppendLine(" ORDER BY A.TIPOPESSOA DESC");
                                        break;
                                    }
                                case 4:     // NomeFantasia
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA)");
                                        else
                                            sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA) DESC");
                                        break;
                                    }
                            }
                        }

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader/*First*/(com, sql, parametros, session.Connection, null/*, request.length, request.start*/))
                        {
                            while (await reader.ReadAsync())
                            {
                                listagem.Add(new UnificarClienteListViewModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    CpfCnpj = Convert.ToString(reader["CPF"]),
                                    TipoPessoa = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])),
                                    NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                    TipoCliente = HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(reader["TIPO_CLIENTE"])),
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Cliente>(Convert.ToString(reader["STATUS"])),
                                });
                            }
                        }
                    }
                }
                retorno2.draw = request.draw;
                retorno2.recordsFiltered = recordsTotal;
                retorno2.data = listagem;
                retorno2.error = "";

            }
            catch (Exception ex)
            {
                retorno2.draw = request.draw;
                retorno2.recordsFiltered = 0;
                retorno2.error = ex.Message;
                retorno2.data = new List<UnificarClienteListViewModel>();
            }

            //Page e retornando DataTable
            return retorno2;
        }

        [HttpPost()]
        [Route("api/clienteapi/GetAllPesquisarPlano")]
        public async Task<DataTableResponse<ClientePlanoListViewModel>> GetAllPesquisarPlano(PesquisarClientePlano_DataTableRequest request)
        {
            List<ClientePlanoListViewModel> retorno = new List<ClientePlanoListViewModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_CLIENTE_PLANO_PRODUTO F ")
                                                .AppendLine("LEFT JOIN TB_CLIENTE A ")
                                                .AppendLine("ON (A.ID = F.ID_CLIENTE) ")
                                                .AppendLine("LEFT JOIN TB_PLANO H ")
                                                .AppendLine("ON (H.ID = F.ID_PLANO) ")
                                                .AppendLine("LEFT JOIN TB_CLIENTE_CONTRATO I ")
                                                .AppendLine("ON (I.ID = F.ID_CLIENTE_CONTRATO) ")
                                                .AppendLine("LEFT JOIN TB_LOGIN_RADIUS J ")
                                                .AppendLine("ON (J.ID_PLANO_PRODUTO = F.ID) ")
                                                .AppendLine("LEFT JOIN TB_ENDERECO_CLIENTE K ")
                                                .AppendLine("ON (K.ID = F.ID_ENDERECO_INSTALACAO) ")
                                                .AppendLine("LEFT OUTER JOIN TB_ENDERECO_CLIENTE B ")
                                                .AppendLine("ON(A.ID_ENDERECO_PRINCIPAL = B.ID) ")
                                                .AppendLine("LEFT OUTER JOIN TB_CATEGORIA_CLIENTE C ")
                                                .AppendLine("ON(A.ID_CATEGORIA = C.ID) ")
                                                .AppendLine("LEFT OUTER JOIN AspNetUsers D ")
                                                .AppendLine("ON(A.ID_IDENTITY = D.ID) ")
                                                .AppendLine("LEFT JOIN TB_TIPO_IMOVEL E ")
                                                .AppendLine("ON (B.ID_TIPO_IMOVEL = E.COD_TIPO_IMOVEL) ")
                                                .AppendLine("WHERE A.ID > 0 ");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "login")
                    {
                        sqlFrom.AppendLine("AND (J.LOGIN LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "endereco")
                    {
                        sqlFrom.AppendLine("AND ((UPPER(K.ENDERECO_LOGRADOURO) LIKE @SEARCH) OR ((UPPER(K.ENDERECO_NUMERO) LIKE @SEARCH)) OR (UPPER(K.ENDERECO_CIDADE) LIKE @SEARCH))");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "numero_contrato")
                    {
                        sqlFrom.AppendLine("AND (UPPER(I.ID) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "plano")
                    {
                        sqlFrom.AppendLine("AND (UPPER(H.DESCRICAO) LIKE @SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                }

                if (!String.IsNullOrEmpty(request.Status))
                {
                    sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                    parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                }

                if (!String.IsNullOrEmpty(request.IdCliente))
                {
                    sqlFrom.AppendLine("AND (A.ID = @ID_CLIENTE)");
                    parametros.Add("@ID_CLIENTE", Convert.ToInt64(request.IdCliente));
                }



                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT F.ID, A.CPF, A.TIPOPESSOA, A.TIPO_CLIENTE, F.STATUS,");
                    sql.AppendLine("A.NOMEFANTASIA,H.DESCRICAO AS PLANO,I.ID AS NUMERO_CONTRATO,J.LOGIN,");
                    sql.AppendLine("K.ENDERECO_LOGRADOURO AS ENDERECO, K.ENDERECO_NUMERO AS NUMERO, K.ENDERECO_CIDADE CIDADE");

                    sql.AppendLine(sqlFrom.ToString());

                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 2:
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.STATUS ");

                                    else
                                        sql.AppendLine(" ORDER BY A.STATUS DESC ");
                                    break;
                                }
                            case 3:     // TipoPessoa
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.TIPOPESSOA");
                                    else
                                        sql.AppendLine(" ORDER BY A.TIPOPESSOA DESC");
                                    break;
                                }
                            case 4:     // NomeFantasia
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA)");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA) DESC");
                                    break;
                                }

                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader/*First*/(com, sql, parametros, session.Connection, null/*, request.length, request.start*/))
                    {
                        while (await reader.ReadAsync())
                        {
                            var endereco = reader["ENDERECO"] != DBNull.Value ? string.Concat(Convert.ToString(reader["ENDERECO"]), ";") : string.Empty;
                            endereco += reader["NUMERO"] != DBNull.Value ? string.Concat(Convert.ToString(reader["NUMERO"]), ";") : string.Empty;
                            endereco += reader["CIDADE"] != DBNull.Value ? string.Concat(Convert.ToString(reader["CIDADE"]), ";") : string.Empty;

                            retorno.Add(new ClientePlanoListViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                CpfCnpj = Convert.ToString(reader["CPF"]),
                                TipoPessoa = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])),
                                NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                TipoCliente = HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(reader["TIPO_CLIENTE"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(Convert.ToString(reader["STATUS"])),
                                Plano = Convert.ToString(reader["PLANO"]),
                                NumeroContrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                Login = Convert.ToString(reader["LOGIN"]),
                                Endereco = endereco,
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ClientePlanoListViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/clienteapi/GetComboContratoCliEndereco")]
        public async Task<ObjCombo<long>> GetComboContratoCliEndereco(RequestObjComboAvo<Int64?, Int64?, Boolean?> request)
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

                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
                                                                  LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
                                                                  LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)
                                                                  LEFT JOIN TB_CLIENTE_CONTRATO D ON (D.ID = A.ID_CLIENTE_CONTRATO)
                                                                  LEFT JOIN TB_MODELO_CONTRATO E ON (E.ID = D.ID_MODELO_CONTRATO)
                                                                  LEFT JOIN TB_ENDERECO_CLIENTE F ON(F.ID = A.ID_ENDERECO_INSTALACAO)");

                    if (request.idpai != null)
                    {
                        sqlFrom.AppendLine(" WHERE A.ID_CLIENTE = @idCliente ");
                        parametros.Add("idCliente", request.idpai);
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

                    List<String> numeros_Contrato = new List<String>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder("SELECT DISTINCT D.ID, A.ID_CLIENTE, D.NUMERO_CONTRATO, E.DESCRICAO AS DESCRICAO_MODELCONTR, F.ENDERECO_LOGRADOURO ,F.ENDERECO_NUMERO , F.ENDERECO_BAIRRO, F.ENDERECO_CIDADE ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader["ID"] != DBNull.Value)
                                {
                                    string logradouro = reader["ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_LOGRADOURO"]) : "-";
                                    string numeroLogradouro = reader["ENDERECO_NUMERO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_NUMERO"]) : "-";
                                    string cidade = reader["ENDERECO_CIDADE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_CIDADE"]) : "-";
                                    string bairro = reader["ENDERECO_BAIRRO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_BAIRRO"]) : "-";

                                    string Descricao = Convert.ToString($"{reader["NUMERO_CONTRATO"]} - Rua {logradouro} - {numeroLogradouro} - {bairro} - {cidade}");

                                    var obj = new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Descricao
                                    };

                                    retorno.results.Add(obj);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            retorno.results.GroupBy(x => x.Text).Select(x => x.FirstOrDefault()).ToList();

            return retorno;
        }


        [HttpPost()]
        [Route("api/clienteapi/GetAllListaContato")]
        public async Task<DataTableResponse<ListaContatoListViewModel>> GetAllContatoCliente(ListaContatoCliente_DataTableRequest request)
        {
            List<ListaContatoListViewModel> retorno = new List<ListaContatoListViewModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                var parametros = new Dictionary<string, object>();
                var condicoes = new List<string>();
                if (!string.IsNullOrEmpty(request.TelefoneComparacao))
                {
                    condicoes.Add("A.TELEFONE LIKE (@TELEFONE)");
                    parametros.Add("@TELEFONE", request.TelefoneComparacao.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""));
                }
                if (!string.IsNullOrEmpty(request.EmailComparacao))
                {
                    condicoes.Add("A.EMAIL LIKE (@EMAIL)");
                    parametros.Add("@EMAIL", request.EmailComparacao);
                }
                if (!string.IsNullOrEmpty(request.IdCliente))
                {
                    condicoes.Add($"A.ID_CLIENTE = {request.IdCliente}");
                }
                StringBuilder sqlFrom = new StringBuilder("FROM TB_CLIENTE_CONTATO A ")
                                                .AppendLine($"WHERE {string.Join(" AND ", condicoes)}");

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ID, A.TELEFONE,A.EMAIL, A.NOME_CONTATO, A.INFORMACAO_ADICIONAL, A.TIPO_CONTATO ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader/*First*/(com, sql, parametros, session.Connection, null/*, request.length, request.start*/))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ListaContatoListViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME_CONTATO"]),
                                InformacaoAdicional = Convert.ToString(reader["INFORMACAO_ADICIONAL"]),
                                Tipo = HtmlEnumExtensions.GetEnumDescription<Tipo_Contato_Cliente>(Convert.ToString(reader["TIPO_CONTATO"])),
                                Email = Convert.ToString(reader["EMAIL"]),
                                Telefone = Convert.ToString(reader["TELEFONE"]),
                                TipoValue = Convert.ToString(reader["TIPO_CONTATO"])
                            });
                        }
                    }
                }
            }
            //Page e retornando DataTable
            return new DataTableResponse<ListaContatoListViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/clienteapi/GetAllPesquisa")]
        public async Task<DataTableResponse<ClientePesquisaListViewModel>> GetAllPesquisa(ClientePesquisa_DataTableRequest request)
        {
            List<ClientePesquisaListViewModel> retorno = new List<ClientePesquisaListViewModel>();
            IOrderedEnumerable<ClientePesquisaListViewModel> listaOrdenada = null;
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
                                                .AppendLine("WHERE A.ID > 0");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine("AND ((A.CPF LIKE @SEARCH) OR (UPPER(A.NOMEFANTASIA) LIKE @SEARCH))");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                if (request.Tipo_Cliente != "-1")
                {
                    sqlFrom.AppendLine("AND (A.TIPO_CLIENTE = @TIPO_CLIENTE)");
                    parametros.Add("@TIPO_CLIENTE", Convert.ToInt32(request.Tipo_Cliente));
                }

                if (request.Tipo_Pessoa != "-1")
                {
                    sqlFrom.AppendLine("AND (A.TIPOPESSOA = @TIPOPESSOA)");
                    parametros.Add("@TIPOPESSOA", Convert.ToInt32(request.Tipo_Pessoa));
                }

                if (!String.IsNullOrEmpty(request.Categoria_Cliente))
                {
                    sqlFrom.AppendLine("AND (A.ID_CATEGORIA = @ID_CATEGORIA)");
                    parametros.Add("@ID_CATEGORIA", Convert.ToInt32(request.Categoria_Cliente));
                }

                if (request.Status != "-1")
                {
                    sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                    parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                }

                if (!String.IsNullOrEmpty(request.Estado))
                {
                    sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_UF) = @ESTADO)");
                    parametros.Add("@ESTADO", request.Estado);
                }

                if (!String.IsNullOrEmpty(request.Cidade))
                {
                    sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_CIDADE) = @CIDADE)");
                    parametros.Add("@CIDADE", request.Cidade);
                }

                if (!String.IsNullOrEmpty(request.Bairro))
                {
                    sqlFrom.AppendLine("AND (UPPER(A.ENDERECO_BAIRRO) = @BAIRRO)");
                    parametros.Add("@BAIRRO", request.Bairro);
                }

                if (!String.IsNullOrEmpty(request.Id_Plano_Produto))
                {
                    sqlFrom.AppendLine("AND(0 < (SELECT COUNT(E.ID_CLIENTE) FROM TB_CLIENTE_PLANO_PRODUTO E WHERE E.ID_CLIENTE = A.ID AND (E.ID_PLANO = @ID_PLANO)))");
                    parametros.Add("@ID_PLANO", request.Id_Plano_Produto);
                }



                if (!String.IsNullOrEmpty(request.Estado_Civil))
                {
                    sqlFrom.AppendLine("AND (A.ESTADO_CIVIL = @ESTADO_CIVIL)");
                    parametros.Add("@ESTADO_CIVIL", request.Estado_Civil);
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

                    sql.AppendLine("SELECT A.ID, C.DESCRICAO AS CATEGORIA_CLIENTE, A.CPF, A.TIPOPESSOA, A.TIPO_CLIENTE, A.TELEFONE_FIXO, ");
                    sql.AppendLine("A.NOMEFANTASIA, B.ENDERECO_CIDADE, B.ENDERECO_UF, B.ENDERECO_BAIRRO, A.SEXO, A.ESTADO_CIVIL, A.SEXO, A.GRAU_INSTRUCAO  , A.STATUS, D.USERNAME, E.DESCRICAO AS TIPO_IMOVEL, C.DESCRICAO AS CATEGORIA ");

                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ClientePesquisaListViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Pendencia = false,
                                Categoria = (Convert.ToString(reader["CATEGORIA"])),
                                CpfCnpj = Convert.ToString(reader["CPF"]),
                                TipoPessoa = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])),
                                Tipo_Cliente = HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(reader["TIPO_CLIENTE"])),
                                TelefonePrincipal = Convert.ToString(reader["TELEFONE_FIXO"]),
                                NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                Estado = Convert.ToString(reader["ENDERECO_UF"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Cliente>(Convert.ToString(reader["STATUS"])),
                                Login = Convert.ToString(reader["USERNAME"]),
                                Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                Estado_Civil = HtmlEnumExtensions.GetEnumDescription<Estado_Civil>(Convert.ToString(reader["ESTADO_CIVIL"])),
                                Sexo = HtmlEnumExtensions.GetEnumDescription<Sexo>(Convert.ToString(reader["SEXO"])),
                                Grau_Instrucao = HtmlEnumExtensions.GetEnumDescription<Grau_Instrucao>(Convert.ToString(reader["GRAU_INSTRUCAO"]))
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
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                    case 1:     // TipoPessoa
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Tipo_Cliente);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tipo_Cliente);
                            break;
                        }
                    case 2:     // TipoPessoa
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.TipoPessoa);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.TipoPessoa);
                            break;
                        }
                    case 3:     // NomeFantasia
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.NomeFantasia);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.NomeFantasia);
                            break;
                        }
                    case 4:     // CpfCnpj
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.CpfCnpj);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.CpfCnpj);
                            break;
                        }
                    case 5:     // Categoria
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Categoria);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Categoria);
                            break;
                        }
                    case 6:     // Estado
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Estado);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Estado);
                            break;
                        }
                    case 7:     // Cidade
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cidade);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cidade);
                            break;
                        }
                    case 8:     // Bairro
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Bairro);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Bairro);
                            break;
                        }
                    case 9:     // sexo
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Sexo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Sexo);
                            break;
                        }
                    case 10:     // grau de instrução
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Grau_Instrucao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Grau_Instrucao);
                            break;
                        }
                    case 11:     // estado civil
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Grau_Instrucao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Grau_Instrucao);
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
            retorno.Ind_Inscricao_Estadual = obj.Ind_Inscricao_Estadual;

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

            /*if (obj.Forma_Pagamento != null)
            {
                retorno.Forma_Pagamento = new Forma_Pagamento()
                {
                    Id = obj.Forma_Pagamento.Id,
                    Descricao = obj.Forma_Pagamento?.Descricao,
                };
            }*/

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
            retorno.NumeroValidado = obj.Numero_Validado;

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
                UF = obj.Endereco_Principal?.UF,

                Busca_Endereco = obj.Endereco_Principal?.Busca_Endereco != null ? obj.Endereco_Principal.Busca_Endereco : null,
                Id_Endereco_Logradouro = obj.Endereco_Principal?.Endereco_Logradouro?.Id,
                Id_Condominio_Predio = obj.Endereco_Principal?.Condominio_Predio?.Id,
                Cod_Area = obj.Endereco_Principal?.Cod_Area,
                Cod_Escritorio = obj.Endereco_Principal?.Cod_Escritorio,
                Cod_Localidade = Convert.ToString(obj.Endereco_Principal?.Cod_Localidade),
                Id_Cliente = obj.Id
            };

            if (obj.End_Instalacao_Igual_Principal)
            {
                retorno.EnderecoInstalacao = new EnderecoClienteViewModel
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

                    Tipo = Tipo_EnderecoCliente.Instalacao,
                    UF = obj.Endereco_Principal?.UF,
                    Id_Cliente = obj.Id
                };
            }
            else
            {
                retorno.EnderecoInstalacao = new EnderecoClienteViewModel
                {
                    Id_Pais = obj.Endereco_Instalacao?.Pais?.Id,
                    Id_Tipo_Imovel = obj.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Id_Bairro = obj.Endereco_Instalacao?.Id_Bairro,
                    Bairro = obj.Endereco_Instalacao?.Bairro,
                    CEP = obj.Endereco_Instalacao?.Cep,
                    Id_Cidade = obj.Endereco_Instalacao?.Id_Cidade,
                    Cidade = obj.Endereco_Instalacao?.Cidade,
                    Complemento = obj.Endereco_Instalacao?.Complemento,
                    Id_Endereco = obj.Endereco_Instalacao != null ? (obj.Endereco_Instalacao.Id == 0 ? (Int64)0 : obj.Endereco_Instalacao.Id) : (Int64)0,
                    Latitude = Convert.ToString(obj.Endereco_Instalacao?.Latitude),
                    Logradouro = obj.Endereco_Instalacao?.Logradouro,
                    Longitude = Convert.ToString(obj.Endereco_Instalacao?.Longitude),
                    Numero = obj.Endereco_Instalacao?.Numero,
                    Ponto_Referencia = obj.Endereco_Instalacao?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Instalacao,
                    UF = obj.Endereco_Instalacao?.UF,
                    Id_Cliente = obj.Id
                };
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
                Numero_ContratoVinculado = p?.Numero_ContratoVinculado,
                //Valor_Desconto = Convert.ToString(p.Desconto?.Valor_Desconto),
                //Percentual_Desconto = Convert.ToString(p.Desconto?.Percentual_Desconto),
                Valor_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Valor_Desconto) : p.Desconto.Valores[0].Valor.Value.ToString(),
                Percentual_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Percentual_Desconto) : p.Desconto.Valores[0].Percentual.Value.ToString(),
                Tipo_Desconto = p.Desconto?.Tipo_Desconto?.Descricao,
                Formato = !String.IsNullOrEmpty(Convert.ToString(p.Desconto?.Tipo_Desconto?.Formato)) ? Convert.ToString(Convert.ToInt64(p.Desconto?.Tipo_Desconto?.Formato)) : String.Empty
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

            var planosCancelados = obj.PlanosProdutos.Where(p => p.Status == Status_PlanoProduto.Venda_Cancelada).ToList();
            if (planosCancelados.Count > 0)
            {
                retorno.PlanosCancelados = planosCancelados.Select(p => { return GetViewPlanoProduto(session, p).Result; }).ToList();
            }


            obj.PlanosProdutos = obj.PlanosProdutos.Where(p => p.Status != Status_PlanoProduto.Venda_Cancelada
                                 && p.Status != Status_PlanoProduto.Aguardando_Aprovacao_Venda).ToList();


            //Contagem de Planos Aprovados for igual que 0
            if (obj.PlanosProdutos.Count() == 0)
                retorno.Planos = new List<PlanoProdutoListModel>();

            //Contagem de Planos Aprovados for maior que 0
            else
            {
                using (var repoONU = new Conexao<ONU>(session))
                {
                    var onu = (await repoONU.selectFirst(y => y.Cliente_PlanoProduto.Id == obj.Id));
                    retorno.Planos = obj.PlanosProdutos.Select(async p => await GetViewPlanoProduto(session, p, repoONU)).Select(p => p.Result).ToList();
                }
            }



            using (var repoPlano = new Conexao<Plano>(session))
            {
                int regimeRetorno = 0;

                foreach (Cliente_PlanoProduto planoProduto in obj.PlanosProdutos)
                {
                    if (planoProduto.Plano != null)
                    {
                        Plano plano = await repoPlano.selectFirst(p => p.Regime == Regime_Anatel.Publico && p.Id == planoProduto.Plano.Id);

                        if (plano != null)
                        {
                            regimeRetorno++;
                        }
                    }
                }

                retorno.Regime = regimeRetorno;
            }

            if (obj.Formas_Pagamento_Cliente.Count() == 0)
            {
                retorno.Formas_Pagamento = new List<Forma_Pagamento_ClienteModels>();
            }
            else
            {
                var formasPagamentoCliente = obj.Formas_Pagamento_Cliente.Where(f => f.Status == Status_Default.Ativo).ToList();
                retorno.Formas_Pagamento = formasPagamentoCliente.Select(f => BindFormaPagamentoCliente(f)).ToList();
            }

            //retorno.Forma_Envio = obj.Forma_Envio;

            RequestObjComboPai<Int64?, Int64?> request = new RequestObjComboPai<long?, long?>();
            request.idpai = obj.Id;
            request.soativos = true;
            var listaAgrupada = await new ClienteContratoApiController().GetCombo(request);
            if (listaAgrupada.results.Count > 0)
            {
                listaAgrupada.results.ForEach(x =>
                {
                    retorno.ClienteContratosAtivos.Add(new ClienteContratoListModel
                    {
                        Id = x.Id,
                        Descricao = x.Text,
                        Numero_Contrato = Convert.ToString(x.Id)
                    });
                });
            }

            retorno.ComposicaoNotaClientes = await new PlanoProdutoApiController().ObterConfigNotaFiscalCliente(obj.Id);



            return retorno;
        }

        private Forma_Pagamento_ClienteModels BindFormaPagamentoCliente(Forma_Pagamento_Cliente f)
        {
            return new Forma_Pagamento_ClienteModels
            {
                Id = f.Id,
                Id_Forma_Pagamento = f.Forma_Pagamento?.Id,
                Desc_Forma_Pagamento = f.Forma_Pagamento?.Descricao,
                Forma_Envio = f.Forma_Envio,
                Desc_Forma_Envio = f.Forma_Envio?.GetDisplayName(),
                Id_Vencimento = f.Vencimento?.Id,
                Desc_Banco = !String.IsNullOrEmpty(f?.Banco?.Descricao) ? f?.Banco?.Descricao : "-",
                Id_Banco = f?.Banco?.Id,
                Conta_Corrente = !String.IsNullOrEmpty(f.Conta_Corrente) ? f.Conta_Corrente : "-",
                Agencia = !String.IsNullOrEmpty(f.Agencia) ? f.Agencia : "-",
                Digito = !String.IsNullOrEmpty(f.Digito) ? f.Digito : "-",
                Id_Cliente = f.Cliente?.Id,
                Dia_Vencimento = f.Dia_Vencimento,
                Id_EnderecoCobranca = f.Endereco_Cobranca?.Id,
                Id_Tipo_ImovelCobranca = f.Endereco_Cobranca?.Tipo_Imovel?.Id,
                Id_PaisCobranca = f.Endereco_Cobranca?.Pais?.Id,
                LatitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Latitude),
                LongitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Longitude),
                CEPCobranca = f.Endereco_Cobranca?.Cep,
                LogradouroCobranca = f.Endereco_Cobranca?.Logradouro,
                NumeroCobranca = f.Endereco_Cobranca?.Numero,
                BairroCobranca = f.Endereco_Cobranca?.Bairro,
                Id_BairroCobranca = f.Endereco_Cobranca?.Id_Bairro,
                Id_CidadeCobranca = f.Endereco_Cobranca?.Id_Cidade,
                CidadeCobranca = f.Endereco_Cobranca?.Cidade,
                UFCobranca = f.Endereco_Cobranca?.UF,
                ComplementoCobranca = f.Endereco_Cobranca?.Complemento,
                Ponto_ReferenciaCobranca = f.Endereco_Cobranca?.Ponto_Referencia,
                Status = f.Status

            };
        }

        private async Task<PlanoProdutoListModel> GetViewPlanoProduto(ISession session, Cliente_PlanoProduto planoProduto, Conexao<ONU> repoONU = null)
        {
            bool possuiDesbloqueioConfiancaRecente = false;

            var dataDesbloqueioConfianca = planoProduto.Data_Desbloqueio_Confianca;

            if (dataDesbloqueioConfianca.HasValue)
            {
                int intervaloDesbloqueioConfianca = (DateTime.Now.Date - dataDesbloqueioConfianca.Value.Date).Days;

                //informação Presente no Modelo de Contrato
                //possuiDesbloqueioConfiancaRecente = intervaloDesbloqueioConfianca < (int)planoProduto.Plano.FrequenciaDesbloqueio.Value;
            }

            long idPlanoProduto = 0;
            var descricao = String.Empty;
            PlanoProdutoEnum? tipo = null;

            string necessitaCriarOS = RsBase.NAO;
            string possuiOSCriada = RsBase.NAO;
            var numOS = String.Empty;

            if (planoProduto.Plano != null)
            {
                idPlanoProduto = planoProduto.Plano.Id;
                descricao = planoProduto.Plano.Descricao;
                tipo = PlanoProdutoEnum.Plano;

                if (planoProduto.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                {
                    necessitaCriarOS = RsBase.SIM;
                }
                else
                {
                    necessitaCriarOS = RsBase.NAO;
                }
            }
            else if (planoProduto.Produto != null)
            {
                idPlanoProduto = planoProduto.Produto.Id;
                descricao = planoProduto.Produto.Descricao;
                tipo = PlanoProdutoEnum.Produto;

                if (planoProduto.Produto.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                {
                    necessitaCriarOS = RsBase.SIM;
                }
                else
                {
                    necessitaCriarOS = RsBase.NAO;
                }
            }
            else if (planoProduto.Combo != null)
            {
                idPlanoProduto = planoProduto.Combo.Id;
                descricao = planoProduto.Combo.Descricao;
                tipo = null;

                // TODO: Descobrir como informar se a venda de um combo necessita de OS criada
                necessitaCriarOS = RsBase.SIM;
            }

            // TODO: Descobrir como pegar a OS de uma venda de combo
            if (planoProduto.Plano != null || planoProduto.Produto != null)
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var apiDocProd = new Documento_ProducaoApiController())
                using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoPlano = new Conexao<Plano>(session))
                using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var idClienteAniel = (await apiDocProd.GetClienteAniel(sessionT5, planoProduto.Cliente)).Cod_Cliente;
                    var idPlanoProdutoAniel = (await apiDocProd.GetPlanoProdutoAniel(sessionT5, planoProduto)).FirstOrDefault()?.Id;

                    var planoProdutoOS = repoDocProd.selectFirst(p => p.Id_PlanoProduto == idPlanoProdutoAniel && p.Cliente.Cod_Cliente == idClienteAniel).Result;
                    //var ClientePlano = repoClientePlano.selectFirst(p => p.Cliente ==  ).Result;


                    if (planoProdutoOS != null)
                    {
                        possuiOSCriada = RsBase.SIM;
                    }
                    else
                    {
                        possuiOSCriada = RsBase.NAO;
                    }

                    numOS = planoProdutoOS?.Numero_Obra_Original ?? "";
                }
            }
            else if (planoProduto.Combo != null)
            {
                possuiOSCriada = RsBase.NAO;
            }

            return new PlanoProdutoListModel
            {
                Id = planoProduto.Id,
                Id_Venda = planoProduto.Id_Venda,
                Status_Venda = planoProduto.Id_Venda.HasValue ? (new Conexao<Venda>(session).selectFirst(o => o.Id == planoProduto.Id_Venda))?.Result?.Status : (Status_Venda?)null,
                Id_Plano_Produto = idPlanoProduto,
                Id_Troca_Plano = planoProduto.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null,
                Descricao = descricao,
                ValorPlanoProduto = String.Format("{0:N2}", planoProduto.Valor_Real),
                ValorInstalacao_Final = String.Format("{0:N2}", planoProduto.Valor_Instalacao_Final),
                ValorPlanoProduto_Final = String.Format("{0:N2}", planoProduto.Valor_Final),
                ValorInstalacao = String.Format("{0:N2}", planoProduto.Valor_Instalacao_Real),
                ValorTotal = String.Format("{0:N2}", planoProduto.Valor_Final_Total),
                Qtde_Dias = planoProduto.Dias_Teste_Gratis == null ? 0 : planoProduto.Dias_Teste_Gratis,
                Meses_Carencia = planoProduto.Meses_Carencia,
                Tipo = tipo,

                Status = planoProduto.Status,
                Status_PlanoProduto_Str = planoProduto.Status != null ? HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(planoProduto.Status.Value) : String.Empty,

                Id_Modelo_Contrato = planoProduto.ClienteContrato?.ModeloContrato?.Id,
                Id_Contrato = planoProduto.ClienteContrato?.Id,
                Numero_Contrato = planoProduto.ClienteContrato?.NumeroContrato ?? "-",

                Velocidade_Reduzida = planoProduto.Velocidade_Reduzida,

                Numero_Parcelas = planoProduto.Parcelas_Instalacao == null ? 0 : planoProduto.Parcelas_Instalacao,
                End_Instalacao_Igual_Principal = planoProduto.End_Instalacao_Igual_Principal,
                Momento_Cobra_Instalacao = planoProduto.Momento_Cobra_Instalacao,
                CobrarInstalacaoEntrada = planoProduto.CobrarInstalacaoEntrada,

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

                Cod_Escritorio = planoProduto.Endereco_Instalacao?.Cod_Escritorio,
                Cod_Area = planoProduto.Endereco_Instalacao?.Cod_Area,
                Cod_Localidade = planoProduto.Endereco_Instalacao?.Cod_Localidade,
                Localidade = planoProduto.Endereco_Instalacao?.Localidade,

                Id_Forma_De_Pagamento = planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Id,
                Id_Forma_Pagamento_Cliente = planoProduto.Forma_Pagamento_Cliente?.Id,
                Desc_Forma_Pagto = !String.IsNullOrEmpty(planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao) ? planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao : "Sem Forma de Pagamento",

                ReducaoVelocidade = planoProduto.Plano?.ReducaoVelocidade ?? false,
                //Listagem vai precisar Consultar o modelo de Contrato?
                //DesbloqueioConfianca = planoProduto.Plano?.UtilizaDesbloqueioConfianca ?? false,

                PossuiDesbloqueioConfiancaRecente = possuiDesbloqueioConfiancaRecente,

                NecessitaCriarOS = necessitaCriarOS,
                PossuiOSCriada = possuiOSCriada,
                NumOS = numOS,

            };
        }

        //GetView dos Planos Cancelados
        private async Task<PlanoProdutoListModel> GetViewPlanoProduto(ISession session, Cliente_PlanoProduto planoProduto)
        {
            bool possuiDesbloqueioConfiancaRecente = false;

            var dataDesbloqueioConfianca = planoProduto.Data_Desbloqueio_Confianca;

            if (dataDesbloqueioConfianca.HasValue)
            {
                int intervaloDesbloqueioConfianca = (DateTime.Now.Date - dataDesbloqueioConfianca.Value.Date).Days;

                //informação Presente no Modelo de Contrato
                //possuiDesbloqueioConfiancaRecente = intervaloDesbloqueioConfianca < (int)planoProduto.Plano.FrequenciaDesbloqueio.Value;
            }

            long idPlanoProduto = 0;
            var descricao = String.Empty;
            PlanoProdutoEnum? tipo = null;

            string necessitaCriarOS = RsBase.NAO;
            string possuiOSCriada = RsBase.NAO;
            var numOS = String.Empty;

            if (planoProduto.Plano != null)
            {
                idPlanoProduto = planoProduto.Plano.Id;
                descricao = planoProduto.Plano.Descricao;
                tipo = PlanoProdutoEnum.Plano;

                if (planoProduto.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                {
                    necessitaCriarOS = RsBase.SIM;
                }
                else
                {
                    necessitaCriarOS = RsBase.NAO;
                }
            }
            else if (planoProduto.Produto != null)
            {
                idPlanoProduto = planoProduto.Produto.Id;
                descricao = planoProduto.Produto.Descricao;
                tipo = PlanoProdutoEnum.Produto;

                if (planoProduto.Produto.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                {
                    necessitaCriarOS = RsBase.SIM;
                }
                else
                {
                    necessitaCriarOS = RsBase.NAO;
                }
            }
            else if (planoProduto.Combo != null)
            {
                idPlanoProduto = planoProduto.Combo.Id;
                descricao = planoProduto.Combo.Descricao;
                tipo = null;

                // TODO: Descobrir como informar se a venda de um combo necessita de OS criada
                necessitaCriarOS = RsBase.SIM;
            }

            // TODO: Descobrir como pegar a OS de uma venda de combo
            if (planoProduto.Plano != null || planoProduto.Produto != null)
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var apiDocProd = new Documento_ProducaoApiController())
                using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoPlano = new Conexao<Plano>(session))
                using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var idClienteAniel = (await apiDocProd.GetClienteAniel(sessionT5, planoProduto.Cliente)).Cod_Cliente;
                    var idPlanoProdutoAniel = (await apiDocProd.GetPlanoProdutoAniel(sessionT5, planoProduto)).FirstOrDefault()?.Id;

                    var planoProdutoOS = repoDocProd.selectFirst(p => p.Id_PlanoProduto == idPlanoProdutoAniel && p.Cliente.Cod_Cliente == idClienteAniel).Result;
                    //var ClientePlano = repoClientePlano.selectFirst(p => p.Cliente ==  ).Result;


                    if (planoProdutoOS != null)
                    {
                        possuiOSCriada = RsBase.SIM;
                    }
                    else
                    {
                        possuiOSCriada = RsBase.NAO;
                    }

                    numOS = planoProdutoOS?.Numero_Obra_Original ?? "";
                }
            }
            else if (planoProduto.Combo != null)
            {
                possuiOSCriada = RsBase.NAO;
            }

            return new PlanoProdutoListModel
            {
                Id = planoProduto.Id,
                Id_Venda = planoProduto.Id_Venda,
                Status_Venda = planoProduto.Id_Venda.HasValue ? (new Conexao<Venda>(session).selectFirst(o => o.Id == planoProduto.Id_Venda))?.Result?.Status : (Status_Venda?)null,
                Id_Plano_Produto = idPlanoProduto,
                Id_Troca_Plano = planoProduto.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null,
                Descricao = descricao,
                ValorPlanoProduto = String.Format("{0:N2}", planoProduto.Valor_Real),
                ValorInstalacao_Final = String.Format("{0:N2}", planoProduto.Valor_Instalacao_Final),
                ValorPlanoProduto_Final = String.Format("{0:N2}", planoProduto.Valor_Final),
                ValorInstalacao = String.Format("{0:N2}", planoProduto.Valor_Instalacao_Real),
                ValorTotal = String.Format("{0:N2}", planoProduto.Valor_Final_Total),
                Qtde_Dias = planoProduto.Dias_Teste_Gratis == null ? 0 : planoProduto.Dias_Teste_Gratis,
                Meses_Carencia = planoProduto.Meses_Carencia,
                Tipo = tipo,
                Status = planoProduto.Status,
                Status_PlanoProduto_Str = planoProduto.Status != null ? HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(planoProduto.Status.Value) : String.Empty,

                Id_Modelo_Contrato = planoProduto.ClienteContrato?.ModeloContrato?.Id,
                Id_Contrato = planoProduto.ClienteContrato?.Id,
                //
                Numero_Contrato = planoProduto.ClienteContrato?.NumeroContrato ?? "-",
                //
                Velocidade_Reduzida = planoProduto.Velocidade_Reduzida,

                Numero_Parcelas = planoProduto.Parcelas_Instalacao == null ? 0 : planoProduto.Parcelas_Instalacao,
                End_Instalacao_Igual_Principal = planoProduto.End_Instalacao_Igual_Principal,

                Teste_Gratis = planoProduto.Dias_Teste_Gratis > 0,
                Periodo_Carencia = planoProduto.Periodo_Carencia,

                Nas = planoProduto.Nas,
                Login_Radius = planoProduto.Login_Radius,
                Senha_Radius = planoProduto.Senha_Radius,

                //Id_Onu = (await repoONU.selectFirst(y => y.Cliente_PlanoProduto.Id == planoProduto.Id))?.Id,

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
                Desc_Forma_Pagto = !String.IsNullOrEmpty(planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao) ? planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao : "Sem Forma de Pagamento",

                ReducaoVelocidade = planoProduto.Plano?.ReducaoVelocidade ?? false,
                //Listagem vai precisar Consultar o modelo de Contrato?
                //DesbloqueioConfianca = planoProduto.Plano?.UtilizaDesbloqueioConfianca ?? false,

                PossuiDesbloqueioConfiancaRecente = possuiDesbloqueioConfiancaRecente,

                NecessitaCriarOS = necessitaCriarOS,
                PossuiOSCriada = possuiOSCriada,
                NumOS = numOS,
                CobrarInstalacaoEntrada = planoProduto.CobrarInstalacaoEntrada,
            };
        }


        private async Task<PlanoProdutoListModel> GetViewObjClientePlanoProduto(Cliente_PlanoProduto obj, ISession session)
        {
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoONU = new Conexao<ONU>(session))
            using (var repoLogin = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (var repoModelo_Contrato = new Conexao<Model.nhibernate.Modelo_Contrato>(session))
            using (var apiLocalidade = new LocalidadeApiController())
            using (var apiPlano = new PlanoApiController())
            using (var repoCaixa_Porta = new Conexao<Elemento_Rede_Porta>(sessionT5))
            {
                var _strings_localidade = obj.Cod_Localidade.HasValue ?
                                          apiLocalidade.GetListFullLocaliade(obj.Cod_Escritorio, obj.Cod_Area, obj.Cod_Localidade.Value).Result :
                                          null;

                var login = await repoLogin.selectFirst(p => p.Cliente_PlanoProduto.Id == obj.Id);
                var onu = await repoONU.selectFirst(p => p.Cliente_PlanoProduto.Id == obj.Id);
                var caixa_porta = await repoCaixa_Porta.selectFirst(p => p.Cliente_Plano_Prod == obj.Id);


                PlanoProdutoListModel retorno = new PlanoProdutoListModel();

                retorno.Id = obj.Id;

                if (obj.Plano != null)
                {
                    retorno.Id_Plano_Produto = obj.Plano.Id;
                    retorno.Descricao = obj.Plano.Descricao;
                    retorno.Tipo = PlanoProdutoEnum.Plano;

                    retorno.Regime = obj.Plano.Regime == null ? Regime_Anatel.Publico : Regime_Anatel.PublicoPrivado;
                }
                else if (obj.Produto != null)
                {
                    retorno.Id_Plano_Produto = obj.Produto.Id;
                    retorno.Descricao = obj.Produto.Descricao;
                    retorno.Tipo = PlanoProdutoEnum.Produto;
                }
                else if (obj.Combo != null)
                {
                    retorno.Id_Plano_Produto = obj.Combo.Id;
                    retorno.Descricao = obj.Combo.Descricao;
                    retorno.Tipo = null;
                }

                retorno.Id_Troca_Plano = obj.Id_TrocaPlano != null ? obj.Id_TrocaPlano : (long?)null;
                retorno.ValorPlanoProduto = String.Format("{0:N2}", obj.Valor_Real.HasValue ? obj.Valor_Real.Value : 0);
                retorno.ValorInstalacao_Final = String.Format("{0:N2}", obj.Valor_Instalacao_Final);
                retorno.ValorPlanoProduto_Final = String.Format("{0:N2}", obj.Valor_Final);
                retorno.ValorInstalacao = String.Format("{0:N2}", obj.Valor_Instalacao_Real);
                retorno.ValorTotal = String.Format("{0:N2}", obj.Valor_Final_Total);
                retorno.Qtde_Dias = obj.Dias_Teste_Gratis == null ? 0 : obj.Dias_Teste_Gratis;
                retorno.Meses_Carencia = obj.Meses_Carencia;

                #region [Regra para multa de rescisão]

                var obj_modelo_contrato = await repoModelo_Contrato.selectFirst(x => x.Id == obj.Plano.Modelo_Contrato.Id);
                Double periodo_fidelidade = obj_modelo_contrato.Tem_Carencia ? Convert.ToDouble(obj_modelo_contrato.Periodo_Carencia) : 0;
                Double multa_fidelidade = obj_modelo_contrato.MultaFidelidade ? Convert.ToDouble(obj_modelo_contrato.Multa_Carencia) : 0;

                DateTime dataFimCarencia = obj.Data_Habilitado != null ? Convert.ToDateTime(obj.Data_Habilitado) : new DateTime();
                int? periodoCarencia = obj?.ClienteContrato?.PeriodoCarencia > 0 ? Convert.ToInt32(obj?.ClienteContrato?.PeriodoCarencia) : (int?)null;
                DateTime fimCarencia = periodoCarencia.HasValue ? dataFimCarencia.AddMonths(periodoCarencia.Value) : dataFimCarencia;

                int anos = fimCarencia.Year - DateTime.Now.Year;
                int meses = fimCarencia.Month - DateTime.Now.Month;

                int totalMeses = anos * 12 + meses;

                // Se o dia final for menor que o dia inicial, desconta um mês
                if (fimCarencia.Day < DateTime.Now.Day)
                {
                    totalMeses--;
                }

                var tempo_restante = totalMeses;

                String multa_pro_rata = obj_modelo_contrato.Tem_Pro_Rata ? String.Format("{0:N2}", Math.Round((multa_fidelidade / periodo_fidelidade) * tempo_restante)) : String.Format("{0:N2}", multa_fidelidade);

                retorno.Multa_Carencia_Reais = multa_pro_rata;
                //retorno.Multa_Carencia_Reais = String.Format("{0:N2}", obj.Multa_Carencia_Reais.HasValue ? obj.Multa_Carencia_Reais.Value : 0);

                #endregion

                retorno.Frequencia_Reajuste = obj.Frequencia_Reajuste.HasValue ? HtmlEnumExtensions.GetEnumDescription(obj.Frequencia_Reajuste) : RsBase.NAO;
                retorno.CobrarInstalacaoEntrada = obj?.CobrarInstalacaoEntrada ?? false;
                retorno.ValorInstalacaoEntrada = obj.CobrarInstalacaoEntrada ? String.Format("{0:N2}", obj.ValorInstalacaoEntrada) : null;

                if (obj.Plano != null)
                {
                    try
                    {
                        retorno.Momento_Cobra_Instalacao_Str = HtmlEnumExtensions.GetEnumDescription(obj.Plano.Momento_Cobra_Instalacao);
                    }
                    catch
                    {
                        retorno.Momento_Cobra_Instalacao_Str = string.Empty;
                    }
                }
                retorno.Renovar_CarenciaAuto = obj.Frequencia_Reajuste.HasValue ? HtmlEnumExtensions.GetEnumDescription(obj.Renovar_CarenciaAuto) : RsBase.NAO;
                retorno.CancelaContratoFimCarencia = obj.CancelaContratoFimCarencia ? RsBase.SIM : RsBase.NAO;
                retorno.Tem_Reajuste = obj.Tem_Reajuste ? RsBase.SIM : RsBase.NAO;
                retorno.Status = obj.Status;

                retorno.Id_Modelo_Contrato = obj.ClienteContrato?.ModeloContrato?.Id;
                retorno.Id_Contrato = obj.ClienteContrato?.Id;
                retorno.Numero_Contrato = obj.ClienteContrato?.NumeroContrato;

                #region campos adicionais  do plano

                retorno.ImpedirBloqueio_Automatico = obj.ImpedirBloqueio_Automatico;
                retorno.ImpedirBloqueio_Sexta = obj.ImpedirBloqueio_Sexta;
                retorno.ImpedirBloqueio_Sabado = obj.ImpedirBloqueio_Sabado;
                retorno.ImpedirBloqueio_Domingo = obj.ImpedirBloqueio_Domingo;
                retorno.ImpedirBloqueio_Feriados = obj.ImpedirBloqueio_Feriados;
                retorno.BloqueioAutomatico_Debito = obj.UtilizaBloqueioDebito;
                retorno.QtdDiasAtraso = obj.QtdDiasAtraso;

                retorno.QtdDiasbloqueados = obj.QtdDiasbloqueados;
                retorno.Velocidade_UploadReduzida = obj.VelocidadeUploadReduzida;
                retorno.Velocidade_DownloadReduzida = obj.VelocidadeDownloadReduzida;
                retorno.Reducao_Velocidade = obj.ReducaoVelocidade;

                retorno.Frequencia_Desbloqueio = obj.FrequenciaDesbloqueio;
                //retorno.Localidade_Anatel = obj.Localidade_Anatel;
                retorno.dias_desbloq = obj.QtdDiasdesbloqueados;
                retorno.Desbloqueio_Conf = obj.UtilizaDesbloqueioConfianca;
                #endregion

                retorno.Numero_Parcelas = obj.Parcelas_Instalacao == null ? 0 : obj.Parcelas_Instalacao;
                retorno.End_Instalacao_Igual_Principal = obj.End_Instalacao_Igual_Principal;

                retorno.Teste_Gratis = obj.Dias_Teste_Gratis > 0;
                retorno.Periodo_Carencia = obj.Periodo_Carencia;

                retorno.Quantidade = obj.Quantidade_Produto_Variavel;
                retorno.Valor_Total_Plano = obj.Valor_Total_Plano_Variavel.HasValue ? String.Format("{0:N2}", obj.Valor_Total_Plano_Variavel.Value) : String.Empty;
                retorno.Parcela = Convert.ToInt32(obj.Parcela_Entrada);
                retorno.Valor_Entrada = obj.Valor_Entrada.HasValue ? String.Format("{0:N2}", obj.Valor_Entrada.Value) : String.Empty;

                retorno.Nas = obj.Nas;
                retorno.Login_Radius = login?.Login;
                retorno.Senha_Radius = login?.SenhaPPPOE;
                retorno.ONU = onu?.Descricao ?? String.Empty;
                retorno.Id_Onu = onu?.Id;
                retorno.PON = onu?.PON?.Descricao ?? String.Empty;
                retorno.Slot = onu?.Slot?.Descricao ?? String.Empty;
                retorno.Olt = onu?.OLT?.Descricao ?? String.Empty;
                retorno.IdIpPool = login?.Cliente_PlanoProduto?.Pool_Ip?.Id;

                retorno.Id_Endereco = obj.Endereco_Instalacao.Id == 0 ? (Int64)0 : obj.Endereco_Instalacao.Id;
                retorno.Id_Bairro = obj.Endereco_Instalacao?.Id_Bairro;
                retorno.Bairro = obj.Endereco_Instalacao?.Bairro;
                retorno.CEP = obj.Endereco_Instalacao?.Cep;
                retorno.Id_Cidade = obj.Endereco_Instalacao?.Id_Cidade;
                retorno.Cidade = obj.Endereco_Instalacao?.Cidade;
                retorno.Ponto_Referencia = obj.Endereco_Instalacao?.Ponto_Referencia;
                retorno.Complemento = obj.Endereco_Instalacao?.Complemento;
                retorno.Logradouro = obj.Endereco_Instalacao?.Logradouro;
                retorno.Numero = obj.Endereco_Instalacao?.Numero;
                retorno.UF = obj.Endereco_Instalacao?.UF;

                retorno.Latitude = Convert.ToString(obj.Endereco_Instalacao?.Latitude);
                retorno.Longitude = Convert.ToString(obj.Endereco_Instalacao?.Longitude);
                retorno.Id_Pais = obj.Endereco_Instalacao?.Pais?.Id;
                retorno.Id_Tipo_Imovel = obj.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel;

                retorno.Cod_Escritorio = obj.Cod_Escritorio;
                retorno.Cod_Area = obj.Cod_Area;
                retorno.Cod_Localidade = obj.Cod_Localidade;

                retorno.Desc_Escritorio = _strings_localidade?.Objeto?.Grupo_Area;
                retorno.Desc_Area = _strings_localidade?.Objeto?.Area;
                retorno.Localidade = _strings_localidade?.Objeto?.Localidade;

                retorno.Caixa_Ftth_Reservado = Convert.ToString(caixa_porta?.Caixa?.Id);
                retorno.Porta_Ftth_Reservado = Convert.ToString(caixa_porta?.Porta);
                retorno.Data_Expiracao_Reserva = caixa_porta?.Data_Expiracao_Reserva?.ToString("dd/MM/yyyy HH:mm:ss");

                if (obj.Plano?.Id != 0 && obj.Plano?.Id != null)
                {
                    using (var repoPlano = new Conexao<Plano>(session))
                    {
                        var planoobj = repoPlano.selectFirst(p => p.Id == obj.Plano.Id).Result;
                        retorno.VelocidadeDownload = planoobj.VelocidadeDownload;
                        retorno.VelocidadeDownloadReduzida = planoobj.VelocidadeDownloadReduzida;
                        retorno.ValorPlanoOriginal = String.Format("{0:N2}", planoobj.ValorPlano);
                        retorno.VelocidadeNominal = planoobj.VelocidadeNominal;
                        retorno.VelocidadeUpload = planoobj.VelocidadeUpload;
                        retorno.Id_Tecnologia = planoobj.Tecnologia?.Id;
                        retorno.VelocidadeUploadReduzida = planoobj.VelocidadeUploadReduzida;
                        retorno.AutenticacaoRadius = planoobj.AutenticacaoRadius ? Resources.RsCadastros.SIM : Resources.RsCadastros.NAO;
                        retorno.Nas = String.Concat(planoobj.Servidor_NAS?.Tecnologia, '-', planoobj.Servidor_NAS?.Ip);
                        retorno.Produto_Plano = "Plano";

                        retorno.Modelo_Anatel = planoobj.Modelo_Anatel;
                    }
                }
                else if (obj.Produto?.Id != 0 && obj.Produto?.Id != null)
                {
                    using (var repoProduto = new Conexao<Produto>(session))
                    {
                        var produtoObj = repoProduto.selectFirst(p => p.Id == obj.Produto.Id).Result;

                        retorno.DataValidade = produtoObj.Data_Validade.HasValue ? produtoObj.Data_Validade.Value.ToString("dd/MM/yyyy") : String.Empty;
                        retorno.BloqueioAutomatico = Resources.RsCadastros.NAO;
                        retorno.Produto_Plano = "Produto";
                    }
                }

                return retorno;
            }
        }

        private List<PostApiMensagem> BindViewToObj(ISession session, Cliente objBanco, ClienteDadosGeraisViewModel view, ref List<Cliente_PlanoProduto_StatusLog> logs)
        {
            var retorno = new List<PostApiMensagem>();

            using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repoBanco = new Conexao<Banco>(session))
            using (var repoCategoriaCliente = new Conexao<Categoria_Cliente>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoFormaPagamento = new Conexao<Forma_Pagamento>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoTipoImovel = new Conexao<Tipo_Imovel>(session))
            using (var repoModeloContrato = new Conexao<Modelo_Contrato>(session))
            using (var repoPlano = new Conexao<Plano>(session))
            using (var repoProduto = new Conexao<Produto>(session))
            using (var repoMeioCaptacao = new Conexao<Meio_Captacao>(session))
            using (var repoVenvimento = new Conexao<Vencimento>(session))
            using (var repoOrganizacao = new Conexao<Organizacao>(session))
            using (var repoFormaPagamentoCliente = new Conexao<Forma_Pagamento_Cliente>(session))
            using (var repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(session))
            using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoCondominioPredio = new Conexao<Condominio_Predio>(session))
            using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
            using (var repoNfCompCliente = new Conexao<ComposicaoNotaFiscalCliente>(session))
            {
                Configuracoes_Gerais config = repoConfigGerais.selectFirst().Result;

                objBanco.Numero_Contrato = config.Identificacao_Cliente.Value == Identificacao_Cliente.Numero_Contrato ? view.Identificacao_Cliente :
                                          (view.Id == 0 || String.IsNullOrEmpty(objBanco.Numero_Contrato) ? Convert.ToString(repoCliente.MaxString("NUMERO_CONTRATO", "TB_CLIENTE").Result + 1) : view.Identificacao_Cliente);

                objBanco.Tipo_Pessoa = view.TipoPessoa;
                objBanco.Cargo_Responsavel = view.Cargo_Responsavel;
                objBanco.Cpf_Responsavel = !String.IsNullOrEmpty(view.CPFResponsavel) ? new String(view.CPFResponsavel.Where(Char.IsDigit).ToArray()) : view.CPFResponsavel;
                objBanco.Nome_Resp_Financeiro = view.Representante;
                objBanco.Inscricao_Municipal = view.Inscricao_Municipal;
                objBanco.Inscricao_Estadual = view.Inscricao_Estadual;
                objBanco.Razao_Social = view.Razao_Social;
                objBanco.Ind_Inscricao_Estadual = view.Ind_Inscricao_Estadual;
                objBanco.Ramo_Atividade = view.Ramo_Atividade;
                objBanco.Porte = view.Porte;
                objBanco.Data_Abertura_Empresa = view.Data_Abertura;

                objBanco.NomeFantasia = objBanco.Tipo_Pessoa == Tipo_Pessoa.Fisica ? view.NomeCompleto : view.NomeFantasia;
                objBanco.CpfCnpj = !String.IsNullOrEmpty(view.CpfCnpj) ? new String(view.CpfCnpj.Where(Char.IsDigit).ToArray()) : String.Empty;
                objBanco.Data_Nascimento = !String.IsNullOrEmpty(view.DataNascimento) ? Convert.ToDateTime(view.DataNascimento) : (DateTime?)null;
                objBanco.Nacionalidade = view.Nacionalidade;
                objBanco.Sexo = view.Sexo;
                objBanco.Ind_Inscricao_Estadual = view.Ind_Inscricao_Estadual;

                objBanco.Organizacao = view.Id_Organizacao.HasValue ? repoOrganizacao.selectFirst(p => p.Id == view.Id_Organizacao.Value).Result : null;

                objBanco.Estado_Civil = view.EstadoCivil;
                objBanco.Rg = view.RG;
                objBanco.Data_Emissao_Rg = view.DataEmissao != null ? Convert.ToDateTime(view.DataEmissao) : (DateTime?)null;
                objBanco.Orgao_Emissao_Rg = view.Orgao;
                objBanco.Grau_Instrucao = view.Instrucao;
                objBanco.Filiacao_Mae = view.Mae;
                objBanco.Filiacao_Pai = view.Pai;
                objBanco.Profissao = view.Profissao;

                objBanco.Email_Principal = view.Email;
                objBanco.Telefone_Fixo = view.TelefoneFixo;
                objBanco.Telefone_Celular = view.Telefone_Celular;
                objBanco.Telefone_Recado = view.TelefoneRecado;
                objBanco.Pessoa_Tel_Recado = view.NomeRecado;

                objBanco.Categoria_Cliente = view.Id_Categoria_Cliente.HasValue ? repoCategoriaCliente.selectFirst(p => p.Id == view.Id_Categoria_Cliente.Value).Result : null;
                objBanco.Quem_Indicou = view.Id_Quem_Indicou.HasValue ? repoCliente.selectFirst(p => p.Id == view.Id_Quem_Indicou.Value).Result : null;

                // ****************************************
                // Endereço Principal
                // ****************************************
                if (objBanco.Id == 0)
                    objBanco.Endereco_Principal.Id = 0;

                if (objBanco.Endereco_Principal == null)
                    objBanco.Endereco_Principal = new Endereco_Cliente();

                objBanco.Endereco_Principal.Pais = view.EnderecoPrincipal.Id_Pais != null ? repoPais.selectFirst(p => p.Id == view.EnderecoPrincipal.Id_Pais.Value).Result : null;
                objBanco.Endereco_Principal.Tipo_Imovel = view.EnderecoPrincipal.Id_Tipo_Imovel != null ? repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == view.EnderecoPrincipal.Id_Tipo_Imovel.Value).Result : null;

                objBanco.Endereco_Principal.Bairro = view.EnderecoPrincipal?.Bairro;
                objBanco.Endereco_Principal.Id_Bairro = view.EnderecoPrincipal?.Id_Bairro;
                objBanco.Endereco_Principal.Cep = view.EnderecoPrincipal?.CEP;
                objBanco.Endereco_Principal.Cidade = view.EnderecoPrincipal?.Cidade;
                objBanco.Endereco_Principal.Id_Cidade = view.EnderecoPrincipal?.Id_Cidade;
                objBanco.Endereco_Principal.Complemento = view.EnderecoPrincipal?.Complemento;
                objBanco.Endereco_Principal.Latitude = view.EnderecoPrincipal?.Latitude != null ? Convert.ToDouble(view.EnderecoPrincipal?.Latitude?.Replace('.', ',')) : (Double?)null;
                objBanco.Endereco_Principal.Longitude = view.EnderecoPrincipal?.Longitude != null ? Convert.ToDouble(view.EnderecoPrincipal?.Longitude?.Replace('.', ',')) : (Double?)null;
                objBanco.Endereco_Principal.Logradouro = view.EnderecoPrincipal?.Logradouro;
                objBanco.Endereco_Principal.Numero = view.EnderecoPrincipal?.Numero;
                objBanco.Endereco_Principal.Ponto_Referencia = view.EnderecoPrincipal?.Ponto_Referencia;
                objBanco.Endereco_Principal.UF = view.EnderecoPrincipal?.UF;
                objBanco.Endereco_Principal.Cod_Escritorio = view.EnderecoPrincipal?.Cod_Escritorio;
                objBanco.Endereco_Principal.Cod_Localidade = Convert.ToInt32(view.EnderecoPrincipal?.Cod_Localidade);
                objBanco.Endereco_Principal.Cod_Area = view.EnderecoPrincipal?.Cod_Area;


                objBanco.Endereco_Principal.Busca_Endereco = view.EnderecoPrincipal.Busca_Endereco != null ? view.EnderecoPrincipal.Busca_Endereco : null;
                objBanco.Endereco_Principal.Condominio_Predio = view.EnderecoPrincipal.Busca_Endereco == Busca_Endereco.Condominio_Predio ? repoCondominioPredio.selectFirst(p => p.Id == view.EnderecoPrincipal.Id_Condominio_Predio).Result : null;
                objBanco.Endereco_Principal.Endereco_Logradouro = view.EnderecoPrincipal.Busca_Endereco == Busca_Endereco.Endereco_Logradouro ? repoEnderecoLogradouro.selectFirst(p => p.Id == view.EnderecoPrincipal.Id_Endereco_Logradouro).Result : null;

                List<Forma_Pagamento_Cliente> forma_pagamento_clientetemp = objBanco.Formas_Pagamento_Cliente.ToList();

                foreach (var forma_pagamento_clienteview in view.Formas_Pagamento.FindAll(p => p.Id == 0))
                {
                    // ****************************************
                    // Endereço Cobranca
                    // ****************************************
                    Endereco_Cliente enderecoCobranca = new Endereco_Cliente();
                    if (forma_pagamento_clienteview.End_Cobranca_Igual_Principal)
                    {
                        enderecoCobranca.Pais = objBanco.Endereco_Principal?.Pais;
                        enderecoCobranca.Tipo_Imovel = objBanco.Endereco_Principal?.Tipo_Imovel;
                        enderecoCobranca.Id_Bairro = objBanco.Endereco_Principal?.Id_Bairro;
                        enderecoCobranca.Id_Cidade = objBanco.Endereco_Principal?.Id_Cidade;
                        enderecoCobranca.Bairro = objBanco.Endereco_Principal?.Bairro;
                        enderecoCobranca.Cep = objBanco.Endereco_Principal?.Cep;
                        enderecoCobranca.Cidade = objBanco.Endereco_Principal?.Cidade;
                        enderecoCobranca.Complemento = objBanco.Endereco_Principal?.Complemento;
                        enderecoCobranca.Latitude = objBanco.Endereco_Principal?.Latitude;
                        enderecoCobranca.Longitude = objBanco.Endereco_Principal?.Longitude;
                        enderecoCobranca.Logradouro = objBanco.Endereco_Principal?.Logradouro;
                        enderecoCobranca.Numero = objBanco.Endereco_Principal?.Numero;
                        enderecoCobranca.Ponto_Referencia = objBanco.Endereco_Principal?.Ponto_Referencia;
                        enderecoCobranca.UF = objBanco.Endereco_Principal?.UF;
                        enderecoCobranca.Busca_Endereco = view.EnderecoPrincipal.Busca_Endereco != null ? view.EnderecoPrincipal.Busca_Endereco : null;
                        enderecoCobranca.Endereco_Logradouro = objBanco.Endereco_Principal?.Endereco_Logradouro;
                        enderecoCobranca.Condominio_Predio = objBanco.Endereco_Principal?.Condominio_Predio;
                        enderecoCobranca.Cod_Escritorio = view.EnderecoPrincipal?.Cod_Escritorio;
                        enderecoCobranca.Cod_Localidade = Convert.ToInt32(view.EnderecoPrincipal?.Cod_Localidade);
                        enderecoCobranca.Cod_Area = view.EnderecoPrincipal?.Cod_Area;
                    }
                    else
                    {
                        enderecoCobranca.Pais = forma_pagamento_clienteview.Id_PaisCobranca != null ? repoPais.selectFirst(p => p.Id == forma_pagamento_clienteview.Id_PaisCobranca.Value).Result : null;
                        enderecoCobranca.Tipo_Imovel = forma_pagamento_clienteview.Id_Tipo_ImovelCobranca != null ? repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == forma_pagamento_clienteview.Id_Tipo_ImovelCobranca.Value).Result : null;
                        enderecoCobranca.Id_Bairro = forma_pagamento_clienteview?.Id_BairroCobranca;
                        enderecoCobranca.Id_Cidade = forma_pagamento_clienteview?.Id_CidadeCobranca;
                        enderecoCobranca.Bairro = forma_pagamento_clienteview?.BairroCobranca;
                        enderecoCobranca.Cep = forma_pagamento_clienteview?.CEPCobranca;
                        enderecoCobranca.Cidade = forma_pagamento_clienteview?.CidadeCobranca;
                        enderecoCobranca.Complemento = forma_pagamento_clienteview?.ComplementoCobranca;
                        enderecoCobranca.Latitude = forma_pagamento_clienteview?.LatitudeCobranca != null ? Convert.ToDouble(forma_pagamento_clienteview?.LatitudeCobranca?.Replace('.', ',')) : (Double?)null;
                        enderecoCobranca.Longitude = forma_pagamento_clienteview?.LongitudeCobranca != null ? Convert.ToDouble(forma_pagamento_clienteview?.LongitudeCobranca?.Replace('.', ',')) : (Double?)null;
                        enderecoCobranca.Logradouro = forma_pagamento_clienteview?.LogradouroCobranca;
                        enderecoCobranca.Numero = forma_pagamento_clienteview?.NumeroCobranca;
                        enderecoCobranca.Ponto_Referencia = forma_pagamento_clienteview?.Ponto_ReferenciaCobranca;
                        enderecoCobranca.UF = forma_pagamento_clienteview?.UFCobranca;
                        enderecoCobranca.Cod_Escritorio = view.EnderecoPrincipal?.Cod_Escritorio;
                        enderecoCobranca.Cod_Localidade = Convert.ToInt32(view.EnderecoPrincipal?.Cod_Localidade);
                        enderecoCobranca.Cod_Area = view.EnderecoPrincipal?.Cod_Area;
                    }

                    // ************************************************
                    // Formas de Pagamento do Cliente
                    // ************************************************
                    var forma_pagamento_cliente = new Forma_Pagamento_Cliente
                    {
                        Cliente = objBanco,
                        Agencia = forma_pagamento_clienteview.Agencia,
                        Banco = forma_pagamento_clienteview.Id_Banco.HasValue ? repoBanco.selectFirst(p => p.Id == forma_pagamento_clienteview.Id_Banco.Value).Result : null,
                        Conta_Corrente = forma_pagamento_clienteview.Conta_Corrente,
                        Dia_Vencimento = forma_pagamento_clienteview.Dia_Vencimento,
                        Digito = forma_pagamento_clienteview.Digito,
                        Forma_Envio = forma_pagamento_clienteview.Forma_Envio,
                        Forma_Pagamento = forma_pagamento_clienteview.Id_Forma_Pagamento.HasValue ? repoFormaPagamento.selectFirst(p => p.Id == forma_pagamento_clienteview.Id_Forma_Pagamento.Value).Result : null,
                        Vencimento = forma_pagamento_clienteview.Id_Vencimento.HasValue ? repoVenvimento.selectFirst(p => p.Id == forma_pagamento_clienteview.Id_Vencimento.Value).Result : null,
                        Endereco_Cobranca = enderecoCobranca,
                        End_Cobranca_Igual_Principal = forma_pagamento_clienteview.End_Cobranca_Igual_Principal,
                        Status = forma_pagamento_clienteview.Status.HasValue ? forma_pagamento_clienteview.Status.Value : Status_Default.Ativo
                    };

                    forma_pagamento_clientetemp.Add(forma_pagamento_cliente);
                }

                objBanco.Formas_Pagamento_Cliente = forma_pagamento_clientetemp;

                #region Plano_Produto

                // ****************************************
                // Planos e Produtos + Endereço Instalacao
                // ****************************************

                List<Cliente_PlanoProduto> planoProdutoTemp = objBanco.PlanosProdutos.ToList();

                if (objBanco.Status.Value == Status_Cliente.Inativo)
                {
                    foreach (var planoProduto in planoProdutoTemp)
                    {
                        planoProduto.Status = Status_PlanoProduto.Cancelado;
                    }
                }
                else
                {
                    foreach (var planoProdutoView in view.Planos.FindAll(p => p.Id == 0))
                    {
                        var planoProduto = new Cliente_PlanoProduto();

                        Forma_Pagamento_Cliente formaPagamentoCliente =
                            objBanco.Formas_Pagamento_Cliente.First(f => f.Forma_Pagamento.Id == planoProdutoView.Id_Forma_De_Pagamento);

                        planoProduto.Cod_Area = planoProdutoView.Cod_Area;
                        planoProduto.Cod_Escritorio = planoProdutoView.Cod_Escritorio;
                        planoProduto.Cod_Localidade = planoProdutoView.Cod_Localidade;
                        planoProduto.Localidade = planoProdutoView.Localidade;
                        planoProduto.Nas = planoProdutoView.Nas;
                        planoProduto.Dias_Teste_Gratis = planoProdutoView.Qtde_Dias;
                        planoProduto.Parcelas_Instalacao = planoProdutoView.Numero_Parcelas;
                        planoProduto.Momento_Cobra_Instalacao = planoProdutoView?.Momento_Cobra_Instalacao;
                        planoProduto.Periodo_Carencia = planoProdutoView.Periodo_Carencia;
                        planoProduto.Meses_Carencia = planoProdutoView.Meses_Carencia;
                        planoProduto.Data_Habilitado = DateTime.Now;
                        planoProduto.End_Instalacao_Igual_Principal = planoProdutoView.End_Instalacao_Igual_Principal;
                        planoProduto.Login_Radius = planoProdutoView.Login_Radius;
                        planoProduto.Localidade_Anatel = planoProdutoView.Localidade_Anatel;
                        planoProduto.Senha_Radius = planoProdutoView.Senha_Radius;
                        planoProduto.Valor_Final = !Double.IsNaN(Convert.ToDouble(planoProdutoView.ValorPlanoProduto_Final)) ? Convert.ToDouble(planoProdutoView.ValorPlanoProduto_Final.Replace(".", ",")) : 0;
                        planoProduto.Valor_Final_Total = !Double.IsNaN(Convert.ToDouble(planoProdutoView.ValorTotal)) ? Convert.ToDouble(planoProdutoView.ValorTotal.Replace(".", ",")) : 0;
                        planoProduto.Valor_Real = !Double.IsNaN(Convert.ToDouble(planoProdutoView.ValorPlanoProduto)) ? Convert.ToDouble(planoProdutoView.ValorPlanoProduto.Replace(".", ",")) : 0;
                        planoProduto.Valor_Instalacao_Final = !String.IsNullOrEmpty(planoProdutoView.ValorInstalacao_Final) ? Convert.ToDouble(planoProdutoView.ValorInstalacao_Final.Replace(".", ",")) : 0;
                        planoProduto.Valor_Instalacao_Real = !String.IsNullOrEmpty(planoProdutoView.ValorInstalacao) ? Convert.ToDouble(planoProdutoView.ValorInstalacao.Replace(".", ",")) : 0;
                        planoProduto.Cliente = objBanco;
                        planoProduto.Atualiza_Ola = true;
                        planoProduto.Plano = planoProdutoView.Tipo == PlanoProdutoEnum.Plano ? repoPlano.selectFirst(p => p.Id == planoProdutoView.Id_Plano_Produto).Result : null;
                        planoProduto.Produto = planoProdutoView.Tipo == PlanoProdutoEnum.Produto ? repoProduto.selectFirst(p => p.Id == planoProdutoView.Id_Plano_Produto).Result : null;
                        planoProduto.Id_TrocaPlano = planoProduto.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null;
                        planoProduto.Pool_Ip = planoProdutoView.IdIpPool != null ? repoPoolIp.selectFirst(p => p.Id == planoProdutoView.IdIpPool).Result : null;
                        planoProduto.Quantidade_Produto_Variavel = planoProdutoView?.Quantidade;
                        planoProduto.Valor_Total_Plano_Variavel = !String.IsNullOrEmpty(planoProdutoView.Valor_Total_Plano) ? Convert.ToDouble(planoProdutoView.Valor_Total_Plano.Replace(".", ",")) : 0;
                        planoProduto.Parcela_Entrada = Convert.ToInt64(planoProdutoView?.Parcela);
                        planoProduto.Valor_Entrada = !String.IsNullOrEmpty(planoProdutoView.Valor_Entrada) ? Convert.ToDouble(planoProdutoView.Valor_Entrada.Replace(".", ",")) : 0;

                        // ************************************************
                        // Forma de Pagamento do Cliente Vinculada ao Plano
                        // ************************************************
                        if (formaPagamentoCliente.Id != 0)
                        {
                            planoProduto.Forma_Pagamento_Cliente = repoFormaPagamentoCliente.selectFirst(f => f.Id == formaPagamentoCliente.Id).Result;
                        }
                        else
                        {
                            planoProduto.Forma_Pagamento_Cliente = formaPagamentoCliente;
                        }

                        Modelo_Contrato modeloContrato =
                            planoProdutoView.Id_Modelo_Contrato.HasValue ?
                                repoModeloContrato.selectFirst(p => p.Id == planoProdutoView.Id_Modelo_Contrato.Value).Result :
                                null;

                        if (modeloContrato != null)
                        {
                            planoProduto.ClienteContrato = ClienteContrato.FromModelo(modeloContrato, planoProduto.Cliente);

                            // Gravação do arquivo
                            string caminhoArquivoModeloContrato =
                                $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modeloContrato.Id}.{modeloContrato.Extensao_Arquivo}";

                            byte[] arquivoModeloContrato = File.ReadAllBytes(caminhoArquivoModeloContrato);
                            planoProduto.ClienteContrato.Arquivo = arquivoModeloContrato;

                            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                            {
                                if (!String.IsNullOrEmpty(planoProdutoView.Numero_Contrato))
                                {
                                    bool existeNumContrato =
                                        repoClienteContrato.selectFirst(p => p.NumeroContrato == planoProdutoView.Numero_Contrato &&
                                                                             p.Cliente.Id != objBanco.Id).Result != null;

                                    if (!existeNumContrato)
                                        planoProduto.ClienteContrato.NumeroContrato = planoProdutoView.Numero_Contrato;
                                    else
                                        throw new Exception("Número de Contrato já Cadastrado para outro Cliente");
                                }
                                else
                                {
                                    long numContrato = repoClientePlanoProduto.MaxString("NUMERO_CONTRATO", "TB_CLIENTE_CONTRATO").Result;

                                    if (numContrato == 0 && config.NumeroContratoInicial.HasValue)
                                        numContrato = config.NumeroContratoInicial.Value;

                                    numContrato++;

                                    planoProduto.ClienteContrato.NumeroContrato = Convert.ToString(numContrato);
                                }
                            }

                            repoClienteContrato.Save(planoProduto.ClienteContrato).Wait();
                        }

                        // Criando Endereço Instalação
                        if (planoProduto.End_Instalacao_Igual_Principal)
                        {
                            planoProduto.Endereco_Instalacao.Pais = objBanco.Endereco_Principal?.Pais;
                            planoProduto.Endereco_Instalacao.Tipo_Imovel = objBanco.Endereco_Principal?.Tipo_Imovel;
                            planoProduto.Endereco_Instalacao.Id_Bairro = objBanco.Endereco_Principal?.Id_Bairro;
                            planoProduto.Endereco_Instalacao.Bairro = objBanco.Endereco_Principal?.Bairro;
                            planoProduto.Endereco_Instalacao.Cep = objBanco.Endereco_Principal?.Cep;
                            planoProduto.Endereco_Instalacao.Id_Cidade = objBanco.Endereco_Principal?.Id_Cidade;
                            planoProduto.Endereco_Instalacao.Cidade = objBanco.Endereco_Principal?.Cidade;
                            planoProduto.Endereco_Instalacao.Complemento = objBanco.Endereco_Principal?.Complemento;
                            planoProduto.Endereco_Instalacao.Latitude = objBanco.Endereco_Principal?.Latitude;
                            planoProduto.Endereco_Instalacao.Longitude = objBanco.Endereco_Principal?.Longitude;
                            planoProduto.Endereco_Instalacao.Logradouro = objBanco.Endereco_Principal?.Logradouro;
                            planoProduto.Endereco_Instalacao.Numero = objBanco.Endereco_Principal?.Numero;
                            planoProduto.Endereco_Instalacao.Ponto_Referencia = objBanco.Endereco_Principal?.Ponto_Referencia;
                            planoProduto.Endereco_Instalacao.UF = objBanco.Endereco_Principal?.UF;
                        }
                        else
                        {
                            planoProduto.Endereco_Instalacao = new Endereco_Cliente
                            {
                                Pais = planoProdutoView.Id_Pais != null ? repoPais.selectFirst(p => p.Id == planoProdutoView.Id_Pais.Value).Result : null,
                                Tipo_Imovel = planoProdutoView.Id_Tipo_Imovel != null ? repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == planoProdutoView.Id_Tipo_Imovel.Value).Result : null,
                                Id_Bairro = planoProdutoView.Id_Bairro,
                                Bairro = planoProdutoView.Bairro,
                                Cep = planoProdutoView.CEP,
                                Id_Cidade = planoProdutoView.Id_Cidade,
                                Cidade = planoProdutoView.Cidade,
                                Complemento = planoProdutoView.Complemento,
                                Latitude = planoProdutoView.Latitude != null ? Convert.ToDouble(planoProdutoView.Latitude?.Replace('.', ',')) : (Double?)null,
                                Longitude = planoProdutoView.Longitude != null ? Convert.ToDouble(planoProdutoView.Longitude?.Replace('.', ',')) : (Double?)null,
                                Logradouro = planoProdutoView.Logradouro,
                                Numero = planoProdutoView.Numero,
                                Ponto_Referencia = planoProdutoView?.Ponto_Referencia,
                                UF = planoProdutoView?.UF
                            };
                        }

                        if (planoProdutoView.Tipo == PlanoProdutoEnum.Plano)
                        {
                            planoProduto.Status = planoProduto.Plano.StatusInicial;
                        }
                        else
                        {
                            planoProduto.Status = planoProduto.Produto.Status_Inicial;
                        }


                        planoProdutoTemp.Add(planoProduto);

                        //Adiciona Log de alteração de status
                        logs.Add(new Cliente_PlanoProduto_StatusLog
                        {
                            AntigoStatus = null,
                            NovoStatus = planoProduto.Status,
                            PlanoProduto = planoProduto,
                            DataHoraAcao = DateTime.Now
                        });
                    }
                }

                objBanco.PlanosProdutos = planoProdutoTemp;

                #endregion

                AplicacaoDescontos(session,
                    objBanco,
                    new ValidaDescontosModel
                    {
                        Descontos = view.Descontos,
                        IdCategoriaCliente = view.Id_Categoria_Cliente,
                        IdMeioCaptacao = view.Id_Meio_Captacao,
                        Planos = view.Planos
                    },
                    retorno);

                objBanco.Qual_Meio = view.Id_Meio_Captacao.HasValue ? repoMeioCaptacao.selectFirst(p => p.Id == view.Id_Meio_Captacao.Value).Result : null;
                objBanco.Outro_Meio = objBanco.Qual_Meio?.Id == 0 ? view.Qual_Meio : null;


                // ****************************************
                // Composição NF Cliente
                // ****************************************

                List<ComposicaoNotaFiscalCliente> nfClientesComp = new List<ComposicaoNotaFiscalCliente>();

                if (view.ComposicaoNotaClientes != null)
                {
                    foreach (var item in view.ComposicaoNotaClientes)
                    {
                        if (item.Id == 0)
                        {
                            using (var repoCliPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                            using (var repoConfigTributaria = new Conexao<Configuracao_Tributaria>(session))
                            using (var repoEmpresa = new Conexao<Empresa>(session))
                            using (var repoFilial = new Conexao<Filial>(session))
                            using (var repoProdutoComp = new Conexao<ProdutosComp>(session))
                            {

                                var cliContrato = repoClienteContrato.selectFirst(x => x.Id == item.IdContrato).Result;
                                var cliPlanProd = repoCliPlanoProduto.selectFirst(c => c.Cliente.Id == objBanco.Id &&
                                                                                        c.ClienteContrato == cliContrato &&
                                                                                        c.Plano.Id == item.IdPlano).Result;

                                var configTributaria = repoConfigTributaria.selectFirst(c => c.Id == item.IdConfigTributaria).Result;
                                var empresa = repoEmpresa.selectFirst(e => e.Id == item.IdEmpresa).Result;
                                var filial = repoFilial.selectFirst(f => f.Id == item.IdFilial).Result;
                                var produtoComp = repoProdutoComp.selectFirst(p => p.Id == item.IdProdutoComp).Result;

                                var composicaoNF = new ComposicaoNotaFiscalCliente()
                                {
                                    ValorDiscriminado = Convert.ToDouble(item.Valor),
                                    ValorDiscriminadoNf = Convert.ToDouble(item.ValorDiscriminadoNf),
                                    PercentualDiscriminado = Convert.ToDouble(item.PercentualRateio.Replace("%", "")),
                                    ModeloNotaFiscal = Convert.ToString(item.ModeloNotaNum),
                                    Cliente = objBanco,
                                    ClienteContrato = cliContrato,
                                    ClientePlanoProduto = cliPlanProd,
                                    Empresa = empresa,
                                    Filial = filial,
                                    ProdutosComp = produtoComp,
                                    Configuracao_Tributaria = configTributaria,
                                    Instalacao = item.Instalacao
                                };

                                nfClientesComp.Add(composicaoNF);
                            }
                        }
                    }

                }

                if (nfClientesComp != null && nfClientesComp.Count > 0)
                {
                    repoNfCompCliente.SaveOrUpdate(nfClientesComp).Wait();
                }
            }

            return retorno;
        }


        public void AplicacaoDescontos(ISession session, Cliente objBanco, ValidaDescontosModel validaDescontosModel, List<PostApiMensagem> retorno, Int64? Id_Venda = null)
        {

            #region Desconto (Meio de Captação)
            using (var repoDesconto = new Conexao<Desconto>(session))
            using (var repoClienteDesconto = new Conexao<Cliente_Desconto>(session))
            using (var repoDescontoMeioCaptacao = new Conexao<Desconto_Meio_Captacao>(session))
            using (var repoDescontoCondicoes = new Conexao<Desconto_Condicoes>(session))
            using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repoDescontoRegioes = new Conexao<Desconto_Regioes>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var descontoApiController = new DescontoApiController())
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoFormaPagamento = new Conexao<Forma_Pagamento>(session))
            using (var repoFormaPagamentoCliente = new Conexao<Forma_Pagamento_Cliente>(session))

            {
                //Nesse momento é carregado com descontos manuais
                List<Cliente_Desconto> descontostemp = objBanco.Descontos.ToList();

                var formasPagamento = repoFormaPagamento.select(p => p.Id.IsIn(validaDescontosModel.Planos.Where(p => !p.isPlanoAdicional).Select(p => p.Id_Forma_De_Pagamento).ToList())).Result;
                var descontosAutomaticos = descontoApiController.Filtragem_Desconto(session, new DescontoParametros
                {
                    Categoria_Cliente = Convert.ToString(objBanco.Categoria_Cliente?.Id),
                    Tipo_Tecnologia = validaDescontosModel.Planos.Select(p => p.Tipo_Tecnologia).ToList(),
                    Meio_Captacao = Convert.ToString(objBanco.Qual_Meio?.Id),
                    Tipo_Forma_Pagamento = formasPagamento.Select(p => (Tipo_Forma_Pagamento?)p.Tipo_Forma_Pagamento).ToList(),
                    Forma_Pagamento = validaDescontosModel.Planos.Select(p => Convert.ToString(p.Id_Forma_De_Pagamento)).ToList(),
                    Tipo_Cliente = objBanco?.Tipo_Pessoa,
                    Plano_Produto = validaDescontosModel.Planos.Select(p => Convert.ToString(p.Id_Plano_Produto)).ToList(),
                    Pais = objBanco?.Endereco_Principal.Pais?.Nome.ToUpperInvariant(),
                    Estado = String.IsNullOrEmpty(objBanco.Endereco_Principal.UF) ? null : repoUf.selectFirst(p => p.Sigla == objBanco.Endereco_Principal.UF).Result?.Sigla,
                    Cidade = objBanco.Endereco_Principal?.Id_Cidade != null ? repoCidade.selectFirst(p => p.Id == objBanco.Endereco_Principal.Id_Cidade).Result?.Descricao : null,
                    Valor_Plano_Final = validaDescontosModel.Planos.Select(p => (double?)Convert.ToDouble(p.ValorTotal)).ToList(),
                    Valor_Final_Total = Convert.ToString(validaDescontosModel.Planos.Select(p => Convert.ToDouble(p.ValorTotal)).Sum()),
                    Pag_Sem_Atraso_Meses = objBanco.Descontos.Select(p =>
                    {
                        var iteratorCondiciones = p.Desconto?.Condicoes?.FirstOrDefault(condicao => condicao.TipoCondicao == "11");
                        if (iteratorCondiciones != null)
                            return Convert.ToInt32(iteratorCondiciones.ValorCondicao);
                        else
                            return 0;
                    }).FirstOrDefault(p => p > 0),
                    TemCondomioPredio = objBanco.Endereco_Principal?.Condominio_Predio,
                }).Result.Where(p => p.Automatico).ToList();



                foreach (var desconto in validaDescontosModel.Descontos.FindAll(p => p.Id == 0))
                {
                    var desc = repoDesconto.selectFirst(p => p.Id == desconto.Id_Desconto).Result;
                    desc.UsuarioCriacao = desconto.Usuario;
                    var descIndeterminado = desc != null ? desc.Desconto_Tempo_Indeterminado : false;

                    var clienteDesconto = new Cliente_Desconto
                    {
                        Valor_Desconto = desconto.Valor_Desconto != "-" && !String.IsNullOrEmpty(desconto.Valor_Desconto) ? Convert.ToDouble(desconto.Valor_Desconto) : 0,
                        Percentual_Desconto = (desconto.Percentual_Desconto != "-" && String.IsNullOrEmpty(desconto.Percentual_Desconto) == false) ? Convert.ToDouble(desconto.Percentual_Desconto) : 0,
                        Desconto = desc,
                        Cliente = objBanco,
                        Numero_ContratoVinculado = String.IsNullOrEmpty(desconto.Numero_ContratoVinculado) ? objBanco.PlanosProdutos.Select(p => p.ClienteContrato?.NumeroContrato).FirstOrDefault() : desconto.Numero_ContratoVinculado,
                        DataAtribuicaoDesconto = DateTime.Now,
                        UsuarioCriacao = desconto.Usuario
                    };

                    if (descIndeterminado)
                        clienteDesconto.DescontoIndeterminado = true;
                    else
                    {
                        clienteDesconto.DataVencimentoDesconto = desc != null ? DateTime.Now.AddMonths((int)desc.Repeticoes_Desconto.Value) : (DateTime?)null;
                        clienteDesconto.DescontoIndeterminado = false;
                    }

                    descontostemp.Add(clienteDesconto);
                }

                Configuracoes_Gerais conf_gerais = repoConfigGerais.selectFirst().Result;

                bool Descontos_Tipo_Diferente = conf_gerais.Acumular_Descontos_Diferentes;
                bool Acumula_Descontos_Mesmo_Tipo = conf_gerais.Acumular_Descontos_Mesmo_Tipo;

                foreach (Desconto item in descontosAutomaticos.Where(p => p.Status == Status_Default.Ativo))
                {
                    if (validaDescontosModel.Descontos.Find(p => p.Id_Desconto == item.Id) == null)
                    {

                        if (Id_Venda != null)
                        {

                            if (item.Condicoes.Any(p => p.TipoCondicao == "7") && item.Valor_Desconto != null)
                            {
                                CalcValorFinalTotal(descontostemp, objBanco, item, Id_Venda).Wait();
                            }

                            //Else
                            ValidaCondicaoDesconto(session, descontostemp, item, retorno, objBanco, Descontos_Tipo_Diferente, Acumula_Descontos_Mesmo_Tipo, true, Id_Venda);
                        }
                        else
                        {
                            ValidaCondicaoDesconto(session, descontostemp, item, retorno, objBanco, Descontos_Tipo_Diferente, Acumula_Descontos_Mesmo_Tipo, true);

                        }
                    }

                }


                objBanco.Descontos = descontostemp;

                #endregion

            }
        }

        private async Task CalcValorFinalTotal(List<Cliente_Desconto> descontostemp, Cliente objBanco, Desconto descontoParam, long? Id_Venda)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoCPP = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoDesconto = new Conexao<Desconto>(session))
            {
                Func<double, double, double> Porcentagem = (valor_item, valorFinalVenda) => (valor_item / valorFinalVenda) * 100;

                IEnumerable<Cliente_PlanoProduto> resultsPlanoProduto = await repoCPP.select(p => p.Id_Venda == Id_Venda);


                double Valor_FinalVenda = resultsPlanoProduto.Sum(p => p?.Valor_Final ?? 0);

                IEnumerable<IGrouping<string, Cliente_PlanoProduto>> Grupos = resultsPlanoProduto.GroupBy(p => p.ClienteContrato.NumeroContrato);
                foreach (var Grupo in Grupos)
                {
                    double PercentualContrato = Porcentagem(Grupo.Sum(p => p.Valor_Final.Value), Valor_FinalVenda);

                    double calculoRepresentacaoDesconto = (descontoParam.Valor_Desconto.Value * PercentualContrato) / 100;

                    descontostemp.Add(new Cliente_Desconto
                    {
                        Cliente = objBanco,
                        Numero_ContratoVinculado = Grupo.Key,
                        Valor_Desconto = calculoRepresentacaoDesconto > 0 ? calculoRepresentacaoDesconto : 0,
                        Percentual_Desconto = 0,
                        Desconto = descontoParam,
                    });
                }


            }
        }
        private void ValidaCondicaoDesconto(ISession session, List<Cliente_Desconto> descontostemp, Desconto descontoParam, List<PostApiMensagem> retorno, Cliente objBanco, bool Descontos_Tipo_Diferente, bool Acumula_Descontos_Mesmo_Tipo, bool novaCondicao, long? Id_venda = null)
        {
            #region Obsolete
            //Func<double, Desconto, bool, double> ValorComDesconto_MeioCaptacaobyContrato = (double param1, Desconto param2, bool porc) =>
            //{

            //    if (porc) { return param1 - (param1 * (param2.Percentual_Desconto.Value / 100)); }

            //    else { return param1 - param2.Valor_Desconto.Value; }
            //};
            #endregion

            using (var repoDesconto = new Conexao<Desconto>(session))
            {
                #region Cálculo de Desconto

                double valorTotalPlanos = 0,
                       Valor_Com_Desconto = 0;
                IEnumerable<IGrouping<string, Cliente_PlanoProduto>> Agrupamentos = Id_venda != null ? objBanco.PlanosProdutos.Where(p => p.Id_Venda == Id_venda)
                                                                                                                              .GroupBy(p => p.ClienteContrato?.NumeroContrato) :
                                                                                                       objBanco.PlanosProdutos.GroupBy(p => p.ClienteContrato?.NumeroContrato);
                foreach (var grupo in Agrupamentos)
                {
                    //Numero_Contrato
                    Desconto Desconto = descontoParam;
                    Desconto.Numero_Contrato = grupo.Key;

                    bool DescontoJaCadastrado = (descontostemp.Find(p => Desconto.Id == (p.Desconto != null ? p.Desconto.Id : -1)) != null) ? true : false;
                    List<Tipo_Desconto> Tipo_Desconto = descontostemp.Select(p => p.Desconto != null ? p.Desconto.Tipo_Desconto : null).ToList();

                    valorTotalPlanos = grupo.ToList().Sum(p => p.Valor_Final_Total.Value);
                    Valor_Com_Desconto += ValorComDesconto_MeioCaptacao(Desconto, valorTotalPlanos/*, new List<Desconto>()*/);

                    AdicionarClienteDesconto(new AdicionarClienteDescontoModel
                    {
                        Acumula_Descontos_Mesmo_Tipo = Acumula_Descontos_Mesmo_Tipo,
                        Desconto = Desconto,
                        DescontoJaCadastrado = (descontostemp.Where(p => p.Numero_ContratoVinculado == grupo.Key).ToList()
                                                             .Find(p => Desconto.Id == (p.Desconto != null ? p.Desconto.Id : -1)) != null) ? true : false,

                        descontostemp = descontostemp,
                        Descontos_Tipo_Diferente = Descontos_Tipo_Diferente,
                        novaCondicao = novaCondicao,
                        objBanco = objBanco,
                        retorno = retorno,

                        Tipo_Desconto = descontostemp.Select(p => p.Desconto != null ? p.Desconto.Tipo_Desconto : null).ToList(),
                        Valor_Com_Desconto = Valor_Com_Desconto,
                        Desconto_Captacao = Desconto.Desconto_Captacao,
                    });
                }
                #endregion
            }

        }

        private void AdicionarClienteDesconto(AdicionarClienteDescontoModel Parametros)
        {
            #region Condições
            if (Parametros.Desconto_Captacao == Desconto_Captacao.Contratante)
            {
                if (Parametros.DescontoJaCadastrado && Parametros.novaCondicao)
                {
                    Parametros.retorno.Add(new PostApiMensagem
                    {
                        Mensagem = String.Format(RsCadastros.DESCONTO_JA_CADASTRADO_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia, Parametros.Desconto.Descricao),
                        Tipo = PostApiMensagemType.Info
                    });
                }
                else if (!Parametros.Acumula_Descontos_Mesmo_Tipo && !Parametros.Descontos_Tipo_Diferente)
                {
                    if (Parametros.descontostemp.Count != 0 && Parametros.novaCondicao)
                    {
                        Parametros.retorno.Add(new PostApiMensagem
                        {
                            Mensagem = String.Format(RsCadastros.SOMENTE_UM_DESCONTO, Parametros.objBanco.NomeFantasia),
                            Tipo = PostApiMensagemType.Info
                        });

                    }
                    else
                    {
                        if (Parametros.Valor_Com_Desconto > 0)
                        {
                            Parametros.descontostemp.Add(new Cliente_Desconto
                            {
                                Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                Desconto = Parametros.Desconto,
                                Cliente = Parametros.objBanco,
                                Numero_ContratoVinculado = Parametros.Desconto.Numero_Contrato,
                            });
                        }
                        else if (Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }
                }
                else if (!Parametros.Acumula_Descontos_Mesmo_Tipo)
                {
                    if (Parametros.Tipo_Desconto.Where(p => p == Parametros.Desconto.Tipo_Desconto).Count() != 0)
                    {
                        if (Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.MESMO_TIPO_DESCONTO_NAO_PERMITIDO, Parametros.objBanco.NomeFantasia, Parametros.Desconto.Tipo_Desconto.Descricao),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }
                    else
                    {
                        if (Parametros.Valor_Com_Desconto > 0)
                        {
                            Parametros.descontostemp.Add(new Cliente_Desconto
                            {
                                Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                Desconto = Parametros.Desconto,
                                Cliente = Parametros.objBanco,
                                Numero_ContratoVinculado = Parametros.Desconto.Numero_Contrato,
                            });
                        }
                        else if (Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }
                }
                else if (!Parametros.Descontos_Tipo_Diferente)
                {
                    var ListaTipo = Parametros.Tipo_Desconto.ToList();
                    ListaTipo.Add(Parametros.Desconto.Tipo_Desconto);

                    if (ListaTipo.Distinct().Count() > 1 && Parametros.novaCondicao)
                    {
                        Parametros.retorno.Add(new PostApiMensagem
                        {
                            Mensagem = String.Format(RsCadastros.TIPO_DESCONTO_DIFERENTE_NAO_PERMITIDO, Parametros.objBanco.NomeFantasia),
                            Tipo = PostApiMensagemType.Info
                        });
                    }
                    else
                    {
                        if (Parametros.Valor_Com_Desconto > 0)
                        {
                            Parametros.descontostemp.Add(new Cliente_Desconto
                            {
                                Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                Desconto = Parametros.Desconto,
                                Cliente = Parametros.objBanco,
                                Numero_ContratoVinculado = Parametros.Desconto.Numero_Contrato,
                            });
                        }
                        else if (Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }
                }
                else
                {
                    if (Parametros.Valor_Com_Desconto > 0)
                    {
                        Parametros.descontostemp.Add(new Cliente_Desconto
                        {
                            Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                            Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                            Desconto = Parametros.Desconto,
                            Cliente = Parametros.objBanco,
                            Numero_ContratoVinculado = Parametros.Desconto.Numero_Contrato,
                        });
                    }
                    else if (Parametros.novaCondicao)
                    {
                        Parametros.retorno.Add(new PostApiMensagem
                        {
                            Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                            Tipo = PostApiMensagemType.Info
                        });
                    }
                }
            }
            else if (Parametros.Desconto_Captacao == Desconto_Captacao.QuemIndicou)
            {
                #region Cálculo de Descontos

                double Valor_Plano_Produto_Quem_Indicou = Parametros?.objBanco?.Quem_Indicou?.PlanosProdutos?.Sum(p => p.Valor_Final_Total.Value) ?? 0;
                //List<Desconto> listaDescontos = Parametros?.objBanco?.Quem_Indicou != null ? Parametros.objBanco.Quem_Indicou.Descontos.Select(p => p.Desconto).ToList() : new List<Desconto>();

                double Valor_Com_Desconto_Quem_Indicou = ValorComDesconto_MeioCaptacao(Parametros.Desconto,
                                                                                        Valor_Plano_Produto_Quem_Indicou
                                                                                        /*listaDescontos*/);

                #endregion

                if (Valor_Com_Desconto_Quem_Indicou > 0)
                {
                    var Descontos_Quem_Indicou = Parametros.objBanco.Quem_Indicou.Descontos.Select(p => p.Desconto).ToList();
                    Descontos_Quem_Indicou.Add(Parametros.Desconto);

                    var Tipo_Desconto_Quem_Indicou = Descontos_Quem_Indicou.Select(p => p.Tipo_Desconto);

                    Parametros.DescontoJaCadastrado = Parametros.objBanco.Quem_Indicou != null ? ((Parametros.objBanco.Quem_Indicou.Descontos.ToList().Find(p => p.Desconto.Id == Parametros.Desconto.Id) != null) ? true : false) : false;
                    bool Tipo_Desconto_Ja_Cadastrado = Tipo_Desconto_Quem_Indicou.Where(p => p == Parametros.Desconto.Tipo_Desconto) != null ? true : false;

                    if (Parametros.DescontoJaCadastrado && Parametros.novaCondicao)
                    {
                        Parametros.retorno.Add(new PostApiMensagem
                        {
                            Mensagem = String.Format(RsCadastros.DESCONTO_JA_CADASTRADO_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou.NomeFantasia, Parametros.Desconto.Descricao),
                            Tipo = PostApiMensagemType.Info
                        });

                    }
                    else if (!Parametros.Acumula_Descontos_Mesmo_Tipo && !Parametros.Descontos_Tipo_Diferente)
                    {
                        if (Parametros.objBanco.Quem_Indicou.Descontos.Count != 0 && Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.SOMENTE_UM_DESCONTO, Parametros.objBanco.Quem_Indicou.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });

                        }
                        else
                        {
                            Parametros.descontostemp.Add(new Cliente_Desconto
                            {
                                Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                Desconto = Parametros.Desconto,
                                Cliente = Parametros.objBanco.Quem_Indicou,
                                Numero_ContratoVinculado = Parametros.Desconto.Numero_Contrato,
                            });
                        }
                    }
                    else if (!Parametros.Acumula_Descontos_Mesmo_Tipo)
                    {
                        if (Tipo_Desconto_Quem_Indicou.Distinct().Count() != Descontos_Quem_Indicou.Count() && Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.MESMO_TIPO_DESCONTO_NAO_PERMITIDO, Parametros.objBanco.Quem_Indicou.NomeFantasia, Parametros.Desconto.Tipo_Desconto.Descricao),
                                Tipo = PostApiMensagemType.Info
                            });

                        }
                        else
                        {
                            Parametros.descontostemp.Add(new Cliente_Desconto
                            {
                                Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                Desconto = Parametros.Desconto,
                                Cliente = Parametros.objBanco.Quem_Indicou,
                                Numero_ContratoVinculado = Parametros.Desconto.Numero_Contrato,
                            });
                        }
                    }
                    else if (!Parametros.Descontos_Tipo_Diferente)
                    {
                        var ListaTipo = Tipo_Desconto_Quem_Indicou.ToList();
                        ListaTipo.Add(Parametros.Desconto.Tipo_Desconto);

                        if (ListaTipo.Distinct().Count() > 1 && Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.TIPO_DESCONTO_DIFERENTE_NAO_PERMITIDO, Parametros.objBanco.Quem_Indicou.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });

                        }
                        else
                        {
                            Parametros.descontostemp.Add(new Cliente_Desconto
                            {
                                Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                Desconto = Parametros.Desconto,
                                Cliente = Parametros.objBanco.Quem_Indicou,
                            });
                        }
                    }
                    else
                    {
                        Parametros.descontostemp.Add(new Cliente_Desconto
                        {
                            Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                            Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                            Desconto = Parametros.Desconto,
                            Cliente = Parametros.objBanco.Quem_Indicou,
                        });
                    }
                }
                else if (Parametros.novaCondicao)
                {
                    Parametros.retorno.Add(new PostApiMensagem
                    {
                        Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco?.Quem_Indicou?.NomeFantasia),
                        Tipo = PostApiMensagemType.Info
                    });
                }

            }
            else
            {
                Parametros.DescontoJaCadastrado = (Parametros.descontostemp.Find(p => Parametros.Desconto.Id == (p.Desconto != null ? p.Desconto.Id : -1)) != null) ? true : false;
                bool DescontoJaCadastrado_QuemIndicou = (Parametros.objBanco.Quem_Indicou?.Descontos.ToList().Find(p => Parametros.Desconto.Id == (p.Desconto != null ? p.Desconto.Id : -1)) != null) ? true : false;

                var Descontos_Quem_Indicou = Parametros.objBanco.Quem_Indicou?.Descontos.Select(p => p.Desconto).ToList() ?? new List<Desconto>();
                Descontos_Quem_Indicou.Add(Parametros.Desconto);

                var Tipo_Desconto_Quem_Indicou = Descontos_Quem_Indicou.Select(p => p.Tipo_Desconto) ?? new List<Tipo_Desconto>();
                Parametros.Tipo_Desconto = Parametros.descontostemp.Select(p => p.Desconto != null ? p.Desconto.Tipo_Desconto : null).ToList() ?? new List<Tipo_Desconto>();

                #region Cálculo do Desconto

                double valorTotalPlanos = Parametros.objBanco.PlanosProdutos.Sum(p => p.Valor_Final_Total.Value);
                List<Desconto> listaDescontos = Parametros.descontostemp.Select(p => p.Desconto).ToList();

                Parametros.Valor_Com_Desconto = ValorComDesconto_MeioCaptacao(Parametros.Desconto, valorTotalPlanos/*, listaDescontos*/);

                double Valor_Plano_Produto_Quem_Indicou = (Parametros.objBanco.Quem_Indicou?.PlanosProdutos.Sum(p => p.Valor_Final_Total.Value)).GetValueOrDefault(0);
                List<Desconto> listaDescontosQuemIndicou = Parametros.objBanco.Quem_Indicou?.Descontos.Select(p => p.Desconto).ToList() ?? new List<Desconto>();

                double Valor_Com_Desconto_Quem_Indicou = ValorComDesconto_MeioCaptacao(Parametros.Desconto,
                                                                                        Valor_Plano_Produto_Quem_Indicou
                                                                                        /*listaDescontosQuemIndicou*/);

                #endregion

                if (Parametros.DescontoJaCadastrado)
                {
                    if (!DescontoJaCadastrado_QuemIndicou)
                    {
                        if (Valor_Com_Desconto_Quem_Indicou > 0)
                        {
                            if (!Parametros.Acumula_Descontos_Mesmo_Tipo && !Parametros.Descontos_Tipo_Diferente)
                            {
                                if (Parametros.objBanco.Quem_Indicou?.Descontos.Count != 0 && Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.SOMENTE_UM_DESCONTO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });

                                }
                                else
                                {
                                    Parametros.descontostemp.Add(new Cliente_Desconto
                                    {
                                        Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                        Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                        Desconto = Parametros.Desconto,
                                        Cliente = Parametros.objBanco.Quem_Indicou,
                                    });
                                }
                            }
                            else if (!Parametros.Acumula_Descontos_Mesmo_Tipo)
                            {
                                if (Tipo_Desconto_Quem_Indicou?.Distinct().Count() != Descontos_Quem_Indicou?.Count && Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.MESMO_TIPO_DESCONTO_NAO_PERMITIDO, Parametros.objBanco.Quem_Indicou?.NomeFantasia, Parametros.Desconto.Tipo_Desconto.Descricao),
                                        Tipo = PostApiMensagemType.Info
                                    });

                                }
                                else
                                {
                                    Parametros.descontostemp.Add(new Cliente_Desconto
                                    {
                                        Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                        Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                        Desconto = Parametros.Desconto,
                                        Cliente = Parametros.objBanco?.Quem_Indicou,
                                    });
                                }
                            }
                            else if (!Parametros.Descontos_Tipo_Diferente)
                            {
                                var ListaTipo = Tipo_Desconto_Quem_Indicou.ToList();
                                ListaTipo.Add(Parametros.Desconto.Tipo_Desconto);

                                if (ListaTipo.Distinct().Count() > 1)
                                {
                                    if (Parametros.novaCondicao)
                                    {
                                        Parametros.retorno.Add(new PostApiMensagem
                                        {
                                            Mensagem = String.Format(RsCadastros.TIPO_DESCONTO_DIFERENTE_NAO_PERMITIDO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                            Tipo = PostApiMensagemType.Info
                                        });
                                    }
                                }
                                else
                                {
                                    Parametros.descontostemp.Add(new Cliente_Desconto
                                    {
                                        Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                        Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                        Desconto = Parametros.Desconto,
                                        Cliente = Parametros.objBanco.Quem_Indicou,
                                    });
                                }
                            }
                            else
                            {
                                Parametros.descontostemp.Add(new Cliente_Desconto
                                {
                                    Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                    Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                    Desconto = Parametros.Desconto,
                                    Cliente = Parametros.objBanco.Quem_Indicou,
                                });
                            }
                        }
                        else if (Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }
                    else if (Parametros.novaCondicao)
                    {
                        Parametros.retorno.Add(new PostApiMensagem
                        {
                            Mensagem = String.Format(RsCadastros.DESCONTO_JA_CADASTRADO_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia, Parametros.Desconto.Descricao),
                            Tipo = PostApiMensagemType.Info
                        });

                    }

                    if (Parametros.novaCondicao)
                    {
                        Parametros.retorno.Add(new PostApiMensagem
                        {
                            Mensagem = String.Format(RsCadastros.DESCONTO_JA_CADASTRADO_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia, Parametros.Desconto.Descricao),
                            Tipo = PostApiMensagemType.Info
                        });
                    }
                }
                else if (DescontoJaCadastrado_QuemIndicou)
                {
                    if (!Parametros.DescontoJaCadastrado)
                    {
                        if (!Parametros.Acumula_Descontos_Mesmo_Tipo && !Parametros.Descontos_Tipo_Diferente)
                        {
                            if (Parametros.descontostemp.Count != 0)
                            {
                                if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.SOMENTE_UM_DESCONTO, Parametros.objBanco.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                            else
                            {
                                if (Parametros.Valor_Com_Desconto > 0)
                                {
                                    Parametros.descontostemp.Add(new Cliente_Desconto
                                    {
                                        Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                        Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                        Desconto = Parametros.Desconto,
                                        Cliente = Parametros.objBanco,
                                    });
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                        }
                        else if (!Parametros.Acumula_Descontos_Mesmo_Tipo)
                        {
                            if (Parametros.Tipo_Desconto.Where(p => p == Parametros.Desconto.Tipo_Desconto).Count() != 0)
                            {
                                if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.MESMO_TIPO_DESCONTO_NAO_PERMITIDO, Parametros.objBanco.NomeFantasia, Parametros.Desconto.Tipo_Desconto.Descricao),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                            else
                            {
                                if (Parametros.Valor_Com_Desconto > 0)
                                {
                                    Parametros.descontostemp.Add(new Cliente_Desconto
                                    {
                                        Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                        Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                        Desconto = Parametros.Desconto,
                                        Cliente = Parametros.objBanco,
                                    });
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                        }
                        else if (!Parametros.Descontos_Tipo_Diferente)
                        {
                            var ListaTipo = Parametros.Tipo_Desconto.ToList();
                            ListaTipo.Add(Parametros.Desconto.Tipo_Desconto);

                            if (ListaTipo.Distinct().Count() > 1)
                            {
                                if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.TIPO_DESCONTO_DIFERENTE_NAO_PERMITIDO, Parametros.objBanco.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                            else
                            {
                                if (Parametros.Valor_Com_Desconto > 0)
                                {
                                    Parametros.descontostemp.Add(new Cliente_Desconto
                                    {
                                        Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                        Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                        Desconto = Parametros.Desconto,
                                        Cliente = Parametros.objBanco,
                                    });
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (Parametros.Valor_Com_Desconto > 0)
                            {
                                Parametros.descontostemp.Add(new Cliente_Desconto
                                {
                                    Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                    Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                    Desconto = Parametros.Desconto,
                                    Cliente = Parametros.objBanco,
                                });
                            }
                            else if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }
                        }
                    }
                    else if (Parametros.novaCondicao)
                    {
                        Parametros.retorno.Add(new PostApiMensagem
                        {
                            Mensagem = String.Format(RsCadastros.DESCONTO_JA_CADASTRADO_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia, Parametros.Desconto.Descricao),
                            Tipo = PostApiMensagemType.Info
                        });
                    }

                    if (Parametros.novaCondicao)
                    {
                        Parametros.retorno.Add(new PostApiMensagem
                        {
                            Mensagem = String.Format(RsCadastros.DESCONTO_JA_CADASTRADO_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia, Parametros.Desconto.Descricao),
                            Tipo = PostApiMensagemType.Info
                        });
                    }
                }
                else
                {
                    if (!Parametros.Acumula_Descontos_Mesmo_Tipo && !Parametros.Descontos_Tipo_Diferente)
                    {
                        if ((Parametros.descontostemp.Count != 0) || (Parametros.objBanco.Quem_Indicou?.Descontos.Count != 0))
                        {
                            if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.SOMENTE_UM_DESCONTO, Parametros.descontostemp.Count != 0 ? Parametros.objBanco.NomeFantasia : Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }

                            if (Parametros.descontostemp.Count != 0)
                            {
                                if (Parametros.objBanco.Quem_Indicou?.Descontos.Count == 0)
                                {
                                    if (Valor_Com_Desconto_Quem_Indicou > 0)
                                    {
                                        Parametros.descontostemp.Add(new Cliente_Desconto
                                        {
                                            Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                            Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                            Desconto = Parametros.Desconto,
                                            Cliente = Parametros.objBanco.Quem_Indicou,
                                        });
                                    }
                                    else if (Parametros.novaCondicao)
                                    {
                                        Parametros.retorno.Add(new PostApiMensagem
                                        {
                                            Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                            Tipo = PostApiMensagemType.Info
                                        });
                                    }
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.SOMENTE_UM_DESCONTO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                            else if (Parametros.objBanco.Quem_Indicou?.Descontos.Count != 0)
                            {
                                if (Parametros.descontostemp.Count == 0)
                                {
                                    if (Parametros.Valor_Com_Desconto > 0)
                                    {
                                        Parametros.descontostemp.Add(new Cliente_Desconto
                                        {
                                            Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                            Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                            Desconto = Parametros.Desconto,
                                            Cliente = Parametros.objBanco,
                                        });
                                    }
                                    else if (Parametros.novaCondicao)
                                    {
                                        Parametros.retorno.Add(new PostApiMensagem
                                        {
                                            Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                            Tipo = PostApiMensagemType.Info
                                        });
                                    }
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.SOMENTE_UM_DESCONTO, Parametros.objBanco.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (Parametros.Valor_Com_Desconto > 0)
                            {
                                Parametros.descontostemp.Add(new Cliente_Desconto
                                {
                                    Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                    Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                    Desconto = Parametros.Desconto,
                                    Cliente = Parametros.objBanco,
                                });
                            }
                            else if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }

                            if (Valor_Com_Desconto_Quem_Indicou > 0)
                            {
                                Parametros.descontostemp.Add(new Cliente_Desconto
                                {
                                    Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                    Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                    Desconto = Parametros.Desconto,
                                    Cliente = Parametros.objBanco.Quem_Indicou,
                                });
                            }
                            else if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }
                        }
                    }
                    else if (!Parametros.Acumula_Descontos_Mesmo_Tipo)
                    {
                        bool tiposContratante = Parametros.Tipo_Desconto.Where(p => p == Parametros.Desconto.Tipo_Desconto).Count() != 0;
                        bool tiposQuemIndicou = Tipo_Desconto_Quem_Indicou?.ToList().Distinct().Count() != Descontos_Quem_Indicou.Count;

                        if (tiposContratante || tiposQuemIndicou)
                        {
                            if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.MESMO_TIPO_DESCONTO_NAO_PERMITIDO, tiposContratante ? Parametros.objBanco.NomeFantasia : Parametros.objBanco.Quem_Indicou?.NomeFantasia, Parametros.Desconto.Tipo_Desconto.Descricao),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }

                            if (tiposContratante)
                            {
                                if (!tiposQuemIndicou)
                                {
                                    if (Valor_Com_Desconto_Quem_Indicou > 0)
                                    {
                                        Parametros.descontostemp.Add(new Cliente_Desconto
                                        {
                                            Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                            Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                            Desconto = Parametros.Desconto,
                                            Cliente = Parametros.objBanco.Quem_Indicou,
                                        });
                                    }
                                    else if (Parametros.novaCondicao)
                                    {
                                        Parametros.retorno.Add(new PostApiMensagem
                                        {
                                            Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                            Tipo = PostApiMensagemType.Info
                                        });
                                    }
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.MESMO_TIPO_DESCONTO_NAO_PERMITIDO, Parametros.objBanco.Quem_Indicou?.NomeFantasia, Parametros.Desconto.Tipo_Desconto.Descricao),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                            else if (tiposQuemIndicou)
                            {
                                if (!tiposContratante)
                                {
                                    if (Parametros.Valor_Com_Desconto > 0)
                                    {
                                        Parametros.descontostemp.Add(new Cliente_Desconto
                                        {
                                            Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                            Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                            Desconto = Parametros.Desconto,
                                            Cliente = Parametros.objBanco,
                                        });
                                    }
                                    else if (Parametros.novaCondicao)
                                    {
                                        Parametros.retorno.Add(new PostApiMensagem
                                        {
                                            Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                            Tipo = PostApiMensagemType.Info
                                        });
                                    }
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.MESMO_TIPO_DESCONTO_NAO_PERMITIDO, Parametros.objBanco.NomeFantasia, Parametros.Desconto.Tipo_Desconto.Descricao),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (Parametros.Valor_Com_Desconto > 0)
                            {
                                Parametros.descontostemp.Add(new Cliente_Desconto
                                {
                                    Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                    Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                    Desconto = Parametros.Desconto,
                                    Cliente = Parametros.objBanco,
                                });
                            }
                            else if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }

                            if (Valor_Com_Desconto_Quem_Indicou > 0)
                            {
                                Parametros.descontostemp.Add(new Cliente_Desconto
                                {
                                    Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                    Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                    Desconto = Parametros.Desconto,
                                    Cliente = Parametros.objBanco.Quem_Indicou,
                                });
                            }
                            else if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }
                        }
                    }
                    else if (!Parametros.Descontos_Tipo_Diferente)
                    {
                        var ListaTipo = Parametros.Tipo_Desconto.ToList();
                        ListaTipo.Add(Parametros.Desconto.Tipo_Desconto);

                        var ListaTipoQuemIndicou = Tipo_Desconto_Quem_Indicou?.ToList();
                        ListaTipoQuemIndicou.Add(Parametros.Desconto.Tipo_Desconto);


                        bool tiposDiferentesContratante = ListaTipo.Distinct().Count() > 1;
                        bool tiposDiferentesQuemIndicou = ListaTipoQuemIndicou.Distinct().Count() > 1;

                        if (tiposDiferentesContratante || tiposDiferentesQuemIndicou)
                        {
                            if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.TIPO_DESCONTO_DIFERENTE_NAO_PERMITIDO, tiposDiferentesContratante ? Parametros.objBanco.NomeFantasia : Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }

                            if (ListaTipo.Distinct().Count() > 1)
                            {
                                if (ListaTipoQuemIndicou.Distinct().Count() <= 1)
                                {
                                    if (Valor_Com_Desconto_Quem_Indicou > 0)
                                    {
                                        Parametros.descontostemp.Add(new Cliente_Desconto
                                        {
                                            Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                            Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                            Desconto = Parametros.Desconto,
                                            Cliente = Parametros.objBanco.Quem_Indicou,
                                        });
                                    }
                                    else if (Parametros.novaCondicao)
                                    {
                                        Parametros.retorno.Add(new PostApiMensagem
                                        {
                                            Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                            Tipo = PostApiMensagemType.Info
                                        });
                                    }
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.TIPO_DESCONTO_DIFERENTE_NAO_PERMITIDO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                            else if (ListaTipoQuemIndicou.Distinct().Count() > 1)
                            {
                                if (ListaTipo.Distinct().Count() <= 1)
                                {
                                    if (Parametros.Valor_Com_Desconto > 0)
                                    {
                                        Parametros.descontostemp.Add(new Cliente_Desconto
                                        {
                                            Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                            Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                            Desconto = Parametros.Desconto,
                                            Cliente = Parametros.objBanco,
                                        });
                                    }
                                    else if (Parametros.novaCondicao)
                                    {
                                        Parametros.retorno.Add(new PostApiMensagem
                                        {
                                            Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                            Tipo = PostApiMensagemType.Info
                                        });
                                    }
                                }
                                else if (Parametros.novaCondicao)
                                {
                                    Parametros.retorno.Add(new PostApiMensagem
                                    {
                                        Mensagem = String.Format(RsCadastros.TIPO_DESCONTO_DIFERENTE_NAO_PERMITIDO, Parametros.objBanco.NomeFantasia),
                                        Tipo = PostApiMensagemType.Info
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (Parametros.Valor_Com_Desconto > 0)
                            {
                                Parametros.descontostemp.Add(new Cliente_Desconto
                                {
                                    Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                    Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                    Desconto = Parametros.Desconto,
                                    Cliente = Parametros.objBanco,
                                });
                            }
                            else if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }

                            if (Valor_Com_Desconto_Quem_Indicou > 0)
                            {
                                Parametros.descontostemp.Add(new Cliente_Desconto
                                {
                                    Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                    Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                    Desconto = Parametros.Desconto,
                                    Cliente = Parametros.objBanco.Quem_Indicou,
                                });
                            }
                            else if (Parametros.novaCondicao)
                            {
                                Parametros.retorno.Add(new PostApiMensagem
                                {
                                    Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                    Tipo = PostApiMensagemType.Info
                                });
                            }
                        }
                    }
                    else
                    {
                        if (Parametros.Valor_Com_Desconto > 0)
                        {
                            Parametros.descontostemp.Add(new Cliente_Desconto
                            {
                                Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                Desconto = Parametros.Desconto,
                                Cliente = Parametros.objBanco,
                            });
                        }
                        else if (Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }

                        if (Valor_Com_Desconto_Quem_Indicou > 0)
                        {
                            Parametros.descontostemp.Add(new Cliente_Desconto
                            {
                                Valor_Desconto = Parametros.Desconto.Valor_Desconto ?? 0,
                                Percentual_Desconto = Parametros.Desconto.Percentual_Desconto ?? 0,
                                Desconto = Parametros.Desconto,
                                Cliente = Parametros.objBanco.Quem_Indicou
                            });
                        }
                        else if (Parametros.novaCondicao)
                        {
                            Parametros.retorno.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.DESCONTO_MAIOR_QUE_VALOR_MEIO_CAPTACAO, Parametros.objBanco.Quem_Indicou?.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }
                }
            }
            #endregion
        }

        private bool DescontoValidaRegiao(string pais, string estado, string cidade, Desconto desconto)
        {

            if (desconto.Regioes.Count == 0)
            {
                return true;
            }
            else
            {
                bool enderecoEncontrado = desconto.Regioes.Where(p => (p.Cidade == cidade || p.Cidade == RsBase.TODOS)
                                                                        && (p.Pais == pais || p.Pais == RsBase.TODOS)
                                                                        && (p.Estado == estado || p.Estado == RsBase.TODOS)).Count() > 0 ? true : false;
                return enderecoEncontrado;
            }

        }
        private static double ValorComDesconto_MeioCaptacao(Desconto desconto, double valorPlanoProduto)
        {
            double somaDescontosPercentuais = 0;
            double valorComDesconto = valorPlanoProduto;

            double valordesconto;

            if (desconto != null)
            {
                if (desconto.Percentual_Desconto.HasValue)
                {
                    valordesconto = (desconto.Percentual_Desconto.Value) / 100;
                    somaDescontosPercentuais += valorPlanoProduto * valordesconto;
                }
                else
                {
                    valorComDesconto -= desconto.Valor_Desconto.Value;
                }
            }


            valorComDesconto -= somaDescontosPercentuais;

            return valorComDesconto;
        }

        [HttpGet()]
        [Route("api/clienteapi/FindById")]
        public async Task<GetApiRetorno<ClienteDadosGeraisViewModel>> FindById(Int64 Id)
        {
            GetApiRetorno<ClienteDadosGeraisViewModel> retorno = new GetApiRetorno<ClienteDadosGeraisViewModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repoCliente = new Conexao<Cliente>(session))
                {
                    try
                    {
                        //var cliente = await repoCliente.selectFirst(p => p.Id == Id);
                        //retorno.Objeto = await GetViewObj(session, cliente);

                        retorno.Objeto = await GetViewObj(session, await repoCliente.selectFirst(p => p.Id == Id));

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
        [Route("api/clienteapi/FindByIdClientePlanoProduto")]
        public async Task<GetApiRetorno<PlanoProdutoListModel>> FindByIdClientePlanoProduto(Int64 Id)
        {
            GetApiRetorno<PlanoProdutoListModel> retorno = new GetApiRetorno<PlanoProdutoListModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repo = new Conexao<Cliente_PlanoProduto>(session))
                {
                    try
                    {
                        retorno.Objeto = await GetViewObjClientePlanoProduto(await repo.selectFirst(p => p.Id == Id), session);

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

        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/clienteapi/GravarFormaPagamento")]
        public async Task<PostApiRetorno<Int64>> GravarFormaPagamento([FromBody] ParamWebApi<ClienteDadosGeraisViewModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            Int32 count = !String.IsNullOrEmpty(value.Objeto.CpfCnpj) ? Existecpf(value.Objeto.CpfCnpj).Result : 0;

            try
            {

                Cliente modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                using (var repoFormaPagamentoCliente = new Conexao<Forma_Pagamento_Cliente>(session))
                using (var repository = new Conexao<Cliente>(session))
                using (var repoPais = new Conexao<Pais>(session))
                using (var repoTipoImovel = new Conexao<Tipo_Imovel>(session))
                using (var repoBanco = new Conexao<Banco>(session))
                using (var repoFormaPagamento = new Conexao<Forma_Pagamento>(session))
                using (var repoVenvimento = new Conexao<Vencimento>(session))
                {

                    if (value.Objeto.Id != 0)
                        modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                    List<Forma_Pagamento_Cliente> forma_pagamento_clientetemp = modelo.Formas_Pagamento_Cliente.ToList();

                    foreach (var forma_pagamento_clienteview in value.Objeto.Formas_Pagamento.FindAll(p => p.Id == 0))
                    {
                        // ****************************************
                        // Endereço Cobranca
                        // ****************************************
                        Endereco_Cliente enderecoCobranca = new Endereco_Cliente();

                        if (forma_pagamento_clienteview.End_Cobranca_Igual_Principal)
                        {
                            enderecoCobranca.Pais = modelo.Endereco_Principal?.Pais;
                            enderecoCobranca.Tipo_Imovel = modelo.Endereco_Principal?.Tipo_Imovel;
                            enderecoCobranca.Id_Bairro = modelo.Endereco_Principal?.Id_Bairro;
                            enderecoCobranca.Id_Cidade = modelo.Endereco_Principal?.Id_Cidade;
                            enderecoCobranca.Bairro = modelo.Endereco_Principal?.Bairro;
                            enderecoCobranca.Cep = modelo.Endereco_Principal?.Cep;
                            enderecoCobranca.Cidade = modelo.Endereco_Principal?.Cidade;
                            enderecoCobranca.Complemento = modelo.Endereco_Principal?.Complemento;
                            enderecoCobranca.Latitude = modelo.Endereco_Principal?.Latitude;
                            enderecoCobranca.Longitude = modelo.Endereco_Principal?.Longitude;
                            enderecoCobranca.Logradouro = modelo.Endereco_Principal?.Logradouro;
                            enderecoCobranca.Numero = modelo.Endereco_Principal?.Numero;
                            enderecoCobranca.Ponto_Referencia = modelo.Endereco_Principal?.Ponto_Referencia;
                            enderecoCobranca.UF = modelo.Endereco_Principal?.UF;
                            enderecoCobranca.Busca_Endereco = value.Objeto.EnderecoPrincipal.Busca_Endereco != null ? value.Objeto.EnderecoPrincipal.Busca_Endereco : null;
                            enderecoCobranca.Endereco_Logradouro = modelo.Endereco_Principal?.Endereco_Logradouro;
                            enderecoCobranca.Condominio_Predio = modelo.Endereco_Principal?.Condominio_Predio;
                            enderecoCobranca.Cod_Escritorio = value.Objeto.EnderecoPrincipal?.Cod_Escritorio;
                            enderecoCobranca.Cod_Localidade = Convert.ToInt32(value.Objeto.EnderecoPrincipal?.Cod_Localidade);
                            enderecoCobranca.Cod_Area = value.Objeto.EnderecoPrincipal?.Cod_Area;
                        }
                        else
                        {
                            enderecoCobranca.Pais = forma_pagamento_clienteview.Id_PaisCobranca != null ? repoPais.selectFirst(p => p.Id == forma_pagamento_clienteview.Id_PaisCobranca.Value).Result : null;
                            enderecoCobranca.Tipo_Imovel = forma_pagamento_clienteview.Id_Tipo_ImovelCobranca != null ? repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == forma_pagamento_clienteview.Id_Tipo_ImovelCobranca.Value).Result : null;
                            enderecoCobranca.Id_Bairro = forma_pagamento_clienteview?.Id_BairroCobranca;
                            enderecoCobranca.Id_Cidade = forma_pagamento_clienteview?.Id_CidadeCobranca;
                            enderecoCobranca.Bairro = forma_pagamento_clienteview?.BairroCobranca;
                            enderecoCobranca.Cep = forma_pagamento_clienteview?.CEPCobranca;
                            enderecoCobranca.Cidade = forma_pagamento_clienteview?.CidadeCobranca;
                            enderecoCobranca.Complemento = forma_pagamento_clienteview?.ComplementoCobranca;
                            enderecoCobranca.Latitude = forma_pagamento_clienteview?.LatitudeCobranca != null ? Convert.ToDouble(forma_pagamento_clienteview?.LatitudeCobranca?.Replace('.', ',')) : (Double?)null;
                            enderecoCobranca.Longitude = forma_pagamento_clienteview?.LongitudeCobranca != null ? Convert.ToDouble(forma_pagamento_clienteview?.LongitudeCobranca?.Replace('.', ',')) : (Double?)null;
                            enderecoCobranca.Logradouro = forma_pagamento_clienteview?.LogradouroCobranca;
                            enderecoCobranca.Numero = forma_pagamento_clienteview?.NumeroCobranca;
                            enderecoCobranca.Ponto_Referencia = forma_pagamento_clienteview?.Ponto_ReferenciaCobranca;
                            enderecoCobranca.UF = forma_pagamento_clienteview?.UFCobranca;
                            enderecoCobranca.Cod_Escritorio = value.Objeto.EnderecoPrincipal?.Cod_Escritorio;
                            enderecoCobranca.Cod_Localidade = Convert.ToInt32(value.Objeto.EnderecoPrincipal?.Cod_Localidade);
                            enderecoCobranca.Cod_Area = value.Objeto.EnderecoPrincipal?.Cod_Area;
                        }

                        // ************************************************
                        // Formas de Pagamento do Cliente
                        // ************************************************
                        var forma_pagamento_cliente = new Forma_Pagamento_Cliente
                        {
                            Cliente = modelo,
                            Agencia = forma_pagamento_clienteview.Agencia,
                            Banco = forma_pagamento_clienteview.Id_Banco.HasValue ? repoBanco.selectFirst(p => p.Id == forma_pagamento_clienteview.Id_Banco.Value).Result : null,
                            Conta_Corrente = forma_pagamento_clienteview.Conta_Corrente,
                            Dia_Vencimento = forma_pagamento_clienteview.Dia_Vencimento,
                            Digito = forma_pagamento_clienteview.Digito,
                            Forma_Envio = forma_pagamento_clienteview.Forma_Envio,
                            Forma_Pagamento = forma_pagamento_clienteview.Id_Forma_Pagamento.HasValue ? repoFormaPagamento.selectFirst(p => p.Id == forma_pagamento_clienteview.Id_Forma_Pagamento.Value).Result : null,
                            Vencimento = forma_pagamento_clienteview.Id_Vencimento.HasValue ? repoVenvimento.selectFirst(p => p.Id == forma_pagamento_clienteview.Id_Vencimento.Value).Result : null,
                            Endereco_Cobranca = enderecoCobranca,
                            End_Cobranca_Igual_Principal = forma_pagamento_clienteview.End_Cobranca_Igual_Principal,
                            Status = forma_pagamento_clienteview.Status.HasValue ? forma_pagamento_clienteview.Status.Value : Status_Default.Ativo
                        };

                        forma_pagamento_clientetemp.Add(forma_pagamento_cliente);
                    }

                    modelo.Formas_Pagamento_Cliente = forma_pagamento_clientetemp;

                    //retorno.Mensagens.AddRange(BindViewToObj(session, modelo, value.Objeto, ref statusLog));

                    await repository.SaveOrUpdate(modelo);


                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;

                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
        [AcceptVerbs("Get", "Post")]
        [Route("api/clienteapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<ClienteDadosGeraisViewModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            Int32 count = !String.IsNullOrEmpty(value.Objeto.CpfCnpj) ? Existecpf(value.Objeto.CpfCnpj).Result : 0;

            try
            {
                if (count > 0 && value.Objeto.Id == 0)
                {
                    throw new Exception(RsCadastros.CPFCNPJ_CADASTRADO);
                }

                Cliente modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Cliente>(session))
                    using (var repositoryT5 = new Conexao<ClienteAniel>(sessionT5))
                    using (var repoMunicipio = new Conexao<MunicipioAniel>(sessionT5))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Cliente
                            {
                                Status = Status_Cliente.Ativo,
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = value.Usuario,
                                Tipo_Cliente = Tipo_Cliente.Cliente
                            };

                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Cliente.Ativo : Status_Cliente.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        List<Cliente_PlanoProduto_StatusLog> statusLog = new List<Cliente_PlanoProduto_StatusLog>();

                        retorno.Mensagens.AddRange(BindViewToObj(session, modelo, value.Objeto, ref statusLog));

                        ClienteAniel modeloT5 = await repositoryT5.selectFirst(p => p.Cod_Cliente == modelo.Id);
                        if (modeloT5 == null)
                        {
                            modeloT5 = new ClienteAniel
                            {
                                Cod_Cliente = Convert.ToInt32(modelo.Id),
                                NomeFantasia = modelo.NomeFantasia,
                                CPF = modelo.CpfCnpj,
                                Data_Cadastro = modelo.DataHoraCriacao,
                                Telefone_Fixo = modelo.Telefone_Fixo,
                                Telefone_Celular = modelo.Telefone_Celular,
                                Email_Principal = modelo.Email_Principal,
                                Tipo_Pessoa = modelo.Tipo_Pessoa,
                                Tipo_Cliente = (Tipo_ClienteAniel)modelo.Tipo_Cliente,
                                Status = (Status_Cliente)modelo.Status,
                                Status_Prospecto = (Status_ProspectoAniel?)modelo.Status_Prospecto,
                                Numero_Contrato = modelo.Numero_Contrato,
                                Cep = modelo.Endereco_Principal?.Cep,
                                Bairro = modelo.Endereco_Principal?.Bairro,
                                Numero = modelo.Endereco_Principal?.Numero,
                                Logradouro = modelo.Endereco_Principal?.Logradouro
                            };

                            if (modelo.Endereco_Principal.Cidade != null)
                                modeloT5.Cidade = Convert.ToInt16(repoMunicipio.selectFirst(p => p.Municipio.Upper() == modelo.Endereco_Principal.Cidade.Upper()).Result.CodMunicipio);

                            if (modelo.Categoria_Cliente != null)
                            {
                                modeloT5.Categoria_Cliente = new Categoria_ClienteAniel
                                {
                                    Id = modelo.Categoria_Cliente.Id,
                                    Descricao = modelo.Categoria_Cliente.Descricao,
                                    Tipo_Prioridade = (Tipo_PrioridadeT5?)modelo.Categoria_Cliente.Tipo_Prioridade,
                                    Status = (Status_Default?)modelo.Categoria_Cliente.Status
                                };
                            }

                            if (modelo.Tipo_Pessoa != null)
                            {
                                switch (modelo.Tipo_Pessoa)
                                {
                                    case Tipo_Pessoa.Fisica:
                                        modeloT5.Tipo = "F";
                                        break;

                                    case Tipo_Pessoa.Juridica:
                                        modeloT5.Tipo = "J";
                                        break;
                                }
                            }
                        }
                        else
                        {
                            modeloT5.NomeFantasia = modelo.NomeFantasia;
                            modeloT5.CPF = modelo.CpfCnpj;
                            modeloT5.Data_Cadastro = modelo.DataHoraCriacao;
                            modeloT5.Telefone_Fixo = modelo.Telefone_Fixo;
                            modeloT5.Telefone_Celular = modelo.Telefone_Celular;
                            modeloT5.Email_Principal = modelo.Email_Principal;
                            modeloT5.Tipo_Pessoa = modelo.Tipo_Pessoa;
                            modeloT5.Tipo_Cliente = (Tipo_ClienteAniel)modelo.Tipo_Cliente;
                            modeloT5.Status = (Status_Cliente)modelo.Status;
                            modeloT5.Status_Prospecto = (Status_ProspectoAniel?)modelo.Status_Prospecto;
                            modeloT5.Numero_Contrato = modelo.Numero_Contrato;
                            modeloT5.Cep = modelo.Endereco_Principal?.Cep;
                            modeloT5.Bairro = modelo.Endereco_Principal?.Bairro;
                            modeloT5.Numero = modelo.Endereco_Principal?.Numero;
                            modeloT5.Logradouro = modelo.Endereco_Principal?.Logradouro;

                            if (modelo.Endereco_Principal.Cidade != null)
                                modeloT5.Cidade = Convert.ToInt16(repoMunicipio.selectFirst(p => p.Municipio.Upper() == modelo.Endereco_Principal.Cidade.Upper()).Result.CodMunicipio);

                            if (modelo.Categoria_Cliente != null)
                            {
                                modeloT5.Categoria_Cliente.Id = modelo.Categoria_Cliente.Id;
                                modeloT5.Categoria_Cliente.Descricao = modelo.Categoria_Cliente.Descricao;
                                modeloT5.Categoria_Cliente.Tipo_Prioridade = (Tipo_PrioridadeT5?)modelo.Categoria_Cliente.Tipo_Prioridade;
                                modeloT5.Categoria_Cliente.Status = (Status_Default?)modelo.Categoria_Cliente.Status;
                            }

                            if (modelo.Tipo_Pessoa != null)
                            {
                                switch (modelo.Tipo_Pessoa)
                                {
                                    case Tipo_Pessoa.Fisica:
                                        modeloT5.Tipo = "F";
                                        break;
                                    case Tipo_Pessoa.Juridica:
                                        modeloT5.Tipo = "J";
                                        break;
                                }
                            }
                        }

                        await repository.SaveOrUpdate(modelo);
                        await repositoryT5.SaveOrUpdate(modeloT5);


                        using (var api = new VendaApiController())
                        {
                            if (value.Objeto.Id == 0)
                            {
                                await EnviarEmailAtivacaoOLA(modelo, null);
                                retorno.Mensagens.AddRange((await api.EnviarEmailContrato(session, modelo, null, null)).Mensagens);
                            }
                        }

                        if (statusLog.Count() > 0)
                        {
                            using (var repositoryLog = new Conexao<Cliente_PlanoProduto_StatusLog>(session))
                            {
                                await repositoryLog.Save(statusLog);
                            }
                        }
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;

                    try
                    {
                        using (var api = new ClienteAnielApiController())
                        {
                            var cliente_aniel = value;
                            cliente_aniel.Objeto.Id = cliente_aniel.Objeto.Id == 0 ? modelo.Id : cliente_aniel.Objeto.Id;
                            await api.Gravar(cliente_aniel);
                        }
                    }
                    catch (Exception ex)
                    {
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }

                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        //[HttpPost()]
        //public async Task<GetApiRetorno<IEnumerable<ClienteGetList>>> GetList()
        //{
        //    GetApiRetorno<IEnumerable<ClienteGetList>> retorno = new GetApiRetorno<IEnumerable<ClienteGetList>>();

        //    using (ISession session = HibernateUtil.GetSession())
        //    {
        //        try
        //        {
        //            var lista = await new Conexao<Cliente>(session).queryover().ListAsync();

        //            retorno.Objeto = lista.Select(p => new ClienteGetList
        //            {
        //                Id = p.Id,
        //                NomeFantasia = p.NomeFantasia
        //            });

        //            retorno.Ok = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            retorno.Mensagem = ex.Message;
        //            if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
        //        }
        //    }
        //    return retorno;

        //}

        [HttpGet()]
        [Route("api/clienteapi/EnviarEmailAtivacaoOLA")]
        public async Task<PostApiRetorno<Int64>> EnviarEmailAtivacaoOLA(Cliente cliente, Int64? IdCliente)
        {


            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                if (
                    String.IsNullOrEmpty(CONSTANTES.Email_De))
                {
                    retorno.Ok = true;
                    return retorno;
                }

                using (ISession session = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                {
                    String ArquivoEmail = $@"{CONSTANTES.CaminhoFisico}\Arquivos\email_ola.html";

                    var Provedor = (await repoConfig.selectFirst()).Nome_Cliente_Sinapse;
                    if (cliente == null)
                        cliente = (await repoCliente.selectFirst(p => p.Id == IdCliente));

                    EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);

                    using (StreamReader stream = File.OpenText(ArquivoEmail))
                    {

                        String texto = stream.ReadToEnd();
                        String textoAlterado = texto.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia).Replace("{{NOME_PROVEDOR}}", Provedor).Replace("{{CPFCNPJ}}", cliente.CpfCnpj);

                        emailService.EnviarMensagem(cliente.Email_Principal, "Instruções do OLA", textoAlterado, CONSTANTES.Email_UsaSsl == "S");
                    }

                    retorno.Ok = true;
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
        [Route("api/clienteapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_CLIENTE SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        #region Endereco
        [HttpGet()]
        [Route("api/clienteapi/FindByIdEndereco")]
        public async Task<GetApiRetorno<AlterarEnderecoClienteViewModel>> FindByIdEndereco(Int64 Id, Int64 IdCliente, Int64 IdClientePlanoProduto)
        {
            GetApiRetorno<AlterarEnderecoClienteViewModel> retorno = new GetApiRetorno<AlterarEnderecoClienteViewModel>
            {
                Objeto = new AlterarEnderecoClienteViewModel
                {
                    EnderecoInstalacao = new EnderecoClienteViewModel()
                }
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    #region [parametro]
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@ID_CLIENTE", Convert.ToInt64(IdCliente) },
                        { "@ID", Convert.ToInt64(IdClientePlanoProduto) }
                    };
                    #endregion

                    #region [sqlFrom]
                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CLIENTE_PLANO_PRODUTO A ")
                                                    .AppendLine("INNER JOIN TB_CLIENTE C ON A.ID_CLIENTE = C.ID ")
                                                    .AppendLine("LEFT JOIN TB_MEIO_CAPTACAO D ON C.ID_MEIO_CAPTACAO = D.ID ")
                                                    .AppendLine("INNER JOIN TB_ENDERECO_CLIENTE B ON A.ID_ENDERECO_INSTALACAO = B.ID ")
                                                    .AppendLine("LEFT JOIN TB_ENDERECO_LOGRADOURO F ON(B.ID_ENDERECO_LOGRADOURO = F.ID) ")
                                                    .AppendLine("LEFT JOIN TB_PAIS P ON(F.ID_PAIS = P.ID) ")
                                                    .AppendLine("LEFT JOIN TB_UF UF ON(F.ID_UF = UF.ID) ")
                                                    .AppendLine("LEFT JOIN TB_CIDADE CI ON(F.ID_CIDADE = CI.ID) ")
                                                    .AppendLine("LEFT JOIN TB_BAIRRO BA ON(F.ID_BAIRRO = BA.ID) ")
                                                    .AppendLine("LEFT JOIN TB_TIPO_IMOVEL E ON B.ID_TIPO_IMOVEL = E.ID ")
                                                    .AppendLine("WHERE A.ID_CLIENTE = @ID_CLIENTE AND A.ID = @ID ")
                                                    //.AppendLine("AND A.ID_VENDA IS NOT NULL AND A.ID NOT IN (SELECT COALESCE(ID_TROCA_PLANO, 0) FROM TB_CLIENTE_PLANO_PRODUTO) ")
                                                    .AppendLine("AND A.STATUS NOT IN ('7', '6')");
                    #endregion

                    #region [select]
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT C.NUMERO_CONTRATO, C.TIPO_CLIENTE, C.CARGO_RESPONSAVEL, C.CPF_RESPONSAVEL, C.NOME_RESP_FINANCEIRO, C.INSCRICAO_ESTADUAL, C.INSCRICAO_MUNICIPAL, C.PORTE, ");
                        sql.AppendLine("C.DATA_ABERTURA_EMPRESA, C.NOMEFANTASIA, C.RAMO_ATIVIDADE, C.RAZAO_SOCIAL, C.ID ID_CLIENTE, C.CPF, C.NOMEFANTASIA, C.DATA_NASC, C.NACIONALIDADE, C.SEXO, C.ESTADO_CIVIL, ");
                        sql.AppendLine("C.RG, C.DATA_EMISSAO_RG, C.ORGAO_EMISSAO_RG, C.GRAU_INSTRUCAO, C.PROFISSAO, C.FILIACAO_PAI, C.FILIACAO_MAE, C.OUTRO_MEIO, C.TIPOPESSOA, C.ID_ORGANIZACAO, ");
                        sql.AppendLine("C.EMAIL_PRINCIPAL, C.TELEFONE_FIXO, C.TELEFONE_RECADO, C.PESSOA_TEL_RECADO, C.TELEFONE_CELULAR, C.ID_QUEM_INDICOU, C.ID_CATEGORIA, C.ID_MEIO_CAPTACAO, D.DESCRICAO, ");
                        sql.AppendLine("C.ID_QUEM_INDICOU, C.NUMERO_VALIDADO, B.ID_PAIS, B.ID_TIPO_IMOVEL, E.COD_TIPO_IMOVEL, B.ID_BAIRRO, COALESCE(B.ENDERECO_BAIRRO, BA.DESCRICAO) ENDERECO_BAIRRO, ");
                        sql.AppendLine("COALESCE(B.ENDERECO_CEP, F.CEP) ENDERECO_CEP, B.ID_CIDADE, COALESCE(B.ENDERECO_CIDADE, CI.DESCRICAO) ENDERECO_CIDADE, B.ENDERECO_COMPLEMENTO, ");
                        sql.AppendLine("B.ID ID_ENDERECO, B.ENDERECO_LATITUDE, B.ENDERECO_LONGITUDE, B.ID_ENDERECO_LOGRADOURO, COALESCE(B.ENDERECO_LOGRADOURO, F.DESCRICAO) ENDERECO_LOGRADOURO, ");
                        sql.AppendLine("B.ENDERECO_NUMERO , B.ENDERECO_PONTO_REFERENCIA, COALESCE(B.ENDERECO_UF, UF.DESCRICAO) ENDERECO_UF, B.COD_AREA, B.COD_EESCRITORIO, B.COD_LOCALIDADE");
                        sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Status_Cliente>("C.STATUS", "STATUS_CLIENTE"));
                        sql.AppendLine(sqlFrom.ToString());
                        retorno.Objeto.EnderecoInstalacao.Tipo = Tipo_EnderecoCliente.Instalacao;
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.EnderecoInstalacao.Id = reader["ID_ENDERECO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ENDERECO"]) : (long)0;
                                retorno.Objeto.EnderecoInstalacao.Id_Pais = reader["ID_PAIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_PAIS"]) : (long?)null;
                                retorno.Objeto.EnderecoInstalacao.Id_Tipo_Imovel = reader["ID_TIPO_IMOVEL"] != DBNull.Value ? Convert.ToInt32(reader["ID_TIPO_IMOVEL"]) : (int?)null;
                                retorno.Objeto.EnderecoInstalacao.Id_Bairro = reader["ID_BAIRRO"] != DBNull.Value ? Convert.ToInt64(reader["ID_BAIRRO"]) : (long?)null;
                                retorno.Objeto.EnderecoInstalacao.Bairro = reader["ENDERECO_BAIRRO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_BAIRRO"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.CEP = reader["ENDERECO_CEP"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_CEP"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Id_Cidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : (long?)null;
                                retorno.Objeto.EnderecoInstalacao.Cidade = reader["ENDERECO_CIDADE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_CIDADE"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Complemento = reader["ENDERECO_COMPLEMENTO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_COMPLEMENTO"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Id_Endereco = reader["ID_ENDERECO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ENDERECO"]) : (long)0;
                                retorno.Objeto.EnderecoInstalacao.Latitude = reader["ENDERECO_LATITUDE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_LATITUDE"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Logradouro = reader["ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_LOGRADOURO"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Id_Endereco_Logradouro = reader["ID_ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ENDERECO_LOGRADOURO"]) : (long?)null;
                                retorno.Objeto.EnderecoInstalacao.Longitude = reader["ENDERECO_LONGITUDE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_LONGITUDE"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Numero = reader["ENDERECO_NUMERO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_NUMERO"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Ponto_Referencia = reader["ENDERECO_PONTO_REFERENCIA"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_PONTO_REFERENCIA"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.UF = reader["ENDERECO_UF"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_UF"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Cod_Area = reader["COD_AREA"] != DBNull.Value ? Convert.ToString(reader["COD_AREA"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["COD_LOCALIDADE"]) : string.Empty;
                                retorno.Objeto.EnderecoInstalacao.Cod_Escritorio = reader["COD_EESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["COD_EESCRITORIO"]) : string.Empty;
                            }
                            retorno.Objeto.EnderecoInstalacao.Tipo = Tipo_EnderecoCliente.Instalacao;
                        }
                    }
                    #endregion

                    /*      Alteração Viabilidade       */

                    retorno.Objeto.Mapa = new BuscarGeolocalizacaoModels { OrigemDialog = true };
                    var configBusiness = new ConfiguracoesGeraisBusiness(new ConfiguracoesGeraisRepository(session));
                    IEnumerable<EnumEtapaVenda> consultasHabilitadas = configBusiness.GetConsultasRestricaoHabilitadasLead();
                    var config = configBusiness.GetConfiguracoesGerais();

                    if (consultasHabilitadas.Count() > 0)
                    {
                        retorno.Objeto.ConsultaViabilidadeHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaViabilidade);

                        retorno.Objeto.ConsultaPendenciaFinanceiraHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaPendenciaFinanceira);

                        retorno.Objeto.ConsultaPendenciaEnderecoHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaPendenciaEndereco);

                        retorno.Objeto.ConsultaCreditoHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaCredito);
                    }

                    retorno.Objeto.ConsultaPendenciaViabilidadeValidade = config.ConsultaViabilidadeTempoValidade != null ? config.ConsultaViabilidadeTempoValidade : null;
                    retorno.Objeto.ConsultaPendenciaEnderecoValidade = config.ConsultaPendenciaEnderecoTempoValidade != null ? config.ConsultaPendenciaEnderecoTempoValidade : null;

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    retorno.Ok = false;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        [HttpGet()]
        [Route("api/clienteapi/FindByIdEndereco_ComAtendimento")]
        public async Task<GetApiRetorno<AlterarEnderecoClienteViewModel>> FindByIdEndereco_ComAtendimento(Int64 Id, String atendimento)
        {
            GetApiRetorno<AlterarEnderecoClienteViewModel> retorno = new GetApiRetorno<AlterarEnderecoClienteViewModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoEndereco = new Conexao<Endereco_Cliente>(session))
            using (var repoEndereco_Logradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            {
                try
                {
                    retorno.Objeto = new AlterarEnderecoClienteViewModel
                    {
                        EnderecoInstalacao = GetViewObj_Endereco_Com_Atendimento(await repoEndereco.selectFirst(p => p.Id == Id), atendimento)
                    };


                    var AtendimentoObject = await repoAtendimento.selectFirst(p => p.Id == Convert.ToInt64(atendimento));

                    if (retorno.Objeto.EnderecoInstalacao.Busca_Endereco == Busca_Endereco.Endereco_Logradouro)
                    {
                        var logradouro = await repoEndereco_Logradouro.selectFirst(p => p.Id == retorno.Objeto.EnderecoInstalacao.Id_Endereco_Logradouro);
                        retorno.Objeto.EnderecoInstalacao.CEP = logradouro.Cep;
                        retorno.Objeto.EnderecoInstalacao.Cod_Area = logradouro.Cod_Area;
                        retorno.Objeto.EnderecoInstalacao.Cod_Escritorio = logradouro.Cod_Escritorio;
                        retorno.Objeto.EnderecoInstalacao.Cod_Localidade = logradouro.Cod_Localidade;
                    }
                    else
                    {
                        retorno.Objeto.EnderecoInstalacao.Cod_Area = AtendimentoObject?.PlanoProduto?.Cod_Area;
                        retorno.Objeto.EnderecoInstalacao.Cod_Localidade = Convert.ToString(AtendimentoObject?.PlanoProduto?.Cod_Localidade);
                        retorno.Objeto.EnderecoInstalacao.Cod_Escritorio = AtendimentoObject?.PlanoProduto?.Cod_Escritorio;
                    }

                    /*      Alteração Viabilidade       */

                    retorno.Objeto.Mapa = new BuscarGeolocalizacaoModels { OrigemDialog = true };
                    var configBusiness = new ConfiguracoesGeraisBusiness(new ConfiguracoesGeraisRepository(session));
                    IEnumerable<EnumEtapaVenda> consultasHabilitadas = configBusiness.GetConsultasRestricaoHabilitadasLead();
                    var config = configBusiness.GetConfiguracoesGerais();

                    if (consultasHabilitadas.Count() > 0)
                    {
                        retorno.Objeto.ConsultaViabilidadeHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaViabilidade);

                        retorno.Objeto.ConsultaPendenciaFinanceiraHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaPendenciaFinanceira);

                        retorno.Objeto.ConsultaPendenciaEnderecoHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaPendenciaEndereco);

                        retorno.Objeto.ConsultaCreditoHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaCredito);
                    }

                    retorno.Objeto.ConsultaPendenciaViabilidadeValidade = config.ConsultaViabilidadeTempoValidade != null ? config.ConsultaViabilidadeTempoValidade : null;
                    retorno.Objeto.ConsultaPendenciaEnderecoValidade = config.ConsultaPendenciaEnderecoTempoValidade != null ? config.ConsultaPendenciaEnderecoTempoValidade : null;


                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/GravarEndereco")]
        public async Task<PostApiRetorno<Int64>> GravarEndereco([FromBody] ParamWebApi<EnderecoClienteViewModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repositoryPlano = new Conexao<Cliente_PlanoProduto>(session))
                    using (var repository = new Conexao<Endereco_Cliente>(session))
                    using (var repositoryCliente = new Conexao<Cliente>(session))
                    {
                        var modelo = new Endereco_Cliente
                        {
                            UsuarioCriacao = value.Usuario,
                            Bairro = value.Objeto.Bairro,
                            Cep = value.Objeto.CEP,
                            Cidade = value.Objeto.Cidade,
                            Complemento = value.Objeto.Complemento,
                            Id_Bairro = value.Objeto.Id_Bairro,
                            Id_Cidade = value.Objeto.Id_Cidade,
                            Id_UF = value.Objeto.Id_UF,
                            Latitude = value.Objeto.Latitude != null ? Convert.ToDouble(value.Objeto.Latitude) : (Double?)null,
                            Longitude = value.Objeto.Longitude != null ? Convert.ToDouble(value.Objeto.Longitude) : (Double?)null,
                            Logradouro = value.Objeto.Logradouro,
                            Numero = value.Objeto.Numero,
                            Pais = await new Conexao<Pais>(session).selectFirst(p => p.Id == value.Objeto.Id_Pais),
                            Tipo_Imovel = await new Conexao<Tipo_Imovel>(session).selectFirst(p => p.Id == value.Objeto.Id_Tipo_Imovel),
                            Ponto_Referencia = value.Objeto.Ponto_Referencia,
                            UF = value.Objeto.UF,
                            Cod_Area = value.Objeto.Cod_Area,
                            Cod_Escritorio = value.Objeto.Cod_Escritorio,
                            Cod_Localidade = value.Objeto.Cod_Localidade != null ? Convert.ToInt32(value.Objeto.Cod_Localidade) : (Int32?)null,
                            Localidade = !String.IsNullOrEmpty(value.Objeto.Localidade) ? value.Objeto.Localidade : "",
                            Busca_Endereco = null
                        };

                        await repository.SaveOrUpdate(modelo);
                        var plano_produto = await repositoryPlano.selectFirst(p => p.Id == value.Objeto.Id_ClientePlanoProduto);

                        if (plano_produto != null)
                        {
                            var IdInstalacao = plano_produto.Endereco_Instalacao.Id;
                            if (plano_produto.Plano.OSTrocaEndereco)
                            {
                                plano_produto.Endereco_Instalacao = await repository.selectFirst(p => p.Id == Convert.ToInt32(IdInstalacao));
                                plano_produto.Endereco_Mudanca_Cliente = modelo;
                            }
                            else
                            {
                                plano_produto.Endereco_Instalacao = modelo;
                                plano_produto.Endereco_Mudanca_Cliente = null;
                            }

                            plano_produto.End_Instalacao_Igual_Principal = false;

                            await repositoryPlano.SaveOrUpdate(plano_produto);
                        }

                        retorno.IdObjeto = modelo.Id;
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




        /// <summary>
        /// Bind de Endereço
        /// </summary>
        /// <param name="session"></param>
        /// <param name="objBanco"></param>
        /// <param name="view"></param>
        private void BindViewToObj(ISession session, Endereco_Cliente objBanco, EnderecoClienteViewModel view)
        {
            using (var repoTipo_Imovel = new Conexao<Tipo_Imovel>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoCondominioPredio = new Conexao<Condominio_Predio>(session))
            {
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
                objBanco.Tipo_Imovel = view.Id_Tipo_Imovel.HasValue ? repoTipo_Imovel.selectFirst(p => p.Cod_Tipo_Imovel == view.Id_Tipo_Imovel.Value).Result : null;
                objBanco.Pais = view.Id_Pais.HasValue ? repoPais.selectFirst(p => p.Id == view.Id_Pais.Value).Result : null;

                objBanco.Cod_Area = view.Cod_Area;
                objBanco.Cod_Escritorio = view.Cod_Escritorio;
                objBanco.Cod_Localidade = !String.IsNullOrEmpty(view.Cod_Localidade) ? Convert.ToInt32(view.Cod_Localidade) : (Int32?)null;
                objBanco.Localidade = view.Localidade;

                objBanco.Busca_Endereco = view.Busca_Endereco != null ? view.Busca_Endereco : null;
                objBanco.Condominio_Predio = view.Busca_Endereco == Busca_Endereco.Condominio_Predio ? repoCondominioPredio.selectFirst(p => p.Id == view.Id_Condominio_Predio).Result : null;
                objBanco.Endereco_Logradouro = view.Busca_Endereco == Busca_Endereco.Endereco_Logradouro ? repoEnderecoLogradouro.selectFirst(p => p.Id == view.Id_Endereco_Logradouro).Result : null;

            }
        }

        private EnderecoClienteViewModel GetViewObj(Endereco_Cliente obj)
        {
            return new EnderecoClienteViewModel
            {
                Bairro = !String.IsNullOrEmpty(obj.Bairro) ? obj.Bairro : "",
                CEP = !String.IsNullOrEmpty(obj.Cep) ? obj.Cep : "",
                Cidade = !String.IsNullOrEmpty(obj.Cidade) ? obj.Cidade : "",
                Complemento = !String.IsNullOrEmpty(obj.Complemento) ? obj.Complemento : "",
                Id_Bairro = obj?.Id_Bairro,
                Id_Cidade = obj?.Id_Cidade,
                Id_Endereco = obj.Id,
                Latitude = !String.IsNullOrEmpty(Convert.ToString(obj.Latitude)) ? Convert.ToString(obj.Latitude) : "",
                Logradouro = !String.IsNullOrEmpty(Convert.ToString(obj.Logradouro)) ? Convert.ToString(obj.Logradouro) : "",
                Longitude = !String.IsNullOrEmpty(Convert.ToString(obj.Longitude)) ? Convert.ToString(obj.Longitude) : "",
                Numero = !String.IsNullOrEmpty(Convert.ToString(obj.Numero)) ? Convert.ToString(obj.Numero) : "",
                Ponto_Referencia = !String.IsNullOrEmpty(obj.Ponto_Referencia) ? obj.Ponto_Referencia : "",
                Tipo = Tipo_EnderecoCliente.Instalacao,
                UF = obj.UF,
                Id_Pais = obj?.Pais?.Id,
                Id_Tipo_Imovel = obj?.Tipo_Imovel?.Cod_Tipo_Imovel,
                Busca_Endereco = obj.Busca_Endereco != null ? obj.Busca_Endereco : null,
                Id_Endereco_Logradouro = obj?.Endereco_Logradouro?.Id,
                Id_Condominio_Predio = obj?.Condominio_Predio?.Id
            };
        }

        private EnderecoClienteViewModel GetViewObj_Endereco_Com_Atendimento(Endereco_Cliente obj, String atendimento)
        {
            using (var apiAtendimento = new AtendimentoApiController())
            using (var apiPlanoProduto = new PlanoProdutoApiController())
            {
                //int id_atendiemnto = Convert.ToInt32(atendimento);
                //GetApiRetorno<AtendimentoCadastroModel> atd = apiAtendimento.FindById(id_atendiemnto).Result;
                //GetApiRetorno<Cliente_Plano_ProdutoViewModel> plano_produto = apiPlanoProduto.FindById(atd.Objeto.Id_Cliente_Plano_Produto.Value).Result;
                return new EnderecoClienteViewModel
                {
                    Bairro = !String.IsNullOrEmpty(obj.Bairro) ? obj.Bairro : "",
                    CEP = !String.IsNullOrEmpty(obj.Cep) ? obj.Cep : "",
                    Cidade = !String.IsNullOrEmpty(obj.Cidade) ? obj.Cidade : "",
                    Complemento = !String.IsNullOrEmpty(obj.Complemento) ? obj.Complemento : "",
                    Id_Bairro = obj?.Id_Bairro,
                    Id_Cidade = obj?.Id_Cidade,
                    Id_Endereco = obj.Id,
                    Latitude = !String.IsNullOrEmpty(Convert.ToString(obj.Latitude)) ? Convert.ToString(obj.Latitude) : "",
                    Logradouro = !String.IsNullOrEmpty(Convert.ToString(obj.Logradouro)) ? Convert.ToString(obj.Logradouro) : "",
                    Longitude = !String.IsNullOrEmpty(Convert.ToString(obj.Longitude)) ? Convert.ToString(obj.Longitude) : "",
                    Numero = !String.IsNullOrEmpty(Convert.ToString(obj.Numero)) ? Convert.ToString(obj.Numero) : "",
                    Ponto_Referencia = !String.IsNullOrEmpty(obj.Ponto_Referencia) ? obj.Ponto_Referencia : "",
                    Tipo = Tipo_EnderecoCliente.Instalacao,
                    UF = obj.UF,
                    Id_Pais = obj?.Pais?.Id,
                    Id_Tipo_Imovel = obj?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Busca_Endereco = obj.Busca_Endereco != null ? obj.Busca_Endereco : null,
                    Id_Endereco_Logradouro = obj?.Endereco_Logradouro?.Id,
                    Id_Condominio_Predio = obj?.Condominio_Predio?.Id
                    //Cod_Escritorio = obj.Atendimento.PlanoProduto.Cod_Escritorio,
                    //Cod_Localidade = Convert.ToString(obj.Atendimento.PlanoProduto.Cod_Localidade),
                    //Cod_Area = obj.Atendimento.PlanoProduto.Cod_Area
                    //Cod_Area = plano_produto.Objeto.Cod_Area,
                    //Cod_Escritorio = plano_produto.Objeto.Cod_Escritorio,
                    //Cod_Localidade = Convert.ToString(plano_produto.Objeto.Cod_Localidade)
                    //Cod_Area = plano_produto.Objeto.Cod_Area,
                    //Cod_Escritorio = plano_produto.Objeto.Endereco_Instalacao.Cod_Escritorio,
                    //Cod_Localidade = Convert.ToString(plano_produto.Objeto.Endereco_Instalacao.Cod_Localidade),
                    //Localidade = plano_produto.Objeto.Endereco_Instalacao.Localidade,
                };
            }
        }



        #endregion
        #region Anexos
        [HttpGet()]
        [Route("api/clienteapi/GetAnexos")]
        public async Task<GetApiRetorno<List<ClienteAnexoModelView>>> GetAnexos(Int64 Id_Cliente)
        {
            GetApiRetorno<List<ClienteAnexoModelView>> retorno = new GetApiRetorno<List<ClienteAnexoModelView>>
            {
                Objeto = new List<ClienteAnexoModelView>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@ID_CLIENTE", Convert.ToInt64(Id_Cliente) }
                    };

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.TITULO, A.NOME_ARQUIVO, A.EXTENSAO_ARQUIVO, A.ID_CLIENTE, A.FOTO_PERFIL, A.IS_ANEXO_CTT_ADITIVO, ");
                        sql.AppendLine("A.DATAHORACRIACAO, B.NUMERO_CONTRATO, B.USUARIOCRIACAO, B.STATUS_ACEITE_CONTRATO, B.ID as IDCONTRATO, B.ID_MODELO_CONTRATO ");
                        sql.AppendLine("FROM TB_CLIENTE_ANEXO A");
                        sql.AppendLine("LEFT JOIN TB_CLIENTE_CONTRATO B ON (B.ID_CLIENTE_ANEXO = A.ID) ");
                        sql.AppendLine("WHERE A.ID_CLIENTE = @ID_CLIENTE");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                Status_Aceite_Contrato? status = null;
                                status = reader["STATUS_ACEITE_CONTRATO"] != DBNull.Value ? (Status_Aceite_Contrato)Convert.ToInt64(reader["STATUS_ACEITE_CONTRATO"]) : status;
                                var statusString = status != null ? HtmlEnumExtensions.GetEnumDescription<Status_Aceite_Contrato>(status.Value) : "";
                                var extensao = reader["EXTENSAO_ARQUIVO"] != DBNull.Value ? Convert.ToString(reader["EXTENSAO_ARQUIVO"]).Contains(".") ? Convert.ToString(reader["EXTENSAO_ARQUIVO"]).Replace(".", "") : Convert.ToString(reader["EXTENSAO_ARQUIVO"]) : String.Empty;

                                retorno.Objeto.Add(new ClienteAnexoModelView
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Nome_Arquivo = Convert.ToString(reader["NOME_ARQUIVO"]),
                                    Titulo = Convert.ToString(reader["TITULO"]),
                                    Extensao_Arquivo = extensao,
                                    //Extensao_Arquivo = Convert.ToString(reader["EXTENSAO_ARQUIVO"]),
                                    Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"]),
                                    Foto_Perfil = Convert.ToBoolean(reader["FOTO_PERFIL"]),
                                    caminho_Arquivo = $"{CONSTANTES.URLPortal}/Arquivos/Cliente/{reader["ID_CLIENTE"]}/{reader["ID"]}.{extensao}",
                                    //caminho_Arquivo = $"{CONSTANTES.URLPortal}/Arquivos/Cliente/{reader["ID_CLIENTE"]}/{reader["ID"]}.{reader["EXTENSAO_ARQUIVO"]}",
                                    NumeroContrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                    DataHora = Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm:ss"),
                                    Usuario = Convert.ToString(reader["USUARIOCRIACAO"]),
                                    AceiteContrato = status == Status_Aceite_Contrato.Pendente,
                                    StatusAceiteContrato = statusString,
                                    IdContrato = reader["IDCONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["IDCONTRATO"]) : (Int64?)null,
                                    ModeloSelecionadoId = reader["ID_MODELO_CONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["ID_MODELO_CONTRATO"]) : (Int64?)null,
                                    IsArquivoContratoAditivo = reader["IS_ANEXO_CTT_ADITIVO"] != DBNull.Value ? Convert.ToBoolean(reader["IS_ANEXO_CTT_ADITIVO"]) : (Boolean?)null,
                                });
                            }
                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/clienteapi/GetAnexosOla")]
        public async Task<GetApiRetorno<List<ClienteOlaAnexoModelView>>> GetAnexosOla(ClienteAnexoOlaRequest value)
        {
            GetApiRetorno<List<ClienteOlaAnexoModelView>> retorno = new GetApiRetorno<List<ClienteOlaAnexoModelView>>
            {
                Objeto = new List<ClienteOlaAnexoModelView>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    foreach (Int64 id in value.list_id_Contrato)
                    {
                        //Filtros
                        Dictionary<String, Object> parametros = new Dictionary<string, object>
                        {
                            { "@ID_CLIENTE", Convert.ToInt64(value.Id_Cliente) },
                            { "@ID_VENDA", Convert.ToInt64(value.Id_Venda) },
                            { "@ID_CLIENTE_CONTRATO", Convert.ToInt64(id) }
                        };

                        //Select
                        using (var com = session.Connection.CreateCommand())
                        {

                            StringBuilder sql = new StringBuilder();

                            sql.AppendLine("SELECT A.ID, A.TITULO, A.NOME_ARQUIVO, A.EXTENSAO_ARQUIVO, A.ID_CLIENTE, A.FOTO_PERFIL, A.IS_ANEXO_CTT_ADITIVO, ");
                            sql.AppendLine("A.DATAHORACRIACAO, B.NUMERO_CONTRATO, B.USUARIOCRIACAO, B.STATUS_ACEITE_CONTRATO, B.ID as IDCONTRATO, B.ID_MODELO_CONTRATO ");
                            sql.AppendLine("FROM TB_CLIENTE_ANEXO A ");
                            sql.AppendLine("LEFT JOIN TB_CLIENTE_CONTRATO B ON (B.ID_CLIENTE_ANEXO = A.ID) ");
                            //sql.AppendLine("WHERE A.ID_CLIENTE = @ID_CLIENTE AND A.ID_VENDA = @ID_VENDA");
                            sql.AppendLine("WHERE A.NOME_ARQUIVO NOT LIKE '%termo_adesao%' AND A.ID_CLIENTE = @ID_CLIENTE AND B.ID = @ID_CLIENTE_CONTRATO");

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    Status_Aceite_Contrato? status = null;
                                    status = reader["STATUS_ACEITE_CONTRATO"] != DBNull.Value ? (Status_Aceite_Contrato)Convert.ToInt64(reader["STATUS_ACEITE_CONTRATO"]) : status;
                                    var statusString = status != null ? HtmlEnumExtensions.GetEnumDescription<Status_Aceite_Contrato>(status.Value) : "";

                                    retorno.Objeto.Add(new ClienteOlaAnexoModelView
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Nome_Arquivo = Convert.ToString(reader["NOME_ARQUIVO"]),
                                        Titulo = Convert.ToString(reader["TITULO"]),
                                        Extensao_Arquivo = Convert.ToString(reader["EXTENSAO_ARQUIVO"]),
                                        Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"]),
                                        caminho_Arquivo = $"{CONSTANTES.URLPortal}/Arquivos/Cliente/{reader["ID_CLIENTE"]}/{reader["ID"]}{reader["EXTENSAO_ARQUIVO"]}",
                                        IsArquivoContratoAditivo = reader["IS_ANEXO_CTT_ADITIVO"] != DBNull.Value ? Convert.ToBoolean(reader["IS_ANEXO_CTT_ADITIVO"]) : (Boolean?)null,
                                        IsArquivoTermoAdesao = false,
                                    });
                                }

                                if (!retorno.Objeto.Any(obj => obj.Id == 0))
                                {
                                    retorno.Objeto.Add(new ClienteOlaAnexoModelView
                                    {
                                        Id = 0,
                                        Nome_Arquivo = "Termo de Adesão",
                                        Titulo = null,
                                        Extensao_Arquivo = ".pdf",
                                        Id_Cliente = value.Id_Cliente,
                                        caminho_Arquivo = $@"{CONSTANTES.URLPortal}/Arquivos/TermoAdesao/termo_adesao_{Convert.ToString(value.Id_Venda)}.pdf",
                                        IsArquivoContratoAditivo = false,
                                        IsArquivoTermoAdesao = true,
                                    });
                                }
                            }
                        }
                    }
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/GravarAnexo")]
        public async Task<PostApiRetorno<Int64>> GravarAnexo([FromBody] ParamWebApi<ClienteAnexoModelView> value)
        {


            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Cliente_Anexo modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoVendaUltimoContato = new Conexao<Venda_Ultimo_Contato>(session))
                {
                    using (var repository = new Conexao<Cliente_Anexo>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Cliente_Anexo
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);
                        await repository.SaveOrUpdate(modelo);

                        if (value.Objeto.Id_Venda.HasValue)
                        {
                            Venda_Ultimo_Contato venda_Ultimo_Contato = new Venda_Ultimo_Contato();
                            Venda venda = await repoVenda.selectFirst(p => p.Id == value.Objeto.Id_Venda);
                            venda_Ultimo_Contato = new Venda_Ultimo_Contato()
                            {
                                Id_Venda = value.Objeto.Id_Venda.Value,
                                Data_Ultimo_Contato = DateTime.Now,
                                UsuarioCriacao = value.Usuario,
                                Forma_Contato = "",
                                Observacao = "arquivo anexado a venda",
                                Acao = "arquivo anexado a venda",
                                Status_Venda = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(venda.Status))
                            };

                            await repoVendaUltimoContato.Save(venda_Ultimo_Contato);

                        }

                        if (modelo.Foto_Perfil)
                        {
                            modelo.Cliente.Foto_Perfil = modelo;
                            await repoCliente.SaveOrUpdate(modelo.Cliente);
                        }

                        retorno.IdObjeto = modelo.Id;
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
        [Route("api/clienteapi/ExcluirListaAnexo")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirListaAnexo(List<Int64> ids)
        {


            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                foreach (Int64 id in ids)
                {
                    await ExcluirAnexo(id);
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


        [HttpGet()]
        [Route("api/clienteapi/ExcluirAnexo")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirAnexo(Int64 Id)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repository = new Conexao<Cliente_Anexo>(session))
                    {
                        Cliente_Anexo modelo = (await repository.select(p => p.Id == Id)).FirstOrDefault();

                        try
                        {
                            String caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{modelo.Cliente.Id}\{modelo.Id}.{modelo.Extensao_Arquivo}";

                            if (File.Exists(caminhoArquivo))
                                File.Delete(caminhoArquivo);
                        }
                        catch { }


                        if (modelo.Foto_Perfil)
                        {
                            var modelo_cliente = modelo.Cliente;

                            modelo_cliente.Foto_Perfil = null;
                            await repoCliente.SaveOrUpdate(modelo_cliente);
                        }
                        await repository.Remove(modelo);

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


        [AcceptVerbs("Get", "Post")]
        [Route("api/clienteapi/ExisteCpf")]
        public async Task<Int32> Existecpf(String cpf)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();
            Int32 recordsTotal = 0;

            string somenteNumeros = new String(cpf.Where(Char.IsDigit).ToArray());

            if (!String.IsNullOrEmpty(somenteNumeros))
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    try
                    {
                        //Filtros
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();

                        //Select
                        using (var com = session.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder();

                            sql.AppendLine("SELECT COUNT(A.ID) CO");
                            sql.AppendLine("FROM TB_CLIENTE A");
                            sql.AppendLine("WHERE (A.CPF = @VALOR)");
                            //parametros.Add("@VALOR", cpf);
                            parametros.Add("@VALOR", somenteNumeros);

                            using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                            {
                                if (await reader.ReadAsync())
                                {
                                    recordsTotal = Convert.ToInt32(reader["CO"]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return recordsTotal;
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/clienteapi/BuscaDadosCPF_CNPJ")]
        public async Task<Int32> BuscaDadosCPF_CNPJ(String cpf_cnpj)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();
            Int32 recordsTotal = 0;

            string somenteNumeros = new String(cpf_cnpj.Where(Char.IsDigit).ToArray());

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT COUNT(A.ID) CO");
                        sql.AppendLine("FROM TB_CLIENTE A");
                        sql.AppendLine("WHERE (A.CPF = @VALOR)");
                        //parametros.Add("@VALOR", cpf);
                        parametros.Add("@VALOR", somenteNumeros);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return recordsTotal;
        }


        [AcceptVerbs("Get", "Post")]
        [Route("api/clienteapi/ClienteChat")]
        public async Task<ClienteChat_Mobile> ClienteChat(String cpf_cnpj, AtendimentoChatMongo mongo)
        {
            ClienteChat_Mobile retorno = new ClienteChat_Mobile();
            Int32 recordsTotal = 0;

            string somenteNumeros = new String(cpf_cnpj.Where(Char.IsDigit).ToArray());

            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.TIPOPESSOA, A.CPF, A.NOMEFANTASIA, A.TIPO_CLIENTE, A.EMAIL_PRINCIPAL ");
                        sql.AppendLine("FROM TB_CLIENTE A");
                        sql.AppendLine("WHERE (A.CPF = @VALOR)");
                        //parametros.Add("@VALOR", cpf);
                        parametros.Add("@VALOR", somenteNumeros);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.cpf_cnpj = Convert.ToString(reader["CPF"]);
                                retorno.idCliente = Convert.ToInt32(reader["ID"]);
                                retorno.nome = Convert.ToString(reader["NOMEFANTASIA"]);
                                retorno.idTipoAtendimento = Convert.ToInt32(reader["TIPO_CLIENTE"]);
                                retorno.email = Convert.ToString(reader["EMAIL_PRINCIPAL"]);
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

        [AcceptVerbs("Get", "Post")]
        [Route("api/clienteapi/ClienteBloqueado")]
        public async Task<Int32> ClienteBloqueado(String idCliente)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ")
                        .AppendLine(" FROM TB_CLIENTE_PLANO_PRODUTO A  ")
                        .AppendLine(" WHERE (A.VELOCIDADE_REDUZIDA = 1) ")
                        .AppendLine(" OR (A.STATUS IN (3, 4, 5))")
                        .AppendLine("AND A.ID_CLIENTE = @ID_CLIENTE");
                        parametros.Add("@ID_CLIENTE", idCliente);


                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return recordsTotal;
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/clienteapi/buscaCpf")]
        public async Task<GetApiRetorno<List<Cliente_ValidacaoModel>>> buscaCpf(String term, Int16? Tipo_Cliente = null, Int16? Tipo_Pessoa = null)
        {
            GetApiRetorno<List<Cliente_ValidacaoModel>> retorno = new GetApiRetorno<List<Cliente_ValidacaoModel>>();

            retorno.Objeto = new List<Cliente_ValidacaoModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.TIPOPESSOA, A.STATUS ");
                        sql.AppendLine("FROM TB_CLIENTE A ");
                        sql.AppendLine("WHERE (UPPER(A.CPF) = @VALOR)");

                        if (Tipo_Cliente != null)
                        {
                            sql.AppendLine("AND A.TIPO_CLIENTE = @TIPOCLIENTE");
                            parametros.Add("@TIPOCLIENTE", Tipo_Cliente);
                        }

                        if (Tipo_Pessoa != null)
                        {
                            sql.AppendLine("AND A.TIPOPESSOA = @TIPOPESSOA");
                            parametros.Add("@TIPOPESSOA", Tipo_Pessoa);
                        }

                        parametros.Add("@VALOR", $"{term.ToUpperInvariant()}");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new Cliente_ValidacaoModel
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    TipoPessoa = reader["TIPOPESSOA"] != DBNull.Value ? Convert.ToInt32(reader["TIPOPESSOA"]) : 0,
                                    Status = reader["STATUS"] != DBNull.Value ? Convert.ToBoolean(reader["STATUS"]) : false
                                });
                            }
                        }

                        if (retorno.Objeto.Count > 0)
                            retorno.Ok = true;

                        else
                        {
                            retorno.Mensagem = "CPF não encontrado";
                            retorno.Ok = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    throw ex;
                }
            }


            return retorno;
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/clienteapi/buscaNome")]
        public async Task<GetApiRetorno<List<Cliente_ValidacaoModel>>> buscaNome(String term, Int16? Tipo_Cliente = null, Int16? Tipo_Pessoa = null)
        {
            GetApiRetorno<List<Cliente_ValidacaoModel>> retorno = new GetApiRetorno<List<Cliente_ValidacaoModel>>();

            retorno.Objeto = new List<Cliente_ValidacaoModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.TIPOPESSOA, A.STATUS ");
                        sql.AppendLine("FROM TB_CLIENTE A ");
                        sql.AppendLine("WHERE (UPPER(A.NOMEFANTASIA) = @VALOR)");

                        if (Tipo_Cliente != null)
                        {
                            sql.AppendLine("AND A.TIPO_CLIENTE = @TIPOCLIENTE");
                            parametros.Add("@TIPOCLIENTE", Tipo_Cliente);
                        }

                        if (Tipo_Pessoa != null)
                        {
                            sql.AppendLine("AND A.TIPOPESSOA = @TIPOPESSOA");
                            parametros.Add("@TIPOPESSOA", Tipo_Pessoa);
                        }

                        parametros.Add("@VALOR", $"{term.ToUpperInvariant()}");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new Cliente_ValidacaoModel
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    TipoPessoa = Convert.ToInt32(reader["TIPOPESSOA"]),
                                    Status = Convert.ToBoolean(reader["STATUS"])
                                });
                            }
                        }

                        if (retorno.Objeto.Count > 0)
                            retorno.Ok = true;

                        else
                        {
                            retorno.Mensagem = "Cliente não encontrado";
                            retorno.Ok = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    throw ex;
                }
            }


            return retorno;
        }

        [HttpGet()]
        [Route("api/clienteapi/busca")]
        public async Task<List<ObjBusca<Int64>>> buscaReloadData(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOMEFANTASIA ");
                        sql.AppendLine("FROM TB_CLIENTE A");
                        sql.AppendLine("WHERE (UPPER(A.NOMEFANTASIA) LIKE @VALOR)");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOMEFANTASIA"])

                                });
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

        [HttpGet()]
        [Route("api/clienteapi/buscaVenda")]
        public async Task<List<ObjBusca<Int64>>> buscaPainelVenda(String term, Int16? Tipo_Cliente, Int16? Tipo_Pessoa)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOMEFANTASIA ");
                        sql.AppendLine("FROM TB_CLIENTE A");
                        sql.AppendLine("WHERE (A.NOMEFANTASIA = @VALOR)");
                        parametros.Add("@VALOR", term);

                        if (Tipo_Cliente != null)
                        {
                            sql.AppendLine("AND A.TIPO_CLIENTE = @TIPOCLIENTE");
                            parametros.Add("@TIPOCLIENTE", Tipo_Cliente);
                        }

                        if (Tipo_Pessoa != null)
                        {
                            sql.AppendLine("AND A.TIPOPESSOA = @TIPOPESSOA");
                            parametros.Add("@TIPOPESSOA", Tipo_Pessoa);
                        }

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOMEFANTASIA"])

                                });
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
        [Route("api/clienteapi/consultaVenda")]
        public async Task<List<string>> consultaVenda(String idCliente)
        {
            List<string> retorno = new List<string>();
            string mensagemRetorno = "";

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DATA_VENDA, A.STATUS, B.TITULO ");
                        sql.AppendLine("FROM TB_VENDA A ");
                        sql.AppendLine("LEFT JOIN TB_CAMPANHA  B ON B.ID = A.ID_CAMPANHA ");
                        sql.AppendLine("LEFT JOIN TB_CAMPANHA_PLANO_PRODUTO C ON C.ID = A.ID_CAMPANHA ");
                        sql.AppendLine("WHERE (A.ID_CLIENTE = @CODCT)");

                        parametros.Add("@CODCT", idCliente);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var idVenda = Convert.ToString(reader["ID"]);
                                var campanha = Convert.ToString(reader["TITULO"]);
                                var dataVenda = reader["DATA_VENDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy HH:mm") : String.Empty;
                                var status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUS"])) : "";

                                if (idVenda != "")
                                {
                                    mensagemRetorno += ("\r\n Nº venda: " + idVenda +
                                        "\r\n  Data: " + dataVenda +
                                        "\r\n  Campanha: " + campanha +
                                        "\r\n  Situação: " + status +
                                        "\r\n");
                                }

                            }

                            if (String.IsNullOrEmpty(mensagemRetorno))
                            {
                                mensagemRetorno = ("Não foram encontradas vendas em aberto para este cliente.");
                            }

                            retorno.Add(mensagemRetorno);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = ex.Message;
                    throw new Exception("erro");
                }
            }
            return retorno;
        }


        [HttpGet()]
        [Route("api/clienteapi/buscaQuemIndicou")]
        public async Task<List<ObjBusca<Int64>>> buscaQuemIndicou(String cpfcnpj, String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOMEFANTASIA ")
                        .AppendLine("FROM TB_CLIENTE A ")
                        .AppendLine($"WHERE (UPPER(A.NOMEFANTASIA) LIKE '%{StringService.Truncate(term.ToUpperInvariant(), 255)}%') OR (A.CPF LIKE '%{StringService.Truncate(term.ToUpperInvariant(), 18)}%')");

                        if (!String.IsNullOrEmpty(cpfcnpj))
                        {
                            sql.AppendLine(" AND(A.CPF <> @CPF) ");
                            parametros.Add("@CPF", cpfcnpj);
                        }

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOMEFANTASIA"])
                                });
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



        [HttpGet()]
        [Route("api/clienteapi/buscaAtedimento")]
        public async Task<List<ClienteListaBuscaAtendimento>> buscaAtedimento(String term)
        {
            List<ClienteListaBuscaAtendimento> retorno = new List<ClienteListaBuscaAtendimento>();
            using (ISession session_endereco = HibernateUtil.GetSession())
            using (ISession session = HibernateUtil.GetSession())
            using (var repoEndereco = new Conexao<Endereco_Cliente>(session_endereco))
            using (var apiPlanoProduto = new PlanoProdutoApiController())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID,A.NOMEFANTASIA, A.CPF, A.RG, A.TELEFONE_FIXO, A.DATA_NASC, COALESCE(A.DATA_CRIACAO_CLIENTE,A.DATAHORACRIACAO) AS DATA_CRIACAO_CLIENTE, ");
                        sql.AppendLine("B.ID AS ID_CATEGORIA_CLIENTE, B.DESCRICAO AS DESC_CATEGORIA, A.TELEFONE_FIXO, A.TELEFONE_CELULAR, A.TELEFONE_RECADO,A.PESSOA_TEL_RECADO,A.NUMERO_CONTRATO, ");
                        sql.AppendLine("A.EMAIL_PRINCIPAL, A.ID_ENDERECO_PRINCIPAL, A.TIPO_CLIENTE, A.DATAHORACRIACAO, A.LOGIN, A.ID_ORGANIZACAO ");
                        sql.AppendLine(" FROM TB_CLIENTE A ");
                        sql.AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE B ");
                        sql.AppendLine(" ON(A.id_categoria = B.ID) ");


                        //sql.AppendLine("WHERE (UPPER(A.CPF) LIKE @VALOR ) OR (UPPER(A.NOMEFANTASIA) LIKE @VALOR ) ");
                        //parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");
                        sql.AppendLine("WHERE A.ID = @ID ");
                        parametros.Add("@ID", term);


                        //sql.AppendLine("WHERE (UPPER(A.NOMEFANTASIA) LIKE @VALOR )");
                        //parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {

                            while (await reader.ReadAsync())
                            {
                                var Data_Nasc_Cliente = reader["DATA_NASC"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_NASC"]) : (DateTime?)null;
                                var temp = (new ClienteListaBuscaAtendimento
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Telefone_Fixo = Convert.ToString(reader["TELEFONE_FIXO"]),
                                    Categoria_Cliente = Convert.ToString(reader["DESC_CATEGORIA"]),
                                    Id_Categoria_Cliente = Convert.ToString(reader["ID_CATEGORIA_CLIENTE"]),
                                    Telefone_Celular = Convert.ToString(reader["TELEFONE_CELULAR"]),
                                    CPF_CNPJ_Cliente = Convert.ToString(reader["CPF"]),
                                    Data_Nasc_Cliente = reader["DATA_NASC"] != DBNull.Value ? Convert.ToString(Data_Nasc_Cliente.Value.ToString("dd/MM/yyyy")) : "-",
                                    RG_Cliente = Convert.ToString(reader["RG"]),
                                    Email_Principal = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                    Telefone_Recado = Convert.ToString(reader["TELEFONE_RECADO"]),
                                    PlanosProdutos = apiPlanoProduto.AtendimentogetByCliente(Convert.ToInt64(reader["ID"])).Result,
                                    Id_Endereco_Cliente = Convert.ToString(reader["ID_ENDERECO_PRINCIPAL"]),
                                    Tipo_Cliente = reader["TIPO_CLIENTE"] != DBNull.Value ? Convert.ToInt32(reader["TIPO_CLIENTE"]) : 0,
                                    Data_Cadastro = Convert.ToString(reader["DATAHORACRIACAO"]),
                                    Data_Criacao_Cliente = reader["DATA_CRIACAO_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["DATA_CRIACAO_CLIENTE"]) : String.Empty,
                                    Login_Radius = Convert.ToString(reader["LOGIN"]),
                                    Numero_Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                    Nome_Recado = Convert.ToString(reader["PESSOA_TEL_RECADO"]),
                                    Id_Organizacao = reader["ID_ORGANIZACAO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ORGANIZACAO"]) : (Int64?)null
                                });
                                var endereco = repoEndereco.selectFirst(p => p.Id == Convert.ToInt64(reader["ID_ENDERECO_PRINCIPAL"])).Result;
                                temp.Endereco_Cliente = String.Concat(endereco.Logradouro, ',', endereco.Numero, '-', endereco.Bairro, '-', endereco.Cidade);
                                retorno.Add(temp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            foreach (var item in retorno)
            {
                item.Cliente_Ha = String.IsNullOrEmpty(retorno[0].Data_Criacao_Cliente) ? 0 : (DateTime.Now - Convert.ToDateTime(retorno[0].Data_Criacao_Cliente)).Days;
            }

            return retorno;

        }

        [HttpGet()]
        [Route("api/clienteapi/buscaCriarOS")]
        public async Task<List<ClienteListaBuscaAtendimento>> BuscaCriarOS(String term)
        {
            List<ClienteListaBuscaAtendimento> retorno = new List<ClienteListaBuscaAtendimento>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var apiPlanoProduto = new PlanoProdutoApiController())
                using (var repo_configuracoes = new Conexao<Configuracoes_Gerais>(sessionNh))
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlFrom = new StringBuilder();
                    var obj_config = await repo_configuracoes.selectFirst();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine("SELECT A.ID, A.NOMEFANTASIA, A.DATAHORACRIACAO, A.CPF, A.TELEFONE_FIXO, A.DATA_NASC,");
                        sql.AppendLine("B.DESCRICAO AS DESC_CATEGORIA, B.TIPO_PRIORIDADE, A.NUMERO_CONTRATO, A.TELEFONE_FIXO, A.TELEFONE_CELULAR, A.TELEFONE_RECADO,");
                        sql.AppendLine("A.EMAIL_PRINCIPAL,A.ID_ENDERECO_PRINCIPAL, A.ID_ENDERECO_PROSPECTO, A.TIPO_CLIENTE");

                        var termoReplace = ($"{term.ToUpperInvariant().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty)}");

                        long n;
                        bool isNumeric = Int64.TryParse(termoReplace, out n);
                        if (isNumeric)
                        {
                            condicoes.Add("A.STATUS = 1 AND ((UPPER(A.CPF) LIKE @VALOR))");
                            parametros.Add("@VALOR", $"%{term.ToUpperInvariant().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty)}%");
                        }
                        else
                        {
                            condicoes.Add(" A.STATUS = 1 AND ((UPPER(A.NOMEFANTASIA) LIKE @VALOR))");
                            parametros.Add("@VALOR", $"%{term.ToUpperInvariant().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty)}%");
                        }

                        sqlFrom.AppendLine("FROM TB_CLIENTE A");
                        sqlFrom.AppendLine("LEFT JOIN TB_CATEGORIA_CLIENTE B ON(A.ID_CATEGORIA = B.ID)");

                        if (condicoes.Count() > 0)
                        {
                            var status_prospecto = obj_config.Permite_OS_LEAD ? "A.TIPO_CLIENTE IN (0, 1)" : "A.TIPO_CLIENTE = 0";
                            sqlFrom.AppendLine($"WHERE {status_prospecto} AND ({String.Join(" AND ", condicoes)})");
                        }

                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NUMERO_CONTRATO)");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ClienteListaBuscaAtendimento
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Telefone_Fixo = Convert.ToString(reader["TELEFONE_FIXO"]),
                                    Categoria_Cliente = Convert.ToString(reader["DESC_CATEGORIA"]),
                                    Telefone_Celular = Convert.ToString(reader["TELEFONE_CELULAR"]),
                                    CPF_CNPJ_Cliente = Convert.ToString(reader["CPF"]),
                                    Email_Principal = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                    //Forma_Pagamento_Utilizada = Convert.ToString(reader["DESC_FORMA_PAGAMENTO"]),
                                    Telefone_Recado = Convert.ToString(reader["TELEFONE_RECADO"]),
                                    PlanosProdutos = apiPlanoProduto.getByCliente(Convert.ToInt64(reader["ID"])).Result,
                                    Id_Endereco_Cliente = Convert.ToString(reader["ID_ENDERECO_PRINCIPAL"]),
                                    Id_Endereco_Prospecto = reader["ID_ENDERECO_PROSPECTO"] != DBNull.Value ? Convert.ToString(reader["ID_ENDERECO_PROSPECTO"]) : "",
                                    Data_Cadastro = reader["DATAHORACRIACAO"] != DBNull.Value ? (DateTime.Now - Convert.ToDateTime(reader["DATAHORACRIACAO"])).Days.ToString() + " Dias" : String.Empty,
                                    Nome_Fantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Numero_Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                    Tipo_Prioridade = Convert.ToString(reader["TIPO_PRIORIDADE"]),
                                    Cliente_Prospecto = Convert.ToInt32(reader["TIPO_CLIENTE"]) == 0 ? false : true
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return retorno;


        }

        #region Mobile

        [HttpGet()]
        [Route("api/clienteapi/findByCPF")]
        public async Task<GetApiRetorno<object>> findByCPF(String term)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();

            retorno.Objeto = new ClienteDadosGeraisViewModel();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionConnect = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
            using (var repoCliente = new Conexao<Cliente>(sessionConnect))
            using (var repoVenda = new Conexao<Venda>(sessionConnect))
            using (var repoArea = new Conexao<AreaAniel>(sessionT5))
            using (var repoEscritorio = new Conexao<EscritorioAniel>(sessionT5))
            using (var apiCampos = new CamposObrigatoriosClienteApiController())
            {
                try
                {
                    var configs = repoConfig.selectFirst().Result;

                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = sessionConnect.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.TIPOPESSOA ");
                        sql.AppendLine("FROM TB_CLIENTE A ");
                        sql.AppendLine("WHERE A.STATUS = 1 AND (UPPER(A.CPF) = @VALOR)");
                        parametros.Add("@VALOR", $"{term.ToUpperInvariant()}");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionConnect.Connection, null, 10, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                try
                                {
                                    Int64 id = Convert.ToInt64(reader["ID"]);
                                    var cliente = GetViewObjMobile(sessionConnect, await repoCliente.selectFirst(s => s.Id == id));
                                    var temVendaAberta = await repoVenda.selectFirst(t => t.Id_Cliente == id && (t.Status == Status_Venda.Aguardando_Avaliacao_Comercial || t.Status == Status_Venda.Analise_Cliente));
                                    cliente.Id_Cliente = cliente.Id;

                                    foreach (PlanoProdutoListModel p in cliente.Planos)
                                    {
                                        if (p.Cod_Area != null && p.Cod_Escritorio != null)
                                        {
                                            p.Desc_Area = (await repoArea.selectFirst(s => s.Cod_Area == p.Cod_Area && s.Cod_Escritorio == p.Cod_Escritorio)).Descricao;
                                            p.Desc_Escritorio = (await repoEscritorio.selectFirst(s => s.Cod_Escritorio == p.Cod_Escritorio)).Descricao;
                                        }
                                    }

                                    var json = JsonConvert.SerializeObject(cliente);
                                    dynamic objRetorno = JsonConvert.DeserializeObject(json, typeof(ExpandoObject));

                                    objRetorno.docs = (await GetAnexos(id)).Objeto.Select(s => new
                                    {
                                        id = s.Id,
                                        caminho = s.caminho_Arquivo,
                                        name = s.Nome_Arquivo,
                                        mimeType = (s.Extensao_Arquivo == "pdf") ? "application/pdf" :
                                                  ((s.Extensao_Arquivo == "doc") ? "application/msword" :
                                                 (((s.Extensao_Arquivo == "docx") ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" :
                                                    "image/*")))
                                    });
                                    objRetorno.descontoTipoDiferente = configs.Acumular_Descontos_Diferentes;
                                    objRetorno.descontoTipoMesmo = configs.Acumular_Descontos_Mesmo_Tipo;
                                    objRetorno.temVendaAberta = temVendaAberta != null;

                                    retorno.Objeto = new
                                    {
                                        cliente = objRetorno,
                                        camposObrigatorio = apiCampos.GetCamposObrigatoriosInvisivel()
                                    };
                                    retorno.Ok = true;
                                }
                                catch (Exception ex)
                                {
                                    retorno.Ok = false;

                                    retorno.Mensagem = ex.Message;
                                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                                }
                            }
                            else
                            {
                                var json = JsonConvert.SerializeObject(new ClienteDadosGeraisViewModel());
                                dynamic objRetorno = JsonConvert.DeserializeObject(json, typeof(ExpandoObject));
                                objRetorno.descontoTipoDiferente = configs.Acumular_Descontos_Diferentes;
                                objRetorno.descontoTipoMesmo = configs.Acumular_Descontos_Mesmo_Tipo;

                                retorno.Objeto = new
                                {
                                    cliente = objRetorno,
                                    camposObrigatorio = apiCampos.GetCamposObrigatoriosInvisivel()
                                };
                                retorno.Ok = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    throw ex;
                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/clienteapi/salvarArquivo")]
        public async Task<PostApiRetorno> salvarArquivo()
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                HttpContext context = HttpContext.Current;
                String objeto = context.Request.Form["json"];

                ParamWebApi<ClienteArquivosMobile> file = JsonService.DeserializeJasonMapper<ParamWebApi<ClienteArquivosMobile>>(objeto);

                ClienteAnexoModelView arquivo = new ClienteAnexoModelView()
                {
                    Id_Cliente = file.Objeto.idCliente,
                    Extensao_Arquivo = file.Objeto.mimeType,
                    Nome_Arquivo = file.Objeto.nome
                };

                using (MemoryStream fotoMemoryStream = new MemoryStream())
                {
                    PostApiRetorno<long> retornoGravarAnexo = new PostApiRetorno<long>();

                    if (context.Request.Files[arquivo.Nome_Arquivo] != null)
                    {
                        context.Request.Files[arquivo.Nome_Arquivo].InputStream.CopyTo(fotoMemoryStream);

                        String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{arquivo.Id_Cliente}";
                        String _arquivo = $@"{_pasta}\{retornoGravarAnexo.IdObjeto}.{arquivo.Extensao_Arquivo}";

                        if (!Directory.Exists(_pasta))
                        {
                            Directory.CreateDirectory(_pasta);
                        }

                        if (context.Request.Files.Count > 0)
                        {
                            context.Request.Files[arquivo.Nome_Arquivo].SaveAs(_arquivo);
                        }

                        //set caminho arquivo
                        arquivo.caminho_Arquivo = _arquivo;

                        retornoGravarAnexo = await GravarAnexo(new ParamWebApi<ClienteAnexoModelView>
                        {
                            Objeto = arquivo
                        });

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/GerarDocumentoContrato")]
        public async Task<PostApiRetorno<ResponseDocContrato>> GerarDocumentoContrato(ClienteDocContratoModel value)
        {
            PostApiRetorno<ResponseDocContrato> response = new PostApiRetorno<ResponseDocContrato>();
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    response.IdObjeto = await GerarDocumentoContratoBusiness(session, value);
                    response.Ok = true;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null) response.InnerException = ex.InnerException.Message;
                    response.Ok = false;
                    response.Mensagem = ex.Message;
                    return response;
                }

            }
            return response;
        }

        public async Task<ResponseDocContrato> GerarDocumentoContratoBusiness(ISession session, ClienteDocContratoModel value)
        {
            ResponseDocContrato response = null;
            using (var cliPlanProdRepo = new Conexao<Cliente_PlanoProduto>(session))
            using (var cliContratoRepo = new Conexao<ClienteContrato>(session))
            using (var cliAnexoRepo = new Conexao<Cliente_Anexo>(session))
            using (var modeloContratoRepo = new Conexao<Modelo_Contrato>(session))
            using (var clienteRepo = new Conexao<Cliente>(session))
            {

                var cliente = await clienteRepo.selectFirst(c => c.Id == value.IdCliente);

                if (cliente is null)
                    throw new Exception("Cliente não encontrado com o Id informado");

                var modeloContrato = await modeloContratoRepo.selectFirst(m => m.Id == Convert.ToInt64(value.IdModeloSelecionado));

                if (modeloContrato is null)
                    throw new Exception("Modelo de Contrato não encontrado com o Id informado");

                if (modeloContrato.Extensao_Arquivo != "docx" && modeloContrato.Extensao_Arquivo != "pdf")
                {
                    throw new Exception("Modelo de Contrato selecionado não é um documento docx, verifique e tente novamente.");
                }
                var _IdClienteContrato = value.IdCliContrato > 0 ? value.IdCliContrato : Convert.ToInt64(value.NumeroContrato);
                var clienteContrato = await cliContratoRepo.selectFirst(ct => ct.Id == _IdClienteContrato); //status verificar caso esteja duplicado ou com dados parecidos no banco

                if (clienteContrato is null)
                    throw new Exception("ClienteContrato não encontrado com o Id informado");

                var clientePlanoProduto = await cliPlanProdRepo.select(cp => cp.Cliente == cliente && cp.ClienteContrato.Id == clienteContrato.Id);

                if (clientePlanoProduto is null)
                    throw new Exception("Plano e Produto não encontrado com o Id informado do Cliente");

                //var alreadyHasContrato = await cliContratoRepo.selectFirst(c => c.Cliente == cliente && c.NumeroContrato == value.NumeroContrato && c.ModeloContrato == modeloContrato);

                //if (alreadyHasContrato?.Cliente_Anexo != null)
                //    throw new Exception("Já existe um Contrato gerado para esse mesmo Cliente, Numero de Contrato e Modelo Selecionado");

                ArquivoContrato pdfContratoGerado = null;

                if (modeloContrato.Extensao_Arquivo == "docx")
                    pdfContratoGerado = await GerarContratoCliente(cliente, clientePlanoProduto, clienteContrato, modeloContrato.Id);
                else
                    pdfContratoGerado = new ArquivoContrato
                    {
                        CaminhoPDF = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modeloContrato.Id}.pdf",
                        ExtensaoArquivo = modeloContrato.Extensao_Arquivo,
                        NomeArquivo = modeloContrato.Arquivo_Doc,
                        TituloArquivo = $"cliente_Contrato-{clienteContrato.NumeroContrato}"
                    };

                if (!File.Exists(pdfContratoGerado.CaminhoPDF))
                {
                    throw new Exception("Arquivo gerado não foi encontrado na pasta de destino.");
                }

                byte[] pdfBytes = File.ReadAllBytes(pdfContratoGerado.CaminhoPDF);

                //ClienteContrato newCliContrato = ClienteContrato.FromModelo(modeloContrato, cliente);

                if (!value.Aditivo)
                    clienteContrato.Arquivo = pdfBytes;

                clienteContrato.Cliente_Anexo = new Cliente_Anexo
                {
                    Nome_Arquivo = pdfContratoGerado.NomeArquivo,
                    Extensao_Arquivo = pdfContratoGerado.ExtensaoArquivo,
                    Titulo = pdfContratoGerado.TituloArquivo,
                    Cliente = cliente,
                    ClienteContrato = clienteContrato,
                    Foto_Perfil = false,
                    Contrato = true,
                    Venda = null,
                    Arquivo = pdfBytes,
                    UsuarioCriacao = User.Identity.GetUserId(),
                    DataHoraCriacao = DateTime.Now,
                    IsAnexoContratoAditivo = value.Aditivo,
                    DescricaoModeloUtilizado = modeloContrato.Descricao,
                };

                await cliContratoRepo.SaveOrUpdate(clienteContrato);

                response = new ResponseDocContrato
                {
                    ClienteId = cliente.Id,
                    ModeloSelecionadoId = modeloContrato.Id,
                    NumeroContrato = clienteContrato.NumeroContrato,
                    IdCliContrato = await cliContratoRepo.Max(x => x.Id)
                };
                return response;
            }
        }




        [HttpPost()]
        [Route("api/clienteapi/enviarContratoCliente")]
        public async Task<PostApiRetorno<String>> EnviarContratoCliente(ClienteDocContratoModel model)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    ClienteContrato clienteContrato = null;

                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoClientePlanProd = new Conexao<Cliente_PlanoProduto>(session))
                    using (var cliContratoRepo = new Conexao<ClienteContrato>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var repoClienteAnexo = new Conexao<Cliente_Anexo>(session))
                    {

                        var cliente = await repoCliente.selectFirst(p => p.Id == model.IdCliente);
                        var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.ClienteContrato.NumeroContrato == model.NumeroContrato);

                        if (String.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            throw new Exception("Cliente não possui email vinculado");
                        }

                        Configuracoes_Gerais config = await repoConfig.selectFirst();
                        clienteContrato = await cliContratoRepo.selectFirst(c => c.Cliente == cliente && c.Id == model.IdCliContrato);

                        EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);
                        String urlAceiteContrato = $@"{CONSTANTES.URLPortal}/pt/Cliente/FormularioCliente/{clienteContrato.Id}"; //REFORMULAR

                        var produtos = "";
                        var arquivo_email = "";
                        var arquivo_modificado = "";

                        if (model.Aditivo)
                        {
                            var clienteAnexo = await repoClienteAnexo.selectFirst(x => x.Id == model.IdClienteAnexo);

                            if (clienteAnexo is null)
                                throw new Exception("Anexo não encontrado");


                            string nomeContrato = string.IsNullOrEmpty(clienteAnexo.DescricaoModeloUtilizado) ? clienteContrato.ModeloContrato.Descricao.ToUpper() : clienteAnexo.DescricaoModeloUtilizado.ToUpper();
                            using var stream = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_aceite_contrato_aditivo.html");
                            arquivo_email = stream.ReadToEnd();
                            arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia)
                                                               .Replace("{{NOME_CONTRATO}}", nomeContrato)
                                                               .Replace("{{URL_TERMO_ADESAO}}", urlAceiteContrato)
                                                               .Replace("{{URL_LOGO}}", config.OLA_URL_Logo);
                        }
                        else
                        {
                            foreach (var planoProduto in listaPlanosProdutos)
                            {
                                if (planoProduto.Combo != null)
                                {
                                    foreach (var planoProdutoCombo in planoProduto.Combo.PlanosProdutos)
                                    {
                                        using var stream_produtos = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_aceite_contrato_produtos.html");
                                        {
                                            String produto = stream_produtos.ReadToEnd();
                                            var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                                            produto = produto.Replace("{{DESC}}", planoProdutoCombo.Plano != null ? planoProdutoCombo.Plano.Descricao : planoProdutoCombo.Produto.Descricao)
                                                 .Replace("{{VENCIMENTO}}", planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "")
                                                 .Replace("{{TESTE}}", dias == 1 ? dias + "dia" : dias + " dias")
                                                 .Replace("{{VALOR}}", planoProduto.Valor_Final != null ? planoProduto.Valor_Final?.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

                                            produtos += produto;
                                        }
                                    }
                                }
                                else
                                {
                                    using var stream_produtos = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_aceite_contrato_produtos.html");
                                    {
                                        String produto = stream_produtos.ReadToEnd();
                                        var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                                        produto = produto.Replace("{{DESC}}", planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao)
                                             .Replace("{{VENCIMENTO}}", planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "")
                                             .Replace("{{TESTE}}", dias == 1 ? dias + "dia" : dias + " dias")
                                             .Replace("{{VALOR}}", planoProduto.Valor_Final != null ? planoProduto.Valor_Final?.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

                                        produtos += produto;
                                    }
                                }
                            }

                            using var stream = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_aceite_contrato.html");
                            arquivo_email = stream.ReadToEnd();
                            arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia)
                                                               .Replace("{{URL_TERMO_ADESAO}}", urlAceiteContrato)
                                                               .Replace("{{URL_LOGO}}", config.OLA_URL_Logo)
                                                               .Replace("{{PRODUTOS}}", produtos);
                        }


                        if (!String.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            emailService.EnviarMensagem(cliente.Email_Principal, $@"Bem-vindo(a)! Segue o Contrato para Aceite.", arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                        }
                        else
                        {
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.CLIENTE_SEM_EMAIL, cliente.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }

                    retorno.IdObjeto = $"Contrato nº {clienteContrato.Id} foi enviado ao email com sucesso!";
                    retorno.Ok = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    transaction.Rollback();
                    retorno.Mensagem = ex.Message;
                    retorno.Ok = false;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/clienteapi/encontrarContratoId")]
        public async Task<GetApiRetorno<ContratoFormularioClienteModel>> EncontrarContratoId(Int64? IdContrato)
        {
            GetApiRetorno<ContratoFormularioClienteModel> retorno = new GetApiRetorno<ContratoFormularioClienteModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var termoAdesaoApi = new Termo_AdesaoApiController())
            using (var cliContratoRepo = new Conexao<ClienteContrato>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoAcoes = new Conexao<Acoes_Prova_Vida_Termo>(session))
            {
                try
                {
                    ClienteContrato clienteContrato = null;
                    Cliente cliente = null;

                    if (IdContrato.HasValue)
                    {
                        clienteContrato = await cliContratoRepo.selectFirst(p => p.Id == IdContrato.Value);
                        cliente = await repoCliente.selectFirst(p => p.Id == clienteContrato.Cliente.Id);
                    }
                    else
                    {
                        throw new Exception(RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    var acoesTermo = await repoAcoes.select();
                    string acoes = JsonConvert.SerializeObject(acoesTermo);

                    Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                    {
                        Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                        Id_Cliente = cliente.Id,
                        Url_Assinar_Documento = clienteContrato.Url_Assinar_Documento,
                        Email_Principal = cliente.Email_Principal,
                        Email_Auxiliar = cliente.Email_Auxiliar,
                        NomeFantasia = cliente.NomeFantasia,
                        Id_Contrato = clienteContrato.Id,
                        Tipo_Integracao_Assinatura = TipoIntegracaoAssinaturaEnum.Lexio
                    };

                    string urlTermoIntegracao = String.Empty;
                    if (config.Utiliza_Certificacao_Assinatura)
                    {
                        if (String.IsNullOrEmpty(clienteContrato.Url_Assinar_Documento))
                        {
                            var retornoIntegracao = new Enviar_Documento_ResponseModel();
                            switch (config.Tipo_Integracao_Assinatura)
                            {
                                case TipoIntegracaoAssinaturaEnum.Lexio:
                                    String file = Convert.ToBase64String(clienteContrato.Cliente_Anexo.Arquivo);
                                    retornoIntegracao = new IntegracaoAssinaturaRepository().Enviar_Documento(new Lexio(), session, configCert, file);
                                    clienteContrato.Tipo_Integracao_Assinatura = TipoIntegracaoAssinaturaEnum.Lexio;
                                    break;
                                default:
                                    break;

                            }

                            clienteContrato.Url_Assinar_Documento = retornoIntegracao.Link_Assinatura;
                            clienteContrato.Token_Documento_Assinatura = retornoIntegracao.Id_Documento_Integracao;

                            await cliContratoRepo.SaveOrUpdate(clienteContrato);
                        }

                        urlTermoIntegracao = termoAdesaoApi.GetHtmlTermoAdesaoIntegracao(cliente, config, null, clienteContrato);
                    }

                    string urlPdfTermoAdesao = $@"{CONSTANTES.URLPortal}/Arquivos/Cliente/{cliente.Id}/{clienteContrato.Cliente_Anexo.Nome_Arquivo}.pdf";

                    var termo = new ContratoFormularioClienteModel
                    {
                        IdContrato = clienteContrato.Id,
                        Cliente = cliente.NomeFantasia,
                        Empresa = config.Nome_Cliente_Sinapse,
                        UrlLogo = config.OLA_URL_Logo,
                        Termo = !String.IsNullOrEmpty(urlTermoIntegracao) ? urlTermoIntegracao : urlPdfTermoAdesao,
                        Aceito = false,
                        isIntegracao = config.Utiliza_Certificacao_Assinatura,
                        ExigeFotoTermo = config.Exige_Foto_Termo,
                        TipoComprovanteTermo = config.Tipo_Comprovante_Termo,
                        Acoes_Prova_Vida = acoes,
                        Permite_Anexar_Doc = config.Permite_Anexar_Doc_Termo,
                    };

                    retorno.Objeto = termo;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/GravarFormulario")]
        public async Task<PostApiRetorno<ContratoFormularioCliResponseModel>> GravarFormulario(ParamWebApi<ContratoFormularioClienteModel> value)
        {
            PostApiRetorno<ContratoFormularioCliResponseModel> retorno = new PostApiRetorno<ContratoFormularioCliResponseModel>();
            retorno.IdObjeto = new ContratoFormularioCliResponseModel();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
            using (var repositoryAnexo = new Conexao<Cliente_Anexo>(session))
            {
                try
                {
                    ClienteContrato clienteContrato = null;
                    clienteContrato = await repoClienteContrato.selectFirst(p => p.Id == value.Objeto.IdContrato);

                    if (clienteContrato.StatusAceiteContrato.HasValue)
                    {
                        if (clienteContrato?.StatusAceiteContrato.Value == Status_Aceite_Contrato.Aprovado && !value.Objeto.Aceito)
                        {
                            retorno.IdObjeto.IsAssinado = true;
                            throw new Exception(RsCadastros.ACEITE_CONTRATO_RECUSADO);
                        }
                    }

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    if (config.Utiliza_Certificacao_Assinatura)
                    {
                        Consultar_Documento_ResponseModel retornoIntegracao = new Consultar_Documento_ResponseModel();

                        Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                        {
                            Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                            Token_Documento_Assinatura = clienteContrato.Token_Documento_Assinatura,
                            Id_Contrato = clienteContrato.Id,
                            Id_Cliente = clienteContrato.Cliente.Id
                        };

                        switch (config.Tipo_Integracao_Assinatura)
                        {
                            case TipoIntegracaoAssinaturaEnum.Lexio:
                                retornoIntegracao = new IntegracaoAssinaturaRepository().Consultar_Documento(new Lexio(), session, configCert);

                                if (!retornoIntegracao.IsAssinado)
                                {
                                    retorno.IdObjeto.IsAssinado = false;
                                    retorno.IdObjeto.IdContrato = retornoIntegracao.Id_Contrato.Value;
                                    throw new Exception(RsCadastros.ACEITE_CONTRATO_RECUSADO);
                                }

                                clienteContrato.StatusAceiteContrato = Status_Aceite_Contrato.Aprovado;
                                clienteContrato.DataAceiteContrato = retornoIntegracao.Data_Assinatura;
                                clienteContrato.Url_Documento_Assinado = retornoIntegracao.Link_Download_Documento;

                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        clienteContrato.StatusAceiteContrato = Status_Aceite_Contrato.Aprovado;
                        clienteContrato.DataAceiteContrato = DateTime.Now;
                    }

                    string ip = HttpContext.Current.Request.Headers["X-Forwarded-For"];

                    if (!string.IsNullOrEmpty(ip))
                    {
                        ip = ip.Split(',')[0].Trim();
                    }
                    else if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]))
                    {
                        ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    else
                    {
                        ip = HttpContext.Current.Request.UserHostAddress;
                    }

                    string porta = HttpContext.Current.Request.ServerVariables["REMOTE_PORT"];

                    if (string.IsNullOrEmpty(porta))
                    {
                        porta = Convert.ToString(HttpContext.Current.Request.Url.Port);
                    }

                    if (clienteContrato.StatusAceiteContrato == Status_Aceite_Contrato.Aprovado)
                    {
                        clienteContrato.IpOrigemAceite = ip;
                        clienteContrato.UserAgentContratoAceite = HttpContext.Current.Request.UserAgent;
                        clienteContrato.CodigoHashContratoAceite = HttpContext.Current.Request.Url.Segments.Last().TrimEnd('/');
                        clienteContrato.UrlAceiteContrato = HttpContext.Current.Request.Url.AbsoluteUri;
                        clienteContrato.PortaOrigemAceite = porta;
                        clienteContrato.AdicionadoAceiteDigital = false;
                    }

                    await repoClienteContrato.SaveOrUpdate(clienteContrato);

                    if (config.Tipo_Comprovante_Termo == Tipo_Comprovante_Termo.Selfie)
                    {
                        await GravarAnexo(value.Objeto, clienteContrato, repositoryAnexo);
                    }

                    retorno.IdObjeto.IsAssinado = true;
                    retorno.IdObjeto.IdContrato = clienteContrato.Id;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.IdObjeto.IdContrato = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/EnviarDocumentacaoMostQIExtractVio")]
        public async Task<Response> EnviarDocumentacaoMostQIExtractVio([FromBody] ContratoFormularioClienteModel form)
        {
            Response resposta = new Response();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var clienteContratoRepo = new Conexao<ClienteContrato>(session))
            using (var repoIntegracoesExt = new Conexao<IntegracoesExternas>(session))
            {
                if (!String.IsNullOrEmpty(form.InputBase64Vio) || !String.IsNullOrEmpty(form.Base64))
                {

                    var clienteContrato = await clienteContratoRepo.selectFirst(x => x.Id == form.IdContrato);
                    string retornoURL = null;
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\_tempVio";

                    string base64 = null;


                    //Recebeu um anexo
                    if (!String.IsNullOrEmpty(form.InputBase64Vio))
                    {

                        base64 = form.InputBase64Vio.Replace($"data:{form.InputBase64Type};base64,", "");
                    }

                    //Recebeu uma foto
                    else
                    {
                        base64 = form.Base64.Replace($"data:{form.InputBase64Type};base64,", "");
                    }

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var bytes = Convert.FromBase64String(base64);

                    var ext = form.InputBase64Type == "application/pdf" ? "pdf" : "png";

                    using (var imageFile = new FileStream(Path.Combine(_pasta, $@"{form.IdContrato}.{ext}"), FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/Cliente/{clienteContrato.Cliente.Id}/_tempVio/{form.IdContrato}.{ext}";

                    VioExtractionRequest request = new VioExtractionRequest
                    {
                        fileUrl = retornoURL,
                        fileBase64 = base64,
                        qrCodeField = null
                    };

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MOSTQI);

                    var config = await repoIntegracoesExt.select();
                    var retornoIntegracao = await new IntegracaoValidacaoDocumentoRepository().Extract(new MostqVioExtraction(externas.TokenAcesso), request);


                    if (retornoIntegracao.status.code == "200")
                    {
                        if (retornoIntegracao.result.Count == 0)
                        {
                            resposta.result = false;
                            resposta.message = "Não foi encontrado um documento válido na foto, por favor, tire a foto novamente e certifique-se de que ela esteja visível e enquadrada.";
                        }
                        else
                        {
                            String _pastaDocumento = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\Documentacao";

                            if (!Directory.Exists(_pastaDocumento))
                            {
                                Directory.CreateDirectory(_pastaDocumento);
                            }
                            string fileName = form.IdContrato + ".txt";
                            string filePath = Path.Combine(_pastaDocumento, fileName);
                            System.IO.File.WriteAllText(filePath, base64, Encoding.UTF8);

                            resposta = await EnviarFaceMatch(form);

                        }
                    }
                    else
                    {
                        resposta.result = false;
                        resposta.message = $@"O processamento da sua solicitação encontrou um erro inesperado. Pedimos desculpas pela inconveniência. Por favor, entre em contato com o administrador do sistema e forneça o seguinte código de referência para que possamos ajudá-lo a resolver o problema: {retornoIntegracao.status.code}";
                    }

                }
                else
                {
                    resposta.result = false;
                    resposta.message = "Arquivo de imagem não encontrado, por favor, tente novamente.";
                }
            }

            return resposta;
        }


        [HttpPost()]
        [Route("api/clienteapi/EnviarVideoLivenessDetect")]
        public async Task<Response> EnviarVideoLivenessDetect([FromBody] ContratoFormularioClienteModel form)
        {

            Response resposta = new Response();

            using (ISession session = HibernateUtil.GetSession())
            using (var clienteContratoRepo = new Conexao<ClienteContrato>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoIntegracoesExt = new Conexao<IntegracoesExternas>(session))
            {
                if (String.IsNullOrEmpty(form.Base64))
                {
                    throw new Exception("Exceção por vazio");
                }
                else
                {

                    var clienteContrato = await clienteContratoRepo.selectFirst(x => x.Id == form.IdContrato);

                    string retornoURL = null;
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}/Arquivos/Cliente/{clienteContrato.Cliente.Id}/_tempLivenessDetect";

                    string base64 = form.Base64.Replace("data:video/webm;base64,", "");
                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var bytes = Convert.FromBase64String(base64);
                    using (var imageFile = new FileStream(Path.Combine(_pasta, $@"{form.IdContrato}.webm"), FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/Cliente/{clienteContrato.Cliente.Id}/_tempLivenessDetect/{form.IdContrato}.webm";

                    DetectRequest detect = new DetectRequest
                    {
                        fileUrl = retornoURL,
                        fileBase64 = null,
                        movements = new List<object>(),
                        returnFullFace = "true"
                    };

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MOSTQI);


                    var config = await repoConfig.selectFirst();
                    var retornoIntegracao = await new IntegracaoProvaVidaRepository().Detect(new MostqLivenessDetect(externas.TokenAcesso), detect);

                    if (retornoIntegracao.status.code == "200")
                    {
                        if (!String.IsNullOrEmpty(retornoIntegracao.result.frontalImage))
                        {

                            if ((int)(Convert.ToInt32(retornoIntegracao.result.imageScore) * 100) >= config.Porcentagem_Video)
                            {
                                String _pastaDocumento = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\LivenessDetect";

                                if (!Directory.Exists(_pastaDocumento))
                                {
                                    Directory.CreateDirectory(_pastaDocumento);
                                }

                                string fileName = form.IdContrato + ".txt";
                                string filePath = Path.Combine(_pastaDocumento, fileName);
                                System.IO.File.WriteAllText(filePath, retornoIntegracao.result.frontalImage, Encoding.UTF8);

                                //Exclui o arquivo temp
                                string arquivoExcluir = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\_tempLivenessDetect\{form.IdContrato}.webm";
                                File.Delete(arquivoExcluir);
                                resposta.result = true;
                            }
                            else
                            {
                                resposta.result = false;
                                resposta.message = "Não foi possível detectar os movimentos e/ou um rosto no vídeo. Por favor, refaça a gravação do vídeo seguindo as instruções.";
                            }
                        }
                        else
                        {
                            resposta.result = false;
                            resposta.message = "Ocorreu um erro ao processar a imagem, por favor, tente novamente";
                        }
                    }
                    else
                    {
                        resposta.result = false;
                        resposta.message = $@"Ocorreu um erro ao processar o vídeo. Pedimos desculpas pela inconveniência. Por favor, entre em contato com o administrador do sistema e forneça o seguinte código de referência para que possamos ajudá-lo a resolver o problema: {retornoIntegracao.status.code}";
                    }
                }
            }

            return resposta;
        }

        [HttpPost()]
        [Route("api/clienteapi/EnviarDocumentacaoMostQIExtractContent")]
        public async Task<Response> EnviarDocumentacaoMostQIExtractContent([FromBody] ContratoFormularioClienteModel form)
        {
            Response resposta = new Response();


            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var clienteContratoRepo = new Conexao<ClienteContrato>(session))
            using (var repoIntegracoesExt = new Conexao<IntegracoesExternas>(session))
            {
                if (!String.IsNullOrEmpty(form.InputBase64Content) || !String.IsNullOrEmpty(form.Base64))
                {

                    var clienteContrato = await clienteContratoRepo.selectFirst(x => x.Id == form.IdContrato);

                    string retornoURL = null;
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\_tempContent";

                    string base64 = null;

                    //Recebeu um anexo
                    if (!String.IsNullOrEmpty(form.InputBase64Content))
                    {

                        base64 = form.InputBase64Content.Replace($"data:{form.InputBase64Type};base64,", "");
                    }

                    //Recebeu uma foto
                    else
                    {
                        base64 = form.Base64.Replace($"data:{form.InputBase64Type};base64,", "");
                    }


                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var bytes = Convert.FromBase64String(base64);

                    var ext = form.InputBase64Type == "application/pdf" ? "pdf" : "png";

                    using (var imageFile = new FileStream(Path.Combine(_pasta, $@"{form.IdContrato}.{ext}"), FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/Cliente/{clienteContrato.Cliente.Id}/_tempContent/{form.IdContrato}.{ext}";

                    ContentExtractionRequest request = new ContentExtractionRequest
                    {
                        fileUrl = retornoURL,
                        fileBase64 = null,
                        qrCodeField = null
                    };

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MOSTQI);

                    var config = await repoIntegracoesExt.select();
                    var retornoIntegracao = await new IntegracaoValidacaoDocumentoRepository().Extract(new MostqContentExtraction(externas.TokenAcesso), request);


                    if (retornoIntegracao.status.code == "200")
                    {
                        if (retornoIntegracao.result.Count == 0)
                        {
                            resposta.result = false;
                            resposta.message = "Não foi encontrado um documento válido na foto, por favor, tire a foto novamente e certifique-se de que ela esteja visível e enquadrada.";
                        }
                        else
                        {
                            String _pastaDocumento = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\Documentacao";

                            if (!Directory.Exists(_pastaDocumento))
                            {
                                Directory.CreateDirectory(_pastaDocumento);
                            }

                            string fileName = form.IdContrato + ".txt";
                            string filePath = Path.Combine(_pastaDocumento, fileName);
                            System.IO.File.WriteAllText(filePath, base64, Encoding.UTF8);

                            resposta = await EnviarFaceMatch(form);

                        }
                    }
                    else
                    {
                        resposta.result = false;
                        resposta.message = $@"O processamento da sua solicitação encontrou um erro inesperado. Pedimos desculpas pela inconveniência. Por favor, entre em contato com o administrador do sistema e forneça o seguinte código de referência para que possamos ajudá-lo a resolver o problema: {retornoIntegracao.status.code}";
                    }
                }
                else
                {
                    resposta.result = false;
                    resposta.message = "Arquivo de imagem não encontrado, por favor, tente novamente.";
                }
            }

            return resposta;
        }


        [HttpPost()]
        [Route("api/clienteapi/EnviarFaceMatch")]
        public async Task<Response> EnviarFaceMatch(ContratoFormularioClienteModel form)
        {
            Response resposta = new Response();


            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var clienteContratoRepo = new Conexao<ClienteContrato>(session))
            using (var repoIntegracoesExt = new Conexao<IntegracoesExternas>(session))
            {
                var clienteContrato = await clienteContratoRepo.selectFirst(x => x.Id == form.IdContrato);
                //LivenessDetect
                String caminhoLiveness = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\LivenessDetect\{form.IdContrato}.txt";
                string base64Liveness = File.ReadAllText(caminhoLiveness, Encoding.UTF8);

                //Doc
                String caminhoDoc = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\Documentacao\{form.IdContrato}.txt";
                string base64Doc = File.ReadAllText(caminhoDoc, Encoding.UTF8);

                var request = new FaceCompareRequest
                {
                    faceFileBase64A = base64Liveness,
                    faceFileBase64B = base64Doc
                };

                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                IntegracoesExternas externas =
                integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MOSTQI);


                var config = await repoConfig.selectFirst();
                var retornoIntegracao = await new IntegracaoReconhecimentoFacialRepository().FaceCompare(new MostqFaceMatch(externas.TokenAcesso), request);

                if (retornoIntegracao.status.code == "200")
                {
                    if ((int)(Convert.ToInt32(retornoIntegracao.result.similarity) * 100) >= config.Porcentagem_Similaridade)
                    {
                        resposta.result = true;
                    }
                    else
                    {
                        resposta.result = false;
                        resposta.message = "A similaridade detectada não é suficiente. Por favor, refaça a gravação do vídeo.";
                    }
                }
                else
                {
                    resposta.result = false;
                    resposta.message = "Ocorreu um erro ao enviar imagens, por favor, tente novamente.";
                }
            }

            return resposta;
        }

        [HttpPost()]
        [Route("api/clienteapi/ExcluirArquivosTemp")]
        public async Task ExcluirArquivosTemp(ExcluirTempContrato value)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var clienteContratoRepo = new Conexao<ClienteContrato>(session))
            {
                var clienteContrato = await clienteContratoRepo.selectFirst(x => x.Id == value.idContrato);
                //LivenessDetect
                String caminhoLiveness = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\LivenessDetect\{value.idContrato}.txt";
                File.Delete(caminhoLiveness);

                //Doc
                String caminhoDoc = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}\Documentacao\{value.idContrato}.txt";
                File.Delete(caminhoDoc);
            }
        }

        [HttpGet()]
        [Route("api/clienteapi/GetPendenciaContrato")]
        public async Task<GetApiRetorno<ConsultaContratoModel>> GetPendenciaTermo(long Id)
        {
            var retorno = new GetApiRetorno<ConsultaContratoModel>();
            retorno.Objeto = new ConsultaContratoModel();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repoTermo = new Conexao<TermoAdesao>(session))
                {
                    using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                    {
                        IConfiguracoesGeraisRepository repository = new ConfiguracoesGeraisRepository(session);
                        var business = new ConfiguracoesGeraisBusiness(repository);
                        var config = business.GetConfiguracoesGerais();

                        var clienteContrato = await repoClienteContrato.selectFirst(x => x.Id == Id);

                        if (config.Utiliza_Certificacao_Assinatura)
                        {
                            Consultar_Documento_ResponseModel retornoIntegracao = new Consultar_Documento_ResponseModel();

                            Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                            {
                                Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                                Token_Documento_Assinatura = clienteContrato.Token_Documento_Assinatura,
                                Id_Contrato = clienteContrato.Id,
                                Id_Cliente = clienteContrato.Cliente.Id
                            };

                            switch (config.Tipo_Integracao_Assinatura)
                            {
                                case TipoIntegracaoAssinaturaEnum.Lexio:
                                    retornoIntegracao = new IntegracaoAssinaturaRepository().Consultar_Documento(new Lexio(), session, configCert);

                                    if (!retornoIntegracao.IsAssinado)
                                    {
                                        retorno.Objeto.IntegracaoPendente = true;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            retorno.Objeto.IntegracaoPendente = false;
                        }
                    }
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                //retorno.IdObjeto.IdVenda = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }


        public async Task<string> GravarAnexo(ContratoFormularioClienteModel model, ClienteContrato clienteContrato, Conexao<Cliente_Anexo> repository)
        {
            string retornoURL = null;

            if (model == null || clienteContrato == null || string.IsNullOrEmpty(model.Base64))
            {
                return retornoURL;
            }

            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteContrato.Cliente.Id}";

            try
            {
                string base64 = model.Base64.Replace("data:image/jpeg;base64,", "");

                if (!Directory.Exists(_pasta))
                {
                    Directory.CreateDirectory(_pasta);
                }

                Cliente_Anexo anexo = new Cliente_Anexo();
                using (var session = HibernateUtil.GetSession())
                using (var repoCliente = new Conexao<Cliente>(session))
                {

                    var cliente = await repoCliente.selectFirst(x => x.Id == clienteContrato.Cliente.Id);

                    anexo.DataHoraCriacao = DateTime.Now;
                    anexo.Nome_Arquivo = clienteContrato.Id.ToString();
                    anexo.Extensao_Arquivo = "jpeg";
                    anexo.Cliente = cliente;
                    anexo.ClienteContrato = clienteContrato;
                    repository.Save(anexo).Wait();


                    var bytes = Convert.FromBase64String(base64);

                    string filePath = Path.Combine(_pasta, $"{anexo.Id}.jpeg");

                    using (var imageFile = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/Cliente/{clienteContrato.Cliente.Id}/{anexo.Id}{anexo.Extensao_Arquivo}";
                    anexo.Arquivo = File.ReadAllBytes(filePath);
                    repository.SaveOrUpdate(anexo).Wait();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro: {ex.Message}");
            }

            return retornoURL;
        }

        [HttpPost()]
        [Route("api/clienteapi/gravarVenda")]
        public async Task<GetApiRetorno<object>> gravarVenda(ParamWebApi<ClienteDadosGeraisViewModel> value)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var apiEndereco = new EnderecoApiController())
                using (var apiClienteProspecto = new Cliente_ProspectoApiController())
                using (var apiVenda = new VendaApiController())
                using (var apiDesconto = new DescontoApiController())
                using (var apiPlanoProduto = new PlanoProdutoApiController())
                using (var apiTermoAdesao = new Termo_AdesaoApiController())
                using (var apiPlano = new PlanoApiController())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        ClienteMobileModel modelo = new ClienteMobileModel();
                        #region Cliente
                        modelo.idCliente = value.Objeto.Id_Cliente;
                        modelo.cargoResponsavel = value.Objeto.Cargo_Responsavel;
                        modelo.celular = value.Objeto.Telefone_Celular;
                        modelo.codArea = value.Objeto.Cod_Area;
                        modelo.codEscritorio = value.Objeto.Cod_Escritorio;
                        modelo.codLocalidade = value.Objeto.Cod_Localidade;


                        //modelo.regime = HtmlEnumExtensions.GetEnumDescription<Regime_Anatel>(Convert.ToString(Plano?.Re?.Pais));

                        ///modelo.Regime = value.Objeto.Regime;
                        //modelo.Modelo_Anatel = value.Objeto.Modelo_Anatel;
                        modelo.cpfCnpj = value.Objeto.CpfCnpj;
                        modelo.cpfResponsavel = value.Objeto.CPFResponsavel;

                        modelo.dataEmissao = value.Objeto.DataEmissao;
                        modelo.dataNascimento = value.Objeto.DataNascimento;
                        modelo.dataAbertura = value.Objeto.Data_Abertura;
                        modelo.descCodArea = value.Objeto.Desc_Cod_Area;
                        modelo.descCodEscritorio = value.Objeto.Desc_Cod_Escritorio;
                        modelo.descCodLocalidade = value.Objeto.Desc_Cod_Localidade;

                        modelo.email = value.Objeto.Email;
                        modelo.enderecoPrincipal = value.Objeto.EnderecoPrincipal;
                        //modelo.isEndCobrancaIgualPrincipal = value.Objeto.End_Cobranca_Igual_Principal;
                        modelo.enderecoCobranca = value.Objeto.EnderecoCobranca;
                        modelo.isEndInstalacaoIgualPrincipal = value.Objeto.End_Instalacao_Igual_Principal;
                        modelo.enderecoInstalacao = value.Objeto.EnderecoInstalacao;
                        modelo.estadoCivil = value.Objeto.EstadoCivil;

                        modelo.idCategoriaCliente = value.Objeto.Id_Categoria_Cliente;
                        modelo.idMeioCaptacao = value.Objeto.Id_Meio_Captacao;
                        modelo.idModeloContrato = value.Objeto.Id_Modelo_Contrato;
                        modelo.idQuemIndicou = value.Objeto.Id_Quem_Indicou;
                        modelo.inscricaoEstadual = value.Objeto.Inscricao_Estadual;
                        modelo.inscricaoMunicipal = value.Objeto.Inscricao_Municipal;
                        modelo.instrucao = value.Objeto.Instrucao;

                        modelo.mae = value.Objeto.Mae;
                        modelo.meioCaptacao = value.Objeto.Meio_Captacao;

                        modelo.nacionalidade = value.Objeto.Nacionalidade;
                        modelo.nomeCompleto = value.Objeto.NomeCompleto;
                        modelo.nomeFantasia = value.Objeto.NomeFantasia;
                        modelo.nomeRecado = value.Objeto.NomeRecado;
                        modelo.numeroContrato = value.Objeto.Id_Cliente > 0 ? value.Objeto.Numero_Contrato : null;

                        modelo.orgao = value.Objeto.Orgao;

                        modelo.pai = value.Objeto.Pai;
                        modelo.porte = value.Objeto.Porte;
                        modelo.profissao = value.Objeto.Profissao;

                        modelo.qualMeio = value.Objeto.Qual_Meio;
                        modelo.quemIndicou = value.Objeto.Quem_Indicou;

                        modelo.ramoAtividade = value.Objeto.Ramo_Atividade;
                        modelo.razaoSocial = value.Objeto.Razao_Social;
                        modelo.representante = value.Objeto.Representante;
                        modelo.rg = value.Objeto.RG;

                        modelo.sexo = value.Objeto.Sexo;
                        modelo.statusCliente = value.Objeto.Status;
                        modelo.statusProspecto = value.Objeto.Status_Prospecto;

                        modelo.telefoneFixo = value.Objeto.TelefoneFixo;
                        //modelo.telefonePrincipal = value.Objeto.TelefonePrincipal;
                        modelo.telefoneRecado = value.Objeto.TelefoneRecado;
                        modelo.tipoCliente = value.Objeto.TipoCliente;
                        modelo.tipoPessoa = value.Objeto.TipoPessoa;
                        #endregion

                        if (value.Objeto.EnderecoPrincipal.Latitude == null && value.Objeto.EnderecoPrincipal.Longitude == null)
                        {
                            var end = value.Objeto.EnderecoPrincipal;
                            object LatLong = apiEndereco.GetLatLong(end.Logradouro, end.Numero, end.Pais, end.UF, end.Cidade, end.Bairro);

                            if (LatLong.GetType() != typeof(String))
                            {
                                modelo.enderecoPrincipal.Latitude = ((dynamic)LatLong).lat;
                                modelo.enderecoPrincipal.Longitude = ((dynamic)LatLong).lng;
                            }
                        }
                        if (value.Objeto.EnderecoCobranca.Latitude == null && value.Objeto.EnderecoCobranca.Longitude == null)
                        {
                            var end = value.Objeto.EnderecoCobranca;
                            object LatLong = apiEndereco.GetLatLong(end.Logradouro, end.Numero, end.Pais, end.UF, end.Cidade, end.Bairro);

                            if (LatLong.GetType() != typeof(String))
                            {
                                modelo.enderecoCobranca.Latitude = ((dynamic)LatLong).lat;
                                modelo.enderecoCobranca.Longitude = ((dynamic)LatLong).lng;
                            }
                        }

                        if (value.Objeto.Planos.Count > 0)
                        {
                            foreach (PlanoProdutoListModel p in value.Objeto.Planos)
                            {
                                if (p.End_Instalacao_Igual_Principal == false && p.Latitude == null && p.Longitude == null)
                                {
                                    object LatLong = apiEndereco.GetLatLong(p.Logradouro, p.Numero, value.Objeto.EnderecoPrincipal.Pais, p.UF, p.Cidade, p.Bairro);

                                    if (LatLong.GetType() != typeof(String))
                                    {
                                        p.Latitude = ((dynamic)LatLong).lat;
                                        p.Longitude = ((dynamic)LatLong).lng;
                                    }
                                }
                            }
                        }

                        ParamWebApi<Cliente_ProspectoDadosGeraisViewModel> paramProspecto = new ParamWebApi<Cliente_ProspectoDadosGeraisViewModel>();
                        paramProspecto.Objeto = modelo.clienteProspectoDados;
                        paramProspecto.Token = value.Token;
                        paramProspecto.Usuario = value.Usuario;

                        var forma_Pagamento_Cliente = value.Objeto.Forma_Pagamento_Cliente;
                        value.Objeto.Formas_Pagamento.Add(forma_Pagamento_Cliente);

                        modelo.idCliente = await apiClienteProspecto.GravarProspecto(session, sessionT5, paramProspecto);

                        //Chamar VendaApiController/Gravar

                        #region Vendas
                        modelo.idVenda = value.Objeto.Id;
                        //modelo.agencia = value.Objeto.Agencia;
                        modelo.confirmarSenha = value.Objeto.Confirmar_Senha;
                        //modelo.contaCorrente = value.Objeto.Conta_Corrente;
                        //modelo.digitoDaConta = value.Objeto.Digito;
                        //modelo.dataVenda = DateTime.Now.ToShortDateString(); --> Não é para preencher
                        //modelo.descBanco = value.Objeto.Desc_Banco;
                        modelo.descontos = value.Objeto.Descontos.Where(w => !w.Excluir).ToList();
                        //modelo.idBanco = value.Objeto.Id_Banco;
                        modelo.idCampanha = value.Objeto.Id_Categoria_Cliente;
                        //modelo.forma_envio = value.Objeto.Forma_Envio;


                        if (value.Objeto.Id_Desconto != null) modelo.idDesconto = value.Objeto.Id_Desconto.Value;

                        //modelo.idFormaPagamento = value.Objeto.Id_Forma_Pagamento;
                        modelo.idPlano = value.Objeto.Id_Plano;




                        //modelo.idVencimento = value.Objeto.Id_Vencimento;
                        modelo.login = value.Objeto.Login;
                        modelo.mesesCarencia = value.Objeto.Meses_Carencia;
                        modelo.numeroContrato = value.Objeto.Numero_Contrato;
                        modelo.numeroParcelasInstalacao = value.Objeto.Numero_Parcelas_Instalacao;
                        modelo.periodoCarencia = value.Objeto.Periodo_Carencia;
                        modelo.planos = value.Objeto.Planos.Select(s =>
                        {
                            s.ValorPlanoProduto_Final = !String.IsNullOrEmpty(s.ValorPlanoProduto) ? s.ValorPlanoProduto : "0.0";
                            s.ValorInstalacao_Final = !String.IsNullOrEmpty(s.ValorInstalacao) ? s.ValorInstalacao : "0.0";
                            s.ValorTotal = (Convert.ToDouble(s.ValorPlanoProduto_Final) + Convert.ToDouble(s.ValorInstalacao_Final)).ToString();

                            if (s.Id_Forma_De_Pagamento == 0 || !s.Id_Forma_De_Pagamento.HasValue)
                                s.Id_Forma_De_Pagamento = value.Objeto.Forma_Pagamento_Cliente.Id_Forma_Pagamento;

                            return s;
                        }).ToList();

                        modelo.qtdeDias = value.Objeto.Qtde_Dias;
                        modelo.senha = value.Objeto.Senha;
                        modelo.testeGratis = value.Objeto.TesteGratis;
                        modelo.valorFinal = Convert.ToString(value.Objeto.Valor_Final);
                        modelo.valorInstalacaoFinal = Convert.ToString(value.Objeto.Valor_Instalacao_Final);
                        modelo.id_Forma_Pagamento_Cliente_Preferida = value.Objeto.Id_Forma_Pagamento_Cliente_Preferida.Value;
                        modelo.Id_Caixa_Mais_Proxima = value.Objeto.IdCaixaMaisProxima;
                        modelo.Consulta_Viabilidade_Realizada = value.Objeto.ConsultaViabilidadeRealizada;


                        // var listaFormasPagamentoClienteModels = new List<Forma_Pagamento_ClienteModels>();
                        //listaFormasPagamentoClienteModels.Add(value.Objeto.Forma_Pagamento_Cliente);

                        // modelo.formasPagamento = listaFormasPagamentoClienteModels;

                        using (var repoUsusario = new Conexao<IdentityUser>(session))
                        {
                            IdentityUser u = (await repoUsusario.selectFirst(p => p.Id == value.Usuario));
                            modelo.idVendedor = u?.Funcionario?.Id;
                            modelo.idSupervisor = u?.Funcionario?.Supervisor?.Id;
                        }
                        #endregion

                        ParamWebApi<VendaDadosGeraisViewModel> paramVendas = new ParamWebApi<VendaDadosGeraisViewModel>();
                        paramVendas.Objeto = modelo.vendasDados;
                        paramVendas.Token = value.Token;
                        paramVendas.Usuario = value.Usuario;

                        paramVendas.Objeto.FormaPagamentoVenda.Id = value.Objeto.Formas_Pagamento.FirstOrDefault(x => x.Id > 0).Id;

                        long Id_Venda = (await apiVenda.GravarVenda(session, paramVendas)).IdObjeto;

                        foreach (DescontoListaModel d in value.Objeto.Descontos.Where(w => w.Excluir).ToList())
                        {
                            await apiDesconto.ExcluirGrid(d.Id);
                        }

                        transaction.Commit();
                        retorno.Ok = true;
                        retorno.Objeto = new
                        {
                            idVenda = Id_Venda,
                            modelo.idCliente
                        };

                        foreach (PlanoProdutoListModel p in value.Objeto.Planos.FindAll(plano => plano.Id != 0 && plano.IsAlterado))
                        {
                            var end = new EnderecoClienteViewModel();


                            end.Id = p.Id_Endereco;
                            end.EnderecoInstalacaoIgualPrincipal = p.End_Instalacao_Igual_Principal;

                            if (p.End_Instalacao_Igual_Principal)
                            {
                                var endPrincipal = modelo.enderecoPrincipal;
                                end.Bairro = endPrincipal.Bairro;
                                end.CEP = endPrincipal.CEP;
                                end.Cidade = endPrincipal.Cidade;
                                end.Complemento = endPrincipal.Complemento;
                                end.Id_Bairro = endPrincipal.Id_Bairro;
                                end.Id_Cidade = endPrincipal.Id_Cidade;
                                end.Id_Pais = endPrincipal.Id_Pais;
                                end.Id_Tipo_Imovel = (int)endPrincipal.Id_Tipo_Imovel;
                                end.Latitude = endPrincipal.Latitude;
                                end.Logradouro = endPrincipal.Logradouro;
                                end.Longitude = endPrincipal.Longitude;
                                end.Localidade = endPrincipal.Localidade;
                                end.Numero = endPrincipal.Numero;
                                end.Pais = endPrincipal.Pais;
                                end.UF = endPrincipal.UF;
                            }
                            else
                            {
                                end.Bairro = p.Bairro;
                                end.CEP = p.CEP;
                                end.Cidade = p.Cidade;
                                end.Complemento = p.Complemento;
                                end.Id_Bairro = p.Id_Bairro;
                                end.Id_Cidade = p.Id_Cidade;
                                end.Id_Endereco = p.Id_Endereco;
                                end.Id_Pais = p.Id_Pais;
                                end.Id_Tipo_Imovel = (int)p.Id_Tipo_Imovel; // O cast foi necessário porque a classe EnderecoClienteViewModel no campo Id_Tipo_Imovel pede um int
                                end.Latitude = p.Latitude;
                                end.Logradouro = p.Logradouro;
                                end.Longitude = p.Longitude;
                                end.Localidade = p.Localidade;
                                end.Numero = p.Numero;
                                end.Pais = value.Objeto.EnderecoPrincipal.Pais;
                                end.UF = p.UF;
                            }

                            end.Cod_Area = p.Cod_Area;
                            end.Cod_Escritorio = p.Cod_Escritorio;
                            end.Cod_Localidade = p.Cod_Localidade?.ToString();

                            var valueEndereco = new ParamWebApi<EnderecoClienteViewModel>();
                            valueEndereco.Objeto = end;

                            await GravarEndereco(valueEndereco);

                            var plano = new PlanoProdutoListModel();
                            plano.Id = p.Id;
                            plano.Qtde_Dias = p.Qtde_Dias;
                            plano.Numero_Parcelas = p.Numero_Parcelas;
                            plano.Meses_Carencia = p.Meses_Carencia;

                            await apiPlanoProduto.AlterarDetalhesPlanoProduto(plano, forma_Pagamento_Cliente, modelo.idCliente);
                        }
                        var urlAssinar = (await apiTermoAdesao.GerarTermoAdesaoIntegracao(Id_Venda)).Objeto;
                        retorno.Objeto = new
                        {
                            idVenda = Id_Venda,
                            modelo.idCliente,
                            urlAssinar
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Objeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }


            return retorno;

        }


        [HttpGet()]
        [Route("api/clienteapi/notificaSupervisor")]
        public async Task notificaSupervisor(long? id_venda)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            {

                var venda = await repoVenda.selectFirst(x => x.Id == id_venda);
                var cliente = await repoCliente.selectFirst(x => x.Id == venda.Id_Cliente);
                String msg = "";
                switch (venda.Status)
                {
                    case Status_Venda.Venda_Aprovada:
                        msg = String.Format(Resources.RsBase.NOTIFICACAO_SUPERVISOR_APROV, venda.Vendedor.Id, venda.Vendedor.Nome, venda.Id, cliente.CpfCnpj, cliente.NomeFantasia);
                        break;
                    case Status_Venda.Venda_Cancelada:
                        msg = String.Format(Resources.RsBase.NOTIFICACAO_SUPERVISRO_REPROV, venda.Vendedor.Id, venda.Vendedor.Nome, venda.Id, cliente.CpfCnpj, cliente.NomeFantasia);
                        break;
                }

                if (!String.IsNullOrEmpty(msg))
                {
                    if (venda.Supervisor != null)
                    {
                        await new NotificaMobileApiController(session).Notifica(venda.Supervisor.Id, "Venda", "Acompanhamento Venda", msg, null);
                    }

                }


            }
        }

        [HttpPost()]
        [Route("api/clienteapi/getWorkflowOptions")]
        public async Task<GetApiRetorno<IList<dynamic>>> getWorkflowOptions(ParamWebApi<long> id)
        {
            GetApiRetorno<IList<dynamic>> retorno = new GetApiRetorno<IList<dynamic>>();
            retorno.Objeto = new List<dynamic>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoWorkflowAtividades = new Conexao<Workflow_VendasAtividade>(session))
                using (var repoVendas = new Conexao<Venda>(session))
                {
                    var vendas = await repoVendas.selectFirst(p => p.Id == id.Objeto);
                    var workflowVendasAtividade = await repoWorkflowAtividades.select(p => p.WorkFlow.Id == vendas.Workflow_Vendas.Id);

                    foreach (var item in workflowVendasAtividade)
                    {
                        var workflowList = new
                        {
                            item.Id,
                            item.Ativ_Decisao,
                            item.Ativ_Descricao,
                            item.Ativ_StatusVenda,
                            item.Ativ_NAtivAprovada,
                            item.Ativ_NAtivReprovada,
                            //item.StatusPadrao,
                            item.Ativ_Status,
                            //item.Id_Status_Padrao
                        };

                        retorno.Objeto.Add(workflowList);
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException.ToString();
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        /*[HttpPost()]
        [Route("api/clienteapi/getWorkflowOptions")]
        public async Task<Response<IList<dynamic>>> getWorkflowOptions(ParamWebApi<long> id)
        {
            Response<IList<dynamic>> retorno = new Response<IList<dynamic>>();
            retorno.valor = new List<dynamic>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoWorkflowAtividades = new Conexao<Workflow_VendasAtividade>(session))
                using (var repoVendas = new Conexao<Venda>(session))
                {
                    var vendas = await repoVendas.selectFirst(p => p.Id == id.Objeto);
                    var workflowVendasAtividade = await repoWorkflowAtividades.select(p => p.WorkFlow.Id == vendas.Workflow_Vendas.Id);

                    foreach (var item in workflowVendasAtividade)
                    {
                        var workflowList = new
                        {
                            item.Id,
                            item.Ativ_Decisao,
                            item.Ativ_Descricao,
                            item.Ativ_StatusVenda,
                            item.Ativ_NAtivAprovada,
                            item.Ativ_NAtivReprovada,
                            item.StatusPadrao,
                            item.Ativ_Status,
                            item.Id_Status_Padrao
                        };
                        retorno.valor.Add(workflowList);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.result = false;
                retorno.message = ex.InnerException.Message.ToString();
            }

            return retorno;
        }*/

        [HttpPost()]
        [Route("api/clienteapi/getWorkflowAtividade")]
        public async Task<GetApiRetorno<dynamic>> getWorkflowAtividade(ParamWebApi<long> id)
        {
            GetApiRetorno<dynamic> retorno = new GetApiRetorno<dynamic>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoWorkflowAtividades = new Conexao<Workflow_VendasAtividade>(session))
                {
                    var venda = await repoVenda.selectFirst(p => p.Id == id.Objeto);
                    if (venda.Workflow_VendasAtividade != null)
                    {
                        var workflowVendasAtividade = await repoWorkflowAtividades.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Id);

                        retorno.Objeto = new
                        {
                            workflowVendasAtividade.Id,
                            workflowVendasAtividade.Ativ_Decisao,
                            workflowVendasAtividade.Ativ_Descricao,
                            workflowVendasAtividade.Ativ_StatusVenda,
                            workflowVendasAtividade.Ativ_NAtivAprovada,
                            workflowVendasAtividade.Ativ_NAtivReprovada,
                            //workflowVendasAtividade.StatusPadrao,
                            workflowVendasAtividade.Ativ_Status,
                            //workflowVendasAtividade.Id_Status_Padrao
                        };
                        retorno.Ok = true;
                    }
                    else
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = "Venda sem Workflow cadastrado.";
                    }
                }

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException.Message.ToString();
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/setWorkflowStatus")]
        public async Task<GetApiRetorno<Boolean>> setWorkflowStatus(ParamWebApi<setWorkflowStatus> value)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoVenda = new Conexao<Venda>(session))
                {
                    var venda = await repoVenda.selectFirst(p => p.Id == value.Objeto.idVenda);

                    venda.StatusVenda = value.Objeto.proximaEtapaWorkflow.Ativ_StatusVenda.ToString();
                    //venda.Status = (Status_Venda)value.Objeto.proximaEtapaWorkflow.Id_Status_Padrao;
                    venda.Status = (Status_Venda)value.Objeto.proximaEtapaWorkflow.Ativ_StatusVenda.StatusPadrao;

                    await repoVenda.SaveOrUpdate(venda);
                    retorno.Ok = true;
                }


            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException.ToString();
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        #endregion

        [HttpGet()]
        [Route("api/clienteapi/getVenda")]
        public async Task<GetApiRetorno<ClienteDadosGeraisViewModel>> getVenda(long idVenda)
        {
            GetApiRetorno<ClienteDadosGeraisViewModel> retorno = new GetApiRetorno<ClienteDadosGeraisViewModel>();


            retorno.Objeto = new ClienteDadosGeraisViewModel();

            using (var apiVenda = new VendaApiController())
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            {
                Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());
                try
                {
                    GetApiRetorno<VendaDadosGeraisViewModel> venda = await apiVenda.FindById(idVenda, null);


                    if (!venda.Ok)
                    {
                        retorno.Ok = false;
                        retorno.InnerException = venda.InnerException;
                        retorno.Mensagem = venda.Mensagem;
                    }
                    else
                    {
                        //retorno.Objeto.Agencia = venda.Objeto.Agencia;
                        retorno.Objeto.Cargo_Responsavel = venda.Objeto.Cargo_Responsavel;
                        retorno.Objeto.Telefone_Celular = venda.Objeto.TelefoneCelular;
                        retorno.Objeto.Cod_Area = venda.Objeto.Cod_Area;
                        retorno.Objeto.Cod_Escritorio = venda.Objeto.Cod_Escritorio;
                        try
                        {
                            retorno.Objeto.Cod_Localidade = venda.Objeto.Cod_Localidade.Value.ToString() ?? "";
                        }
                        catch
                        {
                            retorno.Objeto.Cod_Localidade = "";
                        }

                        retorno.Objeto.Confirmar_Senha = venda.Objeto.Confirmar_Senha;

                        //retorno.Objeto.Conta_Corrente = venda.Objeto.Conta_Corrente;
                        retorno.Objeto.CpfCnpj = venda.Objeto.CpfCnpj;
                        retorno.Objeto.CPFResponsavel = venda.Objeto.CPFResponsavel;
                        retorno.Objeto.DataEmissao = venda.Objeto.DataEmissao;
                        retorno.Objeto.DataNascimento = venda.Objeto.DataNascimento;
                        retorno.Objeto.Data_Abertura = venda.Objeto.Data_Abertura;
                        retorno.Objeto.Descontos = venda.Objeto.Descontos;
                        //retorno.Objeto.Desc_Banco = venda.Objeto.Desc_Banco;
                        retorno.Objeto.Desc_Cod_Area = venda.Objeto.Desc_Cod_Area;
                        retorno.Objeto.Desc_Cod_Escritorio = venda.Objeto.Desc_Cod_Escritorio;
                        retorno.Objeto.Desc_Cod_Localidade = venda.Objeto.Desc_Cod_Localidade;
                        retorno.Objeto.Email = venda.Objeto.Email;
                        retorno.Objeto.EnderecoCobranca = venda.Objeto.EnderecoCobranca;
                        retorno.Objeto.EnderecoInstalacao = venda.Objeto.EnderecoInstalacao;
                        retorno.Objeto.EnderecoPrincipal = venda.Objeto.EnderecoPrincipal;
                        //retorno.Objeto.End_Cobranca_Igual_Principal = venda.Objeto.End_Cobranca_Igual_Principal;
                        retorno.Objeto.End_Instalacao_Igual_Principal = venda.Objeto.End_Instalacao_Igual_Principal;
                        retorno.Objeto.EstadoCivil = venda.Objeto.EstadoCivil;
                        retorno.Objeto.Id = venda.Objeto.Id;
                        //retorno.Objeto.Id_Banco = venda.Objeto.Id_Banco;
                        retorno.Objeto.Id_Campanha = venda.Objeto.Id_Categoria_Cliente;
                        retorno.Objeto.Identificacao_Cliente = venda.Objeto.Id_Cliente.ToString();
                        retorno.Objeto.Id_Desconto = venda.Objeto.Id_Desconto;
                        //retorno.Objeto.Id_Forma_Pagamento = venda.Objeto.Id_Forma_Pagamento;
                        retorno.Objeto.Id_Meio_Captacao = venda.Objeto.Id_Meio_Captacao;
                        retorno.Objeto.Id_Modelo_Contrato = venda.Objeto.Id_Modelo_Contrato;
                        retorno.Objeto.Id_Plano = venda.Objeto.Id_Plano;


                        retorno.Objeto.Id_Quem_Indicou = venda.Objeto.Id_Quem_Indicou;
                        //retorno.Objeto.Id_Vencimento = venda.Objeto.Id_Vencimento;
                        retorno.Objeto.Inscricao_Estadual = venda.Objeto.Inscricao_Estadual;
                        retorno.Objeto.Inscricao_Municipal = venda.Objeto.Inscricao_Municipal;
                        retorno.Objeto.Instrucao = venda.Objeto.Instrucao;

                        //retorno.Objeto.Login = venda.Objeto.Login;
                        //retorno.Objeto.Senha = venda.Objeto.Senha;

                        if (config.Padrao_Login == "PrimeiroNome.UltimoNome" && config.Padrao_Login_Ultimo == null)
                        {
                            retorno.Objeto.Login = venda.Objeto.NomeFantasia.Substring(0, venda.Objeto.NomeFantasia.IndexOf(" ")) + "." + venda.Objeto.NomeFantasia.Substring(venda.Objeto.NomeFantasia.LastIndexOf(" "), venda.Objeto.NomeFantasia.IndexOf(" "));
                            retorno.Objeto.Senha = venda.Objeto.Senha;
                        }
                        retorno.Objeto.Mae = venda.Objeto.Mae;
                        retorno.Objeto.Meio_Captacao = venda.Objeto.Meio_Captacao;
                        retorno.Objeto.Meses_Carencia = venda.Objeto.Meses_Carencia;
                        retorno.Objeto.Nacionalidade = venda.Objeto.Nacionalidade;
                        retorno.Objeto.NomeCompleto = venda.Objeto.NomeCompleto;
                        retorno.Objeto.NomeFantasia = venda.Objeto.NomeFantasia;
                        retorno.Objeto.NomeRecado = venda.Objeto.NomeRecado;
                        retorno.Objeto.Numero_Contrato = venda.Objeto.Numero_Contrato;
                        retorno.Objeto.Numero_Parcelas_Instalacao = venda.Objeto.Numero_Parcelas_Instalacao;
                        retorno.Objeto.Orgao = venda.Objeto.Orgao;
                        retorno.Objeto.Pai = venda.Objeto.Pai;
                        retorno.Objeto.Periodo_Carencia = venda.Objeto.Periodo_Carencia;
                        retorno.Objeto.Planos = venda.Objeto.Planos;
                        retorno.Objeto.Porte = venda.Objeto.Porte;
                        retorno.Objeto.Profissao = venda.Objeto.Profissao;
                        retorno.Objeto.Qtde_Dias = venda.Objeto.Qtde_Dias;
                        retorno.Objeto.Qual_Meio = venda.Objeto.Qual_Meio;
                        retorno.Objeto.Quem_Indicou = venda.Objeto.Quem_Indicou;
                        retorno.Objeto.Ramo_Atividade = venda.Objeto.Ramo_Atividade;
                        retorno.Objeto.Razao_Social = venda.Objeto.Razao_Social;
                        retorno.Objeto.Representante = venda.Objeto.Representante;
                        retorno.Objeto.RG = venda.Objeto.RG;

                        retorno.Objeto.Sexo = venda.Objeto.Sexo;
                        retorno.Objeto.TelefoneFixo = venda.Objeto.TelefoneFixo;

                        retorno.Objeto.TelefoneRecado = venda.Objeto.TelefoneRecado;
                        retorno.Objeto.TesteGratis = venda.Objeto.TesteGratis;
                        retorno.Objeto.TipoPessoa = venda.Objeto.TipoPessoa;
                        retorno.Objeto.TipoCliente = venda.Objeto.Tipo_Cliente == Tipo_Cliente.Cliente ? Tipo_Cliente.Cliente : Tipo_Cliente.Prospecto;
                        retorno.Objeto.Valor_Final = Convert.ToString(venda.Objeto.Valor_Final);
                        retorno.Objeto.Valor_Instalacao_Final = Convert.ToString(venda.Objeto.Valor_Instalacao_Final);
                        retorno.Objeto.Forma_PagamentoVenda = venda.Objeto.FormaPagamentoVenda;


                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.InnerException = ex.InnerException.ToString();
                    retorno.Mensagem = ex.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    throw ex;
                }
            }
            return retorno;
        }
        #endregion

        private void BindViewToObj(ISession session, ref Cliente_Anexo objBanco, ClienteAnexoModelView view)
        {
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            {
                objBanco.Cliente = repoCliente.selectFirst(p => p.Id == view.Id_Cliente).Result;
                objBanco.Titulo = view.Titulo;
                objBanco.Nome_Arquivo = view.Nome_Arquivo;
                objBanco.Extensao_Arquivo = view.Extensao_Arquivo;
                objBanco.Foto_Perfil = view.Foto_Perfil;
                objBanco.Venda = repoVenda.selectFirst(p => p.Id == view.Id_Venda).Result;
                objBanco.UsuarioCriacao = view.Usuario;
                objBanco.UsuarioAlteracao = view.Usuario;
                objBanco.DataHoraCriacao = DateTime.Now;
                objBanco.Arquivo = !String.IsNullOrEmpty(view.caminho_Arquivo) ? File.ReadAllBytes(view.caminho_Arquivo) : new byte[0];
            }
        }

        private ClienteDadosGeraisViewModel GetViewObjMobile(ISession session, Cliente obj)
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
            /*retorno.Forma_Envio = obj.Forma_Envio;

            if (obj.Forma_Pagamento != null)
            {
                retorno.Forma_Pagamento = new Forma_Pagamento()
                {
                    Id = obj.Forma_Pagamento.Id,
                    Tipo_Forma_Pagamento = obj.Forma_Pagamento.Tipo_Forma_Pagamento,
                    Descricao = obj.Forma_Pagamento?.Descricao,
                };
            }

            retorno.Id_Vencimento = obj.Vencimento?.Id;
            retorno.Dia_Vencimento = obj.Vencimento?.DiaVencimento.ToString();

            retorno.End_Cobranca_Igual_Principal = obj.End_Cobranca_Igual_Principal;*/

            retorno.Status = obj.Status == Status_Cliente.Ativo ? true : false;
            retorno.Email = obj.Email_Principal;
            retorno.TelefoneFixo = obj.Telefone_Fixo;
            retorno.Telefone_Celular = obj.Telefone_Celular;
            retorno.TelefoneRecado = obj.Telefone_Recado;
            retorno.NomeRecado = obj.Pessoa_Tel_Recado;
            //retorno.Desc_Banco = obj.Banco?.Descricao;
            retorno.Quem_Indicou = obj.Quem_Indicou?.NomeFantasia;

            retorno.Id_Categoria_Cliente = obj.Categoria_Cliente?.Id;
            retorno.Id_Meio_Captacao = obj.Qual_Meio?.Id;
            retorno.Meio_Captacao = obj.Qual_Meio?.Descricao;
            retorno.Id_Quem_Indicou = obj.Quem_Indicou?.Id;

            if (obj.Tipo_Cliente == Tipo_Cliente.Cliente)
            {

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
                    Id_Endereco = obj.Endereco_Principal?.Id ?? (Int64)0,
                    Latitude = Convert.ToString(obj.Endereco_Principal?.Latitude),
                    Logradouro = obj.Endereco_Principal?.Logradouro,
                    Longitude = Convert.ToString(obj.Endereco_Principal?.Longitude),
                    Numero = obj.Endereco_Principal?.Numero,
                    Ponto_Referencia = obj.Endereco_Principal?.Ponto_Referencia,
                    UF = obj.Endereco_Principal?.UF,
                    Tipo = Tipo_EnderecoCliente.Principal,
                    Id_Condominio_Predio = obj.Endereco_Principal?.Condominio_Predio?.Id,
                    Descricao_Condominio = obj.Endereco_Principal?.Condominio_Predio?.Descricao,
                    Id_Endereco_Logradouro = obj.Endereco_Principal?.Endereco_Logradouro?.Id,
                    Busca_Endereco = obj.Endereco_Principal?.Busca_Endereco,
                    TipoLogradouro = obj.Endereco_Principal?.Endereco_Logradouro?.Tipo ?? obj.Endereco_Principal?.Condominio_Predio?.Endereco_Logradouro?.Tipo
                };
            }
            else
            {
                retorno.EnderecoPrincipal = new EnderecoClienteViewModel
                {
                    Id_Pais = obj.Endereco_Prospecto?.Pais?.Id,
                    Id_Tipo_Imovel = obj.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Id_Bairro = obj.Endereco_Prospecto?.Id_Bairro,
                    Bairro = obj.Endereco_Prospecto?.Bairro,
                    CEP = obj.Endereco_Prospecto?.Cep,
                    Id_Cidade = obj.Endereco_Prospecto?.Id_Cidade,
                    Cidade = obj.Endereco_Prospecto?.Cidade,
                    Complemento = obj.Endereco_Prospecto?.Complemento,
                    Id_Endereco = obj.Endereco_Prospecto?.Id ?? (Int64)0,
                    Latitude = Convert.ToString(obj.Endereco_Prospecto?.Latitude),
                    Logradouro = obj.Endereco_Prospecto?.Logradouro,
                    Longitude = Convert.ToString(obj.Endereco_Prospecto?.Longitude),
                    Numero = obj.Endereco_Prospecto?.Numero,
                    Ponto_Referencia = obj.Endereco_Prospecto?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Prospecto,
                    UF = obj.Endereco_Prospecto?.UF,
                    Id_Condominio_Predio = obj.Endereco_Prospecto?.Condominio_Predio?.Id,
                    Descricao_Condominio = obj.Endereco_Prospecto?.Condominio_Predio?.Descricao,
                    Id_Endereco_Logradouro = obj.Endereco_Prospecto?.Endereco_Logradouro?.Id,
                    Busca_Endereco = obj.Endereco_Prospecto?.Busca_Endereco,
                    TipoLogradouro = obj.Endereco_Prospecto?.Endereco_Logradouro?.Tipo ?? obj.Endereco_Prospecto?.Condominio_Predio?.Endereco_Logradouro?.Tipo
                };
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
                    Id_Endereco = obj.Endereco_Principal?.Id ?? (Int64)0,
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
                    Id_Endereco = obj.Endereco_Cobranca?.Id ?? (Int64)0,
                    Latitude = Convert.ToString(obj.Endereco_Cobranca?.Latitude),
                    Logradouro = obj.Endereco_Cobranca?.Logradouro,
                    Longitude = Convert.ToString(obj.Endereco_Cobranca?.Longitude),
                    Numero = obj.Endereco_Cobranca?.Numero,
                    Ponto_Referencia = obj.Endereco_Cobranca?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Cobranca,
                    UF = obj.Endereco_Cobranca?.UF
                };
            }

            retorno.Id_Forma_Pagamento = obj.Forma_Pagamento?.Id;
            retorno.Conta_Corrente = obj.Conta_Corrente;
            retorno.Digito = obj.Digito;
            retorno.Agencia = obj.Agencia;
            retorno.Id_Vencimento = obj.Vencimento?.Id;
            retorno.Id_Banco = obj.Banco?.Id;*/

            retorno.Formas_Pagamento = obj.Formas_Pagamento_Cliente.Select(f => new Forma_Pagamento_ClienteModels
            {
                Id = f.Id,
                Id_Cliente = f.Cliente?.Id,
                Id_Forma_Pagamento = f.Forma_Pagamento?.Id,
                Desc_Forma_Pagamento = f.Forma_Pagamento?.Descricao,
                Forma_Envio = f.Forma_Envio,
                Desc_Forma_Envio = f.Forma_Envio == null ? "" : f.Forma_Envio.GetDisplayName(),
                Id_Vencimento = f.Vencimento?.Id,
                Id_Banco = f.Banco?.Id,
                Desc_Banco = f.Banco?.Descricao,
                Agencia = f.Agencia,
                Conta_Corrente = f.Conta_Corrente,
                Digito = f.Digito,
                Dia_Vencimento = f.Dia_Vencimento == null ? "" : f.Dia_Vencimento,
                End_Cobranca_Igual_Principal = f.End_Cobranca_Igual_Principal,
                Id_EnderecoCobranca = f.Endereco_Cobranca?.Id,

                Id_Tipo_ImovelCobranca = f.Endereco_Cobranca?.Tipo_Imovel?.Id,
                Id_PaisCobranca = f.Endereco_Cobranca?.Pais?.Id,

                LatitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Latitude),
                LongitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Longitude),
                CEPCobranca = f.Endereco_Cobranca?.Cep,
                LogradouroCobranca = f.Endereco_Cobranca?.Logradouro,
                NumeroCobranca = f.Endereco_Cobranca?.Numero,
                BairroCobranca = f.Endereco_Cobranca?.Bairro,
                Id_BairroCobranca = f.Endereco_Cobranca?.Id_Bairro,
                CidadeCobranca = f.Endereco_Cobranca?.Cidade,
                Id_CidadeCobranca = f.Endereco_Cobranca?.Id_Cidade,
                UFCobranca = f.Endereco_Cobranca?.UF,
                ComplementoCobranca = f.Endereco_Cobranca?.Complemento,
                Ponto_ReferenciaCobranca = f.Endereco_Cobranca?.Ponto_Referencia,
                Status = f.Status,
                Tipo_Forma_Pagamento = f.Forma_Pagamento?.Tipo_Forma_Pagamento

            }).ToList();

            retorno.Descontos = obj.Descontos.Select(p => new DescontoListaModel
            {
                Id = p.Id,
                Id_Desconto = p.Desconto?.Id ?? 0,
                Descricao = p.Desconto?.Descricao,
                //Valor_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Valor_Desconto) : p.Desconto.Valores.Sum(x => x.Valor).Value.ToString(),// : p.Desconto.,
                //Percentual_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Percentual_Desconto) : p.Desconto.Valores.Sum(x => x.Percentual).Value.ToString(),
                Valor_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Valor_Desconto) : p.Desconto.Valores[0].Valor.Value.ToString(),
                Percentual_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Percentual_Desconto) : p.Desconto.Valores[0].Percentual.Value.ToString(),
                Tipo_Desconto = p.Desconto?.Tipo_Desconto?.Descricao

            }).ToList();

            retorno.Planos = obj.PlanosProdutos.ToList().Select(p => new PlanoProdutoListModel
            {
                Id = p.Id,
                Id_Venda = p.Id_Venda,
                Status_Venda = p.Id_Venda.HasValue ? (new Conexao<Venda>(session).selectFirst(o => o.Id == p.Id_Venda))?.Result?.Status : (Status_Venda?)null,
                Id_Plano_Produto = p.Plano?.Id ?? p.Produto.Id,
                Descricao = p.Plano?.Descricao ?? p.Produto.Descricao,
                Id_Troca_Plano = p.Id_TrocaPlano ?? (long?)null,
                ValorPlanoProduto = String.Format("{0:N2}", p.Valor_Real ?? 0.00),
                ValorInstalacao_Final = String.Format("{0:N2}", p.Valor_Instalacao_Final ?? 0.00),
                ValorPlanoProduto_Final = String.Format("{0:N2}", p.Valor_Final ?? 0.00),
                ValorInstalacao = String.Format("{0:N2}", p.Valor_Instalacao_Real ?? 0.00),
                ValorTotal = String.Format("{0:N2}", p.Valor_Final_Total ?? 0.00),
                //Localidade_Anatel=p.Localidade_Anatel,

                Qtde_Dias = p.Dias_Teste_Gratis ?? 0,
                Meses_Carencia = p.Meses_Carencia,
                Tipo = p.Plano == null ? PlanoProdutoEnum.Produto : PlanoProdutoEnum.Plano,
                Status = p.Status,

                // DiasInadimplente = p.
                DiasBloqueado = p.Data_Desabilitado.HasValue ? (DateTime.Now - p.Data_Desabilitado.Value).Days.ToString() : String.Empty,

                Status_PlanoProduto = p.Plano?.StatusInicial ?? p.Produto?.Status_Inicial,
                Status_PlanoProduto_Str = p.Status != null
                           ? HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(p.Status.Value) : String.Empty,

                Id_Modelo_Contrato = p.ClienteContrato?.ModeloContrato?.Id,
                Id_Contrato = p.ClienteContrato?.Id,
                Numero_Contrato = p.ClienteContrato?.NumeroContrato,

                Numero_Parcelas = p.Parcelas_Instalacao ?? 0,
                End_Instalacao_Igual_Principal = p.End_Instalacao_Igual_Principal,

                Teste_Gratis = p.Dias_Teste_Gratis > 0,
                Periodo_Carencia = p.Periodo_Carencia,

                Nas = p.Nas,
                Login_Radius = p.Login_Radius,
                Senha_Radius = p.Senha_Radius,

                Id_Endereco = p.Endereco_Instalacao?.Id ?? (Int64)0,
                Id_Bairro = p.Endereco_Instalacao?.Id_Bairro,
                Bairro = p.Endereco_Instalacao?.Bairro,
                CEP = p.Endereco_Instalacao?.Cep,
                Id_Cidade = p.Endereco_Instalacao?.Id_Cidade,
                Cidade = p.Endereco_Instalacao?.Cidade,
                Ponto_Referencia = p.Endereco_Instalacao?.Ponto_Referencia,
                Complemento = p.Endereco_Instalacao?.Complemento,
                Logradouro = p.Endereco_Instalacao?.Logradouro,
                Numero = p.Endereco_Instalacao?.Numero,
                UF = p.Endereco_Instalacao?.UF,

                Latitude = Convert.ToString(p.Endereco_Instalacao?.Latitude),
                Longitude = Convert.ToString(p.Endereco_Instalacao?.Longitude),
                Id_Pais = p.Endereco_Instalacao?.Pais?.Id,
                Id_Tipo_Imovel = p.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel,

                Cod_Escritorio = p.Cod_Escritorio,
                Cod_Area = p.Cod_Area,
                Cod_Localidade = p.Cod_Localidade,
                Localidade = p.Localidade,

                Detalhes = p.Plano != null ? p.Plano?.DetalhesPlano : p.Produto?.Detalhes,

                VelocidadeUpload = p.Plano?.VelocidadeUpload,
                VelocidadeNominal = p.Plano?.VelocidadeNominal,
                VelocidadeDownload = p.Plano?.VelocidadeDownload,
                Id_Tecnologia = p.Plano?.Tecnologia?.Id,
                VelocidadeUploadReduzida = p.Plano?.VelocidadeUploadReduzida,
                VelocidadeDownloadReduzida = p.Plano?.VelocidadeDownloadReduzida,

                Id_Forma_Pagamento_Cliente = p.Forma_Pagamento_Cliente?.Id,
                Id_Forma_De_Pagamento = p.Forma_Pagamento_Cliente?.Forma_Pagamento?.Id

            }).ToList();

            return retorno;
        }


        #region  Mobile OlaCliente MeusDados
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpGet()]
        [Route("api/clienteapi/GetDadosCliente")]
        public async Task<GetApiRetorno<Cliente_DadosPessoais>> FindByIdClient(Int64 idCliente)
        {
            GetApiRetorno<Cliente_DadosPessoais> retorno = new GetApiRetorno<Cliente_DadosPessoais>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.nomefantasia AS NOME, A.email_principal AS EMAIL, ");
                        sql.AppendLine("A.telefone_celular AS TELEFONE,  A.forma_envio AS FORMA_ENVIO, ");
                        sql.AppendLine("C.DESCRICAO AS PLANO,C.REGIME, A.dia_vencimento as VENCIMENTO ");
                        sql.AppendLine("FROM tb_cliente A ");
                        sql.AppendLine("LEFT JOIN tb_cliente_plano_produto B ");
                        sql.AppendLine("ON A.id = B.id_cliente ");
                        sql.AppendLine("LEFT JOIN tb_plano C ");
                        sql.AppendLine("ON B.id_plano = C.id ");
                        sql.AppendLine("WHERE A.id = @IDCLIENTE ");
                        parametros.Add("@IDCLIENTE", idCliente);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.Objeto = new Cliente_DadosPessoais();
                                retorno.Objeto.Id = Convert.ToInt64(reader["ID"]);
                                retorno.Objeto.Nome = Convert.ToString(reader["NOME"]);
                                retorno.Objeto.Email = Convert.ToString(reader["EMAIL"]);
                                retorno.Objeto.Telefone = Convert.ToString(reader["TELEFONE"]);
                                retorno.Objeto.Plano = Convert.ToString(reader["PLANO"]);
                                retorno.Objeto.Regime = reader["REGIME"] != DBNull.Value ? (Regime_Anatel?)Enum.Parse(typeof(Regime_Anatel), Convert.ToString(reader["REGIME"])) : null;
                                retorno.Objeto.FormaEnvio = reader["FORMA_ENVIO"] != DBNull.Value ? Convert.ToInt64(reader["FORMA_ENVIO"]) : (Int64?)null;
                                retorno.Objeto.DiaVencimento = reader["VENCIMENTO"] != DBNull.Value ? Convert.ToInt64(reader["VENCIMENTO"]) : (Int64?)null;
                            }
                        }
                    }
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql2 = new StringBuilder();
                        sql2.AppendLine("SELECT C.dia_vencimento AS OPCOES_VENCIMENTO ");
                        sql2.AppendLine("FROM tb_cliente A ");
                        sql2.AppendLine("LEFT JOIN tb_forma_pagamento_vencimento B ");
                        sql2.AppendLine("ON A.id_forma_pagamento = B.id_forma_pagamento ");
                        sql2.AppendLine("LEFT JOIN tb_vencimento C ");
                        sql2.AppendLine("ON B.id_vencimento = C.id ");
                        sql2.AppendLine("WHERE A.id = @IDCLIENTE ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql2, parametros, session.Connection, null))
                        {
                            if (retorno.Objeto != null)
                            {
                                retorno.Objeto.OpcoesVencimento = new List<long>();
                                while (await reader.ReadAsync())
                                {
                                    if (reader["OPCOES_VENCIMENTO"] != DBNull.Value)
                                        retorno.Objeto.OpcoesVencimento.Add(Convert.ToInt64(reader["OPCOES_VENCIMENTO"]));
                                }
                            }
                        }

                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql3 = new StringBuilder();
                        sql3.AppendLine("SELECT DISTINCT A.ID_MODELO_CONTRATO,B.DESCRICAO AS TITULO,B.EXTENSAO_ARQUIVO,B.ARQUIVO_DOC AS NOME_ARQUIVO,B.ID ");
                        sql3.AppendLine("FROM TB_CLIENTE_PLANO_PRODUTO AS A ");
                        sql3.AppendLine("RIGHT JOIN TB_MODELO_CONTRATO AS B ");
                        sql3.AppendLine("ON B.ID = A.ID_MODELO_CONTRATO ");
                        sql3.AppendLine("WHERE ID_CLIENTE = @IDCLIENTE  AND ID_MODELO_CONTRATO IS NOT NULL ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql3, parametros, session.Connection, null))
                        {

                            if (retorno.Objeto != null)
                            {
                                retorno.Objeto.contratos = new List<ClienteAnexo_Mobile>();

                                while (await reader.ReadAsync())
                                {
                                    retorno.Objeto.contratos.Add(new ClienteAnexo_Mobile
                                    {
                                        ID = Convert.ToInt64(reader["ID"]),
                                        nome_arquivo = reader["NOME_ARQUIVO"].ToString(),
                                        Titulo = reader["TITULO"].ToString(),
                                        extensao_arquivo = reader["EXTENSAO_ARQUIVO"].ToString(),
                                        url_arquivo = String.Format("{0}/Arquivos/Modelo_Contrato/{1}.{2}", CONSTANTES.URLPortal, Convert.ToInt64(reader["ID"]), reader["EXTENSAO_ARQUIVO"])

                                    });
                                }



                            }
                        }

                    }

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

        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/clienteapi/atualizaTokenCliente")]
        public async Task<PostApiRetorno<Boolean>> atualizaTokenCliente(ClienteToken obj)
        {
            PostApiRetorno<Boolean> retorno = new PostApiRetorno<Boolean>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("  UPDATE TB_CLIENTE C ");
                        sql.AppendLine($" SET C.DEVICE_ID = '{obj.Device_id}' ");
                        sql.AppendLine($" WHERE C.id = {obj.idCliente}");

                        await CommandHelper.ExecuteNonQuery(com, sql, session.Connection, null);

                        retorno.Ok = true;
                        retorno.IdObjeto = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }
            return retorno;
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/clienteapi/GetTokenCliente")]
        public async Task<GetApiRetorno<object>> GetTokenCliente(ClienteToken Cliente)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("  SELECT C.DEVICE_ID FROM TB_CLIENTE C ");
                        sql.AppendLine($" WHERE C.ID = {Cliente.idCliente}");

                        var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null);
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = reader["DEVICE_ID"] == DBNull.Value ? (String)null : Convert.ToString(reader["DEVICE_ID"]);
                        }

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/AtualizaDados")]
        public async Task<PostApiRetorno<Int64>> atualizaDados(Cliente_DadosPessoais meusDados)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    Dictionary<String, Object> param = new Dictionary<String, Object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("UPDATE tb_cliente A ");
                        sql.AppendLine("SET A.email_principal = @EMAIL, ");
                        sql.AppendLine("A.telefone_celular = @TELEFONE, ");
                        sql.AppendLine("A.forma_envio = @FORMA_ENVIO, ");
                        sql.AppendLine("A.dia_vencimento = @DIA_VENCIMENTO ");
                        sql.AppendLine("WHERE A.id = @IDCLIENTE ");
                        parametros.Add("@IDCLIENTE", meusDados.Id);
                        parametros.Add("@EMAIL", meusDados.Email);
                        parametros.Add("@TELEFONE", meusDados.Telefone);
                        parametros.Add("@FORMA_ENVIO", meusDados.FormaEnvio);
                        parametros.Add("@DIA_VENCIMENTO", meusDados.DiaVencimento);

                        StringBuilder sql2 = new StringBuilder();

                        sql2.AppendLine("UPDATE TB_CLIENTE_PLANO_PRODUTO SET ATUALIZA_OLA = 1 WHERE ID_CLIENTE = @ID_CLIENTE");
                        param.Add("@ID_CLIENTE", meusDados.Id);

                        await CommandHelper.ExecuteNonQuery(com, sql2, param, session.Connection, null);

                        var reader = await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                        {
                            retorno.IdObjeto = reader == 1 ? 1 : 0;
                        }
                    }
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

        #endregion
        [HttpGet()]
        [Route("api/clienteapi/buscaReloadDataContrato")]
        public async Task<List<ObjBusca<Int64>>> buscaReloadDataContrato(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NUMERO_CONTRATO ");
                        sql.AppendLine("FROM TB_CLIENTE A");
                        sql.AppendLine("WHERE (UPPER(A.NUMERO_CONTRATO) LIKE @VALOR)");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["NUMERO_CONTRATO"]),
                                    Label = Convert.ToString(reader["NUMERO_CONTRATO"])

                                });
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
        [Route("api/clienteapi/GetComboContrato")]
        public async Task<ObjCombo<Int64>> GetComboContrato(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CLIENTE A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.NUMERO_CONTRATO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.NUMERO_CONTRATO != '' AND ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NUMERO_CONTRATO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.NUMERO_CONTRATO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["NUMERO_CONTRATO"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["NUMERO_CONTRATO"])
                                    });

                                }
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
        [Route("api/clienteapi/GetComboContratoModeloDesc")]
        public async Task<ObjCombo<Int64>> GetComboContratoModeloDesc(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MODELO_CONTRATO A ");


                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.ID != '' AND ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["DESCRICAO"])
                                    });

                                }
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
        [Route("api/clienteapi/GetComboClientePlanoProduto")]
        public async Task<ObjCombo<Int64>> GetComboClientePlanoProduto(RequestObjComboAvo<Int64?, Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
            {
                try
                {
                    #region [Filtros]
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A
                                                                LEFT JOIN TB_CLIENTE_CONTRATO B on A.ID_CLIENTE_CONTRATO = B.ID
                                                                LEFT JOIN TB_PLANO D ON (D.ID = A.ID_PLANO)
                                                                LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)
                                                                LEFT JOIN TB_COMBO F ON (F.ID = A.ID_COMBO)
                                                                LEFT JOIN TB_ENDERECO_CLIENTE E ON E.ID = A.ID_ENDERECO_INSTALACAO");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS NOT IN (8) AND A.STATUS IS NOT NULL");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE = {request.idpai}");
                    }

                    if (request.idavo.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE_CONTRATO = {request.idavo}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER( COALESCE(D.DESCRICAO,D.DESCRICAO)) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
                    }
                    #endregion

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
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID,");
                        sql.AppendLine(@"CASE 
	                                        WHEN A.ID_PLANO IS NOT NULL THEN COALESCE(D.DESCRICAO, '')
	                                        WHEN A.ID_PRODUTO IS NOT NULL THEN COALESCE(C.DESCRICAO, '')
	                                        WHEN A.ID_COMBO IS NOT NULL THEN COALESCE(F.DESCRICAO, '')
                                        END AS DESCRICAO,");
                        sql.AppendLine("B.NUMERO_CONTRATO, B.ID AS ID_CONTRATO,");
                        sql.AppendLine("E.ENDERECO_LOGRADOURO AS ENDERECO, E.ENDERECO_NUMERO AS NUMERO, E.ENDERECO_CIDADE CIDADE, A.STATUS");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                                {
                                    var textTmp = reader["DESCRICAO"] != DBNull.Value ? string.Concat(Convert.ToString(reader["DESCRICAO"]), ";") : string.Empty;
                                    textTmp += reader["ENDERECO"] != DBNull.Value ? string.Concat(Convert.ToString(reader["ENDERECO"]), ";") : string.Empty;
                                    textTmp += reader["NUMERO"] != DBNull.Value ? string.Concat(Convert.ToString(reader["NUMERO"]), ";") : string.Empty;
                                    textTmp += reader["CIDADE"] != DBNull.Value ? string.Concat(Convert.ToString(reader["CIDADE"]), ";") : string.Empty;

                                    var split = textTmp.Split(';');
                                    textTmp = string.Empty;
                                    var flagDescTmp = false;
                                    foreach (var item in split)
                                    {
                                        if (string.IsNullOrEmpty(textTmp))
                                        {
                                            textTmp = string.Concat(item, " - ");
                                        }
                                        else
                                        {
                                            if (textTmp.Contains("-") && !textTmp.Contains(",") && !flagDescTmp)
                                            {
                                                textTmp += item;
                                                flagDescTmp = true;
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(item))
                                                {
                                                    textTmp += string.Concat(", ", item);
                                                }
                                            }
                                        }
                                    }
                                    var status = reader["STATUS"] != DBNull.Value ? Convert.ToInt64(reader["STATUS"]) : (Int64?)null;
                                    if (status != null)
                                    {
                                        switch (status)
                                        {
                                            case 8:
                                                textTmp += " - Cancelado (Mudança de Plano)";
                                                break;
                                            case 5:
                                                textTmp += " - Cancelado";
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    retorno.results.Add(new ObjCombClientePlanoProduto
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = textTmp,
                                        Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                        IdContrato = reader["ID_CONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["ID_CONTRATO"]) : (long?)null,
                                    });

                                }
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

        /// <summary>
        ///     GetCombo para os planos/produtos com filtro pelo contrato
        /// </summary>
        [HttpPost()]
        [Route("api/clienteapi/GetComboClientePlanoProdutoContrato")]
        public async Task<ObjCombo<long>> GetComboClientePlanoProdutoContrato(RequestObjComboAvoConfig<long?, long?, long?, ConfigComboClientePlanoProdutoContrato> request)
        {
            var retorno = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                // Filtros 
                var parametros = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder($@"FROM TB_CLIENTE_PLANO_PRODUTO A
                                                   LEFT JOIN TB_CLIENTE_CONTRATO B ON A.ID_CLIENTE_CONTRATO = B.ID
                                                   LEFT JOIN TB_PLANO D ON (D.ID = A.ID_PLANO)
                                                   LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)");
                var condicoes = new List<string>();

                if (request.config == null || !request.config.ExibeCancelado)
                {
                    condicoes.Add("A.STATUS <> 5");
                }

                if (request.idavo.HasValue)
                {
                    condicoes.Add("A.ID_CLIENTE_CONTRATO = @ID_CONTRATO");
                    parametros.Add("@ID_CONTRATO", request.idavo.Value);
                }

                if (request.idpai.HasValue)
                {
                    condicoes.Add("A.ID_CLIENTE = @ID_CLIENTE");
                    parametros.Add("@ID_CLIENTE", request.idpai.Value);
                }

                if (request.id.HasValue)
                {
                    condicoes.Add("A.ID = @ID_PLANO_PRODUTO");
                    parametros.Add("@ID_PLANO_PRODUTO", request.id.Value);
                }

                if (request != null && !String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add($"(UPPER(D.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                }

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($" WHERE ({String.Join(" AND ", condicoes)})");
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
                    StringBuilder sql = new StringBuilder("SELECT A.ID, D.DESCRICAO, B.NUMERO_CONTRATO, B.ID AS ID_CONTRATO ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine("ORDER BY A.ID");

                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                            {
                                string numContrato = Convert.ToString(reader["NUMERO_CONTRATO"]);
                                string descricao;

                                // Caso o AddSelect não esteja filtrado pelo contrato o número do contrato
                                // será adicionado à descrição para ajudar a identificar planos iguais em
                                // contratos diferentes para o cliente
                                if (!request.idavo.HasValue)
                                {
                                    descricao = numContrato + " - " + Convert.ToString(reader["DESCRICAO"]);
                                }
                                else
                                {
                                    descricao = Convert.ToString(reader["DESCRICAO"]);
                                }

                                retorno.results.Add(new ObjCombClientePlanoProduto
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = descricao,
                                    Contrato = numContrato,
                                    IdContrato = reader["ID_CONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["ID_CONTRATO"]) : (long?)null
                                });
                            }
                        }
                    }
                }
            }

            return retorno;
        }


        //[HttpPost()]
        //[Route("api/clienteapi/GetComboClientePlanoProduto")]
        //public async Task<ObjCombo<Int64>> GetComboClientePlanoProduto(RequestObjComboAvo<Int64?, Int64?, Int64?> request)
        //{
        //    ObjCombo<Int64> retorno = new ObjCombo<Int64>
        //    {
        //        results = new List<IObjComboResult<Int64>>()
        //    };

        //    using (ISession session = HibernateUtil.GetSession())
        //    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
        //    using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
        //    {
        //        try
        //        {
        //            //Filtros
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //            StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
        //                                                          LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
        //                                                          LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)");
        //            List<String> condicoes = new List<String>();

        //            if (request.soativos)
        //            {
        //                //condicoes.Add($"A.STATUS = 1");
        //                //if (request.idavo.GetValueOrDefault(false) == false)
        //                //{
        //                    condicoes.Add($"A.STATUS <> 5");
        //                //}
        //            }
        //            else
        //            {

        //            }

        //            if (request.id.HasValue)
        //            {
        //                condicoes.Add($"A.ID = {request.id}");
        //            }

        //            if (request.idpai.HasValue)
        //            {
        //                condicoes.Add($"A.ID_CLIENTE = {request.idpai}");
        //            }

        //            //if (request.idavo.HasValue)
        //            //{
        //            //    condicoes.Add($"A.NUMERO_CONTRATO = {request.idavo}");
        //            //}

        //            if (request != null && !String.IsNullOrEmpty(request.q))
        //            {
        //                condicoes.Add($"(UPPER( COALESCE(B.DESCRICAO,C.DESCRICAO)) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
        //            }



        //            if (condicoes.Count() > 0)
        //            {
        //                sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
        //            }


        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
        //                sql.AppendLine(sqlFrom.ToString());

        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //                {
        //                    if (await reader.ReadAsync())
        //                        retorno.total_count = Convert.ToInt32(reader["CO"]);
        //                }
        //            }

        //            //Select
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT A.ID, COALESCE(B.DESCRICAO,C.DESCRICAO) AS DESCRICAO,A.NUMERO_CONTRATO  ");
        //                sql.AppendLine(sqlFrom.ToString());
        //                sql.AppendLine(" ORDER BY UPPER(A.ID) ");

        //                using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
        //                        {
        //                            retorno.results.Add(new ObjCombClientePlanoProduto
        //                            {
        //                                Id = Convert.ToInt64(reader["ID"]),
        //                                Text = Convert.ToString(reader["DESCRICAO"]),
        //                                Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),

        //                            });

        //                        }
        //                    }
        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //    return retorno;
        //}

        [HttpPost()]
        [Route("api/clienteapi/GetComboMudancaPlano")]
        public async Task<ObjCombo<Int64>> GetComboMudancaPlano(RequestObjComboAvo<Int64?, Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
            {
                try
                {
                    #region [Filtros]

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
                                                                  LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
                                                                  LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)
                                                                  INNER JOIN TB_CLIENTE_CONTRATO D ON (D.ID = A.ID_CLIENTE_CONTRATO)"
                                                                  );
                    List<String> condicoes = new List<String>();

                    //if (request.soativos)
                    //{
                    condicoes.Add($"A.STATUS IN ('2')");
                    //}

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE_CONTRATO = {request.idpai}");
                    }

                    if (request.idavo.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE = {request.idavo}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER( COALESCE(B.DESCRICAO,C.DESCRICAO)) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    #endregion

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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, COALESCE(B.DESCRICAO,C.DESCRICAO) AS DESCRICAO,D.NUMERO_CONTRATO  ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                                {
                                    retorno.results.Add(new ObjCombClientePlanoProduto
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["DESCRICAO"]) + " - " + Convert.ToString(reader["NUMERO_CONTRATO"]),
                                        Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),

                                    });
                                }
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

        [HttpPost]
        [Route("api/clienteapi/FindByContrato")]
        public async Task<DataTableResponse<ContratoPlanos>> FindByContrato(RequestContratoDataTable request)
        {
            var retorno = new DataTableResponse<ContratoPlanos>();
            if (String.IsNullOrEmpty(request.Contrato))
            {
                retorno.data = new List<ContratoPlanos>();
                return retorno;
            }

            int registros = 0;

            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();


                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())

                using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoTipoServicoEquipe = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                using (var ApiDocumentoProducao = new Documento_ProducaoApiController())
                {


                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
                                                                  LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
                                                                  --LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)
                                                                  LEFT JOIN TB_COMBO COMBO ON (COMBO.ID = A.ID_COMBO)
                                                                  LEFT JOIN TB_CLIENTE_CONTRATO D ON (D.ID = A.ID_CLIENTE_CONTRATO)
                                                                  LEFT JOIN TB_MODELO_CONTRATO E ON (E.ID = D.ID_MODELO_CONTRATO)
                                                                  LEFT JOIN TB_FORMA_PAGAMENTO_CLIENTE F ON (F.ID = A.ID_FORMA_PAGTO_CLIENTE)
                                                                  LEFT JOIN TB_FORMA_PAGAMENTO G ON (G.ID = F.ID_FORMA_PAGAMENTO)
                                                                  LEFT JOIN TB_ONU I ON (A.ID = I.ID_CLIENTE_PLANO_PRODUTO)                                                                 
                                                                  LEFT JOIN TB_OLT L ON (L.ID = (SELECT ID_OLT FROM TB_LOGIN_RADIUS LOG WHERE LOG.ID_PLANO_PRODUTO = A.ID AND LOG.STATUS = 1))
                                                                  LEFT JOIN TB_PON M ON (M.ID = (SELECT ID_PON FROM TB_LOGIN_RADIUS LOG WHERE LOG.ID_PLANO_PRODUTO = A.ID AND LOG.STATUS = 1))
                                                                  LEFT JOIN TB_CIRCUITO_TELEFONIA N ON (A.ID = N.ID_CLIENTE_PLANO_PRODUTO)
                                                                  LEFT JOIN TB_SLOT O ON (O.ID_OLT = L.ID)
                                                                  LEFT JOIN TB_FRANQUIA P ON (P.ID = B.ID_FRANQUIA)
                                                                  LEFT JOIN TB_VENDA Q ON (Q.ID = A.ID_VENDA)");

                    StringBuilder sqlWhere =
                        new StringBuilder($@"WHERE A.ID_CLIENTE_CONTRATO = ('{request.Contrato}') AND 
                                            A.ID_CLIENTE = ('{request.IdCliente}') AND
                                            (A.STATUS <> 5 AND A.STATUS <> 7 AND A.STATUS <> 8)");
                    if (request.PlanosCancelados)
                    {
                        sqlWhere =
                            new StringBuilder($@"WHERE A.ID_CLIENTE_CONTRATO = ('{request.Contrato}') AND 
                                                A.ID_CLIENTE = ('{request.IdCliente}')");
                    }

                    //Documento_Producao
                    var cliente = repoClienteContrato.selectFirst(p => p.Id == Convert.ToInt64(request.Contrato)).Result?.Cliente;

                    var query_clipp = (await repoClientePlanoProduto.select(p => p.Cliente == cliente)).ToList();

                    long idCliente = (await ApiDocumentoProducao.GetClienteAniel(sessionT5, cliente)).Cod_Cliente;

                    List<Int64?> Ids_PlanoProduto = query_clipp.Select(param =>
                    {
                        var planos = ApiDocumentoProducao.GetPlanoProdutoAniel(sessionT5, param).Result;
                        var ids = planos != null ? planos.Select(p => p?.Id) : new List<long?>();
                        return ids;

                    }).SelectMany(p => p).ToList();

                    foreach (var item in query_clipp)
                    {
                        long? id_PlanoProduto = (await ApiDocumentoProducao.GetPlanoProdutoAniel(sessionT5, item)).FirstOrDefault()?.Id;
                    }
                    var listagem = new List<ContratoPlanos>();

                    //Interação objetos Connect - T5
                    //*********************
                    //  Em desenvolvimento
                    //*********************
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendFormat("SELECT COUNT (A.ID) NUM_REGISTROS {0} {1}", sqlFrom.ToString(), sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                registros = Convert.ToInt32(reader["NUM_REGISTROS"]);
                            }
                        }
                    }
                    using (var commContrato = session.Connection.CreateCommand())
                    {
                        var parentese = sqlFrom.ToString().LastIndexOf(')');
                        StringBuilder query = new StringBuilder();
                        query.AppendLine("SELECT  A.VLR_TOTAL_PLANO_VARIAVEL,A.QTD_PROD_VARIAVEL,A.ID,D.TIPO_REAJUSTE, D.PERCENTUAL_REAJUSTE,D.VALOR_REAJUSTE ,COALESCE(B.DESCRICAO, COMBO.DESCRICAO) AS DESCRICAO, A.ID_FORMA_PAGTO_CLIENTE, A.STATUS, A.VALOR_FINAL, A.VALOR_FINAL_TOTAL, A.VALOR_INSTALACAO_FINAL, F.ID AS ID_FORMA_PAGAMENTO_CLIENTE, G.ID AS ID_FORMA_PAGAMENTO, G.DESCRICAO AS FORMA_PAGAMENTO, ");
                        query.AppendLine(" D.TEM_CARENCIA, D.PERIODO_CARENCIA, D.MULTA_CARENCIA, D.RENOVAR_AUTOMATICAMENTE, D.TEM_REAJUSTE, D.FREQUENCIA_REAJUSTE, D.DIAS_TESTE AS DIASTESTE, D.CANCELA_PLANO_CARENCIA, ");
                        query.AppendLine(" A.DATA_HABILITADO, A.DATE_DESABILITADO, D.ID_MODELO_CONTRATO, COALESCE(B.Id, COMBO.Id) AS ID_PLANO_PRODUTO, B.AUTENTICACAORADIUS, ");
                        query.AppendLine(" I.ID AS ID_ONU, I.DESCRICAO AS ONUDESCRICAO, L.ID AS ID_OLT, L.DESCRICAO AS DESCRICAOOLT, M.ID AS ID_PON, M.DESCRICAO AS DESCRICAOPON, N.NUMERO_SIP, N.SENHA AS SENHASIP, O.DESCRICAO AS DESCRICAOSLOT, P.MINUTOS, ");
                        query.AppendLine(" B.VELOCIDADENOMINAL, B.FREQUENCIACOBRANCA, B.FORMATOCOBRANCA, A.UTILIZABLOQUEIODEBITO, D.QTD_DIAS_DESBLOQUEADOS QTDDIASBLOQUEADOS, D.IMPEDIR_BLOQUEIO_AUTOMATICO IMPEDIRBLOQUEIO_AUTOMATICO, A.IMPEDIRBLOQUEIO_SEXTA, A.IMPEDIRBLOQUEIO_SABADO, A.IMPEDIRBLOQUEIO_DOMINGO, A.IMPEDIRBLOQUEIO_FERIADOS, ");

                        query.AppendLine(" B.REDUCAOVELOCIDADE, Q.TERMO_ADESAO_ASSINADO, Q.ID AS ID_VENDA,A.ID_CLIENTE_CONTRATO, E.BLOQUEIO_AUTOMATICO, B.TIPO_TECNOLOGIA, ");
                        query.AppendLine(" D.UTILIZA_DESBLOQUEIO_CONF AS UTILIZADESBLOQUEIOCONF, D.FREQUENCIA_DESBLOQUEIO AS FREQUENCIADESBLOQUEIO, D.QTD_DIAS_DESBLOQUEADOS AS QTDDIASDESBLOQUEADOS , E.BLOQUEAR_NUMERO_DIAS, E.BLOQUEAR_ATRASAR_RENEGOCIADAS, E.MAXIMO_TITULOS_ABERTO, D.IMPEDIR_BLOQUEIO_AUTOMATICO, D.IMPEDIR_BLOQUEIO_SEXTA, D.IMPEDIR_BLOQUEIO_SABADO, D.IMPEDIR_BLOQUEIO_DOMINGO, ");
                        query.AppendLine(" D.IMPEDIR_BLOQUEIO_FERIADO, D.IMPEDIR_BLOQUEIO_SEGUNDA, D.IMPEDIR_BLOQUEIO_TERCA, D.IMPEDIR_BLOQUEIO_QUARTA, D.IMPEDIR_BLOQUEIO_QUINTA, ");
                        query.AppendLine(" (SELECT FIRST 1 (R.DATA_VENCIMENTO) FROM TB_FATURAMENTO_CLIENTE R WHERE R.DATA_PAGAMENTO IS NULL) AS INICIO_INADIMPLENTE, ");
                        query.AppendLine(" (SELECT FIRST 1 (L.ID) FROM TB_LOGIN_RADIUS L WHERE L.ID_PLANO_PRODUTO = A.ID) AS ID_LOGIN_RADIUS, B.VELOCIDADEDOWNLOADREDUZIDA ");

                        query.AppendFormat("{0} {1}", sqlFrom.ToString(), sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(commContrato, query, new Dictionary<String, Object>(), session.Connection, null))
                        {
                            var listagemPlanos = new List<ContratoPlanos>();
                            while (await reader.ReadAsync())
                            {
                                //******************

                                //******************
                                String necessitaCriarOS = String.Empty,
                                       Status = Convert.ToString(reader["STATUS"]);

                                if ((Status_PlanoProduto)Enum.Parse(typeof(Status_PlanoProduto), Status) == Status_PlanoProduto.Aguardando_Criacao_OS)
                                {
                                    necessitaCriarOS = RsBase.SIM;
                                }
                                else
                                {
                                    necessitaCriarOS = RsBase.NAO;
                                }

                                DateTime dataFimCarencia = reader["DATA_HABILITADO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HABILITADO"]) : new DateTime();
                                int? periodoCarencia = reader["PERIODO_CARENCIA"] != DBNull.Value ? Convert.ToInt32(reader["PERIODO_CARENCIA"]) : (int?)null;
                                DateTime fimCarencia = periodoCarencia.HasValue ? dataFimCarencia.AddMonths(periodoCarencia.Value) : dataFimCarencia;

                                Boolean temCarencia = reader["TEM_CARENCIA"] != DBNull.Value ? Convert.ToBoolean(reader["TEM_CARENCIA"]) : false;
                                Boolean temReajuste = reader["TEM_REAJUSTE"] != DBNull.Value ? Convert.ToBoolean(reader["TEM_REAJUSTE"]) : false;
                                Boolean cancelaContratoAutomatico = reader["CANCELA_PLANO_CARENCIA"] != DBNull.Value ? Convert.ToBoolean(reader["CANCELA_PLANO_CARENCIA"]) : false;
                                var obj = new ContratoPlanos();

                                obj.Id = Convert.ToInt64(reader["ID"]);
                                obj.Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : string.Empty;
                                obj.Id_Forma_Pagamento_Cliente = reader["ID_FORMA_PAGAMENTO_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_FORMA_PAGAMENTO_CLIENTE"]) : (Int64?)null;
                                obj.Id_Forma_Pagamento = reader["ID_FORMA_PAGAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_FORMA_PAGAMENTO"]) : (Int64?)null;
                                obj.Forma_Pagamento = reader["FORMA_PAGAMENTO"] != DBNull.Value ? Convert.ToString(reader["FORMA_PAGAMENTO"]) : string.Empty;
                                obj.Id_Cliente_Contrato = reader["ID_CLIENTE_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["ID_CLIENTE_CONTRATO"]) : string.Empty;
                                obj.NecessitaCriarOS = necessitaCriarOS;

                                obj.Status = HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(Status);
                                obj.Valor_Final_Total = reader["VALOR_FINAL_TOTAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_FINAL_TOTAL"]) : (Double?)null;
                                obj.Valor_Final = reader["VALOR_FINAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_FINAL"]) : (Double?)null;
                                obj.Valor_Instalacao = reader["VALOR_INSTALACAO_FINAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_INSTALACAO_FINAL"]) : (Double?)null;
                                var culturaBR = new System.Globalization.CultureInfo("pt-BR");
                                obj.sValor_Final = reader["VALOR_FINAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_FINAL"]).ToString("C", culturaBR) : Convert.ToDouble("0").ToString("C", culturaBR);
                                obj.sValor_Instalacao = reader["VALOR_INSTALACAO_FINAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_INSTALACAO_FINAL"]).ToString("C", culturaBR) : Convert.ToDouble("0").ToString("C", culturaBR);

                                obj.Valor_Reajuste = reader["VALOR_REAJUSTE"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_REAJUSTE"]) : 0;
                                obj.Percentual_Reajuste = reader["PERCENTUAL_REAJUSTE"] != DBNull.Value ? Convert.ToDouble(reader["PERCENTUAL_REAJUSTE"]) : 0;
                                obj.Tipo_Reajuste = Convert.ToString(reader["TIPO_REAJUSTE"]) != null ? Convert.ToString(reader["TIPO_REAJUSTE"]) : null;

                                obj.Tem_Carencia = temCarencia == true ? RsBase.SIM : RsBase.NAO;
                                obj.Periodo_Carencia = periodoCarencia;
                                obj.Multa_Carencia = reader["MULTA_CARENCIA"] != DBNull.Value ? Convert.ToDouble(reader["MULTA_CARENCIA"]) : 0.00;
                                obj.Renova_Carencia_Automaticamente = HtmlEnumExtensions.GetEnumDescription<Renova_FidelidadeEnum>(Convert.ToString(reader["RENOVAR_AUTOMATICAMENTE"]));
                                obj.Tem_Reajuste = temReajuste == true ? RsBase.SIM : RsBase.NAO;
                                obj.Frequencia_Reajuste = HtmlEnumExtensions.GetEnumDescription<Frequencia_Reajuste>(Convert.ToString(reader["FREQUENCIA_REAJUSTE"]));
                                obj.DiasTeste = Convert.ToString(reader["DIASTESTE"]) != null ? Convert.ToString(reader["DIASTESTE"]) : null;
                                obj.Cancela_Contrato_Automatico_Final_Carencia = cancelaContratoAutomatico == true ? RsBase.SIM : RsBase.NAO;
                                obj.Encerra_Carencia = fimCarencia.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR"));
                                obj.Id_Modelo_Contrato = (reader["ID_MODELO_CONTRATO"]) != DBNull.Value ? Convert.ToInt64(reader["ID_MODELO_CONTRATO"]) : (Int64?)null;
                                obj.Id_Plano_Produto = reader["ID_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PLANO_PRODUTO"]) : 0;

                                obj.Autenticacao_Radius = Convert.ToString(reader["AUTENTICACAORADIUS"]) != null ? Convert.ToString(reader["AUTENTICACAORADIUS"]) : null;
                                //obj.Login_Radius = Convert.ToString(reader["LOGIN"]) != null ? Convert.ToString(reader["LOGIN"]) : null;
                                //obj.Senha_PPPOE = Convert.ToString(reader["SENHA_PPPOE"]) != null ? Convert.ToString(reader["SENHA_PPPOE"]) : null;
                                obj.Id_Login_Radius = Convert.ToString(reader["ID_LOGIN_RADIUS"]) != null ? Convert.ToString(reader["ID_LOGIN_RADIUS"]) : null;
                                obj.Id_ONU = Convert.ToString(reader["ID_ONU"]) != null ? Convert.ToString(reader["ID_ONU"]) : null;
                                obj.Descricao_ONU = Convert.ToString(reader["ONUDESCRICAO"]) != null ? Convert.ToString(reader["ONUDESCRICAO"]) : null;
                                obj.Id_OLT = Convert.ToString(reader["ID_OLT"]) != null ? Convert.ToString(reader["ID_OLT"]) : null;
                                obj.Descricao_OLT = Convert.ToString(reader["DESCRICAOOLT"]) != null ? Convert.ToString(reader["DESCRICAOOLT"]) : null;
                                obj.Id_PON = Convert.ToString(reader["ID_PON"]) != null ? Convert.ToString(reader["ID_PON"]) : null;
                                obj.Descricao_PON = Convert.ToString(reader["DESCRICAOPON"]) != null ? Convert.ToString(reader["DESCRICAOPON"]) : null;
                                obj.Descricao_SLOT = Convert.ToString(reader["DESCRICAOSLOT"]) != null ? Convert.ToString(reader["DESCRICAOSLOT"]) : null;
                                obj.Numero_Sip = Convert.ToString(reader["NUMERO_SIP"]) != null ? Convert.ToString(reader["NUMERO_SIP"]) : null;
                                obj.Senha_Circuito_Sip = Convert.ToString(reader["SENHASIP"]) != null ? Convert.ToString(reader["SENHASIP"]) : null;
                                obj.Franquia_Minutos = Convert.ToString(reader["MINUTOS"]) != null ? Convert.ToString(reader["MINUTOS"]) : null;
                                obj.Velocidade_Nominal = Convert.ToString(reader["VELOCIDADENOMINAL"]) != null ? Convert.ToString(reader["VELOCIDADENOMINAL"]) : null;
                                obj.Frequencia_Cobranca = Convert.ToString(reader["FREQUENCIACOBRANCA"]) != null ? Convert.ToString(reader["FREQUENCIACOBRANCA"]) : null;
                                obj.Formato_Cobranca = Convert.ToString(reader["FORMATOCOBRANCA"]) != null ? Convert.ToString(reader["FORMATOCOBRANCA"]) : null;

                                obj.Bloquei_Automatico_Debito = Convert.ToString(reader["UTILIZABLOQUEIODEBITO"]) != null ? Convert.ToString(reader["UTILIZABLOQUEIODEBITO"]) : null;
                                obj.Dias_Atraso = Convert.ToString(reader["QTDDIASBLOQUEADOS"]) != null ? Convert.ToString(reader["QTDDIASBLOQUEADOS"]) : null;
                                obj.Impedir_Bloqueio_Sexta = Convert.ToString(reader["IMPEDIRBLOQUEIO_SEXTA"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_SEXTA"]) : null;
                                obj.Impedir_Bloqueio_Sabado = Convert.ToString(reader["IMPEDIRBLOQUEIO_SABADO"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_SABADO"]) : null;
                                obj.Impedir_Bloqueio_Domingo = Convert.ToString(reader["IMPEDIRBLOQUEIO_DOMINGO"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_DOMINGO"]) : null;
                                obj.Impedir_Bloqueio_Feriados = Convert.ToString(reader["IMPEDIRBLOQUEIO_FERIADOS"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_FERIADOS"]) : null;
                                obj.Reducao_Velocidade = Convert.ToString(reader["REDUCAOVELOCIDADE"]) != null ? Convert.ToString(reader["REDUCAOVELOCIDADE"]) : null;
                                obj.Termo_Adesao_Assinado = Convert.ToString(reader["TERMO_ADESAO_ASSINADO"]) != null ? Convert.ToString(reader["TERMO_ADESAO_ASSINADO"]) : null;
                                obj.Tipo_Tecnologia = reader["TIPO_TECNOLOGIA"] != DBNull.Value ? Convert.ToInt32(reader["TIPO_TECNOLOGIA"]) : (int?)null;


                                obj.Data_Aceite = Convert.ToString(dataFimCarencia.ToString("dd/MM/yyyy HH:mm:ss"));
                                obj.Desbloqueio_Conf = reader["UTILIZADESBLOQUEIOCONF"] != DBNull.Value ? Convert.ToBoolean(reader["UTILIZADESBLOQUEIOCONF"]) : (bool?)null;
                                obj.QuantidadeDiasBloqueio = reader["QTDDIASDESBLOQUEADOS"] != DBNull.Value ? Convert.ToInt32(reader["QTDDIASDESBLOQUEADOS"]) : 0;

                                obj.Frequencia_Desbloqueio = reader["FREQUENCIADESBLOQUEIO"] != DBNull.Value ? Convert.ToInt32(reader["FREQUENCIADESBLOQUEIO"]) : (int?)null;
                                obj.BloquearNumeroDias = reader["BLOQUEAR_NUMERO_DIAS"] != DBNull.Value ? Convert.ToInt32(reader["BLOQUEAR_NUMERO_DIAS"]) : (int?)null;
                                obj.BloquearAtrasarRenegociadas = reader["BLOQUEAR_ATRASAR_RENEGOCIADAS"] != DBNull.Value ? Convert.ToInt32(reader["BLOQUEAR_ATRASAR_RENEGOCIADAS"]) : (int?)null;
                                obj.MaximoTitulosAberto = reader["MAXIMO_TITULOS_ABERTO"] != DBNull.Value ? Convert.ToInt32(reader["MAXIMO_TITULOS_ABERTO"]) : (int?)null;
                                obj.ImpedirBloqueio_Automatico = reader["IMPEDIR_BLOQUEIO_AUTOMATICO"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_AUTOMATICO"]) : (bool?)null;
                                obj.ImpedirBloqueio_Segunda = reader["IMPEDIR_BLOQUEIO_SEGUNDA"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_SEGUNDA"]) : (bool?)null;
                                obj.ImpedirBloqueio_Terca = reader["IMPEDIR_BLOQUEIO_TERCA"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_TERCA"]) : (bool?)null;
                                obj.ImpedirBloqueio_Quarta = reader["IMPEDIR_BLOQUEIO_QUARTA"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_QUARTA"]) : (bool?)null;
                                obj.ImpedirBloqueio_Quinta = reader["IMPEDIR_BLOQUEIO_QUINTA"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_QUINTA"]) : (bool?)null;
                                obj.ImpedirBloqueio_Sexta = reader["IMPEDIR_BLOQUEIO_SEXTA"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_SEXTA"]) : (bool?)null;
                                obj.ImpedirBloqueio_Sabado = reader["IMPEDIR_BLOQUEIO_SABADO"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_SABADO"]) : (bool?)null;
                                obj.ImpedirBloqueio_Domingo = reader["IMPEDIR_BLOQUEIO_DOMINGO"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_DOMINGO"]) : (bool?)null;
                                obj.ImpedirBloqueio_Feriados = reader["IMPEDIR_BLOQUEIO_FERIADO"] != DBNull.Value ? Convert.ToBoolean(reader["IMPEDIR_BLOQUEIO_FERIADO"]) : (bool?)null;
                                obj.BloqueioAutomaticoDebito = reader["BLOQUEIO_AUTOMATICO"] != DBNull.Value ? Convert.ToBoolean(reader["BLOQUEIO_AUTOMATICO"]) : (bool?)null;

                                var data_Desabilitado = reader["DATE_DESABILITADO"] != DBNull.Value ? Convert.ToDateTime(reader["DATE_DESABILITADO"]) : (DateTime?)null;
                                obj.Dias_Desabilitado_Debito = calculaDiasDiferenca(data_Desabilitado);
                                var data_Inadimplente = reader["INICIO_INADIMPLENTE"] != DBNull.Value ? Convert.ToDateTime(reader["INICIO_INADIMPLENTE"]) : (DateTime?)null;
                                obj.Dias_Inadimplente = calculaDiasDiferenca(data_Inadimplente);
                                long? idVenda = reader["ID_VENDA"] != DBNull.Value ? Convert.ToInt64(reader["ID_VENDA"]) : (long?)null;

                                var planoProdutoOS = repoDocProd.selectFirst(p => p.Id_Cliente_PlanoProdutoConnect == obj.Id || p.Id_Venda == idVenda).Result;
                                obj.NumOS = planoProdutoOS?.Numero_Obra_Original;
                                obj.PossuiOSCriada = planoProdutoOS != null ? RsBase.SIM : RsBase.NAO;
                                obj.Velocidade_Reduzida = reader["VELOCIDADEDOWNLOADREDUZIDA"] != DBNull.Value ? Convert.ToString(reader["VELOCIDADEDOWNLOADREDUZIDA"]) : String.Empty;
                                obj.QuantidadeProducaoVariavel = reader["QTD_PROD_VARIAVEL"] != DBNull.Value ? Convert.ToInt32(reader["QTD_PROD_VARIAVEL"]) : (int?)1;
                                obj.sValor_Total_Plano = reader["VLR_TOTAL_PLANO_VARIAVEL"] != DBNull.Value ? Convert.ToDouble(reader["VLR_TOTAL_PLANO_VARIAVEL"]).ToString("C", culturaBR) : obj.sValor_Final;
                                obj.Valor_Total_Plano = reader["VLR_TOTAL_PLANO_VARIAVEL"] != DBNull.Value ? Convert.ToDouble(reader["VLR_TOTAL_PLANO_VARIAVEL"]) : (Double?)null;

                                listagemPlanos.Add(obj);
                            }

                            if (listagemPlanos.Count > 0)
                            {
                                retorno.draw = request.draw;
                                retorno.data = listagemPlanos;
                                retorno.recordsFiltered = registros;
                            }
                            else
                            {
                                retorno.data = new List<ContratoPlanos>();
                                retorno.recordsFiltered = registros;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.error = ex.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/clienteapi/AtualizarInformacoesContrato")]
        public async Task<bool> AtualizarInformacoesContrato(AtualizacaoInformacaoContratoRequest request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoContrato = new Conexao<ClienteContrato>(session))
            {
                var contrato = await repoContrato.selectFirst(x => x.Id == request.IdContrato) ?? throw new Exception($"Nao foi encontrado contrato {request.IdContrato}");

                if (contrato.TipoReajuste == Tipo_Reajuste_Contratual.Percentual)
                    contrato.PercentualReajusteContratual = Convert.ToDouble(request.ValorReajuste);
                else
                    contrato.ValorReajusteContratual = Convert.ToDouble(request.ValorReajuste);

                contrato.UtilizaDesbloqueioConfianca = request.PermiteDesbloqueio;
                contrato.QtdDiasDesbloqueados = (short)request.QuantidadeDiasBloqueio;

                contrato.FrequenciaDesbloqueio = (Frequencia_Desbloqueio)request.FrequenciaDesbloqueio;


                contrato.ImpedirBloqueioAutomatico = request.ImpedirBloqueioAutomatico;
                contrato.ImpedirBloqueioDomingo = request.ImpedirBloqueioAutomaticoDomingo;
                contrato.ImpedirBloqueioFeriados = request.ImpedirBloqueioAutomaticoFeriados;
                contrato.ImpedirBloqueioSegunda = request.ImpedirBloqueioAutomaticoSegunda;
                contrato.ImpedirBloqueioTerca = request.ImpedirBloqueioAutomaticoTerca;
                contrato.ImpedirBloqueioQuarta = request.ImpedirBloqueioAutomaticoQuarta;
                contrato.ImpedirBloqueioQuinta = request.ImpedirBloqueioAutomaticoQuinta;
                contrato.ImpedirBloqueioSexta = request.ImpedirBloqueioAutomaticoSexta;
                contrato.ImpedirBloqueioSabado = request.ImpedirBloqueioAutomaticoSabado;

                await repoContrato.Update(contrato);
            }

            return true;
        }

        [HttpPost]
        [Route("api/clienteapi/FindByPlanoByContrato")]
        public async Task<DataTableResponse<ContratoPlanos>> FindByPlanoByContrato(RequestContratoDataTable request)
        {
            var retorno = new DataTableResponse<ContratoPlanos>();
            if (String.IsNullOrEmpty(request.IdClienteContrato))
            {
                retorno.data = new List<ContratoPlanos>();
                return retorno;
            }

            try
            {
                int records = 0;
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();


                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())

                using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var ApiDocumentoProducao = new Documento_ProducaoApiController())
                {


                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
                                                                  LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
                                                                  --LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)
                                                                  LEFT JOIN TB_COMBO C ON (C.ID = A.ID_COMBO)
                                                                  LEFT JOIN TB_CLIENTE_CONTRATO D ON (D.ID = A.ID_CLIENTE_CONTRATO)
                                                                  LEFT JOIN TB_MODELO_CONTRATO E ON (E.ID = D.ID_MODELO_CONTRATO)
                                                                  INNER JOIN TB_FORMA_PAGAMENTO_CLIENTE F ON (F.ID = A.ID_FORMA_PAGTO_CLIENTE)
                                                                  INNER JOIN TB_FORMA_PAGAMENTO G ON (G.ID = F.ID_FORMA_PAGAMENTO)
                                                                  LEFT JOIN TB_LOGIN_RADIUS H ON (H.ID_PLANO_PRODUTO = A.ID)
                                                                  LEFT JOIN TB_ONU I ON (A.ID = I.ID_CLIENTE_PLANO_PRODUTO)
                                                                  LEFT JOIN TB_OLT L ON (L.ID = H.ID_OLT)
                                                                  LEFT JOIN TB_PON M ON (M.ID = H.ID_PON)
                                                                  LEFT JOIN TB_CIRCUITO_TELEFONIA N ON (A.ID = N.ID_CLIENTE_PLANO_PRODUTO)
                                                                  LEFT JOIN TB_SLOT O ON (O.ID_OLT = L.ID)
                                                                  LEFT JOIN TB_FRANQUIA P ON (P.ID = B.ID_FRANQUIA)
                                                                  LEFT JOIN TB_VENDA Q ON (Q.ID = A.ID_VENDA)
                                                                  LEFT JOIN TB_FATURAMENTO_CLIENTE R ON (R.ID_CLIENTE = A.ID_CLIENTE)");

                    var sqlWhere = new StringBuilder();
                    sqlWhere.AppendLine($"WHERE");
                    sqlWhere.AppendLine($"A.ID_CLIENTE = ('{request.IdCliente}') AND");
                    sqlWhere.AppendLine($"A.ID_CLIENTE_CONTRATO = ('{request.IdClienteContrato}') AND");
                    sqlWhere.AppendLine($"(A.ID_PLANO = ('{request.IdPlanoProduto}') OR A.ID_PRODUTO = ('{request.IdPlanoProduto}') OR A.ID_COMBO = ('{request.IdPlanoProduto}')) ");

                    //Documento_Producao
                    var cliente = await repoCliente.selectFirst(p => p.Id == Convert.ToInt64(request.IdCliente));

                    var query_clipp = (await repoClientePlanoProduto.select(p => p.Cliente == cliente)).ToList();

                    long idCliente = (await ApiDocumentoProducao.GetClienteAniel(sessionT5, cliente)).Cod_Cliente;

                    List<Int64?> Ids_PlanoProduto = query_clipp.Select(param =>
                    {
                        var planos = ApiDocumentoProducao.GetPlanoProdutoAniel(sessionT5, param).Result;
                        var ids = planos != null ? planos.Select(p => p?.Id) : new List<long?>();
                        return ids;

                    }).SelectMany(p => p).ToList();

                    foreach (var item in query_clipp)
                    {
                        long? id_PlanoProduto = (await ApiDocumentoProducao.GetPlanoProdutoAniel(sessionT5, item)).FirstOrDefault()?.Id;
                    }
                    var listagem = new List<ContratoPlanos>();

                    //Interação objetos Connect - T5
                    //*********************
                    //  Em desenvolvimento
                    //*********************
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendFormat("SELECT COUNT (A.ID) NUM_REGISTROS {0} {1}", sqlFrom.ToString(), sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                long registros = Convert.ToInt64(reader["NUM_REGISTROS"]);
                            }
                        }
                    }

                    var listagemPlanos = new List<ContratoPlanos>();
                    using (var commContrato = session.Connection.CreateCommand())
                    {
                        var parentese = sqlFrom.ToString().LastIndexOf(')');
                        StringBuilder query = new StringBuilder();
                        query.AppendLine("SELECT  A.Id, COALESCE(B.DESCRICAO,C.DESCRICAO) AS DESCRICAO, A.ID_FORMA_PAGTO_CLIENTE, A.STATUS, A.VALOR_FINAL_TOTAL, A.VALOR_INSTALACAO_FINAL, G.DESCRICAO AS FORMA_PAGAMENTO, ");
                        query.AppendLine(" D.TEM_CARENCIA, D.PERIODO_CARENCIA, D.MULTA_CARENCIA, D.RENOVAR_AUTOMATICAMENTE, D.TEM_REAJUSTE, D.FREQUENCIA_REAJUSTE, D.DIAS_TESTE AS DIASTESTE, D.CANCELA_PLANO_CARENCIA, ");
                        query.AppendLine(" A.DATA_HABILITADO, A.DATE_DESABILITADO, D.ID_MODELO_CONTRATO, COALESCE(B.Id,C.Id) AS ID_PLANO_PRODUTO, B.AUTENTICACAORADIUS, H.LOGIN, H.SENHA_PPPOE,   ");
                        query.AppendLine(" I.DESCRICAO AS ONUDESCRICAO, L.DESCRICAO AS DESCRICAOOLT, M.DESCRICAO AS DESCRICAOPON, N.NUMERO_SIP, N.SENHA AS SENHASIP, O.DESCRICAO AS DESCRICAOSLOT, P.MINUTOS, ");
                        query.AppendLine(" B.VELOCIDADENOMINAL, B.FREQUENCIACOBRANCA, B.FORMATOCOBRANCA, A.UTILIZABLOQUEIODEBITO, A.QTDDIASBLOQUEADOS, A.IMPEDIRBLOQUEIO_AUTOMATICO, A.IMPEDIRBLOQUEIO_SEXTA, A.IMPEDIRBLOQUEIO_SABADO, A.IMPEDIRBLOQUEIO_DOMINGO, A.IMPEDIRBLOQUEIO_FERIADOS, ");
                        query.AppendLine(" B.REDUCAOVELOCIDADE, Q.TERMO_ADESAO_ASSINADO, Q.ID AS ID_VENDA,B.DETALHESPLANO, ");
                        query.AppendLine(" (SELECT FIRST 1 (R.DATA_VENCIMENTO) FROM TB_FATURAMENTO_CLIENTE R WHERE R.DATA_PAGAMENTO IS NULL) AS INICIO_INADIMPLENTE ");

                        query.AppendFormat("{0} {1}", sqlFrom.ToString(), sqlWhere.ToString());

                        //query.AppendLine($"WHERE A.ID IN ('{String.Join(",", numeros_Contrato)}')  ");

                        using (var reader = await CommandHelper.ExecuteReader(commContrato, query, new Dictionary<String, Object>(), session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                //******************

                                //******************
                                String necessitaCriarOS = String.Empty,
                                       Status = Convert.ToString(reader["STATUS"]);

                                if ((Status_PlanoProduto)Enum.Parse(typeof(Status_PlanoProduto), Status) == Status_PlanoProduto.Aguardando_Criacao_OS)
                                {
                                    necessitaCriarOS = RsBase.SIM;
                                }
                                else
                                {
                                    necessitaCriarOS = RsBase.NAO;
                                }

                                DateTime dataFimCarencia = Convert.ToDateTime(reader["DATA_HABILITADO"]);
                                int? periodoCarencia = reader["PERIODO_CARENCIA"] != DBNull.Value ? Convert.ToInt32(reader["PERIODO_CARENCIA"]) : (int?)null;
                                DateTime fimCarencia = periodoCarencia.HasValue ? dataFimCarencia.AddMonths(periodoCarencia.Value) : dataFimCarencia;

                                Boolean temCarencia = reader["TEM_CARENCIA"] != DBNull.Value ? Convert.ToBoolean(reader["TEM_CARENCIA"]) : false;
                                Boolean temReajuste = reader["TEM_REAJUSTE"] != DBNull.Value ? Convert.ToBoolean(reader["TEM_REAJUSTE"]) : false;
                                Boolean cancelaContratoAutomatico = reader["CANCELA_PLANO_CARENCIA"] != DBNull.Value ? Convert.ToBoolean(reader["CANCELA_PLANO_CARENCIA"]) : false;
                                var obj = new ContratoPlanos();

                                obj.Id = Convert.ToInt64(reader["ID"]);
                                obj.Descricao = Convert.ToString(reader["DESCRICAO"]);
                                obj.Forma_Pagamento = Convert.ToString(reader["FORMA_PAGAMENTO"]);
                                obj.NecessitaCriarOS = necessitaCriarOS;

                                obj.Status = HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(Status);
                                obj.Valor_Final_Total = reader["VALOR_FINAL_TOTAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_FINAL_TOTAL"]) : (Double?)null;
                                obj.Valor_Instalacao = reader["VALOR_INSTALACAO_FINAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_INSTALACAO_FINAL"]) : (Double?)null;

                                obj.Tem_Carencia = temCarencia == true ? RsBase.SIM : RsBase.NAO;
                                obj.Periodo_Carencia = periodoCarencia;
                                obj.Multa_Carencia = reader["MULTA_CARENCIA"] != DBNull.Value ? Convert.ToDouble(reader["MULTA_CARENCIA"]) : 0.00;
                                obj.Renova_Carencia_Automaticamente = HtmlEnumExtensions.GetEnumDescription<Renova_FidelidadeEnum>(Convert.ToString(reader["RENOVAR_AUTOMATICAMENTE"]));
                                obj.Tem_Reajuste = temReajuste == true ? RsBase.SIM : RsBase.NAO;
                                obj.Frequencia_Reajuste = HtmlEnumExtensions.GetEnumDescription<Frequencia_Reajuste>(Convert.ToString(reader["FREQUENCIA_REAJUSTE"]));
                                obj.DiasTeste = Convert.ToString(reader["DIASTESTE"]) != null ? Convert.ToString(reader["DIASTESTE"]) : null;
                                obj.Cancela_Contrato_Automatico_Final_Carencia = cancelaContratoAutomatico == true ? RsBase.SIM : RsBase.NAO;
                                obj.Encerra_Carencia = fimCarencia.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR"));
                                obj.Id_Modelo_Contrato = (reader["ID_MODELO_CONTRATO"]) != DBNull.Value ? Convert.ToInt64(reader["ID_MODELO_CONTRATO"]) : (Int64?)null;
                                obj.Id_Plano_Produto = reader["ID_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PLANO_PRODUTO"]) : 0;
                                obj.Detalhes_Plano = Convert.ToString(reader["DETALHESPLANO"]);

                                obj.Autenticacao_Radius = Convert.ToString(reader["AUTENTICACAORADIUS"]) != null ? Convert.ToString(reader["AUTENTICACAORADIUS"]) : null;
                                obj.Login_Radius = Convert.ToString(reader["LOGIN"]) != null ? Convert.ToString(reader["LOGIN"]) : null;
                                obj.Senha_PPPOE = Convert.ToString(reader["SENHA_PPPOE"]) != null ? Convert.ToString(reader["SENHA_PPPOE"]) : null;
                                obj.Descricao_ONU = Convert.ToString(reader["ONUDESCRICAO"]) != null ? Convert.ToString(reader["ONUDESCRICAO"]) : null;
                                obj.Descricao_OLT = Convert.ToString(reader["DESCRICAOOLT"]) != null ? Convert.ToString(reader["DESCRICAOOLT"]) : null;
                                obj.Descricao_PON = Convert.ToString(reader["DESCRICAOPON"]) != null ? Convert.ToString(reader["DESCRICAOPON"]) : null;
                                obj.Descricao_SLOT = Convert.ToString(reader["DESCRICAOSLOT"]) != null ? Convert.ToString(reader["DESCRICAOSLOT"]) : null;
                                obj.Numero_Sip = Convert.ToString(reader["NUMERO_SIP"]) != null ? Convert.ToString(reader["NUMERO_SIP"]) : null;
                                obj.Senha_Circuito_Sip = Convert.ToString(reader["SENHASIP"]) != null ? Convert.ToString(reader["SENHASIP"]) : null;
                                obj.Franquia_Minutos = Convert.ToString(reader["MINUTOS"]) != null ? Convert.ToString(reader["MINUTOS"]) : null;
                                obj.Velocidade_Nominal = Convert.ToString(reader["VELOCIDADENOMINAL"]) != null ? Convert.ToString(reader["VELOCIDADENOMINAL"]) : null;
                                obj.Frequencia_Cobranca = Convert.ToString(reader["FREQUENCIACOBRANCA"]) != null ? Convert.ToString(reader["FREQUENCIACOBRANCA"]) : null;
                                obj.Formato_Cobranca = Convert.ToString(reader["FORMATOCOBRANCA"]) != null ? Convert.ToString(reader["FORMATOCOBRANCA"]) : null;

                                obj.Bloquei_Automatico_Debito = Convert.ToString(reader["UTILIZABLOQUEIODEBITO"]) != null ? Convert.ToString(reader["UTILIZABLOQUEIODEBITO"]) : null;
                                obj.Dias_Atraso = Convert.ToString(reader["QTDDIASBLOQUEADOS"]) != null ? Convert.ToString(reader["QTDDIASBLOQUEADOS"]) : null;
                                obj.Impedir_Bloqueio_Automatico = Convert.ToString(reader["IMPEDIRBLOQUEIO_AUTOMATICO"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_AUTOMATICO"]) : null;
                                obj.Impedir_Bloqueio_Sexta = Convert.ToString(reader["IMPEDIRBLOQUEIO_SEXTA"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_SEXTA"]) : null;
                                obj.Impedir_Bloqueio_Sabado = Convert.ToString(reader["IMPEDIRBLOQUEIO_SABADO"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_SABADO"]) : null;
                                obj.Impedir_Bloqueio_Domingo = Convert.ToString(reader["IMPEDIRBLOQUEIO_DOMINGO"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_DOMINGO"]) : null;
                                obj.Impedir_Bloqueio_Feriados = Convert.ToString(reader["IMPEDIRBLOQUEIO_FERIADOS"]) != null ? Convert.ToString(reader["IMPEDIRBLOQUEIO_FERIADOS"]) : null;
                                obj.Reducao_Velocidade = Convert.ToString(reader["REDUCAOVELOCIDADE"]) != null ? Convert.ToString(reader["REDUCAOVELOCIDADE"]) : null;
                                obj.Termo_Adesao_Assinado = Convert.ToString(reader["TERMO_ADESAO_ASSINADO"]) != null ? Convert.ToString(reader["TERMO_ADESAO_ASSINADO"]) : null;


                                var data_Desabilitado = reader["DATE_DESABILITADO"] != DBNull.Value ? Convert.ToDateTime(reader["DATE_DESABILITADO"]) : (DateTime?)null;
                                obj.Dias_Desabilitado_Debito = calculaDiasDiferenca(data_Desabilitado);
                                var data_Inadimplente = reader["INICIO_INADIMPLENTE"] != DBNull.Value ? Convert.ToDateTime(reader["INICIO_INADIMPLENTE"]) : (DateTime?)null;
                                obj.Dias_Inadimplente = calculaDiasDiferenca(data_Inadimplente);
                                long? idVenda = reader["ID_VENDA"] != DBNull.Value ? Convert.ToInt64(reader["ID_VENDA"]) : (long?)null;

                                var planoProdutoOS = repoDocProd.selectFirst(p => p.Id_Cliente_PlanoProdutoConnect == obj.Id && p.Id_Venda == idVenda).Result;
                                obj.NumOS = planoProdutoOS?.Numero_Obra_Original;
                                obj.PossuiOSCriada = planoProdutoOS != null ? RsBase.SIM : RsBase.NAO;

                                listagemPlanos.Add(obj);
                            }

                            if (listagemPlanos.Count > 0)
                            {
                                retorno.data = listagemPlanos;
                            }
                            else
                            {
                                retorno.data = new List<ContratoPlanos>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.error = ex.Message;
            }

            return retorno;
        }

        Func<DateTime?, String> calculaDiasDiferenca = (param) =>
        {
            var hoje = DateTime.Now;

            return param != null ? Convert.ToInt32((hoje - param).Value.TotalDays).ToString() : "0";

        };

        [HttpPost()]
        [Route("api/clienteapi/GetComboContratoCliente")]
        public async Task<ObjCombo<Int64>> GetComboContratoCliente(RequestObjComboAvo<Int64?, Int64?, Boolean?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            List<ObjComboContratoCliente> objContratoCliente = new List<ObjComboContratoCliente>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())

            using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))

            using (var ApiDocumentoProducao = new Documento_ProducaoApiController())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
                                                                  LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
                                                                  LEFT JOIN TB_CLIENTE_CONTRATO D ON (D.ID = A.ID_CLIENTE_CONTRATO)");

                    /*
                     * Campos
                     * Descricao do Plano Cliente_planoProduto.Plano.Descricao
                     * Valor Final do Plano Cliente_planoProduto.Valor_Final_Total
                     * Valor Real da INsltalção Cliente_planoProduto.Valor_Instalacao_Real
                     * Forma de Pagamento Cliente_planoProduto.Forma_Pagamento
                     * Status do Plano e Produto Cliente_PlanoProduto.StatusPlanoProduto
                     * Necessita_CriarOS Case When Status_PlanoProduto=1 Then 'SIM'
                     * PossuiOsCriada
                     */

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS NOT IN ('5','7')");
                    }
                    //else
                    //{
                    //    condicoes.Add($"A.STATUS IN ('0','1','2','3','4','5','6','7','8')");
                    //}

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        var filtro = StringService.Truncate(request.q.ToUpperInvariant(), 255)
                                                  .Replace(" ", "")
                                                  .Replace(".", "")
                                                  .Replace("-", "");
                        condicoes.Add($@"(REPLACE(REPLACE(REPLACE(UPPER(COALESCE(B.DESCRICAO, '')), '.', ''), ' ', ''), '-', '') LIKE '%{filtro}%'
                                           OR REPLACE(REPLACE(REPLACE(UPPER(COALESCE(D.DESCRICAO, '')), '.', ''), ' ', ''), '-', '') LIKE '%{filtro}%'
                                           OR REPLACE(REPLACE(REPLACE(UPPER(COALESCE(D.NUMERO_CONTRATO, '')), '.', ''), ' ', ''), '-', '') LIKE '%{filtro}%'
                                             )");

                    }
                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
                    }
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(DISTINCT A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    List<String> numeros_Contrato = new List<String>();

                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder("SELECT DISTINCT D.ID, D.ID_CLIENTE, D.DESCRICAO AS DESCRICAO_MODELCONTR, D.NUMERO_CONTRATO, D.STATUS AS STATUS_CONTRATO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" GROUP BY D.ID, D.ID_CLIENTE, D.DESCRICAO, D.NUMERO_CONTRATO, D.STATUS ");
                        sql.AppendLine(" ORDER BY UPPER(D.ID) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                string Descricao = Convert.ToString(reader["ID"]) + " - " + Convert.ToString(reader["DESCRICAO_MODELCONTR"]);
                                if (Convert.ToString(reader["STATUS_CONTRATO"]) == "4")
                                {
                                    Descricao += " - Cancelado";
                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO_MODELCONTR"])) && !String.IsNullOrEmpty(Convert.ToString(reader["NUMERO_CONTRATO"])))
                                {
                                    numeros_Contrato.Add(Convert.ToString(reader["NUMERO_CONTRATO"]));

                                    objContratoCliente.Add(new ObjComboContratoCliente
                                    {
                                        Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                        Text = Descricao,
                                        Contrato = reader["NUMERO_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["NUMERO_CONTRATO"]) : string.Empty,
                                        IdModeloContrato = Convert.ToInt64(reader["ID"]),
                                        Status_Contrato = Convert.ToString(reader["STATUS_CONTRATO"])
                                    });
                                }
                            }
                        }
                    }

                    using (var commContrato = session.Connection.CreateCommand())
                    {
                        //('1,2,3,4,5')
                        var sqlTmp = sqlFrom.ToString();
                        var parentese = sqlTmp.LastIndexOf(')');

                        StringBuilder query = new StringBuilder();
                        query.AppendLine("SELECT D.ID, D.NUMERO_CONTRATO, D.DESCRICAO AS DESCRICAO, A.ID_FORMA_PAGTO_CLIENTE, A.STATUS, A.VALOR_FINAL_TOTAL, A.VALOR_INSTALACAO_FINAL,")
                            .AppendLine(" A.DATE_DESABILITADO, (SELECT FIRST 1 (Z.Data_Vencimento) FROM tb_faturamento_cliente Z WHERE Z.Data_Pagamento IS NULL) AS INICIO_INADIMPLENTE ");
                        query.AppendLine(sqlTmp.Insert(parentese, $"AND D.NUMERO_CONTRATO IN ('{String.Join("','", numeros_Contrato)}')"));

                        //query.AppendLine($"WHERE A.ID IN ('{String.Join(",", numeros_Contrato)}')  ");
                        var listagemPlanos = new List<ContratoPlanos>();
                        using (var reader = await CommandHelper.ExecuteReader(commContrato, query, new Dictionary<String, Object>(), session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                //******************

                                //******************


                                if (reader["ID"] != DBNull.Value && objContratoCliente.Any(x => x.Id == Convert.ToInt64(reader["ID"])))
                                {

                                    var contratosCli = objContratoCliente.Where(p => p.Id == Convert.ToInt64(reader["ID"])).ToList();

                                    foreach (var ctt in contratosCli)
                                    {
                                        var objTmp = new ContratoPlanos();

                                        var necessitaCriarOS = RsBase.NAO;

                                        var Status = Convert.ToString(reader["STATUS"]);

                                        if (!string.IsNullOrEmpty(Status) &&
                                            (Status_PlanoProduto)Enum.Parse(typeof(Status_PlanoProduto), Status) == Status_PlanoProduto.Aguardando_Criacao_OS)
                                        {
                                            necessitaCriarOS = RsBase.SIM;
                                        }
                                        else
                                        {
                                            necessitaCriarOS = RsBase.NAO;
                                        }

                                        var data_Desabilitado = reader["DATE_DESABILITADO"] != DBNull.Value ? Convert.ToDateTime(reader["DATE_DESABILITADO"]) : (DateTime?)null;
                                        string quantidade_DiasDesabilitado = calculaDiasDiferenca(data_Desabilitado);

                                        var dataVencimento = reader["INICIO_INADIMPLENTE"] != DBNull.Value ? Convert.ToDateTime(reader["INICIO_INADIMPLENTE"]) : (DateTime?)null;
                                        //var dataPagamento = reader["DATA_PAGAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PAGAMENTO"]) : (DateTime?)null;

                                        string calculoDiasInadimplente = calculaDiasDiferenca(dataVencimento);

                                        ctt.Listagem_Planos.Add(new ContratoPlanos
                                        {
                                            Id = Convert.ToInt64(reader["ID"]),
                                            Descricao = Convert.ToString(reader["DESCRICAO"]),
                                            Forma_Pagamento = Convert.ToString(reader["ID_FORMA_PAGTO_CLIENTE"]),
                                            NecessitaCriarOS = necessitaCriarOS,

                                            Status = string.IsNullOrEmpty(Status) ? null : HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(Status),
                                            Valor_Final_Total = reader["VALOR_FINAL_TOTAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_FINAL_TOTAL"]) : (double?)null,
                                            Valor_Instalacao = reader["VALOR_INSTALACAO_FINAL"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_INSTALACAO_FINAL"]) : (double?)null,
                                            Dias_Desabilitado_Debito = $"{quantidade_DiasDesabilitado}",
                                            Dias_Inadimplente = $"{calculoDiasInadimplente}",
                                        });
                                    }
                                }
                            }
                        }


                        retorno.results.AddRange(objContratoCliente);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Não Encontrado!");
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/GetComboClientePlanoProdutoHabilitado")]
        public async Task<ObjCombo<Int64>> GetComboClientePlanoProdutoHabilitado(RequestObjComboAvo<Int64?, Int64?, Boolean?> request)
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

                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
                                                                  LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
                                                                  LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)");

                    List<String> condicoes = new List<String>();


                    condicoes.Add($"A.STATUS = 2");


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER( COALESCE(B.DESCRICAO,C.DESCRICAO)) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }



                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, COALESCE(B.DESCRICAO,C.DESCRICAO) AS DESCRICAO,A.NUMERO_CONTRATO  ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                                {
                                    retorno.results.Add(new ObjCombClientePlanoProduto
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["DESCRICAO"]),
                                        Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                    });

                                }
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
        [Route("api/clienteapi/GetComboClientePlanoProdutoReservar")]
        public async Task<ObjCombo<Int64>> GetComboClientePlanoProdutoReservar(RequestObjComboAvo<Int64?, Int64?, Boolean?> request)
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

                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
                                                                  LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
                                                                  LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)");

                    List<String> condicoes = new List<String>();


                    condicoes.Add($"(A.STATUS = 0 OR A.STATUS = 1 OR A.STATUS = 6)");


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER( COALESCE(B.DESCRICAO,C.DESCRICAO)) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }



                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, COALESCE(B.DESCRICAO,C.DESCRICAO) AS DESCRICAO,A.NUMERO_CONTRATO  ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                                {
                                    retorno.results.Add(new ObjCombClientePlanoProduto
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["DESCRICAO"]),
                                        Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                    });

                                }
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
        [Route("api/ClienteApi/GetListPlanosProdutos")]
        public async Task<GetApiRetorno<List<Planos_HabilitadosModel>>> GetListPlanosProdutos(Int64 Id_Cliente)
        {
            GetApiRetorno<List<Planos_HabilitadosModel>> retorno = new GetApiRetorno<List<Planos_HabilitadosModel>>
            {
                Objeto = new List<Planos_HabilitadosModel>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            {
                try
                {
                    var listaPlanos = await repoCliente_PlanoProduto.queryover()
                                                                    .Where(p => p.Cliente.Id == Id_Cliente &&
                                                                           p.Status != Status_PlanoProduto.Aguardando_Aprovacao_Venda)
                                                                    .ListAsync();

                    foreach (var clientePlano in listaPlanos)
                    {
                        string descricaoPlano = String.Empty;
                        if (clientePlano.Combo != null)
                        {
                            foreach (var planoCombo in clientePlano.Combo.PlanosProdutos)
                            {
                                var planoHabilitado = new Planos_HabilitadosModel();

                                planoHabilitado.Plano = planoCombo.Plano != null ? planoCombo.Plano.Descricao : planoCombo.Produto.Descricao;
                                planoHabilitado.Status = HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(clientePlano.Status.Value);
                                planoHabilitado.Valor = Convert.ToString(String.Format("{0:N2}", planoCombo.Valor));

                                retorno.Objeto.Add(planoHabilitado);
                            }
                        }
                        else
                        {
                            var planoHabilitado = new Planos_HabilitadosModel();

                            planoHabilitado.Plano = clientePlano.Plano != null ? clientePlano.Plano.Descricao : clientePlano.Produto.Descricao;
                            planoHabilitado.Status = HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(clientePlano.Status.Value);
                            planoHabilitado.Valor = Convert.ToString(String.Format("{0:N2}", clientePlano.Valor_Final_Total));

                            retorno.Objeto.Add(planoHabilitado);
                        }
                    }

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

        [HttpGet()]
        [Route("api/ClienteApi/GetTreeViewFibra")]
        public async Task<GetApiRetorno<List<Cliente_TreeViewFibra>>> GetTreeViewFibra(Int64 Id_Pon)
        {
            GetApiRetorno<List<Cliente_TreeViewFibra>> retorno = new GetApiRetorno<List<Cliente_TreeViewFibra>>
            {
                Objeto = new List<Cliente_TreeViewFibra>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder($"SELECT A.ID, A.DESCRICAO, A.LOGIN, C.NOMEFANTASIA, D.DESCRICAO AS PLANO, B.ID AS ID_CLIENTE_PLANO_PRODUTO, C.ID AS ID_CLIENTE ")
                                .AppendLine("FROM TB_ONU A ")
                                .AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO B ")
                                .AppendLine("ON(A.ID_CLIENTE_PLANO_PRODUTO = B.ID) ")
                                .AppendLine("INNER JOIN TB_CLIENTE C ")
                                .AppendLine("ON(B.ID_CLIENTE = C.ID) ")
                                .AppendLine("INNER JOIN TB_PLANO D ")
                                .AppendLine("ON(B.ID_PLANO = D.ID) ")
                                .AppendLine("WHERE A.ID_PON = @ID_PON");
                        Dictionary<String, Object> parametros = new Dictionary<String, Object>
                        {
                            { "@ID_PON", Id_Pon }
                        };

                        var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new Cliente_TreeViewFibra
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Equipamento = $"{reader["DESCRICAO"]} - {reader["LOGIN"]}",
                                Cliente = Convert.ToString(reader["NOMEFANTASIA"]),
                                Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"]),
                                Id_ClientePlanoProduto = Convert.ToInt64(reader["ID_CLIENTE_PLANO_PRODUTO"]),
                                Login = Convert.ToString(reader["LOGIN"]),
                                Plano = Convert.ToString(reader["PLANO"])
                            });
                        }

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
        [Route("api/clienteapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CLIENTE A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        string documento = request.q.Replace(".", "")
                                                    .Replace("-", "")
                                                    .Replace("/", "");

                        condicoes.Add($"((UPPER(A.NOMEFANTASIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%') OR (UPPER(A.CPF) LIKE '%{StringService.Truncate(documento.ToUpperInvariant(), 20)}%'))");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.NOMEFANTASIA != '' AND ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOMEFANTASIA, A.CPF ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["NOMEFANTASIA"])))
                                {
                                    retorno.results.Add(new ClienteObjCombo
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = String.Format("{0} - {1}", reader["CPF"], reader["NOMEFANTASIA"]),
                                        Cpf = Convert.ToString(reader["CPF"])
                                    });

                                }
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
        [Route("api/clienteapi/GetComboIdentificacao")]
        public async Task<ObjCombo<Int64>> GetComboIdentificacao(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CLIENTE A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.NOMEFANTASIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%') OR (UPPER(A.ID) LIKE '%{StringService.Truncate(request.q, 14)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.NOMEFANTASIA != '' AND ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOMEFANTASIA, A.CPF ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["NOMEFANTASIA"])))
                                {
                                    retorno.results.Add(new ClienteObjCombo
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = String.Format("{0} - {1}", reader["ID"], reader["NOMEFANTASIA"]),
                                        Cpf = Convert.ToString(reader["CPF"])
                                    });

                                }
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
        [Route("api/clienteapi/GetComboEnderecosCliente")]
        public async Task<ObjCombo<Int64>> GetComboEnderecosCliente(RequestObjComboAvo<Int64?, Int64?, Boolean?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>()
                    { {"idCliente",request.idpai} };

                    StringBuilder sqlFrom = new StringBuilder(@"FROM TB_FATURAMENTO_CLIENTE A
                                                                INNER JOIN TB_FORMA_PAGAMENTO_CLIENTE H
                                                                ON(H.id = A.ID_FORMA_PAGAMENTO_CLIENTE)
                                                                INNER JOIN TB_ENDERECO_CLIENTE I
                                                                ON(I.ID = H.ID_ENDERECO_COBRANCA)");

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
                        StringBuilder sql = new StringBuilder(" SELECT I.ID,I.ENDERECO_LOGRADOURO,I.ENDERECO_NUMERO ");
                        sql.AppendLine(sqlFrom.ToString());

                        sql.AppendLine("WHERE H.ID_CLIENTE = @idCliente");
                        sql.AppendLine("GROUP BY I.ID,I.ENDERECO_LOGRADOURO,I.ENDERECO_NUMERO");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ClienteObjCombo
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{reader["ENDERECO_LOGRADOURO"]}, {reader["ENDERECO_NUMERO"]}",
                                });
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
        [Route("api/clienteapi/GetComboCPFAtendimento")]
        public async Task<ObjCombo<Int64>> GetComboCPF(RequestObjComboPai<Int64?, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CLIENTE A ");

                    List<String> condicoes = new List<String>();

                    //if (request.soativos)
                    //{
                    //    condicoes.Add($"A.STATUS = 1");
                    //}

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    //Filtra pelo Id da Organização do Cliente
                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_ORGANIZACAO = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        if (request.q.Length <= 18)
                            condicoes.Add($"(UPPER(A.NOMEFANTASIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%') OR (UPPER(A.CPF) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty), 18)}%')");
                        else
                            condicoes.Add($"(UPPER(A.NOMEFANTASIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.NOMEFANTASIA IS NOT NULL AND ( {String.Join(" AND ", condicoes)} ) ");
                    }
                    else
                    {
                        sqlFrom.AppendLine($" WHERE A.NOMEFANTASIA IS NOT NULL AND A.CPF IS NOT NULL AND A.TIPO_CLIENTE IS NOT NULL");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOMEFANTASIA, A.CPF, A.TIPO_CLIENTE ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["NOMEFANTASIA"])))
                                {
                                    retorno.results.Add(new ClienteObjCombo
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["NOMEFANTASIA"]),
                                        Cpf = Convert.ToString(reader["CPF"]),
                                        TipoCliente = Convert.ToString(reader["TIPO_CLIENTE"])
                                    });

                                }
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
        [Route("api/clienteapi/GetComboCpf")]
        public async Task<ObjCombo<Int64>> GetComboCpf(RequestObjComboPai<Int64?, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CLIENTE A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    condicoes.Add($"A.TIPO_CLIENTE = 0");

                    if (request.idpai != null)
                    {
                        if (request.idpai > 1)
                        {
                            condicoes.Add($"A.TIPOPESSOA IN (1,3)");
                        }
                        else
                        {
                            condicoes.Add($"A.TIPOPESSOA IN (0,2)");
                        }
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.NOMEFANTASIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOMEFANTASIA,A.CPF ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.NOMEFANTASIA) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["NOMEFANTASIA"])))
                                {
                                    retorno.results.Add(new ClienteObjCombo
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["NOMEFANTASIA"]),
                                        Cpf = Convert.ToString(reader["CPF"])
                                    });

                                }
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
        [Route("api/clienteapi/UnificarNovoContatoCliente")]
        public async Task<PostApiRetorno> UnificarNovoContatoCliente(UnificacaoNovoContatoClienteRequestModel request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (ITransaction transaction = session.BeginTransaction())
            using (ITransaction transactionLog = sessionLog.BeginTransaction())
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            using (Conexao<Atendimento> repoAtendimento = new Conexao<Atendimento>(session))
            using (Conexao<Cliente_Contato> repoClienteContato = new Conexao<Cliente_Contato>(session))
            using (Conexao<Cliente_PlanoProduto> repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (Conexao<Model.nhibernate.LoginRadius> repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (Conexao<Cliente_Anexo> repoClienteAnexo = new Conexao<Cliente_Anexo>(session))
            using (Conexao<Forma_Pagamento_Cliente> repoForma_Pagamento_Cliente = new Conexao<Forma_Pagamento_Cliente>(session))
            using (Conexao<Cliente_Desconto> repoClienteDesconto = new Conexao<Cliente_Desconto>(session))
            using (Conexao<Pesquisa_Resposta> repoPesquisaResposta = new Conexao<Pesquisa_Resposta>(session))
            using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
            {
                try
                {
                    if (request._IdContatos.Count == 0)
                    {
                        throw new Exception("Nenhum contato a unificar");
                    }
                    else if (request._IdContatos.Count > 1)
                    {
                        throw new Exception("Só é permitido unificar um contato por vez");
                    }
                    Cliente clienteUnificar = await repoCliente.selectFirst(p => p.Id == request.IdClienteUnificar);
                    Cliente clienteDestino = await repoCliente.selectFirst(p => p.Id == request.IdClienteDestino);
                    List<Atendimento> atendimentos = (await repoAtendimento.select(p => p.Cliente == clienteUnificar)).ToList();

                    List<Cliente_Contato> contatos = repoClienteContato.QueryableSelect().Where(p => request._IdContatos.Contains(p.Id)).ToList();
                    List<Cliente_PlanoProduto> planosProdutos = clienteUnificar.PlanosProdutos.ToList();
                    clienteUnificar.PlanosProdutos = new List<Cliente_PlanoProduto>();

                    List<Model.nhibernate.LoginRadius> login = (await repoLoginRadius.select(p => p.Cliente == clienteUnificar)).ToList();
                    List<Cliente_Anexo> anexos = (await repoClienteAnexo.select(p => p.Cliente == clienteUnificar)).ToList();
                    List<Forma_Pagamento_Cliente> formasPagamento = clienteUnificar.Formas_Pagamento_Cliente.ToList();
                    List<Cliente_Desconto> descontos = clienteUnificar.Descontos.ToList();
                    clienteUnificar.Descontos = new List<Cliente_Desconto>();
                    clienteUnificar.Formas_Pagamento_Cliente = new List<Forma_Pagamento_Cliente>();
                    List<Pesquisa_Resposta> respostasPesquisa = (await repoPesquisaResposta.select(p => p.Cliente == clienteUnificar)).ToList();
                    List<Cliente> indicacoes = (await repoCliente.select(p => p.Quem_Indicou == clienteUnificar)).ToList();

                    var unificacao = new Dictionary<string, string>
                    {
                        {"@FIXO", string.IsNullOrEmpty(clienteUnificar.Telefone_Fixo) ? clienteUnificar.Telefone_Fixo2 : clienteUnificar.Telefone_Fixo },
                        {"@EMAIL", string.IsNullOrEmpty(clienteUnificar.Email_Principal) ? clienteUnificar.Email_Aux : clienteUnificar.Email_Principal },
                        {"@CELULAR", clienteUnificar.Telefone_Celular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") }
                    };
                    if (string.IsNullOrEmpty(unificacao["@FIXO"]) && string.IsNullOrEmpty(unificacao["@CELULAR"]) && string.IsNullOrEmpty(unificacao["@EMAIL"]))
                    {
                        throw new Exception("Nenhum contato a unificar");
                    }
                    contatos = contatos.Select(p =>
                    {
                        p.Cliente = clienteDestino;
                        p.NomeContato = clienteUnificar.NomeFantasia;
                        p.Email = clienteUnificar.Email_Principal;
                        p.Telefone = string.IsNullOrEmpty(unificacao["@CELULAR"]) ? unificacao["@FIXO"].Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : unificacao["@CELULAR"];
                        p.Tipo_Contato = string.IsNullOrEmpty(unificacao["@CELULAR"]) ? Tipo_Contato_Cliente.TelefoneFixo : Tipo_Contato_Cliente.TelefoneCelular;

                        return p;
                    }).ToList();

                    await repoClienteContato.SaveOrUpdate(contatos);

                    foreach (Atendimento item in atendimentos)
                    {
                        Atendimento modeloAnterior = (Atendimento)item.Clone();
                        item.Cliente = clienteDestino;
                        item.Pessoa_Contato = clienteUnificar.NomeFantasia;
                        item.Tipo_Pessoa_Contato = Tipo_Pessoa_Contato.Outros;
                        item.IdPessoaContato = contatos.FirstOrDefault()?.Id;

                        await repoAtendimento.SaveOrUpdate(item);
                        var logs = item.GeraLog(modeloAnterior, request.Usuario, request.nomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                        await repositoryLogs.Save(logs);
                    }

                    foreach (Cliente_PlanoProduto item in planosProdutos)
                    {
                        item.Cliente = clienteDestino;
                        await repoClientePlanoProduto.SaveOrUpdate(item);
                    }

                    foreach (Model.nhibernate.LoginRadius item in login)
                    {
                        item.Cliente = clienteDestino;
                        await repoLoginRadius.SaveOrUpdate(item);
                    }

                    foreach (Cliente_Anexo item in anexos)
                    {
                        item.Cliente = clienteDestino;
                        await repoClienteAnexo.SaveOrUpdate(item);
                    }

                    foreach (Forma_Pagamento_Cliente item in formasPagamento)
                    {
                        item.Cliente = clienteDestino;
                        await repoForma_Pagamento_Cliente.SaveOrUpdate(item);
                    }

                    foreach (Cliente_Desconto item in descontos)
                    {
                        item.Cliente = clienteDestino;
                        await repoClienteDesconto.SaveOrUpdate(item);
                    }

                    foreach (Pesquisa_Resposta item in respostasPesquisa)
                    {
                        item.Cliente = clienteDestino;
                        await repoPesquisaResposta.SaveOrUpdate(item);
                    }

                    foreach (Cliente item in indicacoes)
                    {
                        item.Quem_Indicou = clienteDestino;
                        await repoCliente.SaveOrUpdate(item);
                    }
                    //atualizacoes EVA
                    using (var com = session.Connection.CreateCommand())
                    {
                        transaction.Enlist(com);

                        StringBuilder sql = new StringBuilder($"UPDATE TB_NEGOCIACAO SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_FATURAMENTO_CLIENTE SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_CREDITO SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_CONTAS_PAGAR SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_CONTA_RECEBER SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_COBRANCA_NEGOCIACAO SET ID_PAGADOR = {clienteDestino.Id} WHERE ID_PAGADOR = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_COBRANCA_FATURAMENTO SET ID_PAGADOR = {clienteDestino.Id} WHERE ID_PAGADOR = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        retorno.Ok = true;
                    }

                    //await repoCliente.Remove(clienteUnificar);
                    transaction.Commit();
                    transactionLog.Commit();
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/UnificarCliente")]
        public async Task<PostApiRetorno> UnificarCliente(UnificarClienteRequestModel request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (ITransaction transaction = session.BeginTransaction())
            using (ITransaction transactionLog = sessionLog.BeginTransaction())
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            using (Conexao<Atendimento> repoAtendimento = new Conexao<Atendimento>(session))
            using (Conexao<Cliente_Contato> repoClienteContato = new Conexao<Cliente_Contato>(session))
            using (Conexao<Cliente_PlanoProduto> repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (Conexao<Model.nhibernate.LoginRadius> repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (Conexao<Cliente_Anexo> repoClienteAnexo = new Conexao<Cliente_Anexo>(session))
            using (Conexao<Forma_Pagamento_Cliente> repoForma_Pagamento_Cliente = new Conexao<Forma_Pagamento_Cliente>(session))
            using (Conexao<Cliente_Desconto> repoClienteDesconto = new Conexao<Cliente_Desconto>(session))
            using (Conexao<Pesquisa_Resposta> repoPesquisaResposta = new Conexao<Pesquisa_Resposta>(session))
            using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
            {
                try
                {
                    Cliente clienteUnificar = await repoCliente.selectFirst(p => p.Id == request.IdClienteUnificar);
                    Cliente clienteDestino = await repoCliente.selectFirst(p => p.Id == request.IdClienteDestino);
                    List<Atendimento> atendimentos = (await repoAtendimento.select(p => p.Cliente == clienteUnificar)).ToList();

                    List<Cliente_Contato> contatos = (await repoClienteContato.select(p => p.Cliente == clienteUnificar)).ToList();
                    List<Cliente_PlanoProduto> planosProdutos = clienteUnificar.PlanosProdutos.ToList();
                    clienteUnificar.PlanosProdutos = new List<Cliente_PlanoProduto>();

                    List<Model.nhibernate.LoginRadius> login = (await repoLoginRadius.select(p => p.Cliente == clienteUnificar)).ToList();
                    List<Cliente_Anexo> anexos = (await repoClienteAnexo.select(p => p.Cliente == clienteUnificar)).ToList();
                    List<Forma_Pagamento_Cliente> formasPagamento = clienteUnificar.Formas_Pagamento_Cliente.ToList();
                    List<Cliente_Desconto> descontos = clienteUnificar.Descontos.ToList();
                    clienteUnificar.Descontos = new List<Cliente_Desconto>();
                    clienteUnificar.Formas_Pagamento_Cliente = new List<Forma_Pagamento_Cliente>();
                    List<Pesquisa_Resposta> respostasPesquisa = (await repoPesquisaResposta.select(p => p.Cliente == clienteUnificar)).ToList();
                    List<Cliente> indicacoes = (await repoCliente.select(p => p.Quem_Indicou == clienteUnificar)).ToList();

                    contatos = contatos.Select(p => { p.Cliente = clienteDestino; return p; }).ToList();
                    var unificacao = new Dictionary<string, string>
                    {
                        {"@FIXO", string.IsNullOrEmpty(clienteUnificar.Telefone_Fixo) ? clienteUnificar.Telefone_Fixo2 : clienteUnificar.Telefone_Fixo },
                        {"@EMAIL", string.IsNullOrEmpty(clienteUnificar.Email_Principal) ? clienteUnificar.Email_Aux : clienteUnificar.Email_Principal },
                        {"@CELULAR", clienteUnificar.Telefone_Celular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") }
                    };
                    if (string.IsNullOrEmpty(unificacao["@FIXO"]) && string.IsNullOrEmpty(unificacao["@CELULAR"]) && string.IsNullOrEmpty(unificacao["@EMAIL"]))
                    {
                        throw new Exception("Nenhum contato a unificar");
                    }
                    var contatosEmpresa = (await repoClienteContato.select(p => p.Cliente == clienteDestino && p.NomeContato == clienteUnificar.NomeFantasia)).ToList();
                    var contatoAAdicionar = new Cliente_Contato
                    {
                        Cliente = clienteDestino,
                        NomeContato = clienteUnificar.NomeFantasia,
                        Email = clienteUnificar.Email_Principal,
                        Telefone = string.IsNullOrEmpty(unificacao["@CELULAR"]) ? unificacao["@FIXO"].Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : unificacao["@CELULAR"],
                        Tipo_Contato = string.IsNullOrEmpty(unificacao["@CELULAR"]) ? Tipo_Contato_Cliente.TelefoneFixo : Tipo_Contato_Cliente.TelefoneCelular
                    };

                    contatos.Add(contatoAAdicionar);
                    await repoClienteContato.SaveOrUpdate(contatos);

                    foreach (Atendimento item in atendimentos)
                    {
                        Atendimento modeloAnterior = (Atendimento)item.Clone();
                        item.Cliente = clienteDestino;
                        item.Pessoa_Contato = clienteUnificar.NomeFantasia;
                        item.Tipo_Pessoa_Contato = Tipo_Pessoa_Contato.Outros;
                        item.IdPessoaContato = contatos.FirstOrDefault()?.Id;

                        await repoAtendimento.SaveOrUpdate(item);
                        var logs = item.GeraLog(modeloAnterior, request.Usuario, request.nomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                        await repositoryLogs.Save(logs);
                    }

                    foreach (Cliente_PlanoProduto item in planosProdutos)
                    {
                        item.Cliente = clienteDestino;
                        await repoClientePlanoProduto.SaveOrUpdate(item);
                    }

                    foreach (Model.nhibernate.LoginRadius item in login)
                    {
                        item.Cliente = clienteDestino;
                        await repoLoginRadius.SaveOrUpdate(item);
                    }

                    foreach (Cliente_Anexo item in anexos)
                    {
                        item.Cliente = clienteDestino;
                        await repoClienteAnexo.SaveOrUpdate(item);
                    }

                    foreach (Forma_Pagamento_Cliente item in formasPagamento)
                    {
                        item.Cliente = clienteDestino;
                        await repoForma_Pagamento_Cliente.SaveOrUpdate(item);
                    }

                    foreach (Cliente_Desconto item in descontos)
                    {
                        item.Cliente = clienteDestino;
                        await repoClienteDesconto.SaveOrUpdate(item);
                    }

                    foreach (Pesquisa_Resposta item in respostasPesquisa)
                    {
                        item.Cliente = clienteDestino;
                        await repoPesquisaResposta.SaveOrUpdate(item);
                    }

                    foreach (Cliente item in indicacoes)
                    {
                        item.Quem_Indicou = clienteDestino;
                        await repoCliente.SaveOrUpdate(item);
                    }
                    //atualizacoes EVA
                    using (var com = session.Connection.CreateCommand())
                    {
                        transaction.Enlist(com);

                        StringBuilder sql = new StringBuilder($"UPDATE TB_NEGOCIACAO SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_FATURAMENTO_CLIENTE SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_CREDITO SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_CONTAS_PAGAR SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_CONTA_RECEBER SET ID_CLIENTE = {clienteDestino.Id} WHERE ID_CLIENTE = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_COBRANCA_NEGOCIACAO SET ID_PAGADOR = {clienteDestino.Id} WHERE ID_PAGADOR = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        sql = new StringBuilder($"UPDATE TB_COBRANCA_FATURAMENTO SET ID_PAGADOR = {clienteDestino.Id} WHERE ID_PAGADOR = {clienteUnificar.Id}");
                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                        retorno.Ok = true;
                    }

                    //await repoCliente.Remove(clienteUnificar);
                    transaction.Commit();
                    transactionLog.Commit();
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/clienteapi/EnviaEmailCadastroCartao")]
        public async Task<object> EnviaEmailCadastroCartao(EnvioLinkCartaoModel model)
        {
            try
            {
                using (Configuracoes_GeraisApiController configApi = new Configuracoes_GeraisApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoEmail = new Conexao<Conta_Email>(session))
                {
                    var emailConfigurado = await repoEmail.selectFirst();

                    if (emailConfigurado == null || string.IsNullOrEmpty(emailConfigurado.Email))
                        return new { ok = false, mensagem = "E-mail remetente não informado! Configure em: Configurações da Empresa >> Aba Comercial" };

                    EmailUtil email = new EmailUtil(emailConfigurado.Email, emailConfigurado.Senha, emailConfigurado.Servidor_Smtp, emailConfigurado.Porta_Smtp);
                    email.EnviarMensagem(model.EmailCliente, $"Olá, Você está recebendo o link da sua compra, correspondente aos produtos e serviços fornecidos pela empresa {configApi.GetNomeEmpresa()}", $"Segue o link para cadastrar o cartão de crédito  {model.Link}", CONSTANTES.Email_UsaSsl == "S");

                    return new { ok = true };
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [Route("api/clienteapi/GeraLinkCadastroEmail")]
        public async Task<PostApiRetorno<string>> GeraLinkCadastroEmail(CadastroCartaoModel model)
        {
            var retorno = new PostApiRetorno<string>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var clienteRepo = new Conexao<Cliente>(session))
                using (var cadastroRepo = new Conexao<CadastroCartaoEmail>(session))
                using (var urlRepo = new Conexao<URL_Sistema>(session))
                using (var formPag = new Conexao<Forma_Pagamento>(session))
                {
                    var cliente = await clienteRepo.selectFirst(x => x.Id == model.IdCliente);
                    var urlEVA = await urlRepo.selectFirst(x => x.Sistema == Sistema.EVA);

                    if (urlEVA == null)
                    {
                        throw new Exception("Url EVA não cadastrado em configurações da empresa.");
                    }

                    CadastroCartaoEmail obj = new CadastroCartaoEmail
                    {
                        DataExpiracao = DateTime.Now.AddDays(3),
                        IdCliente = model.IdCliente,
                        IdForma = model.IdFormaPagamento ?? formPag.selectFirst(x => x.Id ==
                                                                                cliente.Formas_Pagamento_Cliente.FirstOrDefault(x => x.Forma_Pagamento.Tipo_Forma_Pagamento ==
                                                                                Tipo_Forma_Pagamento.Cartao_Credito).Forma_Pagamento.Id).Result?.Id,
                        Token = Guid.NewGuid().ToString()
                    };

                    obj.Link = string.Concat(urlEVA.URL, "/pt-br/Venda/Cartao/?token=", obj.Token);

                    await cadastroRepo.Save(obj);

                    retorno.IdObjeto = obj.Link;
                    retorno.Ok = true;
                }
            }
            catch (Exception e)
            {
                retorno.Mensagem = e.Message;
                retorno.Ok = false;
            }

            return retorno;
        }

        public async Task<Int64?> GetIdCategoriaCliente(Int64 Id_Cliente_PlanoProduto)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            {
                var Cliente = await repoCliente.selectFirst(p => p.Id == Id_Cliente_PlanoProduto);
                return Cliente.Categoria_Cliente?.Id;
            }
        }


        public async Task<Int64> GetIdCliente(Int64 Id_Cliente_PlanoProduto)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            {
                var ClientePlanoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == Id_Cliente_PlanoProduto);
                return ClientePlanoProduto.Cliente.Id;
            }
        }

        public async Task<string> GetCpfCliente(Int64 Id_Cliente)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            {
                var Cliente = await repoCliente.selectFirst(p => p.Id == Id_Cliente);
                return Cliente.CpfCnpj;
            }
        }


        public async Task<Escritorio_Area> GetEscritorio_Area(Int64 Id_Plano_Produto)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            {
                var plano_produto = await repoPlanoProduto.selectFirst(p => p.Id == Id_Plano_Produto);


                return new Escritorio_Area
                {
                    Cod_Area = plano_produto?.Cod_Area,
                    Cod_Escritorio = plano_produto?.Cod_Escritorio
                };
            }
        }

        public async Task GetEnderecoImportacao(Cliente cliente, Tipo_EnderecoCliente tipo_EnderecoCliente, Cliente_PlanoProduto planoProdutoCobranca = null)
        {
            GetApiRetorno<EnderecoViaCep> retornoApi = null;
            using (EnderecoApiController apiEndereco = new EnderecoApiController())
            {
                switch (tipo_EnderecoCliente)
                {
                    case Tipo_EnderecoCliente.Principal:
                        retornoApi = (await apiEndereco.GetEnderecoByCep(cliente.Endereco_Principal.Cep));

                        if (retornoApi.Ok)
                        {
                            cliente.Endereco_Principal.Id_Cidade = retornoApi.Objeto.Id_Cidade;
                            cliente.Endereco_Principal.Id_Bairro = retornoApi.Objeto.Id_Bairro;
                            cliente.Endereco_Principal.UF = retornoApi.Objeto.Uf;

                            //cliente_prospecto.Endereco_Principal.Logradouro = retornoApi.Objeto.Endereco;

                            object retornoLatLng = apiEndereco.GetLatLong(
                                                                  cliente.Endereco_Principal.Logradouro,
                                                                  Convert.ToString(cliente.Endereco_Principal.Numero),
                                                                  "",
                                                                  cliente.Endereco_Principal.UF,
                                                                  retornoApi.Objeto.Cidade,
                                                                  retornoApi.Objeto.Bairro
                                                                );

                            cliente.Endereco_Principal.Latitude = ((dynamic)retornoLatLng).lat;
                            cliente.Endereco_Principal.Longitude = ((dynamic)retornoLatLng).lng;


                        }
                        break;
                    case Tipo_EnderecoCliente.Cobranca:
                        if (planoProdutoCobranca != null)
                        {
                            retornoApi = (await apiEndereco.GetEnderecoByCep(planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.Cep));

                            if (retornoApi.Ok)
                            {
                                planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.Id_Cidade = retornoApi.Objeto.Id_Cidade;
                                planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.Id_Bairro = retornoApi.Objeto.Id_Bairro;
                                planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.UF = retornoApi.Objeto.Uf;

                                //cliente_prospecto.Endereco_Principal.Logradouro = retornoApi.Objeto.Endereco;

                                object retornoLatLng = apiEndereco.GetLatLong(
                                                                      planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.Logradouro,
                                                                      Convert.ToString(planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.Numero),
                                                                      "",
                                                                      planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.UF,
                                                                      retornoApi.Objeto.Cidade,
                                                                      retornoApi.Objeto.Bairro
                                                                    );

                                planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.Latitude = ((dynamic)retornoLatLng).lat;
                                planoProdutoCobranca.Forma_Pagamento_Cliente.Endereco_Cobranca.Longitude = ((dynamic)retornoLatLng).lng;


                            }
                        }
                        break;
                    case Tipo_EnderecoCliente.Instalacao:
                        if (cliente.PlanosProdutos.Count > 0)
                        {
                            foreach (Cliente_PlanoProduto planoProduto in cliente.PlanosProdutos)
                            {
                                retornoApi = (await apiEndereco.GetEnderecoByCep(planoProduto.Endereco_Instalacao.Cep));

                                if (retornoApi.Ok)
                                {
                                    planoProduto.Endereco_Instalacao.Id_Cidade = retornoApi.Objeto.Id_Cidade;
                                    planoProduto.Endereco_Instalacao.Id_Bairro = retornoApi.Objeto.Id_Bairro;
                                    planoProduto.Endereco_Instalacao.UF = retornoApi.Objeto.Uf;

                                    object retornoLatLng = apiEndereco.GetLatLong(
                                                                  planoProduto.Endereco_Instalacao.Logradouro,
                                                                  Convert.ToString(planoProduto.Endereco_Instalacao.Numero),
                                                                  "",
                                                                  planoProduto.Endereco_Instalacao.UF,
                                                                  retornoApi.Objeto.Cidade,
                                                                  retornoApi.Objeto.Bairro
                                                                );

                                    planoProduto.Endereco_Instalacao.Latitude = ((dynamic)retornoLatLng).lat;
                                    planoProduto.Endereco_Instalacao.Longitude = ((dynamic)retornoLatLng).lng;
                                }
                            }
                        }
                        break;
                }
            }
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, Cliente_Importacao item, Cliente cliente)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<Pais> repoPais = new Conexao<Pais>(session))
            using (Conexao<Categoria_Cliente> repoCategoriaCliente = new Conexao<Categoria_Cliente>(session))
            using (Conexao<Meio_Captacao> repoMeioCaptacao = new Conexao<Meio_Captacao>(session))
            using (Conexao<Tipo_Imovel> repoTipoImovel = new Conexao<Tipo_Imovel>(session))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            using (Conexao<Plano> repoPlano = new Conexao<Plano>(session))
            using (Conexao<Produto> repoProduto = new Conexao<Produto>(session))
            using (Conexao<Banco> repoBanco = new Conexao<Banco>(session))
            using (Conexao<Forma_Pagamento> repoFormaPagamento = new Conexao<Forma_Pagamento>(session))
            using (Conexao<Vencimento> repoVencimento = new Conexao<Vencimento>(session))
            using (ClienteApiController clienteApi = new ClienteApiController())
            {
                if ((await repoCliente.selectFirst(p => p.Numero_Contrato == item.Numero_Contrato)) != null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "A Identificação Cliente pertence a um outro Cliente/Prospect criado. Cliente não foi importado."
                    });

                Pais pais = null;
                Pais pais_Financeiro = null;
                Pais pais_Plano = null;
                Meio_Captacao meio_Captacao = null;
                Categoria_Cliente categoria_Cliente = null;
                Plano plano = null;
                Produto produto = null;
                Forma_Pagamento forma_Pagamento = null;
                Vencimento vencimento = null;
                Banco banco = null;
                Tipo_Imovel tipo_Imovel = null;

                if (item.Pais.HasValue)
                {
                    pais = await repoPais.selectFirst(p => p.Id == item.Pais);
                    if (pais == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "País não encontrado no sistema" });
                }
                if (item.Pais_Financeiro.HasValue)
                {
                    pais_Financeiro = await repoPais.selectFirst(p => p.Id == item.Pais_Financeiro);
                    if (pais == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "País do endereço do Financeiro não encontrado no sistema" });
                }
                if (item.Pais_Plano.HasValue)
                {
                    pais_Plano = await repoPais.selectFirst(p => p.Id == item.Pais_Plano);
                    if (pais_Plano == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "País do endereço do Plano/Produto não encontrado no sistema" });
                }
                if (item.Tipo_Imovel.HasValue)
                {
                    tipo_Imovel = await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == item.Tipo_Imovel);
                    if (tipo_Imovel == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Código do Tipo de Imóvel não encontrado no sistema" });
                }
                if (item.Meio_Captacao.HasValue)
                {
                    meio_Captacao = await repoMeioCaptacao.selectFirst(p => p.Id == Convert.ToInt64(item.Meio_Captacao));
                    if (meio_Captacao == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Código do Meio de Captação não encontrado no sistema" });
                }
                if (item.Categoria_Cliente.HasValue)
                {
                    categoria_Cliente = await repoCategoriaCliente.selectFirst(p => p.Id == Convert.ToInt64(item.Categoria_Cliente));
                    if (categoria_Cliente == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Código da Categoria de Cliente não encontrado no sistema" });
                }
                if (item.Cod_Plano.HasValue)
                {

                    if (item.validaCamposPlano().Count > 0)
                        retorno.AddRange(item.validaCamposPlano());

                    plano = await repoPlano.selectFirst(p => Convert.ToInt64(p.Codigo) == item.Cod_Plano);
                    produto = await repoProduto.selectFirst(p => Convert.ToInt64(p.Codigo) == item.Cod_Plano);
                    if (plano == null && produto == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Código do Plano não encontrado no sistema" });
                }
                if (item.Forma_Pagamento.HasValue)
                {
                    forma_Pagamento = await repoFormaPagamento.selectFirst(p => p.Id == item.Forma_Pagamento);
                    if (forma_Pagamento == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Código da Forma de Pagamento não encontrada no sistema" });
                }

                if (forma_Pagamento != null && forma_Pagamento.Tipo_Forma_Pagamento == Tipo_Forma_Pagamento.Debito_Conta)
                {
                    if (!String.IsNullOrEmpty(item.Banco))
                    {
                        banco = await repoBanco.selectFirst(p => p.Descricao == item.Banco);
                        if (banco == null)
                            retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Banco não encontrado no sistema" });
                    }
                    else
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Necessário informar o banco para Forma de Pagamento Débito em Conta" });
                }

                if (item.Dia_Vencimento.HasValue)
                {
                    vencimento = await repoVencimento.selectFirst(p => p.DiaVencimento == item.Dia_Vencimento);
                    if (vencimento == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Dia de Vencimento não encontrado no sistema" });
                }

                if (retorno.Count() > 0)
                    return retorno;

                cliente.Qual_Meio = meio_Captacao;
                cliente.Categoria_Cliente = categoria_Cliente;
                cliente.Numero_Contrato = item.Numero_Contrato;
                cliente.Telefone_Fixo = item.Telefone_Fixo;
                cliente.Telefone_Celular = item.Telefone_Celular;
                cliente.Telefone_Recado = item.Telefone_Recado;
                cliente.Pessoa_Tel_Recado = item.Pessoa_Recado;
                cliente.Email_Principal = item.Email_Principal;
                cliente.Tipo_Cliente = Tipo_Cliente.Cliente;
                cliente.Status_Prospecto = Status_Prospecto.Efetivado;
                cliente.Status = Status_Cliente.Ativo;
                cliente.Endereco_Principal = new Endereco_Cliente
                {
                    Bairro = item.Bairro,
                    Cep = item.Cep,
                    Cidade = item.Cidade,
                    Numero = item.Numero,
                    UF = item.UF,
                    Pais = pais,
                    Logradouro = item.Logradouro,
                    Complemento = item.Complemento,
                    Ponto_Referencia = item.Ponto_Referencia,
                    Tipo_Imovel = item.Tipo_Imovel.HasValue ? await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == item.Tipo_Imovel) : null
                };

                List<Forma_Pagamento_Cliente> listaFormasPagamentoCliente = new List<Forma_Pagamento_Cliente>();
                listaFormasPagamentoCliente.Add(new Forma_Pagamento_Cliente
                {
                    Forma_Pagamento = forma_Pagamento,
                    Banco = banco,
                    Agencia = item.Agencia,
                    Conta_Corrente = item.Conta_Corrente,
                    Digito = item.Digito,
                    Vencimento = vencimento,
                    Forma_Envio = item.Forma_Envio == null ? (Cliente_FormaEnvio?)null : ((Cliente_FormaEnvio?)Enum.Parse(typeof(Cliente_FormaEnvio), item.Forma_Envio)),
                    Endereco_Cobranca = new Endereco_Cliente
                    {
                        Bairro = item.Bairro_Financeiro,
                        Cep = item.Cep_Financeiro,
                        Cidade = item.Cidade_Financeiro,
                        Numero = item.Numero_Financeiro,
                        UF = item.UF_Financeiro,
                        Pais = pais_Financeiro,
                        Logradouro = item.Logradouro_Financeiro,
                        Complemento = item.Complemento_Financeiro,
                        Ponto_Referencia = item.Ponto_Referencia_Financeiro
                    },
                    Dia_Vencimento = Convert.ToString(vencimento.DiaVencimento),
                    Status = Status_Default.Ativo
                });
                cliente.Formas_Pagamento_Cliente = listaFormasPagamentoCliente;

                if (item.Cod_Plano.HasValue)
                {
                    var listaPlanosProdutos = new List<Cliente_PlanoProduto>();

                    double valor_final;
                    double valor_instalacao_final;

                    double.TryParse(item.Valor_Final_Plano.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valor_final);
                    double.TryParse(item.Valor_Final_Plano.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valor_instalacao_final);

                    listaPlanosProdutos.Add(new Cliente_PlanoProduto
                    {
                        Plano = plano,
                        Produto = produto,
                        Cliente = cliente,
                        Valor_Final = valor_final,
                        Valor_Instalacao_Final = valor_instalacao_final,
                        Valor_Final_Total = valor_final + valor_instalacao_final,
                        Endereco_Instalacao = new Endereco_Cliente
                        {
                            Bairro = item.Bairro_Plano,
                            Cep = item.Cep_Plano,
                            Cidade = item.Cidade_Plano,
                            Numero = item.Numero_Plano,
                            UF = item.UF_Plano,
                            Pais = pais_Plano,
                            Logradouro = item.Logradouro_Plano,
                            Complemento = item.Complemento_Plano,
                            Ponto_Referencia = item.Ponto_Referencia_Plano
                        },
                        Forma_Pagamento_Cliente = cliente.Formas_Pagamento_Cliente[0],
                        Periodo_Carencia = (item.Periodo_Carencia.HasValue) ? true : false,
                        Meses_Carencia = item.Periodo_Carencia,
                        Parcelas_Instalacao = item.Parcelas_Instalacao,
                        Status = Status_PlanoProduto.Habilitado,
                        Cod_Escritorio = item.Grupo_Area,
                        Cod_Area = item.Area
                    });
                    cliente.PlanosProdutos = listaPlanosProdutos;
                }

                await clienteApi.GetEnderecoImportacao(cliente, Tipo_EnderecoCliente.Principal);
                await clienteApi.GetEnderecoImportacao(cliente, Tipo_EnderecoCliente.Instalacao);

                foreach (Cliente_PlanoProduto p in cliente.PlanosProdutos)
                {
                    await clienteApi.GetEnderecoImportacao(cliente, Tipo_EnderecoCliente.Cobranca, p);
                }
            }

            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloFisico(Cliente_FisicaImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            {
                Cliente cliente = new Cliente
                {
                    Status = Status_Cliente.Ativo,
                    DataHoraCriacao = DateTime.Now,
                    UsuarioCriacao = User.Identity.GetUserId(),
                    Tipo_Cliente = Tipo_Cliente.Cliente
                };

                retorno = await ImportacaoModeloBase(session, item, cliente);
                cliente.CpfCnpj = item.Cpf.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                List<Cliente> clientes_cpf = (await repoCliente.select(p => p.CpfCnpj == cliente.CpfCnpj)).ToList();
                if (clientes_cpf.Count > 0)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Cpf já utilizado por outro Cliente/Prospect"
                    });

                cliente.NomeFantasia = item.Nome;
                cliente.Filiacao_Mae = item.Mae;
                cliente.Filiacao_Pai = item.Pai;
                cliente.Rg = item.RG;
                cliente.Data_Nascimento = item.Data_Nascimento;
                cliente.Data_Emissao_Rg = item.Data_Emissao_RG;
                cliente.Orgao_Emissao_Rg = item.Orgao_RG;
                cliente.Nacionalidade = !String.IsNullOrEmpty(item.Nacionalidade) ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.Nacionalidade) : null;
                cliente.Profissao = item.Profissao;
                cliente.Tipo_Pessoa = Tipo_Pessoa.Fisica;

                if (item.Sexo == "M") cliente.Sexo = Sexo.Masculino; else if (item.Sexo == "F") cliente.Sexo = Sexo.Feminino;

                try
                {
                    cliente.Estado_Civil = item.Estado_Civil == null ? (Estado_Civil?)null : ((Estado_Civil?)Enum.Parse(typeof(Estado_Civil), item.Estado_Civil));
                }
                catch
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Erro na leitura do campo Estado Civil"
                    });
                }

                try
                {
                    cliente.Grau_Instrucao = item.Grau_Instrucao == null ? (Grau_Instrucao?)null : ((Grau_Instrucao?)Enum.Parse(typeof(Grau_Instrucao), item.Grau_Instrucao));
                }
                catch
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Erro na leitura do campo Grau de Instrução"
                    });
                }

                if (retorno.Count() > 0)
                    return retorno;


                await repoCliente.SaveOrUpdate(cliente);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Numero_Contrato,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloJuridico(Cliente_JuridicaImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            {
                Cliente cliente = new Cliente
                {
                    Status = Status_Cliente.Ativo,
                    DataHoraCriacao = DateTime.Now,
                    UsuarioCriacao = User.Identity.GetUserId(),
                    Tipo_Cliente = Tipo_Cliente.Cliente
                };

                retorno = await ImportacaoModeloBase(session, item, cliente);
                cliente.CpfCnpj = item.CNPJ.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                List<Cliente> clientes_cpf = (await repoCliente.select(p => p.CpfCnpj == cliente.CpfCnpj)).ToList();
                if (clientes_cpf.Count > 0)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Cnpj já utilizado por outro Cliente/Prospect"
                    });

                cliente.NomeFantasia = item.Nome;
                cliente.Razao_Social = item.Razao_Social;
                cliente.Numero_Contrato = item.Numero_Contrato;
                cliente.Telefone_Fixo = item.Telefone_Fixo;
                cliente.Telefone_Celular = item.Telefone_Celular;
                cliente.Telefone_Recado = item.Telefone_Recado;
                cliente.Pessoa_Tel_Recado = item.Pessoa_Recado;
                cliente.Cargo_Responsavel = item.Cargo_Responsavel;
                cliente.Cpf_Responsavel = String.IsNullOrEmpty(item.Cpf_Responsavel) ? null : item.Cpf_Responsavel.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                cliente.Data_Abertura_Empresa = item.Data_Abertura;
                cliente.Inscricao_Estadual = item.Inscricao_Estadual;
                cliente.Inscricao_Municipal = item.Inscricao_Municipal;
                cliente.Ramo_Atividade = item.Ramo_Atividade;
                cliente.Nome_Resp_Financeiro = item.Responsavel;
                cliente.Tipo_Pessoa = Tipo_Pessoa.Juridica;

                try
                {
                    cliente.Porte = String.IsNullOrEmpty(item.Porte) ? (Porte?)null : ((Porte?)Enum.Parse(typeof(Porte), item.Porte));
                }
                catch
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Erro na leitura do campo Porte"
                    });
                }

                if (retorno.Count() > 0)
                    return retorno;

                await repoCliente.SaveOrUpdate(cliente);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Numero_Contrato,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

        [Route("api/clienteapi/ExcluirContato")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirContato(Int64 id)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Cliente_Contato>(session))
                    {

                        Cliente_Contato modelo = null;
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

        [Route("api/clienteapi/AdicionarContato")]
        [HttpPost()]
        public async Task<PostApiRetorno<Int64>> AdicionarContato(AdicionarContatoRequestModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Cliente_Contato>(session))
                    using (var repositoryCliente = new Conexao<Cliente>(session))
                    {

                        Cliente_Contato modelo = request.Id.HasValue ? await repository.selectFirst(p => p.Id == request.Id) : new Cliente_Contato();
                        modelo.Telefone = request.Telefone;
                        modelo.Email = request.Email;
                        modelo.Informacao_Adicional = request.Nome;
                        modelo.NomeContato = request.NomeContato;
                        modelo.Tipo_Contato = (Tipo_Contato_Cliente?)request.TipoContato;
                        modelo.Cliente = await repositoryCliente.selectFirst(p => p.Id == request.IdCliente);

                        if (modelo.Id == 0)
                        {
                            modelo.UsuarioCriacao = request.Usuario;
                            modelo.DataHoraCriacao = DateTime.Now;
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = request.Usuario;
                            modelo.DataHoraAlteracao = DateTime.Now;
                        }

                        await repository.SaveOrUpdate(modelo);

                        retorno.IdObjeto = modelo.Id;
                        retorno.Ok = true;


                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/clienteapi/ClienteEmNegociacao")]
        [HttpGet()]
        public async Task<GetApiRetorno<NegociacaoCliente>> ClienteEmNegociacao(Int64? Id_Cliente)
        {

            GetApiRetorno<NegociacaoCliente> retorno = new GetApiRetorno<NegociacaoCliente>
            {
                Objeto = new NegociacaoCliente()
            };



            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        Int32 recordsTotal = 0;
                        StringBuilder sql = new StringBuilder("SELECT ID, VALOR_NEGOCIADO FROM TB_NEGOCIACAO WHERE ID_CLIENTE = @ID_CLIENTE AND STATUS_NEGOCIACAO <> 2");
                        parametros.Add("@ID_CLIENTE", Id_Cliente);


                        List<double> valores = new List<double>();
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Possui_Negociacao = true;
                                if (reader["VALOR_NEGOCIADO"] != DBNull.Value)
                                    valores.Add(Convert.ToDouble(reader["VALOR_NEGOCIADO"]));
                            }

                            retorno.Objeto.Valor_Negociado = valores.Sum().ToString("N2");

                        }
                    }

                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.Message != null ? ex.InnerException.Message : null;
            }

            return retorno;
        }


        [Route("api/clienteapi/GetAllFaturasAberto")]
        [HttpPost()]
        public async Task<DataTableResponse<Faturamento_ClienteListaModel>> GetAllFaturasAberto(Faturamento_Cliente_DataTableRequest request)
        {
            List<Faturamento_ClienteListaModel> retorno = new List<Faturamento_ClienteListaModel>();
            string mensagemRetorno = "";

            //List<PostGetFaturasAbertas> retorno = new List<PostGetFaturasAbertas>();
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
                    sqlFrom.AppendLine("ON(E.ID_CLIENTE = B.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_FORMA_PAGAMENTO C");
                    sqlFrom.AppendLine("ON(A.ID_FATURAMENTO_FORMA = C.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_PROTOCOLO D");
                    sqlFrom.AppendLine("ON(C.ID_FATURAMENTO_PROTOCOLO = D.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_COBRANCA_FATURAMENTO F");
                    sqlFrom.AppendLine("ON(F.ID_FATURAMENTO_CLIENTE = A.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FORMA_PAGAMENTO G");
                    sqlFrom.AppendLine("ON(G.id = C.id_forma_pagamento)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FORMA_PAGAMENTO_CLIENTE H");
                    sqlFrom.AppendLine("ON(H.id = A.ID_FORMA_PAGAMENTO_CLIENTE)");
                    sqlFrom.AppendLine("LEFT JOIN TB_ENDERECO_CLIENTE I");
                    sqlFrom.AppendLine("ON(I.ID = H.ID_ENDERECO_COBRANCA)");
                    sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE_CONTRATO J");
                    sqlFrom.AppendLine("ON(J.ID_CLIENTE = B.ID)");

                    StringBuilder sqlCount = new StringBuilder();
                    sqlCount.AppendLine("Select Count(A.ID) CO");
                    sqlCount.AppendLine(sqlFrom.ToString());
                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, E.VALOR_PREVISTO, B.CPF, E.VALOR_RECEBIDO,E.DATA_RECEBIMENTO, E.DATA_VENCIMENTO,");
                    sql.AppendLine("A.ID_CLIENTE, E.DOCUMENTO, D.NUM_PROTOCOLO , E.STATUS, G.DESCRICAO AS FORMA_PAGAMENTO,");
                    sql.AppendLine("f.ID_BOLETO, f.LINHA_DIGITAVEL, f.LINK,");
                    sql.AppendLine("B.NOMEFANTASIA AS NOME_CLIENTE, E.ID AS ID_CONTA_RECEBER,H.DIA_VENCIMENTO,I.ENDERECO_LOGRADOURO, I.ENDERECO_NUMERO, D.ID ID_FATURAMENTO_PROTOCOLO");
                    sql.AppendLine(sqlFrom.ToString());

                    //if (!String.IsNullOrEmpty(request.PeriodoIni) && !String.IsNullOrEmpty(request.PeriodoFim))
                    //{
                    //    sql.AppendLine("AND D.DATAHORACRIACAO BETWEEN @INI AND @FIM ");
                    //    parametros.Add("@INI", String.Format("{0}", Convert.ToDateTime(request.PeriodoIni).ToString("dd/MM/yyyy HH:mm")));
                    //    parametros.Add("@FIM", String.Format("{0}", Convert.ToDateTime(request.PeriodoFim).AddHours(23).AddMinutes(59)
                    //                                                                                        .AddSeconds(59).ToString("dd/MM/yyyy HH:mm")));
                    //}

                    //if (!String.IsNullOrEmpty(request.Data_Vencimento))
                    //{
                    //    sql.AppendLine("AND A.DATA_VENCIMENTO = @VENCIMENTO ");
                    //    parametros.Add("@VENCIMENTO", String.Format("{0}", request.Data_Vencimento));

                    //}

                    List<string> condicoes = new List<string>();
                    sqlWhere.AppendLine(" WHERE ");
                    if (request.Status_Faturamento.HasValue)
                    {
                        /*5 - TODOS*/
                        if (request.Status_Faturamento != 5)
                        {
                            condicoes.Add(" E.STATUS = @STATUS ");
                            parametros.Add("@STATUS", String.Format("{0}", Convert.ToInt32(request.Status_Faturamento.Value)));
                        }
                    }

                    if (request.Id_Cliente.HasValue)
                    {
                        condicoes.Add(" E.ID_CLIENTE = @ID_CLIENTE ");
                        parametros.Add("@ID_CLIENTE", String.Format("{0}", request.Id_Cliente));
                    }

                    if (request.Id_Endereco.HasValue)
                    {
                        condicoes.Add(" H.ID_ENDERECO_COBRANCA = @endereco ");
                        parametros.Add("@endereco", String.Format("{0}", request.Id_Endereco));
                    }

                    if (request.Id_Contrato.HasValue)
                    {
                        condicoes.Add(" J.ID  = @contrato ");
                        parametros.Add("@contrato", String.Format("{0}", request.Id_Contrato));
                    }

                    if (!String.IsNullOrEmpty(request.Data_Emissao_Fatura_Ini) && !String.IsNullOrEmpty(request.Data_Emissao_Fatura_Fim))
                    {
                        //sql.AppendLine("AND E.DATA_EMISSAO BETWEEN @INI AND @FIM ");
                        condicoes.Add(" E.DATA_VENCIMENTO BETWEEN @INI AND @FIM ");
                        parametros.Add("@INI", String.Format("{0}", Convert.ToDateTime(request.Data_Emissao_Fatura_Ini).ToString("dd/MM/yyyy HH:mm")));
                        parametros.Add("@FIM", String.Format("{0}", Convert.ToDateTime(request.Data_Emissao_Fatura_Fim).AddHours(23).AddMinutes(59)
                                                                                                            .AddSeconds(59).ToString("dd/MM/yyyy HH:mm")));
                    }

                    if (condicoes.Count > 1)
                        sqlWhere.AppendLine(string.Join(" AND ", condicoes.ToList()));
                    else
                        sqlWhere.AppendLine(condicoes.First());

                    sql.AppendLine(sqlWhere.ToString());
                    sql.AppendLine("ORDER BY E.DATA_VENCIMENTO, A.ID");
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = Convert.ToString(reader["ID"]);
                            var data_Vencimento = reader["DATA_VENCIMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENCIMENTO"]).ToString("dd/MM/yyyy") : "";
                            var valor_Previsto = Convert.ToDouble(reader["VALOR_PREVISTO"]).ToString("N2");
                            //var status = Convert.ToString(reader["STATUS"]) == "0" ? "Aberto" : "Vencido";
                            var protocolo = Convert.ToString(reader["NUM_PROTOCOLO"]);
                            var id_Conta_Receber = Convert.ToString(reader["ID_CONTA_RECEBER"]);
                            var forma_pagamento = Convert.ToString(reader["FORMA_PAGAMENTO"]);

                            var dados_boleto = "";

                            if (forma_pagamento == "Boleto")
                            {
                                var pdf = Convert.ToString(reader["LINK"]);
                                var linha_digitavel = Convert.ToString(reader["LINHA_DIGITAVEL"]);

                                dados_boleto = ("*Boleto:* " + pdf +
                                    "\n\r *Linha digitável:* " + linha_digitavel);
                            }

                            if (id != "")
                            {
                                mensagemRetorno = ("\r\n *Protocolo:* " + protocolo +
                                    "\r\n  *Valor:* " + valor_Previsto +
                                    "\r\n  *Data de Vencimento:* " + data_Vencimento +
                                    "\r\n  *Forma Pagamento:* " + forma_pagamento +
                                    "\r\n" + dados_boleto);
                            }

                            retorno.Add(new Faturamento_ClienteListaModel
                            {
                                Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                Cliente = Convert.ToString(reader["NOME_CLIENTE"]),
                                Id_Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : (Int64?)null,
                                Id_Faturamento_Protocolo = reader["ID_FATURAMENTO_PROTOCOLO"] != DBNull.Value ? Convert.ToInt64(reader["ID_FATURAMENTO_PROTOCOLO"]) : (Int64?)null,
                                Cpf = Convert.ToString(reader["CPF"]),
                                Documento = Convert.ToString(reader["DOCUMENTO"]),
                                Data_Vencimento = reader["DATA_VENCIMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENCIMENTO"]).ToString("dd/MM/yyyy") : "",
                                Valor_Previsto = Convert.ToDouble(reader["VALOR_PREVISTO"]).ToString("N2"),
                                Valor_Recebido = reader["VALOR_RECEBIDO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_RECEBIDO"]).ToString("N2") : "",
                                Data_Recebimento = reader["DATA_RECEBIMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_RECEBIMENTO"]).ToString("dd/MM/yyyy") : "",
                                Forma_Pagamento = @$"Vencto Dia {Convert.ToString(reader["DIA_VENCIMENTO"])} - {Convert.ToString(reader["FORMA_PAGAMENTO"])} - {Convert.ToString(reader["ENDERECO_LOGRADOURO"])},{Convert.ToString(reader["ENDERECO_NUMERO"])}",
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Conta_Receber>(Convert.ToString(reader["STATUS"])),
                                Protocolo = Convert.ToString(reader["NUM_PROTOCOLO"]),
                                Id_Conta_Receber = Convert.ToInt64(reader["ID_CONTA_RECEBER"]),
                                Mensagem = mensagemRetorno
                            });
                        }

                    }

                    return new DataTableResponse<Faturamento_ClienteListaModel>
                    {
                        data = retorno
                    };
                }
            }
        }


        [HttpPost()]
        [Route("api/clienteapi/GetDiasInadimplentes")]
        public async Task<Int64> GetDiasInadimplentes(string id_Cliente)
        {

            Int32 recordsTotal = 0;
            Int64 retorno = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionNh = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine(" SELECT DISTINCT A.ID, A.DATA_VENCIMENTO ");

                    sql.AppendLine(" FROM TB_CONTA_RECEBER A ");

                    sql.AppendLine(" INNER JOIN TB_CLIENTE B ");
                    sql.AppendLine(" ON(A.ID_CLIENTE = B.ID) ");
                    sql.AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE C ");
                    sql.AppendLine(" ON(B.ID_ENDERECO_PRINCIPAL = C.ID) ");
                    sql.AppendLine(" LEFT JOIN TB_FORMA_PAGAMENTO_CLIENTE H ");
                    sql.AppendLine(" ON(H.ID_CLIENTE = B.ID) ");
                    sql.AppendLine(" LEFT JOIN TB_FORMA_PAGAMENTO D ");
                    sql.AppendLine(" ON(H.ID_FORMA_PAGAMENTO = D.ID) ");
                    sql.AppendLine(" LEFT JOIN TB_EMPRESA E ");
                    sql.AppendLine(" ON(A.ID_EMPRESA = E.ID) ");
                    sql.AppendLine(" LEFT JOIN TB_FILIAL F ");
                    sql.AppendLine(" ON(A.ID_FILIAL = F.ID) ");
                    sql.AppendLine(" LEFT JOIN TB_COBRANCA_NEGOCIACAO G ");
                    sql.AppendLine(" ON(A.ID_COBRANCA_NEGOCIACAO = G.ID) ");

                    sql.AppendLine(" WHERE A.DATA_VENCIMENTO < CURRENT_DATE AND A.STATUS IN(0,4) ");
                    sql.AppendLine("   AND COALESCE(D.DIAS_COMPENSACAO,0) < DATEDIFF(DAY, A.DATA_VENCIMENTO, CURRENT_TIMESTAMP) ");
                    sql.AppendLine("AND A.ID_CLIENTE = @ID_CLIENTE ");
                    parametros.Add("@ID_CLIENTE", String.Format("{0}", id_Cliente));


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno = reader["DATA_VENCIMENTO"] != DBNull.Value ?
                                          Convert.ToInt64((DateTime.Now.Date - Convert.ToDateTime(reader["DATA_VENCIMENTO"]).Date).TotalDays) : 0;

                        }
                    }

                }
            }

            return retorno;
        }


        [Route("api/clienteapi/GetAllFaturasAbertoTotal")]
        [HttpPost()]
        public async Task<int> GetAllFaturasAbertoTotal(Int64 IdCliente)
        {
            int recordsTotal = 0;

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
                    sqlFrom.AppendLine("ON(E.ID_CLIENTE = B.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_FORMA_PAGAMENTO C");
                    sqlFrom.AppendLine("ON(A.ID_FATURAMENTO_FORMA = C.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FATURAMENTO_PROTOCOLO D");
                    sqlFrom.AppendLine("ON(C.ID_FATURAMENTO_PROTOCOLO = D.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_COBRANCA_FATURAMENTO F");
                    sqlFrom.AppendLine("ON(F.ID_FATURAMENTO_CLIENTE = A.ID)");
                    sqlFrom.AppendLine("LEFT JOIN TB_FORMA_PAGAMENTO G");
                    sqlFrom.AppendLine("ON(G.id = C.id_forma_pagamento)");
                    sqlFrom.AppendLine(" WHERE E.STATUS IN (0, 1)");
                    sqlFrom.AppendLine("AND E.ID_CLIENTE = @ID_CLIENTE ");
                    parametros.Add("@ID_CLIENTE", String.Format("{0}", IdCliente));


                    StringBuilder sqlCount = new StringBuilder();
                    sqlCount.AppendLine("Select Count(E.ID) CO");
                    sqlCount.AppendLine(sqlFrom.ToString());
                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    return recordsTotal;
                }
            }
        }
        private void BindViewToObjDetalhePlanoProduto(ISession session, Cliente_PlanoProduto objBanco, PlanoProdutoListModel view)
        {
            using (var repoClientePlan_Prod = new Conexao<Cliente_PlanoProduto>(session))

            {
                //objBanco.Id = view.Id;
                objBanco.ImpedirBloqueio_Automatico = view.ImpedirBloqueio_Automatico;
                objBanco.ImpedirBloqueio_Sexta = view.ImpedirBloqueio_Sexta;
                objBanco.ImpedirBloqueio_Sabado = view.ImpedirBloqueio_Sabado;
                objBanco.ImpedirBloqueio_Domingo = view.ImpedirBloqueio_Domingo;
                objBanco.ImpedirBloqueio_Feriados = view.ImpedirBloqueio_Feriados;

                objBanco.QtdDiasAtraso = view.QtdDiasAtraso;
                objBanco.VelocidadeDownloadReduzida = view.Velocidade_DownloadReduzida;
                objBanco.VelocidadeUploadReduzida = view.Velocidade_UploadReduzida;

                objBanco.ReducaoVelocidade = view.Reducao_Velocidade;

                objBanco.QtdDiasbloqueados = view.QtdDiasbloqueados;
                objBanco.UtilizaBloqueioDebito = view.BloqueioAutomatico_Debito;

                objBanco.FrequenciaDesbloqueio = view.Frequencia_Desbloqueio;
                //objBanco.Localidade_Anatel = view.Localidade_Anatel;
                objBanco.QtdDiasdesbloqueados = view.dias_desbloq;
                objBanco.UtilizaDesbloqueioConfianca = view.Desbloqueio_Conf;

                objBanco.Meses_Carencia = view.Meses_Carencia;
                objBanco.Dias_Teste_Gratis = view.Qtde_Dias;
                objBanco.Parcelas_Instalacao = view.Numero_Parcelas;
                objBanco.Valor_Instalacao_Real = ConvertToDouble(view.ValorInstalacao);
                objBanco.Valor_Instalacao_Final = ConvertToDouble(view.ValorInstalacao);
                objBanco.Momento_Cobra_Instalacao = view.Momento_Cobra_Instalacao;
                objBanco.CobrarInstalacaoEntrada = view.CobrarInstalacaoEntrada;
                objBanco.ValorInstalacaoEntrada = objBanco.CobrarInstalacaoEntrada ? ConvertToDouble(view.ValorInstalacaoEntrada) : (double?)null;
                objBanco.Quantidade_Produto_Variavel = view.Quantidade;
                objBanco.Valor_Real = Convert.ToDouble(view.ValorPlanoOriginal) * view.Quantidade;
                objBanco.Valor_Total_Plano_Variavel = Convert.ToDouble(view.ValorPlanoOriginal) * view.Quantidade;
                objBanco.Valor_Entrada = ConvertToDouble(view.Valor_Entrada);
                objBanco.Parcela_Entrada = view.Parcela;
                if (objBanco.ValorInstalacaoEntrada > objBanco.Valor_Instalacao_Final)
                {
                    throw new Exception("Valor de entrada não pode ser maior do que o valor total de instalação");
                }

            }
        }
        private double ConvertToDouble(string s)
        {
            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
            }
            catch (Exception e)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        result = 0;
                    }
                }
            }
            return result;
        }


        [HttpPost()]
        [Route("api/clienteapi/GravarDetalhesPlanoProduto")]
        public async Task<PostApiRetorno<Int64>> GravarDetalhesPlanoProduto([FromBody] ParamWebApi<PlanoProdutoListModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    Cliente_PlanoProduto modelo = null;
                    using (var repository = new Conexao<Cliente_PlanoProduto>(session))
                    {

                        modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        BindViewToObjDetalhePlanoProduto(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                        transaction.Commit();
                        retorno.Ok = true;
                    }

                }
                catch (Exception ex)
                {

                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                    retorno.Ok = false;
                }
            }

            return retorno;
        }
        [HttpGet()]
        [Route("api/clienteapi/GetAllContrato")]
        public async Task<GetApiRetorno<ClienteDadosGeraisViewModel>> GetAllContrato(Int64 idcliente, Int64 numcontrato)
        {
            GetApiRetorno<ClienteDadosGeraisViewModel> retorno = new GetApiRetorno<ClienteDadosGeraisViewModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                //using (var repoCliente = new Conexao<Cliente>(session))
                //{
                //    try
                //    {
                //        //var cliente = await repoCliente.selectFirst(p => p.Id == Id);
                //        //retorno.Objeto = await GetViewObj(session, cliente);

                //        retorno.Objeto = await GetViewObj(session, await repoCliente.selectFirst(p => p.Id == Id));

                //        retorno.Ok = true;
                //    }
                //    catch (Exception ex)
                //    {
                //        retorno.Mensagem = ex.Message;
                //        if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                //    }
                //}
            }
            return retorno;
        }
        public async Task<IEnumerable<Endereco_Cliente>> BindEnderecosFromVenda(BindEnderecoFromVendaRequestModel request,
                                                                                ISession session)
        {
            var retorno = new List<Endereco_Cliente>();

            Endereco_Cliente objEndereco = null;

            using (var repoPais = new Conexao<Pais>(session))
            using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))

            using (var repoCondominio = new Conexao<Condominio_Predio>(session))
            using (var repoTipoImovel = new Conexao<Tipo_Imovel>(session))
            {
                switch (request.tipoEndereco)
                {
                    case Tipo_EnderecoCliente.Principal:

                        objEndereco = new Endereco_Cliente
                        {
                            Pais = request.viewVenda.EnderecoPrincipal.Id_Pais != null ? await repoPais.selectFirst(p => p.Id == request.viewVenda.EnderecoPrincipal.Id_Pais.Value) : null,
                            Tipo_Imovel = request.viewVenda.EnderecoPrincipal.Id_Tipo_Imovel != null ? await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == request.viewVenda.EnderecoPrincipal.Id_Tipo_Imovel.Value) : null,
                            Bairro = request.viewVenda.EnderecoPrincipal?.Bairro,

                            Id_Bairro = request.viewVenda.EnderecoPrincipal?.Id_Bairro,
                            Cep = request.viewVenda.EnderecoPrincipal?.CEP,
                            Cidade = request.viewVenda.EnderecoPrincipal?.Cidade,

                            Id_Cidade = request.viewVenda.EnderecoPrincipal?.Id_Cidade,
                            Complemento = request.viewVenda.EnderecoPrincipal?.Complemento,

                            Latitude = !String.IsNullOrEmpty(request.viewVenda.EnderecoPrincipal?.Latitude) ?
                            Convert.ToDouble(request.viewVenda.EnderecoPrincipal?.Latitude?.Replace('.', ',')) : (Double?)null,

                            Longitude = !String.IsNullOrEmpty(request.viewVenda.EnderecoPrincipal?.Longitude) ?
                            Convert.ToDouble(request.viewVenda.EnderecoPrincipal?.Longitude?.Replace('.', ',')) : (Double?)null,

                            Logradouro = request.viewVenda.EnderecoPrincipal?.Logradouro,

                            Numero = request.viewVenda.EnderecoPrincipal?.Numero,
                            Ponto_Referencia = request.viewVenda.EnderecoPrincipal?.Ponto_Referencia,
                            UF = request.viewVenda.EnderecoPrincipal?.UF,

                            Busca_Endereco = request.viewVenda.EnderecoPrincipal.Busca_Endereco != null ? request.viewVenda.EnderecoPrincipal.Busca_Endereco : null,
                            Condominio_Predio = request.viewVenda.EnderecoPrincipal?.Busca_Endereco == Busca_Endereco.Condominio_Predio ? await repoCondominio.selectFirst(p => p.Id == request.viewVenda.EnderecoPrincipal.Id_Condominio_Predio) : null,
                            Bloco = request.viewVenda.EnderecoPrincipal?.Id_Bloco,
                            Apartamento = request.viewVenda.EnderecoPrincipal?.Id_Apartamento,
                            Endereco_Logradouro = request.viewVenda.EnderecoPrincipal?.Busca_Endereco == Busca_Endereco.Endereco_Logradouro ? await repoEnderecoLogradouro.selectFirst(p => p.Id == request.viewVenda.EnderecoPrincipal.Id_Endereco_Logradouro) : null,

                            Cod_Escritorio = request.viewVenda.EnderecoPrincipal?.Cod_Escritorio,
                            Cod_Area = request.viewVenda.EnderecoPrincipal?.Cod_Area,
                            Cod_Localidade =
                                !String.IsNullOrEmpty(request.viewVenda.EnderecoPrincipal?.Cod_Localidade) ?
                                    Convert.ToInt32(request.viewVenda.EnderecoPrincipal?.Cod_Localidade) :
                                    (int?)null,
                        };

                        //request.cliente.Endereco_Principal = objEndereco;

                        if (request.cliente.Id == 0)
                            objEndereco.Id = 0;

                        retorno.Add(objEndereco);

                        break;

                    case Tipo_EnderecoCliente.Cobranca:

                        foreach (Forma_Pagamento_ClienteModels forma in request.Formas)
                        {
                            if (forma.End_Cobranca_Igual_Principal)
                            {
                                objEndereco = new Endereco_Cliente
                                {
                                    Pais = request.cliente.Endereco_Principal?.Pais,
                                    Tipo_Imovel = request.cliente.Endereco_Principal?.Tipo_Imovel,
                                    Id_Bairro = request.cliente.Endereco_Principal?.Id_Bairro,

                                    Id_Cidade = request.cliente.Endereco_Principal?.Id_Cidade,
                                    Bairro = request.cliente.Endereco_Principal?.Bairro,
                                    Cep = request.cliente.Endereco_Principal?.Cep,

                                    Cidade = request.cliente.Endereco_Principal?.Cidade,
                                    Complemento = request.cliente.Endereco_Principal?.Complemento,
                                    Numero = request.cliente.Endereco_Principal?.Numero,

                                    Latitude = request.cliente.Endereco_Principal?.Latitude,
                                    Longitude = request.cliente.Endereco_Principal?.Longitude,
                                    Logradouro = request.cliente.Endereco_Principal?.Logradouro,

                                    Ponto_Referencia = request.cliente.Endereco_Principal?.Ponto_Referencia,
                                    UF = request.cliente.Endereco_Principal?.UF,
                                };


                            }
                            else
                            {
                                objEndereco = new Endereco_Cliente
                                {
                                    Pais = forma.Id_PaisCobranca != null ?
                                                        await repoPais.selectFirst(p => p.Id == forma.Id_PaisCobranca.Value) :
                                                        null,

                                    Tipo_Imovel = forma.Id_Tipo_ImovelCobranca != null ?
                                                               await repoTipoImovel.selectFirst(p =>
                                                                                          p.Cod_Tipo_Imovel == forma.
                                                                                          Id_Tipo_ImovelCobranca.Value) :
                                                               null,
                                    Id_Bairro = forma?.Id_BairroCobranca,
                                    Id_Cidade = forma?.Id_CidadeCobranca,
                                    Bairro = forma?.BairroCobranca,

                                    Cep = forma?.CEPCobranca,
                                    Cidade = forma?.CidadeCobranca,
                                    Complemento = forma?.ComplementoCobranca,
                                    Latitude = forma?.LatitudeCobranca != null ?
                                                            Convert.ToDouble(forma?.LatitudeCobranca?.Replace('.', ',')) :
                                                            (Double?)null,

                                    Longitude = forma?.LongitudeCobranca != null ?
                                                             Convert.ToDouble(forma?.LongitudeCobranca?.Replace('.', ',')) :
                                                             (Double?)null,

                                    Logradouro = forma?.LogradouroCobranca,
                                    Numero = forma?.NumeroCobranca,
                                    Ponto_Referencia = forma?.Ponto_ReferenciaCobranca,
                                    UF = forma?.UFCobranca,
                                };


                            }

                            retorno.Add(objEndereco);

                        }
                        break;
                    case Tipo_EnderecoCliente.Instalacao:
                        objEndereco = new Endereco_Cliente
                        {
                            Pais = request.viewVenda.EnderecoInstalacao.Id_Pais != null ? await repoPais.selectFirst(p => p.Id == request.viewVenda.EnderecoInstalacao.Id_Pais.Value) : null,
                            Tipo_Imovel = request.viewVenda.EnderecoInstalacao.Id_Tipo_Imovel != null ? await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == request.viewVenda.EnderecoInstalacao.Id_Tipo_Imovel.Value) : null,
                            Bairro = request.viewVenda.EnderecoInstalacao?.Bairro,

                            Id_Bairro = request.viewVenda.EnderecoInstalacao?.Id_Bairro,
                            Cep = request.viewVenda.EnderecoInstalacao?.CEP,
                            Cidade = request.viewVenda.EnderecoInstalacao?.Cidade,

                            Id_Cidade = request.viewVenda.EnderecoInstalacao?.Id_Cidade,
                            Complemento = request.viewVenda.EnderecoInstalacao?.Complemento,

                            Latitude = !String.IsNullOrEmpty(request.viewVenda.EnderecoInstalacao?.Latitude) ?
                                            Convert.ToDouble(request.viewVenda.EnderecoInstalacao?.Latitude?.Replace('.', ',')) : (Double?)null,

                            Longitude = !String.IsNullOrEmpty(request.viewVenda.EnderecoInstalacao?.Longitude) ?
                                            Convert.ToDouble(request.viewVenda.EnderecoInstalacao?.Longitude?.Replace('.', ',')) : (Double?)null,

                            Logradouro = request.viewVenda.EnderecoInstalacao?.Logradouro,

                            Numero = request.viewVenda.EnderecoInstalacao?.Numero,
                            Ponto_Referencia = request.viewVenda.EnderecoInstalacao?.Ponto_Referencia,
                            UF = request.viewVenda.EnderecoInstalacao?.UF,

                            Busca_Endereco = request.viewVenda.EnderecoInstalacao.Busca_Endereco,
                            Condominio_Predio = request.viewVenda.EnderecoInstalacao.Busca_Endereco == Busca_Endereco.Condominio_Predio ? await repoCondominio.selectFirst(p => p.Id == request.viewVenda.EnderecoInstalacao.Id_Condominio_Predio) : null,
                            Bloco = request.viewVenda.EnderecoInstalacao?.Id_Bloco,
                            Apartamento = request.viewVenda.EnderecoInstalacao?.Id_Apartamento,
                            Endereco_Logradouro = request.viewVenda.EnderecoInstalacao.Busca_Endereco == Busca_Endereco.Endereco_Logradouro ? await repoEnderecoLogradouro.selectFirst(p => p.Id == request.viewVenda.EnderecoInstalacao.Id_Endereco_Logradouro) : null,

                            Cod_Escritorio = request.viewVenda.EnderecoInstalacao?.Cod_Escritorio,
                            Cod_Area = request.viewVenda.EnderecoInstalacao?.Cod_Area,
                            Cod_Localidade =
                            !String.IsNullOrEmpty(request.viewVenda.EnderecoInstalacao?.Cod_Localidade) ?
                                Convert.ToInt32(request.viewVenda.EnderecoInstalacao?.Cod_Localidade) :
                                (int?)null,
                        };

                        retorno.Add(objEndereco);
                        break;
                    case Tipo_EnderecoCliente.Prospecto:

                        objEndereco = new Endereco_Cliente
                        {
                            Pais = request.cliente.Endereco_Prospecto?.Pais,
                            Tipo_Imovel = request.cliente.Endereco_Prospecto?.Tipo_Imovel,
                            Id_Bairro = request.cliente.Endereco_Prospecto?.Id_Bairro,
                            Bairro = request.cliente.Endereco_Prospecto?.Bairro,

                            Cep = request.cliente.Endereco_Prospecto?.Cep,
                            Id_Cidade = request.cliente.Endereco_Prospecto?.Id_Cidade,
                            Cidade = request.cliente.Endereco_Prospecto?.Cidade,
                            Complemento = request.cliente.Endereco_Prospecto?.Complemento,

                            Latitude = request.cliente.Endereco_Prospecto?.Latitude,
                            Longitude = request.cliente.Endereco_Prospecto?.Longitude,
                            Logradouro = request.cliente.Endereco_Prospecto?.Logradouro,
                            Numero = request.cliente.Endereco_Prospecto?.Numero,

                            Ponto_Referencia = request.cliente.Endereco_Prospecto?.Ponto_Referencia,
                            UF = request.cliente.Endereco_Prospecto?.UF,

                            Cod_Escritorio = request.cliente.Endereco_Prospecto?.Cod_Escritorio,
                            Cod_Area = request.cliente.Endereco_Prospecto?.Cod_Area,
                            Cod_Localidade = request.cliente.Endereco_Prospecto?.Cod_Localidade
                        };

                        retorno.Add(objEndereco);
                        break;
                }

                return retorno;
            }
        }

        public async Task<Cliente> BindDadosClienteVenda(BindDadosClienteRequestModel requestVenda, ISession session)
        {
            Cliente objCliente = null;

            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoVenda = new Conexao<Venda>(session))

            using (var repoMeioCaptacao = new Conexao<Meio_Captacao>(session))
            using (var repoCategoriaCliente = new Conexao<Categoria_Cliente>(session))

            using (var repoConfiguracoes = new Conexao<Configuracoes_Gerais>(session))
            using (var repoIdentityUser = new Conexao<IdentityUser>(session))
            {
                Configuracoes_Gerais config = await repoConfiguracoes.selectFirst();
                IdentityUser usuarioLogado = (await repoIdentityUser.selectFirst(p => p.Id == requestVenda.Usuario));
                if (requestVenda.Venda.Id_Cliente == 0)
                {
                    objCliente.UsuarioAlteracao = requestVenda.Usuario;
                }
                objCliente = (await repoCliente.select(p => p.Id == requestVenda.Venda.Id_Cliente)).FirstOrDefault();


                if (objCliente == null)
                {
                    objCliente = new Cliente
                    {
                        Id = requestVenda.Venda.Id_Cliente != 0 ? requestVenda.Venda.Id_Cliente : Convert.ToInt64(0),
                        Tipo_Cliente = (await repoVenda.select(p => p.Id_Cliente == requestVenda.Venda.Id_Cliente))
                                                   .Where(p => p.Status == requestVenda.Venda.Status).Count() > 0 ?
                                                   Tipo_Cliente.Cliente :
                                                   Tipo_Cliente.Prospecto,
                        UsuarioCriacao = requestVenda.Usuario
                    };
                }

                objCliente.Status = Status_Cliente.Ativo;

                objCliente.Nome_Resp_Financeiro = requestVenda.Venda.Representante;


                objCliente.CpfCnpj = requestVenda.Venda.CpfCnpj.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                objCliente.Nacionalidade = requestVenda.Venda.Nacionalidade;
                objCliente.Sexo = requestVenda.Venda.Sexo;
                objCliente.Data_Nascimento = !String.IsNullOrEmpty(requestVenda.Venda.DataNascimento) ?
                    DateTime.ParseExact(requestVenda.Venda.DataNascimento, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;

                objCliente.Estado_Civil = requestVenda.Venda.EstadoCivil;
                objCliente.Rg = requestVenda.Venda.RG;
                objCliente.Orgao_Emissao_Rg = requestVenda.Venda.Orgao;
                objCliente.Data_Emissao_Rg = !string.IsNullOrEmpty(requestVenda.Venda.DataEmissao) ?
                    DateTime.ParseExact(requestVenda.Venda.DataEmissao, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;

                objCliente.Grau_Instrucao = requestVenda.Venda.Instrucao;
                objCliente.Filiacao_Mae = requestVenda.Venda.Mae;
                objCliente.Filiacao_Pai = requestVenda.Venda.Pai;
                objCliente.Profissao = requestVenda.Venda.Profissao;

                objCliente.Email_Principal = requestVenda.Venda.Email;
                objCliente.Telefone_Fixo = requestVenda.Venda.TelefoneFixo;
                objCliente.Telefone_Celular = requestVenda.Venda.TelefoneCelular;
                objCliente.Telefone_Recado = requestVenda.Venda.TelefoneRecado;
                objCliente.Pessoa_Tel_Recado = requestVenda.Venda.NomeRecado;

                objCliente.Vendedor = usuarioLogado?.Funcionario;
                objCliente.Supervisor = usuarioLogado?.Funcionario?.Supervisor;
                objCliente.Categoria_Cliente = requestVenda.Venda.Id_Categoria_Cliente.HasValue ?
                    await repoCategoriaCliente.selectFirst(p => p.Id == requestVenda.Venda.Id_Categoria_Cliente.Value) : null;

                objCliente.Quem_Indicou = requestVenda.Venda.Id_Quem_Indicou.HasValue ?
                    await repoCliente.selectFirst(p => p.Id == requestVenda.Venda.Id_Quem_Indicou.Value) : null;

                objCliente.Dt_Ultimo_Contato = requestVenda.Venda.Ultimo_Contato != null ? requestVenda.Venda.Ultimo_Contato.Value : (DateTime?)null;


                objCliente.Tipo_Pessoa = requestVenda.Venda.TipoPessoa != null ? requestVenda.Venda.TipoPessoa : Tipo_Pessoa.Fisica;





                objCliente.Qual_Meio = await repoMeioCaptacao.selectFirst(p => p.Id == requestVenda.Venda.Id_Meio_Captacao);
                if (requestVenda.Venda.Id == 0 && (objCliente.Tipo_Cliente == Tipo_Cliente.Prospecto))
                {
                    objCliente.Status_Prospecto = Status_Prospecto.Em_Negociacao;
                }

                string numero_ContratoMax = Convert.ToString(repoCliente.MaxString("NUMERO_CONTRATO", "TB_CLIENTE").Result + 1);

                if (!String.IsNullOrEmpty(requestVenda.Venda.Numero_Contrato))
                {
                    objCliente.Numero_Contrato = config.Identificacao_Cliente.Value == Identificacao_Cliente.Numero_Contrato ? requestVenda.Venda.Numero_Contrato :
                                                 (requestVenda.Venda.Id == 0 || String.IsNullOrEmpty(objCliente.Numero_Contrato) ?
                                                 numero_ContratoMax : requestVenda.Venda.Numero_Contrato);
                }
                else
                {
                    objCliente.Numero_Contrato = numero_ContratoMax;
                }

                objCliente.NomeFantasia = objCliente.Tipo_Pessoa == Tipo_Pessoa.Fisica ? requestVenda.Venda.NomeCompleto : requestVenda.Venda.NomeFantasia;
                objCliente.Outro_Meio = objCliente.Qual_Meio?.Id != 0 ? Convert.ToString(objCliente.Qual_Meio?.Id) : null;

                if (objCliente.Id == 0)
                {
                    objCliente.Endereco_Principal.Id = 0;
                }

                await repoCliente.SaveOrUpdate(objCliente);
            }

            return objCliente;
        }

        public async Task<Cliente_PlanoProduto> Bind_CPP_Venda(BindCPP_VendaRequestModel request, ISession session, Venda objVenda)
        {
            Cliente_PlanoProduto clientePlanoProduto = null;
            try
            {

                using (var repoPlano = new Conexao<Plano>(session))
                using (var repoProduto = new Conexao<Produto>(session))

                using (var repoCampanha = new Conexao<Campanha>(session))
                using (var repoModelo = new Conexao<Modelo_Contrato>(session))

                using (var repoCombo = new Conexao<Combo>(session))
                using (var repoClienteContrato = new Conexao<ClienteContrato>(session))

                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoConfiguracoesGerais = new Conexao<Configuracoes_Gerais>(session))

                using (var repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoCPP = new Conexao<Cliente_PlanoProduto>(session))
                {
                    Configuracoes_Gerais config = await repoConfiguracoesGerais.selectFirst();

                    Modelo_Contrato modelo = request.Request_PlanoProduto.Id_Modelo_Contrato.HasValue ?
                                             await repoModelo.selectFirst(p => p.Id == request.Request_PlanoProduto.Id_Modelo_Contrato) :
                                             null;
                    if (modelo == null)
                    {
                        modelo = repoClienteContrato.selectFirst(p => p.Id == request.Request_PlanoProduto.Id_Modelo_Contrato)
                                             .Result.ModeloContrato;
                    }

                    var vlrPlanoProduto_FinalStr = !String.IsNullOrEmpty(request.Request_PlanoProduto.ValorPlanoProduto_Final) ?
                        string.Concat("R$", request.Request_PlanoProduto.ValorPlanoProduto_Final) : String.Empty;
                    var vlrPlanoProduto_Final = StrCurrencyToDecimal(vlrPlanoProduto_FinalStr);

                    var vlrTotalStr = !String.IsNullOrEmpty(request.Request_PlanoProduto.ValorTotal) ?
                        string.Concat("R$", request.Request_PlanoProduto.ValorTotal) : String.Empty;
                    var vlrTotal = StrCurrencyToDecimal(vlrTotalStr);

                    var vlrPlanoProdutoStr = !String.IsNullOrEmpty(request.Request_PlanoProduto.ValorPlanoProduto) ?
                        string.Concat("R$", request.Request_PlanoProduto.ValorPlanoProduto) : String.Empty;
                    var vlrPlanoProduto = StrCurrencyToDecimal(vlrPlanoProdutoStr);

                    var vlrInstalacao_FinalStr = !String.IsNullOrEmpty(request.Request_PlanoProduto.ValorInstalacao_Final) ?
                        string.Concat("R$", request.Request_PlanoProduto.ValorInstalacao_Final) : String.Empty;
                    var vlrInstalacao_Final = StrCurrencyToDecimal(vlrInstalacao_FinalStr);

                    var vlrInstalacaoStr = !String.IsNullOrEmpty(request.Request_PlanoProduto.ValorInstalacao) ?
                        string.Concat("R$", request.Request_PlanoProduto.ValorInstalacao) : String.Empty;
                    var vlrInstalacao = StrCurrencyToDecimal(vlrInstalacaoStr);

                    clientePlanoProduto = new Cliente_PlanoProduto
                    {
                        UsuarioCriacao = User.Identity.GetUserId(),
                        DataHoraCriacao = DateTime.Now,
                        Cod_Escritorio = request.Request_PlanoProduto?.Cod_Escritorio,
                        Cod_Area = request.Request_PlanoProduto?.Cod_Area,
                        Localidade = request.Request_PlanoProduto?.Localidade,

                        Cod_Localidade = request.Request_PlanoProduto?.Cod_Localidade,
                        Dias_Teste_Gratis = request.Request_PlanoProduto?.Qtde_Dias,
                        Parcelas_Instalacao = request.Request_PlanoProduto?.Numero_Parcelas,

                        Periodo_Carencia = request.Request_PlanoProduto.Periodo_Carencia,
                        Meses_Carencia = request.Request_PlanoProduto?.Meses_Carencia,

                        Campanha = request.Request_PlanoProduto.Id_Campanha.HasValue ?
                                   repoCampanha.selectFirst(p => p.Id == request.Request_PlanoProduto.Id_Campanha.Value).Result :
                                   null,

                        Plano = (!request.Request_PlanoProduto.Id_Combo.HasValue && request.Request_PlanoProduto.Tipo == PlanoProdutoEnum.Plano) ?
                                repoPlano.selectFirst(p => p.Id == request.Request_PlanoProduto.Id_Plano_Produto).Result :
                                null,

                        Produto = (!request.Request_PlanoProduto.Id_Combo.HasValue && request.Request_PlanoProduto.Tipo == PlanoProdutoEnum.Produto) ?
                                  repoProduto.selectFirst(p => p.Id == request.Request_PlanoProduto.Id_Plano_Produto).Result :
                                  null,

                        Combo = request.Request_PlanoProduto.Id_Combo.HasValue ?
                                repoCombo.selectFirst(p => p.Id == request.Request_PlanoProduto.Id_Combo).Result :
                                null,

                        Multa_Carencia_Reais = modelo?.Multa_Carencia ?? 0,
                        CancelaContratoFimCarencia = modelo?.Cancela_Contrato_Automatico_Final_Carencia ?? false,

                        Tem_Reajuste = modelo?.Tem_Reajuste ?? false,

                        Frequencia_Reajuste = modelo?.Frequencia_Reajuste ?? null,
                        Renovar_CarenciaAuto = modelo?.Renova_Carencia_Automaticamente ?? null,

                        Data_Habilitado = DateTime.Now,
                        End_Instalacao_Igual_Principal = request.Request_PlanoProduto.End_Instalacao_Igual_Principal,

                        Login_Radius = request.Request_PlanoProduto?.Login_Radius,
                        Senha_Radius = request.Request_PlanoProduto?.Senha_Radius,

                        Pool_Ip = request.Request_PlanoProduto.IdIpPool.HasValue ?
                                  repoPoolIp.selectFirst(p => p.Id == request.Request_PlanoProduto.IdIpPool).Result : null,

                        Valor_Final = vlrPlanoProduto_Final,
                        Valor_Final_Total = vlrTotal,
                        Valor_Real = vlrPlanoProduto,
                        Valor_Instalacao_Final = vlrInstalacao_Final,
                        Valor_Instalacao_Real = vlrInstalacao,

                        Cliente = request.Request_Cliente,
                        Id_Venda = objVenda?.Id,

                        Id_TrocaPlano = request.Request_PlanoProduto?.Id_PlanoMudanca,

                        /****************************************
                               Formato Telefonia
                        ****************************************/

                        JsonTelefonia = request.Request_PlanoProduto?.JsonTelefonia
                    };

                    clientePlanoProduto.Telefonia = !String.IsNullOrEmpty(clientePlanoProduto?.JsonTelefonia) ? true : false;
                    await repoCPP.Save(clientePlanoProduto);

                    if (modelo != null)
                    {
                        // Preenchimento do número do contrato
                        IClienteContratoRepository _repoClienteContrato = new ClienteContratoRepository(session);
                        IClientePlanoProdutoRepository _repoClientePlanoProduto = new ClientePlanoProdutoRepository(session);

                        if (!String.IsNullOrEmpty(request.Request_PlanoProduto.Numero_Contrato) && !request.Request_PlanoProduto.Numero_Contrato.Contains("Plano_TMP"))
                        {
                            var objCC = _repoClienteContrato.GetAll().Where(p => p.Id == Convert.ToInt64(request.Request_PlanoProduto.Numero_Contrato)).FirstOrDefault();

                            if (objCC != null)
                                clientePlanoProduto.ClienteContrato = objCC;
                        }
                        else if (!String.IsNullOrEmpty(request.Request_PlanoProduto.Numero_Contrato) && request.Request_PlanoProduto.Numero_Contrato.Contains("Plano_TMP"))
                        {
                            clientePlanoProduto.ClienteContrato = modelo != null ? ClienteContrato.FromModelo(modelo, clientePlanoProduto.Cliente) : null;
                            _repoClienteContrato.Save(clientePlanoProduto.ClienteContrato);

                            //var numeroContrato = _repoClienteContrato.GetAll().ToList().Select(x => (int?)x.Id).Max() ?? 0;
                            clientePlanoProduto.ClienteContrato.NumeroContrato = Convert.ToString(clientePlanoProduto.ClienteContrato.Id);
                            clientePlanoProduto.ClienteContrato.Status_Contrato = Status_Contrato.Ativo;

                            await repoClienteContrato.SaveOrUpdate(clientePlanoProduto.ClienteContrato);
                        }

                        await repoCPP.Save(clientePlanoProduto);
                    }
                }
            }
            catch (Exception)
            {
                //                  --          Pro Tip         --
                //          --      Use Shif + F9para debuggar o erro       --
            }

            //request.Request_PlanoProduto.Id_PlanoMudanca;

            return clientePlanoProduto;
        }

        private double StrCurrencyToDecimal(string str)
        {
            NumberFormatInfo infoCurrency = new NumberFormatInfo();
            infoCurrency.NegativeSign = "-";
            infoCurrency.CurrencyDecimalSeparator = ",";
            infoCurrency.CurrencyGroupSeparator = ".";
            infoCurrency.CurrencySymbol = "R$";

            if (double.TryParse(str, NumberStyles.Currency, infoCurrency, out var result))
                return result;

            return 0;
        }

        public async Task Mudanca_Plano((long Id_Cliente_PlanoAnterior, long NovoId) IDS, ISession session)
        {
            using (var repoCPP = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repo = new Conexao<Plano>(session))
            {
                var PlanoNovo = await repoCPP.selectFirst(p => p.Id == IDS.NovoId);

                /*---\Lógica Busca e Alteração do Status do Contrato\---*/
                using (var repoCliente_Contrato = new Conexao<ClienteContrato>(session))
                using (var repoMCP = new Conexao<Motivo_Cancelamento_PlanoProduto>(session))
                using (var PlanApi = new PlanoProdutoApiController())
                {
                    var motivoCancelamento = repoMCP.selectFirst(p => p.Motivo == "13");
                    var cliente = repoCliente_Contrato.selectFirst(p => p.Cliente == PlanoNovo.Cliente).Result.Cliente;
                    cliente.PlanosProdutos.Where(p => p.Id == IDS.Id_Cliente_PlanoAnterior).ForEach(item =>
                    {
                        PlanApi.AlterarStatus(new AlterarStatusViewModel
                        {
                            ids = new List<long> { item.Id },
                            id_motivo_cancelamento = 0,
                            status = Convert.ToInt16(Status_PlanoProduto.Cancelado_Mudanca_Plano),
                            usuario = User.Identity.GetUserName()

                        });
                    });
                }


                if (PlanoNovo.Plano.OS_Mudanca_Plano)
                {
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var repoDoc = new Conexao<Documento_ProducaoAniel>(sessionT5))
                    {
                        Documento_ProducaoAniel record = new Documento_ProducaoAniel
                        {
                            //CodCt,
                            //Projeto,
                            //Num_Doc,
                            //Num_Obra,
                            //OXBDPEE = ,
                            //Dia = DateTime.Now,
                            //Bairro = PlanoNovo.Endereco_Instalacao.Bairro,
                            //Logradouro = PlanoNovo.Endereco_Instalacao.Logradouro,
                            //Cidade = PlanoNovo.Endereco_Instalacao.Cidade,
                            //UF = PlanoNovo.Endereco_Instalacao.UF,
                            //Complemento = PlanoNovo.Endereco_Instalacao.Complemento,
                            //Cep = PlanoNovo.Endereco_Instalacao.Cep,
                            //CPF_CLIENTE = PlanoNovo.Cliente.CpfCnpj,

                        };
                    }
                }

                //PlanoAnterior.Status =;
            }
        }

        private byte[] GerarAquivoContratoCliente(ClienteContrato contrato, Modelo_Contrato modelo, Dictionary<String, Object> valores)
        {
            // Gravação do arquivo
            string caminhoArquivoModeloContrato =
                $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modelo.Id}.{modelo.Extensao_Arquivo}",

                   caminhoArquivoHTML =
                $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modelo.Id}.{modelo.Extensao_Arquivo}",

                   caminhoArquivoPDF =
                $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modelo.Id}.{modelo.Extensao_Arquivo}",

                   fileName = String.Empty;

            ConvertToPdfService.modificarDocXContrato(new ConvertToPDFServiceRequestModel
            {
                caminho = caminhoArquivoModeloContrato,
                caminhooutput = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\teste.{modelo.Extensao_Arquivo}",

                caminhohtml = caminhoArquivoHTML,
                caminhopdf = caminhoArquivoPDF,

                filename = fileName,
                lista_replace = valores.Select(p => new ReplaceDocXModel { Placeholder = p.Key, ReplaceValue = Convert.ToString(p.Value) }).ToList()
            });

            byte[] ArquivoModificado = File.ReadAllBytes(caminhoArquivoModeloContrato);
            return ArquivoModificado;
        }

        public async Task GerarDocumentoContrato(Cliente_PlanoProduto clientePlanoProduto, ISession session, Venda objVenda, Modelo_Contrato modelo, List<PostApiMensagem> mensagens)
        {
            var valores = new Dictionary<String, Object>();

            using (var repoVenda = new Conexao<Venda>(session))
            using (var ApiContrato = new Modelo_ContratoApiController())
            {
                try
                {
                    foreach (VariaveisModeloContrato item in Enum.GetValues(typeof(VariaveisModeloContrato)))
                    {

                        List<Cliente_PlanoProduto> listagemPlanos = clientePlanoProduto.Cliente.PlanosProdutos.ToList();
                        Venda venda = repoVenda.selectFirst(p => p.Id == clientePlanoProduto.Id_Venda).Result;

                        KeyValuePair<string, object> variable = await ApiContrato.SetVariable(new Modelo_ContratoGenerateVariable
                        {
                            item = item,
                            clienteContrato = clientePlanoProduto.ClienteContrato,
                            planos = listagemPlanos,
                            Venda = venda,
                            mensagens = mensagens
                        });
                        valores.Add(variable.Key, variable.Value);
                    }
                }
                catch (Exception ex)
                {
                    mensagens.Add(new PostApiMensagem
                    {
                        Mensagem = ex.InnerException != null ? $"Erro ao adicionar ao Dicionário. \r\nMessage:{ex.Message}\r\n{ex.InnerException.Message}" : $" Erro ao adicionar ao Dicionário. \r\nMessage:{ex.Message}",
                        Tipo = PostApiMensagemType.Erro,
                    });
                }

            }

            //Ao Aprovar a venda do plano/produto, gerar contrato
            //if (request.Request_PlanoProduto.Status_Venda == Status_Venda.Venda_Aprovada)
            //{
            clientePlanoProduto.ClienteContrato.Arquivo = this.GerarAquivoContratoCliente(clientePlanoProduto.ClienteContrato,
                                                                                              modelo, valores);
            //}
        }

        [HttpPost]
        [Route("api/clienteapi/GetAllEquipamentos")]
        public async Task<DataTableResponse<ClienteEquipamentos>> CliEquip(EquipamentoClienteRequest request)
        {
            List<ClienteEquipamentos> retorno = new List<ClienteEquipamentos>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT RCODMAT,RMATERIAL,RCODCPL,RSALDO ");
                sql.AppendLine(" FROM SP_CALCULA_SALDO_CLIENTE ( ");
                sql.AppendLine(" COALESCE((SELECT FIRST 1 COD_CLIENTE FROM TB_CLIENTE WHERE CNPJ = @CPF_CNPJ),0),'-','[ALL]') ");
                sql.AppendLine(" WHERE RCODCPL <> '-' ");

                if (!String.IsNullOrEmpty(request.Cpf_Cnpj))
                {
                    parametros.Add("@CPF_CNPJ", Convert.ToString(request.Cpf_Cnpj));
                }
                else
                {
                    parametros.Add("@CPF_CNPJ", Convert.ToString("0"));
                }

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new ClienteEquipamentos
                        {
                            codMat = reader["RCODMAT"].ToString(),
                            descMat = reader["RMATERIAL"].ToString(),
                            codCpl = reader["RCODCPL"].ToString(),
                            saldo = reader["RSALDO"].ToString()
                        });
                    }
                }
            }

            return new DataTableResponse<ClienteEquipamentos>
            {
                data = retorno
            };

        }

        [HttpPost]
        [Route("api/clienteapi/AdicionarPlanoProduto")]
        public async Task<PostApiRetorno> AdicionarPlanoProduto(AdicionarPlanoProdutoRequestModel request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {

                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
                using (Conexao<Forma_Pagamento_Cliente> repoFormaPagamentoCliente = new Conexao<Forma_Pagamento_Cliente>(session))
                using (Conexao<Plano> repoPlano = new Conexao<Plano>(session))
                using (Conexao<Produto> repoProduto = new Conexao<Produto>(session))
                using (Conexao<ClienteContrato> repoCliente_PlanoProduto = new Conexao<ClienteContrato>(session))
                using (var repoPais = new Conexao<Pais>(session))
                using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))
                using (var repoCondominio = new Conexao<Condominio_Predio>(session))
                using (var repoTipoImovel = new Conexao<Tipo_Imovel>(session))
                using (var repoClientePlanoProdutoStLog = new Conexao<Cliente_PlanoProduto_StatusLog>(session))
                {
                    if (!request.Numero_Contrato.Contains("Plano_TMP"))
                    {
                        var lista_cliente_contrato = await repoCliente_PlanoProduto.select(x => x.Id == Convert.ToInt64(request.Numero_Contrato) && x.Status_Contrato == Status_Contrato.Aguardando_Ativacao);
                        if (lista_cliente_contrato.Count > 0)
                        {
                            retorno.Mensagem = "Atenção! Não foi possível vincular o plano/produto. Contrato selecionado aguardando ativação/aprovação em uma venda.";
                            retorno.Ok = false;

                            return retorno;
                        }
                    }

                    Cliente objCliente = await repoCliente.selectFirst(p => p.Id == request.IdCliente);
                    Plano objPlano = await repoPlano.selectFirst(p => p.Id == request.IdPlanoProduto);
                    Produto objProduto = await repoProduto.selectFirst(p => p.Id == request.IdPlanoProduto);



                    if (objCliente == null)
                    {
                        throw new Exception("Por favor salvar o cadastro do cliente antes de prosseguir!");//PlaceHolder DAIANE
                    }
                    //Modelo de Contrato será preenchido dentro do Bind_CPP_Venda
                    Modelo_Contrato modeloContrato = null;
                    var view = objPlano != null ? GetPlanoListModel(objPlano) : GetProdutoListModel(objProduto);

                    if (String.IsNullOrEmpty(request.ValorFinalPersonalizado) == false)
                    {
                        view.ValorPlanoProduto_Final = request.ValorFinalPersonalizado;
                        view.ValorTotal = request.ValorFinalPersonalizado;
                    }

                    view.End_Instalacao_Igual_Principal = request.End_Instalacao_Igual_Principal;
                    view.Id_Modelo_Contrato = request.IdModeloContrato;
                    view.Numero_Contrato = request.Numero_Contrato;

                    Cliente_PlanoProduto planoProduto = await Bind_CPP_Venda(new BindCPP_VendaRequestModel
                    {
                        Request_PlanoProduto = view,
                        Request_Modelo = modeloContrato,

                        Request_Cliente = objCliente,
                        Request_Mensagens = new List<PostApiMensagem>()
                    },
                        session, null);

                    planoProduto.Status = view.Status;
                    planoProduto.End_Instalacao_Igual_Principal = request.End_Instalacao_Igual_Principal;

                    #region Forma de Pagamento de Cliente

                    //*****************************
                    //Forma de Pagamento de Cliente
                    //*****************************

                    Forma_Pagamento_Cliente formaPagamentoCliente =
                        objCliente.Formas_Pagamento_Cliente.FirstOrDefault(f => f.Forma_Pagamento.Id == request.IdFormaPagamentoCliente);

                    if (formaPagamentoCliente == null)
                    {
                        throw new Exception("Forma de Pagamento não encontrada.");
                    }

                    // ************************************************
                    // Forma de Pagamento do Cliente Vinculada ao Plano
                    // ************************************************

                    if (formaPagamentoCliente.Id != 0)
                    {
                        planoProduto.Forma_Pagamento_Cliente = repoFormaPagamentoCliente.selectFirst(f => f.Id == formaPagamentoCliente.Id).Result;
                    }
                    else
                    {
                        planoProduto.Forma_Pagamento_Cliente = formaPagamentoCliente;
                    }
                    #endregion

                    planoProduto.Cod_Escritorio = request.EnderecoInstalacao.Cod_Escritorio;
                    planoProduto.Cod_Area = request.EnderecoInstalacao.Cod_Area;
                    planoProduto.Cod_Localidade = !string.IsNullOrEmpty(request.EnderecoInstalacao.Cod_Localidade) ? Convert.ToInt32(request.EnderecoInstalacao.Cod_Localidade) : (int?)null;
                    planoProduto.Localidade = request.EnderecoInstalacao.Localidade;

                    #region Endereco_Instalacao

                    // ********************************
                    // Gestão de Endereco de Instalação
                    //*********************************

                    if (planoProduto.End_Instalacao_Igual_Principal)
                    {
                        planoProduto.Endereco_Instalacao = new Endereco_Cliente
                        {
                            UsuarioCriacao = User.Identity.GetUserId(),
                            DataHoraCriacao = DateTime.Now,
                            Pais = objCliente.Endereco_Principal.Pais,
                            Tipo_Imovel = objCliente.Endereco_Principal.Tipo_Imovel,
                            Bairro = objCliente.Endereco_Principal.Bairro,
                            Id_Bairro = objCliente.Endereco_Principal.Id_Bairro,
                            Cep = objCliente.Endereco_Principal?.Cep,
                            Cidade = objCliente.Endereco_Principal?.Cidade,
                            Id_Cidade = objCliente.Endereco_Principal?.Id_Cidade,
                            Complemento = objCliente.Endereco_Principal?.Complemento,
                            Latitude = objCliente.Endereco_Principal?.Latitude,
                            Longitude = objCliente.Endereco_Principal?.Longitude,
                            Logradouro = objCliente.Endereco_Principal?.Logradouro,
                            Numero = objCliente.Endereco_Principal?.Numero,
                            Ponto_Referencia = objCliente.Endereco_Principal?.Ponto_Referencia,
                            UF = objCliente.Endereco_Principal?.UF,
                            Busca_Endereco = objCliente.Endereco_Principal?.Busca_Endereco,
                            Condominio_Predio = objCliente.Endereco_Principal?.Condominio_Predio,
                            Endereco_Logradouro = objCliente.Endereco_Principal?.Endereco_Logradouro,
                            Cod_Escritorio = objCliente.Endereco_Principal?.Cod_Escritorio,
                            Cod_Area = objCliente.Endereco_Principal?.Cod_Area,
                            Cod_Localidade = objCliente.Endereco_Principal?.Cod_Localidade,
                            Localidade = objCliente.Endereco_Principal?.Localidade
                        };

                    }
                    else
                    {
                        planoProduto.Endereco_Instalacao = new Endereco_Cliente
                        {
                            UsuarioCriacao = User.Identity.GetUserId(),
                            DataHoraCriacao = DateTime.Now,
                            Pais = request.EnderecoInstalacao.Id_Pais != null ? await repoPais.selectFirst(p => p.Id == request.EnderecoInstalacao.Id_Pais.Value) : null,
                            Tipo_Imovel = request.EnderecoInstalacao.Id_Tipo_Imovel != null ? await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == request.EnderecoInstalacao.Id_Tipo_Imovel.Value) : null,
                            Bairro = request.EnderecoInstalacao?.Bairro,

                            Id_Bairro = request.EnderecoInstalacao?.Id_Bairro,
                            Cep = request.EnderecoInstalacao?.CEP,
                            Cidade = request.EnderecoInstalacao?.Cidade,

                            Id_Cidade = request.EnderecoInstalacao?.Id_Cidade,
                            Complemento = request.EnderecoInstalacao?.Complemento,

                            Latitude = !String.IsNullOrEmpty(request.EnderecoInstalacao?.Latitude) ?
                            Convert.ToDouble(request.EnderecoInstalacao?.Latitude?.Replace('.', ',')) : (Double?)null,

                            Longitude = !String.IsNullOrEmpty(request.EnderecoInstalacao?.Longitude) ?
                            Convert.ToDouble(request.EnderecoInstalacao?.Longitude?.Replace('.', ',')) : (Double?)null,

                            Logradouro = request.EnderecoInstalacao?.Logradouro,

                            Numero = request.EnderecoInstalacao?.Numero,
                            Ponto_Referencia = request.EnderecoInstalacao?.Ponto_Referencia,
                            UF = request.EnderecoInstalacao?.UF,

                            Busca_Endereco = request.EnderecoInstalacao.Busca_Endereco != null ? request.EnderecoInstalacao.Busca_Endereco : null,
                            Condominio_Predio = request.EnderecoInstalacao.Busca_Endereco == Busca_Endereco.Condominio_Predio ? await repoCondominio.selectFirst(p => p.Id == request.EnderecoInstalacao.Id_Condominio_Predio) : null,
                            Endereco_Logradouro = request.EnderecoInstalacao.Busca_Endereco == Busca_Endereco.Endereco_Logradouro ? await repoEnderecoLogradouro.selectFirst(p => p.Id == request.EnderecoInstalacao.Id_Endereco_Logradouro) : null,
                            Cod_Escritorio = request.EnderecoInstalacao.Cod_Escritorio,
                            Cod_Area = request.EnderecoInstalacao.Cod_Area,
                            Cod_Localidade = !string.IsNullOrEmpty(request.EnderecoInstalacao.Cod_Localidade) ? Convert.ToInt32(request.EnderecoInstalacao.Cod_Localidade) : (int?)null,
                            Localidade = request.EnderecoInstalacao.Localidade

                        };


                    }
                    #endregion

                    objCliente.PlanosProdutos.Add(planoProduto);
                    await repoCliente.SaveOrUpdate(objCliente);

                    Status_PlanoProduto? statusAntigo = (await repoClientePlanoProdutoStLog.select(p => p.PlanoProduto.Id == planoProduto.Id))
                                                                     .OrderByDescending(p => p.DataHoraAcao).FirstOrDefault()?.NovoStatus;
                    var objStLog = new Cliente_PlanoProduto_StatusLog
                    {
                        AntigoStatus = statusAntigo,
                        NovoStatus = planoProduto.Status,
                        PlanoProduto = planoProduto,
                        ClienteContrato = planoProduto.ClienteContrato,
                        DataHoraCriacao = DateTime.Now,
                        DataHoraAcao = DateTime.Now,
                        UsuarioCriacao = User.Identity.GetUserId()
                    };
                    await repoClientePlanoProdutoStLog.SaveOrUpdate(objStLog);

                    #region Login Radius
                    if (planoProduto.Plano.AutenticacaoRadius == true)
                    {
                        using (Conexao<Model.nhibernate.LoginRadius> repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
                        using (Conexao<Model.nhibernate.Pool_Ip_Endereco> repoPoolIpEndereco = new Conexao<Model.nhibernate.Pool_Ip_Endereco>(session))
                        using (Conexao<Concentradora> repoConcentradora = new Conexao<Concentradora>(session))
                        using (Conexao<Model.nhibernate.Pool_Ip> repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(session))
                        using (var ApiConcentradora = new ConcentradoraApiController())
                        using (var ApiPool_Ip = new Pool_IpApiController())
                        using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                        {
                            Model.nhibernate.Pool_Ip_Endereco Endereco_IP = null;
                            Model.nhibernate.Pool_Ip pool_Ip = null;
                            var login = new Model.nhibernate.LoginRadius();
                            var cliente = objCliente;
                            var cliente_plano = planoProduto;
                            Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());

                            long? id_Concentradora = await ApiConcentradora.GetConcentradoraByUsuarios(cliente_plano.Cod_Escritorio,
                                                                                                                          cliente_plano.Cod_Area,
                                                                                                                          cliente_plano.Cod_Localidade.GetValueOrDefault(0));

                            login.Concentradora = await repoConcentradora.selectFirst(p => p.Id == id_Concentradora);




                            string primeiroNome = String.Empty;
                            string ultimoNome = String.Empty;

                            int idxPrimeiroEspaco = cliente.NomeFantasia.IndexOf(" ");
                            int idxUltimoEspaco = cliente.NomeFantasia.LastIndexOf(" ");

                            // Se o nome tem espaço preenche o primeiro e o último nome
                            if (idxPrimeiroEspaco != -1)
                            {
                                primeiroNome = cliente.NomeFantasia.Substring(0, idxPrimeiroEspaco);
                                ultimoNome = cliente.NomeFantasia.Substring(idxUltimoEspaco + 1, cliente.NomeFantasia.Length - idxUltimoEspaco - 1);
                            }
                            // Caso o nome não tenha nenhum espaço preenche somente o primeiro nome
                            else
                            {
                                primeiroNome = cliente.NomeFantasia;
                            }

                            login.Login = (config.Padrao_Login.Replace("PrimeiroNome", primeiroNome.ToLowerInvariant())
                                                     .Replace("UltimoNome", ultimoNome.ToLowerInvariant())
                                                     .Replace("Contrato", cliente?.Numero_Contrato)
                                                     .Replace("Cont.PlanoseProdutos", cliente_plano?.ClienteContrato?.Id.ToString())
                                                     .Replace("Código", Convert.ToString(cliente?.Id))
                                                     .Replace("Email", cliente?.Email_Principal.ToLowerInvariant())
                                                     .Replace("CPF/CNPJ", cliente?.CpfCnpj)
                                                     .Replace("Uf", cliente.Endereco_Principal?.UF.ToLowerInvariant())
                                                     .Replace("Cidade", cliente.Endereco_Principal?.Cidade.ToLowerInvariant()));

                            var verificaLoginComSequencial = await new LoginRadiusApiController().GerarLoginComSequencial(login.Login);

                            login.Login = verificaLoginComSequencial;

                            login.SenhaPPPOE = (config.Padrao_Login_Ultimo.Replace("PrimeiroNome", primeiroNome.ToLowerInvariant())
                                                         .Replace("UltimoNome", ultimoNome.ToLowerInvariant())
                                                         .Replace("Contrato", cliente?.Numero_Contrato)
                                                         .Replace("Cont.PlanoseProdutos", cliente_plano?.ClienteContrato?.Id.ToString())
                                                         .Replace("Código", Convert.ToString(cliente?.Id))
                                                         .Replace("Email", cliente?.Email_Principal.ToLowerInvariant())
                                                         .Replace("CPF/CNPJ", cliente?.CpfCnpj)
                                                         .Replace("Uf", cliente.Endereco_Principal?.UF.ToLowerInvariant())
                                                         .Replace("Cidade", cliente.Endereco_Principal?.Cidade.ToLowerInvariant()));

                            var verificaSenhaComSequencial = await new LoginRadiusApiController().GerarSenhaComSequencial(login.SenhaPPPOE);

                            login.SenhaPPPOE = verificaSenhaComSequencial;

                            long? id_Pool_Ip = await ApiPool_Ip.GetPoolIpPadraoConcentradora(id_Concentradora) ?? await ApiPool_Ip.GetPoolIpMenosUsuarios(id_Concentradora);
                            if (id_Pool_Ip != null)
                            {
                                pool_Ip = await repoPoolIp.selectFirst(p => p.Id == id_Pool_Ip);
                                Endereco_IP = pool_Ip.Pool_Ip_Enderecos.FirstOrDefault(p => p.Status_Ip == Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel);
                                login.Cliente_PlanoProduto = cliente_plano;
                                login.Cliente_PlanoProduto.Pool_Ip = pool_Ip;
                                login.Cliente_PlanoProduto.Pool_Ip.Id = pool_Ip.Id;
                                if (pool_Ip.IsPoolIpFixo == true)
                                {
                                    if (pool_Ip?.Padrao_Ip == Tipo_Ip.IPv4)
                                        login.EnderecoIPV4 = Endereco_IP?.Ip;
                                    else
                                        login.EnderecoIPV6 = Endereco_IP?.Ip;

                                    login.Pool_Ip_Endereco = Endereco_IP;
                                }


                            }



                            login.Cliente = cliente;
                            login.Cliente_PlanoProduto = cliente_plano;

                            if (!login.Tipo_Autenticacao_Login.HasValue)
                                login.Tipo_Autenticacao_Login = Tipo_Autenticacao_Login.PPPOE;



                            if (login != null)
                            {
                                if (login.Concentradora != null)
                                {

                                    LoginRadiusCadastroModel loginCadastro = new LoginRadiusApiController().GetViewObj(login, null, ignorarCriticaCaixaPorta: true);

                                    ParamWebApi<LoginRadiusCadastroModel> paramWebApi = new ParamWebApi<LoginRadiusCadastroModel>
                                    {
                                        Objeto = loginCadastro,
                                        Usuario = User.Identity.GetUserName()

                                    };

                                    var cadastrarLoginRadius = await new LoginRadiusApiController().Gravar(paramWebApi);
                                }
                                else
                                {
                                    await repoLoginRadius.SaveOrUpdate(login);
                                }

                            }
                        }



                    }

                    #endregion
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
            }

            return retorno;

        }

        [OlaAuthorize]
        [AllowAnonymous]
        [HttpGet]
        [Route("api/clienteapi/AtualizaClienteOla")]
        public async Task<PostApiRetorno> AtualizaClienteOla(long idCliente)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Cliente>(session))
                {
                    IClienteRepository clienteRepository = new ClienteRepository(session);
                    IConfiguracoesGeraisRepository configuracoesRepository = new ConfiguracoesGeraisRepository(session);
                    ConfiguracoesGeraisBusiness confBusiness = new ConfiguracoesGeraisBusiness(configuracoesRepository);
                    ClienteBusiness clienteBusiness = new ClienteBusiness(clienteRepository);
                    Cliente cliente = await repo.selectFirst(p => p.Id == idCliente);

                    if (cliente == null)
                        throw new EAnielException("Cliente não encontrado com o Id informado");

                    var configGerais = confBusiness.GetConfiguracoesGerais();

                    clienteBusiness.AtualizarClientesOla(confBusiness.GetUrlOla(), CONSTANTES.SenhaOla, configGerais?.CNPJ, configGerais?.Uuid_Provedor_Ola, cliente);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
                retorno.Ok = false;
            }


            return retorno;

        }
        /// <summary>
        /// Consultar se no registro daquele cliente temos número validado.
        /// <code>Select Count(Numero_Validado)</code>
        /// </summary>
        /// <param name="cliente"> Id do Cliente solicitado para a requisição</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/clienteapi/FindNumeroValidadoByCliente")]
        public async Task<GetApiRetorno> FindNumeroValidadoByCliente(long cliente)
        {
            var retorno = new GetApiRetorno();
            //usings
            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder($"SELECT COUNT(A.NUMERO_VALIDADO) AS NUMEROS_VALIDADOS FROM TB_CLIENTE A WHERE A.ID = {cliente}");
                using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno.Ok = Convert.ToInt32(reader["NUMEROS_VALIDADOS"]) > 0;
                    }
                }
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/clienteapi/ValidaNumero")]
        public async Task<GetApiRetorno> ValidaNumero(ValidaNumeroRequest request)
        {
            var retorno = new GetApiRetorno();
            try
            {
                //usings
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCliente = new Conexao<Cliente>(session))
                {
                    Cliente record = (await repoCliente.selectFirst(p => p.Id == request.Cliente));
                    record.Numero_Validado = request.NumeroCelular;

                    await repoCliente.SaveOrUpdate(record);
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = $"{ex.Message} - {ex?.InnerException?.Message}";
            }
            return retorno;
        }

        private PlanoProdutoListModel GetPlanoListModel(Plano plano)
        {
            return new PlanoProdutoListModel
            {
                VelocidadeDownload = plano.VelocidadeDownload,
                VelocidadeNominal = plano.VelocidadeNominal,
                VelocidadeUpload = plano.VelocidadeUpload,
                VelocidadeDownloadReduzida = plano.VelocidadeDownloadReduzida,
                VelocidadeUploadReduzida = plano.VelocidadeUploadReduzida,
                Velocidade_DownloadReduzida = plano.VelocidadeDownloadReduzida,
                ReducaoVelocidade = plano.ReducaoVelocidade,
                Velocidade_Reduzida = plano.ReducaoVelocidade,
                Periodo_Carencia = plano.Periodo_Carencia,
                Id_Plano_Produto = plano?.Id,
                Tipo = PlanoProdutoEnum.Plano,
                Status = plano?.StatusInicial
            };
        }

        private PlanoProdutoListModel GetProdutoListModel(Produto produto)
        {
            return new PlanoProdutoListModel
            {
                Id_Plano_Produto = produto?.Id,
                Tipo = PlanoProdutoEnum.Produto,
                Status = produto?.Status_Inicial
            };
        }

        private async Task<ArquivoContrato> GerarContratoCliente(Cliente cliente, IList<Cliente_PlanoProduto> clientePlanoProduto, ClienteContrato cliContrato, Int64 modelo)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var modeloRepo = new Conexao<TermoAdesao>(session))
            using (var ApiContrato = new Modelo_ContratoApiController())
            using (var clienteProdutoRepo = new Conexao<Cliente_PlanoProduto>(session))
            {

                var valores = new Dictionary<string, object>();
                var nomeArquivo = $"Contrato-{cliContrato.NumeroContrato}";
                string caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modelo}.docx";
                string caminhoHtml = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{cliente.Id}\{nomeArquivo}.html";
                string caminhoPDF = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{cliente.Id}\{nomeArquivo}.pdf";

                if (!Directory.Exists($@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{cliente.Id}"))
                {
                    Directory.CreateDirectory($@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{cliente.Id}");
                }

                foreach (VariaveisModeloContrato item in Enum.GetValues(typeof(VariaveisModeloContrato)))
                {

                    IList<Cliente_PlanoProduto> listagemPlanos = clientePlanoProduto;

                    KeyValuePair<string, object> variable = await ApiContrato.SetVariable(new Modelo_ContratoGenerateVariable
                    {
                        item = item,
                        clienteContrato = cliContrato,
                        planos = listagemPlanos,
                        Venda = null,
                        mensagens = new List<PostApiMensagem>()
                    });

                    if (!String.IsNullOrEmpty(variable.Key) && variable.Value != null)
                        valores.Add(variable.Key, variable.Value);
                }

                valores.Remove("");

                ConvertToPdfService.converterDocXContrato(new ConvertToPDFServiceRequestModel
                {
                    caminho = caminhoArquivo,
                    caminhooutput = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{cliente.Id}\{nomeArquivo}.docx",
                    caminhohtml = caminhoHtml,
                    caminhopdf = caminhoPDF,
                    LogoVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_LOGO),
                    filename = Path.Combine(CONSTANTES.CaminhoFisico, @"Arquivos\LogoCliente.jpg"),//logo do cliente
                    lista_replace = valores.Select(p => new ReplaceDocXModel { Placeholder = p.Key, ReplaceValue = Convert.ToString(p.Value) }).ToList(),
                    FooterVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_FOOTER),
                    FilenameFooter = Path.Combine(CONSTANTES.CaminhoFisico, @"Arquivos\Footer.jpg"),
                    zoomFactorPdf = "1.3"
                });

                ArquivoContrato arquivoContrato = new ArquivoContrato
                {
                    CaminhoPDF = caminhoPDF,
                    ExtensaoArquivo = "pdf",
                    NomeArquivo = nomeArquivo,
                    TituloArquivo = nomeArquivo
                };

                return arquivoContrato;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/clienteapi/GetEnderecoByIdCliente")]
        public async Task<GetApiRetorno<Endereco_Cliente>> GetEnderecoCobranca(int id_cliente)
        {
            GetApiRetorno<Endereco_Cliente> retorno = new GetApiRetorno<Endereco_Cliente>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT ENDERECO_CEP, ENDERECO_LOGRADOURO, ENDERECO_NUMERO, ENDERECO_BAIRRO,");
                        sql.AppendLine("ENDERECO_CIDADE, ENDERECO_UF FROM TB_ENDERECO_CLIENTE");
                        sql.AppendLine($"WHERE ID_CLIENTE = {id_cliente}");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {


                            if (await reader.ReadAsync())
                            {
                                Endereco_Cliente enderecoCliente = new Endereco_Cliente
                                {
                                    Cep = Convert.ToString(reader["ENDERECO_CEP"]),
                                    Logradouro = Convert.ToString(reader["ENDERECO_LOGRADOURO"]),
                                    Numero = Convert.ToString(reader["ENDERECO_NUMERO"]),
                                    Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                    Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                    UF = Convert.ToString(reader["ENDERECO_UF"]),
                                };

                                retorno.Objeto = enderecoCliente;
                                retorno.Ok = true;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                }
            }



            return retorno;
        }


        [HttpPost]
        [Route("api/clienteapi/CheckEnderecoVinculado")]
        public async Task<PostApiRetorno<List<ClientesAtreladosEndereco>>> HasCliEndVinculado(EnderecoCliCheck modal)
        {
            PostApiRetorno<List<ClientesAtreladosEndereco>> response = new PostApiRetorno<List<ClientesAtreladosEndereco>>();
            List<ClientesAtreladosEndereco> clientesAtrelados = new List<ClientesAtreladosEndereco>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var confGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhereSCadastro = new StringBuilder();
                    StringBuilder sqlWhereCadastro = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    var config = await confGerais.selectFirst();

                    sql.AppendLine("SELECT CPR.ID_CLIENTE, CLI.NOMEFANTASIA  ");
                    sql.AppendLine("FROM TB_CLIENTE_PLANO_PRODUTO CPR ");
                    sql.AppendLine("LEFT JOIN TB_CLIENTE CLI ");
                    sql.AppendLine("ON CLI.ID = CPR.ID_CLIENTE  ");
                    sql.AppendLine("LEFT JOIN TB_ENDERECO_CLIENTE ENDERECO ");
                    sql.AppendLine("ON ENDERECO.ID = CPR.ID_ENDERECO_INSTALACAO ");

                    //caso utilize cadastro endereco
                    if (!config.Utiliza_Cadastro_Endereco)
                    {
                        if (modal.Id_Condominio_Predio.HasValue)
                        {
                            var obterSqlCompleto = QueryUtilizaEnderecoCondominio(sql, modal);

                            sql = obterSqlCompleto.Item1; //sql
                            sqlWhereSCadastro = obterSqlCompleto.Item2; //sqlWhere

                            parametros.Add("@CONDPRD", modal.Id_Condominio_Predio.Value);
                            parametros.Add("@NUM", modal.Numero);
                            parametros.Add("@BLOCO", modal.Bloco);
                            parametros.Add("@APARTAMENTO", modal.Apartamento);

                        }
                        else
                        {
                            sqlWhereSCadastro.AppendLine("WHERE ENDERECO.ENDERECO_CEP IS NOT NULL ");
                            sqlWhereSCadastro.AppendLine("AND ENDERECO.ENDERECO_CEP = @CEP ");
                            sqlWhereSCadastro.AppendLine("AND ENDERECO.ENDERECO_NUMERO = @NUM ");
                            sqlWhereSCadastro.AppendLine("AND ENDERECO.ENDERECO_LOGRADOURO = @LOGRADOURO ");
                            sqlWhereSCadastro.AppendLine("AND CLI.STATUS = '1' ");

                            parametros.Add("@CEP", modal.CEP);
                            parametros.Add("@NUM", modal.Numero);
                            parametros.Add("@LOGRADOURO", modal.Logradouro);
                        }
                    }
                    else
                    {
                        if (modal.Id_Condominio_Predio.HasValue)
                        {
                            var obterSqlCompleto = QueryUtilizaEnderecoCondominio(sql, modal);

                            sql = obterSqlCompleto.Item1; //sql
                            sqlWhereCadastro = obterSqlCompleto.Item2; //sqlWhere

                            parametros.Add("@CONDPRD", modal.Id_Condominio_Predio.Value);
                            parametros.Add("@NUM", modal.Numero);
                            parametros.Add("@BLOCO", modal.Bloco);
                            parametros.Add("@APARTAMENTO", modal.Apartamento);

                        }
                        else
                        {
                            sql.AppendLine("LEFT JOIN TB_ENDERECO_LOGRADOURO ENDLOG ");
                            sql.AppendLine("ON ENDLOG.ID = ENDERECO.ID_ENDERECO_LOGRADOURO ");

                            sqlWhereCadastro.AppendLine("WHERE ENDERECO.ID_ENDERECO_LOGRADOURO = @ENDLOGRD ");
                            sqlWhereCadastro.AppendLine("AND ENDERECO.ENDERECO_NUMERO = @NUM ");
                            sqlWhereCadastro.AppendLine("AND CLI.STATUS = '1' ");

                            parametros.Add("@ENDLOGRD", modal.Id_Endereco_Logradouro.Value);
                            parametros.Add("@NUM", modal.Numero);
                        }
                    }


                    using (var com = session.Connection.CreateCommand())
                    {

                        if (!config.Utiliza_Cadastro_Endereco)
                            sql.AppendLine(sqlWhereSCadastro.ToString());
                        else
                            sql.AppendLine(sqlWhereCadastro.ToString());

                        sql.AppendLine("GROUP BY CPR.ID_CLIENTE, CLI.NOMEFANTASIA ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                string nomeCli = Convert.ToString(reader["NOMEFANTASIA"]);
                                long? idCli = reader["NOMEFANTASIA"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : (Int64?)null;

                                ClientesAtreladosEndereco cliAtrelado = new ClientesAtreladosEndereco
                                {
                                    IdentificacaoCli = idCli + " - " + nomeCli
                                };

                                clientesAtrelados.Add(cliAtrelado);
                            }
                        }
                    }

                    response.Ok = true;
                    response.IdObjeto = clientesAtrelados;
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
            }

            return response;
        }


        public (StringBuilder, StringBuilder) QueryUtilizaEnderecoCondominio(StringBuilder sql, EnderecoCliCheck modal)
        {
            StringBuilder sqlWhere = new StringBuilder();

            if (modal.Id_Condominio_Predio.HasValue)
            {

                sql.AppendLine("LEFT JOIN TB_CONDOMINIO_PREDIO CONDOMINIO ");
                sql.AppendLine("ON CONDOMINIO.ID = ENDERECO.ID_CONDOMINIO_PREDIO ");

                if (!string.IsNullOrEmpty(modal.Bloco))
                {
                    sql.AppendLine("LEFT JOIN TB_COND_PR_BLOCO BLOCO ");
                    sql.AppendLine("ON BLOCO.ID_CONDOMINIO = CONDOMINIO.ID ");
                }

                if (!string.IsNullOrEmpty(modal.Apartamento))
                {
                    sql.AppendLine("LEFT JOIN TB_COND_PR_APARTAMENTO APART ");
                    sql.AppendLine("ON APART.ID_BLOCO = BLOCO.ID ");
                }

                sqlWhere.AppendLine("WHERE ENDERECO.ID_CONDOMINIO_PREDIO = @CONDPRD ");
                sqlWhere.AppendLine("AND ENDERECO.ENDERECO_NUMERO = @NUM ");

                if (sql.ToString().Contains("BLOCO"))
                    sqlWhere.AppendLine("AND BLOCO.ID = @BLOCO ");

                if (sql.ToString().Contains("APART"))
                    sqlWhere.AppendLine("AND APART.ID = @APARTAMENTO ");

                sqlWhere.AppendLine("AND CLI.STATUS = '1' ");
            }

            return (sql, sqlWhere);
        }

        [HttpPost()]
        [Route("api/clienteapi/VerificaStatusCartao")]
        public async Task<PostApiRetorno<ResultValidacao>> VerificaStatusCartao(CheckStatusCartao model)
        {
            PostApiRetorno<ResultValidacao> response = new PostApiRetorno<ResultValidacao>();

            using (var session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoCliPlan = new Conexao<Cliente_PlanoProduto>(session))
            {
                var cliente = await repoCliente.selectFirst(x => x.Id == model.IdCliente);

                if (cliente is null)
                {
                    response.Ok = false;
                    response.Mensagem = "Cliente não encontrado";
                    return response;
                }

                var formaPagamento = cliente.Formas_Pagamento_Cliente.FirstOrDefault(x => x.Forma_Pagamento.Tipo_Forma_Pagamento == Tipo_Forma_Pagamento.Cartao_Credito);

                if (formaPagamento is null)
                {
                    response.Ok = false;
                    response.Mensagem = "Cliente não possui Forma de Pagamento na modalidade Cartão";
                    return response;
                }


                var hasCliPlan = await repoCliPlan.selectFirst(x => x.Cliente.Id == cliente.Id &&
                                                               x.Forma_Pagamento_Cliente == formaPagamento) != null;

                if (formaPagamento.StatusValidoCartao.HasValue)
                {

                    ResultValidacao validacao = new ResultValidacao();

                    if (formaPagamento.StatusValidoCartao == StatusValidacaoCartao.Aguardando_Validacao)
                    {
                        validacao.IdStatus = 0;
                        validacao.Mensagem = "Aguardando Validação do Cartão.";
                        validacao.TemPlanProdAtrelado = hasCliPlan;
                    }
                    else if (formaPagamento.StatusValidoCartao == StatusValidacaoCartao.Cartao_Valido)
                    {
                        validacao.IdStatus = 1;
                        validacao.Mensagem = "Cartão válido para realizar a Venda.";
                        validacao.TemPlanProdAtrelado = hasCliPlan;
                    }
                    else
                    {

                        validacao.IdStatus = 2;
                        validacao.Mensagem = "Cartão inválido, valide as informações e tente novamente";
                        validacao.TemPlanProdAtrelado = hasCliPlan;
                    }

                    response.Ok = true;
                    response.IdObjeto = validacao;
                }
                else
                {
                    ResultValidacao validacao = new ResultValidacao
                    {
                        IdStatus = 0,
                        Mensagem = "Aguardando Validação do Cartão.",
                        TemPlanProdAtrelado = hasCliPlan
                    };

                    response.Ok = true;
                    response.IdObjeto = validacao;
                }
            }
            return response;
        }



        [HttpPost()]
        [Route("api/clienteapi/ConsultarCPFCNPJ")]
        public async Task<PostApiRetorno<ClienteValidacaoRetorno>> ValidarCPFeCNJP(string cnpjCPF)
        {
            var response = new PostApiRetorno<ClienteValidacaoRetorno>()
            {
                IdObjeto = null,
                Ok = false,
                Mensagem = ""
            };

            if (string.IsNullOrEmpty(cnpjCPF))
            {
                response.Mensagem = "CPF/CNPJ informado está nulo ou vazio.";
                return response;
            }

            string documentoConvertido = cnpjCPF.Replace(".", "").Replace("-", "");

            using (var session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            {
                var cliente = await repoCliente.selectFirst(x => x.CpfCnpj == documentoConvertido || x.CpfCnpj == cnpjCPF);

                if (cliente == null)
                {
                    response.Mensagem = "Cliente não encontrado com o CPF/CNPJ fornecido.";
                    return response;
                }

                var clienteModel = await BindClienteValidao(cliente);

                if (clienteModel != null)
                {
                    response.Mensagem = "Cliente encontrado com sucesso!";
                    response.IdObjeto = clienteModel;
                    response.Ok = true;
                }
            }

            return response;
        }

        private async Task<ClienteValidacaoRetorno> BindClienteValidao(Cliente cliente)
        {
            var response = new ClienteValidacaoRetorno
            {
                IdCliente = cliente.Id,
                NomeFantasia = cliente?.NomeFantasia,
                TelefoneFixo1 = cliente?.Telefone_Fixo,
                TelefoneCelular = cliente?.Telefone_Celular,
                TelefoneRecado = cliente?.Telefone_Recado,
                Email = cliente?.Email_Principal,
                NomeRecado = cliente?.Pessoa_Tel_Recado,
                CpfCnpj = cliente?.CpfCnpj,
                RazaoSocial = cliente?.Razao_Social,
                DataAberturaEmpresa = cliente?.Data_Abertura_Empresa.ToString(),
                InscricaoEstadual = cliente?.Inscricao_Estadual,
                InscricaoMunicipal = cliente?.Inscricao_Municipal,
                Porte = (long?)cliente?.Porte,
                RamoAtividade = cliente?.Ramo_Atividade,
                CpfResponsavel = cliente?.Cpf_Responsavel,
                CargoResponsavel = cliente?.Cargo_Responsavel,
                DataNascimento = cliente?.Data_Nascimento.ToString(),
                Nacionalidade = cliente?.Nacionalidade,
                Sexo = (long?)cliente?.Sexo,
                EstadoCivil = (long?)cliente?.Estado_Civil,
                DataEmissaoRg = cliente?.Data_Emissao_Rg.ToString(),
                NumeroRg = cliente?.Rg,
                OrgaoRg = cliente?.Orgao_Emissao_Rg,
                Profissao = cliente?.Profissao,
                FiliacaoPai = cliente?.Filiacao_Pai,
                FiliacaoMae = cliente?.Filiacao_Mae,
                GrauInstrucao = (long?)cliente?.Grau_Instrucao,
                Categoria = cliente?.Categoria_Cliente?.Descricao,
                IdCategoriaCli = cliente?.Categoria_Cliente?.Id,
                IdentificacaoCliente = cliente.Numero_Contrato
            };

            return response;
        }

        [HttpPost]
        [Route("api/clienteapi/VerificarContato")]
        public async Task<GetApiRetorno<bool>> VerificarContato(ParamWebApi<ConfigValidacaoContato> value)
        {
            var retorno = new GetApiRetorno<bool>();
            var contatos = new List<ContatosValidacaoListModel>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var comm = session.Connection.CreateCommand())
                {
                    var parametros = new Dictionary<string, object>();
                    var condicoes = new List<string>();
                    if (value.Objeto != null)
                    {
                        condicoes.Add("A.ID = @ID");
                        parametros.Add("@ID", value.Objeto.Id);
                    }
                    var sql = new StringBuilder();
                    sql.AppendLine("SELECT B.ID_CLIENTE AS ID, B.NOME_CONTATO, B.TIPO_CONTATO, COALESCE(B.EMAIL, B.TELEFONE) AS CONTATO")
                       .AppendLine(" FROM TB_CLIENTE A")
                       .AppendLine(" LEFT JOIN TB_CLIENTE_CONTATO B ON B.ID_CLIENTE = A.ID")
                       .AppendLine(" WHERE A.ID = @ID");
                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0;
                            if (id != 0)
                            {
                                contatos.Add(new ContatosValidacaoListModel
                                {
                                    Id = id,
                                    NomeContato = reader["NOME_CONTATO"] != DBNull.Value ? Convert.ToString(reader["NOME_CONTATO"]) : "",
                                    TipoContato = reader["TIPO_CONTATO"] != DBNull.Value ? Convert.ToString(reader["TIPO_CONTATO"]) : "",
                                    Contato = reader["CONTATO"] != DBNull.Value ? Convert.ToString(reader["CONTATO"]) : "",
                                });
                            }
                        }
                    }
                }
                if (contatos.Count > 0)
                {
                    retorno.Objeto = true;
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;
        }
        [HttpPost]
        [Route("api/clienteapi/GetContatos")]
        public async Task<GetApiRetorno<List<ContatosValidacaoListModel>>> GetContatos(ParamWebApi<ConfigValidacaoContato> value)
        {
            var retorno = new GetApiRetorno<List<ContatosValidacaoListModel>> { Objeto = new List<ContatosValidacaoListModel>() };
            var contatos = new List<ContatosValidacaoListModel>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var comm = session.Connection.CreateCommand())
                {
                    var parametros = new Dictionary<string, object>();
                    var condicoes = new List<string>();
                    if (value.Objeto != null)
                    {
                        condicoes.Add("A.ID = @ID");
                        parametros.Add("@ID", value.Objeto.Id);
                    }
                    var sql = new StringBuilder();
                    sql.AppendLine("SELECT B.ID_CLIENTE AS ID, B.NOME_CONTATO, B.TIPO_CONTATO, COALESCE(B.EMAIL, B.TELEFONE) AS CONTATO")
                       .AppendLine(" FROM TB_CLIENTE A")
                       .AppendLine(" LEFT JOIN TB_CLIENTE_CONTATO B ON B.ID_CLIENTE = A.ID")
                       .AppendLine(" WHERE A.ID = @ID");
                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0;
                            if (id != 0)
                            {
                                contatos.Add(new ContatosValidacaoListModel
                                {
                                    Id = id,
                                    NomeContato = reader["NOME_CONTATO"] != DBNull.Value ? Convert.ToString(reader["NOME_CONTATO"]) : "",
                                    TipoContato = reader["TIPO_CONTATO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Contato_Cliente>(Convert.ToString(reader["TIPO_CONTATO"])) : "",
                                    Contato = reader["CONTATO"] != DBNull.Value ? Convert.ToString(reader["CONTATO"]) : "",
                                });
                            }
                        }
                    }
                }
                if (contatos.Count > 0)
                {
                    retorno.Objeto = contatos;
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/GetComboClienteContato")]
        public async Task<ObjCombo<Int64>> GetComboContato(RequestObjComboPai<Int64?, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CLIENTE_CONTATO A ");

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE = {request.idpai}");
                    }


                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME_CONTATO, A.TELEFONE, A.EMAIL ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.NOME_CONTATO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                List<string> listaText = new List<string>();
                                listaText.Add(Convert.ToString(reader["NOME_CONTATO"]));
                                listaText.Add(Convert.ToString(reader["TELEFONE"]));
                                listaText.Add(Convert.ToString(reader["EMAIL"]));
                                listaText = listaText.Where(p => !string.IsNullOrEmpty(p)).ToList();

                                retorno.results.Add(new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = String.Join(" - ", listaText)
                                });
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
    }

    public class ClienteValidacaoRetorno
    {
        public long IdCliente { get; set; }
        public long? IdCategoriaCli { get; set; }
        public string NomeFantasia { get; set; }
        public string TelefoneFixo1 { get; set; }
        public string TelefoneCelular { get; set; }
        public string TelefoneRecado { get; set; }
        public string Email { get; set; }
        public string NomeRecado { get; set; }
        public string Categoria { get; set; }
        public string IdentificacaoCliente { get; set; }
        #region PessoaJuridica
        public string CpfCnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string DataAberturaEmpresa { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string InscricaoEstadual { get; set; }
        public long? Porte { get; set; }
        public string RamoAtividade { get; set; }
        public string CpfResponsavel { get; set; }
        public string CargoResponsavel { get; set; }
        #endregion

        #region PessoaFisica
        public string DataNascimento { get; set; }
        public string Nacionalidade { get; set; }
        public long? Sexo { get; set; }
        public long? EstadoCivil { get; set; }
        public string DataEmissaoRg { get; set; }
        public string OrgaoRg { get; set; }
        public string NumeroRg { get; set; }
        public string Profissao { get; set; }
        public string FiliacaoPai { get; set; }
        public string FiliacaoMae { get; set; }
        public long? GrauInstrucao { get; set; }
        #endregion

    }

    public class CheckStatusCartao
    {
        public long IdCliente { get; set; }
        public long IdVenda { get; set; }
    }

    public class ResultValidacao
    {
        public string Mensagem { get; set; }
        public long IdStatus { get; set; }
        public bool TemPlanProdAtrelado { get; set; }
    }



    public class ResponseDocContrato
    {
        public long ClienteId { get; set; }
        public long ModeloSelecionadoId { get; set; }
        public string NumeroContrato { get; set; }
        public long IdCliContrato { get; set; }

    }

    public class ArquivoContrato
    {
        public string CaminhoPDF { get; set; }
        public string NomeArquivo { get; set; }
        public string ExtensaoArquivo { get; set; }
        public string TituloArquivo { get; set; }
    }

    public class EnvioLinkCartaoModel
    {
        public string EmailCliente { get; set; }
        public string Link { get; set; }


    }

    public class CadastroCartaoModel
    {
        public long IdCliente { get; set; }
        public long? IdFormaPagamento { get; set; }
    }

    public class EquipamentoClienteRequest
    {
        public string Cpf_Cnpj { get; set; }
    }

    public class AdicionarPlanoProdutoRequestModel
    {
        public long IdCliente { get; set; }
        public long IdPlanoProduto { get; set; }
        public long IdFormaPagamentoCliente { get; set; }
        public long? IdModeloContrato { get; set; }
        public string ValorFinalPersonalizado { get; set; }
        public bool End_Instalacao_Igual_Principal { get; set; }
        public EnderecoClienteViewModel EnderecoInstalacao { get; set; }
        public string Numero_Contrato { get; set; }
    }
}


