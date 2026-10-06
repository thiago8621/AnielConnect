using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{

    public class ClienteProspectListModel
    {
        #region Itens de Cliente_Prospect
        public virtual String Id_Cliente { get; set; }
        public virtual String Cpf_Cnpj { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Status { get; set; }
        public virtual String Data_Nascimento { get; set; }
        public virtual String Tel_Fixo { get; set; }
        public virtual String Tel_Celular { get; set; }
        public virtual String Tel_Recado { get; set; }
        public virtual String Nome_Recado { get; set; }
        public virtual String Email { get; set; }
        public virtual String CEP { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Tipo_Imovel { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String Ponto_Referencia { get; set; }
        public virtual String Inadimplente { get; set; } 
        #endregion

    }
    public class ProspectApiRequest : ApiRequest 
    { 
    
    }





}