using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class LoginRadiusMap : ClassMapBase<LoginRadius>
    {
        public LoginRadiusMap() : base()
        {
            Table("TB_LOGIN_RADIUS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOGIN_RADIUS");
            Map(u => u.Status, "STATUS").CustomType<Status_Default>();
            Map(u => u.Login, "LOGIN");
            Map(u => u.EnderecoIPV4, "ENDERECO_IPV4");
            Map(u => u.EnderecoIPV6, "ENDERECO_IPV6");
            Map(u => u.SenhaPPPOE, "SENHA_PPPOE");
            Map(u => u.UsuarioRoteador, "USUARIO_ROTEADOR");
            Map(u => u.SenhaRoteador, "SENHA_ROTEADOR");
            Map(u => u.SSID_WIFI, "SSID_WIFI");
            Map(u => u.Senha_WIFI, "SENHA_WIFI");
            Map(u => u.VLAN, "VLAN");
            Map(u => u.Serial, "SERIAL");
            Map(u => u.MAC, "MAC");
            Map(u => u.Tipo_Autenticacao_Login, "TIPO_AUTENTIFICACAO_LOGIN").CustomType<Tipo_Autenticacao_Login>();
            Map(u => u.Id_Caixa_FTTH, "ID_CAIXA_FTTH");
            Map(u => u.Id_Porta_FTTH, "ID_PORTA_FTTH");
            Map(u => u.Agent_Circuit_Id, "AGENT_CIRCUIT_ID");
            
            Map(u => u.CodigoAuxiliar, "CODIGO_AUXILIAR").Length(50);
            Map(u => u.Observacoes, "OBSERVACOES").Length(5000);
            Map(u => u.Status_Ultima_Autenticacao_Radius, "STATUS_ULTIMA_AUTH_RADIUS").CustomType<Status_Autenticacao_Radius>();
            Map(p => p.DataHora_Ultima_Autenticacao_Radius, "DATAHORA_ULTIMA_AUTH_RADIUS");
            Map(p => p.DataHora_Ultima_Execucao_OLTService, "ULTIMA_EXEC_OLT_SERVICE");
            References(u => u.Atributo_IPOE, "ID_ATRUBUTO_IPOE").ForeignKey("FK_LOGIN_IPOE"); 
            References(u => u.POP, "ID_POP").ForeignKey("FK_LOGIN_POP");
            References(u => u.Concentradora, "ID_CONCENTRADORA").ForeignKey("FK_LOGIN_CONCENTRADORA");           
            References(u => u.Cliente, "ID_CLIENTE").ForeignKey("FK_LOGIN_CLIENTE");
            References(u => u.Cliente_PlanoProduto, "ID_PLANO_PRODUTO").ForeignKey("FK_LOGIN_PLAN_PROD");            
            References(u => u.Pool_Ip_Endereco, "ID_POOL_IP_ENDERECO").ForeignKey("FK_LOGIN_POOL_IP_END");
        }
    }
}

