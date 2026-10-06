using Aniel.Connect.ControllersWebApi.Ativacao;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.Core.Helpers;
using Sinapse.Model.enumerados;
using Aniel.Connect.Model.nhibernate;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ListagemONUApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/ListagemONUApi/GetAllOld")]
        [Obsolete("Utilizar o GetAll sem consulta nas OLTs.")]
        public async Task<DataTableResponse<ONUListModel>> GetAllOld(ListagemONU_DataTableRequest request)
        {
            var retorno = new List<ONUListModel>();
            var recordsTotal = 0;

            StringBuilder sqlFrom = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            sqlFrom.AppendLine("FROM TB_ONU A");

            sqlFrom.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO B");
            sqlFrom.AppendLine("ON(B.ID = A.ID_CLIENTE_PLANO_PRODUTO)");

            sqlFrom.AppendLine("INNER JOIN TB_CLIENTE C");
            sqlFrom.AppendLine("ON(C.ID = B.ID_CLIENTE)");

            sqlFrom.AppendLine("LEFT JOIN TB_PLANO D");
            sqlFrom.AppendLine("ON(D.ID = B.ID_PLANO)");



            sqlFrom.AppendLine("LEFT JOIN TB_LOGIN_RADIUS H");
            sqlFrom.AppendLine("ON(H.ID = A.ID_LOGIN_RADIUS)");


            sqlFrom.AppendLine("INNER JOIN TB_SLOT F");
            sqlFrom.AppendLine("ON(F.ID = A.ID_SLOT)");

            sqlFrom.AppendLine("INNER JOIN TB_PON G");
            sqlFrom.AppendLine("ON(G.ID = A.ID_PON)");

            #region Filtros

            if (!String.IsNullOrEmpty(request.search.value))
            {
                sqlFrom.AppendLine($" WHERE A.MAC LIKE '%{request.search.value}%' OR A.SERIAL LIKE '%{request.search.value}%' ");
            }

            if (request.idOLT.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND A.ID_OLT = @ID_OLT ");
                else
                    sqlFrom.AppendLine(" WHERE A.ID_OLT = @ID_OLT ");

                parametros.Add("@ID_OLT", request.idOLT.Value);
            }

            if (request.idSlot.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND A.ID_SLOT = @ID_SLOT ");
                else
                    sqlFrom.AppendLine(" WHERE A.ID_SLOT = @ID_SLOT ");

                parametros.Add("@ID_SLOT", request.idSlot.Value);
            }

            if (request.idPON.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND A.ID_PON = @ID_PON ");
                else
                    sqlFrom.AppendLine(" WHERE A.ID_PON = @ID_PON ");

                parametros.Add("@ID_PON", request.idPON.Value);
            }

            if (request.idCliente.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND C.ID = @ID_CLIENTE ");
                else
                    sqlFrom.AppendLine(" WHERE C.ID = @ID_CLIENTE ");

                parametros.Add("@ID_CLIENTE", request.idCliente.Value);
            }
            if (request.idPOP.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND H.ID_POP = @ID_POP ");
                else
                    sqlFrom.AppendLine(" WHERE H.ID_POP = @ID_POP ");

                parametros.Add("@ID_POP", request.idPOP.Value);
            }
            if (request.IdStatus.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                {
                    if (request.IdStatus == 2) //Status Inativo
                    {
                        sqlFrom.AppendLine("AND D.STATUSINICIAL IN (3,4)");
                    }
                    else if (request.IdStatus == 1) //Status Ativo
                    {


                        sqlFrom.AppendLine("AND D.STATUSINICIAL NOT IN (3,4)");
                    }
                    parametros.Add("@STATUS", request.IdStatus.Value);
                }
                else
                {
                    if (request.IdStatus == 2) //Status Inativo
                    {
                        sqlFrom.AppendLine("WHERE D.STATUSINICIAL IN (3,4)");
                    }
                    else if (request.IdStatus == 1) //Status Ativo
                    {


                        sqlFrom.AppendLine("WHERE D.STATUSINICIAL NOT IN (3,4)");
                    }


                    parametros.Add("@STATUS", request.IdStatus.Value);

                }
            }


            if (request.idVLAN.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND VLAN = @VLAN ");
                else
                    sqlFrom.AppendLine(" WHERE VLAN = @VLAN");

                parametros.Add("@VLAN", request.idVLAN.Value);
            }
            if (request.idCaixaFTTH.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND H.ID_CAIXA_FTTH = @CAIXAFTTH ");
                else
                    sqlFrom.AppendLine(" WHERE H.ID_CAIXA_FTTH = @CAIXAFTTH");

                parametros.Add("@CAIXAFTTH", request.idCaixaFTTH.Value);
            }

            if (request.idPortaFTTH.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND H.ID_PORTA_FTTH= @PORTAFTTH ");
                else
                    sqlFrom.AppendLine(" WHERE H.ID_PORTA_FTTH= @PORTAFTTH");

                parametros.Add("@PORTAFTTH", request.idPortaFTTH.Value);
            }

            if (request.idPlano.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND B.ID_PLANO = @ID_PLANO ");
                else
                    sqlFrom.AppendLine(" WHERE B.ID_PLANO = @ID_PLANO ");

                parametros.Add("@ID_PLANO", request.idPlano.Value);
            }

            #endregion


            using (ISession session = HibernateUtil.GetSession())
            {
                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sqlCount.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //SQL
                using (var com = session.Connection.CreateCommand())
                using (var apiAtivacao = new IntegracaoAtivacaoApiController())
                {
                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine("SELECT A.ID, A.SERIAL, A.MAC, A.ID_OLT, F.NUM_SLOT, G.NUM_PON, A.ONT_ID, ");
                    sql.AppendLine("       C.NOMEFANTASIA AS CLIENTE,");
                    sql.AppendLine("       D.DESCRICAO AS PLANO_PRODUTO,");
                    sql.AppendLine("       COALESCE(A.VLAN_UPLINK,A.VLAN_PPPOE,A.VLAN_DHCP,A.VLAN_IPTV,A.VLAN_VOIP,A.VLAN_TR69,A.VLAN_OUTROS) AS VLAN");
                    sql.AppendLine(sqlFrom.ToString());


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ONUListModel
                            {
                                Id = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "",

                                IdOLT = Convert.ToInt64(reader["ID_OLT"]),
                                NumSlot = Convert.ToString(reader["NUM_SLOT"]),
                                NumPON = Convert.ToString(reader["NUM_PON"]),

                                Serial = reader["SERIAL"] != DBNull.Value ? Convert.ToString(reader["SERIAL"]) : "",
                                Mac = reader["MAC"] != DBNull.Value ? Convert.ToString(reader["MAC"]) : "",
                                Cliente = reader["CLIENTE"] != DBNull.Value ? Convert.ToString(reader["CLIENTE"]) : "",
                                PlanoProduto = reader["PLANO_PRODUTO"] != DBNull.Value ? Convert.ToString(reader["PLANO_PRODUTO"]) : "",
                                ONTID = Convert.ToString(reader["ONT_ID"])
                                //HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }


                    var listSlotPon = (from ret in retorno
                                       orderby ret.IdOLT, Convert.ToInt32(ret.NumSlot), Convert.ToInt32(ret.NumPON)
                                       select new
                                       {
                                           ret.IdOLT,
                                           ret.NumPON,
                                           ret.NumSlot
                                       }).Distinct();

                    foreach (var item in listSlotPon)
                    {
                        var listRetorno = retorno.Where(p => p.NumSlot == item.NumSlot && p.NumPON == item.NumPON);
                        var listSerial = listRetorno.Select(p => p.Serial).ToList();
                        var listSerialConvertido = listRetorno
                            .Select(p => (p.Serial, p.SerialConvertido))
                            .ToList();
                        var listONTID = listRetorno.Select(p => p.ONTID).ToList();

                        var listStatusONU = await apiAtivacao.GetStatusONUsByPON(item.IdOLT, item.NumSlot, item.NumPON, listSerial, listONTID, listSerialConvertido);
                        if (listStatusONU.Count() > 0)
                        {
                            var listaOnusSucesso = new List<ONUListModel>();
                            foreach (var statusONU in listStatusONU)
                            {
                                retorno.Find(p => p.Serial == statusONU.Serial ||
                                (p.IdOLT == item.IdOLT && p.NumSlot == item.NumSlot && p.NumPON == item.NumPON && p.ONTID == statusONU.ONTID)).Status = HtmlEnumExtensions.GetEnumDescription(statusONU.Status);

                                listaOnusSucesso.Add(retorno.Find(p => p.Serial == statusONU.Serial ||
                                (p.IdOLT == item.IdOLT && p.NumSlot == item.NumSlot && p.NumPON == item.NumPON && p.ONTID == statusONU.ONTID)));
                            }

                            var listStatusErro = retorno.FindAll(p => p.IdOLT == item.IdOLT && p.NumSlot == item.NumSlot && p.NumPON == item.NumPON).Except(listaOnusSucesso).ToList();
                            if (listStatusErro.Count() > 0)
                            {
                                foreach (var statusONU in listStatusErro)
                                {
                                    retorno.Find(p => p.Serial == statusONU.Serial ||
                                    (p.IdOLT == item.IdOLT && p.NumSlot == item.NumSlot && p.NumPON == item.NumPON && p.ONTID == statusONU.ONTID)).Status = HtmlEnumExtensions.GetEnumDescription(Sinapse.Model.enumerados.StatusONU.Erro);
                                }
                            }
                        }
                        else
                        {
                            foreach (var itretorno in listRetorno)
                            {
                                itretorno.Status = HtmlEnumExtensions.GetEnumDescription(Sinapse.Model.enumerados.StatusONU.Offline);
                            }
                        }

                    }
                }
            }




            return new DataTableResponse<ONUListModel>
            {
                data = retorno,
                recordsFiltered = recordsTotal
            };
        }

        [HttpPost()]
        [Route("api/ListagemONUApi/GetAll")]
        public async Task<DataTableResponse<ONUListModel>> GetAll(ListagemONU_DataTableRequest request)
        {
            var retorno = new List<ONUListModel>();
            var recordsTotal = 0;

            StringBuilder sqlFrom = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            sqlFrom.AppendLine("FROM TB_ONU A");

            sqlFrom.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO B");
            sqlFrom.AppendLine("ON(B.ID = A.ID_CLIENTE_PLANO_PRODUTO)");

            sqlFrom.AppendLine("INNER JOIN TB_CLIENTE C");
            sqlFrom.AppendLine("ON(C.ID = B.ID_CLIENTE)");

            sqlFrom.AppendLine("LEFT JOIN TB_PLANO D");
            sqlFrom.AppendLine("ON(D.ID = B.ID_PLANO)");



            sqlFrom.AppendLine("LEFT JOIN TB_LOGIN_RADIUS H");
            sqlFrom.AppendLine("ON(H.ID = A.ID_LOGIN_RADIUS)");


            sqlFrom.AppendLine("INNER JOIN TB_SLOT F");
            sqlFrom.AppendLine("ON(F.ID = A.ID_SLOT)");

            sqlFrom.AppendLine("INNER JOIN TB_PON G");
            sqlFrom.AppendLine("ON(G.ID = A.ID_PON)");

            #region Filtros

            if (!String.IsNullOrEmpty(request.search.value))
            {
                sqlFrom.AppendLine($" WHERE A.MAC LIKE '%{request.search.value}%' OR A.SERIAL LIKE '%{request.search.value}%' OR A.SERIAL_CONVERTIDO LIKE '%{request.search.value}%'");
            }

            if (request.idOLT.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND A.ID_OLT = @ID_OLT ");
                else
                    sqlFrom.AppendLine(" WHERE A.ID_OLT = @ID_OLT ");

                parametros.Add("@ID_OLT", request.idOLT.Value);
            }

            if (request.idSlot.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND A.ID_SLOT = @ID_SLOT ");
                else
                    sqlFrom.AppendLine(" WHERE A.ID_SLOT = @ID_SLOT ");

                parametros.Add("@ID_SLOT", request.idSlot.Value);
            }

            if (request.idPON.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND A.ID_PON = @ID_PON ");
                else
                    sqlFrom.AppendLine(" WHERE A.ID_PON = @ID_PON ");

                parametros.Add("@ID_PON", request.idPON.Value);
            }

            if (request.idCliente.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND C.ID = @ID_CLIENTE ");
                else
                    sqlFrom.AppendLine(" WHERE C.ID = @ID_CLIENTE ");

                parametros.Add("@ID_CLIENTE", request.idCliente.Value);
            }
            if (request.idPOP.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND H.ID_POP = @ID_POP ");
                else
                    sqlFrom.AppendLine(" WHERE H.ID_POP = @ID_POP ");

                parametros.Add("@ID_POP", request.idPOP.Value);
            }
            if (request.IdStatus.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                {
                    sqlFrom.AppendLine("AND A.STATUS_ONU = @STATUS");

                    parametros.Add("@STATUS", request.IdStatus.Value);
                }
                else
                {
                    sqlFrom.AppendLine("AND A.STATUS_ONU = @STATUS");

                    parametros.Add("@STATUS", request.IdStatus.Value);

                }
            }


            if (request.idVLAN.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND VLAN = @VLAN ");
                else
                    sqlFrom.AppendLine(" WHERE VLAN = @VLAN");

                parametros.Add("@VLAN", request.idVLAN.Value);
            }
            if (request.idCaixaFTTH.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND H.ID_CAIXA_FTTH = @CAIXAFTTH ");
                else
                    sqlFrom.AppendLine(" WHERE H.ID_CAIXA_FTTH = @CAIXAFTTH");

                parametros.Add("@CAIXAFTTH", request.idCaixaFTTH.Value);
            }

            if (request.idPortaFTTH.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND H.ID_PORTA_FTTH= @PORTAFTTH ");
                else
                    sqlFrom.AppendLine(" WHERE H.ID_PORTA_FTTH= @PORTAFTTH");

                parametros.Add("@PORTAFTTH", request.idPortaFTTH.Value);
            }

            if (request.idPlano.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND B.ID_PLANO = @ID_PLANO ");
                else
                    sqlFrom.AppendLine(" WHERE B.ID_PLANO = @ID_PLANO ");

                parametros.Add("@ID_PLANO", request.idPlano.Value);
            }

            if (request.idLoginRadius.HasValue)
            {
                if (sqlFrom.ToString().Contains("WHERE"))
                    sqlFrom.AppendLine("   AND A.ID_LOGIN_RADIUS = @ID_LOGIN ");
                else
                    sqlFrom.AppendLine(" WHERE A.ID_LOGIN_RADIUS = @ID_LOGIN ");

                parametros.Add("@ID_LOGIN", request.idLoginRadius.Value);
            }

            #endregion


            using (ISession session = HibernateUtil.GetSession())
            {
                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sqlCount.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //SQL
                using (var com = session.Connection.CreateCommand())
                using (var apiAtivacao = new IntegracaoAtivacaoApiController())
                {
                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine("SELECT A.ID, A.SERIAL, A.SERIAL_CONVERTIDO, A.MAC, A.ID_OLT, F.NUM_SLOT, G.NUM_PON, A.ONT_ID, ");
                    sql.AppendLine("       C.NOMEFANTASIA AS CLIENTE,");
                    sql.AppendLine("       D.DESCRICAO AS PLANO_PRODUTO,");
                    sql.AppendLine("       COALESCE(A.VLAN_UPLINK,A.VLAN_PPPOE,A.VLAN_DHCP,A.VLAN_IPTV,A.VLAN_VOIP,A.VLAN_TR69,A.VLAN_OUTROS) AS VLAN, A.STATUS_ONU, A.ULTIMA_ATUALIZACAO_STATUS ");
                    sql.AppendLine(sqlFrom.ToString());


                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ONUListModel
                            {
                                Id = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "",

                                IdOLT = Convert.ToInt64(reader["ID_OLT"]),
                                NumSlot = Convert.ToString(reader["NUM_SLOT"]),
                                NumPON = Convert.ToString(reader["NUM_PON"]),

                                Serial = reader["SERIAL"] != DBNull.Value ? Convert.ToString(reader["SERIAL"]) : "",
                                SerialConvertido = reader["SERIAL"] != DBNull.Value ? Convert.ToString(reader["SERIAL_CONVERTIDO"]) : "-",
                                Mac = reader["MAC"] != DBNull.Value ? Convert.ToString(reader["MAC"]) : "",
                                Cliente = reader["CLIENTE"] != DBNull.Value ? Convert.ToString(reader["CLIENTE"]) : "",
                                PlanoProduto = reader["PLANO_PRODUTO"] != DBNull.Value ? Convert.ToString(reader["PLANO_PRODUTO"]) : "",
                                ONTID = Convert.ToString(reader["ONT_ID"]),
                                Status = reader["STATUS_ONU"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Sinapse.Model.enumerados.StatusONU>(Convert.ToString(reader["STATUS_ONU"])) : HtmlEnumExtensions.GetEnumDescription(Sinapse.Model.enumerados.StatusONU.Indisponivel),
                                DataUltimaAtualizacaoStatus = reader["ULTIMA_ATUALIZACAO_STATUS"] != DBNull.Value ? Convert.ToDateTime(reader["ULTIMA_ATUALIZACAO_STATUS"]).ToString("dd/MM/yyyy HH:mm:ss") : "-"
                                //HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }
                }
            }

            return new DataTableResponse<ONUListModel>
            {
                draw = request.draw,
                data = retorno,
                recordsFiltered = recordsTotal
            };
        }

        [HttpPost()]
        [Route("api/ListagemONUApi/GetDadosONU")]
        public async Task<ONUListModel> GetDadosONU(string idPlanoProduto)
        {
            var retorno = new ONUListModel();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            using (var apiAtivacao = new IntegracaoAtivacaoApiController())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("SELECT A.ID, A.SERIAL, A.MAC, A.ID_OLT, F.NUM_SLOT, G.NUM_PON,");
                sql.AppendLine("       C.NOMEFANTASIA AS CLIENTE,");
                sql.AppendLine("       D.DESCRICAO AS PLANO_PRODUTO,");


                sql.AppendLine("       COALESCE(A.VLAN_UPLINK,A.VLAN_PPPOE,A.VLAN_DHCP,A.VLAN_IPTV,A.VLAN_VOIP,A.VLAN_TR69,A.VLAN_OUTROS) AS VLAN");

                sql.AppendLine("FROM TB_ONU A");

                sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO B ");
                sql.AppendLine("ON(B.ID = A.ID_CLIENTE_PLANO_PRODUTO)");

                sql.AppendLine("INNER JOIN TB_CLIENTE C ");
                sql.AppendLine("ON(C.ID = B.ID_CLIENTE)");

                sql.AppendLine("LEFT JOIN TB_PLANO D ");
                sql.AppendLine("ON(D.ID = B.ID_PLANO)");



                sql.AppendLine("LEFT JOIN TB_LOGIN_RADIUS H ");
                sql.AppendLine("ON(H.ID = A.ID_LOGIN_RADIUS) ");


                sql.AppendLine("INNER JOIN TB_SLOT F ");
                sql.AppendLine("ON(F.ID = A.ID_SLOT) ");

                sql.AppendLine(" INNER JOIN TB_PON G ");
                sql.AppendLine(" ON(G.ID = A.ID_PON) ");
                sql.AppendLine(" WHERE B.ID_PLANO = @ID_PLANO_PRODUTO ");
                parametros.Add("@ID_PLANO_PRODUTO", idPlanoProduto);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno = (new ONUListModel()
                        {
                            Id = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "",

                            IdOLT = Convert.ToInt64(reader["ID_OLT"]),
                            NumSlot = Convert.ToString(reader["NUM_SLOT"]),
                            NumPON = Convert.ToString(reader["NUM_PON"]),

                            Serial = reader["SERIAL"] != DBNull.Value ? Convert.ToString(reader["SERIAL"]) : "",
                            Mac = reader["MAC"] != DBNull.Value ? Convert.ToString(reader["MAC"]) : "",
                            Cliente = reader["CLIENTE"] != DBNull.Value ? Convert.ToString(reader["CLIENTE"]) : "",
                            PlanoProduto = reader["PLANO_PRODUTO"] != DBNull.Value ? Convert.ToString(reader["PLANO_PRODUTO"]) : "",
                        });
                    }
                }
            }

            return retorno;
        }

        [HttpDelete]
        [Route("api/ListagemONUApi/DeleteONUByID")]
        public async Task<DeleteApiRetorno<int>> DeleteONUByID(string id)
        {
            var retorno = new DeleteApiRetorno<int>();

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    using (ISession session = HibernateUtil.GetSession())
                    {
                        using (var repository = new Conexao<ONUListModel>(session))
                        {
                            string query = "DELETE FROM TB_ONU WHERE ID = " + id;

                            var result = await repository.selectSql(query);

                            retorno.IdObjeto = int.Parse(id);
                            retorno.Ok = true;
                        }
                    }
                }
                else
                {
                    throw new Exception("ID não pode ser vazio ou nulo.");
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
    }
}
