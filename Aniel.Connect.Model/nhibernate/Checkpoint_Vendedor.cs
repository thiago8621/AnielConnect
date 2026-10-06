using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Checkpoint_Vendedor : BaseModel
    {
        public virtual Funcionario Vendedor { get; set; }
        
        public virtual DateTime? Data_Hora { get; set; }
        public virtual String Endereco { get; set; }
        public virtual Double? Longitude{ get; set; }
        public virtual Double? Latitude { get; set; }

        public virtual Tipo_Checkpoint_Vendedor Tipo { get; set; }
        
        public virtual String Observacao { get; set; }
        public virtual DateTime? Atualizado_Em { get; set; }
        public virtual Double? Precisao { get; set; }
        public virtual Double? Bateria { get; set; }
        public virtual String Status_Gps { get; set; }
        public virtual String Dispositivo { get; set; }
        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }

    }


    #region Mobile
    public class Checkpoint_Vendedor_Mobile {
        public virtual Int64 id { get; set; }
        public virtual Int64 id_vendedor {get;set;}
        public virtual Int64? id_tipo { get; set; }
        public virtual String usuario_criacao { get; set; }
        public virtual DateTime? data_hora { get; set; }
        public virtual Double? longitude { get; set; }
        public virtual Double? latitude { get; set; }
        public virtual String endereco { get; set; }
        public virtual String observacao { get; set; }
        public virtual Double? precisao { get; set; }
        public virtual Int64? bateria { get; set; }
        public virtual String status_gps { get; set; }
        public virtual String dispositivo { get; set; }
        public virtual String versao_android { get; set; }
       public virtual String versao_app { get; set; }
    }

    public class CheckinVendedor
    {
        public Int64 IdVendedor { get; set; }

        public String NumFree { get; set; }
        public DateTime DataHora { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public virtual String CEP { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Numero { get; set; }
        public String Observacao { get; set; }
        public Double? Precisao { get; set; }
        public Double? Bateria { get; set; }
        public String StatusGps { get; set; }
        public String Dispositivo { get; set; }
        public String VersaoAndroid { get; set; }
        public String VersaoApp { get; set; }
    }
    #endregion
}
