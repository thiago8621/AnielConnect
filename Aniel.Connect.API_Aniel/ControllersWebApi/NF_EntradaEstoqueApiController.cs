using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Wordprocessing;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Management;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{

    public class NFEntradaEstoqueApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Nota fiscal por Entrada de Material no Aniel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost()]
        [Route("api/NF_EntradaApi/GetAll")]
        public async Task<ApiResponse<NF_Entrada_Estoque>> GetAll(NF_entradaApiRequest request)
        {

            List<NF_Entrada_Estoque> retorno = new List<NF_Entrada_Estoque>();
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
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();



                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        DateTime? Data_Inicial = Convert.ToDateTime(request.DataInicial);
                        DateTime? Data_Final = Convert.ToDateTime(request.DataFinal);
                        StringBuilder sqlFrom = new StringBuilder();

                        sqlFrom.AppendLine(" FROM TB_RECEBIMENTO_MATERIAL A ");
                        sqlFrom.AppendLine(" LEFT JOIN TB_FORNECEDOR B ");
                        sqlFrom.AppendLine(" ON (B.COD_FORNECEDOR = A.COD_FORNECEDOR) ");

                        StringBuilder sqlWhere = new StringBuilder();

                        //Filtrando
                        if (!String.IsNullOrEmpty(request.settings.search))
                        {
                            sqlWhere.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search));
                        }
                        if (!String.IsNullOrEmpty(request.DataInicial) || !String.IsNullOrEmpty(request.DataFinal))
                        {
                            sqlWhere.AppendLine("WHERE (DATA BETWEEN @DATAINI AND @DATAFIM)");
                            parametros.Add("@DATAINI", Data_Inicial);
                            parametros.Add("@DATAFIM", Data_Final);
                        }

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.VALOR_COMPRA, A.CODMAT, A.DATA, A.NOTAFISCAL, A.PEDIDO, A.QTDE_SOLICIT, A.QUANTIDADE, B.CNPJ ");


                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());



                        //  sql.AppendLine(" WHERE A.STATUS = 1 ");




                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new NF_Entrada_Estoque
                                {
                                    Id = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ID"]),
                                    CNPJ_Fornecedor = Convert.ToString(reader["CNPJ"]),
                                    Cod_Material = Convert.ToString(reader["CODMAT"]),
                                    Data_Emissao = Convert.ToString(reader["DATA"]),
                                    Data_Recebimento = Convert.ToString(reader["DATA"]),
                                    Numero_NF = Convert.ToString(reader["NOTAFISCAL"]),
                                    Numero_Pedido = Convert.ToString(reader["PEDIDO"]),
                                    Quantidade = reader["QTDE_SOLICIT"] == DBNull.Value ? 0 : Convert.ToInt64(reader["QTDE_SOLICIT"]),
                                    Unidade = Convert.ToString(reader["QUANTIDADE"]),
                                    Valor = reader["VALOR_COMPRA"] == DBNull.Value ? 0 : Convert.ToDouble(reader["VALOR_COMPRA"]),

                                });
                            }
                        }
                    }
                    return new ApiResponse<NF_Entrada_Estoque>()
                    {
                        draw = request.settings.length,
                        data = retorno,
                        error = ""
                    };
                }
            }

            catch (Exception e)
            {
                return new ApiResponse<NF_Entrada_Estoque>()
                {
                    draw = request.settings.length,
                    data = retorno,
                    error = e.Message
                };

            }



        }

        [HttpPost()]
        [Route("api/NF_EntradaApi/Gravar")]
        public async Task<ApiResponseObj<object>> Gravar(NFEntradaCadastroModel model)
        {
            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(model.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }

            ApiResponseObj<object> retorno = new ApiResponseObj<object>();

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                using (var masterRepo = new Conexao<Master>(sessionT5))
                using (var empresaRepo = new Conexao<EmpresaAniel>(sessionT5))
                using (var filialRepo = new Conexao<FilialAniel>(sessionT5))
                using (var contratoRepo = new Conexao<ContratoAniel>(sessionT5))
                using (var materialRepo = new Conexao<MaterialAniel>(sessionT5))
                {
                    Master entradaDiretaConfig = await masterRepo.selectFirst(m => m.Config_Doc != null);
                    bool entradaDireta = entradaDiretaConfig?.Config_Doc.Contains("ENTRADA_DIRETA=S") != null ? true : false;

                    #region Validações
                    
                    if (String.IsNullOrEmpty(model.NotaFiscal))
                        throw new Exception("Nota fiscal não informada.");
                    
                    if (String.IsNullOrEmpty(model.CodigoMaterial))
                        throw new Exception("Código do material não informado.");
                    
                    if (String.IsNullOrEmpty(model.Quantidade))
                        throw new Exception("Quantidade não informada.");
                    
                    if (String.IsNullOrEmpty(model.Valor_Unitario))
                        throw new Exception("Valor unitário não informado.");

                    if (String.IsNullOrEmpty(model.Contrato))
                        throw new Exception("Contrato não informado.");

                    if (String.IsNullOrEmpty(model.Projeto))
                        throw new Exception("Projeto não informado.");

                    if (String.IsNullOrEmpty(model.CnpjFornecedor))
                        throw new Exception("CNPJ do fornecedor não informado.");

                    if (String.IsNullOrEmpty(model.FilialAuxiliar))
                        throw new Exception("Filial não informada.");

                    var filial = await filialRepo.selectFirst(f => f.CodFilialAux == model.FilialAuxiliar);
                    if (filial == null)
                        throw new Exception("Filial não encontrada.");
                    
                    #endregion

                    var codFornecedor = await getCodFornecedor(model.CnpjFornecedor, sessionT5);

                    if (entradaDireta)
                    {  
                        bool usaCodComplementar = materialRepo.selectFirst(m => m.CodMat == model.CodigoMaterial).Result.SubCod == "S";
                        retorno.data =  await GravarMaterial(model, filial, codFornecedor, sessionT5);
                    }
                    else
                    {
                        List<RecebimentoIntegra> recebimentos = new List<RecebimentoIntegra>();
                        List<RecebimentoIntegraItem> itensRecebimentos = new List<RecebimentoIntegraItem>();

                        using (var RecebIntegraRepo = new Conexao<RecebimentoIntegra>(sessionT5))
                        using (var RecebIntegraItemRepo = new Conexao<RecebimentoIntegraItem>(sessionT5))
                        {
                            var recebIntegra = new RecebimentoIntegra
                            {
                                Pedido = model.Pedido,
                                DataHoraCriacao = DateTime.Now,
                                Dia = Convert.ToDateTime(model.DataRecebimento).Day.ToString(),
                                Hora = Convert.ToDateTime(model.DataRecebimento).TimeOfDay.ToString(),
                                CodEmpresa = filial.CodEmpresa.ToString(),
                                CodFornecedor = codFornecedor,
                                CodFilial = filial.CodFilial.ToString(),
                                NotaFiscal = model.NotaFiscal,
                                Contrato = model.Contrato,
                                Projeto = model.Projeto,
                                Usuario = "APIANIEL",
                            };

                            recebimentos.Add(recebIntegra);
                            await RecebIntegraItemRepo.Save(itensRecebimentos);

                            long recebIntegraId = RecebIntegraRepo.selectFirst(r =>
                                r.Pedido == recebIntegra.Pedido &&
                                r.CodEmpresa == recebIntegra.CodEmpresa &&
                                r.CodFornecedor == recebIntegra.CodFornecedor &&
                                r.Dia == recebIntegra.Dia &&
                                r.Hora == recebIntegra.Hora
                            ).Result.Id;

                            var recebItem = new RecebimentoIntegraItem
                            {
                                Id_Receb = recebIntegraId.ToString(),
                                CodContrato = model.Contrato,
                                CodMaterial = model.CodigoMaterial,
                                DataRecebimento = Convert.ToDateTime(model.DataRecebimento),
                                CodComplementar = !String.IsNullOrEmpty(model.CodigoComplementar) ? model.CodigoComplementar : null,
                                Dia = Convert.ToDateTime(model.DataRecebimento).Day.ToString(),
                                Hora = Convert.ToDateTime(model.DataRecebimento).TimeOfDay.ToString(),
                                DataHoraCriacao = DateTime.Now,
                                Projeto = model.Projeto,
                                Quantidade = model.Quantidade,
                                ValorUnitario = model.Valor_Unitario,
                                ValorTotal = (Convert.ToDouble(model.Valor_Unitario) * Convert.ToInt64(model.Quantidade)).ToString(),
                                NotaFiscal = model.NotaFiscal,
                                Usuario = "APIANIEL",
                            };

                            itensRecebimentos.Add(recebItem);
                            await RecebIntegraRepo.Save(recebimentos);

                            retorno.data = recebIntegra;
                            retorno.data = recebItem;
                        }
                    }

                    retorno.ok = true;
                    return retorno;
                }
            }
            catch (Exception e)
            {
                return new ApiResponseObj<object> { ok = false, error = e.Message };
            }
        }

        private async Task<EstoqueAniel> GravarMaterial(NFEntradaCadastroModel model, FilialAniel filial, string codFornecedor, ISession sessionT5)
        {
            try
            {
                using (var estoqueRepo = new Conexao<EstoqueAniel>(sessionT5))
                using (var materialRepo = new Conexao<MaterialAniel>(sessionT5))
                {
                    bool usaCodComplementar = materialRepo.selectFirst(m => m.CodMat == model.CodigoMaterial.Trim()).Result.SubCod == "S";
                    EstoqueAniel estoque = new EstoqueAniel();

                    if (usaCodComplementar)
                    {
                        if (String.IsNullOrEmpty(model.CodigoComplementar))
                            throw new Exception("Código complementar não informado para o material que exige código complementar.");

                        estoque = new EstoqueAniel
                        {
                            CodCt = model.Contrato.Trim(),
                            Projeto = model.Projeto.Trim(),
                            CodMat = model.CodigoMaterial.Trim(),
                            CodCpl = model.CodigoComplementar?.Trim(),
                            Data = Convert.ToDateTime(model.DataRecebimento),

                            Pedido = !String.IsNullOrEmpty(model.Pedido) ? model.Pedido?.Trim() : model.NotaFiscal.Trim(),
                            Entrada = Convert.ToInt64(model.Quantidade),
                            Saida = 0,
                            NotaFiscal = model.NotaFiscal.Trim(),
                            Cod_Fornecedor = codFornecedor,
                            CodEmpresa = filial.CodEmpresa,
                            Valor_Compra = Convert.ToDouble(model.Valor_Unitario),
                            Qtde_Tarefa = Convert.ToInt64(model.Quantidade),
                            Valor_Total = Convert.ToDouble(model.Valor_Unitario) * Convert.ToInt64(model.Quantidade),
                            Transferencia = 'N',
                            Tipo_Atividade = 'C',

                            SeqLivre = "-",
                            SeqRmdm = "-",
                            SeqDevolucao = "-",
                            SeqTransf = "-",
                            ProjOrigem = "-",
                            ProjDestino = "-",

                            Usuario = "APIANIEL"
                        };

                        var itemExiste = await estoqueRepo.selectFirst(e =>
                            e.CodMat == estoque.CodMat &&
                            e.CodCpl == estoque.CodCpl &&
                            e.CodCt == estoque.CodCt &&
                            e.Projeto == estoque.Projeto &&
                            e.NotaFiscal == estoque.NotaFiscal &&
                            e.Qtde_Tarefa == estoque.Qtde_Tarefa
                        );

                        if (itemExiste != null)
                            throw new Exception("Já existe um registro de estoque para o material com código complementar, contrato, projeto, nota fiscal e quantidade informados.");
                    }
                    else
                    {
                        estoque = new EstoqueAniel
                        {
                            CodCt = model.Contrato.Trim(),
                            Projeto = model.Projeto.Trim(),
                            CodMat = model.CodigoMaterial.Trim(),
                            CodCpl = "-",
                            Data = Convert.ToDateTime(model.DataRecebimento),

                            Pedido = !String.IsNullOrEmpty(model.Pedido) ? model.Pedido?.Trim() : model.NotaFiscal.Trim(),
                            Entrada = Convert.ToInt64(model.Quantidade),
                            Saida = 0,
                            NotaFiscal = model.NotaFiscal.Trim(),
                            Cod_Fornecedor = codFornecedor,
                            CodEmpresa = filial.CodEmpresa,
                            Valor_Compra = Convert.ToDouble(model.Valor_Unitario),
                            Qtde_Tarefa = Convert.ToInt64(model.Quantidade),
                            Valor_Total = Convert.ToDouble(model.Valor_Unitario) * Convert.ToInt64(model.Quantidade),
                            Transferencia = 'N',
                            Tipo_Atividade = 'C',

                            SeqLivre = "-",
                            SeqRmdm = "-",
                            SeqDevolucao = "-",
                            SeqTransf = "-",
                            ProjOrigem = "-",
                            ProjDestino = "-",

                            Usuario = "APIANIEL"
                        };

                        var itemExiste = await estoqueRepo.selectFirst(e =>
                            e.CodMat == estoque.CodMat &&
                            e.CodCt == estoque.CodCt &&
                            e.Projeto == estoque.Projeto &&
                            e.NotaFiscal == estoque.NotaFiscal &&
                            e.Qtde_Tarefa == estoque.Qtde_Tarefa
                        );

                        if (itemExiste != null)
                            throw new Exception("Já existe um registro de estoque para o material com contrato, projeto, nota fiscal e quantidade informados.");
                    }

                    await estoqueRepo.Save(estoque);

                    return estoque;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao gravar estoque do material: " + e.Message);
            }
        }

        private async Task<string> getCodFornecedor(string cnpj, ISession sessionT5)
        {
            using (var fornecedorRepo = new Conexao<Fornecedor>(sessionT5))
            {
                var fornecedor = await fornecedorRepo.selectFirst(f => f.Cnpj == cnpj);
                if (fornecedor == null)
                    throw new Exception("Fornecedor não encontrado para o CNPJ informado.");
                return fornecedor.Cod_Fornecedor;
            }
        }
    }

    public class NFEntradaCadastroModel : ApiRequest
    {
        public string Contrato { get; set; }
        public string Projeto { get; set; }
        public string NotaFiscal { get; set; }
        public string CodigoMaterial { get; set; }
        public string FilialAuxiliar { get; set; }
        public string CnpjFornecedor { get; set; }
        public string Pedido { get; set; }
        public string DataRecebimento { get; set; }
        public string CodigoComplementar { get; set; }
        public string Quantidade { get; set; }
        public string Valor_Unitario { get; set; }   
    }
}

