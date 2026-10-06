using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class SerasaConsultaModels
    {
        public virtual string cnpj { get; set; }
        public virtual string cpf { get; set; }
        public virtual string consulta { get; set; }
    }

    public class BlocosDadosConsulta
    {
        public virtual string BlocoN200 { get; set; }    
        public virtual string BlocoN210 { get; set; }    
        public virtual string BlocoN220 { get; set; }    
        public virtual string BlocoN230 { get; set; }    
        public virtual string BlocoN240 { get; set; }    
        public virtual string BlocoN250 { get; set; }    
        public virtual string BlocoN270 { get; set; }    
        public virtual string BlocoN410 { get; set; }   
        public virtual string BlocoN420 { get; set; }   
        public virtual string BlocoN430 { get; set; }   
        public virtual string BlocoN440 { get; set; }   
        public virtual string BlocoN460 { get; set; }   
        public virtual string BlocoN500 { get; set; }   
        public virtual string BlocoN690 { get; set; }   
    }
}