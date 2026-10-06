using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Fatura
    {
        //public virtual Int64 Cod_Funcao { get; set; }
        //public virtual String Nome { get; set; }
        //public virtual String Plano { get; set; }
        //public virtual String Classe { get; set; }
        //public virtual String Coluna { get; set; }
        //public virtual String Revisao { get; set; }
        //public virtual Int64? HXH { get; set; }
        //public virtual Double? Salario { get; set; }
        //public virtual String Num_Cbo { get; set; }
        //public virtual String Cod_Sindicato { get; set; }
        //public virtual String Integracao { get; set; }
        //public virtual Double? Adic_Insalubre { get; set; }
        //public virtual Double? Adic_Periculosidade { get; set; }
        //public virtual Double? Adic_Transferencia { get; set; }
        //public virtual String Requisito { get; set; }
        //public virtual String Objetivo { get; set; }
        //public virtual String Desc_At_Ppp { get; set; }
        //public virtual String Responsabilidade { get; set; }
        //public virtual Double? Piso_Minimo { get; set; }
        //public virtual String Cod_Funcao_Aux { get; set; }
        //public virtual String Funcao_Tributavel { get; set; }
        //public virtual String Cod_Perfil_Equipe { get; set; }
        //public virtual String Permite_Gerar_Equipe { get; set; }
        //public virtual String Grupo_Funcao { get; set; }
        //public virtual Int32? Status { get; set; }
        public virtual String Cpf_Cliente { get; set; }
        public virtual String Data_Vencimento { get; set; }
        public virtual String Id_fatura { get; set; }
        public virtual String URL { get; set; }
    }
    #region Tb_Faturamento_Protocolo
    //public class Faturamento_Protocolo : BaseModel
    //{
    //    public virtual String Protocolo { get; set; }
    //    public virtual Fechamento Fechamento { get; set; }
    //    public virtual DateTime? Data_Fechamento { get; set; }
    //    public virtual Status_Faturamento Status { get; set; }
    //    public virtual DateTime? Data_Inicial { get; set; }
    //    public virtual DateTime? Data_Final { get; set; }
    //    public virtual DateTime? Data_Vencimento { get; set; }
    //    public virtual Tipo_Faturamento Parcial { get; set; }
    //    public virtual Boolean Tem_Valor_Avulso { get; set; }
    //    public virtual Double? Valor_Avulso { get; set; }
    //}
    #endregion

    #region Forma_Pagamento
    //public class Forma_Pagamento : BaseModel
    //{
    //    public virtual String Descricao { get; set; }
    //    public virtual Tipo_Forma_Pagamento Tipo_Forma_Pagamento { get; set; }
    //    public virtual Double? Percentual_Desconto { get; set; }
    //    public virtual Double? Valor { get; set; }

    //    public virtual Banco Banco { get; set; }
    //    public virtual Conta_Bancaria Conta_Bancaria { get; set; }
    //    public virtual Caixa_Interno Caixa_Interno { get; set; }
    //    public virtual Int32? Dias_Compensacao { get; set; }
    //    public virtual Double? Juros { get; set; }
    //    public virtual Double? Multa { get; set; }
    //    public virtual Boolean Gerar_Boleto_Automatico { get; set; }
    //    public virtual Boolean Existe_Grupo_Rateio { get; set; }
    //    public virtual Metodo_Rateio? Metodo_Rateio { get; set; }
    //    public virtual GateWayPagamento GateWayPagamento { get; set; }
    //    public virtual Status_Default? Status { get; set; }
    //}
    #endregion

    #region Faturamento_Forma_Pagamento
    /*public class Faturamento_Forma_Pagamento : BaseModel
    {
        public virtual Faturamento_Protocolo Faturamento_Protocolo { get; set; }
        public virtual Forma_Pagamento Forma_pagto { get; set; }
        ////public virtual Forma_Pagamento_Cliente Forma_Pagto_Cliente { get; set; }
        public virtual Boolean? Automatico { get; set; }
    } */
    #endregion
}
