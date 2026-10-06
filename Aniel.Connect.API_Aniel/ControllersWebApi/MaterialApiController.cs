using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Repository.T5.Material;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Org.BouncyCastle.Ocsp;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class MaterialApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Material
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MaterialApi/GetAll")]
        public async Task<ApiResponse<Material_ListaModel>> GetAll(Material_ApiRequest request)
        {
            List<Material_ListaModel> retorno = new List<Material_ListaModel>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                int recordsTotal = 0;

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(CODMAT) CO FROM TB_MATERIAL");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        #region Sql

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(@"SELECT CODMAT, CODCLI, CODAUX, DESCRICAO, UNID, VALOR, FAMILIA, PRECISAO, ACRESCIMO_PERC, SUBCOD, VALOR_LOCACAO, TIPO,
                                           PERC_MES, NUM_CONTA, APELIDO, CLASSE_MO, CLASSIFICACAO, ESTOQUE_MINIMO, CODMAT_RET, CODMAT_DET, CODMAT_REAP,
                                           CODMAT_DEFEITO, CODMAT_SUCATA, QTDE_PESO_CONVERSAO, QTDE_METRO_CONVERSAO, LIBERA_CODCPL, CONJUGADO, PROGRAMAR_DM,
                                           AUTORIZACAO, COD_SUBGRUPO_MAT, ORDEM, INTEGRACAO, HISTORICO_CODCPL, HIERARQUIA, DESC_AUXILIAR, VALOR2,
                                           ALERTA_DIVIDA_TRANSF, USUARIO_CRIACAO, DATA_CRIACAO, HORA_CRIACAO, USUARIO_ALTERACAO, DATA_ALTERACAO,
                                           HORA_ALTERACAO, BLOCO_CONTROLADO, QTDE_BLOCO, ENVIA_RESERV, COD_TIPO_PATRIMONIO, UNID_PRECISAO, COD_GRUPO_CA,
                                           DM_UTILIZADO, EXIGIR_NF_TF, PREVER_GARANTIA, CODMAT_INTEGRA, VALOR_SUCATA_INST, ESTOQUE_MAXIMO, INC_RET_AUTO,
                                           FRACIONARIO, SEGMENTO, GERAR_SALDO_AUTO_APLIC, SOBRESSALENTE, GERAR_DM_RET_AUTO, RASTREABILIDADE_DOC,
                                           GERAR_TI_AUTO_APLIC, DM_OBRIGATORIA, OP_CONV_UNID, PRECISAO_CONVERSAO, SUBCOD_EXCLUSIVO, ENVIAR_MOBILE,
                                           GERAR_SALDO_DM, LIBERA_RECEBIMENTO, HISTORICO_ANTERIOR, CODMAT_AUX, TMP_INS, COD_MATERIAL_MERCOSUL, ID_SAP,
                                           NUM_SERIE, BLOCO_FOLHA, CRITICA_SALDO_DM_RET, CALIBRE, CAPACIDADE, DESPESADO, QTDE_EMBALAGEM,
                                           QTDE_MAX_CONV_SERIAL, ID_NCM, HABILITAR_COD_AUX_RETIRADA, INVENTARIO_CICLICO, QTDE_CARACT_CODCPL, COMODATO,
                                           ATIVACAO_SINAL
                                    FROM TB_MATERIAL ");


                        if (String.IsNullOrEmpty(request.Codigo_Material) == false)
                        {
                            sql.AppendLine("WHERE CODMAT = @CODMAT ");
                            parametros.Add("@CODMAT", request.Codigo_Material);
                        }






                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Material_ListaModel
                                {
                                    Codigo_Material = reader["CODMAT"] != DBNull.Value ? Convert.ToString(reader["CODMAT"]) : null,
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : null,
                                    Precisao = reader["PRECISAO"] != DBNull.Value ? Convert.ToString(reader["PRECISAO"]) : null,
                                    Unidade = reader["UNID"] != DBNull.Value ? Convert.ToString(reader["UNID"]) : null,
                                    Valor = reader["VALOR"] != DBNull.Value ? Convert.ToString(reader["VALOR"]) : null,
                                    Familia = reader["FAMILIA"] != DBNull.Value ? Convert.ToString(reader["FAMILIA"]) : null,
                                    Grupo = reader["COD_SUBGRUPO_MAT"] != DBNull.Value ? Convert.ToString(reader["COD_SUBGRUPO_MAT"]) : null,
                                    Classificacao = reader["CLASSIFICACAO"] != DBNull.Value ? Convert.ToString(reader["CLASSIFICACAO"]) : null,
                                    Tipo = reader["TIPO"] != DBNull.Value ? Convert.ToString(reader["TIPO"]) : null,
                                    Data_Criacao = reader["DATA_CRIACAO"] != DBNull.Value ? Convert.ToString(reader["DATA_CRIACAO"]) : null,
                                    Hora_Criacao = reader["HORA_CRIACAO"] != DBNull.Value ? Convert.ToString(reader["HORA_CRIACAO"]) : null,
                                    Usuario_Criacao = reader["USUARIO_CRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIO_CRIACAO"]) : null,
                                    Ativacao_Sinal = reader["ATIVACAO_SINAL"] != DBNull.Value ? Convert.ToString(reader["ATIVACAO_SINAL"]) : null,
                                    Apelido = reader["APELIDO"] != DBNull.Value ? Convert.ToString(reader["APELIDO"]) : null,
                                    Codigo_Complementar = reader["SUBCOD"] != DBNull.Value ? Convert.ToString(reader["SUBCOD"]) : null,
                                    CodMat_Auxiliar = reader["CODMAT_AUX"] != DBNull.Value ? Convert.ToString(reader["CODMAT_AUX"]) : null,
                                    CodMat_Integra = reader["CODMAT_INTEGRA"] != DBNull.Value ? Convert.ToString(reader["CODMAT_INTEGRA"]) : null,
                                    Codigo_Material_Det = reader["CODMAT_DET"] != DBNull.Value ? Convert.ToString(reader["CODMAT_DET"]) : null,
                                    Codigo_Material_Defeito = reader["CODMAT_DEFEITO"] != DBNull.Value ? Convert.ToString(reader["CODMAT_DEFEITO"]) : null,
                                    Codigo_Material_Reap = reader["CODMAT_REAP"] != DBNull.Value ? Convert.ToString(reader["CODMAT_REAP"]) : null,
                                    Codigo_Material_Ret = reader["CODMAT_RET"] != DBNull.Value ? Convert.ToString(reader["CODMAT_RET"]) : null,
                                    Codigo_Material_Sucata = reader["CODMAT_SUCATA"] != DBNull.Value ? Convert.ToString(reader["CODMAT_SUCATA"]) : null,
                                    Estoque_Minimo = reader["ESTOQUE_MINIMO"] != DBNull.Value ? Convert.ToString(reader["ESTOQUE_MINIMO"]) : null,
                                });
                            }
                        }
                    }

                    return new ApiResponse<Material_ListaModel>()
                    {
                        draw = request.settings.length,
                        recordsFiltered = retorno.Count,
                        quantidadeTotalRegistros = recordsTotal,
                        data = retorno,
                        error = ""
                    };
                }
            }
            catch (Exception e)
            {
                return new ApiResponse<Material_ListaModel>()
                {
                    error = e.Message
                };
            }
        }

        /// <summary>
        /// Grava um Material
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MaterialApi/Gravar")]
        public async Task<ApiResponseObj<Material_CadastroModel>> Gravar(Material_CadastroModel request)
        {
            Material_CadastroModel retorno = new Material_CadastroModel();
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
                {
                    var materialAniel = new MaterialAnielRepository(sessionT5);
                    var materialAnielBusiness = new MaterialAnielBusiness(materialAniel);

                    if (String.IsNullOrEmpty(request.Descricao))
                    {
                        throw new Exception("O Campo Nome é um campo obrigatório no registro de materiais.");
                    }

                    if (String.IsNullOrEmpty(request.Codigo_Material) && String.IsNullOrEmpty(request.Desc_Auxiliar))
                    {
                        throw new Exception("O Campo Codigo é um campo obrigatório no registro de materiais.");
                    }

                    MaterialAniel material = !String.IsNullOrEmpty(request.Codigo_Material) ? await materialAnielBusiness.GetById(request.Codigo_Material) : await materialAnielBusiness.GetById(request.Desc_Auxiliar);


                    if (material == null)
                    {
                        material = new MaterialAniel();
                        material.Data_Criacao = DateTime.Now;
                        material.Hora_Criacao = DateTime.Now;
                        material.Usuario_Criacao = request.settings.user;
                        if (!String.IsNullOrEmpty(request.Desc_Auxiliar))
                        {
                            material.CodMat = materialAnielBusiness.GetCodMaterialDesc();
                            request.Codigo_Material = material.CodMat;
                        }
                    }

                    material.SubCod = request.SubCod;
                    material.SubCod_Exclusivo = material.SubCod_Exclusivo;
                    material.Descricao = request.Descricao;
                    material.Familia = request.Familia;
                    material.Unid = request.Unidade;
                    material.Valor = request.Valor;
                    material.Classificacao = request.Classificacao;
                    material.Tipo = request.Tipo;
                    material.Grupo = String.IsNullOrEmpty(request.Grupo) ? (Int16?)null : Convert.ToInt16(request.Grupo);
                    material.Desc_Auxiliar = request.Desc_Auxiliar;
                    material.AtivacaoSinal = request.AtivaSinal.GetValueOrDefault(false) ? "S" : "N";
                    material.TipoEquipamento = request.TipoEquipamento;

                    material.CodMat_Ret = $"R{material.CodMat}";
                    material.CodMat_Det = $"E{material.CodMat}";
                    material.CodMat_Reap = $"B{material.CodMat}";
                    material.CodMat_Defeito = $"D{material.CodMat}";
                    material.CodMat_Sucata = $"S{material.CodMat}";
                    

                    materialAnielBusiness.SaveOrUpdate(material);

                    return new ApiResponseObj<Material_CadastroModel>()
                    {
                        ok = true,
                        data = request,
                        error = ""
                    };

                }
            }
            catch (Exception e)
            {
                return new ApiResponseObj<Material_CadastroModel>()
                {
                    ok = false,
                    data = null,
                    error = e.Message
                };
            }
        }

        /// <summary>
        /// Buscar um Material pelo seu codigo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MaterialApi/BuscarPorCodigo")]
        public async Task<ApiResponseObj<MaterialAniel>> BuscarPorCodigo(Material_ApiRequest request)
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
                {
                    var materialAniel = new MaterialAnielRepository(sessionT5);

                    if (String.IsNullOrEmpty(request.Codigo_Material))
                        throw new Exception("O codigo do material nao foi informado");

                    var material = await materialAniel.GetByCod(request.Codigo_Material);

                    return new ApiResponseObj<MaterialAniel>()
                    {
                        ok = true,
                        data = material,
                        error = ""
                    };
                }
            }
            catch (Exception e)
            {
                return new ApiResponseObj<MaterialAniel>()
                {
                    ok = false,
                    data = null,
                    error = e.Message
                };
            }
        }

    }
}