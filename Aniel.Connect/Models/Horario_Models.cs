using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Horario_ListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }

        public virtual String Tipo { get; set; }

        public virtual String Status { get; set; }

        public virtual String Padrao { get; set; }

        public virtual String Opcao_Trabalha_Feriado { get; set; }

        public virtual String Hora_Inicial { get; set; }
        public virtual String Hora_Final { get; set; }
    }

    public class Horario_CadastroModel : IViewModel<Int64>
    
    {


        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        [MaxLength(4, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        public virtual string Codigo { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Descricao { get; set; }

        public virtual Boolean Padrao { get; set; }

        public virtual Tipo_Hora_Connect Tipo { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Opcao_Trabalha_Feriado { get; set; }

        public virtual String Inicio_Feriado { get; set; }
        public virtual String Termino_Feriado { get; set; }
        public virtual String Inicio_Intervalo_Feriado { get; set; }
        public virtual String Termino_Intervalo_Feriado { get; set; }
        public virtual String Qtd_HE_Feriado { get; set; }

        public virtual String Inicio_Domingo { get; set; }
        public virtual String Termino_Domingo { get; set; }
        public virtual String Inicio_Intervalo_Domingo { get; set; }
        public virtual String Termino_Intervalo_Domingo { get; set; }
        public virtual String Qtd_HE_Domingo { get; set; }

        public virtual String Inicio_Segunda { get; set; }
        public virtual String Termino_Segunda { get; set; }
        public virtual String Inicio_Intervalo_Segunda { get; set; }
        public virtual String Termino_Intervalo_Segunda { get; set; }
        public virtual String Qtd_HE_Segunda { get; set; }

        public virtual String Inicio_Terca { get; set; }
        public virtual String Termino_Terca { get; set; }
        public virtual String Inicio_Intervalo_Terca { get; set; }
        public virtual String Termino_Intervalo_Terca { get; set; }
        public virtual String Qtd_HE_Terca { get; set; }

        public virtual String Inicio_Quarta { get; set; }
        public virtual String Termino_Quarta { get; set; }
        public virtual String Inicio_Intervalo_Quarta { get; set; }
        public virtual String Termino_Intervalo_Quarta { get; set; }
        public virtual String Qtd_HE_Quarta { get; set; }

        public virtual String Inicio_Quinta { get; set; }
        public virtual String Termino_Quinta { get; set; }
        public virtual String Inicio_Intervalo_Quinta { get; set; }
        public virtual String Termino_Intervalo_Quinta { get; set; }
        public virtual String Qtd_HE_Quinta { get; set; }

        public virtual String Inicio_Sexta { get; set; }
        public virtual String Termino_Sexta { get; set; }
        public virtual String Inicio_Intervalo_Sexta { get; set; }
        public virtual String Termino_Intervalo_Sexta { get; set; }
        public virtual String Qtd_HE_Sexta { get; set; }

        public virtual String Inicio_Sabado { get; set; }
        public virtual String Termino_Sabado { get; set; }
        public virtual String Inicio_Intervalo_Sabado { get; set; }
        public virtual String Termino_Intervalo_Sabado { get; set; }
        public virtual String Qtd_HE_Sabado { get; set; }

    }

    public class Horario_DataTableRequest : DataTableRequest
    {
        public virtual String Codigo { get; set; }
       
        public virtual String Descricao { get; set; }

        public virtual String Tipo { get; set; }

        public virtual String Status { get; set; }

        public virtual Boolean Padrao { get; set; }
        public virtual string Motivo { get; set; }

    }

    public class HorarioComboResult : IObjComboResult<long>
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string JornadaDia { get; set; }
    }

    public class GetIsFeriadoModel
    {
        public virtual string data { get; set; }
    }
}