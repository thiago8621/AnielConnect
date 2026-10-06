using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Repository.T5.CodigoComplementar;
using Aniel.Connect.Repository.Repository.T5.Estoque;
using Aniel.Connect.Repository.Repository.T5.Material;
using Aniel.Connect.Repository.Repository.T5.Requisicao;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.T5.Model.nhibernate;
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
    public class RequisicaoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Requisições Gravadas no Aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RequisicaoApi/GetAll")]
        public async Task<ApiResponse<Requisicao_AnielListModel>> GetAll(Requiscao_ApiRequest request)
        {
            List<Requisicao_AnielListModel> retorno = new List<Requisicao_AnielListModel>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                DateTime? Data_Inicial = Convert.ToDateTime(request.Data_Ini);
                DateTime? Data_Final = Convert.ToDateTime(request.Data_Fim);

                int recordsTotal = 0;

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(CODMAT) CO FROM TB_SEQUENCIA_RMDM");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        #region Sql
                        StringBuilder sqlFrom = new StringBuilder();
                        sqlFrom.AppendLine(@"from TB_SEQUENCIA_RMDM A
                                            INNER JOIN TB_MOVIMENTO_RMDM E ON(A.CODCT = E.CODCT and A.PROJETO = E.PROJETO and A.SEQRMDM = E.SEQRMDM)
                                            INNER JOIN TB_MATERIAL G ON(E.CODMAT = G.CODMAT)
                                            LEFT JOIN TB_EQUIPE B ON(A.EQUIPE = B.EQUIPE)
                                            LEFT JOIN TB_CENTRO_CUSTO C ON(A.COD_CCUSTO = C.COD_CCUSTO)
                                            LEFT JOIN TB_REGIAO D ON(A.COD_REGIAO = D.COD_REGIAO)
                                            LEFT JOIN TB_PERFIL_EQUIPE F ON(B.COD_PERFIL_EQUIPE = F.COD_PERFIL_EQUIPE)");

                        StringBuilder sqlWhere = new StringBuilder();
                        //sqlWhere.AppendLine(@"Where A.CODCT =:pCODCT and A.PROJETO =:pPROJETO and A.FILTRO = '1' AND A.BAIXADA = 'S'
                        //AND(A.DATA BETWEEN: @DATA_INI AND: @DATA_FIM) AND COALESCE(E.CANCELADO, 'N') = 'N'");

                        if (!Data_Inicial.HasValue)
                        {
                            throw new Exception("Data Inicial é obrigatório");
                        }

                        if (!Data_Final.HasValue)
                        {
                            throw new Exception("Data Final é obrigatório");
                        }
                        if ((Data_Final.Value - Data_Inicial.Value).TotalDays >= 90
                    || ((Data_Final.Value - Data_Inicial.Value).TotalDays < 0))
                        {
                            throw new Exception("Informe um período dentro de 90 dias");
                        }
                        sqlWhere.AppendLine("Where (A.DATA BETWEEN @DATA_INI AND @DATA_FIM)");
                        parametros.Add("DATA_INI", Data_Inicial);
                        parametros.Add("DATA_FIM", Data_Final);

                        if ((request.Contrato).Count() != 0)
                        {
                            sqlWhere.AppendLine("AND A.CODCT = @CODCT");
                            parametros.Add("@CODCT", request.Contrato);
                        }
                        if ((request.Projeto).Count() != 0)
                        {
                            sqlWhere.AppendLine("AND A.PROJETO = @PROJETO");
                            parametros.Add("@PROJETO", request.Projeto);
                        }

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(@"Select A.CODCT, A.PROJETO, A.SEQRMDM, A.SEQTRANSF, A.NUM_OBRA, A.DATA, A.DATA_IMPRESSAO, A.EQUIPE,
                        B.NOME as EQUIPE,B.NUM_FRE as NUM_FRE, B.COD_PERFIL_EQUIPE lCOD_PERFIL_EQUIPE, F.NOME PERFIL_EQUIPE,
                        A.COD_CCUSTO, C.DESCRICAO as CCUSTO, A.OBSERVACAO, A.HISTORICO,
                        E.CODMAT,E.CODCPL, E.QTDE_ENTREGUE, E.QTDE_SOLICITADA, E.CLASSIFICACAO_RMDM, E.DIA_ALTEROU,
  G.SUBCOD SUBCOD, G.VALOR VALOR, G.CLASSIFICACAO CLASSIFICACAO, G.CODAUX,
  G.LIBERA_CODCPL LIBERA_CODCPL, G.COD_SUBGRUPO_MAT COD_SUBGRUPO_MAT, G.UNID UNIDADE,
  G.CODMAT_DET CODMAT_DET, G.DESCRICAO DESCRICAO_MATERIAL, A.USUARIO, A.USUARIO_AUTORIZ, A.DIA, A.HORA");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());


                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Requisicao_AnielListModel
                                {
                                    Cod_Complementar = String.IsNullOrEmpty(Convert.ToString(reader["CODCPL"])) ? "-" : Convert.ToString(reader["CODCPL"]),
                                    Cod_Complementar_Aux = String.IsNullOrEmpty(Convert.ToString(reader["CODAUX"])) ? "-" : Convert.ToString(reader["CODAUX"]),
                                    Cod_Material = String.IsNullOrEmpty(Convert.ToString(reader["CODMAT"])) ? "-" : Convert.ToString(reader["CODMAT"]),
                                    Contrato = String.IsNullOrEmpty(Convert.ToString(reader["CODCT"])) ? "-" : Convert.ToString(reader["CODCT"]),
                                    Data_Hora_Baixa = String.IsNullOrEmpty(Convert.ToString(reader["DATA_IMPRESSAO"])) ? "-" : Convert.ToString(reader["DATA_IMPRESSAO"]),
                                    Data_Hora_Criacao = String.IsNullOrEmpty(Convert.ToString(reader["DATA"])) ? "-" : Convert.ToString(reader["DATA"]),
                                    Data_Movimento = String.IsNullOrEmpty(Convert.ToString(reader["DIA_ALTEROU"])) ? "-" : Convert.ToString(reader["DIA_ALTEROU"]),
                                    Equipe = String.IsNullOrEmpty(Convert.ToString(reader["EQUIPE"])) ? "-" : Convert.ToString(reader["EQUIPE"]),
                                    Numero_Documento = String.IsNullOrEmpty(Convert.ToString(reader["SEQRMDM"])) ? "-" : Convert.ToString(reader["SEQRMDM"]),
                                    Numero_Obra = String.IsNullOrEmpty(Convert.ToString(reader["NUM_OBRA"])) ? "-" : Convert.ToString(reader["NUM_OBRA"]),
                                    Projeto = String.IsNullOrEmpty(Convert.ToString(reader["PROJETO"])) ? "-" : Convert.ToString(reader["PROJETO"]),
                                    Quantidade_Pedida = String.IsNullOrEmpty(Convert.ToString(reader["QTDE_SOLICITADA"])) ? "-" : Convert.ToString(reader["QTDE_SOLICITADA"]),
                                    Quantidade_Recebida = String.IsNullOrEmpty(Convert.ToString(reader["QTDE_ENTREGUE"])) ? "-" : Convert.ToString(reader["QTDE_ENTREGUE"]),
                                    Valor = String.IsNullOrEmpty(Convert.ToString(reader["VALOR"])) ? "-" : Convert.ToString(reader["VALOR"]),
                                    UsuarioBaixa = String.IsNullOrEmpty(Convert.ToString(reader["USUARIO"])) ? "-" : Convert.ToString(reader["USUARIO"]),
                                    UsuarioCriacao = String.IsNullOrEmpty(Convert.ToString(reader["USUARIO_AUTORIZ"])) ? "-" : Convert.ToString(reader["USUARIO_AUTORIZ"])

                                });
                            }
                        }
                    }
                }

                return new ApiResponse<Requisicao_AnielListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    recordsFiltered = retorno.Count,
                    quantidadeTotalRegistros = recordsTotal,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<Requisicao_AnielListModel>()
                {
                    error = e.Message
                };
            }
        }

        /// <summary>
        /// Gravar requisições no aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RequisicaoApi/Gravar")]
        public async Task<ApiResponseObj<Requisicao_CadastroModel>> Gravar(Requisicao_CadastroModel request)
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

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoRequisicao_SequenciaRMDM = new Conexao<Requisicao_SequenciaRmdmAniel>(sessionT5))
                using (var repoRequisicao_MovimentoRMDM = new Conexao<Requisicao_MovimentoRmdm>(sessionT5))
                using (var repoIntegracaoProtheusReverse = new Conexao<IntegracaoProtheusReverseAniel>(sessionT5))
                {
                    String SeqRmdm = String.Empty, CodCt = String.Empty, Projeto = String.Empty;
                    var codigoComplementarAniel = new CodigoComplementarAnielRepository(sessionT5);
                    var requisicaoAniel = new RequisicaoAnielRepository(sessionT5);
                    var requisicaoAnielBusiness = new RequisicaoAnielBusiness(requisicaoAniel, codigoComplementarAniel);

                    var equipeAniel = new EquipeAnielRepository(sessionT5);
                    var equipeAnielBusiness = new EquipeAnielBusiness(equipeAniel, null, null);

                    var materialAniel = new MaterialAnielRepository(sessionT5);
                    var materialAnielBusiness = new MaterialAnielBusiness(materialAniel);

                    IEstoqueAnielRepository estoqueAnielRepository = new EstoqueAnielRepository(sessionT5);
                    var estoqueBusiness = new EstoqueAnielBusiness(estoqueAnielRepository);

                    var d3Local = !string.IsNullOrEmpty(request.CodLocal) ? request.CodLocal : null;
                    var objIntegracao = repoIntegracaoProtheusReverse.selectFirst(x => x.Empresa == request.CodEmpresa && x.D3_Filial == request.CodFilial && x.D3_Local == d3Local).Result;

                    if (objIntegracao == null)
                    {
                        return new ApiResponseObj<Requisicao_CadastroModel>()
                        {
                            ok = false,
                            data = null,
                            error = "Verifique o código da EMPRESA, FILIAL e/ou LOCAL informado!!"
                        };
                    }

                    CodCt = objIntegracao.CodCt;
                    Projeto = objIntegracao.Projeto;

                    SeqRmdm = await requisicaoAnielBusiness.GetSeqRMDM(CodCt, Projeto);

                    var listaMovimento = new List<Requisicao_MovimentoRmdm>();
                    var sequencia = new Requisicao_SequenciaRmdmAniel();

                    var equipe = equipeAnielBusiness.GetByEquipe(request.CodTecnico);
                    if (equipe == null)
                    {
                        throw new Exception("Equipe não encontrada");
                    }

                    sequencia.Seqrmdm = SeqRmdm;
                    sequencia.Projeto = Projeto;
                    sequencia.Codct = CodCt;
                    sequencia.Equipe = equipe;
                    sequencia.URL_Aceite = request.acceptURL;
                    sequencia.URL_Recusa = request.declineURL;
                    sequencia.Tipo_Movimento = 'R';
                    sequencia.Baixada = 'N';
                    sequencia.Cancelado = 'N';
                    sequencia.Usuario = request.settings.user;
                    sequencia.Dia = DateTime.Now;
                    sequencia.Hora = DateTime.Now;
                    sequencia.Data = DateTime.Now;

                    if (request.productToApproval.Count > 0)
                    {
                        foreach (var item in request.productToApproval)
                        {
                           

                            var objMat = await materialAnielBusiness.GetById(item.IdProduct);
                            if (objMat == null)
                            {
                                return new ApiResponseObj<Requisicao_CadastroModel>()
                                {
                                    ok = false,
                                    data = null,
                                    error = $"O campo IdProduct {item.IdProduct}, não está cadastrado!!"
                                };
                            }

                            if (request.GeraEstoque == "S")
                            {
                                estoqueBusiness.EntradaDireta(objMat, item.Serie, CodCt, Projeto, DateTime.Now, SeqRmdm, item.Quantity.Value,SeqRmdm);
                            }

                            listaMovimento.Add(requisicaoAnielBusiness.GetObjRmdm(item, sequencia, objMat));
                        }

                        await repoRequisicao_SequenciaRMDM.SaveOrUpdate(sequencia);

                        await repoRequisicao_MovimentoRMDM.SaveOrUpdate(listaMovimento);
                    }

                    return new ApiResponseObj<Requisicao_CadastroModel>()
                    {
                        ok = true,
                        data = request,
                        error = ""
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<Requisicao_CadastroModel>()
                {
                    ok = false,
                    data = null,
                    error = ex.Message
                };
            }
        }
    }
}



