using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{ /*2- API para receber as notas fiscais geradas.*/
    //        Deverão ser enviados as informações de número da NF, 
    //        CNPJ do emissor, CPF ou CNPJ do receptor, valor e 
    //        link para baixar a NF.
    public class Nota_FiscalListaModel
    {
        //public virtual Int64 Id { get; set; }

        public virtual String Numero_Nota { get; set; }
        

        public virtual String CNPJ_Emissor { get; set; }

        public virtual String CPF_CNPJ_Receptor { get; set; }

        public virtual String Valor { get; set; }

        public virtual String Link_NF { get; set; }
        public virtual String Link_Nota_XML { get; set; }

    }
    public class NF_ApiRequest : ApiRequest
    {
        //public virtual String Categoria_Cliente { get; set; }
        //public virtual String Plano { get; set; }
        //public virtual String Tipo_Pessoa { get; set; }
        //public virtual String Forma_Pagamento { get; set; }
    }
}
