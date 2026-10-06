using Aniel.Connect.Core;
using System;


namespace Aniel.Connect.Models
{
    public enum LogGenericoEnum
    {
        Atendimento = 1,
        Franquia = 2,
        RotaTelefonia = 3,
        Did = 4,
        Uf = 5,
        Cidade = 6,
        Bairro = 7,
        Endereco_Logradouro = 8,
        Condominio_Predio = 9,
        Vendas = 10
    }


    public class LogGenericoParamListModel
    {
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
    }
    public class LogGenericoModel
    {
        public virtual long Id { get; set; }
        public virtual long IdLog { get; set; }
        public virtual String Nome_usuario { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String DataHoraCriacao { get; set; }
        public virtual String TipoOperacao { get; set; }

        /// <summary>
        /// Identificador do Objeto
        /// </summary>
        public virtual String Chave { get; set; }

        /// <summary>
        /// Texto que representa o Identificador do Objeto 
        /// </summary>
        public virtual String DescricaoResourse { get; set; }

        public virtual String Campo { get; set; }
        public virtual String Campo_ValorAntes { get; set; }
        public virtual String Campo_ValorDepois { get; set; }

        public virtual int Id_Venda { get; set; }
        public virtual String IdClientePlanoProduto { get; set; }
        public virtual String IdFormaPagamentoCliente { get; set; }
        public virtual String Motivo { get; set; }
    }

    public class LogGenerico_DataTableRequest : DataTableRequest
    {
        public virtual LogGenericoEnum Modelo { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
        public virtual String Usuario { get; set; }
       

    }
}