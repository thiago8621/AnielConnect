using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{

    public class Condominio_Predio : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual String Cep { get; set; }
        public virtual Boolean Definir_Caixa_Condominio_Predio { get; set; }
        public virtual Boolean Cadastro_Faixa { get; set; }
        public virtual Endereco_Logradouro Endereco_Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual long? Id_Pais { get; set; }
        public virtual long? Id_Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Bloco { get; set; }
        public virtual String Apto_Numero { get; set; }
        public virtual Status_Default? Status { get; set; }
        private IList<Elemento_Rede_Enderecos> _Caixas;
        public virtual IList<Elemento_Rede_Enderecos> Caixas
        {
            get
            {
                if (_Caixas == null)
                    _Caixas = new List<Elemento_Rede_Enderecos>();
                return _Caixas;
            }
            set
            {
                _Caixas = value;
            }
        }        

        public virtual List<Log_Condominio_Predio> GeraLog(Condominio_Predio pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<Log_Condominio_Predio> logs = new List<Log_Condominio_Predio>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ID, pOld, this, p => p.Id);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Descricao);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.CEP, pOld, this, p => p.Cep);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ENDERECO_LOGRADOURO, pOld, this, p => p.Endereco_Logradouro);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.NUMERO, pOld, this, p => p.Numero);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.PAIS, pOld, this, p => p.Id_Pais);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.CIDADE, pOld, this, p => p.Id_Cidade);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.BAIRRO, pOld, this, p => p.Bairro);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.LOGRADOURO, pOld, this, p => p.Logradouro);
                Log_ComparaProperty<Condominio_Predio, Log_Condominio_Predio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.UF, pOld, this, p => p.Uf);
            }

            return logs;
        }

    }
}

