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

namespace Aniel.Connect.ControllersWebApi.Ativacao.Concentradoras
{
    public class Mikrotik : IntegracaoConcentradora
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
                        InserirSenhaWifi(sql, parametros, login.Senha_WIFI, login.MAC);
                    }

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }
        }

        private static void ConfigurarPlano(LoginRadius login, StringBuilder sql, Dictionary<string, object> parametros)
        {
            sql.Append("INSERT INTO radgroupreply (groupname, `attribute`, op, value, Id_Plano) ");
            sql.Append("VALUES ");
            InserirVelocidade(login.Cliente_PlanoProduto.Plano, sql, parametros);
        }

        public async Task ReduzirVelocidade(ISession session, ISession sessionConnect, LoginRadius login)
        {
            try
            {
                bool velocidadeReduzida = false;

                using (var checkCom = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    sql.AppendLine("SELECT COUNT(id) AS qtd FROM radreply");
                    sql.AppendLine("WHERE id_Cliente = @ID_CLIENTE");
                    sql.AppendLine("  AND username = @USERNAME");
                    sql.AppendLine("  AND `attribute` = 'Mikrotik-Rate-Limit'");

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

                        // O Mikrotik interpreta o primeiro registro como limitação do upload e o segundo como limitação do download

                        sql.AppendLine("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente)");
                        sql.AppendLine("VALUES");
                        InserirVelocidadeReduzida(login.Cliente_PlanoProduto.Plano, sql, parametros);

                        parametros.Add("@ID_CLIENTE", login.Cliente.Id);
                        parametros.Add("@USERNAME", login.Login);
                       
                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);                       

                        // Comando para atualizar a velocidade do usuário na concentradora
                        var strVelocidade = GetVelocidadeReduzidaMikrotik(login.Cliente_PlanoProduto.Plano);
                        AplicarModificacaoVelocidade(login.Concentradora.Radius, login.Login, login.Concentradora.Ip, strVelocidade);
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
                sql.AppendLine("WHERE `attribute` = 'Mikrotik-Rate-Limit'");
                sql.AppendLine("  AND id_Cliente = @ID_CLIENTE");
                sql.AppendLine("  AND username = @USERNAME");

                parametros.Add("@ID_CLIENTE", login.Cliente.Id);
                parametros.Add("@USERNAME", login.Login);

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
               
                // Comando para atualizar a velocidade do usuário na concentradora
                var strVelocidade = GetVelocidadeMikrotik(login.Cliente_PlanoProduto.Plano);
                AplicarModificacaoVelocidade(login.Concentradora.Radius, login.Login, login.Concentradora.Ip, strVelocidade);
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
                    velocidade = "\\\"" + velocidade + "\\\"";
                    string comando = "docker exec -i radius bash -c 'echo \"User-Name = " + usuario + ", Mikrotik-Rate-Limit = " + velocidade + "\" | radclient " + ipConcetradora + " coa testing123'";
                    
                    rc.ExecuteCommand(comando);
                }
            }
        }

        #endregion

        #region Query de inserção do plano

        private void InserirSenhaWifi(StringBuilder sql, Dictionary<string, object> parametros, string senha_WIFI, string mac)
        {
            sql.Append("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente) values (@MAC, 'Mikrotik-Wireless-PSK', ':=', @SENHA_WIFI, @IDCLIENTE);");
            parametros.Add("@MAC", mac);
            parametros.Add("@SENHA_WIFI", senha_WIFI);
        }

        private void InserirIPV6Cliente(StringBuilder sql, Dictionary<string, object> parametros, string login, string enderecoIPV6)
        {
            sql.Append("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente) VALUES (@USERNAME, 'Framed-IPv6-Address', '=', @ENDERECO_IPV6, @IDCLIENTE);");

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

        private static void InserirVelocidade(Plano plano, StringBuilder sql, Dictionary<string, object> parametros)
        {

            sql.Append($" (@DESCRICAO_PLANO, 'Mikrotik-Rate-Limit', '=', @VELOCIDADE_PLANO, @ID_PLANO); ");
            parametros.Add("@VELOCIDADE_PLANO", GetVelocidadeMikrotik(plano));
        }

        private static void InserirVelocidadeReduzida(Plano plano, StringBuilder sql, Dictionary<string, object> parametros)
        {
            sql.Append($" (@USERNAME, 'Mikrotik-Rate-Limit', '=', @VELOCIDADE_PLANO, @ID_CLIENTE); ");
            parametros.Add("@VELOCIDADE_PLANO", GetVelocidadeReduzidaMikrotik(plano));
        }

        #endregion

        #region Velocidade Miktrotik

        private static string GetVelocidadeMikrotik(Plano plano)
        {
            double upload = Convert.ToDouble(plano.VelocidadeUpload) * 1000;
            double download = Convert.ToDouble(plano.VelocidadeDownload) * 1000;

            double limiteMaximoUpload = 0;
            double limiteMaximoDownload = 0;

            double uploadReduzido = Convert.ToDouble(plano.VelocidadeUploadReduzida) * 1000;
            double downloadReduzido = Convert.ToDouble(plano.VelocidadeDownloadReduzida) * 1000;

            Int64 tempoUpload = 0;
            Int64 tempoDownload = 0;

            double garantiaUpload = (Convert.ToDouble(plano.VelocidadeUpload) * 0.4) * 1000;
            double garantiaDownload = (Convert.ToDouble(plano.VelocidadeDownload) * 0.4) * 1000;

            int prioridade = 8;

            return $"{(upload > 0 ? upload.ToString() + "k" : "0")}/{(download > 0 ? download.ToString() + "k" : "0")} {(limiteMaximoUpload > 0 ? limiteMaximoUpload.ToString() + "k" : "0")}/{(limiteMaximoDownload > 0 ? limiteMaximoDownload.ToString() + "k" : "0")} {(uploadReduzido > 0 ? uploadReduzido.ToString() + "k" : "0")}/{(downloadReduzido > 0 ? downloadReduzido.ToString() + "k" : "0")} {tempoUpload}/{tempoDownload} {prioridade} {(garantiaUpload > 0 ? garantiaUpload.ToString() + "k" : "0")}/{(garantiaDownload > 0 ? garantiaDownload.ToString() + "k" : "0")} ";
        }

        private static string GetVelocidadeReduzidaMikrotik(Plano plano)
        {
            double upload = Convert.ToDouble(plano.VelocidadeUploadReduzida) * 1000;
            double download = Convert.ToDouble(plano.VelocidadeDownloadReduzida) * 1000;

            double limiteMaximoUpload = 0;
            double limiteMaximoDownload = 0;

            double uploadReduzido = upload;
            double downloadReduzido = download;

            Int64 tempoUpload = 0;
            Int64 tempoDownload = 0;

            double garantiaUpload = (Convert.ToDouble(plano.VelocidadeUploadReduzida) * 0.4) * 1000;
            double garantiaDownload = (Convert.ToDouble(plano.VelocidadeDownloadReduzida) * 0.4) * 1000;

            // Prioridade mais alta para se sobrepor à velocidade padrão do plano
            int prioridade = 1;

            return $"{(upload > 0 ? upload.ToString() + "k" : "0")}/{(download > 0 ? download.ToString() + "k" : "0")} {(limiteMaximoUpload > 0 ? limiteMaximoUpload.ToString() + "k" : "0")}/{(limiteMaximoDownload > 0 ? limiteMaximoDownload.ToString() + "k" : "0")} {(uploadReduzido > 0 ? uploadReduzido.ToString() + "k" : "0")}/{(downloadReduzido > 0 ? downloadReduzido.ToString() + "k" : "0")} {tempoUpload}/{tempoDownload} {prioridade} {(garantiaUpload > 0 ? garantiaUpload.ToString() + "k" : "0")}/{(garantiaDownload > 0 ? garantiaDownload.ToString() + "k" : "0")} ";
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
                sql.AppendLine(" value = @VALUE ");
                sql.AppendLine(" WHERE Id_Plano = @ID_PLANO ");
                sql.AppendLine("   AND `attribute` = 'Mikrotik-Rate-Limit'; ");

                #region Dados Plano

                var upload = Convert.ToDouble(plano.VelocidadeUpload) * 1000;
                var download = Convert.ToDouble(plano.VelocidadeDownload) * 1000;

                var limiteMaxUpload = 0;
                var limiteMaxDownload = 0;

                var uploadReduzido = Convert.ToDouble(plano.VelocidadeUploadReduzida) * 1000;
                var downloadReduzido = Convert.ToDouble(plano.VelocidadeDownloadReduzida) * 1000;

                var tempoDownload = 0;
                var tempoUpload = 0;

                var garantiaUpload = (Convert.ToDouble(plano.VelocidadeUpload) * 0.4) * 1000;
                var garantiaDownload = (Convert.ToDouble(plano.VelocidadeDownload) * 0.4) * 1000;

                int prioridade = 8;

                var uploadStr = upload > 0 ? upload.ToString() + "k" : "0";
                var downloadStr = download > 0 ? download.ToString() + "k" : "0";

                var limiteMaxUploadStr = limiteMaxUpload > 0 ? limiteMaxUpload.ToString() + "k" : "0";
                var limiteMaxDownloadStr = limiteMaxDownload > 0 ? limiteMaxDownload.ToString() + "k" : "0";

                var uploadReduzidoStr = uploadReduzido > 0 ? uploadReduzido.ToString() + "k" : "0";
                var downloadReduzidoStr = downloadReduzido > 0 ? downloadReduzido.ToString() + "k" : "0";

                var garantiaUploadStr = garantiaUpload > 0 ? garantiaUpload.ToString() + "k" : "0";
                var garantiaDownloadStr = garantiaDownload > 0 ? garantiaDownload.ToString() + "k" : "0";

                #endregion

                string nomePlanoTratado = plano.Descricao.Replace(",", " ");

                parametros.Add("@GROUPNAME", nomePlanoTratado);
                parametros.Add("@VALUE", $"{uploadStr}/{downloadStr} {limiteMaxUploadStr}/{limiteMaxDownloadStr} {uploadReduzidoStr}/{downloadReduzidoStr} {tempoUpload}/{tempoDownload} {prioridade} {garantiaUploadStr}/{garantiaDownloadStr}");
                parametros.Add("@ID_PLANO", plano.Id);

                sql.AppendLine(" UPDATE radgroupcheck SET ");
                sql.AppendLine(" groupname = @GROUPNAME ");
                sql.AppendLine(" value = @STATUS_PLANO ");
                sql.AppendLine(" WHERE Id_Plano = @ID_PLANO ");
                sql.AppendLine("   AND `attribute` = 'Plano-Ativo' ");

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
                    //Informação presente no Modelo de Contrato
                    //var periodicidadeDesbloqueio = planoProduto.Plano?.QtdDiasdesbloqueados; // TODO: Ler o novo campo a ser cadastrado no plano
                    var qtdDiasDesbloqueio = DateTime.Now.Date - planoProduto.Data_Desbloqueio_Confianca.Value;
                    //mudar esse campo para representar o valor correto
                    if (qtdDiasDesbloqueio.TotalDays <= 0)
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