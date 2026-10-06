using Aniel.Connect.ControllersWebApi.Ativacao.Interfaces;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.T5;
using NHibernate;
using Sinapse.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sinapse.DAO.repository;
using Sinapse.Util.RemoteConnection;

namespace Aniel.Connect.ControllersWebApi.Ativação.Concentradoras
{
    public class Huawei : IntegracaoConcentradora
    {
        #region Configuração do Plano

        public async Task ConfiguracaoPlanoCliente(ISession session, LoginRadius login)
        {
            var planoCadastrado = false;

            string nomePlanoTratado = login.Cliente_PlanoProduto.Plano.Descricao.Replace(",", " ");

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql.AppendLine("select count(id) as qtd");
                sql.AppendLine("from radgroupcheck");
                sql.AppendLine("where groupname = @PLANO");


                param.Add("@PLANO", nomePlanoTratado);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                {
                    if (reader.Read())
                        planoCadastrado = Convert.ToInt32(reader["qtd"]) != 0;
                }
            }

            if (!planoCadastrado)
            {
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    parametros.Add("@ID_PLANO", login.Cliente_PlanoProduto.Plano.Id);
                    parametros.Add("@DESCRICAO_PLANO", nomePlanoTratado);
                    parametros.Add("@IDCLIENTE", login.Cliente.Id);

                    sql.Append("INSERT INTO radgroupcheck (groupname, `attribute`, op, value, Id_Plano)");
                    sql.Append("                   VALUES (@DESCRICAO_PLANO, 'Plano-Ativo', '=', 'S', @ID_PLANO);");

                    ConfigurarPlano(login, sql, parametros);

                    if (login.EnderecoIPV4 != null)
                    {
                        InserirIPV4Cliente(sql, parametros, login.Login, login.EnderecoIPV4);
                    }

                    if (login.EnderecoIPV6 != null)
                    {
                        InserirIPV6Cliente(sql, parametros, login.Login, login.EnderecoIPV6);
                    }

                    if (login.Senha_WIFI != null)
                    {
                        //InserirSenhaWifi(sql, parametros, login.Senha_WIFI, login.MAC);
                    }

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }
        }

        private static void ConfigurarPlano(LoginRadius login, StringBuilder sql, Dictionary<string, object> parametros)
        {
            if (login.Cliente_PlanoProduto.Plano.UtilizaQoSProfile)
            {
                sql.Append("INSERT INTO radgroupreply (groupname, `attribute`, op, value, Id_Plano) ");
                sql.Append("VALUES ");
                sql.Append($" (@DESCRICAO_PLANO, 'Huawei-Qos-Profile-Name', '=', @QOS_PROFILE, @ID_PLANO); "); //QoS Profile cadastrado na Concentradora contendo todos as velocidades definidas
                parametros.Add("@QOS_PROFILE", login.Cliente_PlanoProduto.Plano.IdQoSProfile);
            }
            else
            {
                sql.Append("INSERT INTO radgroupreply (groupname, `attribute`, op, value, Id_Plano) ");
                sql.Append("VALUES ");
                InserirVelocidade(Convert.ToDouble(login.Cliente_PlanoProduto.Plano.VelocidadeUpload), sql, parametros, "Huawei-Input-Average-Rate"/*"HW-Input-Committed-Information-Rate"*/); //velocidade mínima de upload a fornecer

                sql.Append("INSERT INTO radgroupreply (groupname, `attribute`, op, value, Id_Plano) ");
                sql.Append("VALUES ");
                InserirVelocidade(Convert.ToDouble(login.Cliente_PlanoProduto.Plano.VelocidadeUpload), sql, parametros, "Huawei-Input-Peak-Rate" /*"HW-Input-Peak-Information-Rate"*/); //velocidade máxima de upload

                sql.Append("INSERT INTO radgroupreply (groupname, `attribute`, op, value, Id_Plano) ");
                sql.Append("VALUES ");
                InserirVelocidade(Convert.ToDouble(login.Cliente_PlanoProduto.Plano.VelocidadeDownload), sql, parametros, "Huawei-Output-Average-Rate" /*"HW-Output-Committed-Information-Rate"*/); //velocidade mínima de download a fornecer

                sql.Append("INSERT INTO radgroupreply (groupname, `attribute`, op, value, Id_Plano) ");
                sql.Append("VALUES ");
                InserirVelocidade(Convert.ToDouble(login.Cliente_PlanoProduto.Plano.VelocidadeDownload), sql, parametros, "Huawei-Output-Peak-Rate" /*"HW-Output-Peak-Information-Rate"*/); //velocidade máxima de download (nominal)
            }

        }

        public async Task ReduzirVelocidade(ISession session, ISession sessionConnect, LoginRadius login)
        {
            try
            {
                bool velocidadeReduzida = false;

                if (login.Cliente_PlanoProduto.Plano.UtilizaQoSProfile)
                {
                    using (var checkCom = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        Dictionary<string, object> parametros = new Dictionary<string, object>();


                        sql.AppendLine("SELECT COUNT(id) AS qtd FROM radreply");
                        sql.AppendLine("WHERE id_Cliente = @ID_CLIENTE");
                        sql.AppendLine("  AND username = @USERNAME");
                        sql.AppendLine("AND `attribute` = 'Huawei-Qos-Profile-Name' ");

                        parametros.Add("@ID_CLIENTE", login.Cliente.Id);
                        parametros.Add("@USERNAME", login.Login);

                        using (var reader = await CommandHelper.ExecuteReader(checkCom, sql, parametros, session.Connection, null))
                        {
                            if (reader.Read())
                                velocidadeReduzida = Convert.ToInt32(reader["qtd"]) != 0;
                        }
                    }

                    if (!velocidadeReduzida)
                    {
                        using (var com = session.Connection.CreateCommand())
                        using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                        {
                            StringBuilder sql = new StringBuilder();
                            Dictionary<string, object> parametros = new Dictionary<string, object>();

                            sql.AppendLine("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente)");
                            sql.AppendLine("VALUES");
                            sql.Append($" (@USERNAME, 'Huawei-Qos-Profile-Name', '=', @QOS_PROFILE, @ID_CLIENTE); ");

                            parametros.Add("@QOS_PROFILE", login.Cliente_PlanoProduto.Plano.IdQoSProfileReduzido);
                            parametros.Add("@ID_CLIENTE", login.Cliente.Id);
                            parametros.Add("@USERNAME", login.Login);

                            await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);                            

                            // Comando para atualizar a velocidade do usuário na concentradora                            
                            AplicarModificacaoVelocidadeQoSProfile(login.Concentradora.Radius, login.Login, login.Concentradora.Ip, login.Cliente_PlanoProduto.Plano.IdQoSProfileReduzido);
                        }
                    }
                }
                else
                {
                    using (var checkCom = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        Dictionary<string, object> parametros = new Dictionary<string, object>();


                        sql.AppendLine("SELECT COUNT(id) AS qtd FROM radreply");
                        sql.AppendLine("WHERE id_Cliente = @ID_CLIENTE");
                        sql.AppendLine("  AND username = @USERNAME");
                        //sql.AppendLine("AND (`attribute` = 'HW-Input-Committed-Burst-Size' ");
                        sql.AppendLine("AND (`attribute` = 'Huawei-Input-Average-Rate' ");
                        sql.AppendLine("OR `attribute` = 'Huawei-Input-Peak-Rate' ");
                        //sql.AppendLine("OR `attribute` = 'HW-Output-Committed-Burst-Size' ");
                        sql.AppendLine("OR `attribute` = 'Huawei-Output-Average-Rate' ");
                        sql.AppendLine("OR `attribute` = 'Huawei-Output-Peak-Rate') ");

                        parametros.Add("@ID_CLIENTE", login.Cliente.Id);
                        parametros.Add("@USERNAME", login.Login);

                        using (var reader = await CommandHelper.ExecuteReader(checkCom, sql, parametros, session.Connection, null))
                        {
                            if (reader.Read())
                                velocidadeReduzida = Convert.ToInt32(reader["qtd"]) != 0;
                        }
                    }

                    if (!velocidadeReduzida)
                    {
                        using (var com = session.Connection.CreateCommand())
                        using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                        {
                            StringBuilder sql = new StringBuilder();
                            Dictionary<string, object> parametros = new Dictionary<string, object>();

                            sql.AppendLine("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente)");
                            sql.AppendLine("VALUES");
                            InserirVelocidadeReduzida(Convert.ToDouble(login.Cliente_PlanoProduto.Plano.VelocidadeUploadReduzida), sql, parametros, "Huawei-Input-Average-Rate");

                            sql.AppendLine("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente)");
                            sql.AppendLine("VALUES");
                            InserirVelocidadeReduzida(Convert.ToDouble(login.Cliente_PlanoProduto.Plano.VelocidadeUploadReduzida), sql, parametros, "Huawei-Input-Peak-Rate");

                            sql.AppendLine("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente)");
                            sql.AppendLine("VALUES");
                            InserirVelocidadeReduzida(Convert.ToDouble(login.Cliente_PlanoProduto.Plano.VelocidadeDownloadReduzida), sql, parametros, "Huawei-Output-Average-Rate");

                            sql.AppendLine("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente)");
                            sql.AppendLine("VALUES");
                            InserirVelocidadeReduzida(Convert.ToDouble(login.Cliente_PlanoProduto.Plano.VelocidadeDownloadReduzida), sql, parametros, "Huawei-Output-Peak-Rate");

                            parametros.Add("@ID_CLIENTE", login.Cliente.Id);
                            parametros.Add("@USERNAME", login.Login);

                            await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                            var planoProduto = login.Cliente_PlanoProduto;
                            planoProduto.Velocidade_Reduzida = true;

                            await repoClientePlanoProduto.Update(planoProduto);

                            // Comando para atualizar a velocidade do usuário na concentradora
                            var strVelocidade = GetVelocidadeReduzidaHuawei(planoProduto.Plano);
                            AplicarModificacaoVelocidade(login.Concentradora.Radius, login.Login, login.Concentradora.Ip, strVelocidade);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task NormalizarVelocidade(ISession session, ISession sessionConnect, LoginRadius login)
        {
            using (var com = session.Connection.CreateCommand())
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("DELETE FROM radreply");
                sql.AppendLine("WHERE (`attribute` = 'Huawei-Input-Burst-Size' ");
                sql.AppendLine("OR `attribute` = 'Huawei-Input-Average-Rate' ");
                sql.AppendLine("OR `attribute` = 'Huawei-Input-Peak-Rate' ");
                sql.AppendLine("OR `attribute` = 'Huawei-Output-Burst-Size' ");
                sql.AppendLine("OR `attribute` = 'Huawei-Output-Average-Rate' ");
                sql.AppendLine("OR `attribute` = 'Huawei-Qos-Profile-Name' ");
                sql.AppendLine("OR `attribute` = 'Huawei-Output-Peak-Rate') ");
                sql.AppendLine("  AND id_Cliente = @ID_CLIENTE");
                sql.AppendLine("  AND username = @USERNAME");

                parametros.Add("@ID_CLIENTE", login.Cliente.Id);
                parametros.Add("@USERNAME", login.Login);

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);                

                // Comando para atualizar a velocidade do usuário na concentradora
                if(login.Cliente_PlanoProduto.Plano.UtilizaQoSProfile)
                {
                    AplicarModificacaoVelocidadeQoSProfile(login.Concentradora.Radius, login.Login, login.Concentradora.Ip, login.Cliente_PlanoProduto.Plano.IdQoSProfile);
                }
                else
                {
                    var strVelocidade = GetVelocidadeHuawei(planoProduto.Plano);
                    AplicarModificacaoVelocidade(login.Concentradora.Radius, login.Login, login.Concentradora.Ip, strVelocidade);
                }
                
            }
        }

        #endregion

        #region Comandos Radius

        public void AplicarModificacaoVelocidade(Model.nhibernate.Radius servidor, string usuario, string ipConcetradora, string velocidade)
        {
            using (var rc = new RemoteConnection(servidor.Ip, Convert.ToInt32(servidor.Porta_SSH), servidor.Dominio, servidor.Senha, Protocol.Ssh))
            {
                rc.Connect();

                if (rc.IsConnected())
                {
                    string[] Velocidades_Huawei = velocidade.Split('|');
                    String Download_Cir = Velocidades_Huawei[0];
                    String Download_Pir = Velocidades_Huawei[1];
                    String Upload_Cir = Velocidades_Huawei[2];
                    String Upload_Pir = Velocidades_Huawei[3];

                    if(CONSTANTES.UsaRadiusSemContainer)
                    {
                        string comando = $"echo \"User-Name = {usuario}, Huawei-Input-Average-Rate = \"{Download_Cir}\"\" | radclient " + ipConcetradora + " coa testing123";
                        rc.ExecuteCommand(comando);

                        comando = $"echo \"User-Name = {usuario}, Huawei-Input-Peak-Rate = \"{Download_Pir}\"\" | radclient " + ipConcetradora + " coa testing123";
                        rc.ExecuteCommand(comando);

                        comando = $"echo \"User-Name = {usuario}, Huawei-Output-Average-Rate = \"{Upload_Cir}\"\" | radclient " + ipConcetradora + " coa testing123";
                        rc.ExecuteCommand(comando);

                        comando = $"echo \"User-Name = {usuario}, Huawei-Output-Peak-Rate = \"{Upload_Pir}\"\" | radclient " + ipConcetradora + " coa testing123";
                        rc.ExecuteCommand(comando);
                    }
                    else
                    {
                        string comando = $"docker exec -i radius bash -c 'echo \"User-Name = {usuario}, Huawei-Input-Average-Rate = \"{Download_Cir}\"\" | radclient " + ipConcetradora + " coa testing123'";
                        rc.ExecuteCommand(comando);

                        comando = $"docker exec -i radius bash -c 'echo \"User-Name = {usuario}, Huawei-Input-Peak-Rate = \"{Download_Pir}\"\" | radclient " + ipConcetradora + " coa testing123'";
                        rc.ExecuteCommand(comando);

                        comando = $"docker exec -i radius bash -c 'echo \"User-Name = {usuario}, Huawei-Output-Average-Rate = \"{Upload_Cir}\"\" | radclient " + ipConcetradora + " coa testing123'";
                        rc.ExecuteCommand(comando);

                        comando = $"docker exec -i radius bash -c 'echo \"User-Name = {usuario}, Huawei-Output-Peak-Rate = \"{Upload_Pir}\"\" | radclient " + ipConcetradora + " coa testing123'";
                        rc.ExecuteCommand(comando);
                    }
                    
                }
            }
        }

        public void AplicarModificacaoVelocidadeQoSProfile(Model.nhibernate.Radius servidor, string usuario, string ipConcetradora, string idQoSProfile)
        {
            using (var rc = new RemoteConnection(servidor.Ip, Convert.ToInt32(servidor.Porta_SSH), servidor.Dominio, servidor.Senha, Protocol.Ssh))
            {
                rc.Connect();

                if (rc.IsConnected())
                {
                    if (CONSTANTES.UsaRadiusSemContainer)
                    {
                        string comando = $"echo \"User-Name = {usuario}, Huawei-Qos-Profile-Name = \"{idQoSProfile}\"\" | radclient " + ipConcetradora + " coa testing123";
                        rc.ExecuteCommand(comando);
                    }
                    else
                    {
                        string comando = $"docker exec -i radius bash -c 'echo \"User-Name = {usuario}, Huawei-Qos-Profile-Name = \"{idQoSProfile}\"\" | radclient " + ipConcetradora + " coa testing123'";
                        rc.ExecuteCommand(comando);
                    }
                    
                }
            }
        }

        #endregion

        #region Query de inserção do plano

        //Pesquisar um atributo no Radius para poder utilizar PSK com a Huawei
        private void InserirSenhaWifi(StringBuilder sql, Dictionary<string, object> parametros, string senha_WIFI, string mac)
        {
            sql.Append("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente) values (@MAC, 'Mikrotik-Wireless-PSK', ':=', @SENHA_WIFI, @IDCLIENTE);");
            parametros.Add("@MAC", mac);
            parametros.Add("@SENHA_WIFI", senha_WIFI);
        }

        private void InserirIPV6Cliente(StringBuilder sql, Dictionary<string, object> parametros, string login, string enderecoIPV6)
        {
            sql.Append("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente) VALUES (@USERNAME, 'Huawei-Framed-IPv6-Address', '=', @ENDERECO_IPV6, @IDCLIENTE);");

            parametros.Add("@ENDERECO_IPV6", enderecoIPV6);

            if (!parametros.ContainsKey("@USERNAME"))
                parametros.Add("@USERNAME", login);
        }

        private void InserirIPV4Cliente(StringBuilder sql, Dictionary<string, object> parametros, string login, string enderecoIPV4)
        {
            sql.Append("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente) VALUES (@USERNAME, 'Framed-IP-Address', '=', @ENDERECO_IPV4, @IDCLIENTE);");

            parametros.Add("@ENDERECO_IPV4", enderecoIPV4);

            if (!parametros.ContainsKey("@USERNAME"))
                parametros.Add("@USERNAME", login);
        }

        private static void InserirVelocidade(Double velocidade, StringBuilder sql, Dictionary<string, object> parametros, String campoSql, bool Converter_Velocidade_Real = true)
        {

            sql.Append($" (@DESCRICAO_PLANO, '{campoSql}', '=', @VELOCIDADE_PLANO, @ID_PLANO); ");
            parametros.Add("@VELOCIDADE_PLANO", ConvertMbpsToKbps(velocidade, Converter_Velocidade_Real));
        }

        private static void InserirVelocidadeReduzida(Double velocidade, StringBuilder sql, Dictionary<string, object> parametros, String campoSql, bool Converter_Velocidade_Real = true)
        {
            sql.Append($" (@USERNAME, '{campoSql}', '=', @VELOCIDADE_PLANO, @ID_CLIENTE); ");
            parametros.Add("@VELOCIDADE_PLANO", ConvertMbpsToKbps(velocidade, Converter_Velocidade_Real));
        }

        #endregion

        #region Velocidade Huawei

        private static string GetVelocidadeHuawei(Plano plano)
        {
            String Download_Cir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeDownload));
            String Download_Pir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeDownload));
            String Upload_Cir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeUpload));
            String Upload_Pir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeUpload));


            return $"{Download_Cir}|{Download_Pir}|{Upload_Cir}|{Upload_Pir}";
        }

        private static string GetVelocidadeReduzidaHuawei(Plano plano)
        {
            String Download_Cir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeDownloadReduzida));
            String Download_Pir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeDownloadReduzida));
            String Upload_Cir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeUploadReduzida));
            String Upload_Pir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeUploadReduzida));


            return $"{Download_Cir}|{Download_Pir}|{Upload_Cir}|{Upload_Pir}";
        }

        private static String ConvertMbpsToKbps(Double Velocidade_Mb, bool Converter_Velocidade_Real = true)
        {
            var Velocidade_Kbps = Velocidade_Mb * 1000;

            //Cálculo será feito para pegar a velocidade real em Kbps (em múltiplos de 8 bits). Ex: 2Mbps = 2048Kbps     
            //Para facilitar o cálculo está sendo multiplicado em Kbits (1 Kbit é equivalente a 1024 bits) 
            if (Converter_Velocidade_Real)
            {
                var Velocidade_Kbps_Real = Velocidade_Mb * 1024;

                return Convert.ToString(Velocidade_Kbps_Real);
            }
            return Convert.ToString(Velocidade_Kbps);
        }
        #endregion

        public async Task EditarPlanoCliente(ISession session, Plano plano)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();


                sql.AppendLine(" UPDATE radgroupreply SET ");
                sql.AppendLine(" groupname = @GROUPNAME, ");
                sql.AppendLine(" value = @VALUE1 ");
                sql.AppendLine(" WHERE Id_Plano = @ID_PLANO ");
                sql.AppendLine(" AND `attribute` = 'Huawei-Input-Average-Rate'; ");

                sql.AppendLine(" UPDATE radgroupreply SET ");
                sql.AppendLine(" groupname = @GROUPNAME, ");
                sql.AppendLine(" value = @VALUE2 ");
                sql.AppendLine(" WHERE Id_Plano = @ID_PLANO ");
                sql.AppendLine(" AND `attribute` = 'Huawei-Input-Peak-Rate'; ");

                sql.AppendLine(" UPDATE radgroupreply SET ");
                sql.AppendLine(" groupname = @GROUPNAME, ");
                sql.AppendLine(" value = @VALUE3 ");
                sql.AppendLine(" WHERE Id_Plano = @ID_PLANO ");
                sql.AppendLine(" AND `attribute` = 'Huawei-Output-Average-Rate'; ");

                sql.AppendLine(" UPDATE radgroupreply SET ");
                sql.AppendLine(" groupname = @GROUPNAME, ");
                sql.AppendLine(" value = @VALUE4 ");
                sql.AppendLine(" WHERE Id_Plano = @ID_PLANO ");
                sql.AppendLine(" AND `attribute` = 'Huawei-Output-Peak-Rate'; ");

                #region Dados Plano              

                String Download_Cir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeDownload));
                String Download_Pir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeDownload));
                String Upload_Cir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeUpload));
                String Upload_Pir = ConvertMbpsToKbps(Convert.ToDouble(plano.VelocidadeUpload));

                #endregion

                string nomePlanoTratado = plano.Descricao.Replace(",", " ");

                parametros.Add("@GROUPNAME", nomePlanoTratado);
                parametros.Add("@VALUE1", $"{Upload_Cir}");
                parametros.Add("@VALUE2", $"{Upload_Pir}");
                parametros.Add("@VALUE3", $"{Download_Cir}");
                parametros.Add("@VALUE4", $"{Download_Pir}");
                parametros.Add("@ID_PLANO", plano.Id);

                sql.AppendLine(" UPDATE radgroupcheck SET ");
                sql.AppendLine(" groupname = @GROUPNAME ");
                sql.AppendLine(" value = @STATUS_PLANO ");
                sql.AppendLine(" WHERE Id_Plano = @ID_PLANO ");
                sql.AppendLine("   AND `attribute` = 'Plano-Ativo'; ");

                parametros.Add("@STATUS_PLANO", plano.SituacaoPlano == Model.enumerado.Situacao_plano.Ativo ? "S" : "N");

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task DesbloqueioConfianca(ISession session, Cliente_PlanoProduto planoProduto, String usuario)
        {
            List<AreaModuloModel> areas = new List<AreaModuloModel>();

            using (var acessoApi = new AcessoApiController())
            {
                areas = (await acessoApi.GetArea_ModuloUsuario(usuario)).Objeto;
            }

            if (areas.Where(p => p.Cod_Area == "183").Count() == 0)
            {
                if (planoProduto.Data_Desbloqueio_Confianca.HasValue)
                {
                    var periodicidadeDesbloqueio = planoProduto.Plano?.QtdDiasdesbloqueados; // TODO: Ler o novo campo a ser cadastrado no plano
                    var qtdDiasDesbloqueio = DateTime.Now.Date - planoProduto.Data_Desbloqueio_Confianca.Value;

                    if (qtdDiasDesbloqueio.TotalDays <= periodicidadeDesbloqueio)
                        throw new EAnielException("O cliente efetuou um desbloqueio em confiança recentemente.");
                }
            }

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("UPDATE radgroupcheck SET");
                sql.AppendLine("groupname = @GROUPNAME,");
                sql.AppendLine("value = 'S'");
                sql.AppendLine("WHERE Id_Plano = @ID_PLANO");
                sql.AppendLine("  AND `attribute` = 'Plano-Ativo'");

                string nomePlanoTratado = planoProduto.Plano.Descricao.Replace(",", " ");

                parametros.Add("@ID_PLANO", planoProduto.Plano.Id);
                parametros.Add("@GROUPNAME", nomePlanoTratado);

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task DeletarPlano(ISession session, Int64 IdClientePlanoProduto)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (var com = session.Connection.CreateCommand())
            {
                sql.AppendLine(" SELECT COUNT(*) as Usuarios_Ativos ");
                sql.AppendLine(" FROM radusergroup radusr ");

                sql.AppendLine(" WHERE idCliente_PlanoProduto = @ID_CLIENTE_PLANO_PRODUTO ");
                sql.AppendLine("   AND(SELECT rdc.value ");
                sql.AppendLine("       FROM radcheck rdc ");
                sql.AppendLine("       WHERE rdc.username = radusr.username ");
                sql.AppendLine("         AND rdc.`attribute` = 'Usuario-Ativo') = 'S' ");

                parametros.Add("@ID_CLIENTE_PLANO_PRODUTO", IdClientePlanoProduto);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.Result.ReadAsync())
                    {
                        if (Convert.ToInt32(reader.Result["Usuarios_Ativos"]) > 0)
                        {
                            throw new EAnielException("Existem Clientes associados a esse plano");
                        }
                    }
                }
            }

            using (var deleteCom = session.Connection.CreateCommand())
            {
                sql.Clear();
                sql.Append("DELETE FROM radusergroup WHERE idCliente_PlanoProduto = @ID_CLIENTE_PLANO_PRODUTO");

                await CommandHelper.ExecuteNonQuery(deleteCom, sql, parametros, session.Connection, null);
            }
        }       
    }
}