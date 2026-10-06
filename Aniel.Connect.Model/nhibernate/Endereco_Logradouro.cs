using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class Endereco_Logradouro : BaseModel
    {
        public virtual String Cep { get; set; }
        public virtual Tipo_Endereco_Logradouro? Tipo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Uf UF { get; set; }
        public virtual Cidade Cidade { get; set; }
        public virtual Bairro Bairro { get; set; }
        public virtual Tipo_Atendimento_Zona? Tipo_Zona { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual String Cod_Localidade { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual string CodAuxiliar { get; set; }

        public virtual List<Log_Endereco_Logradouro> GeraLog(Endereco_Logradouro pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<Log_Endereco_Logradouro> logs = new List<Log_Endereco_Logradouro>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Endereco_Logradouro, Log_Endereco_Logradouro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ID, pOld, this, p => p.Id);
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro,Pais>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.PAIS, pOld, this, p => p.Pais, p => $"{p.Id} - {p.Nome}");
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro,Bairro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.BAIRRO, pOld, this, p => p.Bairro, p => $"{p.Id} - {p.Descricao}");
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro,Cidade>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.CIDADE, pOld, this, p => p.Cidade, p => $"{p.Id} - {p.Descricao}");
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.CEP, pOld, this, p => p.Cep);
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Descricao);
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro,Uf>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.UF, pOld, this, p => p.UF, p => $"{p.Id} - {p.Sigla}");
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<Endereco_Logradouro, Log_Endereco_Logradouro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.TIPO, pOld, this, p => p.Tipo);
            }

            return logs;



        }
    }
}
