using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    public class Faturamento_ClienteApiController : BaseApiController
    {
        public async Task<List<Faturamento_Log>> GetLog(Int64? IdFaturaCliente, Int64? Id_Cliente, Int64 Id_Protocolo, String UrlContasReceber)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                List<Faturamento_Log> retorno = new List<Faturamento_Log>();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT A.DATAHORACRIACAO, A.NOMEFANTASIA, A.OBSERVACAO, A.USUARIOCRIACAO ")
                                            .AppendLine("FROM TB_FATURAMENTO_LOG A ")
                                            .AppendLine("WHERE A.ID_PROTOCOLO = @ID_PROTOCOLO ");

                    if (Id_Cliente.HasValue)
                    {
                        sql.AppendLine("  AND A.ID_CLIENTE = @ID_CLIENTE");
                        parametros.Add("@ID_CLIENTE", Id_Cliente);
                    }

                    if (IdFaturaCliente.HasValue)
                    {
                        sql.AppendLine("  AND A.ID_FATURA_CLIENTE = @ID_FATURA_CLIENTE");
                        parametros.Add("@ID_FATURA_CLIENTE", IdFaturaCliente);
                    }


                    parametros.Add("@ID_PROTOCOLO", Id_Protocolo);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        //,
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Faturamento_Log
                            {
                                DataHoraCriacao = Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm"),
                                NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                Observacao = Convert.ToString(reader["OBSERVACAO"]).Replace("[cr]", "").Replace("[/cr]", ""),//ReplaceTagsContasReceber(Convert.ToString(reader["OBSERVACAO"]), UrlContasReceber),
                                Usuario = Convert.ToString(reader["USUARIOCRIACAO"])
                            });
                        }
                    }

                }
                return retorno;
            }
        }

        private String ReplaceTagsContasReceber(String Texto, String UrlContasReceber)
        {
            //Contas a Receber [cr][/cr]
            var result = Regex.Replace(Texto, @"\[cr](.*?)\[/cr]",
                m =>
                {
                    var codeString = m.Groups[1].Value;

                    var url = $"{UrlContasReceber}/{codeString}";
                    var link = "addTabs({ id: 'null',title: '" + RsFinanceiro.CONTAS_RECEBER + "',close: true, urlType: 'absolute'});";
                    //var link = "addTabs({ id: 'null',title: '" + RsFinanceiro.CONTAS_RECEBER + "',close: true, url: '" + url + "', urlType: 'absolute'});";

                    return $"<a href=\"#\" onclick=\"{link}\">{codeString}</a>";

                });

            return result;
        }

        [Route("api/Faturamentoapi/ModalDetalhesFaturamentoAvulso")]
        public async Task<List<Faturamento_Modal_Detalhes>> ModalDetalhesFaturamentoAvulso(Int64 Id_Faturamento_Cliente)
        {
            List<Faturamento_Modal_Detalhes> retorno = new List<Faturamento_Modal_Detalhes>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFatCliAvulso = new Conexao<Faturamento_Cliente_Avulso_Det>(session))
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                var totalRecorrencias = 0;
                var firstFatCliAvulso = await repoFatCliAvulso.selectFirst(x => x.Faturamento_Cliente.Id == Id_Faturamento_Cliente);

                if (firstFatCliAvulso != null)
                {
                    totalRecorrencias = await repoFatCliAvulso.Count(x => x.Faturamento_Protocolo == firstFatCliAvulso.Faturamento_Protocolo);
                }

                //Select

                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT J.VALOR_AVULSO AS VALOR_AVULSO, J.DATAHORA_ABERTURA AS ABERTURA_AVULSO, J.DATAHORA_FECHAMENTO AS FECHAMENTO_AVULSO,  ")
                                 .AppendLine(" J.NUM_SEQ_RECORRENCIA AS NUM_RECORRENCIA, J.UTILIZA_RECORRENCIA AS UTILIZA_RECO, H.DESC_SERV_UTILIZADOS AS DESCRICAO ")
                                 .AppendLine(" FROM TB_FATURAMENTO_CLIENTE F ")
                                 .AppendLine(" INNER JOIN TB_FATURAMENTO_FORMA_PAGAMENTO G ")
                                 .AppendLine(" ON (G.ID = F.ID_FATURAMENTO_FORMA) ")
                                 .AppendLine(" INNER JOIN TB_FATURAMENTO_PROTOCOLO H ")
                                 .AppendLine(" ON (H.ID = G.ID_FATURAMENTO_PROTOCOLO) ")
                                 .AppendLine(" LEFT JOIN TB_FATURAMENTO_CLI_AVULSODT J ")
                                 .AppendLine(" ON (J.ID_FAT_CLI = F.ID) ")
                                 .AppendLine(" WHERE F.ID = @ID_FATURAMENTO_CLIENTE AND J.VALOR_AVULSO != 0");

                    parametros.Add("@ID_FATURAMENTO_CLIENTE", Id_Faturamento_Cliente);

                    //Busca com paginacao

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var dias_a_faturar = (Convert.ToDateTime(reader["FECHAMENTO_AVULSO"]).Date.AddHours(24) - Convert.ToDateTime(reader["ABERTURA_AVULSO"]).Date).Days;
                            var dias_a_faturar_string = Convert.ToString(dias_a_faturar);
                            var periodoMedicao = reader["ABERTURA_AVULSO"] != DBNull.Value && reader["FECHAMENTO_AVULSO"] != DBNull.Value ? Convert.ToDateTime(reader["ABERTURA_AVULSO"]).ToString("dd/MM") + " - " + Convert.ToDateTime(reader["FECHAMENTO_AVULSO"]).ToString("dd/MM") : "";
                            if (dias_a_faturar_string[0] == '-') dias_a_faturar_string = dias_a_faturar_string.Remove(0, 1);

                            retorno.Add(new Faturamento_Modal_Detalhes
                            {
                                DataHoraCriacao = reader["ABERTURA_AVULSO"] != null ? Convert.ToDateTime(reader["ABERTURA_AVULSO"]).ToString("dd/MM") : "-",
                                DataRegistro = reader["FECHAMENTO_AVULSO"] != null ? Convert.ToDateTime(reader["FECHAMENTO_AVULSO"]).ToString("dd/MM") : "-",
                                Descricao = reader["DESCRICAO"] != null ? Convert.ToString(reader["DESCRICAO"]) : "AVULSO",
                                Valor = reader["VALOR_AVULSO"] != null ? String.Format("{0:N2}", reader["VALOR_AVULSO"]) : "-",
                                PeriodoMedicao = periodoMedicao,
                                QuantidadeDiasFaturar = reader["ABERTURA_AVULSO"] != DBNull.Value && reader["FECHAMENTO_AVULSO"] != DBNull.Value ? dias_a_faturar_string : "",
                                Recorrencia = reader["UTILIZA_RECO"] != DBNull.Value ? reader["NUM_RECORRENCIA"] + $"/{totalRecorrencias}" : "-",
                            });
                        }
                    }
                }
            }
            return retorno;
        }

        [HttpGet()]
        [Route("api/Faturamentoapi/ModalDetalhesFaturamento")]
        public async Task<List<Faturamento_Modal_Detalhes>> ModalDetalhesFaturamento(Int64 Id_Faturamento_Cliente)
        {
            List<Faturamento_Modal_Detalhes> retorno = new List<Faturamento_Modal_Detalhes>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT A.ID, COALESCE(C.DESCRICAO, D.DESCRICAO, I.DESCRICAO) AS DESCRICAO, B.DATAHORA_ABERTURA, B.DATAHORA_FECHAMENTO,B.VALOR AS VALOR, ")
                                 .AppendLine(" COALESCE(C.PRORATA, D.TEM_PRO_RATA) AS PRORATA, COALESCE(C.FREQUENCIACOBRANCA, D.FREQUENCIA_COBRANCA) AS FREQUENCIA_COBRANCA, H.PARCIAL,M.NUMERO_CONTRATO  ")
                                 .AppendLine(" FROM TB_FATURAMENTO_CLIENTE_PLANO A ")
                                 .AppendLine(" INNER JOIN TB_FATURAMENTO_CLIENTE_PLANODT B ")
                                 .AppendLine(" ON(B.ID_FATURAMENTO_CLIENTE_PLANO = A.ID) ")
                                 .AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO E ")
                                 .AppendLine(" ON(A.ID_PLANO_PRODUTO = E.ID) ")
                                 .AppendLine(" LEFT JOIN TB_PLANO C ")
                                 .AppendLine(" ON(E.ID_PLANO = C.ID) ")
                                 .AppendLine(" LEFT JOIN TB_PRODUTO D ")
                                 .AppendLine(" ON(E.ID_PRODUTO = D.ID) ")
                                 .AppendLine(" INNER JOIN TB_FATURAMENTO_CLIENTE F ")
                                 .AppendLine(" ON (F.ID = A.ID_FATURAMENTO_CLIENTE) ")
                                 .AppendLine(" INNER JOIN TB_FATURAMENTO_FORMA_PAGAMENTO G ")
                                 .AppendLine(" ON (G.ID = F.ID_FATURAMENTO_FORMA) ")
                                 .AppendLine(" INNER JOIN TB_FATURAMENTO_PROTOCOLO H ")
                                 .AppendLine(" ON (H.ID = G.ID_FATURAMENTO_PROTOCOLO) ")
                                 .AppendLine(" LEFT JOIN TB_CLIENTE_CONTRATO M ON(M.ID = E.ID_CLIENTE_CONTRATO) ")
                                 .AppendLine(" LEFT JOIN TB_COMBO I ON (E.ID_COMBO = I.ID) ")
                                 .AppendLine(" WHERE A.ID_FATURAMENTO_CLIENTE = @ID_FATURAMENTO_CLIENTE AND VALOR != 0");


                    parametros.Add("@ID_FATURAMENTO_CLIENTE", Id_Faturamento_Cliente);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            var dias_a_faturar = (Convert.ToDateTime(reader["DATAHORA_FECHAMENTO"]).Date.AddHours(24) - Convert.ToDateTime(reader["DATAHORA_ABERTURA"]).Date).Days;
                            var dias_a_faturar_string = Convert.ToString(dias_a_faturar);
                            var periodoMedicao = reader["DATAHORA_ABERTURA"] != DBNull.Value && reader["DATAHORA_FECHAMENTO"] != DBNull.Value ? (reader["DATAHORA_ABERTURA"] != DBNull.Value && reader["DATAHORA_FECHAMENTO"] != DBNull.Value && reader["DESCRICAO"] != DBNull.Value) ? Convert.ToDateTime(reader["DATAHORA_ABERTURA"]).ToString("dd/MM") + " - " + Convert.ToDateTime(reader["DATAHORA_FECHAMENTO"]).ToString("dd/MM") : " - " + Convert.ToDateTime(reader["DATAHORA_ABERTURA"]).ToString("dd/MM") + " - " + Convert.ToDateTime(reader["DATAHORA_FECHAMENTO"]).ToString("dd/MM") : "-";
                            if (dias_a_faturar_string[0] == '-') dias_a_faturar_string = dias_a_faturar_string.Remove(0, 1);
                            retorno.Add(new Faturamento_Modal_Detalhes
                            {
                                DataHoraCriacao = reader["DATAHORA_ABERTURA"] == DBNull.Value ? (reader["ABERTURA_AVULSO"] != null ? Convert.ToDateTime(reader["ABERTURA_AVULSO"]).ToString("dd/MM") : "-") : Convert.ToDateTime(reader["DATAHORA_ABERTURA"]).ToString("dd/MM"),
                                DataRegistro = reader["DATAHORA_FECHAMENTO"] == DBNull.Value ? (reader["FECHAMENTO_AVULSO"] != null ? Convert.ToDateTime(reader["FECHAMENTO_AVULSO"]).ToString("dd/MM") : "-") : Convert.ToDateTime(reader["DATAHORA_ABERTURA"]).ToString("dd/MM"),
                                Descricao = reader["DESCRICAO"] == DBNull.Value ? "-" : Convert.ToString(reader["DESCRICAO"]),
                                Valor = reader["VALOR"] == DBNull.Value ? (reader["VALOR_AVULSO"] != null ? String.Format("{0:N2}", reader["VALOR_AVULSO"]) : "-") : String.Format("{0:N2}", reader["VALOR"]),
                                PeriodoMedicao = periodoMedicao,
                                QuantidadeDiasFaturar = reader["DATAHORA_ABERTURA"] != DBNull.Value && reader["DATAHORA_FECHAMENTO"] != DBNull.Value ? dias_a_faturar_string : "-",
                                NumeroContrato = reader["NUMERO_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["NUMERO_CONTRATO"]) : "-",
                            });
                        }
                    }
                }
            }


            //BUSCA DETALHES DE COB DE SERVICOS
            var detalhesCobExec = await DetalhesFatServCobExec(Id_Faturamento_Cliente);
            if (detalhesCobExec.Count > 0)
                retorno.AddRange(detalhesCobExec);

            return retorno;
        }

        private async Task<List<Faturamento_Modal_Detalhes>> DetalhesFatServCobExec(long idFatCli)
        {
            List<Faturamento_Modal_Detalhes> lsDetalhes = new List<Faturamento_Modal_Detalhes>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT I.VALOR_TOTAL AS COBVALOR, I.DATAHORACRIACAO AS COB_DATA, K.DESCRICAO ");
                    sql.AppendLine("FROM TB_FATURAMENTO_COBSERVEXEC I ");
                    sql.AppendLine("INNER JOIN TB_FATURAMENTO_CLIENTE J ");
                    sql.AppendLine("ON (J.ID = I.ID_FAT_CLI) ");
                    sql.AppendLine("INNER JOIN TB_PLANO K ");
                    sql.AppendLine("ON (K.ID = I.ID_PLANO) ");
                    sql.AppendLine("WHERE J.ID = @ID_FATURAMENTO_CLIENTE AND I.VALOR_TOTAL != 0");


                    parametros.Add("@ID_FATURAMENTO_CLIENTE", idFatCli);


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            lsDetalhes.Add(new Faturamento_Modal_Detalhes
                            {
                                DataRegistro = reader["COB_DATA"] != DBNull.Value ? Convert.ToDateTime(reader["COB_DATA"]).ToString("dd/MM") : "-",
                                Valor = reader["COBVALOR"] != DBNull.Value ? String.Format("{0:N2}", reader["COBVALOR"]) : "-",
                                PeriodoMedicao = "-",
                                QuantidadeDiasFaturar = "-",
                                Recorrencia = "1/1",
                                Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",
                            });
                        }
                    }
                }
            }

            return lsDetalhes;
        }
    }
}