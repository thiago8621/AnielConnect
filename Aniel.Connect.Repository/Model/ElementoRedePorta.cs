using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class Elemento_Rede_PortaCadastroModel
    {
        //public virtual Elemento_Rede Caixa { get; set; }
        public virtual Int64 Id_Caixa { get; set; }
        public virtual Int64 Id { get; set; }
        public virtual Int32 Porta { get; set; }
        public virtual Situacao_Porta Status { get; set; }
    }
    public class Elemento_ReservaCadastroModel
    {
        public virtual Int64 Cliente_PlanoProduto_Novo { get; set; }
        public virtual Int64 Cliente_PlanoProduto_Antigo { get; set; }
        public virtual String Login { get; set; }
    }
    public class ReservaRequest
    {
        public long? Caixa { get; set; }
        public long? ClientePlanoProduto { get; set; }
    }
    public class Elemento_Reserva
    {
        public virtual Int64? Id_Caixa { get; set; }        
        public virtual Int64? Porta { get; set; }
        public virtual Int64? Id_Porta { get; set; }
        public virtual Int64? Id_Login { get; set; }
        public List<long> Ids_Porta { get; set; }
        public virtual Int64 Cod_Cliente { get; set; }
        public virtual Int64 Cliente_PlanoProduto { get; set; }
        public virtual String Login { get; set; }
        public virtual Boolean Reset { get; set; }
       
    }
    public class CaixaPortaReservada
    {
        public long IdCaixa { get; set; }
        public long Id { get; set; }
        public string Caixa { get; set; }
        public string Porta { get; set; }
    }
    public class CaixaPortaReservadaRequest 
    {
        public string ClientePlanoProduto { get; set; } 
    }
}
