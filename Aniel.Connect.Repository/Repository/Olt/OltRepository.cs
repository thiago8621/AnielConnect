using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using RestSharp.Validation;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using System.Data.Common;
using Aniel.Connect.Models;
namespace Aniel.Connect.Repository.T5.Olt
{
    public class OltRepository : IOltRepository
    {
        private readonly ISession _sessionConnNh;

        public OltRepository(ISession sessionConnNh)
        {
            _sessionConnNh = sessionConnNh;
        }
        /// <summary>
        /// Listar Pop
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ID_CIDADE Valor deve ser obtido pelo endpoint CIdade </returns>
        public System.Collections.Generic.List<OltListModel> GetAll(OltRequest request)
        {
            List<OltListModel> retorno = new List<OltListModel>();
            try
            {

                List<string> condicoes = new List<string>();
                Dictionary<String, Object> parametro = new Dictionary<string, object>();
                using (ISession sessionT5 = HibernateUtil.GetSession())
                {
                    var com = sessionT5.Connection.CreateCommand();
                    {
                        StringBuilder sql = new StringBuilder();
                        StringBuilder sqlWhere = new StringBuilder();

                        sql.AppendLine("SELECT  A.ID, A.OLTID, A.DESCRICAO, A.MARCA_MODELO, A.NUM_OLT, A.IP, A.LOGIN, A.SENHA, A.PORTA_TELNET, A.PORTA_API, A.PORTA_HTTP,");
                        sql.AppendLine("A.PORTA, A.IP_TL1, A.LOGIN_TL1, A.SENHA_TL1, A.PORTA_TL1, A.GABINETE, A.SUBRACK, A.STATUS, A.DHCP_DNS_PRIMARIO, A.DHCP_DNS_SECUNDARIO,");
                        sql.AppendLine("A.DHCP_LAN_IP, A.DHCP_LAN_MASCARA, A.DHCP_INICIO_RANGE, A.DHCP_FINAL_RANGE, A.TESTE_IP1, A.TESTE_IP2, A.TIPO_IDENTIFICACAO, A.ID_FILIAL, A.TIPO,");
                        sql.AppendLine("A.GERENCIA_REDEGENERICA, A.GERENCIA_MASCARAREDEGENERICA, A.GERENCIA_LOGIN, A.GERENCIA_SENHA, A.GERENCIA_PORTA_GENERICA, A.GERENCIA_VLAN,");
                        sql.AppendLine("A.GERENCIA_SINALMINIMO, A.GERENCIA_SINALMAXIMO, A.ID_SERVIDOR_GERENCIA, A.ID_POP, A.UTILIZA_CONVERSAO_MAC, A.CODIGO_AUXILIAR, A.VLAN_UPLINK,");
                        sql.AppendLine("A.DELAY, B.ID AS GRUPO_AREA_ID, B.COD_AREA, B.DESC_AREA, B.COD_ESCRITORIO, B.DESC_ESCRITORIO");
                        sql.AppendLine("FROM TB_OLT A");
                        sql.AppendLine("LEFT jOIN TB_OLT_GRUPO_AREA B ON A.ID = B.ID_OLT");
                        if (request.ID != 0)
                        {
                            condicoes.Add("A.ID = @ID ");
                            parametro.Add("@ID", request.ID);
                        }
                        if (condicoes.Count > 0)
                        {
                            sqlWhere.AppendLine(" WHERE ");
                            if (condicoes.Count > 1)
                                sqlWhere.AppendLine(string.Join(" AND ", condicoes));
                            else
                                sqlWhere.AppendLine(condicoes.First());
                        }

                        sql.Append(sqlWhere);
                        using (var reader = CommandHelper.ExecuteReader(com, sql, parametro, sessionT5.Connection, null).Result)
                        {
                            while (reader.Read())
                            {

                                var olt = new OltListModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    OLTID = reader["OLTID"].ToString(),
                                    Descricao = reader["DESCRICAO"].ToString(),
                                    MARCA_MODELO = reader["MARCA_MODELO"] != DBNull.Value ? Convert.ToInt32(reader["MARCA_MODELO"]) : (int?)null,
                                    NumOLT = reader["NUM_OLT"].ToString(),
                                    Ip = reader["IP"].ToString(),
                                    LOGIN = reader["LOGIN"].ToString(),
                                    SENHA = reader["SENHA"].ToString(),
                                    PORTA_TELNET = reader["PORTA_TELNET"] != DBNull.Value ? Convert.ToInt32(reader["PORTA_TELNET"]) : (int?)null,
                                    PORTA_API = reader["PORTA_API"] != DBNull.Value ? Convert.ToInt32(reader["PORTA_API"]) : (int?)null,
                                    PORTA_HTTP = reader["PORTA_HTTP"] != DBNull.Value ? Convert.ToInt32(reader["PORTA_HTTP"]) : (int?)null,
                                    PORTA = reader["PORTA"] != DBNull.Value ? Convert.ToInt32(reader["PORTA"]) : (int?)null,
                                    IP_TL1 = reader["IP_TL1"].ToString(),
                                    LOGIN_TL1 = reader["LOGIN_TL1"].ToString(),
                                    SENHA_TL1 = reader["SENHA_TL1"].ToString(),
                                    GABINETE = reader["GABINETE"].ToString(),
                                    SUBRACK = reader["SUBRACK"].ToString(),
                                    STATUS = Convert.ToInt32(reader["STATUS"].ToString()),
                                    DHCP_DNS_PRIMARIO = reader["DHCP_DNS_PRIMARIO"].ToString(),
                                    DHCP_DNS_SECUNDARIO = reader["DHCP_DNS_SECUNDARIO"].ToString(),
                                    DHCP_LAN_IP = reader["DHCP_LAN_IP"].ToString(),
                                    DHCP_LAN_MASCARA = reader["DHCP_LAN_MASCARA"].ToString(),
                                    DHCP_INICIO_RANGE = reader["DHCP_INICIO_RANGE"].ToString(),
                                    DHCP_FINAL_RANGE = reader["DHCP_FINAL_RANGE"].ToString(),
                                    TESTE_IP1 = reader["TESTE_IP1"].ToString(),
                                    TESTE_IP2 = reader["TESTE_IP2"].ToString(),
                                    TIPO_IDENTIFICACAO = reader["TIPO_IDENTIFICACAO"] != DBNull.Value ? Convert.ToInt32(reader["TIPO_IDENTIFICACAO"]) : (int?)null,
                                    ID_FILIAL = reader["ID_FILIAL"] != DBNull.Value ? Convert.ToInt32(reader["ID_FILIAL"]) : (int?)null,
                                    Tipo = reader["TIPO"].ToString(),
                                    GERENCIA_REDEGENERICA = reader["GERENCIA_REDEGENERICA"].ToString(),
                                    GERENCIA_MASCARAREDEGENERICA = reader["GERENCIA_MASCARAREDEGENERICA"].ToString(),
                                    GERENCIA_LOGIN = reader["GERENCIA_LOGIN"].ToString(),
                                    GERENCIA_SENHA = reader["GERENCIA_SENHA"].ToString(),
                                    GERENCIA_PORTA_GENERICA = reader["GERENCIA_PORTA_GENERICA"].ToString(),
                                    GERENCIA_VLAN = reader["GERENCIA_VLAN"] != DBNull.Value ? Convert.ToInt32(reader["GERENCIA_VLAN"]) : (int?)null,
                                    GERENCIA_SINALMINIMO = reader["GERENCIA_SINALMINIMO"] != DBNull.Value ? Convert.ToInt32(reader["GERENCIA_SINALMINIMO"]) : (int?)null,
                                    GERENCIA_SINALMAXIMO = reader["GERENCIA_SINALMAXIMO"] != DBNull.Value ? Convert.ToInt32(reader["GERENCIA_SINALMAXIMO"]) : (int?)null,
                                    ID_SERVIDOR_GERENCIA = reader["ID_SERVIDOR_GERENCIA"] != DBNull.Value ? Convert.ToInt32(reader["ID_SERVIDOR_GERENCIA"]) : (int?)null,
                                    ID_POP = reader["ID_POP"] != DBNull.Value ? Convert.ToInt32(reader["ID_POP"]) : (int?)null,
                                    UTILIZA_CONVERSAO_MAC = reader["UTILIZA_CONVERSAO_MAC"] != DBNull.Value ? Convert.ToInt32(reader["UTILIZA_CONVERSAO_MAC"]) : (int?)null,
                                    CODIGO_AUXILIAR = reader["CODIGO_AUXILIAR"].ToString(),
                                    VLAN_UPLINK = (reader["VLAN_UPLINK"].ToString()),
                                    DELAY = reader["DELAY"] != DBNull.Value ? Convert.ToInt32(reader["DELAY"]) : (int?)null,
                                    Situacao = reader["STATUS"] != DBNull.Value ? (Convert.ToInt64(reader["STATUS"]) == 1 ? "Ativo" : "Inativo") : string.Empty,
                                    GrupoAreaLista = new List<OLT_GrupoAreaListModel>()
                                };

                                if (reader["GRUPO_AREA_ID"] != DBNull.Value)
                                {
                                    olt.GrupoAreaLista.Add(new OLT_GrupoAreaListModel
                                    {
                                        Id_Area = Convert.ToInt32(reader["GRUPO_AREA_ID"]),
                                        Cod_Area = reader["COD_AREA"].ToString(),
                                        Desc_Area = reader["DESC_AREA"].ToString(),
                                        Cod_Escritorio = reader["COD_ESCRITORIO"].ToString(),
                                        Desc_Escritorio = reader["DESC_ESCRITORIO"].ToString()
                                    });
                                }
                                retorno.Add(olt);

                            }

                        }
                    }
                }
            }



            catch (Exception erro)
            {
                
            }
            return retorno;

        }


        public PostApiRetorno UpdateOLT(OLTModels dados)
        {
            var retorno = new PostApiRetorno();
            try
            {


                OltRequest oltRequest = new OltRequest();
                oltRequest.ID = dados.ID;

                var Olt = GetAll(oltRequest).Where(x => x.Id == dados.ID);
                if (Olt.Count() == 0)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "OLT mão localzada";
                    return retorno;
                }
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (ISession session = HibernateUtil.GetSession())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.Append("UPDATE TB_OLT SET ");
                    sql.Append("OLTID = @OLTID, ");
                    sql.Append("DESCRICAO = @DESCRICAO, ");
                    sql.Append("MARCA_MODELO = @MARCA_MODELO, ");
                    sql.Append("NUM_OLT = @NUM_OLT, ");
                    sql.Append("IP = @IP, ");
                    sql.Append("LOGIN = @LOGIN, ");
                    sql.Append("SENHA = @SENHA, ");
                    sql.Append("PORTA_TELNET = @PORTA_TELNET, ");
                    sql.Append("PORTA_API = @PORTA_API, ");
                    sql.Append("PORTA_HTTP = @PORTA_HTTP, ");
                    sql.Append("PORTA = @PORTA, ");
                    sql.Append("IP_TL1 = @IP_TL1, ");
                    sql.Append("LOGIN_TL1 = @LOGIN_TL1, ");
                    sql.Append("SENHA_TL1 = @SENHA_TL1, ");
                    sql.Append("PORTA_TL1 = @PORTA_TL1, ");
                    sql.Append("GABINETE = @GABINETE, ");
                    sql.Append("SUBRACK = @SUBRACK, ");
                    sql.Append("STATUS = @STATUS, ");
                    sql.Append("DHCP_DNS_PRIMARIO = @DHCP_DNS_PRIMARIO, ");
                    sql.Append("DHCP_DNS_SECUNDARIO = @DHCP_DNS_SECUNDARIO, ");
                    sql.Append("DHCP_LAN_IP = @DHCP_LAN_IP, ");
                    sql.Append("DHCP_LAN_MASCARA = @DHCP_LAN_MASCARA, ");
                    sql.Append("DHCP_INICIO_RANGE = @DHCP_INICIO_RANGE, ");
                    sql.Append("DHCP_FINAL_RANGE = @DHCP_FINAL_RANGE, ");
                    sql.Append("TESTE_IP1 = @TESTE_IP1, ");
                    sql.Append("TESTE_IP2 = @TESTE_IP2, ");
                    sql.Append("TIPO_IDENTIFICACAO = @TIPO_IDENTIFICACAO, ");
                    sql.Append("ID_FILIAL = @ID_FILIAL, ");
                    sql.Append("TIPO = @TIPO, ");
                    sql.Append("GERENCIA_REDEGENERICA = @GERENCIA_REDEGENERICA, ");
                    sql.Append("GERENCIA_MASCARAREDEGENERICA = @GERENCIA_MASCARAREDEGENERICA, ");
                    sql.Append("GERENCIA_LOGIN = @GERENCIA_LOGIN, ");
                    sql.Append("GERENCIA_SENHA = @GERENCIA_SENHA, ");
                    sql.Append("GERENCIA_PORTA_GENERICA = @GERENCIA_PORTA_GENERICA, ");
                    sql.Append("GERENCIA_VLAN = @GERENCIA_VLAN, ");
                    sql.Append("GERENCIA_SINALMINIMO = @GERENCIA_SINALMINIMO, ");
                    sql.Append("GERENCIA_SINALMAXIMO = @GERENCIA_SINALMAXIMO, ");
                    sql.Append("ID_SERVIDOR_GERENCIA = @ID_SERVIDOR_GERENCIA, ");
                    sql.Append("ID_POP = @ID_POP, ");
                    sql.Append("UTILIZA_CONVERSAO_MAC = @UTILIZA_CONVERSAO_MAC, ");
                    sql.Append("CODIGO_AUXILIAR = @CODIGO_AUXILIAR, ");
                    sql.Append("VLAN_UPLINK = @VLAN_UPLINK, ");
                    sql.Append("DELAY = @DELAY ");
                    sql.Append("WHERE ID = @ID");

                    parametros.Add("@OLTID", dados.OLTID);
                    parametros.Add("@DESCRICAO", dados.DESCRICAO);
                    parametros.Add("@MARCA_MODELO", dados.MARCA_MODELO);
                    parametros.Add("@NUM_OLT", dados.NUM_OLT);
                    parametros.Add("@IP", dados.IP);
                    parametros.Add("@LOGIN", dados.LOGIN);
                    parametros.Add("@SENHA", dados.SENHA);
                    parametros.Add("@PORTA_TELNET", dados.PORTA_TELNET);
                    parametros.Add("@PORTA_API", dados.PORTA_API);
                    parametros.Add("@PORTA_HTTP", dados.PORTA_HTTP);
                    parametros.Add("@PORTA", dados.PORTA);
                    parametros.Add("@IP_TL1", dados.IP_TL1);
                    parametros.Add("@LOGIN_TL1", dados.LOGIN_TL1);
                    parametros.Add("@SENHA_TL1", dados.SENHA_TL1);
                    parametros.Add("@PORTA_TL1", dados.PORTA_TL1);
                    parametros.Add("@GABINETE", dados.GABINETE);
                    parametros.Add("@SUBRACK", dados.SUBRACK);
                    parametros.Add("@STATUS", dados.STATUS);
                    parametros.Add("@DHCP_DNS_PRIMARIO", dados.DHCP_DNS_PRIMARIO);
                    parametros.Add("@DHCP_DNS_SECUNDARIO", dados.DHCP_DNS_SECUNDARIO);
                    parametros.Add("@DHCP_LAN_IP", dados.DHCP_LAN_IP);
                    parametros.Add("@DHCP_LAN_MASCARA", dados.DHCP_LAN_MASCARA);
                    parametros.Add("@DHCP_INICIO_RANGE", dados.DHCP_INICIO_RANGE);
                    parametros.Add("@DHCP_FINAL_RANGE", dados.DHCP_FINAL_RANGE);
                    parametros.Add("@TESTE_IP1", dados.TESTE_IP1);
                    parametros.Add("@TESTE_IP2", dados.TESTE_IP2);
                    parametros.Add("@TIPO_IDENTIFICACAO", dados.TIPO_IDENTIFICACAO);
                    parametros.Add("@ID_FILIAL", dados.ID_FILIAL);
                    parametros.Add("@TIPO", dados.TIPO);
                    parametros.Add("@GERENCIA_REDEGENERICA", dados.GERENCIA_REDEGENERICA);
                    parametros.Add("@GERENCIA_MASCARAREDEGENERICA", dados.GERENCIA_MASCARAREDEGENERICA);
                    parametros.Add("@GERENCIA_LOGIN", dados.GERENCIA_LOGIN);
                    parametros.Add("@GERENCIA_SENHA", dados.GERENCIA_SENHA);
                    parametros.Add("@GERENCIA_PORTA_GENERICA", dados.GERENCIA_PORTA_GENERICA);
                    parametros.Add("@GERENCIA_VLAN", dados.GERENCIA_VLAN);
                    parametros.Add("@GERENCIA_SINALMINIMO", dados.GERENCIA_SINALMINIMO);
                    parametros.Add("@GERENCIA_SINALMAXIMO", dados.GERENCIA_SINALMAXIMO);
                    parametros.Add("@ID_SERVIDOR_GERENCIA", dados.ID_SERVIDOR_GERENCIA);
                    parametros.Add("@ID_POP", dados.ID_POP);
                    parametros.Add("@UTILIZA_CONVERSAO_MAC", dados.UTILIZA_CONVERSAO_MAC);
                    parametros.Add("@CODIGO_AUXILIAR", dados.CODIGO_AUXILIAR);
                    parametros.Add("@VLAN_UPLINK", dados.VLAN_UPLINK);
                    parametros.Add("@DELAY", dados.DELAY);
                    parametros.Add("@ID", dados.ID);

                    CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null).Wait();

                    // Atualização na tabela TB_OLT_GRUPO_AREA
                    foreach (var grupoArea in dados.GrupoAreaLista)
                    {
                        sql = new StringBuilder();
                        sql.Append("UPDATE TB_OLT_GRUPO_AREA SET ");
                        sql.Append("COD_AREA = @COD_AREA, ");
                        sql.Append("DESC_AREA = @DESC_AREA, ");
                        sql.Append("COD_ESCRITORIO = @COD_ESCRITORIO, ");
                        sql.Append("DESC_ESCRITORIO = @DESC_ESCRITORIO, ");
                        sql.Append("DATAHORAALTERACAO =CURRENT_TIMESTAMP, ");                        
                        sql.Append("USUARIOALTERACAO = 'INTEGRACAO' ");                       
                        sql.Append("WHERE ID_OLT = @ID_OLT AND COD_AREA = @COD_AREA");
                        parametros = new Dictionary<string, object>();
                        parametros.Add("@COD_AREA", grupoArea.Cod_Area);
                        parametros.Add("@DESC_AREA", grupoArea.Desc_Area);
                        parametros.Add("@COD_ESCRITORIO", grupoArea.Cod_Escritorio);
                        parametros.Add("@DESC_ESCRITORIO", grupoArea.Desc_Escritorio);
                        parametros.Add("@ID_OLT", dados.ID);

                        CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null).Wait();
                        retorno.Mensagem = "OK";
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.InnerException = erro.InnerException.ToString();
                retorno.Mensagem = erro.InnerException.ToString();
                retorno.Ok = false;
                return retorno;
            }
            return retorno;

        }




        public PostApiRetorno InsertOLT(OLTModels dados)
        {
            var retorno = new PostApiRetorno();
            try
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (ISession session = HibernateUtil.GetSession())
                using (DbCommand com = session.Connection.CreateCommand())
                {      // Monta a query de INSERT

                    StringBuilder sql = new StringBuilder();
                    var id = 0;
                    sql.Append("SELECT GEN_ID(GN_OLT, 1)   AS CO FROM RDB$DATABASE ");
                    using (DbDataReader reader = CommandHelper.ExecuteReader(com, sql, session.Connection, null).Result)
                    {
                        if (reader.Read())
                        {

                            id = Convert.ToInt32(reader["CO"]);
                        }
                    }
                    sql = new StringBuilder();
                    //USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN,
                    //@USUARIOCRIACAO, @DATAHORACRIACAO, @USUARIOALTERACAO, @DATAHORAALTERACAO, @TOKEN,
                    sql.Append("INSERT INTO TB_OLT (ID,USUARIOCRIACAO,DATAHORACRIACAO,DATAHORAALTERACAO,  OLTID, DESCRICAO, MARCA_MODELO, NUM_OLT, IP, LOGIN, SENHA, PORTA_TELNET, PORTA_API, PORTA_HTTP, PORTA, IP_TL1, LOGIN_TL1, SENHA_TL1, PORTA_TL1, GABINETE, SUBRACK, STATUS, DHCP_DNS_PRIMARIO, DHCP_DNS_SECUNDARIO, DHCP_LAN_IP, DHCP_LAN_MASCARA, DHCP_INICIO_RANGE, DHCP_FINAL_RANGE, TESTE_IP1, TESTE_IP2, TIPO_IDENTIFICACAO, ID_FILIAL, TIPO, GERENCIA_REDEGENERICA, GERENCIA_MASCARAREDEGENERICA, GERENCIA_LOGIN, GERENCIA_SENHA, GERENCIA_PORTA_GENERICA, GERENCIA_VLAN, GERENCIA_SINALMINIMO, GERENCIA_SINALMAXIMO, ID_SERVIDOR_GERENCIA, ID_POP, UTILIZA_CONVERSAO_MAC, CODIGO_AUXILIAR, VLAN_UPLINK, DELAY) ");
                    sql.Append("VALUES (@ID,'INTEGRACAO',CURRENT_TIMESTAMP,CURRENT_TIMESTAMP , @OLTID, @DESCRICAO, @MARCA_MODELO, @NUM_OLT, @IP, @LOGIN, @SENHA, @PORTA_TELNET, @PORTA_API, @PORTA_HTTP, @PORTA, @IP_TL1, @LOGIN_TL1, @SENHA_TL1, @PORTA_TL1, @GABINETE, @SUBRACK, @STATUS, @DHCP_DNS_PRIMARIO, @DHCP_DNS_SECUNDARIO, @DHCP_LAN_IP, @DHCP_LAN_MASCARA, @DHCP_INICIO_RANGE, @DHCP_FINAL_RANGE, @TESTE_IP1, @TESTE_IP2, @TIPO_IDENTIFICACAO, @ID_FILIAL, @TIPO, @GERENCIA_REDEGENERICA, @GERENCIA_MASCARAREDEGENERICA, @GERENCIA_LOGIN, @GERENCIA_SENHA, @GERENCIA_PORTA_GENERICA, @GERENCIA_VLAN, @GERENCIA_SINALMINIMO, @GERENCIA_SINALMAXIMO, @ID_SERVIDOR_GERENCIA, @ID_POP, @UTILIZA_CONVERSAO_MAC, @CODIGO_AUXILIAR, @VLAN_UPLINK, @DELAY)");
                    parametros.Add("@OLTID", dados.OLTID);
                    parametros.Add("@DESCRICAO", dados.DESCRICAO);
                    parametros.Add("@MARCA_MODELO", dados.MARCA_MODELO);
                    parametros.Add("@NUM_OLT", dados.NUM_OLT);
                    parametros.Add("@IP", dados.IP);
                    parametros.Add("@LOGIN", dados.LOGIN);
                    parametros.Add("@SENHA", dados.SENHA);
                    parametros.Add("@PORTA_TELNET", dados.PORTA_TELNET);
                    parametros.Add("@PORTA_API", dados.PORTA_API);
                    parametros.Add("@PORTA_HTTP", dados.PORTA_HTTP);
                    parametros.Add("@PORTA", dados.PORTA);
                    parametros.Add("@IP_TL1", dados.IP_TL1);
                    parametros.Add("@LOGIN_TL1", dados.LOGIN_TL1);
                    parametros.Add("@SENHA_TL1", dados.SENHA_TL1);
                    parametros.Add("@PORTA_TL1", dados.PORTA_TL1);
                    parametros.Add("@GABINETE", dados.GABINETE);
                    parametros.Add("@SUBRACK", dados.SUBRACK);
                    parametros.Add("@STATUS", dados.STATUS);
                    parametros.Add("@DHCP_DNS_PRIMARIO", dados.DHCP_DNS_PRIMARIO);
                    parametros.Add("@DHCP_DNS_SECUNDARIO", dados.DHCP_DNS_SECUNDARIO);
                    parametros.Add("@DHCP_LAN_IP", dados.DHCP_LAN_IP);
                    parametros.Add("@DHCP_LAN_MASCARA", dados.DHCP_LAN_MASCARA);
                    parametros.Add("@DHCP_INICIO_RANGE", dados.DHCP_INICIO_RANGE);
                    parametros.Add("@DHCP_FINAL_RANGE", dados.DHCP_FINAL_RANGE);
                    parametros.Add("@TESTE_IP1", dados.TESTE_IP1);
                    parametros.Add("@TESTE_IP2", dados.TESTE_IP2);
                    parametros.Add("@TIPO_IDENTIFICACAO", dados.TIPO_IDENTIFICACAO);
                    parametros.Add("@ID_FILIAL", dados.ID_FILIAL);
                    parametros.Add("@TIPO", dados.TIPO);
                    parametros.Add("@GERENCIA_REDEGENERICA", dados.GERENCIA_REDEGENERICA);
                    parametros.Add("@GERENCIA_MASCARAREDEGENERICA", dados.GERENCIA_MASCARAREDEGENERICA);
                    parametros.Add("@GERENCIA_LOGIN", dados.GERENCIA_LOGIN);
                    parametros.Add("@GERENCIA_SENHA", dados.GERENCIA_SENHA);
                    parametros.Add("@GERENCIA_PORTA_GENERICA", dados.GERENCIA_PORTA_GENERICA);
                    parametros.Add("@GERENCIA_VLAN", dados.GERENCIA_VLAN);
                    parametros.Add("@GERENCIA_SINALMINIMO", dados.GERENCIA_SINALMINIMO);
                    parametros.Add("@GERENCIA_SINALMAXIMO", dados.GERENCIA_SINALMAXIMO);
                    parametros.Add("@ID_SERVIDOR_GERENCIA", dados.ID_SERVIDOR_GERENCIA);
                    parametros.Add("@ID_POP", dados.ID_POP);
                    parametros.Add("@UTILIZA_CONVERSAO_MAC", dados.UTILIZA_CONVERSAO_MAC);
                    parametros.Add("@CODIGO_AUXILIAR", dados.CODIGO_AUXILIAR); ;
                    parametros.Add("@VLAN_UPLINK", dados.VLAN_UPLINK);
                    parametros.Add("@DELAY", dados.DELAY);
                    parametros.Add("@ID", id);
                    CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null).Wait();
                    // Inserção na tabela TB_OLT_GRUPO_AREA
                    foreach (var grupoArea in dados.GrupoAreaLista)
                    {
                        var idGrupo = 0;
                        sql = new StringBuilder();
                        sql.Append("SELECT MAX(ID)   AS CO FROM TB_OLT_GRUPO_AREA ");
                        using (DbDataReader reader = CommandHelper.ExecuteReader(com, sql, session.Connection, null).Result)
                        {
                            if (reader.Read())
                            {
                                idGrupo = Convert.ToInt32(reader["CO"]) + 1;
                            }
                        }
                        sql = new StringBuilder();
                        sql.Append("INSERT INTO TB_OLT_GRUPO_AREA (ID,ID_OLT, COD_AREA, DESC_AREA, COD_ESCRITORIO, DESC_ESCRITORIO) ");
                        sql.Append("VALUES (@ID,@ID_OLT, @COD_AREA, @DESC_AREA, @COD_ESCRITORIO, @DESC_ESCRITORIO)");
                        parametros = new Dictionary<string, object>();
                        parametros.Add("@ID", idGrupo);
                        parametros.Add("@ID_OLT", id);
                        parametros.Add("@COD_AREA", grupoArea.Cod_Area);
                        parametros.Add("@DESC_AREA", grupoArea.Desc_Area);
                        parametros.Add("@COD_ESCRITORIO", grupoArea.Cod_Escritorio);
                        parametros.Add("@DESC_ESCRITORIO", grupoArea.Desc_Escritorio);
                        CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null).Wait();
                    }
                    retorno.Mensagem = "OK";
                    retorno.Ok = true;
                    return retorno;
                }
            }
            catch (Exception erro)
            {
                retorno.InnerException = erro.InnerException.ToString();
                retorno.Mensagem = erro.InnerException.ToString();
                retorno.Ok = false;
                return retorno;

            }
        }


    }
}
