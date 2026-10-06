using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Integracoes.Geosite.Models;
using Aniel.Connect.Integracoes.Geosite.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.Hubsoft.ClienteModel;
using Aniel.Connect.Repository.Repository.HubSoft;
using Aniel.Connect.Repository.Repository.Log;
using Aniel.Connect.Repository.T5.Area;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ElementoRede;
using Aniel.Connect.Repository.T5.Localidade;
using Aniel.Connect.Repository.TipoImovel;
using Aniel.Connect.Repository.Voalle;
using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.nhibernate.mongo;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.IntegracoesERP.Ixc;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Elemento_RedeFotosArquivos = Sinapse.Model.Elemento_RedeFotosArquivos;
using Padrao_ABNT = Sinapse.Model.enumerados.Padrao_ABNT;
using Padrao_Cor = Sinapse.Model.enumerados.Padrao_Cor;
using Situacao_Porta = Sinapse.Model.enumerados.Situacao_Porta;

namespace Aniel.Connect.ControllersWebApi.T5
{

    [Authorize]
    public class Elemento_RedeApiController : BaseApiController, IElemento_RedeApi
    {
        [HttpPost]
        [Route("api/Elemento_RedeApi/VincularOltSlotPonEmMassa")]
        public async Task<PostApiRetorno<List<Elemento_Rede>>> VincularOltSlotPonEmMassa(ListaCaixaRequest value)
        {
            PostApiRetorno<List<Elemento_Rede>> retorno = new PostApiRetorno<List<Elemento_Rede>> { Ok = false };
            List<Elemento_Rede> updatedElements = new List<Elemento_Rede>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repo = new Conexao<Elemento_Rede>(sessionT5))
                {
                    foreach (var idCaixa in value.ListIdCaixa)
                    {
                        var obj = await repo.selectFirst(p => p.Id == idCaixa);

                        if (obj != null)
                        {
                            obj.Id_OLT = value.Id_OLT;
                            obj.Id_Slot = value.Id_Slot;
                            obj.Id_Pon = value.Id_Pon;

                            await repo.SaveOrUpdate(obj);
                            updatedElements.Add(obj);
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetAll")]

        #region GetAll
        public async Task<DataTableResponse<Elemento_RedeListaModel>> GetAll(Elemento_RedeDataTableRequest request)
        {
            List<Elemento_RedeListaModel> retorno = new List<Elemento_RedeListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    #region Filtro
                    var parametros = new Dictionary<String, Object>();
                    if (request.IdTipo.HasValue)
                    {
                        parametros.Add("@TIPO", request.IdTipo);
                    }
                    if (!String.IsNullOrEmpty(request.Certificado))
                    {
                        if (request.Certificado == "1")
                        {
                            parametros.Add("@CERTIFICADO", request.Certificado);
                        }
                        else if (request.Certificado == "0")
                        {
                            parametros.Add("@CERTIFICADO", request.Certificado);
                        }
                    }
                    if (request.soativos)
                    {
                        parametros.Add("@SOATIVOS", request.soativos);
                    }
                    if (request.ListaStatus.Count > 0)
                    {
                        var status = String.Join(",", request.ListaStatus.Select(p => $"'{p}'").ToList());
                        parametros.Add("@LISTA_STATUS", status);
                    }
                    if (request.Poste.HasValue)
                    {
                        parametros.Add("@POSTE", request.Poste);
                    }
                    if (request.Cabo.HasValue)
                    {
                        parametros.Add("@CABO", request.Cabo);
                    }
                    if (request.Caixa.HasValue)
                    {
                        parametros.Add("@CAIXA", request.Caixa);
                    }
                    if (String.IsNullOrEmpty(request.Contrato) == false)
                    {

                        parametros.Add("@CODCT", request.Contrato);
                    }
                    if (String.IsNullOrEmpty(request.Projeto) == false)
                    {
                        parametros.Add("@PROJETO", request.Projeto);
                    }
                    if (request.search != null && !String.IsNullOrEmpty(request.search.value))
                    {
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                    parametros.Add("@START", request.start);
                    parametros.Add("@LENGTH", request.length);
                    parametros.Add("@COUNT", false);

                    if (request.order != null && request.order.Any())
                    {
                        parametros.Add("@ORDER_COL", request.order[0].column);
                        parametros.Add("@ORDER_DIR", request.order[0].dir);
                    }
                    else
                    {
                        parametros.Add("@ORDER_COL", null);
                        parametros.Add("@ORDER_DIR", null);
                    }

                    #endregion
                    using (var com = session.Connection.CreateCommand())
                    {
                        parametros["@COUNT"] = true;

                        using (var readerCount = await this.sqlGetAll(session, com, parametros))
                        {
                            if (await readerCount.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(readerCount["IDS"]);
                            }
                        }
                    }
                    using (var com = session.Connection.CreateCommand())
                    {
                        parametros["@COUNT"] = false;

                        using (var reader = await this.sqlGetAll(session, com, parametros, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {

                                retorno.Add(new Elemento_RedeListaModel
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "-",
                                    Certificado = Convert.ToString(reader["CERTIFICADO"]) == "1" ? RsBase.SIM : RsBase.NAO,
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",
                                    Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE"]) : 0,
                                    Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE"]) : 0,

                                    Situacao = reader["SITUACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Elemento>(Convert.ToString(reader["SITUACAO"])) : "-",
                                    Tipo = reader["ID_TIPO"] != DBNull.Value ? Convert.ToString(reader["ID_TIPO"]) : "-",
                                    Cor = reader["COR_LINHA_ATIVA"] != DBNull.Value ? Convert.ToString(reader["COR_LINHA_ATIVA"]) : "-",
                                    Icone = "-",
                                    Descricao_Modelo = reader["DESCRICAO_MODELO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_MODELO"]) : "-",

                                    Descricao_Tipo = reader["DESCRICAO_TIPO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_TIPO"]) : "-",
                                    Contrato = reader["DESC_CONTRATO"] != DBNull.Value ? $"{Convert.ToString(reader["CONTRATO"])} - {Convert.ToString(reader["DESC_CONTRATO"])}" : null,
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Cabo = Convert.ToString(reader["DESC_CABO"]),
                                    Poste = Convert.ToString(reader["DESC_POSTE"]),

                                    Cod_Contrato = Convert.ToString(reader["CONTRATO"]),
                                    OLT = Convert.ToString(reader["OLT"]),
                                    Slot = Convert.ToString(reader["SLOT"]),
                                    Pon = Convert.ToString(reader["PON"]),
                                    POP = Convert.ToString(reader["POP"]),
                                    Nos = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]).Split('/')[0] : String.Empty,
                                    UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                    DataCriacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                    NumDoc = Convert.ToString(reader["NUM_DOC"]),
                                    CodCT = Convert.ToString(reader["CONTRATO"]),
                                    UsuarioCertificacao = reader["USUARIO_CERTIFICOU"] != DBNull.Value ? Convert.ToString(reader["USUARIO_CERTIFICOU"]) : "",
                                    DataCertificacao = reader["DATAHORA_CERTIFICOU"] != DBNull.Value ? Convert.ToString(reader["DATAHORA_CERTIFICOU"]) : "",
                                    IdIntegrador = Convert.ToString(reader["ID_INTEGRADOR"]),
                                });
                            }
                        }
                    }


                    return new DataTableResponse<Elemento_RedeListaModel>()
                    {
                        data = retorno,
                        draw = request.draw,
                        error = "",
                        recordsFiltered = recordsTotal,

                    };
                }

                catch (Exception ex)
                {
                    return new DataTableResponse<Elemento_RedeListaModel>()
                    {

                        error = ex.Message,

                    };
                }
            }
        }
        #endregion

        private async Task<System.Data.Common.DbDataReader> sqlGetAll(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters, int length = 0, int? start = null)
        {
            #region sql_GetAll
            System.Data.Common.DbDataReader reader = null;
            try
            {
                #region Filtro                
                List<String> condicoes = new List<String>();

                bool Count = Convert.ToBoolean(Filters["@COUNT"]);
                //Adicionando termos de busca aos parametros 
                Filters.ForEach(request =>
                {

                    switch (request.Key)
                    {

                        case "@TIPO":
                            condicoes.Add($"A.ID_TIPO = {request.Key}");
                            break;

                        case "@SOATIVOS":
                            condicoes.Add($"COALESCE(A.SITUACAO,0) <> 0");
                            break;

                        case "@CERTIFICADO":
                            condicoes.Add($"COALESCE(A.CERTIFICADO,0) = {request.Key}");
                            break;

                        case "@LISTA_STATUS":
                            condicoes.Add($"A.SITUACAO IN ({request.Value})");
                            break;

                        case "@POSTE":
                            condicoes.Add($"A.ID_POSTE = {request.Key}");
                            break;

                        case "@CABO":
                            condicoes.Add($"A.ID_CABO = {request.Key}");
                            break;

                        case "@CAIXA":
                            condicoes.Add($"A.ID_CAIXA = {request.Key}");
                            break;

                        case "@CODCT":
                            condicoes.Add($"A.CONTRATO = {request.Key}");
                            break;

                        case "@PROJETO":
                            condicoes.Add($"A.PROJETO = {request.Key}");
                            break;

                        case "@SEARCH":
                            condicoes.Add($"UPPER(A.DESCRICAO) LIKE {request.Key.ToUpperInvariant()}");
                            break;

                        default:
                            break;
                    }
                });
                #endregion

                StringBuilder sqlWhere = new StringBuilder(),
                               sqlFrom = new StringBuilder(),
                               sql = new StringBuilder();

                if (condicoes.Count > 0)
                {
                    sqlWhere.AppendLine((" WHERE " + String.Join(" AND ", condicoes)));
                }

                sqlFrom.AppendLine("FROM TB_ELEMENTO_REDE A");

                if (Count)
                {
                    sql.AppendFormat("SELECT COUNT (A.ID) IDS \r\n{0} \r\n {1}", sqlFrom, sqlWhere);
                }
                else
                {
                    sqlFrom.AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE B")
                        .AppendLine(" ON A.ID_TIPO=B.ID")
                        .AppendLine(" LEFT JOIN TB_MODELO_ELEMENTO_REDE C")
                        .AppendLine(" ON B.ID_MODELO=C.ID")
                        .AppendLine(" LEFT JOIN TB_CONTRATO D")
                        .AppendLine(" ON D.CODCT = A.CONTRATO")
                        .AppendLine(" LEFT JOIN TB_PROJETO E")
                        .AppendLine(" ON E.PROJETO = A.PROJETO AND E.CODCT = D.CODCT")
                        .AppendLine(" LEFT JOIN TB_ELEMENTO_REDE F")
                        .AppendLine(" ON F.ID = A.ID_POSTE")
                        .AppendLine(" LEFT JOIN TB_ELEMENTO_REDE G")
                        .AppendLine(" ON G.ID = A.ID_CABO");

                if (Count)
                {
                    sql.AppendFormat("SELECT COUNT (A.ID) IDS \r\n{0} \r\n {1}", sqlFrom, sqlWhere);
                }
                else
                {
                        sql.AppendLine(" SELECT A.ID_INTEGRADOR,A.ID,A.BAIRRO, A.CEP, A.CERTIFICADO, A.CIDADE,")
                           .AppendLine(" A.COMPLEMENTO, A.CONTRATO,A.DESCRICAO, A.ENDERECO, A.POP,")
                           .AppendLine(" A.OLT, A.SLOT, A.PON, A.LATITUDE, A.LONGITUDE, A.NUMERO, A.OBS, A.PROJETO,")
                           .AppendLine(" A.SITUACAO, A.ID_TIPO, A.UF, A.COR_LINHA_ATIVA, B.ID, B.DESCRICAO AS DESCRICAO_TIPO,")
                           .AppendLine(" B.ID_MODELO, C.ID, C.DESCRICAO AS DESCRICAO_MODELO, D.DESCRICAO AS DESC_CONTRATO,E.PROJETO,")
                           .AppendLine(" A.NUM_OBRA, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.NUM_DOC, A.CONTRATO, F.DESCRICAO AS DESC_POSTE,")
                           .AppendFormat(" G.DESCRICAO AS DESC_CABO, A.USUARIO_CERTIFICOU, A.DATAHORA_CERTIFICOU \r\n{0} \r\n{1}", sqlFrom, sqlWhere);

                        #region Ordenação
                        if (Filters["@ORDER_COL"] != null)
                        {
                            int coluna = Convert.ToInt32(Filters["@ORDER_COL"]);
                            string ordenacao = Filters["@ORDER_DIR"].ToString();

                            switch (coluna)
                            {
                                case 1:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.ID ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.ID DESC ");
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.DESCRICAO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.DESCRICAO DESC ");
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.CERTIFICADO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.CERTIFICADO DESC ");
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY B.DESCRICAO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY B.DESCRICAO DESC ");
                                        }
                                        break;
                                    }
                                case 5:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.LATITUDE ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.LATITUDE DESC ");
                                        }
                                        break;
                                    }
                                case 6:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.LONGITUDE ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.LONGITUDE DESC ");
                                        }
                                        break;
                                    }
                                case 7:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.SITUACAO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.SITUACAO DESC ");
                                        }
                                        break;
                                    }
                                case 8:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.CONTRATO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.CONTRATO DESC ");
                                        }
                                        break;
                                    }
                                case 9:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.PROJETO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.PROJETO DESC ");
                                        }
                                        break;
                                    }
                                case 10:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY G.DESCRICAO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY G.DESCRICAO DESC ");
                                        }
                                        break;
                                    }
                                case 11:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY F.DESCRICAO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY F.DESCRICAO DESC ");
                                        }
                                        break;
                                    }
                                case 13:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.USUARIOCRIACAO ");
                            }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.USUARIOCRIACAO DESC ");
                        }
                                        break;
                                    }
                                case 14:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.DATAHORACRIACAO ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.DATAHORACRIACAO DESC ");
                                        }
                                        break;
                                    }

                                case 15:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.USUARIO_CERTIFICOU ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.USUARIO_CERTIFICOU DESC ");
                                        }
                                        break;
                                    }
                                case 16:
                                    {
                                        if (ordenacao == "asc")
                                        {
                                            sql.AppendLine(" ORDER BY A.DATAHORA_CERTIFICOU ");
                                        }
                                        else
                                        {
                                            sql.AppendLine(" ORDER BY A.DATAHORA_CERTIFICOU DESC ");
                                        }
                                        break;
                                    }
                            }
                        }
                        #endregion
                }
                    }
                if (Count)
                {
                    reader = await CommandHelper.ExecuteReader(comm, sql, Filters, session.Connection, null);
                }
                else
                {
                    reader = await CommandHelper.ExecuteReaderFirst(comm, sql, Filters, session.Connection, null, length, start);
                }
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }
            return reader;
            #endregion
        }

        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetCombo")]
        #region GetCombo
        public async Task<Elemento_ObjCombo<Int64>> GetCombo(RequestObjComboConfig<String, RequestElementoComboConfig> request)
        {
            Elemento_ObjCombo<Int64> retorno = new Elemento_ObjCombo<Int64>
            {
                results = new List<Elemento_ObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    parametros.Add("@PAGE", request.page);

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }
                    if (request.config.Cabo)
                    {
                        //condicoes.Add("C.ID = 5");
                        parametros.Add("@CABO", "C.ID = 5");
                    }
                    if (request.config.Poste)
                    {
                        //condicoes.Add("C.ID = 6");
                        parametros.Add("@POSTE", "C.ID = 6");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        //condicoes.Add($"A.ID = '{request.id}'");
                        parametros.Add("@ID", request.id);
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        //condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                        parametros.Add("@SEARCH", StringService.Truncate(request.q.ToUpperInvariant(), 100));

                    }
                    parametros.Add("@ORDER", request.config.Reverse);
                    parametros.Add("@COUNT", false);

                    using (var com = session.Connection.CreateCommand())
                    {
                        parametros["@COUNT"] = true;
                        using (var reader = await sqlGetCombo(session, com, parametros))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        parametros["@COUNT"] = false;
                        using (var reader = await sqlGetCombo(session, com, parametros))
                        {
                            while (await reader.ReadAsync())
                            {
                                var modelo = Convert.ToString(reader["DESCRICAO_MODELO"]);
                                var obj = new Elemento_ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Contrato = reader["CONTRATO"] != DBNull.Value ? Convert.ToString(reader["CONTRATO"]) : null,
                                    Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : null,
                                    Pop = reader["POP"] != DBNull.Value ? Convert.ToString(reader["POP"]) : Convert.ToString(request.config.id_pop),
                                    Olt = reader["OLT"] != DBNull.Value ? Convert.ToString(reader["OLT"]) : Convert.ToString(request.config.id_olt),
                                    Slot = reader["SLOT"] != DBNull.Value ? Convert.ToString(reader["SLOT"]) : Convert.ToString(request.config.id_slot),
                                    Pon = reader["PON"] != DBNull.Value ? Convert.ToString(reader["PON"]) : Convert.ToString(request.config.id_pon),

                                    //Implementação no Mapa de Gestao de Rede
                                    Text = Convert.ToString(reader["DESCRICAO"]).Length == 0 ? $"- No Description -Tipo {Convert.ToString(reader["DESCRICAO_TIPO"])}" :
                                    Convert.ToString(reader["DESCRICAO"]),
                                    Latitude = Convert.ToString(reader["LATITUDE"]),
                                    Longitude = Convert.ToString(reader["LONGITUDE"]),

                                    Modelo = modelo,
                                    Id_Modelo = reader["ID_MODELO"] != DBNull.Value ? Convert.ToInt64(reader["ID_MODELO"]) :  (long?)null,
                                    Id_Tipo = reader["ID_TIPO"] != DBNull.Value ? Convert.ToInt64(reader["ID_TIPO"]) : (long?)null,
                                    Tipo = Convert.ToString(reader["DESCRICAO_TIPO"])

                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var mensagem = ex.Message;
                }
            }
            return retorno;
        }

        private async Task<System.Data.Common.DbDataReader> sqlGetCombo(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters)
        {
            #region SqlGetCombo
            System.Data.Common.DbDataReader reader = null;
            try
            {

                var filterOrder = Convert.ToBoolean(Filters["@ORDER"]);
                var filterCount = Convert.ToBoolean(Filters["@COUNT"]);

                List<String> condicoes = new List<String>();

                Filters.ForEach(request =>
                {
                    switch (request.Key)
                    {
                        case "@POSTE":
                            condicoes.Add($"C.ID = {request.Value}");
                            break;

                        case "@CABO":
                            condicoes.Add($"C.ID = {request.Value}");
                            break;

                        case "@ID_POP":
                            condicoes.Add($"{request.Value}");
                            break;

                        case "@ID":
                            condicoes.Add($"A.ID = {request.Value}");
                            break;

                        case "@SEARCH":
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{request.Value}%')");
                            break;

                        default:
                            break;
                    }

                });

                StringBuilder SqlFrom = new StringBuilder(),
                              SqlWhere = new StringBuilder(),
                              SqlOrder = new StringBuilder(),
                              Sql = new StringBuilder();

                if (condicoes.Count > 0)
                {
                    SqlWhere.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                }

                SqlFrom.AppendLine("FROM TB_ELEMENTO_REDE A");

                if (filterCount)
                {
                    Sql.AppendLine("SELECT COUNT(A.ID) AS CO");
                }

                else
                {
                    SqlFrom.AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE B ON (A.ID_TIPO = B.ID)\r\n ")
                           .AppendLine("LEFT JOIN TB_MODELO_ELEMENTO_REDE C ON (B.ID_MODELO = C.ID) \r\n");

                    Sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.CONTRATO, A.PROJETO, A.POP, A.OLT,A.LATITUDE, A.LONGITUDE, A.SLOT, A.PON, ")
                       .AppendLine(" B.ID ID_TIPO, B.DESCRICAO DESCRICAO_TIPO,C.ID ID_MODELO, C.DESCRICAO DESCRICAO_MODELO ");

                    if (filterOrder)
                    { SqlOrder.AppendLine("ORDER BY UPPER(A.ID) DESC"); }

                    else
                    {
                        SqlOrder.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                    }
                }

                Sql.AppendFormat("{0}\r\n{1}\r\n{2}\r\n", SqlFrom, SqlWhere, SqlOrder);

                if (filterCount)
                {
                    reader = await CommandHelper.ExecuteReader(comm, Sql, Filters, session.Connection, null);
                }
                else 
                {
                    reader = await CommandHelper.ExecuteReaderPage(comm, Sql, Filters, session.Connection, null, Convert.ToInt32(Filters["@PAGE"]));
                }
               
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return reader;
            #endregion
        }


        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetComboPoste")]

        public async Task<Elemento_ObjCombo<Int64>> GetComboPoste(RequestObjComboConfig<String, RequestElementoComboConfig> request)
        {
            Elemento_ObjCombo<Int64> retorno = new Elemento_ObjCombo<Int64>
            {
                results = new List<Elemento_ObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }
                    parametros.Add("@ID_MODELO", "B.ID_MODELO = 5");


                    if (!String.IsNullOrEmpty(request.id))
                    {

                        parametros.Add("@ID", $"A.ID = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {

                        parametros.Add("@SEARCH", $"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    parametros.Add("@PAGE", request.page);
                    parametros.Add("@ORDER", request.config.Reverse);
                    parametros.Add("@COUNT", false);

                    using (var com = session.Connection.CreateCommand())
                    {

                        parametros["@COUNT"] = true;

                        using (var reader = await sqlGetComboPoste(session, com, parametros))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        parametros["@COUNT"] = false;

                        using (var reader = await sqlGetComboPoste(session, com, parametros))
                        {
                            while (await reader.ReadAsync())
                            {
                                var modelo = Convert.ToString(reader["DESCRICAO_MODELO"]);
                                var obj = new Elemento_ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Contrato = Convert.ToString(reader["CONTRATO"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Pop = Convert.ToString(reader["POP"]),
                                    Olt = Convert.ToString(reader["OLT"]),
                                    //Implementação no Mapa de Gestao de Rede
                                    Text = Convert.ToString(reader["DESCRICAO"]).Length == 0 ? $"- No Description -Tipo {Convert.ToString(reader["Descricao_Tipo"])}" :
                                    Convert.ToString(reader["DESCRICAO"]),
                                    Latitude = Convert.ToString(reader["LATITUDE"]),
                                    Longitude = Convert.ToString(reader["LONGITUDE"]),
                                    Modelo = modelo,


                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //                                      *******      Pro Tip     *******
                    //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
                }
            }
            return retorno;
        }

        private async Task<System.Data.Common.DbDataReader> sqlGetComboPoste(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters)
        {
            #region SqlGetCombo
            System.Data.Common.DbDataReader reader = null;
            try
            {

                var filterOrder = Convert.ToBoolean(Filters["@ORDER"]);
                var filterCount = Convert.ToBoolean(Filters["@COUNT"]);

                List<String> condicoes = new List<String>();

                Filters.ForEach(request =>
                {
                    switch (request.Key)
                    {
                        case "@POSTE":
                            condicoes.Add($"C.ID = {request.Key}");
                            break;

                        case "@CABO":
                            condicoes.Add($"C.ID = {request.Key}");
                            break;

                        case "@ID":
                            condicoes.Add($"A.ID = {request.Key}");
                            break;

                        case "@SEARCH":
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{request.Key}%')");
                            break;

                        default:
                            break;
                    }

                });

                StringBuilder SqlFrom = new StringBuilder(),
                              SqlWhere = new StringBuilder(),
                              SqlOrder = new StringBuilder(),
                              Sql = new StringBuilder();

                if (condicoes.Count > 0)
                {
                    SqlWhere.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                }

                SqlFrom.AppendLine("FROM TB_ELEMENTO_REDE A");

                if (filterCount)
                {
                    Sql.AppendLine("SELECT COUNT(A.ID) AS CO");
                }

                else
                {
                    SqlFrom.AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE B ON (A.ID_TIPO = B.ID)\r\n ")
                           .AppendLine("LEFT JOIN TB_MODELO_ELEMENTO_REDE C ON (B.ID_MODELO = C.ID) \r\n");

                    Sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.CONTRATO, A.PROJETO, A.POP, A.OLT,A.LATITUDE, A.LONGITUDE,")
                       .AppendLine(" B.ID ID_TIPO, B.DESCRICAO DESCRICAO_TIPO,C.ID ID_MODELO, C.DESCRICAO DESCRICAO_MODELO ");

                    if (filterOrder)
                    { SqlOrder.AppendLine("ORDER BY UPPER(A.ID) DESC"); }

                    else
                    {
                        SqlOrder.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                    }
                }

                Sql.AppendFormat("{0}\r\n{1}\r\n{2}\r\n", SqlFrom, SqlWhere, SqlOrder);

                reader = await CommandHelper.ExecuteReaderPage(comm, Sql, Filters, session.Connection, null, Convert.ToInt32(Filters["@PAGE"]));
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return reader;
            #endregion
        }

        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetComboCabo")]

        public async Task<Elemento_ObjCombo<Int64>> GetComboCabo(RequestObjComboConfig<String, RequestElementoComboConfig> request)
        {
            Elemento_ObjCombo<Int64> retorno = new Elemento_ObjCombo<Int64>
            {
                results = new List<Elemento_ObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(@"FROM TB_ELEMENTO_REDE A
                                                                LEFT JOIN TB_TIPO_ELEMENTO_REDE B ON (A.ID_TIPO = B.ID)");

                    StringBuilder sqlWhere = new StringBuilder();

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }

                    condicoes.Add($"B.ID_MODELO = 6");

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }



                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.DESCRICAO, A.CONTRATO, A.PROJETO, A.POP, A.OLT,
                                                                A.LATITUDE, A.LONGITUDE, B.ID_MODELO, B.DESCRICAO DESCRICAO_TIPO,
                                                                C.DESCRICAO DESCRICAO_MODELO ");



                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());

                        if (request.config.Reverse)
                        { sql.AppendLine("ORDER BY UPPER(A.ID) DESC"); }

                        else
                        {
                            sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                        }

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var modelo = Convert.ToString(reader["DESCRICAO_MODELO"]);
                                var obj = new Elemento_ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Contrato = Convert.ToString(reader["CONTRATO"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Pop = Convert.ToString(reader["POP"]),
                                    Olt = Convert.ToString(reader["OLT"]),
                                    //Implementação no Mapa de Gestao de Rede
                                    Text = Convert.ToString(reader["DESCRICAO"]).Length == 0 ? $"- No Description -Tipo {Convert.ToString(reader["Descricao_Tipo"])}" :
                                    Convert.ToString(reader["DESCRICAO"]),
                                    Latitude = Convert.ToString(reader["LATITUDE"]),
                                    Longitude = Convert.ToString(reader["LONGITUDE"]),
                                    Modelo = modelo,

                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //                                      *******      Pro Tip     *******
                    //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
                }
            }
            return retorno;
        }
        #endregion

        private async Task<System.Data.Common.DbDataReader> sqlGetComboCabo(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters)
        {
            #region SqlGetCombo
            System.Data.Common.DbDataReader reader = null;
            try
            {

                var filterOrder = Convert.ToBoolean(Filters["@ORDER"]);
                var filterCount = Convert.ToBoolean(Filters["@COUNT"]);

                List<String> condicoes = new List<String>();

                Filters.ForEach(request =>
                {
                    switch (request.Key)
                    {
                        case "@POSTE":
                            condicoes.Add($"C.ID = {request.Key}");
                            break;

                        case "@CABO":
                            condicoes.Add($"C.ID = {request.Key}");
                            break;

                        case "@ID":
                            condicoes.Add($"A.ID = {request.Key}");
                            break;

                        case "@SEARCH":
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{request.Key}%')");
                            break;

                        default:
                            break;
                    }

                });

                StringBuilder SqlFrom = new StringBuilder(),
                              SqlWhere = new StringBuilder(),
                              SqlOrder = new StringBuilder(),
                              Sql = new StringBuilder();

                if (condicoes.Count > 0)
                {
                    SqlWhere.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                }

                SqlFrom.AppendLine("FROM TB_ELEMENTO_REDE A");

                if (filterCount)
                {
                    Sql.AppendLine("SELECT COUNT(A.ID) AS CO");
                }

                else
                {
                    SqlFrom.AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE B ON (A.ID_TIPO = B.ID)\r\n ")
                           .AppendLine("LEFT JOIN TB_MODELO_ELEMENTO_REDE C ON (B.ID_MODELO = C.ID) \r\n");

                    Sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.CONTRATO, A.PROJETO, A.POP, A.OLT,A.LATITUDE, A.LONGITUDE,")
                       .AppendLine(" B.ID ID_TIPO, B.DESCRICAO DESCRICAO_TIPO,C.ID ID_MODELO, C.DESCRICAO DESCRICAO_MODELO ");

                    if (filterOrder)
                    { SqlOrder.AppendLine("ORDER BY UPPER(A.ID) DESC"); }

                    else
                    {
                        SqlOrder.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                    }
                }

                Sql.AppendFormat("{0}\r\n{1}\r\n{2}\r\n", SqlFrom, SqlWhere, SqlOrder);

                reader = await CommandHelper.ExecuteReaderPage(comm, Sql, Filters, session.Connection, null, Convert.ToInt32(Filters["@PAGE"]));
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return reader;
            #endregion
        }

        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetComboCaixa")]
        #region GetCombo
        public async Task<Elemento_ObjCombo<Int64>> GetComboCaixa(RequestObjComboConfig<String, RequestElementoComboConfig> request)
        {
            Elemento_ObjCombo<Int64> retorno = new Elemento_ObjCombo<Int64>
            {
                results = new List<Elemento_ObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ELEMENTO_REDE A ");
                    sqlFrom.AppendLine("  LEFT JOIN TB_TIPO_ELEMENTO_REDE B ")
                      .AppendLine(" ON (A.ID_TIPO = B.ID) ")
                      .AppendLine(" LEFT JOIN TB_MODELO_ELEMENTO_REDE C ON (B.ID_MODELO = C.ID) ");


                    StringBuilder sqlWhere = new StringBuilder();

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"COALESCE(A.SITUACAO,0) <> 0");
                    }
                    //***************************
                    //  7 = Caixa de Atendimento
                    //      4 = Caixa de Emenda
                    //***************************

                    condicoes.Add("(B.ID_MODELO = 4 OR B.ID_MODELO = 7)");

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (request.config.id_olt.HasValue)
                    {
                        condicoes.Add($"A.OLT = '{request.config.id_olt}'");
                    }

                    if (request.config.id_pop.HasValue)
                    {
                        condicoes.Add($"A.POP = '{request.config.id_pop}'");
                    }


                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());

                        using (
                            var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.DESCRICAO, A.CONTRATO, A.PROJETO, A.POP, A.OLT,
                                                                A.LATITUDE, A.LONGITUDE, B.ID ID_TIPO, B.ID_MODELO, B.DESCRICAO DESCRICAO_TIPO,
                                                                C.DESCRICAO DESCRICAO_MODELO ");
                      
                        sql.AppendLine(sqlFrom.ToString());

                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(sqlWhere.ToString());
                        }
                        if (request.config.Reverse)
                        { sql.AppendLine("ORDER BY UPPER(A.ID) DESC"); }

                        else
                        {
                            sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                        }

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var modelo = Convert.ToString(reader["DESCRICAO_MODELO"]);
                                var obj = new Elemento_ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Contrato = Convert.ToString(reader["CONTRATO"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Pop = Convert.ToString(reader["POP"]),
                                    Olt = Convert.ToString(reader["OLT"]),

                                    Id_Tipo = Convert.ToInt64(reader["ID_TIPO"]),
                                    Tipo = Convert.ToString(reader["DESCRICAO_TIPO"]),

                                    //Implementação no Mapa de Gestao de Rede
                                    Text = Convert.ToString(reader["DESCRICAO"]).Length == 0 ? $"- No Description -Tipo {Convert.ToString(reader["Descricao_Tipo"])}" :
                                    Convert.ToString(reader["DESCRICAO"]),
                                    Latitude = Convert.ToString(reader["LATITUDE"]),
                                    Longitude = Convert.ToString(reader["LONGITUDE"]),
                                    Modelo = modelo,

                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //                                      *******      Pro Tip     *******
                    //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
                }
            }
            return retorno;
        }
        #endregion
       
        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetComboCaixaAt")]
        #region GetCombo
        public async Task<Elemento_ObjCombo<Int64>> GetComboCaixaAt(RequestObjComboConfig<String, RequestElementoComboConfig> request)
        {
            Elemento_ObjCombo<Int64> retorno = new Elemento_ObjCombo<Int64>
            {
                results = new List<Elemento_ObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ELEMENTO_REDE A ");

                    StringBuilder sqlWhere = new StringBuilder();

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.SITUACAO = 1");
                    }
                    //***************************
                    //  7 = Caixa de Atendimento
                    //      4 = Caixa de Emenda
                    //***************************

                    condicoes.Add("(B.ID_MODELO = 4 OR B.ID_MODELO = 7)");

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.DESCRICAO, A.CONTRATO, A.PROJETO, A.POP, A.OLT,
                                                                A.LATITUDE, A.LONGITUDE, B.ID ID_TIPO, B.ID_MODELO, B.DESCRICAO DESCRICAO_TIPO,
                                                                C.DESCRICAO DESCRICAO_MODELO ");

                        sqlFrom.AppendLine("  LEFT JOIN TB_TIPO_ELEMENTO_REDE B ")
                        .AppendLine(" ON (A.ID_TIPO = B.ID) ")
                        .AppendLine(" LEFT JOIN TB_MODELO_ELEMENTO_REDE C ON (B.ID_MODELO = C.ID) ");

                        sql.AppendLine(sqlFrom.ToString());

                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(sqlWhere.ToString());
                        }
                        if (request.config.Reverse)
                        { sql.AppendLine("ORDER BY UPPER(A.ID) DESC"); }

                        else
                        {
                            sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                        }

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var modelo = Convert.ToString(reader["DESCRICAO_MODELO"]);
                                var obj = new Elemento_ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Contrato = Convert.ToString(reader["CONTRATO"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Pop = Convert.ToString(reader["POP"]),
                                    Olt = Convert.ToString(reader["OLT"]),

                                    Id_Tipo = Convert.ToInt64(reader["ID_TIPO"]),
                                    Tipo = Convert.ToString(reader["DESCRICAO_TIPO"]),

                                    //Implementação no Mapa de Gestao de Rede
                                    Text = Convert.ToString(reader["DESCRICAO"]).Length == 0 ? $"- No Description -Tipo {Convert.ToString(reader["Descricao_Tipo"])}" :
                                    Convert.ToString(reader["DESCRICAO"]),
                                    Latitude = Convert.ToString(reader["LATITUDE"]),
                                    Longitude = Convert.ToString(reader["LONGITUDE"]),
                                    Modelo = modelo,

                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //                                      *******      Pro Tip     *******
                    //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
                }
            }
            return retorno;
        }
        #endregion

        private async Task<System.Data.Common.DbDataReader> sqlGetComboCaixa(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters)
        {
            #region SqlGetComboCaixa
            System.Data.Common.DbDataReader reader = null;
            try
            {

                var filterOrder = Convert.ToBoolean(Filters["@ORDER"]);
                var filterCount = Convert.ToBoolean(Filters["@COUNT"]);

                List<String> condicoes = new List<String>();

                Filters.ForEach(request =>
                {
                    switch (request.Key)
                    {
                        case "@POSTE":
                            condicoes.Add($"C.ID = {request.Key}");
                            break;

                        case "@CABO":
                            condicoes.Add($"C.ID = {request.Key}");
                            break;

                        case "@ID":
                            condicoes.Add($"A.ID = {request.Key}");
                            break;

                        case "@SEARCH":
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{request.Key}%')");
                            break;

                        default:
                            break;
                    }

                });

                StringBuilder SqlFrom = new StringBuilder(),
                              SqlWhere = new StringBuilder(),
                              SqlOrder = new StringBuilder(),
                              Sql = new StringBuilder();

                if (condicoes.Count > 0)
                {
                    SqlWhere.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                }

                SqlFrom.AppendLine("FROM TB_ELEMENTO_REDE A");

                if (filterCount)
                {
                    Sql.AppendLine("SELECT COUNT(A.ID) AS CO");
                }

                else
                {
                    SqlFrom.AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE B ON (A.ID_TIPO = B.ID)\r\n ")
                           .AppendLine("LEFT JOIN TB_MODELO_ELEMENTO_REDE C ON (B.ID_MODELO = C.ID) \r\n");

                    Sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.CONTRATO, A.PROJETO, A.POP, A.OLT,A.LATITUDE, A.LONGITUDE,")
                       .AppendLine(" B.ID ID_TIPO, B.DESCRICAO DESCRICAO_TIPO,C.ID ID_MODELO, C.DESCRICAO DESCRICAO_MODELO ");

                    if (filterOrder)
                    { SqlOrder.AppendLine("ORDER BY UPPER(A.ID) DESC"); }

                    else
                    {
                        SqlOrder.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                    }
                }

                Sql.AppendFormat("{0}\r\n{1}\r\n{2}\r\n", SqlFrom, SqlWhere, SqlOrder);

                reader = await CommandHelper.ExecuteReaderPage(comm, Sql, Filters, session.Connection, null, Convert.ToInt32(Filters["@PAGE"]));
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return reader;
            #endregion
        }

        [HttpGet()]
        [Route("api/Elemento_RedeApi/GetTreeView_Gestao")]
        #region GetTreeView_Gestao
        public async Task<GetApiRetorno<List<Elemento_RedeTreeViewModel>>> GetTreeView_Gestao(String Text_Reference = null)
        {
            GetApiRetorno<List<Elemento_RedeTreeViewModel>> retorno = new GetApiRetorno<List<Elemento_RedeTreeViewModel>>
            {
                Objeto = new List<Elemento_RedeTreeViewModel>()
            };

            var ObjArea = new List<Elemento_RedeTreeViewModel>();

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            try
            {

                List<Elemento_RedeGestaoViewModel> listamanutencaoObject = new List<Elemento_RedeGestaoViewModel>();

                /*             
                =-=-=-          Repository Usage          =-=-=-                   
                 */

                using (var conexao = new ConexaoMongoGeography())
                using (var repoAreas = new RepositoryMongo<Area_CoberturaMongo>(conexao))
                using (var session = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        #region Area_Cobertura
                        List<Area_CoberturaMongo> bancoarea = new List<Area_CoberturaMongo>();
                        if (!String.IsNullOrEmpty(Text_Reference))
                        {
                            bancoarea = await repoAreas.FindAll(p => p.Descricao == Text_Reference);
                        }
                        else
                        {
                            bancoarea = await repoAreas.FindAll();
                        }

                        foreach (var area in bancoarea)
                        {
                            ObjArea.Add(new Elemento_RedeTreeViewModel
                            {
                                Id = "Area Cobertura" + "-" + area.Id,
                                Text = "Area Cobertura: " + area.Descricao,
                            });
                        }

                        #endregion

                        StringBuilder sql = new StringBuilder();

                        parametros.Add("@HAS_REFERENCE", !String.IsNullOrEmpty(Text_Reference));

                        if ((bool)parametros["@HAS_REFERENCE"])
                        {
                            parametros.Add("@TEXT_REFERENCE", Text_Reference);
                        }

                        using (var reader = await sqlGetTreeviewGestao(session, com, parametros))
                        {
                            while (await reader.ReadAsync())
                            {

                                String codct = Convert.ToString(reader["CODCT"]).Replace("-", String.Empty) + "-" + Convert.ToString(reader["DESCRICAO"]).Replace("-", String.Empty);
                                String projeto = Convert.ToString(reader["ID_PROJETO"]).Replace("-", String.Empty);
                                String tipο = Convert.ToString(reader["ID_TIPO"]);
                                String elemento = Convert.ToString(reader["ID_ELEMENTO"]);
                                String descricaoCodct = "Contrato: " + (String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])) ? "Sem Contrato" : Convert.ToString(reader["DESCRICAO"]));
                                String descricaoProjeto = "Projeto: " + (String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO_PROJETO"])) ? "Sem Projeto" : Convert.ToString(reader["DESCRICAO_PROJETO"]));
                                String descricaoTipo = "Tipo: " + Convert.ToString(reader["DESCRICAO_TIPO"]);
                                String descricaoElemento = "Elemento: " + Convert.ToString(reader["DESCRICAO_ELEMENTO"]);

                                Elemento_RedeGestaoViewModel manutencaoObject = new Elemento_RedeGestaoViewModel
                                {
                                    CodCt = codct,
                                    Elemento = elemento,
                                    Tipo = tipο,
                                    DescricaoCodCt = descricaoCodct,
                                    DescricaoTipo = descricaoTipo,
                                    DescricaoElemento = descricaoElemento,
                                    DescricaoProjeto = descricaoProjeto,
                                    Projeto = projeto

                                };

                                listamanutencaoObject.Add(manutencaoObject);

                                //Obj.Children.Select(p => p.Id == Obj2.Id);                                
                                //retorno.Objeto.Add(Obj4);
                            }
                        }

                        var agrupamento = listamanutencaoObject.GroupBy(p => new { p.CodCt, p.DescricaoCodCt });

                        foreach (var item in agrupamento)
                        {
                            List<Elemento_RedeGestaoViewModel> objetosDoAgrupamento = item.ToList();
                            List<Elemento_RedeTreeViewModel> ListaAgrupamentoTipo = new List<Elemento_RedeTreeViewModel>();
                            List<Elemento_RedeTreeViewModel> ListaProjeto = new List<Elemento_RedeTreeViewModel>();

                            var AgrupamentoProjeto = item.ToList().GroupBy(p => new { p.Projeto, p.DescricaoProjeto });
                            foreach (var itemProjeto in AgrupamentoProjeto)
                            {
                                List<Elemento_RedeGestaoViewModel> ObjetosdoAgrupamentoProjeto = itemProjeto.ToList();

                                var AgrupamentoTipo = itemProjeto.ToList().GroupBy(p => new { p.Tipo, p.DescricaoTipo });

                                Elemento_RedeTreeViewModel proj = new Elemento_RedeTreeViewModel
                                {
                                    Id = item.Key.CodCt + "-" + itemProjeto.Key.Projeto,
                                    Text = itemProjeto.Key.DescricaoProjeto,
                                    Children = new List<Elemento_RedeTreeViewModel>(),
                                };
                                foreach (var itemTipo in AgrupamentoTipo)
                                {
                                    List<Elemento_RedeGestaoViewModel> objetosDoAgrupamentoTipo = itemTipo.ToList();
                                    Elemento_RedeTreeViewModel tipo = new Elemento_RedeTreeViewModel
                                    {
                                        Id = item.Key.CodCt + "-" + itemProjeto.Key.Projeto + "-" + itemTipo.Key.Tipo,
                                        Text = itemTipo.Key.DescricaoTipo,
                                        Children = objetosDoAgrupamentoTipo.Select(s => new Elemento_RedeTreeViewModel
                                        {
                                            Id = item.Key.CodCt + "-" + itemProjeto.Key.Projeto + "-" + itemTipo.Key.Tipo + "-" + s.Elemento,
                                            Text = s.DescricaoElemento
                                        }).ToList()
                                    };
                                    proj.Children.Add(tipo);

                                }

                                ListaProjeto.Add(proj);
                            }

                            Elemento_RedeTreeViewModel Obj = new Elemento_RedeTreeViewModel
                            {
                                Id = item.Key.CodCt,
                                Text = item.Key.DescricaoCodCt,
                                Children = ListaProjeto,
                            };

                            retorno.Objeto.Add(Obj);

                        }
                        Elemento_RedeTreeViewModel Obj2 = new Elemento_RedeTreeViewModel
                        {
                            Id = "Area Cobertura",
                            Text = "Areas de Cobertura",
                            Children = ObjArea,
                        };
                        retorno.Objeto.Add(Obj2);
                    }
                }
                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.Message != null ? erro.InnerException.Message : null;
            }
            return retorno;
        }
        #endregion

        private async Task<System.Data.Common.DbDataReader> sqlGetTreeviewGestao(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters)
        {
            #region SqlGetTreeView
            System.Data.Common.DbDataReader reader = null;
            try
            {

                //var filterOrder = Convert.ToBoolean(Filters["@ORDER"]);
                //var filterCount = Convert.ToBoolean(Filters["@COUNT"]);
                var hasReference = Convert.ToBoolean(Filters["@HAS_REFERENCE"]);

                List<String> condicoes = new List<String>();

                Filters.ForEach(request =>
                {
                    switch (request.Key)
                    {

                        case "@TEXT_REFERENCE":
                            condicoes.Add(@$"A.DESCRICAO LIKE '%{request.Key}%' OR B.DESCRICAO LIKE '%{request.Key}%' OR
                                             C.DESCRICAO LIKE '%{request.Key}%' OR D.PROJETO LIKE '%{request.Key}%');");
                            break;

                        default:
                            break;
                    }

                });

                StringBuilder SqlFrom = new StringBuilder(),
                              SqlWhere = new StringBuilder(),
                              Sql = new StringBuilder();

                if (condicoes.Count > 0)
                {
                    if (hasReference)
                    {
                        SqlWhere.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                    }
                }

                SqlFrom.AppendLine("FROM TB_ELEMENTO_REDE B");

                //if (filterCount)
                //{
                //    Sql.AppendLine("SELECT COUNT(A.ID) AS CO");
                //}


                SqlFrom.AppendLine(" INNER JOIN TB_TIPO_ELEMENTO_REDE C ON C.ID = B.ID_TIPO \r\n")
                       .AppendLine(" LEFT JOIN TB_CONTRATO A ON (A.codct = B.contrato) \r\n")
                       .AppendLine(" LEFT JOIN TB_PROJETO D ON D.CODCT = A.CODCT \r\n");

                Sql.AppendLine(" SELECT DISTINCT A.CODCT, A.DESCRICAO, B.ID AS ID_ELEMENTO, B.DESCRICAO AS DESCRICAO_ELEMENTO, ")
                   .AppendLine(" C.ID AS ID_TIPO, C.DESCRICAO AS DESCRICAO_TIPO, D.ID_PROJETO, D.PROJETO AS DESCRICAO_PROJETO ");

                //if (filterOrder)
                //{ SqlOrder.AppendLine("ORDER BY UPPER(A.ID) DESC"); }

                //else
                //{
                //    SqlOrder.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                //}


                Sql.AppendFormat("{0}\r\n{1}\r\n", SqlFrom, SqlWhere);

                reader = await CommandHelper.ExecuteReader(comm, Sql, Filters, session.Connection, null);
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return reader;
            #endregion
        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Elemento_RedeCaixaListaModel>>> GetList()
        {
            #region GetList

            GetApiRetorno<IEnumerable<Elemento_RedeCaixaListaModel>> retorno = new GetApiRetorno<IEnumerable<Elemento_RedeCaixaListaModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())

            /*                                         
                =-=-=-          Repository Usage          =-=-=-                           
             */

            using (var repository = new Conexao<Elemento_Rede>(session))
            using (var repotipo = new Conexao<Tipo_Elemento_Rede>(session))
            {
                try
                {
                    var lista = (await repository.select(p => p.Id != 0)).Where(p => p.Tipo.E_Caixa == true).ToList();

                    retorno.Objeto = lista.Select(p => new Elemento_RedeCaixaListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao
                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
            #endregion
        }

        /// <summary>
        /// Interações dos dados Selecionado na TreeView do Mapa de Gestao de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Elemento_RedeApi/Gestao_Rede_Consulta")]
        #region Gestao_Rede
        public async Task<GetApiRetorno<List<Gestao_ObjRetorno>>> Gestao_Rede_Consulta(Gestao_RedeModel request)
        {
            var retorno = new GetApiRetorno<List<Gestao_ObjRetorno>>();
            var pesquisa = new List<Gestao_ObjRetorno>();
            var pesquisaConnect = new List<Gestao_ObjRetorno>();

            try
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<String> CondicoesWhere = new List<String>();
                List<String> CondicoesWhereConnect = new List<String>();

                using (var session = HibernateUtil.GetSession())
                using (var sessiont5 = HibernateUtil.GetSessionT5())
                using (var sessionMongo = new ConexaoMongoGeography())

                /*                            
                    =-=-=-          Repository Usage          =-=-=-                                
                 */

                using (var repoElementMongo = new RepositoryMongo<Elemento_RedeMongo>(sessionMongo))
                using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                {
                    using (var com = sessiont5.Connection.CreateCommand())
                    {
                        parametros.Add("@ORDER", false);
                        parametros.Add("@COUNT", false);

                        if (request.Ids_Elemento != null)
                        {
                            #region TV
                            if (!String.IsNullOrEmpty(request.Ref_Id_Elemento))
                            {
                                //CondicoesWhere.Add("A.ID = @ID_ELEMENTO ");
                                parametros.Add("@ID_ELEMENTO", request.Ref_Id_Elemento);
                            }

                            if (!String.IsNullOrEmpty(request.Ref_Tipo_Elemento))
                            {
                                //CondicoesWhere.Add("B.ID = @ID_IDTIPO ");
                                parametros.Add("@ID_IDTIPO", request.Ref_Tipo_Elemento);
                            }

                            if (!String.IsNullOrEmpty(request.Ref_Porta_Login))
                            {
                                //CondicoesWhere.Add("D.ID = @ID_PORTA ");
                                parametros.Add("@ID_PORTA", request.Ref_Porta_Login);
                            }
                            #endregion

                            #region Connect

                            if (!String.IsNullOrEmpty(request.Ref_Cliente))
                            {
                                CondicoesWhereConnect.Add(" C.ID = @ID_CLIENTE ");
                                parametros.Add("@ID_CLIENTE", request.Ref_Cliente);
                            }

                            if (!String.IsNullOrEmpty(request.Ref_Status_Cliente))
                            {
                                CondicoesWhereConnect.Add(" C.STATUS = @STATUS_CLIENTE ");
                                parametros.Add("@STATUS_CLIENTE", request.Ref_Fibra);
                            }

                            if (!String.IsNullOrEmpty(request.Ref_Fibra))
                            {
                                CondicoesWhereConnect.Add(" A.ID = @ID_ELEMENTO ");
                                parametros.Add("@ID_ELEMENTO", request.Ref_Fibra);
                            }
                            parametros.Add("@IDS", request.Ids_Elemento);
                            #endregion

                            using (var reader = await sqlGestao_Rede_Consulta(sessiont5, com, parametros))
                            {
                                while (await reader.ReadAsync())
                                {
                                    var consultasql = new Gestao_ObjRetorno
                                    {
                                        Id_Elemento = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : String.Empty,
                                        Descricao_Tipo = Convert.ToString(reader["DESCRICAO_TIPO"]),
                                        Cor = reader["COR"] != DBNull.Value ? Convert.ToString(reader["COR"]) : String.Empty,
                                        Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
                                        Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE"]) : 0,
                                        Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE"]) : 0,
                                        ArquivoXML_str = reader["ARQUIVO"] != DBNull.Value ? Convert.ToString(reader["ARQUIVO"]) : String.Empty,

                                        Id_Icone = reader["ID_ICONE"] != DBNull.Value ? Convert.ToString(reader["ID_ICONE"]) : String.Empty,
                                        Path = reader["CAMINHO"] != DBNull.Value ? Convert.ToString(reader["CAMINHO"]) : String.Empty,
                                        //Login = reader["LOGIN"] != DBNull.Value ? Convert.ToString(reader["LOGIN"]) : String.Empty,
                                        //Cliente_Plano_Produto = reader["CLIENTE_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToString(reader["CLIENTE_PLANO_PRODUTO"]) : String.Empty,
                                        Estilo_Linha_Ativa = Convert.ToString(reader["ESTILO_LINHA_ATIVA"]),
                                        Estilo_Linha_Inativa = Convert.ToString(reader["ESTILO_LINHA_INATIVA"]),
                                        Situacao = Convert.ToString(reader["SITUACAO"]),

                                        Espessura = reader["ESPESSURA"] != DBNull.Value ? Convert.ToInt32(reader["ESPESSURA"]) : 1,

                                    };

                                    if (reader["ID_MODELO"] != DBNull.Value && Convert.ToInt64(reader["ID_MODELO"]) == 5)
                                        consultasql.Poste = true;

                                    pesquisa.Add(consultasql);

                                }

                            }
                        }

                        foreach (var item in pesquisa)
                        {
                            long element = Convert.ToInt64(item.Id_Elemento);
                            Elemento_RedeMongo elemento_RedeMongo = null;
                            elemento_RedeMongo = await repoElementMongo.Find(prop => prop.Id == element);

                            if (elemento_RedeMongo != null)
                            {
                                item.Coordenadas_Elemento = elemento_RedeMongo.Coordenadas;
                                item.CorLinhaAtiva = elemento_RedeMongo.Cor_Linha_Ativa;
                                item.CorLinhaInativa = elemento_RedeMongo.Cor_Linha_Inativa;
                            }

                        }

                        if (request.Ids_Area != null)
                        {
                            Area_CoberturaMongo Area = null;
                            foreach (var _Id in request.Ids_Area)
                            {
                                Area = await repoMongo.Find(p => p.Id == _Id);
                                var consultaMongo = new Gestao_ObjRetorno
                                {
                                    Id_Area = Area.Id,
                                    Cor_Area = Area.Cor,
                                    Descricao_Area = Area.Descricao,
                                    Coordenadas_Area = Area.Coordenadas,
                                };
                                pesquisa.Add(consultaMongo);

                            }
                            ;

                        }
                        //var thisObj = new Area_Cobertura_Gestao
                        //using (var reader2 = await CommandHelper) ;

                    }
                    var pesquisaFinal = new List<Gestao_ObjRetorno>();
                    if (request.Ids_Elemento != null)
                        pesquisaFinal = pesquisa.Concat(pesquisaConnect).ToList();
                    else
                        pesquisaFinal = pesquisa;

                    retorno.Objeto = pesquisaFinal;
                    retorno.Ok = true;
                }
            }
            catch (Exception e)
            {
                retorno.Ok = false;
                retorno.Mensagem = e.Message;
                retorno.InnerException = e.InnerException.Message;
            }
            return retorno;

        }
        #endregion

        private async Task<System.Data.Common.DbDataReader> sqlGestao_Rede_Consulta(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters)
        {
            #region SqlGestao_Rede_Consulta
            System.Data.Common.DbDataReader reader = null;
            try
            {

                var filterOrder = Convert.ToBoolean(Filters["@ORDER"]);
                var filterCount = Convert.ToBoolean(Filters["@COUNT"]);

                List<String> condicoes = new List<String>();

                Filters.Where(p => p.Value.GetType() != typeof(bool)).ForEach(request =>
                {
                    switch (request.Key)
                    {
                        case "@ID_ELEMENTO":
                            condicoes.Add($"A.ID = {request.Key}");
                            break;

                        case "@ID_IDTIPO":
                            condicoes.Add($"B.ID = {request.Key} ");
                            break;

                        case "@ID_PORTA":
                            condicoes.Add($"D.ID = {request.Key} ");
                            break;
                        case "@IDS":
                            var lista = (List<long?>)request.Value;
                            condicoes.Add($"A.ID IN ({String.Join(", ", lista)}) ");
                            break;


                        case "@SEARCH":
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{request.Key}%')");
                            break;

                        default:
                            break;
                    }

                });

                StringBuilder SqlFrom = new StringBuilder(),
                              SqlWhere = new StringBuilder(),
                              Sql = new StringBuilder();

                if (condicoes.Count > 0)
                {
                    SqlWhere.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                }

                SqlFrom.AppendLine("FROM TB_ELEMENTO_REDE A");

                if (filterCount)
                {
                    Sql.AppendLine("SELECT COUNT(A.ID) AS CO");
                }

                else
                {
                    SqlFrom.AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE B ON (A.ID_TIPO = B.ID)\r\n ")
                           .AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_ICONE C ON B.ID_ICONE = C.ID\r\n ");
                    //.AppendLine(" LEFT JOIN TB_ELEMENTO_REDE_PORTA D ON D.ID_CAIXA = A.ID \r\n");

                    Sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.LATITUDE, A.LONGITUDE,  ")
                       .AppendLine(" B.COR, B.ID ID_TIPO, B.DESCRICAO DESCRICAO_TIPO, B.DESCRICAO AS DESCRICAO_TIPO, B.ESPESSURA, ")
                       .AppendLine(" B.ID_MODELO, B.ESTILO_LINHA_ATIVA, B.ESTILO_LINHA_INATIVA, ")
                       .AppendLine(" B.COR, B.ID ID_TIPO, B.DESCRICAO DESCRICAO_TIPO, B.ID_MODELO, B.ESTILO_LINHA_ATIVA, B.ESTILO_LINHA_INATIVA, ")
                       .AppendLine(" A.SITUACAO, B.ID_ICONE, C.ARQUIVO, C.URL, C.CAMINHO");
                }

                Sql.AppendFormat("{0}\r\n{1}\r\n", SqlFrom, SqlWhere);

                reader = await CommandHelper.ExecuteReader(comm, Sql, Filters, session.Connection, null);
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return reader;
            #endregion
        }

        [HttpPost()]
        [Route("api/Elemento_RedeApi/Gravar")]

        #region Gravar
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Elemento_RedeCadastroModel> request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            Elemento_Rede element;
            Modelo_Elemento_Rede modelo = null;

            Tipo_Elemento_Rede tipo = null;
            //Elemento_Rede_Porta porta = null;

            List<Elemento_Rede_Porta> portas = new List<Elemento_Rede_Porta>();
            List<PostApiMensagem> gravacoes = new List<PostApiMensagem>();
            bool possuiPortasVinculadas = false;

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (ITransaction transacao = sessionT5.BeginTransaction())
                    /*             
                        =-=-=-          Repository Usage          =-=-=-                   
                     */
                    using (var repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                    using (var repoModelo = new Conexao<Modelo_Elemento_Rede>(sessionT5))

                    using (var repoTipo = new Conexao<Tipo_Elemento_Rede>(sessionT5))
                    using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                    {

                        tipo = await repoTipo.selectFirst(p => p.Id == request.Objeto.IdTipo);
                        modelo = await repoModelo.selectFirst(p => p.Id == tipo.Modelo.Id);
                        request.Objeto.IdModelo = tipo.Modelo.Id;
                        request.Objeto.Cor_Linha_Ativa = tipo.Cor != null ? tipo.Cor : tipo.Cor_Linha_Ativa;
                        element = await repoElemento.selectFirst(p => p.Id == request.Objeto.Id);

                        if (element == null)
                        {
                            element = new Elemento_Rede
                            {
                                UsuarioCriacao = request.Usuario,
                                DataHoraCriacao = DateTime.Now,
                                Tipo = tipo,
                                //Capacidade = tipo.Capacidade.ToString(),
                            };
                        }
                        else
                        {
                            portas = (await repoPorta.select(p => p.Caixa.Id == element.Id)).ToList();
                            possuiPortasVinculadas = portas.Where(p => p.Cliente_Plano_Prod != null).Count() > 0 ? true : false;
                        }

                        this.BindViewToObject(element, request.Objeto, sessionT5, possuiPortasVinculadas);

                        await repoElemento.SaveOrUpdate(element);

                        if (tipo?.E_Cabo == true)
                        {
                            gravacoes.Add(await GravarFibras(element, sessionT5));
                        }
                        else if (tipo?.E_Caixa == true)
                        {
                            /* 
                             * **********       Método para Gravação das Portas      **********
                             */
                            gravacoes.Add(await GravarPortas(element, portas, sessionT5));
                        }
                        /*
                         * **********       Método para Gravação dos Arquivos      **********
                         */
                        gravacoes.Add(await GravarArquivosElemento(request.Objeto.Arquivos, element, sessionT5));

                        /*
                         * **********       Método para Gravação das Conexões      **********
                         */
                        gravacoes.Add(await GravarConexoesElemento(element, request.Objeto.Conexoes, sessionT5));

                        /*
                         * **********       Método para Gravação dos Endereços      **********
                         */
                        gravacoes.Add(await GravarVinculoEnderecos(element, request.Objeto.Enderecos, sessionT5, session));

                        /*
                         * **********       Método para Gravação dos Condomínios      **********
                         */
                        gravacoes.Add(await GravarVinculoCondominios(element, request.Objeto.Condominios, sessionT5, session));

                        request.Objeto.Lista_Objetos = JsonService.DeserializeObject<List<Object>>(request.Objeto.Objeto) as IEnumerable<Object>;

                        if (request.Objeto.Lista_Objetos.Count() > 0)
                            await Cadastro_Campos_Dinamicos(request.Objeto.Lista_Objetos, sessionT5);

                        using (ConexaoMongoGeography conexaoMongo = new ConexaoMongoGeography())
                        using (var repository = new RepositoryMongo<Elemento_RedeMongo>(conexaoMongo))
                        {
                            await AtualizaElemento(element, repository);
                        }

                        retorno.IdObjeto = element.Id;
                        retorno.Mensagem = "";
                        retorno.Ok = true;

                        if (retorno.Ok)
                        {
                            transacao.Commit();
                        }

                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.Mensagens = gravacoes;
                retorno.InnerException = erro.InnerException?.Message;
            }
            return retorno;
        }
        #endregion

        #region Relacionados A Gravacao do Elemento de Rede
        public async Task<PostApiMensagem> GravarPortas(Elemento_Rede element, List<Elemento_Rede_Porta> portas, ISession sessionT5)
        {
            #region GravarPortas 
            try
            {

                /*             
                    =-=-=-          Repository Usage          =-=-=-                   
                 */

                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                {
                    Elemento_Rede_Porta porta = null;
                    if (element.Capacidade > portas.Count)
                    {
                        for (int i = 1; i <= element.Capacidade; i++)
                        {
                            if (i > portas.Count)
                            {
                            porta = new Elemento_Rede_Porta
                            {
                                    Porta = i,
                                Caixa = element,
                            };

                            switch (element.Situacao)
                            {
                                case Status_Elemento.Previsto:
                                    porta.Status = Situacao_Porta.Previsto;
                                    break;
                                case Status_Elemento.Construido:
                                    porta.Status = Situacao_Porta.Construido;
                                    break;
                                case Status_Elemento.Ativo:
                                    porta.Status = Situacao_Porta.Disponivel;
                                    break;
                                case Status_Elemento.Inativo:
                                    porta.Status = Situacao_Porta.Indisponivel;
                                    break;
                                case Status_Elemento.AguardandoCertificacao:
                                    porta.Status = Situacao_Porta.Construido;
                                    break;
                                case Status_Elemento.Manutencao:
                                    porta.Status = Situacao_Porta.Indisponivel;
                                    break;
                            }

                            portas.Add(porta);
                        }
                    }
                    }

                    await repoPorta.SaveOrUpdate(portas);

                    if (element.Situacao == Status_Elemento.Ativo)
                    {
                        var portasAlterar = portas.FindAll(p => p.Status == Situacao_Porta.Construido);
                        foreach (var item in portasAlterar)
                        {
                            item.Status = Situacao_Porta.Disponivel;
                        }

                        await repoPorta.Update(portasAlterar);
                    }

                    return new PostApiMensagem
                    {
                        Tipo = PostApiMensagemType.Info,
                        Mensagem = RsBase.OPERACAOBEMSUCEDIDA
                    };
                }
            }
            catch (Exception ex)
            {
                // *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar para mais detalhes do erro.
                return new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = String.IsNullOrEmpty(ex.InnerException.Message) ?
                             ex.Message : $"{ex.Message}Inner Exception:{ex.InnerException?.Message}"
                };
            }
            #endregion
        }

        public async Task<PostApiMensagem> GravarFibras(Elemento_Rede elemento, ISession sessionT5)
        {
            #region GravarFibras
            try
            {
                int? num = elemento?.Tipo?.Num_Pares_Fibra,
                     Tubos_Loose = elemento?.Tipo?.Num_Tubos_Loose;

                /*                             
                    =-=-=-          Repository Usage          =-=-=-                                  
                 */

                using (var repository = new Conexao<Elemento_Rede>(sessionT5))
                using (var repositoryFibra = new Conexao<Elemento_Rede_Fibra>(sessionT5))
                {

                    elemento.Cabo = elemento;
                    elemento.Padrao_Cores = await CheckPattern(elemento);
                    elemento.Fibras = (await repositoryFibra.select(p => p.Elemento_Cabo == elemento)).ToList();

                    if (elemento.Fibras.Count() < 1 && num.HasValue && Tubos_Loose.HasValue)
                    {
                        elemento.Fibras = BindFibras(num.Value, Tubos_Loose.Value, elemento);
                    }

                    await repository.SaveOrUpdate(elemento);
                    await repositoryFibra.SaveOrUpdate(elemento.Fibras);

                }
                return new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Info,
                    Mensagem = RsBase.OPERACAOBEMSUCEDIDA
                };
            }
            catch (Exception ex)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
                return new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = String.IsNullOrEmpty(ex.InnerException.Message) ?
                               ex.Message : $"{ex.Message}Inner Exception:{ex.InnerException?.Message}"
                };
            }
            #endregion
        }

        public List<Elemento_Rede_Fibra> BindFibras(long num, long Tubos_Loose, Elemento_Rede cabo)
        {
            #region BindFibras

            var fibras = new List<Elemento_Rede_Fibra>();
            for (var iterationLoose = 1; iterationLoose <= Tubos_Loose; iterationLoose++)
            {
                for (var iterationFibra = 0; iterationFibra < num;)
                {
                    fibras.Add(new Elemento_Rede_Fibra
                    {
                        numFibra = ++iterationFibra,
                        numLoose = iterationLoose,
                        Elemento_Cabo = cabo
                    });
                }
            }


            return fibras;
            #endregion
        }
        public async Task<Padrao_Cor> CheckPattern(Elemento_Rede element)
        {
            #region  Padrão de Cores
            var pattern = element.Tipo.Padrao.GetValueOrDefault();

            return pattern;
            #endregion
        }

        public async Task<PostApiMensagem> GravarArquivosElemento(IEnumerable<Elemento_RedeFotoArquivo> arquivos, Elemento_Rede element, ISession sessionT5)
        {
            #region Gravar Arquivos

            try
            {

                /*             
                    =-=-=-          Repository Usage          =-=-=-                   
                 */

                using (var repoArquivo = new Conexao<Elemento_RedeFotosArquivos>(sessionT5))
                {
                    var Files = arquivos.Select(p =>
                    {
                        return new Elemento_RedeFotosArquivos
                        {
                            Id = 0,
                            Descricao = p.Descricao,
                            Elemento = element,
                            Tipo_FotoArquivo = p.Tipo,
                            Url = $"{CONSTANTES.CaminhoFisico}/Tmp/Elemento_de_Rede/{p.Descricao}{p.Tipo}",

                        };

                    }).ToList();

                    await repoArquivo.SaveOrUpdate(Files);

                    return new PostApiMensagem
                    {
                        Tipo = PostApiMensagemType.Info,
                        Mensagem = RsBase.OPERACAOBEMSUCEDIDA
                    };
                }
            }
            catch (Exception ex)
            {
                // *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar para mais detalhes do erro.
                return new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = String.IsNullOrEmpty(ex.InnerException.Message) ?
                               ex.Message : $"{ex.Message}Inner Exception:{ex.InnerException?.Message}"
                };
            }
            #endregion
        }

        public async Task<PostApiMensagem> GravarConexoesElemento(Elemento_Rede element, IEnumerable<Elemento_RedeConexoes> Conexoes, ISession sessionT5)
        {
            #region Gravar Conexoes
            try
            {
                /*             
                    =-=-=-          Repository Usage          =-=-=-                   
                 */
                using (var repository = new Conexao<Elemento_Rede>(sessionT5))
                using (var repoDoor = new Conexao<Elemento_Rede_Porta>(sessionT5))

                //using (var repoCable = new Conexao<Elemento_Rede_Cabo>(sessionT5))
                using (var repoFiber = new Conexao<Elemento_Rede_Fibra>(sessionT5))
                {
                    var fibers = new List<Elemento_Rede_Fibra>();
                    //var cable = await repository.selectFirst(p => p == element);
                    foreach (var conexao in Conexoes.Where(p => p.Id == 0))
                    {
                        var fibraSelecionada = element.Fibras.ToList()
                                                           .Find(p => p.Id == conexao.Id_FibraUtilizada);

                        //elemento_rede(Caixa)
                        fibraSelecionada.Ponta_A = conexao.Id_Origem;
                        fibraSelecionada.Ponta_B = conexao.Id_Destino;

                        //fibraSelecionada.numFibra = conexao.Numero_Fibra;
                        //fibraSelecionada.numLoose = conexao.Numero_Loose;
                        //fibraSelecionada.

                        //fibraSelecionada.DataHoraCriacao

                        //elemento_rede(Porta)
                        fibraSelecionada.Conexao_Ponta_A = conexao.Id_Conexao_Origem;
                        fibraSelecionada.Conexao_Ponta_B = conexao.Id_Conexao_Destino;

                        fibers.Add(fibraSelecionada);
                    }
                    //element.Conexoes = connection;
                    await repoFiber.SaveOrUpdate(fibers);

                    return new PostApiMensagem
                    {
                        Tipo = PostApiMensagemType.Info,
                        Mensagem = RsBase.OPERACAOBEMSUCEDIDA
                    };
                }
            }
            catch (Exception ex)
            {

                return new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = String.IsNullOrEmpty(ex.InnerException.Message) ?
                             ex.Message : $"{ex.Message}Inner Exception:{ex.InnerException?.Message}"
                };
            }
            #endregion
        }


        public async Task<PostApiMensagem> GravarVinculoEnderecos(Elemento_Rede element, IEnumerable<Elemento_RedeEnderecosModel> EnderecosView, ISession sessionT5, ISession session)
        {
            #region Gravar Vinculo de Enderecos
            try
            {
                /*             
                    =-=-=-          Repository Usage          =-=-=-                   
                 */
                using (var repository = new Conexao<Elemento_Rede>(sessionT5))
                using (var repoDoor = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (var repoAddress = new Conexao<Elemento_Rede_Enderecos>(sessionT5))
                {
                    var enderecos = new List<Elemento_Rede_Enderecos>();
                    enderecos.AddRange(EnderecosView.Where(p => p.Id == 0).Select(endereco =>
                        new Elemento_Rede_Enderecos
                        {
                            Elemento_Rede = element,
                            Id_EnderecoLogradouro = endereco.Id_Logradouro,
                            Id_CondominioPredio = endereco.Id_Condominio,
                            Numero = endereco.Numero
                        }).ToList());

                    await repoAddress.SaveOrUpdate(enderecos);

                    return new PostApiMensagem
                    {
                        Tipo = PostApiMensagemType.Info,
                        Mensagem = RsBase.OPERACAOBEMSUCEDIDA
                    };
                }
            }
            catch (Exception ex)
            {

                return new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = String.IsNullOrEmpty(ex.InnerException.Message) ?
                             ex.Message : $"{ex.Message}Inner Exception:{ex.InnerException?.Message}"
                };
            }
            #endregion
        }


        public async Task<PostApiMensagem> GravarVinculoCondominios(Elemento_Rede element, IEnumerable<Elemento_RedeCondominiosModel> CondominiosView, ISession sessionT5, ISession session)
        {
            #region Gravar Vinculo de Condominios
            try
            {
                /*             
                    =-=-=-          Repository Usage          =-=-=-                   
                 */
                using (var repository = new Conexao<Elemento_Rede>(sessionT5))
                using (var repoDoor = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (var repoAddress = new Conexao<Elemento_Rede_Enderecos>(sessionT5))
                {
                    var condominios = new List<Elemento_Rede_Enderecos>();
                    condominios.AddRange(CondominiosView.Where(p => p.Id == 0).Select(endereco =>
                        new Elemento_Rede_Enderecos
                        {
                            Elemento_Rede = element,
                            Id_CondominioPredio = endereco.Id_Condominio,
                            Bloco = endereco.Id_Bloco,
                            Apto_numero = endereco.Id_Apto_Numero,
                            Todos_Apto_Numero = endereco.Apto_Numero == "Todos" ? true : false,
                            Todos_Bloco = endereco?.Bloco == "Todos" ? true : false
                        }).ToList());

                    await repoAddress.SaveOrUpdate(condominios);

                    return new PostApiMensagem
                    {
                        Tipo = PostApiMensagemType.Info,
                        Mensagem = RsBase.OPERACAOBEMSUCEDIDA
                    };
                }
            }
            catch (Exception ex)
            {

                return new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = String.IsNullOrEmpty(ex.InnerException.Message) ?
                             ex.Message : $"{ex.Message}Inner Exception:{ex.InnerException?.Message}"
                };
            }
            #endregion
        }

        public void BindViewToObject(Elemento_Rede element, Elemento_RedeBaseRegistrationModel novoEl, ISession session, bool possuiPortaVinculada = false)
        {
            #region BindViewToObject

            /*             
                =-=-=-          Repository Usage          =-=-=-                   
             */
            using (var repoTipo = new Conexao<Tipo_Elemento_Rede>(session))
            using (var repoElemento = new Conexao<Elemento_Rede>(session))
            {
                if (possuiPortaVinculada)
                {
                    if (novoEl.Situacao != Status_Elemento.Ativo && novoEl.Situacao != Status_Elemento.Inativo)
                    {
                        throw new Exception("Elemento possui portas vinculadas a clientes");
                    }
                }

                element.UsuarioCriacao = novoEl.UsuarioCriacao;
                element.UsuarioAlteracao = User.Identity.GetUserId();
                element.Bairro = novoEl.Bairro;
                element.Pais = novoEl.Pais;

                element.Cabo = novoEl.Cabo.HasValue ? repoElemento.selectFirst(p => p.Id == novoEl.Cabo).Result : null;
                element.Poste = novoEl.Poste.HasValue ? repoElemento.selectFirst(p => p.Id == novoEl.Poste).Result : null;
                element.Caixa = novoEl.Caixa.HasValue ? repoElemento.selectFirst(p => p.Id == novoEl.Caixa).Result : null;
                element.CEP = novoEl.Cep;
                if (string.IsNullOrEmpty(element.UsuarioCertificou) && novoEl.Tem_Certificado)
                {
                    element.UsuarioCertificou = User.Identity.GetUserId();
                    element.DataCertificacao = DateTime.Now;
                }
                element.Certificado = novoEl.Situacao == Status_Elemento.Ativo ? true : false;
                element.Capacidade = novoEl.Capacidade != null ? novoEl.Capacidade.Value : 0;
                element.Cidade = novoEl.Cidade;

                element.Complemento = novoEl.Complemento;
                element.Contrato = novoEl.Contrato;
                //element.DataHoraCriacao = DateTime.Now;

                element.Descricao = novoEl.Descricao;
                element.Endereco = novoEl.Endereco;
                element.Id_POP = novoEl.Id_POP;

                element.Id_OLT = novoEl.Id_OLT;
                element.Id_Slot = novoEl.Id_Slot;
                element.Id_Pon = novoEl.Id_Pon;
                element.Cor_linha_Ativa = novoEl.Cor_Linha_Ativa;
                element.Cor_linha_Inativa = novoEl.Cor_Linha_Inativa;

                element.Latitude = novoEl.Latitude;
                element.Longitude = novoEl.Longitude;
                element.Numero = novoEl.Numero;
                element.Nos = novoEl.Nos;

                //element.DataCriacao = novoEl.DataCriacao;

                element.Observacao = novoEl.Observacao;
                element.Projeto = novoEl.Projeto;
                element.Situacao = novoEl.Situacao;

                element.Tipo = repoTipo.selectFirst(p => p.Id == novoEl.IdTipo).Result;
                element.UF = novoEl.Uf;
                element.Coordenadas = novoEl.Coordenadas;
                element.AreaAtendimento = novoEl.AreaAtendimento;

                //element.Modelo.Id = novoEl.IdModelo.Value;
                element.Num_Coordenadas = novoEl.Num_Coordenadas;
                element.Metragem = Convert.ToDouble(novoEl.Metragem);


            }

            #endregion
        }
        #endregion


        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetComboConexoesFibra")]
        #region ComboConexoesFibra
        public async Task<ObjCombo<Int64>> GetComboConexoesFibra(RequestObjComboPai<long?, long?> request)
        {
            var retorno = new ObjCombo<long> { results = new List<IObjComboResult<long>>() };
            List<FiberComboModel> teste = new List<FiberComboModel>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    var parametros = new Dictionary<String, Object>();
                    IElementoRedeAnielRepository repository = new ElementoRedeAnielRepository(sessionT5);
                    var business = new ElementoRedeBusiness(repository, this.Cores);

                    if (request.idpai != null)
                    {
                        parametros.Add("@IDPAI", request.idpai);
                    }
                    if (request.id != null)
                    {
                        parametros.Add("@ID", request.id);
                    }

                    parametros.Add("@PONTA_ENTRADA", true);
                    parametros.Add("@PONTA_SAIDA", true);
                    parametros.Add("@COUNT", false);

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        parametros["@COUNT"] = true;
                        using (var reader = await sqlGetComboConexoesFibra(sessionT5, com, parametros))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                            }
                        }

                        parametros["@COUNT"] = false;
                        using (var reader = await sqlGetComboConexoesFibra(sessionT5, com, parametros))
                        {
                            while (await reader.ReadAsync())
                            {
                                var padrao = (Padrao_Cor)Convert.ToInt16(reader["PADRAO"]);
                                var colorLoose = business.getColorLoose(padrao, Convert.ToInt32(reader["TUBO_LOOSE"]), this.Cores).text;

                                teste.Add(new FiberComboModel
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = $"Loose: {colorLoose} - {Convert.ToString(reader["FIBRA"])}",
                                    Numero_Fibra = Convert.ToInt32(reader["FIBRA"]),

                                    PadraoUtilizado = HtmlEnumExtensions.GetEnumDescription(padrao),
                                    Cabo = Convert.ToInt64(reader["ID_CABO"]),
                                    Numero_Loose = Convert.ToInt32(reader["TUBO_LOOSE"]),
                                });
                            }
                        }

                        retorno.results.AddRange(teste);
                    }
                }
            }
            catch (Exception)
            {
                //                                              Pro Tip:
                //
                // Shif + F9 para utilizar o Quick Watch e então verificando a variável $exception da para visualizar o erro.
            }

            return retorno;
        }
        #endregion

        private async Task<System.Data.Common.DbDataReader> sqlGetComboConexoesFibra(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters)
        {
            #region SqlGetComboConexoesFibra
            System.Data.Common.DbDataReader reader = null;
            try
            {

                //var filterOrder = Convert.ToBoolean(Filters["@ORDER"]);
                var filterCount = Convert.ToBoolean(Filters["@COUNT"]);

                List<String> condicoes = new List<String>();

                Filters.ForEach(request =>
                {
                    switch (request.Key)
                    {
                        case "@IDPAI":
                            condicoes.Add($"C.ID = {request.Key}");
                            break;

                        case "@ID":
                            condicoes.Add($"C.ID = {request.Key}");
                            break;

                        case "@PONTA_ENTRADA":
                            condicoes.Add("A.PONTA_ENTRADA IS NULL");
                            break;

                        case "@PONTA_SAIDA":
                            condicoes.Add("A.PONTA_SAIDA IS NULL");
                            break;

                        default:
                            break;
                    }

                });

                StringBuilder SqlFrom = new StringBuilder(),
                              SqlWhere = new StringBuilder(),
                              Sql = new StringBuilder();

                if (condicoes.Count > 0)
                {
                    SqlWhere.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                }

                SqlFrom.AppendLine("FROM TB_ELEMENTO_REDE_FIBRA A")
                       //.AppendLine(" LEFT JOIN TB_ELEMENTO_REDE B ON B.ID_CABO = A.ID_CABO \r\n ")
                       .AppendLine("LEFT JOIN TB_ELEMENTO_REDE C ON C.ID_CABO = A.ID_CABO \r\n");

                if (filterCount)
                {
                    Sql.AppendLine("SELECT COUNT(A.ID) AS CO ");
                }
                else
                {
                    Sql.AppendLine(" SELECT DISTINCT A.ID, A.FIBRA, A.TUBO_LOOSE, C.ID AS ID_CABO, C.PADRAO ");
                }

                Sql.AppendFormat("{0}\r\n{1}\r\n", SqlFrom, SqlWhere);

                reader = await CommandHelper.ExecuteReader(comm, Sql, Filters, session.Connection, null);
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return reader;
            #endregion
        }

        [HttpPost]
        [Route("api/Elemento_RedeApi/Ellaborate_DiagramaFusao")]
        #region Ellaborate Diagrama

        public async Task<GetApiRetorno<DiagramaFusaoConexoesModel>> Ellaborate_DiagramaFusao([FromBody] long request)
        {
            GetApiRetorno<DiagramaFusaoConexoesModel> retorno = new GetApiRetorno<DiagramaFusaoConexoesModel>
            {
                Objeto = new DiagramaFusaoConexoesModel()
            };
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())

                /*         
                    =-=-=-          Repository Usage          =-=-=-                               
                 */

                using (var repositoryPON = new Conexao<PON>(session))
                using (var repositoryOLT = new Conexao<OLT>(session))

                using (var repositoryDoor = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (var repositoryType = new Conexao<Tipo_Elemento_Rede>(sessionT5))


                using (var repositoryFiber = new Conexao<Elemento_Rede_Fibra>(sessionT5))
                using (var repositoryElement = new Conexao<Elemento_Rede>(sessionT5))

                /*                                       
                    =-=-=-          Api Usage          =-=-=-                                
                 */
                using (var Porta_Api = new Elemento_Rede_PortaApiController())
                {
                    IElementoRedeAnielRepository repository = new ElementoRedeAnielRepository(sessionT5);
                    var business = new ElementoRedeBusiness(repository, this.Cores);

                    Elemento_Rede element = await repositoryElement.selectFirst(p => p.Id == request);
                    Modelo_Elemento_Rede modelo = (await repositoryType.selectFirst(p => p.Id == element.Tipo.Id)).Modelo;
                    retorno.Objeto.Id_Elemento = request;
                    retorno.Objeto.Json_order = element.JSON_Ordenacao_Digarama;

                    if (modelo.Id == 6)
                    {
                        var cabo = await repositoryElement.selectFirst(p => p.Id == request);
                        cabo.Fibras = await repositoryFiber.select(p => p.Elemento_Cabo.Id == cabo.Id);

                        retorno.Objeto.listagem_Cabos = new List<Elemento_Rede_CaboModel>
                        {
                            new Elemento_Rede_CaboModel
                            {
                                Id = cabo.Id,
                                listagem_Tubo_Loose = business.GetFibrasLoose(cabo)
                            }
                        };
                        if (cabo.Fibras.Any(p => p.Ponta_A != null || p.Ponta_B != null))
                        {
                            retorno.Objeto.listagem_Conexoes = cabo.Fibras.Where(p => p.Ponta_A != null).Select(p =>
                            {
                                Elemento_Rede Origem = repositoryElement.selectFirst(obj => obj.Id == p.Ponta_A).Result,
                                              Destino = repositoryElement.selectFirst(obj => obj.Id == p.Ponta_B).Result;
                                return new Elemento_RedeConexoes
                                {
                                    Id_Cabo = p.Elemento_Cabo.Id,
                                    Id_Destino = p.Ponta_B.Value,
                                    Id_Origem = p.Ponta_A.Value,

                                    Id_Conexao_Destino = p.Conexao_Ponta_A.Value,
                                    Id_Conexao_Origem = p.Conexao_Ponta_B.Value,

                                    Tipo_Elemento_Origem = Origem.Tipo.Id,
                                    Tipo_Elemento_Destino = Destino.Tipo.Id,

                                    Descricao_Elemento_Origem = Origem.Descricao,
                                    Descricao_Elemento_Destino = Destino.Descricao,

                                    Descricao_Tipo_Elemento_Origem = Origem.Tipo.Descricao,
                                    Descricao_Tipo_Elemento_Destino = Destino.Tipo.Descricao,
                                };
                            }).ToList();
                        }
                        retorno.Objeto.listagem_Conexoes.ForEach(conexao =>
                        {
                            List<Elemento_RedeCadastroModel> caixasConexao = new List<Elemento_RedeCadastroModel>
                            {
                                    business.FindById(conexao.Id_Origem).Result,
                                    business.FindById(conexao.Id_Destino).Result,
                            };
                            caixasConexao = caixasConexao.Distinct().ToList();

                            retorno.Objeto.listagem_Caixas = caixasConexao;
                            retorno.Objeto.listagem_Portas = repositoryDoor.select(p => p.Caixa == element).Result.Select(p =>
                            {
                                return new Elemento_Rede_PortaCadastroModel
                                {
                                    Id_Caixa = element.Id,
                                    Porta = p.Porta.Value,
                                    Status = p.Status,
                                    Id = p.Id,
                                };
                            }).ToList();
                        });
                    }
                    else if (modelo.Id == 7 || modelo.Id == 4)
                    {

                        /*                                           
                            =-=-=-          Api Usage          =-=-=-                              
                         */
                        using (var elementApi = new Elemento_RedeApiController())
                        using (var PortaApi = new Elemento_Rede_PortaApiController())
                        {
                            var Caixa = (await elementApi.FindById(element.Id));
                            retorno.Objeto.listagem_Caixas = new List<Elemento_RedeCadastroModel> { Caixa.Objeto };
                            retorno.Objeto.listagem_Portas = (await repositoryDoor.select(p => p.Caixa == element)).Select(p =>
                            {
                                return new Elemento_Rede_PortaCadastroModel
                                {
                                    Id_Caixa = element.Id,
                                    Porta = p.Porta.Value,
                                    Status = p.Status,
                                    Id = p.Id,
                                };
                            }).ToList();

                        }
                    }

                }
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.Mensagem = erro?.InnerException?.Message;
            }

            return retorno;
        }
        #endregion

        [HttpGet()]
        [Route("api/Elemento_RedeApi/AtualizaTodosElementosMongo")]

        #region AtualizaTodosElementosMongo
        public async Task<String> AtualizaTodosElementosMongo()
        {
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (ConexaoMongoGeography conexaoMongo = new ConexaoMongoGeography())

                /*         
                    =-=-=-          Repository Usage          =-=-=-               
                 */

                using (var repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                using (var repositoryMongo = new RepositoryMongo<Elemento_RedeMongo>(conexaoMongo))
                {
                    foreach (var elemento in await repoElemento.select())
                    {
                        await AtualizaElemento(elemento, repositoryMongo);
                    }
                }
                return "Ok";
            }
            catch (Exception erro)
            {
                return erro.Message;
            }
        }
        #endregion

        #region AtualizaElemento
        public async Task AtualizaElemento(Elemento_Rede de, RepositoryMongo<Elemento_RedeMongo> repository)
        {
            Elemento_RedeMongo para = await repository.Find(p => p.Id == de.Id);

            Boolean novo = false;
            if (para == null)
            {
                para = new Elemento_RedeMongo
                {
                    Id = de.Id
                };
                novo = true;
            }
            //indicando que são 2 parametros, latitude e longitude
            //const int parametros = 2;

            para.Bairro = de.Bairro;
            para.Pais = de.Pais;
            para.Capacidade = de.Capacidade;
            para.CEP = de.CEP;

            para.Certificado = de.Certificado;
            para.Cidade = de.Cidade;
            para.Complemento = de.Complemento;
            para.Contrato = de.Contrato;

            para.Cor = de.Tipo?.Cor;
            para.Projeto = de.Projeto;
            para.Cor_Linha_Ativa = de.Tipo?.Cor_Linha_Ativa;
            para.Cor_Linha_Inativa = de.Tipo?.Cor_Linha_Inativa;

            para.DataHoraCriacao = de.DataHoraCriacao;
            para.DataHoraAlteracao = de.DataHoraAlteracao;
            para.Descricao = de.Descricao;
            para.Endereco = de.Endereco;

            para.Espessura = de.Tipo?.Espessura;
            //para.Estilo_Linha_Ativa = de.Tipo?.Estilo_Linha_Ativa;
            //para.Estilo_Linha_Inativa = de.Tipo?.Estilo_Linha_Inativa;
            para.E_Cabo = Convert.ToBoolean(de.Tipo?.E_Cabo);
            para.E_Caixa = Convert.ToBoolean(de.Tipo?.E_Caixa);
            para.Fabricante = de.Tipo?.Fabricante;

            para.Icone = de.Tipo?.Icone;
            para.Id_Modelo = Convert.ToInt64(de.Tipo?.Modelo?.Id);
            para.Id_Tipo = de.Tipo.Id;
            //para.Id_Tipo_Cabo = de.Tipo.Tipo_Cabo.Id;
            para.Tipo = de.Tipo.Descricao;

            para.Tipo_Cabo = de?.Tipo.Tipo_Cabo?.Descricao;
            para.Tipo_de_Cabo = de.Tipo.Tipo_de_Cabo;
            para.UF = de.UF;
            para.UsuarioCriacao = de.UsuarioCriacao;

            para.UsuarioAlteracao = de.UsuarioAlteracao;
            para.Latitude = de.Latitude;
            para.Longitude = de.Longitude;

            if (de.Coordenadas != null)
            {
                var coordenadasString = JsonConvert.DeserializeObject<List<string>>(de.Coordenadas);
                var coordenadasDouble = coordenadasString
                    .Select(coord => (double?) Double.Parse(coord.Replace(",", ".")))
                    .ToList();
                para.Coordenadas = coordenadasDouble;
            }

            if (novo) await repository.Save(para);
            else await repository.Update(p => p.Id == de.Id, para);
        }
        #endregion

        #region Cadastro_Campos_Dinamicos
        public async Task<PostApiRetorno> Cadastro_Campos_Dinamicos(IEnumerable<Object> request, ISession session)
        {
            var retorno = new PostApiRetorno();

            /*             
                =-=-=-          Repository Usage          =-=-=-                   
             */

            using (var repoElemento = new Conexao<Elemento_Rede>(session))
            using (var repo = new Conexao<Elemento_RedeCampos>(session))
            {

                foreach (dynamic obj in request)
                {
                    long idElemento = obj.idElemento;
                    string idDinamico = obj.idDinamico;
                    Elemento_Rede elemento = await repoElemento.selectFirst(p => p.Id == idElemento);
                    Elemento_RedeCampos respostaDinamico = await repo.selectFirst(p => p.Id_Campo_Dinamico == idDinamico && p.Elemento == elemento);

                    if (respostaDinamico != null)
                    {
                        respostaDinamico.Valor = obj.value;
                    }
                    else
                    {
                        respostaDinamico = new Elemento_RedeCampos
                        {
                            Id = 0,
                            Elemento = elemento,
                            Id_Campo_Dinamico = obj.idDinamico,
                            Valor = obj.value,
                            Descricao = obj.descricao,
                            Tipo = obj.tipo
                        };
                    }

                    await repo.SaveOrUpdate(respostaDinamico);
                }
            }
            return retorno;
        }
        #endregion

        [Route("api/Elemento_RedeApi/NoSqlQueryMongo")]
        [HttpGet()]
        #region NoSqlQueryMongo
        public async Task<GetApiRetorno<List<Elemento_RedeMongo>>> NoSqlQueryMongo(Elemento_RedeMongo request)
        {
            var retorno = new GetApiRetorno<List<Elemento_RedeMongo>>();
            Elemento_RedeMongo b = new Elemento_RedeMongo();
            var parametros = new Dictionary<String, Object>();
            double[] arraynovo = new double[] { 20.015, 10.2054 };
            parametros.Add("$in", arraynovo);
            parametros.Add("", arraynovo);
            MongoDB.Bson.BsonDocument doc = new MongoDB.Bson.BsonDocument();
            foreach (var registro in parametros)
            {
                var valuesBson = MongoDB.Bson.BsonValue.Create(registro.Value);
                doc.Add(registro.Key, valuesBson);

            }
            using (ConexaoMongoGeography conexaoMongo = new ConexaoMongoGeography())
            using (var repositoryMongo = new RepositoryMongo<Elemento_RedeMongo>(conexaoMongo))
            {
                var a = repositoryMongo.GetCollection();
                var query = await a.FindAsync<Elemento_RedeMongo>(doc);

                while (await query.MoveNextAsync())
                {
                    foreach (var document in query.Current)
                    {
                        retorno.Objeto.Add(document);
                    }
                }
            }
            return retorno;
        }
        #endregion


        [HttpGet]
        [Route("api/Elemento_RedeApi/FindById")]

        #region FindById
        public async Task<GetApiRetorno<Elemento_RedeCadastroModel>> FindById(Int64? Id)
        {
            GetApiRetorno<Elemento_RedeCadastroModel> retorno = new GetApiRetorno<Elemento_RedeCadastroModel>();
            try
            {
                    /*             
                        =-=-=-          Repository Usage          =-=-=-                   
                     */
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                    {
                    IElementoRedeAnielRepository repository = new ElementoRedeAnielRepository(sessionT5);
                    ElementoRedeBusiness business = new ElementoRedeBusiness(repository, this.Cores);
                    retorno.Objeto = await business.FindById(Id);
                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message + $"stackTrace: {erro.StackTrace}";
                retorno.InnerException = erro?.InnerException?.Message;
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Elemento_RedeApi/Excluir")]
        #region Excluir
        public async Task<DeleteApiRetorno<Int64>> Excluir(ParamWebApi<Int64> value)
        {

            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                Elemento_Rede Elemento = null;
                List<Elemento_RedeCampos> Campos_Elemento = null;
                List<Elemento_RedeFotosArquivos> Elemento_Arq = null;
                List<Area_Manutencao_Elementos> elementoMat = null;



                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {

                    using (var repo_Elemento = new Conexao<Elemento_Rede>(sessionT5))
                    using (var repo_Portas = new Conexao<Elemento_Rede_Porta>(sessionT5))
                    using (var repo_Campos = new Conexao<Elemento_RedeCampos>(sessionT5))
                    using (var repo_AreaManutencaoElemento = new Conexao<Area_Manutencao_Elementos>(sessionT5))
                    using (var repoViabilidade_Porta = new Conexao<ConsultaViabilidadePortaDisponivel>(sessionT5))
                    using (var repoViabilidade_Dados = new Conexao<ConsultaViabilidadeDados>(sessionT5))
                    using (var repositorio = new Conexao<Elemento_RedeFotosArquivos>(sessionT5))
                    using (var repMatEle = new Conexao<Area_Manutencao_Elementos>(sessionT5))
                    {
                        Elemento = await repo_Elemento.selectFirst(p => p.Id == value.Objeto);

                        var portas = await repo_Portas.select(p => p.Caixa.Id == Elemento.Id);
                        var portas_array = portas.Select(p => p.Id).ToArray();

                        if (portas?.Count() > 0)
                        {
                            if (portas?.Where(p => p.Cliente_Plano_Prod != null).Count() > 0)
                            {
                                throw new Exception("Exclusão não concluída. Existe(m) porta(s) com cliente vinculado. Realize o desvinculo e tente novamente.");
                        }
                            else
                            {
                                var ViabilidadePorta = await repoViabilidade_Porta.queryover().WhereRestrictionOn(p => p.Porta).IsIn(portas_array).ListAsync();
                                if (ViabilidadePorta.Any())
                                    await repoViabilidade_Porta.Remove(ViabilidadePorta);

                                await repo_Portas.Remove(portas);
                            }
                        }
                                              
                        var AreaManutencaoElemento = await repo_AreaManutencaoElemento.select(p => p.Elemento_de_Rede == Elemento);
                        if (AreaManutencaoElemento.Any())
                            await repo_AreaManutencaoElemento.Remove(AreaManutencaoElemento);
                        
                        var Viabilidade_Dados = await repoViabilidade_Dados.select(p => p.Caixa == Elemento);
                        if (Viabilidade_Dados.Any())
                            await repoViabilidade_Dados.Remove(Viabilidade_Dados);

                        Campos_Elemento = (await repo_Campos.select(p => p.Elemento == Elemento)).ToList();
                        Elemento_Arq = (await repositorio.select(prop => prop.Elemento == Elemento)).ToList();
                        elementoMat = (await repMatEle.select(p => p.Elemento_de_Rede.Id == Elemento.Id)).ToList();

                        await repositorio.Remove(Elemento_Arq);
                        await repMatEle.Remove(elementoMat);
                        await repo_Elemento.Remove(Elemento);

                        retorno.IdObjeto = Elemento.Id;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;

                // Mensagens amigáveis para o usuário
                if (erro is SqlException || erro is HibernateException)
                {
                    retorno.Mensagem = "Não foi possível concluir a exclusão devido a um problema no banco de dados. Verifique os vínculos do elemento e tente novamente.";
            }
                else
                {
                    retorno.Mensagem = "Ocorreu um erro ao processar a exclusão. Tente novamente mais tarde.";
                }

            }
            return retorno;

        }
        #endregion

        [HttpPost()]
        [Route("api/Elemento_RedeApi/VincularPosteCaixa")]
        #region Vincular Poste
        public async Task<PostApiRetorno<Int64>> VincularPosteCaixa(RequestElementoVincularPosteCaixa request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Elemento_Rede ElementoPoste = null;
                Elemento_Rede ElementoCaixa = null;


                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {

                    /*             
                        =-=-=-          Repository Usage          =-=-=-                   
                     */

                    using (var repo_Elemento = new Conexao<Elemento_Rede>(sessionT5))
                    {
                        ElementoPoste = await repo_Elemento.selectFirst(p => p.Id == request.IdPoste);
                        ElementoCaixa = await repo_Elemento.selectFirst(p => p.Id == request.IdCaixa);

                        ElementoCaixa.Poste = ElementoPoste;
                        await repo_Elemento.Update(ElementoCaixa);

                        retorno.IdObjeto = ElementoPoste.Id;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro?.InnerException?.Message;
            }
            return retorno;

        }
        #endregion


        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetElementosMongo")]
        #region GetElementosMongo
        public async Task<List<Elemento_RedeMongo>> GetElementosMongo(long? id = null)
        {
            var retorno = new List<Elemento_RedeMongo>();
            try
            {
                using (ConexaoMongoGeography conexaoMongo = new ConexaoMongoGeography())
                using (var repositoryMongo = new RepositoryMongo<Elemento_RedeMongo>(conexaoMongo))
                {
                    if (id.HasValue)
                        retorno = await repositoryMongo.FindAll(p => p.Id == id);
                    else
                        retorno = await repositoryMongo.FindAll();

                }

            }

            catch (Exception erro)
            {
                _ = erro.Message;
                _ = erro.InnerException.Message;
                throw erro;

            }
            return retorno;
        }


        #endregion


        [HttpGet()]
        [Route("api/Elemento_RedeApi/DeleteFotoArquivo")]

        #region DeleteFotoArquivo

        public async Task<DeleteApiRetorno<Int64?>> DeleteFotoArquivo(long? id_foto)
        {
            DeleteApiRetorno<Int64?> retorno = new DeleteApiRetorno<Int64?>();

            try
            {

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repo = new Conexao<Elemento_RedeFotosArquivos>(sessionT5))
                    {
                        if (id_foto != 0 && id_foto != null)
                        {
                            var obj = await repo.selectFirst(p => p.Id == id_foto);

                            await repo.Remove(obj);
                        }
                        else
                        {
                            throw new Exception(Resources.RsCadastros.NAOENCONTRADO);
                        }
                    }
                }
                retorno.Ok = true;
                retorno.IdObjeto = id_foto;
            }

            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException += erro.InnerException.Message;
            }

            return retorno;
        }
        #endregion


        [HttpPost()]
        [Route("api/Elemento_RedeApi/AlterarStatus")]
        #region AlterarStatus
        public async Task<PostApiRetorno<Elemento_RedeListaModel>> AlterarStatus(AlterarStatusViewModel request)
        {
            PostApiRetorno<Elemento_RedeListaModel> retorno = new PostApiRetorno<Elemento_RedeListaModel>();
            Dictionary<String, object> parametros = new Dictionary<string, Object>();

            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(session))
                {
                    int certificado = 0;

                    StringBuilder sql = new StringBuilder();
                    List<String> sqlUpdate = new List<String>();
                    String ids = $"({string.Join(",", request.ids)})";

                    sql.AppendLine(" UPDATE TB_ELEMENTO_REDE ");
                    sqlUpdate.Add(" SET SITUACAO = @STATUS ");

                    if (request.status == 3 || request.status == 1)
                    {
                        string usuario = "";
                        DateTime dataCertificao = DateTime.Now;
                        certificado = 1;
                        usuario = User.Identity.GetUserId();
                        request.status = 1;

                        sqlUpdate.Add(" CERTIFICADO = @CERTIFICADO ");
                        sqlUpdate.Add(" USUARIO_CERTIFICOU = @USUARIO ");
                        sqlUpdate.Add(" DATAHORA_CERTIFICOU = @DATA ");

                        parametros.Add("@USUARIO", usuario);
                        parametros.Add("@DATA", dataCertificao);
                    }

                    if (request.status == 4)
                    {
                        certificado = 0;
                        sqlUpdate.Add(" CERTIFICADO = @CERTIFICADO ");
                        sqlUpdate.Add(" USUARIO_CERTIFICOU = NULL ");
                        sqlUpdate.Add(" DATAHORA_CERTIFICOU = NULL ");
                    }

                    sql.AppendLine($" {String.Join(", ", sqlUpdate)}")
                       .AppendLine($" WHERE ID IN {ids} ");

                    parametros.Add("@STATUS", request.status);
                    parametros.Add("@CERTIFICADO", certificado);

                    using (var com = session.Connection.CreateCommand())
            {
                       await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null); 
            }

                    foreach (var id in request.ids)
            {
                        var portasAlterar = (await repoPorta.select(p => p.Caixa.Id == id && p.Status == Situacao_Porta.Construido)).ToList();
                        foreach (var item in portasAlterar)
                {
                            item.Status = Situacao_Porta.Disponivel;
                }

                        await repoPorta.Update(portasAlterar);
                    }
            }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        /// <summary>
        /// Coletar projeto, Nome e Descricao do POP cadastrado
        /// </summary>
        /// <param name="codct"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("api/Elemento_RedeApi/GetElemento_RedeByCodCt")]
        #region GetElemento_RedeByCodCt
        public async Task<GetApiRetorno<Elemento_RedeByCodCt>> GetElemento_RedeByCodCt(string codct)
        {
            var retorno = new GetApiRetorno<Elemento_RedeByCodCt>();
            POP pop = null;
            OLT olt = null;
            ContratoAniel contrato = null;
            ProjetoAniel projeto = null;
            //Elemento_Rede elemento = null;

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessiont5 = HibernateUtil.GetSessionT5())

                /*                             
                    =-=-=-          Repository Usage          =-=-=-                                
                 */

                using (var repoPop = new Conexao<POP>(session))
                using (var repoOLT = new Conexao<OLT>(session))
                using (var repoElemento = new Conexao<Elemento_Rede>(sessiont5))
                using (var repoCodCt = new Conexao<ContratoAniel>(sessiont5))
                using (var repoProj = new Conexao<ProjetoAniel>(sessiont5))
                {
                    contrato = await repoCodCt.selectFirst(p => p.CodCt == codct);
                    projeto = await repoProj.selectFirst(p => p.CodCt == codct);
                    pop = await repoPop.selectFirst(p => p.CodCt == codct);
                    if (pop != null)
                    {
                        olt = await repoOLT.selectFirst(p => p.POP.Id == pop.Id);
                    }

                    if (contrato == null) { throw new Exception("Não foi encontrado nenhum Contrato com este Código de Contrato"); }
                    if (!String.IsNullOrEmpty(codct))
                    {
                        retorno.Objeto = new Elemento_RedeByCodCt
                        {
                            CodCt = pop != null ? pop.CodCt : contrato.CodCt,
                            Pop = pop != null ? pop.Id.ToString() : null,
                            Projeto = pop != null ? pop.Projeto : null,
                            NomePop = pop == null ? "" : pop.Nome + " - " + pop.CodCt,
                            Olt = olt != null ? olt.Id.ToString() : null,
                            NomeOlt = olt == null ? "" : olt.Descricao + " - " + olt.Status,
                        };
                    }

                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }

            return retorno;
        }
        #endregion


        [HttpPost()]
        [Route("api/Elemento_RedeApi/GravarFromMap")]
        #region GravarFromMap
        public async Task<PostApiRetorno<Int64>> GravarFromMap(Elemento_RedeMapRequestModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            Elemento_Rede element;
            Modelo_Elemento_Rede modelo = null;

            Tipo_Elemento_Rede tipo = null;
            Elemento_Rede_Porta porta = null;

            List<PostApiMensagem> gravacoes = new List<PostApiMensagem>();
            List<Elemento_Rede_Porta> portas = new List<Elemento_Rede_Porta>();
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {

                    /*             
                    =-=-=-          Repository Usage          =-=-=-                   
                     */
                    using (var repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                    using (var repoArquivo = new Conexao<Elemento_RedeFotosArquivos>(sessionT5))
                    using (var repoModelo = new Conexao<Modelo_Elemento_Rede>(sessionT5))
                    using (var repoTipo = new Conexao<Tipo_Elemento_Rede>(sessionT5))
                    using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                    {

                        tipo = await repoTipo.selectFirst(p => p.Id == request.IdTipo);
                        modelo = await repoModelo.selectFirst(p => p.Id == tipo.Modelo.Id);
                        request.IdModelo = tipo.Modelo.Id;
                        request.Cor_Linha_Ativa = tipo.Cor != null ? tipo.Cor : tipo.Cor_Linha_Ativa;
                        request.Situacao = Status_Elemento.Ativo;
                        element = await repoElemento.selectFirst(p => p.Id == request.Id);

                        if (element == null)
                        {
                            element = new Elemento_Rede
                            {
                                UsuarioCriacao = User.Identity.GetUserId(),
                                DataHoraCriacao = DateTime.Now,
                                Tipo = tipo,
                                //Capacidade = tipo.Capacidade.ToString(),
                            };
                        }
                        else
                        {
                            portas = (await repoPorta.select(p => p.Caixa.Id == element.Id)).ToList();
                            //possuiPortasVinculadas = portas.Where(p => p.Cliente_Plano_Prod != null).Count() > 0 ? true : false;
                        }
                        this.BindViewToObject(element, request, sessionT5);

                        var arquivos = request.Arquivos.Select(p =>
                        {
                            return new Elemento_RedeFotosArquivos
                            {
                                Id = 0,
                                Descricao = p.Descricao,
                                Elemento = element,
                                Tipo_FotoArquivo = p.Tipo,
                                Url = $"{CONSTANTES.CaminhoFisico}/Tmp/Elemento_de_Rede/{p.Descricao}{p.Tipo}",

                            };

                        }).ToList();

                        await repoElemento.SaveOrUpdate(element);

                        gravacoes.Add(await this.GravarArquivosElemento(request.Arquivos, element, sessionT5));
                        if (modelo?.Id == 6)
                        {
                            gravacoes.Add(await this.GravarFibras(element, sessionT5));
                        }
                        else if (modelo?.Id == 7 || modelo?.Id == 4)
                        {
                            gravacoes.Add(await this.GravarPortas(element, portas, sessionT5));
                        }

                        if (request.Conexoes_Request != null)
                        {
                            IEnumerable<Elemento_RedeConexoes> ConexoesObject = (await this.GerarConexoesFromMap(element, request.Conexoes_Request));
                            gravacoes.Add(await this.GravarConexoesElemento(element, ConexoesObject, sessionT5));
                        }

                        if (request.Objeto != null)
                            request.Lista_Objetos = JsonService.DeserializeObject<List<Object>>(request.Objeto);
                        if (request.Lista_Objetos.Count() > 0)
                            await Cadastro_Campos_Dinamicos(request.Lista_Objetos, sessionT5);

                        using (ConexaoMongoGeography conexaoMongo = new ConexaoMongoGeography())
                        using (var repository = new RepositoryMongo<Elemento_RedeMongo>(conexaoMongo))
                        {
                            await AtualizaElemento(element, repository);
                        }

                        retorno.IdObjeto = element.Id;
                        retorno.Mensagem = "";
                        retorno.Ok = true;

                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion


        private async Task<IEnumerable<Elemento_RedeConexoes>> GerarConexoesFromMap(Elemento_Rede element, IEnumerable<Conexao_MapRequestModel> itensListagem)

        {
            #region GerarConexoesFromMap 
            var retorno = new List<Elemento_RedeConexoes>();
            using (ISession session = HibernateUtil.GetSessionT5())

            /*  
                =-=-=-          Repository Usage          =-=-=-          
            */
            using (var repoElement = new Conexao<Elemento_Rede>(session))
            using (var repoDoor = new Conexao<Elemento_Rede_Porta>(session))

            /*                           
                =-=-=-          Api Usage          =-=-=-                 
             */
            using (var PortaApi = new Elemento_Rede_PortaApiController())
            {
                Elemento_Rede cabo = (await repoElement.selectFirst(param => param == element));
                Func<Conexao_MapRequestModel, Task<Elemento_RedeConexoes>> conexaoMapa = async (item) =>
                {


                    var fibra = cabo.Fibras.Where(p => p.Conexao_Ponta_A == null && p.Conexao_Ponta_B == null).First();

                    var elementoOrigem = (await repoElement.selectFirst(param => param.Id == item.Elemento_Origem));
                    var elementoDestino = (await repoElement.selectFirst(param => param.Id == item.Elemento_Destino));
                    return new Elemento_RedeConexoes
                    {
                        Id = 0,

                        Id_Cabo = cabo.Id,
                        Id_FibraUtilizada = fibra.Id,
                        Numero_Fibra = fibra.numFibra,
                        Numero_Loose = fibra.numLoose,

                        Id_Origem = item.Elemento_Origem.Value,
                        Id_Conexao_Origem = item.Porta_Origem.Value,
                        Descricao_Elemento_Origem = elementoOrigem.Descricao,
                        Tipo_Elemento_Origem = elementoOrigem.Tipo.Id,
                        Descricao_Tipo_Elemento_Origem = elementoOrigem.Tipo.Descricao,
                        portaOrigem = Convert.ToInt32(item.Porta_Origem.Value),

                        Id_Destino = item.Elemento_Destino.Value,
                        Id_Conexao_Destino = item.Porta_Destino.Value,
                        Descricao_Elemento_Destino = elementoDestino.Descricao,
                        Tipo_Elemento_Destino = elementoDestino.Tipo.Id,
                        Descricao_Tipo_Elemento_Destino = elementoDestino.Tipo.Descricao,
                        portaDestino = Convert.ToInt32(item.Porta_Destino.Value),

                    };
                };
                retorno = itensListagem.Select((item) => conexaoMapa(item).Result).ToList();
            }
            return retorno;
            #endregion
        }

        [HttpPost()]
        [Route("api/Elemento_RedeApi/SaveDiagram")]
        public async Task<PostApiRetorno> SaveDiagram(Diagrama_FusaoSetFile request)
        {
            #region SaveDiagram

            request.pdfLocation = PdfLocation;
            PostApiRetorno retorno = new PostApiRetorno();
            try

            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Elemento_Rede>(sessionT5))
                using (var repositoryFile = new Conexao<Elemento_RedeFotosArquivos>(sessionT5))
                {

                    Elemento_Rede element = await repository.selectFirst(p => p.Id == request.Id);
                    if (!String.IsNullOrEmpty(request.OrderElements))
                    {
                        element.JSON_Ordenacao_Digarama = request.OrderElements;
                    }



                    Elemento_RedeFotosArquivos file = await repositoryFile.selectFirst(p => p.Descricao == $"{request.Descricao}_{request.Id}.{request.Extensao}");
                    if (file == null)
                    {
                        IO_Diagram(request);
                        file = new Elemento_RedeFotosArquivos
                        {
                            Elemento = await repository.selectFirst(p => p.Id == request.Id),
                            Tipo_FotoArquivo = request.Extensao,
                            Descricao = request.Descricao,
                            Url = $@"{request.pdfLocation}\{request.Descricao}_{request.Id}.{request.Extensao}",
                        };

                    }
                    else
                    {
                        //overwrite file??
                        if (request.OverWrite)
                        {
                            IO_Diagram(request);
                        }


                    }

                    await repository.SaveOrUpdate(element);
                    await repositoryFile.SaveOrUpdate(file);
                }

                retorno.Ok = true;
                retorno.Mensagem = "Diagrama incluido com sucesso!";

            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
            }

            return retorno;
            #endregion
        }


        #region IO_Diagram

        private void IO_Diagram(Diagrama_FusaoSetFile request)
        {
            String _pasta = $@"{request.pdfLocation}";
            if (!Directory.Exists(_pasta))
            {
                Directory.CreateDirectory($"{_pasta}");

            }
            String path = $@"{_pasta}\{request.Descricao}_{request.Id}.{request.Extensao}";
            String path_HTML = $@"{_pasta}\{request.Descricao}_{request.Id}.html";
            //Método Novo
            string html = $"<img src='{request.Context},{request.Base64Image}' width='{request.Largura}' height='{request.Altura}'>";
            System.IO.File.WriteAllText(path_HTML, html);

            if (Convert.ToInt32(request.Largura.Replace("px", "")) > Convert.ToInt32(request.Altura.Replace("px", "")))
            {
                ConvertToPdfService.convertHtmlToPdfLandscape(path, path_HTML);
            }
            else
            {
                ConvertToPdfService.convertHtmlToPdf(path, path_HTML);
            }

            System.IO.File.Delete(path_HTML);

        }

        #endregion

        [HttpPost()]
        [Route("api/Elemento_RedeApi/RemoverConexao")]
        public async Task<PostApiRetorno<Int64>> RemoverConexao(Request_Remover_Conexao_Elemento request)
        {
            #region remover conexões

            var retorno = new PostApiRetorno<long>();
            try
            {

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repo_Elemento = new Conexao<Elemento_Rede>(sessionT5))
                    //using (var repo_Cable = new Conexao<Elemento_Rede_Cabo>(sessionT5))
                    using (var repo_Fiber = new Conexao<Elemento_Rede_Fibra>(sessionT5))
                    {
                        var Elemento = await repo_Elemento.selectFirst(p => p.Id == request.Id_elemento);
                        var Cabo = await repo_Elemento.selectFirst(p => p.Id == request.Id_cabo);
                        var fibra = Cabo.Fibras.FirstOrDefault(p => p.numFibra == request.Num_Fibra);

                        fibra.Ponta_A = null;
                        fibra.Conexao_Ponta_A = null;
                        fibra.Ponta_B = null;
                        fibra.Conexao_Ponta_B = null;

                        await repo_Fiber.SaveOrUpdate(fibra);

                        retorno.IdObjeto = fibra.Id;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception)
            {

                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }
            return retorno;
            #endregion
        }

        #region Obsoletos


        [Obsolete("Endpoint foi substituído pelo Endpoint gravarFromMap")]
        [HttpPost()]
        [Route("Gravar_Elemento_Mapa")]
        #region Gravar_Elemento_Mapa
        public async Task<PostApiRetorno<Elemento_Rede>> Gravar_Elemento_Mapa(ParamWebApi<Elemento_RedeCadastroModel> request)
        {
            var retorno = new PostApiRetorno<Elemento_Rede>();

            Elemento_Rede element;
            Tipo_Elemento_Rede tipo = null;
            List<Elemento_Rede_Porta> portas = new List<Elemento_Rede_Porta>();
            //bool possuiPortasVinculadas = false;

            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {

                    /*             
                        =-=-=-          Repository Usage          =-=-=-                  
                     */

                    using (var repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                    using (var repoArquivo = new Conexao<Elemento_RedeFotosArquivos>(sessionT5))
                    using (var repoModelo = new Conexao<Modelo_Elemento_Rede>(sessionT5))
                    using (var repoTipo = new Conexao<Tipo_Elemento_Rede>(sessionT5))
                    using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                    {
                        element = await repoElemento.selectFirst(p => p.Id == request.Objeto.Id);

                        if (element == null)
                        {
                            element = new Elemento_Rede
                            {
                                //UsuarioCriacao = request.Usuario,
                                DataHoraCriacao = DateTime.Now,
                                Tipo = tipo,
                                //Capacidade = tipo.Capacidade.ToString(),
                            };
                        }

                        BindViewToObject(element, request.Objeto, sessionT5);
                    }

                    retorno.Ok = true;

                }

            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;

        }
        #endregion

        [Obsolete("Método obsoleto devido a revisão do recurso com P&D")]
        [HttpPost()]
        [Route("api/Elemento_RedeApi/Requisicao_ViabilidadeGeosite")]
        #region ViabilidadeGeosite
        public async Task<GetApiRetorno<List<CaixaGeosite>>> Requisicao_ViabilidadeGeosite(Geosite_viabilidadePorta request)
        {
            GetApiRetorno<List<CaixaGeosite>> retorno = new GetApiRetorno<List<CaixaGeosite>>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IntegracoesExternas obj = new IntegracoesExternas();
                    IntegracaoExternaGeositeService integracao = new IntegracaoExternaGeositeService(session);

                    using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                    {
                        obj = (IntegracoesExternas)(await integracao.GetListConfiguracoes(com, IntegracaoExterna.GEOSITE)).FirstOrDefault();
                    }

                    using (var repoConfiguracoes = new Conexao<Configuracoes_Gerais>(session))
                    //using (var repoCliente_Plano = new Conexao<Cliente_PlanoProduto>(session))                    
                    {

                        Configuracoes_Gerais config = await repoConfiguracoes.selectFirst();
                        //Cliente_PlanoProduto Cpp = await repoCliente_Plano.selectFirst();
                        Request_ViabilidadeGeosite requestViabilidade = new Request_ViabilidadeGeosite
                        {

                            Raio = Convert.ToDouble(config.Disponibilidade),
                            Latitude = String.IsNullOrEmpty(request?.RequestEndereco?.Latitude) ? (double?)null :
                            Convert.ToDouble(request?.RequestEndereco?.Latitude),

                            Longitude = String.IsNullOrEmpty(request?.RequestEndereco?.Longitude) ? (double?)null :
                            Convert.ToDouble(request?.RequestEndereco?.Longitude),

                            GooglePlaceId = request?.RequestGooglePlace,

                        };

                        requestViabilidade.Endereco = $"{request?.RequestEndereco?.Complemento}," + " " +
                                                      $"{request?.RequestEndereco?.Numero}," + " " +
                                                      $"{request?.RequestEndereco?.Bairro}," + " " +
                                                      $"{request?.RequestEndereco?.Cidade} - {request?.RequestEndereco?.UF}";

                        requestViabilidade.Mudanca_RequisicaoViabilidade = integracao.SelectParameterRequisicao(requestViabilidade);

                        ResponseGeositeViabilidade response = integracao.Check_Viabilidade(obj, requestViabilidade);

                        if (response.success)
                        {
                            retorno.Ok = true;
                            retorno.Objeto = response.caixasGeosite;
                            retorno.Mensagem = $"Foram encontradas {response.caixasGeosite.Count} CTOs dentro do raio configurado";
                        }
                        else
                        {
                            throw new Exception($"{response.message}");
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
            }

            return retorno;
        }
        #endregion

        [Obsolete("Estrutura das Fibras mudou, tornando esse método desatualizado.")]
        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetComboFibra")]
        #region GetCombo_Cabo
        public async Task<Elemento_ObjCombo<Int64>> GetComboFibra(RequestObjComboConfig<String, RequestElementoComboConfig> request)
        {
            Elemento_ObjCombo<Int64> retorno = new Elemento_ObjCombo<Int64>
            {
                results = new List<Elemento_ObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ELEMENTO_REDE A ");

                    StringBuilder sqlWhere = new StringBuilder();

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }


                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    condicoes.Add("C.ID = 7");
                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.DESCRICAO, A.CONTRATO, A.PROJETO, A.POP, A.OLT,
                                                                A.LATITUDE, A.LONGITUDE, B.ID_MODELO, B.DESCRICAO DESCRICAO_TIPO,
                                                                C.DESCRICAO DESCRICAO_MODELO ");

                        sqlFrom.AppendLine("  LEFT JOIN TB_TIPO_ELEMENTO_REDE B ")
                        .AppendLine(" ON (A.ID_TIPO = B.ID) ")
                        .AppendLine(" LEFT JOIN TB_MODELO_ELEMENTO_REDE C ON (B.ID_MODELO = C.ID) ");

                        sql.AppendLine(sqlFrom.ToString());

                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(sqlWhere.ToString());
                        }

                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var modelo = Convert.ToString(reader["DESCRICAO_MODELO"]);
                                var obj = new Elemento_ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Contrato = Convert.ToString(reader["CONTRATO"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Pop = Convert.ToString(reader["POP"]),
                                    Olt = Convert.ToString(reader["OLT"]),
                                    Modelo = modelo,
                                };
                                retorno.results.Add(obj);
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
        #endregion


        #endregion

        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetComboOLT")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<long, long> request)
        {

            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ELEMENTO_REDE A ");

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<long>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
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

        [HttpGet]
        [Route("api/Elemento_RedeApi/Delete_Endereco_Vinculado")]
        public async Task<DeleteApiRetorno<Int64?>> Delete_Endereco_Vinculado(long? Id_Endereco_Logradouro)
        {
            var retorno = new DeleteApiRetorno<long?>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repo = new Conexao<Elemento_Rede_Enderecos>(sessionT5))
                {

                    if (Id_Endereco_Logradouro != 0 && Id_Endereco_Logradouro != null)
                    {
                        var obj = await repo.selectFirst(p => p.Id == Id_Endereco_Logradouro);

                        await repo.Remove(obj);

                        retorno.Ok = true;
                        retorno.IdObjeto = obj.Id;
                    }
                    else
                    {
                        throw new Exception(Resources.RsCadastros.NAOENCONTRADO);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.IdObjeto = null;
                retorno.Mensagem = ex.Message;
            }
            return retorno;

        }

        #region Mobile      


        ///  Listagem de caixas e planos/produtos disponíveis dentro do raio de lat/long
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/Elemento_RedeApi/ConsultaViabilidade")]
        #region ConsultaViabilidade
        public async Task<GetApiRetorno<Consulta_ViabilidadeModel>> ConsultaViabilidade(string latitude, string longitude, long? idCondominio)
        {
            GetApiRetorno<Consulta_ViabilidadeModel> retorno = new GetApiRetorno<Consulta_ViabilidadeModel>();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoElemento = new Conexao<Elemento_Rede>(sessionT5))
            using (var repoPortas = new Conexao<Elemento_Rede_Porta>(sessionT5))
            using (var RepoCobertura = new Conexao<Area_Cobertura>(sessionT5))
            using (var RepoCoberturaProd = new Conexao<Area_Cobertura_Plano_Produto>(sessionT5))
            using (var repoPlano = new Conexao<Plano>(session))
            using (var repoProduto = new Conexao<Produto>(session))
            using (var repoCondominio = new Conexao<Condominio_Predio>(session))
            using (var repoCaixaCondominio = new Conexao<Condominio_Predio_Caixa>(sessionT5))
            {
                try
                {
                    Configuracoes_Gerais config = await repoConfig.selectFirst();
                    if (!config.Disponibilidade.HasValue)
                    {
                        throw new Exception("Raio limite para consulta de disponibilidade de caixas não informado");
                    }
                    var raio = config.Disponibilidade / 1000.0;

                    IList<Elemento_Rede> caixasRaio = new List<Elemento_Rede>();

                    double latitudeDouble;
                    double longitudeDouble;
                    double.TryParse(latitude.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out latitudeDouble);
                    double.TryParse(longitude.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out longitudeDouble);

                    // Calculo da lat/long mínima e máxima
                    var kmInLongitudeDegree = 111.320 * Math.Cos(latitudeDouble / 180.0 * Math.PI);

                    var deltaLat = raio / 111.1;
                    var deltaLong = raio / kmInLongitudeDegree;

                    var minLat = latitudeDouble - deltaLat;
                    var maxLat = latitudeDouble + deltaLat;
                    var minLong = longitudeDouble - deltaLong;
                    var maxLong = longitudeDouble + deltaLong;

                    if (config.Utiliza_Cadastro_Endereco && idCondominio.HasValue)
                    {
                        var condominio = await repoCondominio.selectFirst(p => p.Id == idCondominio.Value);
                        if (condominio.Definir_Caixa_Condominio_Predio)
                        {
                            caixasRaio = (await repoCaixaCondominio.select(c => c.Id_Condominio_Predio == condominio.Id))?.Select(p => p.Id_Caixa).ToList();

                        }
                        else
                        {
                            caixasRaio = await repoElemento.select(p =>
                            //p.Tipo.E_Caixa &&
                            p.Capacidade > 0 &&
                            (p.Latitude >= minLat) &&
                            (p.Latitude <= maxLat) &&
                            (p.Longitude >= minLong) &&
                            (p.Longitude <= maxLong));
                        }
                    }
                    else
                    {
                        caixasRaio = await repoElemento.select(p =>
                            //p.Tipo.E_Caixa == true &&
                            p.Capacidade > 0 &&
                            (p.Latitude >= minLat) &&
                            (p.Latitude <= maxLat) &&
                            (p.Longitude >= minLong) &&
                            (p.Longitude <= maxLong)
                        );
                    }

                    List<Consulta_ViabilidadeCaixa> caixasViaveis = new List<Consulta_ViabilidadeCaixa>();

                    foreach (var caixa in caixasRaio)
                    {
                        var portasDisponiveis = await repoPortas.select(p =>
                            p.Caixa.Id == caixa.Id &&
                            p.Status == Situacao_Porta.Disponivel
                        );

                        if (portasDisponiveis.Count() > 0)
                        {
                            var distanciaCaixa = MapaService.GetMenorDistancia(latitudeDouble,
                                                                   longitudeDouble,
                                                                   caixa.Latitude,
                                                                   caixa.Longitude) / 1852;

                            caixasViaveis.Add(new Consulta_ViabilidadeCaixa
                            {
                                Id = caixa.Id,
                                Descricao = caixa.Descricao,
                                Latitude = caixa.Latitude,
                                Longitude = caixa.Longitude,
                                PortasDisponiveis = portasDisponiveis.Select(p => p.Porta).ToList(),
                                DistanciaKm = (Int32)distanciaCaixa
                            });
                        }
                    }

                    var areas = await RepoCobertura.select();
                    var idsAreas = new List<long>();

                    foreach (var area in areas)
                    {
                        using (GeographyPattern geographyHandler = new GeographyPattern())
                        {
                            if (!String.IsNullOrEmpty(area.Coordenadas))
                            {
                                // Verifica se são vários polígonos
                                if (area.Coordenadas.StartsWith("[[") && area.Coordenadas.EndsWith("]]"))
                                {
                                    var poligonos = JsonConvert.DeserializeObject<double[][]>(area.Coordenadas);
                                    for (int i = 0; i < poligonos.Length; i++)
                                    {
                                        List<Location> locs = new List<Location>();
                                        for (var j = 0; j < poligonos[i].Length; j += 2)
                                        {
                                            locs.Add(new Location(poligonos[i][j], poligonos[i][j + 1]));
                                        }
                                        var contem = geographyHandler.IsPointInPolygon(locs, new Location(latitudeDouble, longitudeDouble));
                                        if (contem)
                                        {
                                            idsAreas.Add(area.Id);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var poligono = JsonConvert.DeserializeObject<double[]>(area.Coordenadas);
                                    List<Location> locs = new List<Location>();
                                    for (var i = 0; i < poligono.Length; i += 2)
                                    {
                                        locs.Add(new Location(poligono[i], poligono[i + 1]));
                                    }

                                    var contem = geographyHandler.IsPointInPolygon(locs, new Location(latitudeDouble, longitudeDouble));
                                    if (contem) { idsAreas.Add(area.Id); }

                                }
                            }
                        }
                    }

                    List<Consulta_ViabilidadePlanoProduto> planosProdutos = new List<Consulta_ViabilidadePlanoProduto>();
                    if (idsAreas.Count > 0)
                    {
                        foreach (var idArea in idsAreas)
                        {
                            var planosProdutosArea = await RepoCoberturaProd.select(p => p.Area.Id == idArea);

                            foreach (var prod in planosProdutosArea)
                            {
                                var plano = await repoPlano.selectFirst(p => p.Id == Convert.ToInt64(prod.Codigo_Plano_Produto));

                                if (plano != null)
                                {
                                    if (planosProdutos.Find(p => p.Id == plano.Id) == null)
                                    {
                                        planosProdutos.Add(new Consulta_ViabilidadePlanoProduto
                                        {
                                            Id = plano.Id,
                                            Descricao = plano.Descricao,
                                            Valor = Convert.ToString(plano.ValorPlano)
                                        });
                                    }
                                }
                                else
                                {
                                    var produto = await repoProduto.selectFirst(p => p.Id == Convert.ToInt64(prod.Codigo_Plano_Produto));

                                    if (produto != null)
                                    {
                                        if (planosProdutos.Find(p => p.Id == produto.Id) == null)
                                        {
                                            planosProdutos.Add(new Consulta_ViabilidadePlanoProduto
                                            {
                                                Id = produto.Id,
                                                Descricao = produto.Descricao,
                                                Valor = Convert.ToString(produto.ValorProduto)
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var consultaViabilidade = new Consulta_ViabilidadeModel
                    {
                        Caixas = caixasViaveis,
                        PlanosProdutos = planosProdutos
                    };

                    retorno.Objeto = consultaViabilidade;
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
        #endregion

        [AllowAnonymous]
        [HttpGet]
        [Route("api/Elemento_RedeApi/GetRegrasExibicaoCTO")]
        public async Task<Response<RegrasExibicaoCTOModel>> GetRegrasExibicaoCTO()
        {
            Response<RegrasExibicaoCTOModel> retorno = new Response<RegrasExibicaoCTOModel>();
            try
            {
                using(var session = HibernateUtil.GetSession())
                using (var configApi = new Configuracoes_GeraisApiController())
                {
                    var configGeral = (await configApi.FindById(1)).Objeto;

                    var regrasExibicaoCaixa = new RegrasExibicaoCTOModel()
                    {
                        ExibirApenasCaixasDentroRaio = configGeral.Exibir_Apenas_Caixas_Atendimento_Dentro_Raio,
                        LimiteDistanciaCaixa = !String.IsNullOrEmpty(configGeral.LimiteDistanciaCaixaAtendimento) ? Convert.ToInt32(configGeral.LimiteDistanciaCaixaAtendimento) : (int?)null
                    };

                    retorno.valor = regrasExibicaoCaixa;
                }

                retorno.result = true;
            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.result = false;
                retorno.message = ex.Message;

                if (ex.InnerException != null)
                    retorno.exception = ex.InnerException.Message;
            }

            return retorno;
        }

        #endregion

        private Dictionary<String, Object> Cores = Enum.GetNames(typeof(Padrao_ABNT)).Select(p =>
        #region DictionaryColors
        {
            var Value = "";
            switch (p.ToUpper())
            {
                case "AMARELO": Value = "#ffff00"; break;
                case "AQUA": Value = "#00ffff"; break;
                case "AZUL": Value = "#0000ff"; break;
                case "BRANCO": Value = "#ffffff"; break;
                case "CINZA": Value = "#808080"; break;
                case "LARANJA": Value = "#ffa500"; break;
                case "MARROM": Value = "#964b00"; break;
                case "PRETO": Value = "#000000"; break;
                case "ROSA": Value = "#ffcbdb"; break;
                case "VERDE": Value = "#008000"; break;
                case "VERMELHO": Value = "#ff0000"; break;
                case "VIOLETA": Value = "#ee82ee"; break;
            }
            return new KeyValuePair<string, Object>(p, Value);
        }).ToDictionary(param => param.Key, param => param.Value);
        #endregion
        private static String PdfLocation => $"{CONSTANTES.CaminhoFisico}/Tmp/Elemento_de_Rede";

        [HttpPost()]
        [Route("api/Elemento_RedeApi/ImportarElementoRedebyApi")]
        public async Task<PostApiRetorno<List<ResultElementoIntegracao>>> ImportarElementoRedebyApi(SincronizarElementosRedeIntegracoes request)
        {
            PostApiRetorno<List<ResultElementoIntegracao>> response = new PostApiRetorno<List<ResultElementoIntegracao>>();
            List<ResultElementoIntegracao> elementoResults = new List<ResultElementoIntegracao>();
            var connectionString = CONSTANTES.VoalleConnectionString;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoDocConexao = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    ILogRepository repoLog = new LogRepository(sessionLog);
                    IConfiguracoesGeraisRepository repoConfigGerais = new ConfiguracoesGeraisRepository(session);
                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    //List<Config_Aniel_Integrator> configs = await ObterConfigAnielIntegrator(sessionT5);
                    var configEmpresa = repoConfigGerais.GetConfiguracoesGerais();
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas dadosIntegracaoVoalle = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.VOALLE);
                    IntegracoesExternas dadosIntegracaoIxc = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.IXC);
                    IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.HUBSOFT);
                   

                    var elements = request.ElementosRede.Split(',');

                    if (dadosIntegracaoHubsoft != null)
                    {
                        var loginHubsoft = new LoginHubsoftModel
                        {
                            client_id = dadosIntegracaoHubsoft.TokenAcesso,
                            client_secret = dadosIntegracaoHubsoft.ApiSecret,
                            username = dadosIntegracaoHubsoft.Usuario,
                            password = dadosIntegracaoHubsoft.Senha
                        };

                        foreach (var element in elements)
                        {
                            Response<string> retornoImportacaoElementoRede = new Response<string>();
                            try
                            {
                                IElementoRedeAnielRepository repository = new ElementoRedeAnielRepository(sessionT5, new IntegracaoElementoRedeHubSoft(dadosIntegracaoHubsoft.UrlApi));
                                ElementoRedeBusiness business = new ElementoRedeBusiness(repository, this.Cores);
                                retornoImportacaoElementoRede = business.ImportaElementosRede(dadosIntegracaoHubsoft.UrlApi, element, loginHubsoft, CONSTANTES.PadraoIntegracaoTipo);

                                if (retornoImportacaoElementoRede.result)
                                {
                                    string message = null;
                                    if (retornoImportacaoElementoRede.valor == "atualizacao")
                                    {
                                        message = $"{element} atualizado com sucesso";
                                    }
                                    else
                                    {
                                        message = $"{element} cadastrado com sucesso";
                                    }

                                    elementoResults.Add(new ResultElementoIntegracao { ElementoRede = element, Mensagem = message, Situacao = true });
                                }
                                else
                                {
                                    string message = retornoImportacaoElementoRede.message;
                                    elementoResults.Add(new ResultElementoIntegracao { ElementoRede = element, Mensagem = message, Situacao = false });
                                }
                            }
                            catch (Exception erro)
                            {
                                retornoImportacaoElementoRede.message = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                                retornoImportacaoElementoRede.result = false;
                            }

                        }
                    }
                    else
                    {
                        throw new Exception("Configuração Hubsoft não encontrada.");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message);
                response.Mensagem = ex.Message;
                response.Ok = false;
                return response;
            }

            response.IdObjeto = elementoResults.OrderByDescending(x => x.Situacao).ToList();
            response.Ok = true;
            return response;
        }
    }

    #region LocalInterface
    public interface IElemento_RedeApi
    {
        /// <summary>
        /// Listagem de todos os Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<DataTableResponse<Elemento_RedeListaModel>> GetAll(Elemento_RedeDataTableRequest request);
        /// <summary>
        /// GetCombo dos Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Elemento_ObjCombo<Int64>> GetCombo(RequestObjComboConfig<String, RequestElementoComboConfig> request);
        /// <summary>
        /// GetCombo dos Elementos de Rede Caixa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Elemento_ObjCombo<Int64>> GetComboCaixa(RequestObjComboConfig<String, RequestElementoComboConfig> request);
        /// <summary>
        /// Combo das Fibras sem Conexão
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ObjCombo<Int64>> GetComboConexoesFibra(RequestObjComboPai<long?, long?> request);
        /// <summary>
        /// Coletar projeto, Nome e Descricao do POP cadastrado
        /// </summary>
        /// <param name="codct"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<Elemento_RedeByCodCt>> GetElemento_RedeByCodCt(string codct);
        /// <summary>
        /// EndPoint para Alteração de Status
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PostApiRetorno<Elemento_RedeListaModel>> AlterarStatus(AlterarStatusViewModel request);
        /// <summary>
        /// Deleta registros por Id do Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<DeleteApiRetorno<Int64>> Excluir(ParamWebApi<Int64> value);
        /// <summary>
        /// Remover uma Imagem para um elemento de Rede Específico
        /// </summary>
        /// <param name="id_foto"></param>
        /// <returns></returns>
        public Task<DeleteApiRetorno<Int64?>> DeleteFotoArquivo(long? id_foto);
        /// <summary>
        /// Captura os latlongs do Banco Mongo para exibição no mapa
        /// </summary>
        /// <returns></returns>
        public Task<List<Elemento_RedeMongo>> GetElementosMongo(long? id = null);
        /// <summary>
        /// Vincula registro de Poste ao registro de caixa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PostApiRetorno<Int64>> VincularPosteCaixa(RequestElementoVincularPosteCaixa request);
        /// <summary>
        /// Encontrar Elemento de Rede pelo Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<Elemento_RedeCadastroModel>> FindById(Int64? Id);
        public Task<GetApiRetorno<List<Elemento_RedeMongo>>> NoSqlQueryMongo(Elemento_RedeMongo request);
        public Task AtualizaElemento(Elemento_Rede de, RepositoryMongo<Elemento_RedeMongo> repository);
        public Task<String> AtualizaTodosElementosMongo();
        /// <summary>
        /// Elaboração do diagrama de Fusão
        /// </summary>
        /// <param name="request">Request do Elemento de Rede</param>
        /// <returns></returns>
        public Task<GetApiRetorno<DiagramaFusaoConexoesModel>> Ellaborate_DiagramaFusao([FromBody] long request);
        /// <summary>
        /// Gravar um novo elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Elemento_RedeCadastroModel> request);
        /// <summary>
        /// Listagem de Contratos em formato de Árvore 
        /// </summary>
        /// <returns></returns>
        public Task<GetApiRetorno<List<Elemento_RedeTreeViewModel>>> GetTreeView_Gestao(String Text_Reference = null);



    }

    #endregion

}
