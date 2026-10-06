using Aniel.Connect.Model.enumerado;
using System;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary> Lista de Campos referentes a Funcionario </summary>
    public class Atendimento_Aniel 
    {
        public virtual String Cpf { get; set; }
        public virtual String Codigo_Categoria { get; set; }
        public virtual Int64 TipoAtendimento { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Assunto { get; set; }
        public virtual Prioridade_Atendimento Prioridade { get; set; }
    }

    /// <summary>request de Atendimento </summary>
    public class Atendimento_AnielApiRequest : ApiRequest {
        
        // Cliente
        /// <summary> Cpf do Usuario que recebeu o atendimento, </summary>
        public virtual String Cpf { get; set; }
        /// <summary>Código da Categoria do Cliente a Receber Atendimento, </summary>
        public virtual String Codigo_Categoria { get; set; }
        /// <summary> Tipo de Atendimento, </summary>
        public virtual Int64 TipoAtendimento { get; set; } 
        /// <summary> Plano do Usuário que Recebeu o Atendimento, </summary>
        public virtual String Plano { get; set; }
        /// <summary> Código de Origem do Atendimento, </summary>
        public virtual Int64? Codigo_origem { get; set; }
        /// <summary> Código da Venda do Usuário que Recebeu o Aetndimento, </summary>
        public virtual Int64? Venda { get; set; }
       /// <summary> Assunto descrito pelo Usuário, </summary>
        public virtual String Assunto { get; set; }
        /// <summary> Prioridade do Porblema Relatado no Atendimento </summary>
        public virtual Prioridade_Atendimento Prioridade { get; set; }


    }

}