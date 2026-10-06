using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Newtonsoft.Json;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class InterfaceRadioListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Codigo_Auxiliar { get; set; }
        public virtual String Ponto_Acesso_Radio { get; set; }

    }

    public class InterfaceRadioCadastroModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Codigo_Auxiliar { get; set; }
        public Int64? Id_Ponto_Acesso_Radio { get; set; }
    }

    public class InterfaceRadio_DataTableRequest : DataTableRequest
    {
        public virtual String Id_Ponto { get; set; }
        public virtual String Id_Interface { get; set; }
    }


    public class InterfaceRadioTreeViewModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
    }
  


    }