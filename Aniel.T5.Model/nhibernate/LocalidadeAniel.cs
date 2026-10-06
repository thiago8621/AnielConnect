using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class LocalidadeAniel : BaseModel
    {
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String LocalidadeStr { get; set; }

        public virtual Int32 Cod_Localidade { get; set; }

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String CodCt_Infra { get; set; }
        public virtual String Projeto_Infra { get; set; }
        public virtual String Cod_Cliente { get; set; }
        public virtual String PontoRoteamento { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual double? Latitude { get; set; }
        public virtual double? Longitude { get; set; }
        public virtual bool Padrao { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            LocalidadeAniel other = obj as LocalidadeAniel;
            if (other == null)
                return false;
            if (Cod_Escritorio == other.Cod_Escritorio && Cod_Area == other.Cod_Area && Cod_Localidade == other.Cod_Localidade)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual List<LogLocalidadeAniel> GeraLog(LocalidadeAniel pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogLocalidadeAniel> logs = new List<LogLocalidadeAniel>();

            if (pTipoOperLog == TipoOperLog.Importacao)
            {
                Log_ComparaProperty<LocalidadeAniel, LogLocalidadeAniel>(logs, pUsuario, pNomeUsuario, TipoOperLog.Importacao, this.Id.ToString(), RsCadastros.IMPORTAR_LOCALIDADE, pOld, this, p => p.LocalidadeStr);
            }
            else if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<LocalidadeAniel, LogLocalidadeAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<LocalidadeAniel, LogLocalidadeAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<LocalidadeAniel, LogLocalidadeAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO_LOCALIDADE, pOld, this, p => p.LocalidadeStr);
                Log_ComparaProperty<LocalidadeAniel, LogLocalidadeAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO_AREA, pOld, this, p => p.Cod_Area);
                Log_ComparaProperty<LocalidadeAniel, LogLocalidadeAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO_GRUPO_AREA, pOld, this, p => p.Cod_Escritorio);
                Log_ComparaProperty<LocalidadeAniel, LogLocalidadeAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PROJETO, pOld, this, p => p.Projeto);
                Log_ComparaProperty<LocalidadeAniel, LogLocalidadeAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CONTRATO, pOld, this, p => p.CodCt);
            }

            return logs;

        }

    }
}
