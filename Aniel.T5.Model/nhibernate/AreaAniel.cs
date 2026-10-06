using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;

namespace Aniel.T5.Model.nhibernate
{
    public class AreaAniel : BaseModel
    {
        public virtual String Cod_Escritorio { get; set; }
        public virtual String UF { get; set; }
        public virtual double? Latitude { get; set; }
        public virtual double? Longitude { get; set; }
        public virtual int? Distancia { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Cod_Cliente { get; set; }
        public virtual Status_Default Status { get; set; }
        /// <summary>
        ///     Propriedade apenas para leitura em consultas do Hibernate
        /// </summary>
        public virtual EscritorioAniel Grupo_Area { get; set; }
        public virtual String Observacao { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            AreaAniel other = obj as AreaAniel;
            if (other == null)
                return false;
            if (Cod_Escritorio == other.Cod_Escritorio && Cod_Area == other.Cod_Area)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Cod_Escritorio + "|" + Cod_Area).GetHashCode();
        }

        public virtual List<LogAreaAniel> GeraLog(AreaAniel pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogAreaAniel> logs = new List<LogAreaAniel>();

            if (pTipoOperLog == TipoOperLog.Importacao)
            {
                Log_ComparaProperty<AreaAniel, LogAreaAniel>(logs, pUsuario, pNomeUsuario, TipoOperLog.Importacao, this.Id.ToString(), RsCadastros.IMPORTAR_AREA, pOld, this, p => p.Observacao);
            }
            else if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<AreaAniel, LogAreaAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<AreaAniel, LogAreaAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<AreaAniel, LogAreaAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO_AREA, pOld, this, p => p.Descricao);
                Log_ComparaProperty<AreaAniel, LogAreaAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.GRUPO_AREA, pOld, this, p => p.Cod_Escritorio);
            }

            return logs;

        }
    }
}
