using Sinapse.Model.nhibernate;
using System;


namespace Aniel.T5.Model.nhibernate
{
    public class Tb_Pt_Dia_Funcionario : BaseModel        
    {
        
        
        public virtual string NumFre { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual Tb_Pt_Classificacao_Escala Evento { get; set; }
        public virtual int Ciclos { get; set; }
        
        public virtual bool Alterado { get; set; }
        public virtual double? Horas_Abonadas { get; set; }
        
        public virtual DateTime? Hora_Extra_50 { get; set; }
        public virtual DateTime? Hora_Extra_100 { get; set; }
        public virtual DateTime? Adicional_Noturno { get; set; }
        
        public virtual bool Periodo_Fechado { get; set; }
        
        public virtual string Empresa { get; set; }
        public virtual string Filial { get; set; }
        
        public virtual string Departamento { get; set; }
        
        public virtual string CentroCusto { get; set; }
        
        public virtual string Funcao { get; set; }
        public virtual string ValorHH { get; set; }
        
        
    }
}
