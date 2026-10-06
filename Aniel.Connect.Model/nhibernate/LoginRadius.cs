using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class LoginRadius : BaseModel
    {
        public virtual String CodigoAuxiliar { get; set; }
        public virtual String Login { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual String EnderecoIPV4 { get; set; }
        public virtual String EnderecoIPV6 { get; set; }
        public virtual String SenhaPPPOE { get; set; }
        public virtual String UsuarioRoteador { get; set; }
        public virtual String SenhaRoteador { get; set; }
        public virtual String SSID_WIFI { get; set; }
        public virtual String Senha_WIFI { get; set; }
        public virtual String VLAN { get; set; }
        public virtual String Serial { get; set; }
        public virtual String MAC { get; set; }
        public virtual String Agent_Circuit_Id { get; set; }
        public virtual Atributo_IPOE Atributo_IPOE { get; set; }
        public virtual Tipo_Autenticacao_Login? Tipo_Autenticacao_Login { get; set; }
        public virtual POP POP { get; set; }
        public virtual Sinapse.Model.Concentradora Concentradora { get; set; }        
        public virtual Cliente Cliente { get; set; }
        public virtual Cliente_PlanoProduto Cliente_PlanoProduto { get; set; }        
        public virtual Int64? Id_Caixa_FTTH { get; set; }
        public virtual Int64? Id_Porta_FTTH { get; set; }
        public virtual Pool_Ip_Endereco Pool_Ip_Endereco { get; set; }
        public virtual String Observacoes {  get; set; }
        public virtual Status_Autenticacao_Radius? Status_Ultima_Autenticacao_Radius { get; set; }
        public virtual DateTime? DataHora_Ultima_Autenticacao_Radius { get; set; }
        public virtual DateTime? DataHora_Ultima_Execucao_OLTService { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual List<LogLoginRadius> GeraLog(LoginRadius pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogLoginRadius> logs = new List<LogLoginRadius>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<LoginRadius, LogLoginRadius>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {                
                Log_ComparaProperty<LoginRadius, LogLoginRadius>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS_LOGIN, pOld, this, p => p.Status);
                
                Log_ComparaProperty<LoginRadius, LogLoginRadius>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.LOGIN_DO_RADIUS, pOld, this, p => p.Login);
                Log_ComparaProperty<LoginRadius, LogLoginRadius>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.SENHA_PPPOE, pOld, this, p => p.SenhaPPPOE);
                Log_ComparaProperty<LoginRadius, LogLoginRadius>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.SSID_WIFI, pOld, this, p => p.SSID_WIFI);
                Log_ComparaProperty<LoginRadius, LogLoginRadius>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.SENHA_WIFI, pOld, this, p => p.Senha_WIFI);
                Log_ComparaProperty<LoginRadius, LogLoginRadius, Cliente>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CLIENTE, pOld, this, p => p.Cliente, y => $"{y.Id} - {y.NomeFantasia}");
                Log_ComparaProperty<LoginRadius, LogLoginRadius, Cliente_PlanoProduto>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PLANO_PRODUTO, pOld, this, p => p.Cliente_PlanoProduto, y => $"{y.Id} - {y.Plano.Descricao}");
                Log_ComparaProperty<LoginRadius, LogLoginRadius, Concentradora>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CONCENTRADORA, pOld, this, p => p.Concentradora, y => $"{y.Id} - {y.Nome}");
                Log_ComparaProperty<Cliente_PlanoProduto, LogLoginRadius, Pool_Ip>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.POOL_IP, pOld.Cliente_PlanoProduto, this.Cliente_PlanoProduto, p => p.Pool_Ip, y => $"{y.Id} - {y.Descricao}");                
                Log_ComparaProperty<LoginRadius, LogLoginRadius, Pool_Ip_Endereco>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.ENDERECO_POOL_IP, pOld, this, p => p.Pool_Ip_Endereco, y => $"{y.Ip}");

                //verificando o se houve alteracoa de porta
                Log_ComparaProperty<LoginRadius, LogLoginRadius>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PORTA_FTTH, pOld, this, p => p.Id_Porta_FTTH);

                //verificando o se o houve alteracao de caixa
                Log_ComparaProperty<LoginRadius, LogLoginRadius>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CAIXA_FTTH, pOld, this, p => p.Id_Caixa_FTTH);

            }


            return logs;
        }

        public virtual List<LogLoginRadius> GeraLogPool(Pool_Ip pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {

            List<LogLoginRadius> logs = new List<LogLoginRadius>();

            if (pTipoOperLog == TipoOperLog.Alteracao)
            {

                //verificando o se o pold é null
                if (pOld != null && this.Cliente_PlanoProduto?.Pool_Ip != null)
                {

                    Log_ComparaProperty<Pool_Ip, LogLoginRadius>(
                        logs,
                        pUsuario,
                        pNomeUsuario,
                        pTipoOperLog,
                        this.Id.ToString(),
                        RsCadastros.POOL_IP,
                        pOld,
                        this.Cliente_PlanoProduto.Pool_Ip,
                        p => p.Descricao
                    );

                }
            }
                return logs;
            
        }
    }
}
