using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class LegendaTimelineTipoServicosApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/LegendaTimelineTipoServicosApi/GetAll")]
        public async Task<DataTableResponse<LegendaTimelineTipoServicos_ListModel>> GetAll(LegendaTimelineTipoServicos_DataTableRequest request)
        {

            List<LegendaTimelineTipoServicos_ListModel> retorno = new List<LegendaTimelineTipoServicos_ListModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.Append("SELECT COUNT(TSE.COD_TIPO_SERV) CO ");
                    sql.Append("FROM TB_TIPO_SERVICO_EQUIPE TSE ");
                    sql.Append("INNER JOIN TB_NATUREZA_PRODUCAO NP ON TSE.COD_NATUREZA = NP.ID ");
                    sql.Append("LEFT JOIN TB_CONTRATO C ON C.CODCT = NP.CODCT ");
                    sql.Append("LEFT JOIN TB_LEGENDA_TIM_TIPO_SERV LTTS ON LTTS.ID_TP_SERV = TSE.COD_TIPO_SERV ");

                    using var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);
                    if (await reader.ReadAsync())
                    {
                        recordsTotal = Convert.ToInt32(reader["CO"]);
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.Append("SELECT TSE.COD_TIPO_SERV, C.CODCT, C.DESCRICAO AS CONTRATO, NP.DESCRICAO AS NATUREZA, TSE.DESCRICAO AS TIPO_SERVICO, ");
                    sql.Append("LTTS.COR, TSE.SITUACAO ");
                    sql.Append("FROM TB_TIPO_SERVICO_EQUIPE TSE ");
                    sql.Append("INNER JOIN TB_NATUREZA_PRODUCAO NP ON TSE.COD_NATUREZA = NP.ID ");
                    sql.Append("LEFT JOIN TB_CONTRATO C ON C.CODCT = TSE.CODCT ");
                    sql.Append("LEFT JOIN TB_LEGENDA_TIM_TIPO_SERV LTTS ON LTTS.ID_TP_SERV = TSE.COD_TIPO_SERV ");

                    //Filtrando
                    List<String> filtros = new List<string>();
                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        filtros.Add("TSE.SITUACAO = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        filtros.Add("TSE.CODCT = @CODCT");
                        parametros.Add("@CODCT", request.Contrato);
                    }
                    
                    if (!String.IsNullOrEmpty(request.Natureza))
                    {
                        filtros.Add("TSE.COD_NATUREZA = @CODNATUREZA");
                        parametros.Add("@CODNATUREZA", request.Natureza);
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        filtros.Add("UPPER(TSE.DESCRICAO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (filtros.Count > 0)
                    {
                        sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }

                    //Busca com paginacao
                    using var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start);
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new LegendaTimelineTipoServicos_ListModel()
                        {
                            Id = reader["COD_TIPO_SERV"] != DBNull.Value ? Convert.ToInt64(reader["COD_TIPO_SERV"]) : 0,
                            CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : string.Empty,
                            Contrato = reader["CONTRATO"] != DBNull.Value ? Convert.ToString(reader["CONTRATO"]) : string.Empty,
                            Natureza = reader["NATUREZA"] != DBNull.Value ? Convert.ToString(reader["NATUREZA"]) : string.Empty,
                            TipoServico = reader["TIPO_SERVICO"] != DBNull.Value ? Convert.ToString(reader["TIPO_SERVICO"]) : string.Empty,
                            Cor = reader["COR"] != DBNull.Value ? Convert.ToString(reader["COR"]) : "#A9A9A9",
                            Status = reader["SITUACAO"] != DBNull.Value && Convert.ToString(reader["SITUACAO"]) == "S" ? "Ativo" : "Inativo",
                        });
                    }

                }
            }

            
            //Page e retornando DataTable
            return new DataTableResponse<LegendaTimelineTipoServicos_ListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }



        [HttpPost]
        [Route("api/LegendaTimelineTipoServicosApi/Gravar")]
        public async Task<PostApiRetorno<long>> Gravar(GravarListaViewModel value)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var repositoryLegenda = new Conexao<LegendaTimelineTipoServicos>(session))
                using (var repositoryContrato = new Conexao<ContratoAniel>(session))
                using (var repositoryNatureza = new Conexao<Natureza_ProducaoAniel>(session))
                using (var repositoryTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
                {
                    foreach (var item in value.ids)
                    {
                        var tipoServico = await repositoryTipoServico.selectFirst(p => p.Cod_Tipo_Serv == item);

                        if (tipoServico == null)
                        {
                            continue;
                        }

                        var modelo = await repositoryLegenda.selectFirst(p => p.Tipo_Servico_Equipe.Cod_Tipo_Serv == item)
                                    ?? new LegendaTimelineTipoServicos();

                        modelo.Cor = value.cor;
                        modelo.Contrato = await repositoryContrato.selectFirst(p => p.CodCt == tipoServico.CodCt);
                        modelo.Natureza_Producao = await repositoryNatureza.selectFirst(p => p.Id == tipoServico.Cod_Natureza);
                        modelo.Tipo_Servico_Equipe = tipoServico;

                        await repositoryLegenda.SaveOrUpdate(modelo);
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message ?? string.Empty;
                retorno.Ok = false;
            }

            return retorno;
        }

    }
}