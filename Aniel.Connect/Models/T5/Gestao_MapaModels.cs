using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    public class Gestao_MapaFilterModel
    {
        public virtual String Id { get; set; }

        public virtual String Certificado { get; set; }
        public virtual String Elemento_Rede { get; set; }
        public virtual String Tipo_Elemento { get; set; }
        public virtual String Caixa_FTTH { get; set; }
        public virtual String Situacao_Fibra { get; set; }
        public virtual String Cliente { get; set; }
        public virtual String Categoria_Cliente { get; set; }
        public virtual String Status_Conexao_Cliente { get; set; }
        public virtual String Status_Cliente { get; set; }
        public virtual String Plano_Produto { get; set; }
        public virtual String Tecnologia { get; set; }
        public virtual String Login { get; set; }
        public virtual String Potencia_ONU { get; set; }
    }
}
