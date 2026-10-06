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
    public class Criterio_RoteirizacaoListaModel
    {

        public Int64 Id { get; set; }

        public Int16 Ordem { get; set; }

        public virtual Int16 Criterio_Val { get; set; }

        public virtual String Criterio { get; set; }
        
    }




    public class Criterio_RoteirizacaoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        private List<Criterio_RoteirizacaoListaModel> CriteriosLista;


        public virtual List<Criterio_RoteirizacaoListaModel> Criterios

        {
            get
            {
                if (CriteriosLista == null)
                    CriteriosLista = new List<Criterio_RoteirizacaoListaModel>();
                return CriteriosLista;
            }
            set
            {
                CriteriosLista = value;
            }
        }

    }

    public class Criterio_Roteirizacao_DataTableRequest : DataTableRequest
    {

    }
}