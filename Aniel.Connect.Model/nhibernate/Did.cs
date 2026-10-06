using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Did : BaseModel
    {
        public virtual String Numero { get; set; }
        public virtual StatusDid StatusDid { get; set; }
        public virtual DateTime? DiaLiberado { get; set; }
        public virtual Double? Valor { get; set; }
        public virtual TipoDiscagem TipoDiscagem { get; set; }
        public virtual bool ChamadaCobrar { get; set; }
        public virtual Cidade Cidade { get; set; }
        public virtual RotaTelefonia RotaTelefonia { get; set; }
        public virtual String UF { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Int64? IdVenda { get; set; }
        public virtual String MotivoIndisponivel { get; set; }
        public virtual String CodigoAuxiliar { get; set; }
        public virtual bool Reservado_Ddr { get; set; }
        public virtual  CircuitoTelefonia Circuito { get; set; }
        
        public virtual List<Log_Did> GeraLog(Did pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<Log_Did> logs = new List<Log_Did>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ID, pOld, this, p => p.Id);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.STATUS_DID, pOld, this, p => p.StatusDid);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.DIA_LIBERADO, pOld, this, p => p.DiaLiberado);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.NUMERO, pOld, this, p => p.Numero);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.CIDADE, pOld, this, p => p.Cidade);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.CHAMADA_COBRAR, pOld, this, p => p.ChamadaCobrar);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ID_VENDA, pOld, this, p => p.IdVenda);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.MOTIVO, pOld, this, p => p.MotivoIndisponivel);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.PAIS, pOld, this, p => p.Pais);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ROTA_TELEFONIA, pOld, this, p => p.RotaTelefonia);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.TIPO_DISCAGEM, pOld, this, p => p.TipoDiscagem);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.TOKEN, pOld, this, p => p.Token);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.UF, pOld, this, p => p.UF);
                Log_ComparaProperty<Did, Log_Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.VALOR, pOld, this, p => p.Valor);

            }

            return logs;
        }
    }
}
