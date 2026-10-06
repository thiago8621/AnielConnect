using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using System.Linq;
using Aniel.Connect.Core.Helpers;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class VeiculoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/VeiculoApi/GetAll")]
        public async Task<ApiResponse<Veiculos>> GetAll(VeiculosRequest request)
        {
            List<Veiculos> retorno = new List<Veiculos>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("  SELECT A.CODEMPRESA,A.CODFILIAL,A.COD_FROTA,A.PLACA,A.COD_MARCA,A.COD_MODELO,A.NUM_FRE,A.COD_CCUSTO,A.COD_FORNECEDOR, ");
                        sql.AppendLine("         A.SITUACAO,(A.LOCADO_PROPRIO)AS COD_PROPRIETARIO, ");
                        sql.AppendLine("        (CASE A.LOCADO_PROPRIO WHEN 'P' THEN 'Próprio da Empresa' WHEN 'A' THEN 'Locadora' WHEN 'E' THEN 'Empresa Terceiro/Próprio Fusão' WHEN 'L' THEN 'Agregado' END)AS PROPRIETARIO, ");
                        sql.AppendLine("         COALESCE(A.KM,0)AS KM,A.CHASSI,A.COD_RENAVAM,A.ANO,A.COR,A.COD_COMBUSTIVEL,A.CIDADE, ");
                        sql.AppendLine("         A.DATA_AQUISICAO,A.DATA_BAIXA,A.OBSERVACAO,A.HISTORICO,A.NUM_PASSAGEIRO,COALESCE(A.TIPO_VEICULO,0)AS COD_TIPO_VEICULO, ");
                        sql.AppendLine("        (CASE COALESCE(A.TIPO_VEICULO,0) WHEN 0 THEN 'Titular' WHEN 1 THEN 'Reserva' WHEN 2 THEN 'Provisório' WHEN 3 THEN 'Rent a Car' WHEN 4 THEN 'Pool' END)AS TIPO_VEICULO, "); 
                        sql.AppendLine("         A.ANO_CARROCERIA,A.BANCO_ALIENACAO,A.COD_TIPO_CARRO,COALESCE(O.VALOR_LOCACAO,0)AS VALOR_LOCACAO,A.LOCALIDADE, ");
                        sql.AppendLine("         COALESCE(A.MEDIA_KM_LITRO,0)AS MEDIA_KM_LITRO,COALESCE(A.MEDIA_KM_GNV,0)AS MEDIA_KM_GNV, ");
                        sql.AppendLine("         A.CONTABILIZADO,COALESCE(A.VALOR_VENDA,0)AS VALOR_VENDA,COALESCE(A.VALOR_AQUISICAO,0)AS VALOR_AQUISICAO, ");
                        sql.AppendLine("         A.LOCAL_AQUISICAO,A.GARAGEM,A.COD_CLIENTE,A.UF,A.SEGURO,A.ANO_FABRICACAO,A.NOTA_FISCAL,A.KM_ORIGINAL, ");
                        sql.AppendLine("         A.CAPACIDADE_TANQUE,A.DATA_CADASTRO,(A.COD_FORNEC_SERV)AS COD_POOL,O.INICPREV,O.TERMPREV,O.CODCT_ORIGEM,A.VALIDADE_EXTINTOR, ");
                        sql.AppendLine("         A.TAG,A.IPVA,A.CRV,A.CAPACIDADE_CILINDRO,A.COD_DEPARTAMENTO_FROTA,A.COD_TARIFA_LOCACAO,(B.DESCRICAO)AS MARCA, ");
                        sql.AppendLine("        (C.DESCRICAO)AS MODELO,D.DESCRICAO_SITUACAO,(E.NOME)AS CONDUTOR,(F.DESCRICAO)AS CENTRO_CUSTO,(G.DESCRICAO)AS CATEGORIA, ");
                        sql.AppendLine("         H.LOCAL_GARAGEM,(H.ENDERECO)AS ENDERECO_GARAGEM,(H.NUMERO)AS NUMERO_GARAGEM,(H.BAIRRO)AS BAIRRO_GARAGEM,(H.MUNICIPIO)AS MUNICIPIO_GARAGEM, ");
                        sql.AppendLine("        (H.CEP)AS CEP_GARAGEM,(H.UF)AS UF_GARAGEM,(H.LATITUDE)AS LATITUDE_GARAGEM,(H.LONGITUDE)AS LONGITUDE_GARAGEM, ");
                        sql.AppendLine("        (I.DESCRICAO)AS TIPO_COMBUSTIVEL,(J.DESCRICAO)AS POOL,K.EMPRESA,(L.NOME)AS FILIAL,(M.FORNECEDOR)AS FORNECEDOR_LOCADORA, ");
                        sql.AppendLine("        (N.DESCRICAO)AS DEPARTAMENTO_FROTA,A.CELULA_RIF,A.KM_CONTRATADO,A.NUM_CARTAO,(P.RAZAO_SOCIAL)AS CLIENTE ");
                        sql.AppendLine("  FROM TB_VEICULO A ");
                        sql.AppendLine("  LEFT JOIN TB_MARCA B ON (A.COD_MARCA = B.COD_MARCA) ");
                        sql.AppendLine("  LEFT JOIN TB_MODELO_VEICULO C ON (A.COD_MARCA = C.COD_MARCA AND A.COD_MODELO = C.COD_MODELO) ");
                        sql.AppendLine("  LEFT JOIN TB_SITUACAO_VEICULO D ON (A.SITUACAO = D.SITUACAO) ");
                        sql.AppendLine("  LEFT JOIN TB_FUNCIONARIO E ON (A.NUM_FRE = E.NUM_FRE) ");
                        sql.AppendLine("  LEFT JOIN TB_CENTRO_CUSTO F ON (A.COD_CCUSTO = F.COD_CCUSTO) ");
                        sql.AppendLine("  LEFT JOIN TB_TIPO_CARRO G ON (A.COD_TIPO_CARRO = G.COD_TIPO_CARRO) ");
                        sql.AppendLine("  LEFT JOIN TB_VEICULO_GARAGEM H ON (A.GARAGEM = H.GARAGEM) ");
                        sql.AppendLine("  LEFT JOIN TB_TIPO_COMBUSTIVEL I ON (A.COD_COMBUSTIVEL = I.COD_COMBUSTIVEL) ");
                        sql.AppendLine("  LEFT JOIN TB_FORNECEDOR_SERVICO J ON (A.COD_FORNEC_SERV = J.COD_FORNEC_SERV) ");
                        sql.AppendLine("  LEFT JOIN TB_EMPRESA K ON (A.CODEMPRESA = K.CODEMPRESA) ");
                        sql.AppendLine("  LEFT JOIN TB_FILIAL L ON (A.CODEMPRESA = L.CODEMPRESA AND A.CODFILIAL = L.CODFILIAL) ");
                        sql.AppendLine("  LEFT JOIN TB_FORNECEDOR M ON (A.COD_FORNECEDOR = M.COD_FORNECEDOR) ");
                        sql.AppendLine("  LEFT JOIN TB_DEPARTAMENTO_FROTA N ON (A.COD_DEPARTAMENTO_FROTA = N.COD_DEPARTAMENTO_FROTA) ");
                        sql.AppendLine("  LEFT JOIN TB_CONTRATO_LOCACAO_PRORRATA O ON (A.COD_FORNECEDOR = O.COD_FORNECEDOR AND A.COD_TARIFA_LOCACAO = O.ID) ");
                        sql.AppendLine("  LEFT JOIN TB_CLIENTE P ON (A.COD_CLIENTE = P.COD_CLIENTE) ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Veiculos
                                {
                                    codEmpresa = reader["CODEMPRESA"] != DBNull.Value ? Convert.ToString(reader["CODEMPRESA"]) : "0",
                                    codFilial = reader["CODFILIAL"] != DBNull.Value ? Convert.ToString(reader["CODFILIAL"]) : "0",
                                    codFrota = reader["COD_FROTA"] != DBNull.Value ? Convert.ToString(reader["COD_FROTA"]) : "-",
                                    placa = reader["PLACA"] != DBNull.Value ? Convert.ToString(reader["PLACA"]) : "-",
                                    codMarca = reader["COD_MARCA"] != DBNull.Value ? Convert.ToString(reader["COD_MARCA"]) : "0",
                                    codModelo = reader["COD_MODELO"] != DBNull.Value ? Convert.ToString(reader["COD_MODELO"]) : "0",
                                    numFre = reader["NUM_FRE"] != DBNull.Value ? Convert.ToString(reader["NUM_FRE"]) : "-",
                                    codCCusto = reader["COD_CCUSTO"] != DBNull.Value ? Convert.ToString(reader["COD_CCUSTO"]) : "0",
                                    codFornecedor = reader["COD_FORNECEDOR"] != DBNull.Value ? Convert.ToString(reader["COD_FORNECEDOR"]) : "-",
                                    codSituacao = reader["SITUACAO"] != DBNull.Value ? Convert.ToString(reader["SITUACAO"]) : "-",
                                    codProprietario = reader["COD_PROPRIETARIO"] != DBNull.Value ? Convert.ToString(reader["COD_PROPRIETARIO"]) : "-",
                                    proprietario = reader["PROPRIETARIO"] != DBNull.Value ? Convert.ToString(reader["PROPRIETARIO"]) : "-",
                                    km = reader["KM"] != DBNull.Value ? Convert.ToString(reader["KM"]) : "0",
                                    chassi = reader["CHASSI"] != DBNull.Value ? Convert.ToString(reader["CHASSI"]) : "-",
                                    codRenavam = reader["COD_RENAVAM"] != DBNull.Value ? Convert.ToString(reader["COD_RENAVAM"]) : "-",
                                    ano = reader["ANO"] != DBNull.Value ? Convert.ToString(reader["ANO"]) : "0",
                                    cor = reader["COR"] != DBNull.Value ? Convert.ToString(reader["COR"]) : "-",
                                    codCombustivel = reader["COD_COMBUSTIVEL"] != DBNull.Value ? Convert.ToString(reader["COD_COMBUSTIVEL"]) : "0",
                                    cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : "-",
                                    dataAquisicao = reader["DATA_AQUISICAO"] != DBNull.Value ? Convert.ToString(reader["DATA_AQUISICAO"]) : "",
                                    dataBaixa = reader["DATA_BAIXA"] != DBNull.Value ? Convert.ToString(reader["DATA_BAIXA"]) : "",
                                    observacao = reader["OBSERVACAO"] != DBNull.Value ? Convert.ToString(reader["OBSERVACAO"]) : "-",
                                    historico = reader["HISTORICO"] != DBNull.Value ? Convert.ToString(reader["HISTORICO"]) : "-",
                                    numPassageiro = reader["NUM_PASSAGEIRO"] != DBNull.Value ? Convert.ToString(reader["NUM_PASSAGEIRO"]) : "0",
                                    anoCarroceria = reader["ANO_CARROCERIA"] != DBNull.Value ? Convert.ToString(reader["ANO_CARROCERIA"]) : "0",
                                    bancoAlienacao = reader["BANCO_ALIENACAO"] != DBNull.Value ? Convert.ToString(reader["BANCO_ALIENACAO"]) : "-",
                                    codCategoria = reader["COD_TIPO_CARRO"] != DBNull.Value ? Convert.ToString(reader["COD_TIPO_CARRO"]) : "0",
                                    valorLocacao = reader["VALOR_LOCACAO"] != DBNull.Value ? Convert.ToString(reader["VALOR_LOCACAO"]) : "0",
                                    localidade = reader["LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["LOCALIDADE"]) : "-",
                                    mediaKmLitro = reader["MEDIA_KM_LITRO"] != DBNull.Value ? Convert.ToString(reader["MEDIA_KM_LITRO"]) : "0",
                                    mediaKmGnv = reader["MEDIA_KM_GNV"] != DBNull.Value ? Convert.ToString(reader["MEDIA_KM_GNV"]) : "0",
                                    contabilizado = reader["CONTABILIZADO"] != DBNull.Value ? Convert.ToString(reader["CONTABILIZADO"]) : "N",
                                    valorVenda = reader["VALOR_VENDA"] != DBNull.Value ? Convert.ToString(reader["VALOR_VENDA"]) : "0",
                                    valorAquisicao = reader["VALOR_AQUISICAO"] != DBNull.Value ? Convert.ToString(reader["VALOR_AQUISICAO"]) : "0",
                                    codGaragem = reader["GARAGEM"] != DBNull.Value ? Convert.ToString(reader["GARAGEM"]) : "-",
                                    codCliente = reader["COD_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["COD_CLIENTE"]) : "0",
                                    uf = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : "-",
                                    seguro = reader["SEGURO"] != DBNull.Value ? Convert.ToString(reader["SEGURO"]) : "N",
                                    anoFabricacao = reader["ANO_FABRICACAO"] != DBNull.Value ? Convert.ToString(reader["ANO_FABRICACAO"]) : "0",
                                    notaFiscal = reader["NOTA_FISCAL"] != DBNull.Value ? Convert.ToString(reader["NOTA_FISCAL"]) : "-",
                                    kmOriginal = reader["KM_ORIGINAL"] != DBNull.Value ? Convert.ToString(reader["KM_ORIGINAL"]) : "0",
                                    capacidadeTanque = reader["CAPACIDADE_TANQUE"] != DBNull.Value ? Convert.ToString(reader["CAPACIDADE_TANQUE"]) : "0",
                                    dataCadastro = reader["DATA_CADASTRO"] != DBNull.Value ? Convert.ToString(reader["DATA_CADASTRO"]) : "",
                                    codPool = reader["COD_POOL"] != DBNull.Value ? Convert.ToString(reader["COD_POOL"]) : "0",
                                    dataInicPrevCt = reader["INICPREV"] != DBNull.Value ? Convert.ToString(reader["INICPREV"]) : "",
                                    dataTermPrevCt = reader["TERMPREV"] != DBNull.Value ? Convert.ToString(reader["TERMPREV"]) : "",
                                    codCtOrigem = reader["CODCT_ORIGEM"] != DBNull.Value ? Convert.ToString(reader["CODCT_ORIGEM"]) : "-",
                                    validadeExtintor = reader["VALIDADE_EXTINTOR"] != DBNull.Value ? Convert.ToString(reader["VALIDADE_EXTINTOR"]) : "",
                                    tag = reader["TAG"] != DBNull.Value ? Convert.ToString(reader["TAG"]) : "N",
                                    anoIpva = reader["IPVA"] != DBNull.Value ? Convert.ToString(reader["IPVA"]) : "0",
                                    crv = reader["CRV"] != DBNull.Value ? Convert.ToString(reader["CRV"]) : "0",
                                    capacidadeCilindro = reader["CAPACIDADE_CILINDRO"] != DBNull.Value ? Convert.ToString(reader["CAPACIDADE_CILINDRO"]) : "0",
                                    codDepartamentoFrota = reader["COD_DEPARTAMENTO_FROTA"] != DBNull.Value ? Convert.ToString(reader["COD_DEPARTAMENTO_FROTA"]) : "0",
                                    codTarifaLocacao = reader["COD_TARIFA_LOCACAO"] != DBNull.Value ? Convert.ToString(reader["COD_TARIFA_LOCACAO"]) : "0",
                                    marca = reader["MARCA"] != DBNull.Value ? Convert.ToString(reader["MARCA"]) : "-",
                                    modelo = reader["MODELO"] != DBNull.Value ? Convert.ToString(reader["MODELO"]) : "-",
                                    descricaoSituacao = reader["DESCRICAO_SITUACAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_SITUACAO"]) : "-",
                                    condutor = reader["CONDUTOR"] != DBNull.Value ? Convert.ToString(reader["CONDUTOR"]) : "-",
                                    centroCusto = reader["CENTRO_CUSTO"] != DBNull.Value ? Convert.ToString(reader["CENTRO_CUSTO"]) : "-",
                                    categoria = reader["CATEGORIA"] != DBNull.Value ? Convert.ToString(reader["CATEGORIA"]) : "-",
                                    localGaragem = reader["LOCAL_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["LOCAL_GARAGEM"]) : "-",
                                    enderecoGaragem = reader["ENDERECO_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_GARAGEM"]) : "-",
                                    numeroGaragem = reader["NUMERO_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["NUMERO_GARAGEM"]) : "-",
                                    bairroGaragem = reader["BAIRRO_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["BAIRRO_GARAGEM"]) : "-",
                                    municipioGaragem = reader["MUNICIPIO_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["MUNICIPIO_GARAGEM"]) : "-",
                                    cepGaragem = reader["CEP_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["CEP_GARAGEM"]) : "-",
                                    ufGaragem = reader["UF_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["UF_GARAGEM"]) : "-",
                                    latitudeGaragem = reader["LATITUDE_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["LATITUDE_GARAGEM"]) : "-",
                                    longitudeGaragem = reader["LONGITUDE_GARAGEM"] != DBNull.Value ? Convert.ToString(reader["LONGITUDE_GARAGEM"]) : "-",
                                    tipoCombustivel = reader["TIPO_COMBUSTIVEL"] != DBNull.Value ? Convert.ToString(reader["TIPO_COMBUSTIVEL"]) : "-",
                                    pool = reader["POOL"] != DBNull.Value ? Convert.ToString(reader["POOL"]) : "-",
                                    empresa = reader["EMPRESA"] != DBNull.Value ? Convert.ToString(reader["EMPRESA"]) : "-",
                                    filial = reader["FILIAL"] != DBNull.Value ? Convert.ToString(reader["FILIAL"]) : "-",
                                    fornecedorLocadora = reader["FORNECEDOR_LOCADORA"] != DBNull.Value ? Convert.ToString(reader["FORNECEDOR_LOCADORA"]) : "-",
                                    departamentoFrota = reader["DEPARTAMENTO_FROTA"] != DBNull.Value ? Convert.ToString(reader["DEPARTAMENTO_FROTA"]) : "-",
                                    celulaRif = reader["CELULA_RIF"] != DBNull.Value ? Convert.ToString(reader["CELULA_RIF"]) : "-",
                                    kmContratado = reader["KM_CONTRATADO"] != DBNull.Value ? Convert.ToString(reader["KM_CONTRATADO"]) : "0",
                                    numCartao = reader["NUM_CARTAO"] != DBNull.Value ? Convert.ToString(reader["NUM_CARTAO"]) : "-",
                                    codTipoVeiculo = reader["COD_TIPO_VEICULO"] != DBNull.Value ? Convert.ToString(reader["COD_TIPO_VEICULO"]) : "0",
                                    tipoVeiculo = reader["TIPO_VEICULO"] != DBNull.Value ? Convert.ToString(reader["TIPO_VEICULO"]) : "-",
                                    cliente = reader["CLIENTE"] != DBNull.Value ? Convert.ToString(reader["CLIENTE"]) : "-"
                                });
                            }
                        }

                    }


                    return new ApiResponse<Veiculos>()
                    {
                        draw = request.settings.length,
                        data = retorno,
                        error = ""
                    };

                }
            }
            catch (Exception e)
            {
                return new ApiResponse<Veiculos>()
                {
                    draw = request.settings.length,
                    data = retorno,
                    error = e.Message
                };
            }
        }

    }
}