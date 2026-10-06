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
using System.Threading.Tasks;
using Sinapse.Integracao.Ativacao.Models;

namespace Aniel.Connect.Repository.T5.Olt
{
    public class ONURepository : IONURepository
    {
        private readonly ISession _sessionConnNh;

        public async Task InsertONU(ConfigurarONUModel model, string usuarioCriacao)
        {
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" INSERT INTO TB_ONU (ID, DATAHORACRIACAO, USUARIOCRIACAO, DESCRICAO, MAC, SERIAL, GEM_PORT, SERVICEPORT, SENORID, LOGIN, ");
                sql.AppendLine(" PORTA_TELNET, PORTA_HTTP, VLAN_UPLINK, VLAN_PPPOE, VLAN_DHCP, VLAN_IPTV, VLAN_VOIP, VLAN_TR69, VLAN_OUTROS, VLAN_PROFILE, ID_LOGIN_RADIUS, ID_CLIENTE_PLANO_PRODUTO, ID_OLT, ID_SLOT, ID_PON)");
                sql.AppendLine($" VALUES ((SELECT GEN_ID(GN_ONU, 1) FROM RDB$DATABASE), CURRENT_TIMESTAMP, @USUARIOCRIACAO, @DESCRICAO, @MAC, @SERIAL, @GEM_PORT, @SERVICEPORT, @SENORID, @LOGIN, ");
                sql.AppendLine($" @PORTA_TELNET, @PORTA_HTTP, @VLAN_UPLINK, @VLAN_PPPOE, @VLAN_DHCP, @VLAN_IPTV, @VLAN_VOIP, @VLAN_TR69, @VLAN_OUTROS, @VLAN_PROFILE, @ID_LOGIN_RADIUS, @ID_CLIENTE_PLANO_PRODUTO, @ID_OLT, @ID_SLOT, @ID_PON) ");

                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@USUARIOCRIACAO", usuarioCriacao);
                parametros.Add("@DESCRICAO", model.Descricao);
                parametros.Add("@MAC", model.Mac);
                parametros.Add("@SERIAL", model.Serial);
                parametros.Add("@GEM_PORT", model.GemPort);
                parametros.Add("@SERVICEPORT", model.ServicePort);
                parametros.Add("@SENORID", model.SenorId);
                parametros.Add("@LOGIN", model.Login);
                parametros.Add("@PORTA_TELNET", model.PortaTelnet);
                parametros.Add("@PORTA_HTTP", model.PortaHttp);
                parametros.Add("@VLAN_UPLINK", model.VLAN_Uplink);
                parametros.Add("@VLAN_PPPOE", model.VLAN_PPPOE);
                parametros.Add("@VLAN_DHCP", model.VLAN_DHCP);
                parametros.Add("@VLAN_IPTV", model.VLAN_IPTV);
                parametros.Add("@VLAN_VOIP", model.VLAN_VOIP);
                parametros.Add("@VLAN_TR69", model.VLAN_TR69);
                parametros.Add("@VLAN_OUTROS", model.VLAN_Outros);
                parametros.Add("@VLAN_PROFILE", model.VLAN_Profile);
                parametros.Add("@ID_LOGIN_RADIUS", model.IdLogin);
                parametros.Add("@ID_CLIENTE_PLANO_PRODUTO", model.IdPlano);
                parametros.Add("@ID_OLT", model.IdOLT);
                parametros.Add("@ID_SLOT", model.IdSlot);
                parametros.Add("@ID_PON", model.IdPon);

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task<Sinapse.Integracao.Ativacao.Models.OnuExistenteModel> ExisteONUMesmoSerialOuLogin(string serial, string login)
        {
            OnuExistenteModel retorno = null;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 A.ID_LOGIN_RADIUS, COALESCE(A.SERIAL,A.MAC) AS SERIAL_EQUIPAMENTO FROM TB_ONU A ");
                sql.AppendLine(" LEFT JOIN TB_LOGIN_RADIUS B ");
                sql.AppendLine(" ON (A.ID_LOGIN_RADIUS = B.ID) ");
                sql.AppendLine(" WHERE UPPER(A.SERIAL) = UPPER(@SERIAL) OR UPPER(A.MAC) = UPPER(@SERIAL) OR B.LOGIN = @LOGIN ");

                parametros.Add("@SERIAL", serial);
                parametros.Add("@LOGIN", login);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new OnuExistenteModel();
                        retorno.MesmoLogin = reader["LOGIN"] != DBNull.Value && Convert.ToString(reader["LOGIN"]) == login ? true : false;
                        retorno.MesmoSerial = reader["SERIAL_EQUIPAMENTO"] != DBNull.Value && Convert.ToString(reader["SERIAL_EQUIPAMENTO"]).ToUpperInvariant() == serial.ToUpperInvariant() ? true : false;
                    }
                }
            }

            return retorno;
        }

        public async Task<bool> ExisteONUMesmoONTID(long idOLT, long idSlot, long idPon, int ONTID)
        {
            bool retorno = false;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 A.ID FROM TB_ONU A ");
                sql.AppendLine(" WHERE A.ID_OLT = @ID_OLT AND A.ID_SLOT = @ID_SLOT AND A.ID_PON = @ID_PON AND ONT_ID = @ONTID ");

                parametros.Add("@ID_OLT", idOLT);
                parametros.Add("@ID_SLOT", idSlot);
                parametros.Add("@ID_PON", idPon);
                parametros.Add("@ONTID", ONTID);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = true;
                    }
                }
            }

            return retorno;
        }


    }
}
