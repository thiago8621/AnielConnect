using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Repository.T5.CodigoComplementar;
using Aniel.Connect.Repository.Repository.T5.Devolucao;
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
    public class DevolucaoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de devolucao
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DevolucaoApi/GetAll")]
        public async Task<ApiResponse<Devolucao_AnielListModel>> GetAll(Devolucao_AnielApiRequest request)
        {
            List<Devolucao_AnielListModel> retorno = new List<Devolucao_AnielListModel>();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
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

                    if (String.IsNullOrEmpty(request.Data_Ini))
                    {
                        throw new Exception("Data Inicial é obrigatório");
                    }

                    if (String.IsNullOrEmpty(request.Data_Fim))
                    {
                        throw new Exception("Data Final é obrigatório");
                    }

                    DateTime? Data_Fim = Convert.ToDateTime(request.Data_Fim);
                    DateTime? Data_Ini = Convert.ToDateTime(request.Data_Ini);

                    if ((Data_Fim.Value - Data_Ini.Value).TotalDays >= 90
                     || ((Data_Fim.Value - Data_Ini.Value).TotalDays < 0))
                    {
                        throw new Exception("Informe um período dentro de 90 dias");
                    }


                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    int recordsTotal = 0;

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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_SEQUENCIA_RMDM A")
                        .AppendLine(" INNER JOIN TB_MOVIMENTO_RMDM E")
                        .AppendLine("ON (A.CODCT= E.CODCT AND A.PROJETO = E.PROJETO AND A.SEQRMDM = E.SEQRMDM)")
                        .AppendLine(" INNER JOIN TB_MATERIAL G ON (E.CODMAT=G.CODMAT)")
                        .AppendLine(" LEFT JOIN TB_EQUIPE B ON (A.EQUIPE=B.EQUIPE)")
                        .AppendLine(" LEFT JOIN TB_CENTRO_CUSTO C ON (A.COD_CCUSTO=C.COD_CCUSTO)")
                        .AppendLine(" LEFT JOIN TB_REGIAO D ON (A.COD_REGIAO = D.COD_REGIAO)")
                        .AppendLine(" LEFT JOIN TB_PERFIL_EQUIPE F ON (B.COD_PERFIL_EQUIPE = F.COD_PERFIL_EQUIPE)");

                    #region //Select

                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.CODCT, A.PROJETO, A.SEQRMDM, A.SEQTRANSF, A.NUM_OBRA, A.DATA, A.DATA_IMPRESSAO, A.EQUIPE,");
                        sql.AppendLine(" B.NOME AS EQUIPE,B.NUM_FRE AS NUM_FRE, B.COD_PERFIL_EQUIPE LCOD_PERFIL_EQUIPE, F.NOME PERFIL_EQUIPE,");
                        sql.AppendLine(" A.COD_CCUSTO, C.DESCRICAO AS CCUSTO, A.OBSERVACAO, A.HISTORICO,");
                        sql.AppendLine(" C.CODAUX,");
                        sql.AppendLine(" E.CODMAT,E.CODCPL, E.QTDE_ENTREGUE, E.QTDE_SOLICITADA,E.QTDE_DESCONTADA, E.CLASSIFICACAO_RMDM, E.VALOR_DESCONTAR, E.DIA,");
                        sql.AppendLine(" G.SUBCOD SUBCOD, G.VALOR VALOR, G.CLASSIFICACAO CLASSIFICACAO,");
                        sql.AppendLine(" G.LIBERA_CODCPL LIBERA_CODCPL, G.COD_SUBGRUPO_MAT COD_SUBGRUPO_MAT, G.UNID UNIDADE, G.USUARIO_CRIACAO,");
                        sql.AppendLine(" G.CODMAT_DET CODMAT_DET, G.DESCRICAO DESCRICAO_MATERIAL, G.DATA_CRIACAO");

                        sql.AppendLine(sqlFrom.ToString());

                        #endregion

                        #region //Filtrando

                        sql.AppendLine("WHERE G.DATA_CRIACAO BETWEEN @DATA_INI AND @DATA_FIM");
                        parametros.Add("@DATA_INI", request.Data_Ini);
                        parametros.Add("@DATA_FIM", request.Data_Fim);

                        if (!string.IsNullOrEmpty(request.Contrato))
                        {
                            sql.AppendLine("AND A.CODCT = @CODCT");
                            parametros.Add("@CODCT", request.Contrato);
                        }

                        if (!string.IsNullOrEmpty(request.Projeto))
                        {
                            sql.AppendLine("AND A.PROJETO = @PROJETO");
                            parametros.Add("@PROJETO", request.Projeto);
                        }

                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Devolucao_AnielListModel
                                {
                                    Codct = DBNull.Value == reader["CODCT"] ? null : Convert.ToString(reader["CODCT"]),
                                    Projeto = DBNull.Value == reader["PROJETO"] ? null : Convert.ToString(reader["PROJETO"]),
                                    Numero_Documento = DBNull.Value == reader["SEQRMDM"] ? null : Convert.ToString(reader["SEQRMDM"]),
                                    Data_Hora_Criacao = DBNull.Value == reader["DATA_CRIACAO"] ? null : Convert.ToString(reader["DATA_CRIACAO"]),
                                    Usuario_Criacao = DBNull.Value == reader["USUARIO_CRIACAO"] ? null : Convert.ToString(reader["USUARIO_CRIACAO"]),
                                    //Data_Hora_Baixa = DBNull.Value == reader[""]? null : Convert.ToString(reader[""]),
                                    //Usuario_Baixa = DBNull.Value == reader[""]? null : Convert.ToString(reader[""]),
                                    Data_Movimento = DBNull.Value == reader["DIA"] ? null : Convert.ToString(reader["DIA"]),
                                    Equipe = DBNull.Value == reader["EQUIPE"] ? null : Convert.ToString(reader["EQUIPE"]),
                                    Numero_Obra = DBNull.Value == reader["NUM_OBRA"] ? null : Convert.ToString(reader["NUM_OBRA"]),
                                    Cod_Material = DBNull.Value == reader["CODMAT"] ? null : Convert.ToString(reader["CODMAT"]),
                                    Cod_Complementar = DBNull.Value == reader["CODCPL"] ? null : Convert.ToString(reader["CODCPL"]),
                                    Cod_Complementar_Auxiliar = DBNull.Value == reader["CODAUX"] ? null : Convert.ToString(reader["CODAUX"]),
                                    Quantidade_Pedida = DBNull.Value == reader["QTDE_SOLICITADA"] ? 0 : Convert.ToInt32(reader["QTDE_SOLICITADA"]),
                                    Quantidade_Recebida = DBNull.Value == reader["QTDE_ENTREGUE"] ? 0 : Convert.ToInt32(reader["QTDE_ENTREGUE"]),
                                    Quantidade_Desconto = DBNull.Value == reader["QTDE_DESCONTADA"] ? 0 : Convert.ToInt32(reader["QTDE_DESCONTADA"]),
                                    Valor = DBNull.Value == reader["VALOR"] ? 0 : Convert.ToDouble(reader["VALOR"]),
                                    Valor_Desconto = DBNull.Value == reader["VALOR_DESCONTAR"] ? 0 : Convert.ToDouble(reader["VALOR_DESCONTAR"]),
                                    Classificacao = DBNull.Value == reader["CLASSIFICACAO"] ? null : Convert.ToString(reader["CLASSIFICACAO"]),
                                });
                            }
                        }

                    }

                    return new ApiResponse<Devolucao_AnielListModel>()
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
                    return new ApiResponse<Devolucao_AnielListModel>()
                    {
                        error = e.Message,
                    };
                }
            }
        }

        /// <summary>
        /// Grava um novo registro de devolução
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DevolucaoApi/GravarDevolucao")]
        public async Task<ApiResponseObj<EstoqueDevolucao_ApiRequest>> GravarDevolucao(EstoqueDevolucao_ApiRequest request)
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
                using (var repoEstoque = new Conexao<EstoqueAniel>(sessionT5))
                using (var repoIntegracaoProtheusReverse = new Conexao<IntegracaoProtheusReverseAniel>(sessionT5))
                {
                    String SeqRmdm = String.Empty, CodCt = String.Empty, Projeto = String.Empty;

                    var codigoComplementarAniel = new CodigoComplementarAnielRepository(sessionT5);
                    var requisicaoAniel = new RequisicaoAnielRepository(sessionT5);
                    var requisicaoAnielBusiness = new RequisicaoAnielBusiness(requisicaoAniel, codigoComplementarAniel);

                    var equipeAniel = new EquipeAnielRepository(sessionT5);
                    var equipeAnielBusiness = new EquipeAnielBusiness(equipeAniel,null, null);

                    var materialAniel = new MaterialAnielRepository(sessionT5);
                    var materialAnielBusiness = new MaterialAnielBusiness(materialAniel);

                    var d3Local = !string.IsNullOrEmpty(request.CodLocal) ? request.CodLocal : null;
                    var objIntegracao = repoIntegracaoProtheusReverse.selectFirst(x => x.Empresa == request.CodEmpresa && x.D3_Filial == request.CodFilial && x.D3_Local == d3Local).Result;

                    if (objIntegracao == null)
                    {
                        return new ApiResponseObj<EstoqueDevolucao_ApiRequest>()
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

                    if(equipe == null)
                    {
                        throw new Exception("Equipe não encontrada");
                    }


                    sequencia.Seqrmdm = SeqRmdm;
                    sequencia.Projeto = Projeto;
                    sequencia.Codct = CodCt;
                    sequencia.Equipe = equipe;
                    sequencia.Cancelado = 'N';
                    sequencia.Tipo_Movimento = 'D';
                    sequencia.Usuario = request.settings.user;
                    sequencia.Dia = DateTime.Now;
                    sequencia.Hora = DateTime.Now;

                    if (request.productToApproval.Count > 0)
                    {
                        foreach (var item in request.productToApproval)
                        {
                            var objMat = await materialAnielBusiness.GetById(item.IdProduct);
                            if (objMat == null)
                            {
                                return new ApiResponseObj<EstoqueDevolucao_ApiRequest>()
                                {
                                    ok = false,
                                    data = null,
                                    error = $"O campo IdProduct {item.IdProduct}, não está cadastrado!!"
                                };
                            }

                            listaMovimento.Add(requisicaoAnielBusiness.GetObjRmdm(item, sequencia, objMat));
                        }

                        await repoRequisicao_SequenciaRMDM.SaveOrUpdate(sequencia);

                        await repoRequisicao_MovimentoRMDM.SaveOrUpdate(listaMovimento);

                        /*
                         * Foi necessario incluir mais um SaveOrUpdate na tabela de Sequencia_RMDM, por conta
                         * da existencia de uma trigger, pois não estava salvando.
                         **/
                        sequencia.Baixada = 'S';
                        sequencia.Data = DateTime.Now;
                        await repoRequisicao_SequenciaRMDM.SaveOrUpdate(sequencia);

                        var estoqueAniel = new DevolucaoAnielRepository(sessionT5);
                        var estoqueAnielBusiness = new DevolucaoAnielBusiness(estoqueAniel);
                        var listaEstoque = new List<EstoqueAniel>();

                        /*
                         * Campo de observação inserido para ser adicionado na tabela de estoque, caso tenha informação.
                         * Esse campo ObservacaoGenerecia, não é mapeado no banco de dados**/
                        sequencia.ObservacaoGenerecia = request.Doc_EasyWear;

                        foreach (var item in listaMovimento)
                        {
                            var retorno = await estoqueAnielBusiness.GetObjEstoque(sequencia, item);
                            listaEstoque.Add(retorno);
                        }

                        await repoEstoque.SaveOrUpdate(listaEstoque);
                    }
                }

                return new ApiResponseObj<EstoqueDevolucao_ApiRequest>()
                {
                    ok = true,
                    data = request,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<EstoqueDevolucao_ApiRequest>()
                {
                    ok = false,
                    data = null,
                    error = ex.Message
                };
            }
        }
    }
}