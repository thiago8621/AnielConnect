using System;

namespace Aniel.Connect.Repository.Model
{
    public class DescontoListaModel
    {
        public Int64 Id { get; set; }
               
        public Int64 Id_Desconto { get; set; }
        public String Descricao { get; set; }
        public String Tipo_Desconto { get; set; }
        public String Formato { get; set; }
        public String CodigoAuxiliar { get; set; }
               
        public String Desconto_Captacao { get; set; }
        public String Percentual_Desconto { get; set; }
        public String Valor_Desconto { get; set; }
        public String Repeticoes_Desconto { get; set; }
               
        public String Status { get; set; }
        public String Detalhes { get; set; }
               
        public String Desconto_Tempo_Indeterminado { get; set; }
               
        public Boolean Excluir { get; set; }
        public String Numero_ContratoVinculado { get; set; }
        public String Usuario { get; set; }
        //public virtual String Plano { get; set; }
        //public virtual String Produto { get; set; }
        //public virtual String Tem_Desconto { get; set; }
        //public virtual String Status { get; set; }

    }
}
